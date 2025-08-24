using System.Collections;
using UnityEngine;

namespace NKC.Patcher;

public class WaitForInternetConnection : IPatchProcessStrategy, IEnumerable
{
	public IPatchProcessStrategy.ExecutionStatus Status { get; private set; }

	public string ReasonOfFailure { get; private set; } = string.Empty;

	public IEnumerator GetEnumerator()
	{
		Status = IPatchProcessStrategy.ExecutionStatus.Success;
		ReasonOfFailure = string.Empty;
		NKCPatchChecker.PatcherUI?.SetProgressText(NKCUtilString.GET_STRING_PATCHER_CHECKING_VERSION_INFORMATION);
		NKCPatchChecker.PatcherUI?.Progress();
		while (Application.internetReachability == NetworkReachability.NotReachable)
		{
			yield return NKCPatcherManager.GetPatcherManager().WaitForOKBox(NKCUtilString.GET_STRING_PATCHER_WARNING, NKCUtilString.GET_STRING_DECONNECT_INTERNET, NKCUtilString.GET_STRING_RETRY);
		}
	}
}
