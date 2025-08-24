using System;
using KeraLua;

namespace NLua.Event;

public class DebugHookEventArgs : EventArgs
{
	public LuaDebug LuaDebug { get; }

	public DebugHookEventArgs(LuaDebug luaDebug)
	{
		LuaDebug = luaDebug;
	}
}
