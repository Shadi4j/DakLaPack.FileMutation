namespace FileMutation.Services;

/// <summary>
/// Defines the operation that appends generated metadata to text file content.
/// </summary>
public interface ITextFileMutatorService
{
    /// <summary>
    /// Returns the supplied content with the current UTC date and a random token appended.
    /// </summary>
    string Mutate(string content);
}