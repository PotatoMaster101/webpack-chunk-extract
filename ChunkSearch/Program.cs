using System.CommandLine;
using ChunkSearch;

var pathArg = new Argument<string>("path", "Path to the JS file containing the chunk loader");
var dirOpt = new Option<bool>(["-d", "--dir"], "Specifies that the path is a directory path");
var verboseOpt = new Option<bool>(["-v", "--verbose"], "Specifies verbose output");
var headersOpt = new Option<List<string>>(["-H", "--header"], "Specifies the HTTP headers for chunk file GET requests");
var domainOpt = new Option<string>(["-D", "--domain"], "Specifies the base URL for chunk file GET requests");
var proxyOpt = new Option<string>(["-x", "--proxy"], "Specifies the proxy URL for chunk file GET requests");
var insecureOpt = new Option<bool>(["-k", "--insecure"], "Specifies to not validate HTTPS certificates for chunk file GET requests");
var threadsOpt = new Option<int>(["-t", "--threads"], "Specifies the number of threads to use");
threadsOpt.SetDefaultValue(int.MaxValue);
var rootCmd = new RootCommand("Webpack Chunk Loader Extractor")
{
    pathArg,
    dirOpt,
    verboseOpt,
    headersOpt,
    domainOpt,
    proxyOpt,
    insecureOpt,
    threadsOpt,
};

rootCmd.SetHandler(async (path, isDir, isVerbose, headers, domain, proxy, isInsecure, threads) =>
{
    var args = new ArgOption(path, isDir, isVerbose, headers, domain, proxy, isInsecure, threads);
    try
    {
        await ArgHelper.Handle(args);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"ERROR: {ex.Message}");
        var inner = ex.InnerException;
        while (inner is not null)
        {
            Console.Error.WriteLine($"ERROR: {inner.Message}");
            inner = inner.InnerException;
        }
    }

}, pathArg, dirOpt, verboseOpt, headersOpt, domainOpt, proxyOpt, insecureOpt, threadsOpt);

await rootCmd.InvokeAsync(args);
