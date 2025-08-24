using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;

[TaskCategory("Unity/NavMeshAgent")]
[TaskDescription("Apply relative movement to the current position. Returns Success.")]
public class Move : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The relative movement vector")]
	public SharedVector3 offset;

	private NavMeshAgent navMeshAgent;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			navMeshAgent = defaultGameObject.GetComponent<NavMeshAgent>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (navMeshAgent == null)
		{
			Debug.LogWarning("NavMeshAgent is null");
			return TaskStatus.Failure;
		}
		navMeshAgent.Move(offset.Value);
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		offset = Vector3.zero;
	}
}
