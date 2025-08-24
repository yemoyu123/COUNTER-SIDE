using NKM.Templet.Base;

namespace NKM;

public class NKCEquipAutoWeightTemplet : INKMTemplet
{
	public int index;

	public NKM_STAT_TYPE m_OptionWeight;

	public NKM_FIND_TARGET_TYPE m_OptionWeight_Exception;

	public float m_Prefer_Atk;

	public float m_Prefer_Def;

	public float NURT_DEFENDER;

	public float NURT_RANGER;

	public float NURT_STRIKER;

	public float NURT_SNIPER;

	public float NURT_SUPPORTER;

	public float NURT_TOWER;

	public float NURT_SIEGE;

	public float NURT_INVALID;

	public int Key => (int)m_OptionWeight;

	public static NKCEquipAutoWeightTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKCEquipAutoWeightTemplet nKCEquipAutoWeightTemplet = new NKCEquipAutoWeightTemplet();
		int num = 1 & (cNKMLua.GetData("m_OptionWeight", ref nKCEquipAutoWeightTemplet.m_OptionWeight) ? 1 : 0);
		cNKMLua.GetData("m_OptionWeight_Exception", ref nKCEquipAutoWeightTemplet.m_OptionWeight_Exception);
		_ = (uint)num & (cNKMLua.GetData("m_Prefer_Atk", ref nKCEquipAutoWeightTemplet.m_Prefer_Atk) ? 1u : 0u) & (cNKMLua.GetData("m_Prefer_Def", ref nKCEquipAutoWeightTemplet.m_Prefer_Def) ? 1u : 0u) & (cNKMLua.GetData("NURT_DEFENDER", ref nKCEquipAutoWeightTemplet.NURT_DEFENDER) ? 1u : 0u) & (cNKMLua.GetData("NURT_RANGER", ref nKCEquipAutoWeightTemplet.NURT_RANGER) ? 1u : 0u) & (cNKMLua.GetData("NURT_STRIKER", ref nKCEquipAutoWeightTemplet.NURT_STRIKER) ? 1u : 0u) & (cNKMLua.GetData("NURT_SNIPER", ref nKCEquipAutoWeightTemplet.NURT_SNIPER) ? 1u : 0u) & (cNKMLua.GetData("NURT_SUPPORTER", ref nKCEquipAutoWeightTemplet.NURT_SUPPORTER) ? 1u : 0u) & (cNKMLua.GetData("NURT_TOWER", ref nKCEquipAutoWeightTemplet.NURT_TOWER) ? 1u : 0u) & (cNKMLua.GetData("NURT_SIEGE", ref nKCEquipAutoWeightTemplet.NURT_SIEGE) ? 1u : 0u);
		cNKMLua.GetData("NURT_INVALID", ref nKCEquipAutoWeightTemplet.NURT_INVALID);
		nKCEquipAutoWeightTemplet.CheckValidation();
		return nKCEquipAutoWeightTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	private void CheckValidation()
	{
	}
}
