namespace ChunkSearch.Chunk;

/// <summary>
/// Represents an entry in a chunk loader.
/// </summary>
/// <param name="chunkId">The ID of the entry.</param>
/// <param name="chunkFile">The chunk filename.</param>
public class ChunkEntry(ChunkId chunkId, string chunkFile)
{
    /// <summary>
    /// Gets the chunk ID.
    /// </summary>
    public ChunkId ChunkId { get; } = chunkId;

    /// <summary>
    /// Gets the chunk filename.
    /// </summary>
    public string ChunkFile { get; } = chunkFile;

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        return $"{ChunkId}: {ChunkFile}";
    }
}
