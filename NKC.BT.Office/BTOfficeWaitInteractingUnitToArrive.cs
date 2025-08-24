using BehaviorDesigner.Runtime.Tasks;
using Cs.Math;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeWaitInteractingUnitToArrive : BTOfficeActionBase
{
	private bool bWaitRequired;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		bActionSuccessFlag = true;
		if (m_Character.CurrentUnitInteractionTemplet != null && !m_Character.CurrentUnitInteractionTemplet.IsSoloAction && m_Character.CurrentUnitInteractionTarget != null)
		{
			Vector3 vector = m_Character.transform.parent.TransformPoint(m_Character.CurrentUnitInteractionTarget.CurrentUnitInteractionPosition);
			bool bLeft = m_Character.transform.position.x >= vector.x;
			m_Character.EnqueueAnimation(GetInvertDirectionInstance(bLeft));
			float animTime = m_Character.GetAnimTime("SD_IDLE");
			NKCAnimationInstance idleInstance = GetIdleInstance(animTime, m_Character.transform.localPosition);
			m_Character.EnqueueAnimation(idleInstance);
			bWaitRequired = true;
		}
		else
		{
			bWaitRequired = false;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (!bActionSuccessFlag)
		{
			return TaskStatus.Failure;
		}
		if (!bWaitRequired)
		{
			return TaskStatus.Success;
		}
		if (m_Character.CurrentUnitInteractionTarget == null || !m_Character.CurrentUnitInteractionTarget.HasInteractionTarget())
		{
			m_Character.UnregisterInteraction();
			return TaskStatus.Failure;
		}
		Vector3 localPosition = m_Character.CurrentUnitInteractionTarget.transform.localPosition;
		Vector3 currentUnitInteractionPosition = m_Character.CurrentUnitInteractionTarget.CurrentUnitInteractionPosition;
		if (localPosition.x.IsNearlyEqual(currentUnitInteractionPosition.x, 0.0001f) && localPosition.y.IsNearlyEqual(currentUnitInteractionPosition.y, 0.0001f) && localPosition.z.IsNearlyEqual(currentUnitInteractionPosition.z, 0.0001f))
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}

	public override void OnEnd()
	{
		m_Character.StopAllAnimInstances();
	}
}
