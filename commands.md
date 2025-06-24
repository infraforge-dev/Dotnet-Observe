# Dotnet-Observe CLI – Command Reference

This document tracks the subcommands available in the `dotnet-observe` CLI toolkit, both implemented and planned.

---

## ✅ Implemented Commands

### `tail`

Stream and filter structured logs with support for paging, filtering, and multiple output formats.

**Options:**

- `--take`, `-n`: Number of logs to display
- `--level`, `-l`: Log level filter (e.g., Info, Warn, Error)
- `--since`: Filter logs after a specific UTC timestamp
- `--contains`: Filter by keyword in message or context
- `--page-size`: Show logs in pages
- `--json <pretty|compact>`: Render logs in structured JSON output

---

## 🧩 In Progress / Planned

### `analyze`

Aggregate logs and show patterns, frequencies, or trends (e.g., top errors).

**Potential Flags:**

- `--group-by`: e.g., level, status, source
- `--top <n>`: Show top N occurrences
- `--since`
- `--json`

---

### `explain`

Explain a correlation ID or exception chain across services.

**Potential Flags:**

- `--id <correlationId>`
- `--depth`
- `--json`

---

### `summarize`

Generate a high-level summary of system health or error activity.

**Example Usage:**

```bash
dotnet-observe summarize --since "2025-06-21T00:00:00Z" --level Error
```

---

### `metrics`

Display metrics from a structured source or endpoint.

**Potential Features:**

- Parse Prometheus `/metrics`
- Show request counts, error rates, averages
- Output in table or JSON

---

### `trace`

View trace spans across services or events from log data.

---

### `config`

Manage CLI settings like themes, symbols, and log paths.

---

## 🧪 Experimental / Future Ideas

### `validate`

Run a diagnostic against a project or log source to ensure observability fields are present.

### `simulate` / `replay`

Inject test events or replay logs for development and testing purposes.

---

## 📌 Notes

- Subcommands are designed to be composable and pipeline-friendly.
- Future support for agent-mode and remote log streaming is under consideration.

