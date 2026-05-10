---
name: markdown-handler
description: Use when reading or editing Markdown files, AGENTS.md files, project history logs, TODO files, decision records, changelogs, or section-based documentation without loading or rewriting the entire file.
---

# markdown-handler

Use this skill whenever targeted Markdown reading or controlled section editing is safer or more efficient than opening or rewriting full files.

## Rules

1. Use `markdown_read.py` for Markdown inspection only.
2. Use `markdown_write.py` only when modifying Markdown files.
3. Inspect headings before opening structured Markdown files.
4. Use targeted section extraction before reading full files when practical.
5. Keep script output bounded using limits (`--limit`, `--max-lines`, etc.).
6. Use controlled write commands instead of manually rewriting full Markdown files.
7. Use dry-run (`--dry-run`) before destructive edits.
8. Verify edited sections after writing.
9. Treat source code and Git history as more authoritative than Markdown history files.
10. Use this skill for any Markdown file when targeted reading or controlled editing is safer or more efficient than opening or rewriting the full file.
11. For very small files, direct reading is acceptable if it is simpler.
12. Use exact heading names for section-based commands.
13. If the exact heading is unknown, list headings first instead of guessing.

## Scripts

- `scripts/markdown_read.py`
  - Targeted read operations:
    - `list-headings`
    - `print-section`
    - `print-sections`
    - `search`
    - `print-date-section`

- `scripts/markdown_write.py`
  - Controlled write operations:
    - `add-start`
    - `add-end`
    - `append-under`
    - `replace-section`

- `scripts/markdown_utils.py`
  - Shared helpers for UTF-8 I/O, backup creation, heading parsing, exact case-insensitive heading matching, candidate generation, section boundary detection, and bounded output formatting.

## Suggested workflow

1. Run `list-headings` to understand structure.
2. Use `print-section`, `print-sections`, `print-date-section`, or `search` with limits.
3. For edits, run the intended write command with `--dry-run` first.
4. Apply the same command without `--dry-run` once verified.
5. Re-check updated sections with read commands.
