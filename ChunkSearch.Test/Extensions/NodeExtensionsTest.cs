using Acornima;
using Acornima.Ast;
using ChunkSearch.Extensions;

namespace ChunkSearch.Test.Extensions;

/// <summary>
/// Unit tests for <see cref="ChunkSearch.Extensions.NodeExtensions"/>.
/// </summary>
public class NodeExtensionsTest
{
    [Fact]
    public void Walk_ReturnsCorrectValue()
    {
        // arrange
        var parser = new Parser();
        var expr = parser.ParseExpression("e => ({\"a\":\"chunk\",\"b\":\"chunk\"}[e] + \".js\")");

        // act
        var result = expr.Walk().ToList();

        // assert
        Assert.Equal(13, result.Count);
        Assert.True(result[0] is ArrowFunctionExpression);
        Assert.True(result[1] is Identifier);
        Assert.True(result[2] is BinaryExpression);
        Assert.True(result[3] is MemberExpression);
        Assert.True(result[4] is ObjectExpression);
        Assert.True(result[5] is Property);
        Assert.True(result[6] is Literal);
        Assert.True(result[7] is Literal);
        Assert.True(result[8] is Property);
        Assert.True(result[9] is Literal);
        Assert.True(result[10] is Literal);
        Assert.True(result[11] is Identifier);
        Assert.True(result[12] is Literal);
    }

    [Fact]
    public void WalkT_ReturnsCorrectValue()
    {
        // arrange
        var parser = new Parser();
        var expr = parser.ParseExpression("e => ({\"a\":\"chunk\",\"b\":\"chunk\"}[e] + \".js\")");

        // act
        var result = expr.Walk<Literal>().ToList();

        // assert
        Assert.Equal(5, result.Count);
    }
}
