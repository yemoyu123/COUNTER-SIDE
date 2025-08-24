using System.Collections.Generic;
using Cs.Logging;

namespace NKC.Patcher;

public class BasePatchInfoController
{
	private NKCPatchInfo _curPatchInfo;

	private NKCPatchInfo _latestPatchInfo;

	private NKCPatchInfo _filteredPatchInfo;

	private NKCPatchInfo _downloadHistoryPatchInfo;

	private void LoadCurrentManifest()
	{
		_curPatchInfo = PatchManifestManager.LoadManifest(PatchManifestPath.PatchType.CurrentManifest);
	}

	private void LoadLatestManifest()
	{
		_latestPatchInfo = PatchManifestManager.LoadManifest(PatchManifestPath.PatchType.LatestManifest);
	}

	public NKCPatchInfo LoadDownloadHistoryManifest()
	{
		_downloadHistoryPatchInfo = PatchManifestManager.LoadManifest(PatchManifestPath.PatchType.TempManifest);
		return _downloadHistoryPatchInfo;
	}

	public NKCPatchInfo GetCurPatchInfo()
	{
		if (_curPatchInfo == null)
		{
			LoadCurrentManifest();
		}
		return _curPatchInfo;
	}

	public NKCPatchInfo GetLatestPatchInfo()
	{
		if (_latestPatchInfo == null)
		{
			LoadLatestManifest();
		}
		return _latestPatchInfo;
	}

	public NKCPatchInfo GetDefaultDownloadHistoryPatchInfo()
	{
		if (_downloadHistoryPatchInfo == null)
		{
			LoadDownloadHistoryManifest();
		}
		return _downloadHistoryPatchInfo;
	}

	public NKCPatchInfo CreateFilteredManifestInfo(NKCPatchInfo patchInfo, List<string> lstVariants)
	{
		_filteredPatchInfo = patchInfo.FilterByVariants(lstVariants);
		return _filteredPatchInfo;
	}

	public void AppendFilteredManifestToCurrentManifest()
	{
		if (_curPatchInfo == null)
		{
			Log.Error("[OverwriteLatestManifestToCurrentManifest] curPatchInfo is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchInfoController/BasePatchInfoController.cs", 69);
			return;
		}
		if (_filteredPatchInfo == null)
		{
			Log.Error("[OverwriteLatestManifestToCurrentManifest] filteredPatchInfo is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchInfoController/BasePatchInfoController.cs", 75);
			return;
		}
		_curPatchInfo = _curPatchInfo.Append(_filteredPatchInfo);
		_curPatchInfo.SaveAsJSON(PatchManifestPath.LocalDownloadPath, "PatchInfo.json");
	}

	public bool NeedToBeUpdated(string assetBundleName)
	{
		NKCPatchInfo.PatchFileInfo latestFileInfo = GetLatestFileInfo(assetBundleName);
		if (latestFileInfo == null)
		{
			return false;
		}
		NKCPatchInfo.PatchFileInfo patchInfo = _curPatchInfo.GetPatchInfo(assetBundleName);
		if (patchInfo != null)
		{
			if (!PatchManifestManager.IsFileExist(assetBundleName))
			{
				_curPatchInfo.RemovePatchFileInfo(assetBundleName);
				return true;
			}
			return latestFileInfo.FileUpdated(patchInfo);
		}
		return true;
	}

	public NKCPatchInfo.PatchFileInfo GetLatestFileInfo(string assetBundleName)
	{
		return _latestPatchInfo.GetPatchInfo(assetBundleName);
	}

	public void CleanUp()
	{
		_curPatchInfo = null;
		_latestPatchInfo = null;
		_filteredPatchInfo = null;
		_downloadHistoryPatchInfo = null;
	}
}
