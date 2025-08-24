using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;
using KeraLua;
using NLua.Exceptions;
using NLua.Extensions;
using NLua.Method;

namespace NLua;

public class ObjectTranslator
{
	private class ReferenceComparer : IEqualityComparer<object>
	{
		public new bool Equals(object x, object y)
		{
			if (x != null && y != null && x.GetType() == y.GetType() && x.GetType().IsValueType && y.GetType().IsValueType)
			{
				return x.Equals(y);
			}
			return x == y;
		}

		public int GetHashCode(object obj)
		{
			return obj.GetHashCode();
		}
	}

	private static readonly KeraLua.LuaFunction _registerTableFunction = RegisterTable;

	private static readonly KeraLua.LuaFunction _unregisterTableFunction = UnregisterTable;

	private static readonly KeraLua.LuaFunction _getMethodSigFunction = GetMethodSignature;

	private static readonly KeraLua.LuaFunction _getConstructorSigFunction = GetConstructorSignature;

	private static readonly KeraLua.LuaFunction _importTypeFunction = ImportType;

	private static readonly KeraLua.LuaFunction _loadAssemblyFunction = LoadAssembly;

	private static readonly KeraLua.LuaFunction _ctypeFunction = CType;

	private static readonly KeraLua.LuaFunction _enumFromIntFunction = EnumFromInt;

	private readonly Dictionary<object, int> _objectsBackMap = new Dictionary<object, int>(new ReferenceComparer());

	private readonly Dictionary<int, object> _objects = new Dictionary<int, object>();

	private readonly ConcurrentQueue<int> finalizedReferences = new ConcurrentQueue<int>();

	internal EventHandlerContainer PendingEvents = new EventHandlerContainer();

	private MetaFunctions metaFunctions;

	private List<Assembly> assemblies;

	internal CheckType typeChecker;

	internal Lua interpreter;

	private int _nextObj;

	private readonly IntPtr _tagPtr;

	public MetaFunctions MetaFunctionsInstance => metaFunctions;

	public Lua Interpreter => interpreter;

	public IntPtr Tag => _tagPtr;

	public ObjectTranslator(Lua interpreter, KeraLua.Lua luaState)
	{
		_tagPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
		this.interpreter = interpreter;
		typeChecker = new CheckType(this);
		metaFunctions = new MetaFunctions(this);
		assemblies = new List<Assembly>();
		CreateLuaObjectList(luaState);
		CreateIndexingMetaFunction(luaState);
		CreateBaseClassMetatable(luaState);
		CreateClassMetatable(luaState);
		CreateFunctionMetatable(luaState);
		SetGlobalFunctions(luaState);
	}

	private void CreateLuaObjectList(KeraLua.Lua luaState)
	{
		luaState.PushString("luaNet_objects");
		luaState.NewTable();
		luaState.NewTable();
		luaState.PushString("__mode");
		luaState.PushString("v");
		luaState.SetTable(-3);
		luaState.SetMetaTable(-2);
		luaState.SetTable(-1001000);
	}

	private void CreateIndexingMetaFunction(KeraLua.Lua luaState)
	{
		luaState.PushString("luaNet_indexfunction");
		luaState.DoString("local a={}local function b(c,d)local e=getmetatable(c)local f=e.cache[d]if f~=nil then if f==a then return nil end;return f else local g,h=get_object_member(c,d)if h then if g==nil then e.cache[d]=a else e.cache[d]=g end end;return g end end;return b");
		luaState.RawSet(LuaRegistry.Index);
	}

	private void CreateBaseClassMetatable(KeraLua.Lua luaState)
	{
		luaState.NewMetaTable("luaNet_searchbase");
		luaState.PushString("__gc");
		luaState.PushCFunction(MetaFunctions.GcFunction);
		luaState.SetTable(-3);
		luaState.PushString("__tostring");
		luaState.PushCFunction(MetaFunctions.ToStringFunction);
		luaState.SetTable(-3);
		luaState.PushString("__index");
		luaState.PushCFunction(MetaFunctions.BaseIndexFunction);
		luaState.SetTable(-3);
		luaState.PushString("__newindex");
		luaState.PushCFunction(MetaFunctions.NewIndexFunction);
		luaState.SetTable(-3);
		luaState.SetTop(-2);
	}

