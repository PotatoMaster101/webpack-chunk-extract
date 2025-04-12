using Acornima;
using Acornima.Ast;

namespace WpkSnoop.Core.Extractors;

/// <summary>
/// Represents an extractor that can extract <see cref="ChunkId"/> from switch statements.
/// For example: <c>switch(e) { case 1: return "chunk1"; case 2: return "chunk2"; }</c>.
/// </summary>
public class SwitchStatementExtractor : IExtractor
{
    /// <inheritdoc />
    public bool CanExtract(Node node)
    {
        return node is SwitchStatement;
    }

    /// <inheritdoc />
    public IEnumerable<ChunkId> Extract(Node node)
    {
        if (!CanExtract(node))
            yield break;

        var statement = (SwitchStatement)node;
        foreach (var c in statement.Cases)
        {
            if (c.Test is Literal { Value: not null } literal)
                yield return new ChunkId(literal.Value.ToString()!, literal.Kind is TokenKind.StringLiteral);
        }
    }
}
