using HLSLSharp.Compiler;
using HLSLSharp.Translator;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Tests.Compiler;

public class BasicCompilerTests
{
    [Test]
    public void EmptyComputeCompilation()
    {
        string source = $$"""
                         using HLSLSharp.CoreLib;
                         using HLSLSharp.CoreLib.Shaders;

                         [ComputeShader(1, 1, 1)]
                         public partial struct EmptyCompute : IComputeShader
                         {
                            [Kernel]
                            public void Compute()
                            {

                            }
                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        ProjectEmitResult result = translator.Emit();

        LogDiagnostics(result);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void SimpleComputeCompilation()
    {
        string source = $$"""
                         using HLSLSharp.CoreLib;
                         using HLSLSharp.CoreLib.Shaders;

                         [ComputeShader(1, 1, 1)]
                         public partial struct SuperSimpleCompute : IComputeShader
                         {
                            [Kernel]
                            public void Compute()
                            {
                                Vector3UI threadIdCopy = ThreadId;

                                Vector2UI threadIdXY = ThreadId.XY;

                                uint threadIdX = ThreadId.X;
                            }
                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        ProjectEmitResult result = translator.Emit();

        LogDiagnostics(result);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void SimpleComputeCompilationInvalidField()
    {
        string source = $$"""
                         using HLSLSharp.CoreLib;
                         using HLSLSharp.CoreLib.Shaders;

                         [ComputeShader(1, 1, 1)]
                         public partial struct SuperSimpleCompute : IComputeShader
                         {
                            public object ObjectTypeField;

                            [Kernel]
                            public void Compute()
                            {
                                Vector3UI threadIdCopy = ThreadId;

                                Vector2UI threadIdXY = ThreadId.XY;

                                uint threadIdX = ThreadId.X;
                            }
                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        ProjectEmitResult result = translator.Emit();

        LogDiagnostics(result);

        Assert.That(result.Success, Is.False);
    }

    public void LogDiagnostics(ProjectEmitResult result)
    {
        foreach (Diagnostic diagnostic in result.AllDiagnostics)
        {
            Console.WriteLine(diagnostic.ToString());
        }


        foreach (ShaderEmitResult shaderResult in result.ShaderEmitResults)
        {
            Console.WriteLine("//// ---- Emitted Shader Source ---- ////");
            Console.WriteLine($"//// ---- Type: {shaderResult.FullyQualifiedShaderTypeName} ---- ////");
            Console.WriteLine(shaderResult.Result);
        }
    }
}
