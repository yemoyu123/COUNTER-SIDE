using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3;

[TaskCategory("Unity/Vector3")]
[TaskDescription("Sets the X, Y, and Z values of the Vector3.")]
public class SetXYZ : Action
{
	[Tooltip("The Vector3 to set the values of")]
	public SharedVector3 vector3Variable;

	[Tooltip("The X value. Set to None to have the value ignored")]
	public SharedFloat xValue;

	[Tooltip("The Y value. Set to None to have the value ignored")]
	public SharedFloat yValue;

	[Tooltip("The Z value. Set to None to have the value ignored")]
	public SharedFloat zValue;

	public override TaskStatus OnUpdate()
	{
		Vector3 value = vector3Variable.Value;
		if (!xValue.IsNone)
		{
			value.x = xValue.Value;
		}
		if (!yValue.IsNone)
		{
			value.y = yValue.Value;
		}
		if (!zValue.IsNone)
		{
			value.z = zValue.Value;
		}
		vector3Variable.Value = value;
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		vector3Variable = Vector3.zero;
		xValue = (yValue = (zValue = 0f));
	}
}
