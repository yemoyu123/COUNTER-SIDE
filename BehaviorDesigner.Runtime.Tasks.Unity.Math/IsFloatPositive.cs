namespace BehaviorDesigner.Runtime.Tasks.Unity.Math;

[TaskCategory("Unity/Math")]
[TaskDescription("Is the float a positive value?")]
public class IsFloatPositive : Conditional
{
	[Tooltip("The float to check if positive")]
	public SharedFloat floatVariable;

	public override TaskStatus OnUpdate()
	{
		if (!(floatVariable.Value > 0f))
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		floatVariable = 0f;
	}
}
