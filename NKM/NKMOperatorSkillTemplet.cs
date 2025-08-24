using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM;

public class NKMOperatorSkillTemplet : INKMTemplet
{
	public int m_OperSkillID;

	public string m_OperSkillStrID;

	public string m_OperSkillNameStrID;

	public string m_OperSkillDescStrID;

	public string m_OperSkillIcon;

	public OperatorSkillType m_OperSkillType;

	public string m_OperSkillTarget;

	public int m_MaxSkillLevel;

	public bool UseFilter;

	public int Key => m_OperSkillID;

	public static NKMOperatorSkillTemplet Find(int id)
	{
		return NKMTempletContainer<NKMOperatorSkillTemplet>.Find(id);
	}

	public static NKMOperatorSkillTemplet Find(string strID)
	{
		return NKMTempletContainer<NKMOperatorSkillTemplet>.Find(strID);
	}

	public static List<string> GetAllSkillStrID_ByType(OperatorSkillType eOperatorSkillType)
	{
		List<string> list = new List<string>();
		foreach (NKMOperatorSkillTemplet value in NKMTempletContainer<NKMOperatorSkillTemplet>.Values)
		{
			if (value != null && value.m_OperSkillType == eOperatorSkillType)
			{
				list.Add(value.m_OperSkillStrID);
			}
		}
		return list;
	}

	public static NKMOperatorSkillTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorSkillTemplet.cs", 55))
		{
			return null;
		}
		NKMOperatorSkillTemplet nKMOperatorSkillTemplet = new NKMOperatorSkillTemplet();
		int num = (int)(1u & (lua.GetData("m_OperSkillID", ref nKMOperatorSkillTemplet.m_OperSkillID) ? 1u : 0u) & (lua.GetData("m_OperSkillStrID", ref nKMOperatorSkillTemplet.m_OperSkillStrID) ? 1u : 0u) & (lua.GetData("m_OperSkillNameStrID", ref nKMOperatorSkillTemplet.m_OperSkillNameStrID) ? 1u : 0u) & (lua.GetData("m_OperSkillDescStrID", ref nKMOperatorSkillTemplet.m_OperSkillDescStrID) ? 1u : 0u) & (lua.GetData("m_OperSkillIcon", ref nKMOperatorSkillTemplet.m_OperSkillIcon) ? 1u : 0u) & (lua.GetData("m_OperSkillType", ref nKMOperatorSkillTemplet.m_OperSkillType) ? 1u : 0u) & (lua.GetData("m_OperSkillTarget", ref nKMOperatorSkillTemplet.m_OperSkillTarget) ? 1u : 0u)) & (lua.GetData("m_MaxSkillLevel", ref nKMOperatorSkillTemplet.m_MaxSkillLevel) ? 1 : 0);
		lua.GetData("UseFilter", ref nKMOperatorSkillTemplet.UseFilter);
		if (num == 0)
		{
			return null;
		}
		return nKMOperatorSkillTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
