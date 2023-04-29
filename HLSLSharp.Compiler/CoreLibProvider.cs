using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler;

public class CoreLibProvider
{
    private static readonly string RuntimeResourceName = "HLSLSharp.Compiler.Embedded_Resources.System.Runtime.dll";

    public static PortableExecutableReference Reference;

    static CoreLibProvider()
    {
        Reference = MetadataReference.CreateFromStream(typeof(CoreLibProvider).Assembly.GetManifestResourceStream(RuntimeResourceName)!);
    }
}
