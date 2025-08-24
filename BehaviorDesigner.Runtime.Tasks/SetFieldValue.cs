using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Sets the field to the value specified. Returns success if the field was set.")]
[TaskCategory("Reflection")]
[TaskIcon("{SkinColor}ReflectionIcon.png")]
public class SetFieldValue : Action
{
	[Tooltip("The GameObject to set the field on")]
	public SharedGameObject targetGameObject;

	[Tooltip("The component to set the field on")]
	public SharedString componentName;

	[Tooltip("The name of the field")]
	public SharedString fieldName;

	[Tooltip("The value to set")]
	public SharedVariable fieldValue;

	public override TaskStatus OnUpdate()
	{
		if (fieldValue == null)
		{
			Debug.LogWarning("Unable to get field - field value is null");
			return TaskStatus.Failure;
		}
		Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(componentName.Value);
		if (typeWithinAssembly == null)
		{
			Debug.LogWarning("Unable to set field - type is null");
			return TaskStatus.Failure;
		}
		Component component = GetDefaultGameObject(targetGameObject.Value).GetComponent(typeWithinAssembly);
		if (component == null)
		{
			Debug.LogWarning("Unable to set the field with component " + componentName.Value);
			return TaskStatus.Failure;
		}
		component.GetType().GetField(fieldName.Value).SetValue(component, fieldValue.GetValue());
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		componentName = null;
		fieldName = null;
		fieldValue = null;
	}
}
