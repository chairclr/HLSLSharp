using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using HLSLSharp.Translator.Emit;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler.Emit;

internal abstract class HLSLEmitter
{
    public readonly Compilation Compilation;

    public readonly INamedTypeSymbol ShaderType;

    public readonly IMethodSymbol ShaderKernelMethod;

    protected readonly SourceBuilder SourceBuilder;

    public readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();

    public HLSLEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod)
    {
        Compilation = compilation;

        ShaderType = shaderType;

        ShaderKernelMethod = shaderKernelMethod;

        SourceBuilder = new SourceBuilder();
    }

    public abstract void Emit();

    public string GetSource()
    {
        return SourceBuilder.ToString();
    }

    protected void ReportDiagnostic(Diagnostic diagnostic)
    {
        Diagnostics.Add(diagnostic);
    }

    protected void WriteEmitter(HLSLEmitter emitter, bool indent = false)
    {
        emitter.Emit();

        if (indent)
        {
            SourceBuilder.Concat(emitter.SourceBuilder.GetLines().Select(x => "    " + x));
        }
        else
        {
            SourceBuilder.Concat(emitter.SourceBuilder.GetLines());
        }
    }
}
