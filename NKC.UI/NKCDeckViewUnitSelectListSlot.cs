using System;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCDeckViewUnitSelectListSlot : NKCUIUnitSelectListSlotBase
{
	[Header("일반 유닛 전용 정보")]
	public Slider m_sliderExp;

	public Text m_lbSummonCost;

	public GameObject m_objAutoSelect;

	public GameObject m_objClearDeck;

	public CanvasGroup m_cgCard;

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

	[Header("전술업데이트")]
	public NKCUITacticUpdateLevelSlot m_tacticUpdateSlot;

	[Header("지부 소속 정보")]
	public GameObject m_objCityLeaderRoot;

	public Image m_imgCityLeaderBG;

	public Text m_lbCityLeader;

	public Sprite m_spCityLeaderBG;

	public Sprite m_spOtherCityBG;

	[Header("슬롯 선택 표시")]
	public GameObject m_objBorderSelect;

	protected override NKCResourceUtility.eUnitResourceType UseResourceType => NKCResourceUtility.eUnitResourceType.INVEN_ICON;

	public override void SetData(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, int officeID = 0)
	{
		base.SetData(cNKMUnitData, deckIndex, bEnableLayoutElement, onSelectThisSlot);
		ProcessBanUIForUnit();
		if (cNKMUnitData != null)
		{
			if (m_lbSummonCost != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(cNKMUnitData.m_UnitID);
				int num = 0;
				if (m_bEnableShowBan && NKCBanManager.IsBanUnit(cNKMUnitData.m_UnitID))
				{
					num = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null);
					m_lbSummonCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_BAN_COST, num.ToString());
				}
				else if (m_bEnableShowUpUnit && NKCBanManager.IsUpUnit(cNKMUnitData.m_UnitID))
				{
					num = unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData);
					m_lbSummonCost.text = string.Format(NKCUtilString.GET_STRING_UNIT_UP_COST, num.ToString());
				}
				else
				{
					num = unitStatTemplet.GetRespawnCost(bPVP: false, bLeader: false, null, null);
					m_lbSummonCost.text = num.ToString();
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
			m_tacticUpdateSlot?.SetLevel(cNKMUnitData.tacticLevel);
		}
		else
		{
			m_tacticUpdateSlot?.SetLevel(0);
		}
		SetCityLeaderTag(NKMWorldMapManager.WorldMapLeaderState.None);
		if (m_objEquip != null)
		{
			SetEquipListData(m_NKMUnitData);
		}
	}

	public override void SetData(NKMUnitTempletBase templetBase, int levelToDisplay, int skinID, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot)
	{
		base.SetData(templetBase, levelToDisplay, skinID, bEnableLayoutElement, onSelectThisSlot);
		if (templetBase != null)
		{
			if (m_lbSummonCost != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(templetBase.m_UnitID);
				m_lbSummonCost.text = unitStatTemplet.GetRespawnCost(bLeader: false, null, null).ToString();
			}
			if (m_sliderExp != null)
			{
				m_sliderExp.value = 0f;
			}
			m_comStarRank?.SetStarRank(templetBase, levelToDisplay);
		}
		m_tacticUpdateSlot?.SetLevel(0);
		if (m_objEquip != null)
		{
			SetEquipListData(m_NKMUnitData);
		}
	}

	public override void SetDataForBan(NKMUnitTempletBase templetBase, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, bool bUp = false, bool bSetOriginalCost = false)
	{
		SetData(templetBase, 0, bEnableLayoutElement, onSelectThisSlot);
		ProcessBanUIForUnit();
		if (templetBase != null)
		{
			if (m_lbSummonCost != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(templetBase.m_UnitID);
				m_lbSummonCost.text = unitStatTemplet.GetRespawnCost(bLeader: false, null, null).ToString();
			}
			if (m_sliderExp != null)
			{
				m_sliderExp.value = 0f;
			}
			m_comStarRank?.SetStarRank(3, templetBase.m_StarGradeMax);
		}
		m_tacticUpdateSlot?.SetLevel(0);
		SetCityLeaderMark(value: false);
		SetCityLeaderTag(NKMWorldMapManager.WorldMapLeaderState.None);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
	}

	public override void SetSlotState(NKCUnitSortSystem.eUnitState eUnitSlotState)
	{
		m_eUnitSlotState = eUnitSlotState;
		NKCUtil.SetGameobjectActive(m_objInCityMission, m_eUnitSlotState != NKCUnitSortSystem.eUnitState.NONE);
		NKCUtil.SetGameobjectActive(m_objSeized, bValue: false);
		if (m_cgCard != null)
		{
			m_cgCard.alpha = ((eUnitSlotState == NKCUnitSortSystem.eUnitState.NONE) ? 1f : 0.5f);
		}
		switch (m_eUnitSlotState)
		{
		case NKCUnitSortSystem.eUnitState.DUPLICATE:
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_DECK_UNIT_STATE_DUPLICATE);
			break;
		case NKCUnitSortSystem.eUnitState.CITY_SET:
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_WORLDMAP_CITY_LEADER);
			break;
		case NKCUnitSortSystem.eUnitState.CITY_MISSION:
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_DECK_UNIT_STATE_MISSION);
			break;
		case NKCUnitSortSystem.eUnitState.WARFARE_BATCH:
		case NKCUnitSortSystem.eUnitState.DIVE_BATCH:
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_DECK_UNIT_STATE_DOING);
			break;
		case NKCUnitSortSystem.eUnitState.LOCKED:
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_DECK_UNIT_STATE_LOCKED);
			break;
		case NKCUnitSortSystem.eUnitState.DECKED:
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_DECK_UNIT_STATE_DECKED);
			break;
		case NKCUnitSortSystem.eUnitState.MAINUNIT:
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_DECK_UNIT_STATE_MAINUNIT);
			break;
		case NKCUnitSortSystem.eUnitState.SEIZURE:
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_DECK_UNIT_STATE_SEIZURE);
			break;
		case NKCUnitSortSystem.eUnitState.LOBBY_UNIT:
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_LOBBY_UNIT_CAPTAIN);
			break;
		case NKCUnitSortSystem.eUnitState.DUNGEON_RESTRICTED:
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCStringTable.GetString("SI_DP_DECK_UNIT_STATE_CANNOT_USE"));
			break;
		case NKCUnitSortSystem.eUnitState.LEAGUE_BANNED:
			NKCUtil.SetCanvasGroupAlpha(m_cgCard, 1f);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeagueBanned, bValue: true);
			NKCUtil.SetGameobjectActive(m_objLeaguePicked, bValue: false);
			NKCUtil.SetLabelText(m_lbBusyText, "");
			NKCUtil.SetLabelText(m_lbMissionStatus, "");
			break;
		case NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_LEFT:
		case NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_RIGHT:
		{
			NKCUtil.SetCanvasGroupAlpha(m_cgCard, 1f);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeagueBanned, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeaguePicked, bValue: true);
			Color color = ((m_eUnitSlotState == NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_LEFT) ? m_colorLeaguePickedLeft : m_colorLeaguePickedRight);
			NKCUtil.SetImageColor(m_imgLeaguePicked, color);
			NKCUtil.SetLabelText(m_lbMissionStatus, "");
			break;
		}
		case NKCUnitSortSystem.eUnitState.CHECKED:
			break;
		}
	}

	public override void SetEquipData(NKMEquipmentSet equipSet)
	{
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
	}

	protected override void OnClick()
	{
		NKCUnitSortSystem.eUnitState eUnitSlotState = m_eUnitSlotState;
		if ((uint)(eUnitSlotState - 13) > 2u)
		{
			base.OnClick();
		}
	}

	protected override void SetMode(eUnitSlotMode mode)
	{
		base.SetMode(mode);
		if ((uint)(mode - 5) <= 1u)
		{
			NKCUtil.SetGameobjectActive(m_objSlotStatus, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objAutoSelect, mode == eUnitSlotMode.AutoComplete);
		NKCUtil.SetGameobjectActive(m_objClearDeck, mode == eUnitSlotMode.ClearAll);
		NKCUtil.SetGameobjectActive(m_lbName, mode == eUnitSlotMode.Character);
	}

	public void SetCityLeaderTag(NKMWorldMapManager.WorldMapLeaderState eWorldmapState)
	{
		bool flag = false;
		bool flag2;
		switch (eWorldmapState)
		{
		default:
			flag2 = false;
			break;
		case NKMWorldMapManager.WorldMapLeaderState.CityLeaderOther:
			flag2 = true;
			flag = true;
			break;
		case NKMWorldMapManager.WorldMapLeaderState.CityLeader:
			flag2 = true;
			flag = false;
			break;
		}
		NKCUtil.SetGameobjectActive(m_objCityLeaderRoot, flag2);
		if (flag2)
		{
			if (!flag)
			{
				NKCUtil.SetImageSprite(m_imgCityLeaderBG, m_spCityLeaderBG);
				NKCUtil.SetLabelText(m_lbCityLeader, NKCUtilString.GET_STRING_WORLDMAP_CITY_LEADER);
			}
			else
			{
				NKCUtil.SetImageSprite(m_imgCityLeaderBG, m_spOtherCityBG);
				NKCUtil.SetLabelText(m_lbCityLeader, NKCUtilString.GET_STRING_WORLDMAP_ANOTHER_CITY);
			}
		}
	}

	protected void SetEquipListData(NKMUnitData unitData)
	{
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

	public void SetSlotSelect(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_objBorderSelect, bActive);
	}
}
