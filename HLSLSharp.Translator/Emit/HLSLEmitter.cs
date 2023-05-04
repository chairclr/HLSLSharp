using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler.Emit;

internal abstract class HLSLEmitter
{
    public readonly Compilation Compilation;

    public readonly INamedTypeSymbol ShaderType;

    public readonly IMethodSymbol ShaderKernelMethod;

    private readonly StringBuilder SourceStringBuilder;

    protected readonly StringWriter SourceBuilder;

    public readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();

    public HLSLEmitter(Compilation compilation, INamedTypeSymbol shaderType, IMethodSymbol shaderKernelMethod)
    {
        Compilation = compilation;

        ShaderType = shaderType;

        ShaderKernelMethod = shaderKernelMethod;

        SourceStringBuilder = new StringBuilder();

        SourceBuilder = new StringWriter(SourceStringBuilder)
        {
            NewLine = "\n"
        };
    }

    protected abstract void Emit();

    public void EmitHLSLSource()
    {
        Emit();
    }

    public string GetSource()
    {
        return SourceBuilder.ToString();
    }

    protected void ReportDiagnostic(Diagnostic diagnostic)
    {
        Diagnostics.Add(diagnostic);
    }

    protected void WriteEmitterSource(HLSLEmitter emitter, bool indent = false)
    {
        emitter.EmitHLSLSource();

        foreach (string line in emitter.GetSource().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (indent)
            {
                SourceBuilder.Write("    ");
            }

            SourceBuilder.WriteLine(line);
        }
    }

    ~HLSLEmitter()
    {
        SourceBuilder.Dispose();
    }
}
