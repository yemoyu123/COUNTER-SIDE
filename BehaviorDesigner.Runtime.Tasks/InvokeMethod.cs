using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Invokes the specified method with the specified parameters. Can optionally store the return value. Returns success if the method was invoked.")]
[TaskCategory("Reflection")]
[TaskIcon("{SkinColor}ReflectionIcon.png")]
public class InvokeMethod : Action
{
	[Tooltip("The GameObject to invoke the method on")]
	public SharedGameObject targetGameObject;

	[Tooltip("The component to invoke the method on")]
	public SharedString componentName;

	[Tooltip("The name of the method")]
	public SharedString methodName;

	[Tooltip("The first parameter of the method")]
	public SharedVariable parameter1;

	[Tooltip("The second parameter of the method")]
	public SharedVariable parameter2;

	[Tooltip("The third parameter of the method")]
	public SharedVariable parameter3;

	[Tooltip("The fourth parameter of the method")]
	public SharedVariable parameter4;

	[Tooltip("Store the result of the invoke call")]
	public SharedVariable storeResult;

	public override TaskStatus OnUpdate()
	{
		Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(componentName.Value);
		if (typeWithinAssembly == null)
		{
			Debug.LogWarning("Unable to invoke - type is null");
			return TaskStatus.Failure;
		}
		Component component = GetDefaultGameObject(targetGameObject.Value).GetComponent(typeWithinAssembly);
		if (component == null)
		{
			Debug.LogWarning("Unable to invoke method with component " + componentName.Value);
			return TaskStatus.Failure;
		}
		List<object> list = new List<object>();
		List<Type> list2 = new List<Type>();
		SharedVariable sharedVariable = null;
		for (int i = 0; i < 4 && GetType().GetField("parameter" + (i + 1)).GetValue(this) is SharedVariable sharedVariable2; i++)
		{
			list.Add(sharedVariable2.GetValue());
			list2.Add(sharedVariable2.GetType().GetProperty("Value").PropertyType);
		}
		MethodInfo method = component.GetType().GetMethod(methodName.Value, list2.ToArray());
		if (method == null)
		{
			Debug.LogWarning("Unable to invoke method " + methodName.Value + " on component " + componentName.Value);
			return TaskStatus.Failure;
		}
		object value = method.Invoke(component, list.ToArray());
		if (storeResult != null)
		{
			storeResult.SetValue(value);
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		componentName = null;
		methodName = null;
		parameter1 = null;
		parameter2 = null;
		parameter3 = null;
		parameter4 = null;
		storeResult = null;
	}
}
