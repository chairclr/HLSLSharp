using System.Collections.Generic;
using System.Linq;
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
