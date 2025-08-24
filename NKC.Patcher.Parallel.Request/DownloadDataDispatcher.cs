using System;
using System.Collections;
using System.Collections.Concurrent;
using Cs.Logging;

namespace NKC.Patcher.Parallel.Request;

public class DownloadDataDispatcher
{
	private class DownloadedData
	{
		public string LocalPath { get; private set; }

		public NKCPatchInfo.PatchFileInfo FileInfo { get; private set; }

		public string Error { get; private set; }

		public bool IsValid => Error == "Success";

		public void Init(string localPath, NKCPatchInfo.PatchFileInfo fileInfo, string error)
		{
			LocalPath = localPath;
			FileInfo = fileInfo;
			Error = error;
		}
	}

	private readonly ConcurrentQueue<DownloadedData> _downloadedQueue = new ConcurrentQueue<DownloadedData>();

	private readonly ConcurrentQueue<DownloadedData> _unusedQueue = new ConcurrentQueue<DownloadedData>();

	private NKCPatchInfo _downloadHistoryPatchInfo;

	private bool _paused;

	private int _count;

	private const int _yieldMaxCount = 10;

	public int Count => _count;

	public void SetHistoryFile(NKCPatchInfo historyPatchInfo)
	{
		_count = 0;
		_downloadHistoryPatchInfo = historyPatchInfo;
	}

	public void Pause()
	{
		_paused = true;
	}

	public void OnDownloadComplete(string localPath, string error, NKCPatchInfo.PatchFileInfo fileInfo)
	{
		DownloadedData unusedData = GetUnusedData();
		unusedData.Init(localPath, fileInfo, error);
		_downloadedQueue.Enqueue(unusedData);
	}

	public IEnumerator Update(Action<long, string> onDownloading, bool yieldReturn)
	{
		if (_downloadedQueue.Count == 0)
		{
			yield return null;
			yield break;
		}
		int loopCount = 0;
		bool isChanged = false;
		while (_downloadedQueue.Count > 0)
		{
			if (_downloadedQueue.TryDequeue(out var result))
			{
				if (result.FileInfo == null)
				{
					Log.Error("[DownloadedData] what the..", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/Parallal/Request/DownloadDataDispatcher.cs", 66);
					ReturnToUnusedQueue(result);
					continue;
				}
				if (result.IsValid)
				{
					isChanged = true;
					_downloadHistoryPatchInfo.AddPatchFileInfo(result.FileInfo);
				}
				else
				{
					Log.Error(result.Error, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/Parallal/Request/DownloadDataDispatcher.cs", 78);
				}
				_count++;
				onDownloading?.Invoke(result.FileInfo.Size, result.Error);
				ReturnToUnusedQueue(result);
			}
			loopCount++;
			if (!_paused && yieldReturn && loopCount % 10 == 0)
			{
				yield return null;
			}
		}
		if (_paused)
		{
			Log.Debug($"[DownloadedDataContainer] loopCount: {loopCount}, puased: {_paused}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/Parallal/Request/DownloadDataDispatcher.cs", 108);
			_paused = false;
		}
		if (isChanged)
		{
			_downloadHistoryPatchInfo.SaveAsJSON();
		}
	}

	private void ReturnToUnusedQueue(DownloadedData data)
	{
		data.Init(string.Empty, null, string.Empty);
		_unusedQueue.Enqueue(data);
	}

	private DownloadedData GetUnusedData()
	{
		if (!_unusedQueue.TryDequeue(out var result))
		{
			return new DownloadedData();
		}
		return result;
	}

	public void Clear()
	{
		do
		{
			_downloadedQueue.TryDequeue(out var _);
		}
		while (!_downloadedQueue.IsEmpty);
		do
		{
			_unusedQueue.TryDequeue(out var _);
		}
		while (!_unusedQueue.IsEmpty);
	}
}
