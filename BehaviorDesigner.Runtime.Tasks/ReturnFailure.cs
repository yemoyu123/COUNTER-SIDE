namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("The return failure task will always return failure except when the child task is running.")]
[TaskIcon("{SkinColor}ReturnFailureIcon.png")]
public class ReturnFailure : Decorator
{
	private TaskStatus executionStatus;

	public override bool CanExecute()
	{
		if (executionStatus != TaskStatus.Inactive)
		{
			return executionStatus == TaskStatus.Running;
		}
		return true;
	}

	public override void OnChildExecuted(TaskStatus childStatus)
	{
		executionStatus = childStatus;
	}

	public override TaskStatus Decorate(TaskStatus status)
	{
		if (status == TaskStatus.Success)
		{
			return TaskStatus.Failure;
		}
		return status;
	}

	public override void OnEnd()
	{
		executionStatus = TaskStatus.Inactive;
	}
}
