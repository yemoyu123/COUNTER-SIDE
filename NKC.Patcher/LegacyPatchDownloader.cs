using System.Collections;
using System.Collections.Generic;
using Cs.Logging;
using NKC.Patcher.Parallel;
using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC.Patcher;

public class LegacyPatchDownloader : NKCPatchDownloader
{
	private ILegacyImpl impl;

	private Coroutine<string> _backGroundCoroutine;

	public override long CurrentSize
	{
		get
		{
			long num = 0L;
			if (impl != null)
			{
				num += impl.CurrentBytesToDownload;
			}
			return num;
		}
	}

	public override long TotalSize => _totalBytesToDownload;

	public override bool IsInit => impl != null;

	public static NKCPatchDownloader InitInstance(OnError onErrorDelegate)
	{
		Log.Debug("Init LegacyPatchDownloader", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloader.cs", 39);
		GameObject gameObject = new GameObject("PatchDownloader");
		if (string.IsNullOrEmpty(NKCConnectionInfo.DownloadServerAddress))
		{
			Debug.LogError("DownloadServerAddress not initialized!!");
		}
		LegacyPatchDownloader legacyPatchDownloader = (LegacyPatchDownloader)(NKCPatchDownloader.Instance = gameObject.AddComponent<LegacyPatchDownloader>());
		NKCPatchDownloader.Instance.onError = onErrorDelegate;
		if (NKCDefineManager.USE_ANDROIDSERVICE())
		{
			Log.Debug("[AndroidService]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloader.cs", 54);
			NKADownloadImpl nKADownloadImpl = gameObject.AddComponent<NKADownloadImpl>();
			legacyPatchDownloader.SetImpl(nKADownloadImpl);
		}
		else if (!NKCDefineManager.DEFINE_STEAM() && NKMContentsVersionManager.HasTag("MULTITASK_DOWNLOAD"))
		{
			Log.Debug("[MultiTaskDownloaderImpl]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloader.cs", 61);
			MultiTaskDownloaderImpl multiTaskDownloaderImpl = gameObject.AddComponent<MultiTaskDownloaderImpl>();
			legacyPatchDownloader.SetImpl(multiTaskDownloaderImpl);
		}
		else
		{
			Log.Debug("[LegacyPatchDownloaderImpl]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloader.cs", 68);
			LegacyPatchDownloaderImpl legacyPatchDownloaderImpl = gameObject.AddComponent<LegacyPatchDownloaderImpl>();
			legacyPatchDownloader.SetImpl(legacyPatchDownloaderImpl);
		}
		Object.DontDestroyOnLoad(gameObject);
		return NKCPatchDownloader.Instance;
	}

	public void SetImpl(ILegacyImpl impl)
	{
		this.impl = impl;
	}

	protected override void CheckVersionImpl(List<string> lstVariants)
	{
		BeginCoroutine(this.StartCoroutine<string>(StartVersionCheck(lstVariants)), delegate(string s)
		{
			CheckVersionImplAfterProcess(BuildStatus.Error, VersionStatus.Error, s);
		});
	}

	private IEnumerator StartVersionCheck(List<string> lstVariants)
	{
		ClearFileDownloadContainer();
		FullBuildCheck();
		yield return DownloadListCheck(lstVariants, extra: false);
		foreach (NKCPatchInfo.PatchFileInfo download in base.DownloadList)
		{
			_totalBytesToDownload += download.Size;
		}
		if (NKCDefineManager.DEFINE_EXTRA_ASSET())
		{
			SetExtraLocalBasePath();
			CleanUpPcExtraPath();
			yield return DownloadListCheck(lstVariants, extra: true);
			foreach (NKCPatchInfo.PatchFileInfo extraDownloadFile in _extraDownloadFiles)
			{
				_totalBytesToDownload += extraDownloadFile.Size;
			}
		}
		OnEndVersionCheck();
	}

	private bool NeedToExtraDownload()
	{
		if (NKCDefineManager.DEFINE_EXTRA_ASSET())
		{
			HashSet<NKCPatchInfo.PatchFileInfo> extraDownloadFiles = _extraDownloadFiles;
			if (extraDownloadFiles != null && extraDownloadFiles.Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator StartDownload()
	{
		impl.CleanUp();
		impl.SetDownloadInfo(base.ServerBaseAddress, base.LocalDownloadPath, base.DownloadList, PatchManifestManager.BasePatchInfoController.GetDefaultDownloadHistoryPatchInfo());
		yield return impl.ProcessFileDownload(EventProcessDownload, OnFinishedDownload);
		if (NeedToExtraDownload())
		{
			Log.Debug("[StartDownload] Start ExtraDownload", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloader.cs", 144);
			foreach (NKCPatchInfo.PatchFileInfo extraDownloadFile in _extraDownloadFiles)
			{
				Log.Debug("[StartDownload] Extra download target : " + extraDownloadFile.FileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloader.cs", 147);
			}
			impl.CleanUp();
			impl.SetDownloadInfo(base.ExtraServerBaseAddress, ExtraLocalDownloadPath, _extraDownloadFiles, PatchManifestManager.ExtraPatchInfoController.GetDownloadHistoryExtraPatchInfo());
			yield return impl.ProcessFileDownload(EventProcessDownload, OnFinishedExtraDownload);
		}
		DownloadFinished(PatchDownloadStatus.Finished, "");
		StartBackgroundDownload();
	}

	private void OnFinishedDownload(PatchDownloadStatus downloadStatus, string errorCode)
	{
		Log.Debug($"[OnFinishedDownload] : status {downloadStatus}, errorCode {errorCode}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloader.cs", 165);
		if (NeedToExtraDownload())
		{
			base.DownloadStatus = PatchDownloadStatus.Downloading;
		}
		else
		{
			base.DownloadStatus = downloadStatus;
		}
		base.ErrorString = errorCode;
		PatchManifestManager.BasePatchInfoController.AppendFilteredManifestToCurrentManifest();
		PatchManifestManager.RemoveManifestFile(PatchManifestPath.PatchType.TempManifest);
	}

	private void OnFinishedExtraDownload(PatchDownloadStatus downloadStatus, string errorCode)
	{
		Log.Debug($"[OnFinishedExtraDownload][status:{downloadStatus}][errorCode:{errorCode}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloader.cs", 185);
		base.DownloadStatus = downloadStatus;
		base.ErrorString = errorCode;
		PatchManifestManager.ExtraPatchInfoController.AppendFilteredManifestToCurrentManifest();
		PatchManifestManager.RemoveManifestFile(PatchManifestPath.PatchType.TempExtraManifest);
	}

	private void OnFinishedNonEssentialDownload(PatchDownloadStatus downloadStatus, string errorCode)
	{
		Log.Debug($"[OnFinishedNonEssentialDownload][status:{downloadStatus}][errorCode:{errorCode}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloader.cs", 196);
		NKCUIOverlayPatchProgress.CheckInstanceAndClose();
		PatchManifestManager.BasePatchInfoController.AppendFilteredManifestToCurrentManifest();
		PatchManifestManager.RemoveManifestFile(PatchManifestPath.PatchType.TempManifest);
	}

	public override void StartFileDownload()
	{
		base.DownloadStatus = PatchDownloadStatus.Downloading;
		base.VersionCheckStatus = VersionStatus.Downloading;
		this.StartCoroutine<string>(StartDownload());
	}

	public override void StartBackgroundDownload()
	{
		if (IsBackGroundDownload())
		{
			HashSet<NKCPatchInfo.PatchFileInfo> finalBackGroundDownList = GetFinalBackGroundDownList();
			_backGroundCoroutine = this.StartCoroutine<string>(BackGroundDownload(finalBackGroundDownList));
		}
	}

	public override void StopBackgroundDownload()
	{
		if (_backGroundCoroutine?.coroutine != null)
		{
			impl.CleanUp();
			StopCoroutine(_backGroundCoroutine.coroutine);
			_backGroundCoroutine = null;
		}
	}

	private IEnumerator BackGroundDownload(HashSet<NKCPatchInfo.PatchFileInfo> patchFiles)
	{
		yield return null;
		_totalBytesToDownload = 0L;
		foreach (NKCPatchInfo.PatchFileInfo patchFile in patchFiles)
		{
			_totalBytesToDownload += patchFile.Size;
		}
		impl.CleanUp();
		impl.SetDownloadInfo(base.ServerBaseAddress, base.LocalDownloadPath, patchFiles, PatchManifestManager.OptimizationPatchInfoController.GetBackgroundDownloadHistoryPatchInfo());
		yield return impl.ProcessFileDownload(EventProcessDownload, OnFinishedNonEssentialDownload);
	}

	private void EventProcessDownload(long currentByte, long totalByte)
	{
		onDownloadProgress?.Invoke(currentByte, totalByte);
	}

	public override void Unload()
	{
		if (impl != null && impl.ImplGameObject != null)
		{
			Object.Destroy(impl.ImplGameObject);
		}
	}

	public override bool IsFileWillDownloaded(string filePath)
	{
		return impl.IsFileWillDownloaded(filePath);
	}
}
