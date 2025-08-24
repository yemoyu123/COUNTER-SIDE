using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;

[TaskCategory("Unity/NavMeshAgent")]
[TaskDescription("Gets the distance between the agent's position and the destination on the current path. Returns Success.")]
public class GetRemainingDistance : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[SharedRequired]
	[Tooltip("The remaining distance")]
	public SharedFloat storeValue;

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
		storeValue.Value = navMeshAgent.remainingDistance;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		storeValue = 0f;
	}
}
