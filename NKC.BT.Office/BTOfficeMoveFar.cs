using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using NKC.Office;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeMoveFar : BTOfficeActionBase
{
	public SharedString MoveAnimEventName;

	public SharedFloat MinRange = 600f;

	public SharedBool m_bIgnoreObstacle = false;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		bActionSuccessFlag = true;
		if (string.IsNullOrEmpty(MoveAnimEventName.Value))
		{
			Debug.LogError("BTOfficeMoveFar : has no warpAnimation!");
			bActionSuccessFlag = false;
			return;
		}
		List<NKCAnimationEventTemplet> list = NKCAnimationEventManager.Find(MoveAnimEventName.Value);
		if (list == null)
		{
			Debug.LogError("BTWarp : animation " + MoveAnimEventName.Value + " not found!");
			bActionSuccessFlag = false;
			return;
		}
		Vector3 localPosition = transform.localPosition;
		float num = MinRange.Value * MinRange.Value;
		List<OfficeFloorPosition> list2 = new List<OfficeFloorPosition>();
		for (int i = 0; i < m_OfficeBuilding.m_SizeX; i++)
		{
			for (int j = 0; j < m_OfficeBuilding.m_SizeY; j++)
			{
				if (m_OfficeBuilding.FloorMap[i, j] == 0L)
				{
					Vector3 localPos = m_OfficeBuilding.m_Floor.GetLocalPos(i, j);
					if ((localPosition - localPos).sqrMagnitude >= num)
					{
						list2.Add(new OfficeFloorPosition(i, j));
					}
				}
			}
		}
		if (list2.Count == 0)
		{
			bActionSuccessFlag = false;
			return;
		}
		OfficeFloorPosition pos = list2[Random.Range(0, list2.Count)];
		Vector3 localPos2 = m_OfficeBuilding.m_Floor.GetLocalPos(pos);
		if (!Move(list, localPos2, m_bIgnoreObstacle.Value))
		{
			bActionSuccessFlag = false;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (!bActionSuccessFlag)
		{
			return TaskStatus.Failure;
		}
		if (m_Character.PlayAnimCompleted())
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}
}
