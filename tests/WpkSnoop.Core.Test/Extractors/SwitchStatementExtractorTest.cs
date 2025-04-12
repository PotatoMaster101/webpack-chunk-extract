using System.Collections;
using Acornima;
using Acornima.Ast;
using WpkSnoop.Core.Extensions;
using WpkSnoop.Core.Extractors;

namespace WpkSnoop.Core.Test.Extractors;

/// <summary>
/// Unit tests for <see cref="SwitchStatementExtractor"/>.
/// </summary>
public class SwitchStatementExtractorTest
{
    [Fact]
    public void CanExtract_ReturnsCorrectValue()
    {
        // arrange
        var parser = new Parser();
        var script = parser.ParseScript(File.ReadAllText("Samples/switch.js"));
        var sut = new SwitchStatementExtractor();

        // assert
        Assert.True(sut.CanExtract(script.Walk<SwitchStatement>().First()));
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
        var node = expr.Walk<SwitchStatement>().First();
        var sut = new SwitchStatementExtractor();

        // act
        var result = sut.Extract(node).ToList();

        // assert
        Assert.Equal(expected.Length, result.Count);
        foreach (var item in expected)
            Assert.Contains(item, result);
    }

    /// <summary>
    /// Test data for <see cref="SwitchStatementExtractorTest.Extract_ReturnsCorrectValue"/>.
    /// </summary>
    private class ExtractTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return
            [
                "e => {switch(e) {case 1: return \"chunk.js\";}}", new ChunkId[] { new("1", false) }
            ];
            yield return
            [
                "e => {switch(e) {case 1: return \"chunk.js\"; case 2: return \"chunk.js\"}}",
                new ChunkId[] { new("1", false), new("2", false) }
            ];
            yield return
            [
                "e => {switch(e) {case \"a\": return \"chunk.js\"; case \"b\": return \"chunk.js\"}}",
                new ChunkId[] { new("a"), new("b") }
            ];
            yield return
            [
                "e => {switch(e) {case 1: return \"chunk.js\"; case \"a\": return \"chunk.js\"; case \"b\": return \"chunk.js\"}}",
                new ChunkId[] { new("1", false), new("a"), new("b") }
            ];
            yield return
            [
                "e => {switch(e) {case 1: return \"chunk.js\"; case \"1\": return \"chunk.js\"; case 1: return \"chunk.js\"}}",
                new ChunkId[] { new("1", false), new("1"), new("1", false) }
            ];
        }
    }
}
