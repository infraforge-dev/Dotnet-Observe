# Dotnet-Observe

**Plug-and-Play Observability Toolkit for .NET**

---

## Table of Contents

1. [Mission Statement](#mission-statement)
2. [Features](#features)
3. [Quickstart](#quickstart)
4. [Data Model Overview](#data-model-overview)
5. [CLI Overview](#cli-overview)
6. [MVP User Stories](#mvp-user-stories)
7. [Roadmap](#roadmap)
8. [Vision](#vision)
9. [License](#license)
10. [Contributing](#contributing)
11. [Author](#author)

---

## Mission Statement

*Empower .NET developers to effortlessly gain deep insight into their applications with a plug-and-play observability toolkit—combining structured logging, metrics, and tracing in one developer-friendly package.*

---

## Features

- 📦 **Single NuGet Package:** Add one library, get observability everywhere.
- 📝 **Structured JSON Logging:** Correlation IDs and context included, ready for parsing.
- ⚡ **Automatic Metrics:** Request durations, error rates, and custom counters out of the box.
- 🧑‍💻 **CLI Tool:** Real-time, color-highlighted log tailing, filtering, and searching.
- 🔗 **Trace Support:** Track requests and operations with trace and span IDs.
- 🔧 **Minimal Configuration:** Just works—but ready to extend and scale.
- 📖 **Professional Documentation:** Easy onboarding, clear API, and plenty of examples.

---

## Quickstart (**Coming Soon**)

1. **Install the NuGet package**

 ```sh
 dotnet add package ProjectName
 ```
2. **Plug into your .NET app**

 ```csharp
  // In Program.cs or Startup.cs
  app.DotnetObserve(options =>
  {
    options.LogLevel = "Info";
    options.EnableMetrics = true;
    options.EnableTracing = true;
  });
 ```
 3. Use the CLI to tail logs and view metrics

 ```sh
 dotnet-observe tail --level Error
 dotnet-observe metrics
 dotnet-observe traces --correlation abc123
 ```

   ---

## Data Model Overview

- **LogEntry**: Structured log messages (level, timestamp, context, correlationId)

  ```csharp
  public class LogEntry
  {
      public Guid Id { get; set; }
      public DateTime Timestamp { get; set; }
      public string Level { get; set; }           // "Info", "Warn", "Error", etc.
      public string Message { get; set; }
      public string Source { get; set; }          // e.g., "API", "BackgroundJob"
      public string? Exception { get; set; }
      public string? CorrelationId { get; set; }  // For tracing across requests
      public Dictionary<string, object>? Context { get; set; } // Enrichment: userId, machine, etc.
  }
  ```
  
- **MetricEntry**: Metrics (name, value, timestamp, tags)
  ```csharp
  public class MetricEntry
  {
      public Guid Id { get; set; }
      public DateTime Timestamp { get; set; }
      public string Name { get; set; }            // e.g., "RequestDuration", "ErrorRate"
      public double Value { get; set; }
      public string? Unit { get; set; }           // ms, count, percent, etc.
      public string? Tags { get; set; }           // For grouping/aggregation, e.g., endpoint, status
  }
  ```
- **TraceSpan**: Spans with traceId, spanId, parentSpanId, operation name, timings
  ```csharp
  public class TraceSpan
  {
      public Guid Id { get; set; }
      public string TraceId { get; set; }         // All spans in one trace share this
      public string SpanId { get; set; }
      public string? ParentSpanId { get; set; }
      public string Name { get; set; }            // e.g., "HTTP GET /api/products"
      public DateTime StartTime { get; set; }
      public DateTime EndTime { get; set; }
      public Dictionary<string, object>? Attributes { get; set; } // e.g., http status, userId
  }
  ```
- **ToolkitConfig**: Toolkit settings (log level, enabled features, exporters)
  ```csharp
  public class ToolkitConfig
  {
      public string LogLevel { get; set; }            // "Info", "Debug", etc.
      public bool EnableMetrics { get; set; }
      public bool EnableTracing { get; set; }
      public string Exporter { get; set; }            // "Console", "File", "Prometheus", etc.
      // Additional config options can be added here
  }
  ```
- **CLI Preference**
  ```csharp
  public class CliPreferences
  {
      public string? LastFilter { get; set; }
      public string? ColorScheme { get; set; }
  }
  ```
---

## CLI Overview
- `tail`: View and filter logs in real time with color highlighting
- `metrics`: Display and export performance metrics
- `traces`: Search and display trace data by correlation or span
- `config`: View and edit toolkit configuration

---

## MVP User Stories
- Add a NuGet package and enable middleware for instant logs, metrics, and traces
- Auto log HTTP requests/responses with correlation IDs
- Show error logs with stack traces for fast debugging
- JSON log formatting for easy integration and search
- CLI for log tailing and filtering with color highlights
- Basic metrics and health tracking
- Custom context enrichment (user, machine, etc.)
- Easy onboarding with professional docs

---

## Roadmap
 - ~~Data models defined~~
 - ~~MVP user stories prioritized~~
 - CLI/wireframe designed
 - Core architecture mapped
 - Project setup & scaffolding
 - CLI tool MVP
 - Storage layer (file/DB)
 - More exporters/integrations
 - Web dashboard (future)

---

## Vision

Vision
- Developer-first: Works instantly, grows with you.
- Flagship quality: Robust, scalable, and beautiful CLI experience.
- Open source: Contributions, ideas, and feedback welcome!

---

## License
MIT — Open source and free for personal or commercial use.

---

## Contributing
See CONTRIBUTING.md for guidelines (coming soon).

---

## Author
Jose E. Rodriguez-Marrero @infraforge.dev
