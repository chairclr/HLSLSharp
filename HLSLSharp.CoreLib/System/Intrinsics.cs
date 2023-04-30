namespace System;

// https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-intrinsic-functions

public class Intrinsics
{
    public static extern T Sqrt<T>(IScalar<T> x) where T : IFloatable;

    public static extern T Sqrt<T>(IVector<T> x) where T : IFloatable;
}
