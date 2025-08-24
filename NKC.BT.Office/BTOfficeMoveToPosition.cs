using BehaviorDesigner.Runtime.Tasks;
using Cs.Math;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeMoveToPosition : BTOfficeActionBase
{
	public Vector3 TargetLocalPos;

	public float moveSpeed = 200f;

	public string AnimName;

	[Header("이동시 사용할 이벤트. AnimEventName 있는 경우 AnimName, moveSpeed는 무시")]
	public string AnimEventName;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
		}
		else
		{
			bActionSuccessFlag = true;
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
			if (transform.localPosition.x.IsNearlyEqual(TargetLocalPos.x) && transform.localPosition.y.IsNearlyEqual(TargetLocalPos.y) && transform.localPosition.z.IsNearlyEqual(TargetLocalPos.z))
			{
				return TaskStatus.Success;
			}
			if (string.IsNullOrEmpty(AnimEventName))
			{
				m_Character.EnqueueAnimation(GetWalkInstance(transform.localPosition, TargetLocalPos, moveSpeed, AnimName));
			}
			else
			{
				m_Character.EnqueueAnimation(new NKCAnimationInstance(m_Character, m_OfficeBuilding.transform, NKCAnimationEventManager.Find(AnimEventName), transform.localPosition, TargetLocalPos));
			}
		}
		return TaskStatus.Running;
	}
}
