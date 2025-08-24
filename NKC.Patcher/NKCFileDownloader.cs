using System;
using System.Collections;

namespace NKC.Patcher;

public abstract class NKCFileDownloader : IDisposable
{
	public delegate void OnDownloadProgressUpdated(long currentBytes, long totalBytes);

	public delegate void OnDownloadCompleted(bool bSucceed);

	public OnDownloadProgressUpdated dOnDownloadProgressUpdated;

	public OnDownloadCompleted dOnDownloadCompleted;

	protected string m_strServerBaseAddress;

	protected string m_strLocalBaseAddress;

	protected bool disposedValue;

	public NKCFileDownloader(string ServerBaseAddress, string localDownloadBaseAddress)
	{
		Initialize(ServerBaseAddress, localDownloadBaseAddress);
	}

	public void Initialize(string svrBaseAddress, string localBaseAddr)
	{
		if (svrBaseAddress.EndsWith("/"))
		{
			m_strServerBaseAddress = svrBaseAddress;
		}
		else
		{
			m_strServerBaseAddress = svrBaseAddress + "/";
		}
		if (localBaseAddr.EndsWith("/"))
		{
			m_strLocalBaseAddress = localBaseAddr;
		}
		else
		{
			m_strLocalBaseAddress = localBaseAddr + "/";
		}
	}

	protected void OnProgress(long currentBytes, long totalBytes)
	{
		if (dOnDownloadProgressUpdated != null)
		{
			dOnDownloadProgressUpdated(currentBytes, totalBytes);
		}
	}

	protected void OnComplete(bool bSucceed)
	{
		if (dOnDownloadCompleted != null)
		{
			dOnDownloadCompleted(bSucceed);
		}
	}

	public string GetLocalFullPath(string relativeTargetPath)
	{
		return m_strLocalBaseAddress + relativeTargetPath;
	}

	public abstract IEnumerator DownloadFile(string relativeDownloadPath, string relativeTargetPath, long targetFileSize);

	protected abstract void Dispose(bool disposing);

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
