using System;
using System.Collections.Concurrent;
using KeraLua;

namespace NLua;

internal class ObjectTranslatorPool
{
	private static volatile ObjectTranslatorPool _instance = new ObjectTranslatorPool();

	private ConcurrentDictionary<KeraLua.Lua, ObjectTranslator> translators = new ConcurrentDictionary<KeraLua.Lua, ObjectTranslator>();

	public static ObjectTranslatorPool Instance => _instance;

	public void Add(KeraLua.Lua luaState, ObjectTranslator translator)
	{
		if (!translators.TryAdd(luaState, translator))
		{
			throw new ArgumentException("An item with the same key has already been added. ", "luaState");
		}
	}

	public ObjectTranslator Find(KeraLua.Lua luaState)
	{
		if (!translators.TryGetValue(luaState, out var value))
		{
			KeraLua.Lua mainThread = luaState.MainThread;
			if (!translators.TryGetValue(mainThread, out value))
			{
				throw new Exception("Invalid luaState, couldn't find ObjectTranslator");
			}
		}
		return value;
	}

	public void Remove(KeraLua.Lua luaState)
	{
		translators.TryRemove(luaState, out var _);
	}
}
