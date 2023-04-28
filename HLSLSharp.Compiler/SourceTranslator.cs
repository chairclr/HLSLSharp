using Microsoft.CodeAnalysis.CSharp;

namespace HLSLSharp.Compiler;

public class SourceTranslator : Translator
{
    public SourceTranslator(string source)
        : base(CSharpSyntaxTree.ParseText(source))
    {

    }
}
