using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HLSLSharp.Compiler.SyntaxRewriters;

internal abstract class SourceRewriter : CSharpSyntaxRewriter
{
    protected readonly SemanticModel SemanticModel;

    public SourceRewriter(SemanticModel semanticModel)
    {
        SemanticModel = semanticModel;
    }
}
