using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;

[TaskCategory("Unity/NavMeshAgent")]
[TaskDescription("Resumes the movement along the current path after a pause. Returns Success.")]
public class Resume : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

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
		navMeshAgent.isStopped = false;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
	}
}
