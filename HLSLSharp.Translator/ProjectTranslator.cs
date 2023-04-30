using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HLSLSharp.Compiler;
using HLSLSharp.Compiler.Generators;
using HLSLSharp.Compiler.Generators.Internal.Compute;
using HLSLSharp.Compiler.Generators.Internal.Vectors;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Translator;

public abstract class ProjectTranslator
{
    private static readonly string ComputeShaderAttributeFullName = "HLSLSharp.CoreLib.Shaders.ComputeShaderAttribute";

    protected CSharpCompilation Compilation;

    private readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();

    internal virtual IEnumerable<IInternalProjectGenerator> InternalGenerators => new IInternalProjectGenerator[] { new AliasGenerator() };

    internal List<InternalGeneratorSource> InternalGeneratedSources = new List<InternalGeneratorSource>();

    internal List<ShaderTranslator> ShaderTranslators = new List<ShaderTranslator>();

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

        InitializeShaderTranslators();

        GeneratePerShaderSource();

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

        InitializeShaderTranslators();

        GeneratePerShaderSource();

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

    private void InitializeShaderTranslators()
    {
        INamedTypeSymbol computeShaderAttributeSymbol = Compilation.GetTypeByMetadataName(ComputeShaderAttributeFullName)!;

        IEnumerable<StructDeclarationSyntax> structNodes = Compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes().OfType<StructDeclarationSyntax>());

        IEnumerable<StructDeclarationSyntax> computeStructNodes = structNodes.Where(node =>
            Compilation.GetSemanticModel(node.SyntaxTree).GetDeclaredSymbol(node)!.GetAttributes()
            .Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, computeShaderAttributeSymbol)));

        List<INamedTypeSymbol> computeShaderTypes = computeStructNodes
            .Select(x => Compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x)!)
            .ToList();

        foreach (INamedTypeSymbol shaderType in computeShaderTypes)
        {
            ShaderTranslator shaderTranslator = new ShaderTranslator(Compilation, shaderType);

            ShaderTranslators.Add(shaderTranslator);
        }
    }

    private void GeneratePerShaderSource()
    {
        foreach (ShaderTranslator translator in ShaderTranslators)
        {
            foreach (IInternalShaderGenerator generator in translator.ShaderGenerators)
            {
                InternalShaderGenerationContext context = new InternalShaderGenerationContext(Compilation, translator.ShaderType);

                generator.Execute(context);

                InternalGeneratedSources.AddRange(context.AdditionalSources);

                foreach (Diagnostic diagnostic in context.Diagnostics)
                {
                    ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
