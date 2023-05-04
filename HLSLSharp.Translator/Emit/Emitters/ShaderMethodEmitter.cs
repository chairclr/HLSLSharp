using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HLSLSharp.Compiler.Emit;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class ShaderMethodEmitter : HLSLEmitter
{
    public ShaderMethodEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod)
        : base(compilation, shaderType, shaderKernelMethod)
    {

    }

    public override void Emit()
    {
        // Emit method signatures BEFORE emitting their declarations 
        foreach (IMethodSymbol methodSymbol in ShaderType.GetMembers().Where(x => x.Kind == SymbolKind.Method))
        {
            if (!ValidateMethodDeclaration(methodSymbol))
            {
                continue;
            }

            MethodSignatureEmitter signatureEmitter = new MethodSignatureEmitter(Compilation, ShaderType, ShaderKernelMethod, methodSymbol);

            signatureEmitter.Emit();

            SourceBuilder.WriteLine($"{signatureEmitter.GetSource()};");
        }

        // Emit method declarations
        foreach (IMethodSymbol methodSymbol in ShaderType.GetMembers().Where(x => x.Kind == SymbolKind.Method))
        {
            if (!ValidateMethodDeclaration(methodSymbol))
            {
                continue;
            }

            MethodSignatureEmitter signatureEmitter = new MethodSignatureEmitter(Compilation, ShaderType, ShaderKernelMethod, methodSymbol);

            signatureEmitter.Emit();

            SourceBuilder.WriteLine($"{signatureEmitter.GetSource()}");

            MethodDeclarationSyntax methodDeclaration = methodSymbol.DeclaringSyntaxReferences
                .Select(x => x.GetSyntax())
                .Where(x => x.IsKind(SyntaxKind.MethodDeclaration))
                .Cast<MethodDeclarationSyntax>()
                .Where(x => x.Body is not null)
                .Single();

            MethodCodeBlockEmitter codeBlockEmitter = new MethodCodeBlockEmitter(Compilation, ShaderType, ShaderKernelMethod, methodDeclaration.Body!, methodDeclaration.SyntaxTree, Compilation.GetSemanticModel(methodDeclaration.SyntaxTree));

            WriteEmitter(codeBlockEmitter);
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
