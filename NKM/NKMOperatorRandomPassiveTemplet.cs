using NKM.Templet.Base;

namespace NKM;

public class NKMOperatorRandomPassiveTemplet : INKMTemplet
{
	public int groupId;

	public int operSkillId;

	public int ratio;

	public int extractItemID_SSR;

	public int extractItemID_SR;

	public int extractItemID_R;

	public int extractItemID_N;

	public int Key => groupId;

	public static NKMOperatorRandomPassiveTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorRandomPassiveTemplet.cs", 21))
		{
			return null;
		}
		NKMOperatorRandomPassiveTemplet nKMOperatorRandomPassiveTemplet = new NKMOperatorRandomPassiveTemplet();
		if ((1u & (lua.GetData("m_OprPassiveGroupID", ref nKMOperatorRandomPassiveTemplet.groupId) ? 1u : 0u) & (lua.GetData("m_OperSkillID", ref nKMOperatorRandomPassiveTemplet.operSkillId) ? 1u : 0u) & (lua.GetData("m_Ratio", ref nKMOperatorRandomPassiveTemplet.ratio) ? 1u : 0u) & (lua.GetData("m_ExtractItemID_SSR", ref nKMOperatorRandomPassiveTemplet.extractItemID_SSR) ? 1u : 0u) & (lua.GetData("m_ExtractItemID_SR", ref nKMOperatorRandomPassiveTemplet.extractItemID_SR) ? 1u : 0u) & (lua.GetData("m_ExtractItemID_R", ref nKMOperatorRandomPassiveTemplet.extractItemID_R) ? 1u : 0u) & (lua.GetData("m_ExtractItemID_N", ref nKMOperatorRandomPassiveTemplet.extractItemID_N) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMOperatorRandomPassiveTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
