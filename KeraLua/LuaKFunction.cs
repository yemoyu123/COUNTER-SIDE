using System;
using System.Runtime.InteropServices;
using System.Security;

namespace KeraLua;

[SuppressUnmanagedCodeSecurity]
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int LuaKFunction(IntPtr L, int status, IntPtr ctx);
