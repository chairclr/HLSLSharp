using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator.Emit;

internal class IntrinsicCallTransformer
{
    private static Dictionary<string, string> IntrinsicMappings = new Dictionary<string, string>()
    {
        {  "HLSLSharp.CoreLib.Intrinsics.Sqrt", "sqrt" },
    }; 

    public static bool TryGetIntrinsicMethodCall(IMethodSymbol methodSymbol, out string? intrinsicName)
    {
        methodSymbol = methodSymbol.OriginalDefinition;

        string fullyQualifiedName = $"{methodSymbol.ContainingType}.{methodSymbol.MetadataName}";

        return IntrinsicMappings.TryGetValue(fullyQualifiedName, out intrinsicName);
    }
}
