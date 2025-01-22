namespace ChunkSearch;

/// <summary>
/// Represents the CLI argument options.
/// </summary>
/// <param name="Path">Specifies the JS file path.</param>
/// <param name="IsDirectory">Specifies that <paramref name="Path"/> is a directory path.</param>
/// <param name="IsVerbose">Specifies to verbose output.</param>
/// <param name="HttpHeaders">Specifies the HTTP headers for chunk file GET requests.</param>
/// <param name="Domain">Specifies the base URL for chunk file GET requests.</param>
/// <param name="Proxy">Specifies the proxy URL for chunk file GET requests.</param>
/// <param name="IsInsecure">Specifies to not validate HTTPS certificates for chunk file GET requests.</param>
/// <param name="NumThreads">Specifies the number of threads to use.</param>
public record ArgOption(
    string Path,
    bool IsDirectory,
    bool IsVerbose,
    List<string> HttpHeaders,
    string? Domain,
    string? Proxy,
    bool IsInsecure,
    int NumThreads);
