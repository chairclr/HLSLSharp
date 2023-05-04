using HLSLSharp.Compiler.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Translator.Emit.Emitters;

internal class MethodCodeBlockEmitter : HLSLEmitter
{
    public readonly BlockSyntax CodeBlock;

    public readonly SyntaxTree CodeBlockSyntaxTree;

    public readonly SemanticModel CodeBlockSemanticModel;

    public MethodCodeBlockEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod, BlockSyntax codeBlock, SyntaxTree codeBlockSyntaxTree, SemanticModel codeBlockSemanticModel)
        : base(compilation, shaderType, shaderKernelMethod)
    {
        CodeBlock = codeBlock;

        CodeBlockSyntaxTree = codeBlockSyntaxTree;

        CodeBlockSemanticModel = codeBlockSemanticModel;
    }

    public override void Emit()
    {
        SourceBuilder.WriteLine($"{{");

        CodeBlockEmitter codeBlockEmitter = new CodeBlockEmitter(Compilation, ShaderType, ShaderKernelMethod, CodeBlock, CodeBlockSyntaxTree, CodeBlockSemanticModel);

        WriteEmitter(codeBlockEmitter, true);

        SourceBuilder.WriteLine($"}}");
    }
}
