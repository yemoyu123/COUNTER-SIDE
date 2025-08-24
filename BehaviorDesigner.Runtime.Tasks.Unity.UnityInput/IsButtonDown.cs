using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;

[TaskCategory("Unity/Input")]
[TaskDescription("Returns success when the specified button is pressed.")]
public class IsButtonDown : Conditional
{
	[Tooltip("The name of the button")]
	public SharedString buttonName;

	public override TaskStatus OnUpdate()
	{
		if (!Input.GetButtonDown(buttonName.Value))
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		buttonName = "Fire1";
	}
}
