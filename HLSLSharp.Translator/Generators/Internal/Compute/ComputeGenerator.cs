using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler.Generators.Internal.Compute;

internal class ComputeGenerator : IInternalShaderGenerator
{
    private static readonly string ComputeShaderAttributeFullName = "System.Shaders.ComputeShaderAttribute";

    public void Execute(InternalShaderGenerationContext context)
    {
        Compilation compilation = context.Compilation;

        INamedTypeSymbol structSymbol = context.ShaderStructType;

        StringBuilder sb = new StringBuilder();

        if (!string.IsNullOrEmpty(structSymbol.ContainingNamespace.Name))
        {
            sb.AppendLine($"namespace {structSymbol.ContainingNamespace.Name};");
        }

        sb.AppendLine($"partial struct {structSymbol.Name}");
        sb.AppendLine($"{{");
        sb.AppendLine($"    private Vector3UI ThreadId;");
        sb.AppendLine($"}}");

        context.AddSource($"Compute.{structSymbol.Name}.g.cs", sb.ToString());

    }
}
