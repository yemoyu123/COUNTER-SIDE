using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;

[TaskCategory("Unity/Rigidbody")]
[TaskDescription("Stores the freeze rotation value of the Rigidbody. Returns Success.")]
public class GetFreezeRotation : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The freeze rotation value of the Rigidbody")]
	[RequiredField]
	public SharedBool storeValue;

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
		storeValue.Value = rigidbody.freezeRotation;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		storeValue = false;
	}
}
