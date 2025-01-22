using ChunkSearch.Chunk;

namespace ChunkSearch.Test.Chunk;

/// <summary>
/// Unit tests for <see cref="ChunkEntry"/>.
/// </summary>
public class ChunkEntryTest
{
    [Theory]
    [InlineData("1", false, "abc.js", "1: abc.js")]
    [InlineData("1", true, "abc.js", "\"1\": abc.js")]
    public void ToString_ReturnsCorrectValue(string content, bool quote, string file, string expected)
    {
        // arrange
        var sut = new ChunkEntry(new ChunkId(content, quote), file);

        // act
        var result = sut.ToString();

        // assert
        Assert.Equal(expected, result);
    }
}
