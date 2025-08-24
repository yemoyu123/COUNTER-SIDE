using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM;

public class NKMUnitSkillTempletContainer
{
	private SortedDictionary<int, NKMUnitSkillTemplet> m_dicSkillTemplet = new SortedDictionary<int, NKMUnitSkillTemplet>();

	public IDictionary<int, NKMUnitSkillTemplet> dicTemplets => m_dicSkillTemplet;

	public int SkillID { get; private set; }

	public string SkillStrID { get; private set; }

	public NKM_SKILL_TYPE SkillType { get; private set; }

	public NKMUnitSkillTempletContainer(int skillID, string strID)
	{
		SkillID = skillID;
		SkillStrID = strID;
	}

	public NKMUnitSkillTemplet GetSkillTemplet(int level)
	{
		if (m_dicSkillTemplet.TryGetValue(level, out var value))
		{
			return value;
		}
		return null;
	}

	public void AddSkillTemplet(NKMUnitSkillTemplet skillTemplet)
	{
		if (!m_dicSkillTemplet.Any())
		{
			SkillType = skillTemplet.m_NKM_SKILL_TYPE;
		}
		if (m_dicSkillTemplet.TryGetValue(skillTemplet.m_Level, out var value))
		{
			NKMTempletError.Add($"[UnitSkill] 동일 레벨의 스킬 템플릿이 이미 존재. level:{skillTemplet.m_Level} skillId:{skillTemplet.m_strID}/{value.m_strID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitSkillManager.cs", 75);
		}
		else
		{
			m_dicSkillTemplet.Add(skillTemplet.m_Level, skillTemplet);
		}
	}
}
