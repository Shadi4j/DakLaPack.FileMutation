using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FileMutation.Tests.Controllers;

public sealed class FilesControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Mutate_ReturnsBadRequest_WhenFileIsMissing()
    {
        using var content = new MultipartFormDataContent();
        using var response = await factory.CreateClient().PostAsync("/api/files/mutate", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Mutate_ReturnsMutatedTextFile_WhenFileIsEmpty()
    {
        using var response = await SendFileAsync(string.Empty, "empty.txt");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("empty-mutated.txt", response.Content.Headers.ContentDisposition?.ToString());
    }

    [Fact]
    public async Task Mutate_ReturnsBadRequest_WhenFileExceedsOneMegabyte()
    {
        var oversizedContent = new string('a', 1_048_577);
        using var response = await SendFileAsync(oversizedContent, "oversized.txt");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Mutate_ReturnsMutatedTextFile_WhenTextFileIsUploaded()
    {
        const string fileContents = "Existing file content";
        using var response = await SendFileAsync(fileContents, "upload.txt");

        var mutatedContent = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("upload-mutated.txt", response.Content.Headers.ContentDisposition?.ToString());
        Assert.StartsWith($"{fileContents}{Environment.NewLine}Mutated at (UTC): ", mutatedContent);
    }

    [Fact]
    public async Task Mutate_ReturnsBadRequest_WhenFileIsNotText()
    {
        using var response = await SendFileAsync("column,value", "data.csv");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Mutate_ReturnsBadRequest_WhenFileHasNoExtension()
    {
        using var response = await SendFileAsync("plain content", "upload");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HttpResponseMessage> SendFileAsync(string fileContents, string fileName)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(fileContents), "file", fileName);
        return await factory.CreateClient().PostAsync("/api/files/mutate", content);
    }
}
