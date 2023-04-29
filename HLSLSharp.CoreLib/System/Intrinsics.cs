namespace System;

public class Intrinsics
{
    public static extern T Sqrt<T>(IScalar<T> x) where T : IFloatable;

    public static extern T Sqrt<T>(IVector<T> x) where T : IFloatable;
}
