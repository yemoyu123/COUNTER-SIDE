using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AssetBundles;
using Cs.Logging;

namespace NKC.Patcher;

public class PatchManifestManager
{
	public static readonly BasePatchInfoController BasePatchInfoController = new BasePatchInfoController();

	public static readonly ExtraPatchInfoController ExtraPatchInfoController = new ExtraPatchInfoController();

	public static readonly OptimizationPatchInfoController OptimizationPatchInfoController = new OptimizationPatchInfoController();

	public static readonly InnerPatchInfoController InnerPatchInfoController = new InnerPatchInfoController();

	public static bool SuccessLatestManifest;

	public static NKCPatchInfo LoadManifest(PatchManifestPath.PatchType type)
	{
		return NKCPatchInfo.LoadFromJSON(PatchManifestPath.GetLocalPathBy(type));
	}

	public static NKCPatchInfo GetDownloadHistoryPatchInfoFor(bool extra)
	{
		if (!extra)
		{
			return BasePatchInfoController.LoadDownloadHistoryManifest();
		}
		return ExtraPatchInfoController.LoadDownloadHistoryExtraManifest();
	}

	public static NKCPatchInfo GetCurrentPatchInfoFor(bool extra)
	{
		if (!extra)
		{
			return BasePatchInfoController.GetCurPatchInfo();
		}
		return ExtraPatchInfoController.GetCurrentExtraPatchInfo();
	}

	public static NKCPatchInfo GetLatestPatchInfoFor(bool extra)
	{
		if (!extra)
		{
			return BasePatchInfoController.GetLatestPatchInfo();
		}
		return ExtraPatchInfoController.GetLatestExtraPatchInfo();
	}

	public static bool IsFileExist(string assetBundleName)
	{
		if (File.Exists(AssetBundleManager.GetLocalDownloadPath() + "/" + assetBundleName))
		{
			return true;
		}
		return false;
	}

	public static void RemoveManifestFile(PatchManifestPath.PatchType type)
	{
		string localPathBy = PatchManifestPath.GetLocalPathBy(type);
		if (!(localPathBy == string.Empty))
		{
			if (File.Exists(localPathBy))
			{
				File.Delete(localPathBy);
			}
			else
			{
				Log.Warn("[RemoveManifestFile][Not exist file:" + localPathBy + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 64);
			}
		}
	}

