using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;

[TaskCategory("Unity/GameObject")]
[TaskDescription("Returns Success if the layermasks match, otherwise Failure.")]
public class CompareLayerMask : Conditional
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The layermask to compare against")]
	public LayerMask layermask;

	public override TaskStatus OnUpdate()
	{
		if (((1 << GetDefaultGameObject(targetGameObject.Value).layer) & layermask.value) == 0)
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
	}
}
