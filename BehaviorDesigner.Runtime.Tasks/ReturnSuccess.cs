namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("The return success task will always return success except when the child task is running.")]
[TaskIcon("{SkinColor}ReturnSuccessIcon.png")]
public class ReturnSuccess : Decorator
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
		if (status == TaskStatus.Failure)
		{
			return TaskStatus.Success;
		}
		return status;
	}

	public override void OnEnd()
	{
		executionStatus = TaskStatus.Inactive;
	}
}
