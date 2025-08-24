using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cs.Logging;
using NKC.Patcher.Parallel.Request;
using UnityEngine;

namespace NKC.Patcher.Parallel;

public class MultiTaskDownloaderImpl : MonoBehaviour, ILegacyImpl
{
	private string _sourceBaseAddress;

	private string _localDownloadBasePath;

	private HashSet<NKCPatchInfo.PatchFileInfo> _downloadList;

	private NKCPatchInfo _downloadHistoryPatchInfo;

	private HttpWebRequestPool _webRequestPool;

	private readonly DownloadDataDispatcher _dispatcher = new DownloadDataDispatcher();

	private CancellationTokenSource _cts;

	private CancellationToken _token;

	private int _downloadSeparationCount = 1;

	public long CurrentBytesToDownload { get; private set; }

	public GameObject ImplGameObject => base.gameObject;

	public void CleanUp()
	{
		CancelDownloadTask();
		CurrentBytesToDownload = 0L;
		_dispatcher.Clear();
		_downloadList = null;
		_downloadHistoryPatchInfo = null;
	}

	public bool IsFileWillDownloaded(string filePath)
	{
		if (_downloadList != null)
		{
			return _downloadList.Any((NKCPatchInfo.PatchFileInfo file) => file.FileName == filePath);
		}
		return false;
	}

	public void SetDownloadInfo(string serverPath, string localPath, HashSet<NKCPatchInfo.PatchFileInfo> downLoadList, NKCPatchInfo historyPatchInfo)
	{
		_downloadSeparationCount = (NKCDefineManager.DEFINE_SERVICE() ? 1 : 3);
		_sourceBaseAddress = serverPath;
		_localDownloadBasePath = localPath;
		_downloadList = downLoadList;
		_downloadHistoryPatchInfo = historyPatchInfo;
	}

	private void OnDownloaded(long downloadedSize, string error)
	{
		CurrentBytesToDownload += downloadedSize;
	}

	private void CancelDownloadTask()
	{
		if (_cts != null && !_cts.IsCancellationRequested)
		{
			_cts.Cancel();
			_cts.Dispose();
		}
	}

	private void Download()
	{
		Log.Debug($"[MultiTaskDownloader] TaskCount: {_downloadSeparationCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/Parallal/MultiTaskDownloaderImpl.cs", 75);
		int num = _downloadList.Count / _downloadSeparationCount;
		List<NKCPatchInfo.PatchFileInfo> list = _downloadList.ToList();
		for (int i = 0; i < _downloadSeparationCount; i++)
		{
			int num2 = i * num;
			int num3 = ((i == _downloadSeparationCount - 1) ? list.Count : ((i + 1) * num));
			List<NKCPatchInfo.PatchFileInfo> filesForTask = list.GetRange(num2, num3 - num2);
			Task.Run(async delegate
			{
				foreach (NKCPatchInfo.PatchFileInfo item in filesForTask)
				{
					_token.ThrowIfCancellationRequested();
					await _webRequestPool.DownloadAsync(item);
				}
			}, _token);
		}
	}

	private void CreateDirectory()
	{
		foreach (NKCPatchInfo.PatchFileInfo download in _downloadList)
		{
			string directoryName = Path.GetDirectoryName(_localDownloadBasePath + download.FileName);
			if (directoryName != null)
			{
				Directory.CreateDirectory(directoryName);
			}
		}
	}

	public IEnumerator ProcessFileDownload(NKCPatchDownloader.OnDownloadProgress onDownloadProgress, NKCPatchDownloader.OnDownloadFinished onDownloadFinished)
	{
		if (_downloadList != null)
		{
			Log.Debug("[MultiTaskDownloader][ProcessFileDownload] Start", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/Parallal/MultiTaskDownloaderImpl.cs", 118);
			_dispatcher.SetHistoryFile(_downloadHistoryPatchInfo);
			_webRequestPool = new HttpWebRequestPool(_sourceBaseAddress, _localDownloadBasePath, _downloadSeparationCount, _dispatcher.OnDownloadComplete);
			_cts = new CancellationTokenSource();
			_token = _cts.Token;
			CreateDirectory();
			Download();
			while (_dispatcher.Count < _downloadList.Count)
			{
				yield return _dispatcher.Update(OnDownloaded, yieldReturn: true);
			}
			Log.Debug($"[MultiTaskDownloader][ProcessFileDownload] DownloadCount: {_dispatcher.Count} / FieldDownloadCount : {_downloadList.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/Parallal/MultiTaskDownloaderImpl.cs", 133);
			onDownloadFinished?.Invoke(NKCPatchDownloader.PatchDownloadStatus.Finished, "");
		}
	}

	public void OnDestroy()
	{
		CleanUp();
	}

	public void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			_dispatcher.Pause();
		}
	}
}
