namespace BehaviorDesigner.Runtime.Tasks.Unity.Math;

[TaskCategory("Unity/Math")]
[TaskDescription("Is the int a positive value?")]
public class IsIntPositive : Conditional
{
	[Tooltip("The int to check if positive")]
	public SharedInt intVariable;

	public override TaskStatus OnUpdate()
	{
		if (intVariable.Value <= 0)
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		intVariable = 0;
	}
}
