using System.Collections;
using System.Collections.Generic;
using System.IO;
using AssetBundles;
using Cs.Logging;

namespace NKC.Patcher;

public static class NKCPatchManifestManager
{
	public const string manifestFileName = "PatchInfo.json";

	public const string LatestManifestFileName = "LatestPatchInfo.json";

	public const string LatestExtraManifestFileName = "LatestExtraPatchInfo.json";

	public const string tempManifestFileName = "TempPatchInfo.json";

	public const string filteredManifestFileName = "FilteredPatchInfo.json";

	public const string NonEssentialTempManifestFileName = "NonEssentialTempPatchInfo.json";

	public static NKCPatchInfo m_latestManifest;

	public static NKCPatchInfo m_currentManifest;

	public static NKCPatchInfo m_prevDownloadedManifest;

	public static NKCPatchInfo m_filteredLatestManifest;

	public static NKCPatchInfo m_latestExtraAssetManifest;

	public static NKCPatchInfo m_currentExtraAssetManifest;

	public static NKCPatchInfo m_prevDownloadedExtraAssetManifest;

	public static List<NKCPatchInfo.PatchFileInfo> m_requiredDownloadList = new List<NKCPatchInfo.PatchFileInfo>();

	public static List<NKCPatchInfo.PatchFileInfo> m_requiredExtraAssetDownloadList = new List<NKCPatchInfo.PatchFileInfo>();

	public static List<NKCPatchInfo.PatchFileInfo> m_requiredForegroundDownloadList = new List<NKCPatchInfo.PatchFileInfo>();

	public static List<NKCPatchInfo.PatchFileInfo> m_requiredBackgroundDownloadList = new List<NKCPatchInfo.PatchFileInfo>();

	public static IEnumerator MakeDownloadListForTutorialAsset()
	{
		yield return null;
	}

	public static string GetLatestManifestPath()
	{
		return Path.Combine(AssetBundleManager.GetLocalDownloadPath(), "LatestPatchInfo.json");
	}

	public static string GetCurrentManifestPath()
	{
		string text = Path.Combine(AssetBundleManager.GetLocalDownloadPath(), "PatchInfo.json");
		if (NKCPatchUtility.IsFileExists(text))
		{
			return text;
		}
		string innerAssetPath = NKCPatchUtility.GetInnerAssetPath("PatchInfo.json");
		if (NKCPatchUtility.IsFileExists(innerAssetPath))
		{
			return innerAssetPath;
		}
		return text;
	}

	public static string GetInnerManifestPath()
	{
		return NKCPatchUtility.GetInnerAssetPath("PatchInfo.json");
	}

	public static string GetTempManifestPath()
	{
		return Path.Combine(AssetBundleManager.GetLocalDownloadPath(), "TempPatchInfo.json");
	}

	public static string GetNonEssentialManifestPath()
	{
		return Path.Combine(AssetBundleManager.GetLocalDownloadPath(), "NonEssentialTempPatchInfo.json");
	}

	public static string GetCurrentExtraAssetManifestPath()
	{
		return Path.Combine(NKCUtil.GetExtraDownloadPath(), "PatchInfo.json");
	}

	public static string GetLatestExtraAssetManifestPath()
	{
		return Path.Combine(NKCUtil.GetExtraDownloadPath(), "LatestExtraPatchInfo.json");
	}

	public static string GetDownloadedExtraAssetManifestPath()
	{
		return Path.Combine(NKCUtil.GetExtraDownloadPath(), "TempPatchInfo.json");
	}

	public static int GetBackgroundRequiredDownloadCount()
	{
		if (m_requiredBackgroundDownloadList == null)
		{
			return 0;
		}
		return m_requiredBackgroundDownloadList.Count;
	}

