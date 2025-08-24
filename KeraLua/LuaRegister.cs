using System.Runtime.InteropServices;

namespace KeraLua;

public struct LuaRegister
{
	public string name;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	public LuaFunction function;
}
