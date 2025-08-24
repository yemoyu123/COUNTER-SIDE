using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables;

[TaskCategory("Unity/SharedVariable")]
[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
public class CompareSharedVector4 : Conditional
{
	[Tooltip("The first variable to compare")]
	public SharedVector4 variable;

	[Tooltip("The variable to compare to")]
	public SharedVector4 compareTo;

	public override TaskStatus OnUpdate()
	{
		if (!variable.Value.Equals(compareTo.Value))
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		variable = Vector4.zero;
		compareTo = Vector4.zero;
	}
}
