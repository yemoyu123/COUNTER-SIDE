using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public class NKMWorldMapCityExpTemplet : INKMTemplet
{
	public int m_Level;

	public int m_ExpRequired;

	public int m_ExpCumulated;

	public int m_LevelUpReqCredit;

	public int Key => m_Level;

	public static NKMWorldMapCityExpTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMWorldMapCityExpTemplet nKMWorldMapCityExpTemplet = new NKMWorldMapCityExpTemplet();
		if ((1u & (cNKMLua.GetData("m_iLevel", ref nKMWorldMapCityExpTemplet.m_Level) ? 1u : 0u) & (cNKMLua.GetData("m_iExpRequired", ref nKMWorldMapCityExpTemplet.m_ExpRequired) ? 1u : 0u) & (cNKMLua.GetData("m_iExpCumulated", ref nKMWorldMapCityExpTemplet.m_ExpCumulated) ? 1u : 0u) & (cNKMLua.GetData("m_LevelUpReqCredit", ref nKMWorldMapCityExpTemplet.m_LevelUpReqCredit) ? 1u : 0u)) == 0)
		{
			Log.Error($"NKMWorldMapCityExpTemplet Load Fail - {nKMWorldMapCityExpTemplet.m_Level}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMWorldMapManager.cs", 155);
			return null;
		}
		return nKMWorldMapCityExpTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
