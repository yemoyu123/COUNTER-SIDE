using System;
using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCTopologicalSort<T>
{
	private class Node
	{
		public readonly int Index;

		public readonly T Target;

		private readonly HashSet<int> m_hsDependancy;

		public IEnumerable<int> GetDependencies => m_hsDependancy;

		public int DependancyCount => m_hsDependancy.Count;

		public Node(int index, T target, HashSet<int> dependancy)
		{
			Index = index;
			Target = target;
			m_hsDependancy = dependancy;
		}

		public bool IsDependantTo(int index)
		{
			return m_hsDependancy.Contains(index);
		}

		public void RemoveDependancy(int index)
		{
			m_hsDependancy.Remove(index);
		}
	}

	private class Graph
	{
		public Dictionary<int, Node> m_dicNode = new Dictionary<int, Node>();

		private List<int> m_lstSorted = new List<int>();

		private List<int> m_lstUnSorted;

		public List<int> SortedList => m_lstSorted;

		public List<int> UnsortedList => m_lstUnSorted;

		public void AddNode(int index, Node node)
		{
			m_dicNode.Add(index, node);
		}

		public void Sort()
		{
			m_lstSorted.Clear();
			TopologicalSort();
		}

		public Node GetNode(int index)
		{
			return m_dicNode[index];
		}

		private void TopologicalSort()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (Node value in m_dicNode.Values)
			{
				foreach (int getDependency in value.GetDependencies)
				{
					if (dictionary.ContainsKey(getDependency))
					{
						dictionary[getDependency]++;
					}
					else
					{
						dictionary.Add(getDependency, 1);
					}
				}
			}
			Queue<int> queue = new Queue<int>();
			foreach (KeyValuePair<int, Node> item in m_dicNode)
			{
				if (!dictionary.ContainsKey(item.Key) || dictionary[item.Key] == 0)
				{
					queue.Enqueue(item.Key);
				}
			}
			HashSet<int> hashSet = new HashSet<int>();
			while (queue.Count > 0)
			{
				int num = queue.Dequeue();
				if (hashSet.Contains(num))
				{
					Debug.LogError("logic error?");
					continue;
				}
				m_lstSorted.Add(num);
				hashSet.Add(num);
				foreach (int getDependency2 in GetNode(num).GetDependencies)
				{
					dictionary[getDependency2]--;
					if (dictionary[getDependency2] == 0)
					{
						queue.Enqueue(getDependency2);
					}
				}
			}
			if (m_lstSorted.Count == m_dicNode.Count)
			{
				return;
			}
			Debug.LogError("Top sort impossible case!. doing fallback");
			m_lstUnSorted = new List<int>();
			foreach (KeyValuePair<int, Node> item2 in m_dicNode)
			{
				if (!m_lstSorted.Contains(item2.Key))
				{
					m_lstUnSorted.Add(item2.Key);
				}
			}
		}
	}

	public delegate(bool, int) RelationFunction(T a, T b);

	private struct Relation
	{
		public bool hasDependancy;

		public int compairison;

		public Relation(bool _dependancy, int _compairison)
		{
			hasDependancy = _dependancy;
			compairison = _compairison;
		}

		public Relation((bool, int) relation)
		{
			(hasDependancy, compairison) = relation;
		}

		public override string ToString()
		{
			return $"{hasDependancy}, {compairison}";
		}
	}

	private RelationFunction dRelationFunction;

	private bool m_bHasCycle;

	private Dictionary<(int, int), Relation> m_dicRelation = new Dictionary<(int, int), Relation>();

	private List<T> m_lstTarget;

	public T GetObject(int index)
	{
		return m_lstTarget[index];
	}

	private Relation GetRelation(int indexA, int indexB)
	{
		if (m_dicRelation.TryGetValue((indexA, indexB), out var value))
		{
			return value;
		}
		Relation relation = new Relation(dRelationFunction(GetObject(indexA), GetObject(indexB)));
		Relation value2 = new Relation(relation.hasDependancy, -1 * relation.compairison);
		m_dicRelation[(indexA, indexB)] = relation;
		m_dicRelation[(indexB, indexA)] = value2;
		return relation;
	}

	private bool IsADefendsOnB(int indexA, int indexB)
	{
		Relation relation = GetRelation(indexA, indexB);
		if (relation.hasDependancy)
		{
			return relation.compairison > 0;
		}
		return false;
	}

	public NKCTopologicalSort(RelationFunction relationFunc)
	{
		dRelationFunction = relationFunc;
	}

	public List<T> DoSort(List<T> lstTarget)
	{
		m_lstTarget = lstTarget;
		try
		{
			List<Graph> lstSortedGraph = MakeGraphGropus(lstTarget);
			if (m_bHasCycle)
			{
				return null;
			}
			List<int> list = SortAndMerge(lstSortedGraph);
			List<T> list2 = new List<T>();
			foreach (int item in list)
			{
				list2.Add(GetObject(item));
			}
			list2.Reverse();
			return list2;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			return lstTarget;
		}
	}

	private List<Graph> MakeGraphGropus(List<T> lstTarget)
	{
		List<HashSet<int>> list = new List<HashSet<int>>();
		Dictionary<int, int> dicGroupID = new Dictionary<int, int>();
		int currentGroupID = 0;
		HashSet<int> hsCurrentGroup;
		for (int i = 0; i < lstTarget.Count; i++)
		{
			if (!dicGroupID.ContainsKey(i))
			{
				hsCurrentGroup = new HashSet<int>();
				list.Add(hsCurrentGroup);
				dicGroupID[i] = currentGroupID;
				hsCurrentGroup.Add(i);
				GroupRelation(i, currentGroupID);
				currentGroupID++;
			}
		}
		List<Graph> list2 = new List<Graph>();
		foreach (HashSet<int> item2 in list)
		{
			Graph item = BuildPrunedGraph(item2);
			if (m_bHasCycle)
			{
				return list2;
			}
			list2.Add(item);
		}
		return list2;
		void GroupRelation(int targetIndex, int groupID)
		{
			for (int j = 0; j < lstTarget.Count; j++)
			{
				if (!dicGroupID.ContainsKey(j) && j != targetIndex && GetRelation(targetIndex, j).hasDependancy)
				{
					dicGroupID[j] = currentGroupID;
					hsCurrentGroup.Add(j);
					GroupRelation(j, currentGroupID);
				}
			}
		}
	}

	private Graph BuildPrunedGraph(IEnumerable<int> lstIndices)
	{
		Graph graph = new Graph();
		foreach (int lstIndex in lstIndices)
		{
			Node node = MakePrunedNode(lstIndex, lstIndices);
			graph.AddNode(lstIndex, node);
		}
		HashSet<int> hsVisited = new HashSet<int>();
		Stack<int> stkRoute = new Stack<int>();
		foreach (KeyValuePair<int, Node> item in graph.m_dicNode)
		{
			if (HasCycle(item.Key, graph, ref hsVisited, ref stkRoute))
			{
				m_bHasCycle = true;
				return graph;
			}
		}
		return graph;
	}

	private bool HasCycle(int index, Graph graph, ref HashSet<int> hsVisited, ref Stack<int> stkRoute)
	{
		if (!hsVisited.Contains(index))
		{
			hsVisited.Add(index);
			stkRoute.Push(index);
			foreach (int getDependency in graph.GetNode(index).GetDependencies)
			{
				if (!hsVisited.Contains(getDependency) && HasCycle(getDependency, graph, ref hsVisited, ref stkRoute))
				{
					return true;
				}
				if (stkRoute.Contains(getDependency))
				{
					Debug.LogWarning("Cycle Detected");
					return true;
				}
			}
			if (stkRoute.Pop() != index)
			{
				Debug.LogError("Logic Error");
			}
		}
		return false;
	}

	private void BreakCycleDFS(int index, Graph graph, ref HashSet<int> hsVisited, ref Stack<int> stkRoute, ref List<(int, int)> lstToRemove)
	{
		if (hsVisited.Contains(index))
		{
			return;
		}
		hsVisited.Add(index);
		stkRoute.Push(index);
		foreach (int getDependency in graph.GetNode(index).GetDependencies)
		{
			if (!hsVisited.Contains(getDependency))
			{
				BreakCycleDFS(getDependency, graph, ref hsVisited, ref stkRoute, ref lstToRemove);
			}
			else
			{
				if (!stkRoute.Contains(getDependency))
				{
					continue;
				}
				Debug.LogWarning("Cycle Detected!");
				List<int> list = new List<int>(stkRoute.ToArray());
				list.Reverse();
				int count = list.IndexOf(getDependency);
				list.RemoveRange(0, count);
				int num = list[0];
				foreach (int item in list)
				{
					if (item < num)
					{
						num = item;
					}
				}
				int num2 = list.IndexOf(num);
				int index2 = (num2 + 1) % list.Count;
				lstToRemove.Add((list[num2], list[index2]));
			}
		}
		if (stkRoute.Pop() != index)
		{
			Debug.LogError("Logic Error");
		}
	}

	private Node MakePrunedNode(int index, IEnumerable<int> lstIndices)
	{
		HashSet<int> hashSet = new HashSet<int>();
		foreach (int lstIndex in lstIndices)
		{
			if (index == lstIndex || !IsADefendsOnB(index, lstIndex))
			{
				continue;
			}
			bool flag = false;
			foreach (int lstIndex2 in lstIndices)
			{
				if (lstIndex2 != index && lstIndex2 != lstIndex && IsADefendsOnB(index, lstIndex2) && IsADefendsOnB(lstIndex2, lstIndex))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				hashSet.Add(lstIndex);
			}
		}
		return new Node(index, GetObject(index), hashSet);
	}

	private List<int> SortAndMerge(List<Graph> lstSortedGraph)
	{
		List<int> list = new List<int>();
		foreach (Graph item in lstSortedGraph)
		{
			item.Sort();
			if (item.UnsortedList != null)
			{
				List<int> unsortedList = item.UnsortedList;
				unsortedList.Sort((int a, int b) => GetRelation(b, a).compairison);
				List<int> lstB = MergeSortedIndices(item.SortedList, unsortedList);
				list = MergeSortedIndices(list, lstB);
			}
			else
			{
				list = MergeSortedIndices(list, item.SortedList);
			}
		}
		return list;
	}

	private List<int> MergeSortedIndices(List<int> lstA, List<int> lstB)
	{
		List<int> retval = new List<int>(lstA.Count + lstB.Count);
		int indexA = 0;
		int indexB = 0;
		while (indexA < lstA.Count || indexB < lstB.Count)
		{
			if (indexA >= lstA.Count)
			{
				AddB();
				continue;
			}
			if (indexB >= lstB.Count)
			{
				AddA();
				continue;
			}
			Relation relation = GetRelation(lstA[indexA], lstB[indexB]);
			if (relation.compairison < 0)
			{
				AddB();
			}
			else if (relation.compairison > 0)
			{
				AddA();
			}
			else if (lstA[indexA] < lstB[indexB])
			{
				AddA();
			}
			else
			{
				AddB();
			}
		}
		return retval;
		void AddA()
		{
			retval.Add(lstA[indexA]);
			indexA++;
		}
		void AddB()
		{
			retval.Add(lstB[indexB]);
			indexB++;
		}
	}
}
