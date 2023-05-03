﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Compiler.Emit;

internal abstract class HLSLEmitter
{
    public readonly Compilation Compilation;

    public readonly INamedTypeSymbol ShaderType;

    public readonly IMethodSymbol ShaderKernelMethod;

    public readonly MethodDeclarationSyntax KernelBodyDeclaration;

    public readonly SyntaxTree KernelBodySyntaxTree;

    public readonly SemanticModel KernelBodySemanticModel;

    protected readonly StringBuilder SourceBuilder;

    public readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();

    public HLSLEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod, MethodDeclarationSyntax kernelBodyDeclaration, SyntaxTree kernelBodySyntaxTree, SemanticModel kernelBodySemanticModel)
    {
        Compilation = compilation;

        ShaderType = shaderType;

        ShaderKernelMethod = shaderKernelMethod;

        KernelBodyDeclaration = kernelBodyDeclaration;

        KernelBodySyntaxTree = kernelBodySyntaxTree;

        KernelBodySemanticModel = kernelBodySemanticModel;

        SourceBuilder = new StringBuilder();
    }

    protected abstract void Emit();

    public void EmitHLSLSource()
    {
        Emit();
    }

    public string GetSource()
    {
        return SourceBuilder.ToString();
    }

    protected void ReportDiagnostic(Diagnostic diagnostic)
    {
        Diagnostics.Add(diagnostic);
    }
}
