using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("The random probability task will return success when the random probability is below the succeed probability. It will otherwise return failure.")]
public class RandomProbability : Conditional
{
	[Tooltip("The chance that the task will return success")]
	public SharedFloat successProbability = 0.5f;

	[Tooltip("Seed the random number generator to make things easier to debug")]
	public SharedInt seed;

	[Tooltip("Do we want to use the seed?")]
	public SharedBool useSeed;

	public override void OnAwake()
	{
		if (useSeed.Value)
		{
			Random.InitState(seed.Value);
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (Random.value < successProbability.Value)
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}

	public override void OnReset()
	{
		successProbability = 0.5f;
		seed = 0;
		useSeed = false;
	}
}
