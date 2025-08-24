using System;
using System.Runtime.InteropServices;
using System.Security;

namespace KeraLua;

[SuppressUnmanagedCodeSecurity]
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int LuaFunction(IntPtr luaState);
