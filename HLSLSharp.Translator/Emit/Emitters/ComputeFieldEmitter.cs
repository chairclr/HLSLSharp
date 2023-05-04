using System.Linq;
using HLSLSharp.Compiler.Emit;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class ComputeFieldEmitter : HLSLEmitter
{
    private readonly INamedTypeSymbol RegisterAttributeType;

    private readonly INamedTypeSymbol RWBufferType;

    public ComputeFieldEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod)
        : base(compilation, shaderType, shaderKernelMethod)
    {
        RegisterAttributeType = compilation.GetTypeByMetadataName("HLSLSharp.CoreLib.Shaders.Registers.RegisterAttribute")!;

        RWBufferType = compilation.GetTypeByMetadataName("HLSLSharp.CoreLib.Compute.RWBuffer`1")!;
    }

    public override void Emit()
    {
        foreach (IFieldSymbol fieldSymbol in ShaderType.GetMembers().Where(x => x.Kind == SymbolKind.Field))
        {
            if (!ValidateField(fieldSymbol))
            {
                SourceBuilder.WriteLine($"// Invalid Field {fieldSymbol.Type} {fieldSymbol.Name}");
                continue;
            }

            if (SymbolEqualityComparer.Default.Equals(fieldSymbol.Type.OriginalDefinition, RWBufferType))
            {
                AttributeData? attributeData = fieldSymbol.GetAttributes().Where(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, RegisterAttributeType)).FirstOrDefault();

                if (attributeData is null)
                {
                    ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.ShadersCannotHaveClassFields, fieldSymbol.Locations.Single()));
                    continue;
                }

                string csRegisterType = attributeData.ConstructorArguments[0].ToCSharpString();
                int slot = (int)attributeData.ConstructorArguments[1].Value!;

                string registerType = csRegisterType switch
                {
                    "HLSLSharp.CoreLib.Shaders.Registers.RegisterType.UnorderedAccessView" => "u",
                    _ => ""
                };

                INamedTypeSymbol type = (INamedTypeSymbol)((INamedTypeSymbol)fieldSymbol.Type).TypeArguments.Single();

                if (BasicTypeTransformer.TryGetHLSLTypeName(type, out string? hlslType))
                {
                    SourceBuilder.WriteLine($"RWBuffer<{hlslType}> {fieldSymbol.Name} : register({registerType}{slot});");
                }
            }
        }
    }

    private bool ValidateField(IFieldSymbol fieldSymbol)
    {
        if (fieldSymbol.Type.TypeKind == TypeKind.Class && !BasicTypeTransformer.TryGetHLSLTypeName((INamedTypeSymbol)fieldSymbol.Type, out _) && !BasicTypeTransformer.TryGetComputeBufferName((INamedTypeSymbol)fieldSymbol.Type, out _))
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.ShadersCannotHaveClassFields, fieldSymbol.Locations.Single(), fieldSymbol.ToString()));

            return false;
        }

        return true;
    }
}
