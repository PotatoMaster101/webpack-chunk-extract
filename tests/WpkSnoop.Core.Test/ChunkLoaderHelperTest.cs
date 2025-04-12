using System.Collections;
using Acornima;

namespace WpkSnoop.Core.Test;

/// <summary>
/// Unit tests for <see cref="ChunkLoaderHelper"/>.
/// </summary>
public class ChunkLoaderHelperTest
{
    [Theory]
    [InlineData("e => ['chunk1', 'chunk2'][e] + '.js'", true)]
    [InlineData("e => {if (e === 1) { return 'chunk1.js' } if (e === 2) { return 'chunk2.js' }}", true)]
    [InlineData("e => ['chunk1', 'chunk2'][e] + '.css'", false)]
    [InlineData("e === 3", false)]
    [InlineData("e => { while(true) {} return 'chunk1.js'}", false)]
    [InlineData("e => { func(); return 'chunk1.js'}", false)]
    public void IsChunkLoader_ReturnsCorrectValue(string js, bool expected)
    {
        // arrange
        var parser = new Parser();
        var expr = parser.ParseExpression(js);

        // act
        var result = ChunkLoaderHelper.IsChunkLoader(expr);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [ClassData(typeof(GetChunkLoadersTestData))]
    public void GetChunkLoaders_ReturnsCorrectValue(string js, int expectedCount)
    {
        // arrange
        var parser = new Parser();
        var module = parser.ParseModule(js);

        // act
        var result = ChunkLoaderHelper.GetChunkLoaders(module).ToList();

        // assert
        Assert.Equal(expectedCount, result.Count);
    }

    /// <summary>
    /// Test data for <see cref="ChunkLoaderHelperTest.GetChunkLoaders_ReturnsCorrectValue"/>.
    /// </summary>
    private class GetChunkLoadersTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [File.ReadAllText("Samples/array-lookup.js"), 1];
            yield return [File.ReadAllText("Samples/big.js"), 1];
            yield return [File.ReadAllText("Samples/field-ref.js"), 1];
            yield return [File.ReadAllText("Samples/func-declare.js"), 1];
            yield return [File.ReadAllText("Samples/has-source-map.js"), 1];
            yield return [File.ReadAllText("Samples/ifs.js"), 1];
            yield return [File.ReadAllText("Samples/inf-loop.js"), 1];
            yield return [File.ReadAllText("Samples/mix-ternary-map.js"), 1];
            yield return [File.ReadAllText("Samples/multi-field-ref.js"), 1];
            yield return [File.ReadAllText("Samples/multi-roots.js"), 1];
            yield return [File.ReadAllText("Samples/no-minification.js"), 1];
            yield return [File.ReadAllText("Samples/no-numeric-ids.js"), 1];
            yield return [File.ReadAllText("Samples/small.js"), 1];
            yield return [File.ReadAllText("Samples/switch.js"), 1];
            yield return [File.ReadAllText("Samples/ternary.js"), 1];
            yield return [File.ReadAllText("Samples/two-loaders.js"), 2];
        }
    }
}
