using System.Collections.Generic;
using System.Linq;
using NKM.Contract2;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMItemEquipUpgradeTemplet : INKMTemplet
{
	private const int MaxMaterialCount = 10;

	private int upgradeIdx;

	private int upgradeEquipId;

	private int coreEquipId;

	private string openTag;

	private readonly List<MiscItemUnit> miscMaterials = new List<MiscItemUnit>();

	private readonly List<MiscItemUnit> equipMaterials = new List<MiscItemUnit>();

	public int Key => coreEquipId;

	public int IDX => upgradeIdx;

	public NKMEquipTemplet CoreEquipTemplet { get; private set; }

	public NKMEquipTemplet UpgradeEquipTemplet { get; private set; }

	public IReadOnlyList<MiscItemUnit> MiscMaterials => miscMaterials;

	public IReadOnlyList<MiscItemUnit> EquipMaterials => equipMaterials;

	public string OpenTag => openTag;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(OpenTag);

	public static NKMItemEquipUpgradeTemplet Find(int coreEquipID)
	{
		return NKMTempletContainer<NKMItemEquipUpgradeTemplet>.Find(coreEquipID);
	}

	public static int GetMaxUpgradedEquip(int equipID)
	{
		NKMItemEquipUpgradeTemplet nKMItemEquipUpgradeTemplet = NKMTempletContainer<NKMItemEquipUpgradeTemplet>.Find(equipID);
		if (nKMItemEquipUpgradeTemplet != null)
		{
			return GetMaxUpgradedEquip(nKMItemEquipUpgradeTemplet.upgradeEquipId);
		}
		return equipID;
	}

	public static NKMItemEquipUpgradeTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 44))
		{
			return null;
		}
		NKMItemEquipUpgradeTemplet nKMItemEquipUpgradeTemplet = new NKMItemEquipUpgradeTemplet();
		nKMItemEquipUpgradeTemplet.upgradeIdx = lua.GetInt32("UpgradeIDX");
		nKMItemEquipUpgradeTemplet.upgradeEquipId = lua.GetInt32("UpgradeEquipID");
		nKMItemEquipUpgradeTemplet.coreEquipId = lua.GetInt32("CoreEquipID");
		lua.GetData("m_OpenTag", ref nKMItemEquipUpgradeTemplet.openTag);
		long @int = lua.GetInt64("UpgradeReqResource");
		nKMItemEquipUpgradeTemplet.miscMaterials.Add(new MiscItemUnit(1, @int));
		for (int i = 0; i < 10; i++)
		{
			int num = i + 1;
			if (!lua.GetDataEnum<NKM_REWARD_TYPE>($"Material{num}_ItemType", out var result))
			{
				break;
			}
			int rValue = 0;
			int rValue2 = 0;
			lua.GetData($"Material{num}_ItemID", ref rValue);
			lua.GetData($"Material{num}_ItemCount", ref rValue2);
			MiscItemUnit item = new MiscItemUnit(rValue, rValue2);
			switch (result)
			{
			case NKM_REWARD_TYPE.RT_MISC:
				nKMItemEquipUpgradeTemplet.miscMaterials.Add(item);
				break;
			case NKM_REWARD_TYPE.RT_EQUIP:
				nKMItemEquipUpgradeTemplet.equipMaterials.Add(item);
				break;
			default:
				NKMTempletError.Add($"[EquipUpgrade] unsupported rewardType:{result}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 81);
				break;
			}
		}
		return nKMItemEquipUpgradeTemplet;
	}

	public void Join()
	{
		CoreEquipTemplet = NKMTempletContainer<NKMEquipTemplet>.Find(coreEquipId);
		if (CoreEquipTemplet == null)
		{
			NKMTempletError.Add($"[ItemEquipUpgradeTemple:{upgradeIdx}] 업그레이드 기반 장비 정보가 없음. upgradeEquipId:{coreEquipId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 94);
		}
		UpgradeEquipTemplet = NKMTempletContainer<NKMEquipTemplet>.Find(upgradeEquipId);
		if (UpgradeEquipTemplet == null)
		{
			NKMTempletError.Add($"[ItemEquipUpgradeTemple:{upgradeIdx}] 업그레이드 결과 장비 정보가 없음. upgradeEquipId:{upgradeEquipId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 100);
		}
	}

	public void Validate()
	{
		if (CheckCycle())
		{
			NKMTempletError.Add($"[ItemEquipUpgradeTemple:{upgradeIdx}] 업그레이드 재료 장비와 결과 장비가 같거나 트리에 Cycle이 존재함. equipId : {coreEquipId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 108);
		}
		if (CoreEquipTemplet.m_StatType != UpgradeEquipTemplet.m_StatType)
		{
			NKMTempletError.Add($"[ItemEquipUpgradeTemple:{upgradeIdx}] 업그레이드 장비의 메인 스텟 정보가 불일치. coreEquipTemplet.StatType:{CoreEquipTemplet.m_StatType} upgradeEquipTemplet.StatType:{UpgradeEquipTemplet.m_StatType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 113);
		}
		int[] array = (from e in equipMaterials
			group e by e.ItemId into e
			where e.Count() > 1
			select e.Key).ToArray();
		if (array.Any())
		{
			string arg = string.Join(", ", array);
			NKMTempletError.Add($"[ItemEquipUpgradeTemple:{upgradeIdx}] 업그레이드 장비 재료 중복. duplicateEquipId:{arg}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 124);
		}
		foreach (MiscItemUnit equipMaterial in equipMaterials)
		{
			if (NKMItemManager.GetEquipTemplet(equipMaterial.ItemId) == null)
			{
				NKMTempletError.Add($"[ItemEquipUpgradeTemple:{upgradeIdx}] 업그레이드 재료 장비 정보가 없음. material_Id:{equipMaterial}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 131);
			}
			if (equipMaterial.Count <= 0 || equipMaterial.Count > 1)
			{
				NKMTempletError.Add($"[ItemEquipUpgradeTemple:{upgradeIdx}] 업그레이드 재료 장비 필요 개수가 1이 아님. material_Id:{equipMaterial}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 136);
			}
		}
		array = (from e in miscMaterials
			group e by e.ItemId into e
			where e.Count() > 1
			select e.Key).ToArray();
		if (array.Any())
		{
			string arg2 = string.Join(", ", array);
			NKMTempletError.Add($"[ItemEquipUpgradeTemple:{upgradeIdx}] 업그레이드 misc 재료 중복. duplicateMiscId:{arg2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 148);
		}
		foreach (MiscItemUnit miscMaterial in miscMaterials)
		{
			if (NKMItemManager.GetItemMiscTempletByID(miscMaterial.ItemId) == null)
			{
				NKMTempletError.Add($"[ItemEquipUpgradeTemple:{upgradeIdx}] 업그레이드 재료 mics 정보가 없음. material_Id:{miscMaterial.ItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 155);
			}
			if (miscMaterial.Count <= 0)
			{
				NKMTempletError.Add($"[ItemEquipUpgradeTemple:{upgradeIdx}] 업그레이드 재료 misc 필요 개수가 0 이하. material_Id:{miscMaterial.ItemId}, material_count:{miscMaterial.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMItemEquipUpgradeTemplet.cs", 160);
			}
		}
	}

	private bool CheckCycle()
	{
		if (coreEquipId == upgradeEquipId)
		{
			return true;
		}
		for (NKMItemEquipUpgradeTemplet nKMItemEquipUpgradeTemplet = Find(upgradeEquipId); nKMItemEquipUpgradeTemplet != null; nKMItemEquipUpgradeTemplet = Find(nKMItemEquipUpgradeTemplet.upgradeEquipId))
		{
			if (nKMItemEquipUpgradeTemplet.upgradeEquipId == coreEquipId)
			{
				return true;
			}
		}
		return false;
	}
}
