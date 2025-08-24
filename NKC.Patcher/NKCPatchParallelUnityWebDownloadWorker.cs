using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace NKC.Patcher;

public class NKCPatchParallelUnityWebDownloadWorker : NKCPatchParallelDownloadWorker
{
	private UnityWebRequest webRequest;

	private ulong m_DownloadedByte;

	private float m_Timeout;

	private const float TIMEOUT_SECOND = 30f;

	public NKCPatchParallelUnityWebDownloadWorker(string serverBaseAddress, string localBaseAddress)
		: base(serverBaseAddress, localBaseAddress)
	{
	}

	public override void StartDownloadFile(string relativeSourcePath, string relativeTargetPath)
	{
		if (base.State == eState.Busy)
		{
			Debug.LogError("Logic Error : Already working!");
			return;
		}
		m_strRelativeSourcePath = relativeSourcePath;
		m_strRelativeTargetPath = relativeTargetPath;
		base.State = eState.Busy;
		string url = m_strServerBaseAddress + relativeSourcePath;
		string path = m_strLocalBaseAddress + relativeTargetPath;
		if (webRequest != null)
		{
			webRequest.Dispose();
			webRequest = null;
		}
		webRequest = new UnityWebRequest(url);
		m_DownloadedByte = 0uL;
		m_Timeout = 0f;
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		webRequest.downloadHandler = new DownloadHandlerFile(path);
		webRequest.SendWebRequest();
	}

	public override long Update()
	{
		if (base.State == eState.Retry)
		{
			m_retryWaitTime += Time.unscaledDeltaTime;
			if (m_retryWaitTime > 1f)
			{
				m_retryWaitTime = 0f;
				RetryDownloadFile();
				return 0L;
			}
		}
		if (webRequest == null)
		{
			return 0L;
		}
		if (!webRequest.isDone)
		{
			if (m_DownloadedByte == webRequest.downloadedBytes)
			{
				m_Timeout += Time.unscaledDeltaTime;
				if (m_Timeout > 30f)
				{
					webRequest.Abort();
					webRequest.Dispose();
					webRequest = null;
					WaitAndRetry("Request timeout");
					return 0L;
				}
			}
			else
			{
				m_DownloadedByte = webRequest.downloadedBytes;
				m_Timeout = 0f;
			}
			return (long)webRequest.downloadedBytes;
		}
		if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
		{
			WaitAndRetry(webRequest.error);
		}
		else
		{
			OnDownloadFinished();
		}
		webRequest.Dispose();
		webRequest = null;
		return 0L;
	}

	private void OnDownloadFinished()
	{
		if (base.PatchFileInfo != null)
		{
			string fullPath = m_strLocalBaseAddress + m_strRelativeTargetPath;
			switch (base.PatchFileInfo.CheckFileIntegrity(fullPath))
			{
			case NKCPatchInfo.eFileIntergityStatus.ERROR_HASH:
				WaitAndRetry($"Integrity check failed : {m_strRelativeTargetPath}");
				break;
			case NKCPatchInfo.eFileIntergityStatus.ERROR_SIZE:
				WaitAndRetry($"size check failed : {m_strRelativeTargetPath}");
				break;
			default:
				WaitAndRetry("Integrity Check error : behaivor undefined");
				break;
			case NKCPatchInfo.eFileIntergityStatus.OK:
				base.State = eState.Complete;
				m_retryCount = 0;
				break;
			}
		}
		else
		{
			m_retryCount = 0;
			base.State = eState.Complete;
		}
	}

	public override void Cleanup()
	{
		if (webRequest != null)
		{
			webRequest.Dispose();
			webRequest = null;
		}
		base.State = eState.Idle;
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing && webRequest != null)
			{
				webRequest.Dispose();
				webRequest = null;
			}
			disposedValue = true;
		}
	}
}
