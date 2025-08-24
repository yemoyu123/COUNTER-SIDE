using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using NKC.Office;
using NKM;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeRandomWalk : BTOfficeActionBase
{
	[Header("한번에 최대 몇번까지 걷기 액션을 하는가")]
	public int RandomWalkCount = 10;

	private int randomWalkStepLeft;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		bActionSuccessFlag = true;
		randomWalkStepLeft = Random.Range(1, RandomWalkCount);
	}

	public override TaskStatus OnUpdate()
	{
		if (!bActionSuccessFlag)
		{
			return TaskStatus.Failure;
		}
		if (m_Character.PlayAnimCompleted())
		{
			if (randomWalkStepLeft <= 0)
			{
				return TaskStatus.Success;
			}
			randomWalkStepLeft--;
			OfficeFloorPosition origin = m_OfficeBuilding.CalculateFloorPosition(transform.localPosition);
			Vector3 localPosition = transform.localPosition;
			OfficeFloorPosition randomWalkCell = GetRandomWalkCell(origin, NKMRandom.Range(3, 5));
			Vector3 localPos = m_Character.GetLocalPos(randomWalkCell);
			m_Character.EnqueueAnimation(GetWalkInstance(localPosition, localPos));
			if (NKMRandom.Range(0, 3) == 0)
			{
				m_Character.EnqueueAnimation(GetIdleInstance(Random.Range(1f, 3f), localPos));
			}
		}
		return TaskStatus.Running;
	}

	private OfficeFloorPosition GetRandomWalkCell(OfficeFloorPosition origin, int step)
	{
		long[,] FloorMap = m_OfficeBuilding.FloorMap;
		if (FloorMap == null)
		{
			return origin;
		}
		List<OfficeFloorPosition> list = new List<OfficeFloorPosition>(4);
		OfficeFloorPosition result = origin;
		for (int i = 0; i < step; i++)
		{
			OfficeFloorPosition officeFloorPosition = new OfficeFloorPosition(result.x - 1, result.y);
			OfficeFloorPosition officeFloorPosition2 = new OfficeFloorPosition(result.x + 1, result.y);
			OfficeFloorPosition officeFloorPosition3 = new OfficeFloorPosition(result.x, result.y + 1);
			OfficeFloorPosition officeFloorPosition4 = new OfficeFloorPosition(result.x, result.y - 1);
			if (Possible(officeFloorPosition))
			{
				list.Add(officeFloorPosition);
			}
			if (Possible(officeFloorPosition2))
			{
				list.Add(officeFloorPosition2);
			}
			if (Possible(officeFloorPosition3))
			{
				list.Add(officeFloorPosition3);
			}
			if (Possible(officeFloorPosition4))
			{
				list.Add(officeFloorPosition4);
			}
			if (list.Count == 0)
			{
				return result;
			}
			result = list[Random.Range(0, list.Count)];
		}
		return result;
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
