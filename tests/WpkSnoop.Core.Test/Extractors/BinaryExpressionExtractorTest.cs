using System.Collections;
using Acornima;
using WpkSnoop.Core.Extractors;

namespace WpkSnoop.Core.Test.Extractors;

/// <summary>
/// Unit tests for <see cref="BinaryExpressionExtractor"/>.
/// </summary>
public class BinaryExpressionExtractorTest
{
    [Fact]
    public void CanExtract_ReturnsCorrectValue()
    {
        // arrange
        var parser = new Parser();
        var sut = new BinaryExpressionExtractor();

        // assert
        Assert.True(sut.CanExtract(parser.ParseExpression("e === 1")));
        Assert.True(sut.CanExtract(parser.ParseExpression("e == 1")));
        Assert.False(sut.CanExtract(parser.ParseExpression("e >= 1")));
    }

    [Theory]
    [ClassData(typeof(ExtractTestData))]
    public void Extract_ReturnsCorrectValue(string js, ChunkId[] expected)
    {
        // arrange
        var parser = new Parser();
        var node = parser.ParseExpression(js);
        var sut = new BinaryExpressionExtractor();

        // act
        var result = sut.Extract(node).ToList();

        // assert
        Assert.Equal(expected.Length, result.Count);
        foreach (var item in expected)
            Assert.Contains(item, result);
    }

    /// <summary>
    /// Test data for <see cref="BinaryExpressionExtractorTest.Extract_ReturnsCorrectValue"/>.
    /// </summary>
    private class ExtractTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return ["e == 1", new ChunkId[] { new("1", false) }];
            yield return ["e === \"1\"", new ChunkId[] { new("1") }];
            yield return ["1 == e", new ChunkId[] { new("1", false) }];
            yield return ["1 === e", new ChunkId[] { new("1", false) }];
            yield return ["e === x", Array.Empty<ChunkId>()];
            yield return ["e >= 1", Array.Empty<ChunkId>()];
        }
    }
}
