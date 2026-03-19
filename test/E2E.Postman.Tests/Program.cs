using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var category = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
var logger = loggerFactory.CreateLogger(category);

logger.LogInformation("Running Postman tests via Newman...");

var process = new Process();
process.StartInfo.FileName = "newman";
process.StartInfo.Arguments = "run postman/collection.json -e postman/environment.json";
process.StartInfo.RedirectStandardOutput = true;
process.StartInfo.UseShellExecute = false;

process.Start();

string output = process.StandardOutput.ReadToEnd();
process.WaitForExit();

logger.LogInformation("{Output}", output);

if (process.ExitCode != 0)
{
    throw new Exception("Postman tests failed");
}