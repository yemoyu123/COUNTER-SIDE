using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem;

[TaskCategory("Unity/ParticleSystem")]
[TaskDescription("Sets the playback speed of the Particle System.")]
public class SetPlaybackSpeed : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The playback speed of the ParticleSystem")]
	public SharedFloat playbackSpeed = 1f;

	private ParticleSystem particleSystem;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			particleSystem = defaultGameObject.GetComponent<ParticleSystem>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (particleSystem == null)
		{
			Debug.LogWarning("ParticleSystem is null");
			return TaskStatus.Failure;
		}
		ParticleSystem.MainModule main = particleSystem.main;
		main.simulationSpeed = playbackSpeed.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		playbackSpeed = 1f;
	}
}
