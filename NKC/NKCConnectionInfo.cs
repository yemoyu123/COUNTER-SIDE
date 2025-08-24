using System;
using System.Collections.Generic;
using System.IO;
using AssetBundles;
using Cs.Logging;
using NKC.Patcher;
using NKM;
using SimpleJSON;
using UnityEngine;

namespace NKC;

public static class NKCConnectionInfo
{
	public enum LOGIN_SERVER_TYPE
	{
		None,
		Default,
		Korea,
		Global,
		Max
	}

	public enum SERVER_INFO_TYPE
	{
		Original,
		SelectServer
	}

	public class LoginServerInfo
	{
		public HashSet<string> m_defaultTagSet = new HashSet<string>();

		public string m_serviceIP { get; set; }

		public int m_servicePort { get; set; }

		public LoginServerInfo(string ip = "", int port = 22000)
		{
			m_serviceIP = ip;
			m_servicePort = port;
		}
	}

	public const string ConnectionFilePath = "ConnectionInfo.json";

	public const string DataVersionJSONName = "dv";

	public const string IPJSONName = "ip";

	public const string PortJSONName = "port";

	public const string ServerTypeJSONName = "type";

	private const string ServerInfoJSONName = "server";

	private const string DOWNLOAD_TIMEOUT = "DownloadTimeout";

	private const string CDN = "cdn";

	private const string VERSION_JSON = "versionJson";

	private const string IGNORE_VARIANT_LIST = "IgnoreVariantList";

	private const string DEFAULT_TAG_SET = "defaultTagSet";

	public const int LOGIN_SERVER_PORT = 22000;

	public const string DEFAULT_SERVER_ADDRESS = "192.168.0.201";

	public const string m_LOCAL_SAVE_LAST_LOGIN_SERVER_TYPE = "LOCAL_SAVE_LAST_LOGIN_SERVER_TYPE";

	public static string s_LoginFailMsg = "";

	public static string s_ServerType;

	private static Dictionary<SERVER_INFO_TYPE, string> m_dicServerInfoTypeString = new Dictionary<SERVER_INFO_TYPE, string>
	{
		{
			SERVER_INFO_TYPE.Original,
			"ServerInfo.json"
		},
		{
			SERVER_INFO_TYPE.SelectServer,
			"ServerInfo_V2.json"
		}
	};

	private static Dictionary<LOGIN_SERVER_TYPE, LoginServerInfo> m_dicLoginServerInfo = new Dictionary<LOGIN_SERVER_TYPE, LoginServerInfo>();

	private static readonly Dictionary<LOGIN_SERVER_TYPE, string> m_dicLoginServerPostFix = new Dictionary<LOGIN_SERVER_TYPE, string>
	{
		{
			LOGIN_SERVER_TYPE.None,
			""
		},
		{
			LOGIN_SERVER_TYPE.Default,
			""
		},
		{
			LOGIN_SERVER_TYPE.Korea,
			"_KOR"
		},
		{
			LOGIN_SERVER_TYPE.Global,
			"_GLOBAL"
		},
		{
			LOGIN_SERVER_TYPE.Max,
			""
		}
	};

	private static LOGIN_SERVER_TYPE m_currentLoginServerType = LOGIN_SERVER_TYPE.None;

	private static DateTime m_lastDownloadedTime;

	private static double DOWNLOAD_INTERVAL = 180.0;

	public static string m_downloadedConfigJSONString;

	private static bool m_isUnderMaintenance = false;

	public const string MAINTENANCE_DESC_STRING_KEY = "SI_SYSTEM_NOTICE_MAINTENANCE_DESC";

