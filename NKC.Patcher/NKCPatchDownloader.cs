using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using AssetBundles;
using Cs.Logging;
using NKC.Publisher;
using NKM;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace NKC.Patcher;

public abstract class NKCPatchDownloader : MonoBehaviour
{
	public enum BuildStatus
	{
		Unchecked,
		UpToDate,
		UpdateAvailable,
		RequireAppUpdate,
		Error
	}

	public enum VersionStatus
	{
		Unchecked,
		UpToDate,
		RequireDownload,
		Downloading,
		Error
	}

	public enum PatchDownloadStatus
	{
		Idle,
		Downloading,
		UserCancel,
		Finished,
		Error,
		UpdateRequired
	}

	public delegate void OnError(string Error);

	public delegate void OnVersionCheckResult(BuildStatus buildStatus, VersionStatus versionStatus, string ErrorCode);

	public delegate void OnDownloadProgress(long currentByte, long totalByte);

	public delegate void OnDownloadFinished(PatchDownloadStatus downloadStatus, string ErrorCode);

	public delegate void OnIntegrityCheckProgress(int currentCount, int totalCount);

	public enum DownType
	{
		FullDownload,
		TutorialWithBackground,
		Count
	}

	public static NKCPatchDownloader Instance;

	public const int MAX_ERROR_COUNT = 10;

	public OnError onError;

	public OnVersionCheckResult onVersionCheckResult;

	public OnDownloadProgress onDownloadProgress;

	public OnDownloadFinished onDownloadFinished;

	public OnIntegrityCheckProgress onIntegrityCheckProgress;

	public bool ProloguePlay;

	private DateTime m_dtNextVersionCheckTime;

	private bool m_bIntegrityCheck;

	private string localDownloadPath;

	private const string _extraAssetPath = "ExtraAsset";

	private JSONNode _versionJson;

	private string m_webRequestError;

	protected readonly HashSet<NKCPatchInfo.PatchFileInfo> _downloadfiles = new HashSet<NKCPatchInfo.PatchFileInfo>();

	protected readonly HashSet<NKCPatchInfo.PatchFileInfo> _extraDownloadFiles = new HashSet<NKCPatchInfo.PatchFileInfo>();

	protected readonly HashSet<NKCPatchInfo.PatchFileInfo> _tutorialDownloadFiles = new HashSet<NKCPatchInfo.PatchFileInfo>();

	protected readonly HashSet<NKCPatchInfo.PatchFileInfo> _tutorialBackGroundDownloadFiles = new HashSet<NKCPatchInfo.PatchFileInfo>();

	protected long _totalBytesToDownload;

	protected long _currentBytesToDownload;

	private static string _logHeader = "NKCPatchDownloader";

	public abstract bool IsInit { get; }

	public BuildStatus BuildCheckStatus { get; set; }

	public VersionStatus VersionCheckStatus { get; set; }

	public PatchDownloadStatus DownloadStatus { get; protected set; }

	public bool ConnectionInfoUpdated { get; protected set; }

	public string ErrorString { get; protected set; }

	public abstract long TotalSize { get; }

	public abstract long CurrentSize { get; }

	public float DownloadPercent
	{
		get
		{
			if (CurrentSize == TotalSize)
			{
				return 1f;
			}
			if (TotalSize == 0L)
			{
				return 1f;
			}
			return (float)CurrentSize / (float)TotalSize;
		}
	}

	public virtual bool BackgroundDownloadAvailble => false;

	public string ServerBaseAddress { get; private set; }

	public string LocalDownloadPath
	{
		get
		{
			if (localDownloadPath == null)
			{
				localDownloadPath = AssetBundleManager.GetLocalDownloadPath();
				if (!localDownloadPath.EndsWith("/"))
				{
					localDownloadPath += "/";
				}
			}
			return localDownloadPath;
		}
	}

	public string ExtraServerBaseAddress { get; private set; }

	public virtual string ExtraLocalDownloadPath { get; private set; }

