"""Shared Markdown utilities for markdown-handler scripts."""

from __future__ import annotations

from dataclasses import dataclass
from datetime import datetime
from pathlib import Path
import re
import shutil
from typing import Iterable, Sequence

HEADING_RE = re.compile(r"^(#{1,6})\s+(.*\S)?\s*$")


@dataclass(frozen=True)
class Heading:
    line_no: int
    level: int
    raw_line: str
    text: str


class MarkdownError(Exception):
    """Known markdown tool error."""


def read_utf8(path: Path) -> str:
    if not path.exists():
        raise MarkdownError(f"File not found: {path}")
    return path.read_text(encoding="utf-8")


def write_utf8(path: Path, content: str) -> None:
    path.write_text(content, encoding="utf-8")


def create_backup(path: Path) -> Path:
    timestamp = datetime.utcnow().strftime("%Y%m%d-%H%M%S")
    backup = path.with_suffix(path.suffix + f".bak.{timestamp}")
    shutil.copy2(path, backup)
    return backup


def split_lines(content: str) -> list[str]:
    return content.splitlines(keepends=True)


def parse_headings(lines: Sequence[str]) -> list[Heading]:
    headings: list[Heading] = []
    for idx, line in enumerate(lines, start=1):
        m = HEADING_RE.match(line.rstrip("\n\r"))
        if not m:
            continue
        hashes, text = m.group(1), (m.group(2) or "")
        headings.append(Heading(line_no=idx, level=len(hashes), raw_line=line, text=text.strip()))
    return headings


def exact_heading_matches(headings: Sequence[Heading], target: str) -> list[Heading]:
    wanted = target.strip().casefold()
    return [h for h in headings if h.text.casefold() == wanted]


def candidate_headings(
    headings: Sequence[Heading],
    target: str,
    limit: int = 10,
) -> list[Heading]:
    needle = target.strip().casefold()
    contains = [h for h in headings if needle and needle in h.text.casefold()]
    if contains:
        return contains[:limit]
    return list(headings[:limit])


def section_boundaries(lines: Sequence[str], headings: Sequence[Heading], heading: Heading) -> tuple[int, int]:
    start_idx = heading.line_no - 1
    end_idx = len(lines)
    for h in headings:
        if h.line_no <= heading.line_no:
            continue
        if h.level <= heading.level:
            end_idx = h.line_no - 1
            break
    return start_idx, end_idx


def format_lines(lines: Sequence[str], start_line_no: int = 1, line_numbers: bool = False) -> str:
    out: list[str] = []
    for offset, line in enumerate(lines):
        text = line.rstrip("\n\r")
        if line_numbers:
            out.append(f"{start_line_no + offset}: {text}")
        else:
            out.append(text)
    return "\n".join(out)


def truncate_lines(lines: Sequence[str], max_lines: int | None) -> tuple[list[str], bool]:
    if max_lines is None or max_lines < 0:
        return list(lines), False
    if len(lines) <= max_lines:
        return list(lines), False
    return list(lines[:max_lines]), True


def ensure_single_content(content: str | None, content_file: str | None) -> str:
    if bool(content) == bool(content_file):
        raise MarkdownError("Provide exactly one of --content or --content-file.")
    if content_file:
        return read_utf8(Path(content_file))
    return content or ""


def heading_display(heading: Heading, include_line_number: bool = False) -> str:
    prefix = "#" * heading.level
    if include_line_number:
        return f"{heading.line_no}: {prefix} {heading.text}".rstrip()
    return f"{prefix} {heading.text}".rstrip()


def validate_level(level: int) -> None:
    if level < 1 or level > 6:
        raise MarkdownError("--level must be between 1 and 6.")


def join_sections(sections: Iterable[str]) -> str:
    return "\n\n-----\n\n".join(s.strip("\n") for s in sections)
