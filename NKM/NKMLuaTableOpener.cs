using System;

namespace NKM;

public sealed class NKMLuaTableOpener : IDisposable
{
	private readonly NKMLua lua;

	public NKMLuaTableOpener(NKMLua lua)
	{
		this.lua = lua;
	}

	public void Dispose()
	{
		lua.CloseTable();
	}
}
