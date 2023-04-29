using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.CoreLib.Generator.Vectors;

[Generator(LanguageNames.CSharp)]
public class AliasGenerator : IIncrementalGenerator
{
    private static readonly (string Postfix, string GenericType)[] AliasPostfixes = new (string Postfix, string GenericType)[] { ("", "float"), ("D", "double"), ("I", "int"), ("UI", "uint"), ("B", "bool") };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        bool FilterAliasables(SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node is not ClassDeclarationSyntax classDeclaration)
            {
                return false;
            }

            string fullTypeName = classDeclaration.Identifier.ValueText + classDeclaration.TypeParameterList?.ToString();

            return fullTypeName switch
            {
                "Vector1<T>" => true,
                "Vector2<T>" => true,
                "Vector3<T>" => true,
                "Vector4<T>" => true,
                _ => false,
            };
        };

        ImmutableArray<AliasTransform> TransformAliasables(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)context.Node;

            return AliasPostfixes.Select(x => new AliasTransform()
            {
                Namespace = "System",
                GenericVectorTypeName = classDeclaration.Identifier.ValueText,
                NewVectorTypeName = classDeclaration.Identifier.ValueText + x.Postfix,
                GenericType = x.GenericType
            }).ToImmutableArray();
        };

        IncrementalValuesProvider<ImmutableArray<AliasTransform>> aliasTransformCollection = context.SyntaxProvider.CreateSyntaxProvider(FilterAliasables, TransformAliasables);

        IncrementalValuesProvider<AliasTransform> aliasTransforms = aliasTransformCollection.SelectMany((x, _) => x);

        context.RegisterSourceOutput(aliasTransforms, (spc, aliasInfo) =>
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"global using {aliasInfo.NewVectorTypeName} = System.{aliasInfo.GenericVectorTypeName}<{aliasInfo.GenericType}>;");

            spc.AddSource($"Alias.{aliasInfo.Namespace}.{aliasInfo.NewVectorTypeName}.g.cs", sb.ToString());
        });
    }

    private sealed class AliasTransform
    {
        public string? Namespace;

        public string GenericVectorTypeName = "";

        public string NewVectorTypeName = "";

        public string GenericType = "";
    }
}
