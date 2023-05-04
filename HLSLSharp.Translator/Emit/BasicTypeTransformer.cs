using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HLSLSharp.Translator.Emit;

internal class BasicTypeTransformer
{
    private static readonly Dictionary<string, string> BasicTypeMappings = new Dictionary<string, string>()
    {
        { "float", "float" },
        { "double", "double" },
        { "int", "int" },
        { "uint", "uint" },
        { "bool", "bool" },

        { "HLSLSharp.CoreLib.Vector1<float>", "float1" },
        { "HLSLSharp.CoreLib.Vector2<float>", "float2" },
        { "HLSLSharp.CoreLib.Vector3<float>", "float3" },
        { "HLSLSharp.CoreLib.Vector4<float>", "float4" },

        { "HLSLSharp.CoreLib.Vector1<double>", "double1" },
        { "HLSLSharp.CoreLib.Vector2<double>", "double2" },
        { "HLSLSharp.CoreLib.Vector3<double>", "double3" },
        { "HLSLSharp.CoreLib.Vector4<double>", "double4" },

        { "HLSLSharp.CoreLib.Vector1<int>", "int1" },
        { "HLSLSharp.CoreLib.Vector2<int>", "int2" },
        { "HLSLSharp.CoreLib.Vector3<int>", "int3" },
        { "HLSLSharp.CoreLib.Vector4<int>", "int4" },

        { "HLSLSharp.CoreLib.Vector1<uint>", "uint1" },
        { "HLSLSharp.CoreLib.Vector2<uint>", "uint2" },
        { "HLSLSharp.CoreLib.Vector3<uint>", "uint3" },
        { "HLSLSharp.CoreLib.Vector4<uint>", "uint4" },

        { "HLSLSharp.CoreLib.Vector1<bool>", "bool1" },
        { "HLSLSharp.CoreLib.Vector2<bool>", "bool2" },
        { "HLSLSharp.CoreLib.Vector3<bool>", "bool3" },
        { "HLSLSharp.CoreLib.Vector4<bool>", "bool4" },
    };

    public static bool TryGetHLSLTypeName(INamedTypeSymbol csType, out string? hlslType)
    {
        string fullyQualifiedName = csType.ToString();

        return BasicTypeMappings.TryGetValue(fullyQualifiedName, out hlslType);
    }
}
