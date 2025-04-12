using System.Net;
using Microsoft.Extensions.FileSystemGlobbing;
using WpkSnoop.Core;

namespace WpkSnoop.Cli;

/// <summary>
/// Helper methods for handling CLI.
/// </summary>
public static class CliHelper
{
    private static readonly Lock Lock = new();

    /// <summary>
    /// Runs the process.
    /// </summary>
    /// <param name="options">The CLI options.</param>
    public static async Task Run(CliOptions options)
    {
        try
        {
            if (options.IsDirectory)
                await RunDirectory(options);
            else
                await RunFile(options);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"ERROR: {ex.Message}");
            var inner = ex.InnerException;
            while (inner is not null)
            {
                await Console.Error.WriteLineAsync($"DUE TO: {inner.Message}");
                inner = inner.InnerException;
            }
        }
    }

    /// <summary>
    /// Runs the process on a JS file.
    /// </summary>
    /// <param name="options">The CLI options.</param>
    private static async Task RunFile(CliOptions options)
    {
        using var client = GetProxyHttpClient(options);
        await Extract(options.Path, options, client);
    }

    /// <summary>
    /// Runs the process on a JS directory.
    /// </summary>
    /// <param name="options">The CLI options.</param>
    private static async Task RunDirectory(CliOptions options)
    {
        using var client = GetProxyHttpClient(options);
        var matcher = new Matcher();
        matcher.AddInclude("**/*.js");
        foreach (var filePath in matcher.GetResultsInFullPath(options.Path))
            await Extract(filePath, options, client);
    }

    /// <summary>
    /// Runs the chunk extraction process.
    /// </summary>
    /// <param name="path">The JS file path.</param>
    /// <param name="options">The user CLI options.</param>
    /// <param name="client">The HTTP client for proxy.</param>
    private static async Task Extract(string path, CliOptions options, HttpClient client)
    {
        if (options.IsVerbose)
            Console.WriteLine($"PROCESS {path}");

        var js = await File.ReadAllTextAsync(path);
        var loaders = ChunkLoaderHelper.GetChunkLoaders(js);
        foreach (var loader in loaders)
        {
            var ids = loader.GetIds().ToList();
            if (ids.Count == 0)
                continue;

            var entries = loader.Execute(ids).ToList();
            PrintLoader(path, loader.ToString(), entries);
            if (!string.IsNullOrEmpty(options.Domain))
                await SendGet(client, entries, options.Domain, options.IsVerbose, options.Threads);
        }
    }

    /// <summary>
    /// Outputs the chunk loader to console.
    /// </summary>
    /// <param name="path">The path to the file containing the loader.</param>
    /// <param name="loader">The loader JS code.</param>
    /// <param name="entries">The entries in the loader.</param>
    private static void PrintLoader(string path, string loader, List<ChunkEntry> entries)
    {
        Console.WriteLine($"Found chunk loader in {path}:");
        Console.WriteLine(loader);
        if (entries.Count > 0)
            Console.WriteLine(Environment.NewLine + string.Join(Environment.NewLine, entries));
        Console.WriteLine();
    }

    /// <summary>
    /// Sends GET requests to the site using the specified chunk entries.
    /// </summary>
    /// <param name="client">The HTTP client to use.</param>
    /// <param name="entries">The entries in the loader.</param>
    /// <param name="domain">The site domain.</param>
    /// <param name="verbose">Whether to verbose output.</param>
    /// <param name="threads">The number of threads to use.</param>
    private static async Task SendGet(HttpClient client, List<ChunkEntry> entries, string domain, bool verbose, int threads)
    {
        var tasks = new List<Task>();
        foreach (var entry in entries)
        {
            var url = $"{domain.TrimEnd('/')}/{entry.ChunkFile.TrimStart('/')}";
            tasks.Add(Task.Run(() => SendGet(client, url, verbose)));
        }

        foreach (var chunk in tasks.Chunk(threads))
            await Task.WhenAll(chunk);
    }

    /// <summary>
    /// Sends a GET request to a specific URL.
    /// </summary>
    /// <param name="client">The HTTP client to use.</param>
    /// <param name="url">The URL to the site.</param>
    /// <param name="verbose">Whether to output verbose.</param>
    private static async Task SendGet(HttpClient client, string url, bool verbose)
    {
        var result = await client.GetAsync(new Uri(url));
        if (verbose)
        {
            lock (Lock)
            {
                Console.WriteLine($"HTTP GET {url}: {result.StatusCode}");
            }
        }
    }

    /// <summary>
    /// Returns a <see cref="HttpClient"/> with user's proxy setup.
    /// </summary>
    /// <param name="options">The CLI options.</param>
    /// <returns>The <see cref="HttpClient"/> with user's proxy setup.</returns>
    private static HttpClient GetProxyHttpClient(CliOptions options)
    {
        var handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            Proxy = string.IsNullOrEmpty(options.Proxy) ? null : new WebProxy(new Uri(options.Proxy), false),
            UseProxy = !string.IsNullOrEmpty(options.Proxy),
        };
        if (options.IsInsecure)
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var client = new HttpClient(handler);
        foreach (var header in options.HttpHeaders)
        {
            var splits = header.Split(':');
            if (splits.Length == 2)
                client.DefaultRequestHeaders.Add(splits[0].Trim(), splits[1].Trim());
        }
        return client;
    }
}
