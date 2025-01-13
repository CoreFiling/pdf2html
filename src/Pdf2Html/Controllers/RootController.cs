using System.Diagnostics;
using System.Net.Mime;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Pdf2Html.Controllers;

[ApiController]
[Route("/")]
public class RootController : ControllerBase
{
    private readonly ILogger<RootController> _logger;

    public RootController(ILogger<RootController> logger)
    {
        _logger = logger;
    }

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
                _logger.LogInformation($"Copied {FormatToMb(new FileInfo(inputFile).Length)} to {inputFile}");
            }

            _logger.LogInformation("Starting conversion...");
            var (success, logs) = await ConvertAsync(inputFile, outputFile);

            if (!success)
            {
                _logger.LogError("Conversion failed");
                return StatusCode(StatusCodes.Status500InternalServerError, new { pdf2htmlEX = new { logs } });
            }

            _logger.LogInformation($"Conversion completed ({FormatToMb(new FileInfo(outputFile).Length)})");
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
        const string conversionOptions = "--embed-javascript=0 --process-outline=0 --printing=0 --bg-format=svg --svg-node-count-limit=100 --decompose-ligature 1 --tounicode 1";
        p.StartInfo = new ProcessStartInfo
        {
            FileName = "pdf2htmlEX",
            Arguments = $"{conversionOptions} --dest-dir={Path.GetDirectoryName(outputFile)} {inputFile} {Path.GetFileName(outputFile)}",
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
            _logger.LogInformation(log);
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

    private static string FormatToMb(long bytesLength)
    {
        return (bytesLength / 1024.0 / 1024.0).ToString("0.00 MB");
    }
}
