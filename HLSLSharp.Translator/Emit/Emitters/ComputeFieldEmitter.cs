using System.Linq;
using HLSLSharp.Compiler.Emit;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class ComputeFieldEmitter : HLSLEmitter
{
    public ComputeFieldEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod)
        : base(compilation, shaderType, shaderKernelMethod)
    {

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

            SourceBuilder.WriteLine($"// Valid Field {fieldSymbol.Type} {fieldSymbol.Name}");
        }
    }

    private bool ValidateField(IFieldSymbol field)
    {
        if (field.IsVirtual)
        {
            return false;
        }

        if (field.IsAbstract)
        {
            return false;
        }

        if (field.Type.TypeKind == TypeKind.Class && !field.Type.ToString().StartsWith("HLSLSharp.CoreLib.Vector"))
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.ShadersCannotHaveClassFields, field.Locations.Single(), field.ToString()));

            return false;
        }

        return true;
    }
}
