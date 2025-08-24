namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;

[TaskCategory("Unity/GameObject")]
[TaskDescription("Returns Success if tags match, otherwise Failure.")]
public class CompareTag : Conditional
{
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	public SharedGameObject targetGameObject;

	[Tooltip("The tag to compare against")]
	public SharedString tag;

	public override TaskStatus OnUpdate()
	{
		if (!GetDefaultGameObject(targetGameObject.Value).CompareTag(tag.Value))
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetGameObject = null;
		tag = "";
	}
}
