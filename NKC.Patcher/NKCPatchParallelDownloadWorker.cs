using System;
using System.IO;
using UnityEngine;

namespace NKC.Patcher;

public abstract class NKCPatchParallelDownloadWorker : IDisposable
{
	public enum eState
	{
		Idle,
		Busy,
		Retry,
		Error,
		Complete
	}

	protected string m_strRelativeSourcePath;

	protected string m_strRelativeTargetPath;

	protected string m_strServerBaseAddress;

	protected string m_strLocalBaseAddress;

	protected const float RETRY_WAIT_SECOND = 1f;

	protected float m_retryWaitTime;

	protected int m_retryCount;

	protected bool disposedValue;

	public string lastError { get; protected set; }

	public NKCPatchInfo.PatchFileInfo PatchFileInfo { get; protected set; }

	public eState State { get; protected set; }

	public NKCPatchParallelDownloadWorker(string serverBaseAddress, string localBaseAddress)
	{
		m_strServerBaseAddress = serverBaseAddress;
		m_strLocalBaseAddress = localBaseAddress;
	}

	public void StartDownloadFile(NKCPatchInfo.PatchFileInfo fileInfo)
	{
		PatchFileInfo = fileInfo;
		StartDownloadFile(fileInfo.FileName, fileInfo.FileName);
	}

	public abstract void StartDownloadFile(string relativeSourcePath, string relativeTargetPath);

	public abstract long Update();

	public virtual void RetryDownloadFile()
	{
		StartDownloadFile(m_strRelativeSourcePath, m_strRelativeTargetPath);
	}

	public abstract void Cleanup();

	protected void WaitAndRetry(string error)
	{
		lastError = error;
		Debug.LogError(m_strRelativeTargetPath + " : " + error);
		FileInfo fileInfo = new FileInfo(m_strLocalBaseAddress + m_strRelativeTargetPath);
		if (fileInfo.Exists)
		{
			fileInfo.Attributes = FileAttributes.Normal;
			fileInfo.Delete();
		}
		if (m_retryCount > 10)
		{
			State = eState.Error;
			return;
		}
		State = eState.Retry;
		m_retryWaitTime = 0f;
		m_retryCount++;
	}

	protected abstract void Dispose(bool disposing);

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
