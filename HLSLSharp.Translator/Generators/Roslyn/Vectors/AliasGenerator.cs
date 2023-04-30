using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler.Generators.Source.Roslyn;

/// <summary>
/// Roslyn source generator responsible for generating vector aliases
/// such as <see cref="Vector2I"/>, <see cref="Vector4UI"/>, etc.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class AliasGenerator : ISourceGenerator
{
    private static readonly (string Postfix, string GenericType)[] AliasPostfixes = new (string Postfix, string GenericType)[] { ("", "float"), ("D", "double"), ("I", "int"), ("UI", "uint"), ("B", "bool") };

    public void Execute(GeneratorExecutionContext context)
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

    public void Initialize(GeneratorInitializationContext context)
    {

    }
}
