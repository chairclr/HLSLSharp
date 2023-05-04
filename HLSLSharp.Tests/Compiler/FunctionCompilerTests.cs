using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLSLSharp.Compiler;
using HLSLSharp.Translator;

namespace HLSLSharp.Tests.Compiler;

public class FunctionCompilerTests : CompilerTests
{
    [Test]
    public void SimpleFunctionCompilation()
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

                            public void TestVoidParameterless()
                            {
                                
                            }

                            public void TestVoidInt(int x)
                            {
                                int xp1 = x + 1;
                            }

                            public void TestVoidVector(Vector4 x)
                            {
                                float x_xp1= x.X + 1f;
                            }

                            public float TestReturn()
                            {
                                return 1.67f;
                            }

                            public int TestAdd(int x, int y)
                            {
                                return x + y;
                            }
                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        ProjectEmitResult result = translator.Emit();

        LogDiagnostics(result);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);

            string shaderSource = result.ShaderEmitResults.Single().Result!;

            Assert.That(shaderSource, Contains.Substring($"void TestVoidParameterless()"));

            Assert.That(shaderSource, Contains.Substring($"void TestVoidInt(int x)"));

            Assert.That(shaderSource, Contains.Substring($"void TestVoidVector(float4 x)"));

            Assert.That(shaderSource, Contains.Substring($"float TestReturn()"));

            Assert.That(shaderSource, Contains.Substring($"return 1.67;"));

            Assert.That(shaderSource, Contains.Substring($"int TestAdd(int x, int y)"));

            Assert.That(shaderSource, Contains.Substring($"return x + y;"));
        });
    }

    [Test]
    public void InvalidMethodCompilation()
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

                            public void TestInvalidMethod<T>()
                            {
                                T x = new T();
                            }

                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        ProjectEmitResult result = translator.Emit();

        LogDiagnostics(result);

        Assert.That(result.Success, Is.False);
    }
}
