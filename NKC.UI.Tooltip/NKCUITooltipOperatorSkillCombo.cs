using System.Collections.Generic;

namespace NKC.UI.Tooltip;

public class NKCUITooltipOperatorSkillCombo : NKCUITooltipBase
{
	public List<NKCGameHudComboSlot> m_lstComboSlot;

	public override void Init()
	{
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.OperatorSkillComboData operatorSkillComboData))
		{
			return;
		}
		for (int i = 0; i < m_lstComboSlot.Count; i++)
		{
			if (operatorSkillComboData.skillCombo.Count <= i)
			{
				NKCUtil.SetGameobjectActive(m_lstComboSlot[i].gameObject, bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_lstComboSlot[i].gameObject, bValue: true);
			m_lstComboSlot[i].SetUI(operatorSkillComboData.skillCombo[i], i < operatorSkillComboData.skillCombo.Count - 1);
		}
	}
}
