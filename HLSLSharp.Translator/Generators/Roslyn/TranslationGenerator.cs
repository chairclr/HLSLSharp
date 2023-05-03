using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HLSLSharp.Translator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Compiler.Generators.Roslyn;

/// <summary>
/// Roslyn source generator responsible for applying Translators to a project
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class TranslationGenerator : ISourceGenerator
{
    private static readonly string ComputeShaderAttributeFullName = "HLSLSharp.CoreLib.Shaders.ComputeShaderAttribute";

    public void Execute(GeneratorExecutionContext context)
    {
        Compilation compilation = context.Compilation;

        RoslynProjectTranslator translator = new RoslynProjectTranslator((CSharpCompilation)compilation);

        ProjectEmitResult result = translator.Emit();

        foreach (InternalGeneratorSource source in translator.InternalGeneratedSources)
        {
            context.AddSource(source.HintName, source.Source);
        }

        foreach (Diagnostic diagnostic in result.AllDiagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }

        INamedTypeSymbol computeShaderAttributeSymbol = compilation.GetTypeByMetadataName(ComputeShaderAttributeFullName)!;

        IEnumerable<StructDeclarationSyntax> structNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes().OfType<StructDeclarationSyntax>());

        IEnumerable<StructDeclarationSyntax> computeStructNodes = structNodes.Where(node =>
            compilation.GetSemanticModel(node.SyntaxTree).GetDeclaredSymbol(node)!.GetAttributes()
            .Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, computeShaderAttributeSymbol)));

        List<INamedTypeSymbol> computeShaderTypes = computeStructNodes
            .Select(x => compilation.GetSemanticModel(x.SyntaxTree).GetDeclaredSymbol(x)!)
            .ToList();

        StringBuilder sb = new StringBuilder();

        foreach (INamedTypeSymbol shaderType in computeShaderTypes)
        {
            sb.Clear();

            string shaderSource = result.ShaderEmitResults.Single(x => x.FullyQualifiedShaderTypeName == $"{shaderType.ContainingNamespace}.{shaderType.MetadataName}").Result ?? "";

            if (!shaderType.ContainingNamespace.IsGlobalNamespace)
            {
                sb.AppendLine($"namespace {shaderType.ContainingNamespace};");
            }

            sb.AppendLine($"partial struct {shaderType.Name}");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public static string GetHLSLSource()");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        return");
            sb.AppendLine($"              \"\"\"\"");
            foreach (string line in shaderSource.Split('\n'))
            {
            sb.AppendLine($"              {line}");
            }
            sb.AppendLine($"              \"\"\"\";");
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");

            context.AddSource($"Source.{shaderType.MetadataName}.g.cs", sb.ToString());
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {

    }
}
