# Changelog

## [Unreleased]

## [v0.2.0] - 2025-06-20

### 🎯 Highlights

- Introduced pluggable renderer system with support for:
  - ANSI-styled output (default)
  - Plain text
  - Compact one-line summaries
  - JSON (pretty and compact)

- Improved `tail` command usability and flexibility.

---

### 🚀 Features

- Added `ILogRenderer` interface for extensible log output.
- Added `RendererFactory` to route CLI options to appropriate format.
- Implemented new renderers:
  - `AnsiRenderer` (Spectre.Console-based)
  - `PlainTextRenderer`
  - `CompactRenderer`
  - `JsonRenderer`

---

### 🐛 Bug Fixes

- Fixed: `--take` was returning fewer logs due to unintended default log level filtering.
- Fixed: Paging prompt was overlapping log output; now uses Spectre formatting and spacing.
- Fixed: `--contains` did not match logs reliably — now searches message, exception, context, etc.

---

### 🧪 CLI Examples

```bash
dotnet-observe tail --take 1000
dotnet-observe tail --contains "Not Found"
dotnet-observe tail --json pretty
dotnet-observe tail --page-size 5
```

## [0.2.0] - 2025-06-20
### Added
- Enhanced `--json pretty` output with color-coded, aligned fields
- New Spectre dividers and spacing between log entries
- UTC-enforced timestamps for time filtering
- Improved exception formatting (cleaned file paths)
- Error-handling for Spectre markup edge cases

### Fixed
- `--page-size` now works correctly when combined with `--take`

### Changed
- Refactored CLI structure and added XML docs
