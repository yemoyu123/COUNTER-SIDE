using NKM.Templet.Base;

namespace NKM;

public class NKMEquipEnchantExpTemplet : INKMTemplet
{
	public int m_EquipTier;

	public int m_EquipEnchantLevel;

	public int m_ReqLevelupExp_N;

	public int m_ReqLevelupExp_R;

	public int m_ReqLevelupExp_SR;

	public int m_ReqLevelupExp_SSR;

	public float m_ReqEnchantFeedEXPBonusRate;

	public int Key => GetHashCode();

	public static NKMEquipEnchantExpTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMEquipEnchantExpTemplet nKMEquipEnchantExpTemplet = new NKMEquipEnchantExpTemplet();
		cNKMLua.GetData("m_EquipTier", ref nKMEquipEnchantExpTemplet.m_EquipTier);
		cNKMLua.GetData("m_EquipEnchantLevel", ref nKMEquipEnchantExpTemplet.m_EquipEnchantLevel);
		cNKMLua.GetData("m_ReqLevelupEXP_N", ref nKMEquipEnchantExpTemplet.m_ReqLevelupExp_N);
		cNKMLua.GetData("m_ReqLevelupEXP_R", ref nKMEquipEnchantExpTemplet.m_ReqLevelupExp_R);
		cNKMLua.GetData("m_ReqLevelupEXP_SR", ref nKMEquipEnchantExpTemplet.m_ReqLevelupExp_SR);
		cNKMLua.GetData("m_ReqLevelupEXP_SSR", ref nKMEquipEnchantExpTemplet.m_ReqLevelupExp_SSR);
		cNKMLua.GetData("m_ReqEnchantFeedEXPBonusRate", ref nKMEquipEnchantExpTemplet.m_ReqEnchantFeedEXPBonusRate);
		return nKMEquipEnchantExpTemplet;
	}

	public override int GetHashCode()
	{
		return (m_EquipTier, m_EquipEnchantLevel).GetHashCode();
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
