using System.Security.Cryptography;

namespace FileMutation.Services;

/// <summary>
/// Mutates text by appending generation metadata.
/// </summary>
public sealed class TextFileMutatorService : ITextFileMutatorService
{
    /// <inheritdoc />
    public string Mutate(string content)
    {
        var randomToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
        return $"{content}{Environment.NewLine}Mutated at (UTC): {DateTime.UtcNow:O}{Environment.NewLine}Random token: {randomToken}{Environment.NewLine}";
    }
}