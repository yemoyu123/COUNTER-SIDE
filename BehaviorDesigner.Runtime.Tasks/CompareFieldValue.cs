using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Compares the field value to the value specified. Returns success if the values are the same.")]
[TaskCategory("Reflection")]
[TaskIcon("{SkinColor}ReflectionIcon.png")]
public class CompareFieldValue : Conditional
{
	[Tooltip("The GameObject to compare the field on")]
	public SharedGameObject targetGameObject;

	[Tooltip("The component to compare the field on")]
	public SharedString componentName;

	[Tooltip("The name of the field")]
	public SharedString fieldName;

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
			Debug.LogWarning("Unable to compare field - type is null");
			return TaskStatus.Failure;
		}
		Component component = GetDefaultGameObject(targetGameObject.Value).GetComponent(typeWithinAssembly);
		if (component == null)
		{
			Debug.LogWarning("Unable to compare the field with component " + componentName.Value);
			return TaskStatus.Failure;
		}
		object value = component.GetType().GetField(fieldName.Value).GetValue(component);
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
		fieldName = null;
		compareValue = null;
	}
}
