using System;
using System.Collections;
using System.Collections.Generic;
using Cs.Logging;
using NKC.UI;
using UnityEngine;

namespace NKC.Patcher;

public class NKCPatchParallelDownloader : NKCPatchDownloader
{
	private Queue<NKCPatchInfo.PatchFileInfo> DownloadQueue;

	private List<NKCPatchParallelDownloadWorker> lstWorker;

	private readonly DownloadHistoryController _downloadHistoryController = new DownloadHistoryController();

	private bool m_bInit;

	private Action<bool, string> _onEndAction;

	private const int PARALLEL_DOWNLOAD_COUNT = 16;

	private Coroutine _backGroundCoroutine;

	public override bool IsInit => m_bInit;

	public override long TotalSize => _totalBytesToDownload;

	public override long CurrentSize => _currentBytesToDownload;

	public static NKCPatchDownloader InitInstance(OnError onErrorDelegate)
	{
		Log.Debug("Init NKCPatchParallelDownloader", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchParallelDownloader.cs", 19);
		GameObject obj = new GameObject("PatchDownloader");
		if (string.IsNullOrEmpty(NKCConnectionInfo.DownloadServerAddress))
		{
			Debug.LogError("DownloadServerAddress not initialized!!");
		}
		((NKCPatchParallelDownloader)(NKCPatchDownloader.Instance = obj.AddComponent<NKCPatchParallelDownloader>())).Init(onErrorDelegate);
		UnityEngine.Object.DontDestroyOnLoad(obj);
		return NKCPatchDownloader.Instance;
	}

	private void Init(OnError onErrorDelegate)
	{
		onError = onErrorDelegate;
		m_bInit = true;
	}

	public override bool IsFileWillDownloaded(string filePath)
	{
		if (base.DownloadList == null)
		{
			return false;
		}
		foreach (NKCPatchInfo.PatchFileInfo download in base.DownloadList)
		{
			if (download.FileName == filePath)
			{
				return true;
			}
		}
		return false;
	}

	public override void StartFileDownload()
	{
		_downloadHistoryController.SetDownloadHistoryPatchInfo(PatchManifestManager.BasePatchInfoController.GetDefaultDownloadHistoryPatchInfo());
		SetDownloadState();
		SetOnEndAction(OnEndDownload);
		StartCoroutine(DownloadProcess(base.DownloadList));
	}

	public override void StartBackgroundDownload()
	{
		if (!IsBackGroundDownload())
		{
			return;
		}
		_downloadHistoryController.SetDownloadHistoryPatchInfo(PatchManifestManager.OptimizationPatchInfoController.GetBackgroundDownloadHistoryPatchInfo());
		HashSet<NKCPatchInfo.PatchFileInfo> finalBackGroundDownList = GetFinalBackGroundDownList();
		_totalBytesToDownload = 0L;
		foreach (NKCPatchInfo.PatchFileInfo item in finalBackGroundDownList)
		{
			_totalBytesToDownload += item.Size;
		}
		SetOnEndAction(OnEndNonEssentialDownload);
		_backGroundCoroutine = StartCoroutine(DownloadProcess(finalBackGroundDownList));
	}

	public override void StopBackgroundDownload()
	{
		if (_backGroundCoroutine != null)
		{
			StopCoroutine(_backGroundCoroutine);
			_backGroundCoroutine = null;
		}
	}

	private void SetOnEndAction(Action<bool, string> onEnd)
	{
		_onEndAction = onEnd;
	}

	public void OnEndDownload(bool errorOccurred, string errorStr)
	{
		if (errorOccurred)
		{
			Log.Error($"[OnEndDownload][errorOccurred:{true}][errorStr:{errorStr}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchParallelDownloader.cs", 117);
		}
		else
		{
			Log.Debug($"[OnEndDownload][errorOccurred:{false}][errorStr:{errorStr}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchParallelDownloader.cs", 121);
		}
		if (errorOccurred)
		{
			EventDownloadFinished(PatchDownloadStatus.Error, errorStr);
		}
		else
		{
			EventDownloadFinished(PatchDownloadStatus.Finished, "");
			PatchManifestManager.BasePatchInfoController.AppendFilteredManifestToCurrentManifest();
			PatchManifestManager.RemoveManifestFile(PatchManifestPath.PatchType.TempManifest);
		}
		StartBackgroundDownload();
	}

	public void OnEndNonEssentialDownload(bool errorOccurred, string errorStr)
	{
		if (errorOccurred)
		{
			Log.Error($"[OnEndDownload][errorOccurred:{true}][errorStr:{errorStr}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchParallelDownloader.cs", 145);
		}
		else
		{
			Log.Debug($"[OnEndDownload][errorOccurred:{false}][errorStr:{errorStr}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchParallelDownloader.cs", 149);
		}
		NKCUIOverlayPatchProgress.CheckInstanceAndClose();
		PatchManifestManager.BasePatchInfoController.AppendFilteredManifestToCurrentManifest();
		PatchManifestManager.RemoveManifestFile(PatchManifestPath.PatchType.BackgroundDownloadHistoryManifest);
	}

	private void SetDownloadState()
	{
		base.DownloadStatus = PatchDownloadStatus.Downloading;
		base.VersionCheckStatus = VersionStatus.Downloading;
	}

	public IEnumerator DownloadProcess(HashSet<NKCPatchInfo.PatchFileInfo> downloadList)
	{
		_currentBytesToDownload = 0L;
		if (downloadList == null)
		{
			EventDownloadFinished(PatchDownloadStatus.Error, "No  file to download!");
			yield break;
		}
		if (string.IsNullOrEmpty(base.ServerBaseAddress))
		{
			EventDownloadFinished(PatchDownloadStatus.Error, "PatchManifestPath.ServerBaseAddress Is Empty!");
			yield break;
		}
		DownloadQueue = new Queue<NKCPatchInfo.PatchFileInfo>(downloadList);
		lstWorker = new List<NKCPatchParallelDownloadWorker>(16);
		for (int i = 0; i < 16; i++)
		{
			lstWorker.Add(new NKCPatchParallelUnityWebDownloadWorker(base.ServerBaseAddress, base.LocalDownloadPath));
		}
		_downloadHistoryController.CleanUp();
		string lastError = null;
		bool bErrorStop = false;
		bool bFinished;
		do
		{
			bFinished = true;
			long num = 0L;
			foreach (NKCPatchParallelDownloadWorker item in lstWorker)
			{
				num += item.Update();
				switch (item.State)
				{
				case NKCPatchParallelDownloadWorker.eState.Busy:
				case NKCPatchParallelDownloadWorker.eState.Retry:
					bFinished = false;
					break;
				case NKCPatchParallelDownloadWorker.eState.Error:
					bErrorStop = true;
					lastError = item.lastError;
					break;
				case NKCPatchParallelDownloadWorker.eState.Idle:
				case NKCPatchParallelDownloadWorker.eState.Complete:
					if (item.State == NKCPatchParallelDownloadWorker.eState.Complete)
					{
						if (item.PatchFileInfo != null)
						{
							_downloadHistoryController.AddTo(item.PatchFileInfo);
						}
						item.Cleanup();
					}
					if (DownloadQueue.Count != 0 && !bErrorStop)
					{
						NKCPatchInfo.PatchFileInfo fileInfo = DownloadQueue.Dequeue();
						item.StartDownloadFile(fileInfo);
						bFinished = false;
					}
					break;
				}
			}
			_downloadHistoryController.UpdateDownloadHistoryPatchInfo();
			_currentBytesToDownload = _downloadHistoryController.CurrentDownloadedCompletedSize + num;
			onDownloadProgress?.Invoke(_currentBytesToDownload, _totalBytesToDownload);
			yield return null;
		}
		while (!bFinished);
		Debug.Log("Download Finished. Disposing");
		DisposeDownloadWorker();
		_onEndAction?.Invoke(bErrorStop, lastError);
	}

	private void DisposeDownloadWorker()
	{
		if (lstWorker == null)
		{
			return;
		}
		foreach (NKCPatchParallelDownloadWorker item in lstWorker)
		{
			item.Dispose();
		}
		lstWorker.Clear();
	}

	public void EventDownloadFinished(PatchDownloadStatus downloadStatus, string errorString)
	{
		Log.Debug($"EventDownloadFinished : status {downloadStatus}, ErrorCode {errorString}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchParallelDownloader.cs", 292);
		base.DownloadStatus = downloadStatus;
		base.ErrorString = errorString;
		DownloadFinished(downloadStatus, errorString);
	}

	public override void Unload()
	{
		if (base.gameObject != null)
		{
			StopAllCoroutines();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected override void CheckVersionImpl(List<string> lstVariants)
	{
		if (!m_bInit)
		{
			CheckVersionImplAfterProcess(BuildStatus.Error, VersionStatus.Error, "Patcher not initialized");
			return;
		}
		Debug.Log("downloader Version Check Start");
		BeginCoroutine(this.StartCoroutine<string>(ProcessCheckVersion(lstVariants)), delegate(string s)
		{
			CheckVersionImplAfterProcess(BuildStatus.Error, VersionStatus.Error, s);
		});
	}

	private IEnumerator ProcessCheckVersion(List<string> lstVariants)
	{
		ClearFileDownloadContainer();
		FullBuildCheck();
		yield return DownloadListCheck(lstVariants, extra: false);
		foreach (NKCPatchInfo.PatchFileInfo download in base.DownloadList)
		{
			_totalBytesToDownload += download.Size;
		}
		OnEndVersionCheck();
		Log.Debug("[ProcessCheckVersion] DownLoader end of version  check", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchParallelDownloader.cs", 342);
	}

	private void OnDestroy()
	{
		DisposeDownloadWorker();
	}
}
