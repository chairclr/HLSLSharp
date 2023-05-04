using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler.Generators.Internal.Vectors;

internal class AliasGenerator : IInternalProjectGenerator
{
    private static readonly (string Postfix, string GenericType)[] AliasPostfixes = new (string Postfix, string GenericType)[] { ("", "float"), ("D", "double"), ("I", "int"), ("UI", "uint"), ("B", "bool") };

    public void Execute(InternalProjectGenerationContext context)
    {
        Compilation compilation = context.Compilation;

        StringBuilder sb = new StringBuilder();

        for (int i = 1; i <= 4; i++)
        {
            INamedTypeSymbol vectorType = compilation.GetTypeByMetadataName($"HLSLSharp.CoreLib.Vector{i}`1")!;
            sb.Clear();

            sb.AppendLine($"#pragma warning disable CS8019");

            foreach ((string postfix, string genericType) in AliasPostfixes)
            {
                sb.AppendLine($"global using {vectorType.Name}{postfix} = {vectorType.ContainingNamespace}.{vectorType.Name}<{genericType}>;");
            }

            sb.AppendLine($"#pragma warning restore CS8019");

            context.AddSource($"Alias.{vectorType.Name}.g.cs", sb.ToString());
        }
    }
}
