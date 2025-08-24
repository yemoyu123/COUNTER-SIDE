using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using AssetBundles;
using Cs.Logging;
using UnityEngine;

namespace NKC;

public class NKCObbUtil
{
	public class OBB_Info
	{
		public Dictionary<string, OBB_EntryInfo> dicObb_EntryInfo = new Dictionary<string, OBB_EntryInfo>();

		public List<string> lstFileName = new List<string>();

		public List<ulong> lstUncompressedFileSize = new List<ulong>();

		public List<ulong> lstLocalOffset = new List<ulong>();

		public BinaryReader br;
	}

	public class OBB_EntryInfo
	{
		public int index;

		public ulong size;
	}

	public enum OBB_TYPE
	{
		main,
		patch,
		num
	}

	private static bool _loadedObb = false;

	public static string s_OBBFullPath = "";

	public const string CS_OBB_FILE = "CS_OBB_FILE/";

	public const int CS_OBB_FILE_COUNT = 12;

	private const string _logHeader = "NKCObbUtil";

	private static List<OBB_Info> s_lst_dicOBB = new List<OBB_Info>();

	private static int s_OBB_Version = 0;

	public static bool s_bLoadedOBB
	{
		get
		{
			return _loadedObb;
		}
		private set
		{
			_loadedObb = value;
			Log.Debug($"[NKCObbUtil] loaded : {_loadedObb}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Utility/NKCObbUtil.cs", 21);
		}
	}

	public static bool IsOBBPath(string path)
	{
		if (s_bLoadedOBB)
		{
			return path.Contains("CS_OBB_FILE/");
		}
		return false;
	}

	private static void InitOBB_EntryIndex_(OBB_TYPE obbType)
	{
		DebugLog("Start", "InitOBB_EntryIndex_");
		string.IsNullOrWhiteSpace(GetOBBFullPath(obbType.ToString()));
	}

	public static void Init()
	{
		Log.Debug("[NKCObbUtil] Init", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Utility/NKCObbUtil.cs", 110);
		InitOBB_EntryIndex();
	}

	public static void ExtractFile(string relativePath)
	{
		string localDownloadPath = AssetBundleManager.GetLocalDownloadPath();
		for (int i = 0; i < 2; i++)
		{
			if (!s_lst_dicOBB[i].dicObb_EntryInfo.TryGetValue(relativePath, out var value))
			{
				continue;
			}
			string text = Path.Combine(localDownloadPath, relativePath);
			string directoryName = Path.GetDirectoryName(text);
			DebugLog("[fullPath:" + text + "][relativePath:" + relativePath + "]", "ExtractFile");
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			try
			{
				File.WriteAllBytes(text, GetEntryBufferByIndex((OBB_TYPE)i, value.index));
				break;
			}
			catch (Exception ex)
			{
				ErrorLog("[fullPath:" + text + "][ExceptionMessage:" + ex.Message + "] obb extract error exception", "ExtractFile");
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				break;
			}
		}
	}

	public static void InitOBB_EntryIndex()
	{
		if (s_lst_dicOBB.Count <= 0)
		{
			for (int i = 0; i < 2; i++)
			{
				s_lst_dicOBB.Add(new OBB_Info());
			}
			s_bLoadedOBB = false;
			for (int j = 0; j < 2; j++)
			{
				InitOBB_EntryIndex_((OBB_TYPE)j);
			}
		}
	}

	public static OBB_TYPE GetObbType(string relativePath)
	{
		for (int i = 0; i < 2; i++)
		{
			if (s_lst_dicOBB[i].dicObb_EntryInfo.ContainsKey(relativePath))
			{
				return (OBB_TYPE)i;
			}
		}
		return OBB_TYPE.num;
	}

	public static int GetObbEntryIndex(string relativePath)
	{
		return GetObbEntryIndex(GetObbType(relativePath), relativePath);
	}

	public static int GetObbEntryIndex(OBB_TYPE obbType, string relativePath)
	{
		if (obbType == OBB_TYPE.num)
		{
			return -1;
		}
		for (int i = 0; i < 2; i++)
		{
			if (s_lst_dicOBB[i].dicObb_EntryInfo.TryGetValue(relativePath, out var value))
			{
				return value.index;
			}
		}
		return -1;
	}

	public static ulong GetObbEntrySize(string relativePath)
	{
		return GetObbEntrySize(GetObbType(relativePath), relativePath);
	}

	public static ulong GetObbEntrySize(OBB_TYPE obbType, string relativePath)
	{
		if (obbType == OBB_TYPE.num)
		{
			return 0uL;
		}
		if (!s_lst_dicOBB[(int)obbType].dicObb_EntryInfo.TryGetValue(relativePath, out var value))
		{
			return 0uL;
		}
		return value.size;
	}

	public static string GetRelativePathFromOBB(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return "";
		}
		return path.Remove(0, 12);
	}

	public static byte[] GetEntryBufferByFullPath(string fullPath)
	{
		string relativePathFromOBB = GetRelativePathFromOBB(fullPath);
		OBB_TYPE obbType = GetObbType(relativePathFromOBB);
		int obbEntryIndex = GetObbEntryIndex(obbType, relativePathFromOBB);
		return GetEntryBufferByIndex(obbType, obbEntryIndex);
	}

	private static byte[] GetEntryBufferByIndex(OBB_TYPE obbType, int index)
	{
		if (obbType == OBB_TYPE.num)
		{
			return null;
		}
		if (index < 0)
		{
			return null;
		}
		_ = s_lst_dicOBB[(int)obbType]?.br;
		return null;
	}

	private static int GetAppBundleVersion()
	{
		if (s_OBB_Version > 0)
		{
			return s_OBB_Version;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			s_OBB_Version = androidJavaObject.Call<AndroidJavaObject>("getPackageManager", Array.Empty<object>()).Call<AndroidJavaObject>("getPackageInfo", new object[2]
			{
				androidJavaObject.Call<string>("getPackageName", Array.Empty<object>()),
				0
			}).Get<int>("versionCode");
		}
		return s_OBB_Version;
	}

	private static string GetOBBFullPath(string obbType)
	{
		return GetAndroidInternalFileFullPath("Android/obb/" + Application.identifier + "/" + obbType + "." + GetAppBundleVersion() + "." + Application.identifier + ".obb");
	}

	public static string GetAndroidInternalFileFullPath(string relativePath)
	{
		string[] array = new string[6] { "/storage", "/sdcard", "/storage/emulated/0", "/mnt/sdcard", "/storage/sdcard0", "/storage/sdcard1" };
		if (Application.platform == RuntimePlatform.Android)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (Directory.Exists(array[i]))
				{
					string text = array[i] + "/" + relativePath;
					if (File.Exists(text))
					{
						DebugLog("[fullPath:" + text + "] Exist Path!", "GetAndroidInternalFileFullPath");
						return text;
					}
				}
			}
		}
		DebugLog("[relativePath:" + relativePath + "] not found full obb path", "GetAndroidInternalFileFullPath");
		return "";
	}

	private static void DebugLog(string log, [CallerMemberName] string caller = "")
	{
		Log.Debug("[NKCObbUtil][" + caller + "] _ " + log, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Utility/NKCObbUtil.cs", 391);
	}

	private static void ErrorLog(string log, [CallerMemberName] string caller = "")
	{
		Log.Error("[NKCObbUtil][" + caller + "] _ " + log, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Utility/NKCObbUtil.cs", 397);
	}
}
