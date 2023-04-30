using HLSLSharp.Compiler.Generators;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using HLSLSharp.Compiler.Generators.Internal.Compute;

namespace HLSLSharp.Compiler;

internal class ShaderTranslator
{
    public Compilation Compilation { get; }

    public INamedTypeSymbol ShaderType { get; }

    public IEnumerable<IInternalShaderGenerator> ShaderGenerators => new IInternalShaderGenerator[] { new ComputeGenerator() };

    public ShaderTranslator(Compilation compilation, INamedTypeSymbol shaderType)
    {
        Compilation = compilation;
        ShaderType = shaderType;
    }


}
