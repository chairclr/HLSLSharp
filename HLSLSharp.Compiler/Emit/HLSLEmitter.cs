using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Compiler.Emit;

internal partial class HLSLEmitter
{
    private readonly SyntaxTree SyntaxTree;

    private readonly CompilationUnitSyntax CompilationUnit;

    private readonly CSharpCompilation Compilation;

    private readonly SemanticModel SemanticModel;

    private readonly StringBuilder SourceBuilder;

    public HLSLEmitter(SyntaxTree syntaxTree, CompilationUnitSyntax compilationUnit, CSharpCompilation compilation, SemanticModel semanticModel)
    {
        SyntaxTree = syntaxTree;

        CompilationUnit = compilationUnit;

        Compilation = compilation;

        SemanticModel = semanticModel;

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
