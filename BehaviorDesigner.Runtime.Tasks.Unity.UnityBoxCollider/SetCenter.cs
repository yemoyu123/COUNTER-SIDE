using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityBoxCollider;

[TaskCategory("Unity/BoxCollider")]
[TaskDescription("Sets the center of the BoxCollider. Returns Success.")]
public class SetCenter : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The center of the BoxCollider")]
	public SharedVector3 center;

	private BoxCollider boxCollider;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			boxCollider = defaultGameObject.GetComponent<BoxCollider>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (boxCollider == null)
		{
			Debug.LogWarning("BoxCollider is null");
			return TaskStatus.Failure;
		}
		boxCollider.center = center.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		center = Vector3.zero;
	}
}
