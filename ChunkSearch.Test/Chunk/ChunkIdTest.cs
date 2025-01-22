using ChunkSearch.Chunk;

namespace ChunkSearch.Test.Chunk;

/// <summary>
/// Unit tests for <see cref="ChunkId"/>.
/// </summary>
public class ChunkIdTest
{
    [Theory]
    [InlineData("1", true, "\"1\"")]
    [InlineData("1", false, "1")]
    [InlineData("abc", true, "\"abc\"")]
    public void ToString_ReturnsCorrectValue(string content, bool quote, string expected)
    {
        // arrange
        var sut = new ChunkId(content, quote);

        // act
        var result = sut.ToString();

        // assert
        Assert.Equal(expected, result);
    }
}
