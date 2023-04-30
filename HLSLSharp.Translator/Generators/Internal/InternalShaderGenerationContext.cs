using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace HLSLSharp.Compiler.Generators;

internal struct InternalShaderGenerationContext
{
    public Compilation Compilation { get; }

    public INamedTypeSymbol ShaderStructType { get; }

    public readonly HashSet<InternalGeneratorSource> AdditionalSources = new HashSet<InternalGeneratorSource>();

    public readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();

    private readonly CSharpParseOptions ParseOptions;

    public InternalShaderGenerationContext(Compilation compilation, INamedTypeSymbol shaderStructType)
    {
        Compilation = compilation;
        ShaderStructType = shaderStructType;

        if (compilation.SyntaxTrees.Any(x => x.Options is CSharpParseOptions))
        {
            ParseOptions = (CSharpParseOptions)compilation.SyntaxTrees.Where(x => x.Options is CSharpParseOptions).First().Options;
        }
        else
        {
            ParseOptions = CSharpParseOptions.Default;
        }
    }

    public void AddSource(string hintName, string source)
    {
        InternalGeneratorSource internalGeneratorSource = new InternalGeneratorSource(hintName, source, ParseOptions);

        if (AdditionalSources.Contains(internalGeneratorSource))
        {
            throw new ArgumentException("Generated source file names must be unique within a generator.", nameof(hintName));
        }

        AdditionalSources.Add(internalGeneratorSource);
    }

    public void AddSource(string hintName, SourceText source)
    {
        InternalGeneratorSource internalGeneratorSource = new InternalGeneratorSource(hintName, source, ParseOptions);

        if (AdditionalSources.Contains(internalGeneratorSource))
        {
            throw new ArgumentException("Generated source file names must be unique within a generator.", nameof(hintName));
        }

        AdditionalSources.Add(internalGeneratorSource);
    }

    public void ReportDiagnostic(Diagnostic diagnostic)
    {
        Diagnostics.Add(diagnostic);
    }
}
