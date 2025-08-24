using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;

[TaskCategory("Unity/Transform")]
[TaskDescription("Stores the parent of the Transform. Returns Success.")]
public class GetParent : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The parent of the Transform")]
	[RequiredField]
	public SharedTransform storeValue;

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
		storeValue.Value = targetTransform.parent;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		storeValue = null;
	}
}
