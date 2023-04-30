using HLSLSharp.CoreLib;
using HLSLSharp.CoreLib.Shaders;

namespace HLSLSharp.Tests.ShaderLibrary;

[ComputeShader(1, 1, 1)]
public partial struct SuperSimpleCompute : IComputeShader
{
    [Kernel]
    public void Compute()
    {
        Vector3I v = new Vector3I(0, 0, 0);

        
    }
}

[ComputeShader(1, 1, 1)]
public partial struct ComputeCopy : IComputeShader
{
    [Kernel]
    public void Compute()
    {

    }
}
