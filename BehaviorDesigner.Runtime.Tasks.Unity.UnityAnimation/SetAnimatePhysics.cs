using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation;

[TaskCategory("Unity/Animation")]
[TaskDescription("Sets animate physics to the specified value. Returns Success.")]
public class SetAnimatePhysics : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("Are animations executed in the physics loop?")]
	public SharedBool animatePhysics;

	private Animation animation;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			animation = defaultGameObject.GetComponent<Animation>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (animation == null)
		{
			Debug.LogWarning("Animation is null");
			return TaskStatus.Failure;
		}
		animation.animatePhysics = animatePhysics.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		animatePhysics.Value = false;
	}
}
