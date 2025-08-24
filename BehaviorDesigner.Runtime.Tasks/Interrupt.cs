namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("The interrupt task will stop all child tasks from running if it is interrupted. The interruption can be triggered by the perform interruption task. The interrupt task will keep running its child until this interruption is called. If no interruption happens and the child task completed its execution the interrupt task will return the value assigned by the child task.")]
[TaskIcon("{SkinColor}InterruptIcon.png")]
public class Interrupt : Decorator
{
	private TaskStatus interruptStatus = TaskStatus.Failure;

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

	public void DoInterrupt(TaskStatus status)
	{
		interruptStatus = status;
		BehaviorManager.instance.Interrupt(base.Owner, this, status);
	}

	public override TaskStatus OverrideStatus()
	{
		return interruptStatus;
	}

	public override void OnEnd()
	{
		interruptStatus = TaskStatus.Failure;
		executionStatus = TaskStatus.Inactive;
	}
}
