using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler.Generators.Internal.Vectors;

internal class AliasGenerator : IInternalGenerator
{
    private static readonly (string Postfix, string GenericType)[] AliasPostfixes = new (string Postfix, string GenericType)[] { ("", "float"), ("D", "double"), ("I", "int"), ("UI", "uint"), ("B", "bool") };

    public void Execute(InternalGenerationContext context)
    {
        Compilation compilation = context.Compilation;

        StringBuilder sb = new StringBuilder();

        for (int i = 1; i <= 4; i++)
        {
            INamedTypeSymbol vectorType = compilation.GetTypeByMetadataName($"System.Vector{i}`1")!;
            sb.Clear();

            foreach ((string postfix, string genericType) in AliasPostfixes)
            {
                sb.AppendLine($"global using {vectorType.Name}{postfix} = System.{vectorType.Name}<{genericType}>;");
            }

            context.AddSource($"Alias.{vectorType.Name}.g.cs", sb.ToString());
        }
    }
}
