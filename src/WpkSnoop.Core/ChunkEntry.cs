namespace WpkSnoop.Core;

/// <summary>
/// Represents an entry in a chunk loader.
/// </summary>
/// <param name="ChunkId">The ID of the entry.</param>
/// <param name="ChunkFile">The chunk filename.</param>
public record ChunkEntry(ChunkId ChunkId, string ChunkFile)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{ChunkId}: {ChunkFile}";
    }
}
