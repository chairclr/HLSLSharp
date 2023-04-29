using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using HLSLSharp.Compiler.Emit;
using HLSLSharp.Compiler.Generators;
using HLSLSharp.Compiler.Generators.Internal.Vectors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace HLSLSharp.Compiler;

public class Translator
{
    protected CSharpCompilation Compilation;

    public SyntaxTree ShaderSyntaxTree { get; protected set; }

    protected CompilationUnitSyntax ShaderCompilationUnit;

    protected SemanticModel ShaderSemanticModel;

    internal readonly List<SyntaxNode> NodesAddedToShaderStruct = new List<SyntaxNode>();

    internal IEnumerable<InternalGeneratorSource> InternalGeneratedSourceText = Enumerable.Empty<InternalGeneratorSource>();

    public Translator(SyntaxTree shaderSyntaxTree)
    {
        ShaderSyntaxTree = shaderSyntaxTree;

        ShaderCompilationUnit = ShaderSyntaxTree.GetCompilationUnitRoot();

        Compilation = CSharpCompilation.Create($"__Translation")
            .AddReferences(CoreLibProvider.Reference)
            .AddSyntaxTrees(ShaderSyntaxTree);

        ShaderSemanticModel = Compilation.GetSemanticModel(ShaderSyntaxTree);

        InternalGeneratedSourceText = GenerateInternalSource();

        Compilation = Compilation
            .AddSyntaxTrees(InternalGeneratedSourceText.Select(x => x.SyntaxTree));

        ShaderCompilationUnit = ShaderSyntaxTree.GetCompilationUnitRoot();

        ShaderSemanticModel = Compilation.GetSemanticModel(ShaderSyntaxTree);
    }

    private IEnumerable<InternalGeneratorSource> GenerateInternalSource()
    {
        IEnumerable<InternalGeneratorSource> newSources = Enumerable.Empty<InternalGeneratorSource>();

        newSources = newSources.Concat(ApplyGeneration<AliasGenerator>());

        return newSources;
    }

    private IEnumerable<InternalGeneratorSource> ApplyGeneration<T>() where T : IInternalGenerator, new()
    {
        IInternalGenerator generator = new T();

        InternalGenerationContext context = new InternalGenerationContext(Compilation);

        generator.Execute(context);

        return context.AdditionalSources;
    }

    public EmitResult Emit()
    {
        HLSLEmitter emitter = new HLSLEmitter(Compilation, ShaderSyntaxTree, ShaderCompilationUnit, ShaderSemanticModel);

        emitter.EmitHLSLSource();

        return new EmitResult(emitter.GetSource(), ShaderSemanticModel.GetDiagnostics(), ShaderSemanticModel.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error));
    }
}
