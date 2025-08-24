using BehaviorDesigner.Runtime;
using NKC.Office;
using NKM;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeRandomRush : BTOfficeActionBase
{
	public SharedString RushAnimName;

	public SharedFloat RunSpeed = 600f;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		bActionSuccessFlag = true;
		OfficeFloorPosition pos = new OfficeFloorPosition(NKMRandom.Range(0, m_OfficeBuilding.m_SizeX), NKMRandom.Range(0, m_OfficeBuilding.m_SizeY));
		pos = m_OfficeBuilding.FindNearestEmptyCell(pos);
		Vector3 localPos = m_OfficeBuilding.m_Floor.GetLocalPos(pos);
		NKCAnimationInstance runInstance = GetRunInstance(transform.localPosition, localPos, RunSpeed.Value, RushAnimName.Value);
		m_Character.EnqueueAnimation(runInstance);
	}
}
