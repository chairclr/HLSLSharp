using HLSLSharp.Compiler;
using HLSLSharp.Translator;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Tests.Compiler;

public class CompilerTests
{
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
