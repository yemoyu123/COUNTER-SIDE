using System;
using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKC.Patcher;
using UnityEngine;

namespace NKA.Service.Android;

internal class DownloadServiceImpl : MonoBehaviour, IService
{
	private int _downloadedHandle;

	private static DownloadServiceImpl _instance;

	internal static DownloadServiceImpl Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject obj = new GameObject
				{
					name = "DownloadServiceObject"
				};
				_instance = obj.AddComponent<DownloadServiceImpl>();
				UnityEngine.Object.DontDestroyOnLoad(obj);
			}
			return _instance;
		}
	}

	public void BindService()
	{
		if (!IsValid())
		{
			return;
		}
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager");
		Log.Debug("[DownloadServiceImpl] BindDownloadService", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/DownloadServiceImpl.cs", 44);
		androidJavaClass.CallStatic("BindDownloadService");
	}

	public void UnbindService()
	{
		if (!IsValid())
		{
			return;
		}
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager");
		Log.Debug("[DownloadServiceImpl] UnbindService", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/DownloadServiceImpl.cs", 58);
		androidJavaClass.CallStatic("UnbindService");
	}

	public void OnPause(bool pauseState)
	{
		if (!IsValid())
		{
			return;
		}
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager");
		androidJavaClass.CallStatic("OnPause", pauseState);
	}

	public bool IsValid()
	{
		return NKCDefineManager.USE_ANDROIDSERVICE();
	}

	public void InitDownloadService(string downloadPath, string serverPath, HashSet<NKCPatchInfo.PatchFileInfo> files, CallBackMethod.NKAServiceReadOnlyDelegate<FromService.DownloadedFileInfo> downloadedFileInfoCallBack)
	{
		if (!IsValid())
		{
			return;
		}
		if (_downloadedHandle != 0)
		{
			CallBackHandler.UnregisterCallback(_downloadedHandle);
		}
		_downloadedHandle = CallBackHandler.RegisterCallBack(downloadedFileInfoCallBack);
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager");
		androidJavaClass.CallStatic("InitDownloader", downloadPath, serverPath, NKCUtilString.GET_STRING_PATCHER_DOWNLOADING, NKCUtilString.GET_NXPATCHER_DOWNLOAD_COMPLETE);
		foreach (NKCPatchInfo.PatchFileInfo file in files)
		{
			androidJavaClass.CallStatic("AddDownloadFile", file.FileName, file.Size);
		}
	}

	public void StartDownloadService()
	{
		if (!IsValid())
		{
			return;
		}
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager");
		Log.Debug("[DownloadServiceImpl] StartDownload", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/DownloadServiceImpl.cs", 120);
		androidJavaClass.CallStatic("StartDownload");
	}

	public void StopDownload()
	{
		if (!IsValid())
		{
			return;
		}
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager");
		Log.Debug("[DownloadServiceImpl] StopDownload", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/DownloadServiceImpl.cs", 134);
		androidJavaClass.CallStatic("StopDownload");
	}

	public string GetDownloadedFileInfo()
	{
		if (!IsValid())
		{
			return string.Empty;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager"))
		{
			string text = androidJavaClass.CallStatic<string>("GetDownloadFile", Array.Empty<object>());
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
		}
		return string.Empty;
	}
}
