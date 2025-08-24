using Cs.Logging;

namespace NKC.Patcher;

public class ExtraPatchInfoController
{
	private NKCPatchInfo _curExtraPatchInfo;

	private NKCPatchInfo _latestExtraPatchInfo;

	private NKCPatchInfo _downloadHistoryExtraPatchInfo;

	public NKCPatchInfo LoadCurrentExtraManifest()
	{
		_curExtraPatchInfo = PatchManifestManager.LoadManifest(PatchManifestPath.PatchType.CurrentExtraManifestPath);
		return _curExtraPatchInfo;
	}

	public NKCPatchInfo LoadExtraLatestManifest()
	{
		_latestExtraPatchInfo = PatchManifestManager.LoadManifest(PatchManifestPath.PatchType.LatestExtraManifest);
		return _latestExtraPatchInfo;
	}

	public NKCPatchInfo GetCurrentExtraPatchInfo()
	{
		if (_curExtraPatchInfo == null)
		{
			LoadCurrentExtraManifest();
		}
		return _curExtraPatchInfo;
	}

	public NKCPatchInfo GetLatestExtraPatchInfo()
	{
		if (_latestExtraPatchInfo == null)
		{
			LoadExtraLatestManifest();
		}
		return _latestExtraPatchInfo;
	}

	public NKCPatchInfo LoadDownloadHistoryExtraManifest()
	{
		_downloadHistoryExtraPatchInfo = PatchManifestManager.LoadManifest(PatchManifestPath.PatchType.TempExtraManifest);
		return _downloadHistoryExtraPatchInfo;
	}

	public NKCPatchInfo GetDownloadHistoryExtraPatchInfo()
	{
		if (_downloadHistoryExtraPatchInfo == null)
		{
			LoadDownloadHistoryExtraManifest();
		}
		return _downloadHistoryExtraPatchInfo;
	}

	public void AppendFilteredManifestToCurrentManifest()
	{
		if (_curExtraPatchInfo == null)
		{
			Log.Debug("[OverwriteLatestManifestToCurrentManifest] curPatchInfo is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchInfoController/ExtraPatchInfoController.cs", 62);
			return;
		}
		if (_latestExtraPatchInfo == null)
		{
			Log.Debug("[OverwriteLatestManifestToCurrentManifest] latestExtraPatchInfo is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchInfoController/ExtraPatchInfoController.cs", 68);
			return;
		}
		_curExtraPatchInfo = _curExtraPatchInfo.Append(_latestExtraPatchInfo);
		_curExtraPatchInfo.SaveAsJSON(PatchManifestPath.ExtraLocalDownloadPath, "PatchInfo.json");
	}

	public void CleanUp()
	{
		_curExtraPatchInfo = null;
		_latestExtraPatchInfo = null;
		_downloadHistoryExtraPatchInfo = null;
	}
}
