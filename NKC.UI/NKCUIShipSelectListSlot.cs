using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShipSelectListSlot : NKCUIUnitSelectListSlotBase
{
	[Header("함선 전용 정보")]
	public Text m_lbTitle;

	public Text m_lbShipClass;

	public Image m_imgShipClass;

	public Image m_imgShipMove;

	[Header("대표 유닛 마크")]
	public GameObject m_objRootMainUnitMark;

	public Text m_lbMainUnitMark;

	public Image m_imgMainUnitMark;

	public Color m_colMainUnitMark;

	[Header("함선 스킬 슬롯")]
	public List<NKCUIShipSkillSlot> m_lstSkillSlot;

	[Header("함선 비활성화")]
	public GameObject m_NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_CARD_COLLECITON_DENIED;

	[Header("격전지원 UI")]
	public GameObject m_NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_FIERCE_BATTLE;

	public Text m_NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_FIERCE_BATTLE_TEXT;

	[Header("리콜")]
	public GameObject m_objRecall;

	private void ProcessBanUI()
	{
		if (m_NKMUnitTempletBase != null)
		{
			if (m_bEnableShowBan && NKCBanManager.IsBanShip(m_NKMUnitTempletBase.m_ShipGroupID, m_eBanDataType))
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
				int shipBanLevel = NKCBanManager.GetShipBanLevel(m_NKMUnitTempletBase.m_ShipGroupID, m_eBanDataType);
				NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, shipBanLevel));
				int nerfPercentByShipBanLevel = NKMUnitStatManager.GetNerfPercentByShipBanLevel(shipBanLevel);
				NKCUtil.SetLabelText(m_lbBanApplyDesc, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_APPLY_DESC_ONE_PARAM, nerfPercentByShipBanLevel));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
			}
		}
	}

	public override void SetData(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, int officeID = 0)
	{
		base.SetData(cNKMUnitData, deckIndex, bEnableLayoutElement, onSelectThisSlot);
		ProcessBanUI();
		if (cNKMUnitData != null)
		{
			NKCUtil.SetLabelText(m_lbTitle, m_NKMUnitTempletBase.GetUnitTitle());
			NKCUtil.SetLabelText(m_lbShipClass, NKCUtilString.GetUnitStyleName(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE));
			if (m_imgShipClass != null)
			{
				m_imgShipClass.sprite = GetClassIcon(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
			}
			if (m_imgShipMove != null)
			{
				m_imgShipMove.sprite = GetSpriteMoveType(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
			}
			m_comStarRank?.SetStarRank(cNKMUnitData);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData.m_UnitID);
			SetSkillSlot(unitTempletBase);
		}
		else
		{
			SetSkillSlot(null);
		}
		NKCUtil.SetGameobjectActive(m_objRootMainUnitMark, bValue: false);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
	}

	public override void SetData(NKMUnitTempletBase templetBase, int levelToDisplay, int skinID, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot)
	{
		base.SetData(templetBase, levelToDisplay, skinID, bEnableLayoutElement, onSelectThisSlot);
		if (templetBase != null)
		{
			NKCUtil.SetLabelText(m_lbTitle, m_NKMUnitTempletBase.GetUnitTitle());
			NKCUtil.SetLabelText(m_lbShipClass, NKCUtilString.GetUnitStyleName(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE));
			if (m_imgShipClass != null)
			{
				Sprite classIcon = GetClassIcon(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
				NKCUtil.SetGameobjectActive(m_imgShipClass, classIcon != null);
				m_imgShipClass.sprite = classIcon;
			}
			if (m_imgShipMove != null)
			{
				Sprite spriteMoveType = GetSpriteMoveType(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
				NKCUtil.SetGameobjectActive(m_imgShipMove, spriteMoveType != null);
				m_imgShipMove.sprite = spriteMoveType;
			}
			m_comStarRank?.SetStarRank(3, 6);
		}
		SetSkillSlot(templetBase);
		NKCUtil.SetGameobjectActive(m_objRootMainUnitMark, bValue: false);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
	}

	public override void SetDataForBan(NKMUnitTempletBase templetBase, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, bool bUp = false, bool bSetOriginalCost = false)
	{
		base.SetData(templetBase, 0, 0, bEnableLayoutElement, onSelectThisSlot);
		ProcessBanUI();
		if (templetBase != null)
		{
			NKCUtil.SetLabelText(m_lbTitle, m_NKMUnitTempletBase.GetUnitTitle());
			NKCUtil.SetLabelText(m_lbShipClass, NKCUtilString.GetUnitStyleName(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE));
			if (m_imgShipClass != null)
			{
				Sprite classIcon = GetClassIcon(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
				NKCUtil.SetGameobjectActive(m_imgShipClass, classIcon != null);
				m_imgShipClass.sprite = classIcon;
			}
			if (m_imgShipMove != null)
			{
				Sprite spriteMoveType = GetSpriteMoveType(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
				NKCUtil.SetGameobjectActive(m_imgShipMove, spriteMoveType != null);
				m_imgShipMove.sprite = spriteMoveType;
			}
			m_comStarRank?.SetStarRank(3, 6);
		}
		SetSkillSlot(templetBase);
		NKCUtil.SetGameobjectActive(m_objRootMainUnitMark, bValue: false);
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
	}

	public override void SetDataForCollection(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, OnSelectThisSlot onSelectThisSlot, bool bEnable = false)
	{
		base.SetData(cNKMUnitData, deckIndex, bEnableLayoutElement: true, onSelectThisSlot);
		if (cNKMUnitData != null)
		{
			NKCUtil.SetLabelText(m_lbTitle, m_NKMUnitTempletBase.GetUnitTitle());
			NKCUtil.SetLabelText(m_lbShipClass, NKCUtilString.GetUnitStyleName(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE));
			if (m_imgShipClass != null)
			{
				m_imgShipClass.sprite = GetClassIcon(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
			}
			if (m_imgShipMove != null)
			{
				m_imgShipMove.sprite = GetSpriteMoveType(m_NKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE);
			}
			m_comStarRank?.SetStarRank(3, 6);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData.m_UnitID);
			SetSkillSlot(unitTempletBase);
		}
		else
		{
			SetSkillSlot(null);
		}
		SetFierceBattleOtherBossAlreadyUsed(bVal: false);
		NKCUtil.SetGameobjectActive(m_objMaxExp, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootMainUnitMark, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDisableSelectSlot, !bEnable);
		NKCUtil.SetGameobjectActive(m_NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_CARD_COLLECITON_DENIED, !bEnable);
	}

	public override void SetDataForRearm(NKMUnitData unitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, bool bShowEqup = true, bool bShowLevel = false, bool bUnable = false)
	{
	}

	protected override void SetFierceBattleOtherBossAlreadyUsed(bool bVal)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_FIERCE_BATTLE, bVal);
	}

	public void SetMainShipMark()
	{
		NKCUtil.SetGameobjectActive(m_objRootMainUnitMark, bValue: true);
		NKCUtil.SetLabelText(m_lbMainUnitMark, NKCUtilString.GET_STRING_MAIN_SHIP);
		if (m_imgMainUnitMark != null)
		{
			m_imgMainUnitMark.color = m_colMainUnitMark;
		}
	}

	private Sprite GetClassIcon(NKM_UNIT_STYLE_TYPE classType)
	{
		return NKCResourceUtility.GetOrLoadUnitStyleIcon(classType, bSmall: true);
	}

	private Sprite GetSpriteMoveType(NKM_UNIT_STYLE_TYPE type)
	{
		string assetName;
		switch (type)
		{
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT:
			assetName = "NKM_UI_SHIP_SELECT_LIST_SHIP_MOVETYPE_1";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY:
			assetName = "NKM_UI_SHIP_SELECT_LIST_SHIP_MOVETYPE_4";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER:
			assetName = "NKM_UI_SHIP_SELECT_LIST_SHIP_MOVETYPE_2";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL:
			assetName = "NKM_UI_SHIP_SELECT_LIST_SHIP_MOVETYPE_3";
			break;
		default:
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_SHIP_SLOT_CARD_SPRITE", assetName);
	}

	private void SetSkillSlot(NKMUnitTempletBase cNKMShipTemplet)
	{
		if (m_lstSkillSlot == null)
		{
			return;
		}
		if (cNKMShipTemplet != null)
		{
			for (int i = 0; i < m_lstSkillSlot.Count; i++)
			{
				NKCUIShipSkillSlot nKCUIShipSkillSlot = m_lstSkillSlot[i];
				NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(cNKMShipTemplet, i);
				if (shipSkillTempletByIndex != null)
				{
					NKCUtil.SetGameobjectActive(nKCUIShipSkillSlot, bValue: true);
					nKCUIShipSkillSlot.SetData(shipSkillTempletByIndex);
				}
				else
				{
					NKCUtil.SetGameobjectActive(nKCUIShipSkillSlot, bValue: false);
				}
			}
			return;
		}
		foreach (NKCUIShipSkillSlot item in m_lstSkillSlot)
		{
			NKCUtil.SetGameobjectActive(item, bValue: false);
		}
	}

	protected override void RestoreSprite()
	{
		if (m_spSSR == null)
		{
			m_spSSR = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_ship_slot_card_sprite", "NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_SSR");
		}
		if (m_spSR == null)
		{
			m_spSR = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_ship_slot_card_sprite", "NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_SR");
		}
		if (m_spR == null)
		{
			m_spR = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_ship_slot_card_sprite", "NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_R");
		}
		if (m_spN == null)
		{
			m_spN = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_ship_slot_card_sprite", "NKM_UI_SHIP_SELECT_LIST_SHIP_SLOT_N");
		}
	}

	public override void SetRecall(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objRecall, bValue);
	}
}
