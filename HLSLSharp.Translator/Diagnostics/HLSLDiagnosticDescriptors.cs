using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator.Diagnostics;

public static class HLSLDiagnosticDescriptors
{
    public static readonly string IdPrefix = "HLSLS";

    public static DiagnosticDescriptor MissingOrInvalidCoreLibReference => new DiagnosticDescriptor($"{IdPrefix}0001", "Missing or invalid CoreLib reference", "Missing or invalid CoreLib reference", "Project", DiagnosticSeverity.Error, true);

    public static DiagnosticDescriptor MoreThanOneKernel => new DiagnosticDescriptor($"{IdPrefix}0002", "Multiple Kernels methods defined for one shader", "Multiple Kernels methods defined for one shader", "Emit", DiagnosticSeverity.Error, true);

    public static DiagnosticDescriptor NoKernelDefined => new DiagnosticDescriptor($"{IdPrefix}0003", "No kernel method defined for the shader", "No kernel method defined for the shader", "Emit", DiagnosticSeverity.Error, true);

    public static DiagnosticDescriptor ShadersCannotHaveClassFields => new DiagnosticDescriptor($"{IdPrefix}0004", "Shaders cannot have class fields", "Shaders cannot have class fields (field '{0}')", "Emit", DiagnosticSeverity.Error, true);

    public static DiagnosticDescriptor ComputeShaderNumThreadsMustBeGreaterThan0 => new DiagnosticDescriptor($"{IdPrefix}0005", "Thread count must be greater than 0", "Thread count must be greater than 0 (thread '{0}')", "Emit", DiagnosticSeverity.Error, true);

    public static DiagnosticDescriptor ComputeShaderNumThreadsExceedsMaximum => new DiagnosticDescriptor($"{IdPrefix}0006", "Total thread count (x * y * z) must be lesser than maximum", "Total thread count (x * y * z) must be lesser than {0}", "Emit", DiagnosticSeverity.Error, true);

    public static DiagnosticDescriptor MethodGeneric => new DiagnosticDescriptor($"{IdPrefix}0007", "Method cannot be generic", "Shader methods cannot be generic (method {0})", "Emit", DiagnosticSeverity.Error, true);
    
    public static DiagnosticDescriptor MethodAbstract => new DiagnosticDescriptor($"{IdPrefix}0007", "Method cannot be abstract", "Shader methods cannot be abstract (method {0})", "Emit", DiagnosticSeverity.Error, true);
    
    public static DiagnosticDescriptor ShaderDestructor => new DiagnosticDescriptor($"{IdPrefix}0008", "Shader shouldn't have destructor", "Shaders should not have destructors", "Emit", DiagnosticSeverity.Warning, true);
}
