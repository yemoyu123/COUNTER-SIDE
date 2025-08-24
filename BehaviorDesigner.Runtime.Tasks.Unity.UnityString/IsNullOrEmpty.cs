namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityString;

[TaskCategory("Unity/String")]
[TaskDescription("Returns success if the string is null or empty")]
public class IsNullOrEmpty : Conditional
{
	[Tooltip("The target string")]
	public SharedString targetString;

	public override TaskStatus OnUpdate()
	{
		if (!string.IsNullOrEmpty(targetString.Value))
		{
			return TaskStatus.Failure;
		}
		return TaskStatus.Success;
	}

	public override void OnReset()
	{
		targetString = "";
	}
}
