using System.Collections.Generic;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupSelectionConfirmShip : MonoBehaviour
{
	public NKCUIShipSelectListSlot m_slot;

	public Text m_lbHP;

	public Text m_lbATK;

	public Text m_lbDEF;

	public List<NKCUIComStateButton> m_lstSkillBtn = new List<NKCUIComStateButton>(3);

	public void SetData(int shipID, int shipLv = 1, int limitBreakLv = 0)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipID);
		if (unitTempletBase == null)
		{
			return;
		}
		m_slot.SetData(unitTempletBase, shipLv, bEnableLayoutElement: false, null);
		SetShipStat(shipID, shipLv, limitBreakLv);
		NKMUnitTempletBase maxGradeShipTemplet = NKMShipManager.GetMaxGradeShipTemplet(unitTempletBase);
		int skillCount = maxGradeShipTemplet.GetSkillCount();
		for (int i = 0; i < m_lstSkillBtn.Count; i++)
		{
			if (i < skillCount)
			{
				NKMShipSkillTemplet shipSkillTemplet = NKMShipSkillManager.GetShipSkillTempletByIndex(maxGradeShipTemplet, i);
				if (shipSkillTemplet != null)
				{
					NKCUtil.SetGameobjectActive(m_lstSkillBtn[i], bValue: true);
					NKCUtil.SetImageSprite(m_lstSkillBtn[i].transform.GetComponent<Image>(), NKCUtil.GetSkillIconSprite(shipSkillTemplet));
					m_lstSkillBtn[i].PointerDown.RemoveAllListeners();
					m_lstSkillBtn[i].PointerDown.AddListener(delegate(PointerEventData e)
					{
						OnPointDownSkill(shipSkillTemplet, e);
					});
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstSkillBtn[i], bValue: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstSkillBtn[i], bValue: false);
			}
		}
	}

	private void SetShipStat(int unitID, int shipLv, int limitBreakLv)
	{
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitID);
		NKMUnitData item = NKMUnitManager.CreateShipData(unitID, unitID, shipLv, limitBreakLv).unitData;
		NKMStatData nKMStatData = new NKMStatData();
		nKMStatData.Init();
		nKMStatData.MakeBaseStat(null, bPvP: false, item, unitStatTemplet.m_StatData);
		NKCUtil.SetLabelText(m_lbHP, ((int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_HP)).ToString());
		NKCUtil.SetLabelText(m_lbATK, ((int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_ATK)).ToString());
		NKCUtil.SetLabelText(m_lbDEF, ((int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_DEF)).ToString());
	}

	private void OnPointDownSkill(NKMShipSkillTemplet shipSkillTemplet, PointerEventData eventData)
	{
		if (shipSkillTemplet != null)
		{
			NKCUITooltip.Instance.Open(shipSkillTemplet, eventData.position);
		}
	}
}
