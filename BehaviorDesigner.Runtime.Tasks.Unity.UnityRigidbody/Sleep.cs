using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;

[TaskCategory("Unity/Rigidbody")]
[TaskDescription("Forces the Rigidbody to sleep at least one frame. Returns Success.")]
public class Sleep : Conditional
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

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
		rigidbody.Sleep();
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
	}
}
