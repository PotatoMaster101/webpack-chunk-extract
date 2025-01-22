using System.Net;
using Acornima;
using ChunkSearch.Chunk;
using Microsoft.Extensions.FileSystemGlobbing;

namespace ChunkSearch;

/// <summary>
/// Helper methods for handling CLI arguments.
/// </summary>
public static class ArgHelper
{
    private static readonly Lock Lock = new();

    /// <summary>
    /// Handles the user CLI arguments.
    /// </summary>
    /// <param name="args">The user CLI arguments.</param>
    public static Task Handle(ArgOption args)
    {
        return args.IsDirectory ? HandleDirectoryArg(args) : HandleFileArg(args);
    }

    /// <summary>
    /// Handles file arguments.
    /// </summary>
    /// <param name="args">The user CLI arguments.</param>
    public static async Task HandleFileArg(ArgOption args)
    {
        using var client = GetProxyHttpClient(args);
        await Extract(args.Path, args, client);
    }

    /// <summary>
    /// Handle directory arguments.
    /// </summary>
    /// <param name="args">The user CLI arguments.</param>
    public static async Task HandleDirectoryArg(ArgOption args)
    {
        using var client = GetProxyHttpClient(args);
        var matcher = new Matcher();
        matcher.AddInclude("**/*.js");
        foreach (var filePath in matcher.GetResultsInFullPath(args.Path))
            await Extract(filePath, args, client);
    }

    /// <summary>
    /// Runs the chunk extraction.
    /// </summary>
    /// <param name="path">The JS file path.</param>
    /// <param name="args">The user CLI arguments.</param>
    /// <param name="client">The HTTP client for proxy.</param>
    private static async Task Extract(string path, ArgOption args, HttpClient client)
    {
        if (args.IsVerbose)
            Console.WriteLine($"PROCESS {path}");

        var parser = new Parser();
        var js = await File.ReadAllTextAsync(path);
        var loaders = ChunkLoader.FromRootNode(parser.ParseModule(js));
        foreach (var loader in loaders)
        {
            var ids = loader.GetChunkIds();
            if (ids.Count == 0)
                continue;

            var entries = loader.Run(ids).ToList();
            PrintLoader(path, loader.ToString(), entries);
            if (args.Domain is not null)
                await SendGet(client, entries, args.Domain, args.IsVerbose, args.NumThreads);
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
    /// Returns a <see cref="HttpClient"/> with user's proxy setup.
    /// </summary>
    /// <param name="args">The user CLI arguments.</param>
    /// <returns>The <see cref="HttpClient"/> with user's proxy setup.</returns>
    private static HttpClient GetProxyHttpClient(ArgOption args)
    {
        var handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            Proxy = args.Proxy is null ? null : new WebProxy(new Uri(args.Proxy), false),
            UseProxy = args.Proxy is not null,
        };
        if (args.IsInsecure)
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var client = new HttpClient(handler);
        foreach (var header in args.HttpHeaders)
        {
            var splits = header.Split(':');
            if (splits.Length == 2)
                client.DefaultRequestHeaders.Add(splits[0].Trim(), splits[1].Trim());
        }
        return client;
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
}
