using Pdf2Html.Settings;

using System.Diagnostics;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<ConversionOptions>();

var app = builder.Build();
var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
app.Logger.LogInformation($"Starting {versionInfo.ProductName} {versionInfo.ProductVersion}");
app.Logger.LogInformation($"Using pdf2htmlEX command line arguments: {app.Services.GetService<ConversionOptions>()!.CommandLineArguments}");

app.MapControllers();
app.Run();