	private void CreateClassMetatable(KeraLua.Lua luaState)
	{
		luaState.NewMetaTable("luaNet_class");
		luaState.PushString("__gc");
		luaState.PushCFunction(MetaFunctions.GcFunction);
		luaState.SetTable(-3);
		luaState.PushString("__tostring");
		luaState.PushCFunction(MetaFunctions.ToStringFunction);
		luaState.SetTable(-3);
		luaState.PushString("__index");
		luaState.PushCFunction(MetaFunctions.ClassIndexFunction);
		luaState.SetTable(-3);
		luaState.PushString("__newindex");
		luaState.PushCFunction(MetaFunctions.ClassNewIndexFunction);
		luaState.SetTable(-3);
		luaState.PushString("__call");
		luaState.PushCFunction(MetaFunctions.CallConstructorFunction);
		luaState.SetTable(-3);
		luaState.SetTop(-2);
	}

	private void SetGlobalFunctions(KeraLua.Lua luaState)
	{
		luaState.PushCFunction(MetaFunctions.IndexFunction);
		luaState.SetGlobal("get_object_member");
		luaState.PushCFunction(_importTypeFunction);
		luaState.SetGlobal("import_type");
		luaState.PushCFunction(_loadAssemblyFunction);
		luaState.SetGlobal("load_assembly");
		luaState.PushCFunction(_registerTableFunction);
		luaState.SetGlobal("make_object");
		luaState.PushCFunction(_unregisterTableFunction);
		luaState.SetGlobal("free_object");
		luaState.PushCFunction(_getMethodSigFunction);
		luaState.SetGlobal("get_method_bysig");
		luaState.PushCFunction(_getConstructorSigFunction);
		luaState.SetGlobal("get_constructor_bysig");
		luaState.PushCFunction(_ctypeFunction);
		luaState.SetGlobal("ctype");
		luaState.PushCFunction(_enumFromIntFunction);
		luaState.SetGlobal("enum");
	}

	private void CreateFunctionMetatable(KeraLua.Lua luaState)
	{
		luaState.NewMetaTable("luaNet_function");
		luaState.PushString("__gc");
		luaState.PushCFunction(MetaFunctions.GcFunction);
		luaState.SetTable(-3);
		luaState.PushString("__call");
		luaState.PushCFunction(MetaFunctions.ExecuteDelegateFunction);
		luaState.SetTable(-3);
		luaState.SetTop(-2);
	}

