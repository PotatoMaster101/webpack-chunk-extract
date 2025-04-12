using Acornima;
using Acornima.Ast;
using WpkSnoop.Core.Extensions;

namespace WpkSnoop.Core.Extractors;

/// <summary>
/// Represents an extractor that can extract <see cref="ChunkId"/> from object expressions.
/// For example: <c>{ 1: "chunk1.js", 2: "chunk2.js" }</c>.
/// </summary>
public class ObjectExpressionExtractor : IExtractor
{
    /// <inheritdoc />
    public bool CanExtract(Node node)
    {
        return node is ObjectExpression;
    }

    /// <inheritdoc />
    public IEnumerable<ChunkId> Extract(Node node)
    {
        if (!CanExtract(node))
            yield break;

        foreach (var prop in node.Walk<Property>())
        {
            if (prop.Key is Identifier identifier)
                yield return new ChunkId(identifier.Name);
            else if (prop.Key is Literal { Value: not null } literal)
                yield return new ChunkId(literal.Value.ToString()!, literal.Kind is TokenKind.StringLiteral);
        }
    }
}
