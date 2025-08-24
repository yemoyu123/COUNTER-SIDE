using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;

[TaskCategory("Unity/Vector2")]
[TaskDescription("Sets the X and Y values of the Vector2.")]
public class SetXY : Action
{
	[Tooltip("The Vector2 to set the values of")]
	public SharedVector2 vector2Variable;

	[Tooltip("The X value. Set to None to have the value ignored")]
	public SharedFloat xValue;

	[Tooltip("The Y value. Set to None to have the value ignored")]
	public SharedFloat yValue;

	public override TaskStatus OnUpdate()
	{
		Vector2 value = vector2Variable.Value;
		if (!xValue.IsNone)
		{
			value.x = xValue.Value;
		}
		if (!yValue.IsNone)
		{
			value.y = yValue.Value;
		}
		vector2Variable.Value = value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		vector2Variable = Vector2.zero;
		xValue = 0f;
		yValue = 0f;
	}
}
