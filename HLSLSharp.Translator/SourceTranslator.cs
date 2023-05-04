using HLSLSharp.Translator;
using Microsoft.CodeAnalysis.CSharp;

namespace HLSLSharp.Compiler;

public class SourceTranslator : ProjectTranslator
{
    public SourceTranslator(string source)
        : base(CSharpSyntaxTree.ParseText(source))
    {

    }
}
