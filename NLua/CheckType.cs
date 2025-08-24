using System;
using System.Collections.Generic;
using KeraLua;
using NLua.Extensions;
using NLua.Method;

namespace NLua;

internal sealed class CheckType
{
	private readonly Dictionary<Type, ExtractValue> _extractValues = new Dictionary<Type, ExtractValue>();

	private readonly ExtractValue _extractNetObject;

	private readonly ObjectTranslator _translator;

	public CheckType(ObjectTranslator translator)
	{
		_translator = translator;
		_extractValues.Add(typeof(object), GetAsObject);
		_extractValues.Add(typeof(sbyte), GetAsSbyte);
		_extractValues.Add(typeof(byte), GetAsByte);
		_extractValues.Add(typeof(short), GetAsShort);
		_extractValues.Add(typeof(ushort), GetAsUshort);
		_extractValues.Add(typeof(int), GetAsInt);
		_extractValues.Add(typeof(uint), GetAsUint);
		_extractValues.Add(typeof(long), GetAsLong);
		_extractValues.Add(typeof(ulong), GetAsUlong);
		_extractValues.Add(typeof(double), GetAsDouble);
		_extractValues.Add(typeof(char), GetAsChar);
		_extractValues.Add(typeof(float), GetAsFloat);
		_extractValues.Add(typeof(decimal), GetAsDecimal);
		_extractValues.Add(typeof(bool), GetAsBoolean);
		_extractValues.Add(typeof(string), GetAsString);
		_extractValues.Add(typeof(char[]), GetAsCharArray);
		_extractValues.Add(typeof(byte[]), GetAsByteArray);
		_extractValues.Add(typeof(LuaFunction), GetAsFunction);
		_extractValues.Add(typeof(LuaTable), GetAsTable);
		_extractValues.Add(typeof(LuaThread), GetAsThread);
		_extractValues.Add(typeof(LuaUserData), GetAsUserdata);
		_extractNetObject = GetAsNetObject;
	}

	internal ExtractValue GetExtractor(ProxyType paramType)
	{
		return GetExtractor(paramType.UnderlyingSystemType);
	}

	internal ExtractValue GetExtractor(Type paramType)
	{
		if (paramType.IsByRef)
		{
			paramType = paramType.GetElementType();
		}
		if (!_extractValues.ContainsKey(paramType))
		{
			return _extractNetObject;
		}
		return _extractValues[paramType];
	}

	internal ExtractValue CheckLuaType(KeraLua.Lua luaState, int stackPos, Type paramType)
	{
		LuaType luaType = luaState.Type(stackPos);
		if (paramType.IsByRef)
		{
			paramType = paramType.GetElementType();
		}
		Type underlyingType = Nullable.GetUnderlyingType(paramType);
		if (underlyingType != null)
		{
			paramType = underlyingType;
		}
		bool flag = paramType == typeof(int) || paramType == typeof(uint) || paramType == typeof(long) || paramType == typeof(ulong) || paramType == typeof(short) || paramType == typeof(ushort) || paramType == typeof(float) || paramType == typeof(double) || paramType == typeof(decimal) || paramType == typeof(byte);
		if (underlyingType != null && luaType == LuaType.Nil)
		{
			if (flag || paramType == typeof(bool))
			{
				return _extractValues[paramType];
			}
			return _extractNetObject;
		}
		if (paramType == typeof(object))
		{
			return _extractValues[paramType];
		}
		if (paramType.IsGenericParameter)
		{
			switch (luaType)
			{
			case LuaType.Boolean:
				return _extractValues[typeof(bool)];
			case LuaType.String:
				return _extractValues[typeof(string)];
			case LuaType.Table:
				return _extractValues[typeof(LuaTable)];
			case LuaType.Thread:
				return _extractValues[typeof(LuaThread)];
			case LuaType.UserData:
				return _extractValues[typeof(object)];
			case LuaType.Function:
				return _extractValues[typeof(LuaFunction)];
			case LuaType.Number:
				return _extractValues[typeof(double)];
			}
		}
		bool flag2 = paramType == typeof(string) || paramType == typeof(char[]) || paramType == typeof(byte[]);
		if (flag)
		{
			if (luaState.IsNumericType(stackPos) && !flag2)
			{
				return _extractValues[paramType];
			}
		}
		else if (paramType == typeof(bool))
		{
			if (luaState.IsBoolean(stackPos))
			{
				return _extractValues[paramType];
			}
		}
		else if (flag2)
		{
			if (luaState.IsString(stackPos) || luaType == LuaType.Nil)
			{
				return _extractValues[paramType];
			}
		}
		else if (paramType == typeof(LuaTable))
		{
			if (luaType == LuaType.Table || luaType == LuaType.Nil)
			{
				return _extractValues[paramType];
			}
		}
		else if (paramType == typeof(LuaThread))
		{
			if (luaType == LuaType.Thread || luaType == LuaType.Nil)
			{
				return _extractValues[paramType];
			}
		}
		else if (paramType == typeof(LuaUserData))
		{
			if (luaType == LuaType.UserData || luaType == LuaType.Nil)
			{
				return _extractValues[paramType];
			}
		}
		else if (paramType == typeof(LuaFunction))
		{
			if (luaType == LuaType.Function || luaType == LuaType.Nil)
			{
				return _extractValues[paramType];
			}
		}
		else
		{
			if (typeof(Delegate).IsAssignableFrom(paramType) && luaType == LuaType.Function && paramType.GetMethod("Invoke") != null)
			{
				return new DelegateGenerator(_translator, paramType).ExtractGenerated;
			}
			if (paramType.IsInterface && luaType == LuaType.Table)
			{
				return new ClassGenerator(_translator, paramType).ExtractGenerated;
			}
			if ((paramType.IsInterface || paramType.IsClass) && luaType == LuaType.Nil)
			{
				return _extractNetObject;
			}
			if (luaState.Type(stackPos) == LuaType.Table)
			{
				if (luaState.GetMetaField(stackPos, "__index") == LuaType.Nil)
				{
					return null;
				}
				object netObject = _translator.GetNetObject(luaState, -1);
				luaState.SetTop(-2);
				if (netObject != null && paramType.IsInstanceOfType(netObject))
				{
					return _extractNetObject;
				}
			}
		}
		object netObject2 = _translator.GetNetObject(luaState, stackPos);
		if (netObject2 != null && paramType.IsInstanceOfType(netObject2))
		{
			return _extractNetObject;
		}
		return null;
	}

