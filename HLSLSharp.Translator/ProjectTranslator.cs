using System;
using System.Collections.Concurrent;
using System.Linq;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HLSLSharp.Translator;

public abstract class ProjectTranslator
{
    protected CSharpCompilation Compilation;

    private readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();



    public ProjectTranslator(CSharpCompilation compilation)
    {
        Compilation = compilation;

        if (!Compilation.References.Contains(CoreLibProvider.Reference))
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.MissingOrInvalidCoreLibReference, null));
        }

        foreach (Diagnostic diagnostic in Compilation.GetDiagnostics())
        {
            ReportDiagnostic(diagnostic);
        }
    }

    public ProjectTranslator(SyntaxTree singleTree)
    {
        Compilation = CSharpCompilation.Create($"__Translation", options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(SystemReferenceProvider.References)
            .AddReferences(CoreLibProvider.Reference)
            .AddSyntaxTrees(singleTree);

        foreach (Diagnostic diagnostic in Compilation.GetDiagnostics())
        {
            ReportDiagnostic(diagnostic);
        }
    }

    protected void ReportDiagnostic(Diagnostic diagnostic)
    {
        Diagnostics.Add(diagnostic);
    }

    public ProjectEmitResult Emit()
    {
        return new ProjectEmitResult(new System.Collections.Generic.List<Compiler.ShaderEmitResult>(), Diagnostics);
    }
}
