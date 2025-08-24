using System;
using KeraLua;

namespace NLua;

public class LuaFunction : LuaBase
{
	internal readonly KeraLua.LuaFunction function;

	public LuaFunction(int reference, Lua interpreter)
		: base(reference, interpreter)
	{
		function = null;
	}

	public LuaFunction(KeraLua.LuaFunction nativeFunction, Lua interpreter)
		: base(0, interpreter)
	{
		function = nativeFunction;
	}

	internal object[] Call(object[] args, Type[] returnTypes)
	{
		if (!TryGet(out var lua))
		{
			return null;
		}
		return lua.CallFunction(this, args, returnTypes);
	}

	public object[] Call(params object[] args)
	{
		if (!TryGet(out var lua))
		{
			return null;
		}
		return lua.CallFunction(this, args);
	}

	internal void Push(KeraLua.Lua luaState)
	{
		if (TryGet(out var lua))
		{
			if (_Reference != 0)
			{
				luaState.RawGetInteger(LuaRegistry.Index, _Reference);
			}
			else
			{
				lua.PushCSFunction(function);
			}
		}
	}

	public override string ToString()
	{
		return "function";
	}

	public override bool Equals(object o)
	{
		if (!(o is LuaFunction luaFunction))
		{
			return false;
		}
		if (!TryGet(out var lua))
		{
			return false;
		}
		if (_Reference != 0 && luaFunction._Reference != 0)
		{
			return lua.CompareRef(luaFunction._Reference, _Reference);
		}
		return function == luaFunction.function;
	}

	public override int GetHashCode()
	{
		if (_Reference == 0)
		{
			return function.GetHashCode();
		}
		return _Reference;
	}
}
