using System;

namespace NKC.Patcher;

public static class PatchManifestPath
{
	public enum PatchType
	{
		CurrentManifest,
		InnerManifest,
		LatestManifest,
		TempManifest,
		BackgroundDownloadHistoryManifest,
		FilteredManifest,
		FullBuildManifest,
		ObbManifest,
		CurrentExtraManifestPath,
		LatestExtraManifest,
		TempExtraManifest
	}

	public static string TutorialPatchFileName = "tutorialDungeonResources.json";

	public const string manifestFileName = "PatchInfo.json";

	public const string LatestManifestFileName = "LatestPatchInfo.json";

	public const string LatestExtraManifestFileName = "LatestExtraPatchInfo.json";

	public const string tempManifestFileName = "TempPatchInfo.json";

	public const string filteredManifestFileName = "FilteredPatchInfo.json";

	public const string BackgroundDownloadHistoryManifestFileName = "BackgroundDownloadHistoryPatchInfo.json";

	public static string ServerBaseAddress => NKCPatchDownloader.Instance.ServerBaseAddress;

	public static string LocalDownloadPath => NKCPatchDownloader.Instance.LocalDownloadPath ?? "";

	public static string ExtraServerBaseAddress => NKCPatchDownloader.Instance.ExtraServerBaseAddress;

	public static string ExtraLocalDownloadPath => NKCPatchDownloader.Instance.ExtraLocalDownloadPath ?? "";

	public static string CurrentManifestPath => LocalDownloadPath + "PatchInfo.json";

	public static string LatestManifestPath => LocalDownloadPath + "LatestPatchInfo.json";

	public static string TempManifestPath => LocalDownloadPath + "TempPatchInfo.json";

	public static string BackgroundDownloadHistoryManifestPath => LocalDownloadPath + "BackgroundDownloadHistoryPatchInfo.json";

	public static string FilteredManifestPath => LocalDownloadPath + "FilteredPatchInfo.json";

	public static string InnerManifestPath => NKCPatchUtility.GetInnerAssetPath("PatchInfo.json");

	public static string FullBuildManifestPath => NKCPatchUtility.GetFullBuildAssetPath("PatchInfo.json");

	public static string ObbBuildManifestPath => NKCPatchUtility.GetObbBuildAssetPath("PatchInfo.json");

	public static string LatestExtraManifestPath => ExtraLocalDownloadPath + "LatestExtraPatchInfo.json";

	public static string CurrentExtraManifestPath => ExtraLocalDownloadPath + "PatchInfo.json";

	public static string TempExtraManifestPath => ExtraLocalDownloadPath + "TempPatchInfo.json";

	public static string GetLocalPathBy(PatchType type)
	{
		return type switch
		{
			PatchType.CurrentManifest => NKCPatchUtility.IsFileExists(CurrentManifestPath) ? CurrentManifestPath : InnerManifestPath, 
			PatchType.LatestManifest => LatestManifestPath, 
			PatchType.TempManifest => TempManifestPath, 
			PatchType.BackgroundDownloadHistoryManifest => BackgroundDownloadHistoryManifestPath, 
			PatchType.FilteredManifest => FilteredManifestPath, 
			PatchType.InnerManifest => InnerManifestPath, 
			PatchType.FullBuildManifest => FullBuildManifestPath, 
			PatchType.ObbManifest => ObbBuildManifestPath, 
			PatchType.CurrentExtraManifestPath => CurrentExtraManifestPath, 
			PatchType.LatestExtraManifest => LatestExtraManifestPath, 
			PatchType.TempExtraManifest => TempExtraManifestPath, 
			_ => throw new ArgumentOutOfRangeException("type", type, null), 
		};
	}

	public static string GetServerBasePath(bool extra)
	{
		if (!extra)
		{
			return ServerBaseAddress;
		}
		return ExtraServerBaseAddress;
	}

	public static string GetLocalDownloadPath(bool extra)
	{
		if (!extra)
		{
			return LocalDownloadPath;
		}
		return ExtraLocalDownloadPath;
	}

	public static string GetManifestPath(bool extra)
	{
		if (!extra)
		{
			return LatestManifestPath;
		}
		return LatestExtraManifestPath;
	}

	public static string GetManifestFileName(bool extra)
	{
		if (!extra)
		{
			return "LatestPatchInfo.json";
		}
		return "LatestExtraPatchInfo.json";
	}
}
