namespace DotnetObserve.Core.Models;

/// <summary>
/// Stores local CLI preferences such as last-used filters or theme.
/// </summary>
public class CliPreferences
{
    /// <summary>
    /// Last-used log filter (e.g., level or path).
    /// </summary>
    public string? LastFilter { get; set; }

    /// <summary>
    /// Preferred CLI color scheme (e.g., dark, light).
    /// </summary>
    public string? ColorScheme { get; set; } = "dark";
}
