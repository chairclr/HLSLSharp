using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler;

public class ShaderEmitResult
{
    public string? Result;

    public ImmutableArray<Diagnostic> Diagnostics { get; }

    public bool IsError { get; }

    internal ShaderEmitResult(string? result, ImmutableArray<Diagnostic> diagnostics, bool isError)
    {
        Result = result;
        Diagnostics = diagnostics;
        IsError = isError;
    }
}
