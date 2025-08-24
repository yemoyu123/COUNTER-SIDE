namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("The until success task will keep executing its child task until the child task returns success.")]
[TaskIcon("{SkinColor}UntilSuccessIcon.png")]
public class UntilSuccess : Decorator
{
	private TaskStatus executionStatus;

	public override bool CanExecute()
	{
		if (executionStatus != TaskStatus.Failure)
		{
			return executionStatus == TaskStatus.Inactive;
		}
		return true;
	}

	public override void OnChildExecuted(TaskStatus childStatus)
	{
		executionStatus = childStatus;
	}

	public override void OnEnd()
	{
		executionStatus = TaskStatus.Inactive;
	}
}
