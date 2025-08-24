using System;
using UnityEngine;

namespace AssetBundles;

public class Utility
{
	public const string AssetBundlesOutputPath = "AssetBundles";

	public static string GetPlatformName()
	{
		return GetPlatformForAssetBundles(Application.platform);
	}

	private static string GetPlatformForAssetBundles(RuntimePlatform platform)
	{
		switch (platform)
		{
		case RuntimePlatform.Android:
			return "Android";
		case RuntimePlatform.IPhonePlayer:
			return "iOS";
		case RuntimePlatform.WebGLPlayer:
			return "WebGL";
		case RuntimePlatform.WindowsPlayer:
			if (IntPtr.Size == 8)
			{
				return "StandaloneWindows64";
			}
			return "StandaloneWindows";
		case RuntimePlatform.OSXPlayer:
			return "OSX";
		default:
			return null;
		}
	}
}
