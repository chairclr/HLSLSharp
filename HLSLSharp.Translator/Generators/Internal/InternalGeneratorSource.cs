using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace HLSLSharp.Compiler.Generators;

internal class InternalGeneratorSource
{
    public string HintName;

    public SourceText Source;

    public SyntaxTree SyntaxTree;

    public InternalGeneratorSource(string hintName, string source)
    {
        HintName = hintName;
        Source = SourceText.From(source, Encoding.Unicode);
        SyntaxTree = CSharpSyntaxTree.ParseText(source);
    }

    public InternalGeneratorSource(string hintName, SourceText source)
    {
        HintName = hintName;
        Source = source;
        SyntaxTree = CSharpSyntaxTree.ParseText(source);
    }

    public override int GetHashCode()
    {
        return HintName.GetHashCode();
    }
}
