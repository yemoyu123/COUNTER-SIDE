using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;
using KeraLua;
using NLua.Event;
using NLua.Exceptions;
using NLua.Extensions;
using NLua.Method;

namespace NLua;

public class Lua : IDisposable
{
	private LuaHookFunction _hookCallback;

	private readonly List<string> _globals = new List<string>();

	private bool _globalsSorted;

	private KeraLua.Lua _luaState;

	private ObjectTranslator _translator;

	private bool _StatePassed;

	private bool _executing;

	private const string InitLuanet = "local a={}local rawget=rawget;local b=luanet.import_type;local c=luanet.load_assembly;luanet.error,luanet.type=error,type;function a:__index(d)local e=rawget(self,'.fqn')e=(e and e..'.'or'')..d;local f=rawget(luanet,d)or b(e)if f==nil then pcall(c,e)f={['.fqn']=e}setmetatable(f,a)end;rawset(self,d,f)return f end;function a:__call(...)error('No such type: '..rawget(self,'.fqn'),2)end;luanet['.fqn']=false;setmetatable(luanet,a)luanet.load_assembly('mscorlib')";

	private const string ClrPackage = "if not luanet then require'luanet'end;local a,b=luanet.import_type,luanet.load_assembly;local c={__index=function(d,e)local f=rawget(d,e)if f==nil then f=a(d.packageName..\".\"..e)if f==nil then f=a(e)end;d[e]=f end;return f end}function luanet.namespace(g)if type(g)=='table'then local h={}for i=1,#g do h[i]=luanet.namespace(g[i])end;return unpack(h)end;local j={packageName=g}setmetatable(j,c)return j end;local k,l;local function m()l={}k={__index=function(n,e)for i,d in ipairs(l)do local f=d[e]if f then _G[e]=f;return f end end end}setmetatable(_G,k)end;function CLRPackage(o,p)p=p or o;local q=pcall(b,o)return luanet.namespace(p)end;function import(o,p)if not k then m()end;if not p then local i=o:find('%.dll$')if i then p=o:sub(1,i-1)else p=o end end;local j=CLRPackage(o,p)table.insert(l,j)return j end;function luanet.make_array(r,s)local t=r[#s]for i,u in ipairs(s)do t:SetValue(u,i-1)end;return t end;function luanet.each(v)local w=v:GetEnumerator()return function()if w:MoveNext()then return w.Current end end end";

	public bool IsExecuting => _executing;

	public KeraLua.Lua State => _luaState;

	internal ObjectTranslator Translator => _translator;

	public bool UseTraceback { get; set; }

	public int MaximumRecursion { get; set; } = 2;

	public IEnumerable<string> Globals
	{
		get
		{
			if (!_globalsSorted)
			{
				_globals.Sort();
				_globalsSorted = true;
			}
			return _globals;
		}
	}

	public LuaThread Thread
	{
		get
		{
			int top = _luaState.GetTop();
			_luaState.PushThread();
			object obj = _translator.GetObject(_luaState, -1);
			_luaState.SetTop(top);
			return (LuaThread)obj;
		}
	}

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

	public object this[string fullPath]
	{
		get
		{
			object objectFromPath = GetObjectFromPath(fullPath);
			if (objectFromPath is long num)
			{
				return (double)num;
			}
			return objectFromPath;
		}
		set
		{
			SetObjectToPath(fullPath, value);
		}
	}

	public event EventHandler<HookExceptionEventArgs> HookException;

	public event EventHandler<DebugHookEventArgs> DebugHook;

	public Lua(bool openLibs = true)
	{
		_luaState = new KeraLua.Lua(openLibs);
		Init();
		_luaState.AtPanic(PanicCallback);
	}

	public Lua(KeraLua.Lua luaState)
	{
		luaState.PushString("NLua_Loaded");
		luaState.GetTable(-1001000);
		if (luaState.ToBoolean(-1))
		{
			luaState.SetTop(-2);
			throw new LuaException("There is already a NLua.Lua instance associated with this Lua state");
		}
		_luaState = luaState;
		_StatePassed = true;
		luaState.SetTop(-2);
		Init();
	}

