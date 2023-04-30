using System;

namespace HLSLSharp.CoreLib.Shaders.Registers;

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public class RegisterAttribute : Attribute
{
    public RegisterAttribute(RegisterType type, int slot)
    {

    }
}
