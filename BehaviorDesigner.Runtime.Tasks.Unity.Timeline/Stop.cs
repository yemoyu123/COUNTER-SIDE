using UnityEngine;
using UnityEngine.Playables;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Timeline;

[TaskCategory("Unity/Timeline")]
[TaskDescription("Stops playback of the current Playable and destroys the corresponding graph.")]
public class Stop : Action
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
		playableDirector.Stop();
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
	}
}
