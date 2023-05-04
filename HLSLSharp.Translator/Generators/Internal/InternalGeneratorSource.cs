using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace HLSLSharp.Compiler.Generators;

internal class InternalGeneratorSource
{
    public string HintName;

    public string Source;

    public SyntaxTree SyntaxTree;

    public InternalGeneratorSource(string hintName, string source, CSharpParseOptions options)
    {
        HintName = hintName;
        Source = source;
        SyntaxTree = CSharpSyntaxTree.ParseText(source, options, hintName);
    }

    public InternalGeneratorSource(string hintName, SourceText source, CSharpParseOptions options)
    {
        HintName = hintName;
        Source = source.ToString();
        SyntaxTree = CSharpSyntaxTree.ParseText(source, options, hintName);
    }

    public override int GetHashCode()
    {
        return HintName.GetHashCode();
    }
}
