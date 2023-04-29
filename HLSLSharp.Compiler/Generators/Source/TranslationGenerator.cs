using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace HLSLSharp.Compiler.Generators.Source;

[Generator(LanguageNames.CSharp)]
internal class TranslationGenerator : ISourceGenerator
{
    private static readonly string ComputeShaderAttributeFullName = "System.Shaders.ComputeShaderAttribute";

    public void Execute(GeneratorExecutionContext context)
    {

        if (!Debugger.IsAttached)
            Debugger.Launch();

        Compilation compilation = context.Compilation;

        INamedTypeSymbol computeShaderAttributeSymbol = compilation.GetTypeByMetadataName(ComputeShaderAttributeFullName)!;

        IEnumerable<StructDeclarationSyntax> structNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes().OfType<StructDeclarationSyntax>());

        IEnumerable<StructDeclarationSyntax> computeStructNodes = structNodes.Where(node => 
            compilation.GetSemanticModel(node.SyntaxTree).GetDeclaredSymbol(node)!.GetAttributes()
            .Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, computeShaderAttributeSymbol)));

        foreach (StructDeclarationSyntax structDeclarationSyntax in computeStructNodes)
        {
            SyntaxTree tree = structDeclarationSyntax.SyntaxTree;

            SemanticModel semanticModel = compilation.GetSemanticModel(tree);

            INamedTypeSymbol structSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(structDeclarationSyntax)!;

            SourceGeneratorTranslator translator = new SourceGeneratorTranslator(tree, structSymbol);

            EmitResult result = translator.Emit();

            foreach (Diagnostic diag in result.Diagnostics)
            {
                context.ReportDiagnostic(diag);
            }

            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(structSymbol.ContainingNamespace.Name))
            {
                sb.AppendLine($"namespace {structSymbol.ContainingNamespace};");
            }

            sb.AppendLine($"partial struct {structSymbol.Name}");
            sb.AppendLine($"{{");

            foreach (SyntaxNode addedNode in translator.NodesAddedToShaderStruct)
            {
                sb.AppendLine(addedNode.ToFullString());
            }

            sb.AppendLine($"    public static string GetHLSLSource()");
            sb.AppendLine($"    {{");
            sb.AppendLine($"""""
                                    return 
                                        """"
                                        {result.Result}
                                        """";
                            """"");
            sb.AppendLine($"    }}");



            sb.AppendLine($"}}");

            context.AddSource($"{structSymbol.Name}.HLSLBuilder.g.cs", sb.ToString());

            foreach (InternalGeneratorSource generatedSource in translator.InternalGeneratedSourceText)
            {
                context.AddSource($"{structSymbol.Name}.InternalGenerator.{generatedSource.HintName}", generatedSource.Source);
            }
        }

    }

    public void Initialize(GeneratorInitializationContext context)
    {
        
    }
}
