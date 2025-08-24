using System;
using System.Runtime.InteropServices;
using System.Security;

namespace KeraLua;

[SuppressUnmanagedCodeSecurity]
internal static class NativeMethods
{
	private const string LuaLibraryName = "lua54";

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_absindex(IntPtr luaState, int idx);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_arith(IntPtr luaState, int op);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_atpanic(IntPtr luaState, IntPtr panicf);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_callk(IntPtr luaState, int nargs, int nresults, IntPtr ctx, IntPtr k);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_checkstack(IntPtr luaState, int extra);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_close(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_compare(IntPtr luaState, int index1, int index2, int op);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_concat(IntPtr luaState, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_copy(IntPtr luaState, int fromIndex, int toIndex);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_createtable(IntPtr luaState, int elements, int records);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_dump(IntPtr luaState, IntPtr writer, IntPtr data, int strip);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_error(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_gc(IntPtr luaState, int what, int data);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_gc(IntPtr luaState, int what, int data, int data2);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_getallocf(IntPtr luaState, ref IntPtr ud);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int lua_getfield(IntPtr luaState, int index, string k);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int lua_getglobal(IntPtr luaState, string name);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_gethook(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_gethookcount(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_gethookmask(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_geti(IntPtr luaState, int index, long i);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int lua_getinfo(IntPtr luaState, string what, IntPtr ar);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_getiuservalue(IntPtr luaState, int idx, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern IntPtr lua_getlocal(IntPtr luaState, IntPtr ar, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_getmetatable(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_getstack(IntPtr luaState, int level, IntPtr n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_gettable(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_gettop(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern IntPtr lua_getupvalue(IntPtr luaState, int funcIndex, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_iscfunction(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_isinteger(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_isnumber(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_isstring(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_isuserdata(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_isyieldable(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_len(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int lua_load(IntPtr luaState, IntPtr reader, IntPtr data, string chunkName, string mode);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_newstate(IntPtr allocFunction, IntPtr ud);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_newthread(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_newuserdatauv(IntPtr luaState, UIntPtr size, int nuvalue);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_next(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_pcallk(IntPtr luaState, int nargs, int nresults, int errorfunc, IntPtr ctx, IntPtr k);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_pushboolean(IntPtr luaState, int value);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_pushcclosure(IntPtr luaState, IntPtr f, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_pushinteger(IntPtr luaState, long n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_pushlightuserdata(IntPtr luaState, IntPtr udata);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_pushlstring(IntPtr luaState, byte[] s, UIntPtr len);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_pushnil(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_pushnumber(IntPtr luaState, double number);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_pushthread(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_pushvalue(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_rawequal(IntPtr luaState, int index1, int index2);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_rawget(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_rawgeti(IntPtr luaState, int index, long n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_rawgetp(IntPtr luaState, int index, IntPtr p);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern UIntPtr lua_rawlen(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_rawset(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_rawseti(IntPtr luaState, int index, long i);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_rawsetp(IntPtr luaState, int index, IntPtr p);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_resetthread(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_resume(IntPtr luaState, IntPtr from, int nargs, out int results);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_rotate(IntPtr luaState, int index, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_setallocf(IntPtr luaState, IntPtr f, IntPtr ud);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern void lua_setfield(IntPtr luaState, int index, string key);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern void lua_setglobal(IntPtr luaState, string key);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_sethook(IntPtr luaState, IntPtr f, int mask, int count);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_seti(IntPtr luaState, int index, long n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_setiuservalue(IntPtr luaState, int index, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern IntPtr lua_setlocal(IntPtr luaState, IntPtr ar, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_setmetatable(IntPtr luaState, int objIndex);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_settable(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_settop(IntPtr luaState, int newTop);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_setupvalue(IntPtr luaState, int funcIndex, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_setwarnf(IntPtr luaState, IntPtr warningFunctionPtr, IntPtr ud);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_status(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern UIntPtr lua_stringtonumber(IntPtr luaState, string s);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_toboolean(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_tocfunction(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_toclose(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern long lua_tointegerx(IntPtr luaState, int index, out int isNum);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_tolstring(IntPtr luaState, int index, out UIntPtr strLen);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern double lua_tonumberx(IntPtr luaState, int index, out int isNum);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_topointer(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_tothread(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_touserdata(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_type(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern IntPtr lua_typename(IntPtr luaState, int type);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr lua_upvalueid(IntPtr luaState, int funcIndex, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_upvaluejoin(IntPtr luaState, int funcIndex1, int n1, int funcIndex2, int n2);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern double lua_version(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern void lua_warning(IntPtr luaState, string msg, int tocont);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void lua_xmove(IntPtr from, IntPtr to, int n);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int lua_yieldk(IntPtr luaState, int nresults, IntPtr ctx, IntPtr k);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_argerror(IntPtr luaState, int arg, string message);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_callmeta(IntPtr luaState, int obj, string e);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void luaL_checkany(IntPtr luaState, int arg);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern long luaL_checkinteger(IntPtr luaState, int arg);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr luaL_checklstring(IntPtr luaState, int arg, out UIntPtr len);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern double luaL_checknumber(IntPtr luaState, int arg);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_checkoption(IntPtr luaState, int arg, string def, string[] list);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern void luaL_checkstack(IntPtr luaState, int sz, string message);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void luaL_checktype(IntPtr luaState, int arg, int type);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern IntPtr luaL_checkudata(IntPtr luaState, int arg, string tName);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern void luaL_checkversion_(IntPtr luaState, double ver, UIntPtr sz);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_error(IntPtr luaState, string message);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int luaL_execresult(IntPtr luaState, int stat);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_fileresult(IntPtr luaState, int stat, string fileName);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_getmetafield(IntPtr luaState, int obj, string e);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_getsubtable(IntPtr luaState, int index, string name);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern long luaL_len(IntPtr luaState, int index);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_loadbufferx(IntPtr luaState, byte[] buff, UIntPtr sz, string name, string mode);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_loadfilex(IntPtr luaState, string name, string mode);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_newmetatable(IntPtr luaState, string name);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr luaL_newstate();

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void luaL_openlibs(IntPtr luaState);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern long luaL_optinteger(IntPtr luaState, int arg, long d);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern double luaL_optnumber(IntPtr luaState, int arg, double d);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int luaL_ref(IntPtr luaState, int registryIndex);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern void luaL_requiref(IntPtr luaState, string moduleName, IntPtr openFunction, int global);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void luaL_setfuncs(IntPtr luaState, [In] LuaRegister[] luaReg, int numUp);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern void luaL_setmetatable(IntPtr luaState, string tName);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern IntPtr luaL_testudata(IntPtr luaState, int arg, string tName);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern IntPtr luaL_tolstring(IntPtr luaState, int index, out UIntPtr len);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern IntPtr luaL_traceback(IntPtr luaState, IntPtr luaState2, string message, int level);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	internal static extern int luaL_typeerror(IntPtr luaState, int arg, string typeName);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void luaL_unref(IntPtr luaState, int registryIndex, int reference);

	[DllImport("lua54", CallingConvention = CallingConvention.Cdecl)]
	internal static extern void luaL_where(IntPtr luaState, int level);
}
