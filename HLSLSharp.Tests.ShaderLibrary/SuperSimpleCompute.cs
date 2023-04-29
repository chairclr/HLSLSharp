using System;
using System.Shaders;

namespace HLSLSharp.Tests.ShaderLibrary;

[ComputeShader(1, 1, 1)]
public partial struct SuperSimpleCompute : IComputeShader
{
    [Kernel]
    public void Compute()
    { 
        Vector3UI threadIdCopy = ThreadId;

        Vector2UI threadIdXY = ThreadId.XY;

        uint threadIdX = ThreadId.X;
    }
}

[ComputeShader(1, 1, 1)]
public partial struct SuperSimpleCompute2 : IComputeShader
{
    [Kernel]
    public void Compute()
    {
        Vector3UI threadIdCopy = ThreadId;

        Vector2UI threadIdXY = ThreadId.XY;

        uint threadIdX = ThreadId.X;
    }
}