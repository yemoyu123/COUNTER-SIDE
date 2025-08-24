using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace KeraLua;

public class Lua : IDisposable
{
	private IntPtr _luaState;

	private readonly Lua _mainState;

	public IntPtr Handle => _luaState;

	public Encoding Encoding { get; set; }

	public IntPtr ExtraSpace => _luaState - IntPtr.Size;

	public Lua MainThread => _mainState ?? this;

	public LuaHookFunction Hook => NativeMethods.lua_gethook(_luaState).ToLuaHookFunction();

	public int HookCount => NativeMethods.lua_gethookcount(_luaState);

	public LuaHookMask HookMask => (LuaHookMask)NativeMethods.lua_gethookmask(_luaState);

	public bool IsYieldable => NativeMethods.lua_isyieldable(_luaState) != 0;

	public LuaStatus Status => (LuaStatus)NativeMethods.lua_status(_luaState);

	public Lua(bool openLibs = true)
	{
		Encoding = Encoding.ASCII;
		_luaState = NativeMethods.luaL_newstate();
		if (openLibs)
		{
			OpenLibs();
		}
		SetExtraObject(this, weak: true);
	}

	public Lua(LuaAlloc allocator, IntPtr ud)
	{
		Encoding = Encoding.ASCII;
		_luaState = NativeMethods.lua_newstate(allocator.ToFunctionPointer(), ud);
		SetExtraObject(this, weak: true);
	}

	private Lua(IntPtr luaThread, Lua mainState)
	{
		_mainState = mainState;
		_luaState = luaThread;
		Encoding = mainState.Encoding;
		SetExtraObject(this, weak: false);
		GC.SuppressFinalize(this);
	}

	public static Lua FromIntPtr(IntPtr luaState)
	{
		if (luaState == IntPtr.Zero)
		{
			return null;
		}
		Lua extraObject = GetExtraObject<Lua>(luaState);
		if (extraObject != null && extraObject._luaState == luaState)
		{
			return extraObject;
		}
		return new Lua(luaState, extraObject.MainThread);
	}

	~Lua()
	{
		Dispose(disposing: false);
	}

	protected virtual void Dispose(bool disposing)
	{
		Close();
	}

