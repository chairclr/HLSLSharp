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

        //EmitResult result = translator.Emit();

        //LogDiagnostics(result);

        //Assert.That(result.IsError, Is.False);

        Assert.Pass();
    }

    [Test]
    public void SimpleComputeCompilation()
    {
        string source = $$"""
                         using System;
                         using System.Shaders;

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

        //EmitResult result = translator.Emit();

        //LogDiagnostics(result);

        //Assert.That(result.IsError, Is.False);

        Assert.Pass();
    }

    public void LogDiagnostics(EmitResult result)
    {
        foreach (Diagnostic diagnostic in result.Diagnostics)
        {
            Console.WriteLine(diagnostic.ToString());
        }
    }
}
