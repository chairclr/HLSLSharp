using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler;

public class ShaderEmitResult
{
    public string? Result;

    public string FullyQualifiedShaderTypeName;

    public readonly ImmutableArray<Diagnostic> Diagnostics;

    public readonly bool Success;

    internal ShaderEmitResult(string? result, INamedTypeSymbol shaderType, ImmutableArray<Diagnostic> diagnostics, bool success)
    {
        Result = result;
        Diagnostics = diagnostics;
        Success = success;
        FullyQualifiedShaderTypeName = $"{shaderType.ContainingNamespace}.{shaderType.MetadataName}";
    }
}
