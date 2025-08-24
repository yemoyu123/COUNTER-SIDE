using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;

[TaskCategory("Unity/Input")]
[TaskDescription("Returns success when the specified mouse button is pressed.")]
public class IsMouseUp : Conditional
{
	[Tooltip("The button index")]
	public SharedInt buttonIndex;

	public override TaskStatus OnUpdate()
	{
		if (!Input.GetMouseButtonUp(buttonIndex.Value))
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		buttonIndex = 0;
	}
}
