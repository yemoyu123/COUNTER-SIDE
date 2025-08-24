using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;

[TaskCategory("Unity/Rigidbody")]
[TaskDescription("Applies a torque to the rigidbody relative to its coordinate system. Returns Success.")]
public class AddRelativeTorque : Action
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
		rigidbody.AddRelativeTorque(torque.Value, forceMode);
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		torque = Vector3.zero;
		forceMode = ForceMode.Force;
	}
}
