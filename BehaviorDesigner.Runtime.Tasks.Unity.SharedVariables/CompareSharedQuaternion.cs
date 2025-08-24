using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables;

[TaskCategory("Unity/SharedVariable")]
[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
public class CompareSharedQuaternion : Conditional
{
	[Tooltip("The first variable to compare")]
	public SharedQuaternion variable;

	[Tooltip("The variable to compare to")]
	public SharedQuaternion compareTo;

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
		variable = Quaternion.identity;
		compareTo = Quaternion.identity;
	}
}
