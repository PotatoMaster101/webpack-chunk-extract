namespace WpkSnoop.Core;

/// <summary>
/// Represents the ID of a chunk loader entry.
/// </summary>
/// <param name="Content">The ID value.</param>
/// <param name="Quote">Whether the ID is a string, hence added quote.</param>
public readonly record struct ChunkId(string Content, bool Quote = true)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return Quote ? $"\"{Content}\"" : Content;
    }
}
