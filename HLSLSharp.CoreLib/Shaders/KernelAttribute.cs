using System;

namespace HLSLSharp.CoreLib.Shaders;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class KernelAttribute : Attribute
{
    public KernelAttribute()
    {

    }
}
