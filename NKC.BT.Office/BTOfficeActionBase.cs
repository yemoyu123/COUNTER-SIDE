using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using NKC.AI.PathFinder;
using NKC.Office;
using UnityEngine;

namespace NKC.BT.Office;

public abstract class BTOfficeActionBase : Action
{
	protected NKCOfficeCharacter m_Character;

	protected NKCOfficeBuildingBase m_OfficeBuilding;

	protected bool bActionSuccessFlag;

	protected NKCOfficeFloorBase Floor => m_OfficeBuilding?.m_Floor;

	protected long[,] FloorMap => m_OfficeBuilding?.FloorMap;

	public override void OnAwake()
	{
		m_Character = GetComponent<NKCOfficeCharacter>();
		m_OfficeBuilding = m_Character?.OfficeBuilding;
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

	protected NKCAnimationInstance GetInvertDirectionInstance(bool bLeft)
	{
		List<NKCAnimationEventTemplet> list = new List<NKCAnimationEventTemplet>();
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = "InvertDir",
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.INVERT_MODEL_X,
			m_BoolValue = bLeft
		});
		return new NKCAnimationInstance(m_Character, m_OfficeBuilding.transform, list, m_Character.transform.localPosition, m_Character.transform.localPosition);
	}

	protected NKCAnimationInstance GetWalkInstance(Vector3 startPos, Vector3 destination, float speed = 150f, string animName = "")
	{
		return new NKCAnimationInstance(m_Character, m_OfficeBuilding.transform, DefaultWalkEvent(speed, animName), startPos, destination);
	}

	protected static List<NKCAnimationEventTemplet> DefaultWalkEvent(float speed = 150f, string animName = "")
	{
		List<NKCAnimationEventTemplet> list = new List<NKCAnimationEventTemplet>();
		if (string.IsNullOrEmpty(animName))
		{
			list.Add(new NKCAnimationEventTemplet
			{
				m_AniEventStrID = "WALK_DEFAULT",
				m_StartTime = 0f,
				m_AniEventType = AnimationEventType.ANIMATION_SPINE,
				m_StrValue = "SD_WALK",
				m_FloatValue = 1f,
				m_BoolValue = true
			});
		}
		else
		{
			list.Add(new NKCAnimationEventTemplet
			{
				m_AniEventStrID = "WALK_DEFAULT",
				m_StartTime = 0f,
				m_AniEventType = AnimationEventType.ANIMATION_NAME_SPINE,
				m_StrValue = animName,
				m_FloatValue = 1f,
				m_BoolValue = true
			});
		}
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = "WALK_DEFAULT",
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.SET_ABSOLUTE_MOVE_SPEED,
			m_FloatValue = speed
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = "WALK_DEFAULT",
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.INVERT_MODEL_X_BY_DIRECTION,
			m_BoolValue = true
		});
		return list;
	}

	protected NKCAnimationInstance GetIdleInstance(float time, Vector3 position, string idleAnimName = "")
	{
		return new NKCAnimationInstance(m_Character, m_OfficeBuilding.transform, DefaultIdleEvent(time, idleAnimName), position, position);
	}

	protected static List<NKCAnimationEventTemplet> DefaultIdleEvent(float time, string idleAnimName = "")
	{
		List<NKCAnimationEventTemplet> list = new List<NKCAnimationEventTemplet>();
		if (string.IsNullOrEmpty(idleAnimName))
		{
			list.Add(new NKCAnimationEventTemplet
			{
				m_AniEventStrID = "IDLE_DEFAULT",
				m_StartTime = 0f,
				m_AniEventType = AnimationEventType.ANIMATION_SPINE,
				m_StrValue = "SD_IDLE",
				m_FloatValue = 1f,
				m_BoolValue = true
			});
		}
		else
		{
			list.Add(new NKCAnimationEventTemplet
			{
				m_AniEventStrID = "IDLE_DEFAULT",
				m_StartTime = 0f,
				m_AniEventType = AnimationEventType.ANIMATION_NAME_SPINE,
				m_StrValue = idleAnimName,
				m_FloatValue = 1f,
				m_BoolValue = true
			});
		}
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = "IDLE_DEFAULT",
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.SET_MOVE_SPEED,
			m_FloatValue = 0f
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = "IDLE_DEFAULT",
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.SET_POSITION,
			m_FloatValue = 0f
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = "IDLE_DEFAULT",
			m_StartTime = time,
			m_AniEventType = AnimationEventType.SET_POSITION,
			m_FloatValue = 1f
		});
		return list;
	}

	protected NKCAnimationInstance GetRunInstance(Vector3 startPos, Vector3 destination, float speed = 600f, string animName = "")
	{
		return new NKCAnimationInstance(m_Character, m_OfficeBuilding.transform, DefaultRunEvent(speed, animName), startPos, destination);
	}

	public static List<NKCAnimationEventTemplet> DefaultRunEvent(float speed = 600f, string animName = "")
	{
		List<NKCAnimationEventTemplet> list = new List<NKCAnimationEventTemplet>();
		if (string.IsNullOrEmpty(animName))
		{
			list.Add(new NKCAnimationEventTemplet
			{
				m_AniEventStrID = "RUN_DEFAULT",
				m_StartTime = 0f,
				m_AniEventType = AnimationEventType.ANIMATION_SPINE,
				m_StrValue = "SD_RUN",
				m_FloatValue = 1.2f,
				m_BoolValue = true
			});
		}
		else
		{
			list.Add(new NKCAnimationEventTemplet
			{
				m_AniEventStrID = "RUN_DEFAULT",
				m_StartTime = 0f,
				m_AniEventType = AnimationEventType.ANIMATION_NAME_SPINE,
				m_StrValue = animName,
				m_FloatValue = 1.2f,
				m_BoolValue = true
			});
		}
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = "RUN_DEFAULT",
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.SET_ABSOLUTE_MOVE_SPEED,
			m_FloatValue = speed
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = "RUN_DEFAULT",
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.INVERT_MODEL_X_BY_DIRECTION,
			m_BoolValue = true
		});
		return list;
	}

	protected bool Move(List<NKCAnimationEventTemplet> lstMoveEventTemplet, Vector3 endLocalPos, bool ignoreObstacles)
	{
		if (ignoreObstacles)
		{
			NKCAnimationInstance instance = new NKCAnimationInstance(m_Character, m_OfficeBuilding.transform, lstMoveEventTemplet, transform.localPosition, endLocalPos);
			m_Character.EnqueueAnimation(instance);
			return true;
		}
		OfficeFloorPosition officeFloorPosition = m_OfficeBuilding.CalculateFloorPosition(endLocalPos, bClamp: true);
		OfficeFloorPosition officeFloorPosition2 = m_OfficeBuilding.CalculateFloorPosition(transform.localPosition, bClamp: true);
		List<(int, int)> path = new NKCAStar(FloorMap, officeFloorPosition2.ToPair, officeFloorPosition.ToPair).GetPath(smoothen: true);
		if (path == null)
		{
			return false;
		}
		Vector3 startPos = transform.localPosition;
		for (int i = 0; i < path.Count - 1; i++)
		{
			(int, int) pos = path[i];
			Vector3 localPos = m_Character.GetLocalPos(pos);
			NKCAnimationInstance instance2 = new NKCAnimationInstance(m_Character, m_OfficeBuilding.transform, lstMoveEventTemplet, startPos, localPos);
			m_Character.EnqueueAnimation(instance2);
			startPos = localPos;
		}
		NKCAnimationInstance instance3 = new NKCAnimationInstance(m_Character, m_OfficeBuilding.transform, lstMoveEventTemplet, startPos, endLocalPos);
		m_Character.EnqueueAnimation(instance3);
		return true;
	}

	protected bool Move(Vector3 endLocalPos, bool ignoreObstacles, float speed = 150f, string animName = "")
	{
		if (ignoreObstacles)
		{
			NKCAnimationInstance walkInstance = GetWalkInstance(transform.localPosition, endLocalPos, speed, animName);
			m_Character.EnqueueAnimation(walkInstance);
			return true;
		}
		OfficeFloorPosition officeFloorPosition = m_OfficeBuilding.CalculateFloorPosition(endLocalPos, bClamp: true);
		OfficeFloorPosition officeFloorPosition2 = m_OfficeBuilding.CalculateFloorPosition(transform.localPosition, bClamp: true);
		List<(int, int)> path = new NKCAStar(FloorMap, officeFloorPosition2.ToPair, officeFloorPosition.ToPair).GetPath(smoothen: true);
		if (path == null)
		{
			Debug.LogWarning($"From {officeFloorPosition2} to {officeFloorPosition} : path not found!");
			return false;
		}
		Vector3 startPos = transform.localPosition;
		for (int i = 0; i < path.Count - 1; i++)
		{
			(int, int) pos = path[i];
			Vector3 localPos = m_Character.GetLocalPos(pos);
			NKCAnimationInstance walkInstance2 = GetWalkInstance(startPos, localPos, speed, animName);
			m_Character.EnqueueAnimation(walkInstance2);
			startPos = localPos;
		}
		NKCAnimationInstance walkInstance3 = GetWalkInstance(startPos, endLocalPos, speed, animName);
		m_Character.EnqueueAnimation(walkInstance3);
		return true;
	}

	protected bool Move(OfficeFloorPosition endPos, bool ignoreObstacles)
	{
		if (ignoreObstacles)
		{
			Vector3 localPos = Floor.GetLocalPos(endPos);
			NKCAnimationInstance walkInstance = GetWalkInstance(transform.localPosition, localPos);
			m_Character.EnqueueAnimation(walkInstance);
			return true;
		}
		OfficeFloorPosition officeFloorPosition = m_OfficeBuilding.CalculateFloorPosition(transform.localPosition);
		List<(int, int)> path = new NKCAStar(FloorMap, officeFloorPosition.ToPair, endPos.ToPair).GetPath(smoothen: true);
		if (path == null)
		{
			Debug.LogWarning($"From {officeFloorPosition} to {endPos} : path not found!");
			return false;
		}
		Vector3 startPos = transform.localPosition;
		foreach (var item in path)
		{
			Vector3 localPos2 = m_Character.GetLocalPos(item);
			NKCAnimationInstance walkInstance2 = GetWalkInstance(startPos, localPos2);
			m_Character.EnqueueAnimation(walkInstance2);
			startPos = localPos2;
		}
		return true;
	}
}
