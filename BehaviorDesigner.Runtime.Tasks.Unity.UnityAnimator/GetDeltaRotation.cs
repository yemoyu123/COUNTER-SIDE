using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator;

[TaskCategory("Unity/Animator")]
[TaskDescription("Gets the avatar delta rotation for the last evaluated frame. Returns Success.")]
public class GetDeltaRotation : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The avatar delta rotation")]
	[RequiredField]
	public SharedQuaternion storeValue;

	private Animator animator;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			animator = defaultGameObject.GetComponent<Animator>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (animator == null)
		{
			Debug.LogWarning("Animator is null");
			return TaskStatus.Failure;
		}
		storeValue.Value = animator.deltaRotation;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		if (storeValue != null)
		{
			storeValue.Value = Quaternion.identity;
		}
	}
}
