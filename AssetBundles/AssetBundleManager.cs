using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NKC;
using NKC.Localization;
using NKC.Patcher;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetBundles;

public class AssetBundleManager : MonoBehaviour
{
	public enum LogMode
	{
		All,
		JustErrors
	}

	public enum LogType
	{
		Info,
		Warning,
		Error
	}

	public delegate string OverrideBaseDownloadingURLDelegate(string bundleName);

	public static readonly ulong[] MaskList = new ulong[4] { 6024583083643712183uL, 9359322754833546465uL, 920365553137762983uL, 7040909297097279612uL };

	public static List<ulong> maskList = new List<ulong>();

	public const long AssetMaskLength = 212L;

	private static LogMode m_LogMode = LogMode.All;

	private static string m_BaseDownloadingURL = "";

	private static string[] m_ActiveVariants = new string[1] { "asset" };

	private static AssetBundleManifest m_AssetBundleManifest = null;

	private static GameObject s_objAssetBundleManager;

	private static HashSet<string> m_TotalLoadedAssetBundleNames = new HashSet<string>();

	private static HashSet<string> m_TotalLoadedAssetBundlePath = new HashSet<string>();

	private static Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();

	private static Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string>();

	private static Dictionary<string, int> m_DownloadingBundles = new Dictionary<string, int>();

	private static List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation>();

	private static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();

	public const string DEFAULT_VARIENT = "asset";

	public const string DEVELOPMENT_VARIANT = "dev";

	public const string DEVELOPMENT_VARIANT_TAG = "DEV_VARIANT";

	private static Dictionary<string, HashSet<string>> m_dicVariants = null;

	private static Dictionary<string, string> m_bundleVariantCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private static Dictionary<string, string> m_bundlePathCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	public static LogMode logMode
	{
		get
		{
			return m_LogMode;
		}
		set
		{
			m_LogMode = value;
		}
	}

	public static string BaseDownloadingURL
	{
		get
		{
			return m_BaseDownloadingURL;
		}
		set
		{
			m_BaseDownloadingURL = value;
		}
	}

	public static string[] ActiveVariants
	{
		get
		{
			return m_ActiveVariants;
		}
		set
		{
			m_ActiveVariants = value;
		}
	}

	public static AssetBundleManifest AssetBundleManifestObject
	{
		set
		{
			m_AssetBundleManifest = value;
		}
	}

	private static Dictionary<string, HashSet<string>> dicVariants
	{
		get
		{
			if (m_dicVariants == null)
			{
				m_bundleVariantCache.Clear();
				m_bundlePathCache.Clear();
				m_dicVariants = new Dictionary<string, HashSet<string>>();
				string[] allAssetBundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();
				for (int i = 0; i < allAssetBundlesWithVariant.Length; i++)
				{
					string[] array = allAssetBundlesWithVariant[i].Split('.');
					string key = array[0];
					string item = array[1];
					if (!m_dicVariants.ContainsKey(key))
					{
						m_dicVariants[key] = new HashSet<string>();
					}
					m_dicVariants[key].Add(item);
				}
			}
			return m_dicVariants;
		}
	}

	public static event OverrideBaseDownloadingURLDelegate overrideBaseDownloadingURL;

	public static ulong[] GetMaskList(string filePath, bool bWriteLog = false)
	{
		string text = (text = Path.GetFileNameWithoutExtension(filePath.ToLower()));
		string text2 = NKCPatchUtility.CalculateMD5(Encoding.UTF8.GetBytes(text.ToCharArray()));
		text2.GetHashCode();
		string s = text2.Substring(0, 16);
		string s2 = text2.Substring(16, 16);
		string s3 = text2.Substring(0, 8) + text2.Substring(16, 8);
		string s4 = text2.Substring(8, 8) + text2.Substring(24, 8);
		ulong item = ulong.Parse(s, NumberStyles.HexNumber);
		ulong item2 = ulong.Parse(s2, NumberStyles.HexNumber);
		ulong item3 = ulong.Parse(s3, NumberStyles.HexNumber);
		ulong item4 = ulong.Parse(s4, NumberStyles.HexNumber);
		if (bWriteLog)
		{
			Debug.Log("[ENCRYPT_TEST][" + text + "] mask1[" + item + "] mask2[" + item2 + "] mask3[" + item3 + "] mask4[" + item4 + "]");
		}
		maskList.Clear();
		maskList.Add(item);
		maskList.Add(item2);
		maskList.Add(item3);
		maskList.Add(item4);
		return maskList.ToArray();
	}

