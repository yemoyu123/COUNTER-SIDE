namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Similar to the parallel selector task, except the parallel complete task will return the child status as soon as the child returns success or failure.The child tasks are executed simultaneously.")]
[TaskIcon("{SkinColor}ParallelCompleteIcon.png")]
public class ParallelComplete : Composite
{
	private int currentChildIndex;

	private TaskStatus[] executionStatus;

	public override void OnAwake()
	{
		executionStatus = new TaskStatus[children.Count];
	}

	public override void OnChildStarted(int childIndex)
	{
		currentChildIndex++;
		executionStatus[childIndex] = TaskStatus.Running;
	}

	public override bool CanRunParallelChildren()
	{
		return true;
	}

	public override int CurrentChildIndex()
	{
		return currentChildIndex;
	}

	public override bool CanExecute()
	{
		return currentChildIndex < children.Count;
	}

	public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
	{
		executionStatus[childIndex] = childStatus;
	}

	public override void OnConditionalAbort(int childIndex)
	{
		currentChildIndex = 0;
		for (int i = 0; i < executionStatus.Length; i++)
		{
			executionStatus[i] = TaskStatus.Inactive;
		}
	}

	public override TaskStatus OverrideStatus(TaskStatus status)
	{
		if (currentChildIndex == 0)
		{
			return TaskStatus.Success;
		}
		for (int i = 0; i < currentChildIndex; i++)
		{
			if (executionStatus[i] == TaskStatus.Success || executionStatus[i] == TaskStatus.Failure)
			{
				return executionStatus[i];
			}
		}
		return TaskStatus.Running;
	}

	public override void OnEnd()
	{
		for (int i = 0; i < executionStatus.Length; i++)
		{
			executionStatus[i] = TaskStatus.Inactive;
		}
		currentChildIndex = 0;
	}
}
