namespace BehaviorDesigner.Runtime.Tasks.Unity.Math;

[TaskCategory("Unity/Math")]
[TaskDescription("Performs a comparison between two bools.")]
public class BoolComparison : Conditional
{
	[Tooltip("The first bool")]
	public SharedBool bool1;

	[Tooltip("The second bool")]
	public SharedBool bool2;

	public override TaskStatus OnUpdate()
	{
		if (bool1.Value != bool2.Value)
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		bool1 = false;
		bool2 = false;
	}
}
