using System;
using System.Collections.Concurrent;

namespace Cs.Engine.Core;

public sealed class SimpleObjectPool<T>
{
	private readonly ConcurrentQueue<T> objects = new ConcurrentQueue<T>();

	private readonly Func<T> factoryMethod;

	public SimpleObjectPool(Func<T> factoryMethod)
	{
		this.factoryMethod = factoryMethod;
	}

	public T CreateInstance()
	{
		if (objects.TryDequeue(out var result))
		{
			return result;
		}
		return factoryMethod();
	}

	public void ToRecycleBin(T item)
	{
		objects.Enqueue(item);
	}
}
