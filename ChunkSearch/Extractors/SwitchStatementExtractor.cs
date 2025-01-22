using Acornima;
using Acornima.Ast;
using ChunkSearch.Chunk;

namespace ChunkSearch.Extractors;

/// <summary>
/// Represents an extractor that can extract chunk IDs from switch statements.
/// For example: <c>switch(e) { case 1: return "chunk"; }</c>.
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
