using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityLight;

[TaskCategory("Unity/Light")]
[TaskDescription("Sets the shadow strength of the light.")]
public class SetShadowSoftnessStrength : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The shadow strength to set")]
	public SharedFloat shadowStrength;

	private Light light;

	private GameObject prevGameObject;

	public override void OnStart()
	{
		GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (defaultGameObject != prevGameObject)
		{
			light = defaultGameObject.GetComponent<Light>();
			prevGameObject = defaultGameObject;
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (light == null)
		{
			Debug.LogWarning("Light is null");
			return TaskStatus.Failure;
		}
		light.shadowStrength = shadowStrength.Value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		shadowStrength = 0f;
	}
}
