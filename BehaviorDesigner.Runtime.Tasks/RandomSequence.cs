using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Similar to the sequence task, the random sequence task will return success as soon as every child task returns success.  The difference is that the random sequence class will run its children in a random order. The sequence task is deterministic in that it will always run the tasks from left to right within the tree. The random sequence task shuffles the child tasks up and then begins execution in a random order. Other than that the random sequence class is the same as the sequence class. It will stop running tasks as soon as a single task ends in failure. On a task failure it will stop executing all of the child tasks and return failure. If no child returns failure then it will return success.")]
[TaskIcon("{SkinColor}RandomSequenceIcon.png")]
public class RandomSequence : Composite
{
	[Tooltip("Seed the random number generator to make things easier to debug")]
	public int seed;

	[Tooltip("Do we want to use the seed?")]
	public bool useSeed;

	private List<int> childIndexList = new List<int>();

	private Stack<int> childrenExecutionOrder = new Stack<int>();

	private TaskStatus executionStatus;

	public override void OnAwake()
	{
		if (useSeed)
		{
			Random.InitState(seed);
		}
		childIndexList.Clear();
		for (int i = 0; i < children.Count; i++)
		{
			childIndexList.Add(i);
		}
	}

	public override void OnStart()
	{
		ShuffleChilden();
	}

	public override int CurrentChildIndex()
	{
		return childrenExecutionOrder.Peek();
	}

	public override bool CanExecute()
	{
		if (childrenExecutionOrder.Count > 0)
		{
			return executionStatus != TaskStatus.Failure;
		}
		return false;
	}

	public override void OnChildExecuted(TaskStatus childStatus)
	{
		if (childrenExecutionOrder.Count > 0)
		{
			childrenExecutionOrder.Pop();
		}
		executionStatus = childStatus;
	}

	public override void OnConditionalAbort(int childIndex)
	{
		childrenExecutionOrder.Clear();
		executionStatus = TaskStatus.Inactive;
		ShuffleChilden();
	}

	public override void OnEnd()
	{
		executionStatus = TaskStatus.Inactive;
		childrenExecutionOrder.Clear();
	}

	public override void OnReset()
	{
		seed = 0;
		useSeed = false;
	}

	private void ShuffleChilden()
	{
		for (int num = childIndexList.Count; num > 0; num--)
		{
			int index = Random.Range(0, num);
			int num2 = childIndexList[index];
			childrenExecutionOrder.Push(num2);
			childIndexList[index] = childIndexList[num - 1];
			childIndexList[num - 1] = num2;
		}
	}
}