	public static bool IsAssetBundleManifestLoaded()
	{
		return m_AssetBundleManifest != null;
	}

	private static void Log(LogType logType, string text)
	{
		if (logType == LogType.Error)
		{
			Debug.LogError("[AssetBundleManager] " + text);
		}
		else if (m_LogMode == LogMode.All && logType == LogType.Warning)
		{
			Debug.LogWarning("[AssetBundleManager] " + text);
		}
		else
		{
			_ = m_LogMode;
		}
	}

	public static string GetLocalDownloadPath()
	{
		if (Application.isEditor)
		{
			return Environment.CurrentDirectory.Replace("\\", "/") + "/AssetBundles/" + Utility.GetPlatformName();
		}
		if (NKCDefineManager.DEFINE_PC_EXTRA_DOWNLOAD_IN_EXE_FOLDER())
		{
			return Application.dataPath + "/../Assetbundles/";
		}
		return Application.persistentDataPath + "/Assetbundles/";
	}

	public static void SetSourceAssetBundleDirectory(string relativePath)
	{
		BaseDownloadingURL = GetLocalDownloadPath() + relativePath;
	}

	public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
	{
		if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
		{
			return null;
		}
		LoadedAssetBundle value = null;
		m_LoadedAssetBundles.TryGetValue(assetBundleName, out value);
		if (value == null)
		{
			return null;
		}
		string[] value2 = null;
		if (!m_Dependencies.TryGetValue(assetBundleName, out value2))
		{
			return value;
		}
		string[] array = value2;
		foreach (string text in array)
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (m_DownloadingErrors.TryGetValue(text, out error))
				{
					return null;
				}
				m_LoadedAssetBundles.TryGetValue(text, out var value3);
				if (value3 == null)
				{
					return null;
				}
			}
		}
		return value;
	}

	public static bool IsAssetBundleDownloaded(string assetBundleName)
	{
		return m_LoadedAssetBundles.ContainsKey(assetBundleName);
	}

	public static AssetBundleLoadManifestOperation Initialize()
	{
		SetSourceAssetBundleDirectory("");
		return Initialize(Utility.GetPlatformName());
	}

	public static AssetBundleLoadManifestOperation Initialize(string manifestAssetBundleName)
	{
		m_dicVariants = null;
		if (s_objAssetBundleManager == null)
		{
			s_objAssetBundleManager = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
			UnityEngine.Object.DontDestroyOnLoad(s_objAssetBundleManager);
		}
		LoadAssetBundle(manifestAssetBundleName, async: false, isLoadingAssetBundleManifest: true);
		AssetBundleLoadManifestOperation assetBundleLoadManifestOperation = new AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
		m_InProgressOperations.Add(assetBundleLoadManifestOperation);
		return assetBundleLoadManifestOperation;
	}

	public static void LoadAssetBundle(string assetBundleName, bool async)
	{
		assetBundleName = RemapVariantName(assetBundleName);
		LoadAssetBundle(assetBundleName, async, isLoadingAssetBundleManifest: false);
	}

	protected static void LoadAssetBundle(string assetBundleName, bool async, bool isLoadingAssetBundleManifest)
	{
		Log(LogType.Info, "Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);
		if (!isLoadingAssetBundleManifest && m_AssetBundleManifest == null)
		{
			Log(LogType.Error, "Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
			return;
		}
		bool flag = ((!async) ? LoadAssetBundleInternal(assetBundleName) : LoadAssetBundleInternalAsync(assetBundleName));
		if (flag)
		{
			flag = CheckFullyLoaded(assetBundleName);
		}
		if (!flag && !isLoadingAssetBundleManifest)
		{
			LoadDependencies(assetBundleName, async);
		}
	}

	private static bool CheckFullyLoaded(string assetbundleName)
	{
		if (!m_LoadedAssetBundles.TryGetValue(assetbundleName, out var value))
		{
			if (m_DownloadingBundles.ContainsKey(assetbundleName))
			{
				return true;
			}
			Log(LogType.Error, "[CheckFullyLoaded] is not loadedAssetBundle _ assetbundleName: " + assetbundleName);
			return false;
		}
		if (!value.IsFullyLoaded)
		{
			value.IsFullyLoaded = true;
			if (m_AssetBundleManifest.GetAllDependencies(assetbundleName).Length == 0)
			{
				return true;
			}
			if (!m_Dependencies.ContainsKey(assetbundleName))
			{
				Debug.LogWarning("[CheckFullyLoaded] Check AssetBundle Dependencies _ assetbundleName: " + assetbundleName);
				return false;
			}
		}
		return true;
	}

	protected static string GetAssetBundleBaseDownloadingURL(string bundleName)
	{
		if (AssetBundleManager.overrideBaseDownloadingURL != null)
		{
			Delegate[] invocationList = AssetBundleManager.overrideBaseDownloadingURL.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				string text = ((OverrideBaseDownloadingURLDelegate)invocationList[i])(bundleName);
				if (text != null)
				{
					return text;
				}
			}
		}
		return m_BaseDownloadingURL;
	}

	protected static bool UsesExternalBundleVariantResolutionMechanism(string baseAssetBundleName)
	{
		return false;
	}

	public static string GetFolderedBundleName(string bundleName)
	{
		if (bundleName.StartsWith("ab_fx", StringComparison.OrdinalIgnoreCase))
		{
			return "fx/" + bundleName;
		}
		if (bundleName.StartsWith("ab_ui_shop", StringComparison.OrdinalIgnoreCase) || bundleName.StartsWith("package", StringComparison.OrdinalIgnoreCase))
		{
			return "shop/" + bundleName;
		}
		if (bundleName.StartsWith("ab_ui_skin_voice", StringComparison.OrdinalIgnoreCase) || bundleName.StartsWith("ab_ui_unit_voice", StringComparison.OrdinalIgnoreCase))
		{
			return "voice/" + bundleName;
		}
		return bundleName;
	}

	public static string GetBundleNameWithoutFolder(string bundleName)
	{
		if (string.IsNullOrEmpty(bundleName))
		{
			return bundleName;
		}
		if (bundleName.Contains('/'))
		{
			int num = bundleName.LastIndexOf('/');
			return bundleName.Substring(num + 1);
		}
		return bundleName;
	}

	public static string RemapVariantName(string assetBundleName)
	{
		if (m_bundleVariantCache.TryGetValue(assetBundleName, out var value))
		{
			return value;
		}
		assetBundleName = GetFolderedBundleName(assetBundleName);
		if (m_AssetBundleManifest == null)
		{
			return assetBundleName;
		}
		assetBundleName = assetBundleName.ToLower();
		string text = assetBundleName.Split('.')[0];
		if (UsesExternalBundleVariantResolutionMechanism(text))
		{
			m_bundleVariantCache[assetBundleName] = text;
			return text;
		}
		if (dicVariants.TryGetValue(text, out var value2))
		{
			for (int i = 0; i < m_ActiveVariants.Length; i++)
			{
				if (value2.Contains(m_ActiveVariants[i]))
				{
					string text2 = text + "." + m_ActiveVariants[i];
					m_bundleVariantCache[assetBundleName] = text2;
					return text2;
				}
			}
			if (value2.Contains("asset"))
			{
				string text3 = text + ".asset";
				m_bundleVariantCache[assetBundleName] = text3;
				Log(LogType.Warning, "default asset bundle variant chosen because there was no matching active variant: " + text3);
				return text3;
			}
		}
		m_bundleVariantCache[assetBundleName] = assetBundleName;
		return assetBundleName;
	}

	protected static bool LoadAssetBundleInternalAsync(string assetBundleName)
	{
		LoadedAssetBundle value = null;
		m_LoadedAssetBundles.TryGetValue(assetBundleName, out value);
		if (value != null)
		{
			value.m_ReferencedCount++;
			return true;
		}
		if (m_DownloadingBundles.ContainsKey(assetBundleName))
		{
			m_DownloadingBundles[assetBundleName] += 1;
			return true;
		}
		string bundleFilePath = GetBundleFilePath(assetBundleName);
		if (string.IsNullOrWhiteSpace(bundleFilePath))
		{
			Debug.LogWarning("path is invalid, assetBundleName : " + assetBundleName);
		}
		AddToTotalLoadedAssetBundle(assetBundleName, bundleFilePath);
		try
		{
			m_InProgressOperations.Add(new AssetBundleLoadFromFileOperation(assetBundleName, bundleFilePath));
		}
		catch
		{
			throw new InvalidOperationException("Could not load asset : assetbundleName: " + assetBundleName + ", path: " + bundleFilePath);
		}
		m_DownloadingBundles.Add(assetBundleName, 1);
		return false;
	}

	protected static void AddToTotalLoadedAssetBundle(string assetBundleName, string assetBundlePath)
	{
		if (NKCDefineManager.DEFINE_ZLONG_CHN() && NKCDefineManager.DEFINE_USE_CHEAT() && !m_TotalLoadedAssetBundleNames.Contains(assetBundleName))
		{
			Debug.Log("[ASSETBUNDLE_LOAD] NAME " + assetBundleName);
			Debug.Log("[ASSETBUNDLE_LOAD] PATH " + assetBundlePath);
			m_TotalLoadedAssetBundleNames.Add(assetBundleName);
			if (!m_TotalLoadedAssetBundlePath.Contains(assetBundlePath))
			{
				m_TotalLoadedAssetBundlePath.Add(assetBundlePath);
			}
		}
	}

	protected static bool LoadAssetBundleInternal(string assetBundleName)
	{
		LoadedAssetBundle value = null;
		m_LoadedAssetBundles.TryGetValue(assetBundleName, out value);
		if (value != null)
		{
			value.m_ReferencedCount++;
			return true;
		}
		int referencedCount = 1;
		if (m_DownloadingBundles.ContainsKey(assetBundleName))
		{
			referencedCount = m_DownloadingBundles[assetBundleName] + 1;
			foreach (AssetBundleLoadOperation inProgressOperation in m_InProgressOperations)
			{
				if (inProgressOperation is AssetBundleDownloadOperation assetBundleDownloadOperation && assetBundleDownloadOperation.assetBundleName == assetBundleName)
				{
					Debug.LogWarning("[LoadAssetBundleInternal][" + assetBundleDownloadOperation.assetBundleName + "] ForceFlush");
					assetBundleDownloadOperation.bForceFlush = true;
					break;
				}
			}
		}
		if (m_DownloadingErrors.TryGetValue(assetBundleName, out var value2))
		{
			Log(LogType.Error, value2);
			return true;
		}
		if (!IsBundleExists(assetBundleName))
		{
			m_DownloadingErrors.Add(assetBundleName, assetBundleName + " Bundle Not Exists");
			Log(LogType.Error, "assetbundle " + assetBundleName + " does not exists!");
			return true;
		}
		string bundleFilePath = GetBundleFilePath(assetBundleName);
		Stream stream = null;
		stream = OpenCryptoBundleFileStream(bundleFilePath);
		value = new LoadedAssetBundle(AssetBundle.LoadFromStream(stream), stream);
		value.m_ReferencedCount = referencedCount;
		m_LoadedAssetBundles.Add(assetBundleName, value);
		AddToTotalLoadedAssetBundle(assetBundleName, bundleFilePath);
		return false;
	}

	protected static void LoadDependencies(string assetBundleName, bool async)
	{
		if (m_AssetBundleManifest == null)
		{
			Log(LogType.Error, "Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
			return;
		}
		string[] allDependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
		if (allDependencies.Length == 0)
		{
			return;
		}
		for (int i = 0; i < allDependencies.Length; i++)
		{
			allDependencies[i] = RemapVariantName(allDependencies[i]);
		}
		m_Dependencies[assetBundleName] = allDependencies;
		for (int j = 0; j < allDependencies.Length; j++)
		{
			if (string.IsNullOrEmpty(allDependencies[j]))
			{
				Debug.LogWarning("Assetbundle " + assetBundleName + " has empty dependancy. check for missing texture/reference");
			}
			else if (async)
			{
				LoadAssetBundleInternalAsync(allDependencies[j]);
			}
			else
			{
				LoadAssetBundleInternal(allDependencies[j]);
			}
		}
	}

	public static void UnloadAssetBundle(string assetBundleName)
	{
		assetBundleName = RemapVariantName(assetBundleName);
		if (UnloadAssetBundleInternal(assetBundleName))
		{
			UnloadDependencies(assetBundleName);
		}
	}

	public static void UnloadAllAndCleanup()
	{
		foreach (LoadedAssetBundle value in m_LoadedAssetBundles.Values)
		{
			value.OnUnload();
		}
		m_dicVariants = null;
		m_bundlePathCache.Clear();
		m_bundleVariantCache.Clear();
		m_LoadedAssetBundles.Clear();
		m_Dependencies.Clear();
		m_AssetBundleManifest = null;
	}

	protected static void UnloadDependencies(string assetBundleName)
	{
		string[] value = null;
		if (m_Dependencies.TryGetValue(assetBundleName, out value))
		{
			string[] array = value;
			for (int i = 0; i < array.Length; i++)
			{
				UnloadAssetBundleInternal(array[i]);
			}
			m_Dependencies.Remove(assetBundleName);
		}
	}

	protected static bool UnloadAssetBundleInternal(string assetBundleName)
	{
		string error;
		LoadedAssetBundle loadedAssetBundle = GetLoadedAssetBundle(assetBundleName, out error);
		if (loadedAssetBundle == null)
		{
			return false;
		}
		if (--loadedAssetBundle.m_ReferencedCount == 0)
		{
			loadedAssetBundle.OnUnload();
			m_LoadedAssetBundles.Remove(assetBundleName);
			Log(LogType.Info, assetBundleName + " has been unloaded successfully");
			return true;
		}
		return false;
	}

	private void Update()
	{
		int num = 0;
		while (num < m_InProgressOperations.Count)
		{
			AssetBundleLoadOperation assetBundleLoadOperation = m_InProgressOperations[num];
			if (assetBundleLoadOperation.Update())
			{
				num++;
				continue;
			}
			m_InProgressOperations.RemoveAt(num);
			ProcessFinishedOperation(assetBundleLoadOperation);
		}
	}

	private void ProcessFinishedOperation(AssetBundleLoadOperation operation)
	{
		if (!(operation is AssetBundleDownloadOperation assetBundleDownloadOperation))
		{
			return;
		}
		if (assetBundleDownloadOperation.bForceFlush)
		{
			if (assetBundleDownloadOperation.assetBundle != null)
			{
				if (assetBundleDownloadOperation.assetBundle.m_AssetBundle != null)
				{
					Debug.LogWarning("[ProcessFinishedOperation][" + assetBundleDownloadOperation.assetBundle.m_AssetBundle.name + "] ForceFlush");
				}
				assetBundleDownloadOperation.assetBundle.OnUnload();
			}
			return;
		}
		if (string.IsNullOrEmpty(assetBundleDownloadOperation.error))
		{
			assetBundleDownloadOperation.assetBundle.m_ReferencedCount = m_DownloadingBundles[assetBundleDownloadOperation.assetBundleName];
			m_LoadedAssetBundles.Add(assetBundleDownloadOperation.assetBundleName, assetBundleDownloadOperation.assetBundle);
		}
		else if (!m_DownloadingErrors.ContainsKey(assetBundleDownloadOperation.assetBundleName))
		{
			string value = $"Failed downloading bundle {assetBundleDownloadOperation.assetBundleName} from {assetBundleDownloadOperation.GetSourceURL()}: {assetBundleDownloadOperation.error}";
			m_DownloadingErrors.Add(assetBundleDownloadOperation.assetBundleName, value);
		}
		m_DownloadingBundles.Remove(assetBundleDownloadOperation.assetBundleName);
	}

	public static T LoadAsset<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
	{
		Log(LogType.Info, "Instant Loading " + assetName + " from " + assetBundleName + " bundle");
		assetBundleName = RemapVariantName(assetBundleName);
		LoadAssetBundle(assetBundleName, async: false, isLoadingAssetBundleManifest: false);
		string error;
		LoadedAssetBundle loadedAssetBundle = GetLoadedAssetBundle(assetBundleName, out error);
		if (!string.IsNullOrEmpty(error))
		{
			Log(LogType.Error, error);
			return null;
		}
		if (loadedAssetBundle == null || loadedAssetBundle.m_AssetBundle == null)
		{
			Log(LogType.Error, "Could not open target assetbundle " + assetBundleName);
			return null;
		}
		T val = loadedAssetBundle.m_AssetBundle.LoadAsset<T>(assetName);
		if (val == null)
		{
			Log(LogType.Error, "Could not open target asset " + assetName + " from assetbundle " + assetBundleName);
			return null;
		}
		return val;
	}

	public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, Type type)
	{
		Log(LogType.Info, "Loading " + assetName + " from " + assetBundleName + " bundle");
		assetBundleName = RemapVariantName(assetBundleName);
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = null;
		if (m_DownloadingErrors.TryGetValue(assetBundleName, out var value))
		{
			Log(LogType.Error, value);
			return null;
		}
		if (!IsBundleExists(assetBundleName))
		{
			if (NKCPatchUtility.BackgroundPatchEnabled() && IsNonEssentialAssetBundle(assetBundleName))
			{
				Log(LogType.Warning, "bundle " + assetBundleName + " Is NonEssential AssetBundle");
			}
			else
			{
				m_DownloadingErrors.Add(assetBundleName, assetBundleName + " Bundle Not Exists");
				Log(LogType.Error, "bundle " + assetBundleName + " does not exist");
			}
			return null;
		}
		LoadAssetBundle(assetBundleName, async: true, isLoadingAssetBundleManifest: false);
		assetBundleLoadAssetOperation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);
		m_InProgressOperations.Add(assetBundleLoadAssetOperation);
		return assetBundleLoadAssetOperation;
	}

	private static bool IsNonEssentialAssetBundle(string assetBundleName)
	{
		string[] array = assetBundleName.Split('.');
		if (array.Length == 2)
		{
			return NKCLocalization.IsVoiceVariant(array[1]);
		}
		return false;
	}

	public static AssetBundleLoadOperation LoadLevelAsync(string levelName, bool isAddictive, AssetBundleLoadLevelOperation.OnComplete dOnComplete)
	{
		return LoadLevelAsync("scene/" + levelName, levelName, isAddictive, dOnComplete);
	}

	public static AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAddictive, AssetBundleLoadLevelOperation.OnComplete dOnComplete)
	{
		Log(LogType.Info, "Loading " + levelName + " from " + assetBundleName + " bundle");
		assetBundleName = RemapVariantName(assetBundleName);
		AssetBundleLoadOperation assetBundleLoadOperation = null;
		LoadAssetBundle(assetBundleName, async: true, isLoadingAssetBundleManifest: false);
		assetBundleLoadOperation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAddictive, dOnComplete);
		m_InProgressOperations.Add(assetBundleLoadOperation);
		return assetBundleLoadOperation;
	}

	public static void LoadLevel(string levelName, bool isAddictive)
	{
		LoadLevel("scene/" + levelName, levelName, isAddictive);
	}

	public static void LoadLevel(string assetBundleName, string levelName, bool isAdditive)
	{
		assetBundleName = RemapVariantName(assetBundleName);
		Log(LogType.Info, "Loading " + levelName + " from " + assetBundleName + " bundle");
		LoadAssetBundle(assetBundleName, async: false, isLoadingAssetBundleManifest: false);
		if (GetLoadedAssetBundle(assetBundleName, out var error) == null)
		{
			Debug.LogError("Scene bundle not found");
		}
		else if (!string.IsNullOrEmpty(error))
		{
			Debug.LogError(error);
		}
		else if (isAdditive)
		{
			SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
		}
		else
		{
			SceneManager.LoadScene(levelName, LoadSceneMode.Single);
		}
	}

	public static string GetRawFilePath(string filePath)
	{
		string[] activeVariants = m_ActiveVariants;
		foreach (string text in activeVariants)
		{
			string text2 = "ASSET_RAW/" + filePath.Replace(".", "_" + text.ToUpper() + ".");
			if (IsFileExists(text2))
			{
				return GetBundleFilePath(text2);
			}
		}
		return GetBundleFilePath("ASSET_RAW/" + filePath);
	}

	public static IEnumerable<string> GetMergedVariantString(string path)
	{
		string[] activeVariants = m_ActiveVariants;
		foreach (string text in activeVariants)
		{
			yield return path.Replace(".", "_" + text.ToUpper() + ".");
		}
	}

	private static bool IsFileExists(string path)
	{
		return NKCPatchUtility.IsFileExists(NKCPatchDownloader.Instance.GetFileFullPath(path));
	}

	public static string GetBundleFilePath(string bundleName)
	{
		if (m_bundlePathCache.TryGetValue(bundleName, out var value))
		{
			return value;
		}
		if (NKCObbUtil.s_bLoadedOBB && NKCObbUtil.IsOBBPath(NKCPatchDownloader.Instance.GetFileFullPath(bundleName)))
		{
			NKCObbUtil.ExtractFile(bundleName);
			string fileFullPath = NKCPatchDownloader.Instance.GetFileFullPath(bundleName);
			m_bundlePathCache[bundleName] = fileFullPath;
			return fileFullPath;
		}
		string fileFullPath2 = NKCPatchDownloader.Instance.GetFileFullPath(bundleName);
		m_bundlePathCache[bundleName] = fileFullPath2;
		return fileFullPath2;
	}

	public static bool IsBundleExists(string bundleName)
	{
		if (string.IsNullOrEmpty(bundleName))
		{
			return false;
		}
		bundleName = RemapVariantName(bundleName);
		return !string.IsNullOrEmpty(GetBundleFilePath(bundleName));
	}

	public static bool IsAssetExists(string bundleName, string assetName)
	{
		bundleName = RemapVariantName(bundleName);
		string error;
		LoadedAssetBundle loadedAssetBundle = GetLoadedAssetBundle(bundleName, out error);
		if (loadedAssetBundle != null && loadedAssetBundle.m_AssetBundle != null)
		{
			return loadedAssetBundle.m_AssetBundle.Contains(assetName);
		}
		return false;
	}

	public static bool IsAssetBundleLoaded(string bundleName)
	{
		string error;
		return GetLoadedAssetBundle(bundleName, out error) != null;
	}

	public static string[] GetAllAssetNameInBundle(string bundleName)
	{
		bundleName = bundleName.ToLower();
		bundleName = RemapVariantName(bundleName);
		string error;
		LoadedAssetBundle loadedAssetBundle = GetLoadedAssetBundle(bundleName, out error);
		if (!string.IsNullOrEmpty(error))
		{
			Debug.LogError(error);
			return null;
		}
		if (loadedAssetBundle == null || loadedAssetBundle.m_AssetBundle == null)
		{
			Debug.Log("target bundle not loaded");
			return null;
		}
		string[] allAssetNames = loadedAssetBundle.m_AssetBundle.GetAllAssetNames();
		if (allAssetNames != null)
		{
			for (int i = 0; i < allAssetNames.Length; i++)
			{
				allAssetNames[i] = Path.GetFileNameWithoutExtension(allAssetNames[i]).ToUpper();
			}
		}
		return allAssetNames;
	}

	public static Stream OpenCryptoBundleFileStream(string path)
	{
		if (path.Contains("jar:"))
		{
			return new NKCAssetbundleInnerStream(path);
		}
		if (NKCObbUtil.IsOBBPath(path))
		{
			return new NKCAssetbundleCryptoStreamMem(NKCObbUtil.GetEntryBufferByFullPath(path), path);
		}
		return new NKCAssetbundleCryptoStream(path, FileMode.Open, FileAccess.Read);
	}

	public static List<string> LoadSavedAssetBundleLists(string fileName)
	{
		List<string> obj = new List<string> { Path.Combine(GetLocalDownloadPath(), fileName) };
		List<string> list = new List<string>(obj.Count());
		foreach (string item in obj)
		{
			if (!File.Exists(item))
			{
				continue;
			}
			using StreamReader streamReader = new StreamReader(item);
			JSONArray asArray = JSONNode.Parse(streamReader.ReadToEnd()).AsArray;
			if (asArray == null)
			{
				continue;
			}
			for (int i = 0; i < asArray.Count; i++)
			{
				list.Add(asArray[i].Value);
			}
			break;
		}
		return list;
	}

	public static void SaveResourceListsToFile(string fileName, string path, HashSet<string> resourceList)
	{
		string path2 = Path.Combine(path, fileName);
		new JSONObject();
		JSONNode jSONNode = new JSONArray();
		int num = 0;
		foreach (string resource in resourceList)
		{
			jSONNode[num] = resource;
			num++;
		}
		using (StreamWriter streamWriter = new StreamWriter(path2, append: false, Encoding.UTF8))
		{
			streamWriter.WriteLine(jSONNode.ToString().ToLower());
		}
		Debug.Log("[SaveResourceListsToFile] fileName[" + fileName + "]");
	}

	public static void SaveLoadedAssetBundleListToFile(string fileName)
	{
		string dataPath = Application.dataPath;
		if (!Directory.Exists(dataPath))
		{
			Directory.CreateDirectory(dataPath);
		}
		string path = Path.Combine(dataPath, fileName);
		new JSONObject();
		JSONNode jSONNode = new JSONArray();
		int num = 0;
		foreach (KeyValuePair<string, LoadedAssetBundle> loadedAssetBundle in m_LoadedAssetBundles)
		{
			new JSONObject()["filename"] = loadedAssetBundle.Key;
			jSONNode[num] = loadedAssetBundle.Key;
			num++;
		}
		using (StreamWriter streamWriter = new StreamWriter(path, append: false, Encoding.UTF8))
		{
			streamWriter.WriteLine(jSONNode.ToString());
		}
		Debug.Log("[SaveLoadedAssets] fileName[" + fileName + "]");
	}
}
