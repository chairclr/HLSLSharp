namespace System.Shaders;

[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
public class ComputeShaderAttribute : Attribute
{
    public ComputeShaderAttribute(int x, int y, int z)
    {

    }
}
