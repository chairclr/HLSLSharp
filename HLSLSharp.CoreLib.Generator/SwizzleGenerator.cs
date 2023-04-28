using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.CoreLib.Generator;

[Generator(LanguageNames.CSharp)]
public class SwizzleGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        bool FilterSwizzleables(SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node is StructDeclarationSyntax structDeclaration)
            {
                if (structDeclaration.BaseList is not null)
                {
                    SeparatedSyntaxList<BaseTypeSyntax> types = structDeclaration.BaseList.Types;

                    string firstType = types.First().Type.ToString();

                    if (firstType.StartsWith("IVector<"))
                    {
                        return true;
                    }
                }
            }

            return false;
        };


        SwizzleTransform TransformSwizzleables(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            if (context.Node is StructDeclarationSyntax structDeclaration)
            {
                if (structDeclaration.BaseList is not null)
                {
                    SwizzleTransform GenerateSwizzles(int vectorLength, string type)
                    {
                        SwizzleTransform transform = new SwizzleTransform()
                        {
                            Namespace = "System",
                            SwizzleType = type,
                            VectorType = structDeclaration.Identifier.ValueText + structDeclaration.TypeParameterList?.ToString(),
                        };

                        return transform;
                    };

                    SeparatedSyntaxList<BaseTypeSyntax> types = structDeclaration.BaseList.Types;

                    string firstType = types.First().Type.ToString();

                    switch (structDeclaration.Identifier.ValueText)
                    {
                        case "Vector1<T>":
                            return GenerateSwizzles(1, "T");
                        case "Vector2<T>":
                            return GenerateSwizzles(2, "T");
                        case "Vector3<T>":
                            return GenerateSwizzles(3, "T");
                        case "Vector4<T>":
                            return GenerateSwizzles(4, "T");

                        case "Vector1":
                            return GenerateSwizzles(1, "float");
                        case "Vector2":
                            return GenerateSwizzles(2, "float");
                        case "Vector3":
                            return GenerateSwizzles(3, "float");
                        case "Vector4":
                            return GenerateSwizzles(4, "float");

                        case "Vector1D":
                            return GenerateSwizzles(1, "double");
                        case "Vector2D":
                            return GenerateSwizzles(2, "double");
                        case "Vector3D":
                            return GenerateSwizzles(3, "double");
                        case "Vector4D":
                            return GenerateSwizzles(4, "double");

                        case "Vector1I":
                            return GenerateSwizzles(1, "int");
                        case "Vector2I":
                            return GenerateSwizzles(2, "int");
                        case "Vector3I":
                            return GenerateSwizzles(3, "int");
                        case "Vector4I":
                            return GenerateSwizzles(4, "int");

                        case "Vector1UI":
                            return GenerateSwizzles(1, "uint");
                        case "Vector2UI":
                            return GenerateSwizzles(2, "uint");
                        case "Vector3UI":
                            return GenerateSwizzles(3, "uint");
                        case "Vector4UI":
                            return GenerateSwizzles(4, "uint");

                        case "Vector1B":
                            return GenerateSwizzles(1, "bool");
                        case "Vector2B":
                            return GenerateSwizzles(2, "bool");
                        case "Vector3B":
                            return GenerateSwizzles(3, "bool");
                        case "Vector4B":
                            return GenerateSwizzles(4, "bool");
                    }
                }
            }

            throw new Exception("Oops.");
        };

        IncrementalValuesProvider<SwizzleTransform> swizzleTransforms = context.SyntaxProvider.CreateSyntaxProvider(FilterSwizzleables, TransformSwizzleables);

        context.RegisterSourceOutput(swizzleTransforms, (spc, swizzleInfo) =>
        {
            StringBuilder sb = new StringBuilder();

            if (swizzleInfo.Namespace is not null)
            {
                sb.AppendLine($"namespace {swizzleInfo.Namespace};");
            }

            sb.AppendLine("/// TODO");

            spc.AddSource($"Swizzle.{swizzleInfo.Namespace}.{swizzleInfo.VectorType.Replace("<T>", "T")}.g.cs", sb.ToString());
        });
    }

    private sealed class SwizzleTransform
    {
        public string? Namespace;

        public string VectorType = "";

        public string SwizzleType = "";

        public List<string> SwizzledFields = new List<string>();


    }
}
