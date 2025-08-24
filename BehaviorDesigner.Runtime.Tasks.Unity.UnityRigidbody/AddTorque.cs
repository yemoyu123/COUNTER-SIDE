using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;

[TaskCategory("Unity/Rigidbody")]
[TaskDescription("Applies a torque to the rigidbody. Returns Success.")]
public class AddTorque : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The amount of torque to apply")]
	public SharedVector3 torque;

	[Tooltip("The type of torque")]
	public ForceMode forceMode;

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
		rigidbody.AddTorque(torque.Value, forceMode);
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		torque = Vector3.zero;
		forceMode = ForceMode.Force;
	}
}
