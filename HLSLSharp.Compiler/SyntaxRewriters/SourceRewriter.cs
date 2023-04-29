using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HLSLSharp.Compiler.SyntaxRewriters;

internal abstract class SourceRewriter : CSharpSyntaxRewriter
{
    protected readonly SemanticModel SemanticModel;

    public readonly List<SyntaxNode> AddedNodes = new List<SyntaxNode>();

    public SourceRewriter(SemanticModel semanticModel)
    {
        SemanticModel = semanticModel;
    }

    public void RegisterNewMember(SyntaxNode newMemeber)
    {
        AddedNodes.Add(newMemeber);
    }
}