	private object GetAsSbyte(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (sbyte)luaState.ToInteger(stackPos);
		}
		return (sbyte)luaState.ToNumber(stackPos);
	}

	private object GetAsByte(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (byte)luaState.ToInteger(stackPos);
		}
		return (byte)luaState.ToNumber(stackPos);
	}

	private object GetAsShort(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (short)luaState.ToInteger(stackPos);
		}
		return (short)luaState.ToNumber(stackPos);
	}

	private object GetAsUshort(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (ushort)luaState.ToInteger(stackPos);
		}
		return (ushort)luaState.ToNumber(stackPos);
	}

	private object GetAsInt(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (int)luaState.ToInteger(stackPos);
		}
		return (int)luaState.ToNumber(stackPos);
	}

	private object GetAsUint(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (uint)luaState.ToInteger(stackPos);
		}
		return (uint)luaState.ToNumber(stackPos);
	}

	private object GetAsLong(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return luaState.ToInteger(stackPos);
		}
		return (long)luaState.ToNumber(stackPos);
	}

	private object GetAsUlong(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (ulong)luaState.ToInteger(stackPos);
		}
		return (ulong)luaState.ToNumber(stackPos);
	}

	private object GetAsDouble(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (double)luaState.ToInteger(stackPos);
		}
		return luaState.ToNumber(stackPos);
	}

	private object GetAsChar(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (char)luaState.ToInteger(stackPos);
		}
		return (char)luaState.ToNumber(stackPos);
	}

	private object GetAsFloat(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (float)luaState.ToInteger(stackPos);
		}
		return (float)luaState.ToNumber(stackPos);
	}

	private object GetAsDecimal(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsNumericType(stackPos))
		{
			return null;
		}
		if (luaState.IsInteger(stackPos))
		{
			return (decimal)luaState.ToInteger(stackPos);
		}
		return (decimal)luaState.ToNumber(stackPos);
	}

	private object GetAsBoolean(KeraLua.Lua luaState, int stackPos)
	{
		return luaState.ToBoolean(stackPos);
	}

	private object GetAsCharArray(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsString(stackPos))
		{
			return null;
		}
		return luaState.ToString(stackPos, callMetamethod: false).ToCharArray();
	}

	private object GetAsByteArray(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsString(stackPos))
		{
			return null;
		}
		return luaState.ToBuffer(stackPos, callMetamethod: false);
	}

	private object GetAsString(KeraLua.Lua luaState, int stackPos)
	{
		if (!luaState.IsString(stackPos))
		{
			return null;
		}
		return luaState.ToString(stackPos, callMetamethod: false);
	}

	private object GetAsTable(KeraLua.Lua luaState, int stackPos)
	{
		return _translator.GetTable(luaState, stackPos);
	}

	private object GetAsThread(KeraLua.Lua luaState, int stackPos)
	{
		return _translator.GetThread(luaState, stackPos);
	}

	private object GetAsFunction(KeraLua.Lua luaState, int stackPos)
	{
		return _translator.GetFunction(luaState, stackPos);
	}

	private object GetAsUserdata(KeraLua.Lua luaState, int stackPos)
	{
		return _translator.GetUserData(luaState, stackPos);
	}

	public object GetAsObject(KeraLua.Lua luaState, int stackPos)
	{
		if (luaState.Type(stackPos) == LuaType.Table && luaState.GetMetaField(stackPos, "__index") != LuaType.Nil)
		{
			if (luaState.CheckMetaTable(-1, _translator.Tag))
			{
				luaState.Insert(stackPos);
				luaState.Remove(stackPos + 1);
			}
			else
			{
				luaState.SetTop(-2);
			}
		}
		return _translator.GetObject(luaState, stackPos);
	}

	public object GetAsNetObject(KeraLua.Lua luaState, int stackPos)
	{
		object netObject = _translator.GetNetObject(luaState, stackPos);
		if (netObject != null || luaState.Type(stackPos) != LuaType.Table)
		{
			return netObject;
		}
		if (luaState.GetMetaField(stackPos, "__index") == LuaType.Nil)
		{
			return null;
		}
		if (luaState.CheckMetaTable(-1, _translator.Tag))
		{
			luaState.Insert(stackPos);
			luaState.Remove(stackPos + 1);
			netObject = _translator.GetNetObject(luaState, stackPos);
		}
		else
		{
			luaState.SetTop(-2);
		}
		return netObject;
	}
}
