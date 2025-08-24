using System.Collections.Generic;
using UnityEngine;

namespace NKC.Patcher;

public class DownloadHistoryController
{
	private readonly List<NKCPatchInfo.PatchFileInfo> _lstDownloadCompletedThisFrame = new List<NKCPatchInfo.PatchFileInfo>();

	private NKCPatchInfo _downloadHistoryPatchInfo;

	public long CurrentDownloadedCompletedSize { get; private set; }

	public void SetDownloadHistoryPatchInfo(NKCPatchInfo downloadHistoryPatchInfo)
	{
		_downloadHistoryPatchInfo = downloadHistoryPatchInfo;
	}

	public void CleanUp()
	{
		_lstDownloadCompletedThisFrame.Clear();
		CurrentDownloadedCompletedSize = 0L;
	}

	public void AddTo(NKCPatchInfo.PatchFileInfo patchFile)
	{
		if (!_lstDownloadCompletedThisFrame.Contains(patchFile))
		{
			_lstDownloadCompletedThisFrame.Add(patchFile);
		}
	}

	public void UpdateDownloadHistoryPatchInfo()
	{
		if (_downloadHistoryPatchInfo == null)
		{
			Debug.LogWarning("[UpdateDownloadHistoryPatchInfo] downloadHistoryInfo is null");
			return;
		}
		foreach (NKCPatchInfo.PatchFileInfo item in _lstDownloadCompletedThisFrame)
		{
			_downloadHistoryPatchInfo.AddPatchFileInfo(item);
			CurrentDownloadedCompletedSize += item.Size;
		}
		_downloadHistoryPatchInfo.SaveAsJSON();
		_lstDownloadCompletedThisFrame.Clear();
	}
}
