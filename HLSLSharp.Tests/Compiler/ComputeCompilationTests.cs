using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLSLSharp.Compiler;
using HLSLSharp.Translator;

namespace HLSLSharp.Tests.Compiler;

public class ComputeCompilationTests : CompilerTests
{
    [Test]
    public void CompileRWBufferTest()
    {
        string source = $$"""
                         using HLSLSharp.CoreLib;
                         using HLSLSharp.CoreLib.Compute;
                         using HLSLSharp.CoreLib.Shaders;
                         using HLSLSharp.CoreLib.Shaders.Registers;

                         [ComputeShader(1, 1, 1)]
                         public partial struct SqrtBufferCompute : IComputeShader
                         {
                            [Register(RegisterType.UnorderedAccessView, 0)]
                            public RWBuffer<float> ReadWriteBuffer;

                            [Kernel]
                            public void Compute()
                            {
                                float s = ReadWriteBuffer[ThreadId.X];

                                float sqrtS = Intrinsics.Sqrt(s);

                                ReadWriteBuffer[ThreadId.X] = sqrtS;
                            }
                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        ProjectEmitResult result = translator.Emit();

        LogDiagnostics(result);

        Assert.That(result.Success, Is.True);
    }
}
