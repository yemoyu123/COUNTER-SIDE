using System;
using System.Reflection;

namespace NLua;

public static class LuaRegistrationHelper
{
	public static void TaggedInstanceMethods(Lua lua, object o)
	{
		if (lua == null)
		{
			throw new ArgumentNullException("lua");
		}
		if (o == null)
		{
			throw new ArgumentNullException("o");
		}
		MethodInfo[] methods = o.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
		foreach (MethodInfo methodInfo in methods)
		{
			object[] customAttributes = methodInfo.GetCustomAttributes(typeof(LuaGlobalAttribute), inherit: true);
			for (int j = 0; j < customAttributes.Length; j++)
			{
				LuaGlobalAttribute luaGlobalAttribute = (LuaGlobalAttribute)customAttributes[j];
				if (string.IsNullOrEmpty(luaGlobalAttribute.Name))
				{
					lua.RegisterFunction(methodInfo.Name, o, methodInfo);
				}
				else
				{
					lua.RegisterFunction(luaGlobalAttribute.Name, o, methodInfo);
				}
			}
		}
	}

	public static void TaggedStaticMethods(Lua lua, Type type)
	{
		if (lua == null)
		{
			throw new ArgumentNullException("lua");
		}
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (!type.IsClass)
		{
			throw new ArgumentException("The type must be a class!", "type");
		}
		MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
		foreach (MethodInfo methodInfo in methods)
		{
			object[] customAttributes = methodInfo.GetCustomAttributes(typeof(LuaGlobalAttribute), inherit: false);
			for (int j = 0; j < customAttributes.Length; j++)
			{
				LuaGlobalAttribute luaGlobalAttribute = (LuaGlobalAttribute)customAttributes[j];
				if (string.IsNullOrEmpty(luaGlobalAttribute.Name))
				{
					lua.RegisterFunction(methodInfo.Name, null, methodInfo);
				}
				else
				{
					lua.RegisterFunction(luaGlobalAttribute.Name, null, methodInfo);
				}
			}
		}
	}

	public static void Enumeration<T>(Lua lua)
	{
		if (lua == null)
		{
			throw new ArgumentNullException("lua");
		}
		Type typeFromHandle = typeof(T);
		if (!typeFromHandle.IsEnum)
		{
			throw new ArgumentException("The type must be an enumeration!");
		}
		string[] names = Enum.GetNames(typeFromHandle);
		T[] array = (T[])Enum.GetValues(typeFromHandle);
		lua.NewTable(typeFromHandle.Name);
		for (int i = 0; i < names.Length; i++)
		{
			string fullPath = typeFromHandle.Name + "." + names[i];
			lua.SetObjectToPath(fullPath, array[i]);
		}
	}
}
