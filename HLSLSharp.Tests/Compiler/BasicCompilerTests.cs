using HLSLSharp.Compiler;

namespace HLSLSharp.Tests.Compiler;

public class BasicCompilerTests
{
    [Test]
    public void BasicComputeCompilation()
    {
        string source = $$"""
                         [ComputeShader(64, 1, 1)]
                         public partial struct ParallelSqrt : IComputeShader
                         {
                            [Register(RegisterType.UnorderedAccessView, 0)]
                            private readonly ReadWriteBuffer<float> Buffer;

                            [Kernel]
                            public void Compute()
                            {
                                Buffer[ThreadId.x] = sqrt(Buffer[ThreadId.x]);
                            }
                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        Assert.Pass();
    }
}
