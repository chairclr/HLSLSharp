using System;
using System.Collections.Generic;
using System.Text;

namespace HLSLSharp.Compiler.Generators;

internal interface IInternalGenerator
{
    public void Execute(InternalGenerationContext context);
}
