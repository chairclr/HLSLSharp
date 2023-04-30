using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator;
public static class SystemReferenceProvider
{
    public static IEnumerable<MetadataReference> References;

    static SystemReferenceProvider()
    {
        string assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

        References = new string[]
        {
            "System.Runtime.dll",
        }
        .Select(x => MetadataReference.CreateFromFile(Path.Combine(assemblyPath, x)))
        .Concat(new Type[]
        {
            typeof(object)
        }.Select(x => MetadataReference.CreateFromFile(x.Assembly.Location)));
    }
}
