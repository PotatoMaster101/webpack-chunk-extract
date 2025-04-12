using System.Collections;
using Acornima;
using Acornima.Ast;
using WpkSnoop.Core.Extensions;
using WpkSnoop.Core.Extractors;

namespace WpkSnoop.Core.Test.Extractors;

/// <summary>
/// Unit tests for <see cref="ArrayExpression"/>.
/// </summary>
public class ArrayExpressionExtractorTest
{
    [Fact]
    public void CanExtract_ReturnsCorrectValue()
    {
        // arrange
        var parser = new Parser();
        var script = parser.ParseScript(File.ReadAllText("Samples/array-lookup.js"));
        var sut = new ArrayExpressionExtractor();

        // assert
        Assert.True(sut.CanExtract(script.Walk<ArrayExpression>().First()));
        Assert.False(sut.CanExtract(script.Walk<BinaryExpression>().First()));
        Assert.False(sut.CanExtract(script.Walk<Literal>().First()));
    }

    [Theory]
    [ClassData(typeof(ExtractTestData))]
    public void Extract_ReturnsCorrectValue(string js, ChunkId[] expected)
    {
        // arrange
        var parser = new Parser();
        var expr = parser.ParseExpression(js);
        var node = expr.Walk<ArrayExpression>().First();
        var sut = new ArrayExpressionExtractor();

        // act
        var result = sut.Extract(node).ToList();

        // assert
        Assert.Equal(expected.Length, result.Count);
        foreach (var item in expected)
            Assert.Contains(item, result);
    }

    /// <summary>
    /// Test data for <see cref="ArrayExpressionExtractorTest.Extract_ReturnsCorrectValue"/>.
    /// </summary>
    private class ExtractTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return
            [
                "e => [0,1,2][e]", new ChunkId[] { new("0", false), new("1", false), new("2", false) }
            ];
            yield return
            [
                "e => [0,\"0\"][e]", new ChunkId[] { new("0", false), new("1", false) }
            ];
            yield return
            [
                "e => [\"chunk1\",\"chunk2\"][e]", new ChunkId[] { new("0", false), new("1", false) }
            ];
            yield return
            [
                "e => [e,e,\"chunk\",x][e]", new ChunkId[] { new("0", false) }
            ];
        }
    }
}
