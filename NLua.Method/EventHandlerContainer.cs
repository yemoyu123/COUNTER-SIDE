using System;
using System.Collections.Generic;

namespace NLua.Method;

internal class EventHandlerContainer : IDisposable
{
	private readonly Dictionary<Delegate, RegisterEventHandler> _dict = new Dictionary<Delegate, RegisterEventHandler>();

	public void Add(Delegate handler, RegisterEventHandler eventInfo)
	{
		_dict.Add(handler, eventInfo);
	}

	public void Remove(Delegate handler)
	{
		_dict.Remove(handler);
	}

	public void Dispose()
	{
		foreach (KeyValuePair<Delegate, RegisterEventHandler> item in _dict)
		{
			item.Value.RemovePending(item.Key);
		}
		_dict.Clear();
	}
}
