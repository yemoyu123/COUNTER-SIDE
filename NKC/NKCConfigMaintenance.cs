using System.Collections.Generic;
using Cs.Logging;
using SimpleJSON;

namespace NKC;

public class NKCConfigMaintenance
{
	private readonly Dictionary<NKCConnectionInfo.LOGIN_SERVER_TYPE, NKCConfigMaintenanceData> _configMaintenanceData = new Dictionary<NKCConnectionInfo.LOGIN_SERVER_TYPE, NKCConfigMaintenanceData>();

	public bool UseMaintenance(NKCConnectionInfo.LOGIN_SERVER_TYPE type)
	{
		if (!_configMaintenanceData.TryGetValue(type, out var value))
		{
			Log.Warn($"[ConfigMaintenance][UseMaintenance] Invalid serverType _ type : {type}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/Maintenance/NKCConfigMaintenance.cs", 15);
			return false;
		}
		return value.UseMaintenance;
	}

	public string GetDescription(NKCConnectionInfo.LOGIN_SERVER_TYPE type)
	{
		if (!_configMaintenanceData.TryGetValue(type, out var value))
		{
			Log.Warn($"[ConfigMaintenance][GetDescription] Invalid serverType _ type : {type}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/Maintenance/NKCConfigMaintenance.cs", 26);
			return string.Empty;
		}
		return value.GetDescription();
	}

	public void SetDescription(NKCConnectionInfo.LOGIN_SERVER_TYPE type, JSONArray defaultTagSetArray, JSONNode maintenanceNode)
	{
		if (!_configMaintenanceData.TryGetValue(type, out var value))
		{
			value = new NKCConfigMaintenanceData();
			_configMaintenanceData.Add(type, value);
		}
		value.SetDescription(defaultTagSetArray, maintenanceNode);
	}
}
