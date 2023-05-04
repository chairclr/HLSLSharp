using HLSLSharp.Compiler;
using HLSLSharp.Translator;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Tests.Compiler;

public class BasicCompilerTests : CompilerTests
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

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);

            Assert.That(result.ShaderEmitResults.Single().Result, Contains.Substring($"uint3 threadIdCopy = ThreadId;"));
            Assert.That(result.ShaderEmitResults.Single().Result, Contains.Substring($"uint2 threadIdXY = ThreadId.xy;"));
            Assert.That(result.ShaderEmitResults.Single().Result, Contains.Substring($"uint threadIdX = ThreadId.x;"));
        });
    }

    [Test]
    public void SimpleIfStatementComputeCompilation()
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
                                uint threadIdX = ThreadId.X;

                                if (threadIdX > 10)
                                {
                                    threadIdX = 69;
                                }
                            }
                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        ProjectEmitResult result = translator.Emit();

        LogDiagnostics(result);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);

            Assert.That(result.ShaderEmitResults.Single().Result, Contains.Substring($"if (threadIdX > 10)"));
        });
    }

    [Test]
    public void SimpleOperationComputeCompilation()
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
                                uint threadIdX = ThreadId.X;

                                uint val = threadIdX * (ThreadId.X - (ThreadId.Y * ThreadId.ZZZ.X));

                                val /= 2;

                                val *= 2;
                            }
                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        ProjectEmitResult result = translator.Emit();

        LogDiagnostics(result);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);

            Assert.That(result.ShaderEmitResults.Single().Result, Contains.Substring($"uint val = threadIdX * (ThreadId.x - (ThreadId.y * ThreadId.zzz.x));"));
        });
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
}
