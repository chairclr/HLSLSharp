using System;
using System.Numerics;

namespace HLSLSharp.CoreLib;

// https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-intrinsic-functions

public class Intrinsics
{
    public static extern T Sqrt<T>(INumber<T> x) where T : INumber<T>, IConvertible;

    public static extern T Sqrt<T>(IVector<T> x) where T : INumber<T>, IConvertible;
}
