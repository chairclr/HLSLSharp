using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HLSLSharp.Compiler.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class ComputeFieldEmitter : HLSLEmitter
{
    public ComputeFieldEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod, MethodDeclarationSyntax kernelBodyDeclaration, SyntaxTree kernelBodySyntaxTree, SemanticModel kernelBodySemanticModel) 
        : base(compilation, shaderType, shaderKernelMethod, kernelBodyDeclaration, kernelBodySyntaxTree, kernelBodySemanticModel)
    {

    }

    protected override void Emit()
    {
        foreach (IFieldSymbol fieldSymbol in ShaderType.GetMembers().Where(x => x.Kind == SymbolKind.Field))
        {
            if (!ValidateField(fieldSymbol))
            {
                SourceBuilder.AppendLine($"// Invalid Field {fieldSymbol.Type} {fieldSymbol.Name}");
                continue;
            }

            SourceBuilder.AppendLine($"// Valid Field {fieldSymbol.Type} {fieldSymbol.Name}");
        }
    }

    private bool ValidateField(IFieldSymbol field)
    {
        if (field.IsVirtual)
        {
            return false;
        }

        if (field.IsAbstract)
            return false;

        if (field.Type.TypeKind == TypeKind.Class && !field.Type.ToString().StartsWith("HLSLSharp.CoreLib.Vector"))
        {
            return false;
        }

        return true;
    }
}
