using System.Collections.Generic;
using System.Linq;
using HLSLSharp.Compiler.Emit;
using HLSLSharp.Compiler.Generators;
using HLSLSharp.Compiler.Generators.Internal.Compute;
using HLSLSharp.Compiler.Generators.Internal.Vectors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HLSLSharp.Compiler;

public class TopLevelTranslator
{
    protected CSharpCompilation Compilation;

    public SyntaxTree ShaderSyntaxTree { get; protected set; }

    protected CompilationUnitSyntax ShaderCompilationUnit;

    protected SemanticModel ShaderSemanticModel;

    public INamedTypeSymbol ShaderStructType { get; private set; }

    internal readonly List<SyntaxNode> NodesAddedToShaderStruct = new List<SyntaxNode>();

    internal IEnumerable<InternalGeneratorSource> InternalGeneratedSourceText = Enumerable.Empty<InternalGeneratorSource>();

    internal virtual IInternalGenerator[] InternalGenerators => new IInternalGenerator[] { new AliasGenerator(), new ComputeGenerator() };

    public TopLevelTranslator(SyntaxTree shaderSyntaxTree, string shaderStructFullyQualifiedName)
    {
        ShaderSyntaxTree = shaderSyntaxTree;

        ShaderCompilationUnit = ShaderSyntaxTree.GetCompilationUnitRoot();

        Compilation = CSharpCompilation.Create($"__Translation")
            .AddReferences(CoreLibProvider.Reference)
            .AddSyntaxTrees(ShaderSyntaxTree);

        ShaderSemanticModel = Compilation.GetSemanticModel(ShaderSyntaxTree);

        ShaderStructType = Compilation.GetTypeByMetadataName(shaderStructFullyQualifiedName)!;

        InternalGeneratedSourceText = GenerateInternalSource();

        Compilation = Compilation
            .AddSyntaxTrees(InternalGeneratedSourceText.Select(x => x.SyntaxTree));

        ShaderSemanticModel = Compilation.GetSemanticModel(ShaderSyntaxTree);

        ShaderStructType = Compilation.GetTypeByMetadataName(shaderStructFullyQualifiedName)!;
    }

    public TopLevelTranslator(SyntaxTree shaderSyntaxTree, StructDeclarationSyntax shaderDeclaration)
    {
        ShaderSyntaxTree = shaderSyntaxTree;

        ShaderCompilationUnit = ShaderSyntaxTree.GetCompilationUnitRoot();

        Compilation = CSharpCompilation.Create($"__Translation")
            .AddReferences(CoreLibProvider.Reference)
            .AddSyntaxTrees(ShaderSyntaxTree);

        ShaderSemanticModel = Compilation.GetSemanticModel(ShaderSyntaxTree);

        ShaderStructType = ShaderSemanticModel.GetDeclaredSymbol(shaderDeclaration)!;

        InternalGeneratedSourceText = GenerateInternalSource();

        Compilation = Compilation
            .AddSyntaxTrees(InternalGeneratedSourceText.Select(x => x.SyntaxTree));

        ShaderSemanticModel = Compilation.GetSemanticModel(ShaderSyntaxTree);

        ShaderStructType = ShaderSemanticModel.GetDeclaredSymbol(shaderDeclaration)!;
    }

    public TopLevelTranslator(SyntaxTree shaderSyntaxTree, INamedTypeSymbol shaderStructType)
    {
        ShaderSyntaxTree = shaderSyntaxTree;

        ShaderCompilationUnit = ShaderSyntaxTree.GetCompilationUnitRoot();

        Compilation = CSharpCompilation.Create($"__Translation")
            .AddReferences(CoreLibProvider.Reference)
            .AddSyntaxTrees(ShaderSyntaxTree);

        ShaderSemanticModel = Compilation.GetSemanticModel(ShaderSyntaxTree);

        ShaderStructType = Compilation.GetTypeByMetadataName(shaderStructType.ToDisplayString())!;

        InternalGeneratedSourceText = GenerateInternalSource();

        Compilation = Compilation
            .AddSyntaxTrees(InternalGeneratedSourceText.Select(x => x.SyntaxTree));

        ShaderSemanticModel = Compilation.GetSemanticModel(ShaderSyntaxTree);

        ShaderStructType = Compilation.GetTypeByMetadataName(shaderStructType.ToDisplayString())!;
    }

    private IEnumerable<InternalGeneratorSource> GenerateInternalSource()
    {
        IEnumerable<InternalGeneratorSource> newSources = Enumerable.Empty<InternalGeneratorSource>();

        foreach (IInternalGenerator generator in InternalGenerators)
        {
            newSources = newSources.Concat(ApplyGeneration(generator));
        }

        return newSources;
    }

    private IEnumerable<InternalGeneratorSource> ApplyGeneration(IInternalGenerator generator)
    {
        InternalGenerationContext context = new InternalGenerationContext(Compilation, ShaderSyntaxTree, ShaderStructType);

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
