using System;

namespace KeraLua;

[Flags]
public enum LuaHookMask
{
	Disabled = 0,
	Call = 1,
	Return = 2,
	Line = 4,
	Count = 8
}
