using NKM.Templet.Base;

namespace NKM;

public class NKMGameStatRateTemplet : INKMTemplet
{
	public int m_ID;

	public string m_strID;

	public NKMGameStatRate m_StatRate = new NKMGameStatRate();

	public int Key => m_ID;

	public static NKMGameStatRateTemplet Find(int key)
	{
		return NKMTempletContainer<NKMGameStatRateTemplet>.Find(key);
	}

	public static NKMGameStatRateTemplet Find(string strKey)
	{
		return NKMTempletContainer<NKMGameStatRateTemplet>.Find(strKey);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public static NKMGameStatRateTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 1527))
		{
			return null;
		}
		NKMGameStatRateTemplet nKMGameStatRateTemplet = new NKMGameStatRateTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_ID", ref nKMGameStatRateTemplet.m_ID) ? 1u : 0u) & (cNKMLua.GetData("m_strID", ref nKMGameStatRateTemplet.m_strID) ? 1u : 0u) & (cNKMLua.GetData("m_MainStatFactorRate", ref nKMGameStatRateTemplet.m_StatRate.m_MainStatFactorRate) ? 1u : 0u)) & (cNKMLua.GetData("m_SubStatRate", ref nKMGameStatRateTemplet.m_StatRate.m_SubStatValueRate) ? 1 : 0);
		cNKMLua.GetData("m_EquipStatValueRate", ref nKMGameStatRateTemplet.m_StatRate.m_EquipStatRate);
		if (((uint)num & (cNKMLua.GetData("NST_HP", ref nKMGameStatRateTemplet.m_StatRate.m_HPRate) ? 1u : 0u) & (cNKMLua.GetData("NST_ATK", ref nKMGameStatRateTemplet.m_StatRate.m_ATKRate) ? 1u : 0u) & (cNKMLua.GetData("NST_DEF", ref nKMGameStatRateTemplet.m_StatRate.m_DEFRate) ? 1u : 0u) & (cNKMLua.GetData("NST_CRITICAL", ref nKMGameStatRateTemplet.m_StatRate.m_CRITRate) ? 1u : 0u) & (cNKMLua.GetData("NST_HIT", ref nKMGameStatRateTemplet.m_StatRate.m_HITRate) ? 1u : 0u) & (cNKMLua.GetData("NST_EVADE", ref nKMGameStatRateTemplet.m_StatRate.m_EvadeRate) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMGameStatRateTemplet;
	}
}
