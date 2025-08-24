using System;
using System.Runtime.InteropServices;
using System.Security;

namespace KeraLua;

[SuppressUnmanagedCodeSecurity]
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate IntPtr LuaAlloc(IntPtr ud, IntPtr ptr, UIntPtr osize, UIntPtr nsize);
