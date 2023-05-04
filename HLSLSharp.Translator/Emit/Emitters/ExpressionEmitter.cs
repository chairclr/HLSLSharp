using System;
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

    public override void Emit()
    {
        if (Expression is MemberAccessExpressionSyntax memberAccessExpression)
        {
            INamedTypeSymbol? type = (INamedTypeSymbol?)ExpressionSemanticModel.GetTypeInfo(memberAccessExpression.Expression).Type;

            if (type is not null)
            {
                if (BasicTypeTransformer.IsVectorType(type))
                {
                    ExpressionEmitter expressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, memberAccessExpression.Expression, ExpressionSemanticModel);

                    expressionEmitter.Emit();

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

            leftExpressionEmitter.Emit();

            ExpressionEmitter rightExpressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, assignmentExpression.Right, ExpressionSemanticModel);

            rightExpressionEmitter.Emit();

            SourceBuilder.Write($"{leftExpressionEmitter.GetSource()} {assignmentExpression.OperatorToken} {rightExpressionEmitter.GetSource()};");
        }

        if (Expression is LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression.Token.Value is float floatValue)
            {
                string floatString = floatValue.ToString();

                float frac = floatValue % 1.0f;

                SourceBuilder.Write($"{floatString}");

                if (frac == 0)
                {
                    SourceBuilder.Write(".0");
                }
            }
            else if (literalExpression.Token.Value is double doubleValue)
            {
                string floatString = doubleValue.ToString();

                double frac = doubleValue % 1.0d;

                SourceBuilder.Write($"{floatString}");

                if (frac == 0)
                {
                    SourceBuilder.Write(".0");
                }
            }
            else
            {
                SourceBuilder.Write($"{literalExpression.Token}");
            }
        }

        if (Expression is BinaryExpressionSyntax binaryExpression)
        {
            ExpressionEmitter leftExpressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, binaryExpression.Left, ExpressionSemanticModel);

            leftExpressionEmitter.Emit();

            ExpressionEmitter rightExpressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, binaryExpression.Right, ExpressionSemanticModel);

            rightExpressionEmitter.Emit();

            SourceBuilder.Write($"{leftExpressionEmitter.GetSource()} {binaryExpression.OperatorToken} {rightExpressionEmitter.GetSource()}");
        }

        if (Expression is ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            ExpressionEmitter expressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, parenthesizedExpression.Expression, ExpressionSemanticModel);

            expressionEmitter.Emit();

            SourceBuilder.Write($"({expressionEmitter.GetSource()})");
        }

        if (Expression is InvocationExpressionSyntax invocationExpression)
        {
            if (IntrinsicCallTransformer.TryGetIntrinsicMethodCall((IMethodSymbol)ExpressionSemanticModel.GetSymbolInfo(invocationExpression).Symbol!, out string? intrinsicName))
            {
                SourceBuilder.Write($"{intrinsicName}(");

                ArgumentListEmitter argumentEmitter = new ArgumentListEmitter(Compilation, ShaderType, ShaderKernelMethod, invocationExpression.ArgumentList);

                argumentEmitter.Emit();

                SourceBuilder.Write($"{argumentEmitter.GetSource()})");
            }
            else
            {
                ExpressionEmitter expressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, invocationExpression.Expression, ExpressionSemanticModel);

                expressionEmitter.Emit();

                SourceBuilder.Write($"{expressionEmitter.GetSource()}(");

                ArgumentListEmitter argumentEmitter = new ArgumentListEmitter(Compilation, ShaderType, ShaderKernelMethod, invocationExpression.ArgumentList);

                argumentEmitter.Emit();

                SourceBuilder.Write($"{argumentEmitter.GetSource()})");
            }
        }
    }
}
