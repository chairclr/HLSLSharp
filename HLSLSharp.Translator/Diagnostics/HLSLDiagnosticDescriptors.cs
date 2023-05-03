using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator.Diagnostics;

public static class HLSLDiagnosticDescriptors
{
    public static readonly string IdPrefix = "HLSLS";

    public static DiagnosticDescriptor MissingOrInvalidCoreLibReference => new DiagnosticDescriptor($"{IdPrefix}0001", "Missing or invalid CoreLib reference", "Missing or invalid CoreLib reference", "Project", DiagnosticSeverity.Error, true);

    public static DiagnosticDescriptor MoreThanOneKernel => new DiagnosticDescriptor($"{IdPrefix}0002", "Multiple Kernels methods defined for one shader", "Multiple Kernels methods defined for one shader", "Emit", DiagnosticSeverity.Error, true);
    
    public static DiagnosticDescriptor NoKernelDefined => new DiagnosticDescriptor($"{IdPrefix}0003", "No kernel method defined for the shader", "No kernel method defined for the shader", "Emit", DiagnosticSeverity.Error, true);

    public static DiagnosticDescriptor ShadersCannotHaveClassFields => new DiagnosticDescriptor($"{IdPrefix}0004", "Shaders cannot have class fields", "Shaders cannot have class fields (field '{0}')", "Emit", DiagnosticSeverity.Error, true);
}
