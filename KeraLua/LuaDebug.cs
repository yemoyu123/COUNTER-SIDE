using System;
using System.Runtime.InteropServices;
using System.Text;

namespace KeraLua;

public struct LuaDebug
{
	[MarshalAs(UnmanagedType.I4)]
	public LuaHookEvent Event;

	private IntPtr name;

	private IntPtr nameWhat;

	private IntPtr what;

	private IntPtr source;

	private IntPtr sourceLen;

	public int CurrentLine;

	public int LineDefined;

	public int LastLineDefined;

	public byte NumberUpValues;

	public byte NumberParameters;

	[MarshalAs(UnmanagedType.I1)]
	public bool IsVarArg;

	[MarshalAs(UnmanagedType.I1)]
	public bool IsTailCall;

	public ushort IndexFirstValue;

	public ushort NumberTransferredValues;

	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
	private byte[] shortSource;

	private IntPtr i_ci;

	public string Name => Marshal.PtrToStringAnsi(name);

	public string NameWhat => Marshal.PtrToStringAnsi(what);

	public string What => Marshal.PtrToStringAnsi(what);

	public string Source => Marshal.PtrToStringAnsi(source, SourceLength);

	public int SourceLength => sourceLen.ToInt32();

	public string ShortSource
	{
		get
		{
			if (shortSource[0] == 0)
			{
				return string.Empty;
			}
			int i;
			for (i = 0; i < shortSource.Length && shortSource[i] != 0; i++)
			{
			}
			return Encoding.ASCII.GetString(shortSource, 0, i);
		}
	}

	public static LuaDebug FromIntPtr(IntPtr ar)
	{
		return Marshal.PtrToStructure<LuaDebug>(ar);
	}
}
