using System.Collections;
using NKC.Publisher;
using UnityEngine;

namespace NKC.Patcher;

public class WaitForDownloadStatus : IPatchProcessStrategy, IEnumerable
{
	public IPatchProcessStrategy.ExecutionStatus Status { get; private set; }

	public string ReasonOfFailure { get; private set; } = string.Empty;

	public IEnumerator GetEnumerator()
	{
		Status = IPatchProcessStrategy.ExecutionStatus.Success;
		ReasonOfFailure = string.Empty;
		yield return null;
		if (IsDownloading())
		{
			NKCPatchChecker.PatcherUI?.SetProgressText(NKCUtilString.GET_STRING_PATCHER_DOWNLOADING);
			NKCPatchChecker.PatcherUI?.SetActiveBackGround(NKCPatchDownloader.Instance.BackgroundDownloadAvailble);
			NKCPatchChecker.PatcherUI?.Set_lbCanDownloadBackground(NKCUtilString.GET_STRING_PATCHER_CAN_BACKGROUND_DOWNLOAD);
			while (NKCPatchDownloader.Instance.DownloadStatus == NKCPatchDownloader.PatchDownloadStatus.Downloading)
			{
				NKCPatchChecker.PatcherUI?.OnFileDownloadProgressTotal(NKCPatchDownloader.Instance.CurrentSize, NKCPatchDownloader.Instance.TotalSize);
				yield return null;
			}
			Debug.Log("PatchLoop finished, patcherStatus " + NKCPatchDownloader.Instance.DownloadStatus);
			switch (NKCPatchDownloader.Instance.DownloadStatus)
			{
			case NKCPatchDownloader.PatchDownloadStatus.Finished:
				Debug.Log("Download finished");
				NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.Patch_DownloadComplete);
				NKCPatchChecker.PatcherUI?.SetProgressText(NKCUtilString.GET_STRING_PATCHER_DOWNLOADING);
				break;
			case NKCPatchDownloader.PatchDownloadStatus.Error:
				NKCPatcherManager.GetPatcherManager().ShowError(NKCStringTable.GetString("SI_DP_PATCHER_DOWNLOAD_ERROR", NKCPatchDownloader.Instance.ErrorString));
				break;
			case NKCPatchDownloader.PatchDownloadStatus.Idle:
				NKCPatcherManager.GetPatcherManager().ShowError(NKCStringTable.GetString("SI_DP_PATCHER_DOWNLOAD_ERROR", ""));
				break;
			case NKCPatchDownloader.PatchDownloadStatus.UpdateRequired:
				NKCPatcherManager.GetPatcherManager().ShowUpdate();
				break;
			case NKCPatchDownloader.PatchDownloadStatus.UserCancel:
				NKCPatcherManager.GetPatcherManager().ShowError("User Canceled");
				break;
			}
		}
		NKCPatchChecker.PatcherUI?.Progress();
		NKCMMPManager.OnCustomEvent("03_downLoad_complete");
		NKCPatchChecker.PatcherUI?.Progress();
		NKCPatchChecker.PatcherUI?.OnFileDownloadProgressTotal(1L, 1L);
	}

	private bool IsDownloading()
	{
		if (!NKCPatchDownloader.Instance.ProloguePlay)
		{
			return NKCPatchDownloader.Instance.DownloadStatus == NKCPatchDownloader.PatchDownloadStatus.Downloading;
		}
		return false;
	}
}
