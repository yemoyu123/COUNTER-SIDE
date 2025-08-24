using System.Collections.Generic;

namespace NKM;

public class PriorityQueue<T>
{
	private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

	public int Count => elements.Count;

	public void Enqueue(T item, float priority)
	{
		elements.Add(new KeyValuePair<T, float>(item, priority));
	}

	public T Dequeue()
	{
		int index = 0;
		for (int i = 0; i < elements.Count; i++)
		{
			if (elements[i].Value < elements[index].Value)
			{
				index = i;
			}
		}
		T key = elements[index].Key;
		elements.RemoveAt(index);
		return key;
	}
}
