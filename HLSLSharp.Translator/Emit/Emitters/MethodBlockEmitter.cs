using System;
using System.Collections.Generic;
using System.Text;
using HLSLSharp.Compiler.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Translator.Emit.Emitters;
internal class MethodBlockEmitter : HLSLEmitter
{
    public MethodBlockEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod, MethodDeclarationSyntax kernelBodyDeclaration, SyntaxTree kernelBodySyntaxTree, SemanticModel kernelBodySemanticModel) 
        : base(compilation, shaderType, shaderKernelMethod, kernelBodyDeclaration, kernelBodySyntaxTree, kernelBodySemanticModel)
    {

    }

    protected override void Emit()
    {
        SourceBuilder.AppendLine("{");
        SourceBuilder.AppendLine("}");
    }
}
