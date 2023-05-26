using System.Net;
using System.Text.RegularExpressions;

namespace E2E.Tests;

public partial class ConvertPdfTest
{
    [GeneratedRegex("^@font-face{.*$", RegexOptions.Multiline)]
    private static partial Regex FontFaceRegex();

    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _client = new HttpClient
            { BaseAddress = new Uri(Environment.GetEnvironmentVariable("TEST_SERVICE_URI") ?? "http://localhost:8080") };
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }

    [Test]
    public async Task TestGetStatus()
    {
        var response = await _client.GetAsync("/");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task TestConvertPdf()
    {
        var response = await _client.PostAsync("/", new StreamContent(GetResourceStream("CS_cheat_sheet.pdf")));
        Assert.Multiple(async () =>
        {
            var responseStream = await response.Content.ReadAsStreamAsync();
            var content = await new StreamReader(responseStream).ReadToEndAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                () => $"Conversion failed: {content}");
            var expected = await new StreamReader(GetResourceStream("CS_cheat_sheet.html")).ReadToEndAsync();

            string RemoveFonts(string input) => FontFaceRegex().Replace(input, "");
            Assert.That(RemoveFonts(content), Is.EqualTo(RemoveFonts(expected)));
        });
    }

    private static Stream GetResourceStream(string resourceName)
    {
        return typeof(ConvertPdfTest).Assembly.GetManifestResourceStream($"E2E.Tests.Resources.{resourceName}") ??
            throw new InvalidOperationException($"No resource with name '{resourceName}'");
    }
}
