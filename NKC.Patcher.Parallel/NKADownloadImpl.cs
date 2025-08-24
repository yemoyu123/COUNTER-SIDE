using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cs.Logging;
using NKA.Service;
using UnityEngine;

namespace NKC.Patcher.Parallel;

public class NKADownloadImpl : MonoBehaviour, ILegacyImpl
{
	private string _sourceBaseAddress;

	private string _localDownloadBasePath;

	private Dictionary<string, NKCPatchInfo.PatchFileInfo> _downloadList;

	private int _downloadedCount;

	private NKCPatchInfo _historyPatchInfo;

	public long CurrentBytesToDownload { get; private set; }

	public GameObject ImplGameObject => base.gameObject;

	public void SetDownloadInfo(string serverPath, string localPath, HashSet<NKCPatchInfo.PatchFileInfo> downLoadList, NKCPatchInfo historyPatchInfo)
	{
		_localDownloadBasePath = localPath;
		_sourceBaseAddress = serverPath;
		CurrentBytesToDownload = 0L;
		_downloadedCount = 0;
		_downloadList = new Dictionary<string, NKCPatchInfo.PatchFileInfo>();
		foreach (NKCPatchInfo.PatchFileInfo downLoad in downLoadList)
		{
			_downloadList.Add(downLoad.FileName, downLoad);
		}
		_historyPatchInfo = historyPatchInfo;
		CreateDirectory();
		ServiceManager.DownloadService.StopDownload();
		ServiceManager.DownloadService.Init(localPath, serverPath, downLoadList, delegate(in FromService.DownloadedFileInfo downloadedFileInfo)
		{
			if (downloadedFileInfo.IsSuccess)
			{
				_downloadedCount++;
			}
			else
			{
				Log.Error("[NKADownloadImpl] download fail _ error: " + downloadedFileInfo.Error + " _ fileName : " + downloadedFileInfo.FileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKADownload/NKADownloadImpl.cs", 57);
			}
		});
	}

	public IEnumerator ProcessFileDownload(NKCPatchDownloader.OnDownloadProgress onDownloadProgress, NKCPatchDownloader.OnDownloadFinished onDownloadFinished)
	{
		Debug.Log("[NKADownloadImpl][ProcessFileDownload] start");
		ServiceManager.DownloadService.StartDownload();
		while (_downloadedCount < _downloadList.Count)
		{
			DispatchDownloadedInfo();
			yield return null;
		}
		onDownloadFinished(NKCPatchDownloader.PatchDownloadStatus.Finished, "");
		ServiceManager.DownloadService.StopDownload();
	}

	private void Add(string str)
	{
		_downloadedCount++;
		Debug.Log($"[NKADownloadImpl][ProcessFileDownload] downloaded! _ fileName: {str} _ count: {_downloadedCount}");
		if (_downloadList.TryGetValue(str, out var value))
		{
			CurrentBytesToDownload += value.Size;
			_historyPatchInfo.AddPatchFileInfo(value);
		}
		else
		{
			Log.Error("[NKADownloadImpl][ProcessFileDownload] not contain downloadlist ! _ fileName: " + str, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKADownload/NKADownloadImpl.cs", 92);
		}
	}

	private void CreateDirectory()
	{
		foreach (KeyValuePair<string, NKCPatchInfo.PatchFileInfo> download in _downloadList)
		{
			string directoryName = Path.GetDirectoryName(_localDownloadBasePath + download.Value.FileName);
			if (directoryName != null)
			{
				Directory.CreateDirectory(directoryName);
			}
		}
	}

	public void CleanUp()
	{
		CurrentBytesToDownload = 0L;
		_downloadList = null;
		_historyPatchInfo = null;
		ServiceManager.DownloadService.StopDownload();
	}

	public bool IsFileWillDownloaded(string str)
	{
		return _historyPatchInfo?.GetPatchInfo(str) != null;
	}

	public void OnDestroy()
	{
		CleanUp();
	}

	public void OnApplicationPause(bool pauseStatus)
	{
		Debug.Log($"[NKADownloadImpl][OnApplicationPause] pauseStatus _ {pauseStatus}");
		ServiceManager.OnPause(pauseStatus);
	}

	private void DispatchDownloadedInfo()
	{
		string downloadedFileInfo = ServiceManager.DownloadService.GetDownloadedFileInfo();
		while (!string.IsNullOrEmpty(downloadedFileInfo))
		{
			Add(downloadedFileInfo);
			downloadedFileInfo = ServiceManager.DownloadService.GetDownloadedFileInfo();
		}
		_historyPatchInfo.SaveAsJSON();
	}
}
