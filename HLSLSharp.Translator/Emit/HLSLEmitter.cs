using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Compiler.Emit;

internal partial class HLSLEmitter
{
    private readonly CSharpCompilation Compilation;

    private readonly SyntaxTree ShaderSyntaxTree;

    private readonly CompilationUnitSyntax ShaderCompilationUnit;

    private readonly SemanticModel ShaderSemanticModel;

    private readonly StringBuilder SourceBuilder;

    public HLSLEmitter(CSharpCompilation compilation, SyntaxTree shaderSyntaxTree, CompilationUnitSyntax sahderCompilationUnit, SemanticModel shaderSemanticModel)
    {
        ShaderSyntaxTree = shaderSyntaxTree;

        ShaderCompilationUnit = sahderCompilationUnit;

        Compilation = compilation;

        ShaderSemanticModel = shaderSemanticModel;

        SourceBuilder = new StringBuilder();
    }

    public void EmitHLSLSource()
    {
        CacheComputeTranslationInfo();


    }

    public string GetSource()
    {
        return SourceBuilder.ToString();
    }
}
