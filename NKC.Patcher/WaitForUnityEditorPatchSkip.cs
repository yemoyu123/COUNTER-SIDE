using System.Collections;

namespace NKC.Patcher;

public class WaitForUnityEditorPatchSkip : IPatchProcessStrategy, IEnumerable
{
	public IPatchProcessStrategy.ExecutionStatus Status { get; private set; }

	public string ReasonOfFailure { get; private set; } = string.Empty;

	public IEnumerator GetEnumerator()
	{
		Status = IPatchProcessStrategy.ExecutionStatus.Success;
		ReasonOfFailure = string.Empty;
		yield break;
	}
}
