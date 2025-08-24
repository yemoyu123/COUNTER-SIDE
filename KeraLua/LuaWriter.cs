using System;
using System.Runtime.InteropServices;
using System.Security;

namespace KeraLua;

[SuppressUnmanagedCodeSecurity]
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int LuaWriter(IntPtr L, IntPtr p, UIntPtr size, IntPtr ud);
