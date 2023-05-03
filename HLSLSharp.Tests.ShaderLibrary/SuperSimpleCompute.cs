using System;
using System.Numerics;
using System.Threading;
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

        Vector2UI xy = ThreadId.XY;

        Vector4UI xyxy = xy.XYXY;

        Vector2UI uh = ThreadId.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX.XX;

    }
}

[ComputeShader(1, 1, 1)]
public partial struct ComputeCopy : IComputeShader
{
    [Kernel]
    public void Compute()
    {
        Vector3UI yxz = ThreadId.YXZ;
    }
}

[ComputeShader(1, 1, 1)]
public partial struct NotSoShrimple : IComputeShader
{
    [Kernel]
    public void Compute()
    {
        float s = (float)ThreadId.X;

        float sqrtS = Intrinsics.Sqrt(s);

        Vector2 v = new Vector2((float)ThreadId.X, (float)ThreadId.Y);

        float sqrtV = Intrinsics.Sqrt(v);
    }
}
