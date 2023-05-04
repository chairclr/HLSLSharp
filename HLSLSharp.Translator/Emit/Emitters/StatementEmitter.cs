using HLSLSharp.Compiler.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class StatementEmitter : HLSLEmitter
{
    private readonly StatementSyntax Statement;

    private readonly SemanticModel StatementSemanticModel;

    public StatementEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod, StatementSyntax statement, SemanticModel statementSemanticModel)
        : base(compilation, shaderType, shaderKernelMethod)
    {
        Statement = statement;
        StatementSemanticModel = statementSemanticModel;
    }

    public override void Emit()
    {
        if (Statement is LocalDeclarationStatementSyntax localDeclarationStatement)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclarationStatement.Declaration;

            INamedTypeSymbol? symbol = (INamedTypeSymbol?)StatementSemanticModel.GetTypeInfo(variableDeclaration.Type).Type;

            if (symbol is not null)
            {
                if (BasicTypeTransformer.TryGetHLSLTypeName(symbol, out string? hlslType))
                {
                    foreach (VariableDeclaratorSyntax declaratorSyntax in variableDeclaration.Variables)
                    {
                        SourceBuilder.Write($"{hlslType} {declaratorSyntax.Identifier}");

                        if (declaratorSyntax.Initializer is not null)
                        {
                            ExpressionEmitter expressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, declaratorSyntax.Initializer.Value, StatementSemanticModel);

                            expressionEmitter.Emit();

                            SourceBuilder.Write($" = {expressionEmitter.GetSource()}");
                        }

                        SourceBuilder.WriteLine($";");
                    }
                }
            }
        }

        if (Statement is IfStatementSyntax ifStatement)
        {
            ExpressionEmitter conditionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, ifStatement.Condition, StatementSemanticModel);

            SourceBuilder.WriteLine($"if ({conditionEmitter.GetSource()})");

            SourceBuilder.WriteLine($"{{");

            StatementEmitter statementEmitter = new StatementEmitter(Compilation, ShaderType, ShaderKernelMethod, ifStatement.Statement, StatementSemanticModel);

            WriteEmitter(statementEmitter, true);

            SourceBuilder.WriteLine($"}}");
        }

        if (Statement is BlockSyntax codeBlock)
        {
            CodeBlockEmitter codeBlockEmitter = new CodeBlockEmitter(Compilation, ShaderType, ShaderKernelMethod, codeBlock, codeBlock.SyntaxTree, Compilation.GetSemanticModel(codeBlock.SyntaxTree));

            WriteEmitter(codeBlockEmitter, false);
        }

        if (Statement is ExpressionStatementSyntax expressionStatement)
        {
            ExpressionEmitter expressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, expressionStatement.Expression, StatementSemanticModel);

            WriteEmitter(expressionEmitter);
        }
    }
}
