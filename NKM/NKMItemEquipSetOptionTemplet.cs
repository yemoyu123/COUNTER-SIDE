using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public class NKMItemEquipSetOptionTemplet : INKMTemplet
{
	public int m_EquipSetID;

	public string m_OpenTag;

	public string m_EquipSetStrID;

	public int m_EquipSetPart;

	public string m_EquipSetIconEffect;

	public string m_EquipSetName;

	public string m_EquipSetIcon;

	public NKM_STAT_TYPE m_StatType_1 = NKM_STAT_TYPE.NST_RANDOM;

	public float m_StatValue_1;

	public NKM_STAT_TYPE m_StatType_2 = NKM_STAT_TYPE.NST_RANDOM;

	public float m_StatValue_2;

	public bool UseFilter;

	public int Key => m_EquipSetID;

	public static NKMItemEquipSetOptionTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 437))
		{
			return null;
		}
		NKMItemEquipSetOptionTemplet nKMItemEquipSetOptionTemplet = new NKMItemEquipSetOptionTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_EquipSetID", ref nKMItemEquipSetOptionTemplet.m_EquipSetID) ? 1u : 0u) & (cNKMLua.GetData("m_EquipSetStrID", ref nKMItemEquipSetOptionTemplet.m_EquipSetStrID) ? 1u : 0u) & (cNKMLua.GetData("m_EquipSetPart", ref nKMItemEquipSetOptionTemplet.m_EquipSetPart) ? 1u : 0u) & (cNKMLua.GetData("m_EquipSetIconEffect", ref nKMItemEquipSetOptionTemplet.m_EquipSetIconEffect) ? 1u : 0u) & (cNKMLua.GetData("m_EquipSetName", ref nKMItemEquipSetOptionTemplet.m_EquipSetName) ? 1u : 0u) & (cNKMLua.GetData("m_EquipSetIcon", ref nKMItemEquipSetOptionTemplet.m_EquipSetIcon) ? 1u : 0u)) & (cNKMLua.GetData("m_StatType_1", ref nKMItemEquipSetOptionTemplet.m_StatType_1) ? 1 : 0);
		NKMUnitStatManager.LoadStat(cNKMLua, "m_StatValue_1", "m_StatRate_1", ref nKMItemEquipSetOptionTemplet.m_StatType_1, ref nKMItemEquipSetOptionTemplet.m_StatValue_1);
		cNKMLua.GetData("m_StatType_2", ref nKMItemEquipSetOptionTemplet.m_StatType_2);
		NKMUnitStatManager.LoadStat(cNKMLua, "m_StatValue_2", "m_StatRate_2", ref nKMItemEquipSetOptionTemplet.m_StatType_2, ref nKMItemEquipSetOptionTemplet.m_StatValue_2);
		cNKMLua.GetData("UseFilter", ref nKMItemEquipSetOptionTemplet.UseFilter);
		cNKMLua.GetData("m_OpenTag", ref nKMItemEquipSetOptionTemplet.m_OpenTag);
		if (num == 0)
		{
			Log.Error($"NKMItemEquipSetOptionTemplet Load fail - {nKMItemEquipSetOptionTemplet.m_EquipSetID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 463);
			return null;
		}
		return nKMItemEquipSetOptionTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_StatType_1 == NKM_STAT_TYPE.NST_RANDOM)
		{
			NKMTempletError.Add($"NKMItemEquipSetOptionTemplet: id: {m_EquipSetID} 의 StatType:{m_StatType_1}을 확인해주세요", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMItemManager.cs", 478);
		}
	}
}
