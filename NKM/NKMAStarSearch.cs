using System;
using System.Collections.Generic;

namespace NKM;

public class NKMAStarSearch
{
	public Dictionary<NKMAStarSearchLocation, NKMAStarSearchLocation> cameFrom = new Dictionary<NKMAStarSearchLocation, NKMAStarSearchLocation>();

	public Dictionary<NKMAStarSearchLocation, float> costSoFar = new Dictionary<NKMAStarSearchLocation, float>();

	private NKMAStarSearchLocation start;

	private NKMAStarSearchLocation goal;

	public static float Heuristic(NKMAStarSearchLocation a, NKMAStarSearchLocation b)
	{
		return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
	}

	public NKMAStarSearch(SquareGrid graph, NKMAStarSearchLocation start, NKMAStarSearchLocation goal)
	{
		this.start = start;
		this.goal = goal;
		PriorityQueue<NKMAStarSearchLocation> priorityQueue = new PriorityQueue<NKMAStarSearchLocation>();
		priorityQueue.Enqueue(start, 0f);
		cameFrom.Add(start, start);
		costSoFar.Add(start, 0f);
		while ((float)priorityQueue.Count > 0f)
		{
			NKMAStarSearchLocation nKMAStarSearchLocation = priorityQueue.Dequeue();
			if (nKMAStarSearchLocation.Equals(goal))
			{
				break;
			}
			foreach (NKMAStarSearchLocation item in graph.Neighbors(nKMAStarSearchLocation))
			{
				float num = costSoFar[nKMAStarSearchLocation] + graph.Cost(nKMAStarSearchLocation, item);
				if (!costSoFar.ContainsKey(item) || num < costSoFar[item])
				{
					if (costSoFar.ContainsKey(item))
					{
						costSoFar.Remove(item);
						cameFrom.Remove(item);
					}
					costSoFar.Add(item, num);
					cameFrom.Add(item, nKMAStarSearchLocation);
					float priority = num + Heuristic(item, goal);
					priorityQueue.Enqueue(item, priority);
				}
			}
		}
	}

	public List<NKMAStarSearchLocation> FindPath()
	{
		List<NKMAStarSearchLocation> list = new List<NKMAStarSearchLocation>();
		NKMAStarSearchLocation nKMAStarSearchLocation = goal;
		while (!nKMAStarSearchLocation.Equals(start))
		{
			if (!cameFrom.ContainsKey(nKMAStarSearchLocation))
			{
				return new List<NKMAStarSearchLocation>();
			}
			list.Add(nKMAStarSearchLocation);
			nKMAStarSearchLocation = cameFrom[nKMAStarSearchLocation];
		}
		list.Reverse();
		return list;
	}
}
