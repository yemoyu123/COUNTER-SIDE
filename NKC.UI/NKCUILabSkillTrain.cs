using System;
using System.Collections.Generic;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILabSkillTrain : MonoBehaviour
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

	public delegate void OnTrySkillTrain(long targetUnitUID, int skillID);

	private const int HYPER_SKILL_SLOT_INDEX = 3;

	public List<UnitSkillSlot> m_lstSkillSlot;

	[Header("상세정보")]
	public GameObject m_objRootSkillDetail;

	public Text m_lbSkillName;

	public Image m_imgSkillLock;

	public Text m_lbSkillType;

	public GameObject m_objSkillCooldown;

	public Text m_lbSkillCooldown;

	public GameObject m_objSkillAttackCount;

	public Text m_lbSkillAttackCount;

	public GameObject m_objHyperSkillGlowEffect;

	public GameObject m_objSkillLockRoot;

	public GameObject m_NKM_UI_LAB_TRAINING_SKILL_LV_BACK;

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

	[Header("버튼")]
	public NKCUIComStateButton m_csbtnToSkillMenu;

	public GameObject m_objButtonEffect;

	public Text m_NKM_UI_LAB_TRAINING_ENTER_TEXT;

	public Image m_ImgNKM_UI_LAB_TRAINING_ENTER_ICON;

	private OnTrySkillTrain dOnTrySkillTrain;

	private NKMUnitSkillTemplet m_currentSkill;

	private NKMUnitData m_UnitData;

	private int CurrentSkillID
	{
		get
		{
			if (m_currentSkill == null)
			{
				return 0;
			}
			return m_currentSkill.m_ID;
		}
	}

	private NKMUserData UserData => NKCScenManager.CurrentUserData();

	private void OnDisable()
	{
		for (int i = 0; i < m_lstSkillSlot.Count; i++)
		{
			m_lstSkillSlot[i].ShowEffect(value: false);
		}
	}

	public void Init(OnTrySkillTrain onTrySkillTrain)
	{
		for (int i = 0; i < m_lstSkillSlot.Count; i++)
		{
			m_lstSkillSlot[i].Init(OnSelectSlot);
		}
		m_csbtnToSkillMenu.PointerClick.RemoveAllListeners();
		m_csbtnToSkillMenu.PointerClick.AddListener(StartTrain);
		dOnTrySkillTrain = onTrySkillTrain;
	}

	public void Cleanup()
	{
		m_UnitData = null;
		m_currentSkill = null;
		foreach (UnitSkillSlot item in m_lstSkillSlot)
		{
			item.Cleanup();
		}
	}

	public void SetData(NKMUserData userData, NKMUnitData unitData, int selectedSkillID = -1, bool bAnimate = false)
	{
		if (!m_NKM_UI_LAB_TRAINING_SKILL_LV_BACK.activeSelf)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_LAB_TRAINING_SKILL_LV_BACK, bValue: true);
		}
		m_UnitData = unitData;
		m_currentSkill = null;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase != null)
		{
			for (int i = 0; i < m_lstSkillSlot.Count; i++)
			{
				NKMUnitSkillTemplet unitSkillTemplet = NKMUnitSkillManager.GetUnitSkillTemplet(unitTempletBase.GetSkillStrID(i), unitData);
				bool bIsHyper = unitSkillTemplet != null && unitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER;
				m_lstSkillSlot[i].SetData(unitSkillTemplet, bIsHyper);
				if (unitSkillTemplet != null && NKMUnitSkillManager.IsLockedSkill(unitSkillTemplet.m_ID, m_UnitData.m_LimitBreakLevel))
				{
					m_lstSkillSlot[i].LockSkill(value: true);
				}
				if (i == 0)
				{
					if (unitSkillTemplet == null)
					{
						Debug.Log("Unit have no skill. UnitID : " + unitData.m_UnitID);
					}
					else if (selectedSkillID == -1)
					{
						selectedSkillID = unitSkillTemplet.m_ID;
					}
				}
				if (i == 3)
				{
					if (unitSkillTemplet == null || NKMUnitSkillManager.IsLockedSkill(unitSkillTemplet.m_ID, unitData.m_LimitBreakLevel))
					{
						NKCUtil.SetGameobjectActive(m_objHyperSkillGlowEffect, bValue: false);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_objHyperSkillGlowEffect, bValue: true);
					}
				}
			}
			SelectSkill(userData, unitData, selectedSkillID, bAnimate);
		}
		else
		{
			for (int j = 0; j < m_lstSkillSlot.Count; j++)
			{
				m_lstSkillSlot[j].SetData(null, bIsHyper: false);
			}
			SelectSkill(userData, null, 0, bAnimate);
			NKCUtil.SetGameobjectActive(m_objHyperSkillGlowEffect, bValue: false);
		}
	}

	private void SelectSkill(NKMUserData userData, NKMUnitData unitData, int skillID, bool bAnimate = true)
	{
		if (unitData == null)
		{
			SetSkillDetail(null);
			UpdateSkillTrainButton(userData, unitData, skillID, bAnimate);
			return;
		}
		NKMUnitSkillTemplet unitSkillTemplet = NKMUnitSkillManager.GetUnitSkillTemplet(skillID, unitData);
		if (unitSkillTemplet == null)
		{
			if (unitData.GetUnitSkillCount() == 0)
			{
				Debug.LogError("Unit have no skill. unitID : " + unitData.m_UnitID);
			}
			SetSkillDetail(null);
			UpdateSkillTrainButton(userData, unitData, skillID, bAnimate);
		}
		else
		{
			m_currentSkill = unitSkillTemplet;
			SetHighlightSlot(skillID);
			SetSkillDetail(unitSkillTemplet);
			UpdateSkillTrainButton(userData, unitData, skillID, bAnimate);
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
		SetSkillType(skillTemplet.m_NKM_SKILL_TYPE);
		SetUnlockReqUpgradeCount(skillTemplet);
		if (skillTemplet.m_fCooltimeSecond > 0f)
		{
			NKCUtil.SetGameobjectActive(m_objSkillCooldown, bValue: true);
			NKCUtil.SetLabelText(m_lbSkillCooldown, string.Format(NKCUtilString.GET_STRING_TIME_SECOND_ONE_PARAM, skillTemplet.m_fCooltimeSecond));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSkillCooldown, bValue: false);
		}
		int skillAttackCount = GetSkillAttackCount(skillTemplet);
		if (skillAttackCount > 0)
		{
			NKCUtil.SetGameobjectActive(m_objSkillAttackCount, bValue: true);
			NKCUtil.SetLabelText(m_lbSkillAttackCount, string.Format(NKCUtilString.GET_STRING_SKILL_ATTACK_COUNT_ONE_PARAM, skillAttackCount));
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

	private int GetSkillAttackCount(NKMUnitSkillTemplet unitTemplet)
	{
		return unitTemplet?.m_AttackCount ?? 0;
	}

	public void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (m_currentSkill == null)
		{
			return;
		}
		foreach (NKCUIItemCostSlot item in m_lstCostItemUI)
		{
			if (item.ItemID == itemData.ItemID)
			{
				UpdateRequiredItemData(m_currentSkill.m_ID, m_currentSkill.m_Level + 1);
				UpdateSkillTrainButton(UserData, m_UnitData, m_currentSkill.m_ID);
				break;
			}
		}
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

	private void UpdateSkillTrainButton(NKMUserData userData, NKMUnitData unitData, int skillID, bool bAnimate = true)
	{
		switch (CanTrainSkill(userData, unitData, skillID))
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

	private NKM_ERROR_CODE CanTrainSkill(NKMUserData userData, NKMUnitData targetUnit, int skillID)
	{
		if (targetUnit == null || userData == null || userData.m_ArmyData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (targetUnit.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED;
		}
		return userData.m_ArmyData.GetUnitDeckState(targetUnit) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKMUnitSkillManager.CanTrainSkill(userData, targetUnit, skillID), 
		};
	}

	private void SetSkillType(NKM_SKILL_TYPE type)
	{
		m_lbSkillType.color = NKCUtil.GetSkillTypeColor(type);
		m_lbSkillType.text = NKCUtilString.GetSkillTypeName(type);
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
		NKMUnitSkillManager.GetUnlockReqUpgradeFromSkillId(skillTemplet.m_ID);
		NKCUtil.SetGameobjectActive(m_objSkillLockRoot, bValue: true);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitData.m_UnitID);
		NKCUtil.SetSkillUnlockStarRank(m_lstObjSkillLockStar, skillTemplet, unitTempletBase.m_StarGradeMax);
	}

	private void OnSelectSlot(NKMUnitSkillTemplet skillTemplet)
	{
		if (skillTemplet != null)
		{
			SelectSkill(UserData, m_UnitData, skillTemplet.m_ID);
		}
	}

	public void StartTrain()
	{
		if (m_currentSkill == null || UserData == null)
		{
			return;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = CanTrainSkill(UserData, m_UnitData, CurrentSkillID);
		switch (nKM_ERROR_CODE)
		{
		case NKM_ERROR_CODE.NEC_OK:
			if (dOnTrySkillTrain != null)
			{
				dOnTrySkillTrain(m_UnitData.m_UnitUID, CurrentSkillID);
			}
			break;
		case NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_NEED_LIMIT_BREAK:
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
			break;
		case NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_NOT_ENOUGH_ITEM:
		{
			int unitSkillLevel = m_UnitData.GetUnitSkillLevel(CurrentSkillID);
			NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(CurrentSkillID, unitSkillLevel + 1);
			if (skillTemplet == null || UserData == null || UserData.m_InventoryData == null)
			{
				break;
			}
			for (int i = 0; i < skillTemplet.m_lstUpgradeReqItem.Count; i++)
			{
				if (UserData.m_InventoryData.GetCountMiscItem(skillTemplet.m_lstUpgradeReqItem[i].ItemID) < skillTemplet.m_lstUpgradeReqItem[i].ItemCount)
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
		}
	}

	public void UnitUpdated(long uid, NKMUnitData unitData)
	{
		if (m_UnitData != null && m_UnitData.m_UnitUID == uid && unitData != null)
		{
			SetData(UserData, unitData, CurrentSkillID, bAnimate: true);
		}
	}

	public void SwitchSkillBack(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_LAB_TRAINING_SKILL_LV_BACK, bActive);
	}

	public void OnSkillLevelUp(int skillID)
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
