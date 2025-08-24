using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;

[TaskCategory("Unity/NavMeshAgent")]
[TaskDescription("Sets the stop status. Returns Success.")]
public class SetIsStopped : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The stop status")]
	public SharedBool isStopped;

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
		navMeshAgent.isStopped = isStopped.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
	}
}
