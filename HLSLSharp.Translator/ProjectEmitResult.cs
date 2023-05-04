using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HLSLSharp.Compiler;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator;

public class ProjectEmitResult
{
    public readonly IReadOnlyList<ShaderEmitResult> ShaderEmitResults;

    public readonly IEnumerable<Diagnostic> AllDiagnostics;

    public readonly IEnumerable<Diagnostic> TranslationDiagnostics;

    public readonly bool Success;

    internal ProjectEmitResult(List<ShaderEmitResult> results, ConcurrentBag<Diagnostic> translationDiagnostics)
    {
        ShaderEmitResults = results;

        TranslationDiagnostics = translationDiagnostics;

        AllDiagnostics = TranslationDiagnostics;

        Success = !AllDiagnostics.Any(x => x.Severity == DiagnosticSeverity.Error);
    }
}
