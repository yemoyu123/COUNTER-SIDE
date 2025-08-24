using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;

[TaskCategory("Unity/NavMeshAgent")]
[TaskDescription("Gets the destination of the agent in world-space units. Returns Success.")]
public class GetDestination : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[SharedRequired]
	[Tooltip("The NavMeshAgent destination")]
	public SharedVector3 storeValue;

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
		storeValue.Value = navMeshAgent.destination;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		storeValue = Vector3.zero;
	}
}
