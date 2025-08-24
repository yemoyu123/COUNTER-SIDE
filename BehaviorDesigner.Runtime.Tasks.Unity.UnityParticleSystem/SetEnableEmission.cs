using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem;

[TaskCategory("Unity/ParticleSystem")]
[TaskDescription("Enables or disables the Particle System emission.")]
public class SetEnableEmission : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("Enable the ParticleSystem emissions?")]
	public SharedBool enable;

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
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.enabled = enable.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		enable = false;
	}
}
