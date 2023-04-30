using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HLSLSharp.Compiler.Generators;
using HLSLSharp.Compiler.Generators.Internal.Compute;
using HLSLSharp.Compiler.Generators.Internal.Vectors;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HLSLSharp.Translator;

public abstract class ProjectTranslator
{
    protected CSharpCompilation Compilation;

    private readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();

    internal virtual IEnumerable<IInternalProjectGenerator> InternalGenerators => new IInternalProjectGenerator[] { new AliasGenerator() };

    internal List<InternalGeneratorSource> InternalGeneratedSources = new List<InternalGeneratorSource>();

    public ProjectTranslator(CSharpCompilation compilation)
    {
        Compilation = compilation;
        
        IEnumerable<MetadataReference> containsAssemblyReference = Compilation.References.Where(x => x.Display?.EndsWith($"{System.IO.Path.DirectorySeparatorChar}HLSLSharp.CoreLib.dll") ?? false);

        if (!Compilation.References.Contains(CoreLibProvider.Reference) && !containsAssemblyReference.Any())
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.MissingOrInvalidCoreLibReference, null));
        }

        if (containsAssemblyReference.Any())
        {
            Compilation = Compilation.ReplaceReference(containsAssemblyReference.Single(), CoreLibProvider.Reference);
        }

        GenerateProjectSource();

        Compilation = Compilation
            .AddSyntaxTrees(InternalGeneratedSources.Select(x => x.SyntaxTree));

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

        GenerateProjectSource();

        Compilation = Compilation
            .AddSyntaxTrees(InternalGeneratedSources.Select(x => x.SyntaxTree));

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
        return new ProjectEmitResult(new List<Compiler.ShaderEmitResult>(), Diagnostics);
    }

    private void GenerateProjectSource()
    {
        foreach (IInternalProjectGenerator generator in InternalGenerators)
        {
            InternalProjectGenerationContext context = new InternalProjectGenerationContext(Compilation);

            generator.Execute(context);

            InternalGeneratedSources.AddRange(context.AdditionalSources);

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                ReportDiagnostic(diagnostic);
            }
        }
    }
}
