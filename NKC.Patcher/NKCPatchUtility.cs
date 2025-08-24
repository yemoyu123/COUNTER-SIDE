using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AssetBundles;
using Cs.Logging;
using NKM;
using UnityEngine;

namespace NKC.Patcher;

public static class NKCPatchUtility
{
	public struct FileFilterInfo
	{
		public string originalName;

		public string variant;

		public FileFilterInfo(string _name, string _variant)
		{
			originalName = _name;
			variant = _variant;
		}
	}

	public class VariantSetInfo
	{
		public List<string> lstUseVariant;

		public VariantSetInfo()
		{
			lstUseVariant = new List<string>();
		}

		public VariantSetInfo(List<string> lstVariant)
		{
			lstUseVariant = lstVariant;
		}
	}

	public const string PREF_KEY_TUTORIAL_CLEARED = "PKTutorialCleared";

	private const string PREF_KEY_RESERVE_SKIP_TEST = "PKSkipTestReserve";

	public static bool _enablePatchOptimizationInEditor
	{
		get
		{
			int num = PlayerPrefs.GetInt("ENABLE_PATCH_OPTIMIZATION", 0);
			Log.Debug($"[PATCH_OPTIMIZATION] get _enablePatchOptimizationInEditor[{num}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 584);
			if (num != 1)
			{
				return false;
			}
			return true;
		}
		set
		{
			int num = (value ? 1 : 0);
			Log.Debug($"[PATCH_OPTIMIZATION] set _enablePatchOptimizationInEditor[{num}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 591);
			PlayerPrefs.SetInt("ENABLE_PATCH_OPTIMIZATION", num);
		}
	}

	public static string GetHash<T>(this Stream stream) where T : HashAlgorithm, new()
	{
		StringBuilder stringBuilder = new StringBuilder();
		using (T val = new T())
		{
			byte[] array = val.ComputeHash(stream);
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
		}
		return stringBuilder.ToString();
	}

	public static long FileSize(string FullPath)
	{
		if (FullPath.Contains("jar:"))
		{
			return BetterStreamingAssets.FileSize(NKCAssetbundleInnerStream.GetJarRelativePath(FullPath));
		}
		if (NKCObbUtil.IsOBBPath(FullPath))
		{
			return (long)NKCObbUtil.GetObbEntrySize(NKCObbUtil.GetRelativePathFromOBB(FullPath));
		}
		return new FileInfo(FullPath).Length;
	}

	public static string CalculateMD5(string fullPath)
	{
		if (fullPath.Contains("jar:"))
		{
			using (Stream stream = BetterStreamingAssets.OpenRead(NKCAssetbundleInnerStream.GetJarRelativePath(fullPath)))
			{
				return CalculateMD5(stream);
			}
		}
		if (NKCObbUtil.IsOBBPath(fullPath))
		{
			return CalculateMD5(NKCObbUtil.GetEntryBufferByFullPath(fullPath));
		}
		using FileStream stream2 = File.OpenRead(fullPath);
		return CalculateMD5(stream2);
	}

	public static string CalculateMD5(Stream stream)
	{
		using MD5 mD = MD5.Create();
		return ConvertByteCode(mD.ComputeHash(stream));
	}

	public static string CalculateMD5(byte[] buffer)
	{
		using MD5 mD = MD5.Create();
		byte[] hashBytes = mD.ComputeHash(buffer);
		buffer = null;
		return ConvertByteCode(hashBytes);
	}

	private static string ConvertByteCode(byte[] hashBytes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (byte b in hashBytes)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	public static void CopyFile(string sourcePath, string targetPath)
	{
		if (!File.Exists(sourcePath))
		{
			Debug.LogError("File " + sourcePath + " Not Found");
			return;
		}
		string directoryName = Path.GetDirectoryName(targetPath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		File.Copy(sourcePath, targetPath, overwrite: true);
	}

	public static void MoveFile(string sourcePath, string targetPath)
	{
		if (!File.Exists(sourcePath))
		{
			Debug.LogError("File " + sourcePath + " Not Found");
			return;
		}
		string directoryName = Path.GetDirectoryName(targetPath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		File.Move(sourcePath, targetPath);
	}

	public static void CleanupDirectory(string basePath, NKCPatchInfo lastedPatchInfo, List<string> ignoreList, NKCPatchInfo innerPatchInfo = null, NKCPatchInfo currentPatchInfo = null)
	{
		_CleanupDirectory(basePath, new Uri(basePath), lastedPatchInfo, ignoreList, innerPatchInfo, currentPatchInfo);
	}

	private static void _CleanupDirectory(string currentPath, Uri basePathUri, NKCPatchInfo lastedPatchInfo, List<string> ignoreList, NKCPatchInfo innerPatchInfo = null, NKCPatchInfo currentPatchInfo = null)
	{
		string[] directories = Directory.GetDirectories(currentPath);
		foreach (string text in directories)
		{
			_CleanupDirectory(text, basePathUri, lastedPatchInfo, ignoreList, innerPatchInfo, currentPatchInfo);
			if (IsDirectoryEmpty(text))
			{
				Directory.Delete(text);
			}
		}
		char[] trimChars = new char[3]
		{
			Path.AltDirectorySeparatorChar,
			Path.DirectorySeparatorChar,
			Path.PathSeparator
		};
		directories = Directory.GetFiles(currentPath);
		foreach (string text2 in directories)
		{
			Uri uri = new Uri(text2);
			Uri uri2 = basePathUri.MakeRelativeUri(uri);
			string currentRelativePath = Uri.UnescapeDataString(uri2.ToString());
			currentRelativePath = currentRelativePath.TrimStart(trimChars);
			bool flag = ignoreList.Exists((string x) => string.Equals(x, currentRelativePath, StringComparison.OrdinalIgnoreCase));
			if (!(lastedPatchInfo.PatchInfoExists(currentRelativePath) || flag))
			{
				if (string.IsNullOrEmpty(Path.GetExtension(currentRelativePath)))
				{
					string name = currentRelativePath + ".asset";
					string text3 = text2 + ".asset";
					string name2 = currentRelativePath + ".vkor";
					string text4 = text2 + ".vkor";
					if (lastedPatchInfo.PatchInfoExists(name) && !File.Exists(text3))
					{
						Log.Debug("[CleanupDirectory] File " + text2 + " rename to " + text3 + " from patcher", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 203);
						File.Move(text2, text3);
					}
					else if (lastedPatchInfo.PatchInfoExists(name2) && !File.Exists(text4))
					{
						Log.Debug("[CleanupDirectory] File " + text2 + " rename to " + text4 + " from patcher", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 208);
						File.Move(text2, text4);
					}
					else
					{
						Log.Debug("[CleanupDirectory] File " + text2 + " Deleted from patcher", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 213);
						File.Delete(text2);
					}
				}
				else
				{
					Log.Debug("[CleanupDirectory] File " + text2 + " Deleted from patcher", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 219);
					File.Delete(text2);
				}
			}
			else
			{
				if (innerPatchInfo == null || !lastedPatchInfo.PatchInfoExists(currentRelativePath))
				{
					continue;
				}
				NKCPatchInfo.PatchFileInfo patchInfo = lastedPatchInfo.GetPatchInfo(currentRelativePath);
				NKCPatchInfo.PatchFileInfo patchInfo2 = innerPatchInfo.GetPatchInfo(currentRelativePath);
				if (patchInfo == null || patchInfo2 == null)
				{
					continue;
				}
				string fullBuildAssetPath = GetFullBuildAssetPath(currentRelativePath);
				if (patchInfo.FileUpdated(patchInfo2) || !IsFileExists(fullBuildAssetPath))
				{
					continue;
				}
				if (currentPatchInfo != null)
				{
					NKCPatchInfo.PatchFileInfo patchInfo3 = currentPatchInfo.GetPatchInfo(currentRelativePath);
					if (patchInfo3 != null)
					{
						patchInfo3.Size = patchInfo2.Size;
						patchInfo3.Hash = patchInfo2.Hash;
					}
					else
					{
						NKCPatchInfo.PatchFileInfo value = new NKCPatchInfo.PatchFileInfo(patchInfo2.FileName, patchInfo2.Hash, patchInfo2.Size);
						currentPatchInfo.m_dicPatchInfo.Add(currentRelativePath, value);
					}
				}
				else
				{
					Log.Warn("[CleanupDirectory] CurrentPatchInfo is null when CleanupDirectory", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 255);
				}
				Log.Debug("[CleanupDirectory] File " + text2 + " Deleted from patcher, because Inner file is same as download planned file", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 258);
				File.Delete(text2);
			}
		}
	}

	private static bool IsDirectoryEmpty(string path)
	{
		return Directory.GetFileSystemEntries(path).Length == 0;
	}

	public static bool CheckIntegrity(string path, string targetHash)
	{
		if (string.IsNullOrEmpty(targetHash))
		{
			return true;
		}
		if (!IsFileExists(path))
		{
			return false;
		}
		return CalculateMD5(path).Equals(targetHash);
	}

	public static bool CheckSize(string path, long targetSize)
	{
		if (!IsFileExists(path))
		{
			return false;
		}
		if (targetSize == 0L)
		{
			return true;
		}
		return FileSize(path).Equals(targetSize);
	}

	public static string GetInnerAssetPath(string relativePath, bool isLBDefined = false)
	{
		if (NKCDefineManager.DEFINE_OBB())
		{
			return GetObbBuildAssetPath(relativePath);
		}
		return GetFullBuildAssetPath(relativePath, isLBDefined);
	}

	public static string GetFullBuildAssetPath(string relativePath, bool isLBDefined = false)
	{
		if (NKCDefineManager.DEFINE_LB() || isLBDefined)
		{
			return Application.streamingAssetsPath + "/Assetbundles/" + relativePath;
		}
		return Application.streamingAssetsPath + "/" + relativePath;
	}

	public static string GetObbBuildAssetPath(string relativePath)
	{
		return string.Empty;
	}

	public static bool IsFileExists(string path)
	{
		if (path.Contains("jar:"))
		{
			string jarRelativePath = NKCAssetbundleInnerStream.GetJarRelativePath(path);
			if (string.IsNullOrEmpty(jarRelativePath))
			{
				return false;
			}
			return BetterStreamingAssets.FileExists(jarRelativePath);
		}
		if (NKCObbUtil.IsOBBPath(path))
		{
			return NKCObbUtil.GetObbEntryIndex(NKCObbUtil.GetRelativePathFromOBB(path)) != -1;
		}
		return File.Exists(path);
	}

	public static Tuple<string, string> GetVariantFromFilename(string path, List<string> lstVariants)
	{
		string text = path;
		int num = text.LastIndexOf('.');
		string text2;
		if (num > 0)
		{
			text2 = ((!text.EndsWith(".")) ? text.Substring(num + 1) : "");
			text = text.Substring(0, num);
		}
		else
		{
			text2 = "";
		}
		int num2 = text.LastIndexOf('_');
		if (num2 > 0 && !text.EndsWith("_"))
		{
			string item = text.Substring(0, num2) + "." + text2;
			string value = text.Substring(num2 + 1);
			foreach (string lstVariant in lstVariants)
			{
				if (lstVariant.Equals(value, StringComparison.InvariantCultureIgnoreCase))
				{
					return new Tuple<string, string>(item, lstVariant);
				}
			}
		}
		return new Tuple<string, string>(path, "asset");
	}

	public static bool IsPatchSkip()
	{
		if (NKCPatchDownloader.Instance == null)
		{
			Debug.LogError("PatchDownloader not initialized!");
			return false;
		}
		if (!NKCDefineManager.DEFINE_PATCH_SKIP())
		{
			return false;
		}
		if (GetTutorialClearedStatus())
		{
			return false;
		}
		if (!IsFileExists(GetInnerAssetPath("PatchInfo.json")))
		{
			return false;
		}
		return true;
	}

	public static bool SaveTutorialClearedStatus()
	{
		GetTutorialClearedStatus();
		PlayerPrefs.SetInt("PKTutorialCleared", 1);
		PlayerPrefs.Save();
		return false;
	}

	public static bool GetTutorialClearedStatus()
	{
		return PlayerPrefs.GetInt("PKTutorialCleared", 0) != 0;
	}

	public static void DeleteTutorialClearedStatus()
	{
		PlayerPrefs.DeleteKey("PKTutorialCleared");
	}

	public static void ClearAllInnerAssetsFromDownloaded(NKCPatchInfo innerPatchInfo, string innerPath)
	{
		foreach (KeyValuePair<string, NKCPatchInfo.PatchFileInfo> item in innerPatchInfo.m_dicPatchInfo)
		{
			string key = item.Key;
			FileInfo fileInfo = new FileInfo(Path.Combine(innerPath, key));
			if (fileInfo.Exists)
			{
				File.SetAttributes(fileInfo.FullName, FileAttributes.Normal);
				fileInfo.Delete();
			}
		}
	}

	public static void ReservePatchSkipTest()
	{
		PlayerPrefs.SetInt("PKSkipTestReserve", 1);
		PlayerPrefs.Save();
	}

	public static void ProcessPatchSkipTest(string localDownloadPath)
	{
		if (PlayerPrefs.GetInt("PKSkipTestReserve", 0) != 0)
		{
			PlayerPrefs.DeleteKey("PKSkipTestReserve");
			DeleteTutorialClearedStatus();
			PlayerPrefs.Save();
			if (Directory.Exists(localDownloadPath))
			{
				Directory.Delete(localDownloadPath, recursive: true);
			}
		}
	}

	public static long GetDownloadFileSize(List<NKCPatchInfo.PatchFileInfo> patchFiles)
	{
		long num = 0L;
		foreach (NKCPatchInfo.PatchFileInfo patchFile in patchFiles)
		{
			num += patchFile.Size;
		}
		return num;
	}

	public static NKCPatchDownloader.DownType GetDownloadType()
	{
		int num = PlayerPrefs.GetInt("DownloadTypeKey", -1);
		if (num == -1)
		{
			NKCPatchDownloader.DownType downType = NKCPatchDownloader.DownType.TutorialWithBackground;
			if (NKMContentsVersionManager.HasTag("STOP_BACKGROUND_PATCH"))
			{
				downType = NKCPatchDownloader.DownType.FullDownload;
			}
			SaveDownloadType(downType);
			return downType;
		}
		NKCPatchDownloader.DownType downType2 = (NKCPatchDownloader.DownType)num;
		Log.Debug("[PatcherManager][GetDownloadType] DownloadType[" + downType2.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 558);
		return (NKCPatchDownloader.DownType)num;
	}

	public static void SaveDownloadType(NKCPatchDownloader.DownType type)
	{
		Log.Debug("[PatcherManager][SaveDownloadType] DownloadType[" + type.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 565);
		PlayerPrefs.SetInt("DownloadTypeKey", (int)type);
		PlayerPrefs.Save();
	}

	public static void RemoveDownloadType()
	{
		Log.Debug("[PatcherManager][RemoveDownloadType]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchUtility.cs", 573);
		PlayerPrefs.DeleteKey("DownloadTypeKey");
		PlayerPrefs.Save();
	}

	public static bool BackgroundPatchEnabled()
	{
		if (_enablePatchOptimizationInEditor)
		{
			return true;
		}
		if (NKCDefineManager.DEFINE_LB())
		{
			return false;
		}
		if (!NKCDefineManager.DEFINE_PATCH_OPTIMIZATION())
		{
			return false;
		}
		if (NKMContentsVersionManager.HasTag("STOP_BACKGROUND_PATCH"))
		{
			return false;
		}
		return true;
	}
}
