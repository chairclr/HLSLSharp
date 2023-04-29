using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Compiler;

public class CoreLibProvider
{
    private static readonly string RuntimeResourceName = "HLSLSharp.Compiler.Embedded_Resources.System.Runtime.dll";

    public PortableExecutableReference Reference;

    public CoreLibProvider()
    {
        Reference = MetadataReference.CreateFromStream(typeof(CoreLibProvider).Assembly.GetManifestResourceStream(RuntimeResourceName)!);
    }
}
