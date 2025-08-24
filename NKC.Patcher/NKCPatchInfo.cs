using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AssetBundles;
using Cs.Logging;
using NKC.Localization;
using NKC.Publisher;
using SimpleJSON;
using UnityEngine;

namespace NKC.Patcher;

public class NKCPatchInfo
{
	public enum eFileIntergityStatus
	{
		OK,
		ERROR_SIZE,
		ERROR_HASH
	}

	public class PatchFileInfo
	{
		public string FileName;

		public string Hash;

		public long Size;

		public PatchFileInfo(string bundleName, string hash, long size)
		{
			FileName = bundleName;
			Hash = hash;
			Size = size;
		}

		public PatchFileInfo(JSONNode node)
		{
			FileName = node[0].Value;
			Hash = node[1].Value;
			Size = ((node.Count > 2) ? long.Parse(node[2]) : 0);
		}

		public JSONNode GetJSONNode()
		{
			return new JSONArray
			{
				[0] = FileName,
				[1] = Hash,
				[2] = Size.ToString()
			};
		}

		public bool FileUpdated(PatchFileInfo newFile)
		{
			if (FileName != newFile.FileName)
			{
				Debug.Log("Wrong Comparison");
				return false;
			}
			return !Hash.Equals(newFile.Hash);
		}

		public eFileIntergityStatus CheckFileIntegrity(string fullPath)
		{
			if (!NKCPatchUtility.CheckIntegrity(fullPath, Hash))
			{
				return eFileIntergityStatus.ERROR_HASH;
			}
			if (!NKCPatchUtility.CheckSize(fullPath, Size))
			{
				return eFileIntergityStatus.ERROR_SIZE;
			}
			return eFileIntergityStatus.OK;
		}

		public override string ToString()
		{
			return $"{FileName} : {Hash}";
		}
	}

	public Dictionary<string, PatchFileInfo> m_dicPatchInfo = new Dictionary<string, PatchFileInfo>();

	public const bool bSaveAsBinary = true;

	private const string VersionJSONName = "version";

	private const string FileInfoListJsonName = "data";

	private string _versionString { get; set; }

	public string VersionString
	{
		get
		{
			return _versionString;
		}
		set
		{
			_versionString = value;
			if (!string.IsNullOrEmpty(_versionString))
			{
				string[] array = _versionString.Split('_');
				if (array.Length == 0)
				{
					VersionNumber = 0;
				}
				if (!int.TryParse(array.Last(), out var result))
				{
					VersionNumber = 0;
				}
				VersionNumber = result;
				Log.Debug($"[NKCPatchInfo] patchVersionNum: {VersionNumber}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchInfo.cs", 118);
			}
			else
			{
				VersionNumber = 0;
			}
		}
	}

	public int VersionNumber { get; private set; }

	public string FileFullPath { get; private set; }

	public string FilePath { get; private set; }

