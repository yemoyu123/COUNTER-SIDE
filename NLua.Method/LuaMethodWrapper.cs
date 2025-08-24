using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KeraLua;
using NLua.Exceptions;
using NLua.Extensions;

namespace NLua.Method;

internal class LuaMethodWrapper
{
	internal KeraLua.LuaFunction InvokeFunction;

	private readonly ObjectTranslator _translator;

	private readonly MethodBase _method;

	private readonly ExtractValue _extractTarget;

	private readonly object _target;

	private readonly bool _isStatic;

	private readonly string _methodName;

	private readonly MethodInfo[] _members;

	private MethodCache _lastCalledMethod;

	public LuaMethodWrapper(ObjectTranslator translator, object target, ProxyType targetType, MethodBase method)
	{
		InvokeFunction = Call;
		_translator = translator;
		_target = target;
		_extractTarget = translator.typeChecker.GetExtractor(targetType);
		_lastCalledMethod = new MethodCache();
		_method = method;
		_methodName = method.Name;
		_isStatic = method.IsStatic;
	}

	public LuaMethodWrapper(ObjectTranslator translator, ProxyType targetType, string methodName, BindingFlags bindingType)
	{
		InvokeFunction = Call;
		_translator = translator;
		_methodName = methodName;
		_extractTarget = translator.typeChecker.GetExtractor(targetType);
		_lastCalledMethod = new MethodCache();
		_isStatic = (bindingType & BindingFlags.Static) == BindingFlags.Static;
		MethodInfo[] methodsRecursively = GetMethodsRecursively(targetType.UnderlyingSystemType, methodName, bindingType | BindingFlags.Public);
		_members = ReorderMethods(methodsRecursively);
	}

	private static MethodInfo[] ReorderMethods(MethodInfo[] m)
	{
		if (m.Length < 2)
		{
			return m;
		}
		return (from c in m
			group c by c.GetParameters().Length).SelectMany((IGrouping<int, MethodInfo> g) => g.OrderByDescending((MethodInfo ci) => ci.ToString())).ToArray();
	}

	private MethodInfo[] GetMethodsRecursively(Type type, string methodName, BindingFlags bindingType)
	{
		if (type == typeof(object))
		{
			return type.GetMethods(methodName, bindingType);
		}
		MethodInfo[] methods = type.GetMethods(methodName, bindingType);
		MethodInfo[] methodsRecursively = GetMethodsRecursively(type.BaseType, methodName, bindingType);
		return methods.Concat(methodsRecursively).ToArray();
	}

	private int SetPendingException(Exception e)
	{
		return _translator.interpreter.SetPendingException(e);
	}

	private void FillMethodArguments(KeraLua.Lua luaState, int numStackToSkip)
	{
		object[] args = _lastCalledMethod.args;
		for (int i = 0; i < _lastCalledMethod.argTypes.Length; i++)
		{
			MethodArgs methodArgs = _lastCalledMethod.argTypes[i];
			int num = i + 1 + numStackToSkip;
			if (_lastCalledMethod.argTypes[i].IsParamsArray)
			{
				int count = _lastCalledMethod.argTypes.Length - i;
				Array array = _translator.TableToArray(luaState, methodArgs.ExtractValue, methodArgs.ParameterType, num, count);
				args[_lastCalledMethod.argTypes[i].Index] = array;
			}
			else
			{
				args[methodArgs.Index] = methodArgs.ExtractValue(luaState, num);
			}
			if (_lastCalledMethod.args[_lastCalledMethod.argTypes[i].Index] == null && !luaState.IsNil(i + 1 + numStackToSkip))
			{
				throw new LuaException($"Argument number {i + 1} is invalid");
			}
		}
	}

	private int PushReturnValue(KeraLua.Lua luaState)
	{
		int num = 0;
		for (int i = 0; i < _lastCalledMethod.outList.Length; i++)
		{
			num++;
			_translator.Push(luaState, _lastCalledMethod.args[_lastCalledMethod.outList[i]]);
		}
		if (!_lastCalledMethod.IsReturnVoid && num > 0)
		{
			num++;
		}
		if (num >= 1)
		{
			return num;
		}
		return 1;
	}

	private int CallInvoke(KeraLua.Lua luaState, MethodBase method, object targetObject)
	{
		if (!luaState.CheckStack(_lastCalledMethod.outList.Length + 6))
		{
			throw new LuaException("Lua stack overflow");
		}
		try
		{
			object o = ((!method.IsConstructor) ? method.Invoke(targetObject, _lastCalledMethod.args) : ((ConstructorInfo)method).Invoke(_lastCalledMethod.args));
			_translator.Push(luaState, o);
		}
		catch (TargetInvocationException ex)
		{
			if (_translator.interpreter.UseTraceback)
			{
				ex.GetBaseException().Data["Traceback"] = _translator.interpreter.GetDebugTraceback();
			}
			return SetPendingException(ex.GetBaseException());
		}
		catch (Exception pendingException)
		{
			return SetPendingException(pendingException);
		}
		return PushReturnValue(luaState);
	}

