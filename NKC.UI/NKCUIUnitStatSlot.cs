using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitStatSlot : MonoBehaviour
{
	public Text m_lbStatName;

	public Text m_lbStatBonus;

	public Text m_lbStatNumber;

	public Image m_imgEnhanceGauge;

	public GameObject m_goMaxBG;

	public GameObject m_lbMax;

	public NKCComStatInfoToolTip m_StatInfoToolTip;

	public Text m_lbStatPlusPercentage;

	[Header("TextMeshPro")]
	public TMP_Text m_lbStatNameTMPro;

	public TMP_Text m_lbStatNumberTMPro;

	public TMP_Text m_lbStatPlusPercentageTMPro;

	public void SetStat(NKM_STAT_TYPE eStatType, NKMStatData baseStatData, NKMUnitData unitData)
	{
		SetStat(eStatType, baseStatData.GetStatBase(eStatType), baseStatData.GetBaseBonusStat(eStatType), unitData);
	}

	private void SetStat(NKM_STAT_TYPE eStatType, float number, float modifier, NKMUnitData unitData)
	{
		bool num = NKMUnitStatManager.IsPercentStat(eStatType);
		if (null != m_imgEnhanceGauge)
		{
			float enhancePercent = GetEnhancePercent(eStatType, unitData);
			NKCUtil.SetGameobjectActive(m_imgEnhanceGauge, enhancePercent < 1f);
			m_imgEnhanceGauge.fillAmount = enhancePercent;
			NKCUtil.SetGameobjectActive(m_goMaxBG, enhancePercent >= 1f);
			NKCUtil.SetGameobjectActive(m_lbMax, enhancePercent >= 1f);
		}
		string statShortName = NKCUtilString.GetStatShortName(eStatType, number + modifier);
		NKCUtil.SetLabelText(m_lbStatName, statShortName);
		NKCUtil.SetLabelText(m_lbStatNameTMPro, statShortName);
		NKCUtil.SetLabelTextColor(m_lbStatBonus, NKCUtil.GetBonusColor(modifier));
		if (m_StatInfoToolTip != null)
		{
			m_StatInfoToolTip.SetType(eStatType, number + modifier < 0f);
		}
		if (m_lbStatPlusPercentage != null)
		{
			float statPercentage = NKCUtil.GetStatPercentage(eStatType, number + modifier);
			NKCUtil.SetLabelText(m_lbStatPlusPercentage, "(" + statPercentage.ToString("N2") + "%)");
			NKCUtil.SetGameobjectActive(m_lbStatPlusPercentage.gameObject, statPercentage != 0f);
		}
		if (m_lbStatPlusPercentageTMPro != null)
		{
			float statPercentage2 = NKCUtil.GetStatPercentage(eStatType, number + modifier);
			NKCUtil.SetLabelText(m_lbStatPlusPercentageTMPro, "(" + statPercentage2.ToString("N2") + "%)");
			NKCUtil.SetGameobjectActive(m_lbStatPlusPercentageTMPro, statPercentage2 != 0f);
		}
		if (num)
		{
			NKCUtil.SetLabelText(m_lbStatBonus, $"{(int)modifier: (+#%); (-#%);''}");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbStatBonus, $"{(int)modifier: (+#); (-#);''}");
		}
		string text = "";
		text = ((!num) ? $"{(int)number + (int)modifier:#,0;-#,0;0}" : $"{(int)number + (int)modifier:+#%;-#%;0}");
		NKCUtil.SetLabelText(m_lbStatNumber, text);
		NKCUtil.SetLabelText(m_lbStatNumberTMPro, text);
	}

	private float GetEnhancePercent(NKM_STAT_TYPE eNKM_STAT_TYPE, NKMUnitData unitData, float fClampMin = 0f)
	{
		if (unitData == null)
		{
			return 0f;
		}
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		if (unitStatTemplet == null)
		{
			return 0f;
		}
		if (unitData.m_listStatEXP.Count <= (int)eNKM_STAT_TYPE)
		{
			return 0f;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		int num = (int)NKMUnitStatManager.CalculateStat(eNKM_STAT_TYPE, unitStatTemplet.m_StatData, unitData.m_UnitLevel, unitData.m_LimitBreakLevel, unitData.GetMultiplierByPermanentContract(), null, null, 0, unitTempletBase.m_NKM_UNIT_TYPE);
		int num2 = (int)NKMUnitStatManager.GetMaxStat(eNKM_STAT_TYPE, unitStatTemplet.m_StatData, unitData.m_UnitLevel, unitData.m_LimitBreakLevel, unitData.GetMultiplierByPermanentContract(), null, null, unitTempletBase.m_NKM_UNIT_TYPE);
		if (num2 <= num)
		{
			return 1f;
		}
		return Mathf.Clamp((float)num / (float)num2, fClampMin, 1f);
	}

	public void SetStatString(string stats, string plusPercentage = "")
	{
		NKCUtil.SetLabelText(m_lbStatNumberTMPro, stats);
		NKCUtil.SetLabelText(m_lbStatPlusPercentageTMPro, plusPercentage);
	}
}
