using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Compares the property value to the value specified. Returns success if the values are the same.")]
[TaskCategory("Reflection")]
[TaskIcon("{SkinColor}ReflectionIcon.png")]
public class ComparePropertyValue : Conditional
{
	[Tooltip("The GameObject to compare the property of")]
	public SharedGameObject targetGameObject;

	[Tooltip("The component to compare the property of")]
	public SharedString componentName;

	[Tooltip("The name of the property")]
	public SharedString propertyName;

	[Tooltip("The value to compare to")]
	public SharedVariable compareValue;

	public override TaskStatus OnUpdate()
	{
		if (compareValue == null)
		{
			Debug.LogWarning("Unable to compare field - compare value is null");
			return TaskStatus.Failure;
		}
		Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(componentName.Value);
		if (typeWithinAssembly == null)
		{
			Debug.LogWarning("Unable to compare property - type is null");
			return TaskStatus.Failure;
		}
		Component component = GetDefaultGameObject(targetGameObject.Value).GetComponent(typeWithinAssembly);
		if (component == null)
		{
			Debug.LogWarning("Unable to compare the property with component " + componentName.Value);
			return TaskStatus.Failure;
		}
		object value = component.GetType().GetProperty(propertyName.Value).GetValue(component, null);
		if (value == null && compareValue.GetValue() == null)
		{
			return TaskStatus.Success;
		}
		if (!value.Equals(compareValue.GetValue()))
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		componentName = null;
		propertyName = null;
		compareValue = null;
	}
}
