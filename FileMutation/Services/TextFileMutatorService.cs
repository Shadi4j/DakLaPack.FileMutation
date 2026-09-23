using System.Security.Cryptography;

namespace FileMutation.Services;

/// <summary>
/// Mutates text by appending generation metadata.
/// </summary>
public sealed class TextFileMutatorService : ITextFileMutatorService
{
    /// <summary>
    /// Appends a UTC timestamp and a cryptographically random hexadecimal token to the supplied text content.
    /// </summary>
    public string Mutate(string content)
    {
        var randomToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
        return $"{content}{Environment.NewLine}Mutated at (UTC): {DateTime.UtcNow:O}{Environment.NewLine}Random token: {randomToken}{Environment.NewLine}";
    }
}