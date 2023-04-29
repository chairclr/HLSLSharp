namespace System.Shaders.Registers;

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public class RegisterAttribute : Attribute
{
    public RegisterAttribute(RegisterType type, int slot)
    {

    }
}