	public static IEnumerator MakeDownloadListForExtraAsset()
	{
		Log.Debug("[MakeDownloadListForExtraAsset] Start", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManager/NKCPatchManifestManager.cs", 110);
		m_latestExtraAssetManifest = NKCPatchInfo.LoadFromJSON(GetLatestExtraAssetManifestPath());
		m_currentExtraAssetManifest = NKCPatchInfo.LoadFromJSON(GetCurrentExtraAssetManifestPath());
		m_prevDownloadedExtraAssetManifest = NKCPatchInfo.LoadFromJSON(GetDownloadedExtraAssetManifestPath());
		if (!m_prevDownloadedExtraAssetManifest.IsEmpty())
		{
			m_currentExtraAssetManifest.Append(m_prevDownloadedExtraAssetManifest);
		}
		bool calculateMD = false;
		foreach (KeyValuePair<string, NKCPatchInfo.PatchFileInfo> item in m_latestExtraAssetManifest.m_dicPatchInfo)
		{
			NKCPatchInfo.PatchFileInfo value = item.Value;
			NKCPatchInfo.PatchFileInfo patchInfo = m_currentExtraAssetManifest.GetPatchInfo(item.Key);
			if (value == null)
			{
				Log.Error("[MakeDownloadList] Invalid Latest PatchInfo key[" + item.Key + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManager/NKCPatchManifestManager.cs", 137);
				continue;
			}
			string filePath = Path.Combine(NKCUtil.GetExtraDownloadPath(), value.FileName);
			if (patchInfo != null && patchInfo.FileUpdated(value) && !CompareFileInInPath(patchInfo, filePath, calculateMD))
			{
				AddRequiredDownloadList(m_requiredExtraAssetDownloadList, value);
			}
		}
		yield return null;
	}

	public static IEnumerator MakeDownloadList()
	{
		Log.Debug("[MakeDownloadList] Start", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManager/NKCPatchManifestManager.cs", 162);
		List<string> lstVariants = new List<string>(AssetBundleManager.ActiveVariants);
		m_latestManifest = NKCPatchInfo.LoadFromJSON(GetLatestManifestPath());
		m_currentManifest = NKCPatchInfo.LoadFromJSON(GetCurrentManifestPath());
		m_prevDownloadedManifest = NKCPatchInfo.LoadFromJSON(GetTempManifestPath());
		m_filteredLatestManifest = m_latestManifest.FilterByVariants(lstVariants);
		if (!m_prevDownloadedManifest.IsEmpty())
		{
			m_currentManifest.Append(m_prevDownloadedManifest);
		}
		m_currentManifest.IsEmpty();
		if (NKCDefineManager.DEFINE_OBB())
		{
			_ = NKCObbUtil.s_bLoadedOBB;
		}
		bool bFullIntegrityCheck = false;
		_ = m_filteredLatestManifest.m_dicPatchInfo.Count;
		int currentCount = 0;
		foreach (KeyValuePair<string, NKCPatchInfo.PatchFileInfo> kvPair in m_filteredLatestManifest.m_dicPatchInfo)
		{
			currentCount++;
			if (bFullIntegrityCheck)
			{
				yield return null;
			}
			NKCPatchInfo.PatchFileInfo value = kvPair.Value;
			NKCPatchInfo.PatchFileInfo patchInfo = m_currentManifest.GetPatchInfo(kvPair.Key);
			if (value == null)
			{
				Log.Error("[MakeDownloadList] Invalid Latest PatchInfo key[" + kvPair.Key + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManager/NKCPatchManifestManager.cs", 212);
				continue;
			}
			string innerAssetPath = NKCPatchUtility.GetInnerAssetPath(value.FileName);
			string filePath = Path.Combine(AssetBundleManager.GetLocalDownloadPath(), value.FileName);
			if (patchInfo != null && patchInfo.FileUpdated(value))
			{
				if (!CompareFileInInPath(patchInfo, filePath, bFullIntegrityCheck))
				{
					AddRequiredDownloadList(m_requiredDownloadList, value);
				}
			}
			else if (!CompareFileInInPath(patchInfo, innerAssetPath, bFullIntegrityCheck))
			{
				AddRequiredDownloadList(m_requiredDownloadList, value);
			}
		}
		yield return null;
	}

	private static bool CompareFileInInPath(NKCPatchInfo.PatchFileInfo patchFileInfo, string filePath, bool calculateMD5)
	{
		if (!NKCPatchUtility.IsFileExists(filePath))
		{
			return false;
		}
		if (patchFileInfo == null)
		{
			return false;
		}
		if (calculateMD5)
		{
			return NKCPatchUtility.CheckIntegrity(filePath, patchFileInfo.Hash);
		}
		if (!NKCPatchUtility.CheckSize(filePath, patchFileInfo.Size))
		{
			return false;
		}
		return true;
	}

	private static void AddRequiredDownloadList(List<NKCPatchInfo.PatchFileInfo> retVal, NKCPatchInfo.PatchFileInfo newInfo)
	{
		if (retVal.Contains(newInfo))
		{
			Log.Debug("[" + newInfo.FileName + "] already exist", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManager/NKCPatchManifestManager.cs", 275);
		}
		else
		{
			retVal.Add(newInfo);
		}
	}
}
