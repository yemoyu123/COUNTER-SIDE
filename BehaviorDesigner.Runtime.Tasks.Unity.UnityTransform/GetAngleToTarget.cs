using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;

[TaskCategory("Unity/Transform")]
[TaskDescription("Gets the Angle between a GameObject's forward direction and a target. Returns Success.")]
public class GetAngleToTarget : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The target object to measure the angle to. If null the targetPosition will be used.")]
	public SharedGameObject targetObject;

	[Tooltip("The world position to measure an angle to. If the targetObject is also not null, this value is used as an offset from that object's position.")]
	public SharedVector3 targetPosition;

	[Tooltip("Ignore height differences when calculating the angle?")]
	public SharedBool ignoreHeight = true;

	[Tooltip("The angle to the target")]
	[RequiredField]
	public SharedFloat storeValue;

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
		Vector3 vector = ((!(targetObject.Value != null)) ? targetPosition.Value : targetObject.Value.transform.InverseTransformPoint(targetPosition.Value));
		if (ignoreHeight.Value)
		{
			vector.y = targetTransform.position.y;
		}
		Vector3 vector2 = vector - targetTransform.position;
		storeValue.Value = Vector3.Angle(vector2, targetTransform.forward);
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		targetObject = null;
		targetPosition = Vector3.zero;
		ignoreHeight = true;
		storeValue = 0f;
	}
}