	internal void ThrowError(KeraLua.Lua luaState, object e)
	{
		int top = luaState.GetTop();
		luaState.Where(1);
		object[] array = PopValues(luaState, top);
		string source = string.Empty;
		if (array.Length != 0)
		{
			source = array[0].ToString();
		}
		string text = e as string;
		if (text != null)
		{
			if (interpreter.UseTraceback)
			{
				text = text + Environment.NewLine + interpreter.GetDebugTraceback();
			}
			e = new LuaScriptException(text, source);
		}
		else if (e is Exception ex)
		{
			if (interpreter.UseTraceback)
			{
				ex.Data["Traceback"] = interpreter.GetDebugTraceback();
			}
			e = new LuaScriptException(ex, source);
		}
		Push(luaState, e);
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int LoadAssembly(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = objectTranslator.LoadAssemblyInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	private int LoadAssemblyInternal(KeraLua.Lua luaState)
	{
		try
		{
			string text = luaState.ToString(1, callMetamethod: false);
			Assembly assembly = null;
			Exception ex = null;
			try
			{
				assembly = Assembly.Load(text);
			}
			catch (BadImageFormatException)
			{
			}
			catch (FileNotFoundException ex3)
			{
				ex = ex3;
			}
			if (assembly == null)
			{
				try
				{
					assembly = Assembly.Load(AssemblyName.GetAssemblyName(text));
				}
				catch (FileNotFoundException ex4)
				{
					ex = ex4;
				}
				if (assembly == null)
				{
					AssemblyName name = assemblies[0].GetName();
					AssemblyName assemblyName = new AssemblyName();
					assemblyName.Name = text;
					assemblyName.CultureInfo = name.CultureInfo;
					assemblyName.Version = name.Version;
					assemblyName.SetPublicKeyToken(name.GetPublicKeyToken());
					assemblyName.SetPublicKey(name.GetPublicKey());
					assembly = Assembly.Load(assemblyName);
					if (assembly != null)
					{
						ex = null;
					}
				}
				if (ex != null)
				{
					ThrowError(luaState, ex);
					return 1;
				}
			}
			if (assembly != null && !assemblies.Contains(assembly))
			{
				assemblies.Add(assembly);
			}
		}
		catch (Exception e)
		{
			ThrowError(luaState, e);
			return 1;
		}
		return 0;
	}

	internal Type FindType(string className)
	{
		foreach (Assembly assembly in assemblies)
		{
			Type type = assembly.GetType(className);
			if (type != null)
			{
				return type;
			}
		}
		return null;
	}

	public bool TryGetExtensionMethod(Type type, string name, out MethodInfo method)
	{
		method = GetExtensionMethod(type, name);
		return method != null;
	}

	public MethodInfo GetExtensionMethod(Type type, string name)
	{
		return type.GetExtensionMethod(name, assemblies);
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int ImportType(IntPtr luaState)
	{
		KeraLua.Lua luaState2 = KeraLua.Lua.FromIntPtr(luaState);
		return ObjectTranslatorPool.Instance.Find(luaState2).ImportTypeInternal(luaState2);
	}

	private int ImportTypeInternal(KeraLua.Lua luaState)
	{
		string className = luaState.ToString(1, callMetamethod: false);
		Type type = FindType(className);
		if (type != null)
		{
			PushType(luaState, type);
		}
		else
		{
			luaState.PushNil();
		}
		return 1;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int RegisterTable(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = objectTranslator.RegisterTableInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	private int RegisterTableInternal(KeraLua.Lua luaState)
	{
		if (luaState.Type(1) != LuaType.Table)
		{
			ThrowError(luaState, "register_table: first arg is not a table");
			return 1;
		}
		LuaTable table = GetTable(luaState, 1);
		string text = luaState.ToString(2, callMetamethod: false);
		if (string.IsNullOrEmpty(text))
		{
			ThrowError(luaState, "register_table: superclass name can not be null");
			return 1;
		}
		Type type = FindType(text);
		if (type == null)
		{
			ThrowError(luaState, "register_table: can not find superclass '" + text + "'");
			return 1;
		}
		object classInstance = CodeGeneration.Instance.GetClassInstance(type, table);
		PushObject(luaState, classInstance, "luaNet_metatable");
		luaState.NewTable();
		luaState.PushString("__index");
		luaState.PushCopy(-3);
		luaState.SetTable(-3);
		luaState.PushString("__newindex");
		luaState.PushCopy(-3);
		luaState.SetTable(-3);
		luaState.SetMetaTable(1);
		luaState.PushString("base");
		int index = AddObject(classInstance);
		PushNewObject(luaState, classInstance, index, "luaNet_searchbase");
		luaState.RawSet(1);
		return 0;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int UnregisterTable(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int result = objectTranslator.UnregisterTableInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return result;
	}

	private int UnregisterTableInternal(KeraLua.Lua luaState)
	{
		if (!luaState.GetMetaTable(1))
		{
			ThrowError(luaState, "unregister_table: arg is not valid table");
			return 1;
		}
		luaState.PushString("__index");
		luaState.GetTable(-2);
		object rawNetObject = GetRawNetObject(luaState, -1);
		if (rawNetObject == null)
		{
			ThrowError(luaState, "unregister_table: arg is not valid table");
			return 1;
		}
		FieldInfo field = rawNetObject.GetType().GetField("__luaInterface_luaTable");
		if (field == null)
		{
			ThrowError(luaState, "unregister_table: arg is not valid table");
			return 1;
		}
		field.SetValue(rawNetObject, null);
		luaState.PushNil();
		luaState.SetMetaTable(1);
		luaState.PushString("base");
		luaState.PushNil();
		luaState.SetTable(1);
		return 0;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int GetMethodSignature(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int methodSignatureInternal = objectTranslator.GetMethodSignatureInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return methodSignatureInternal;
	}

	private int GetMethodSignatureInternal(KeraLua.Lua luaState)
	{
		int num = luaState.CheckUObject(1, "luaNet_class");
		ProxyType proxyType;
		object obj;
		if (num != -1)
		{
			proxyType = (ProxyType)_objects[num];
			obj = null;
		}
		else
		{
			obj = GetRawNetObject(luaState, 1);
			if (obj == null)
			{
				ThrowError(luaState, "get_method_bysig: first arg is not type or object reference");
				return 1;
			}
			proxyType = new ProxyType(obj.GetType());
		}
		string name = luaState.ToString(2, callMetamethod: false);
		Type[] array = new Type[luaState.GetTop() - 2];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = FindType(luaState.ToString(i + 3, callMetamethod: false));
		}
		try
		{
			MethodInfo method = proxyType.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, array);
			KeraLua.LuaFunction invokeFunction = new LuaMethodWrapper(this, obj, proxyType, method).InvokeFunction;
			PushFunction(luaState, invokeFunction);
		}
		catch (Exception e)
		{
			ThrowError(luaState, e);
		}
		return 1;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int GetConstructorSignature(IntPtr luaState)
	{
		KeraLua.Lua lua = KeraLua.Lua.FromIntPtr(luaState);
		ObjectTranslator objectTranslator = ObjectTranslatorPool.Instance.Find(lua);
		int constructorSignatureInternal = objectTranslator.GetConstructorSignatureInternal(lua);
		if (objectTranslator.GetObject(lua, -1) is LuaScriptException)
		{
			return lua.Error();
		}
		return constructorSignatureInternal;
	}

	private int GetConstructorSignatureInternal(KeraLua.Lua luaState)
	{
		ProxyType proxyType = null;
		int num = luaState.CheckUObject(1, "luaNet_class");
		if (num != -1)
		{
			proxyType = (ProxyType)_objects[num];
		}
		if (proxyType == null)
		{
			ThrowError(luaState, "get_constructor_bysig: first arg is invalid type reference");
			return 1;
		}
		Type[] array = new Type[luaState.GetTop() - 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = FindType(luaState.ToString(i + 2, callMetamethod: false));
		}
		try
		{
			ConstructorInfo constructor = proxyType.UnderlyingSystemType.GetConstructor(array);
			KeraLua.LuaFunction invokeFunction = new LuaMethodWrapper(this, null, proxyType, constructor).InvokeFunction;
			PushFunction(luaState, invokeFunction);
		}
		catch (Exception e)
		{
			ThrowError(luaState, e);
		}
		return 1;
	}

	internal void PushType(KeraLua.Lua luaState, Type t)
	{
		PushObject(luaState, new ProxyType(t), "luaNet_class");
	}

	internal void PushFunction(KeraLua.Lua luaState, KeraLua.LuaFunction func)
	{
		PushObject(luaState, func, "luaNet_function");
	}

	internal void PushObject(KeraLua.Lua luaState, object o, string metatable)
	{
		int value = -1;
		if (o == null)
		{
			luaState.PushNil();
			return;
		}
		if ((!o.GetType().IsValueType || o.GetType().IsEnum) && _objectsBackMap.TryGetValue(o, out value))
		{
			luaState.GetMetaTable("luaNet_objects");
			luaState.RawGetInteger(-1, value);
			if (luaState.Type(-1) != LuaType.Nil)
			{
				luaState.Remove(-2);
				return;
			}
			luaState.Remove(-1);
			luaState.Remove(-1);
			CollectObject(o, value);
		}
		value = AddObject(o);
		PushNewObject(luaState, o, value, metatable);
	}

	private void PushNewObject(KeraLua.Lua luaState, object o, int index, string metatable)
	{
		if (metatable == "luaNet_metatable")
		{
			luaState.GetMetaTable(o.GetType().AssemblyQualifiedName);
			if (luaState.IsNil(-1))
			{
				luaState.SetTop(-2);
				luaState.NewMetaTable(o.GetType().AssemblyQualifiedName);
				luaState.PushString("cache");
				luaState.NewTable();
				luaState.RawSet(-3);
				luaState.PushLightUserData(_tagPtr);
				luaState.PushNumber(1.0);
				luaState.RawSet(-3);
				luaState.PushString("__index");
				luaState.PushString("luaNet_indexfunction");
				luaState.RawGet(LuaRegistry.Index);
				luaState.RawSet(-3);
				luaState.PushString("__gc");
				luaState.PushCFunction(MetaFunctions.GcFunction);
				luaState.RawSet(-3);
				luaState.PushString("__tostring");
				luaState.PushCFunction(MetaFunctions.ToStringFunction);
				luaState.RawSet(-3);
				luaState.PushString("__newindex");
				luaState.PushCFunction(MetaFunctions.NewIndexFunction);
				luaState.RawSet(-3);
				RegisterOperatorsFunctions(luaState, o.GetType());
				RegisterCallMethodForDelegate(luaState, o);
			}
		}
		else
		{
			luaState.GetMetaTable(metatable);
		}
		luaState.GetMetaTable("luaNet_objects");
		luaState.NewUData(index);
		luaState.PushCopy(-3);
		luaState.Remove(-4);
		luaState.SetMetaTable(-2);
		luaState.PushCopy(-1);
		luaState.RawSetInteger(-3, index);
		luaState.Remove(-2);
	}

	private void RegisterCallMethodForDelegate(KeraLua.Lua luaState, object o)
	{
		if (o is Delegate)
		{
			luaState.PushString("__call");
			luaState.PushCFunction(MetaFunctions.CallDelegateFunction);
			luaState.RawSet(-3);
		}
	}

	private void RegisterOperatorsFunctions(KeraLua.Lua luaState, Type type)
	{
		if (type.HasAdditionOperator())
		{
			luaState.PushString("__add");
			luaState.PushCFunction(MetaFunctions.AddFunction);
			luaState.RawSet(-3);
		}
		if (type.HasSubtractionOperator())
		{
			luaState.PushString("__sub");
			luaState.PushCFunction(MetaFunctions.SubtractFunction);
			luaState.RawSet(-3);
		}
		if (type.HasMultiplyOperator())
		{
			luaState.PushString("__mul");
			luaState.PushCFunction(MetaFunctions.MultiplyFunction);
			luaState.RawSet(-3);
		}
		if (type.HasDivisionOperator())
		{
			luaState.PushString("__div");
			luaState.PushCFunction(MetaFunctions.DivisionFunction);
			luaState.RawSet(-3);
		}
		if (type.HasModulusOperator())
		{
			luaState.PushString("__mod");
			luaState.PushCFunction(MetaFunctions.ModulosFunction);
			luaState.RawSet(-3);
		}
		if (type.HasUnaryNegationOperator())
		{
			luaState.PushString("__unm");
			luaState.PushCFunction(MetaFunctions.UnaryNegationFunction);
			luaState.RawSet(-3);
		}
		if (type.HasEqualityOperator())
		{
			luaState.PushString("__eq");
			luaState.PushCFunction(MetaFunctions.EqualFunction);
			luaState.RawSet(-3);
		}
		if (type.HasLessThanOperator())
		{
			luaState.PushString("__lt");
			luaState.PushCFunction(MetaFunctions.LessThanFunction);
			luaState.RawSet(-3);
		}
		if (type.HasLessThanOrEqualOperator())
		{
			luaState.PushString("__le");
			luaState.PushCFunction(MetaFunctions.LessThanOrEqualFunction);
			luaState.RawSet(-3);
		}
	}

	internal object GetAsType(KeraLua.Lua luaState, int stackPos, Type paramType)
	{
		return typeChecker.CheckLuaType(luaState, stackPos, paramType)?.Invoke(luaState, stackPos);
	}

	internal void CollectObject(int udata)
	{
		if (_objects.TryGetValue(udata, out var value))
		{
			CollectObject(value, udata);
		}
	}

	private void CollectObject(object o, int udata)
	{
		_objects.Remove(udata);
		if (!o.GetType().IsValueType || o.GetType().IsEnum)
		{
			_objectsBackMap.Remove(o);
		}
	}

	private int AddObject(object obj)
	{
		int num = _nextObj++;
		_objects[num] = obj;
		if (!obj.GetType().IsValueType || obj.GetType().IsEnum)
		{
			_objectsBackMap[obj] = num;
		}
		return num;
	}

	internal object GetObject(KeraLua.Lua luaState, int index)
	{
		switch (luaState.Type(index))
		{
		case LuaType.Number:
			if (luaState.IsInteger(index))
			{
				return luaState.ToInteger(index);
			}
			return luaState.ToNumber(index);
		case LuaType.String:
			return luaState.ToString(index, callMetamethod: false);
		case LuaType.Boolean:
			return luaState.ToBoolean(index);
		case LuaType.Table:
			return GetTable(luaState, index);
		case LuaType.Function:
			return GetFunction(luaState, index);
		case LuaType.UserData:
		{
			int num = luaState.ToNetObject(index, Tag);
			if (num == -1)
			{
				return GetUserData(luaState, index);
			}
			return _objects[num];
		}
		case LuaType.Thread:
			return GetThread(luaState, index);
		default:
			return null;
		}
	}

	internal LuaTable GetTable(KeraLua.Lua luaState, int index)
	{
		CleanFinalizedReferences(luaState);
		luaState.PushCopy(index);
		int num = luaState.Ref(LuaRegistry.Index);
		if (num == -1)
		{
			return null;
		}
		return new LuaTable(num, interpreter);
	}

	internal LuaThread GetThread(KeraLua.Lua luaState, int index)
	{
		CleanFinalizedReferences(luaState);
		luaState.PushCopy(index);
		int num = luaState.Ref(LuaRegistry.Index);
		if (num == -1)
		{
			return null;
		}
		return new LuaThread(num, interpreter);
	}

	internal LuaUserData GetUserData(KeraLua.Lua luaState, int index)
	{
		CleanFinalizedReferences(luaState);
		luaState.PushCopy(index);
		int num = luaState.Ref(LuaRegistry.Index);
		if (num == -1)
		{
			return null;
		}
		return new LuaUserData(num, interpreter);
	}

	internal LuaFunction GetFunction(KeraLua.Lua luaState, int index)
	{
		CleanFinalizedReferences(luaState);
		luaState.PushCopy(index);
		int num = luaState.Ref(LuaRegistry.Index);
		if (num == -1)
		{
			return null;
		}
		return new LuaFunction(num, interpreter);
	}

	internal object GetNetObject(KeraLua.Lua luaState, int index)
	{
		int num = luaState.ToNetObject(index, Tag);
		if (num == -1)
		{
			return null;
		}
		return _objects[num];
	}

	internal object GetRawNetObject(KeraLua.Lua luaState, int index)
	{
		int num = luaState.RawNetObj(index);
		if (num == -1)
		{
			return null;
		}
		return _objects[num];
	}

	internal object[] PopValues(KeraLua.Lua luaState, int oldTop)
	{
		int top = luaState.GetTop();
		if (oldTop == top)
		{
			return new object[0];
		}
		List<object> list = new List<object>();
		for (int i = oldTop + 1; i <= top; i++)
		{
			list.Add(GetObject(luaState, i));
		}
		luaState.SetTop(oldTop);
		return list.ToArray();
	}

	internal object[] PopValues(KeraLua.Lua luaState, int oldTop, Type[] popTypes)
	{
		int top = luaState.GetTop();
		if (oldTop == top)
		{
			return new object[0];
		}
		List<object> list = new List<object>();
		int num = ((popTypes[0] == typeof(void)) ? 1 : 0);
		for (int i = oldTop + 1; i <= top; i++)
		{
			list.Add(GetAsType(luaState, i, popTypes[num]));
			num++;
		}
		luaState.SetTop(oldTop);
		return list.ToArray();
	}

	private static bool IsILua(object o)
	{
		if (o is ILuaGeneratedType)
		{
			return o.GetType().GetInterface("ILuaGeneratedType", ignoreCase: true) != null;
		}
		return false;
	}

	internal void Push(KeraLua.Lua luaState, object o)
	{
		if (o == null)
		{
			luaState.PushNil();
		}
		else if (o is sbyte b)
		{
			luaState.PushInteger(b);
		}
		else if (o is byte b2)
		{
			luaState.PushInteger(b2);
		}
		else if (o is short num)
		{
			luaState.PushInteger(num);
		}
		else if (o is ushort num2)
		{
			luaState.PushInteger(num2);
		}
		else if (o is int num3)
		{
			luaState.PushInteger(num3);
		}
		else if (o is uint num4)
		{
			luaState.PushInteger(num4);
		}
		else if (o is long n)
		{
			luaState.PushInteger(n);
		}
		else if (o is ulong n2)
		{
			luaState.PushInteger((long)n2);
		}
		else if (o is char c)
		{
			luaState.PushInteger(c);
		}
		else if (o is float num5)
		{
			luaState.PushNumber(num5);
		}
		else if (o is decimal num6)
		{
			luaState.PushNumber((double)num6);
		}
		else if (o is double number)
		{
			luaState.PushNumber(number);
		}
		else if (o is string value)
		{
			luaState.PushString(value);
		}
		else if (o is bool b3)
		{
			luaState.PushBoolean(b3);
		}
		else if (IsILua(o))
		{
			((ILuaGeneratedType)o).LuaInterfaceGetLuaTable().Push(luaState);
		}
		else if (o is LuaTable luaTable)
		{
			luaTable.Push(luaState);
		}
		else if (o is LuaThread luaThread)
		{
			luaThread.Push(luaState);
		}
		else if (o is KeraLua.LuaFunction func)
		{
			PushFunction(luaState, func);
		}
		else if (o is LuaFunction luaFunction)
		{
			luaFunction.Push(luaState);
		}
		else if (o is LuaUserData luaUserData)
		{
			luaUserData.Push(luaState);
		}
		else
		{
			PushObject(luaState, o, "luaNet_metatable");
		}
	}

	internal bool MatchParameters(KeraLua.Lua luaState, MethodBase method, MethodCache methodCache, int skipParam)
	{
		return metaFunctions.MatchParameters(luaState, method, methodCache, skipParam);
	}

	internal Array TableToArray(KeraLua.Lua luaState, ExtractValue extractValue, Type paramArrayType, int startIndex, int count)
	{
		return metaFunctions.TableToArray(luaState, extractValue, paramArrayType, ref startIndex, count);
	}

	private Type TypeOf(KeraLua.Lua luaState, int idx)
	{
		int num = luaState.CheckUObject(idx, "luaNet_class");
		if (num == -1)
		{
			return null;
		}
		return ((ProxyType)_objects[num]).UnderlyingSystemType;
	}

	private static int PushError(KeraLua.Lua luaState, string msg)
	{
		luaState.PushNil();
		luaState.PushString(msg);
		return 2;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int CType(IntPtr luaState)
	{
		KeraLua.Lua luaState2 = KeraLua.Lua.FromIntPtr(luaState);
		return ObjectTranslatorPool.Instance.Find(luaState2).CTypeInternal(luaState2);
	}

	private int CTypeInternal(KeraLua.Lua luaState)
	{
		Type type = TypeOf(luaState, 1);
		if (type == null)
		{
			return PushError(luaState, "Not a CLR Class");
		}
		PushObject(luaState, type, "luaNet_metatable");
		return 1;
	}

	[MonoPInvokeCallback(typeof(KeraLua.LuaFunction))]
	private static int EnumFromInt(IntPtr luaState)
	{
		KeraLua.Lua luaState2 = KeraLua.Lua.FromIntPtr(luaState);
		return ObjectTranslatorPool.Instance.Find(luaState2).EnumFromIntInternal(luaState2);
	}

	private int EnumFromIntInternal(KeraLua.Lua luaState)
	{
		Type type = TypeOf(luaState, 1);
		if (type == null || !type.IsEnum)
		{
			return PushError(luaState, "Not an Enum.");
		}
		object o = null;
		switch (luaState.Type(2))
		{
		case LuaType.Number:
		{
			int value2 = (int)luaState.ToNumber(2);
			o = Enum.ToObject(type, value2);
			break;
		}
		case LuaType.String:
		{
			string value = luaState.ToString(2, callMetamethod: false);
			string text = null;
			try
			{
				o = Enum.Parse(type, value, ignoreCase: true);
			}
			catch (ArgumentException ex)
			{
				text = ex.Message;
			}
			if (text != null)
			{
				return PushError(luaState, text);
			}
			break;
		}
		default:
			return PushError(luaState, "Second argument must be a integer or a string.");
		}
		PushObject(luaState, o, "luaNet_metatable");
		return 1;
	}

	internal void AddFinalizedReference(int reference)
	{
		finalizedReferences.Enqueue(reference);
	}

	private void CleanFinalizedReferences(KeraLua.Lua state)
	{
		if (finalizedReferences.Count != 0)
		{
			int result;
			while (finalizedReferences.TryDequeue(out result))
			{
				state.Unref(LuaRegistry.Index, result);
			}
		}
	}
}
