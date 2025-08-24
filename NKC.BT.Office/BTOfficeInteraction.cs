using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using NKC.Office;
using NKC.Templet.Office;

namespace NKC.BT.Office;

public class BTOfficeInteraction : BTOfficeActionBase
{
	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
		}
		else if (m_Character.CurrentFurnitureInteractionTemplet != null && m_Character.CurrentInteractionTargetFurniture != null)
		{
			NKCOfficeFurnitureInteractionTemplet currentFurnitureInteractionTemplet = m_Character.CurrentFurnitureInteractionTemplet;
			List<NKCAnimationEventTemplet> list = NKCAnimationEventManager.Find(currentFurnitureInteractionTemplet.UnitAni);
			if (list == null)
			{
				bActionSuccessFlag = false;
				return;
			}
			m_Character.SetPlayingInteractionAnimation(value: true);
			if (m_Character.CurrentFurnitureInteractionTemplet.eActType == NKCOfficeFurnitureInteractionTemplet.ActType.Reaction || m_Character.CurrentInteractionTargetFurniture.GetInteractionPoint() == null)
			{
				bool bLeft = m_Character.transform.position.x >= m_Character.CurrentInteractionTargetFurniture.transform.position.x;
				m_Character.EnqueueAnimation(GetInvertDirectionInstance(bLeft));
			}
			else
			{
				m_Character.EnqueueAnimation(GetInvertDirectionInstance(m_Character.CurrentInteractionTargetFurniture.GetInvert()));
			}
			m_Character.EnqueueAnimation(list);
			m_Character.CurrentInteractionTargetFurniture.PlayAnimationEvent(currentFurnitureInteractionTemplet.InteriorAni);
			bActionSuccessFlag = true;
		}
		else if (m_Character.CurrentUnitInteractionTemplet != null)
		{
			if (m_Character.CurrentUnitInteractionTemplet.IsSoloAction)
			{
				List<NKCAnimationEventTemplet> list2 = NKCAnimationEventManager.Find(m_Character.CurrentUnitInteractionTemplet.ActorAni);
				if (list2 == null)
				{
					bActionSuccessFlag = false;
					return;
				}
				m_Character.SetPlayingInteractionAnimation(value: true);
				m_Character.EnqueueAnimation(list2);
				bActionSuccessFlag = true;
				return;
			}
			NKCOfficeCharacter currentUnitInteractionTarget = m_Character.CurrentUnitInteractionTarget;
			NKCOfficeUnitInteractionTemplet currentUnitInteractionTemplet = m_Character.CurrentUnitInteractionTemplet;
			if (currentUnitInteractionTemplet == null || currentUnitInteractionTarget == null)
			{
				bActionSuccessFlag = false;
				return;
			}
			List<NKCAnimationEventTemplet> list3 = NKCAnimationEventManager.Find(m_Character.CurrentUnitInteractionIsMainActor ? currentUnitInteractionTemplet.ActorAni : currentUnitInteractionTemplet.TargetAni);
			if (list3 == null)
			{
				bActionSuccessFlag = false;
				return;
			}
			m_Character.SetPlayingInteractionAnimation(value: true);
			bool bLeft2 = m_Character.transform.position.x >= currentUnitInteractionTarget.transform.position.x;
			m_Character.EnqueueAnimation(GetInvertDirectionInstance(bLeft2));
			m_Character.EnqueueAnimation(list3);
			bActionSuccessFlag = true;
		}
		else
		{
			m_Character.UnregisterInteraction();
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

	public override void OnEnd()
	{
		m_Character.SetPlayingInteractionAnimation(value: false);
	}
}
