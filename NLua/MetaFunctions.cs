using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AOT;
using KeraLua;
using NLua.Exceptions;
using NLua.Extensions;
using NLua.Method;

namespace NLua;

public class MetaFunctions
{
	public static readonly KeraLua.LuaFunction GcFunction = CollectObject;

	public static readonly KeraLua.LuaFunction IndexFunction = GetMethod;

	public static readonly KeraLua.LuaFunction NewIndexFunction = SetFieldOrProperty;

	public static readonly KeraLua.LuaFunction BaseIndexFunction = GetBaseMethod;

	public static readonly KeraLua.LuaFunction ClassIndexFunction = GetClassMethod;

	public static readonly KeraLua.LuaFunction ClassNewIndexFunction = SetClassFieldOrProperty;

	public static readonly KeraLua.LuaFunction ExecuteDelegateFunction = RunFunctionDelegate;

	public static readonly KeraLua.LuaFunction CallConstructorFunction = CallConstructor;

	public static readonly KeraLua.LuaFunction ToStringFunction = ToStringLua;

	public static readonly KeraLua.LuaFunction CallDelegateFunction = CallDelegate;

	public static readonly KeraLua.LuaFunction AddFunction = AddLua;

	public static readonly KeraLua.LuaFunction SubtractFunction = SubtractLua;

	public static readonly KeraLua.LuaFunction MultiplyFunction = MultiplyLua;

	public static readonly KeraLua.LuaFunction DivisionFunction = DivideLua;

	public static readonly KeraLua.LuaFunction ModulosFunction = ModLua;

	public static readonly KeraLua.LuaFunction UnaryNegationFunction = UnaryNegationLua;

	public static readonly KeraLua.LuaFunction EqualFunction = EqualLua;

	public static readonly KeraLua.LuaFunction LessThanFunction = LessThanLua;

	public static readonly KeraLua.LuaFunction LessThanOrEqualFunction = LessThanOrEqualLua;

	private readonly Dictionary<object, Dictionary<object, object>> _memberCache = new Dictionary<object, Dictionary<object, object>>();

	private readonly ObjectTranslator _translator;

	public const string LuaIndexFunction = "local a={}local function b(c,d)local e=getmetatable(c)local f=e.cache[d]if f~=nil then if f==a then return nil end;return f else local g,h=get_object_member(c,d)if h then if g==nil then e.cache[d]=a else e.cache[d]=g end end;return g end end;return b";

