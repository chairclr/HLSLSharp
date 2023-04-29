using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace HLSLSharp.Compiler.Generators;

internal struct InternalGenerationContext
{
    public Compilation Compilation { get; }

    public readonly Dictionary<string, (SyntaxTree, SourceText)> AdditionalSources = new Dictionary<string, (SyntaxTree, SourceText)>();

    internal InternalGenerationContext(Compilation compilation)
    {
        Compilation = compilation;
    }

    public void AddSource(string hintName, string source)
    {
        if (AdditionalSources.ContainsKey(hintName)) 
        {
            throw new ArgumentException("Generated source file names must be unique within a generator.", nameof(hintName));
        }

        AdditionalSources.Add(hintName, (CSharpSyntaxTree.ParseText(source), SourceText.From(source, Encoding.Unicode)));
    }

    public void AddSource(string hintName, SourceText source)
    {
        if (AdditionalSources.ContainsKey(hintName))
        {
            throw new ArgumentException("Generated source file names must be unique within a generator.", nameof(hintName));
        }

        AdditionalSources.Add(hintName, (CSharpSyntaxTree.ParseText(source), source));
    }
}