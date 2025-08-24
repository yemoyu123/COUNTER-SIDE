using KeraLua;
using NLua.Extensions;

namespace NLua;

public class LuaUserData : LuaBase
{
	public object this[string field]
	{
		get
		{
			if (!TryGet(out var lua))
			{
				return null;
			}
			return lua.GetObject(_Reference, field);
		}
		set
		{
			if (TryGet(out var lua))
			{
				lua.SetObject(_Reference, field, value);
			}
		}
	}

	public object this[object field]
	{
		get
		{
			if (!TryGet(out var lua))
			{
				return null;
			}
			return lua.GetObject(_Reference, field);
		}
		set
		{
			if (TryGet(out var lua))
			{
				lua.SetObject(_Reference, field, value);
			}
		}
	}

	public LuaUserData(int reference, Lua interpreter)
		: base(reference, interpreter)
	{
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
		luaState.GetRef(_Reference);
	}

	public override string ToString()
	{
		return "userdata";
	}
}
