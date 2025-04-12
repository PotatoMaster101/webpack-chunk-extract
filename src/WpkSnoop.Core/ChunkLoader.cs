using System.Dynamic;
using Acornima;
using Acornima.Ast;
using Jint;
using WpkSnoop.Core.Extensions;
using WpkSnoop.Core.Extractors;

namespace WpkSnoop.Core;

/// <summary>
/// Represents a chunk loader.
/// </summary>
/// <param name="Node">The chunk loader root node.</param>
/// <param name="Assignments">The JS script assignments.</param>
public record ChunkLoader(Node Node, IReadOnlyDictionary<string, AssignmentExpression> Assignments)
{
    /// <summary>
    /// Returns the collection of <see cref="ChunkId"/> in this loader.
    /// </summary>
    /// <returns>The collection of <see cref="ChunkId"/> in this loader.</returns>
    public IEnumerable<ChunkId> GetIds()
    {
        var extractors = new IExtractor[]
        {
            new BinaryExpressionExtractor(),
            new ArrayExpressionExtractor(),
            new ObjectExpressionExtractor(),
            new SwitchStatementExtractor()
        };

        var result = new HashSet<ChunkId>();
        foreach (var child in Node.Walk())
        {
            foreach (var extractor in extractors)
                if (extractor.CanExtract(child))
                    result.UnionWith(extractor.Extract(child));
        }
        return result;
    }

    /// <summary>
    /// Executes this loader and returns the collection of <see cref="ChunkEntry"/>.
    /// </summary>
    /// <param name="ids">The input <see cref="ChunkId"/>.</param>
    /// <returns>The collection of <see cref="ChunkEntry"/>.</returns>
    public IEnumerable<ChunkEntry> Execute(IEnumerable<ChunkId> ids)
    {
        using var engine = new Engine();
        var result = new HashSet<ChunkEntry>();
        try
        {
            DeclareVariables(engine);
            foreach (var id in ids)
            {
                var eval = engine.Evaluate($"({Node.ToJavaScript()})({id});").ToString();
                if (eval != "undefined")
                    result.Add(new ChunkEntry(id, eval));
            }
            return result;
        }
        catch
        {
            return result;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Node.ToJavaScript(true);
    }

    /// <summary>
    /// Creates the necessary variables required for the loader to run.
    /// </summary>
    /// <param name="engine">The JavaScript engine.</param>
    private void DeclareVariables(Engine engine)
    {
        var seen = new HashSet<string>();
        foreach (var member in Node.Walk<MemberExpression>())
        {
            if (member.Object is not Identifier obj)
                continue;
            if (!Assignments.TryGetValue(member.ToJavaScript(), out var assignment))
                continue;

            if (seen.Add(obj.Name))
                engine.SetValue(obj.Name, new ExpandoObject());
            engine.Execute(assignment.ToJavaScript());
        }
    }
}
