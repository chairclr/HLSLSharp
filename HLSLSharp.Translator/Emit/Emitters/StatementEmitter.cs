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

    protected override void Emit()
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

                            expressionEmitter.EmitHLSLSource();

                            SourceBuilder.Write($" = {expressionEmitter.GetSource()}");
                        }

                        SourceBuilder.WriteLine($";");
                    }
                }
            }
        }

        if (Statement is IfStatementSyntax ifStatement)
        {
            SourceBuilder.WriteLine($"if ({ifStatement.Condition})");

            SourceBuilder.WriteLine($"{{");

            StatementEmitter statementEmitter = new StatementEmitter(Compilation, ShaderType, ShaderKernelMethod, ifStatement.Statement, StatementSemanticModel);

            WriteEmitterSource(statementEmitter, true);

            SourceBuilder.WriteLine($"}}");
        }

        if (Statement is BlockSyntax codeBlock)
        {
            CodeBlockEmitter codeBlockEmitter = new CodeBlockEmitter(Compilation, ShaderType, ShaderKernelMethod, codeBlock, codeBlock.SyntaxTree, Compilation.GetSemanticModel(codeBlock.SyntaxTree));

            WriteEmitterSource(codeBlockEmitter, false);
        }

        if (Statement is ExpressionStatementSyntax expressionStatement)
        {
            ExpressionEmitter expressionEmitter = new ExpressionEmitter(Compilation, ShaderType, ShaderKernelMethod, expressionStatement.Expression, StatementSemanticModel);

            WriteEmitterSource(expressionEmitter);
        }
    }
}
