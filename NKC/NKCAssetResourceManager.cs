using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using Cs.Logging;
using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCAssetResourceManager : MonoBehaviour
{
	private static Dictionary<string, NKCAssetResourceBundle> m_dicNKCResourceBundle = new Dictionary<string, NKCAssetResourceBundle>(500);

	private static LinkedList<NKCAssetResourceData> m_linklistNKCResourceDataAsync = new LinkedList<NKCAssetResourceData>();

	private static int m_AsyncResourceLoadingCount = 0;

	private static LinkedList<NKCAssetInstanceData> m_linklistNKCInstanceDataAsyncReady = new LinkedList<NKCAssetInstanceData>();

	private static LinkedList<NKCAssetInstanceData> m_linklistNKCInstanceDataAsync = new LinkedList<NKCAssetInstanceData>();

	private static int m_AsyncInstantLoadingCount = 0;

	private static HashSet<NKCAssetInstanceData> m_setNKCAssetInstanceDataToReservedClose = new HashSet<NKCAssetInstanceData>();

	private static HashSet<NKCAssetResourceData> m_setNKCAssetResourceDataToReservedClose = new HashSet<NKCAssetResourceData>();

	private static Queue<NKCAssetResourceData> m_qResourceDataPool = new Queue<NKCAssetResourceData>(500);

	private static Queue<NKCAssetInstanceData> m_qInstanceDataPool = new Queue<NKCAssetInstanceData>(500);

	private static int m_ResourceDataLoadCountMax = 0;

	private static int m_InstantDataLoadCountMax = 0;

	private static Dictionary<string, string> m_dicFileList = new Dictionary<string, string>(50000);

	private static HashSet<string> m_setLocFileList = new HashSet<string>();

	private static List<string> lstLocFileList = new List<string>();

	private static bool m_bChecked = false;

	public static IReadOnlyDictionary<string, string> dicFileList => m_dicFileList;

	public static void Init()
	{
		ReserveInstanceData(200);
		ReserveResourceData(200);
		m_dicFileList.Clear();
		m_setLocFileList.Clear();
		LoadAssetBundleNamingFromLua("AB_SCRIPT", "LUA_ASSET_BUNDLE_FILE_LIST");
		Log.Info("AssetNaming Loaded", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCAssetResourceManager.cs", 312);
	}

	public static void LoadAssetBundleNamingFromLua(string bundlePath, string luaFilePath)
	{
		Log.Info("LoadAssetBundleNamingFromLua - [" + luaFilePath + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCAssetResourceManager.cs", 318);
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath(bundlePath, luaFilePath))
		{
			if (nKMLua.OpenTable("AssetBundleFileList"))
			{
				int num = 1;
				while (nKMLua.OpenTable(num))
				{
					string rValue = "";
					nKMLua.GetData(1, ref rValue);
					string rValue2 = "";
					nKMLua.GetData(2, ref rValue2);
					if (!m_dicFileList.ContainsKey(rValue))
					{
						m_dicFileList.Add(rValue, rValue2);
					}
					else
					{
						Log.Error("m_dicFileList Duplicate fileName: " + rValue2 + ", " + rValue, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCAssetResourceManager.cs", 340);
					}
					num++;
					nKMLua.CloseTable();
				}
				nKMLua.CloseTable();
			}
			if (nKMLua.OpenTable("LocAssetBundleFileList"))
			{
				int i = 1;
				for (string rValue3 = ""; nKMLua.GetData(i, ref rValue3); i++)
				{
					m_setLocFileList.Add(rValue3);
				}
			}
		}
		nKMLua.LuaClose();
	}

	public static bool IsAssetInLocBundle(string bundleName, string assetName)
	{
		string item = bundleName.ToLower() + "_loc@" + assetName;
		return m_setLocFileList.Contains(item);
	}

	public static void SetLocFileCheck(bool bOn)
	{
		if (bOn)
		{
			lstLocFileList.Clear();
		}
		m_bChecked = bOn;
	}

	public static List<string> GetMissedLocFileList()
	{
		List<string> list = new List<string>();
		foreach (string setLocFile in m_setLocFileList)
		{
			if (!lstLocFileList.Contains(setLocFile))
			{
				list.Add(setLocFile);
			}
		}
		return list;
	}

	public static bool IsAssetInLocBundleCheckAll(string bundleName, string assetName)
	{
		string item = bundleName.ToLower() + "@" + assetName;
		if (m_bChecked)
		{
			lstLocFileList.Add(item);
		}
		if (m_setLocFileList.Contains(item))
		{
			return true;
		}
		string item2 = bundleName.ToLower() + "_loc@" + assetName;
		return m_setLocFileList.Contains(item2);
	}

	public static string GetBundleName(string fileName, bool bIgnoreNotFoundError = false)
	{
		if (m_dicFileList.ContainsKey(fileName))
		{
			return m_dicFileList[fileName];
		}
		if (!bIgnoreNotFoundError)
		{
			Log.Error("GetBundleName Null: " + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCAssetResourceManager.cs", 416);
		}
		return "";
	}

	private static void ReserveResourceData(int count)
	{
		for (int i = 0; i < count; i++)
		{
			NKCAssetResourceData item = new NKCAssetResourceData("", "", bAsync: false);
			m_qResourceDataPool.Enqueue(item);
		}
	}

	private static void ReserveInstanceData(int count)
	{
		for (int i = 0; i < count; i++)
		{
			NKCAssetInstanceData item = new NKCAssetInstanceData();
			m_qInstanceDataPool.Enqueue(item);
		}
	}

	private static NKCAssetResourceData GetResourceDataFromPool(string bundleName, string assetName, bool bAsync)
	{
		if (m_qResourceDataPool.Count <= 0)
		{
			ReserveResourceData(200);
		}
		NKCAssetResourceData nKCAssetResourceData = m_qResourceDataPool.Dequeue();
		nKCAssetResourceData.Init(bundleName, assetName, bAsync);
		return nKCAssetResourceData;
	}

	private static NKCAssetInstanceData GetInstanceDataFromPool()
	{
		if (m_qInstanceDataPool.Count <= 0)
		{
			ReserveInstanceData(200);
		}
		return m_qInstanceDataPool.Dequeue();
	}

	private static void ReturnResourceDataToPool(NKCAssetResourceData cNKCResourceData)
	{
		cNKCResourceData.Unload();
	}

	private static void ReturnInstanceDataToPool(NKCAssetInstanceData cNKCInstantData)
	{
		cNKCInstantData.Unload();
	}

	public static bool IsLoadEnd()
	{
		if (!IsLoadEndResource())
		{
			return false;
		}
		if (!IsLoadEndInstant())
		{
			return false;
		}
		m_ResourceDataLoadCountMax = 0;
		m_InstantDataLoadCountMax = 0;
		return true;
	}

	public static bool IsLoadEndResource()
	{
		if (m_linklistNKCResourceDataAsync.Count > 0)
		{
			return false;
		}
		return true;
	}

	public static bool IsLoadEndInstant()
	{
		if (m_linklistNKCInstanceDataAsync.Count > 0)
		{
			return false;
		}
		return true;
	}

	public static float GetLoadProgress()
	{
		float num = 0f;
		num = ((m_ResourceDataLoadCountMax != 0) ? (num + (1f - (float)m_linklistNKCResourceDataAsync.Count / (float)m_ResourceDataLoadCountMax) * 0.5f) : (num + 0.5f));
		if (m_InstantDataLoadCountMax == 0)
		{
			return num + 0.5f;
		}
		return num + (1f - (float)m_linklistNKCInstanceDataAsync.Count / (float)m_InstantDataLoadCountMax) * 0.5f;
	}

	public static void Update()
	{
		LinkedListNode<NKCAssetResourceData> linkedListNode = m_linklistNKCResourceDataAsync.First;
		while (linkedListNode != null)
		{
			NKCAssetResourceData value = linkedListNode.Value;
			if (value.IsDone())
			{
				if (value.callBack != null)
				{
					value.callBack(value);
				}
				m_AsyncResourceLoadingCount--;
				LinkedListNode<NKCAssetResourceData> next = linkedListNode.Next;
				m_linklistNKCResourceDataAsync.Remove(linkedListNode);
				linkedListNode = next;
			}
			else
			{
				linkedListNode = linkedListNode.Next;
			}
		}
		if (m_linklistNKCResourceDataAsync.Count == 0)
		{
			while (m_linklistNKCInstanceDataAsyncReady.Count > 0 && m_AsyncInstantLoadingCount <= 100)
			{
				NKCAssetInstanceData value2 = m_linklistNKCInstanceDataAsyncReady.First.Value;
				m_linklistNKCInstanceDataAsyncReady.RemoveFirst();
				m_AsyncInstantLoadingCount++;
				value2.m_fTime = Time.time;
				NKCScenManager.GetScenManager().StartCoroutine(LoadInstanceAsync(value2));
			}
			LinkedListNode<NKCAssetInstanceData> linkedListNode2 = m_linklistNKCInstanceDataAsync.First;
			while (linkedListNode2 != null)
			{
				NKCAssetInstanceData value3 = linkedListNode2.Value;
				if (value3.m_bLoad || value3.GetLoadFail())
				{
					m_AsyncInstantLoadingCount--;
					LinkedListNode<NKCAssetInstanceData> next2 = linkedListNode2.Next;
					m_linklistNKCInstanceDataAsync.Remove(linkedListNode2);
					linkedListNode2 = next2;
				}
				else
				{
					linkedListNode2 = linkedListNode2.Next;
				}
			}
		}
		if (m_linklistNKCResourceDataAsync.Count != 0 || m_linklistNKCInstanceDataAsync.Count != 0)
		{
			return;
		}
		foreach (NKCAssetInstanceData item in m_setNKCAssetInstanceDataToReservedClose)
		{
			Debug.LogWarning("m_qNKCAssetInstanceDataToReservedClose Processing, assetName : " + item.m_InstantOrg.m_NKMAssetName.m_AssetName);
			CloseInstance(item);
		}
		m_setNKCAssetInstanceDataToReservedClose.Clear();
		foreach (NKCAssetResourceData item2 in m_setNKCAssetResourceDataToReservedClose)
		{
			if (item2.m_RefCount <= 0)
			{
				DeleteResource(item2);
			}
		}
		m_setNKCAssetResourceDataToReservedClose.Clear();
	}

	public static NKCAssetResourceData OpenResource<T>(string assetName, bool bAsync = false) where T : Object
	{
		return OpenResource<T>(GetBundleName(assetName), assetName, bAsync);
	}

	public static NKCAssetResourceData OpenResource<T>(NKMAssetName cNKMAssetName, bool bAsync = false) where T : Object
	{
		return OpenResource<T>(cNKMAssetName.m_BundleName, cNKMAssetName.m_AssetName, bAsync);
	}

	public static NKCAssetResourceData OpenResource<T>(string bundleName, string assetName, bool bAsync = false, CallBackHandler callBackFunc = null) where T : Object
	{
		bundleName = bundleName.ToLower();
		assetName = assetName.ToUpper();
		string text = bundleName + "_loc";
		if (IsAssetInLocBundle(bundleName, assetName))
		{
			if (AssetBundleManager.IsBundleExists(text))
			{
				bundleName = text;
			}
			else
			{
				if (NKCDefineManager.DEFINE_USE_CHEAT() || !NKCDefineManager.DEFINE_SERVICE())
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, "Loc Bundle not found : " + text + "\nNationTag: " + NKMContentsVersionManager.GetCountryTag());
					if (!NKCDefineManager.DEFINE_SERVICE())
					{
						NKMContentsVersionManager.DeleteLocalTag();
					}
				}
				Debug.LogError("Loc Bundle not found : " + text);
			}
		}
		NKCAssetResourceData nKCAssetResourceData = null;
		NKCAssetResourceBundle assetResourceBundle = GetAssetResourceBundle(bundleName);
		if (assetResourceBundle.m_dicNKCResourceData.ContainsKey(assetName))
		{
			nKCAssetResourceData = assetResourceBundle.m_dicNKCResourceData[assetName];
			if (nKCAssetResourceData != null)
			{
				nKCAssetResourceData.m_RefCount++;
				if (!bAsync && !nKCAssetResourceData.IsDone())
				{
					Debug.LogWarning($"Trying sync load while processing async load {bundleName}, {assetName} Forcing Sync load..");
					nKCAssetResourceData.ForceSyncLoad();
				}
			}
			else
			{
				Debug.LogWarning($"Found Resource Data is null, bundleName : {bundleName}, assetName {assetName}");
			}
		}
		else
		{
			nKCAssetResourceData = GetResourceDataFromPool(bundleName, assetName, bAsync);
			nKCAssetResourceData.BeginLoad<T>();
			nKCAssetResourceData.callBack = callBackFunc;
			if (bAsync)
			{
				m_linklistNKCResourceDataAsync.AddLast(nKCAssetResourceData);
				m_ResourceDataLoadCountMax = m_linklistNKCResourceDataAsync.Count;
			}
			assetResourceBundle.m_dicNKCResourceData.Add(assetName, nKCAssetResourceData);
		}
		return nKCAssetResourceData;
	}

	public static string RemapLocBundle(string bundleName, string assetName)
	{
		if (IsAssetInLocBundle(bundleName, assetName))
		{
			string text = bundleName.ToLower() + "_loc";
			if (AssetBundleManager.IsBundleExists(text))
			{
				return text;
			}
			Debug.LogError("Loc Bundle not found : " + text);
		}
		return bundleName.ToLower();
	}

	public static bool IsBundleExists(NKMAssetName assetName)
	{
		return IsBundleExists(assetName.m_BundleName, assetName.m_AssetName);
	}

	public static bool IsBundleExists(string bundleName, string assetName)
	{
		bundleName = bundleName.ToLower();
		if (IsAssetInLocBundle(bundleName, assetName))
		{
			return AssetBundleManager.IsBundleExists(bundleName + "_loc");
		}
		return AssetBundleManager.IsBundleExists(bundleName);
	}

	public static bool IsAssetExists(string bundleName, string assetName, bool loadUnloadedBundle)
	{
		bundleName = bundleName.ToLower();
		if (IsAssetInLocBundle(bundleName, assetName))
		{
			bundleName += "_loc";
		}
		if (!AssetBundleManager.IsBundleExists(bundleName))
		{
			return false;
		}
		if (!AssetBundleManager.IsAssetBundleLoaded(bundleName))
		{
			if (!loadUnloadedBundle)
			{
				return false;
			}
			AssetBundleManager.LoadAssetBundle(bundleName, async: false);
		}
		return AssetBundleManager.IsAssetExists(bundleName, assetName);
	}

	public static int CloseResource(string bundleName, string assetName)
	{
		assetName = assetName.ToUpper();
		bundleName = bundleName.ToLower();
		NKCAssetResourceBundle assetResourceBundle = GetAssetResourceBundle(bundleName);
		if (assetResourceBundle.m_dicNKCResourceData.ContainsKey(assetName))
		{
			return CloseResource(assetResourceBundle.m_dicNKCResourceData[assetName]);
		}
		return -1;
	}

	public static int CloseResource(NKCAssetResourceData cNKCResourceData)
	{
		int result = -1;
		if (cNKCResourceData == null)
		{
			Debug.LogWarning("AssetResourceMgr.CloseResource Fail Because Data is null ");
			return result;
		}
		if (cNKCResourceData.m_RefCount <= 0)
		{
			Debug.LogWarning("AssetResourceMgr.CloseResource Fail Because Ref Count <= 0, AssetName : " + cNKCResourceData.m_NKMAssetName.m_AssetName + " ");
			return result;
		}
		cNKCResourceData.m_RefCount--;
		result = cNKCResourceData.m_RefCount;
		if (cNKCResourceData.m_RefCount == 0)
		{
			if (m_linklistNKCResourceDataAsync.Count == 0 && m_linklistNKCInstanceDataAsync.Count == 0 && cNKCResourceData.IsDone())
			{
				DeleteResource(cNKCResourceData);
			}
			else if (!m_setNKCAssetResourceDataToReservedClose.Contains(cNKCResourceData))
			{
				m_setNKCAssetResourceDataToReservedClose.Add(cNKCResourceData);
			}
		}
		return result;
	}

	private static void DeleteResource(NKCAssetResourceData cNKCResourceData)
	{
		NKCAssetResourceBundle assetResourceBundle = GetAssetResourceBundle(cNKCResourceData.m_NKMAssetName.m_BundleName);
		if (assetResourceBundle.m_dicNKCResourceData.ContainsKey(cNKCResourceData.m_NKMAssetName.m_AssetName))
		{
			assetResourceBundle.m_dicNKCResourceData.Remove(cNKCResourceData.m_NKMAssetName.m_AssetName);
			ReturnResourceDataToPool(cNKCResourceData);
		}
		else
		{
			Debug.LogWarning("AssetResourceMgr.DeleteResource Fail, AssetName : " + cNKCResourceData.m_NKMAssetName.m_AssetName + " ");
		}
	}

	public static void UnloadAllResources()
	{
		foreach (NKCAssetResourceBundle value in m_dicNKCResourceBundle.Values)
		{
			foreach (NKCAssetResourceData value2 in value.m_dicNKCResourceData.Values)
			{
				ReturnResourceDataToPool(value2);
			}
			value.m_dicNKCResourceData.Clear();
		}
		m_dicNKCResourceBundle.Clear();
	}

	private static NKCAssetResourceBundle GetAssetResourceBundle(string bundleName)
	{
		NKCAssetResourceBundle nKCAssetResourceBundle = null;
		if (!m_dicNKCResourceBundle.ContainsKey(bundleName))
		{
			nKCAssetResourceBundle = new NKCAssetResourceBundle();
			m_dicNKCResourceBundle.Add(bundleName, nKCAssetResourceBundle);
		}
		else
		{
			nKCAssetResourceBundle = m_dicNKCResourceBundle[bundleName];
		}
		return nKCAssetResourceBundle;
	}

	private static IEnumerator LoadInstanceAsync(NKCAssetInstanceData cNKCInstantData)
	{
		while (cNKCInstantData.m_InstantOrg != null && cNKCInstantData.m_InstantOrg.GetAsset<GameObject>() == null && !cNKCInstantData.GetLoadFail())
		{
			yield return null;
		}
		if (cNKCInstantData.m_InstantOrg != null && !cNKCInstantData.GetLoadFail())
		{
			cNKCInstantData.m_Instant = Object.Instantiate(cNKCInstantData.m_InstantOrg.GetAsset<GameObject>());
			cNKCInstantData.m_bLoad = true;
			cNKCInstantData.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_NKM_NEW_INSTANT().transform);
		}
		yield return cNKCInstantData;
	}

	public static NKCAssetInstanceData OpenInstance<T>(NKMAssetName cNKMAssetName, bool bAsync = false, Transform parent = null) where T : Object
	{
		return OpenInstance<T>(cNKMAssetName.m_BundleName, cNKMAssetName.m_AssetName, bAsync, parent);
	}

	public static NKCAssetInstanceData OpenInstance<T>(string bundleName, string assetName, bool bAsync = false, Transform parent = null) where T : Object
	{
		NKCAssetResourceData nKCAssetResourceData = OpenResource<T>(bundleName, assetName, bAsync);
		NKCAssetInstanceData instanceDataFromPool = GetInstanceDataFromPool();
		instanceDataFromPool.m_BundleName = bundleName;
		instanceDataFromPool.m_AssetName = assetName;
		instanceDataFromPool.m_InstantOrg = nKCAssetResourceData;
		instanceDataFromPool.m_bLoadTypeAsync = bAsync;
		if (!bAsync)
		{
			if (nKCAssetResourceData != null && nKCAssetResourceData.GetAsset<GameObject>() != null)
			{
				if (parent != null)
				{
					instanceDataFromPool.m_Instant = Object.Instantiate(nKCAssetResourceData.GetAsset<GameObject>(), parent);
				}
				else
				{
					instanceDataFromPool.m_Instant = Object.Instantiate(nKCAssetResourceData.GetAsset<GameObject>());
				}
				instanceDataFromPool.m_bLoad = true;
				if (parent == null)
				{
					if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().Get_NKM_NEW_INSTANT() != null)
					{
						instanceDataFromPool.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_NKM_NEW_INSTANT().transform);
					}
					else
					{
						instanceDataFromPool.m_Instant.transform.SetParent(null);
					}
				}
				return instanceDataFromPool;
			}
			return null;
		}
		m_linklistNKCInstanceDataAsyncReady.AddLast(instanceDataFromPool);
		m_linklistNKCInstanceDataAsync.AddLast(instanceDataFromPool);
		m_InstantDataLoadCountMax = m_linklistNKCInstanceDataAsync.Count;
		return instanceDataFromPool;
	}

	public static void CloseInstance(NKCAssetInstanceData cInstantData)
	{
		if (cInstantData == null)
		{
			return;
		}
		if (cInstantData.m_bLoadTypeAsync && !cInstantData.m_bLoad && m_linklistNKCInstanceDataAsync.Count > 0)
		{
			LinkedListNode<NKCAssetInstanceData> linkedListNode = m_linklistNKCInstanceDataAsync.First;
			while (linkedListNode != null)
			{
				NKCAssetInstanceData value = linkedListNode.Value;
				if (value == cInstantData)
				{
					if (!m_setNKCAssetInstanceDataToReservedClose.Contains(value))
					{
						m_setNKCAssetInstanceDataToReservedClose.Add(value);
					}
					else
					{
						Debug.LogWarning("AssetResourceMgr.CloseInstance fail because this instance is already reserved to close, assetName : " + value.m_AssetName);
					}
					return;
				}
				if (linkedListNode != null)
				{
					linkedListNode = linkedListNode.Next;
				}
			}
		}
		ReturnInstanceDataToPool(cInstantData);
	}
}
