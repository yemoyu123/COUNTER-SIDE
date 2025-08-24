using NKC.Office;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeMoveToEmptyCell : BTOfficeActionBase
{
	public bool bWarp;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		bActionSuccessFlag = true;
		OfficeFloorPosition officeFloorPosition = m_OfficeBuilding.CalculateFloorPosition(transform.localPosition);
		OfficeFloorPosition pos = new OfficeFloorPosition(Mathf.Clamp(officeFloorPosition.x, 0, m_OfficeBuilding.m_SizeX - 1), Mathf.Clamp(officeFloorPosition.y, 0, m_OfficeBuilding.m_SizeY - 1));
		pos = m_OfficeBuilding.FindNearestEmptyCell(pos);
		if (!officeFloorPosition.Equals(pos))
		{
			Vector3 localPos = base.Floor.GetLocalPos(pos);
			if (bWarp)
			{
				transform.localPosition = localPos;
			}
			else
			{
				Move(pos, ignoreObstacles: true);
			}
		}
	}
}
