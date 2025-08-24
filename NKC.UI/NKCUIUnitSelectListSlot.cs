using System;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitSelectListSlot : NKCUIUnitSelectListSlotBase
{
	[Header("일반 유닛 전용 정보")]
	public Slider m_sliderExp;

	public Text m_lbSummonCost;

	public Text m_lbClassMark;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EMPLOYEE;

	public Text m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EMPLOYEE_TEXT;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_DISABLE;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_CARD_DENIED;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_STAR_GRADE;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_COST;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_BOTTOM;

	[Header("전술업데이트/코스트 부모")]
	public GameObject m_objCostRoot;

	[Header("전술업데이트")]
	public NKCUITacticUpdateLevelSlot m_tacticUpdateSlot;

	[Header("역할군 마크")]
	public Image m_imgUnitRole;

	[Header("하단 장비 표시")]
	public GameObject m_objEquip;

	public Sprite m_spEquipEmpty;

	public Sprite m_spEquipN;

	public Sprite m_spEquipR;

	public Sprite m_spEquipSR;

	public Sprite m_spEquipSSR;

	public Sprite m_spEquipLock;

	public Sprite m_spEquipReactor;

	public Image m_imgEquipWeapon;

	public Image m_imgEquipArmor;

	public Image m_imgEquipAcc;

	public Image m_imgEquipAcc2;

	public Image m_imgEquipReactor;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_1_SET;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_2_SET;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_3_SET;

	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_4_SET;

	[Header("경험치 보너스 있음")]
	public GameObject m_objExpBonus;

	[Header("당장 초월 가능")]
	public GameObject m_objCanLimitbreakNow;

	public GameObject m_objRootLimitBreak;

	public GameObject m_objLimitBreakCharCount;

	public Text m_lbLimitBreakCharCount;

	public GameObject m_objRootTranscendence;

	public GameObject m_objTranscendenceCharCount;

	public Text m_lbTranscendenceCharCount;

	[Header("전술 업데이트")]
	public GameObject m_objCanTacticUpdateNow;

	public Text m_lbTacticUpdateCharCount;

	public GameObject m_objTacticUpdateMultiSelect;

	public Text m_lbTacticUpdateMultiSelect;

	[Header("경험치 바")]
	public Image m_Img_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL_GAUGE;

	[Header("종신계약")]
	public GameObject m_objLifetime;

	[Header("월드맵 지부장 표시")]
	public GameObject m_objCityLeader;

	[Header("채용획득 표기")]
	public GameObject m_objContractGainUnit;

	[Header("보유중")]
	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_UNIT_HAVE_COUNT;

	[Header("격전 지원")]
	public Text m_NKM_UI_UNIT_SELECT_LIST_FIERCE_BATTLE_TEXT;

	[Header("리콜")]
	public GameObject m_objRecall;

	[Header("미션 달성도")]
	public GameObject m_objUnitAchievement;

	public GameObject m_objAchievementGauge;

	public GameObject m_objAchievementComplete;

	public Image m_imgAchieveGauge;

	public Text m_lbAchieveCount;

	[Header("레드닷")]
	public GameObject m_objRedDot;

	private bool m_bSetEquipOtherWay;

	protected override void SetTempletData(NKMUnitTempletBase templetBase)
	{
		base.SetTempletData(templetBase);
		Sprite unitClassIcon = GetUnitClassIcon(templetBase);
		NKCUtil.SetImageSprite(m_imgUnitRole, unitClassIcon, bDisableIfSpriteNull: true);
		NKCUtil.SetGameobjectActive(m_objCostRoot, templetBase != null && !templetBase.IsTrophy);
		SetClassMark(templetBase);
	}

	public override void SetData(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, int officeID = 0)
	{
		base.SetData(cNKMUnitData, deckIndex, bEnableLayoutElement, onSelectThisSlot, officeID);
		ProcessBanUIForUnit();
		NKCUtil.SetGameobjectActive(m_objExpBonus, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanLimitbreakNow, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanTacticUpdateNow, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTacticUpdateMultiSelect, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLifetime, bValue: false);
		NKCUtil.SetGameobjectActive(m_objContractGainUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_UNIT_HAVE_COUNT, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRecall, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUnitAchievement, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		if (cNKMUnitData != null)
		{
			if (m_lbSummonCost != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(cNKMUnitData.m_UnitID);
				if (m_bEnableShowBan && NKCBanManager.IsBanUnit(cNKMUnitData.m_UnitID))
				{
					int respawnCost = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null);
					m_lbSummonCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_BAN_COST, respawnCost.ToString());
				}
				else if (m_bEnableShowUpUnit && NKCBanManager.IsUpUnit(cNKMUnitData.m_UnitID))
				{
					int respawnCost2 = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData);
					m_lbSummonCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_UP_COST, respawnCost2.ToString());
				}
				else
				{
					m_lbSummonCost.text = unitStatTemplet.GetRespawnCost(bLeader: false, null, null).ToString();
				}
			}
			if (m_sliderExp != null)
			{
				if (NKCExpManager.GetUnitMaxLevel(cNKMUnitData) == cNKMUnitData.m_UnitLevel)
				{
					m_sliderExp.value = 1f;
				}
				else
				{
					m_sliderExp.value = NKCExpManager.GetUnitNextLevelExpProgress(cNKMUnitData);
				}
			}
			m_comStarRank?.SetStarRank(cNKMUnitData);
			if (m_Img_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL_GAUGE != null)
			{
				if (NKCExpManager.GetUnitMaxLevel(cNKMUnitData) == cNKMUnitData.m_UnitLevel)
				{
					m_Img_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL_GAUGE.fillAmount = 1f;
				}
				else
				{
					m_Img_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL_GAUGE.fillAmount = NKCExpManager.GetUnitNextLevelExpProgress(cNKMUnitData);
				}
			}
			NKCUtil.SetGameobjectActive(m_objLifetime, cNKMUnitData.IsPermanentContract);
			if (m_tacticUpdateSlot != null)
			{
				m_tacticUpdateSlot.SetLevel(m_NKMUnitData.tacticLevel);
			}
		}
		else
		{
			m_tacticUpdateSlot?.SetLevel(0);
		}
		SetEnableEquipListData(!m_bEnableShowCastingBan);
		SetEquipListData(cNKMUnitData);
		SetCityLeaderMark(value: false);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
	}

	public override void SetData(NKMUnitTempletBase templetBase, int levelToDisplay, int skinID, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot)
	{
		base.SetData(templetBase, levelToDisplay, skinID, bEnableLayoutElement, onSelectThisSlot);
		if (m_tacticUpdateSlot != null)
		{
			m_tacticUpdateSlot.SetLevel(0);
		}
		NKCUtil.SetGameobjectActive(m_objExpBonus, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanLimitbreakNow, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanTacticUpdateNow, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTacticUpdateMultiSelect, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLifetime, bValue: false);
		NKCUtil.SetGameobjectActive(m_objContractGainUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_UNIT_HAVE_COUNT, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRecall, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUnitAchievement, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		if (templetBase != null)
		{
			if (m_lbSummonCost != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(templetBase.m_UnitID);
				if (unitStatTemplet != null)
				{
					m_lbSummonCost.text = unitStatTemplet.GetRespawnCost(bLeader: false, null, null).ToString();
				}
			}
			if (m_sliderExp != null)
			{
				m_sliderExp.value = 0f;
			}
			m_comStarRank?.SetStarRank(templetBase, levelToDisplay);
		}
		SetEnableEquipListData(bEnable: false);
		SetEquipListData(null);
		SetCityLeaderMark(value: false);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
	}

	public override void SetDataForBan(NKMUnitTempletBase templetBase, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, bool bUp = false, bool bSetOriginalCost = false)
	{
		SetData(templetBase, 0, bEnableLayoutElement, onSelectThisSlot);
		NKCUtil.SetGameobjectActive(m_objExpBonus, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanLimitbreakNow, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanTacticUpdateNow, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTacticUpdateMultiSelect, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLifetime, bValue: false);
		NKCUtil.SetGameobjectActive(m_objContractGainUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_UNIT_HAVE_COUNT, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRecall, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUnitAchievement, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		ProcessBanUIForUnit();
		if (templetBase != null && m_lbSummonCost != null)
		{
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(templetBase.m_UnitID);
			if (unitStatTemplet != null)
			{
				int num = 0;
				if (bSetOriginalCost)
				{
					num = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, null);
					NKCUtil.SetLabelText(m_lbSummonCost, num.ToString());
				}
				else if (!bUp)
				{
					num = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(m_eBanDataType), null);
					m_lbSummonCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_BAN_COST, num.ToString());
				}
				else
				{
					num = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData);
					m_lbSummonCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_UP_COST, num.ToString());
				}
			}
		}
		SetEnableEquipListData(bEnable: false);
		SetEquipListData(null);
		SetCityLeaderMark(value: false);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
	}

	public override void SetDataForContractSelection(NKMUnitData cNKMUnitData, bool bHave = true)
	{
		NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EMPLOYEE, bValue: false);
		base.SetData(cNKMUnitData, NKMDeckIndex.None, bEnableLayoutElement: true, null);
		NKCUtil.SetGameobjectActive(m_objDisableSelectSlot, bValue: false);
		if (cNKMUnitData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData);
			if (unitTempletBase != null)
			{
				m_comStarRank?.SetStarRank(cNKMUnitData.m_LimitBreakLevel, unitTempletBase.m_StarGradeMax);
			}
			if (m_lbSummonCost != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(cNKMUnitData.m_UnitID);
				if (unitStatTemplet != null)
				{
					m_lbSummonCost.text = unitStatTemplet.GetRespawnCost(bLeader: false, null, null).ToString();
				}
			}
			NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL, bValue: false);
			if (bHave)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_UNIT_HAVE_COUNT, !NKCScenManager.CurrentUserData().m_ArmyData.IsFirstGetUnit(cNKMUnitData.m_UnitID));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_UNIT_HAVE_COUNT, bValue: false);
			}
			if (m_tacticUpdateSlot != null)
			{
				m_tacticUpdateSlot.SetLevel(cNKMUnitData.tacticLevel);
			}
		}
		else
		{
			m_tacticUpdateSlot.SetLevel(0);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_DISABLE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_CARD_DENIED, bValue: false);
		UpdateCommonUIForMaxData(bEnable: true);
	}

	public override void SetDataForCollection(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, OnSelectThisSlot onSelectThisSlot, bool bEnable = false)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_UNIT_HAVE_COUNT, bValue: false);
		base.SetData(cNKMUnitData, deckIndex, bEnableLayoutElement: true, onSelectThisSlot);
		if (cNKMUnitData != null)
		{
			NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(cNKMUnitData.m_UnitID);
			NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EMPLOYEE, unitTemplet != null && !unitTemplet.m_bExclude);
			NKCUtil.SetLabelText(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EMPLOYEE_TEXT, NKCCollectionManager.GetEmployeeNumber(cNKMUnitData.m_UnitID));
			if (m_lbSummonCost != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(cNKMUnitData.m_UnitID);
				if (unitStatTemplet != null)
				{
					m_lbSummonCost.text = unitStatTemplet.GetRespawnCost(bLeader: false, null, null).ToString();
				}
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData);
			if (unitTempletBase != null)
			{
				m_comStarRank?.SetStarRank(3, unitTempletBase.m_StarGradeMax);
			}
			if (m_Img_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL_GAUGE != null)
			{
				if (NKCExpManager.GetUnitMaxLevel(cNKMUnitData) == cNKMUnitData.m_UnitLevel)
				{
					m_Img_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL_GAUGE.fillAmount = 1f;
				}
				else
				{
					m_Img_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL_GAUGE.fillAmount = NKCExpManager.GetUnitNextLevelExpProgress(cNKMUnitData);
				}
			}
			m_tacticUpdateSlot.SetLevel(cNKMUnitData.tacticLevel);
		}
		else
		{
			m_tacticUpdateSlot.SetLevel(0);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_DISABLE, !bEnable);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_CARD_DENIED, !bEnable);
		UpdateCommonUIForMaxData(bEnable);
	}

	public override void SetDataForRearm(NKMUnitData unitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, bool bShowEqup = true, bool bShowLevel = false, bool bUnable = false)
	{
		SetData(unitData, deckIndex, bEnableLayoutElement, onSelectThisSlot);
		SetEnableLevelInfo(bShowLevel);
		SetEnableEquipListData(bShowEqup);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_DISABLE, bUnable);
	}

	public override void SetDataForDummyUnit(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, int officeID = 0)
	{
		m_bSetEquipOtherWay = true;
		base.SetData(cNKMUnitData, deckIndex, bEnableLayoutElement, onSelectThisSlot, officeID);
		NKCUtil.SetGameobjectActive(m_objExpBonus, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanLimitbreakNow, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanTacticUpdateNow, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTacticUpdateMultiSelect, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLifetime, bValue: false);
		NKCUtil.SetGameobjectActive(m_objContractGainUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_UNIT_HAVE_COUNT, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRecall, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUnitAchievement, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		if (cNKMUnitData != null)
		{
			if (m_lbSummonCost != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(cNKMUnitData.m_UnitID);
				if (m_bEnableShowBan && NKCBanManager.IsBanUnit(cNKMUnitData.m_UnitID))
				{
					int respawnCost = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null);
					m_lbSummonCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_BAN_COST, respawnCost.ToString());
				}
				else if (m_bEnableShowUpUnit && NKCBanManager.IsUpUnit(cNKMUnitData.m_UnitID))
				{
					int respawnCost2 = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData);
					m_lbSummonCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_UP_COST, respawnCost2.ToString());
				}
				else
				{
					m_lbSummonCost.text = unitStatTemplet.GetRespawnCost(bLeader: false, null, null).ToString();
				}
			}
			if (m_sliderExp != null)
			{
				if (NKCExpManager.GetUnitMaxLevel(cNKMUnitData) == cNKMUnitData.m_UnitLevel)
				{
					m_sliderExp.value = 1f;
				}
				else
				{
					m_sliderExp.value = NKCExpManager.GetUnitNextLevelExpProgress(cNKMUnitData);
				}
			}
			m_comStarRank?.SetStarRank(cNKMUnitData);
			if (m_Img_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL_GAUGE != null)
			{
				if (NKCExpManager.GetUnitMaxLevel(cNKMUnitData) == cNKMUnitData.m_UnitLevel)
				{
					m_Img_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL_GAUGE.fillAmount = 1f;
				}
				else
				{
					m_Img_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL_GAUGE.fillAmount = NKCExpManager.GetUnitNextLevelExpProgress(cNKMUnitData);
				}
			}
			NKCUtil.SetGameobjectActive(m_objLifetime, cNKMUnitData.IsPermanentContract);
			if (m_tacticUpdateSlot != null)
			{
				m_tacticUpdateSlot.SetLevel(m_NKMUnitData.tacticLevel);
			}
		}
		else
		{
			m_tacticUpdateSlot?.SetLevel(0);
		}
		SetCityLeaderMark(value: false);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
		if (cNKMUnitData != null && NKCReactorUtil.IsReactorUnit(cNKMUnitData.m_UnitID))
		{
			if (cNKMUnitData.reactorLevel > 0)
			{
				NKCUtil.SetImageSprite(m_imgEquipReactor, m_spEquipReactor);
			}
			else
			{
				NKCUtil.SetImageSprite(m_imgEquipReactor, m_spEquipEmpty);
			}
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgEquipReactor, m_spEquipLock);
		}
	}

	public override void SetEquipData(NKMEquipmentSet equipSet)
	{
		ClearSetOptionEffect();
		if (equipSet == null)
		{
			return;
		}
		Image equipIconImage = GetEquipIconImage(ITEM_EQUIP_POSITION.IEP_WEAPON);
		if (equipSet.Weapon != null)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipSet.Weapon.m_ItemEquipID);
			if (equipTemplet != null)
			{
				equipIconImage.sprite = GetItemSprite(equipTemplet.m_NKM_ITEM_GRADE);
			}
			else
			{
				equipIconImage.sprite = m_spEquipEmpty;
			}
			if (NKMItemManager.IsActiveSetOptionItem(equipSet.Weapon, equipSet))
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_1_SET, bValue: true);
			}
		}
		else
		{
			equipIconImage.sprite = m_spEquipEmpty;
		}
		Image equipIconImage2 = GetEquipIconImage(ITEM_EQUIP_POSITION.IEP_DEFENCE);
		if (equipSet.Defence != null)
		{
			NKMEquipTemplet equipTemplet2 = NKMItemManager.GetEquipTemplet(equipSet.Defence.m_ItemEquipID);
			if (equipTemplet2 != null)
			{
				equipIconImage2.sprite = GetItemSprite(equipTemplet2.m_NKM_ITEM_GRADE);
			}
			else
			{
				equipIconImage2.sprite = m_spEquipEmpty;
			}
			if (NKMItemManager.IsActiveSetOptionItem(equipSet.Defence, equipSet))
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_2_SET, bValue: true);
			}
		}
		else
		{
			equipIconImage2.sprite = m_spEquipEmpty;
		}
		Image equipIconImage3 = GetEquipIconImage(ITEM_EQUIP_POSITION.IEP_ACC);
		if (equipSet.Accessory != null)
		{
			NKMEquipTemplet equipTemplet3 = NKMItemManager.GetEquipTemplet(equipSet.Accessory.m_ItemEquipID);
			if (equipTemplet3 != null)
			{
				equipIconImage3.sprite = GetItemSprite(equipTemplet3.m_NKM_ITEM_GRADE);
			}
			else
			{
				equipIconImage3.sprite = m_spEquipEmpty;
			}
			if (NKMItemManager.IsActiveSetOptionItem(equipSet.Accessory, equipSet))
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_3_SET, bValue: true);
			}
		}
		else
		{
			equipIconImage3.sprite = m_spEquipEmpty;
		}
		Image equipIconImage4 = GetEquipIconImage(ITEM_EQUIP_POSITION.IEP_ACC2);
		if (equipSet.Accessory2 != null)
		{
			NKMEquipTemplet equipTemplet4 = NKMItemManager.GetEquipTemplet(equipSet.Accessory2.m_ItemEquipID);
			if (equipTemplet4 != null)
			{
				equipIconImage4.sprite = GetItemSprite(equipTemplet4.m_NKM_ITEM_GRADE);
			}
			else
			{
				equipIconImage4.sprite = m_spEquipEmpty;
			}
			if (NKMItemManager.IsActiveSetOptionItem(equipSet.Accessory2, equipSet))
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_4_SET, bValue: true);
			}
		}
		else
		{
			equipIconImage4.sprite = m_spEquipEmpty;
		}
	}

	public void SetSlotDisable(bool bDisable)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_DISABLE, bDisable);
	}

	protected override void OnClick()
	{
		if (dOnSelectThisSlot != null)
		{
			dOnSelectThisSlot(m_NKMUnitData, m_NKMUnitTempletBase, m_DeckIndex, m_eUnitSlotState, m_eUnitSelectState);
		}
	}

	private void UpdateCommonUIForMaxData(bool bEnable)
	{
		NKCUtil.SetGameobjectActive(m_objDisableSelectSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objExpBonus, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanLimitbreakNow, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanTacticUpdateNow, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTacticUpdateMultiSelect, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLifetime, bValue: false);
		NKCUtil.SetGameobjectActive(m_objContractGainUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRecall, bValue: false);
		SetEnableEquipListData(bEnable: false);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
		SetEquipListData(null);
		SetCityLeaderMark(value: false);
		SetUnitAchievementInfo();
		NKCUtil.SetGameobjectActive(m_lbName.gameObject, bEnable);
		NKCUtil.SetGameobjectActive(m_imgUnitRole.gameObject, bEnable);
		NKCUtil.SetGameobjectActive(m_comStarRank.gameObject, bEnable);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_STAR_GRADE, bEnable);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_COST, bEnable);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_BOTTOM, bEnable);
	}

	public void SetExpBonusMark(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objExpBonus, value);
	}

	public void SetLimitPossibleMark(bool value, bool bTranscendence)
	{
		NKCUtil.SetGameobjectActive(m_objCanLimitbreakNow, value);
		if (value)
		{
			NKCUtil.SetGameobjectActive(m_objRootLimitBreak, !bTranscendence);
			NKCUtil.SetGameobjectActive(m_objRootTranscendence, bTranscendence);
			NKCUtil.SetGameobjectActive(m_objLimitBreakCharCount, bValue: false);
			NKCUtil.SetGameobjectActive(m_objTranscendenceCharCount, bValue: false);
		}
	}

	public void SetTacticPossibleMark(int sameCharCount)
	{
		NKCUtil.SetGameobjectActive(m_objCanTacticUpdateNow, sameCharCount > 0);
		NKCUtil.SetLabelText(m_lbTacticUpdateCharCount, sameCharCount.ToString());
	}

	public void SetTacticSelectUnitCnt(int selectedUnitCnt)
	{
		NKCUtil.SetGameobjectActive(m_objTacticUpdateMultiSelect, selectedUnitCnt > 0);
		NKCUtil.SetLabelText(m_lbTacticUpdateMultiSelect, selectedUnitCnt.ToString());
	}

	private void SetClassMark(NKMUnitTempletBase templetBase)
	{
		if (templetBase != null)
		{
			NKCUtil.SetLabelText(m_lbClassMark, NKCUtilString.GetUnitStyleName(templetBase.m_NKM_UNIT_STYLE_TYPE));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbClassMark, string.Empty);
		}
	}

	protected override void SetMode(eUnitSlotMode mode)
	{
		base.SetMode(mode);
		if (mode != eUnitSlotMode.Character)
		{
			SetEquipListData(null);
		}
		else
		{
			SetEquipListData(m_NKMUnitData);
		}
	}

	public override void SetCityLeaderMark(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objCityLeader, value);
	}

	public override void SetCityMissionStatus(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objInCityMission, value);
	}

	private Sprite GetUnitClassIcon(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase == null)
		{
			return null;
		}
		if (unitTempletBase.IsTrophy)
		{
			return null;
		}
		return NKCResourceUtility.GetOrLoadUnitRoleAttackTypeIcon(unitTempletBase, bSmall: true);
	}

	protected void SetEnableEquipListData(bool bEnable)
	{
		NKCUtil.SetGameobjectActive(m_objEquip, bEnable);
	}

	public override void SetContractedUnitMark(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objContractGainUnit, value);
	}

	public override void SetRecall(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objRecall, bValue);
	}

	protected void SetEquipListData(NKMUnitData unitData)
	{
		if (m_bSetEquipOtherWay)
		{
			return;
		}
		ClearSetOptionEffect();
		if (unitData == null)
		{
			foreach (ITEM_EQUIP_POSITION value in Enum.GetValues(typeof(ITEM_EQUIP_POSITION)))
			{
				NKCUtil.SetImageSprite(GetEquipIconImage(value), m_spEquipEmpty);
			}
		}
		else
		{
			foreach (ITEM_EQUIP_POSITION value2 in Enum.GetValues(typeof(ITEM_EQUIP_POSITION)))
			{
				SetWeaponImage(unitData, value2);
			}
		}
		if (unitData != null && NKCReactorUtil.IsReactorUnit(unitData.m_UnitID))
		{
			if (unitData.reactorLevel > 0)
			{
				NKCUtil.SetImageSprite(m_imgEquipReactor, m_spEquipReactor);
			}
			else
			{
				NKCUtil.SetImageSprite(m_imgEquipReactor, m_spEquipEmpty);
			}
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgEquipReactor, m_spEquipLock);
		}
	}

	private void ClearSetOptionEffect()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_1_SET, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_2_SET, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_3_SET, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_4_SET, bValue: false);
	}

	private void SetWeaponImage(NKMUnitData unitData, ITEM_EQUIP_POSITION position)
	{
		Image equipIconImage = GetEquipIconImage(position);
		if (equipIconImage == null)
		{
			return;
		}
		long equipUid = unitData.GetEquipUid(position);
		if (equipUid == 0L)
		{
			if (position == ITEM_EQUIP_POSITION.IEP_ACC2 && !unitData.IsUnlockAccessory2())
			{
				equipIconImage.sprite = m_spEquipLock;
			}
			else
			{
				equipIconImage.sprite = m_spEquipEmpty;
			}
			return;
		}
		if (NKMItemManager.IsActiveSetOptionItem(equipUid))
		{
			switch (position)
			{
			case ITEM_EQUIP_POSITION.IEP_WEAPON:
				NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_1_SET, bValue: true);
				break;
			case ITEM_EQUIP_POSITION.IEP_DEFENCE:
				NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_2_SET, bValue: true);
				break;
			case ITEM_EQUIP_POSITION.IEP_ACC:
				NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_3_SET, bValue: true);
				break;
			case ITEM_EQUIP_POSITION.IEP_ACC2:
				NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_EQUIP_4_SET, bValue: true);
				break;
			}
		}
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(equipUid);
		if (itemEquip == null)
		{
			Debug.LogError($"equipped equip not exist. uid {equipUid}");
			equipIconImage.sprite = m_spEquipEmpty;
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (equipTemplet == null)
		{
			Debug.LogError($"equiptemplet not exist. id {itemEquip.m_ItemEquipID}");
			equipIconImage.sprite = m_spEquipEmpty;
		}
		else
		{
			equipIconImage.sprite = GetItemSprite(equipTemplet.m_NKM_ITEM_GRADE);
		}
	}

	private void SetUnitAchievementInfo()
	{
		bool openTagCollectionMission = NKCUnitMissionManager.GetOpenTagCollectionMission();
		NKCUtil.SetGameobjectActive(m_objUnitAchievement, openTagCollectionMission);
		if (!openTagCollectionMission)
		{
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
			return;
		}
		int total = 0;
		int completed = 0;
		int rewardEnable = 0;
		NKCUnitMissionManager.GetUnitMissionRewardEnableCount(m_NKMUnitData.m_UnitID, ref total, ref completed, ref rewardEnable);
		NKCUtil.SetGameobjectActive(m_objAchievementGauge, total > completed);
		NKCUtil.SetGameobjectActive(m_objAchievementComplete, total <= completed);
		NKCUtil.SetLabelText(m_lbAchieveCount, $"{completed}/{total}");
		NKCUtil.SetImageFillAmount(m_imgAchieveGauge, (float)completed / (float)total);
		NKCUtil.SetGameobjectActive(m_objRedDot, rewardEnable > 0);
	}

	private Image GetEquipIconImage(ITEM_EQUIP_POSITION position)
	{
		return position switch
		{
			ITEM_EQUIP_POSITION.IEP_WEAPON => m_imgEquipWeapon, 
			ITEM_EQUIP_POSITION.IEP_DEFENCE => m_imgEquipArmor, 
			ITEM_EQUIP_POSITION.IEP_ACC => m_imgEquipAcc, 
			ITEM_EQUIP_POSITION.IEP_ACC2 => m_imgEquipAcc2, 
			_ => null, 
		};
	}

	private Sprite GetItemSprite(NKM_ITEM_GRADE grade)
	{
		return grade switch
		{
			NKM_ITEM_GRADE.NIG_R => m_spEquipR, 
			NKM_ITEM_GRADE.NIG_SR => m_spEquipSR, 
			NKM_ITEM_GRADE.NIG_SSR => m_spEquipSSR, 
			_ => m_spEquipN, 
		};
	}
}
