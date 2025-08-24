using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMOperatorExpData : INKMTemplet
{
	public int m_iLevel;

	public NKM_UNIT_GRADE m_NKM_UNIT_GRADE;

	public int m_iExpRequiredOpr;

	public int m_iExpCumulatedOpr;

	public int Key => (int)m_NKM_UNIT_GRADE;

	public static NKMOperatorExpData LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorExpTemplet.cs", 54))
		{
			return null;
		}
		NKMOperatorExpData nKMOperatorExpData = new NKMOperatorExpData();
		if ((1u & (lua.GetData("m_iLevel", ref nKMOperatorExpData.m_iLevel) ? 1u : 0u) & (lua.GetData("m_NKM_UNIT_GRADE", ref nKMOperatorExpData.m_NKM_UNIT_GRADE) ? 1u : 0u) & (lua.GetData("m_iExpRequiredOpr", ref nKMOperatorExpData.m_iExpRequiredOpr) ? 1u : 0u) & (lua.GetData("m_iExpCumulatedOpr", ref nKMOperatorExpData.m_iExpCumulatedOpr) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMOperatorExpData;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