	private void Init()
	{
		_luaState.PushString("NLua_Loaded");
		_luaState.PushBoolean(b: true);
		_luaState.SetTable(-1001000);
		if (!_StatePassed)
		{
			_luaState.NewTable();
			_luaState.SetGlobal("luanet");
		}
		_luaState.PushGlobalTable();
		_luaState.GetGlobal("luanet");
		_luaState.PushString("getmetatable");
		_luaState.GetGlobal("getmetatable");
		_luaState.SetTable(-3);
		_luaState.PopGlobalTable();
		_translator = new ObjectTranslator(this, _luaState);
		ObjectTranslatorPool.Instance.Add(_luaState, _translator);
		_luaState.PopGlobalTable();
		_luaState.DoString("local a={}local rawget=rawget;local b=luanet.import_type;local c=luanet.load_assembly;luanet.error,luanet.type=error,type;function a:__index(d)local e=rawget(self,'.fqn')e=(e and e..'.'or'')..d;local f=rawget(luanet,d)or b(e)if f==nil then pcall(c,e)f={['.fqn']=e}setmetatable(f,a)end;rawset(self,d,f)return f end;function a:__call(...)error('No such type: '..rawget(self,'.fqn'),2)end;luanet['.fqn']=false;setmetatable(luanet,a)luanet.load_assembly('mscorlib')");
	}

	public void Close()
	{
		if (!_StatePassed && _luaState != null)
		{
			_luaState.Close();
			ObjectTranslatorPool.Instance.Remove(_luaState);
			_luaState = null;
		}
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int PanicCallback(IntPtr state)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(state);
		throw new LuaException($"Unprotected error in call to Lua API ({lua.ToString(-1, callMetamethod: false)})");
	}

	private void ThrowExceptionFromError(int oldTop)
	{
		object obj = _translator.GetObject(_luaState, -1);
		_luaState.SetTop(oldTop);
		if (obj is LuaScriptException ex)
		{
			throw ex;
		}
		if (obj == null)
		{
			obj = "Unknown Lua Error";
		}
		throw new LuaScriptException(obj.ToString(), string.Empty);
	}

	private static int PushDebugTraceback(KeraLua.Lua luaState, int argCount)
	{
		luaState.GetGlobal("debug");
		luaState.GetField(-1, "traceback");
		luaState.Remove(-2);
		int num = -argCount - 2;
		luaState.Insert(num);
		return num;
	}

	public string GetDebugTraceback()
	{
		int top = _luaState.GetTop();
		_luaState.GetGlobal("debug");
		_luaState.GetField(-1, "traceback");
		_luaState.Remove(-2);
		_luaState.PCall(0, -1, 0);
		return _translator.PopValues(_luaState, top)[0] as string;
	}

	internal int SetPendingException(Exception e)
	{
		if (e == null)
		{
			return 0;
		}
		_translator.ThrowError(_luaState, e);
		return 1;
	}

	public LuaFunction LoadString(string chunk, string name)
	{
		int top = _luaState.GetTop();
		_executing = true;
		try
		{
			if (_luaState.LoadString(chunk, name) != LuaStatus.OK)
			{
				ThrowExceptionFromError(top);
			}
		}
		finally
		{
			_executing = false;
		}
		LuaFunction function = _translator.GetFunction(_luaState, -1);
		_translator.PopValues(_luaState, top);
		return function;
	}

	public LuaFunction LoadString(byte[] chunk, string name)
	{
		int top = _luaState.GetTop();
		_executing = true;
		try
		{
			if (_luaState.LoadBuffer(chunk, name) != LuaStatus.OK)
			{
				ThrowExceptionFromError(top);
			}
		}
		finally
		{
			_executing = false;
		}
		LuaFunction function = _translator.GetFunction(_luaState, -1);
		_translator.PopValues(_luaState, top);
		return function;
	}

	public LuaFunction LoadFile(string fileName)
	{
		int top = _luaState.GetTop();
		if (_luaState.LoadFile(fileName) != LuaStatus.OK)
		{
			ThrowExceptionFromError(top);
		}
		LuaFunction function = _translator.GetFunction(_luaState, -1);
		_translator.PopValues(_luaState, top);
		return function;
	}

