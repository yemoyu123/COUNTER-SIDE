using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;

[TaskCategory("Unity/Rigidbody")]
[TaskDescription("Sets the drag of the Rigidbody. Returns Success.")]
public class SetDrag : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The drag of the Rigidbody")]
	public SharedFloat drag;

	private Rigidbody rigidbody;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			rigidbody = defaultGameObject.GetComponent<Rigidbody>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (rigidbody == null)
		{
			Debug.LogWarning("Rigidbody is null");
			return TaskStatus.Failure;
		}
		rigidbody.drag = drag.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		drag = 0f;
	}
}
