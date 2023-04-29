using System.Linq;
using HLSLSharp.Compiler.Generators;
using HLSLSharp.Compiler.Generators.Internal.Vectors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Compiler;

internal class SourceGeneratorTranslator : TopLevelTranslator
{
    internal override IInternalGenerator[] InternalGenerators => base.InternalGenerators.Where(x => x.GetType() != typeof(AliasGenerator)).ToArray();

    public SourceGeneratorTranslator(SyntaxTree shaderSyntaxTree, INamedTypeSymbol shaderDeclaration)
        : base(shaderSyntaxTree, shaderDeclaration)
    {

    }
}
