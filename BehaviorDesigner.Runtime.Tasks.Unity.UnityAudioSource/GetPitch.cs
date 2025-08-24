using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAudioSource;

[TaskCategory("Unity/AudioSource")]
[TaskDescription("Stores the pitch value of the AudioSource. Returns Success.")]
public class GetPitch : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The pitch value of the AudioSource")]
	[RequiredField]
	public SharedFloat storeValue;

	private AudioSource audioSource;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			audioSource = defaultGameObject.GetComponent<AudioSource>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (audioSource == null)
		{
			Debug.LogWarning("AudioSource is null");
			return TaskStatus.Failure;
		}
		storeValue.Value = audioSource.pitch;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		storeValue = 1f;
	}
}
