from __future__ import annotations

import argparse
from pathlib import Path
from typing import Sequence

from markdown_utils import (
    MarkdownError,
    candidate_headings,
    exact_heading_matches,
    format_lines,
    heading_display,
    parse_headings,
    read_utf8,
    section_boundaries,
    split_lines,
    truncate_lines,
)


def _candidate_limit(value: int | None) -> int:
    return 10 if value is None else value


def cmd_list_headings(args: argparse.Namespace) -> None:
    lines = split_lines(read_utf8(Path(args.file)))
    headings = parse_headings(lines)
    if args.level is not None and args.max_level is not None:
        raise MarkdownError("Do not use --level and --max-level together.")
    if args.level is not None:
        headings = [h for h in headings if h.level == args.level]
    elif args.max_level is not None:
        headings = [h for h in headings if h.level <= args.max_level]
    if args.limit is not None:
        headings = headings[: args.limit]
    for h in headings:
        print(heading_display(h, include_line_number=args.line_numbers))


def _print_single_section(lines: Sequence[str], headings, heading, max_lines: int | None, line_numbers: bool) -> None:
    start, end = section_boundaries(lines, headings, heading)
    section_lines = lines[start:end]
    out_lines, truncated = truncate_lines(section_lines, max_lines)
    print(format_lines(out_lines, start_line_no=start + 1, line_numbers=line_numbers))
    if truncated:
        print("[Output truncated]")


def cmd_print_section(args: argparse.Namespace) -> None:
    lines = split_lines(read_utf8(Path(args.file)))
    headings = parse_headings(lines)
    matches = exact_heading_matches(headings, args.heading)
    if not matches:
        print("No exact heading match found. Candidate headings:")
        for cand in candidate_headings(headings, args.heading, _candidate_limit(args.candidate_limit)):
            print(heading_display(cand, include_line_number=args.line_numbers))
        raise MarkdownError("No exact match for --heading.")
    if len(matches) > 1:
        print("Multiple exact heading matches found:")
        for match in matches:
            print(heading_display(match, include_line_number=True))
        raise MarkdownError("Multiple exact matches for --heading.")
    _print_single_section(lines, headings, matches[0], args.max_lines, args.line_numbers)


def cmd_print_sections(args: argparse.Namespace) -> None:
    lines = split_lines(read_utf8(Path(args.file)))
    headings = [h for h in parse_headings(lines) if h.level == args.level]
    if args.limit is not None:
        headings = headings[: args.limit]
    blocks = []
    for h in headings:
        start, end = section_boundaries(lines, parse_headings(lines), h)
        section_lines = lines[start:end]
        out_lines, truncated = truncate_lines(section_lines, args.max_lines_per_section)
        block = format_lines(out_lines, start_line_no=start + 1, line_numbers=args.line_numbers)
        if truncated:
            block = f"{block}\n[Section truncated]"
        blocks.append(block)
    if blocks:
        print("\n\n-----\n\n".join(blocks))


def cmd_search(args: argparse.Namespace) -> None:
    text = read_utf8(Path(args.file))
    lines = split_lines(text)
    needle = args.query.casefold()
    matches = [i for i, line in enumerate(lines) if needle in line.casefold()]
    if not matches:
        raise MarkdownError("No matches found for --query.")

    ranges: list[tuple[int, int]] = []
    context = args.context
    for idx in matches:
        start = max(0, idx - context)
        end = min(len(lines), idx + context + 1)
        if ranges and start <= ranges[-1][1]:
            ranges[-1] = (ranges[-1][0], max(ranges[-1][1], end))
        else:
            ranges.append((start, end))
    if args.limit is not None:
        ranges = ranges[: args.limit]

    blocks = []
    for start, end in ranges:
        blocks.append(format_lines(lines[start:end], start_line_no=start + 1, line_numbers=args.line_numbers))
    print("\n\n-----\n\n".join(blocks))


def cmd_print_date_section(args: argparse.Namespace) -> None:
    lines = split_lines(read_utf8(Path(args.file)))
    headings = parse_headings(lines)
    matches = [h for h in headings if args.date in h.text]
    if not matches:
        raise MarkdownError(f"No heading found containing date: {args.date}")
    if len(matches) > 1:
        print("Multiple headings contain the date:")
        for match in matches:
            print(heading_display(match, include_line_number=True))
        raise MarkdownError("Multiple date matches found.")
    _print_single_section(lines, headings, matches[0], args.max_lines, args.line_numbers)


def build_parser() -> argparse.ArgumentParser:
    p = argparse.ArgumentParser(description="Targeted Markdown read operations")
    sub = p.add_subparsers(dest="cmd", required=True)

    p1 = sub.add_parser("list-headings", help="List headings only")
    p1.add_argument("file")
    p1.add_argument("--limit", type=int)
    p1.add_argument("--level", type=int)
    p1.add_argument("--max-level", type=int)
    p1.add_argument("--line-numbers", action="store_true")
    p1.set_defaults(func=cmd_list_headings)

    p2 = sub.add_parser("print-section", help="Print one section by exact heading")
    p2.add_argument("file")
    p2.add_argument("--heading", required=True)
    p2.add_argument("--max-lines", type=int)
    p2.add_argument("--line-numbers", action="store_true")
    p2.add_argument("--candidate-limit", type=int)
    p2.set_defaults(func=cmd_print_section)

    p3 = sub.add_parser("print-sections", help="Print sections by exact heading level")
    p3.add_argument("file")
    p3.add_argument("--level", type=int, required=True)
    p3.add_argument("--limit", type=int)
    p3.add_argument("--max-lines-per-section", type=int)
    p3.add_argument("--line-numbers", action="store_true")
    p3.set_defaults(func=cmd_print_sections)

    p4 = sub.add_parser("search", help="Search with bounded context")
    p4.add_argument("file")
    p4.add_argument("--query", required=True)
    p4.add_argument("--context", type=int, default=3)
    p4.add_argument("--limit", type=int)
    p4.add_argument("--line-numbers", action="store_true")
    p4.set_defaults(func=cmd_search)

    p5 = sub.add_parser("print-date-section", help="Print one date section")
    p5.add_argument("file")
    p5.add_argument("--date", required=True)
    p5.add_argument("--max-lines", type=int)
    p5.add_argument("--line-numbers", action="store_true")
    p5.set_defaults(func=cmd_print_date_section)
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