	public MetaFunctions(ObjectTranslator translator)
	{
		_translator = translator;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int RunFunctionDelegate(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		KeraLua.LuaFunction luaFunction = (KeraLua.LuaFunction)objectTranslator.GetRawNetObject(lua, 1);
		if (luaFunction == null)
		{
			return lua.Error();
		}
		lua.Remove(1);
		int result = luaFunction(luaState);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int CollectObject(IntPtr state)
	{
		KeraLua.Lua luaState = KeraLua.Lua.FromIntPtr(state);
		ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(luaState);
		return CollectObject(luaState, translator);
	}

	private static int CollectObject(KeraLua.Lua luaState, ObjectTranslator translator)
	{
		int num = luaState.RawNetObj(1);
		if (num != -1)
		{
			translator.CollectObject(num);
		}
		return 0;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int ToStringLua(IntPtr state)
	{
		KeraLua.Lua luaState = KeraLua.Lua.FromIntPtr(state);
		ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(luaState);
		return ToStringLua(luaState, translator);
	}

	private static int ToStringLua(KeraLua.Lua luaState, ObjectTranslator translator)
	{
		object rawNetObject = translator.GetRawNetObject(luaState, 1);
		if (rawNetObject != null)
		{
			translator.Push(luaState, rawNetObject?.ToString() + ": " + rawNetObject.GetHashCode());
		}
		else
		{
			luaState.PushNil();
		}
		return 1;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int AddLua(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = MatchOperator(lua, "op_Addition", objectTranslator);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int SubtractLua(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = MatchOperator(lua, "op_Subtraction", objectTranslator);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int MultiplyLua(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = MatchOperator(lua, "op_Multiply", objectTranslator);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int DivideLua(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = MatchOperator(lua, "op_Division", objectTranslator);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int ModLua(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = MatchOperator(lua, "op_Modulus", objectTranslator);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int UnaryNegationLua(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = UnaryNegationLua(lua, objectTranslator);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	private static int UnaryNegationLua(KeraLua.Lua luaState, ObjectTranslator translator)
	{
		object rawNetObject = translator.GetRawNetObject(luaState, 1);
		if (rawNetObject == null)
		{
			translator.ThrowError(luaState, "Cannot negate a nil object");
			return 1;
		}
		Type type = rawNetObject.GetType();
		MethodInfo method = type.GetMethod("op_UnaryNegation");
		if (method == null)
		{
			translator.ThrowError(luaState, "Cannot negate object (" + type.Name + " does not overload the operator -)");
			return 1;
		}
		rawNetObject = method.Invoke(rawNetObject, new object[1] { rawNetObject });
		translator.Push(luaState, rawNetObject);
		return 1;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int EqualLua(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = MatchOperator(lua, "op_Equality", objectTranslator);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int LessThanLua(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = MatchOperator(lua, "op_LessThan", objectTranslator);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int LessThanOrEqualLua(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = MatchOperator(lua, "op_LessThanOrEqual", objectTranslator);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	public static void DumpStack(ObjectTranslator translator, KeraLua.Lua luaState)
	{
		int top = luaState.GetTop();
		for (int i = 1; i <= top; i++)
		{
			LuaType luaType = luaState.Type(i);
			if (luaType != LuaType.Table)
			{
				luaState.TypeName(luaType);
			}
			luaState.ToString(i, callMetamethod: false);
			if (luaType == LuaType.UserData)
			{
				translator.GetRawNetObject(luaState, i)?.ToString();
			}
		}
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int GetMethod(IntPtr state)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(state);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int methodInternal = objectTranslator.MetaFunctionsInstance.GetMethodInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return methodInternal;
	}

	private int GetMethodInternal(KeraLua.Lua luaState)
	{
		object rawNetObject = _translator.GetRawNetObject(luaState, 1);
		if (rawNetObject == null)
		{
			_translator.ThrowError(luaState, "Trying to index an invalid object reference");
			return 1;
		}
		object obj = _translator.GetObject(luaState, 2);
		string text = obj as string;
		Type type = rawNetObject.GetType();
		ProxyType objType = new ProxyType(type);
		if (!string.IsNullOrEmpty(text) && IsMemberPresent(objType, text))
		{
			return GetMember(luaState, objType, rawNetObject, text, BindingFlags.Instance);
		}
		if (TryAccessByArray(luaState, type, rawNetObject, obj))
		{
			return 1;
		}
		int methodFallback = GetMethodFallback(luaState, type, rawNetObject, text);
		if (methodFallback != 0)
		{
			return methodFallback;
		}
		if (!string.IsNullOrEmpty(text) || obj != null)
		{
			if (string.IsNullOrEmpty(text))
			{
				text = obj.ToString();
			}
			return PushInvalidMethodCall(luaState, type, text);
		}
		luaState.PushBoolean(b: false);
		return 2;
	}

	private int PushInvalidMethodCall(KeraLua.Lua luaState, Type type, string name)
	{
		SetMemberCache(type, name, null);
		_translator.Push(luaState, null);
		_translator.Push(luaState, false);
		return 2;
	}

	private bool TryAccessByArray(KeraLua.Lua luaState, Type objType, object obj, object index)
	{
		if (!objType.IsArray)
		{
			return false;
		}
		int num = -1;
		if (index is long num2)
		{
			num = (int)num2;
		}
		else if (index is double num3)
		{
			num = (int)num3;
		}
		if (num == -1)
		{
			return false;
		}
		Type underlyingSystemType = objType.UnderlyingSystemType;
		if (underlyingSystemType == typeof(long[]))
		{
			long[] array = (long[])obj;
			_translator.Push(luaState, array[num]);
			return true;
		}
		if (underlyingSystemType == typeof(float[]))
		{
			float[] array2 = (float[])obj;
			_translator.Push(luaState, array2[num]);
			return true;
		}
		if (underlyingSystemType == typeof(double[]))
		{
			double[] array3 = (double[])obj;
			_translator.Push(luaState, array3[num]);
			return true;
		}
		if (underlyingSystemType == typeof(int[]))
		{
			int[] array4 = (int[])obj;
			_translator.Push(luaState, array4[num]);
			return true;
		}
		if (underlyingSystemType == typeof(byte[]))
		{
			byte[] array5 = (byte[])obj;
			_translator.Push(luaState, array5[num]);
			return true;
		}
		if (underlyingSystemType == typeof(short[]))
		{
			short[] array6 = (short[])obj;
			_translator.Push(luaState, array6[num]);
			return true;
		}
		if (underlyingSystemType == typeof(ushort[]))
		{
			ushort[] array7 = (ushort[])obj;
			_translator.Push(luaState, array7[num]);
			return true;
		}
		if (underlyingSystemType == typeof(ulong[]))
		{
			ulong[] array8 = (ulong[])obj;
			_translator.Push(luaState, array8[num]);
			return true;
		}
		if (underlyingSystemType == typeof(uint[]))
		{
			uint[] array9 = (uint[])obj;
			_translator.Push(luaState, array9[num]);
			return true;
		}
		if (underlyingSystemType == typeof(sbyte[]))
		{
			sbyte[] array10 = (sbyte[])obj;
			_translator.Push(luaState, array10[num]);
			return true;
		}
		object value = ((Array)obj).GetValue(num);
		_translator.Push(luaState, value);
		return true;
	}

	private int GetMethodFallback(KeraLua.Lua luaState, Type objType, object obj, string methodName)
	{
		if (!string.IsNullOrEmpty(methodName) && TryGetExtensionMethod(objType, methodName, out var method))
		{
			return PushExtensionMethod(luaState, objType, obj, methodName, method);
		}
		MethodInfo[] methods = objType.GetMethods();
		int num = TryIndexMethods(luaState, methods, obj);
		if (num != 0)
		{
			return num;
		}
		methods = objType.GetRuntimeMethods().ToArray();
		num = TryIndexMethods(luaState, methods, obj);
		if (num != 0)
		{
			return num;
		}
		num = TryGetValueForKeyMethods(luaState, methods, obj);
		if (num != 0)
		{
			return num;
		}
		MethodInfo methodInfo = objType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault((MethodInfo m) => m.Name == methodName && m.IsPrivate && m.IsVirtual && m.IsFinal);
		if (methodInfo != null)
		{
			ProxyType proxyType = new ProxyType(objType);
			KeraLua.LuaFunction luaFunction = new LuaMethodWrapper(_translator, obj, proxyType, methodInfo).InvokeFunction.Invoke;
			SetMemberCache(proxyType, methodName, luaFunction);
			_translator.PushFunction(luaState, luaFunction);
			_translator.Push(luaState, true);
			return 2;
		}
		return 0;
	}

	private int TryGetValueForKeyMethods(KeraLua.Lua luaState, MethodInfo[] methods, object obj)
	{
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.Name != "TryGetValueForKey" || methodInfo.GetParameters().Length != 2)
			{
				continue;
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			object asType = _translator.GetAsType(luaState, 2, parameters[0].ParameterType);
			if (asType == null)
			{
				break;
			}
			object[] array = new object[2] { asType, null };
			try
			{
				if (!(bool)methodInfo.Invoke(obj, array))
				{
					_translator.ThrowError(luaState, "key not found: " + asType);
					return 1;
				}
				_translator.Push(luaState, array[1]);
				return 1;
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException is KeyNotFoundException)
				{
					_translator.ThrowError(luaState, "key '" + asType?.ToString() + "' not found ");
				}
				else
				{
					_translator.ThrowError(luaState, "exception indexing '" + asType?.ToString() + "' " + ex.Message);
				}
				return 1;
			}
		}
		return 0;
	}

	private int TryIndexMethods(KeraLua.Lua luaState, MethodInfo[] methods, object obj)
	{
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.Name != "get_Item" || methodInfo.GetParameters().Length != 1)
			{
				continue;
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			object asType = _translator.GetAsType(luaState, 2, parameters[0].ParameterType);
			if (asType == null)
			{
				continue;
			}
			object[] parameters2 = new object[1] { asType };
			try
			{
				object o = methodInfo.Invoke(obj, parameters2);
				_translator.Push(luaState, o);
				return 1;
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException is KeyNotFoundException)
				{
					_translator.ThrowError(luaState, "key '" + asType?.ToString() + "' not found ");
				}
				else
				{
					_translator.ThrowError(luaState, "exception indexing '" + asType?.ToString() + "' " + ex.Message);
				}
				return 1;
			}
		}
		return 0;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int GetBaseMethod(IntPtr state)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(state);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int baseMethodInternal = objectTranslator.MetaFunctionsInstance.GetBaseMethodInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return baseMethodInternal;
	}

	private int GetBaseMethodInternal(KeraLua.Lua luaState)
	{
		object rawNetObject = _translator.GetRawNetObject(luaState, 1);
		if (rawNetObject == null)
		{
			_translator.ThrowError(luaState, "Trying to index an invalid object reference");
			return 1;
		}
		string text = luaState.ToString(2, callMetamethod: false);
		if (string.IsNullOrEmpty(text))
		{
			luaState.PushNil();
			luaState.PushBoolean(b: false);
			return 2;
		}
		GetMember(luaState, new ProxyType(rawNetObject.GetType()), rawNetObject, "__luaInterface_base_" + text, BindingFlags.Instance);
		luaState.SetTop(-2);
		if (luaState.Type(-1) == LuaType.Nil)
		{
			luaState.SetTop(-2);
			return GetMember(luaState, new ProxyType(rawNetObject.GetType()), rawNetObject, text, BindingFlags.Instance);
		}
		luaState.PushBoolean(b: false);
		return 2;
	}

	private bool IsMemberPresent(ProxyType objType, string methodName)
	{
		if (CheckMemberCache(objType, methodName) != null)
		{
			return true;
		}
		return objType.GetMember(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Length != 0;
	}

	private bool TryGetExtensionMethod(Type type, string name, out object method)
	{
		object obj = CheckMemberCache(type, name);
		if (obj != null)
		{
			method = obj;
			return true;
		}
		MethodInfo method2;
		bool result = _translator.TryGetExtensionMethod(type, name, out method2);
		method = method2;
		return result;
	}

	private int PushExtensionMethod(KeraLua.Lua luaState, Type type, object obj, string name, object method)
	{
		if (method is KeraLua.LuaFunction func)
		{
			_translator.PushFunction(luaState, func);
			_translator.Push(luaState, true);
			return 2;
		}
		MethodInfo method2 = (MethodInfo)method;
		KeraLua.LuaFunction luaFunction = new LuaMethodWrapper(_translator, obj, new ProxyType(type), method2).InvokeFunction.Invoke;
		SetMemberCache(type, name, luaFunction);
		_translator.PushFunction(luaState, luaFunction);
		_translator.Push(luaState, true);
		return 2;
	}

	private int GetMember(KeraLua.Lua luaState, ProxyType objType, object obj, string methodName, BindingFlags bindingType)
	{
		bool flag = false;
		MemberInfo memberInfo = null;
		object obj2 = CheckMemberCache(objType, methodName);
		if (obj2 is KeraLua.LuaFunction)
		{
			_translator.PushFunction(luaState, (KeraLua.LuaFunction)obj2);
			_translator.Push(luaState, true);
			return 2;
		}
		if (obj2 != null)
		{
			memberInfo = (MemberInfo)obj2;
		}
		else
		{
			MemberInfo[] member = objType.GetMember(methodName, bindingType | BindingFlags.Public);
			if (member.Length != 0)
			{
				memberInfo = member[0];
			}
			else
			{
				member = objType.GetMember(methodName, bindingType | BindingFlags.Static | BindingFlags.Public);
				if (member.Length != 0)
				{
					memberInfo = member[0];
					flag = true;
				}
			}
		}
		if (memberInfo != null)
		{
			if (memberInfo.MemberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = (FieldInfo)memberInfo;
				if (obj2 == null)
				{
					SetMemberCache(objType, methodName, memberInfo);
				}
				try
				{
					object value = fieldInfo.GetValue(obj);
					_translator.Push(luaState, value);
				}
				catch
				{
					luaState.PushNil();
				}
			}
			else if (memberInfo.MemberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
				if (obj2 == null)
				{
					SetMemberCache(objType, methodName, memberInfo);
				}
				try
				{
					object value2 = propertyInfo.GetValue(obj, null);
					_translator.Push(luaState, value2);
				}
				catch (ArgumentException)
				{
					if (objType.UnderlyingSystemType != typeof(object))
					{
						return GetMember(luaState, new ProxyType(objType.UnderlyingSystemType.BaseType), obj, methodName, bindingType);
					}
					luaState.PushNil();
				}
				catch (TargetInvocationException e)
				{
					ThrowError(luaState, e);
					luaState.PushNil();
				}
			}
			else if (memberInfo.MemberType == MemberTypes.Event)
			{
				EventInfo eventInfo = (EventInfo)memberInfo;
				if (obj2 == null)
				{
					SetMemberCache(objType, methodName, memberInfo);
				}
				_translator.Push(luaState, new RegisterEventHandler(_translator.PendingEvents, obj, eventInfo));
			}
			else
			{
				if (flag)
				{
					_translator.ThrowError(luaState, "Can't pass instance to static method " + methodName);
					return 1;
				}
				if (memberInfo.MemberType != MemberTypes.NestedType || !(memberInfo.DeclaringType != null))
				{
					KeraLua.LuaFunction invokeFunction = new LuaMethodWrapper(_translator, objType, methodName, bindingType).InvokeFunction;
					if (obj2 == null)
					{
						SetMemberCache(objType, methodName, invokeFunction);
					}
					_translator.PushFunction(luaState, invokeFunction);
					_translator.Push(luaState, true);
					return 2;
				}
				if (obj2 == null)
				{
					SetMemberCache(objType, methodName, memberInfo);
				}
				string name = memberInfo.Name;
				string className = memberInfo.DeclaringType.FullName + "+" + name;
				Type t = _translator.FindType(className);
				_translator.PushType(luaState, t);
			}
			_translator.Push(luaState, false);
			return 2;
		}
		if (objType.UnderlyingSystemType != typeof(object))
		{
			return GetMember(luaState, new ProxyType(objType.UnderlyingSystemType.BaseType), obj, methodName, bindingType);
		}
		_translator.ThrowError(luaState, "Unknown member name " + methodName);
		return 1;
	}

	private object CheckMemberCache(Type objType, string memberName)
	{
		return CheckMemberCache(new ProxyType(objType), memberName);
	}

	private object CheckMemberCache(ProxyType objType, string memberName)
	{
		if (!_memberCache.TryGetValue(objType, out var value))
		{
			return null;
		}
		if (value == null || !value.TryGetValue(memberName, out var value2))
		{
			return null;
		}
		return value2;
	}

	private void SetMemberCache(Type objType, string memberName, object member)
	{
		SetMemberCache(new ProxyType(objType), memberName, member);
	}

	private void SetMemberCache(ProxyType objType, string memberName, object member)
	{
		Dictionary<object, object> dictionary;
		if (_memberCache.TryGetValue(objType, out var value))
		{
			dictionary = value;
		}
		else
		{
			dictionary = new Dictionary<object, object>();
			_memberCache[objType] = dictionary;
		}
		dictionary[memberName] = member;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int SetFieldOrProperty(IntPtr state)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(state);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = objectTranslator.MetaFunctionsInstance.SetFieldOrPropertyInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	private int SetFieldOrPropertyInternal(KeraLua.Lua luaState)
	{
		object rawNetObject = _translator.GetRawNetObject(luaState, 1);
		if (rawNetObject == null)
		{
			_translator.ThrowError(luaState, "trying to index and invalid object reference");
			return 1;
		}
		Type type = rawNetObject.GetType();
		if (TrySetMember(luaState, new ProxyType(type), rawNetObject, BindingFlags.Instance, out var detailMessage))
		{
			return 0;
		}
		try
		{
			if (type.IsArray && luaState.IsNumber(2))
			{
				int index = (int)luaState.ToNumber(2);
				Array array = (Array)rawNetObject;
				object asType = _translator.GetAsType(luaState, 3, array.GetType().GetElementType());
				array.SetValue(asType, index);
			}
			else
			{
				MethodInfo method = type.GetMethod("set_Item");
				if (!(method != null))
				{
					_translator.ThrowError(luaState, detailMessage);
					return 1;
				}
				ParameterInfo[] parameters = method.GetParameters();
				Type parameterType = parameters[1].ParameterType;
				object asType2 = _translator.GetAsType(luaState, 3, parameterType);
				Type parameterType2 = parameters[0].ParameterType;
				object asType3 = _translator.GetAsType(luaState, 2, parameterType2);
				method.Invoke(rawNetObject, new object[2] { asType3, asType2 });
			}
		}
		catch (Exception e)
		{
			ThrowError(luaState, e);
			return 1;
		}
		return 0;
	}

	private bool TrySetMember(KeraLua.Lua luaState, ProxyType targetType, object target, BindingFlags bindingType, out string detailMessage)
	{
		detailMessage = null;
		if (luaState.Type(2) != LuaType.String)
		{
			detailMessage = "property names must be strings";
			return false;
		}
		string text = luaState.ToString(2, callMetamethod: false);
		if (string.IsNullOrEmpty(text) || (!char.IsLetter(text[0]) && text[0] != '_'))
		{
			detailMessage = "Invalid property name";
			return false;
		}
		MemberInfo memberInfo = (MemberInfo)CheckMemberCache(targetType, text);
		if (memberInfo == null)
		{
			MemberInfo[] member = targetType.GetMember(text, bindingType | BindingFlags.Public);
			if (member.Length == 0)
			{
				detailMessage = "field or property '" + text + "' does not exist";
				return false;
			}
			memberInfo = member[0];
			SetMemberCache(targetType, text, memberInfo);
		}
		if (memberInfo.MemberType == MemberTypes.Field)
		{
			FieldInfo fieldInfo = (FieldInfo)memberInfo;
			object asType = _translator.GetAsType(luaState, 3, fieldInfo.FieldType);
			try
			{
				fieldInfo.SetValue(target, asType);
			}
			catch (Exception ex)
			{
				detailMessage = "Error setting field: " + ex.Message;
				return false;
			}
			return true;
		}
		if (memberInfo.MemberType == MemberTypes.Property)
		{
			PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
			object asType2 = _translator.GetAsType(luaState, 3, propertyInfo.PropertyType);
			try
			{
				propertyInfo.SetValue(target, asType2, null);
			}
			catch (Exception ex2)
			{
				detailMessage = "Error setting property: " + ex2.Message;
				return false;
			}
			return true;
		}
		detailMessage = "'" + text + "' is not a .net field or property";
		return false;
	}

	private int SetMember(KeraLua.Lua luaState, ProxyType targetType, object target, BindingFlags bindingType)
	{
		if (!TrySetMember(luaState, targetType, target, bindingType, out var detailMessage))
		{
			_translator.ThrowError(luaState, detailMessage);
			return 1;
		}
		return 0;
	}

	private void ThrowError(KeraLua.Lua luaState, Exception e)
	{
		if (e is TargetInvocationException ex)
		{
			e = ex.InnerException;
		}
		_translator.ThrowError(luaState, e);
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int GetClassMethod(IntPtr state)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(state);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int classMethodInternal = objectTranslator.MetaFunctionsInstance.GetClassMethodInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return classMethodInternal;
	}

	private int GetClassMethodInternal(KeraLua.Lua luaState)
	{
		if (!(_translator.GetRawNetObject(luaState, 1) is ProxyType proxyType))
		{
			_translator.ThrowError(luaState, "Trying to index an invalid type reference");
			return 1;
		}
		if (luaState.IsNumber(2))
		{
			int length = (int)luaState.ToNumber(2);
			_translator.Push(luaState, Array.CreateInstance(proxyType.UnderlyingSystemType, length));
			return 1;
		}
		string text = luaState.ToString(2, callMetamethod: false);
		if (string.IsNullOrEmpty(text))
		{
			luaState.PushNil();
			return 1;
		}
		return GetMember(luaState, proxyType, null, text, BindingFlags.Static);
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int SetClassFieldOrProperty(IntPtr state)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(state);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = objectTranslator.MetaFunctionsInstance.SetClassFieldOrPropertyInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	private int SetClassFieldOrPropertyInternal(KeraLua.Lua luaState)
	{
		if (!(_translator.GetRawNetObject(luaState, 1) is ProxyType targetType))
		{
			_translator.ThrowError(luaState, "trying to index an invalid type reference");
			return 1;
		}
		return SetMember(luaState, targetType, null, BindingFlags.Static);
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int CallDelegate(IntPtr state)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(state);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = objectTranslator.MetaFunctionsInstance.CallDelegateInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	private int CallDelegateInternal(KeraLua.Lua luaState)
	{
		if (!(_translator.GetRawNetObject(luaState, 1) is Delegate obj))
		{
			_translator.ThrowError(luaState, "Trying to invoke a not delegate or callable value");
			return 1;
		}
		luaState.Remove(1);
		MethodCache methodCache = new MethodCache();
		MethodBase method = obj.Method;
		if (MatchParameters(luaState, method, methodCache, 0))
		{
			try
			{
				object o = ((!method.IsStatic) ? method.Invoke(obj.Target, methodCache.args) : method.Invoke(null, methodCache.args));
				_translator.Push(luaState, o);
				return 1;
			}
			catch (TargetInvocationException ex)
			{
				if (_translator.interpreter.UseTraceback)
				{
					ex.GetBaseException().Data["Traceback"] = _translator.interpreter.GetDebugTraceback();
				}
				return _translator.Interpreter.SetPendingException(ex.GetBaseException());
			}
			catch (Exception pendingException)
			{
				return _translator.Interpreter.SetPendingException(pendingException);
			}
		}
		_translator.ThrowError(luaState, "Cannot invoke delegate (invalid arguments for  " + method.Name + ")");
		return 1;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int CallConstructor(IntPtr state)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(state);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = objectTranslator.MetaFunctionsInstance.CallConstructorInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	private static ConstructorInfo[] ReorderConstructors(ConstructorInfo[] constructors)
	{
		if (constructors.Length < 2)
		{
			return constructors;
		}
		return (from c in constructors
			group c by c.GetParameters().Length).SelectMany((IGrouping<int, ConstructorInfo> g) => g.OrderByDescending((ConstructorInfo ci) => ci.ToString())).ToArray();
	}

	private int CallConstructorInternal(KeraLua.Lua luaState)
	{
		if (!(_translator.GetRawNetObject(luaState, 1) is ProxyType proxyType))
		{
			_translator.ThrowError(luaState, "Trying to call constructor on an invalid type reference");
			return 1;
		}
		MethodCache methodCache = new MethodCache();
		luaState.Remove(1);
		ConstructorInfo[] constructors = proxyType.UnderlyingSystemType.GetConstructors();
		constructors = ReorderConstructors(constructors);
		ConstructorInfo[] array = constructors;
		foreach (ConstructorInfo constructorInfo in array)
		{
			if (MatchParameters(luaState, constructorInfo, methodCache, 0))
			{
				try
				{
					_translator.Push(luaState, constructorInfo.Invoke(methodCache.args));
				}
				catch (TargetInvocationException e)
				{
					ThrowError(luaState, e);
					return 1;
				}
				catch
				{
					luaState.PushNil();
				}
				return 1;
			}
		}
		if (proxyType.UnderlyingSystemType.IsValueType && luaState.GetTop() == 0)
		{
			_translator.Push(luaState, Activator.CreateInstance(proxyType.UnderlyingSystemType));
			return 1;
		}
		string arg = ((constructors.Length == 0) ? "unknown" : constructors[0].Name);
		_translator.ThrowError(luaState, $"{proxyType.UnderlyingSystemType} does not contain constructor({arg}) argument match");
		return 1;
	}

	private static bool IsInteger(double x)
	{
		return Math.Ceiling(x) == x;
	}

	private static object GetTargetObject(KeraLua.Lua luaState, string operation, ObjectTranslator translator)
	{
		object rawNetObject = translator.GetRawNetObject(luaState, 1);
		if (rawNetObject != null && rawNetObject.GetType().HasMethod(operation))
		{
			return rawNetObject;
		}
		rawNetObject = translator.GetRawNetObject(luaState, 2);
		if (rawNetObject != null && rawNetObject.GetType().HasMethod(operation))
		{
			return rawNetObject;
		}
		return null;
	}

	private static int MatchOperator(KeraLua.Lua luaState, string operation, ObjectTranslator translator)
	{
		MethodCache methodCache = new MethodCache();
		object targetObject = GetTargetObject(luaState, operation, translator);
		if (targetObject == null)
		{
			translator.ThrowError(luaState, "Cannot call " + operation + " on a nil object");
			return 1;
		}
		Type type = targetObject.GetType();
		MethodInfo[] methods = type.GetMethods(operation, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		foreach (MethodInfo methodInfo in methods)
		{
			if (translator.MatchParameters(luaState, methodInfo, methodCache, 0))
			{
				object o = ((!methodInfo.IsStatic) ? methodInfo.Invoke(targetObject, methodCache.args) : methodInfo.Invoke(null, methodCache.args));
				translator.Push(luaState, o);
				return 1;
			}
		}
		translator.ThrowError(luaState, "Cannot call (" + operation + ") on object type " + type.Name);
		return 1;
	}

	internal Array TableToArray(KeraLua.Lua luaState, ExtractValue extractValue, Type paramArrayType, ref int startIndex, int count)
	{
		if (count == 0)
		{
			return Array.CreateInstance(paramArrayType, 0);
		}
		object obj = extractValue(luaState, startIndex);
		startIndex++;
		Array array;
		if (obj is LuaTable)
		{
			LuaTable luaTable = (LuaTable)obj;
			IDictionaryEnumerator enumerator = luaTable.GetEnumerator();
			enumerator.Reset();
			array = Array.CreateInstance(paramArrayType, luaTable.Values.Count);
			int num = 0;
			while (enumerator.MoveNext())
			{
				object obj2 = enumerator.Value;
				if (paramArrayType == typeof(object) && obj2 != null && obj2 is double && IsInteger((double)obj2))
				{
					obj2 = Convert.ToInt32((double)obj2);
				}
				array.SetValue(Convert.ChangeType(obj2, paramArrayType), num);
				num++;
			}
		}
		else
		{
			array = Array.CreateInstance(paramArrayType, count);
			array.SetValue(obj, 0);
			for (int i = 1; i < count; i++)
			{
				object value = extractValue(luaState, startIndex);
				array.SetValue(value, i);
				startIndex++;
			}
		}
		return array;
	}

	internal bool MatchParameters(KeraLua.Lua luaState, MethodBase method, MethodCache methodCache, int skipParam)
	{
		ParameterInfo[] parameters = method.GetParameters();
		int startIndex = 1;
		int num = luaState.GetTop() - skipParam;
		List<object> list = new List<object>();
		List<int> list2 = new List<int>();
		List<MethodArgs> list3 = new List<MethodArgs>();
		ParameterInfo[] array = parameters;
		foreach (ParameterInfo parameterInfo in array)
		{
			ExtractValue extractValue;
			if (!parameterInfo.IsIn && parameterInfo.IsOut)
			{
				list.Add(null);
				list2.Add(list.Count - 1);
			}
			else if (IsParamsArray(luaState, num, startIndex, parameterInfo, out extractValue))
			{
				int count = num - startIndex + 1;
				Type elementType = parameterInfo.ParameterType.GetElementType();
				Array item = TableToArray(luaState, extractValue, elementType, ref startIndex, count);
				list.Add(item);
				int index = list.LastIndexOf(item);
				MethodArgs methodArgs = new MethodArgs();
				methodArgs.Index = index;
				methodArgs.ExtractValue = extractValue;
				methodArgs.IsParamsArray = true;
				methodArgs.ParameterType = elementType;
				list3.Add(methodArgs);
			}
			else if (startIndex > num)
			{
				if (!parameterInfo.IsOptional)
				{
					return false;
				}
				list.Add(parameterInfo.DefaultValue);
			}
			else if (IsTypeCorrect(luaState, startIndex, parameterInfo, out extractValue))
			{
				object item2 = extractValue(luaState, startIndex);
				list.Add(item2);
				int num2 = list.Count - 1;
				MethodArgs methodArgs2 = new MethodArgs();
				methodArgs2.Index = num2;
				methodArgs2.ExtractValue = extractValue;
				methodArgs2.ParameterType = parameterInfo.ParameterType;
				list3.Add(methodArgs2);
				if (parameterInfo.ParameterType.IsByRef)
				{
					list2.Add(num2);
				}
				startIndex++;
			}
			else
			{
				if (!parameterInfo.IsOptional)
				{
					return false;
				}
				list.Add(parameterInfo.DefaultValue);
			}
		}
		if (startIndex != num + 1)
		{
			return false;
		}
		methodCache.args = list.ToArray();
		methodCache.cachedMethod = method;
		methodCache.outList = list2.ToArray();
		methodCache.argTypes = list3.ToArray();
		return true;
	}

	private bool IsTypeCorrect(KeraLua.Lua luaState, int currentLuaParam, ParameterInfo currentNetParam, out ExtractValue extractValue)
	{
		extractValue = _translator.typeChecker.CheckLuaType(luaState, currentLuaParam, currentNetParam.ParameterType);
		return extractValue != null;
	}

	private bool IsParamsArray(KeraLua.Lua luaState, int nLuaParams, int currentLuaParam, ParameterInfo currentNetParam, out ExtractValue extractValue)
	{
		extractValue = null;
		if (!currentNetParam.GetCustomAttributes(typeof(ParamArrayAttribute), inherit: false).Any())
		{
			return false;
		}
		bool result = nLuaParams < currentLuaParam;
		if (luaState.Type(currentLuaParam) == LuaType.Table)
		{
			extractValue = _translator.typeChecker.GetExtractor(typeof(LuaTable));
			if (extractValue != null)
			{
				return true;
			}
		}
		else
		{
			Type elementType = currentNetParam.ParameterType.GetElementType();
			extractValue = _translator.typeChecker.CheckLuaType(luaState, currentLuaParam, elementType);
			if (extractValue != null)
			{
				return true;
			}
		}
		return result;
	}
}
