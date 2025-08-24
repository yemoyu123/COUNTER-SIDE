using System;
using System.Collections.Generic;
using System.Text;
using ClientPacket.Common;
using NKC.Publisher;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCDeckViewSide : MonoBehaviour
{
	private enum COST_TYPE
	{
		CT_ETERNIUM,
		CT_TICKET,
		CT_INFO
	}

	public delegate void OnConfirm();

	public delegate void OnClickCloseBtn();

	public delegate bool CheckMultiply(bool bMsg);

	public class SkillSlot
	{
		public Image SkillImg;

		public Text SkillLevel;
	}

	public Animator m_animLoading;

	public NKCDeckViewSideUnitIllust m_DeckViewSideUnitIllust;

	public GameObject m_objUnitChanged;

	[Header("오른쪽 아래 시작 버튼")]
	public GameObject m_objStartButtonRoot;

	public GameObject m_objPossibleFX;

	public Image m_imgStartBG;

	public Image m_imgStartArrow;

	public Text m_lbStartTextWithCost;

	public Text m_lbStartTextWithoutCost;

	public GameObject m_objCost;

	public GameObject m_objCostBG;

	public GameObject m_objSuccessRate;

	public Text m_lbSuccessRate;

	public NKCUIComResourceButton m_ResourceButton;

	public Text m_lbMultiSelectCount;

	public GameObject m_objDisableBtnExtraMsg;

	private bool m_bDisableButtonExtraMsg;

	public Color m_colMainText;

	public Color m_colMainTextDisable;

	public Color m_colArrow;

	public Color m_colArrowDisable;

	public Color m_colCost;

	public Color m_colCostDisable;

	public Sprite m_spStartButtonBG;

	public Sprite m_spStartButtonBGDisable;

	[Header("왼쪽에 닫기 버튼")]
	public GameObject m_objCloseBtn;

	public NKCUIComStateButton m_csbtnClose;

	[Header("밴 관련")]
	public GameObject m_objBan;

	public Text m_lbBanLevel;

	[Header("스킵")]
	public GameObject m_objSkip;

	public NKCUIOperationSkip m_NKCUIOperationSkip;

	public NKCUIComToggle m_tglSkip;

	private NKM_ERROR_CODE m_eButtonDisableReason;

	private OnConfirm dOnConfirm;

	private OnClickCloseBtn dOnClickCloseBtn;

	private CheckMultiply dCheckMultiply;

	private bool m_berrorCodePopupMsg;

	private NKCUIDeckViewer.DeckViewerMode m_DeckViewerMode;

	private NKMDeckIndex m_SelectDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, 0);

	private int m_CurrMultiplyRewardCount = 1;

	private int m_costItemID;

	private int m_costItemCount;

	private bool m_bOperationSkip;

	public const int MAX_COUNT_MULTIPLY_AND_SKIP = 99;

	[Header("유닛정보 텍스트")]
	public Text m_lbUnitInfo;

	[Header("장비 표시")]
	[FormerlySerializedAs("m_NKM_DECK_VIEW_SIDE_UNIT_ILLUST_SUB_MENU_INFO_3")]
	public GameObject m_objUnitEquip;

	public Sprite m_spEquipLock;

	public Sprite m_spEquipEmpty;

	public Sprite m_spEquipN;

	public Sprite m_spEquipR;

	public Sprite m_spEquipSR;

	public Sprite m_spEquipSSR;

	public Sprite m_spEquipReactor;

	public Image m_imgEquipWeapon;

	public Image m_imgEquipArmor;

	public Image m_imgEquipAcc;

	public Image m_imgEquipAcc2;

	public Image m_imgEquipReactor;

	public GameObject m_EQUIP_1_SET;

	public GameObject m_EQUIP_2_SET;

	public GameObject m_EQUIP_3_SET;

	public GameObject m_EQUIP_4_SET;

	public GameObject m_EQUIP_1_Changed;

	public GameObject m_EQUIP_2_Changed;

	public GameObject m_EQUIP_3_Changed;

	public GameObject m_EQUIP_4_Changed;

	public GameObject m_EQUIP_5_Changed;

	[Header("스킬")]
	public NKCUIComStateButton m_csbtnSkillInfo;

	[Header("전술 업데이트")]
	public NKCUITacticUpdateLevelSlot m_tacticUpdateLvSlot;

	private NKMUnitData m_UnitData;

	private NKMOperator m_OperatorData;

	[Header("정보창 호출")]
	public NKCUIComButton m_cbtn_NKM_DECK_VIEW_SIDE_UNIT_ILLUST_SUB_MENU_INFO_TEXT;

	private List<NKMShipSkillTemplet> m_lstShipSkillTemplet = new List<NKMShipSkillTemplet>();

	private string m_unitName = "";

	private int m_unitStarGradeMax;

	private int m_unitLimitBreakLevel;

	private int m_CurUnitID;

	private long m_CurUnitUID;

	public bool OperationSkip => m_bOperationSkip;

	public int GetCurrMultiplyRewardCount()
	{
		return m_CurrMultiplyRewardCount;
	}

	public NKCDeckViewSideUnitIllust GetDeckViewSideUnitIllust()
	{
		return m_DeckViewSideUnitIllust;
	}

	private NKMStageTempletV2 GetStageTemplet()
	{
		if (NKCUIDeckViewer.IsDungeonAtkReadyScen(m_DeckViewerMode))
		{
			return NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetStageTemplet();
		}
		return null;
	}

	public void Init(NKCDeckViewSideUnitIllust.OnUnitInfoClick onUnitInfo, NKCDeckViewSideUnitIllust.OnUnitChangeClick onUnitChange, NKCDeckViewSideUnitIllust.OnLeaderChange onLeaderChange, OnConfirm onConfirm, OnClickCloseBtn onClickCloseBtn, CheckMultiply checkMultiply)
	{
		m_DeckViewSideUnitIllust.Init(onUnitInfo, onUnitChange, onLeaderChange, m_animLoading);
		dOnConfirm = onConfirm;
		dOnClickCloseBtn = onClickCloseBtn;
		dCheckMultiply = checkMultiply;
		NKCUtil.SetBindFunction(m_ResourceButton, OnBtnConfirm);
		NKCUtil.SetBindFunction(m_csbtnClose, OnBtnClose);
		m_ResourceButton?.SetHotkey(HotkeyEventType.Confirm);
		NKCUtil.SetToggleValueChangedDelegate(m_tglSkip, OnClickSkip);
		NKCUtil.SetButtonClickDelegate(m_csbtnSkillInfo, OpenSkillInfoPopup);
		m_CurrMultiplyRewardCount = 1;
		if (m_NKCUIOperationSkip != null)
		{
			m_NKCUIOperationSkip.Init(OnOperationSkipUpdated, OnClickOperationSkipClose);
		}
		NKCUtil.SetBindFunction(m_cbtn_NKM_DECK_VIEW_SIDE_UNIT_ILLUST_SUB_MENU_INFO_TEXT, OpenInfo);
		NKCUtil.SetHotkey(m_tglSkip, HotkeyEventType.RotateLeft, null, bUpDownEvent: true);
		if (NKCPublisherModule.Instance.IsReviewServer() && m_animLoading != null)
		{
			NKCUtil.SetGameobjectActive(m_animLoading.gameObject, bValue: false);
		}
	}

	private bool IsCanStartEterniumDungeon()
	{
		if (NKCUIDeckViewer.IsDungeonAtkReadyScen(m_DeckViewerMode))
		{
			NKC_SCEN_DUNGEON_ATK_READY sCEN_DUNGEON_ATK_READY = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY();
			if (sCEN_DUNGEON_ATK_READY != null)
			{
				return NKCUtil.IsCanStartEterniumStage(sCEN_DUNGEON_ATK_READY.GetStageTemplet(), bCallLackPopup: true);
			}
		}
		return true;
	}

	public void OnBtnConfirm()
	{
		if (m_eButtonDisableReason == NKM_ERROR_CODE.NEC_OK)
		{
			if ((m_DeckViewerMode == NKCUIDeckViewer.DeckViewerMode.AsyncPvpDefenseDeck || m_DeckViewerMode == NKCUIDeckViewer.DeckViewerMode.PvPBattleFindTarget || m_DeckViewerMode == NKCUIDeckViewer.DeckViewerMode.UnlimitedDeck) && !NKMArmyData.IsAllUnitsEquipedAllGears(m_SelectDeckIndex))
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_GAUNTLET_DECK_UNIT_NOT_ALL_EQUIPED_GEAR_DESC, delegate
				{
					if (dOnConfirm != null)
					{
						dOnConfirm();
					}
				});
			}
			else if (dOnConfirm != null)
			{
				dOnConfirm();
			}
		}
		else if (!m_berrorCodePopupMsg)
		{
			NKCPopupOKCancel.OpenOKBox(m_eButtonDisableReason);
		}
		else
		{
			NKCPopupMessageManager.AddPopupMessage(m_eButtonDisableReason);
		}
	}

	public void OnBtnClose()
	{
		if (dOnClickCloseBtn != null)
		{
			dOnClickCloseBtn();
		}
	}

	public void SetAttackCost(int itemID, int itemCount)
	{
		if (m_ResourceButton != null)
		{
			m_ResourceButton.SetData(itemID, itemCount);
		}
	}

	public void SetMultiSelectedCount(int count, int maxCount)
	{
		NKCUtil.SetLabelText(m_lbMultiSelectCount, count + " / " + maxCount);
	}

	private void ResetUIByScen(NKCUIDeckViewer.DeckViewerOption sDeckViewerOption, bool bUseCost)
	{
		bool flag = true;
		bool bValue = false;
		bool flag2 = false;
		m_bDisableButtonExtraMsg = false;
		bool bValue2 = false;
		bool enableControlLeaderBtn = true;
		bool flag3 = bUseCost;
		switch (sDeckViewerOption.eDeckviewerMode)
		{
		case NKCUIDeckViewer.DeckViewerMode.PrepareRaid:
			flag = false;
			bValue2 = true;
			break;
		case NKCUIDeckViewer.DeckViewerMode.GuildCoopBoss:
			flag = false;
			bValue2 = true;
			break;
		case NKCUIDeckViewer.DeckViewerMode.PrepareDungeonBattleWithoutCost:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = false;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_START;
			break;
		case NKCUIDeckViewer.DeckViewerMode.PrepareDungeonBattle_CC:
			flag = true;
			bUseCost = false;
			flag3 = true;
			bValue = false;
			m_lbStartTextWithCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_START;
			break;
		case NKCUIDeckViewer.DeckViewerMode.PrepareDungeonBattle_Daily:
			flag = true;
			bUseCost = true;
			flag3 = true;
			bValue = false;
			m_lbStartTextWithCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_START;
			break;
		case NKCUIDeckViewer.DeckViewerMode.PrepareDungeonBattle:
			flag = true;
			bUseCost = true;
			flag3 = true;
			bValue = false;
			m_lbStartTextWithCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_START;
			break;
		case NKCUIDeckViewer.DeckViewerMode.PrepareBattle:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = false;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_START;
			break;
		case NKCUIDeckViewer.DeckViewerMode.WorldMapMissionDeckSelect:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = true;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_OK;
			m_bDisableButtonExtraMsg = true;
			break;
		case NKCUIDeckViewer.DeckViewerMode.DeckSelect:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = false;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_OK;
			break;
		case NKCUIDeckViewer.DeckViewerMode.DeckMultiSelect:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = false;
			flag2 = true;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_SELECT;
			break;
		case NKCUIDeckViewer.DeckViewerMode.WarfareBatch:
			flag = true;
			bUseCost = false;
			flag3 = true;
			bValue = false;
			m_lbStartTextWithCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_BATCH;
			break;
		case NKCUIDeckViewer.DeckViewerMode.WarfareBatch_Assault:
			flag = true;
			bUseCost = false;
			flag3 = true;
			bValue = false;
			m_lbStartTextWithCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_BATCH;
			m_bDisableButtonExtraMsg = true;
			break;
		case NKCUIDeckViewer.DeckViewerMode.WarfareRecovery:
			flag = true;
			flag3 = true;
			bValue = false;
			m_lbStartTextWithCost.text = NKCUtilString.GET_STRING_WARFARE_RECOVERY;
			break;
		case NKCUIDeckViewer.DeckViewerMode.PvPBattleFindTarget:
		case NKCUIDeckViewer.DeckViewerMode.UnlimitedDeck:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = false;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_PVP;
			break;
		case NKCUIDeckViewer.DeckViewerMode.AsyncPvPBattleStart:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = false;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_ACTION;
			break;
		case NKCUIDeckViewer.DeckViewerMode.AsyncPvpDefenseDeck:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = false;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_ACTION;
			break;
		case NKCUIDeckViewer.DeckViewerMode.MainDeckSelect:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = false;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_ACTION;
			break;
		case NKCUIDeckViewer.DeckViewerMode.PrivatePvPReady:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = false;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_ACTION;
			break;
		case NKCUIDeckViewer.DeckViewerMode.LeaguePvPMain:
			flag = true;
			bUseCost = false;
			flag3 = false;
			bValue = false;
			m_lbStartTextWithCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_OK;
			break;
		case NKCUIDeckViewer.DeckViewerMode.PrepareLocalDeck:
			flag = true;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_OK;
			break;
		case NKCUIDeckViewer.DeckViewerMode.TournamentApply:
			flag = true;
			m_lbStartTextWithoutCost.text = NKCUtilString.GET_STRING_DECK_BUTTON_OK;
			break;
		default:
			flag = false;
			bValue = false;
			break;
		}
		if (bUseCost && sDeckViewerOption.CostItemCount > 0)
		{
			SetAttackCost(sDeckViewerOption.CostItemID, sDeckViewerOption.CostItemCount * m_CurrMultiplyRewardCount);
			m_costItemID = sDeckViewerOption.CostItemID;
			m_costItemCount = sDeckViewerOption.CostItemCount;
		}
		else
		{
			bUseCost = false;
			m_costItemID = 0;
			m_costItemCount = 0;
		}
		NKMStageTempletV2 stageTemplet = GetStageTemplet();
		bool flag4 = false;
		if (stageTemplet != null && stageTemplet.EnterLimit > 0)
		{
			int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(stageTemplet.Key);
			if (stageTemplet.EnterLimit - statePlayCnt <= 0 && stageTemplet.RestoreReqItem != null)
			{
				bUseCost = true;
				flag3 = true;
				flag4 = true;
				m_CurrMultiplyRewardCount = 1;
				m_costItemID = stageTemplet.RestoreReqItem.ItemId;
				m_costItemCount = stageTemplet.RestoreReqItem.Count32;
				m_lbStartTextWithCost.text = NKCUtilString.GET_STRING_WARFARE_GAME_HUD_OPERATION_RESTORE;
				UpdateAttackCost();
			}
		}
		if (flag4)
		{
			NKCUtil.SetImageSprite(m_imgStartArrow, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_ENTERLIMIT_RECOVER_SMALL"));
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgStartArrow, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_GAUNTLET"));
		}
		NKCUtil.SetGameobjectActive(m_objSuccessRate, bValue);
		NKCUtil.SetGameobjectActive(m_objStartButtonRoot, flag);
		NKCUtil.SetGameobjectActive(m_objCost, bUseCost || flag2);
		NKCUtil.SetGameobjectActive(m_objCostBG, bUseCost || flag2);
		NKCUtil.SetGameobjectActive(m_lbStartTextWithCost, flag3);
		NKCUtil.SetGameobjectActive(m_lbStartTextWithoutCost, !flag3);
		NKCUtil.SetGameobjectActive(m_lbMultiSelectCount, flag2);
		if (m_ResourceButton != null)
		{
			m_ResourceButton.OnShow(bUseCost);
		}
		NKCUtil.SetGameobjectActive(m_objCloseBtn, bValue2);
		NKCUtil.SetGameobjectActive(m_NKCUIOperationSkip, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSkip, sDeckViewerOption.bUsableOperationSkip);
		if (sDeckViewerOption.bUsableOperationSkip)
		{
			m_tglSkip.Select(bSelect: false);
		}
		if (sDeckViewerOption.bNoUseLeaderBtn)
		{
			enableControlLeaderBtn = false;
			NKCUtil.SetGameobjectActive(m_DeckViewSideUnitIllust.m_cbtnLeader, bValue: false);
		}
		m_DeckViewSideUnitIllust.SetEnableControlLeaderBtn(enableControlLeaderBtn);
	}

	public void Open(NKCUIDeckViewer.DeckViewerOption sDeckViewerOption, bool bInit, bool bUseCost)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		m_DeckViewerMode = sDeckViewerOption.eDeckviewerMode;
		m_SelectDeckIndex = sDeckViewerOption.DeckIndex;
		m_CurrMultiplyRewardCount = 1;
		m_bOperationSkip = false;
		m_DeckViewSideUnitIllust.Open(sDeckViewerOption.eDeckviewerMode, bInit);
		ResetUIByScen(sDeckViewerOption, bUseCost);
	}

	public void ChangeDeckIndex(NKMDeckIndex deckIdx)
	{
		m_SelectDeckIndex = deckIdx;
	}

	public void SetEnableButtons(NKM_ERROR_CODE errorCode)
	{
		m_eButtonDisableReason = errorCode;
		bool flag = errorCode == NKM_ERROR_CODE.NEC_OK;
		NKCUtil.SetGameobjectActive(m_objPossibleFX, flag);
		m_imgStartBG.sprite = (flag ? m_spStartButtonBG : m_spStartButtonBGDisable);
		m_imgStartArrow.color = (flag ? m_colArrow : m_colArrowDisable);
		m_lbStartTextWithCost.color = (flag ? m_colMainText : m_colMainTextDisable);
		m_lbStartTextWithoutCost.color = (flag ? m_colMainText : m_colMainTextDisable);
		if (m_ResourceButton != null)
		{
			if (!flag)
			{
				m_ResourceButton.SetTextColor(m_colCostDisable);
			}
			else
			{
				m_ResourceButton.SetTextColor(m_colCost);
				UpdateAttackCost();
			}
		}
		NKCUtil.SetGameobjectActive(m_objDisableBtnExtraMsg, m_bDisableButtonExtraMsg && errorCode == NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_ASSAULT_POSITION);
		if (errorCode == NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_DECK_HAS_UNIT_FROM_ANOTHER_CITY || errorCode == NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_ASSAULT_POSITION)
		{
			m_berrorCodePopupMsg = m_bDisableButtonExtraMsg;
		}
		else
		{
			m_berrorCodePopupMsg = false;
		}
	}

	public void Close()
	{
		m_DeckViewSideUnitIllust.Close();
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void PlayLoadingAnim(string name)
	{
		if (!(m_animLoading == null) && m_animLoading.gameObject.activeInHierarchy)
		{
			m_animLoading.Play(name, -1, 0f);
		}
	}

	public void SetSuccessRate(int number, bool bDeckReady)
	{
		if (bDeckReady)
		{
			NKCUtil.SetLabelText(m_lbSuccessRate, string.Format(NKCUtilString.GET_STRING_DECK_SUCCESS_RATE_ONE_PARAM, number));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSuccessRate, NKCUtilString.GET_STRING_DECK_CANNOT_START);
		}
	}

	protected void SetEquipListData(NKMUnitData unitData)
	{
		NKCUtil.SetGameobjectActive(m_objUnitChanged, bValue: false);
		NKCUtil.SetGameobjectActive(m_EQUIP_1_Changed, bValue: false);
		NKCUtil.SetGameobjectActive(m_EQUIP_2_Changed, bValue: false);
		NKCUtil.SetGameobjectActive(m_EQUIP_3_Changed, bValue: false);
		NKCUtil.SetGameobjectActive(m_EQUIP_4_Changed, bValue: false);
		NKCUtil.SetGameobjectActive(m_EQUIP_5_Changed, bValue: false);
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
			switch (position)
			{
			case ITEM_EQUIP_POSITION.IEP_WEAPON:
				NKCUtil.SetGameobjectActive(m_EQUIP_1_SET, bValue: false);
				break;
			case ITEM_EQUIP_POSITION.IEP_DEFENCE:
				NKCUtil.SetGameobjectActive(m_EQUIP_2_SET, bValue: false);
				break;
			case ITEM_EQUIP_POSITION.IEP_ACC:
				NKCUtil.SetGameobjectActive(m_EQUIP_3_SET, bValue: false);
				break;
			case ITEM_EQUIP_POSITION.IEP_ACC2:
				NKCUtil.SetGameobjectActive(m_EQUIP_4_SET, bValue: false);
				break;
			}
			return;
		}
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(equipUid);
		if (itemEquip == null)
		{
			Debug.LogError($"equipped equip not exist. uid {equipUid}");
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
		switch (position)
		{
		case ITEM_EQUIP_POSITION.IEP_WEAPON:
			NKCUtil.SetGameobjectActive(m_EQUIP_1_SET, NKMItemManager.IsActiveSetOptionItem(itemEquip));
			break;
		case ITEM_EQUIP_POSITION.IEP_DEFENCE:
			NKCUtil.SetGameobjectActive(m_EQUIP_2_SET, NKMItemManager.IsActiveSetOptionItem(itemEquip));
			break;
		case ITEM_EQUIP_POSITION.IEP_ACC:
			NKCUtil.SetGameobjectActive(m_EQUIP_3_SET, NKMItemManager.IsActiveSetOptionItem(itemEquip));
			break;
		case ITEM_EQUIP_POSITION.IEP_ACC2:
			NKCUtil.SetGameobjectActive(m_EQUIP_4_SET, NKMItemManager.IsActiveSetOptionItem(itemEquip));
			break;
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

	public void UpdateUnitData(NKMUnitData unitData)
	{
		if (m_UnitData != null && m_UnitData.m_UnitUID == unitData.m_UnitUID)
		{
			SetUnitData(unitData, bForce: true);
		}
	}

	public void SetUnitData(NKMUnitData unitData, bool bForce = false)
	{
		if ((bForce || m_UnitData != unitData) && unitData != null)
		{
			m_UnitData = unitData;
			UpdateRoleSlot();
			UpdateSkillSlot();
			UpdateBanUI(unitData);
			SetEquipListData(unitData);
		}
	}

	public void SetOperatorData(NKMOperator operatorData, bool bForce = false)
	{
		if ((bForce || m_OperatorData != operatorData) && operatorData != null)
		{
			m_UnitData = null;
			m_OperatorData = operatorData;
			NKCUtil.SetLabelText(m_lbUnitInfo, " ");
			NKCUtil.SetGameobjectActive(m_tacticUpdateLvSlot.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitEquip, bValue: false);
			UpdateOperatorSkillSlot();
			UpdateBanUI(m_OperatorData);
			SetEquipListData(null);
		}
	}

	private void UpdateBanUI(NKMUnitData unitData)
	{
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		if (unitData == null)
		{
			return;
		}
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitData.m_UnitID);
		if (nKMUnitTempletBase != null)
		{
			if (NKCUtil.CheckPossibleShowBan(m_DeckViewerMode) && NKCBanManager.IsBanUnitByUTB(nKMUnitTempletBase))
			{
				int unitBanLevelByUTB = NKCBanManager.GetUnitBanLevelByUTB(nKMUnitTempletBase);
				NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, unitBanLevelByUTB));
				m_lbBanLevel.color = Color.red;
				NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
			}
			else if (NKCUtil.CheckPossibleShowUpUnit(m_DeckViewerMode) && NKCBanManager.IsUpUnitByUTB(nKMUnitTempletBase))
			{
				int unitUpLevelByUTB = NKCBanManager.GetUnitUpLevelByUTB(nKMUnitTempletBase);
				NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_UP_LEVEL_ONE_PARAM, unitUpLevelByUTB));
				m_lbBanLevel.color = NKCBanManager.UP_COLOR;
				NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
			}
		}
	}

	private void UpdateBanUI(NKMOperator operData)
	{
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		if (operData != null)
		{
			if (NKCUtil.CheckPossibleShowBan(m_DeckViewerMode) && NKCBanManager.IsBanOperator(operData.id))
			{
				NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, NKCBanManager.GetOperBanLevel(operData.id)));
				m_lbBanLevel.color = Color.red;
				NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
			}
		}
	}

	private void UpdateRoleSlot()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitData.m_UnitID);
		if (unitTempletBase != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(NKCUtilString.GetUnitStyleName(unitTempletBase.m_NKM_UNIT_STYLE_TYPE));
			if (unitTempletBase.m_NKM_UNIT_STYLE_TYPE_SUB != NKM_UNIT_STYLE_TYPE.NUST_INVALID)
			{
				stringBuilder.Append("·");
				stringBuilder.Append(NKCUtilString.GetUnitStyleName(unitTempletBase.m_NKM_UNIT_STYLE_TYPE_SUB));
			}
			NKCUtil.SetGameobjectActive(m_objUnitEquip, unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL);
			if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP || unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
			{
				NKCUtil.SetLabelText(m_lbUnitInfo, stringBuilder.ToString());
				return;
			}
			stringBuilder.Append("·");
			stringBuilder.Append(NKCUtilString.GetRoleText(unitTempletBase));
			NKCUtil.SetLabelText(m_lbUnitInfo, stringBuilder.ToString());
		}
	}

	private void UpdateSkillSlot()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitData.m_UnitID);
		if (unitTempletBase != null)
		{
			m_lstShipSkillTemplet.Clear();
			m_unitName = unitTempletBase.GetUnitName();
			m_tacticUpdateLvSlot.SetLevel(m_UnitData.tacticLevel);
			NKCUtil.SetGameobjectActive(m_tacticUpdateLvSlot.gameObject, unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL);
			switch (unitTempletBase.m_NKM_UNIT_TYPE)
			{
			default:
				return;
			case NKM_UNIT_TYPE.NUT_NORMAL:
				NKCUtil.SetGameobjectActive(m_csbtnSkillInfo, unitTempletBase.GetSkillCount() > 0);
				m_unitStarGradeMax = unitTempletBase.m_StarGradeMax;
				m_unitLimitBreakLevel = m_UnitData.m_LimitBreakLevel;
				m_CurUnitID = m_UnitData.m_UnitID;
				m_CurUnitUID = m_UnitData.m_UnitUID;
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
				m_CurUnitID = m_UnitData.m_UnitID;
				m_CurUnitUID = m_UnitData.m_UnitUID;
				NKCUtil.SetGameobjectActive(m_csbtnSkillInfo, unitTempletBase.GetSkillCount() > 0);
				break;
			}
			NKCUtil.SetGameobjectActive(m_lbUnitInfo, unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL);
		}
	}

	private void UpdateOperatorSkillSlot()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_OperatorData.id);
		if (unitTempletBase != null)
		{
			m_lstShipSkillTemplet.Clear();
			m_unitName = unitTempletBase.GetUnitName();
			m_CurUnitID = m_OperatorData.id;
			m_CurUnitUID = m_OperatorData.uid;
			if (NKCUtil.CheckPossibleShowBan(m_DeckViewerMode))
			{
				NKCBanManager.IsBanOperator(m_OperatorData.id);
			}
			else
				_ = 0;
			NKCUtil.SetGameobjectActive(m_csbtnSkillInfo, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbUnitInfo, bValue: false);
		}
	}

	public void OpenSkillInfoPopup()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_CurUnitID);
		if (unitTempletBase != null)
		{
			switch (unitTempletBase.m_NKM_UNIT_TYPE)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				NKCPopupSkillFullInfo.UnitInstance.OpenForUnit(m_UnitData, m_unitName, m_unitStarGradeMax, m_unitLimitBreakLevel, unitTempletBase.IsRearmUnit);
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
				NKCPopupSkillFullInfo.ShipInstance.OpenForShip(m_CurUnitID, m_CurUnitUID);
				break;
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				NKCUIOperatorPopUpSkill.Instance.Open(m_CurUnitUID);
				break;
			}
		}
	}

	private void OpenInfo()
	{
		if (m_UnitData != null)
		{
			NKCPopupUnitRoleInfo.Instance.OpenPopup(m_UnitData);
		}
	}

	private void OnClickSkip(bool bSet)
	{
		if (bSet)
		{
			if ((dCheckMultiply != null && !dCheckMultiply(bMsg: true)) || !IsCanStartEterniumDungeon())
			{
				m_tglSkip.Select(bSelect: false);
				return;
			}
			m_bOperationSkip = true;
			UpdateAttackCost();
			SetSkipCountUIData();
		}
		NKCUtil.SetGameobjectActive(m_NKCUIOperationSkip, bSet);
		if (!bSet)
		{
			m_CurrMultiplyRewardCount = 1;
			m_bOperationSkip = false;
			UpdateAttackCost();
			SetSkipCountUIData();
		}
	}

	private void OnOperationSkipUpdated(int newCount)
	{
		m_CurrMultiplyRewardCount = newCount;
		UpdateAttackCost();
	}

	private void OnClickOperationSkipClose()
	{
		m_tglSkip.Select(bSelect: false);
	}

	private void SetSkipCountUIData()
	{
		NKMStageTempletV2 stageTemplet = GetStageTemplet();
		m_NKCUIOperationSkip.SetData(stageTemplet, m_CurrMultiplyRewardCount, bShowUseDeckTypeNotice: false);
	}

	public void UpdateAttackCost()
	{
		SetAttackCost(m_costItemID, m_costItemCount * m_CurrMultiplyRewardCount);
	}

	public void UpdateCostUI(NKMItemMiscData itemData)
	{
		if (itemData.ItemID == m_costItemID)
		{
			UpdateAttackCost();
			SetSkipCountUIData();
		}
	}

	public void CheckUnitChanged(NKMAsyncUnitData lastUnitData)
	{
		if (m_UnitData == null || lastUnitData == null || m_UnitData.m_UnitUID != lastUnitData.unitUid)
		{
			NKCUtil.SetGameobjectActive(m_objUnitChanged, bValue: true);
			NKCUtil.SetGameobjectActive(m_EQUIP_1_Changed, bValue: false);
			NKCUtil.SetGameobjectActive(m_EQUIP_2_Changed, bValue: false);
			NKCUtil.SetGameobjectActive(m_EQUIP_3_Changed, bValue: false);
			NKCUtil.SetGameobjectActive(m_EQUIP_4_Changed, bValue: false);
			NKCUtil.SetGameobjectActive(m_EQUIP_5_Changed, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objUnitChanged, NKCTournamentManager.IsUnitChanged(lastUnitData, m_UnitData));
			for (int i = 0; i < lastUnitData.equipUids.Count; i++)
			{
				bool bValue = NKCTournamentManager.IsEquipChanged(lastUnitData.equipUids[i], m_UnitData.EquipItemUids[i]);
				SetEquipChanged(i, bValue);
			}
			NKCUtil.SetGameobjectActive(m_EQUIP_5_Changed, lastUnitData.reactorLevel != m_UnitData.reactorLevel);
		}
	}

	private void SetEquipChanged(int idx, bool bValue)
	{
		switch (idx)
		{
		case 0:
			NKCUtil.SetGameobjectActive(m_EQUIP_1_Changed, bValue);
			break;
		case 1:
			NKCUtil.SetGameobjectActive(m_EQUIP_2_Changed, bValue);
			break;
		case 2:
			NKCUtil.SetGameobjectActive(m_EQUIP_3_Changed, bValue);
			break;
		case 3:
			NKCUtil.SetGameobjectActive(m_EQUIP_4_Changed, bValue);
			break;
		case 4:
			NKCUtil.SetGameobjectActive(m_EQUIP_5_Changed, bValue);
			break;
		}
	}
}
