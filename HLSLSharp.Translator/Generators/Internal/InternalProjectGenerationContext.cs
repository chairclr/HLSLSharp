﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace HLSLSharp.Compiler.Generators;

internal struct InternalProjectGenerationContext
{
    public Compilation Compilation { get; }

    public readonly HashSet<InternalGeneratorSource> AdditionalSources = new HashSet<InternalGeneratorSource>();

    public readonly ConcurrentBag<Diagnostic> Diagnostics = new ConcurrentBag<Diagnostic>();

    public InternalProjectGenerationContext(Compilation compilation)
    {
        Compilation = compilation;
    }

    public void AddSource(string hintName, string source)
    {
        InternalGeneratorSource internalGeneratorSource = new InternalGeneratorSource(hintName, source);

        if (AdditionalSources.Contains(internalGeneratorSource))
        {
            throw new ArgumentException("Generated source file names must be unique within a generator.", nameof(hintName));
        }

        AdditionalSources.Add(internalGeneratorSource);
    }

    public void AddSource(string hintName, SourceText source)
    {
        InternalGeneratorSource internalGeneratorSource = new InternalGeneratorSource(hintName, source);

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