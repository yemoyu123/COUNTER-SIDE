using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using NKC.Publisher;
using UnityEngine;

namespace NKC.Patcher;

public class WaitForAppVersionCheckStatus : IPatchProcessStrategy, IEnumerable
{
	public IPatchProcessStrategy.ExecutionStatus Status { get; private set; }

	public string ReasonOfFailure { get; private set; } = string.Empty;

	public IEnumerator GetEnumerator()
	{
		Status = IPatchProcessStrategy.ExecutionStatus.Success;
		ReasonOfFailure = string.Empty;
		if (PlayerPrefsContainer.GetBoolean("PatchIntegrityCheck"))
		{
			NKCPatchChecker.PatcherUI?.SetProgressText(NKCStringTable.GetString("SI_DP_PATCHER_INTEGRITY_CHECK"));
		}
		NKCPatchChecker.PatcherUI?.Progress();
		Debug.Log("IPatchProcessStrategy Begin WaitForVersionCheckStatus");
		NKCPatchDownloader.Instance.InitCheckTime();
		NKCPatchDownloader.Instance.CheckVersion(new List<string>(AssetBundleManager.ActiveVariants), PlayerPrefsContainer.GetBoolean("PatchIntegrityCheck"));
		PlayerPrefsContainer.Set("PatchIntegrityCheck", value: false);
		while (NKCPatchDownloader.Instance.BuildCheckStatus == NKCPatchDownloader.BuildStatus.Unchecked)
		{
			yield return null;
		}
		switch (NKCPatchDownloader.Instance.BuildCheckStatus)
		{
		case NKCPatchDownloader.BuildStatus.Error:
			NKCPatcherManager.GetPatcherManager().ShowError(NKCPatchDownloader.Instance.ErrorString);
			Status = IPatchProcessStrategy.ExecutionStatus.Fail;
			ReasonOfFailure = "[PatchProcess] " + NKCPatchDownloader.Instance.ErrorString;
			yield break;
		case NKCPatchDownloader.BuildStatus.UpdateAvailable:
			yield return NKCPatcherManager.GetPatcherManager().WaitForOKCancel(NKCUtilString.GET_STRING_PATCHER_NOTICE, NKCUtilString.GET_STRING_PATCHER_CAN_UPDATE, NKCUtilString.GET_STRING_PATCHER_MOVE_TO_MARKET, NKCUtilString.GET_STRING_PATCHER_CONTINUE, NKCPatcherManager.GetPatcherManager().MoveToMarket);
			break;
		case NKCPatchDownloader.BuildStatus.RequireAppUpdate:
			NKCPatcherManager.GetPatcherManager().ShowUpdate();
			break;
		}
		NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.Patch_VersionCheckComplete);
		NKCPatchChecker.PatcherUI?.Progress();
	}
}
