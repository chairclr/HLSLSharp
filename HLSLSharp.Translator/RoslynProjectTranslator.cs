using HLSLSharp.Translator;
using Microsoft.CodeAnalysis.CSharp;

namespace HLSLSharp.Compiler;

internal class RoslynProjectTranslator : ProjectTranslator
{
    public RoslynProjectTranslator(CSharpCompilation compilation)
        : base(compilation)
    {

    }
}
