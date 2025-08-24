using System;
using System.Collections;
using System.Collections.Generic;
using Cs.Logging;
using UnityEngine;

namespace NKC;

public class Coroutine<T>
{
	private T returnVal;

	private Exception e;

	public Coroutine coroutine;

	public T Value
	{
		get
		{
			if (e != null)
			{
				throw e;
			}
			return returnVal;
		}
	}

	public IEnumerator InternalRoutine(IEnumerator coroutine)
	{
		Stack<IEnumerator> stack = new Stack<IEnumerator>();
		stack.Push(coroutine);
		while (stack.Count != 0)
		{
			IEnumerator enumerator = stack.Peek();
			object current;
			try
			{
				if (!enumerator.MoveNext())
				{
					stack.Pop();
					continue;
				}
				current = enumerator.Current;
			}
			catch (Exception ex)
			{
				Exception ex2 = new Exception(ex.Message, ex);
				e = ex2;
				Log.Error($"Coroutine Exception : {ex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCUtil.cs", 4740);
				break;
			}
			if (current is IEnumerator item)
			{
				stack.Push(item);
				continue;
			}
			if (current != null && current.GetType() == typeof(T))
			{
				returnVal = (T)current;
				break;
			}
			yield return current;
		}
	}
}
