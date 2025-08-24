using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using NKC.Office;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeCondStuck : BTOfficeConditionBase
{
	[Header("이동 가능한 칸 수가 이 칸보다 작거나 같음")]
	public int freeSpaceCount;

	public override TaskStatus OnUpdate()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			return TaskStatus.Failure;
		}
		OfficeFloorPosition origin = m_OfficeBuilding.CalculateFloorPosition(transform.localPosition);
		if (IsStuck(freeSpaceCount, origin))
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}

	private bool IsStuck(int spaceCount, OfficeFloorPosition origin)
	{
		long[,] FloorMap = m_OfficeBuilding.FloorMap;
		if (FloorMap == null)
		{
			return false;
		}
		Queue<OfficeFloorPosition> queue = new Queue<OfficeFloorPosition>();
		HashSet<OfficeFloorPosition> hashSet = new HashSet<OfficeFloorPosition>();
		queue.Enqueue(origin);
		while (queue.Count > 0)
		{
			OfficeFloorPosition item = queue.Dequeue();
			hashSet.Add(item);
			OfficeFloorPosition officeFloorPosition = new OfficeFloorPosition(item.x - 1, item.y);
			OfficeFloorPosition officeFloorPosition2 = new OfficeFloorPosition(item.x + 1, item.y);
			OfficeFloorPosition officeFloorPosition3 = new OfficeFloorPosition(item.x, item.y + 1);
			OfficeFloorPosition officeFloorPosition4 = new OfficeFloorPosition(item.x, item.y - 1);
			if (!hashSet.Contains(officeFloorPosition) && Possible(officeFloorPosition))
			{
				queue.Enqueue(officeFloorPosition);
			}
			if (!hashSet.Contains(officeFloorPosition2) && Possible(officeFloorPosition2))
			{
				queue.Enqueue(officeFloorPosition2);
			}
			if (!hashSet.Contains(officeFloorPosition3) && Possible(officeFloorPosition3))
			{
				queue.Enqueue(officeFloorPosition3);
			}
			if (!hashSet.Contains(officeFloorPosition4) && Possible(officeFloorPosition4))
			{
				queue.Enqueue(officeFloorPosition4);
			}
			if (hashSet.Count + queue.Count > spaceCount)
			{
				return false;
			}
		}
		return true;
		bool Possible(OfficeFloorPosition pos)
		{
			if (pos.x < 0)
			{
				return false;
			}
			if (pos.y < 0)
			{
				return false;
			}
			if (pos.x >= FloorMap.GetLength(0))
			{
				return false;
			}
			if (pos.y >= FloorMap.GetLength(1))
			{
				return false;
			}
			return FloorMap[pos.x, pos.y] == 0;
		}
	}
}
