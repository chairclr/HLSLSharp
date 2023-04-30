using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.CoreLib.Generator.Vectors;

[Generator(LanguageNames.CSharp)]
public class SwizzleGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        bool FilterSwizzleables(SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node is not ClassDeclarationSyntax classDeclaration)
            {
                return false;
            }

            if (classDeclaration.BaseList is null)
            {
                return false;
            }

            SeparatedSyntaxList<BaseTypeSyntax> types = classDeclaration.BaseList.Types;

            if (types.Count < 1)
            {
                return false;
            }

            string firstType = types.First().Type.ToString();

            if (firstType.StartsWith("IVector<"))
            {
                return true;
            }

            return false;
        };

        SwizzleTransform TransformSwizzleables(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)context.Node;

            SwizzleTransform GetTransformInfo(int vectorLength, string type)
            {
                SwizzleTransform transform = new SwizzleTransform()
                {
                    Namespace = "HLSLSharp.CoreLib",
                    VectorTypeName = classDeclaration.Identifier.ValueText + classDeclaration.TypeParameterList?.ToString(),
                };

                GenerateSwizzles(vectorLength, type, transform);

                return transform;
            };

            SeparatedSyntaxList<BaseTypeSyntax> types = classDeclaration.BaseList!.Types;

            string firstType = types.First().Type.ToString();

            string fullTypeName = classDeclaration.Identifier.ValueText + classDeclaration.TypeParameterList?.ToString();

            return fullTypeName switch
            {
                "Vector1<T>" => GetTransformInfo(1, "T"),
                "Vector2<T>" => GetTransformInfo(2, "T"),
                "Vector3<T>" => GetTransformInfo(3, "T"),
                "Vector4<T>" => GetTransformInfo(4, "T"),
                _ => throw new Exception("Invalid Vector"),
            };
        };

        IncrementalValuesProvider<SwizzleTransform> swizzleTransforms = context.SyntaxProvider.CreateSyntaxProvider(FilterSwizzleables, TransformSwizzleables);

        context.RegisterSourceOutput(swizzleTransforms, (spc, swizzleInfo) =>
        {
            StringBuilder sb = new StringBuilder();

            if (swizzleInfo.Namespace is not null)
            {
                sb.AppendLine($"namespace {swizzleInfo.Namespace};");
            }

            sb.AppendLine($"public partial class {swizzleInfo.VectorTypeName}");
            sb.AppendLine("{");
            foreach ((string type, string name) in swizzleInfo.SwizzledFields)
            {
                sb.AppendLine($"    public {type} {name};");
            }
            sb.AppendLine("}");

            spc.AddSource($"Swizzle.{swizzleInfo.Namespace}.{swizzleInfo.VectorTypeName.Replace("<T>", "T")}.g.cs", sb.ToString());
        });
    }

    private static void GenerateSwizzles(int vectorLength, string type, SwizzleTransform swizzleTransform)
    {
        char[] coordinateNames = { 'X', 'Y', 'Z', 'W' };
        //char[] colorNames = { 'R', 'G', 'B', 'A' };

        HashSet<string> coordSwizzles = new HashSet<string>();
        //HashSet<string> colorSwizzles = new HashSet<string>();

        char[] validCoords = coordinateNames.Take(vectorLength).ToArray();
        //char[] validColors = colorNames.Take(vectorLength).ToArray();

        GenerateCombinations(validCoords, coordSwizzles);
        //GenerateCombinations(validColors, colorSwizzles);

        foreach (string coordSwizzle in coordSwizzles)
        {
            if (coordSwizzle.Length == 1)
            {
                swizzleTransform.SwizzledFields.Add((type, coordSwizzle));
            }
            else
            {
                swizzleTransform.SwizzledFields.Add(("Vector" + coordSwizzle.Length + "<" + type + ">", coordSwizzle));
            }
        }

        //foreach (string colorSwizzle in colorSwizzles)
        //{
        //    if (colorSwizzle.Length == 1)
        //    {
        //        swizzleTransform.SwizzledFields.Add((type, colorSwizzle));
        //    }
        //    else
        //    {
        //        swizzleTransform.SwizzledFields.Add(("Vector" + vectorLength + "<" + type + ">", colorSwizzle));
        //    }
        //}
    }

    private static void GenerateCombinations(char[] validChars, HashSet<string> combinations)
    {
        int validLength = validChars.Length;

        for (int length = 1; length <= 4; length++)
        {
            char[] combo = new char[length];

            for (int i = 0; i < length; i++)
            {
                combo[i] = validChars[0];
            }

            while (true)
            {
                combinations.Add(new string(combo));

                int j = length - 1;

                while (j >= 0 && combo[j] == validChars[validLength - 1])
                {
                    j--;
                }

                if (j < 0)
                {
                    break;
                }

                combo[j] = validChars[Array.IndexOf(validChars, combo[j]) + 1];

                for (int k = j + 1; k < length; k++)
                {
                    combo[k] = validChars[0];
                }
            }
        }
    }

    private sealed class SwizzleTransform
    {
        public string? Namespace;

        public string VectorTypeName = "";

        public List<(string type, string name)> SwizzledFields = new List<(string type, string name)>();
    }
}
