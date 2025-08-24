using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation;

[TaskCategory("Unity/Animation")]
[TaskDescription("Returns Success if the animation is currently playing.")]
public class IsPlaying : Conditional
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The name of the animation")]
	public SharedString animationName;

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
		if (string.IsNullOrEmpty(animationName.Value))
		{
			if (!animation.isPlaying)
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}
		if (!animation.IsPlaying(animationName.Value))
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		animationName.Value = "";
	}
}
