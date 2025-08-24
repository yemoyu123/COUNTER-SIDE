using System;

namespace NLua.Exceptions;

[Serializable]
public class LuaScriptException : LuaException
{
	private readonly string _source;

	public bool IsNetException { get; }

	public override string Source => _source;

	public LuaScriptException(string message, string source)
		: base(message)
	{
		_source = source;
	}

	public LuaScriptException(Exception innerException, string source)
		: base("A .NET exception occured in user-code", innerException)
	{
		_source = source;
		IsNetException = true;
	}

	public override string ToString()
	{
		return GetType().FullName + ": " + _source + Message;
	}
}
