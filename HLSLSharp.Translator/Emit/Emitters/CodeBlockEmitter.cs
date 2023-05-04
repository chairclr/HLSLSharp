using System;
using System.Collections.Generic;
using System.Linq;
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
        SourceBuilder.WriteLine($"/// -- Original Code Block Source code -- ///");
        SourceBuilder.WriteLine($"/*");
        SourceBuilder.WriteLine($"{CodeBlock.OpenBraceToken.LeadingTrivia.Where(x => x.IsKind(SyntaxKind.WhitespaceTrivia)).First()}{CodeBlock}");
        SourceBuilder.WriteLine($"*/");

        foreach (StatementSyntax statement in CodeBlock.Statements)
        {
            StatementEmitter statementEmitter = new StatementEmitter(Compilation, ShaderType, ShaderKernelMethod, statement, CodeBlockSemanticModel);

            WriteEmitterSource(statementEmitter, false);
        }
    }
}
