using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator.Diagnostics;

public static class HLSLDiagnosticDescriptors
{
    public static readonly string IdPrefix = "HLSLS";

    public static DiagnosticDescriptor MissingOrInvalidCoreLibReference => new DiagnosticDescriptor($"{IdPrefix}0001", "Missing or invalid CoreLib reference", "", "Project", DiagnosticSeverity.Error, true);
}
