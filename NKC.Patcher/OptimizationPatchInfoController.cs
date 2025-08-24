using System.Collections.Generic;
using AssetBundles;

namespace NKC.Patcher;

public class OptimizationPatchInfoController
{
	private NKCPatchInfo _tutorialPatchInfo;

	private NKCPatchInfo _nonEssentialPatchInfo;

	private NKCPatchInfo _backgroundDownloadHistoryPatchInfo;

	public NKCPatchInfo CreateTutorialOnlyManifestInfo(NKCPatchInfo patchInfo)
	{
		_tutorialPatchInfo = patchInfo.GetClone();
		List<string> list = AssetBundleManager.LoadSavedAssetBundleLists(PatchManifestPath.TutorialPatchFileName);
		if (list.Count == 0)
		{
			_tutorialPatchInfo = null;
			return null;
		}
		list.Add(Utility.GetPlatformName().ToLower());
		foreach (string key in _tutorialPatchInfo.m_dicPatchInfo.Keys)
		{
			string text = key.Split('.')[0].ToLower();
			if (text.Contains("ab_script"))
			{
				list.Add(text);
			}
			else if (text.Contains("ab_ui_nkm_ui_popup_enemy_sprite"))
			{
				list.Add(text);
			}
			else if (text.Contains("login"))
			{
				list.Add(text);
			}
			else if (text.Contains("ab_music/cutscen"))
			{
				list.Add(text);
			}
			else if (text.Contains("tutorial"))
			{
				list.Add(text);
			}
			else if (text.Contains("ab_sound_fx"))
			{
				list.Add(text);
			}
		}
		_tutorialPatchInfo = _tutorialPatchInfo.MakePatchinfoSubset(list);
		return _tutorialPatchInfo;
	}

	public NKCPatchInfo LoadBackgroundDownloadHistoryPatchInfo()
	{
		_backgroundDownloadHistoryPatchInfo = PatchManifestManager.LoadManifest(PatchManifestPath.PatchType.BackgroundDownloadHistoryManifest);
		return _backgroundDownloadHistoryPatchInfo;
	}

	public NKCPatchInfo GetBackgroundDownloadHistoryPatchInfo()
	{
		if (_backgroundDownloadHistoryPatchInfo == null)
		{
			LoadBackgroundDownloadHistoryPatchInfo();
		}
		return _backgroundDownloadHistoryPatchInfo;
	}

	public void CleanUp()
	{
		_tutorialPatchInfo = null;
		_backgroundDownloadHistoryPatchInfo = null;
	}
}
