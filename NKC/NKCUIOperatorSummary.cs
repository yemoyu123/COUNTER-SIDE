using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIOperatorSummary : MonoBehaviour
{
	public Text m_NKM_UI_UNIT_INFO_DESC_SUMMARY_TITLE;

	public Text m_NKM_UI_UNIT_INFO_DESC_SUMMARY_NAME;

	public Image m_Rarity_icon;

	public Image m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL_GAUGE;

	public Text m_NKM_UI_UNIT_INFO_DESC_SUMMARY_EXP;

	public Text m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL_TEXT;

	public Text m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL_MAX_TEXT;

	public Image m_OPERATOR;

	public void SetData(NKMOperator opeatorData)
	{
		if (opeatorData == null || !NKCOperatorUtil.IsOperatorUnit(opeatorData.id))
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(opeatorData.id);
		if (unitTempletBase == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_NKM_UI_UNIT_INFO_DESC_SUMMARY_TITLE, unitTempletBase.GetUnitTitle());
		NKCUtil.SetLabelText(m_NKM_UI_UNIT_INFO_DESC_SUMMARY_NAME, unitTempletBase.GetUnitName() ?? "");
		NKCUtil.SetImageSprite(m_Rarity_icon, NKCUtil.GetSpriteUnitGrade(unitTempletBase.m_NKM_UNIT_GRADE));
		NKCUtil.SetLabelText(m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL_TEXT, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, opeatorData.level));
		NKCUtil.SetLabelText(m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL_MAX_TEXT, "/" + NKMCommonConst.OperatorConstTemplet.unitMaximumLevel);
		ShowLevelExpGauge(bShow: true);
		int num = opeatorData.exp;
		if (m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL_GAUGE != null)
		{
			if (NKMCommonConst.OperatorConstTemplet.unitMaximumLevel == opeatorData.level)
			{
				num = 0;
				m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL_GAUGE.fillAmount = 1f;
			}
			else
			{
				m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL_GAUGE.fillAmount = NKCExpManager.GetOperatorNextLevelExpProgress(NKCOperatorUtil.GetOperatorData(opeatorData.uid));
			}
		}
		NKCUtil.SetLabelText(m_NKM_UI_UNIT_INFO_DESC_SUMMARY_EXP, $"{num}/{NKCOperatorUtil.GetRequiredExp(opeatorData)}");
	}

	public void ShowLevelExpGauge(bool bShow)
	{
		if (m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL_GAUGE != null)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_INFO_DESC_SUMMARY_LEVEL_GAUGE.transform.parent, bShow);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_INFO_DESC_SUMMARY_EXP, bShow);
	}
}
