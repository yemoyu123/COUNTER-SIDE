using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

[RequireComponent(typeof(NKCUIUnitSelectListSlot))]
public class NKCUIScoutSelectListSlot : MonoBehaviour
{
	private NKCUIUnitSelectListSlot m_UnitSlot;

	public GameObject m_objReddot;

	public GameObject m_objUnitNotOwned;

	public GameObject m_objSelected;

	public Text m_lbPieceCount;

	public Slider m_slPieceCountBar;

	public GameObject m_objCanExchangeBarBG;

	public GameObject m_objNoExchangeBarBG;

	public void Init()
	{
		m_UnitSlot = GetComponent<NKCUIUnitSelectListSlot>();
		m_UnitSlot.Init();
	}

	public void SetData(NKMPieceTemplet templet, NKMUnitData fakeUnitData, bool bSelected, NKCUIUnitSelectListSlotBase.OnSelectThisSlot onSelectThisSlot)
	{
		if (templet == null)
		{
			m_UnitSlot.SetEmpty(bEnableLayoutElement: true, onSelectThisSlot);
		}
		else
		{
			if (fakeUnitData == null)
			{
				NKMUnitTempletBase templetBase = NKMUnitTempletBase.Find(templet.m_PieceGetUintId);
				m_UnitSlot?.SetData(templetBase, 1, bEnableLayoutElement: true, onSelectThisSlot);
			}
			else
			{
				m_UnitSlot?.SetDataForCollection(fakeUnitData, NKMDeckIndex.None, onSelectThisSlot, bEnable: true);
			}
			NKCUtil.SetGameobjectActive(m_UnitSlot.m_lbName, bValue: true);
			NKCUtil.SetGameobjectActive(m_UnitSlot.m_imgUnitRole, bValue: true);
			NKCUtil.SetGameobjectActive(m_UnitSlot.m_comStarRank, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitSlot.m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_STAR_GRADE, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitSlot.m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_COST, bValue: true);
			NKCUtil.SetGameobjectActive(m_UnitSlot.m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_BOTTOM, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objSelected, bSelected);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool flag = nKMUserData.m_ArmyData.IsCollectedUnit(templet.m_PieceGetUintId);
		long num = (flag ? templet.m_PieceReq : templet.m_PieceReqFirst);
		long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(templet.m_PieceId);
		if (countMiscItem < num)
		{
			NKCUtil.SetLabelText(m_lbPieceCount, $"<color=#ff0000>{countMiscItem}</color>/{num}");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbPieceCount, $"{countMiscItem}/{num}");
		}
		NKCUtil.SetGameobjectActive(m_objReddot, NKCUIScout.IsReddotNeeded(nKMUserData, templet.Key));
		bool flag2 = countMiscItem >= num;
		NKCUtil.SetGameobjectActive(m_objCanExchangeBarBG, flag2);
		NKCUtil.SetGameobjectActive(m_objNoExchangeBarBG, !flag2);
		NKCUtil.SetGameobjectActive(m_objUnitNotOwned, !flag);
		if (m_slPieceCountBar != null)
		{
			m_slPieceCountBar.minValue = 0f;
			m_slPieceCountBar.maxValue = num;
			if (flag2)
			{
				m_slPieceCountBar.normalizedValue = 1f;
			}
			else
			{
				m_slPieceCountBar.value = countMiscItem;
			}
		}
	}
}
