namespace WpkSnoop.Cli;

/// <summary>
/// Represents the CLI options.
/// </summary>
public record CliOptions(
    string Path,
    bool IsDirectory,
    bool IsVerbose,
    string Domain,
    string Proxy,
    List<string> HttpHeaders,
    bool IsInsecure,
    int Threads);
