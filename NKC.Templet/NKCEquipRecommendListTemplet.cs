using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCEquipRecommendListTemplet : INKMTemplet
{
	public int m_EquipRecommendID;

	public int m_WeaponSlot;

	public int m_DefenceSlot;

	public int m_ACCSlot_1;

	public int m_ACCSlot_2;

	public int Key => m_EquipRecommendID;

	public static NKCEquipRecommendListTemplet LoadFromLua(NKMLua cNKMLua)
	{
		NKCEquipRecommendListTemplet nKCEquipRecommendListTemplet = new NKCEquipRecommendListTemplet();
		cNKMLua.GetData("EquipRecommendID", ref nKCEquipRecommendListTemplet.m_EquipRecommendID);
		cNKMLua.GetData("WeaponSlot", ref nKCEquipRecommendListTemplet.m_WeaponSlot);
		cNKMLua.GetData("DefenceSlot", ref nKCEquipRecommendListTemplet.m_DefenceSlot);
		cNKMLua.GetData("ACCSlot_1", ref nKCEquipRecommendListTemplet.m_ACCSlot_1);
		cNKMLua.GetData("ACCSlot_2", ref nKCEquipRecommendListTemplet.m_ACCSlot_2);
		return nKCEquipRecommendListTemplet;
	}

	public static NKCEquipRecommendListTemplet Find(int recommendID)
	{
		return NKMTempletContainer<NKCEquipRecommendListTemplet>.Find(recommendID);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
