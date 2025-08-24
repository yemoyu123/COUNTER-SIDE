using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator;

[TaskCategory("Unity/Animator")]
[TaskDescription("Sets a trigger parameter to active or inactive. Returns Success.")]
public class SetTrigger : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The name of the parameter")]
	public SharedString paramaterName;

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
		animator.SetTrigger(paramaterName.Value);
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		paramaterName = "";
	}
}
