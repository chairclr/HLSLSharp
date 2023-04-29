using Microsoft.CodeAnalysis.CSharp;

namespace HLSLSharp.Compiler;

public class SourceTranslator : TopLevelTranslator
{
    public SourceTranslator(string source, string shaderStructFullyQualifiedName)
        : base(CSharpSyntaxTree.ParseText(source), shaderStructFullyQualifiedName)
    {
        
    }
}
