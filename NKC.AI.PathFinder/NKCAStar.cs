using System.Collections.Generic;
using UnityEngine;

namespace NKC.AI.PathFinder;

public class NKCAStar
{
	private class PriorityQueue<T>
	{
		private LinkedList<KeyValuePair<T, float>> elements = new LinkedList<KeyValuePair<T, float>>();

		public int Count => elements.Count;

		public void Enqueue(T item, float priority)
		{
			KeyValuePair<T, float> value = new KeyValuePair<T, float>(item, priority);
			for (LinkedListNode<KeyValuePair<T, float>> linkedListNode = elements.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
			{
				if (priority > linkedListNode.Value.Value)
				{
					elements.AddAfter(linkedListNode, value);
					return;
				}
			}
			elements.AddFirst(value);
		}

		public T Dequeue()
		{
			KeyValuePair<T, float> value = elements.First.Value;
			elements.RemoveFirst();
			return value.Key;
		}
	}

	private Dictionary<(int, int), (int, int)> cameFrom = new Dictionary<(int, int), (int, int)>();

	private Dictionary<(int, int), float> costSoFar = new Dictionary<(int, int), float>();

	private (int, int) start;

	private (int, int) goal;

	private (int, int) size;

	private long[,] map;

	public NKCAStar(long[,] _map, (int, int) startPos, (int, int) endPos)
	{
		map = _map;
		if (map[startPos.Item1, startPos.Item2] != 0L || map[endPos.Item1, endPos.Item2] != 0L)
		{
			return;
		}
		size = (map.GetLength(0), map.GetLength(1));
		start = startPos;
		goal = endPos;
		PriorityQueue<(int, int)> priorityQueue = new PriorityQueue<(int, int)>();
		priorityQueue.Enqueue(startPos, 0f);
		cameFrom.Add(startPos, startPos);
		costSoFar.Add(startPos, 0f);
		while (priorityQueue.Count > 0)
		{
			(int, int) tuple = priorityQueue.Dequeue();
			(int, int) tuple2 = tuple;
			(int, int) tuple3 = endPos;
			if (tuple2.Item1 == tuple3.Item1 && tuple2.Item2 == tuple3.Item2)
			{
				break;
			}
			foreach (var neighbor in GetNeighbors(tuple))
			{
				if (map[neighbor.Item1, neighbor.Item2] == 0L)
				{
					float num = costSoFar[tuple] + CalcSimpleCost(tuple, neighbor);
					if (!costSoFar.ContainsKey(neighbor) || num < costSoFar[neighbor])
					{
						costSoFar[neighbor] = num;
						cameFrom[neighbor] = tuple;
						float priority = num + Heuristic(neighbor, endPos);
						priorityQueue.Enqueue(neighbor, priority);
					}
				}
			}
		}
	}

	public List<(int, int)> GetPath(bool smoothen)
	{
		List<(int, int)> list = new List<(int, int)>();
		(int, int) tuple = goal;
		while (!tuple.Equals(start))
		{
			if (!cameFrom.ContainsKey(tuple))
			{
				return null;
			}
			list.Add(tuple);
			tuple = cameFrom[tuple];
		}
		if (!smoothen)
		{
			list.Reverse();
			return list;
		}
		list.Add(start);
		list.Reverse();
		return SmoothenPath(list);
	}

	private List<(int, int)> SmoothenPath(List<(int, int)> path)
	{
		if (map == null)
		{
			return path;
		}
		if (path.Count <= 2)
		{
			return path;
		}
		LinkedList<(int, int)> linkedList = new LinkedList<(int, int)>(path);
		LinkedListNode<(int, int)> linkedListNode = linkedList.First;
		LinkedListNode<(int, int)> next = linkedListNode.Next;
		while (next.Next != null)
		{
			if (Walkable(linkedListNode.Value, next.Next.Value))
			{
				LinkedListNode<(int, int)> node = next;
				next = next.Next;
				linkedList.Remove(node);
			}
			else
			{
				linkedListNode = next;
				next = next.Next;
			}
		}
		linkedList.RemoveFirst();
		return new List<(int, int)>(linkedList);
	}