	public object[] DoString(byte[] chunk, string chunkName = "chunk", string mode = "t")
	{
		int num = _luaState.GetTop();
		_executing = true;
		if (_luaState.LoadBuffer(chunk, chunkName, mode) != LuaStatus.OK)
		{
			ThrowExceptionFromError(num);
		}
		int errorFunctionIndex = 0;
		if (UseTraceback)
		{
			errorFunctionIndex = PushDebugTraceback(_luaState, 0);
			num++;
		}
		try
		{
			if (_luaState.PCall(0, -1, errorFunctionIndex) != LuaStatus.OK)
			{
				ThrowExceptionFromError(num);
			}
			return _translator.PopValues(_luaState, num);
		}
		finally
		{
			_executing = false;
		}
	}

	public object[] DoString(string chunk, string chunkName = "chunk")
	{
		int num = _luaState.GetTop();
		_executing = true;
		if (_luaState.LoadString(chunk, chunkName) != LuaStatus.OK)
		{
			ThrowExceptionFromError(num);
		}
		int errorFunctionIndex = 0;
		if (UseTraceback)
		{
			errorFunctionIndex = PushDebugTraceback(_luaState, 0);
			num++;
		}
		try
		{
			if (_luaState.PCall(0, -1, errorFunctionIndex) != LuaStatus.OK)
			{
				ThrowExceptionFromError(num);
			}
			return _translator.PopValues(_luaState, num);
		}
		finally
		{
			_executing = false;
		}
	}

	public object[] DoFile(string fileName)
	{
		int num = _luaState.GetTop();
		if (_luaState.LoadFile(fileName) != LuaStatus.OK)
		{
			ThrowExceptionFromError(num);
		}
		_executing = true;
		int errorFunctionIndex = 0;
		if (UseTraceback)
		{
			errorFunctionIndex = PushDebugTraceback(_luaState, 0);
			num++;
		}
		try
		{
			if (_luaState.PCall(0, -1, errorFunctionIndex) != LuaStatus.OK)
			{
				ThrowExceptionFromError(num);
			}
			return _translator.PopValues(_luaState, num);
		}
		finally
		{
			_executing = false;
		}
	}

	public object GetObjectFromPath(string fullPath)
	{
		int top = _luaState.GetTop();
		string[] array = FullPathToArray(fullPath);
		_luaState.GetGlobal(array[0]);
		object obj = _translator.GetObject(_luaState, -1);
		if (array.Length > 1)
		{
			LuaBase obj2 = obj as LuaBase;
			string[] array2 = new string[array.Length - 1];
			Array.Copy(array, 1, array2, 0, array.Length - 1);
			obj = GetObject(array2);
			obj2?.Dispose();
		}
		_luaState.SetTop(top);
		return obj;
	}

	public void SetObjectToPath(string fullPath, object value)
	{
		int top = _luaState.GetTop();
		string[] array = FullPathToArray(fullPath);
		if (array.Length == 1)
		{
			_translator.Push(_luaState, value);
			_luaState.SetGlobal(fullPath);
		}
		else
		{
			_luaState.GetGlobal(array[0]);
			string[] array2 = new string[array.Length - 1];
			Array.Copy(array, 1, array2, 0, array.Length - 1);
			SetObject(array2, value);
		}
		_luaState.SetTop(top);
		if (value == null)
		{
			_globals.Remove(fullPath);
		}
		else if (!_globals.Contains(fullPath))
		{
			RegisterGlobal(fullPath, value.GetType(), 0);
		}
	}

