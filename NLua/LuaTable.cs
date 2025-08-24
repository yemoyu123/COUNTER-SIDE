using System.Collections;
using KeraLua;
using NLua.Extensions;

namespace NLua;

public class LuaTable : LuaBase
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

	public ICollection Keys
	{
		get
		{
			if (!TryGet(out var lua))
			{
				return null;
			}
			return lua.GetTableDict(this).Keys;
		}
	}

	public ICollection Values
	{
		get
		{
			if (!TryGet(out var lua))
			{
				return new object[0];
			}
			return lua.GetTableDict(this).Values;
		}
	}

	public LuaTable(int reference, Lua interpreter)
		: base(reference, interpreter)
	{
	}

	public IDictionaryEnumerator GetEnumerator()
	{
		if (!TryGet(out var lua))
		{
			return null;
		}
		return lua.GetTableDict(this).GetEnumerator();
	}

	internal object RawGet(string field)
	{
		if (!TryGet(out var lua))
		{
			return null;
		}
		return lua.RawGetObject(_Reference, field);
	}

	internal void Push(KeraLua.Lua luaState)
	{
		luaState.GetRef(_Reference);
	}

	public override string ToString()
	{
		return "table";
	}
}
