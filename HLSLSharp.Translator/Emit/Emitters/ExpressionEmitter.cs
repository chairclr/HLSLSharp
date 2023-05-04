using System;
using System.Collections.Generic;
using System.Text;
using HLSLSharp.Compiler.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class ExpressionEmitter : HLSLEmitter
{
    private readonly ExpressionSyntax Expression;

    private readonly SemanticModel ExpressionSemanticModel;

    public ExpressionEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod, ExpressionSyntax expression, SemanticModel expresisonSemanticModel) 
        : base(compilation, shaderType, shaderKernelMethod)
    {
        Expression = expression;
        ExpressionSemanticModel = expresisonSemanticModel;
    }

    protected override void Emit()
    {
        if (Expression is MemberAccessExpressionSyntax memberAccessExpression)
        {
            INamedTypeSymbol? type = (INamedTypeSymbol?)ExpressionSemanticModel.GetTypeInfo(memberAccessExpression.Expression).Type;

            if (type is not null)
            {
                if (BasicTypeTransformer.IsVectorType(type))
                {
                    ExpressionEmitter expressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, memberAccessExpression.Expression, ExpressionSemanticModel);

                    expressionEmitter.EmitHLSLSource();

                    SourceBuilder.Write($"{expressionEmitter.GetSource()}.{memberAccessExpression.Name.ToString().ToLower()}");
                }
            }
        }

        if (Expression is IdentifierNameSyntax identifierName)
        {
            SourceBuilder.Write(identifierName.Identifier.Text);
        }

        if (Expression is AssignmentExpressionSyntax assignmentExpression)
        {
            ExpressionEmitter leftExpressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, assignmentExpression.Left, ExpressionSemanticModel);

            leftExpressionEmitter.EmitHLSLSource();

            ExpressionEmitter rightExpressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, assignmentExpression.Right, ExpressionSemanticModel);

            rightExpressionEmitter.EmitHLSLSource();

            SourceBuilder.Write($"{leftExpressionEmitter.GetSource()} = {rightExpressionEmitter.GetSource()};");
        }

        if (Expression is LiteralExpressionSyntax literalExpression)
        {
            SourceBuilder.Write($"{literalExpression.Token}");
        }
    }
}
