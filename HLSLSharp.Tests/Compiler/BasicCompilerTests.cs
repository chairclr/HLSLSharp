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

    [TestCase(0, 1, 1, ExpectedResult = false)]     // < 0
    [TestCase(20, 20, 20, ExpectedResult = false)]  // > 1024
    [TestCase(-5, 1, -5, ExpectedResult = false)]   // < 0
    [TestCase(1, 1, 0, ExpectedResult = false)]     // < 0
    [TestCase(1, 1, 10000, ExpectedResult = false)] // > 1024
    [TestCase(10, 10, 10, ExpectedResult = true)]   // = 1000
    [TestCase(8, 16, 8, ExpectedResult = true)]     // = 1024
    public bool SimpleComputeCompilationInvalidNumThreads(int numx, int numy, int numz)
    {
        string source = $$"""
                         using HLSLSharp.CoreLib;
                         using HLSLSharp.CoreLib.Shaders;

                         [ComputeShader({{numx}}, {{numy}}, {{numz}})]
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

        return result.Success;
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
