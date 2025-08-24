using UnityEngine;
using UnityEngine.Playables;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Timeline;

[TaskCategory("Unity/Timeline")]
[TaskDescription("Resume playing a paused playable.")]
public class Resume : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("Should the task be stopped when the timeline has stopped playing?")]
	public SharedBool stopWhenComplete;

	private PlayableDirector playableDirector;

	private GameObject prevGameObject;

	private bool playbackStarted;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			playableDirector = defaultGameObject.GetComponent<PlayableDirector>();
			prevGameObject = defaultGameObject;
		}
		playbackStarted = false;
	}

	public override TaskStatus OnUpdate()
	{
		if (playableDirector == null)
		{
			Debug.LogWarning("PlayableDirector is null");
			return TaskStatus.Failure;
		}
		if (playbackStarted)
		{
			if (stopWhenComplete.Value && playableDirector.state == PlayState.Playing)
			{
				return TaskStatus.Running;
			}
			return TaskStatus.Success;
		}
		playableDirector.Resume();
		playbackStarted = true;
		if (!stopWhenComplete.Value)
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		stopWhenComplete = false;
	}
}
