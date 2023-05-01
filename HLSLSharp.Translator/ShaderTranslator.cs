using HLSLSharp.Compiler.Generators;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using HLSLSharp.Compiler.Generators.Internal.Compute;
using System.Collections.Immutable;
using System.Text;
using HLSLSharp.Compiler.Emit;
using System.Collections.Concurrent;

namespace HLSLSharp.Compiler;

internal class ShaderTranslator
{
    public readonly Compilation Compilation;

    public readonly INamedTypeSymbol ShaderType;

    public IEnumerable<IInternalShaderGenerator> ShaderGenerators => new IInternalShaderGenerator[] { new ComputeGenerator() };

    public readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();

    public ShaderTranslator(Compilation compilation, INamedTypeSymbol shaderType)
    {
        Compilation = compilation;
        ShaderType = shaderType;
    }

    public ShaderEmitResult Emit()
    {
        HLSLEmitter emitter = new HLSLEmitter(Compilation, ShaderType);

        emitter.EmitHLSLSource();

        foreach (Diagnostic diagnostic in emitter.Diagnostics)
        {
            ReportDiagnostic(diagnostic);
        }

        return new ShaderEmitResult(emitter.GetSource(), ShaderType, ImmutableArray<Diagnostic>.Empty, true);
    }

    protected void ReportDiagnostic(Diagnostic diagnostic)
    {
        Diagnostics.Add(diagnostic);
    }
}