	public void Close()
	{
		if (!(_luaState == IntPtr.Zero) && _mainState == null)
		{
			NativeMethods.lua_close(_luaState);
			_luaState = IntPtr.Zero;
			GC.SuppressFinalize(this);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private void SetExtraObject<T>(T obj, bool weak) where T : class
	{
		GCHandle value = GCHandle.Alloc(obj, (!weak) ? GCHandleType.Normal : GCHandleType.Weak);
		Marshal.WriteIntPtr(_luaState - IntPtr.Size, GCHandle.ToIntPtr(value));
	}

	private static T GetExtraObject<T>(IntPtr luaState) where T : class
	{
		GCHandle gCHandle = GCHandle.FromIntPtr(Marshal.ReadIntPtr(luaState - IntPtr.Size));
		if (!gCHandle.IsAllocated)
		{
			return null;
		}
		return (T)gCHandle.Target;
	}

	public int AbsIndex(int index)
	{
		return NativeMethods.lua_absindex(_luaState, index);
	}

	public void Arith(LuaOperation operation)
	{
		NativeMethods.lua_arith(_luaState, (int)operation);
	}

	public LuaFunction AtPanic(LuaFunction panicFunction)
	{
		IntPtr panicf = panicFunction.ToFunctionPointer();
		return NativeMethods.lua_atpanic(_luaState, panicf).ToLuaFunction();
	}

	public void Call(int arguments, int results)
	{
		NativeMethods.lua_callk(_luaState, arguments, results, IntPtr.Zero, IntPtr.Zero);
	}

	public void CallK(int arguments, int results, int context, LuaKFunction continuation)
	{
		IntPtr k = continuation.ToFunctionPointer();
		NativeMethods.lua_callk(_luaState, arguments, results, (IntPtr)context, k);
	}

	public bool CheckStack(int nExtraSlots)
	{
		return NativeMethods.lua_checkstack(_luaState, nExtraSlots) != 0;
	}

	public bool Compare(int index1, int index2, LuaCompare comparison)
	{
		return NativeMethods.lua_compare(_luaState, index1, index2, (int)comparison) != 0;
	}

	public void Concat(int n)
	{
		NativeMethods.lua_concat(_luaState, n);
	}

	public void Copy(int fromIndex, int toIndex)
	{
		NativeMethods.lua_copy(_luaState, fromIndex, toIndex);
	}

	public void CreateTable(int elements, int records)
	{
		NativeMethods.lua_createtable(_luaState, elements, records);
	}

	public int Dump(LuaWriter writer, IntPtr data, bool stripDebug)
	{
		return NativeMethods.lua_dump(_luaState, writer.ToFunctionPointer(), data, stripDebug ? 1 : 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Error()
	{
		return NativeMethods.lua_error(_luaState);
	}

	public int GarbageCollector(LuaGC what, int data)
	{
		return NativeMethods.lua_gc(_luaState, (int)what, data);
	}

	public int GarbageCollector(LuaGC what, int data, int data2)
	{
		return NativeMethods.lua_gc(_luaState, (int)what, data, data2);
	}

	public LuaAlloc GetAllocFunction(ref IntPtr ud)
	{
		return NativeMethods.lua_getallocf(_luaState, ref ud).ToLuaAlloc();
	}

	public LuaType GetField(int index, string key)
	{
		return (LuaType)NativeMethods.lua_getfield(_luaState, index, key);
	}

	public LuaType GetField(LuaRegistry index, string key)
	{
		return (LuaType)NativeMethods.lua_getfield(_luaState, (int)index, key);
	}

	public LuaType GetGlobal(string name)
	{
		return (LuaType)NativeMethods.lua_getglobal(_luaState, name);
	}

	public LuaType GetInteger(int index, long i)
	{
		return (LuaType)NativeMethods.lua_geti(_luaState, index, i);
	}

	public bool GetInfo(string what, IntPtr ar)
	{
		return NativeMethods.lua_getinfo(_luaState, what, ar) != 0;
	}

	public bool GetInfo(string what, ref LuaDebug ar)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ar));
		bool flag = false;
		try
		{
			Marshal.StructureToPtr(ar, intPtr, fDeleteOld: false);
			flag = GetInfo(what, intPtr);
			ar = LuaDebug.FromIntPtr(intPtr);
			return flag;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public string GetLocal(IntPtr ar, int n)
	{
		return Marshal.PtrToStringAnsi(NativeMethods.lua_getlocal(_luaState, ar, n));
	}

	public string GetLocal(LuaDebug ar, int n)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ar));
		string empty = string.Empty;
		try
		{
			Marshal.StructureToPtr(ar, intPtr, fDeleteOld: false);
			empty = GetLocal(intPtr, n);
			ar = LuaDebug.FromIntPtr(intPtr);
			return empty;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public bool GetMetaTable(int index)
	{
		return NativeMethods.lua_getmetatable(_luaState, index) != 0;
	}

	public int GetStack(int level, IntPtr ar)
	{
		return NativeMethods.lua_getstack(_luaState, level, ar);
	}

	public int GetStack(int level, ref LuaDebug ar)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ar));
		int num = 0;
		try
		{
			Marshal.StructureToPtr(ar, intPtr, fDeleteOld: false);
			num = GetStack(level, intPtr);
			ar = LuaDebug.FromIntPtr(intPtr);
			return num;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public LuaType GetTable(int index)
	{
		return (LuaType)NativeMethods.lua_gettable(_luaState, index);
	}

	public LuaType GetTable(LuaRegistry index)
	{
		return (LuaType)NativeMethods.lua_gettable(_luaState, (int)index);
	}

	public int GetTop()
	{
		return NativeMethods.lua_gettop(_luaState);
	}

	public int GetIndexedUserValue(int index, int nth)
	{
		return NativeMethods.lua_getiuservalue(_luaState, index, nth);
	}

	public int GetUserValue(int index)
	{
		return GetIndexedUserValue(index, 1);
	}

	public string GetUpValue(int functionIndex, int n)
	{
		return Marshal.PtrToStringAnsi(NativeMethods.lua_getupvalue(_luaState, functionIndex, n));
	}

	public void Insert(int index)
	{
		NativeMethods.lua_rotate(_luaState, index, 1);
	}

	public bool IsBoolean(int index)
	{
		return Type(index) == LuaType.Boolean;
	}

	public bool IsCFunction(int index)
	{
		return NativeMethods.lua_iscfunction(_luaState, index) != 0;
	}

	public bool IsFunction(int index)
	{
		return Type(index) == LuaType.Function;
	}

	public bool IsInteger(int index)
	{
		return NativeMethods.lua_isinteger(_luaState, index) != 0;
	}

	public bool IsLightUserData(int index)
	{
		return Type(index) == LuaType.LightUserData;
	}

	public bool IsNil(int index)
	{
		return Type(index) == LuaType.Nil;
	}

	public bool IsNone(int index)
	{
		return Type(index) == LuaType.None;
	}

	public bool IsNoneOrNil(int index)
	{
		if (!IsNone(index))
		{
			return IsNil(index);
		}
		return true;
	}

	public bool IsNumber(int index)
	{
		return NativeMethods.lua_isnumber(_luaState, index) != 0;
	}

	public bool IsStringOrNumber(int index)
	{
		return NativeMethods.lua_isstring(_luaState, index) != 0;
	}

	public bool IsString(int index)
	{
		return Type(index) == LuaType.String;
	}

	public bool IsTable(int index)
	{
		return Type(index) == LuaType.Table;
	}

	public bool IsThread(int index)
	{
		return Type(index) == LuaType.Thread;
	}

	public bool IsUserData(int index)
	{
		return NativeMethods.lua_isuserdata(_luaState, index) != 0;
	}

	public void PushLength(int index)
	{
		NativeMethods.lua_len(_luaState, index);
	}

	public LuaStatus Load(LuaReader reader, IntPtr data, string chunkName, string mode)
	{
		return (LuaStatus)NativeMethods.lua_load(_luaState, reader.ToFunctionPointer(), data, chunkName, mode);
	}

	public void NewTable()
	{
		NativeMethods.lua_createtable(_luaState, 0, 0);
	}

	public Lua NewThread()
	{
		return new Lua(NativeMethods.lua_newthread(_luaState), this);
	}

	public IntPtr NewIndexedUserData(int size, int uv)
	{
		return NativeMethods.lua_newuserdatauv(_luaState, (UIntPtr)(ulong)size, uv);
	}

	public IntPtr NewUserData(int size)
	{
		return NewIndexedUserData(size, 1);
	}

	public bool Next(int index)
	{
		return NativeMethods.lua_next(_luaState, index) != 0;
	}

	public LuaStatus PCall(int arguments, int results, int errorFunctionIndex)
	{
		return (LuaStatus)NativeMethods.lua_pcallk(_luaState, arguments, results, errorFunctionIndex, IntPtr.Zero, IntPtr.Zero);
	}

	public LuaStatus PCallK(int arguments, int results, int errorFunctionIndex, int context, LuaKFunction k)
	{
		return (LuaStatus)NativeMethods.lua_pcallk(_luaState, arguments, results, errorFunctionIndex, (IntPtr)context, k.ToFunctionPointer());
	}

	public void Pop(int n)
	{
		NativeMethods.lua_settop(_luaState, -n - 1);
	}

	public void PushBoolean(bool b)
	{
		NativeMethods.lua_pushboolean(_luaState, b ? 1 : 0);
	}

	public void PushCClosure(LuaFunction function, int n)
	{
		NativeMethods.lua_pushcclosure(_luaState, function.ToFunctionPointer(), n);
	}

	public void PushCFunction(LuaFunction function)
	{
		PushCClosure(function, 0);
	}

	public void PushGlobalTable()
	{
		NativeMethods.lua_rawgeti(_luaState, -1001000, 2L);
	}

	public void PushInteger(long n)
	{
		NativeMethods.lua_pushinteger(_luaState, n);
	}

	public void PushLightUserData(IntPtr data)
	{
		NativeMethods.lua_pushlightuserdata(_luaState, data);
	}

	public void PushObject<T>(T obj)
	{
		if (obj == null)
		{
			PushNil();
			return;
		}
		GCHandle value = GCHandle.Alloc(obj);
		PushLightUserData(GCHandle.ToIntPtr(value));
	}

	public void PushBuffer(byte[] buffer)
	{
		if (buffer == null)
		{
			PushNil();
		}
		else
		{
			NativeMethods.lua_pushlstring(_luaState, buffer, (UIntPtr)(ulong)buffer.Length);
		}
	}

	public void PushString(string value)
	{
		if (value == null)
		{
			PushNil();
			return;
		}
		byte[] bytes = Encoding.GetBytes(value);
		PushBuffer(bytes);
	}

	public void PushString(string value, params object[] args)
	{
		PushString(string.Format(value, args));
	}

	public void PushNil()
	{
		NativeMethods.lua_pushnil(_luaState);
	}

	public void PushNumber(double number)
	{
		NativeMethods.lua_pushnumber(_luaState, number);
	}

	public bool PushThread()
	{
		return NativeMethods.lua_pushthread(_luaState) == 1;
	}

	public void PushCopy(int index)
	{
		NativeMethods.lua_pushvalue(_luaState, index);
	}

	public bool RawEqual(int index1, int index2)
	{
		return NativeMethods.lua_rawequal(_luaState, index1, index2) != 0;
	}

	public LuaType RawGet(int index)
	{
		return (LuaType)NativeMethods.lua_rawget(_luaState, index);
	}

	public LuaType RawGet(LuaRegistry index)
	{
		return (LuaType)NativeMethods.lua_rawget(_luaState, (int)index);
	}

	public LuaType RawGetInteger(int index, long n)
	{
		return (LuaType)NativeMethods.lua_rawgeti(_luaState, index, n);
	}

	public LuaType RawGetInteger(LuaRegistry index, long n)
	{
		return (LuaType)NativeMethods.lua_rawgeti(_luaState, (int)index, n);
	}

	public LuaType RawGetByHashCode(int index, object obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj", "obj shouldn't be null");
		}
		return (LuaType)NativeMethods.lua_rawgetp(_luaState, index, (IntPtr)obj.GetHashCode());
	}