	private bool Walkable((int, int) start, (int, int) end)
	{
		(int, int) tuple = start;
		(int, int) tuple2 = end;
		if (tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2)
		{
			return true;
		}
		if (start.Item1 == end.Item1)
		{
			int num = Mathf.Max(start.Item2, end.Item2);
			for (int i = Mathf.Min(start.Item2, end.Item2); i <= num; i++)
			{
				if (map[start.Item1, i] != 0L)
				{
					return false;
				}
			}
			return true;
		}
		if (start.Item2 == end.Item2)
		{
			int num2 = Mathf.Max(start.Item1, end.Item1);
			for (int j = Mathf.Min(start.Item1, end.Item1); j <= num2; j++)
			{
				if (map[j, start.Item2] != 0L)
				{
					return false;
				}
			}
			return true;
		}
		float num3 = (float)(end.Item2 - start.Item2) / (float)(end.Item1 - start.Item1);
		float num4 = (float)start.Item2 - num3 * (float)start.Item1;
		int num5 = Mathf.Min(start.Item1, end.Item1);
		int num6 = Mathf.Max(start.Item1, end.Item1);
		for (int k = num5; k <= num6; k++)
		{
			float num7 = ((k == num5) ? ((float)k) : ((float)k - 0.5f));
			float num8 = ((k == num6) ? ((float)k) : ((float)k + 0.5f));
			float num9 = num3 * num7 + num4;
			float num10 = num3 * num8 + num4;
			int a = Mathf.FloorToInt(num9 + 0.5f);
			int b = Mathf.FloorToInt(num10 + 0.5f);
			int num11 = Mathf.Min(a, b);
			int num12 = Mathf.Max(a, b);
			for (int l = num11; l <= num12; l++)
			{
				if (map[k, l] != 0L)
				{
					return false;
				}
			}
		}
		return true;
	}

	private IEnumerable<(int, int)> GetNeighbors((int, int) position)
	{
		if (IsInBound(position.Item1, position.Item2 + 1))
		{
			yield return (position.Item1, position.Item2 + 1);
		}
		if (IsInBound(position.Item1 + 1, position.Item2))
		{
			yield return (position.Item1 + 1, position.Item2);
		}
		if (IsInBound(position.Item1, position.Item2 - 1))
		{
			yield return (position.Item1, position.Item2 - 1);
		}
		if (IsInBound(position.Item1 - 1, position.Item2))
		{
			yield return (position.Item1 - 1, position.Item2);
		}
		if (IsInBound(position.Item1 - 1, position.Item2 + 1))
		{
			yield return (position.Item1 - 1, position.Item2 + 1);
		}
		if (IsInBound(position.Item1 + 1, position.Item2 + 1))
		{
			yield return (position.Item1 + 1, position.Item2 + 1);
		}
		if (IsInBound(position.Item1 + 1, position.Item2 - 1))
		{
			yield return (position.Item1 + 1, position.Item2 - 1);
		}
		if (IsInBound(position.Item1 - 1, position.Item2 - 1))
		{
			yield return (position.Item1 - 1, position.Item2 - 1);
		}
	}

	private float Heuristic((int, int) pos1, (int, int) pos2)
	{
		return CalcSimpleCost(pos1, pos2);
	}

	private float CalcSimpleCost((int, int) pos1, (int, int) pos2)
	{
		int num = Mathf.Abs(pos1.Item1 - pos2.Item1);
		int num2 = Mathf.Abs(pos1.Item2 - pos2.Item2);
		if (num > num2)
		{
			return (float)num + (float)num2 * 0.4f;
		}
		return (float)num2 + (float)num * 0.4f;
	}

	private bool IsInBound(int x, int y)
	{
		if (x < 0)
		{
			return false;
		}
		if (y < 0)
		{
			return false;
		}
		if (x >= size.Item1)
		{
			return false;
		}
		if (y >= size.Item2)
		{
			return false;
		}
		return true;
	}
}
