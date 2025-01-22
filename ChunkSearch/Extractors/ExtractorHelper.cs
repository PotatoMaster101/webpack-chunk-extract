using Acornima.Ast;
using ChunkSearch.Chunk;
using ChunkSearch.Extensions;

namespace ChunkSearch.Extractors;

/// <summary>
/// Helper class for chunk ID extraction.
/// </summary>
public static class ExtractorHelper
{
    /// <summary>
    /// Extracts all chunk IDs from a node.
    /// </summary>
    /// <param name="node">The node to extract.</param>
    /// <returns>The chunk IDs extracted.</returns>
    public static IEnumerable<ChunkId> ExtractAll(Node node)
    {
        var extractors = new IExtractor[]
        {
            new BinaryExpressionExtractor(),
            new ArrayExpressionExtractor(),
            new ObjectExpressionExtractor(),
            new SwitchStatementExtractor()
        };

        var result = new HashSet<ChunkId>();
        foreach (var child in node.Walk())
        {
            foreach (var extractor in extractors)
                if (extractor.CanExtract(child))
                    result.UnionWith(extractor.Extract(child));
        }
        return result;
    }
}
