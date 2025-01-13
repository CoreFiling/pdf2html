using System.Diagnostics;
using System.Net.Mime;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace Pdf2Html.Controllers;

[ApiController]
[Route("/")]
public class RootController(ILogger<RootController> logger, IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public ActionResult Get()
    {
        var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        return Ok(new
        {
            name = versionInfo.ProductName,
            version = versionInfo.ProductVersion
        });
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<ActionResult> Post()
    {
        var inputFile = Path.GetTempFileName();
        var outputFile = $"{inputFile}.html";
        try
        {
            await using (var tempFileStream = System.IO.File.Open(inputFile, FileMode.Truncate))
            {
                await Request.Body.CopyToAsync(tempFileStream);
                logger.LogInformation($"Copied {FormatToMb(new FileInfo(inputFile).Length)} to {inputFile}");
            }

            logger.LogInformation("Starting conversion...");
            var (success, logs) = await ConvertAsync(inputFile, outputFile);

            if (!success)
            {
                logger.LogError("Conversion failed");
                return StatusCode(StatusCodes.Status500InternalServerError, new { pdf2htmlEX = new { logs } });
            }

            logger.LogInformation($"Conversion completed ({FormatToMb(new FileInfo(outputFile).Length)})");
            return File(await System.IO.File.ReadAllBytesAsync(outputFile), MediaTypeNames.Text.Html);
        }
        finally
        {
            System.IO.File.Delete(inputFile);
            System.IO.File.Delete(outputFile);
        }
    }

    private async Task<(bool Success, ICollection<string> logs)> ConvertAsync(string inputFile, string outputFile)
    {
        using var p = new Process();
        string options = ToCommandLineArguments(configuration.GetSection("ConversionOptions").AsEnumerable());
        p.StartInfo = new ProcessStartInfo
        {
            FileName = "pdf2htmlEX",
            Arguments = $"{options} --dest-dir={Path.GetDirectoryName(outputFile)} {inputFile} {Path.GetFileName(outputFile)}",
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var logs = new List<string>();

        void AddLog(string? log)
        {
            if (string.IsNullOrEmpty(log))
            {
                return;
            }

            logs.Add(log);
            logger.LogInformation(log);
        }

        p.OutputDataReceived += (_, e) => AddLog(e.Data);
        p.ErrorDataReceived += (_, e) => AddLog(e.Data);
        p.EnableRaisingEvents = true;

        p.Start();
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();
        await p.WaitForExitAsync();
        return (p.ExitCode == 0, logs);
    }

    internal static string ToCommandLineArguments(IEnumerable<KeyValuePair<string, string?>> options) =>
        string.Join(' ', options.Where(kvp => kvp.Value != null).Select(kvp => $"--{ToKebabCase(kvp.Key.Replace("ConversionOptions:", ""))}={ValueToString(kvp.Value!)}"));

    private static string ValueToString(string value) => bool.TryParse(value, out var boolValue) ? (boolValue ? "1" : "0") : value;

    private static string ToKebabCase(string value) =>
        Regex.Replace(value, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])", "-$1", RegexOptions.Compiled).Trim().ToLower();

    private static string FormatToMb(long bytesLength) => (bytesLength / 1024.0 / 1024.0).ToString("0.00 MB");
}
