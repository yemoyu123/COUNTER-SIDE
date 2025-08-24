using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody2D;

[TaskCategory("Unity/Rigidbody2D")]
[TaskDescription("Returns Success if the Rigidbody2D is sleeping, otherwise Failure.")]
public class IsSleeping : Conditional
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	private Rigidbody2D rigidbody2D;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			rigidbody2D = defaultGameObject.GetComponent<Rigidbody2D>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (rigidbody2D == null)
		{
			Debug.LogWarning("Rigidbody2D is null");
			return TaskStatus.Failure;
		}
		if (!rigidbody2D.IsSleeping())
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
	}
}
