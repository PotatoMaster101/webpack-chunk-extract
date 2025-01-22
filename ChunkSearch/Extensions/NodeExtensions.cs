using System.Runtime.CompilerServices;
using Acornima;
using Acornima.Ast;
using ChunkSearch.Chunk;

namespace ChunkSearch.Extensions;

/// <summary>
/// Extension methods for <see cref="Node"/>.
/// </summary>
public static class NodeExtensions
{
    /// <summary>
    /// Walks the node, visiting itself and all child nodes.
    /// </summary>
    /// <param name="node">The node to start the walk.</param>
    /// <returns>A collection of nodes visited.</returns>
    public static IEnumerable<Node> Walk(this Node node)
    {
        yield return node;
        foreach (var child in node.ChildNodes.SelectMany(Walk))
            yield return child;
    }

    /// <summary>
    /// Walks the node, visiting itself and all child nodes. Returns only the node of a specific type.
    /// </summary>
    /// <param name="node">The node to start the walk.</param>
    /// <typeparam name="T">The type of the node.</typeparam>
    /// <returns>A collection of specific type nodes visited.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Walk<T>(this Node node) where T : Node
    {
        return node.Walk().OfType<T>();
    }

    /// <summary>
    /// Determines whether the node is likely a chunk loader.
    /// </summary>
    /// <param name="node">The node to check.</param>
    /// <returns>Whether the node is likely a chunk loader.</returns>
    public static bool IsChunkLoader(this Node node)
    {
        if (node is not FunctionExpression && node is not ArrowFunctionExpression)
            return false;
        if (((IFunction)node).Params.Count != 1)
            return false;

        var hasJsLiteral = false;
        foreach (var child in node.Walk())
        {
            if (child is ForStatement or ForOfStatement or WhileStatement or DoWhileStatement or CallExpression)
                return false;
            if (child is Literal { Value: string value } && value.EndsWith(".js") && !value.Contains('/'))
                hasJsLiteral = true;
        }
        return hasJsLiteral;
    }

    /// <summary>
    /// Gets a list of chunk loaders from a root node.
    /// </summary>
    /// <param name="root">The root node to get the chunk loaders.</param>
    /// <returns>The list of chunk loaders.</returns>
    public static IEnumerable<Loader> GetLoaders(this Node root)
    {
        var loaderNodes = new List<Node>();
        var nonFuncAssigns = new Dictionary<string, AssignmentExpression>();
        foreach (var node in root.Walk())
        {
            if (node.IsChunkLoader())
                loaderNodes.Add(node);

            if (node is not AssignmentExpression { Operator: Operator.Assignment } assignment)
                continue;
            if (assignment.Left is not MemberExpression)
                continue;
            if (assignment.Right is FunctionExpression or ArrowFunctionExpression)
                continue;

            nonFuncAssigns[assignment.Left.ToJavaScript()] = assignment;
        }
        return loaderNodes.Select(x => new Loader(x, nonFuncAssigns));
    }
}
