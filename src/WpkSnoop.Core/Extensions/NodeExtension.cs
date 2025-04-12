using Acornima.Ast;

namespace WpkSnoop.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="Node"/>.
/// </summary>
public static class NodeExtensions
{
    /// <summary>
    /// Determines whether the node is a function.
    /// </summary>
    /// <param name="node">The node to test.</param>
    /// <returns>Whether the node is a function.</returns>
    public static bool IsFunction(this Node node)
    {
        return node is ArrowFunctionExpression or FunctionExpression or FunctionDeclaration;
    }

    /// <summary>
    /// Determines whether the node is a loop.
    /// </summary>
    /// <param name="node">The node to test.</param>
    /// <returns>Whether the node is a loop.</returns>
    public static bool IsLoop(this Node node)
    {
        return node is ForStatement or ForOfStatement or WhileStatement or DoWhileStatement;
    }

    /// <summary>
    /// Determines whether the node is a <see cref="Literal"/> containing a JS filename.
    /// </summary>
    /// <param name="node">The node to test.</param>
    /// <returns>Whether the node is a <see cref="Literal"/> containing a JS filename.</returns>
    public static bool IsJsLiteral(this Node node)
    {
        return node is Literal { Value: string str } && str.EndsWith(".js") && !str.Contains('/');
    }

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
    public static IEnumerable<T> Walk<T>(this Node node) where T : Node
    {
        return node.Walk().OfType<T>();
    }
}
