using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC.UI.Tooltip;

public class NKCUITooltipSkillLevel : NKCUITooltipBase
{
	public List<NKCUIComSkillLevelDetail> m_skillLevelDetail;

	public override void Init()
	{
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.SkillLevelData { SkillTemplet: var skillTemplet }))
		{
			Debug.LogError("Tooltip SkillLevelData is null");
			return;
		}
		NKMUnitSkillTempletContainer skillTempletContainer = NKMUnitSkillManager.GetSkillTempletContainer(skillTemplet.m_ID);
		for (int i = 0; i < m_skillLevelDetail.Count; i++)
		{
			NKCUIComSkillLevelDetail nKCUIComSkillLevelDetail = m_skillLevelDetail[i];
			if (skillTempletContainer.GetSkillTemplet(nKCUIComSkillLevelDetail.m_iLevel) != null)
			{
				NKCUtil.SetGameobjectActive(nKCUIComSkillLevelDetail, bValue: true);
				nKCUIComSkillLevelDetail.SetData(skillTemplet.m_ID, nKCUIComSkillLevelDetail.m_iLevel <= skillTemplet.m_Level);
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUIComSkillLevelDetail, bValue: false);
			}
		}
	}
}
