using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math;

[TaskCategory("Unity/Math")]
[TaskDescription("Performs comparison between two floats: less than, less than or equal to, equal to, not equal to, greater than or equal to, or greater than.")]
public class FloatComparison : Conditional
{
	public enum Operation
	{
		LessThan,
		LessThanOrEqualTo,
		EqualTo,
		NotEqualTo,
		GreaterThanOrEqualTo,
		GreaterThan
	}

	[Tooltip("The operation to perform")]
	public Operation operation;

	[Tooltip("The first float")]
	public SharedFloat float1;

	[Tooltip("The second float")]
	public SharedFloat float2;

	public override TaskStatus OnUpdate()
	{
		switch (operation)
		{
		case Operation.LessThan:
			if (!(float1.Value < float2.Value))
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		case Operation.LessThanOrEqualTo:
			if (!(float1.Value <= float2.Value))
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		case Operation.EqualTo:
			if (!Mathf.Approximately(float1.Value, float2.Value))
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		case Operation.NotEqualTo:
			if (Mathf.Approximately(float1.Value, float2.Value))
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		case Operation.GreaterThanOrEqualTo:
			if (!(float1.Value >= float2.Value))
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		case Operation.GreaterThan:
			if (!(float1.Value > float2.Value))
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		default:
			return TaskStatus.Failure;
		}
	}

	public override void OnReset()
	{
		operation = Operation.LessThan;
		float1.Value = 0f;
		float2.Value = 0f;
	}
}
