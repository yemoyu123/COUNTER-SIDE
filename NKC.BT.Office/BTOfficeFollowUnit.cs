using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeFollowUnit : BTOfficeActionBase
{
	[Header("목표 유닛. TargetUnitSelector에서 지정됨")]
	public BTSharedOfficeCharacter FollowTarget;

	[Header("이 거리 안까지 도달하면 완료")]
	public float FollowTargetRange = 200f;

	private float m_fTargetRangeSquared;

	private bool m_bFollowing;

	private Vector3 m_vMoveTargetPosition;

	[Header("따라갈때 사용할 애니")]
	public SharedString MoveAnimName;

	[Header("따라갈때 이동속도")]
	public SharedFloat MoveSpeed = 450f;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		m_fTargetRangeSquared = FollowTargetRange * FollowTargetRange;
		if (FollowTarget.Value == null)
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
		if (FollowTarget.Value == null)
		{
			return TaskStatus.Failure;
		}
		Vector3 position = FollowTarget.Value.transform.position;
		position = m_OfficeBuilding.trActorRoot.InverseTransformPoint(position);
		if (m_bFollowing)
		{
			Vector3 position2 = m_Character.transform.position;
			position2 = m_OfficeBuilding.trActorRoot.InverseTransformPoint(position2);
			if ((position2 - position).sqrMagnitude <= m_fTargetRangeSquared)
			{
				m_Character.StopAllAnimInstances();
				m_bFollowing = false;
				return TaskStatus.Success;
			}
			if (m_Character.PlayAnimCompleted())
			{
				m_vMoveTargetPosition = position;
				if (!Move(position, ignoreObstacles: false, MoveSpeed.Value, MoveAnimName.Value))
				{
					return TaskStatus.Failure;
				}
				return TaskStatus.Running;
			}
			if ((position - m_vMoveTargetPosition).sqrMagnitude > m_fTargetRangeSquared)
			{
				m_Character.StopAllAnimInstances();
				m_vMoveTargetPosition = position;
				if (!Move(position, ignoreObstacles: false, MoveSpeed.Value, MoveAnimName.Value))
				{
					return TaskStatus.Failure;
				}
				return TaskStatus.Running;
			}
			return TaskStatus.Running;
		}
		m_bFollowing = true;
		if (m_Character.PlayAnimCompleted())
		{
			m_vMoveTargetPosition = position;
			if (!Move(position, ignoreObstacles: false, MoveSpeed.Value, MoveAnimName.Value))
			{
				return TaskStatus.Failure;
			}
		}
		return TaskStatus.Running;
	}
}
