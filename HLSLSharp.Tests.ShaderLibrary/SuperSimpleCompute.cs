using HLSLSharp.CoreLib;
using HLSLSharp.CoreLib.Shaders;

namespace HLSLSharp.Tests.ShaderLibrary;

[ComputeShader(1, 1, 1)]
public partial struct SuperSimpleCompute : IComputeShader
{
    [Kernel]
    public void Compute()
    {

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
