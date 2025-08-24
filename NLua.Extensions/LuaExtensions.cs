using System;
using System.Runtime.InteropServices;
using KeraLua;

namespace NLua.Extensions;

internal static class LuaExtensions
{
	public static bool CheckMetaTable(this KeraLua.Lua state, int index, IntPtr tag)
	{
		if (!state.GetMetaTable(index))
		{
			return false;
		}
		state.PushLightUserData(tag);
		state.RawGet(-2);
		bool result = !state.IsNil(-1);
		state.SetTop(-3);
		return result;
	}

	public static void PopGlobalTable(this KeraLua.Lua luaState)
	{
		luaState.RawSetInteger(LuaRegistry.Index, 2L);
	}

	public static void GetRef(this KeraLua.Lua luaState, int reference)
	{
		luaState.RawGetInteger(LuaRegistry.Index, reference);
	}

	public static void Unref(this KeraLua.Lua luaState, int reference)
	{
		luaState.Unref(LuaRegistry.Index, reference);
	}

	public static bool AreEqual(this KeraLua.Lua luaState, int ref1, int ref2)
	{
		return luaState.Compare(ref1, ref2, LuaCompare.Equal);
	}

	public static IntPtr CheckUData(this KeraLua.Lua state, int ud, string name)
	{
		IntPtr intPtr = state.ToUserData(ud);
		if (intPtr == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		if (!state.GetMetaTable(ud))
		{
			return IntPtr.Zero;
		}
		state.GetField(LuaRegistry.Index, name);
		bool num = state.RawEqual(-1, -2);
		state.Pop(2);
		if (num)
		{
			return intPtr;
		}
		return IntPtr.Zero;
	}

	public static int ToNetObject(this KeraLua.Lua state, int index, IntPtr tag)
	{
		if (state.Type(index) != LuaType.UserData)
		{
			return -1;
		}
		IntPtr intPtr;
		if (state.CheckMetaTable(index, tag))
		{
			intPtr = state.ToUserData(index);
			if (intPtr != IntPtr.Zero)
			{
				return Marshal.ReadInt32(intPtr);
			}
		}
		intPtr = state.CheckUData(index, "luaNet_class");
		if (intPtr != IntPtr.Zero)
		{
			return Marshal.ReadInt32(intPtr);
		}
		intPtr = state.CheckUData(index, "luaNet_searchbase");
		if (intPtr != IntPtr.Zero)
		{
			return Marshal.ReadInt32(intPtr);
		}
		intPtr = state.CheckUData(index, "luaNet_function");
		if (intPtr != IntPtr.Zero)
		{
			return Marshal.ReadInt32(intPtr);
		}
		return -1;
	}

	public static void NewUData(this KeraLua.Lua state, int val)
	{
		Marshal.WriteInt32(state.NewUserData(Marshal.SizeOf(typeof(int))), val);
	}

	public static int RawNetObj(this KeraLua.Lua state, int index)
	{
		IntPtr intPtr = state.ToUserData(index);
		if (intPtr == IntPtr.Zero)
		{
			return -1;
		}
		return Marshal.ReadInt32(intPtr);
	}

	public static int CheckUObject(this KeraLua.Lua state, int index, string name)
	{
		IntPtr intPtr = state.CheckUData(index, name);
		if (intPtr == IntPtr.Zero)
		{
			return -1;
		}
		return Marshal.ReadInt32(intPtr);
	}

	public static bool IsNumericType(this KeraLua.Lua state, int index)
	{
		return state.Type(index) == LuaType.Number;
	}
}
