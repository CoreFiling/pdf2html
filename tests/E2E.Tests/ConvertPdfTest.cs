using System.Net;

namespace E2E.Tests;

public class ConvertPdfTest
{
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
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await response.Content.ReadAsStreamAsync(), Is.EqualTo(GetResourceStream("CS_cheat_sheet.html")));
        });
    }

    private static Stream GetResourceStream(string resourceName)
    {
        return typeof(ConvertPdfTest).Assembly.GetManifestResourceStream($"E2E.Tests.Resources.{resourceName}") ??
            throw new InvalidOperationException($"No resource with name '{resourceName}'");
    }
}
