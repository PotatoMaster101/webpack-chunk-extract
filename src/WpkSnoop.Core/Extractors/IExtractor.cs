using Acornima.Ast;

namespace WpkSnoop.Core.Extractors;

/// <summary>
/// Represents an extractor for <see cref="ChunkId"/>.
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
    /// Returns the collection of <see cref="ChunkId"/> extracted.
    /// </summary>
    /// <param name="node">The node to extract.</param>
    /// <returns>The extracted <see cref="ChunkId"/>.</returns>
    IEnumerable<ChunkId> Extract(Node node);
}
