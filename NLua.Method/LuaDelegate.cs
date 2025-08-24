using System;

namespace NLua.Method;

public class LuaDelegate
{
	public LuaFunction Function;

	public Type[] ReturnTypes;

	public LuaDelegate()
	{
		Function = null;
		ReturnTypes = null;
	}

	public object CallFunction(object[] args, object[] inArgs, int[] outArgs)
	{
		object[] array = Function.Call(inArgs, ReturnTypes);
		object result;
		int num;
		if (ReturnTypes[0] == typeof(void))
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
