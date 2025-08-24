using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NKC.Patcher;

public static class PatchLogContainer
{
	private class DownloadResult
	{
		public readonly NKCPatchInfo.PatchFileInfo FileInfo;

		public readonly string Path;

		public DownloadResult(NKCPatchInfo.PatchFileInfo fileInfo, string path)
		{
			FileInfo = fileInfo;
			Path = path;
		}
	}

	public const string NotExistInCurrentManifest = "Not exist in currentManifest";

	public const string IsOldFile = "Is old file";

	public const string DifferentSize = "Different size";

	public const string CheckIntegrity = "Check integrity";

	public const string NotExist = "Not exist";

	public const string NotRequiredUpdate = "Not required update";

	public const string Inner_FileUpdated = "[Inner] FileUpdated";

	public const string Inner_NotExist = "[Inner] Not exist";

	public const string Inner_DifferentSize = "[Inner] Different size";

	public const string Inner_CheckIntegrity = "[Inner] Check integrity";

	private static readonly Dictionary<string, List<DownloadResult>> _downloadListContainer = new Dictionary<string, List<DownloadResult>>();

	public static void AddToLog(string key, NKCPatchInfo.PatchFileInfo newInfo, string path)
	{
		if (NKCDefineManager.DEFINE_SAVE_LOG())
		{
			if (!_downloadListContainer.TryGetValue(key, out var value))
			{
				value = new List<DownloadResult>();
				_downloadListContainer.Add(key, value);
			}
			DownloadResult item = new DownloadResult(newInfo, path);
			value.Add(item);
		}
	}

	public static void DownloadListLogOutPut()
	{
		if (!NKCDefineManager.DEFINE_SAVE_LOG())
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DownloadListLogOutPut]");
		foreach (KeyValuePair<string, List<DownloadResult>> item in _downloadListContainer)
		{
			double totalSize = 0.0;
			item.Value.ForEach(delegate(DownloadResult _)
			{
				totalSize += _.FileInfo.Size;
			});
			stringBuilder.Append($":: [{item.Key}][Count:{item.Value.Count}][Size:{(double)(float)totalSize / 1048576.0}mb] ::");
			foreach (DownloadResult item2 in item.Value)
			{
				stringBuilder.Append(" - [" + item.Key + "][Path:" + item2.Path + "][FileName:" + item2.FileInfo.FileName + "]");
			}
		}
		Debug.Log(stringBuilder);
		Clear();
	}

	public static void Clear()
	{
		_downloadListContainer.Clear();
	}
}
