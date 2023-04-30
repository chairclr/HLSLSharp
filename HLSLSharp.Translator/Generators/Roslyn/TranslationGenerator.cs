using System.Collections.Generic;
using System.Diagnostics;
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
    }

    public void Initialize(GeneratorInitializationContext context)
    {

    }
}
