using System.Collections.Generic;
using Cs.Logging;
using NKA.Service.Android;
using NKC;
using NKC.Patcher;
using UnityEngine;

namespace NKA.Service;

public class ServiceManager
{
	public static class DownloadService
	{
		public static void Init(string downloadPath, string serverPath, HashSet<NKCPatchInfo.PatchFileInfo> files, CallBackMethod.NKAServiceReadOnlyDelegate<FromService.DownloadedFileInfo> downloadedFileInfoCallBack)
		{
			if (!(downloadServiceImpl == null))
			{
				downloadServiceImpl.InitDownloadService(downloadPath, serverPath, files, downloadedFileInfoCallBack);
			}
		}

		public static void BindService()
		{
			if (!(downloadServiceImpl == null))
			{
				downloadServiceImpl.BindService();
			}
		}

		public static void UnbindService()
		{
			if (!(downloadServiceImpl == null))
			{
				downloadServiceImpl.UnbindService();
			}
		}

		public static void StartDownload()
		{
			if (!(downloadServiceImpl == null))
			{
				downloadServiceImpl.StartDownloadService();
			}
		}

		public static void StopDownload()
		{
			if (!(downloadServiceImpl == null))
			{
				downloadServiceImpl.StopDownload();
			}
		}

		public static string GetDownloadedFileInfo()
		{
			if (downloadServiceImpl == null)
			{
				return string.Empty;
			}
			return downloadServiceImpl.GetDownloadedFileInfo();
		}
	}

	public const string PackageName = "com.studiobside.nkaservice.ServiceManager";

	private static DownloadServiceImpl downloadServiceImpl;

	private static List<IService> listService = new List<IService>();

	public static void Init()
	{
		if (NKCDefineManager.USE_ANDROIDSERVICE())
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager"))
			{
				Log.Debug("[ServiceManager] InitActivity", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/ServiceManager.cs", 26);
				androidJavaClass.CallStatic("InitActivity");
				downloadServiceImpl = DownloadServiceImpl.Instance;
				listService.Add(downloadServiceImpl);
			}
		}
	}

	public static void BindAllService()
	{
		foreach (IService item in listService)
		{
			item?.BindService();
		}
	}

	public static void UnbindAllService()
	{
		foreach (IService item in listService)
		{
			item?.UnbindService();
		}
	}

	public static void OnPause(bool pauseState)
	{
		foreach (IService item in listService)
		{
			item?.OnPause(pauseState);
		}
	}
}
