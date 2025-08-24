using System;
using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMShipBuildTemplet : INKMTemplet
{
	public enum BuildUnlockType
	{
		BUT_UNABLE,
		BUT_ALWAYS,
		BUT_PLAYER_LEVEL,
		BUT_DUNGEON_CLEAR,
		BUT_WARFARE_CLEAR,
		BUT_SHIP_GET,
		BUT_SHIP_LV100,
		BUT_WORLDMAP_CITY_COUNT,
		BUT_SHADOW_CLEAR
	}

	public enum ShipUITabType
	{
		SHIP_NORMAL,
		SHIP_EVENT
	}

	private int m_ShipID;

	private string m_ShipName;

	private ShipUITabType m_ShipType;

	private int m_ShipUpgradeTarget1;

	private int m_ShipUpgradeTarget2;

	private int m_ShipUpgradeCredit;

	private List<UpgradeMaterial> m_UpgradeMaterialList = new List<UpgradeMaterial>();

	private BuildUnlockType m_BuildUnlockType;

	private int m_UnlockValue;

	private string m_UnlockPathDesc;

	private List<BuildMaterial> m_BuildMaterialList = new List<BuildMaterial>();

	public int ShipID => m_ShipID;

	public string ShipName => m_ShipName;

	public ShipUITabType ShipType => m_ShipType;

	public int ShipUpgradeTarget1 => m_ShipUpgradeTarget1;

	public int ShipUpgradeTarget2 => m_ShipUpgradeTarget2;

	public int ShipUpgradeCredit => m_ShipUpgradeCredit;

	public List<UpgradeMaterial> UpgradeMaterialList => m_UpgradeMaterialList;

	public BuildUnlockType ShipBuildUnlockType => m_BuildUnlockType;

	public int UnlockValue => m_UnlockValue;

	public string UnlockPathDesc => m_UnlockPathDesc;

	public List<BuildMaterial> BuildMaterialList => m_BuildMaterialList;

	public int Key => m_ShipID;

	public static NKMShipBuildTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 69))
		{
			return null;
		}
		NKMShipBuildTemplet nKMShipBuildTemplet = new NKMShipBuildTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_ShipID", ref nKMShipBuildTemplet.m_ShipID);
		flag &= cNKMLua.GetData("m_ShipName", ref nKMShipBuildTemplet.m_ShipName);
		flag &= cNKMLua.GetData("m_ShipType", ref nKMShipBuildTemplet.m_ShipType);
		cNKMLua.GetData("m_ShipUpgradeTarget1", ref nKMShipBuildTemplet.m_ShipUpgradeTarget1);
		cNKMLua.GetData("m_ShipUpgradeTarget2", ref nKMShipBuildTemplet.m_ShipUpgradeTarget2);
		cNKMLua.GetData("m_ShipUpgradeCredit", ref nKMShipBuildTemplet.m_ShipUpgradeCredit);
		UpgradeMaterial item = default(UpgradeMaterial);
		for (int i = 0; i < 4; i++)
		{
			int rValue = 0;
			int rValue2 = 0;
			string text = (i + 1).ToString("D");
			if ((1u & (cNKMLua.GetData("m_ShipUpgradeMaterial" + text, ref rValue) ? 1u : 0u) & (cNKMLua.GetData("m_ShipUpgradeMaterialCount" + text, ref rValue2) ? 1u : 0u)) != 0)
			{
				if (rValue != 0 && rValue2 > 0)
				{
					item.m_ShipUpgradeMaterial = rValue;
					item.m_ShipUpgradeMaterialCount = rValue2;
					nKMShipBuildTemplet.m_UpgradeMaterialList.Add(item);
				}
				else
				{
					Log.Error($"WARNING : NKMShipBuildTemplet : weird upgrade material data. ID : {nKMShipBuildTemplet.m_ShipID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 106);
				}
			}
		}
		flag &= cNKMLua.GetData("m_ShipBuildUnlockType", ref nKMShipBuildTemplet.m_BuildUnlockType);
		flag &= cNKMLua.GetData("m_ShipBuildUnlockValue", ref nKMShipBuildTemplet.m_UnlockValue);
		cNKMLua.GetData("m_ShipBuildUnlock_Title", ref nKMShipBuildTemplet.m_UnlockPathDesc);
		for (int j = 0; j < 4; j++)
		{
			int rValue3 = 0;
			int rValue4 = 0;
			string rValue5 = "";
			string text2 = (j + 1).ToString("D");
			if ((1u & (cNKMLua.GetData("m_ShipBuildMaterialType_" + text2, ref rValue5) ? 1u : 0u) & (cNKMLua.GetData("m_ShipBuildMaterialID_" + text2, ref rValue3) ? 1u : 0u) & (cNKMLua.GetData("m_ShipBuildMaterialValue_" + text2, ref rValue4) ? 1u : 0u)) != 0)
			{
				_ = (NKM_REWARD_TYPE)Enum.Parse(typeof(NKM_REWARD_TYPE), rValue5);
				if (rValue3 != 0 && rValue4 > 0)
				{
					BuildMaterial item2 = new BuildMaterial
					{
						m_ShipBuildMaterialID = rValue3,
						m_ShipBuildMaterialCount = rValue4
					};
					nKMShipBuildTemplet.m_BuildMaterialList.Add(item2);
				}
				else
				{
					Log.Error($"WARNING : NKMShipBuildTemplet : weird build material data. ID : {nKMShipBuildTemplet.m_ShipID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 142);
				}
			}
		}
		if (!flag)
		{
			return null;
		}
		return nKMShipBuildTemplet;
	}

	public static NKMShipBuildTemplet Find(int key)
	{
		return NKMTempletContainer<NKMShipBuildTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (NKMUnitManager.GetUnitTempletBase(m_ShipID) == null)
		{
			NKMTempletError.Add($"[ShipBuildTemplet] \ufffdԼ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_ShipID : {m_ShipID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 162);
		}
		if (m_ShipUpgradeTarget1 > 0 && NKMUnitManager.GetUnitTempletBase(m_ShipUpgradeTarget1) == null)
		{
			NKMTempletError.Add($"[ShipBuildTemplet] \ufffd\ufffd\ufffd\u05f7\ufffd\ufffd\u0335\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_ShipID : {m_ShipID}, m_ShipUpgradeTarget1 : {m_ShipUpgradeTarget1}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 169);
		}
		if (m_ShipUpgradeTarget2 > 0 && NKMUnitManager.GetUnitTempletBase(m_ShipUpgradeTarget2) == null)
		{
			NKMTempletError.Add($"[ShipBuildTemplet] \ufffd\ufffd\ufffd\u05f7\ufffd\ufffd\u0335\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_ShipID : {m_ShipID}, m_ShipUpgradeTarget2 : {m_ShipUpgradeTarget2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 177);
		}
		switch (m_BuildUnlockType)
		{
		case BuildUnlockType.BUT_DUNGEON_CLEAR:
			if (NKMDungeonManager.GetDungeonTemplet(m_UnlockValue) == null)
			{
				NKMTempletError.Add($"[ShipBuildTemplet] \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(\ufffd\ufffd\ufffd\ufffd Ŭ\ufffd\ufffd\ufffd\ufffd) \ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_ShipID : {m_ShipID}, m_BuildUnlockType : {m_BuildUnlockType}, m_UnlockValue : {m_UnlockValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 186);
			}
			break;
		case BuildUnlockType.BUT_WARFARE_CLEAR:
			if (NKMWarfareTemplet.Find(m_UnlockValue) == null)
			{
				NKMTempletError.Add($"[ShipBuildTemplet] \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(\ufffd\ufffd\ufffd\ufffd Ŭ\ufffd\ufffd\ufffd\ufffd) \ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_ShipID : {m_ShipID}, m_BuildUnlockType : {m_BuildUnlockType}, m_UnlockValue : {m_UnlockValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 193);
			}
			break;
		case BuildUnlockType.BUT_SHIP_GET:
		case BuildUnlockType.BUT_SHIP_LV100:
			if (NKMUnitManager.GetUnitTempletBase(m_UnlockValue) == null)
			{
				NKMTempletError.Add($"[ShipBuildTemplet] \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(\ufffdԼ\ufffd ȹ\ufffd\ufffd) \ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_ShipID : {m_ShipID}, m_BuildUnlockType : {m_BuildUnlockType}, m_UnlockValue : {m_UnlockValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 201);
			}
			break;
		case BuildUnlockType.BUT_SHADOW_CLEAR:
			if (NKMTempletContainer<NKMShadowPalaceTemplet>.Find(m_UnlockValue) == null)
			{
				NKMTempletError.Add($"[ShipBuildTemplet] \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(\ufffd\u05f8\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd Ŭ\ufffd\ufffd\ufffd\ufffd). \ufffdش\ufffd \ufffd\u05f8\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_ShipID : {m_ShipID}, m_BuildUnlockType : {m_BuildUnlockType}, m_UnlockValue : {m_UnlockValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 207);
			}
			break;
		}
		foreach (UpgradeMaterial upgradeMaterial in m_UpgradeMaterialList)
		{
			if (NKMItemManager.GetItemMiscTempletByID(upgradeMaterial.m_ShipUpgradeMaterial) == null || upgradeMaterial.m_ShipUpgradeMaterialCount <= 0)
			{
				Log.ErrorAndExit($"[ShipBuildTemplet] \ufffd\ufffd\ufffd\u05f7\ufffd\ufffd\u0335\ufffd \ufffd\ufffdᰡ \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_ShipID : {m_ShipID}, m_ShipUpgradeMaterial : {upgradeMaterial.m_ShipUpgradeMaterial}, m_ShipUpgradeMaterialCount : {upgradeMaterial.m_ShipUpgradeMaterialCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 220);
			}
		}
		foreach (BuildMaterial buildMaterial in m_BuildMaterialList)
		{
			if (NKMItemManager.GetItemMiscTempletByID(buildMaterial.m_ShipBuildMaterialID) == null || buildMaterial.m_ShipBuildMaterialCount <= 0)
			{
				Log.ErrorAndExit($"[ShipBuildTemplet] \ufffd\ufffd\ufffd\ufffd \ufffd\ufffdᰡ \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd m_ShipID : {m_ShipID}, m_ShipBuildMaterial : {buildMaterial.m_ShipBuildMaterialID}, m_ShipBuildMaterialCount : {buildMaterial.m_ShipBuildMaterialCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMShipManager.cs", 228);
			}
		}
	}
}
