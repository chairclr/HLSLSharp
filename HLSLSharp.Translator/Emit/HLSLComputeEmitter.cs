using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Compiler.Emit;

internal partial class HLSLEmitter
{
    private static readonly string ComputeShaderAttributeFullName = "System.Shaders.ComputeShaderAttribute";

    private INamedTypeSymbol? ComputeShaderAttributeSymbol;

    private ImmutableArray<ComputeStructTranslationInfo> ComputeStructsToTranslate = ImmutableArray<ComputeStructTranslationInfo>.Empty;

    private void CacheComputeTranslationInfo()
    {
        ComputeShaderAttributeSymbol ??= ShaderSemanticModel.Compilation.GetTypeByMetadataName(ComputeShaderAttributeFullName)!;

        IEnumerable<StructDeclarationSyntax> structDeclarations = ShaderCompilationUnit.DescendantNodes().OfType<StructDeclarationSyntax>();

        IEnumerable<INamedTypeSymbol> structsWithAttribute = structDeclarations
            .Select(x => (INamedTypeSymbol)ShaderSemanticModel.GetDeclaredSymbol(x)!)
            .Where(x => x!.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, ComputeShaderAttributeSymbol)));

        IEnumerable<ComputeStructTranslationInfo> structTranslationInfos = structsWithAttribute
            .Select(x => new ComputeStructTranslationInfo(x, x.GetAttributes().First(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, ComputeShaderAttributeSymbol))));

        ComputeStructsToTranslate.AddRange(structTranslationInfos);
    }

    private sealed class ComputeStructTranslationInfo
    {
        public INamedTypeSymbol StructSymbol;

        public AttributeData ComputeShaderAttributeData;

        public ComputeStructTranslationInfo(INamedTypeSymbol structSymbol, AttributeData computeShaderAttributeData)
        {
            StructSymbol = structSymbol;
            ComputeShaderAttributeData = computeShaderAttributeData;
        }
    }
}