	public int RawLen(int index)
	{
		return (int)(uint)NativeMethods.lua_rawlen(_luaState, index);
	}

	public void RawSet(int index)
	{
		NativeMethods.lua_rawset(_luaState, index);
	}

	public void RawSet(LuaRegistry index)
	{
		NativeMethods.lua_rawset(_luaState, (int)index);
	}

	public void RawSetInteger(int index, long i)
	{
		NativeMethods.lua_rawseti(_luaState, index, i);
	}

	public void RawSetInteger(LuaRegistry index, long i)
	{
		NativeMethods.lua_rawseti(_luaState, (int)index, i);
	}

	public void RawSetByHashCode(int index, object obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj", "obj shouldn't be null");
		}
		NativeMethods.lua_rawsetp(_luaState, index, (IntPtr)obj.GetHashCode());
	}

	public void Register(string name, LuaFunction function)
	{
		PushCFunction(function);
		SetGlobal(name);
	}

	public void Remove(int index)
	{
		Rotate(index, -1);
		Pop(1);
	}

	public void Replace(int index)
	{
		Copy(-1, index);
		Pop(1);
	}

	public LuaStatus Resume(Lua from, int arguments, out int results)
	{
		return (LuaStatus)NativeMethods.lua_resume(_luaState, from?._luaState ?? IntPtr.Zero, arguments, out results);
	}

	public LuaStatus Resume(Lua from, int arguments)
	{
		int results;
		return (LuaStatus)NativeMethods.lua_resume(_luaState, from?._luaState ?? IntPtr.Zero, arguments, out results);
	}

	public int ResetThread()
	{
		return NativeMethods.lua_resetthread(_luaState);
	}

	public void Rotate(int index, int n)
	{
		NativeMethods.lua_rotate(_luaState, index, n);
	}

	public void SetAllocFunction(LuaAlloc alloc, ref IntPtr ud)
	{
		NativeMethods.lua_setallocf(_luaState, alloc.ToFunctionPointer(), ud);
	}

	public void SetField(int index, string key)
	{
		NativeMethods.lua_setfield(_luaState, index, key);
	}

	public void SetHook(LuaHookFunction hookFunction, LuaHookMask mask, int count)
	{
		NativeMethods.lua_sethook(_luaState, hookFunction.ToFunctionPointer(), (int)mask, count);
	}

	public void SetGlobal(string name)
	{
		NativeMethods.lua_setglobal(_luaState, name);
	}

	public void SetInteger(int index, long n)
	{
		NativeMethods.lua_seti(_luaState, index, n);
	}

	public string SetLocal(IntPtr ar, int n)
	{
		return Marshal.PtrToStringAnsi(NativeMethods.lua_setlocal(_luaState, ar, n));
	}

	public string SetLocal(LuaDebug ar, int n)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ar));
		string empty = string.Empty;
		try
		{
			Marshal.StructureToPtr(ar, intPtr, fDeleteOld: false);
			empty = SetLocal(intPtr, n);
			ar = LuaDebug.FromIntPtr(intPtr);
			return empty;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public void SetMetaTable(int index)
	{
		NativeMethods.lua_setmetatable(_luaState, index);
	}

	public void SetTable(int index)
	{
		NativeMethods.lua_settable(_luaState, index);
	}

	public void SetTop(int newTop)
	{
		NativeMethods.lua_settop(_luaState, newTop);
	}

	public string SetUpValue(int functionIndex, int n)
	{
		return Marshal.PtrToStringAnsi(NativeMethods.lua_setupvalue(_luaState, functionIndex, n));
	}

	public void SetWarningFunction(LuaWarnFunction function, IntPtr userData)
	{
		NativeMethods.lua_setwarnf(_luaState, function.ToFunctionPointer(), userData);
	}

	public void SetIndexedUserValue(int index, int nth)
	{
		NativeMethods.lua_setiuservalue(_luaState, index, nth);
	}

	public void SetUserValue(int index)
	{
		SetIndexedUserValue(index, 1);
	}

	public bool StringToNumber(string s)
	{
		return NativeMethods.lua_stringtonumber(_luaState, s) != UIntPtr.Zero;
	}

	public bool ToBoolean(int index)
	{
		return NativeMethods.lua_toboolean(_luaState, index) != 0;
	}

	public LuaFunction ToCFunction(int index)
	{
		return NativeMethods.lua_tocfunction(_luaState, index).ToLuaFunction();
	}

	public void ToClose(int index)
	{
		NativeMethods.lua_toclose(_luaState, index);
	}

	public long ToInteger(int index)
	{
		int isNum;
		return NativeMethods.lua_tointegerx(_luaState, index, out isNum);
	}

	public long? ToIntegerX(int index)
	{
		int isNum;
		long value = NativeMethods.lua_tointegerx(_luaState, index, out isNum);
		if (isNum != 0)
		{
			return value;
		}
		return null;
	}

	public byte[] ToBuffer(int index)
	{
		return ToBuffer(index, callMetamethod: true);
	}

	public byte[] ToBuffer(int index, bool callMetamethod)
	{
		IntPtr intPtr;
		UIntPtr strLen;
		if (callMetamethod)
		{
			intPtr = NativeMethods.luaL_tolstring(_luaState, index, out strLen);
			Pop(1);
		}
		else
		{
			intPtr = NativeMethods.lua_tolstring(_luaState, index, out strLen);
		}
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		int num = (int)(uint)strLen;
		if (num == 0)
		{
			return new byte[0];
		}
		byte[] array = new byte[num];
		Marshal.Copy(intPtr, array, 0, num);
		return array;
	}

	public string ToString(int index)
	{
		return ToString(index, callMetamethod: true);
	}

	public string ToString(int index, bool callMetamethod)
	{
		byte[] array = ToBuffer(index, callMetamethod);
		if (array == null)
		{
			return null;
		}
		return Encoding.GetString(array);
	}

	public double ToNumber(int index)
	{
		int isNum;
		return NativeMethods.lua_tonumberx(_luaState, index, out isNum);
	}

	public double? ToNumberX(int index)
	{
		int isNum;
		double value = NativeMethods.lua_tonumberx(_luaState, index, out isNum);
		if (isNum != 0)
		{
			return value;
		}
		return null;
	}

	public IntPtr ToPointer(int index)
	{
		return NativeMethods.lua_topointer(_luaState, index);
	}

	public Lua ToThread(int index)
	{
		IntPtr intPtr = NativeMethods.lua_tothread(_luaState, index);
		if (intPtr == _luaState)
		{
			return this;
		}
		return FromIntPtr(intPtr);
	}

	public T ToObject<T>(int index, bool freeGCHandle = true)
	{
		if (IsNil(index) || !IsLightUserData(index))
		{
			return default(T);
		}
		IntPtr intPtr = ToUserData(index);
		if (intPtr == IntPtr.Zero)
		{
			return default(T);
		}
		GCHandle gCHandle = GCHandle.FromIntPtr(intPtr);
		if (!gCHandle.IsAllocated)
		{
			return default(T);
		}
		T result = (T)gCHandle.Target;
		if (freeGCHandle)
		{
			gCHandle.Free();
		}
		return result;
	}

	public IntPtr ToUserData(int index)
	{
		return NativeMethods.lua_touserdata(_luaState, index);
	}

	public LuaType Type(int index)
	{
		return (LuaType)NativeMethods.lua_type(_luaState, index);
	}

	public string TypeName(LuaType type)
	{
		return Marshal.PtrToStringAnsi(NativeMethods.lua_typename(_luaState, (int)type));
	}

	public long UpValueId(int functionIndex, int n)
	{
		return (long)NativeMethods.lua_upvalueid(_luaState, functionIndex, n);
	}

	public static int UpValueIndex(int i)
	{
		return -1001000 - i;
	}

	public void UpValueJoin(int functionIndex1, int n1, int functionIndex2, int n2)
	{
		NativeMethods.lua_upvaluejoin(_luaState, functionIndex1, n1, functionIndex2, n2);
	}

	public double Version()
	{
		return NativeMethods.lua_version(_luaState);
	}

	public void XMove(Lua to, int n)
	{
		if (to == null)
		{
			throw new ArgumentNullException("to", "to shouldn't be null");
		}
		NativeMethods.lua_xmove(_luaState, to._luaState, n);
	}

	public int Yield(int results)
	{
		return NativeMethods.lua_yieldk(_luaState, results, IntPtr.Zero, IntPtr.Zero);
	}

	public int YieldK(int results, int context, LuaKFunction continuation)
	{
		IntPtr k = continuation.ToFunctionPointer();
		return NativeMethods.lua_yieldk(_luaState, results, (IntPtr)context, k);
	}

	public void ArgumentCheck(bool condition, int argument, string message)
	{
		if (!condition)
		{
			ArgumentError(argument, message);
		}
	}

	public int ArgumentError(int argument, string message)
	{
		return NativeMethods.luaL_argerror(_luaState, argument, message);
	}

	public bool CallMetaMethod(int obj, string field)
	{
		return NativeMethods.luaL_callmeta(_luaState, obj, field) != 0;
	}

	public void CheckAny(int argument)
	{
		NativeMethods.luaL_checkany(_luaState, argument);
	}

	public long CheckInteger(int argument)
	{
		return NativeMethods.luaL_checkinteger(_luaState, argument);
	}

	public byte[] CheckBuffer(int argument)
	{
		UIntPtr len;
		IntPtr intPtr = NativeMethods.luaL_checklstring(_luaState, argument, out len);
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		int num = (int)(uint)len;
		if (num == 0)
		{
			return new byte[0];
		}
		byte[] array = new byte[num];
		Marshal.Copy(intPtr, array, 0, num);
		return array;
	}

	public string CheckString(int argument)
	{
		byte[] array = CheckBuffer(argument);
		if (array == null)
		{
			return null;
		}
		return Encoding.GetString(array);
	}

	public double CheckNumber(int argument)
	{
		return NativeMethods.luaL_checknumber(_luaState, argument);
	}

	public int CheckOption(int argument, string def, string[] list)
	{
		return NativeMethods.luaL_checkoption(_luaState, argument, def, list);
	}

	public void CheckStack(int newSize, string message)
	{
		NativeMethods.luaL_checkstack(_luaState, newSize, message);
	}

	public void CheckType(int argument, LuaType type)
	{
		NativeMethods.luaL_checktype(_luaState, argument, (int)type);
	}

	public T CheckObject<T>(int argument, string typeName, bool freeGCHandle = true)
	{
		if (IsNil(argument) || !IsLightUserData(argument))
		{
			return default(T);
		}
		IntPtr intPtr = CheckUserData(argument, typeName);
		if (intPtr == IntPtr.Zero)
		{
			return default(T);
		}
		GCHandle gCHandle = GCHandle.FromIntPtr(intPtr);
		if (!gCHandle.IsAllocated)
		{
			return default(T);
		}
		T result = (T)gCHandle.Target;
		if (freeGCHandle)
		{
			gCHandle.Free();
		}
		return result;
	}

	public IntPtr CheckUserData(int argument, string typeName)
	{
		return NativeMethods.luaL_checkudata(_luaState, argument, typeName);
	}

	public bool DoFile(string file)
	{
		if (LoadFile(file) == LuaStatus.OK)
		{
			return PCall(0, -1, 0) != LuaStatus.OK;
		}
		return true;
	}

	public bool DoString(string file)
	{
		if (LoadString(file) == LuaStatus.OK)
		{
			return PCall(0, -1, 0) != LuaStatus.OK;
		}
		return true;
	}

	public int Error(string value, params object[] v)
	{
		string message = string.Format(value, v);
		return NativeMethods.luaL_error(_luaState, message);
	}

	public int ExecResult(int stat)
	{
		return NativeMethods.luaL_execresult(_luaState, stat);
	}

	public int FileResult(int stat, string fileName)
	{
		return NativeMethods.luaL_fileresult(_luaState, stat, fileName);
	}

	public LuaType GetMetaField(int obj, string field)
	{
		return (LuaType)NativeMethods.luaL_getmetafield(_luaState, obj, field);
	}

	public LuaType GetMetaTable(string tableName)
	{
		return GetField(LuaRegistry.Index, tableName);
	}

	public bool GetSubTable(int index, string name)
	{
		return NativeMethods.luaL_getsubtable(_luaState, index, name) != 0;
	}

	public long Length(int index)
	{
		return NativeMethods.luaL_len(_luaState, index);
	}

	public LuaStatus LoadBuffer(byte[] buffer, string name, string mode)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer", "buffer shouldn't be null");
		}
		return (LuaStatus)NativeMethods.luaL_loadbufferx(_luaState, buffer, (UIntPtr)(ulong)buffer.Length, name, mode);
	}

	public LuaStatus LoadBuffer(byte[] buffer, string name)
	{
		return LoadBuffer(buffer, name, null);
	}

	public LuaStatus LoadBuffer(byte[] buffer)
	{
		return LoadBuffer(buffer, null, null);
	}

	public LuaStatus LoadString(string chunk, string name)
	{
		byte[] bytes = Encoding.GetBytes(chunk);
		return LoadBuffer(bytes, name);
	}

	public LuaStatus LoadString(string chunk)
	{
		return LoadString(chunk, null);
	}

	public LuaStatus LoadFile(string file, string mode)
	{
		return (LuaStatus)NativeMethods.luaL_loadfilex(_luaState, file, mode);
	}

	public LuaStatus LoadFile(string file)
	{
		return LoadFile(file, null);
	}

	public void NewLib(LuaRegister[] library)
	{
		NewLibTable(library);
		SetFuncs(library, 0);
	}

	public void NewLibTable(LuaRegister[] library)
	{
		if (library == null)
		{
			throw new ArgumentNullException("library", "library shouldn't be null");
		}
		CreateTable(0, library.Length);
	}

	public bool NewMetaTable(string name)
	{
		return NativeMethods.luaL_newmetatable(_luaState, name) != 0;
	}

	public void OpenLibs()
	{
		NativeMethods.luaL_openlibs(_luaState);
	}

	public long OptInteger(int argument, long d)
	{
		return NativeMethods.luaL_optinteger(_luaState, argument, d);
	}

	public byte[] OptBuffer(int index, byte[] def)
	{
		if (IsNoneOrNil(index))
		{
			return def;
		}
		return CheckBuffer(index);
	}

	public string OptString(int index, string def)
	{
		if (IsNoneOrNil(index))
		{
			return def;
		}
		return CheckString(index);
	}

	public double OptNumber(int index, double def)
	{
		return NativeMethods.luaL_optnumber(_luaState, index, def);
	}

	public int Ref(LuaRegistry tableIndex)
	{
		return NativeMethods.luaL_ref(_luaState, (int)tableIndex);
	}

	public void RequireF(string moduleName, LuaFunction openFunction, bool global)
	{
		NativeMethods.luaL_requiref(_luaState, moduleName, openFunction.ToFunctionPointer(), global ? 1 : 0);
	}

	public void SetFuncs(LuaRegister[] library, int numberUpValues)
	{
		NativeMethods.luaL_setfuncs(_luaState, library, numberUpValues);
	}

	public void SetMetaTable(string name)
	{
		NativeMethods.luaL_setmetatable(_luaState, name);
	}

	public T TestObject<T>(int argument, string typeName, bool freeGCHandle = true)
	{
		if (IsNil(argument) || !IsLightUserData(argument))
		{
			return default(T);
		}
		IntPtr intPtr = TestUserData(argument, typeName);
		if (intPtr == IntPtr.Zero)
		{
			return default(T);
		}
		GCHandle gCHandle = GCHandle.FromIntPtr(intPtr);
		if (!gCHandle.IsAllocated)
		{
			return default(T);
		}
		T result = (T)gCHandle.Target;
		if (freeGCHandle)
		{
			gCHandle.Free();
		}
		return result;
	}

	public IntPtr TestUserData(int argument, string typeName)
	{
		return NativeMethods.luaL_testudata(_luaState, argument, typeName);
	}

	public void Traceback(Lua state, int level = 0)
	{
		Traceback(state, null, level);
	}

	public void Traceback(Lua state, string message, int level)
	{
		if (state == null)
		{
			throw new ArgumentNullException("state", "state shouldn't be null");
		}
		NativeMethods.luaL_traceback(_luaState, state._luaState, message, level);
	}

	public string TypeName(int index)
	{
		LuaType type = Type(index);
		return TypeName(type);
	}

	public void Unref(LuaRegistry tableIndex, int reference)
	{
		NativeMethods.luaL_unref(_luaState, (int)tableIndex, reference);
	}

	public void Warning(string message, bool toContinue)
	{
		NativeMethods.lua_warning(_luaState, message, toContinue ? 1 : 0);
	}

	public void Where(int level)
	{
		NativeMethods.luaL_where(_luaState, level);
	}
}
