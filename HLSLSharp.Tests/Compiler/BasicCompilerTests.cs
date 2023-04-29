using HLSLSharp.Compiler;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Tests.Compiler;

public class BasicCompilerTests
{
    [Test]
    public void EmptyComputeCompilation()
    {
        string source = $$"""
                         using System;
                         using System.Shaders;

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

        EmitResult result = translator.Emit();

        LogDiagnostics(result);

        Assert.That(result.IsError, Is.False);
    }

    [Test]
    public void SimpleComputeCompilation()
    {
        string source = $$"""
                         using System;
                         using System.Shaders;

                         [ComputeShader(1, 1, 1)]
                         public partial struct EmptyCompute : IComputeShader
                         {
                            [Kernel]
                            public void Compute()
                            {
                                Vector3I threadIdCopy = ThreadId;

                                Vector2I threadIdXY = (Vector2I)ThreadId.XY;

                                int threadIdX = ThreadId.X;
                            }
                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        EmitResult result = translator.Emit();

        LogDiagnostics(result);

        Assert.That(result.IsError, Is.False);
    }

    public void LogDiagnostics(EmitResult result)
    {
        foreach (Diagnostic diagnostic in result.Diagnostics)
        {
            Console.WriteLine(diagnostic.ToString());
        }
    }
}