	private bool IsMethodCached(KeraLua.Lua luaState, int numArgsPassed, int skipParams)
	{
		if (_lastCalledMethod.cachedMethod == null)
		{
			return false;
		}
		if (numArgsPassed != _lastCalledMethod.argTypes.Length)
		{
			return false;
		}
		if (_members.Length == 1)
		{
			return true;
		}
		return _translator.MatchParameters(luaState, _lastCalledMethod.cachedMethod, _lastCalledMethod, skipParams);
	}

	private int CallMethodFromName(KeraLua.Lua luaState)
	{
		object obj = null;
		if (!_isStatic)
		{
			obj = _extractTarget(luaState, 1);
		}
		int num = ((!_isStatic) ? 1 : 0);
		int numArgsPassed = luaState.GetTop() - num;
		if (IsMethodCached(luaState, numArgsPassed, num))
		{
			MethodBase cachedMethod = _lastCalledMethod.cachedMethod;
			if (!luaState.CheckStack(_lastCalledMethod.outList.Length + 6))
			{
				throw new LuaException("Lua stack overflow");
			}
			FillMethodArguments(luaState, num);
			return CallInvoke(luaState, cachedMethod, obj);
		}
		if (!_isStatic)
		{
			if (obj == null)
			{
				_translator.ThrowError(luaState, $"instance method '{_methodName}' requires a non null target object");
				return 1;
			}
			luaState.Remove(1);
		}
		bool flag = false;
		string text = null;
		MethodInfo[] members = _members;
		foreach (MethodInfo methodInfo in members)
		{
			if (!(methodInfo.ReflectedType == null))
			{
				text = methodInfo.ReflectedType.Name + "." + methodInfo.Name;
				if (_translator.MatchParameters(luaState, methodInfo, _lastCalledMethod, 0))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			string e = ((text == null) ? "Invalid arguments to method call" : ("Invalid arguments to method: " + text));
			_translator.ThrowError(luaState, e);
			return 1;
		}
		if (_lastCalledMethod.cachedMethod.ContainsGenericParameters)
		{
			return CallInvokeOnGenericMethod(luaState, (MethodInfo)_lastCalledMethod.cachedMethod, obj);
		}
		return CallInvoke(luaState, _lastCalledMethod.cachedMethod, obj);
	}

	private int CallInvokeOnGenericMethod(KeraLua.Lua luaState, MethodInfo methodToCall, object targetObject)
	{
		List<Type> list = new List<Type>();
		ParameterInfo[] parameters = methodToCall.GetParameters();
		for (int i = 0; i < parameters.Length; i++)
		{
			if (parameters[i].ParameterType.IsGenericParameter)
			{
				list.Add(_lastCalledMethod.args[i].GetType());
			}
		}
		MethodInfo methodInfo = methodToCall.MakeGenericMethod(list.ToArray());
		_translator.Push(luaState, methodInfo.Invoke(targetObject, _lastCalledMethod.args));
		return PushReturnValue(luaState);
	}

	private int Call(IntPtr state)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(state);
		MethodBase method = _method;
		object obj = _target;
		if (!lua.CheckStack(5))
		{
			throw new LuaException("Lua stack overflow");
		}
		SetPendingException(null);
		if (method == null)
		{
			return CallMethodFromName(lua);
		}
		if (!method.ContainsGenericParameters)
		{
			if (!method.IsStatic && !method.IsConstructor && obj == null)
			{
				obj = _extractTarget(lua, 1);
				lua.Remove(1);
			}
			if (!_translator.MatchParameters(lua, method, _lastCalledMethod, 0))
			{
				_translator.ThrowError(lua, "Invalid arguments to method call");
				return 1;
			}
			if (_isStatic)
			{
				obj = null;
			}
			return CallInvoke(lua, _lastCalledMethod.cachedMethod, obj);
		}
		if (!method.IsGenericMethodDefinition)
		{
			_translator.ThrowError(lua, "Unable to invoke method on generic class as the current method is an open generic method");
			return 1;
		}
		_translator.MatchParameters(lua, method, _lastCalledMethod, 0);
		return CallInvokeOnGenericMethod(lua, (MethodInfo)method, obj);
	}
}
