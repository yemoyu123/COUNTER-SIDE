using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Sets the property to the value specified. Returns success if the property was set.")]
[TaskCategory("Reflection")]
[TaskIcon("{SkinColor}ReflectionIcon.png")]
public class SetPropertyValue : Action
{
	[Tooltip("The GameObject to set the property on")]
	public SharedGameObject targetGameObject;

	[Tooltip("The component to set the property on")]
	public SharedString componentName;

	[Tooltip("The name of the property")]
	public SharedString propertyName;

	[Tooltip("The value to set")]
	public SharedVariable propertyValue;

	public override TaskStatus OnUpdate()
	{
		if (propertyValue == null)
		{
			Debug.LogWarning("Unable to get field - field value is null");
			return TaskStatus.Failure;
		}
		Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(componentName.Value);
		if (typeWithinAssembly == null)
		{
			Debug.LogWarning("Unable to set property - type is null");
			return TaskStatus.Failure;
		}
		Component component = GetDefaultGameObject(targetGameObject.Value).GetComponent(typeWithinAssembly);
		if (component == null)
		{
			Debug.LogWarning("Unable to set the property with component " + componentName.Value);
			return TaskStatus.Failure;
		}
		component.GetType().GetProperty(propertyName.Value).SetValue(component, propertyValue.GetValue(), null);
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		componentName = null;
		propertyName = null;
		propertyValue = null;
	}
}
