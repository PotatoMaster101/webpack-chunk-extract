using System.Dynamic;
using Acornima;
using Acornima.Ast;
using ChunkSearch.Extensions;
using Jint;

namespace ChunkSearch.Chunk;

/// <summary>
/// Represents a chunk loader.
/// </summary>
public class Loader
{
    private readonly Node _node;
    private readonly IReadOnlyDictionary<string, AssignmentExpression> _nonFuncAssignments;

    /// <summary>
    /// Constructs a new <see cref="Loader"/>.
    /// </summary>
    /// <param name="node">The node containing the loader.</param>
    /// <param name="nonFuncAssignments">The map containing the non-function assignments.</param>
    public Loader(Node node, IReadOnlyDictionary<string, AssignmentExpression> nonFuncAssignments)
    {
        _node = node;
        _nonFuncAssignments = nonFuncAssignments;
    }

    /// <summary>
    /// Gets a list of chunk IDs.
    /// </summary>
    /// <returns>The list of chunk IDs.</returns>
    public IReadOnlyCollection<ChunkId> GetChunkIds()
    {
        var result = new HashSet<ChunkId>();
        var objs = new List<ObjectExpression>();
        var bins = new List<BinaryExpression>();
        var cases = new List<SwitchCase>();
        foreach (var child in _node.Walk())
        {
            if (child is ObjectExpression obj) objs.Add(obj);
            else if (child is BinaryExpression bin) bins.Add(bin);
            else if (child is SwitchCase switchCase) cases.Add(switchCase);
        }

        if (bins.Count > 0) result.UnionWith(GetChunkIdsFromComparison(bins));
        if (cases.Count > 0) result.UnionWith(GetChunkIdsFromCase(cases));
        if (objs.Count > 0) result.UnionWith(GetChunkIdsFromMap(objs));
        return result;
    }

    /// <summary>
    /// Executes the chunk loader and returns the result.
    /// </summary>
    /// <param name="args">The arguments for the chunk loader.</param>
    /// <returns>The result of execution.</returns>
    public IReadOnlyCollection<ChunkEntry> Run(IEnumerable<ChunkId> args)
    {
        using var engine = new Engine();
        var result = new HashSet<ChunkEntry>();
        try
        {
            DeclareVariables(engine);
            foreach (var arg in args)
            {
                var eval = engine.Evaluate($"({_node.ToJavaScript()})({arg});").ToString();
                if (eval != "undefined")
                    result.Add(new ChunkEntry(arg, eval));
            }
            return result;
        }
        catch
        {
            return result;
        }
    }

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString()
    {
        return _node.ToJavaScript(true);
    }

    /// <summary>
    /// Creates the necessary variables required for the loader to run.
    /// </summary>
    /// <param name="engine">The JavaScript engine.</param>
    private void DeclareVariables(Engine engine)
    {
        var seen = new HashSet<string>();
        foreach (var member in _node.Walk<MemberExpression>())
        {
            if (member.Object is not Identifier obj)
                continue;
            if (!_nonFuncAssignments.TryGetValue(member.ToJavaScript(), out var assignment))
                continue;

            if (seen.Add(obj.Name))
                engine.SetValue(obj.Name, new ExpandoObject());
            engine.Execute(assignment.ToJavaScript());
        }
    }

    /// <summary>
    /// Returns the chunk IDs that are inside object maps, for example <c>{1:'chunk'}[e] + '.js'</c>.
    /// </summary>
    /// <param name="nodes">The object map nodes.</param>
    /// <returns>Chunk IDs that are inside an object maps.</returns>
    private static HashSet<ChunkId> GetChunkIdsFromMap(IEnumerable<ObjectExpression> nodes)
    {
        var result = new HashSet<ChunkId>();
        foreach (var prop in nodes.SelectMany(x => x.Walk<Property>()))
        {
            if (prop.Key is Identifier identifier)
                result.Add(new ChunkId(identifier.Name));
            else if (prop.Key is Literal { Value: not null } literal)
                result.Add(new ChunkId(literal.Value.ToString()!, literal.Kind is TokenKind.StringLiteral));
        }
        return result;
    }

    /// <summary>
    /// Returns the chunk IDs that are inside comparisons such as <c>if</c> and ternary.
    /// For example <c>if(e===1)return 'chunk.js'</c>.
    /// </summary>
    /// <param name="nodes">The comparison nodes.</param>
    /// <returns>Chunk IDs that are inside comparisons.</returns>
    private static HashSet<ChunkId> GetChunkIdsFromComparison(IEnumerable<BinaryExpression> nodes)
    {
        var result = new HashSet<ChunkId>();
        foreach (var node in nodes)
        {
            if (node.Operator != Operator.Equality && node.Operator != Operator.StrictEquality)
                continue;

            if (node is { Left: Literal { Value: not null } left, Right: Identifier })
                result.Add(new ChunkId(left.Value.ToString()!, left.Kind is TokenKind.StringLiteral));
            else if (node is { Left: Identifier, Right: Literal { Value: not null } right })
                result.Add(new ChunkId(right.Value.ToString()!, right.Kind is TokenKind.StringLiteral));
        }
        return result;
    }

    /// <summary>
    /// Returns the chunk IDs that are inside <c>switch</c>. For example <c>switch(e) {case 1: return 'chunk.js'}</c>.
    /// </summary>
    /// <param name="nodes">The switch case nodes.</param>
    /// <returns>Chunk IDs that are inside switch statements.</returns>
    private static HashSet<ChunkId> GetChunkIdsFromCase(IEnumerable<SwitchCase> nodes)
    {
        var result = new HashSet<ChunkId>();
        foreach (var node in nodes)
        {
            if (node.Test is Literal { Value: not null } literal)
                result.Add(new ChunkId(literal.Value.ToString()!, literal.Kind is TokenKind.StringLiteral));
        }
        return result;
    }
}