	public static bool CleanUpFiles(bool extra)
	{
		Log.Debug("[CleanUpFiles] Start", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 73);
		bool flag = false;
		if (!extra && NKCDefineManager.DEFINE_SEMI_FULL_BUILD())
		{
			InnerPatchInfoController.LoadFullBuildManifest();
			if (InnerPatchInfoController.FullBuildPatchInfo == null)
			{
				Log.Debug("[CleanUpFiles] Not found FullBuildPatchInfo", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 82);
			}
			flag = true;
		}
		NKCPatchInfo latestPatchInfoFor = GetLatestPatchInfoFor(extra);
		if (latestPatchInfoFor == null)
		{
			Log.Debug("[CleanUpFiles] latestPatchInfo is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 91);
			return false;
		}
		NKCPatchInfo currentPatchInfoFor = GetCurrentPatchInfoFor(extra);
		if (currentPatchInfoFor == null)
		{
			Log.Debug("[CleanUpFiles] CurrentPatchInfo is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 98);
			return false;
		}
		List<string> ignoreList = new List<string>
		{
			"PatchInfo.json",
			"LatestPatchInfo.json",
			"TempPatchInfo.json",
			"LatestExtraPatchInfo.json",
			"ConnectionInfo.json",
			Utility.GetPlatformName()
		};
		NKCPatchUtility.CleanupDirectory(extra ? PatchManifestPath.ExtraLocalDownloadPath : PatchManifestPath.LocalDownloadPath, latestPatchInfoFor, ignoreList, flag ? InnerPatchInfoController.FullBuildPatchInfo : null, flag ? currentPatchInfoFor : null);
		return true;
	}

	public static IEnumerator DownloadLatestManifest(bool extra)
	{
		if (File.Exists(PatchManifestPath.GetManifestPath(extra)))
		{
			File.Delete(PatchManifestPath.GetManifestPath(extra));
		}
		UnityWebRequestDownloader unityWebRequestDownloader = new UnityWebRequestDownloader(PatchManifestPath.GetServerBasePath(extra), PatchManifestPath.GetLocalDownloadPath(extra));
		unityWebRequestDownloader.dOnDownloadCompleted = (NKCFileDownloader.OnDownloadCompleted)Delegate.Combine(unityWebRequestDownloader.dOnDownloadCompleted, new NKCFileDownloader.OnDownloadCompleted(OnComplete));
		Log.Debug($"[DownloadLatestManifest][serverDownloadPath:{PatchManifestPath.GetServerBasePath(extra)}][localDownloadPath:{PatchManifestPath.GetLocalDownloadPath(extra)}][extra:{extra}] Start PatchFile Down ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 141);
		yield return unityWebRequestDownloader.DownloadFile("PatchInfo.json", PatchManifestPath.GetManifestFileName(extra), 0L);
		static void OnComplete(bool success)
		{
			SuccessLatestManifest = success;
		}
	}

	public static IEnumerator GetDownloadList(HashSet<NKCPatchInfo.PatchFileInfo> downloadTargetContainer, NKCPatchInfo currentManifest, NKCPatchInfo latestManifest, string downloadBasePath, bool fullIntegrityCheck = false, NKCPatchDownloader.OnIntegrityCheckProgress onIntegrityProgress = null)
	{
		if (downloadTargetContainer == null)
		{
			Log.Error("[GetDownloadList] downloadTargetContainer Null!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 157);
			yield break;
		}
		downloadTargetContainer.Clear();
		if (latestManifest == null)
		{
			Log.Error("[GetDownloadList] NewManifest Null!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 166);
			yield break;
		}
		if (currentManifest == null)
		{
			Log.Error("[GetDownloadList] All new Download", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 173);
			{
				foreach (NKCPatchInfo.PatchFileInfo value2 in latestManifest.m_dicPatchInfo.Values)
				{
					downloadTargetContainer.Add(value2);
				}
				yield break;
			}
		}
		Log.Debug(fullIntegrityCheck ? "[GetDownloadList] Doing full integrity check" : "[GetDownloadList] Integrity check skip", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 183);
		if (latestManifest.m_dicPatchInfo == null)
		{
			Log.Error("[GetDownloadList] latestManifest patchInfo dic is null!!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 187);
			yield break;
		}
		if (NKCDefineManager.DEFINE_OBB())
		{
			InnerPatchInfoController.LoadObbManifest();
		}
		else if (NKCDefineManager.DEFINE_SEMI_FULL_BUILD())
		{
			InnerPatchInfoController.LoadFullBuildManifest();
		}
		int totalCount = latestManifest.m_dicPatchInfo.Count;
		int currentCount = 0;
		foreach (KeyValuePair<string, NKCPatchInfo.PatchFileInfo> kvPair in latestManifest.m_dicPatchInfo)
		{
			if (fullIntegrityCheck)
			{
				currentCount++;
				if (currentCount % 10 == 0)
				{
					onIntegrityProgress?.Invoke(currentCount, totalCount);
					yield return null;
				}
			}
			NKCPatchInfo.PatchFileInfo value = kvPair.Value;
			NKCPatchInfo.PatchFileInfo patchInfo = currentManifest.GetPatchInfo(kvPair.Key);
			if (value == null)
			{
				Log.Error("[GetDownloadList] NewInfo is null!!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/PatchManifestManager.cs", 218);
				continue;
			}
			if (patchInfo == null)
			{
				CheckInnerPatchInfo(kvPair, downloadTargetContainer, value, fullIntegrityCheck);
				continue;
			}
			string path = Path.Combine(downloadBasePath, value.FileName);
			if (patchInfo.FileUpdated(value))
			{
				AddTo(downloadTargetContainer, value);
				PatchLogContainer.AddToLog("Is old file", value, path);
				continue;
			}
			if (fullIntegrityCheck || !NKCDefineManager.DEFINE_LB())
			{
				if (!NKCPatchUtility.IsFileExists(path))
				{
					CheckInnerPatchInfo(kvPair, downloadTargetContainer, value, fullIntegrityCheck);
					continue;
				}
				if (!NKCPatchUtility.CheckSize(path, value.Size))
				{
					AddTo(downloadTargetContainer, value);
					PatchLogContainer.AddToLog("Different size", value, path);
					continue;
				}
			}
			if (fullIntegrityCheck && !NKCPatchUtility.CheckIntegrity(path, value.Hash))
			{
				AddTo(downloadTargetContainer, value);
				PatchLogContainer.AddToLog("Check integrity", value, path);
			}
		}
		PatchLogContainer.DownloadListLogOutPut();
	}

	private static void AddTo(HashSet<NKCPatchInfo.PatchFileInfo> downloadTargetList, NKCPatchInfo.PatchFileInfo latestPatchFileInfo)
	{
		if (!downloadTargetList.Contains(latestPatchFileInfo))
		{
			downloadTargetList.Add(latestPatchFileInfo);
		}
	}

	private static void CheckInnerPatchInfo(KeyValuePair<string, NKCPatchInfo.PatchFileInfo> kvPair, HashSet<NKCPatchInfo.PatchFileInfo> downloadTargetList, NKCPatchInfo.PatchFileInfo latestFileInfo, bool fullIntegrityCheck)
	{
		if (InnerPatchInfoController.ObbPatchInfo != null)
		{
			NKCPatchInfo.PatchFileInfo patchInfo = InnerPatchInfoController.ObbPatchInfo.GetPatchInfo(kvPair.Key);
			string obbBuildAssetPath = NKCPatchUtility.GetObbBuildAssetPath(latestFileInfo.FileName);
			ProcessInnerAsset(downloadTargetList, patchInfo, latestFileInfo, obbBuildAssetPath, fullIntegrityCheck);
		}
		else if (InnerPatchInfoController.FullBuildPatchInfo != null)
		{
			NKCPatchInfo.PatchFileInfo patchInfo2 = InnerPatchInfoController.FullBuildPatchInfo.GetPatchInfo(kvPair.Key);
			string fullBuildAssetPath = NKCPatchUtility.GetFullBuildAssetPath(latestFileInfo.FileName);
			ProcessInnerAsset(downloadTargetList, patchInfo2, latestFileInfo, fullBuildAssetPath, fullIntegrityCheck);
		}
		else
		{
			AddTo(downloadTargetList, latestFileInfo);
			PatchLogContainer.AddToLog("Not exist in currentManifest", latestFileInfo, latestFileInfo.FileName);
		}
	}

	private static void ProcessInnerAsset(HashSet<NKCPatchInfo.PatchFileInfo> retVal, NKCPatchInfo.PatchFileInfo innerInfo, NKCPatchInfo.PatchFileInfo newInfo, string innerPath, bool FullIntegrityCheck)
	{
		if (!NKCPatchUtility.IsFileExists(innerPath))
		{
			AddTo(retVal, newInfo);
			PatchLogContainer.AddToLog("[Inner] Not exist", newInfo, innerPath);
		}
		else if (!NKCPatchUtility.CheckSize(innerPath, newInfo.Size))
		{
			AddTo(retVal, newInfo);
			PatchLogContainer.AddToLog("[Inner] Different size", newInfo, innerPath);
		}
		else if (innerInfo != null)
		{
			if (innerInfo.FileUpdated(newInfo))
			{
				AddTo(retVal, newInfo);
				PatchLogContainer.AddToLog("[Inner] FileUpdated", newInfo, innerPath);
			}
		}
		else if (FullIntegrityCheck && !NKCPatchUtility.CheckIntegrity(innerPath, newInfo.Hash))
		{
			AddTo(retVal, newInfo);
			PatchLogContainer.AddToLog("[Inner] Check integrity", newInfo, innerPath);
		}
	}
}
