namespace System;

// https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-vector

#region Vector1
/// <summary>
/// Represents a 1-dimensional vector of
/// <typeparamref name="T"/>
/// </summary>
public partial class Vector1<T> : IVector<T> where T : IFloatable
{
    public extern Vector1(T x);
}

/// <summary>
/// Represents a 1-dimensional vector of <see cref="float"/>
/// </summary>
public partial class Vector1 : IVector<float>
{
    public extern Vector1(float x);
}

/// <summary>
/// Represents a 1-dimensional vector of <see cref="double"/>
/// </summary>
public partial class Vector1D : IVector<double>
{
    public extern Vector1D(double x);
}

/// <summary>
/// Represents a 1-dimensional vector of <see cref="int"/>
/// </summary>
public partial class Vector1I : IVector<int>
{
    public extern Vector1I(int x);
}

/// <summary>
/// Represents a 1-dimensional vector of <see cref="uint"/>
/// </summary>
public partial class Vector1UI : IVector<uint>
{
    public extern Vector1UI(uint x);
}

/// <summary>
/// Represents a 1-dimensional vector of <see cref="bool"/>
/// </summary>
public partial class Vector1B : IVector<bool>
{
    public extern Vector1B(bool x);
}
#endregion

#region Vector2
/// <summary>
/// Represents a 2-dimensional vector of
/// <typeparamref name="T"/>
/// </summary>
public partial class Vector2<T> : IVector<T> where T : IFloatable
{
    public extern Vector2(T x, T y);
}

/// <summary>
/// Represents a 2-dimensional vector of <see cref="float"/>
/// </summary>
public partial class Vector2 : IVector<float>
{
    public extern Vector2(float x, float y);
}

/// <summary>
/// Represents a 2-dimensional vector of <see cref="double"/>
/// </summary>
public partial class Vector2D : IVector<double>
{
    public extern Vector2D(double x, double y);
}

/// <summary>
/// Represents a 2-dimensional vector of <see cref="int"/>
/// </summary>
public partial class Vector2I : IVector<int>
{
    public extern Vector2I(int x, int y);
}

/// <summary>
/// Represents a 2-dimensional vector of <see cref="uint"/>
/// </summary>
public partial class Vector2UI : IVector<uint>
{
    public extern Vector2UI(uint x, uint y);
}

/// <summary>
/// Represents a 2-dimensional vector of <see cref="bool"/>
/// </summary>
public partial class Vector2B : IVector<bool>
{
    public extern Vector2B(bool x, bool y);
}
#endregion

#region Vector3
/// <summary>
/// Represents a 3-dimensional vector of
/// <typeparamref name="T"/>
/// </summary>
public partial class Vector3<T> : IVector<T> where T : IFloatable
{
    public extern Vector3(T x, T y, T z);
}

/// <summary>
/// Represents a 3-dimensional vector of <see cref="float"/>
/// </summary>
public partial class Vector3 : IVector<float>
{
    public extern Vector3(float x, float y, float z);
}

/// <summary>
/// Represents a 3-dimensional vector of <see cref="double"/>
/// </summary>
public partial class Vector3D : IVector<double>
{
    public extern Vector3D(double x, double y, double z);
}

/// <summary>
/// Represents a 3-dimensional vector of <see cref="int"/>
/// </summary>
public partial class Vector3I : IVector<int>
{
    public extern Vector3I(int x, int y, int z);
}

/// <summary>
/// Represents a 3-dimensional vector of <see cref="uint"/>
/// </summary>
public partial class Vector3UI : IVector<uint>
{
    public extern Vector3UI(uint x, uint y, uint z);
}

/// <summary>
/// Represents a 3-dimensional vector of <see cref="bool"/>
/// </summary>
public partial class Vector3B : IVector<bool>
{
    public extern Vector3B(bool x, bool y, bool z);
}
#endregion

#region Vector4
/// <summary>
/// Represents a 4-dimensional vector of
/// <typeparamref name="T"/>
/// </summary>
public partial class Vector4<T> : IVector<T> where T : IFloatable
{
    public extern Vector4(T x, T y, T z, T w);
}

/// <summary>
/// Represents a 4-dimensional vector of <see cref="float"/>
/// </summary>
public partial class Vector4 : IVector<float>
{
    public extern Vector4(float x, float y, float z, float w);
}

/// <summary>
/// Represents a 4-dimensional vector of <see cref="double"/>
/// </summary>
public partial class Vector4D : IVector<double>
{
    public extern Vector4D(double x, double y, double z, double w);
}

/// <summary>
/// Represents a 4-dimensional vector of <see cref="int"/>
/// </summary>
public partial class Vector4I : IVector<int>
{
    public extern Vector4I(int x, int y, int z, int w);
}

/// <summary>
/// Represents a 4-dimensional vector of <see cref="uint"/>
/// </summary>
public partial class Vector4UI : IVector<uint>
{
    public extern Vector4UI(uint x, uint y, uint z, uint w);
}

/// <summary>
/// Represents a 4-dimensional vector of <see cref="bool"/>
/// </summary>
public partial class Vector4B : IVector<bool>
{
    public extern Vector4B(bool x, bool y, bool z, bool w);
}
#endregion