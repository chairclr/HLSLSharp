﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using HLSLSharp.Translator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        foreach (string line in emitter.GetSource().Split('\n'))
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
