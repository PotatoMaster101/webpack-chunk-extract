using Acornima;
using Acornima.Ast;
using WpkSnoop.Core.Extensions;

namespace WpkSnoop.Core;

/// <summary>
/// Helper methods for chunk loader detection.
/// </summary>
public static class ChunkLoaderHelper
{
    /// <summary>
    /// Determines whether the node might be a chunk loader.
    /// </summary>
    /// <param name="node">The node to check.</param>
    /// <returns>Whether the node might be a chunk loader.</returns>
    public static bool IsChunkLoader(Node node)
    {
        if (!node.IsFunction())
            return false;
        if (((IFunction)node).Params.Count is not 1)
            return false;

        var hasJsLiteral = false;
        foreach (var child in node.Walk())
        {
            if (child.IsLoop() || child is CallExpression)
                return false;
            if (child.IsJsLiteral())
                hasJsLiteral = true;
        }
        return hasJsLiteral;
    }

    /// <summary>
    /// Gets a collection of <see cref="ChunkLoader"/> from a root node.
    /// </summary>
    /// <param name="root">The root node to get the <see cref="ChunkLoader"/>.</param>
    /// <returns>The collection of <see cref="ChunkLoader"/>.</returns>
    public static IEnumerable<ChunkLoader> GetChunkLoaders(Node root)
    {
        var loaders = new List<Node>();
        var assignments = new Dictionary<string, AssignmentExpression>();
        foreach (var node in root.Walk())
        {
            if (IsChunkLoader(node))
                loaders.Add(node);

            if (node is not AssignmentExpression { Operator: Operator.Assignment } assignment)
                continue;
            if (assignment.Left is not MemberExpression)
                continue;
            if (assignment.Right is FunctionExpression or ArrowFunctionExpression)
                continue;

            assignments[assignment.Left.ToJavaScript()] = assignment;
        }
        return loaders.Select(x => new ChunkLoader(x, assignments));
    }

    /// <summary>
    /// Gets a collection of <see cref="ChunkLoader"/> from JS content.
    /// </summary>
    /// <param name="script">The JS content.</param>
    /// <returns>The collection of <see cref="ChunkLoader"/>.</returns>
    public static IEnumerable<ChunkLoader> GetChunkLoaders(string script)
    {
        var parser = new Parser();
        var module = parser.ParseModule(script);
        return GetChunkLoaders(module);
    }
}
