using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnitySphereCollider;

[TaskCategory("Unity/SphereCollider")]
[TaskDescription("Stores the radius of the SphereCollider. Returns Success.")]
public class GetRadius : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The radius of the SphereCollider")]
	[RequiredField]
	public SharedFloat storeValue;

	private SphereCollider sphereCollider;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			sphereCollider = defaultGameObject.GetComponent<SphereCollider>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (sphereCollider == null)
		{
			Debug.LogWarning("SphereCollider is null");
			return TaskStatus.Failure;
		}
		storeValue.Value = sphereCollider.radius;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		storeValue = 0f;
	}
}
