using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComStatEnhanceBar : MonoBehaviour
{
	public Text m_lbStatCurrent;

	public Text m_lbStatPlus;

	public Slider m_slStatEXPNew;

	public Slider m_slStatEXP;

	public GameObject m_objMax;

	public NKCComStatInfoToolTip m_StatInfoToolTip;

	public void SetData(NKMUnitData unitData, NKM_STAT_TYPE stat, int expGain)
	{
		if (m_StatInfoToolTip != null)
		{
			m_StatInfoToolTip.SetType(stat);
		}
		if (unitData == null)
		{
			SetUIData(0, 0, 1);
			return;
		}
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		if (unitStatTemplet == null)
		{
			Debug.LogError("Unit statTemplt Not Found! unitID : " + unitData.m_UnitID);
			SetUIData(0, 0, 1);
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		int currentStat = (int)NKMUnitStatManager.CalculateStat(stat, unitStatTemplet.m_StatData, unitData.m_UnitLevel, unitData.m_LimitBreakLevel, unitData.GetMultiplierByPermanentContract(), null, null, 0, unitTempletBase.m_NKM_UNIT_TYPE);
		int newStat = (int)NKMUnitStatManager.CalculateStat(stat, unitStatTemplet.m_StatData, unitData.m_UnitLevel, unitData.m_LimitBreakLevel, unitData.GetMultiplierByPermanentContract(), null, null, 0, unitTempletBase.m_NKM_UNIT_TYPE);
		int currentMaxStat = (int)NKMUnitStatManager.GetMaxStat(stat, unitStatTemplet.m_StatData, unitData.m_UnitLevel, unitData.m_LimitBreakLevel, unitData.GetMultiplierByPermanentContract(), null, null, unitTempletBase.m_NKM_UNIT_TYPE);
		SetUIData(currentStat, newStat, currentMaxStat);
	}

	private void SetUIData(int currentStat, int newStat, int currentMaxStat)
	{
		if (currentMaxStat == 0)
		{
			currentMaxStat = 1;
		}
		if (newStat > currentMaxStat)
		{
			newStat = currentMaxStat;
		}
		NKCUtil.SetLabelText(m_lbStatCurrent, currentStat.ToString());
		int num = newStat - currentStat;
		if (currentMaxStat == currentStat)
		{
			if (m_slStatEXPNew != null)
			{
				m_slStatEXPNew.value = 1f;
			}
			NKCUtil.SetGameobjectActive(m_objMax, bValue: true);
			NKCUtil.SetGameobjectActive(m_slStatEXP, bValue: false);
			NKCUtil.SetGameobjectActive(m_slStatEXPNew, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbStatPlus, bValue: false);
		}
		else if (num > 0)
		{
			if (m_slStatEXP != null)
			{
				m_slStatEXP.value = (float)currentStat / (float)currentMaxStat;
			}
			if (m_slStatEXPNew != null)
			{
				m_slStatEXPNew.value = (float)newStat / (float)currentMaxStat;
			}
			NKCUtil.SetGameobjectActive(m_objMax, bValue: false);
			NKCUtil.SetGameobjectActive(m_slStatEXP, bValue: true);
			NKCUtil.SetGameobjectActive(m_slStatEXPNew, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbStatPlus, bValue: true);
			NKCUtil.SetLabelText(m_lbStatPlus, (newStat - currentStat).ToString("+0"));
		}
		else
		{
			if (m_slStatEXP != null)
			{
				m_slStatEXP.value = (float)currentStat / (float)currentMaxStat;
			}
			NKCUtil.SetGameobjectActive(m_objMax, bValue: false);
			NKCUtil.SetGameobjectActive(m_slStatEXP, bValue: true);
			NKCUtil.SetGameobjectActive(m_slStatEXPNew, bValue: false);
			NKCUtil.SetGameobjectActive(m_lbStatPlus, bValue: false);
		}
	}
}
