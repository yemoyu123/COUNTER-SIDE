using System.Collections;

namespace NKC.Patcher;

public class WaitForTouch : IPatchProcessStrategy, IEnumerable
{
	public IPatchProcessStrategy.ExecutionStatus Status { get; private set; }

	public string ReasonOfFailure { get; private set; } = string.Empty;

	private bool IsNullBackGroundText()
	{
		if (NKCPatchChecker.PatcherUI != null)
		{
			return NKCPatchChecker.PatcherUI.BackGroundTextIsNull();
		}
		return false;
	}

	public IEnumerator GetEnumerator()
	{
		Status = IPatchProcessStrategy.ExecutionStatus.Success;
		ReasonOfFailure = string.Empty;
		if (!NKCPatchDownloader.Instance.ProloguePlay && IsDownloading() && !IsNullBackGroundText())
		{
			NKCPatchChecker.PatcherUI?.SetForTouchWait();
			yield return NKCPatchChecker.PatcherUI.WaitForTouch();
		}
		NKCPatchChecker.PatcherUI?.SetProgressText(NKCUtilString.GET_STRING_PATCHER_INITIALIZING);
		NKCPatchChecker.PatcherUI?.Progress();
		yield return null;
		static bool IsDownloading()
		{
			return NKCPatchDownloader.Instance.DownloadStatus == NKCPatchDownloader.PatchDownloadStatus.Finished;
		}
	}
}
