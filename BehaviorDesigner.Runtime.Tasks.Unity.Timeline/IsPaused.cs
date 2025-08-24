using UnityEngine;
using UnityEngine.Playables;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Timeline;

[TaskCategory("Unity/Timeline")]
[TaskDescription("Is the timeline currently paused?")]
public class IsPaused : Conditional
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	private PlayableDirector playableDirector;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			playableDirector = defaultGameObject.GetComponent<PlayableDirector>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (playableDirector == null)
		{
			Debug.LogWarning("PlayableDirector is null");
			return TaskStatus.Failure;
		}
		if (playableDirector.state != PlayState.Paused)
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
