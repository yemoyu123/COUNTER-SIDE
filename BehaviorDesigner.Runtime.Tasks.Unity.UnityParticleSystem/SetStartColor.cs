using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem;

[TaskCategory("Unity/ParticleSystem")]
[TaskDescription("Sets the start color of the Particle System.")]
public class SetStartColor : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The start color of the ParticleSystem")]
	public SharedColor startColor;

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
		main.startColor = startColor.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		startColor = Color.white;
	}
}
