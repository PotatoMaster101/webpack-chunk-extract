using System.Collections;
using Acornima;
using Acornima.Ast;
using WpkSnoop.Core.Extensions;
using WpkSnoop.Core.Extractors;

namespace WpkSnoop.Core.Test.Extractors;

/// <summary>
/// Unit tests for <see cref="ObjectExpression"/>.
/// </summary>
public class ObjectExpressionExtractorTest
{
    [Fact]
    public void CanExtract_ReturnsCorrectValue()
    {
        // arrange
        var parser = new Parser();
        var script = parser.ParseScript(File.ReadAllText("Samples/small.js"));
        var sut = new ObjectExpressionExtractor();

        // assert
        Assert.True(sut.CanExtract(script.Walk<ObjectExpression>().First()));
        Assert.False(sut.CanExtract(script.Walk<BinaryExpression>().First()));
        Assert.False(sut.CanExtract(script.Walk<Literal>().First()));
    }

    [Theory]
    [ClassData(typeof(ExtractTestData))]
    public void Extract_ReturnsCorrectValue(string js, ChunkId[] expected)
    {
        // arrange
        var parser = new Parser();
        var parsed = parser.ParseExpression(js);
        var node = parsed.Walk<ObjectExpression>().First();
        var sut = new ObjectExpressionExtractor();

        // act
        var result = sut.Extract(node).ToList();

        // assert
        Assert.Equal(expected.Length, result.Count);
        for (var i = 0; i < expected.Length; i++)
            Assert.Equal(expected[i], result[i]);
    }

    /// <summary>
    /// Test data for <see cref="ObjectExpressionExtractorTest.Extract_ReturnsCorrectValue"/>.
    /// </summary>
    private class ExtractTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return
            [
                "e => ({1:\"chunk\"}[e] + \".js\")", new ChunkId[] { new("1", false) }
            ];
            yield return
            [
                "e => ({2:\"chunk\"}[e] + \".js\")", new ChunkId[] { new("2", false) }
            ];
            yield return
            [
                "e => ({1:\"chunk\",2:\"chunk\"}[e] + \".js\")", new ChunkId[] { new("1", false), new("2", false) }
            ];
            yield return
            [
                "e => ({\"a\":\"chunk\",\"b\":\"chunk\"}[e] + \".js\")", new ChunkId[] { new("a"), new("b") }
            ];
            yield return
            [
                "e => ({1:\"chunk\",\"a\":\"chunk\",\"b\":\"chunk\"}[e] + \".js\")",
                new ChunkId[] { new("1", false), new("a"), new("b") }
            ];
            yield return
            [
                "e => ({\"1\":\"chunk\",\"1\":\"chunk\",1:\"chunk\"}[e] + \".js\")",
                new ChunkId[] { new("1"), new("1"), new("1", false) }
            ];
        }
    }
}
