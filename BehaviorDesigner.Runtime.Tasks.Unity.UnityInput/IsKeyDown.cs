using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;

[TaskCategory("Unity/Input")]
[TaskDescription("Returns success when the specified key is pressed.")]
public class IsKeyDown : Conditional
{
	[Tooltip("The key to test")]
	public KeyCode key;

	public override TaskStatus OnUpdate()
	{
		if (!Input.GetKeyDown(key))
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		key = KeyCode.None;
	}
}
