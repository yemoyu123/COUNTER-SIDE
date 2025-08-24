using System.Collections;

namespace NKC.Patcher;

public interface IPatchProcessStrategy : IEnumerable
{
	public enum ExecutionStatus
	{
		Success,
		Fail,
		SkipNextProcess
	}

	ExecutionStatus Status { get; }

	string ReasonOfFailure { get; }
}
