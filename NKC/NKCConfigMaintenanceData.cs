using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKC.Localization;
using SimpleJSON;
using UnityEngine;

namespace NKC;

public class NKCConfigMaintenanceData
{
	private readonly Dictionary<string, string> _descriptions = new Dictionary<string, string>();

	private const string LANGUAGE_DEF = "LANGUAGE_DEF";

	private const string Description = "Description";

	private const string Use = "Use";

	public bool UseMaintenance { get; private set; }

	private string DefaultDescription { get; set; } = "Maintenance";

	private void Add(string key, string value)
	{
		Debug.Log("[ConfigMaintenance] key: " + key + " / value : " + value);
		if (!_descriptions.ContainsKey(key))
		{
			_descriptions[key] = value;
		}
	}

	public string GetDescription()
	{
		NKM_NATIONAL_CODE languageCode = NKCGameOptionData.LoadLanguageCode(NKM_NATIONAL_CODE.NNC_KOREA);
		Debug.Log("[GetServerMaintenanceString] : " + languageCode);
		string key = NKCLocalization.s_dicLanguageTag.FirstOrDefault((KeyValuePair<string, NKM_NATIONAL_CODE> keyValuePair) => keyValuePair.Value == languageCode).Key;
		if (string.IsNullOrEmpty(key))
		{
			Debug.Log($"[ConfigMaintenance] Not found description in NKCLocalization languageTag Dic _ languageCode : {languageCode}");
			return DefaultDescription;
		}
		if (!_descriptions.TryGetValue(key, out var value))
		{
			Debug.Log($"[ConfigMaintenance] Not found description in config descriptions _ languageCode : {languageCode} _ languageCodeKey : {key}");
			return DefaultDescription;
		}
		return value;
	}

	public void SetDescription(JSONArray defaultTagSetArray, JSONNode maintenanceNode)
	{
		if (maintenanceNode["Use"] == null)
		{
			UseMaintenance = false;
			return;
		}
		UseMaintenance = maintenanceNode["Use"].AsBool;
		if (!UseMaintenance || maintenanceNode["Description"] == null)
		{
			return;
		}
		JSONNode jSONNode = maintenanceNode["Description"]["LANGUAGE_DEF"];
		if (jSONNode == null)
		{
			Log.Warn("[ConfigMaintenance] Config default language is empty", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/Maintenance/NKCConfigMaintenanceData.cs", 76);
		}
		else
		{
			DefaultDescription = jSONNode.ToString().Trim('"');
		}
		foreach (object item in defaultTagSetArray)
		{
			if (item == null)
			{
				continue;
			}
			string text = item.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				string aKey = text.Trim('"');
				JSONNode jSONNode2 = maintenanceNode["Description"][aKey];
				if (!(jSONNode2 == null))
				{
					Debug.Log($"[ConfigMaintenance] Add _ Key : {text}, Value : {jSONNode2}");
					Add(text, jSONNode2.ToString().Trim('"'));
				}
			}
		}
	}
}
