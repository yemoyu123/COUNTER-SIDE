using System;
using AssetBundles;
using Cs.Logging;
using NKC;
using NKC.Patcher;
using NKM;
using UnityEngine;

public class NKCPatcherStringList
{
	public static void LoadStrings(string fileName, string tableName, bool bOverwriteDuplicate)
	{
		TextAsset textAsset = Resources.Load<TextAsset>(fileName);
		string str = "";
		if (textAsset == null)
		{
			string text = Application.streamingAssetsPath + "/" + fileName;
			if (NKCPatchUtility.IsFileExists(text))
			{
				Debug.Log("patcherString exist in SA");
				if (text.Contains("jar:"))
				{
					str = BetterStreamingAssets.ReadAllText(NKCAssetbundleInnerStream.GetJarRelativePath(text));
				}
			}
			else
			{
				Debug.Log("patcherString not exist in SA");
			}
		}
		else
		{
			str = textAsset.ToString();
		}
		NKMLua nKMLua = new NKMLua();
		if (!nKMLua.DoString(str))
		{
			Log.ErrorAndExit("[PatchString] lua file loading fail. fileName:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPatcherStringList.cs", 53);
		}
		if (!nKMLua.OpenTable(tableName))
		{
			Log.ErrorAndExit("[PatchString] lua table open fail. fileName:" + fileName + " tableName:" + tableName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPatcherStringList.cs", 62);
		}
		int num = 1;
		while (nKMLua.OpenTable(num))
		{
			string rValue = null;
			if (nKMLua.GetData("m_StringID", ref rValue))
			{
				foreach (NKM_NATIONAL_CODE value in Enum.GetValues(typeof(NKM_NATIONAL_CODE)))
				{
					string rValue2 = null;
					if (nKMLua.GetData(value.ToString(), ref rValue2))
					{
						NKCStringTable.AddString(value, rValue, rValue2, bOverwriteDuplicate);
					}
				}
			}
			num++;
			nKMLua.CloseTable();
		}
		nKMLua.CloseTable();
		nKMLua.LuaClose();
	}
}
