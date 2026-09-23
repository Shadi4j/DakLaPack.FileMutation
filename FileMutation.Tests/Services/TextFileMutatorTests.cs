using System.Globalization;
using System.Text.RegularExpressions;
using FileMutation.Services;
using Xunit;

namespace FileMutation.Tests.Services;

public sealed partial class TextFileMutatorTests
{
    [Fact]
    public void Mutate_PreservesExistingContent()
    {
        const string originalContent = "Existing file content";
        var mutator = new TextFileMutatorService();

        var result = mutator.Mutate(originalContent);

        Assert.StartsWith($"{originalContent}{Environment.NewLine}Mutated at (UTC): ", result);
    }

    [Fact]
    public void Mutate_PreservesMultilineContent()
    {
        const string originalContent = "First line\nSecond line\nThird line";
        var mutator = new TextFileMutatorService();

        var result = mutator.Mutate(originalContent);

        Assert.StartsWith($"{originalContent}{Environment.NewLine}Mutated at (UTC): ", result);
    }

    [Fact]
    public void Mutate_AllowsEmptyContent()
    {
        var mutator = new TextFileMutatorService();

        var result = mutator.Mutate(string.Empty);

        Assert.StartsWith($"{Environment.NewLine}Mutated at (UTC): ", result);
    }

    [Fact]
    public void Mutate_AppendsUtcTimestampAndRandomHexToken()
    {
        var mutator = new TextFileMutatorService();

        var result = mutator.Mutate("Existing file content");

        var match = MutationMetadataPattern().Match(result);
        Assert.True(match.Success);
        Assert.True(DateTime.TryParse(match.Groups["timestamp"].Value, CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind, out var timestamp));
        Assert.Equal(DateTimeKind.Utc, timestamp.Kind);
        Assert.Matches("^[0-9A-F]{16}$", match.Groups["token"].Value);
    }

    [GeneratedRegex("Mutated at \\(UTC\\): (?<timestamp>.+)\\r?\\nRandom token: (?<token>[0-9A-F]{16})\\r?\\n$")]
    private static partial Regex MutationMetadataPattern();
}