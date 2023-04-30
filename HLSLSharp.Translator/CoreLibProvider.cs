using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator;

internal static class CoreLibProvider
{
    private static readonly string RuntimeResourceName = "HLSLSharp.Translator.Embedded_Resources.HLSLSharp.CoreLib.dll";

    public static readonly PortableExecutableReference Reference;

    static CoreLibProvider()
    {
        Reference = MetadataReference.CreateFromStream(typeof(CoreLibProvider).Assembly.GetManifestResourceStream(RuntimeResourceName)!);
    }
}
