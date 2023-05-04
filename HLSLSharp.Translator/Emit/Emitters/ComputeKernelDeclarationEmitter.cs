using System;
using System.Linq;
using HLSLSharp.Compiler.Emit;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class ComputeKernelDeclarationEmitter : HLSLEmitter
{
    private static readonly string ComputeShaderAttributeFullName = "HLSLSharp.CoreLib.Shaders.ComputeShaderAttribute";

    private readonly INamedTypeSymbol ComputeShaderAttributeSymbol;

    public ComputeKernelDeclarationEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod)
        : base(compilation, shaderType, shaderKernelMethod)
    {
        ComputeShaderAttributeSymbol = Compilation.GetTypeByMetadataName(ComputeShaderAttributeFullName)!;

    }

    public override void Emit()
    {
        AttributeData computeShaderAttribute = ShaderType.GetAttributes().Where(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, ComputeShaderAttributeSymbol)).Single();

        int x = (int)computeShaderAttribute.ConstructorArguments[0].Value!;
        int y = (int)computeShaderAttribute.ConstructorArguments[1].Value!;
        int z = (int)computeShaderAttribute.ConstructorArguments[2].Value!;

        ValidateComputeShaderAttribute(computeShaderAttribute, x, y, z);

        SourceBuilder.WriteLine($"[numthreads({x}, {y}, {z})]");
        SourceBuilder.WriteLine($"void __CSMain(uint3 ThreadId : SV_DispatchThreadID)");
    }

    private void ValidateComputeShaderAttribute(AttributeData computeShaderAttribute, int x, int y, int z)
    {
        Lazy<Location?> computeShaderAttributeLocation = new Lazy<Location?>(() => computeShaderAttribute.ApplicationSyntaxReference?.GetSyntax().GetLocation());

        if (x < 1)
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.ComputeShaderNumThreadsMustBeGreaterThan0, computeShaderAttributeLocation.Value, "x"));
        }

        if (y < 1)
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.ComputeShaderNumThreadsMustBeGreaterThan0, computeShaderAttributeLocation.Value, "x"));
        }

        if (z < 1)
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.ComputeShaderNumThreadsMustBeGreaterThan0, computeShaderAttributeLocation.Value, "x"));
        }

        if (x * y * z > 1024)
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.ComputeShaderNumThreadsExceedsMaximum, computeShaderAttributeLocation.Value, 1024));
        }
    }
}
