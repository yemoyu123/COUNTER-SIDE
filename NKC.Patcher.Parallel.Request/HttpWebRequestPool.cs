using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NKC.Patcher.Parallel.Request;

public class HttpWebRequestPool
{
	private readonly List<HttpDownloadRequest> _weqRequests = new List<HttpDownloadRequest>();

	private readonly string _sourceBaseAddress;

	private readonly string _localDownloadBasePath;

	public HttpWebRequestPool(string sourcePath, string localPath, int webRequestCount, Action<string, string, NKCPatchInfo.PatchFileInfo> onDownloadComplete)
	{
		_sourceBaseAddress = sourcePath;
		_localDownloadBasePath = localPath;
		for (int i = 0; i < webRequestCount; i++)
		{
			_weqRequests.Add(new HttpDownloadRequest(onDownloadComplete));
		}
	}

	private HttpDownloadRequest GetFreeRequest(string webRequestPath, string localPath, string fileName, string hash, long size)
	{
		foreach (HttpDownloadRequest weqRequest in _weqRequests)
		{
			if (weqRequest.ExchangeToBusy())
			{
				weqRequest.SetDownloadInfo(webRequestPath, localPath, new NKCPatchInfo.PatchFileInfo(fileName, hash, size));
				return weqRequest;
			}
		}
		return null;
	}

	public async Task DownloadAsync(NKCPatchInfo.PatchFileInfo fileInfo)
	{
		string webRequestPath = _sourceBaseAddress + fileInfo.FileName;
		string localPath = _localDownloadBasePath + fileInfo.FileName;
		HttpDownloadRequest downloadRequest;
		for (downloadRequest = GetFreeRequest(webRequestPath, localPath, fileInfo.FileName, fileInfo.Hash, fileInfo.Size); downloadRequest == null; downloadRequest = GetFreeRequest(webRequestPath, localPath, fileInfo.FileName, fileInfo.Hash, fileInfo.Size))
		{
		}
		while (!(await downloadRequest.WaitForDownload().ConfigureAwait(continueOnCapturedContext: true)))
		{
		}
		downloadRequest.Free();
	}
}
