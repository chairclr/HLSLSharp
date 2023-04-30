
# [HLSLSharp Solution](https://github.com/CEO-Chair/HLSLSharp)
 - [HLSLSharp.CoreLib](https://github.com/CEO-Chair/HLSLSharp/tree/master/HLSLSharp.CoreLib)
  - Stub library containing types like Vectors (float1, float2, ...) and matricies
  - [HLSLSharp.CoreLib.Generator](https://github.com/CEO-Chair/HLSLSharp/tree/master/HLSLSharp.CoreLib.Generator)
    - Generates Vector aliases for the HLSLSharp.CoreLib
    - Generates Vector [swizzles](https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx9-graphics-reference-asm-ps-registers-modifiers-source-register-swizzling)
  - [HLSLSharp.Translator](https://github.com/CEO-Chair/HLSLSharp/tree/master/HLSLSharp.Translator)
    - Generators
      - Internal
        - Custom "Source Generation" framework
        - Applies custom source generators to C# source code before it is translated
		- Once-per-project source generators
		- Once-per-shader source generators

   - Translators