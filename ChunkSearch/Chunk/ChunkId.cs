namespace ChunkSearch.Chunk;

/// <summary>
/// Represents the ID of a chunk loader entry.
/// </summary>
/// <param name="content">The ID value.</param>
/// <param name="quote">Whether the ID is a string, hence added quote.</param>
public class ChunkId(string content, bool quote = true)
{
    /// <summary>
    /// Gets the content of the ID.
    /// </summary>
    public string Content { get; } = content;

    /// <summary>
    /// Gets whether the ID is a string, hence added quote.
    /// </summary>
    public bool Quote { get; } = quote;

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        return Quote ? $"\"{Content}\"" : Content;
    }

    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? obj)
    {
        return obj is ChunkId chunkId && Content == chunkId.Content;
    }

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode()
    {
        return Content.GetHashCode();
    }
}
