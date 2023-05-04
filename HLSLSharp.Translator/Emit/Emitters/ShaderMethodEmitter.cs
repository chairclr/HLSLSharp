using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HLSLSharp.Compiler.Emit;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class ShaderMethodEmitter : HLSLEmitter
{
    public ShaderMethodEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod)
        : base(compilation, shaderType, shaderKernelMethod)
    {

    }

    public override void Emit()
    {
        foreach (IMethodSymbol methodSymbol in ShaderType.GetMembers().Where(x => x.Kind == SymbolKind.Method))
        {
            if (!ValidateMethodDeclaration(methodSymbol))
            {
                SourceBuilder.WriteLine($"// Invalid Method {methodSymbol}");
                continue;
            }

            SourceBuilder.WriteLine($"// Valid Method {methodSymbol}");


        }
    }

    private bool ValidateMethodDeclaration(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.IsAbstract)
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.MethodAbstract, methodSymbol.Locations.FirstOrDefault(), methodSymbol.ToString()));

            return false;
        }

        if (methodSymbol.IsGenericMethod)
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.MethodGeneric, methodSymbol.Locations.FirstOrDefault(), methodSymbol.ToString()));

            return false;
        }

        // Don't emit constructor methods
        if (methodSymbol.MethodKind == MethodKind.Constructor)
        {
            return false;
        }

        // Don't emit destructor methods
        if (methodSymbol.MethodKind == MethodKind.Destructor)
        {
            ReportDiagnostic(Diagnostic.Create(HLSLDiagnosticDescriptors.ShaderDestructor, methodSymbol.Locations.FirstOrDefault()));

            return false;
        }

        if (SymbolEqualityComparer.Default.Equals(methodSymbol, ShaderKernelMethod))
        {
            return false;
        }

        return true;
    }
}
