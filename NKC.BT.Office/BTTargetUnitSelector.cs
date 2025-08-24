using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using NKC.Office;

namespace NKC.BT.Office;

public class BTTargetUnitSelector : BTOfficeActionBase
{
	public BTSharedTargetUnitType ActorTargetType = ActTargetType.Unit;

	public BTSharedStringList lstUnitID;

	public SharedBool KeepSelectedUnit = true;

	public BTSharedOfficeCharacter TargetUnit;

	private HashSet<string> hsActTargetGroupID;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		if (lstUnitID == null || lstUnitID.Value == null || lstUnitID.Value.Count == 0)
		{
			bActionSuccessFlag = false;
			return;
		}
		if (KeepSelectedUnit.Value && TargetUnit.Value != null)
		{
			bActionSuccessFlag = true;
			return;
		}
		if (hsActTargetGroupID == null || hsActTargetGroupID.Count != lstUnitID.Value.Count)
		{
			hsActTargetGroupID = new HashSet<string>(lstUnitID.Value);
		}
		ActTargetType value = ActorTargetType.Value;
		foreach (NKCOfficeCharacter item in m_OfficeBuilding.GetCharacterEnumerator())
		{
			if (NKCOfficeManager.IsActTarget(item, value, hsActTargetGroupID) && !(item == m_Character))
			{
				TargetUnit.Value = item;
			}
		}
		bActionSuccessFlag = true;
	}

	public override TaskStatus OnUpdate()
	{
		if (!bActionSuccessFlag)
		{
			return TaskStatus.Failure;
		}
		if (TargetUnit.Value == null)
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}
}