	public HashSet<NKCPatchInfo.PatchFileInfo> DownloadList
	{
		get
		{
			if (!NKCPatchUtility.BackgroundPatchEnabled())
			{
				NKCPatchUtility.SaveDownloadType(DownType.FullDownload);
				return _downloadfiles;
			}
			return NKCPatchUtility.GetDownloadType() switch
			{
				DownType.FullDownload => _downloadfiles, 
				DownType.TutorialWithBackground => _tutorialDownloadFiles, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
	}

	protected bool _succeedDownloadlatestManifest { get; private set; }

	public void InitCheckTime()
	{
		Log.Debug("[PatcherManager] Init skip CheckTime", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 149);
		m_dtNextVersionCheckTime = DateTime.UtcNow;
	}

	public void CheckVersion(List<string> lstVariants, bool bIntegrityCheck = false)
	{
		Log.Debug($"[CheckVersion] BuildCheckStatus:{BuildCheckStatus} _ DownloadStatus:{VersionCheckStatus}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 163);
		if (BuildCheckStatus != BuildStatus.Unchecked && VersionCheckStatus != VersionStatus.Unchecked)
		{
			if (VersionCheckStatus == VersionStatus.RequireDownload && DownloadStatus == PatchDownloadStatus.Downloading)
			{
				VersionCheckStatus = VersionStatus.Downloading;
			}
			if (VersionCheckStatus == VersionStatus.Downloading)
			{
				Debug.LogWarning("Skip VersionCheck : already downloading");
				return;
			}
			if (DateTime.UtcNow < m_dtNextVersionCheckTime)
			{
				Debug.LogWarning("Skip VersionCheck");
				return;
			}
		}
		Debug.Log("Version Check 1");
		m_dtNextVersionCheckTime = DateTime.UtcNow.AddMinutes(5.0);
		m_bIntegrityCheck = bIntegrityCheck;
		BuildCheckStatus = BuildStatus.Unchecked;
		VersionCheckStatus = VersionStatus.Unchecked;
		Debug.Log("Version Check Imple");
		CheckVersionImpl(lstVariants);
	}

	protected abstract void CheckVersionImpl(List<string> lstVariants);

	protected void CheckVersionImplAfterProcess(BuildStatus buildStatus, VersionStatus versionStatus, string ErrorCode)
	{
		BuildCheckStatus = buildStatus;
		VersionCheckStatus = versionStatus;
		ErrorString = ErrorCode;
		onVersionCheckResult?.Invoke(buildStatus, versionStatus, ErrorCode);
	}

	protected virtual void UpdateDataVersion()
	{
		NKCConnectionInfo.UpdateDataVersionOnly();
		Log.Debug($"DataVersion Updated : {NKMDataVersion.DataVersion}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 231);
	}

	public abstract void StartFileDownload();

	public abstract void StartBackgroundDownload();

	public abstract void StopBackgroundDownload();

	public virtual void DoWhenEndDownload()
	{
	}

	protected HashSet<NKCPatchInfo.PatchFileInfo> GetFinalBackGroundDownList()
	{
		HashSet<NKCPatchInfo.PatchFileInfo> hashSet = new HashSet<NKCPatchInfo.PatchFileInfo>();
		foreach (NKCPatchInfo.PatchFileInfo tutorialBackGroundDownloadFile in _tutorialBackGroundDownloadFiles)
		{
			if (PatchManifestManager.BasePatchInfoController.NeedToBeUpdated(tutorialBackGroundDownloadFile.FileName))
			{
				NKCPatchInfo.PatchFileInfo item = new NKCPatchInfo.PatchFileInfo(tutorialBackGroundDownloadFile.FileName, tutorialBackGroundDownloadFile.Hash, tutorialBackGroundDownloadFile.Size);
				hashSet.Add(item);
			}
		}
		return hashSet;
	}

	protected void DownloadFinished(PatchDownloadStatus status, string errorStr)
	{
		DownloadStatus = status;
		if (status == PatchDownloadStatus.Finished)
		{
			DoWhenEndDownload();
			VersionCheckStatus = VersionStatus.UpToDate;
			if (NKCPublisherModule.ServerInfo.IsUsePatchConnectionInfo())
			{
				UpdateDataVersion();
			}
		}
		onDownloadFinished?.Invoke(status, errorStr);
	}

	public virtual string GetFileFullPath(string filePath)
	{
		if (false || (VersionCheckStatus == VersionStatus.UpToDate && !ProloguePlay))
		{
			string text = LocalDownloadPath;
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			string text2 = text + filePath;
			if (NKCPatchUtility.IsFileExists(text2))
			{
				return text2;
			}
		}
		string innerAssetPath = NKCPatchUtility.GetInnerAssetPath(filePath);
		if (NKCPatchUtility.IsFileExists(innerAssetPath))
		{
			return innerAssetPath;
		}
		return "";
	}

	public virtual string GetLocalDownloadedPath(string filePath)
	{
		string text = LocalDownloadPath;
		if (!text.EndsWith("/"))
		{
			text += "/";
		}
		string text2 = text + filePath;
		if (NKCPatchUtility.IsFileExists(text2))
		{
			return text2;
		}
		return "";
	}

	public abstract void Unload();

	public abstract bool IsFileWillDownloaded(string filePath);

	protected void NotifyError(string msg)
	{
		ErrorString = msg;
		onError?.Invoke(ErrorString);
	}

	public virtual bool HasNoDownloadedFiles()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(LocalDownloadPath);
		if (!directoryInfo.Exists)
		{
			return true;
		}
		return directoryInfo.GetFiles().Length <= 1;
	}

	public void SetUpdatedForSkipPatch()
	{
		VersionCheckStatus = VersionStatus.UpToDate;
		BuildCheckStatus = BuildStatus.UpToDate;
	}

	public void SetBaseDownloadPath(string downloadServerAddress, string versionString, bool useExtra)
	{
		if (useExtra)
		{
			ExtraServerBaseAddress = downloadServerAddress + "ExtraAsset/" + versionString + "/";
			DebugLog($"[Extra:{useExtra}] ExtraServerBaseAddress : {ExtraServerBaseAddress}", "SetBaseDownloadPath");
			return;
		}
		ServerBaseAddress = downloadServerAddress + Utility.GetPlatformName() + "/" + versionString + "/";
		DebugLog($"[Extra:{useExtra}] ServerBaseAddress : {ServerBaseAddress}", "SetBaseDownloadPath");
	}

	public void SetExtraLocalBasePath()
	{
		ExtraLocalDownloadPath = NKCUtil.GetExtraDownloadPath();
		if (!ExtraLocalDownloadPath.EndsWith("/"))
		{
			ExtraLocalDownloadPath += "/";
		}
	}

	protected void CleanUpPcExtraPath()
	{
		if (string.IsNullOrEmpty(ExtraLocalDownloadPath))
		{
			DebugLog("ExtraLocalDownloadPath is null or empty", "CleanUpPcExtraPath");
		}
		else if (NKCDefineManager.DEFINE_PC_EXTRA_DOWNLOAD_IN_EXE_FOLDER())
		{
			string text = Application.persistentDataPath + "/Assetbundles/";
			if (!text.EndsWith("/"))
			{
				text = ExtraLocalDownloadPath + "/";
			}
			if (Directory.Exists(text))
			{
				Directory.Delete(text, recursive: true);
			}
			text = Application.persistentDataPath + "/Replay/";
			if (!text.EndsWith("/"))
			{
				text = ExtraLocalDownloadPath + "/";
			}
			if (Directory.Exists(text))
			{
				DebugLog("Delete PC ExtraPath : " + text, "CleanUpPcExtraPath");
				Directory.Delete(text, recursive: true);
			}
		}
	}

	public void FullBuildCheck()
	{
		if (!NKCDefineManager.DEFINE_FULL_BUILD())
		{
			return;
		}
		string text = PlayerPrefs.GetString("NKC_FULL_BUILD_VERSION", "");
		string version = Application.version;
		DebugLog("[NKC_FULL_BUILD_VERSION:" + text + "][ApplicationVersion:" + version + "]", "FullBuildCheck");
		if (text != version)
		{
			DebugLog("Change FullBuildVersion", "FullBuildCheck");
			if (Directory.Exists(LocalDownloadPath))
			{
				DebugLog("Delete _ " + LocalDownloadPath, "FullBuildCheck");
				Directory.Delete(LocalDownloadPath, recursive: true);
			}
			PlayerPrefs.SetString("NKC_FULL_BUILD_VERSION", Application.version);
		}
	}

	private IEnumerator VersionFileDownload(string versionFilePath)
	{
		using UnityWebRequest www = UnityWebRequest.Get(versionFilePath);
		yield return www.SendWebRequest();
		if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
		{
			_versionJson = null;
			m_webRequestError = www.error;
		}
		else
		{
			_versionJson = JSONNode.Parse(www.downloadHandler.text);
			m_webRequestError = string.Empty;
		}
	}

	private static string GetVersionFileDownLoadPath(string downloadServerAddress, bool useExtraPath)
	{
		string text = UnityEngine.Random.Range(1000000, 8000000).ToString();
		text += UnityEngine.Random.Range(1000000, 8000000);
		string text2 = "?p=" + text;
		string versionJson = NKCConnectionInfo.VersionJson;
		string result = downloadServerAddress + Utility.GetPlatformName() + versionJson + text2;
		if (useExtraPath)
		{
			result = downloadServerAddress + "ExtraAsset" + versionJson + text2;
		}
		return result;
	}

	protected IEnumerator DownloadAppVersion(bool extra)
	{
		Log.Debug("[DownloadAppVersion] DownLoader version file getting...", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 512);
		string targetVersion = null;
		string versionFilePath = GetVersionFileDownLoadPath(NKCConnectionInfo.DownloadServerAddress, extra);
		yield return VersionFileDownload(versionFilePath);
		string downloadServerAddress;
		if (_versionJson == null)
		{
			Log.Debug("[DownloadAppVersion] Fail VersionFileDownload [Error:" + m_webRequestError + "][Path:" + versionFilePath + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 524);
			versionFilePath = GetVersionFileDownLoadPath(NKCConnectionInfo.DownloadServerAddress2, extra);
			yield return VersionFileDownload(versionFilePath);
			if (_versionJson == null)
			{
				Log.Error("[DownloadAppVersion] Fail VersionFileDownload2 [Error:" + m_webRequestError + "][Path:" + versionFilePath + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 533);
				CheckVersionImplAfterProcess(BuildStatus.Error, VersionStatus.Error, NKCUtilString.GET_STRING_FAIL_VERSION);
				yield break;
			}
			downloadServerAddress = NKCConnectionInfo.DownloadServerAddress2;
		}
		else
		{
			downloadServerAddress = NKCConnectionInfo.DownloadServerAddress;
		}
		m_webRequestError = string.Empty;
		bool flag = false;
		JSONArray jSONArray = _versionJson["versionList"]?.AsArray;
		if (jSONArray != null)
		{
			targetVersion = jSONArray[0]["version"];
			flag = true;
			if (!extra)
			{
				NKCUtil.PatchVersion = targetVersion;
			}
			else
			{
				NKCUtil.PatchVersionEA = targetVersion;
			}
			CheckVersionImplAfterProcess(BuildStatus.UpToDate, VersionStatus.Unchecked, NKCUtilString.GET_STRING_FAIL_VERSION);
			Log.Debug("[DownloadAppVersion] Found downLoader version", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 568);
		}
		if (!flag)
		{
			Log.Error($"[DownloadAppVersion] Not found downLoader version _ {_versionJson}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 573);
			CheckVersionImplAfterProcess(BuildStatus.RequireAppUpdate, VersionStatus.Unchecked, "");
		}
		SetBaseDownloadPath(downloadServerAddress, targetVersion, extra);
	}

	public bool IsBackGroundDownload()
	{
		if (NKCPatchUtility.GetDownloadType() != DownType.TutorialWithBackground)
		{
			return false;
		}
		if (_tutorialBackGroundDownloadFiles == null || _tutorialBackGroundDownloadFiles.Count == 0)
		{
			return false;
		}
		return true;
	}

	public IEnumerator DownloadLatestManifest(bool extra)
	{
		_succeedDownloadlatestManifest = false;
		yield return PatchManifestManager.DownloadLatestManifest(extra);
		if (!PatchManifestManager.SuccessLatestManifest)
		{
			ErrorLog($"[extra:{extra}] LatestManifest download fail", "DownloadLatestManifest");
			CheckVersionImplAfterProcess(BuildStatus.Error, VersionStatus.Error, NKCUtilString.GET_STRING_FAIL_PATCHDATA);
		}
		else if (PatchManifestManager.GetLatestPatchInfoFor(extra) == null)
		{
			ErrorLog($"[extra:{extra}] Not found latestManifest", "DownloadLatestManifest");
			CheckVersionImplAfterProcess(BuildStatus.Error, VersionStatus.Error, NKCUtilString.GET_STRING_FAIL_PATCHDATA);
		}
		else
		{
			_succeedDownloadlatestManifest = true;
		}
	}

	public IEnumerator DownloadTutorialPatchData()
	{
		if (NKCPatchUtility.BackgroundPatchEnabled())
		{
			string text = Path.Combine(PatchManifestPath.GetLocalDownloadPath(extra: false), PatchManifestPath.TutorialPatchFileName);
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			UnityWebRequestDownloader unityWebRequestDownloader = new UnityWebRequestDownloader(ServerBaseAddress, LocalDownloadPath);
			unityWebRequestDownloader.dOnDownloadCompleted = (NKCFileDownloader.OnDownloadCompleted)Delegate.Combine(unityWebRequestDownloader.dOnDownloadCompleted, (NKCFileDownloader.OnDownloadCompleted)delegate(bool result)
			{
				Log.Debug($"[DownloadTutorialPatchData] Download Result[{result}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 683);
			});
			Log.Debug("[DownloadTutorialPatchData] FilePath[" + text + "] Download Start", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 685);
			yield return unityWebRequestDownloader.DownloadFile(PatchManifestPath.TutorialPatchFileName, PatchManifestPath.TutorialPatchFileName, 0L);
		}
	}

	public IEnumerator GetDownloadFiles(List<string> lstVariants, bool extra)
	{
		NKCPatchInfo currentPatchInfo = PatchManifestManager.GetCurrentPatchInfoFor(extra);
		if (currentPatchInfo == null)
		{
			ErrorLog($"[extra:{extra}] CurrentPatchInfo is null", "GetDownloadFiles");
			CheckVersionImplAfterProcess(BuildStatus.Error, VersionStatus.Error, NKCUtilString.GET_STRING_FAIL_PATCHDATA);
			yield break;
		}
		NKCPatchInfo downloadHistoryPatchInfoFor = PatchManifestManager.GetDownloadHistoryPatchInfoFor(extra);
		if (downloadHistoryPatchInfoFor != null)
		{
			currentPatchInfo = currentPatchInfo.Append(downloadHistoryPatchInfoFor);
		}
		NKCPatchInfo backgroundDownloadHistoryPatchInfo = PatchManifestManager.OptimizationPatchInfoController.GetBackgroundDownloadHistoryPatchInfo();
		if (backgroundDownloadHistoryPatchInfo != null)
		{
			currentPatchInfo = currentPatchInfo.Append(backgroundDownloadHistoryPatchInfo);
		}
		NKCPatchInfo latestPatchInfoFor = PatchManifestManager.GetLatestPatchInfoFor(extra);
		if (latestPatchInfoFor == null)
		{
			ErrorLog($"[extra:{extra}] latestPatchInfo is null", "GetDownloadFiles");
			CheckVersionImplAfterProcess(BuildStatus.Error, VersionStatus.Error, NKCUtilString.GET_STRING_FAIL_PATCHDATA);
			yield break;
		}
		NKCPatchInfo filteredPatchInfo = (extra ? latestPatchInfoFor : PatchManifestManager.BasePatchInfoController.CreateFilteredManifestInfo(latestPatchInfoFor, lstVariants));
		HashSet<NKCPatchInfo.PatchFileInfo> downloadContainer = (extra ? _extraDownloadFiles : _downloadfiles);
		string localBasePath = (extra ? ExtraLocalDownloadPath : LocalDownloadPath);
		yield return StartCoroutine(PatchManifestManager.GetDownloadList(downloadContainer, currentPatchInfo, filteredPatchInfo, localBasePath, m_bIntegrityCheck, onIntegrityCheckProgress));
		DebugLog($"[extra:{extra}][curVer:{currentPatchInfo.VersionString}][latestVer:{filteredPatchInfo.VersionString}][downLoadListCount:{downloadContainer.Count}] Download list created ", "GetDownloadFiles");
		if (m_bIntegrityCheck || NKCPatchUtility.GetTutorialClearedStatus() || NKCPatchUtility.GetDownloadType() == DownType.FullDownload)
		{
			NKCPatchUtility.SaveDownloadType(DownType.FullDownload);
		}
		else if (!extra && NKCPatchUtility.BackgroundPatchEnabled())
		{
			NKCPatchInfo tutorialPatchInfo = PatchManifestManager.OptimizationPatchInfoController.CreateTutorialOnlyManifestInfo(filteredPatchInfo);
			if (tutorialPatchInfo == null)
			{
				NKCPatchUtility.SaveDownloadType(DownType.FullDownload);
			}
			else
			{
				yield return StartCoroutine(PatchManifestManager.GetDownloadList(_tutorialDownloadFiles, currentPatchInfo, tutorialPatchInfo, localBasePath, m_bIntegrityCheck, onIntegrityCheckProgress));
				NKCPatchInfo latestManifest = filteredPatchInfo.DifferenceOfSetBy(tutorialPatchInfo);
				yield return StartCoroutine(PatchManifestManager.GetDownloadList(_tutorialBackGroundDownloadFiles, currentPatchInfo, latestManifest, localBasePath, m_bIntegrityCheck, onIntegrityCheckProgress));
				NKCPatchUtility.SaveDownloadType(DownType.TutorialWithBackground);
			}
		}
		yield return null;
	}

	protected void ClearFileDownloadContainer()
	{
		_downloadfiles?.Clear();
		_extraDownloadFiles?.Clear();
		_tutorialDownloadFiles?.Clear();
		_tutorialBackGroundDownloadFiles?.Clear();
		_totalBytesToDownload = 0L;
	}

	public void OnEndVersionCheck()
	{
		if (DownloadList.Count > 0 || _extraDownloadFiles.Count > 0)
		{
			CheckVersionImplAfterProcess(BuildCheckStatus, VersionStatus.RequireDownload, "");
			return;
		}
		if (IsBackGroundDownload())
		{
			Log.Debug("[OnEndVersionChec ] Need to background down", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 778);
			StartBackgroundDownload();
		}
		CheckVersionImplAfterProcess(BuildCheckStatus, VersionStatus.UpToDate, "");
	}

	public IEnumerator DownloadListCheck(List<string> variantList, bool extra)
	{
		yield return DownloadAppVersion(extra);
		if (BuildCheckStatus == BuildStatus.Error)
		{
			ErrorLog($"[extra:{extra}] Fail DownloadAppVersion", "DownloadListCheck");
			yield break;
		}
		if (BuildCheckStatus == BuildStatus.RequireAppUpdate)
		{
			ErrorLog($"[extra:{extra}] Fail DownloadAppVersion", "DownloadListCheck");
			yield break;
		}
		DebugLog($"[extra:{extra}][LocalDownloadPath:{LocalDownloadPath}] DownLoader Ini t Finished, Starting download patchInfo", "DownloadListCheck");
		yield return DownloadLatestManifest(extra);
		if (BuildCheckStatus == BuildStatus.Error)
		{
			yield break;
		}
		if (!extra)
		{
			yield return DownloadTutorialPatchData();
		}
		string[] files = Directory.GetFiles(localDownloadPath);
		foreach (string text in files)
		{
			string fileName = Path.GetFileName(text);
			string folderedBundleName = AssetBundleManager.GetFolderedBundleName(fileName);
			if (fileName.Length != folderedBundleName.Length)
			{
				string targetPath = Path.Combine(localDownloadPath, folderedBundleName);
				NKCPatchUtility.MoveFile(text, targetPath);
			}
		}
		yield return GetDownloadFiles(variantList, extra);
		if (BuildCheckStatus == BuildStatus.Error)
		{
			ErrorLog($"[extra:{extra}] Fail GetDownloadFiles", "DownloadListCheck");
		}
		else if (!PatchManifestManager.CleanUpFiles(extra))
		{
			ErrorLog($"[extra:{extra}] Fail CleanUpFiles ", "DownloadListCheck");
			CheckVersionImplAfterProcess(BuildStatus.Error, VersionStatus.Error, NKCUtilString.GET_STRING_FAIL_PATCHDATA);
		}
	}

	public virtual void MoveToMarket()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			Application.OpenURL("market://details?id=" + Application.identifier);
			Application.Quit();
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.IPhonePlayer:
		case RuntimePlatform.tvOS:
			Application.Quit();
			break;
		default:
			Application.Quit();
			break;
		}
	}

	protected IEnumerator BeginCoroutine(Coroutine<string> routine, OnError onError)
	{
		yield return routine.coroutine;
		try
		{
			DebugLog(routine.Value, "BeginCoroutine");
		}
		catch (WebException ex)
		{
			if (ex.Status == WebExceptionStatus.ProtocolError)
			{
				string text = $"{((HttpWebResponse)ex.Response).StatusCode} : {((HttpWebResponse)ex.Response).StatusDescription}";
				ErrorLog(text, "BeginCoroutine");
				onError?.Invoke(text);
			}
			else
			{
				ErrorLog(ex.Message, "BeginCoroutine");
				onError?.Invoke(ex.Message);
			}
		}
		catch (Exception ex2)
		{
			ErrorLog(ex2.Message, "BeginCoroutine");
			onError?.Invoke(ex2.Message);
		}
	}

	private static void DebugLog(string log, [CallerMemberName] string caller = "")
	{
		Log.Debug("[" + _logHeader + "][" + caller + "] _ " + log, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 942);
	}

	private static void WarnLog(string log, [CallerMemberName] string caller = "")
	{
		Log.Warn("[" + _logHeader + "][" + caller + "] _ " + log, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 947);
	}

	private static void ErrorLog(string log, [CallerMemberName] string caller = "")
	{
		Log.Error("[" + _logHeader + "][" + caller + "] _ " + log, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchDownloader.cs", 952);
	}
}
