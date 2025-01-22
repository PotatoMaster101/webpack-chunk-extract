using Acornima.Ast;
using ChunkSearch.Chunk;
using ChunkSearch.Extensions;

namespace ChunkSearch.Extractors;

/// <summary>
/// Represents an extractor that can extract chunk IDs from array expressions.
/// For example: <c>["chunk"][e]</c>.
/// </summary>
public class ArrayExpressionExtractor : IExtractor
{
    /// <inheritdoc />
    public bool CanExtract(Node node)
    {
        return node is ArrayExpression;
    }

    /// <inheritdoc />
    public IEnumerable<ChunkId> Extract(Node node)
    {
        if (!CanExtract(node))
            yield break;

        var idx = 0;
        foreach (var literal in node.Walk<Literal>())
        {
            if (literal.Value is not null)
                yield return new ChunkId(idx.ToString(), false);
            idx++;
        }
    }
}
