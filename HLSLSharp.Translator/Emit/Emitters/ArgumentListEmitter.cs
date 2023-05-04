using System;
using System.Collections.Generic;
using System.Text;
using HLSLSharp.Compiler.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class ArgumentListEmitter : HLSLEmitter
{
    private readonly ArgumentListSyntax ArgumentList;

    public ArgumentListEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod, ArgumentListSyntax argumentList)
        : base(compilation, shaderType, shaderKernelMethod)
    {
        ArgumentList = argumentList;
    }

    public override void Emit()
    {
        for (int i = 0; i < ArgumentList.Arguments.Count; i++)
        {
            ArgumentSyntax argument = ArgumentList.Arguments[i];

            ExpressionEmitter expressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, argument.Expression, Compilation.GetSemanticModel(ArgumentList.SyntaxTree));

            expressionEmitter.Emit();

            SourceBuilder.Write(expressionEmitter.GetSource());

            if (i + 1 < ArgumentList.Arguments.Count)
            {
                SourceBuilder.Write(", ");
            }
        }
    }
}