	private static SERVER_INFO_TYPE ServerInfoType
	{
		get
		{
			if (NKCDefineManager.DEFINE_SELECT_SERVER())
			{
				Log.Debug("[ServerInfoType] SelectServer", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCConnectionInfo.cs", 77);
				return SERVER_INFO_TYPE.SelectServer;
			}
			Log.Debug("[ServerInfoType] Original", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCConnectionInfo.cs", 81);
			return SERVER_INFO_TYPE.Original;
		}
	}

	public static string CustomServerInfoAddress
	{
		get
		{
			string text = "CSConfigServerAddress.txt";
			if (NKCDefineManager.DEFINE_SB_GB())
			{
				text = "csconfigserveraddress.txt";
			}
			string text2 = Application.streamingAssetsPath + "/" + text;
			Debug.Log("[ServerInfoAddress] ServerInfoAddressPath : " + text2);
			if (NKCPatchUtility.IsFileExists(text2))
			{
				Debug.Log("[ServerInfoAddress] ServerInfoAddress exist");
				string text3 = "";
				text3 = ((!text2.Contains("jar:")) ? File.ReadAllText(text2) : BetterStreamingAssets.ReadAllText(NKCAssetbundleInnerStream.GetJarRelativePath(text2)));
				JSONNode jSONNode = JSONNode.Parse(text3);
				if (jSONNode != null)
				{
					return jSONNode["address"];
				}
			}
			return "";
		}
	}

	public static string ServerInfoFileName => m_dicServerInfoTypeString[ServerInfoType];

	public static IEnumerable<LoginServerInfo> LoginServerInfos => m_dicLoginServerInfo.Values;

	public static LOGIN_SERVER_TYPE CurrentLoginServerType
	{
		get
		{
			if (!NKCDefineManager.DEFINE_SELECT_SERVER())
			{
				return LOGIN_SERVER_TYPE.Default;
			}
			return m_currentLoginServerType;
		}
		set
		{
			m_currentLoginServerType = value;
		}
	}

	public static LOGIN_SERVER_TYPE LastLoginServerType => (LOGIN_SERVER_TYPE)PlayerPrefs.GetInt("LOCAL_SAVE_LAST_LOGIN_SERVER_TYPE", 0);

	public static string ServiceIP
	{
		get
		{
			if (!HasLoginServerInfo(CurrentLoginServerType))
			{
				Debug.LogError($"[ConnectionInfo] Not Setted Login Server. CurrentLoginServerType : {CurrentLoginServerType}");
				return "";
			}
			return m_dicLoginServerInfo[CurrentLoginServerType].m_serviceIP;
		}
	}

	public static int ServicePort
	{
		get
		{
			if (!HasLoginServerInfo(CurrentLoginServerType))
			{
				Debug.LogError($"[ConnectionInfo] Not Setted Login Server. CurrentLoginServerType : {CurrentLoginServerType}");
				return -1;
			}
			return m_dicLoginServerInfo[CurrentLoginServerType].m_servicePort;
		}
	}

	public static HashSet<string> CurrentLoginServerTagSet
	{
		get
		{
			if (!HasLoginServerInfo(CurrentLoginServerType))
			{
				Debug.LogError($"[ConnectionInfo] Not Setted Login Server. CurrentLoginServerType : {CurrentLoginServerType}");
				return null;
			}
			return m_dicLoginServerInfo[CurrentLoginServerType].m_defaultTagSet;
		}
	}

	public static string DownloadServerAddress { get; set; }

	public static string DownloadServerAddress2 { get; set; }

	public static List<string> IgnoreVariantList { get; set; } = new List<string>();

	public static string VersionJson { get; private set; } = "/version.json";

	public static int DownloadTimeout { get; private set; } = 300;

	public static bool HasLoginServerInfo(LOGIN_SERVER_TYPE loginServerType)
	{
		if (m_dicLoginServerInfo.ContainsKey(loginServerType))
		{
			return true;
		}
		return false;
	}

	public static int GetLoginServerCount()
	{
		return m_dicLoginServerInfo.Count;
	}

	public static LOGIN_SERVER_TYPE GetFirstLoginServerType()
	{
		using (Dictionary<LOGIN_SERVER_TYPE, LoginServerInfo>.KeyCollection.Enumerator enumerator = m_dicLoginServerInfo.Keys.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return LOGIN_SERVER_TYPE.None;
	}

	public static string GetCurrentLoginServerPostFix()
	{
		return GetLoginServerPostFix(CurrentLoginServerType);
	}

	public static string GetLoginServerPostFix(LOGIN_SERVER_TYPE serverType)
	{
		if (!m_dicLoginServerPostFix.ContainsKey(serverType))
		{
			return string.Empty;
		}
		return m_dicLoginServerPostFix[serverType];
	}

	public static string GetLoginServerString(LOGIN_SERVER_TYPE serverType, string strID, bool bSkipErrorCheck = false)
	{
		return NKCStringTable.GetString(strID + GetLoginServerPostFix(serverType), bSkipErrorCheck);
	}

	public static string GetCurrentLoginServerString(string strID, bool bSkipErrorCheck = false)
	{
		return GetLoginServerString(CurrentLoginServerType, strID, bSkipErrorCheck);
	}

	public static void SetLoginServerInfo(LOGIN_SERVER_TYPE loginServerType, string ip = "", int port = -1, JSONArray defaultTagSet = null)
	{
		Debug.Log($"[ConnectionInfo] ConnectionInfo Updated : type {loginServerType}, ip {ip}, port {port}, protocol {960}, data {NKMDataVersion.DataVersion}");
		LoginServerInfo loginServerInfo;
		if (!HasLoginServerInfo(loginServerType))
		{
			loginServerInfo = new LoginServerInfo(ip, port);
			m_dicLoginServerInfo.Add(loginServerType, loginServerInfo);
		}
		else
		{
			loginServerInfo = m_dicLoginServerInfo[loginServerType];
			if (!string.IsNullOrEmpty(ip))
			{
				loginServerInfo.m_serviceIP = ip;
			}
			if (port > -1)
			{
				loginServerInfo.m_servicePort = port;
			}
		}
		if (!(defaultTagSet != null))
		{
			return;
		}
		foreach (JSONNode child in defaultTagSet.Children)
		{
			loginServerInfo.m_defaultTagSet.Add(child);
		}
	}

	public static void SaveCurrentLoginServerType()
	{
		Debug.Log($"[ConnectionInfo] SaveLocalTag {CurrentLoginServerType}");
		PlayerPrefs.SetInt("LOCAL_SAVE_LAST_LOGIN_SERVER_TYPE", (int)CurrentLoginServerType);
	}

	public static void DeleteLocalTag()
	{
		Debug.Log("[ConnectionInfo] DeleteLocalTag");
		PlayerPrefs.DeleteKey("LOCAL_SAVE_LAST_LOGIN_SERVER_TYPE");
	}

	public static void Clear()
	{
		m_dicLoginServerInfo.Clear();
	}

	public static void LoadFromJSON(string jsonString)
	{
		JSONNode jSONNode = JSON.Parse(jsonString);
		if (jSONNode != null)
		{
			LoadFromJSON(jSONNode);
			SetConfigJSONString(jsonString);
		}
	}

	public static bool IsServerUnderMaintenance()
	{
		if (!NKMContentsVersionManager.HasTag("CHECK_MAINTENANCE"))
		{
			if (NKCDefineManager.DEFINE_USE_CHEAT())
			{
				Log.Debug("[IsServerUnderMaintenance] Need Tag", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCConnectionInfo.cs", 333);
				NKMContentsVersionManager.PrintCurrentTagSet();
			}
			return false;
		}
		Path.Combine(AssetBundleManager.GetLocalDownloadPath(), "manifest_test.ini");
		if (File.Exists("localDownloadPath"))
		{
			Log.Debug("[IsServerUnderMaintenance] manifest_test.ini exists", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCConnectionInfo.cs", 344);
			return false;
		}
		return m_isUnderMaintenance;
	}

	public static bool CheckDownloadInterval()
	{
		if (string.IsNullOrEmpty(m_downloadedConfigJSONString))
		{
			Log.Debug("[PatcherManager][Maintenance][CheckDownloadInterval] configString is empty", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCConnectionInfo.cs", 355);
			return true;
		}
		if (DateTime.Now.Subtract(m_lastDownloadedTime).TotalSeconds < DOWNLOAD_INTERVAL)
		{
			Log.Debug("[PatcherManager][Maintenance][CheckDownloadInterval] too early", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCConnectionInfo.cs", 363);
			return false;
		}
		return true;
	}

	public static void SetConfigJSONString(string jsonString)
	{
		m_downloadedConfigJSONString = jsonString;
		m_lastDownloadedTime = DateTime.Now;
	}

	public static void LoadMaintenanceDataFromJSON()
	{
		Log.Debug("LoadMaintenanceDataFromJSON - Start", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCConnectionInfo.cs", 379);
		m_isUnderMaintenance = false;
		if (string.IsNullOrEmpty(m_downloadedConfigJSONString))
		{
			Debug.LogError("LoadMaintenanceDataFromJSON - Downloaded JSON is empty");
			return;
		}
		JSONNode jSONNode = JSON.Parse(m_downloadedConfigJSONString);
		if (jSONNode == null)
		{
			Debug.LogError("LoadMaintenanceDataFromJSON - Parse failed");
			return;
		}
		JSONNode jSONNode2 = jSONNode["server"][m_currentLoginServerType.ToString()];
		if (jSONNode2 == null)
		{
			Debug.LogError($"LoadMaintenanceDataFromJSON - CurrentLoginServerType Node is null!! type[{m_currentLoginServerType}]");
			return;
		}
		JSONNode jSONNode3 = jSONNode2["Maintenance"];
		if (jSONNode3 == null)
		{
			Debug.LogError($"LoadMaintenanceDataFromJSON - maintenanceNode Node is null!! type[{m_currentLoginServerType}]");
			return;
		}
		if (jSONNode3["Interval"] != null)
		{
			DOWNLOAD_INTERVAL = jSONNode3["Interval"].AsDouble;
		}
		if (jSONNode3["Use"] != null)
		{
			m_isUnderMaintenance = jSONNode3["Use"].AsBool;
			Log.Debug($"LoadMaintenanceDataFromJSON - Use : {m_isUnderMaintenance}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCConnectionInfo.cs", 419);
		}
		if (m_isUnderMaintenance)
		{
			NKM_NATIONAL_CODE nationalCode = NKCGameOptionData.LoadLanguageCode(NKM_NATIONAL_CODE.NNC_KOREA);
			string text = jSONNode3["Description"][nationalCode.ToString()];
			if (text == null)
			{
				text = jSONNode3["Description"]["DEFAULT"];
			}
			if (!string.IsNullOrEmpty(text))
			{
				NKCStringTable.AddString(nationalCode, "SI_SYSTEM_NOTICE_MAINTENANCE_DESC", text, bOverwriteDuplicate: true);
			}
		}
	}

	public static void LoadFromJSON(JSONNode node)
	{
		SERVER_INFO_TYPE serverInfoType = ServerInfoType;
		if (serverInfoType == SERVER_INFO_TYPE.Original || serverInfoType != SERVER_INFO_TYPE.SelectServer)
		{
			SetLoginServerInfo(LOGIN_SERVER_TYPE.Default, node["ip"], node["port"].AsInt);
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				LOGIN_SERVER_TYPE lOGIN_SERVER_TYPE = (LOGIN_SERVER_TYPE)i;
				string aKey = lOGIN_SERVER_TYPE.ToString();
				if (node["server"][aKey] == null)
				{
					continue;
				}
				JSONArray defaultTagSet = node["server"][aKey]["defaultTagSet"]?.AsArray;
				SetLoginServerInfo((LOGIN_SERVER_TYPE)i, node["server"][aKey]["ip"], node["server"][aKey]["port"].AsInt, defaultTagSet);
				m_isUnderMaintenance = false;
				JSONNode jSONNode = node["server"][aKey]["Maintenance"];
				if (!(jSONNode != null))
				{
					continue;
				}
				if (jSONNode["Use"] != null)
				{
					m_isUnderMaintenance = jSONNode["Use"].AsBool;
				}
				if (m_isUnderMaintenance)
				{
					NKM_NATIONAL_CODE nationalCode = NKCGameOptionData.LoadLanguageCode(NKM_NATIONAL_CODE.NNC_KOREA);
					string text = jSONNode["Description"][nationalCode.ToString()];
					if (text == null)
					{
						text = jSONNode["Description"]["DEFAULT"];
					}
					if (!string.IsNullOrEmpty(text))
					{
						NKCStringTable.AddString(nationalCode, "SI_SYSTEM_NOTICE_MAINTENANCE_DESC", text, bOverwriteDuplicate: true);
					}
				}
			}
		}
		s_ServerType = node["type"];
		if (node["cdn"] != null)
		{
			DownloadServerAddress = node["cdn"];
		}
		if (node["versionJson"] != null)
		{
			VersionJson = node["versionJson"];
		}
		if (node["DownloadTimeout"] != null)
		{
			DownloadTimeout = node["DownloadTimeout"];
		}
		foreach (JSONNode child in (node["IgnoreVariantList"]?.AsArray).Children)
		{
			IgnoreVariantList.Add(child);
		}
	}

	public static void UpdateDataVersionOnly()
	{
		string fileFullPath = NKCPatchDownloader.Instance.GetFileFullPath("ConnectionInfo.json");
		if (NKCPatchUtility.IsFileExists(fileFullPath))
		{
			NKMDataVersion.DataVersion = JSONNode.LoadFromFile(fileFullPath)["dv"].AsInt;
		}
	}
}
