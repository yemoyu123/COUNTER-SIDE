using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Cs.Logging;
using UnityEngine;

namespace NKC.Patcher;

public class LegacyPatchDownloaderImpl : MonoBehaviour, ILegacyImpl
{
	private string _sourceBaseAddress;

	private string _localDownloadBasePath;

	private string _errorString;

	private long m_CurrentDownloadedCompletedSize;

	private HashSet<NKCPatchInfo.PatchFileInfo> _filesToDownload;

	private NKCPatchInfo _downloadHistoryPatchInfo;

	private NKCPatchDownloader.OnDownloadProgress _onDownloadProgress;

	private int retryCount;

	private const int MAX_RETRY_COUNT = 10;

	public long TotalBytesToDownload { get; private set; }

	public long CurrentBytesToDownload { get; private set; }

	public GameObject ImplGameObject => base.gameObject;

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	public void SetDownloadInfo(string serverPath, string localPath, HashSet<NKCPatchInfo.PatchFileInfo> downLoadList, NKCPatchInfo historyPatchInfo)
	{
		SetPath(serverPath, localPath);
		SetDownLoadList(downLoadList);
		SetDownloadHistoryPatchInfo(historyPatchInfo);
	}

	private void SetDownLoadList(HashSet<NKCPatchInfo.PatchFileInfo> downLoadList)
	{
		_filesToDownload = null;
		_filesToDownload = downLoadList;
	}

	private void SetDownloadHistoryPatchInfo(NKCPatchInfo historyPatchInfo)
	{
		_downloadHistoryPatchInfo = historyPatchInfo;
	}

	private void SetPath(string serverPath, string localPath)
	{
		_sourceBaseAddress = serverPath;
		_localDownloadBasePath = localPath;
	}

	public IEnumerator ProcessFileDownload(NKCPatchDownloader.OnDownloadProgress onDownloadProgress, NKCPatchDownloader.OnDownloadFinished onDownloadFinished)
	{
		if (_filesToDownload == null)
		{
			Log.Warn("[ProcessFileDownload] Download list is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloaderImpl.cs", 59);
			yield break;
		}
		m_CurrentDownloadedCompletedSize = 0L;
		_onDownloadProgress = onDownloadProgress;
		using (NKCFileDownloader downloader = new UnityWebRequestDownloader(_sourceBaseAddress, _localDownloadBasePath))
		{
			downloader.dOnDownloadProgressUpdated = (NKCFileDownloader.OnDownloadProgressUpdated)Delegate.Combine(downloader.dOnDownloadProgressUpdated, new NKCFileDownloader.OnDownloadProgressUpdated(OnFileDownloadProgress));
			foreach (NKCPatchInfo.PatchFileInfo targetFile in _filesToDownload)
			{
				yield return DownloadFile(downloader, targetFile.FileName, targetFile.FileName, targetFile);
				if (!string.IsNullOrEmpty(_errorString))
				{
					Log.Error("[ProcessFileDownload] Download fail _ [error:" + _errorString + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/LegacyPatchDownloaderImpl.cs", 77);
					_errorString = string.Empty;
				}
				else
				{
					_downloadHistoryPatchInfo.AddPatchFileInfo(targetFile);
					_downloadHistoryPatchInfo.SaveAsJSON();
					m_CurrentDownloadedCompletedSize += targetFile.Size;
				}
			}
		}
		CurrentBytesToDownload = m_CurrentDownloadedCompletedSize;
		onDownloadFinished?.Invoke(NKCPatchDownloader.PatchDownloadStatus.Finished, "");
	}

	public void CleanUp()
	{
	}

	private void OnFileDownloadProgress(long currentByte, long maxByte)
	{
		CurrentBytesToDownload = m_CurrentDownloadedCompletedSize + currentByte;
		_onDownloadProgress?.Invoke(CurrentBytesToDownload, TotalBytesToDownload);
	}

	private IEnumerator DownloadFile(NKCFileDownloader downloader, string relativeDownloadPath, string relativeTargetPath, NKCPatchInfo.PatchFileInfo fileInfo)
	{
		Exception ex2;
		while (true)
		{
			Coroutine<string> routine = this.StartCoroutine<string>(downloader.DownloadFile(relativeDownloadPath, relativeTargetPath, fileInfo?.Size ?? 0));
			yield return routine.coroutine;
			try
			{
				_ = routine.Value;
				if (fileInfo != null)
				{
					string localFullPath = downloader.GetLocalFullPath(relativeTargetPath);
					switch (fileInfo.CheckFileIntegrity(localFullPath))
					{
					case NKCPatchInfo.eFileIntergityStatus.ERROR_HASH:
						throw new Exception("Integrity check failed : " + localFullPath);
					case NKCPatchInfo.eFileIntergityStatus.ERROR_SIZE:
						throw new Exception("size check failed : " + localFullPath);
					default:
						throw new NotImplementedException("Integrity Check behaivor undefined");
					case NKCPatchInfo.eFileIntergityStatus.OK:
						break;
					}
				}
				retryCount = 0;
				yield break;
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					string message = $"{((HttpWebResponse)ex.Response).StatusCode} : {((HttpWebResponse)ex.Response).StatusDescription}";
					Debug.LogError(message);
					ex2 = new Exception(message, ex);
				}
				else
				{
					ex2 = new Exception(ex.Message, ex);
				}
			}
			catch (Exception ex3)
			{
				ex2 = ex3;
			}
			if (retryCount >= 10)
			{
				break;
			}
			Debug.LogError(ex2);
			retryCount++;
			yield return new WaitForSecondsRealtime(1f);
		}
		retryCount = 0;
		Debug.LogError(ex2);
		_errorString = string.Format(NKCUtilString.GET_STRING_ERROR_DOWNLOAD_ONE_PARAM, ex2.Message);
	}

	public bool IsFileWillDownloaded(string filePath)
	{
		if (_filesToDownload == null)
		{
			return false;
		}
		foreach (NKCPatchInfo.PatchFileInfo item in _filesToDownload)
		{
			if (item.FileName == filePath)
			{
				return true;
			}
		}
		return false;
	}
}
