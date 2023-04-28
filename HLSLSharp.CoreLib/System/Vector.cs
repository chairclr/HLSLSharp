namespace System;

// https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-vector

/// <summary>
/// Represents a 1-dimensional vector of
/// <typeparamref name="T"/>
/// </summary>
public partial class Vector1<T> : IVector<T> where T : IFloatable
{
    public extern Vector1();

    public extern Vector1(T x);
}

/// <summary>
/// Represents a 2-dimensional vector of
/// <typeparamref name="T"/>
/// </summary>
public partial class Vector2<T> : IVector<T> where T : IFloatable
{
    public extern Vector2();

    public extern Vector2(T x, T y);
}

/// <summary>
/// Represents a 3-dimensional vector of
/// <typeparamref name="T"/>
/// </summary>
public partial class Vector3<T> : IVector<T> where T : IFloatable
{
    public extern Vector3();

    public extern Vector3(T x, T y, T z);
}

/// <summary>
/// Represents a 4-dimensional vector of
/// <typeparamref name="T"/>
/// </summary>
public partial class Vector4<T> : IVector<T> where T : IFloatable
{
    public extern Vector4();

    public extern Vector4(T x, T y, T z, T w);
}