using System;
using KeraLua;
using NLua.Extensions;

namespace NLua;

public class LuaThread : LuaBase, IEquatable<LuaThread>, IEquatable<KeraLua.Lua>, IEquatable<Lua>
{
	private KeraLua.Lua _luaState;

	private ObjectTranslator _translator;

	public KeraLua.Lua State => _luaState;

	public LuaThread MainThread
	{
		get
		{
			KeraLua.Lua mainThread = _luaState.MainThread;
			int top = mainThread.GetTop();
			mainThread.PushThread();
			object obj = _translator.GetObject(mainThread, -1);
			mainThread.SetTop(top);
			return (LuaThread)obj;
		}
	}

	public LuaThread(int reference, Lua interpreter)
		: base(reference, interpreter)
	{
		_luaState = interpreter.GetThreadState(reference);
		_translator = interpreter.Translator;
	}

	public int Reset()
	{
		int top = _luaState.GetTop();
		int result = _luaState.ResetThread();
		_luaState.SetTop(top);
		return result;
	}

	public void XMove(KeraLua.Lua to, object val, int index = 1)
	{
		int top = _luaState.GetTop();
		_translator.Push(_luaState, val);
		_luaState.XMove(to, index);
		_luaState.SetTop(top);
	}

	public void XMove(Lua to, object val, int index = 1)
	{
		int top = _luaState.GetTop();
		_translator.Push(_luaState, val);
		_luaState.XMove(to.State, index);
		_luaState.SetTop(top);
	}

	public void XMove(LuaThread thread, object val, int index = 1)
	{
		int top = _luaState.GetTop();
		_translator.Push(_luaState, val);
		_luaState.XMove(thread.State, index);
		_luaState.SetTop(top);
	}

	internal void Push(KeraLua.Lua luaState)
	{
		luaState.GetRef(_Reference);
	}

	public override string ToString()
	{
		return "thread";
	}

	public override bool Equals(object obj)
	{
		if (obj is LuaThread luaThread)
		{
			return State == luaThread.State;
		}
		if (obj is Lua lua)
		{
			return State == lua.State;
		}
		if (obj is KeraLua.Lua lua2)
		{
			return State == lua2;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public bool Equals(LuaThread other)
	{
		return State == other.State;
	}

	public bool Equals(KeraLua.Lua other)
	{
		return State == other;
	}

	public bool Equals(Lua other)
	{
		return State == other.State;
	}

	public static explicit operator KeraLua.Lua(LuaThread thread)
	{
		return thread.State;
	}

	public static explicit operator LuaThread(Lua interpreter)
	{
		return interpreter.Thread;
	}

	public static bool operator ==(LuaThread threadA, LuaThread threadB)
	{
		return threadA.State == threadB.State;
	}

	public static bool operator !=(LuaThread threadA, LuaThread threadB)
	{
		return threadA.State != threadB.State;
	}

	public static bool operator ==(LuaThread thread, KeraLua.Lua state)
	{
		return thread.State == state;
	}

	public static bool operator !=(LuaThread thread, KeraLua.Lua state)
	{
		return thread.State != state;
	}

	public static bool operator ==(KeraLua.Lua state, LuaThread thread)
	{
		return state == thread.State;
	}

	public static bool operator !=(KeraLua.Lua state, LuaThread thread)
	{
		return state != thread.State;
	}

	public static bool operator ==(LuaThread thread, Lua interpreter)
	{
		return thread.State == interpreter.State;
	}

	public static bool operator !=(LuaThread thread, Lua interpreter)
	{
		return thread.State != interpreter.State;
	}

	public static bool operator ==(Lua interpreter, LuaThread thread)
	{
		return interpreter.State == thread.State;
	}

	public static bool operator !=(Lua interpreter, LuaThread thread)
	{
		return interpreter.State != thread.State;
	}
}
