from __future__ import annotations

import argparse
from pathlib import Path

from markdown_utils import (
    MarkdownError,
    candidate_headings,
    create_backup,
    ensure_single_content,
    exact_heading_matches,
    heading_display,
    parse_headings,
    read_utf8,
    section_boundaries,
    split_lines,
    validate_level,
    write_utf8,
)


def _normalize_block(text: str) -> str:
    return text.strip("\n\r")


def _make_section(level: int, heading: str, body: str) -> str:
    title = f"{'#' * level} {heading.strip()}\n"
    block = _normalize_block(body)
    if block:
        return f"{title}\n{block}\n"
    return title


def _check_duplicates(headings, heading: str, allow_duplicate: bool) -> None:
    if allow_duplicate:
        return
    if exact_heading_matches(headings, heading):
        raise MarkdownError("Duplicate heading exists. Use --allow-duplicate to override.")


def _title_block_end(lines: list[str]) -> int:
    i = 0
    while i < len(lines) and not lines[i].strip():
        i += 1
    if i >= len(lines):
        raise MarkdownError("Cannot use --after-title on an empty file.")
    first_non_blank = lines[i].rstrip("\n\r")
    if not first_non_blank.startswith("# "):
        raise MarkdownError("--after-title requires first non-blank line to be a level-1 heading ('# ').")
    j = i + 1
    heading_re = __import__("re").compile(r"^#{1,6}\s+")
    while j < len(lines):
        if heading_re.match(lines[j].rstrip("\n\r")):
            break
        j += 1
    return j


def cmd_add_start(args: argparse.Namespace) -> None:
    validate_level(args.level)
    path = Path(args.file)
    original = read_utf8(path)
    lines = split_lines(original)
    headings = parse_headings(lines)
    _check_duplicates(headings, args.heading, args.allow_duplicate)
    body = ensure_single_content(args.content, args.content_file)
    new_section = _make_section(args.level, args.heading, body)

    if args.after_title:
        idx = _title_block_end(lines)
        prefix = "".join(lines[:idx])
        suffix = "".join(lines[idx:])
        new_content = prefix.rstrip("\n") + "\n\n" + new_section.rstrip("\n") + "\n\n" + suffix.lstrip("\n")
    else:
        new_content = new_section.rstrip("\n") + "\n\n" + original.lstrip("\n")

    if args.dry_run:
        print(new_content)
        return
    backup = create_backup(path)
    write_utf8(path, new_content)
    print(f"Added section at start: '{args.heading}' (backup: {backup})")


def cmd_add_end(args: argparse.Namespace) -> None:
    validate_level(args.level)
    path = Path(args.file)
    original = read_utf8(path)
    headings = parse_headings(split_lines(original))
    _check_duplicates(headings, args.heading, args.allow_duplicate)
    body = ensure_single_content(args.content, args.content_file)
    new_section = _make_section(args.level, args.heading, body)
    new_content = original.rstrip("\n") + "\n\n" + new_section
    if args.dry_run:
        print(new_content)
        return
    backup = create_backup(path)
    write_utf8(path, new_content)
    print(f"Added section at end: '{args.heading}' (backup: {backup})")


def _resolve_single_heading(args: argparse.Namespace, headings):
    matches = exact_heading_matches(headings, args.heading)
    if not matches:
        print("No exact heading match found. Candidate headings:")
        for cand in candidate_headings(headings, args.heading, args.candidate_limit):
            print(heading_display(cand))
        raise MarkdownError("No exact match for --heading.")
    if len(matches) > 1:
        print("Multiple exact heading matches found:")
        for m in matches:
            print(heading_display(m, include_line_number=True))
        raise MarkdownError("Multiple exact matches for --heading.")
    return matches[0]


def cmd_append_under(args: argparse.Namespace) -> None:
    path = Path(args.file)
    original = read_utf8(path)
    lines = split_lines(original)
    headings = parse_headings(lines)
    target = _resolve_single_heading(args, headings)
    body = ensure_single_content(args.content, args.content_file).strip("\n")
    start, end = section_boundaries(lines, headings, target)
    section = "".join(lines[start:end]).rstrip("\n")
    section = f"{section}\n{body}\n"
    new_content = "".join(lines[:start]) + section + "".join(lines[end:])
    if args.dry_run:
        print(section)
        return
    backup = create_backup(path)
    write_utf8(path, new_content)
    print(f"Appended under heading: '{args.heading}' (backup: {backup})")


def cmd_replace_section(args: argparse.Namespace) -> None:
    path = Path(args.file)
    original = read_utf8(path)
    lines = split_lines(original)
    headings = parse_headings(lines)
    target = _resolve_single_heading(args, headings)
    new_body = ensure_single_content(args.content, args.content_file).strip("\n")
    start, end = section_boundaries(lines, headings, target)
    heading_line = lines[start].rstrip("\n")
    new_section = heading_line + "\n"
    if new_body:
        new_section += "\n" + new_body + "\n"
    new_content = "".join(lines[:start]) + new_section + "".join(lines[end:])
    if args.dry_run:
        print(new_section)
        return
    backup = create_backup(path)
    write_utf8(path, new_content)
    print(f"Replaced section body under heading: '{args.heading}' (backup: {backup})")


def build_parser() -> argparse.ArgumentParser:
    p = argparse.ArgumentParser(description="Controlled Markdown write operations")
    sub = p.add_subparsers(dest="cmd", required=True)

    p1 = sub.add_parser("add-start", help="Add section at file start")
    p1.add_argument("file")
    p1.add_argument("--heading", required=True)
    p1.add_argument("--level", type=int, default=2)
    p1.add_argument("--content-file")
    p1.add_argument("--content")
    p1.add_argument("--after-title", action="store_true")
    p1.add_argument("--allow-duplicate", action="store_true")
    p1.add_argument("--dry-run", action="store_true")
    p1.set_defaults(func=cmd_add_start)

    p2 = sub.add_parser("add-end", help="Add section at file end")
    p2.add_argument("file")
    p2.add_argument("--heading", required=True)
    p2.add_argument("--level", type=int, default=2)
    p2.add_argument("--content-file")
    p2.add_argument("--content")
    p2.add_argument("--allow-duplicate", action="store_true")
    p2.add_argument("--dry-run", action="store_true")
    p2.set_defaults(func=cmd_add_end)

    p3 = sub.add_parser("append-under", help="Append content under exact heading")
    p3.add_argument("file")
    p3.add_argument("--heading", required=True)
    p3.add_argument("--content-file")
    p3.add_argument("--content")
    p3.add_argument("--candidate-limit", type=int, default=10)
    p3.add_argument("--dry-run", action="store_true")
    p3.set_defaults(func=cmd_append_under)

    p4 = sub.add_parser("replace-section", help="Replace section body under exact heading")
    p4.add_argument("file")
    p4.add_argument("--heading", required=True)
    p4.add_argument("--content-file")
    p4.add_argument("--content")
    p4.add_argument("--candidate-limit", type=int, default=10)
    p4.add_argument("--dry-run", action="store_true")
    p4.set_defaults(func=cmd_replace_section)
    return p


def main() -> int:
    parser = build_parser()
    args = parser.parse_args()
    try:
        args.func(args)
    except MarkdownError as exc:
        parser.error(str(exc))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
