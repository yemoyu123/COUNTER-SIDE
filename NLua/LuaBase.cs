using System;

namespace NLua;

public abstract class LuaBase : IDisposable
{
	private bool _disposed;

	protected readonly int _Reference;

	private Lua _lua;

	protected bool TryGet(out Lua lua)
	{
		if (_lua.State == null)
		{
			lua = null;
			return false;
		}
		lua = _lua;
		return true;
	}

	protected LuaBase(int reference, Lua lua)
	{
		_lua = lua;
		_Reference = reference;
	}

	~LuaBase()
	{
		Dispose(disposeManagedResources: false);
	}

	public void Dispose()
	{
		Dispose(disposeManagedResources: true);
		GC.SuppressFinalize(this);
	}

	private void DisposeLuaReference(bool finalized)
	{
		if (_lua != null && TryGet(out var lua))
		{
			lua.DisposeInternal(_Reference, finalized);
		}
	}

	public virtual void Dispose(bool disposeManagedResources)
	{
		if (!_disposed)
		{
			bool finalized = !disposeManagedResources;
			if (_Reference != 0)
			{
				DisposeLuaReference(finalized);
			}
			_lua = null;
			_disposed = true;
		}
	}

	public override bool Equals(object o)
	{
		if (!(o is LuaBase luaBase))
		{
			return false;
		}
		if (!TryGet(out var lua))
		{
			return false;
		}
		return lua.CompareRef(luaBase._Reference, _Reference);
	}

	public override int GetHashCode()
	{
		return _Reference;
	}
}
