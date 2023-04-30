using System.Linq;
using HLSLSharp.Compiler.Generators;
using HLSLSharp.Compiler.Generators.Internal.Vectors;
using HLSLSharp.Translator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HLSLSharp.Compiler;

public class SourceTranslator : ProjectTranslator
{
    //internal override IInternalGenerator[] InternalGenerators => base.InternalGenerators.Where(x => x.GetType() != typeof(AliasGenerator)).ToArray();

    public SourceTranslator(string source)
        : base(CSharpSyntaxTree.ParseText(source))
    {

    }
}
