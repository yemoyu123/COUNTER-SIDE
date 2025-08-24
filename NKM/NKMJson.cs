using System;
using System.Collections.Generic;
using Cs.Logging;
using NKC;
using SimpleJSON;
using UnityEngine;

namespace NKM;

internal sealed class NKMJson
{
	private readonly Stack<JSONNode> stack = new Stack<JSONNode>();

	public bool LoadCommonPath(string bundleName, string fileName)
	{
		if (stack.Count > 0)
		{
			throw new Exception($"try to using uninitialized instance stackCount:{stack.Count}");
		}
		NKCAssetResourceData nKCAssetResourceData = null;
		try
		{
			nKCAssetResourceData = NKCAssetResourceManager.OpenResource<TextAsset>(bundleName, fileName);
			TextAsset asset = nKCAssetResourceData.GetAsset<TextAsset>();
			if (asset == null)
			{
				Log.Error("Resources.Load null: " + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMJson.cs", 37);
				return false;
			}
			stack.Push(JSON.Parse(asset.ToString()));
			NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
			return true;
		}
		catch (Exception ex)
		{
			Log.ErrorAndExit("루아 로딩 에러 FileName : " + fileName + ", BundleName : " + bundleName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMJson.cs", 47);
			Log.ErrorAndExit(ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMJson.cs", 48);
			NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
			return false;
		}
	}

	private void PushStack(JSONNode node)
	{
		stack.Push(node);
	}
}
