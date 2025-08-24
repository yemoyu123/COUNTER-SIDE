using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator;

[TaskCategory("Unity/Animator")]
[TaskDescription("Stores the layer's weight. Returns Success.")]
public class GetLayerWeight : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The index of the layer")]
	public SharedInt index;

	[Tooltip("The value of the float parameter")]
	[RequiredField]
	public SharedFloat storeValue;

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
		storeValue.Value = animator.GetLayerWeight(index.Value);
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		index = 0;
		storeValue = 0f;
	}
}
