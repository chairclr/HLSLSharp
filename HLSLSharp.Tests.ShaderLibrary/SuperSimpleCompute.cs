using System;
using System.Shaders;

namespace HLSLSharp.Tests.ShaderLibrary;

[ComputeShader(1, 1, 1)]
public partial struct SuperSimpleCompute : IComputeShader
{
    [Kernel]
    public void Compute()
    {
        Vector3I threadIdCopy = ThreadId;

        Vector2I threadIdXY = (Vector2I)ThreadId.XY;

        int threadIdX = ThreadId.X;
    }
}