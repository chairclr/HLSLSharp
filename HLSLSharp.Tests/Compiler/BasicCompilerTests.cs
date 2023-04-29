using System.Formats.Asn1;
using HLSLSharp.Compiler;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Tests.Compiler;

public class BasicCompilerTests
{
    [Test]
    public void BasicComputeCompilation()
    {
        string source = $$"""
                         using System;
                         using System.Shaders;
                         using System.Shaders.Registers;
                         using static System.Intrinsics;

                         [ComputeShader(64, 1, 1)]
                         public partial struct ParallelSqrt : IComputeShader
                         {
                            [Kernel]
                            public void Compute()
                            {
                                
                            }
                         }
                         """;

        SourceTranslator translator = new SourceTranslator(source);

        EmitResult result = translator.Emit();

        foreach (Diagnostic diagnostic in result.Diagnostics)
        {
            Console.WriteLine(diagnostic.ToString());
        }

        Assert.That(result.IsError, Is.False);

    }
}
