using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeCondFindUnit : BTOfficeConditionBase
{
	[Header("목표 유닛 (UnitSelector에서 지정됨)")]
	public BTSharedOfficeCharacter FollowTarget;

	[Header("거리. 음수인 경우 유닛이 있기만 하면 True")]
	public float m_fRange;

	public override TaskStatus OnUpdate()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			return TaskStatus.Failure;
		}
		if (IsTargetInRoom())
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}

	private bool IsTargetInRoom()
	{
		Vector3 position = m_Character.transform.position;
		float num = m_fRange * m_fRange;
		if (FollowTarget.Value == null)
		{
			return false;
		}
		if (m_fRange < 0f)
		{
			return true;
		}
		Vector3 position2 = FollowTarget.Value.transform.position;
		if ((position - position2).sqrMagnitude <= num)
		{
			return true;
		}
		return false;
	}
}
