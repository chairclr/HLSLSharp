using System.Collections.Generic;
using System.Linq;
using HLSLSharp.Compiler.Emit;
using HLSLSharp.Compiler.SyntaxRewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Compiler;

public class Translator
{
    public SyntaxTree SyntaxTree { get; protected set; }

    protected CompilationUnitSyntax CompilationUnit;

    protected CSharpCompilation Compilation;

    protected SemanticModel SemanticModel;

    protected readonly CoreLibProvider CoreLib;

    public readonly List<SyntaxNode> NodesAddedToStruct = new List<SyntaxNode>();

#pragma warning disable CS8618
    public Translator(SyntaxTree syntaxTree)
#pragma warning restore CS8618
    {
        CoreLib = new CoreLibProvider();

        SyntaxTree = syntaxTree;

        CompilationUnit = SyntaxTree.GetCompilationUnitRoot();

        CreateSemanticModel();

        RewriteSource();

        CompilationUnit = SyntaxTree.GetCompilationUnitRoot();

        CreateSemanticModel();
    }

    private void RewriteSource()
    {
        SyntaxNode root = SyntaxTree.GetRoot();

        ComputeRewriter computeRewriter = new ComputeRewriter(SemanticModel);

        root = computeRewriter.Visit(root);

        NodesAddedToStruct.AddRange(computeRewriter.AddedNodes);

        SyntaxTree = SyntaxTree.WithRootAndOptions(root, SyntaxTree.Options);
    }

    private void CreateSemanticModel()
    {
        Compilation = CSharpCompilation.Create($"__Translation")
            .AddReferences(CoreLib.Reference)
            .AddSyntaxTrees(SyntaxTree);

        SemanticModel = Compilation.GetSemanticModel(SyntaxTree);
    }

    public EmitResult Emit()
    {
        HLSLEmitter emitter = new HLSLEmitter(SyntaxTree, CompilationUnit, Compilation, SemanticModel);

        emitter.EmitHLSLSource();

        return new EmitResult(emitter.GetSource(), SemanticModel.GetDiagnostics(), SemanticModel.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error));
    }
}
