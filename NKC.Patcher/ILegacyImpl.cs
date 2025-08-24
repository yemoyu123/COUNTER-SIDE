using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKC.Patcher;

public interface ILegacyImpl
{
	long CurrentBytesToDownload { get; }

	GameObject ImplGameObject { get; }

	void SetDownloadInfo(string serverPath, string localPath, HashSet<NKCPatchInfo.PatchFileInfo> downLoadList, NKCPatchInfo historyPatchInfo);

	IEnumerator ProcessFileDownload(NKCPatchDownloader.OnDownloadProgress onDownloadProgress, NKCPatchDownloader.OnDownloadFinished onDownloadFinished);

	void CleanUp();

	bool IsFileWillDownloaded(string str);
}
