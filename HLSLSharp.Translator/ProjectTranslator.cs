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


    }

    public ProjectTranslator(SyntaxTree singleTree)
    {
        Compilation = CSharpCompilation.Create($"__Translation")
            .AddReferences(CoreLibProvider.Reference)
            .AddSyntaxTrees(singleTree);
    }

    protected void ReportDiagnostic(Diagnostic diagnostic)
    {
        Diagnostics.Add(diagnostic);
    }
}
