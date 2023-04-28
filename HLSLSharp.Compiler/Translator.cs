using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler;

public abstract class Translator
{
    protected SyntaxTree SyntaxTree;

    public Translator(SyntaxTree syntaxTree)
    {
        SyntaxTree = syntaxTree;
    }
}
