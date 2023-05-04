using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HLSLSharp.Compiler.Emit;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class MethodSignatureEmitter : HLSLEmitter
{
    private IMethodSymbol MethodSymbol;

    public MethodSignatureEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod, IMethodSymbol methodSymbol)
        : base(compilation, shaderType, shaderKernelMethod)
    {
        MethodSymbol = methodSymbol;
    }

    public override void Emit()
    {
        if (BasicTypeTransformer.TryGetHLSLTypeName((INamedTypeSymbol)MethodSymbol.ReturnType, out string? hlslReturnType))
        {
            SourceBuilder.Write($"{hlslReturnType} ");
        }
        else
        {
            // TODO: Report error diagnostic when type transformation fails (invalid return type)
        }

        SourceBuilder.Write($"{MethodSymbol.Name}(");

        for (int i = 0; i < MethodSymbol.Parameters.Length; i++) 
        {
            IParameterSymbol parameter = MethodSymbol.Parameters[i];

            if (BasicTypeTransformer.TryGetHLSLTypeName((INamedTypeSymbol)parameter.Type, out string? hlslParameterType))
            {
                SourceBuilder.Write($"{hlslParameterType} {parameter.Name}");
            }
            else
            {
                // TODO: Report error diagnostic when type transformation fails (invalid parameter type)
            }

            if (i + 1 < MethodSymbol.Parameters.Length)
            {
                SourceBuilder.Write(", ");
            }
        }

        SourceBuilder.Write($")");
    }
}
