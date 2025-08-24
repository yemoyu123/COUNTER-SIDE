using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;

[TaskCategory("Unity/Transform")]
[TaskDescription("Sets the local scale of the Transform. Returns Success.")]
public class SetLocalScale : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The local scale of the Transform")]
	public SharedVector3 localScale;

	private Transform targetTransform;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			targetTransform = defaultGameObject.GetComponent<Transform>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (targetTransform == null)
		{
			Debug.LogWarning("Transform is null");
			return TaskStatus.Failure;
		}
		targetTransform.localScale = localScale.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		localScale = Vector3.zero;
	}
}
