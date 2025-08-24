using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Gets the value from the property specified. Returns success if the property was retrieved.")]
[TaskCategory("Reflection")]
[TaskIcon("{SkinColor}ReflectionIcon.png")]
public class GetPropertyValue : Action
{
	[Tooltip("The GameObject to get the property of")]
	public SharedGameObject targetGameObject;

	[Tooltip("The component to get the property of")]
	public SharedString componentName;

	[Tooltip("The name of the property")]
	public SharedString propertyName;

	[Tooltip("The value of the property")]
	[RequiredField]
	public SharedVariable propertyValue;

	public override TaskStatus OnUpdate()
	{
		if (propertyValue == null)
		{
			Debug.LogWarning("Unable to get property - property value is null");
			return TaskStatus.Failure;
		}
		Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(componentName.Value);
		if (typeWithinAssembly == null)
		{
			Debug.LogWarning("Unable to get property - type is null");
			return TaskStatus.Failure;
		}
		Component component = GetDefaultGameObject(targetGameObject.Value).GetComponent(typeWithinAssembly);
		if (component == null)
		{
			Debug.LogWarning("Unable to get the property with component " + componentName.Value);
			return TaskStatus.Failure;
		}
		PropertyInfo property = component.GetType().GetProperty(propertyName.Value);
		propertyValue.SetValue(property.GetValue(component, null));
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
