namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("The until failure task will keep executing its child task until the child task returns failure.")]
[TaskIcon("{SkinColor}UntilFailureIcon.png")]
public class UntilFailure : Decorator
{
	private TaskStatus executionStatus;

	public override bool CanExecute()
	{
		if (executionStatus != TaskStatus.Success)
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
