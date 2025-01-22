using Acornima.Ast;
using ChunkSearch.Chunk;

namespace ChunkSearch.Extractors;

/// <summary>
/// Represents a chunk ID extractor.
/// </summary>
public interface IExtractor
{
    /// <summary>
    /// Determines whether this extractor can extract from the specified node.
    /// </summary>
    /// <param name="node">The node to check.</param>
    /// <returns>Whether this extractor can extract from <paramref name="node"/>.</returns>
    bool CanExtract(Node node);

    /// <summary>
    /// Returns the chunk IDs extracted.
    /// </summary>
    /// <param name="node">The node to extract.</param>
    /// <returns>The extraction result.</returns>
    IEnumerable<ChunkId> Extract(Node node);
}