	private void RegisterGlobal(string path, Type type, int recursionCounter)
	{
		if (type == typeof(LuaFunction))
		{
			_globals.Add(path + "(");
		}
		else if ((type.IsClass || type.IsInterface) && type != typeof(string) && recursionCounter < MaximumRecursion)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
			foreach (MethodInfo methodInfo in methods)
			{
				string name = methodInfo.Name;
				if (!methodInfo.GetCustomAttributes(typeof(LuaHideAttribute), inherit: false).Any() && !methodInfo.GetCustomAttributes(typeof(LuaGlobalAttribute), inherit: false).Any() && name != "GetType" && name != "GetHashCode" && name != "Equals" && name != "ToString" && name != "Clone" && name != "Dispose" && name != "GetEnumerator" && name != "CopyTo" && !name.StartsWith("get_", StringComparison.Ordinal) && !name.StartsWith("set_", StringComparison.Ordinal) && !name.StartsWith("add_", StringComparison.Ordinal) && !name.StartsWith("remove_", StringComparison.Ordinal))
				{
					string text = path + ":" + name + "(";
					if (methodInfo.GetParameters().Length == 0)
					{
						text += ")";
					}
					_globals.Add(text);
				}
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!fieldInfo.GetCustomAttributes(typeof(LuaHideAttribute), inherit: false).Any() && !fieldInfo.GetCustomAttributes(typeof(LuaGlobalAttribute), inherit: false).Any())
				{
					RegisterGlobal(path + "." + fieldInfo.Name, fieldInfo.FieldType, recursionCounter + 1);
				}
			}
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.GetCustomAttributes(typeof(LuaHideAttribute), inherit: false).Any() && !propertyInfo.GetCustomAttributes(typeof(LuaGlobalAttribute), inherit: false).Any() && propertyInfo.Name != "Item")
				{
					RegisterGlobal(path + "." + propertyInfo.Name, propertyInfo.PropertyType, recursionCounter + 1);
				}
			}
		}
		else
		{
			_globals.Add(path);
		}
		_globalsSorted = false;
	}

	private object GetObject(string[] remainingPath)
	{
		object obj = null;
		for (int i = 0; i < remainingPath.Length; i++)
		{
			_luaState.PushString(remainingPath[i]);
			_luaState.GetTable(-2);
			obj = _translator.GetObject(_luaState, -1);
			if (obj == null)
			{
				break;
			}
		}
		return obj;
	}

	public double GetNumber(string fullPath)
	{
		object objectFromPath = GetObjectFromPath(fullPath);
		if (objectFromPath is long num)
		{
			return num;
		}
		return (double)objectFromPath;
	}

	public int GetInteger(string fullPath)
	{
		object objectFromPath = GetObjectFromPath(fullPath);
		if (objectFromPath == null)
		{
			return 0;
		}
		return (int)(long)objectFromPath;
	}

	public long GetLong(string fullPath)
	{
		object objectFromPath = GetObjectFromPath(fullPath);
		if (objectFromPath == null)
		{
			return 0L;
		}
		return (long)objectFromPath;
	}

	public string GetString(string fullPath)
	{
		return GetObjectFromPath(fullPath)?.ToString();
	}

	public LuaTable GetTable(string fullPath)
	{
		return (LuaTable)GetObjectFromPath(fullPath);
	}

	public object GetTable(Type interfaceType, string fullPath)
	{
		return CodeGeneration.Instance.GetClassInstance(interfaceType, GetTable(fullPath));
	}

	public LuaThread GetThread(string fullPath)
	{
		return (LuaThread)GetObjectFromPath(fullPath);
	}

	public LuaFunction GetFunction(string fullPath)
	{
		object objectFromPath = GetObjectFromPath(fullPath);
		if (objectFromPath is LuaFunction result)
		{
			return result;
		}
		return new LuaFunction((KeraLua.LuaFunction)objectFromPath, this);
	}

	public void RegisterLuaDelegateType(Type delegateType, Type luaDelegateType)
	{
		CodeGeneration.Instance.RegisterLuaDelegateType(delegateType, luaDelegateType);
	}

	public void RegisterLuaClassType(Type klass, Type luaClass)
	{
		CodeGeneration.Instance.RegisterLuaClassType(klass, luaClass);
	}

	public void LoadCLRPackage()
	{
		_luaState.DoString("if not luanet then require'luanet'end;local a,b=luanet.import_type,luanet.load_assembly;local c={__index=function(d,e)local f=rawget(d,e)if f==nil then f=a(d.packageName..\".\"..e)if f==nil then f=a(e)end;d[e]=f end;return f end}function luanet.namespace(g)if type(g)=='table'then local h={}for i=1,#g do h[i]=luanet.namespace(g[i])end;return unpack(h)end;local j={packageName=g}setmetatable(j,c)return j end;local k,l;local function m()l={}k={__index=function(n,e)for i,d in ipairs(l)do local f=d[e]if f then _G[e]=f;return f end end end}setmetatable(_G,k)end;function CLRPackage(o,p)p=p or o;local q=pcall(b,o)return luanet.namespace(p)end;function import(o,p)if not k then m()end;if not p then local i=o:find('%.dll$')if i then p=o:sub(1,i-1)else p=o end end;local j=CLRPackage(o,p)table.insert(l,j)return j end;function luanet.make_array(r,s)local t=r[#s]for i,u in ipairs(s)do t:SetValue(u,i-1)end;return t end;function luanet.each(v)local w=v:GetEnumerator()return function()if w:MoveNext()then return w.Current end end end");
	}

	public Delegate GetFunction(Type delegateType, string fullPath)
	{
		return CodeGeneration.Instance.GetDelegate(delegateType, GetFunction(fullPath));
	}

	internal object[] CallFunction(object function, object[] args)
	{
		return CallFunction(function, args, null);
	}

	internal object[] CallFunction(object function, object[] args, Type[] returnTypes)
	{
		int num = 0;
		int num2 = _luaState.GetTop();
		if (!_luaState.CheckStack(args.Length + 6))
		{
			throw new LuaException("Lua stack overflow");
		}
		_translator.Push(_luaState, function);
		if (args.Length != 0)
		{
			num = args.Length;
			for (int i = 0; i < args.Length; i++)
			{
				_translator.Push(_luaState, args[i]);
			}
		}
		_executing = true;
		try
		{
			int errorFunctionIndex = 0;
			if (UseTraceback)
			{
				errorFunctionIndex = PushDebugTraceback(_luaState, num);
				num2++;
			}
			if (_luaState.PCall(num, -1, errorFunctionIndex) != LuaStatus.OK)
			{
				ThrowExceptionFromError(num2);
			}
		}
		finally
		{
			_executing = false;
		}
		if (returnTypes != null)
		{
			return _translator.PopValues(_luaState, num2, returnTypes);
		}
		return _translator.PopValues(_luaState, num2);
	}

	private void SetObject(string[] remainingPath, object val)
	{
		for (int i = 0; i < remainingPath.Length - 1; i++)
		{
			_luaState.PushString(remainingPath[i]);
			_luaState.GetTable(-2);
		}
		_luaState.PushString(remainingPath[remainingPath.Length - 1]);
		_translator.Push(_luaState, val);
		_luaState.SetTable(-3);
	}

	private string[] FullPathToArray(string fullPath)
	{
		return fullPath.SplitWithEscape('.', '\\').ToArray();
	}

	public void NewTable(string fullPath)
	{
		string[] array = FullPathToArray(fullPath);
		int top = _luaState.GetTop();
		if (array.Length == 1)
		{
			_luaState.NewTable();
			_luaState.SetGlobal(fullPath);
		}
		else
		{
			_luaState.GetGlobal(array[0]);
			for (int i = 1; i < array.Length - 1; i++)
			{
				_luaState.PushString(array[i]);
				_luaState.GetTable(-2);
			}
			_luaState.PushString(array[array.Length - 1]);
			_luaState.NewTable();
			_luaState.SetTable(-3);
		}
		_luaState.SetTop(top);
	}

	public Dictionary<object, object> GetTableDict(LuaTable table)
	{
		if (table == null)
		{
			throw new ArgumentNullException("table");
		}
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		int top = _luaState.GetTop();
		_translator.Push(_luaState, table);
		_luaState.PushNil();
		while (_luaState.Next(-2))
		{
			dictionary[_translator.GetObject(_luaState, -2)] = _translator.GetObject(_luaState, -1);
			_luaState.SetTop(-2);
		}
		_luaState.SetTop(top);
		return dictionary;
	}

	public int SetDebugHook(LuaHookMask mask, int count)
	{
		if (_hookCallback == null)
		{
			_hookCallback = DebugHookCallback;
			_luaState.SetHook(_hookCallback, mask, count);
		}
		return -1;
	}

	public void RemoveDebugHook()
	{
		_hookCallback = null;
		_luaState.SetHook(null, LuaHookMask.Disabled, 0);
	}

	public LuaHookMask GetHookMask()
	{
		return _luaState.HookMask;
	}

	public int GetHookCount()
	{
		return _luaState.HookCount;
	}

	public string GetLocal(LuaDebug luaDebug, int n)
	{
		return _luaState.GetLocal(luaDebug, n);
	}

	public string SetLocal(LuaDebug luaDebug, int n)
	{
		return _luaState.SetLocal(luaDebug, n);
	}

	public int GetStack(int level, ref LuaDebug ar)
	{
		return _luaState.GetStack(level, ref ar);
	}

	public bool GetInfo(string what, ref LuaDebug ar)
	{
		return _luaState.GetInfo(what, ref ar);
	}

	public string GetUpValue(int funcindex, int n)
	{
		return _luaState.GetUpValue(funcindex, n);
	}

	public string SetUpValue(int funcindex, int n)
	{
		return _luaState.SetUpValue(funcindex, n);
	}

	[MonoPInvokeCallback(typeof(LuaHookFunction))]
	private static void DebugHookCallback(IntPtr luaState, IntPtr luaDebug)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		lua.GetStack(0, luaDebug);
		if (lua.GetInfo("Snlu", luaDebug))
		{
			LuaDebug luaDebug2 = LuaDebug.FromIntPtr(luaDebug);
			ObjectTranslatorPool.Instance.Find(lua).Interpreter.DebugHookCallbackInternal(luaDebug2);
		}
	}

	private void DebugHookCallbackInternal(LuaDebug luaDebug)
	{
		try
		{
			this.DebugHook?.Invoke(this, new DebugHookEventArgs(luaDebug));
		}
		catch (Exception ex)
		{
			OnHookException(new HookExceptionEventArgs(ex));
		}
	}

	private void OnHookException(HookExceptionEventArgs e)
	{
		this.HookException?.Invoke(this, e);
	}

	public object Pop()
	{
		int top = _luaState.GetTop();
		return _translator.PopValues(_luaState, top - 1)[0];
	}

	public void Push(object value)
	{
		_translator.Push(_luaState, value);
	}

	internal void DisposeInternal(int reference, bool finalized)
	{
		if (finalized && _translator != null)
		{
			_translator.AddFinalizedReference(reference);
		}
		else if (_luaState != null && !finalized)
		{
			_luaState.Unref(reference);
		}
	}

	internal object RawGetObject(int reference, string field)
	{
		int top = _luaState.GetTop();
		_luaState.GetRef(reference);
		_luaState.PushString(field);
		_luaState.RawGet(-2);
		object result = _translator.GetObject(_luaState, -1);
		_luaState.SetTop(top);
		return result;
	}

	internal object GetObject(int reference, string field)
	{
		int top = _luaState.GetTop();
		_luaState.GetRef(reference);
		object result = GetObject(FullPathToArray(field));
		_luaState.SetTop(top);
		return result;
	}

	internal object GetObject(int reference, object field)
	{
		int top = _luaState.GetTop();
		_luaState.GetRef(reference);
		_translator.Push(_luaState, field);
		_luaState.GetTable(-2);
		object result = _translator.GetObject(_luaState, -1);
		_luaState.SetTop(top);
		return result;
	}

	internal void SetObject(int reference, string field, object val)
	{
		int top = _luaState.GetTop();
		_luaState.GetRef(reference);
		SetObject(FullPathToArray(field), val);
		_luaState.SetTop(top);
	}

	internal void SetObject(int reference, object field, object val)
	{
		int top = _luaState.GetTop();
		_luaState.GetRef(reference);
		_translator.Push(_luaState, field);
		_translator.Push(_luaState, val);
		_luaState.SetTable(-3);
		_luaState.SetTop(top);
	}

	internal KeraLua.Lua GetThreadState(int reference)
	{
		int top = _luaState.GetTop();
		_luaState.GetRef(reference);
		KeraLua.Lua result = _luaState.ToThread(-1);
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
		_luaState.XMove(to._luaState, index);
		_luaState.SetTop(top);
	}

	public void XMove(LuaThread thread, object val, int index = 1)
	{
		int top = _luaState.GetTop();
		_translator.Push(_luaState, val);
		_luaState.XMove(thread.State, index);
		_luaState.SetTop(top);
	}

	public KeraLua.Lua NewThread(out LuaThread thread)
	{
		int top = _luaState.GetTop();
		KeraLua.Lua result = _luaState.NewThread();
		thread = (LuaThread)_translator.GetObject(_luaState, -1);
		_luaState.SetTop(top);
		return result;
	}

	public KeraLua.Lua NewThread(string fullPath)
	{
		string[] array = FullPathToArray(fullPath);
		int top = _luaState.GetTop();
		KeraLua.Lua result;
		if (array.Length == 1)
		{
			result = _luaState.NewThread();
			_luaState.SetGlobal(fullPath);
		}
		else
		{
			_luaState.GetGlobal(array[0]);
			for (int i = 1; i < array.Length - 1; i++)
			{
				_luaState.PushString(array[i]);
				_luaState.GetTable(-2);
			}
			_luaState.PushString(array[array.Length - 1]);
			result = _luaState.NewThread();
			_luaState.SetTable(-3);
		}
		_luaState.SetTop(top);
		return result;
	}

	public KeraLua.Lua NewThread(LuaFunction function, out LuaThread thread)
	{
		int top = _luaState.GetTop();
		KeraLua.Lua lua = _luaState.NewThread();
		thread = (LuaThread)_translator.GetObject(_luaState, -1);
		_translator.Push(_luaState, function);
		_luaState.XMove(lua, 1);
		_luaState.SetTop(top);
		return lua;
	}

	public void NewThread(string fullPath, LuaFunction function)
	{
		string[] array = FullPathToArray(fullPath);
		int top = _luaState.GetTop();
		KeraLua.Lua to;
		if (array.Length == 1)
		{
			to = _luaState.NewThread();
			_luaState.SetGlobal(fullPath);
		}
		else
		{
			_luaState.GetGlobal(array[0]);
			for (int i = 1; i < array.Length - 1; i++)
			{
				_luaState.PushString(array[i]);
				_luaState.GetTable(-2);
			}
			_luaState.PushString(array[array.Length - 1]);
			to = _luaState.NewThread();
			_luaState.SetTable(-3);
		}
		_translator.Push(_luaState, function);
		_luaState.XMove(to, 1);
		_luaState.SetTop(top);
	}

	public LuaFunction RegisterFunction(string path, MethodBase function)
	{
		return RegisterFunction(path, null, function);
	}

	public LuaFunction RegisterFunction(string path, object target, MethodBase function)
	{
		int top = _luaState.GetTop();
		LuaMethodWrapper luaMethodWrapper = new LuaMethodWrapper(_translator, target, new ProxyType(function.DeclaringType), function);
		_translator.Push(_luaState, new KeraLua.LuaFunction(luaMethodWrapper.InvokeFunction.Invoke));
		object value = _translator.GetObject(_luaState, -1);
		SetObjectToPath(path, value);
		LuaFunction function2 = GetFunction(path);
		_luaState.SetTop(top);
		return function2;
	}

	internal bool CompareRef(int ref1, int ref2)
	{
		int top = _luaState.GetTop();
		_luaState.GetRef(ref1);
		_luaState.GetRef(ref2);
		bool result = _luaState.AreEqual(-1, -2);
		_luaState.SetTop(top);
		return result;
	}

	internal void PushCSFunction(KeraLua.LuaFunction function)
	{
		_translator.PushFunction(_luaState, function);
	}

	~Lua()
	{
		Dispose();
	}

	public virtual void Dispose()
	{
		if (_translator != null)
		{
			_translator.PendingEvents.Dispose();
			if (_translator.Tag != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(_translator.Tag);
			}
			_translator = null;
		}
		Close();
		GC.SuppressFinalize(this);
	}
}
