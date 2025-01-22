using System.Collections;
using Acornima;
using ChunkSearch.Chunk;
using ChunkSearch.Extractors;

namespace ChunkSearch.Test.Extractors;

/// <summary>
/// Unit tests for <see cref="ExtractorHelper"/>
/// </summary>
public class ExtractorHelperTest
{
    [Theory]
    [ClassData(typeof(ExtractAllTestData))]
    public void ExtractAll_ReturnsCorrectValue(string js, ChunkId[] expected)
    {
        // arrange
        var parser = new Parser();
        var node = parser.ParseExpression(js);

        // act
        var result = ExtractorHelper.ExtractAll(node).ToList();

        // assert
        Assert.Equal(expected.Length, result.Count);
        for (var i = 0; i < expected.Length; i++)
            Assert.Equal(expected[i], result[i]);
    }

    /// <summary>
    /// Test data for <see cref="ExtractorHelperTest.ExtractAll_ReturnsCorrectValue"/>.
    /// </summary>
    private class ExtractAllTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return
            [
                File.ReadAllText("SampleNodes/array-lookup.js"),
                new ChunkId[] { new("0", false), new("1", false), new("2", false) }
            ];
            yield return
            [
                File.ReadAllText("SampleNodes/binary-expression.js"),
                new ChunkId[] { new("0", false), new("b"), new("2", false), new("c"), new("4", false) }
            ];
            yield return
            [
                File.ReadAllText("SampleNodes/object-expression.js"),
                new ChunkId[] { new("0", false), new("1", false), new("c") }
            ];
            yield return
            [
                File.ReadAllText("SampleNodes/switch-statement.js"),
                new ChunkId[] { new("a"), new("1", false), new("2", false) }
            ];
        }
    }
}
