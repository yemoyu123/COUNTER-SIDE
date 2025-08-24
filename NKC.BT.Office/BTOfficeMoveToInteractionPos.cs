using BehaviorDesigner.Runtime.Tasks;
using Cs.Math;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeMoveToInteractionPos : BTOfficeActionBase
{
	public bool m_bRun;

	private Vector3 TargetLocalPos;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		if (!m_Character.HasInteractionTarget())
		{
			bActionSuccessFlag = false;
			return;
		}
		TargetLocalPos = m_Character.GetInteractionPosition();
		bActionSuccessFlag = true;
		Vector3 localPosition = m_Character.transform.localPosition;
		if (!localPosition.x.IsNearlyEqual(TargetLocalPos.x, 0.0001f) || !localPosition.y.IsNearlyEqual(TargetLocalPos.y, 0.0001f) || !localPosition.z.IsNearlyEqual(TargetLocalPos.z, 0.0001f))
		{
			if (m_bRun)
			{
				m_Character.EnqueueAnimation(GetRunInstance(transform.localPosition, TargetLocalPos));
			}
			else
			{
				Move(TargetLocalPos, ignoreObstacles: false);
			}
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
			m_Character.transform.localPosition = TargetLocalPos;
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}

	public override void OnEnd()
	{
		base.OnEnd();
		if (!bActionSuccessFlag && m_Character != null)
		{
			m_Character.UnregisterInteraction();
		}
	}
}
