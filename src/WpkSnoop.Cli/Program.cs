using System.CommandLine;
using WpkSnoop.Cli;

var pathArg = new Argument<string>("path", "Path to the JS file containing the chunk loader");
var dirOpt = new Option<bool>(["-d", "--dir"], "Specifies that the path is a directory path");
var verboseOpt = new Option<bool>(["-v", "--verbose"], "Specifies verbose output");
var domainOpt = new Option<string>(["-D", "--domain"], "Specifies the base URL for chunk file GET requests");
var proxyOpt = new Option<string>(["-x", "--proxy"], "Specifies the proxy URL for chunk file GET requests");
var headersOpt = new Option<List<string>>(["-H", "--header"], "Specifies the HTTP headers for chunk file GET requests");
var insecureOpt = new Option<bool>(["-k", "--insecure"], "Specifies to not validate HTTPS certificates for chunk file GET requests");
var threadsOpt = new Option<int>(["-t", "--threads"], "Specifies the number of threads to use");
domainOpt.SetDefaultValue(string.Empty);
proxyOpt.SetDefaultValue(string.Empty);
threadsOpt.SetDefaultValue(int.MaxValue);

var rootCmd = new RootCommand("Webpack Chunk Loader Extractor")
{
    pathArg,
    dirOpt,
    verboseOpt,
    domainOpt,
    proxyOpt,
    headersOpt,
    insecureOpt,
    threadsOpt,
};

rootCmd.SetHandler(async (path, dir, verbose, domain, proxy, headers, insecure, threads) =>
{
    var args = new CliOptions(path, dir, verbose, domain, proxy, headers, insecure, threads);
    await CliHelper.Run(args);
}, pathArg, dirOpt, verboseOpt, domainOpt, proxyOpt, headersOpt, insecureOpt, threadsOpt);
await rootCmd.InvokeAsync(args);
