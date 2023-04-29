using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using HLSLSharp.Compiler.Emit;
using HLSLSharp.Compiler.Generators;
using HLSLSharp.Compiler.Generators.Internal.Vectors;
using HLSLSharp.Compiler.SyntaxRewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace HLSLSharp.Compiler;

public class Translator
{
    protected CSharpCompilation Compilation;

    public SyntaxTree ShaderSyntaxTree { get; protected set; }

    protected CompilationUnitSyntax ShaderCompilationUnit;

    protected SemanticModel ShaderSemanticModel;

    internal readonly List<SyntaxNode> NodesAddedToShaderStruct = new List<SyntaxNode>();

    internal IEnumerable<(string, SourceText)> InternalGeneratedSourceText = Enumerable.Empty<(string, SourceText)>();

    public Translator(SyntaxTree shaderSyntaxTree)
    {
        ShaderSyntaxTree = shaderSyntaxTree;

        ShaderCompilationUnit = ShaderSyntaxTree.GetCompilationUnitRoot();

        Compilation = CSharpCompilation.Create($"__Translation")
            .AddReferences(CoreLibProvider.Reference)
            .AddSyntaxTrees(ShaderSyntaxTree);

        ShaderSemanticModel = Compilation.GetSemanticModel(ShaderSyntaxTree);

        RewriteSource();

        ShaderCompilationUnit = ShaderSyntaxTree.GetCompilationUnitRoot();

        ShaderSemanticModel = Compilation.GetSemanticModel(ShaderSyntaxTree);
    }

    private void RewriteSource()
    {
        SyntaxNode root = ShaderSyntaxTree.GetRoot();

        ComputeRewriter computeRewriter = new ComputeRewriter(ShaderSemanticModel);

        root = computeRewriter.Visit(root);

        NodesAddedToShaderStruct.AddRange(computeRewriter.AddedNodes);

        SyntaxTree oldTree = ShaderSyntaxTree;

        ShaderSyntaxTree = ShaderSyntaxTree.WithRootAndOptions(root, ShaderSyntaxTree.Options);

        IEnumerable<(SyntaxTree, (string, SourceText))> generated = GenerateInternalSource();

        Compilation = Compilation.ReplaceSyntaxTree(oldTree, ShaderSyntaxTree)
            .AddSyntaxTrees(generated.Select(x => x.Item1));

        InternalGeneratedSourceText = generated.Select(x => x.Item2);
    }

    private IEnumerable<(SyntaxTree, (string, SourceText))> GenerateInternalSource()
    {
        IEnumerable<(SyntaxTree, (string, SourceText))> newTrees = Enumerable.Empty<(SyntaxTree, (string, SourceText))>();

        newTrees = newTrees.Concat(ApplyGeneration<AliasGenerator>());

        return newTrees;
    }

    private IEnumerable<(SyntaxTree, (string, SourceText))> ApplyGeneration<T>() where T : IInternalGenerator, new()
    {
        IInternalGenerator generator = new T();

        InternalGenerationContext context = new InternalGenerationContext(Compilation);

        generator.Execute(context);

        return context.AdditionalSources.Select(x => (x.Value.Item1, (x.Key, x.Value.Item2)));
    }

    public EmitResult Emit()
    {
        HLSLEmitter emitter = new HLSLEmitter(Compilation, ShaderSyntaxTree, ShaderCompilationUnit, ShaderSemanticModel);

        emitter.EmitHLSLSource();

        return new EmitResult(emitter.GetSource(), ShaderSemanticModel.GetDiagnostics(), ShaderSemanticModel.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error));
    }
}
