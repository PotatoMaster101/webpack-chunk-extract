using Acornima;
using Acornima.Ast;

namespace WpkSnoop.Core.Extractors;

/// <summary>
/// Represents an extractor that can extract <see cref="ChunkId"/> from binary expressions.
/// For example: <c>e === 1</c>.
/// </summary>
public class BinaryExpressionExtractor : IExtractor
{
    /// <inheritdoc />
    public bool CanExtract(Node node)
    {
        return node is BinaryExpression { Operator: Operator.Equality or Operator.StrictEquality };
    }

    /// <inheritdoc />
    public IEnumerable<ChunkId> Extract(Node node)
    {
        if (!CanExtract(node))
            yield break;

        var bin = (BinaryExpression)node;
        if (bin is { Left: Literal { Value: not null } left, Right: Identifier })
            yield return new ChunkId(left.Value.ToString()!, left.Kind is TokenKind.StringLiteral);
        else if (bin is { Left: Identifier, Right: Literal { Value: not null } right })
            yield return new ChunkId(right.Value.ToString()!, right.Kind is TokenKind.StringLiteral);
    }
}
