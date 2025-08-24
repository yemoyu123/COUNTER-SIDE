using System;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIUnitInfoSkillTrain : MonoBehaviour
{
	[Serializable]
	public class UnitSkillSlot
	{
		public NKCUISkillSlot m_slot;

		public GameObject m_objEffect;

		public int CurrentSkillID
		{
			get
			{
				if (!(m_slot != null))
				{
					return 0;
				}
				return m_slot.CurrentSkillID;
			}
		}

		public void Init(NKCUISkillSlot.OnClickSkillSlot onClick)
		{
			if (m_slot != null)
			{
				m_slot.Init(onClick);
			}
			NKCUtil.SetGameobjectActive(m_objEffect, bValue: false);
		}

		public void Cleanup()
		{
			if (m_slot != null)
			{
				m_slot.Cleanup();
			}
			NKCUtil.SetGameobjectActive(m_objEffect, bValue: false);
		}

		public void ShowEffect(bool value)
		{
			if (value)
			{
				NKCUtil.SetGameobjectActive(m_objEffect, bValue: false);
				NKCUtil.SetGameobjectActive(m_objEffect, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objEffect, bValue: false);
			}
		}

		public void SetData(NKMUnitSkillTemplet unitSkillTemplet, bool bIsHyper)
		{
			if (m_slot != null)
			{
				m_slot.SetData(unitSkillTemplet, bIsHyper);
			}
		}

		public void LockSkill(bool value)
		{
			if (m_slot != null)
			{
				m_slot.LockSkill(value);
			}
		}

		public void SetHighlight(bool value)
		{
			if (m_slot != null)
			{
				m_slot.SetHighlight(value);
			}
		}
	}

	[Header("스킬")]
	public GameObject m_objLeaderSkill;

	public List<UnitSkillSlot> m_lstSkillSlot;

	public GameObject m_objRootSkillDetail;

	public Text m_lbSkillName;

	public Image m_imgSkillLock;

	public Text m_lbSkillType;

	public GameObject m_objSkillCooldown;

	public Text m_lbSkillCooldown;

	public GameObject m_objSkillAttackCount;

	public Text m_lbSkillAttackCount;

	[Space]
	public GameObject m_objHyperSkillGlowEffect;

	public GameObject m_objSkillLockRoot;

	public List<GameObject> m_lstObjSkillLockStar;

	public ScrollRect m_SkillDescScrollRect;

	public NKCComTMPUIText m_lbSkillDescription;

	[Header("스킬 레벨 정보")]
	public List<NKCUIComSkillLevelDetail> m_lstSkillLevelDetail;

	[Header("훈련 필요 아이템")]
	public List<NKCUIItemCostSlot> m_lstCostItemUI;

	public int m_QuantityCheckNum = 1;

	public GameObject m_objLimitBreak;

	public NKCUIItemCostSlot m_itemLimitBreak;

	[Header("훈련 버튼")]
	public NKCUIComStateButton m_csbtnToSkillMenu;

	public GameObject m_objButtonEffect;

	public Text m_NKM_UI_LAB_TRAINING_ENTER_TEXT;

	public Image m_ImgNKM_UI_LAB_TRAINING_ENTER_ICON;

	private NKMUnitSkillTemplet m_SkillTemplet;

	private NKMUnitData m_UnitData;

	public int SelectedSkillID
	{
		get
		{
			if (m_SkillTemplet == null)
			{
				return -1;
			}
			return m_SkillTemplet.m_ID;
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < m_lstSkillSlot.Count; i++)
		{
			m_lstSkillSlot[i].ShowEffect(value: false);
		}
	}

	public void Init()
	{
		for (int i = 0; i < m_lstSkillSlot.Count; i++)
		{
			m_lstSkillSlot[i].Init(OnSelectSlot);
		}
		NKCUtil.SetBindFunction(m_csbtnToSkillMenu, OnClickSkillTrain);
	}

	public void Clear()
	{
		foreach (UnitSkillSlot item in m_lstSkillSlot)
		{
			item.Cleanup();
		}
	}

	private void OnSelectSlot(NKMUnitSkillTemplet skillTemplet)
	{
		if ((!NKCUIUnitInfo.IsInstanceOpen || !NKCUIUnitInfo.Instance.IsBlockedUnit()) && skillTemplet != null)
		{
			SelectSkill(skillTemplet.m_ID);
		}
	}

	public void OnUnitUpdate(long uid, NKMUnitData unitData)
	{
		if (m_UnitData != null && m_UnitData.m_UnitUID == uid && unitData != null)
		{
			SetData(unitData, m_SkillTemplet.m_ID, bSkillUpAni: true);
		}
	}

	public void OnInventoryChange(NKMItemMiscData itemData)
	{
		foreach (NKCUIItemCostSlot item in m_lstCostItemUI)
		{
			if (item.ItemID == itemData.ItemID)
			{
				UpdateRequiredItemData(m_SkillTemplet.m_ID, m_SkillTemplet.m_Level + 1);
				UpdateSkillTrainButton(m_SkillTemplet.m_ID);
				break;
			}
		}
	}

	public void SetData(NKMUnitData unitData, int selectedSkillID = -1, bool bSkillUpAni = false)
	{
		m_UnitData = unitData;
		m_SkillTemplet = null;
		if (m_UnitData.GetUnitSkillCount() <= 0)
		{
			Debug.Log("Unit have no skill. unitID : " + unitData.m_UnitID);
			return;
		}
		List<NKMUnitSkillTemplet> unitAllSkillTempletList = NKMUnitSkillManager.GetUnitAllSkillTempletList(unitData);
		if (selectedSkillID != -1 && !unitAllSkillTempletList.Exists((NKMUnitSkillTemplet x) => x.m_ID == selectedSkillID))
		{
			selectedSkillID = -1;
		}
		bool flag = false;
		int num = 1;
		for (int num2 = 0; num2 < m_lstSkillSlot.Count; num2++)
		{
			if (unitAllSkillTempletList.Count <= num2)
			{
				continue;
			}
			NKCUtil.SetGameobjectActive(m_lstSkillSlot[0].m_slot, flag);
			NKMUnitSkillTemplet nKMUnitSkillTemplet = unitAllSkillTempletList[num2];
			if (nKMUnitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_LEADER)
			{
				flag = true;
				m_lstSkillSlot[0].SetData(nKMUnitSkillTemplet, bIsHyper: false);
				if (selectedSkillID == -1)
				{
					selectedSkillID = nKMUnitSkillTemplet.m_ID;
				}
			}
			else
			{
				m_lstSkillSlot[num].SetData(nKMUnitSkillTemplet, nKMUnitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER);
				NKCUtil.SetGameobjectActive(m_lstSkillSlot[num].m_slot, bValue: true);
				num++;
			}
		}
		for (int num3 = num; num3 < m_lstSkillSlot.Count; num3++)
		{
			NKCUtil.SetGameobjectActive(m_lstSkillSlot[num3].m_slot, bValue: false);
		}
		if (flag)
		{
			OnSkillLevelUp();
		}
		NKCUtil.SetGameobjectActive(m_objLeaderSkill, flag);
		NKCUtil.SetGameobjectActive(m_lstSkillSlot[0].m_slot, flag);
		if (selectedSkillID == -1 && unitAllSkillTempletList.Count > 0)
		{
			selectedSkillID = unitAllSkillTempletList[0].m_ID;
		}
		SelectSkill(selectedSkillID, bSkillUpAni);
	}

	private void SelectSkill(int skillID, bool bAnimate = true)
	{
		NKMUnitSkillTemplet skillDetail = (m_SkillTemplet = NKMUnitSkillManager.GetUnitSkillTemplet(skillID, m_UnitData));
		SetHighlightSlot(skillID);
		SetSkillDetail(skillDetail);
		UpdateSkillTrainButton(skillID, bAnimate);
	}

	private void SetHighlightSlot(int HighlightedSlotSkillID)
	{
		if (HighlightedSlotSkillID == 0)
		{
			for (int i = 0; i < m_lstSkillSlot.Count; i++)
			{
				m_lstSkillSlot[i].SetHighlight(value: false);
			}
		}
		else
		{
			for (int j = 0; j < m_lstSkillSlot.Count; j++)
			{
				m_lstSkillSlot[j].SetHighlight(m_lstSkillSlot[j].CurrentSkillID == HighlightedSlotSkillID);
			}
		}
	}

	public void OnClickSkillTrain()
	{
		if (NKCUIUnitInfo.IsInstanceOpen && NKCUIUnitInfo.Instance.IsBlockedUnit())
		{
			return;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = CanTrainSkill(m_UnitData, m_SkillTemplet.m_ID);
		switch (nKM_ERROR_CODE)
		{
		case NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_NEED_LIMIT_BREAK:
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
			break;
		case NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_NOT_ENOUGH_ITEM:
		{
			int unitSkillLevel = m_UnitData.GetUnitSkillLevel(m_SkillTemplet.m_ID);
			NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(m_SkillTemplet.m_ID, unitSkillLevel + 1);
			NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
			if (skillTemplet == null || inventoryData == null)
			{
				break;
			}
			for (int i = 0; i < skillTemplet.m_lstUpgradeReqItem.Count; i++)
			{
				if (inventoryData.GetCountMiscItem(skillTemplet.m_lstUpgradeReqItem[i].ItemID) < skillTemplet.m_lstUpgradeReqItem[i].ItemCount)
				{
					NKCShopManager.OpenItemLackPopup(skillTemplet.m_lstUpgradeReqItem[i].ItemID, skillTemplet.m_lstUpgradeReqItem[i].ItemCount);
					break;
				}
			}
			break;
		}
		case NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING:
		case NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING:
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
			break;
		case NKM_ERROR_CODE.NEC_OK:
			NKCPacketSender.Send_Packet_NKMPacket_UNIT_SKILL_UPGRADE_REQ(m_UnitData.m_UnitUID, m_SkillTemplet.m_ID);
			break;
		}
	}

	private NKM_ERROR_CODE CanTrainSkill(NKMUnitData targetUnit, int skillID)
	{
		if (m_UnitData == null || targetUnit == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (targetUnit.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		return nKMUserData.m_ArmyData.GetUnitDeckState(targetUnit) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKMUnitSkillManager.CanTrainSkill(nKMUserData, targetUnit, skillID), 
		};
	}

	private void UpdateSkillTrainButton(int skillID, bool bAnimate = true)
	{
		switch (CanTrainSkill(m_UnitData, skillID))
		{
		default:
			m_csbtnToSkillMenu.Lock();
			NKCUtil.SetGameobjectActive(m_objButtonEffect, bValue: false);
			m_NKM_UI_LAB_TRAINING_ENTER_TEXT.color = NKCUtil.GetButtonUIColor(Active: false);
			m_ImgNKM_UI_LAB_TRAINING_ENTER_ICON.color = NKCUtil.GetButtonUIColor(Active: false);
			break;
		case NKM_ERROR_CODE.NEC_OK:
			m_csbtnToSkillMenu.UnLock();
			NKCUtil.SetGameobjectActive(m_objButtonEffect, bValue: true);
			m_NKM_UI_LAB_TRAINING_ENTER_TEXT.color = NKCUtil.GetButtonUIColor();
			m_ImgNKM_UI_LAB_TRAINING_ENTER_ICON.color = NKCUtil.GetButtonUIColor();
			break;
		case NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_NOT_EXIST:
			Debug.LogError("No SkillLevelData");
			m_csbtnToSkillMenu.Lock();
			NKCUtil.SetGameobjectActive(m_objButtonEffect, bValue: false);
			m_NKM_UI_LAB_TRAINING_ENTER_TEXT.color = NKCUtil.GetButtonUIColor(Active: false);
			m_ImgNKM_UI_LAB_TRAINING_ENTER_ICON.color = NKCUtil.GetButtonUIColor(Active: false);
			break;
		}
	}

	private void SetSkillDetail(NKMUnitSkillTemplet skillTemplet)
	{
		if (skillTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_objRootSkillDetail, bValue: false);
			UpdateRequiredItemData(0, 0);
			{
				foreach (NKCUIComSkillLevelDetail item in m_lstSkillLevelDetail)
				{
					NKCUtil.SetGameobjectActive(item, bValue: false);
				}
				return;
			}
		}
		NKCUtil.SetGameobjectActive(m_objRootSkillDetail, bValue: true);
		if (m_lbSkillName != null)
		{
			m_lbSkillName.text = skillTemplet.GetSkillName();
			bool flag = NKMUnitSkillManager.IsLockedSkill(skillTemplet.m_ID, m_UnitData.m_LimitBreakLevel);
			if (flag)
			{
				m_lbSkillName.color = new Color(0.39607844f, 0.39607844f, 0.39607844f);
			}
			else
			{
				m_lbSkillName.color = new Color(1f, 1f, 1f);
			}
			NKCUtil.SetGameobjectActive(m_imgSkillLock, flag);
		}
		m_lbSkillType.color = NKCUtil.GetSkillTypeColor(skillTemplet.m_NKM_SKILL_TYPE);
		m_lbSkillType.text = NKCUtilString.GetSkillTypeName(skillTemplet.m_NKM_SKILL_TYPE);
		SetUnlockReqUpgradeCount(skillTemplet);
		if (skillTemplet.m_fCooltimeSecond > 0f)
		{
			NKCUtil.SetGameobjectActive(m_objSkillCooldown, bValue: true);
			NKMUnitTempletBase unitTempletBase = m_UnitData.GetUnitTempletBase();
			if (unitTempletBase != null && unitTempletBase.StopDefaultCoolTime)
			{
				NKCUtil.SetLabelText(m_lbSkillCooldown, string.Format(NKCUtilString.GET_STRING_COUNT_ONE_PARAM, skillTemplet.m_fCooltimeSecond));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbSkillCooldown, string.Format(NKCUtilString.GET_STRING_COOLTIME_ONE_PARAM, skillTemplet.m_fCooltimeSecond));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSkillCooldown, bValue: false);
		}
		int num = skillTemplet?.m_AttackCount ?? 0;
		if (num > 0)
		{
			NKCUtil.SetGameobjectActive(m_objSkillAttackCount, bValue: true);
			NKCUtil.SetLabelText(m_lbSkillAttackCount, string.Format(NKCUtilString.GET_STRING_SKILL_ATTACK_COUNT_ONE_PARAM, num));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSkillAttackCount, bValue: false);
		}
		if (skillTemplet.m_Level == 1)
		{
			NKCUtil.SetLabelText(m_lbSkillDescription, skillTemplet.GetSkillDesc());
		}
		else
		{
			NKMUnitSkillTemplet skillTemplet2 = NKMUnitSkillManager.GetSkillTemplet(skillTemplet.m_ID, 1);
			if (skillTemplet2 != null)
			{
				NKCUtil.SetLabelText(m_lbSkillDescription, skillTemplet2.GetSkillDesc());
			}
		}
		m_SkillDescScrollRect.verticalNormalizedPosition = 1f;
		UpdateRequiredItemData(skillTemplet.m_ID, skillTemplet.m_Level + 1);
		int maxSkillLevel = NKMUnitSkillManager.GetMaxSkillLevel(skillTemplet.m_ID);
		foreach (NKCUIComSkillLevelDetail item2 in m_lstSkillLevelDetail)
		{
			if (item2.m_iLevel <= maxSkillLevel)
			{
				NKCUtil.SetGameobjectActive(item2, bValue: true);
				item2.SetData(skillTemplet.m_ID, item2.m_iLevel <= skillTemplet.m_Level);
			}
			else
			{
				NKCUtil.SetGameobjectActive(item2, bValue: false);
			}
		}
	}

	private void SetUnlockReqUpgradeCount(NKMUnitSkillTemplet skillTemplet)
	{
		if (skillTemplet == null || m_UnitData == null)
		{
			NKCUtil.SetGameobjectActive(m_objSkillLockRoot, bValue: false);
			return;
		}
		if (!NKMUnitSkillManager.IsLockedSkill(skillTemplet.m_ID, m_UnitData.m_LimitBreakLevel))
		{
			NKCUtil.SetGameobjectActive(m_objSkillLockRoot, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objSkillLockRoot, bValue: true);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitData.m_UnitID);
		NKCUtil.SetSkillUnlockStarRank(m_lstObjSkillLockStar, skillTemplet, unitTempletBase.m_StarGradeMax);
	}

	private void UpdateRequiredItemData(int skillID, int targetLevel)
	{
		NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(skillID, targetLevel);
		for (int i = 0; i < m_lstCostItemUI.Count; i++)
		{
			NKCUIItemCostSlot nKCUIItemCostSlot = m_lstCostItemUI[i];
			if (skillTemplet != null)
			{
				if (i < skillTemplet.m_lstUpgradeReqItem.Count)
				{
					NKCUtil.SetGameobjectActive(nKCUIItemCostSlot, bValue: true);
					long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(skillTemplet.m_lstUpgradeReqItem[i].ItemID);
					nKCUIItemCostSlot.SetData(skillTemplet.m_lstUpgradeReqItem[i].ItemID, skillTemplet.m_lstUpgradeReqItem[i].ItemCount, countMiscItem);
				}
				else
				{
					NKCUtil.SetGameobjectActive(nKCUIItemCostSlot, bValue: false);
					nKCUIItemCostSlot.SetData(0, 0, 0L);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUIItemCostSlot, bValue: false);
				nKCUIItemCostSlot.SetData(0, 0, 0L);
			}
		}
		if (skillTemplet != null && skillTemplet.m_UnlockReqUpgrade > 0)
		{
			int num = NKMUnitManager.GetUnitTempletBase(m_UnitData.m_UnitID).m_StarGradeMax - 3;
			int reqCnt = skillTemplet.m_UnlockReqUpgrade + num;
			int num2 = m_UnitData.m_LimitBreakLevel + num;
			NKCUtil.SetGameobjectActive(m_objLimitBreak, bValue: true);
			NKCUtil.SetGameobjectActive(m_itemLimitBreak, bValue: true);
			m_itemLimitBreak.SetData(912, reqCnt, num2);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objLimitBreak, bValue: false);
			NKCUtil.SetGameobjectActive(m_itemLimitBreak, bValue: false);
			m_itemLimitBreak.SetData(0, 0, 0L);
		}
	}

	public void OnSkillLevelUp(int skillID = 0)
	{
		if (skillID == 0)
		{
			for (int i = 0; i < m_lstSkillSlot.Count; i++)
			{
				m_lstSkillSlot[i].ShowEffect(value: false);
			}
		}
		else
		{
			for (int j = 0; j < m_lstSkillSlot.Count; j++)
			{
				m_lstSkillSlot[j].ShowEffect(m_lstSkillSlot[j].CurrentSkillID == skillID);
			}
		}
	}
}
