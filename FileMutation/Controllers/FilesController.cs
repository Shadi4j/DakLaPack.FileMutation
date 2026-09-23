using System.Text;
using FileMutation.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileMutation.Controllers;

/// <summary>
/// Uploads and mutates text files.
/// </summary>
[ApiController]
[Route("api/files")]
public sealed class FilesController(ITextFileMutatorService textFileMutator) : ControllerBase
{
    private const long MaxFileSizeInBytes = 1_048_576;

    /// <summary>
    /// Appends metadata to an uploaded text file and returns it as a download.
    /// </summary>
    /// <param name="file">The text file to mutate.</param>
    /// <returns>The mutated text file.</returns>
    [HttpPost("mutate")]
    [Consumes("multipart/form-data")]
    [Produces("text/plain")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Mutate(IFormFile? file)
    {
        if (file is null)
        {
            return ValidationProblem("Please upload a text file.");
        }

        if (file.Length > MaxFileSizeInBytes)
        {
            return ValidationProblem("The uploaded file must be 1 MB or smaller.");
        }

        if (!Path.GetExtension(file.FileName).Equals(".txt", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationProblem("Only .txt files are supported.");
        }

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var content = await reader.ReadToEndAsync();
        var mutatedContent = textFileMutator.Mutate(content);
        var downloadName = $"{Path.GetFileNameWithoutExtension(Path.GetFileName(file.FileName))}-mutated.txt";

        return File(Encoding.UTF8.GetBytes(mutatedContent), "text/plain", downloadName);
    }
}