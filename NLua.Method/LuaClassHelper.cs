using System;

namespace NLua.Method;

public class LuaClassHelper
{
	public static LuaFunction GetTableFunction(LuaTable luaTable, string name)
	{
		if (luaTable == null)
		{
			return null;
		}
		if (luaTable.RawGet(name) is LuaFunction result)
		{
			return result;
		}
		return null;
	}

	public static object CallFunction(LuaFunction function, object[] args, Type[] returnTypes, object[] inArgs, int[] outArgs)
	{
		object[] array = function.Call(inArgs, returnTypes);
		if (array == null || returnTypes.Length == 0)
		{
			return null;
		}
		object result;
		int num;
		if (returnTypes[0] == typeof(void))
		{
			result = null;
			num = 0;
		}
		else
		{
			result = array[0];
			num = 1;
		}
		for (int i = 0; i < outArgs.Length; i++)
		{
			args[outArgs[i]] = array[num];
			num++;
		}
		return result;
	}
}
