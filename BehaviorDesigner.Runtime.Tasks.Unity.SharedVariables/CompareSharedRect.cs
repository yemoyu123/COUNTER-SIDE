using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables;

[TaskCategory("Unity/SharedVariable")]
[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
public class CompareSharedRect : Conditional
{
	[Tooltip("The first variable to compare")]
	public SharedRect variable;

	[Tooltip("The variable to compare to")]
	public SharedRect compareTo;

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
		variable = default(Rect);
		compareTo = default(Rect);
	}
}