	public string FileName { get; private set; }

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("[NKCPatchInfo] version : " + VersionString);
		foreach (KeyValuePair<string, PatchFileInfo> item in m_dicPatchInfo)
		{
			stringBuilder.AppendLine(item.Value.ToString());
		}
		return stringBuilder.ToString();
	}

	public void AddFiles(IEnumerable<string> FilesToAdd, string basePath)
	{
		bool flag = false;
		foreach (string item in FilesToAdd)
		{
			string text = Path.Combine(basePath, item);
			if (!File.Exists(text))
			{
				Debug.LogError($"file {text} Not Found, Something Wrong");
				flag = true;
			}
			string hash = NKCPatchUtility.CalculateMD5(text);
			long size = NKCPatchUtility.FileSize(text);
			m_dicPatchInfo.Add(item, new PatchFileInfo(item, hash, size));
		}
		if (flag)
		{
			throw new Exception("Built Assetbundle missing");
		}
	}

	public void SetFilePath(string filePath)
	{
		FileFullPath = filePath;
		FileName = Path.GetFileName(FileFullPath);
		FilePath = FileFullPath.Replace(FileName, "");
	}

	public static NKCPatchInfo LoadFromJSON(string path)
	{
		Debug.Log("[LoadFromJSON] patch info load : " + path);
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		NKCPatchInfo nKCPatchInfo = new NKCPatchInfo();
		nKCPatchInfo.SetFilePath(path);
		if (NKCPatchUtility.IsFileExists(path))
		{
			Debug.Log("[LoadFromJSON] patch info load exist: " + path);
			JSONNode jSONNode;
			if (path.Contains("jar:"))
			{
				string jarRelativePath = NKCAssetbundleInnerStream.GetJarRelativePath(path);
				Debug.Log("[LoadFromJSON] open from jar : " + jarRelativePath);
				jSONNode = JSONNode.LoadFromStream(BetterStreamingAssets.OpenRead(jarRelativePath));
			}
			else if (NKCObbUtil.IsOBBPath(path))
			{
				MemoryStream memoryStream = new MemoryStream(NKCObbUtil.GetEntryBufferByFullPath(path));
				Debug.Log("[LoadFromJSON] Inner PatchInfo");
				jSONNode = JSONNode.LoadFromStream(memoryStream);
				memoryStream.Dispose();
			}
			else
			{
				jSONNode = JSONNode.LoadFromFile(path);
			}
			Debug.Log("[LoadFromJSON] patch info load from file end");
			nKCPatchInfo.VersionString = jSONNode["version"];
			Debug.Log("[LoadFromJSON] patch info load from file end version : " + nKCPatchInfo.VersionString);
			JSONNode jSONNode2 = jSONNode["data"];
			for (int i = 0; i < jSONNode2.Count; i++)
			{
				PatchFileInfo patchFileInfo = new PatchFileInfo(jSONNode2[i]);
				nKCPatchInfo.m_dicPatchInfo.Add(patchFileInfo.FileName, patchFileInfo);
			}
			Debug.Log("[LoadFromJSON] patch info load exist End");
		}
		else
		{
			Debug.Log("[LoadFromJSON] patch info load fail: " + path);
			nKCPatchInfo.VersionString = "";
		}
		return nKCPatchInfo;
	}

	private JSONNode GetJSONNode()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject["version"] = VersionString;
		int num = 0;
		JSONNode jSONNode = (jSONObject["data"] = new JSONArray());
		foreach (KeyValuePair<string, PatchFileInfo> item in m_dicPatchInfo)
		{
			jSONNode[num] = item.Value.GetJSONNode();
			num++;
		}
		return jSONObject;
	}

	public void SaveAsJSON(string path, string fileName)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		string text = Path.Combine(path, fileName);
		GetJSONNode().SaveToFile(text);
		FileFullPath = text;
	}

	public void SaveAsJSON()
	{
		if (string.IsNullOrEmpty(FilePath))
		{
			Debug.LogWarning("[SaveAsJSON] FilePath is null or empty");
		}
		if (string.IsNullOrEmpty(FileName))
		{
			Debug.LogWarning("[SaveAsJSON] FilePath is null or empty");
		}
		SaveAsJSON(FilePath, FileName);
	}

	public bool IsEmpty()
	{
		if (m_dicPatchInfo == null)
		{
			return true;
		}
		if (m_dicPatchInfo.Count == 0)
		{
			return true;
		}
		return false;
	}

	public NKCPatchInfo Append(NKCPatchInfo targetPatchInfo)
	{
		NKCPatchInfo nKCPatchInfo = new NKCPatchInfo();
		nKCPatchInfo.VersionString = targetPatchInfo.VersionString;
		nKCPatchInfo.m_dicPatchInfo = new Dictionary<string, PatchFileInfo>(m_dicPatchInfo);
		foreach (KeyValuePair<string, PatchFileInfo> item in targetPatchInfo.m_dicPatchInfo)
		{
			PatchFileInfo patchInfo = nKCPatchInfo.GetPatchInfo(item.Key);
			if (patchInfo == null)
			{
				nKCPatchInfo.AddPatchFileInfo(item.Value);
			}
			else if (patchInfo.FileUpdated(item.Value))
			{
				nKCPatchInfo.AddPatchFileInfo(item.Value);
			}
		}
		return nKCPatchInfo;
	}

	public PatchFileInfo GetPatchInfo(string name)
	{
		if (!m_dicPatchInfo.TryGetValue(name, out var value))
		{
			return null;
		}
		return value;
	}

	public bool PatchInfoExists(string name)
	{
		return m_dicPatchInfo.ContainsKey(name);
	}

	public void RemovePatchFileInfo(string name)
	{
		if (PatchInfoExists(name))
		{
			m_dicPatchInfo.Remove(name);
		}
	}

	public void AddPatchFileInfo(PatchFileInfo patchFileInfo)
	{
		if (m_dicPatchInfo.ContainsKey(patchFileInfo.FileName))
		{
			m_dicPatchInfo.Remove(patchFileInfo.FileName);
		}
		PatchFileInfo value = new PatchFileInfo(patchFileInfo.FileName, patchFileInfo.Hash, patchFileInfo.Size);
		m_dicPatchInfo[patchFileInfo.FileName] = value;
	}

	public IEnumerable<KeyValuePair<string, PatchFileInfo>> GetPatchFile(Func<KeyValuePair<string, PatchFileInfo>, bool> predicate)
	{
		return m_dicPatchInfo.Where(predicate);
	}

	public NKCPatchInfo DifferenceOfSetBy(NKCPatchInfo targetPatchInfo)
	{
		NKCPatchInfo nKCPatchInfo = new NKCPatchInfo();
		foreach (KeyValuePair<string, PatchFileInfo> item in m_dicPatchInfo)
		{
			if (targetPatchInfo.GetPatchInfo(item.Key) == null)
			{
				nKCPatchInfo.AddPatchFileInfo(item.Value);
			}
		}
		return nKCPatchInfo;
	}

	public NKCPatchInfo GetClone()
	{
		return new NKCPatchInfo
		{
			VersionString = VersionString,
			m_dicPatchInfo = new Dictionary<string, PatchFileInfo>(m_dicPatchInfo)
		};
	}

	public NKCPatchInfo FilterByVariants(List<string> lstVariants)
	{
		NKCPatchInfo nKCPatchInfo = new NKCPatchInfo();
		nKCPatchInfo.VersionString = VersionString;
		Dictionary<string, NKCPatchUtility.FileFilterInfo> dictionary = new Dictionary<string, NKCPatchUtility.FileFilterInfo>();
		List<string> allVariants = NKCLocalization.GetAllVariants();
		foreach (KeyValuePair<string, PatchFileInfo> item in m_dicPatchInfo)
		{
			string key = item.Key;
			string key2;
			string text;
			if (key.Contains("ASSET_RAW"))
			{
				Tuple<string, string> variantFromFilename = NKCPatchUtility.GetVariantFromFilename(key, allVariants);
				key2 = variantFromFilename.Item1;
				text = variantFromFilename.Item2;
			}
			else
			{
				string[] array = key.Split('.');
				if (array.Length == 1)
				{
					key2 = key;
					text = null;
				}
				else
				{
					key2 = array[0];
					text = array[1];
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				nKCPatchInfo.m_dicPatchInfo.Add(key, item.Value);
				continue;
			}
			int num = lstVariants.IndexOf(text);
			if (num < 0)
			{
				continue;
			}
			if (dictionary.TryGetValue(key2, out var value))
			{
				int num2 = lstVariants.IndexOf(value.variant);
				if (num < num2)
				{
					dictionary[key2] = new NKCPatchUtility.FileFilterInfo(key, text);
				}
			}
			else if (!NKCConnectionInfo.IgnoreVariantList.Contains(text) && !NKCPublisherModule.CheckReviewServerSkipVariant(text))
			{
				dictionary.Add(key2, new NKCPatchUtility.FileFilterInfo(key, text));
			}
		}
		foreach (KeyValuePair<string, NKCPatchUtility.FileFilterInfo> item2 in dictionary)
		{
			string originalName = item2.Value.originalName;
			if (m_dicPatchInfo.ContainsKey(originalName))
			{
				nKCPatchInfo.m_dicPatchInfo.Add(originalName, m_dicPatchInfo[originalName]);
			}
			else
			{
				Debug.LogError("Logic Error!");
			}
		}
		return nKCPatchInfo;
	}

	public NKCPatchInfo MakePatchinfoSubset(IEnumerable<string> bundleNames)
	{
		NKCPatchInfo nKCPatchInfo = new NKCPatchInfo();
		nKCPatchInfo.VersionString = VersionString;
		HashSet<string> hashSet = new HashSet<string>();
		foreach (string bundleName in bundleNames)
		{
			hashSet.Add(bundleName);
		}
		foreach (KeyValuePair<string, PatchFileInfo> item2 in m_dicPatchInfo)
		{
			string item = item2.Key.Split('.')[0];
			if (hashSet.Contains(item))
			{
				nKCPatchInfo.m_dicPatchInfo.Add(item2.Key, item2.Value);
			}
		}
		return nKCPatchInfo;
	}
}
