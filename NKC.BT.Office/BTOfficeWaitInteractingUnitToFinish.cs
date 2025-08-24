using BehaviorDesigner.Runtime.Tasks;

namespace NKC.BT.Office;

public class BTOfficeWaitInteractingUnitToFinish : BTOfficeActionBase
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
		if (m_Character.CurrentUnitInteractionTarget != null && m_Character.CurrentUnitInteractionTarget.PlayingInteractionAnimation)
		{
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
			return TaskStatus.Success;
		}
		if (!m_Character.CurrentUnitInteractionTarget.PlayingInteractionAnimation)
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}

	public override void OnEnd()
	{
		m_Character.StopAllAnimInstances();
		m_Character.UnregisterInteraction();
	}
}
