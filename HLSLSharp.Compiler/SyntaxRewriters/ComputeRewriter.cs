using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace HLSLSharp.Compiler.SyntaxRewriters;

internal class ComputeRewriter : SourceRewriter
{
    private static readonly string ComputeShaderAttributeFullName = "System.Shaders.ComputeShaderAttribute";

    private readonly INamedTypeSymbol ComputeShaderAttributeSymbol;

    public ComputeRewriter(SemanticModel semanticModel)
        : base(semanticModel)
    {
        ComputeShaderAttributeSymbol = SemanticModel.Compilation.GetTypeByMetadataName(ComputeShaderAttributeFullName)!;
    }

    public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node)
    {
        AttributeData? computeAttribute = SemanticModel.GetDeclaredSymbol(node)!.GetAttributes()
            .FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, ComputeShaderAttributeSymbol));

        if (computeAttribute is null)
        {
            return base.VisitStructDeclaration(node);
        }

        FieldDeclarationSyntax field =
            FieldDeclaration(
                VariableDeclaration(
                    QualifiedName(
                        AliasQualifiedName(
                            IdentifierName(
                                Token(SyntaxKind.GlobalKeyword)),
                            IdentifierName("System")),
                        IdentifierName("Vector3I")))
                .WithVariables(
                    SingletonSeparatedList(
                        VariableDeclarator(
                            Identifier("ThreadId")))))
            .WithModifiers(
                TokenList(
                    new[]{
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.ReadOnlyKeyword)}))
            .NormalizeWhitespace();

        node = node.AddMembers(field);

        RegisterNewMember(field);

        return base.VisitStructDeclaration(node);
    }
}
