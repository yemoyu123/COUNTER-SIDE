using System.Collections.Generic;
using Cs.Logging;
using NKM;
using UnityEngine;

namespace NKC;

public static class NKCContentsVersionManager
{
	public const string LUA_DEFAULT_CONTENTS_TAG = "LUA_DEFAULT_CONTENTS_TAG";

	public static string TagVariableName = "KOR";

	public static bool s_DefaultTagLoaded = false;

	private static HashSet<string> CurrentContentsTag => NKMContentsVersionManager.CurrentContentsTag;

	public static void TryRecoverTag()
	{
		if (CurrentContentsTag.Count > 0)
		{
			Log.Debug("[ContentsVersion] CurrentContentsTag exist. Skip recovery.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 22);
			return;
		}
		string text = PlayerPrefs.GetString("LOCAL_SAVE_CONTENTS_TAG_KEY");
		if (!string.IsNullOrEmpty(text))
		{
			Log.Debug("[ContentsVersion] Local CurrentContentsTag exist. Get tag from local data.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 30);
			SetTagList(text);
			return;
		}
		TextAsset textAsset = Resources.Load<TextAsset>("LUA_DEFAULT_CONTENTS_TAG");
		if (textAsset == null)
		{
			Log.ErrorAndExit("[ContentsVersion] Cannot find file :LUA_DEFAULT_CONTENTS_TAG", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 39);
			return;
		}
		string str = textAsset.ToString();
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.DoString(str))
		{
			Log.ErrorAndExit("[ContentsVersion] loading file failed:LUA_DEFAULT_CONTENTS_TAG", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 50);
		}
		Log.Debug("TagVariableName : " + TagVariableName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 53);
		List<string> list = new List<string>();
		if (!nKMLua.GetData(TagVariableName, list))
		{
			Log.ErrorAndExit("[ContentsVersion] loading default tag failed:" + TagVariableName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 58);
		}
		foreach (string item in list)
		{
			NKMContentsVersionManager.AddTag(item);
			Log.Debug("NKMContentsVersionManager AddTag : " + item, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 64);
			s_DefaultTagLoaded = true;
		}
	}

	public static void SaveTagToLocal()
	{
		string value = string.Join(";", CurrentContentsTag);
		PlayerPrefs.SetString("LOCAL_SAVE_CONTENTS_TAG_KEY", value);
		PlayerPrefs.SetString("LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_IP", NKCConnectionInfo.ServiceIP);
		PlayerPrefs.SetInt("LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_PORT", NKCConnectionInfo.ServicePort);
		Log.Info($"SaveTagInfo IP[{NKCConnectionInfo.ServiceIP}] PORT[{NKCConnectionInfo.ServicePort}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 77);
	}

	public static void SetTagList(IReadOnlyList<string> tagList)
	{
		CurrentContentsTag.Clear();
		foreach (string tag in tagList)
		{
			NKMContentsVersionManager.AddTag(tag);
		}
	}

	public static bool CheckSameTagList(IReadOnlyList<string> tagList)
	{
		if (CurrentContentsTag.Count != tagList.Count)
		{
			Log.Warn("client tagList:" + string.Join(", ", CurrentContentsTag), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 93);
			Log.Warn("server tagList:" + string.Join(", ", tagList), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 94);
			return false;
		}
		foreach (string tag in tagList)
		{
			if (!NKMContentsVersionManager.HasTag(tag))
			{
				Log.Warn("client tagList:" + string.Join(", ", CurrentContentsTag), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 102);
				Log.Warn("server tagList:" + string.Join(", ", tagList), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCContentsVersionManager.cs", 103);
				return false;
			}
		}
		return true;
	}

	private static void SetTagList(string tagList)
	{
		CurrentContentsTag.Clear();
		string[] array = tagList.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			NKMContentsVersionManager.AddTag(array[i]);
		}
	}
}
