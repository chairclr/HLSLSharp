using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    private static readonly string ComputeShaderAttributeFullName = "System.Shaders.ComputeShaderAttribute";

    public void Execute(GeneratorExecutionContext context)
    {
        Compilation compilation = context.Compilation;

        INamedTypeSymbol computeShaderAttributeSymbol = compilation.GetTypeByMetadataName(ComputeShaderAttributeFullName)!;

        IEnumerable<StructDeclarationSyntax> structNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes().OfType<StructDeclarationSyntax>());

        IEnumerable<StructDeclarationSyntax> computeStructNodes = structNodes.Where(node =>
            compilation.GetSemanticModel(node.SyntaxTree).GetDeclaredSymbol(node)!.GetAttributes()
            .Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, computeShaderAttributeSymbol)));

        RoslynProjectTranslator translator = new RoslynProjectTranslator((CSharpCompilation)compilation);

        //foreach (StructDeclarationSyntax structDeclarationSyntax in computeStructNodes)
        //{
        //    SyntaxTree tree = structDeclarationSyntax.SyntaxTree;

        //    SemanticModel semanticModel = compilation.GetSemanticModel(tree);

        //    INamedTypeSymbol structSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(structDeclarationSyntax)!;



        //    //EmitResult result = translator.Emit();
        //    //
        //    //foreach (Diagnostic diag in )
        //    //{
        //    //    context.ReportDiagnostic(diag);
        //    //}

        //    StringBuilder sb = new StringBuilder();

        //    if (!string.IsNullOrEmpty(structSymbol.ContainingNamespace.Name))
        //    {
        //        sb.AppendLine($"namespace {structSymbol.ContainingNamespace};");
        //    }

        //    sb.AppendLine($"partial struct {structSymbol.Name}");
        //    sb.AppendLine($"{{");

        //    sb.AppendLine($"    public static string GetHLSLSource()");
        //    sb.AppendLine($"    {{");
        //    sb.AppendLine($"""""
        //                            return 
        //                                """"

        //                                """";
        //                    """"");
        //    sb.AppendLine($"    }}");



        //    sb.AppendLine($"}}");

        //    context.AddSource($"{structSymbol.Name}.HLSLBuilder.g.cs", sb.ToString());

        //    //foreach (InternalGeneratorSource generatedSource in translator.InternalGeneratedSourceText)
        //    //{
        //    //    context.AddSource($"{structSymbol.Name}.InternalGenerator.{generatedSource.HintName}", generatedSource.Source);
        //    //}
        //}
    }

    public void Initialize(GeneratorInitializationContext context)
    {

    }
}
