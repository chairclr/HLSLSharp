using HLSLSharp.Compiler.Generators;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using HLSLSharp.Compiler.Generators.Internal.Compute;
using System.Collections.Immutable;
using System.Text;
using HLSLSharp.Compiler.Emit;
using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using HLSLSharp.Translator.Diagnostics;
using System.Linq;
using System;
using HLSLSharp.Translator.Emit.Emitters;

namespace HLSLSharp.Compiler;

internal class ShaderTranslator
{
    private static readonly string KernelAttributeFullName = "HLSLSharp.CoreLib.Shaders.KernelAttribute";

    public readonly Compilation Compilation;

    public readonly INamedTypeSymbol ShaderType;

    public readonly IMethodSymbol ShaderKernelMethod;

    public readonly MethodDeclarationSyntax KernelBodyDeclaration;

    public readonly SyntaxTree KernelBodySyntaxTree;

    public readonly SemanticModel KernelBodySemanticModel;

    public IEnumerable<IInternalShaderGenerator> ShaderGenerators => new IInternalShaderGenerator[] { new ComputeGenerator() };

    public readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();

    private readonly List<HLSLEmitter> ShaderEmitters;

    public ShaderTranslator(Compilation compilation, INamedTypeSymbol shaderType)
    {
        Compilation = compilation;
        ShaderType = shaderType;

        INamedTypeSymbol kernelAttributeSymbol = Compilation.GetTypeByMetadataName(KernelAttributeFullName)!;

        IMethodSymbol[] kernelMethods = ShaderType.GetMembers()
            .Where(x => x.Kind == SymbolKind.Method)
            .Cast<IMethodSymbol>()
            .Where(x => x.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, kernelAttributeSymbol)))
            .ToArray();

        if (kernelMethods.Length > 1)
        {
            foreach (IMethodSymbol methodSymbol in kernelMethods)
            {
                foreach (Location location in methodSymbol.Locations)
                {
                    ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.MoreThanOneKernel, location));
                }
            }
        }
        else if (kernelMethods.Length < 1)
        {
            foreach (Location location in shaderType.Locations)
            {
                ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.NoKernelDefined, location));
            }
        }

        ShaderKernelMethod = kernelMethods.Single();

        KernelBodyDeclaration = ShaderKernelMethod.DeclaringSyntaxReferences
            .Select(x => x.GetSyntax())
            .OfType<MethodDeclarationSyntax>()
            .Where(x => x.Body is not null)
            .Single();

        KernelBodySyntaxTree = KernelBodyDeclaration.SyntaxTree;

        KernelBodySemanticModel = compilation.GetSemanticModel(KernelBodySyntaxTree);

        ShaderEmitters = new List<HLSLEmitter>();

        // if-check to allow different shader types in the future
        if (true)
        {
            ShaderEmitters.Add(CreateEmitter<ComputeFieldEmitter>());
            ShaderEmitters.Add(CreateEmitter<ComputeKernelDeclarationEmitter>());
        }

        ShaderEmitters.Add(CreateEmitter<CodeBlockEmitter>(KernelBodyDeclaration.Body!, KernelBodySyntaxTree, KernelBodySemanticModel));
    }

    public ShaderEmitResult Emit()
    {
        StringBuilder sourceResult = new StringBuilder();

        foreach (HLSLEmitter emitter in ShaderEmitters)
        {
            emitter.EmitHLSLSource();

            foreach (Diagnostic diagnostic in emitter.Diagnostics)
            {
                ReportDiagnostic(diagnostic);
            }

            sourceResult.Append(emitter.GetSource());
        }

        return new ShaderEmitResult(sourceResult.ToString(), ShaderType, Diagnostics.ToImmutableArray(), true);
    }

    protected void ReportDiagnostic(Diagnostic diagnostic)
    {
        Diagnostics.Add(diagnostic);
    }

    private HLSLEmitter CreateEmitter<T>(params object?[] args) where T : HLSLEmitter
    {
        if (args.Length < 1)
        {
            return (T)Activator.CreateInstance(typeof(T), Compilation, ShaderType, ShaderKernelMethod);
        }
        else
        {
            return (T)Activator.CreateInstance(typeof(T), new object?[] { Compilation, ShaderType, ShaderKernelMethod }.Concat(args).ToArray());
        }
    }
}
