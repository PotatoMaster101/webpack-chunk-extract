using Acornima;
using Acornima.Ast;
using WpkSnoop.Core.Extensions;

namespace WpkSnoop.Core.Test.Extensions;

/// <summary>
/// Unit tests for <see cref="Core.Extensions.NodeExtensions"/>.
/// </summary>
public class NodeExtensionsTest
{
    [Fact]
    public void IsJsLiteral_ReturnsCorrectValue()
    {
        // arrange
        var parser = new Parser();
        var expr = parser.ParseExpression("e => ({\"a\":\"chunk1\",2:\"chunk2\"}[e] + \".js\")");
        var literals = expr.Walk<Literal>().ToList();

        // assert
        Assert.False(literals[0].IsJsLiteral());
        Assert.False(literals[1].IsJsLiteral());
        Assert.False(literals[2].IsJsLiteral());
        Assert.False(literals[3].IsJsLiteral());
        Assert.True(literals[4].IsJsLiteral());
    }

    [Fact]
    public void Walk_ReturnsCorrectValue()
    {
        // arrange
        var parser = new Parser();
        var expr = parser.ParseExpression("e => ({\"a\":\"chunk1\",\"b\":\"chunk2\"}[e] + \".js\")");

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
        var expr = parser.ParseExpression("e => ({\"a\":\"chunk1\",\"b\":\"chunk2\"}[e] + \".js\")");

        // act
        var result = expr.Walk<Literal>().ToList();

        // assert
        Assert.Equal(5, result.Count);
        Assert.Equal("a", result[0].Value);
        Assert.Equal("chunk1", result[1].Value);
        Assert.Equal("b", result[2].Value);
        Assert.Equal("chunk2", result[3].Value);
        Assert.Equal(".js", result[4].Value);
    }
}
