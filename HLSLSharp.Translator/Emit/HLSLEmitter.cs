using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Compiler.Emit;

internal partial class HLSLEmitter
{
    private static readonly string KernelAttributeFullName = "HLSLSharp.CoreLib.Shaders.KernelAttribute";

    public readonly Compilation Compilation;

    public readonly INamedTypeSymbol ShaderType;

    public readonly IMethodSymbol ShaderKernelMethod;

    public readonly MethodDeclarationSyntax KernelBodyDeclaration;

    public readonly SyntaxTree KernelBodySyntaxTree;

    public readonly SemanticModel KernelBodySemanticModel;

    private StringBuilder SourceBuilder;

    public readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();

    public HLSLEmitter(Compilation compilation, INamedTypeSymbol shaderType)
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

        SourceBuilder = new StringBuilder();
    }

    public void EmitHLSLSource()
    {
        // CacheComputeTranslationInfo();

        EmitFieldMembers();
    }

    public string GetSource()
    {
        return SourceBuilder.ToString();
    }

    protected void ReportDiagnostic(Diagnostic diagnostic)
    {
        Diagnostics.Add(diagnostic);
    }

    private void EmitFieldMembers()
    {
        foreach (IFieldSymbol fieldSymbol in ShaderType.GetMembers().OfType<IFieldSymbol>())
        {
            SourceBuilder.AppendLine($"// {fieldSymbol.Type} {fieldSymbol.Name}");
        }
    }
}
