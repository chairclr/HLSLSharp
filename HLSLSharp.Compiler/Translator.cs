using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Compiler;

public abstract class Translator
{
    protected readonly SyntaxTree SyntaxTree;

    protected readonly CompilationUnitSyntax CompilationUnit;

    protected readonly CSharpCompilation Compilation;

    protected readonly SemanticModel SemanticModel;

    protected readonly CoreLibProvider CoreLib;

    public Translator(SyntaxTree syntaxTree)
    {
        CoreLib = new CoreLibProvider();

        SyntaxTree = syntaxTree;

        CompilationUnit = syntaxTree.GetCompilationUnitRoot();

        Compilation = CSharpCompilation.Create($"__Translation")
            .AddReferences(CoreLib.Reference)
            .AddSyntaxTrees(SyntaxTree);

        SemanticModel = Compilation.GetSemanticModel(SyntaxTree);
    }

    public EmitResult Emit()
    {
        return new EmitResult(null, SemanticModel.GetDiagnostics(), SemanticModel.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error));
    }
}
