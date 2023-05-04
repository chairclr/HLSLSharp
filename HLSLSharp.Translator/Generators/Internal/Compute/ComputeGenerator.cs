using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler.Generators.Internal.Compute;

internal class ComputeGenerator : IInternalShaderGenerator
{
    public void Execute(InternalShaderGenerationContext context)
    {
        Compilation compilation = context.Compilation;

        INamedTypeSymbol structSymbol = context.ShaderStructType;

        StringBuilder sb = new StringBuilder();

        if (!structSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            sb.AppendLine($"namespace {structSymbol.ContainingNamespace};");
        }

        sb.AppendLine($"partial struct {structSymbol.Name}");
        sb.AppendLine($"{{");
        sb.AppendLine($"#pragma warning disable CS0649");
        sb.AppendLine($"    private Vector3UI ThreadId;");
        sb.AppendLine($"#pragma warning restore CS0649");
        sb.AppendLine($"}}");

        context.AddSource($"Compute.{structSymbol.Name}.g.cs", sb.ToString());

    }
}
