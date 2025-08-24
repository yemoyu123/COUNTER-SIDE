using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NKC.Patcher.Parallel.Request;

public class HttpDownloadRequest
{
	private volatile int _isBusy;

	private readonly HttpClient _httpClient;

	private string _url;

	private string _localPath;

	private const int _retryMaxCount = 5;

	private volatile int _retryCount;

	private readonly Action<string, string, NKCPatchInfo.PatchFileInfo> _onEncAction;

	private NKCPatchInfo.PatchFileInfo _patchFileInfo;

	public HttpDownloadRequest(Action<string, string, NKCPatchInfo.PatchFileInfo> onEndAction)
	{
		_onEncAction = onEndAction;
		_httpClient = new HttpClient();
		_httpClient.Timeout = Timeout.InfiniteTimeSpan;
	}

	public void SetDownloadInfo(string url, string localPath, NKCPatchInfo.PatchFileInfo patchFileInfo)
	{
		_url = url;
		_localPath = localPath;
		_patchFileInfo = patchFileInfo;
		Interlocked.Exchange(ref _retryCount, 0);
	}

	public async Task<bool> WaitForDownload()
	{
		_ = 3;
		try
		{
			HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(_url).ConfigureAwait(continueOnCapturedContext: true);
			using (httpResponseMessage)
			{
				if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
				{
					return RetryCountCAS();
				}
				using HttpContent content = httpResponseMessage.Content;
				byte[] array = await content.ReadAsByteArrayAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (array.Length != _patchFileInfo.Size)
				{
					return RetryCountCAS();
				}
				using (FileStream stream = new FileStream(_localPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
				{
					await stream.WriteAsync(array, 0, array.Length).ConfigureAwait(continueOnCapturedContext: false);
					await stream.FlushAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
				if (_patchFileInfo.CheckFileIntegrity(_localPath) != NKCPatchInfo.eFileIntergityStatus.OK)
				{
					return RetryCountCAS();
				}
				_onEncAction?.Invoke(_localPath, "Success", _patchFileInfo);
				return true;
			}
		}
		catch (Exception ex)
		{
			_onEncAction?.Invoke(_localPath, ex.ToString(), _patchFileInfo);
			return true;
		}
		bool RetryCountCAS()
		{
			if (Interlocked.CompareExchange(ref _retryCount, _retryCount + 1, _retryCount) == 5)
			{
				_onEncAction?.Invoke(_localPath, "[HttpDownloadRequest] Retry fail. fileName: " + _patchFileInfo.FileName + ")", _patchFileInfo);
				return true;
			}
			return false;
		}
	}

	public void Free()
	{
		Interlocked.Exchange(ref _retryCount, 0);
		Interlocked.Exchange(ref _isBusy, 0);
	}

	public bool ExchangeToBusy()
	{
		return Interlocked.CompareExchange(ref _isBusy, 1, 0) == 0;
	}
}
