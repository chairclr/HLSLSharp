using System;

namespace HLSLSharp.CoreLib.Compute;

// https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/sm5-object-rwbuffer

public class RWBuffer<T> where T : IConvertible
{
    public T this[uint index]
    {
        get { return default; }
        set {  }
    }

    extern T Load();

    extern void GetDimensions(out uint dim);
}