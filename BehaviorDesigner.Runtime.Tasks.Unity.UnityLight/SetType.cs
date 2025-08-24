using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityLight;

[TaskCategory("Unity/Light")]
[TaskDescription("Sets the type of the light.")]
public class SetType : Action
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The type to set")]
	public LightType type;

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
		light.type = type;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
	}
}
