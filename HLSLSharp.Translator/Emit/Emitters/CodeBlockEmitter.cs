using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HLSLSharp.Compiler.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Translator.Emit.Emitters;
internal class CodeBlockEmitter : HLSLEmitter
{
    public readonly BlockSyntax CodeBlock;

    public readonly SyntaxTree CodeBlockSyntaxTree;

    public readonly SemanticModel CodeBlockSemanticModel;

    public CodeBlockEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod, BlockSyntax codeBlock, SyntaxTree codeBlockSyntaxTree, SemanticModel codeBlockSemanticModel)
        : base(compilation, shaderType, shaderKernelMethod)
    {
        CodeBlock = codeBlock;

        CodeBlockSyntaxTree = codeBlockSyntaxTree;

        CodeBlockSemanticModel = codeBlockSemanticModel;
    }

    protected override void Emit()
    {
        SourceBuilder.AppendLine("{");

        SourceBuilder.AppendLine($"    /// -- Original Method Source code -- ///");
        SourceBuilder.AppendLine($"    /*");
        SourceBuilder.AppendLine($"    {CodeBlock}");
        SourceBuilder.AppendLine($"    */");

        foreach (StatementSyntax statement in CodeBlock.Statements)
        {
            SyntaxKind kind = statement.Kind();

            if (statement is IfStatementSyntax ifStatement)
            {
                SourceBuilder.AppendLine($"    /// -- If Statement -- ///");
                SourceBuilder.AppendLine($"    /// -- if ({ifStatement.Condition}) -- ///");
                SourceBuilder.AppendLine($"    /*");
                SourceBuilder.AppendLine($"    {ifStatement.Statement}");
                SourceBuilder.AppendLine($"    */");
            }
            else
            {

                SourceBuilder.AppendLine($"    /// -- Statement -- ///");
                SourceBuilder.AppendLine($"    /*");
                SourceBuilder.AppendLine($"    {statement}");
                SourceBuilder.AppendLine($"    */");
            }
        }

        SourceBuilder.AppendLine("}");
    }
}
