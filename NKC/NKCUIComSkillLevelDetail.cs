using NKC.UI.Component;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIComSkillLevelDetail : MonoBehaviour
{
	public int m_iLevel;

	public Image m_imgLevel;

	public Text m_lbLevel;

	public NKCComTMPUIText m_lbSkillLevelDesc;

	public Color m_colTextTrained = Color.white;

	public Color m_colTextUnTrained = new Color(0.4f, 0.4f, 0.4f, 1f);

	public LayoutElement m_LayoutElement;

	public void SetData(int skillID, bool bTrained, int iLevel = -1)
	{
		if (-1 != iLevel)
		{
			m_iLevel = iLevel;
		}
		NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(skillID, m_iLevel);
		if (skillTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, skillTemplet.m_Level));
			NKCUtil.SetLabelText(m_lbSkillLevelDesc, skillTemplet.GetSkillDesc());
			NKCUtil.SetLabelTextColor(m_lbSkillLevelDesc, bTrained ? m_colTextTrained : m_colTextUnTrained);
			NKCUtil.SetImageColor(m_imgLevel, bTrained ? m_colTextTrained : m_colTextUnTrained);
			if (m_LayoutElement == null)
			{
				m_LayoutElement = GetComponent<LayoutElement>();
			}
			if (m_LayoutElement != null && m_lbSkillLevelDesc != null)
			{
				float num = CalculatePreferredHeight();
				m_LayoutElement.minHeight = num;
				m_LayoutElement.preferredHeight = num;
			}
		}
	}

	private float CalculatePreferredHeight()
	{
		float height = m_imgLevel.GetComponent<RectTransform>().GetHeight();
		return Mathf.Max(m_lbSkillLevelDesc.preferredHeight, height);
	}
}
