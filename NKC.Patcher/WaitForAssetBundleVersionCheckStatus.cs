using System.Collections;
using NKC.Publisher;
using UnityEngine;

namespace NKC.Patcher;

public class WaitForAssetBundleVersionCheckStatus : IPatchProcessStrategy, IEnumerable
{
	public IPatchProcessStrategy.ExecutionStatus Status { get; private set; }

	public string ReasonOfFailure { get; private set; } = string.Empty;

	public IEnumerator GetEnumerator()
	{
		Status = IPatchProcessStrategy.ExecutionStatus.Success;
		ReasonOfFailure = string.Empty;
		while (NKCPatchDownloader.Instance.VersionCheckStatus == NKCPatchDownloader.VersionStatus.Unchecked)
		{
			yield return null;
		}
		NKCPatchDownloader.Instance.ProloguePlay = false;
		switch (NKCPatchDownloader.Instance.VersionCheckStatus)
		{
		case NKCPatchDownloader.VersionStatus.UpToDate:
			if (NKCDefineManager.DEFINE_SEMI_FULL_BUILD())
			{
				NKCPatchDownloader.Instance.DoWhenEndDownload();
			}
			break;
		case NKCPatchDownloader.VersionStatus.Error:
			NKCPatcherManager.GetPatcherManager().ShowError(NKCPatchDownloader.Instance.ErrorString);
			Status = IPatchProcessStrategy.ExecutionStatus.Fail;
			ReasonOfFailure = "[PatchProcess] " + NKCPatchDownloader.Instance.ErrorString;
			break;
		case NKCPatchDownloader.VersionStatus.RequireDownload:
		{
			NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.Patch_DownloadAvailable);
			float num = (float)NKCPatchDownloader.Instance.TotalSize / 1048576f;
			string message = string.Format(NKCUtilString.GET_STRING_NOTICE_DOWNLOAD_ONE_PARAM, num);
			bool m_bUserPermission = false;
			while (!m_bUserPermission)
			{
				yield return NKCPatcherManager.GetPatcherManager().WaitForOKCancel(NKCUtilString.GET_STRING_PATCHER_WARNING, message, "", "", delegate
				{
					m_bUserPermission = true;
				});
				if (!m_bUserPermission)
				{
					yield return NKCPatcherManager.GetPatcherManager().WaitForOKCancel(NKCUtilString.GET_STRING_PATCHER_WARNING, NKCStringTable.GetString("SI_DP_PATCHER_QUIT_CONFIRM"), "", "", Application.Quit);
				}
			}
			NKCMMPManager.OnCustomEvent("02_downLoad_start");
			NKCPatchDownloader.Instance.StartFileDownload();
			NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.Patch_DownloadStart);
			break;
		}
		}
	}
}
