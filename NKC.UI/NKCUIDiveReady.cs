using System;
using System.Collections.Generic;
using ClientPacket.Mode;
using NKC.UI.Guide;
using NKC.UI.NPC;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIDiveReady : NKCUIBase
{
	public class CompAscendDiveTemplet : IComparer<NKMDiveTemplet>
	{
		public int Compare(NKMDiveTemplet x, NKMDiveTemplet y)
		{
			if (y.IndexID <= x.IndexID)
			{
				return 1;
			}
			return -1;
		}
	}

	[Header("배경")]
	public GameObject m_NKM_UI_DIVE_BG_NORMAL;

	public GameObject m_NKM_UI_DIVE_BG_HURDLE;

	[Header("왼쪽")]
	public NKCUINPCOperatorChloe m_UINPCOperatorChloe;

	public GameObject m_objDiveReset;

	public NKCUIComStateButton m_csbtnReset;

	public Text m_lbRemainTimeToReset;

	public Image m_imgResetCostIcon;

	public Text m_lbResetCost;

	[Header("가운데 다이브 슬롯")]
	public LoopScrollRect m_LoopScrollRect;

	public GameObject m_NKM_UI_DIVE_LIST_Content;

	private List<NKCUIDiveReadySlot> m_lstNKCUIDiveReadySlot = new List<NKCUIDiveReadySlot>();

	private Stack<NKCUIDiveReadySlot> m_stkNKCUIDiveReadySlot = new Stack<NKCUIDiveReadySlot>();

	[Header("오른쪽 다이브 정보")]
	public GameObject m_NKM_UI_DIVE_INFO;

	public Animator m_NKM_UI_DIVE_INFO_Animator;

	public Text m_NKM_UI_DIVE_INFO_TITLE_TEXT;

	public Text m_NKM_UI_DIVE_INFO_SUBTITLE_TEXT;

	public GameObject m_NKM_UI_DIVE_INFO_CLEARED_ICON;

	public Text m_NKM_UI_DIVE_INFO_LEVEL_COUNT;

	public Text m_NKM_UI_DIVE_INFO_AREA_COUNT;

	public GameObject m_NKM_UI_DIVE_INFO_DENIED;

	public GameObject m_NKM_UI_DIVE_INFO_CLEARED;

	public GameObject m_NKM_UI_DIVE_INFO_DIVE_BUTTON_FX;

	public GameObject m_BG_HURDLE;

	public GameObject m_BG_NORMAL;

	public GameObject m_NKM_UI_DIVE_INFO_SQUAD_TEXT;

	public Text m_NKM_UI_DIVE_INFO_FIRST_REWARD_TEXT;

	public List<NKCUIDiveReadySquadSlot> m_lstNKCUIDiveReadySquadSlot = new List<NKCUIDiveReadySquadSlot>();

	public NKCUIComStateButton m_csbtnDeckEdit;

	public GameObject m_NKM_UI_DIVE_INFO_SQUAD_LIST;

	public GameObject m_NKM_UI_DIVE_INFO_REWARD_CONTENT;

	public Text m_NKM_UI_DIVE_INFO_DIVE_BUTTON_COUNT;

	public Image m_NKM_UI_DIVE_INFO_DIVE_BUTTON_ICON;

	public NKCUIComButton m_NKM_UI_DIVE_INFO_DIVE_BUTTON;

	public Text m_NKM_UI_DIVE_INFO_DIVE_BUTTON_TEXT;

	public NKCUIComButton m_NKM_UI_DIVE_INFO_REDIVE_BUTTON;

	public Text m_lbLocked;

	public NKCUIOperationSkip m_operationSkip;

	[Header("강습전")]
	public GameObject m_objJumpToggle;

	public NKCUIComToggle m_tglJump;

	public GameObject m_objJumpEvent;

	public GameObject m_objJumpPopup;

	public NKCUIComStateButton m_btnJumpPopupInfo;

	public NKCUIComStateButton m_btnJumpPopupClose;

	public NKCUIComStateButton m_btnJumpPlusInfo;

	public NKCUIComStateButton m_btnJumpDiscountInfo;

	public NKCUIItemCostSlot m_slotJumpBasic;

	public NKCUIItemCostSlot m_slotJumpPlus;

	public NKCUIItemCostSlot m_slotJumpDiscount;

	private int m_SelectedIndex = -1;

	private bool m_bFirstOpen = true;

	private int m_cityID;

	private NKMDiveTemplet m_eventDiveTemplet;

	private IReadOnlyList<NKMDiveTemplet> m_lstDiveTemplet;

	private List<NKCUISlot> m_lstNKCUISlot = new List<NKCUISlot>();

	private float m_fPrevUpdateTime;

	private DateTime m_DiveResetTicketChargeDate;

	private List<int> m_upsideMenuResourceList = new List<int>();

	public override string MenuName => NKCUtilString.GET_STRING_DIVE_READY;

	public override List<int> UpsideMenuShowResourceList => m_upsideMenuResourceList;

	public override string GuideTempletID => "ARTICLE_DIVE_INFO";

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	private bool JumpSelected
	{
		get
		{
			if (m_tglJump != null && m_tglJump.gameObject.activeInHierarchy)
			{
				return m_tglJump.m_bSelect;
			}
			return false;
		}
	}

	private void SetBG(bool bHurdle = false)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_BG_NORMAL, !bHurdle);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_BG_HURDLE, bHurdle);
	}

	private void SetDiveInfoBG(bool bHurdle = false)
	{
		NKCUtil.SetGameobjectActive(m_BG_NORMAL, !bHurdle);
		NKCUtil.SetGameobjectActive(m_BG_HURDLE, bHurdle);
	}

	private void Update()
	{
		if (base.IsOpen && m_fPrevUpdateTime + 1f < Time.time)
		{
			m_fPrevUpdateTime = Time.time;
			UpdateDiveResetUIOnlyTime(m_DiveResetTicketChargeDate);
		}
	}

	private NKMDiveTemplet GetSelectedDiveTemplet()
	{
		if (m_eventDiveTemplet != null)
		{
			if (m_SelectedIndex >= 0)
			{
				return m_eventDiveTemplet;
			}
			return null;
		}
		if (m_SelectedIndex >= 0 && m_lstDiveTemplet.Count > m_SelectedIndex)
		{
			return m_lstDiveTemplet[m_SelectedIndex];
		}
		return null;
	}

	public static NKCUIDiveReady InitUI()
	{
		NKCUIDiveReady nKCUIDiveReady = NKCUIManager.OpenUI<NKCUIDiveReady>("NKM_UI_DIVE");
		if (nKCUIDiveReady != null)
		{
			if ((bool)nKCUIDiveReady.gameObject)
			{
				nKCUIDiveReady.gameObject.SetActive(value: false);
			}
			nKCUIDiveReady.m_LoopScrollRect.dOnGetObject += nKCUIDiveReady.GetDiveReadySlot;
			nKCUIDiveReady.m_LoopScrollRect.dOnReturnObject += nKCUIDiveReady.ReturnDiveReadySlot;
			nKCUIDiveReady.m_LoopScrollRect.dOnProvideData += nKCUIDiveReady.ProvideDiveReadySlotData;
			NKCUtil.SetScrollHotKey(nKCUIDiveReady.m_LoopScrollRect);
			int num = 0;
			for (num = 0; num < nKCUIDiveReady.m_lstNKCUIDiveReadySquadSlot.Count; num++)
			{
				NKCUIDiveReadySquadSlot nKCUIDiveReadySquadSlot = nKCUIDiveReady.m_lstNKCUIDiveReadySquadSlot[num];
				nKCUIDiveReadySquadSlot.SetSelectedEvent(nKCUIDiveReady.OnSelectedSquad);
				NKCUtil.SetGameobjectActive(nKCUIDiveReadySquadSlot, bValue: true);
				nKCUIDiveReadySquadSlot.SetUnSelected();
			}
			List<NKMDiveTemplet> list = new List<NKMDiveTemplet>();
			foreach (NKMDiveTemplet sortedTemplate in NKCDiveManager.SortedTemplates)
			{
				if (!sortedTemplate.IsEventDive && sortedTemplate.EnableByTag)
				{
					list.Add(sortedTemplate);
				}
			}
			nKCUIDiveReady.m_lstDiveTemplet = list;
			int num2 = Math.Max(3, 3);
			for (num = 0; num < num2; num++)
			{
				nKCUIDiveReady.m_lstNKCUISlot.Add(NKCUISlot.GetNewInstance(nKCUIDiveReady.m_NKM_UI_DIVE_INFO_REWARD_CONTENT.transform));
				nKCUIDiveReady.m_lstNKCUISlot[num].transform.localScale = new Vector3(1f, 1f, 1f);
				NKCUtil.SetGameobjectActive(nKCUIDiveReady.m_lstNKCUISlot[num], bValue: true);
			}
			nKCUIDiveReady.m_NKM_UI_DIVE_INFO_DIVE_BUTTON.PointerClick.RemoveAllListeners();
			nKCUIDiveReady.m_NKM_UI_DIVE_INFO_DIVE_BUTTON.PointerClick.AddListener(nKCUIDiveReady.OnClickDive);
			NKCUtil.SetHotkey(nKCUIDiveReady.m_NKM_UI_DIVE_INFO_DIVE_BUTTON, HotkeyEventType.Confirm);
			nKCUIDiveReady.m_NKM_UI_DIVE_INFO_REDIVE_BUTTON.PointerClick.RemoveAllListeners();
			nKCUIDiveReady.m_NKM_UI_DIVE_INFO_REDIVE_BUTTON.PointerClick.AddListener(nKCUIDiveReady.OnClickDive);
			NKCUtil.SetHotkey(nKCUIDiveReady.m_NKM_UI_DIVE_INFO_REDIVE_BUTTON, HotkeyEventType.Confirm);
			nKCUIDiveReady.m_UINPCOperatorChloe.Init(bUseIdleAnimation: false);
			nKCUIDiveReady.m_csbtnReset.PointerClick.RemoveAllListeners();
			nKCUIDiveReady.m_csbtnReset.PointerClick.AddListener(nKCUIDiveReady.OnClickDiveReset);
			nKCUIDiveReady.m_tglJump.OnValueChanged.RemoveAllListeners();
			nKCUIDiveReady.m_tglJump.OnValueChanged.AddListener(nKCUIDiveReady.OnValueChangedJump);
			nKCUIDiveReady.m_tglJump.m_bGetCallbackWhileLocked = true;
			nKCUIDiveReady.m_btnJumpPopupInfo.PointerDown.RemoveAllListeners();
			nKCUIDiveReady.m_btnJumpPopupInfo.PointerDown.AddListener(nKCUIDiveReady.OnClickJumpInfo);
			nKCUIDiveReady.m_btnJumpPlusInfo.PointerDown.RemoveAllListeners();
			nKCUIDiveReady.m_btnJumpPlusInfo.PointerDown.AddListener(nKCUIDiveReady.OnClickJumpPlusInfo);
			nKCUIDiveReady.m_btnJumpDiscountInfo.PointerDown.RemoveAllListeners();
			nKCUIDiveReady.m_btnJumpDiscountInfo.PointerDown.AddListener(nKCUIDiveReady.OnClickJumpDiscountInfo);
			nKCUIDiveReady.m_btnJumpPopupClose.PointerClick.RemoveAllListeners();
			nKCUIDiveReady.m_btnJumpPopupClose.PointerClick.AddListener(nKCUIDiveReady.OnClickJumpClose);
			nKCUIDiveReady.m_operationSkip?.Init(nKCUIDiveReady.UpdateDiveResourceCount, nKCUIDiveReady.OnCloseSafeMining);
			NKCUtil.SetButtonClickDelegate(nKCUIDiveReady.m_csbtnDeckEdit, nKCUIDiveReady.OnSelectedSquad);
		}
		return nKCUIDiveReady;
	}

	private void OnClickJumpInfo(PointerEventData eventData)
	{
		NKCUIPopUpGuide.Instance.Open("ARTICLE_DIVE_EVENT", 1);
	}

	private void OnClickJumpPlusInfo(PointerEventData eventData)
	{
		NKCUITooltip.TextData textData = new NKCUITooltip.TextData(NKCStringTable.GetString("SI_PF_DIVE_JUMP_DESC01"));
		NKCUITooltip.Instance.Open(textData, eventData.position);
	}

	private void OnClickJumpDiscountInfo(PointerEventData eventData)
	{
		NKCUITooltip.TextData textData = new NKCUITooltip.TextData(NKCStringTable.GetString("SI_PF_DIVE_JUMP_DESC02"));
		NKCUITooltip.Instance.Open(textData, eventData.position);
	}

	private void OnClickJumpClose()
	{
		OnValueChangedJump(bValue: false);
	}

	private void OnValueChangedJump(bool bValue)
	{
		if (m_tglJump.m_bLock)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_CONTENTS_UNLOCK_CLEAR_STAGE, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		if (NKCScenManager.CurrentUserData().m_DiveGameData != null)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_PF_DIVE_SKIP_UNAVAILABLE"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		if (selectedDiveTemplet == null || !selectedDiveTemplet.IsEventDive)
		{
			NKCUtil.SetGameobjectActive(m_objJumpPopup, bValue: false);
			m_tglJump.Select(bSelect: false, bForce: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objJumpPopup, bValue);
			m_tglJump.Select(bValue, bForce: true);
			UpdateDiveCost(selectedDiveTemplet);
		}
	}

	private void OnClickDiveReset()
	{
		NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_DIVE_RESET, NKCUtilString.GET_STRING_DIVE_RESET_CONFIRM, GetCurrDiveResetItemID(), 1, delegate
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null && nKMUserData.m_DiveGameData != null)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_DIVE_ALREADY_STARTED);
			}
		});
	}

	private void OnClickDive()
	{
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		if (selectedDiveTemplet != null)
		{
			if (CheckSameSelectedAndOnGoing())
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().SetIntro();
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DIVE);
				return;
			}
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			bool flag = false;
			if (nKMUserData != null)
			{
				if (nKMUserData.m_DiveGameData != null)
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_DIVE_ALREADY_STARTED));
					return;
				}
				flag = nKMUserData.CheckDiveClear(selectedDiveTemplet.StageID);
			}
			if (flag && m_eventDiveTemplet == null && m_operationSkip != null)
			{
				if (!m_operationSkip.gameObject.activeSelf)
				{
					ShowSafeMiningPanel(selectedDiveTemplet);
				}
				else if (HaveEnoughResource(flag, selectedDiveTemplet, bJump: false))
				{
					NKCPacketSender.Send_NKMPacket_DIVE_SKIP_REQ(selectedDiveTemplet.StageID, m_operationSkip.CurrentCount);
				}
				return;
			}
		}
		SendDiveReq();
	}

	private void SendDiveReq()
	{
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		if (selectedDiveTemplet == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool bDiveCleared = false;
		if (nKMUserData != null)
		{
			if (nKMUserData.m_DiveGameData != null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_DIVE_ALREADY_STARTED));
				return;
			}
			bDiveCleared = nKMUserData.CheckDiveClear(selectedDiveTemplet.StageID);
		}
		if (!HaveEnoughResource(bDiveCleared, selectedDiveTemplet, JumpSelected))
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < selectedDiveTemplet.SquadCount; i++)
		{
			if (NKMMain.IsValidDeck(NKCScenManager.CurrentArmyData(), NKM_DECK_TYPE.NDT_DIVE, (byte)i) == NKM_ERROR_CODE.NEC_OK)
			{
				list.Add(i);
			}
		}
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.DIVE_SKIP_V2))
		{
			if (JumpSelected)
			{
				NKCPacketSender.Send_NKMPacket_DIVE_SKIP_REQ(selectedDiveTemplet.StageID, 1, m_cityID);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_DIVE_START_REQ(m_cityID, selectedDiveTemplet.StageID, list, JumpSelected);
			}
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_DIVE_START_REQ(m_cityID, selectedDiveTemplet.StageID, list, JumpSelected);
		}
	}

	private bool HaveEnoughResource(bool bDiveCleared, NKMDiveTemplet cNKMDiveTemplet, bool bJump)
	{
		if (cNKMDiveTemplet == null)
		{
			return false;
		}
		int diveCost = NKCDiveManager.GetDiveCost(bDiveCleared, cNKMDiveTemplet, m_cityID, bJump);
		if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(cNKMDiveTemplet.StageReqItemId) < diveCost)
		{
			NKCShopManager.OpenItemLackPopup(cNKMDiveTemplet.StageReqItemId, diveCost);
			return false;
		}
		return true;
	}

	private bool IsDiveStartPossible()
	{
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool flag = false;
		if (nKMUserData != null && nKMUserData.m_DiveClearData != null)
		{
			flag = nKMUserData.m_DiveClearData.Contains(selectedDiveTemplet.StageID);
		}
		if (flag)
		{
			return false;
		}
		NKMArmyData cNKMArmyData = NKCScenManager.CurrentArmyData();
		for (byte b = 0; b < selectedDiveTemplet.SquadCount; b++)
		{
			if (NKMMain.IsValidDeck(cNKMArmyData, NKM_DECK_TYPE.NDT_DIVE, b) == NKM_ERROR_CODE.NEC_OK)
			{
				return true;
			}
		}
		return false;
	}

	private void SetDivePossibleFX()
	{
		if (GetSelectedDiveTemplet() != null)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_FX, IsDiveStartPossible());
		}
	}

	private void OnDeckViewConfirm(NKMDeckIndex selectIndex, long supportUserUID = 0L)
	{
		SetDivePossibleFX();
		UpdateSquadSlots();
		NKCUIDeckViewer.CheckInstanceAndClose();
	}

	public override void UnHide()
	{
		base.UnHide();
		if (GetSelectedDiveTemplet() != null && NKCScenManager.CurrentUserData() != null)
		{
			UpdateDiveInfo();
		}
	}

	public void Refresh()
	{
		m_LoopScrollRect?.RefreshCells();
		UpdateDiveInfo();
	}

	private void SelectSquad()
	{
		if (GetSelectedDiveTemplet() != null)
		{
			NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
			{
				MenuName = NKCUtilString.GET_STRING_SELECT_SQUAD,
				eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.DeckSetupOnly,
				dOnSideMenuButtonConfirm = OnDeckViewConfirm,
				DeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, 0),
				dOnBackButton = NKCUIDeckViewer.CheckInstanceAndClose,
				SelectLeaderUnitOnOpen = false,
				bEnableDefaultBackground = true,
				bUpsideMenuHomeButton = false,
				StageBattleStrID = string.Empty
			};
			NKCUIDeckViewer.Instance.Open(options);
		}
	}

	private void SendDiveGiveUp()
	{
		NKMPacket_DIVE_GIVE_UP_REQ packet = new NKMPacket_DIVE_GIVE_UP_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
	}

	public void OnRecv(NKMPacket_DIVE_GIVE_UP_ACK cNKMPacket_DIVE_GIVE_UP_ACK)
	{
		m_LoopScrollRect.RefreshCells();
		UpdateDiveInfo();
	}

	public void OnRecv(NKMPacket_DIVE_EXPIRE_NOT cNKMPacket_DIVE_EXPIRE_NOT)
	{
		NKMDiveTemplet nKMDiveTemplet = NKMDiveTemplet.Find(cNKMPacket_DIVE_EXPIRE_NOT.stageID);
		if (nKMDiveTemplet == null)
		{
			return;
		}
		if (nKMDiveTemplet.IsEventDive)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetReservedDiveReverseAni(bSet: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
			return;
		}
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		if (selectedDiveTemplet != null && nKMDiveTemplet.StageID == selectedDiveTemplet.StageID)
		{
			CloseDiveInfo();
			m_SelectedIndex = -1;
			m_LoopScrollRect.RefreshCells();
		}
	}

	private void OnSelectedSquad()
	{
		if (GetSelectedDiveTemplet() != null && !CheckSameSelectedAndOnGoing())
		{
			if (CheckDiffSelectedAndOnGoingExist())
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_DIVE_GIVE_UP_AND_START, SendDiveGiveUp);
			}
			else
			{
				SelectSquad();
			}
		}
	}

	private void UpdateSquadSlots()
	{
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		if (selectedDiveTemplet == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		bool num = NKMContentUnlockManager.IsContentUnlocked(myUserData, new UnlockInfo(selectedDiveTemplet.StageUnlockReqType, selectedDiveTemplet.StageUnlockReqValue));
		bool flag = false;
		if (myUserData != null && myUserData.m_DiveClearData != null)
		{
			flag = myUserData.m_DiveClearData.Contains(selectedDiveTemplet.StageID);
		}
		if (num && !flag)
		{
			int squadCount = selectedDiveTemplet.SquadCount;
			for (int i = 0; i < m_lstNKCUIDiveReadySquadSlot.Count; i++)
			{
				NKCUIDiveReadySquadSlot nKCUIDiveReadySquadSlot = m_lstNKCUIDiveReadySquadSlot[i];
				if (!(nKCUIDiveReadySquadSlot != null))
				{
					continue;
				}
				if (i < squadCount)
				{
					NKCUtil.SetGameobjectActive(nKCUIDiveReadySquadSlot, bValue: true);
					NKMDeckIndex nKMDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, i);
					NKMDeckData deckData = NKCScenManager.CurrentArmyData().GetDeckData(nKMDeckIndex);
					if (NKMMain.IsValidDeck(NKCScenManager.CurrentArmyData(), nKMDeckIndex) == NKM_ERROR_CODE.NEC_OK || (deckData != null && deckData.GetState() == NKM_DECK_STATE.DECK_STATE_DIVE))
					{
						nKCUIDiveReadySquadSlot.SetSelected(nKMDeckIndex);
					}
					else
					{
						nKCUIDiveReadySquadSlot.SetUnSelected();
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(nKCUIDiveReadySquadSlot, bValue: false);
				}
			}
			return;
		}
		for (int j = 0; j < m_lstNKCUIDiveReadySquadSlot.Count; j++)
		{
			NKCUIDiveReadySquadSlot nKCUIDiveReadySquadSlot2 = m_lstNKCUIDiveReadySquadSlot[j];
			if (nKCUIDiveReadySquadSlot2 != null)
			{
				NKCUtil.SetGameobjectActive(nKCUIDiveReadySquadSlot2, bValue: false);
			}
		}
	}

	public RectTransform GetDiveReadySlot(int index)
	{
		NKCUIDiveReadySlot nKCUIDiveReadySlot = null;
		nKCUIDiveReadySlot = ((m_stkNKCUIDiveReadySlot.Count <= 0) ? NKCUIDiveReadySlot.GetNewInstance(m_NKM_UI_DIVE_LIST_Content.transform, OnSelectedDiveReadySlot) : m_stkNKCUIDiveReadySlot.Pop());
		if (nKCUIDiveReadySlot != null)
		{
			m_lstNKCUIDiveReadySlot.Add(nKCUIDiveReadySlot);
			return nKCUIDiveReadySlot.GetComponent<RectTransform>();
		}
		return null;
	}

	public void ReturnDiveReadySlot(Transform tr)
	{
		NKCUIDiveReadySlot component = tr.GetComponent<NKCUIDiveReadySlot>();
		m_lstNKCUIDiveReadySlot.Remove(component);
		m_stkNKCUIDiveReadySlot.Push(component);
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
	}

	public void ProvideDiveReadySlotData(Transform tr, int index)
	{
		NKCUIDiveReadySlot component = tr.GetComponent<NKCUIDiveReadySlot>();
		if (component != null)
		{
			if (m_eventDiveTemplet != null)
			{
				component.SetUI(index, m_eventDiveTemplet, m_cityID);
			}
			else
			{
				NKMDiveTemplet cNKMDiveTemplet = m_lstDiveTemplet[index];
				component.SetUI(index, cNKMDiveTemplet);
			}
			component.SetSelected(m_SelectedIndex == index);
		}
	}

	private void OnSelectedDiveReadySlot(NKCUIDiveReadySlot cNKCUIDiveReadySlotSelected)
	{
		int num = 0;
		for (num = 0; num < m_lstNKCUIDiveReadySlot.Count; num++)
		{
			NKCUIDiveReadySlot nKCUIDiveReadySlot = m_lstNKCUIDiveReadySlot[num];
			nKCUIDiveReadySlot.SetSelected(nKCUIDiveReadySlot == cNKCUIDiveReadySlotSelected);
		}
		m_SelectedIndex = cNKCUIDiveReadySlotSelected.GetIndex();
		OpenDiveInfo(cNKCUIDiveReadySlotSelected);
	}

	private bool CheckSameSelectedAndOnGoing()
	{
		NKMDiveGameData diveGameData = NKCScenManager.GetScenManager().GetMyUserData().m_DiveGameData;
		if (diveGameData == null || diveGameData.Floor.Templet == null)
		{
			return false;
		}
		if (m_SelectedIndex < 0)
		{
			return false;
		}
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		if (selectedDiveTemplet != null && diveGameData.Floor.Templet.StageID == selectedDiveTemplet.StageID)
		{
			if (!diveGameData.Floor.Templet.IsEventDive)
			{
				return true;
			}
			int cityIDByEventData = NKCScenManager.CurrentUserData().m_WorldmapData.GetCityIDByEventData(NKM_WORLDMAP_EVENT_TYPE.WET_DIVE, diveGameData.DiveUid);
			if (m_cityID == cityIDByEventData)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckDiffSelectedAndOnGoingExist()
	{
		NKMDiveGameData diveGameData = NKCScenManager.GetScenManager().GetMyUserData().m_DiveGameData;
		if (diveGameData == null || diveGameData.Floor.Templet == null)
		{
			return false;
		}
		if (m_SelectedIndex < 0)
		{
			return false;
		}
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		if (selectedDiveTemplet != null)
		{
			if (diveGameData.Floor.Templet.StageID != selectedDiveTemplet.StageID)
			{
				return true;
			}
			if (diveGameData.Floor.Templet.IsEventDive)
			{
				int cityIDByEventData = NKCScenManager.CurrentUserData().m_WorldmapData.GetCityIDByEventData(NKM_WORLDMAP_EVENT_TYPE.WET_DIVE, diveGameData.DiveUid);
				if (m_cityID != cityIDByEventData)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void UpdateDiveInfo()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		bool flag = CheckSameSelectedAndOnGoing();
		m_tglJump.Select(bSelect: false);
		UpdateSquadSlots();
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		if (selectedDiveTemplet != null)
		{
			SetBG(selectedDiveTemplet.StageType == NKM_DIVE_STAGE_TYPE.NDST_HARD);
			SetDiveInfoBG(selectedDiveTemplet.StageType == NKM_DIVE_STAGE_TYPE.NDST_HARD);
			bool flag2 = NKMContentUnlockManager.IsContentUnlocked(myUserData, new UnlockInfo(selectedDiveTemplet.StageUnlockReqType, selectedDiveTemplet.StageUnlockReqValue));
			bool flag3 = false;
			if (myUserData.m_DiveClearData != null)
			{
				flag3 = myUserData.m_DiveClearData.Contains(selectedDiveTemplet.StageID);
			}
			if (selectedDiveTemplet.IsEventDive)
			{
				m_NKM_UI_DIVE_INFO_FIRST_REWARD_TEXT.text = NKCUtilString.GET_STRING_DIVE_READY_EXPLORE_REWARD;
			}
			else if (flag3)
			{
				m_NKM_UI_DIVE_INFO_FIRST_REWARD_TEXT.text = NKCUtilString.GET_STRING_DIVE_READY_SAFE_MINING_REWARD;
			}
			else
			{
				m_NKM_UI_DIVE_INFO_FIRST_REWARD_TEXT.text = NKCUtilString.GET_STRING_DIVE_READY_FIRST_REWARD;
			}
			bool bValue = NKMOpenTagManager.IsOpened("DIVE_SKIP") && selectedDiveTemplet.IsEventDive && flag2;
			NKCUtil.SetGameobjectActive(m_objJumpToggle, bValue);
			m_tglJump.UnLock();
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_DENIED, !flag2);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_CLEARED_ICON, flag3);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_CLEARED, flag3);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_SQUAD_TEXT, !flag3);
			if (flag2)
			{
				NKCUtil.SetGameobjectActive(m_lbLocked, bValue: false);
				m_NKM_UI_DIVE_INFO_LEVEL_COUNT.text = selectedDiveTemplet.StageLevel.ToString();
				m_NKM_UI_DIVE_INFO_AREA_COUNT.text = selectedDiveTemplet.RandomSetCount.ToString();
				if (flag)
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_REDIVE_BUTTON, bValue: true);
					NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_DIVE_BUTTON, bValue: false);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_REDIVE_BUTTON, bValue: false);
					NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_DIVE_BUTTON.gameObject, flag2);
					NKCUtil.SetLabelText(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_TEXT, flag3 ? NKCUtilString.GET_STRING_DIVE_SAFE_MINING : NKCUtilString.GET_STRING_DIVE_GO);
					UpdateDiveButtonState(flag3, selectedDiveTemplet, myUserData);
					UpdateDiveCost(selectedDiveTemplet);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lbLocked, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_DIVE_BUTTON.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_REDIVE_BUTTON, bValue: false);
				m_NKM_UI_DIVE_INFO_LEVEL_COUNT.text = "?";
				m_NKM_UI_DIVE_INFO_AREA_COUNT.text = "?";
			}
			m_NKM_UI_DIVE_INFO_TITLE_TEXT.text = selectedDiveTemplet.Get_STAGE_NAME();
			m_NKM_UI_DIVE_INFO_SUBTITLE_TEXT.text = selectedDiveTemplet.Get_STAGE_NAME_SUB();
			int num = 0;
			for (num = 0; num < m_lstNKCUISlot.Count; num++)
			{
				bool flag4 = false;
				if (!flag3)
				{
					if (num < selectedDiveTemplet.FirstRewardList.Count && selectedDiveTemplet.FirstRewardList[num].FIRSTREWARD_TYPE != NKM_REWARD_TYPE.RT_NONE)
					{
						NKCUtil.SetGameobjectActive(m_lstNKCUISlot[num], bValue: true);
						NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(selectedDiveTemplet.FirstRewardList[num].FIRSTREWARD_TYPE, selectedDiveTemplet.FirstRewardList[num].FIRSTREWARD_ID, selectedDiveTemplet.FirstRewardList[num].FIRSTREWARD_QUANTITY);
						m_lstNKCUISlot[num].SetData(data);
						m_lstNKCUISlot[num].SetCompleteMark(flag3);
						flag4 = true;
					}
				}
				else if (num < selectedDiveTemplet.SafeRewards.Count && selectedDiveTemplet.SafeRewards[num].RewardType != NKM_REWARD_TYPE.RT_NONE)
				{
					NKCUtil.SetGameobjectActive(m_lstNKCUISlot[num], bValue: true);
					NKCUISlot.SlotData data2 = NKCUISlot.SlotData.MakeRewardTypeData(selectedDiveTemplet.SafeRewards[num].RewardType, selectedDiveTemplet.SafeRewards[num].RewardId, selectedDiveTemplet.SafeRewards[num].RewardQuantity);
					m_lstNKCUISlot[num].SetData(data2);
					m_lstNKCUISlot[num].SetCompleteMark(bValue: false);
					flag4 = true;
				}
				if (!flag4)
				{
					NKCUtil.SetGameobjectActive(m_lstNKCUISlot[num], bValue: false);
				}
			}
			if (!flag)
			{
				SetDivePossibleFX();
			}
			UpdateUpsideResourceList(selectedDiveTemplet.StageReqItemId);
		}
		else
		{
			SetBG();
			SetDiveInfoBG();
			NKCUtil.SetGameobjectActive(m_objJumpToggle, bValue: false);
		}
		if (m_operationSkip != null)
		{
			m_operationSkip.Close();
		}
	}

	private void UpdateDiveCost(NKMDiveTemplet cNKMDiveTemplet)
	{
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool bDiveCleared = false;
		if (nKMUserData != null && nKMUserData.m_DiveClearData != null && cNKMDiveTemplet != null)
		{
			bDiveCleared = nKMUserData.m_DiveClearData.Contains(cNKMDiveTemplet.StageID);
		}
		int diveCost = NKCDiveManager.GetDiveCost(bDiveCleared, selectedDiveTemplet, m_cityID, JumpSelected);
		if (diveCost == 0)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_COUNT, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_ICON, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_COUNT, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_ICON, bValue: true);
		NKCUtil.SetLabelText(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_COUNT, diveCost.ToString());
		if (selectedDiveTemplet != null)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(selectedDiveTemplet.StageReqItemId);
			NKCUtil.SetImageSprite(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_ICON, orLoadMiscItemSmallIcon);
		}
		if (selectedDiveTemplet != null && m_objJumpPopup.activeSelf)
		{
			m_slotJumpBasic.SetData(selectedDiveTemplet.StageReqItemId, selectedDiveTemplet.StageReqItemCount, nKMUserData.m_InventoryData.GetCountMiscItem(selectedDiveTemplet.StageReqItemId));
			m_slotJumpPlus.SetData(NKMDiveTemplet.DiveStormCostMiscId, selectedDiveTemplet.GetDiveJumpPlusCost(), nKMUserData.m_InventoryData.GetCountMiscItem(NKMDiveTemplet.DiveStormCostMiscId));
			m_slotJumpDiscount.SetData(selectedDiveTemplet.StageReqItemId, NKCDiveManager.GetDiveDiscountCost(m_cityID, selectedDiveTemplet.StageReqItemCount + selectedDiveTemplet.GetDiveJumpPlusCost()), nKMUserData.m_InventoryData.GetCountMiscItem(selectedDiveTemplet.StageReqItemId));
		}
	}

	private void UpdateDiveButtonState(bool bDiveCleared, NKMDiveTemplet cNKMDiveTemplet, NKMUserData cNKMUserData)
	{
		if (m_NKM_UI_DIVE_INFO_DIVE_BUTTON == null)
		{
			return;
		}
		if (bDiveCleared)
		{
			bool flag = true;
			if (cNKMDiveTemplet != null && cNKMUserData != null)
			{
				flag = cNKMDiveTemplet.SafeMineReqItemCount <= cNKMUserData.m_InventoryData.GetCountMiscItem(cNKMDiveTemplet.SafeMineReqItemID);
			}
			if (flag)
			{
				m_NKM_UI_DIVE_INFO_DIVE_BUTTON.UnLock();
			}
			else
			{
				m_NKM_UI_DIVE_INFO_DIVE_BUTTON.Lock();
			}
		}
		else
		{
			m_NKM_UI_DIVE_INFO_DIVE_BUTTON.UnLock();
		}
	}

	private void OpenDiveInfo(NKCUIDiveReadySlot cNKCUIDiveReadySlot)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (cNKCUIDiveReadySlot == null || myUserData == null)
		{
			CloseDiveInfo();
		}
		else
		{
			OpenDiveInfo();
		}
	}

	private void OpenDiveInfo()
	{
		bool flag = false;
		if (!m_NKM_UI_DIVE_INFO.activeSelf)
		{
			flag = true;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO, bValue: true);
		if (!flag)
		{
			m_NKM_UI_DIVE_INFO_Animator.Play("NKM_UI_DIVE_INFO_INTRO");
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_FX, bValue: false);
		UpdateDiveInfo();
	}

	private void CloseDiveInfo()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_INFO, bValue: false);
		SetBG();
	}

	private void AutoSelect()
	{
		NKCScenManager.GetScenManager().GetMyUserData();
		int num = -1;
		if (m_eventDiveTemplet == null)
		{
			NKCDiveManager.GetCurrNormalDiveTemplet(out var selectedIndex);
			num = selectedIndex;
		}
		else
		{
			num = 0;
		}
		m_SelectedIndex = num;
		m_LoopScrollRect.velocity = new Vector2(0f, 0f);
		m_LoopScrollRect.SetIndexPosition(0);
		if (m_SelectedIndex >= 0)
		{
			int num2 = Mathf.Max(0, m_SelectedIndex - 15);
			float time = Mathf.Max(1f, (float)(m_SelectedIndex - num2) / 10f);
			m_LoopScrollRect.SetIndexPosition(num2);
			m_LoopScrollRect.ScrollToCell(m_SelectedIndex, time, LoopScrollRect.ScrollTarget.Center, OnCompleteScroll);
			OpenDiveInfo();
		}
	}

	private void OnCompleteScroll()
	{
		for (int i = 0; i < m_lstNKCUIDiveReadySlot.Count; i++)
		{
			NKCUIDiveReadySlot nKCUIDiveReadySlot = m_lstNKCUIDiveReadySlot[i];
			if (nKCUIDiveReadySlot != null && nKCUIDiveReadySlot.IsSelected())
			{
				nKCUIDiveReadySlot.PlayScrollArriveEffect();
				break;
			}
		}
	}

	public void Open(int cityID, int eventDiveID, DateTime _ResetTicketChargeDate)
	{
		m_DiveResetTicketChargeDate = _ResetTicketChargeDate;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_bFirstOpen)
		{
			m_LoopScrollRect.PrepareCells();
			m_bFirstOpen = false;
		}
		int num = 0;
		for (num = 0; num < m_lstNKCUIDiveReadySquadSlot.Count; num++)
		{
			NKCUIDiveReadySquadSlot nKCUIDiveReadySquadSlot = m_lstNKCUIDiveReadySquadSlot[num];
			if (nKCUIDiveReadySquadSlot != null)
			{
				nKCUIDiveReadySquadSlot.SetUnSelected();
			}
		}
		if (cityID > 0 && eventDiveID > 0)
		{
			m_cityID = cityID;
			m_LoopScrollRect.TotalCount = 1;
			m_eventDiveTemplet = NKMDiveTemplet.Find(eventDiveID);
		}
		else
		{
			m_LoopScrollRect.TotalCount = m_lstDiveTemplet.Count;
			m_cityID = 0;
			m_eventDiveTemplet = null;
		}
		NKCUtil.SetGameobjectActive(m_objJumpPopup, bValue: false);
		m_SelectedIndex = -1;
		for (num = 0; num < m_lstNKCUIDiveReadySlot.Count; num++)
		{
			m_lstNKCUIDiveReadySlot[num].SetSelected(bSet: false);
		}
		CloseDiveInfo();
		UpdateDiveResetUI(_ResetTicketChargeDate);
		UIOpened();
		AutoSelect();
		bool bMute = CheckTutorial();
		m_UINPCOperatorChloe.PlayAni(NPC_ACTION_TYPE.START, bMute);
	}

	private int GetCurrDiveResetItemID()
	{
		int result = 1041;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return result;
		}
		if (!nKMUserData.CheckPrice(1L, 1041) && nKMUserData.CheckPrice(1L, 1042))
		{
			result = 1042;
		}
		return result;
	}

	private long GetResetItemTotalCount()
	{
		return NKCScenManager.CurrentUserData()?.m_InventoryData.GetCountMiscItem(GetCurrDiveResetItemID()) ?? 0;
	}

	private void UpdateDiveResetUI(DateTime _ResetTicketChargeDate)
	{
		NKCUtil.SetGameobjectActive(m_objDiveReset, m_eventDiveTemplet == null);
		if (m_objDiveReset != null && m_objDiveReset.activeSelf)
		{
			NKCUtil.SetLabelText(m_lbResetCost, GetResetItemTotalCount().ToString());
			NKCUtil.SetImageSprite(m_imgResetCostIcon, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(GetCurrDiveResetItemID()));
		}
		UpdateDiveResetUIOnlyTime(_ResetTicketChargeDate);
	}

	private void UpdateDiveResetUIOnlyTime(DateTime _ResetTicketChargeDate)
	{
		if (m_objDiveReset != null && m_objDiveReset.activeSelf)
		{
			NKCUtil.SetLabelText(m_lbRemainTimeToReset, string.Format(NKCUtilString.GET_STRING_DIVE_REMAIN_TIME_TO_RESET, NKCUtilString.GetRemainTimeStringExWithoutEnd(_ResetTicketChargeDate)));
		}
	}

	private void UpdateUpsideResourceList(int resourceID = 0)
	{
		m_upsideMenuResourceList.Clear();
		m_upsideMenuResourceList.Add(2);
		m_upsideMenuResourceList.Add(101);
		if (resourceID > 0)
		{
			m_upsideMenuResourceList.Insert(0, resourceID);
		}
		UpdateUpsideMenu();
	}

	private void ShowSafeMiningPanel(NKMDiveTemplet cNKMDiveTemplet)
	{
		if (cNKMDiveTemplet == null)
		{
			m_operationSkip?.Close();
			return;
		}
		int diveCost = NKCDiveManager.GetDiveCost(bDiveCleared: true, cNKMDiveTemplet, 0, bJump: false);
		NKCUtil.SetGameobjectActive(m_operationSkip, bValue: true);
		m_operationSkip?.SetData(0, 0, cNKMDiveTemplet.StageReqItemId, diveCost, 1);
		NKCUtil.SetLabelText(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_TEXT, NKCUtilString.GET_STRING_DIVE_SAFE_MINING_START);
	}

	private void UpdateDiveResourceCount(int count)
	{
		if (count > 0)
		{
			NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			bool bDiveCleared = false;
			if (nKMUserData != null && nKMUserData.m_DiveClearData != null && selectedDiveTemplet != null)
			{
				bDiveCleared = nKMUserData.m_DiveClearData.Contains(selectedDiveTemplet.StageID);
			}
			int num = NKCDiveManager.GetDiveCost(bDiveCleared, selectedDiveTemplet, m_cityID, JumpSelected) * count;
			NKCUtil.SetLabelText(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_COUNT, num.ToString());
		}
	}

	private void OnCloseSafeMining()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKMDiveTemplet selectedDiveTemplet = GetSelectedDiveTemplet();
		bool flag = false;
		if (nKMUserData != null && nKMUserData.m_DiveClearData != null && selectedDiveTemplet != null)
		{
			flag = nKMUserData.m_DiveClearData.Contains(selectedDiveTemplet.StageID);
		}
		UpdateDiveCost(selectedDiveTemplet);
		if (flag)
		{
			NKCUtil.SetLabelText(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_TEXT, NKCUtilString.GET_STRING_DIVE_SAFE_MINING);
		}
		else
		{
			NKCUtil.SetLabelText(m_NKM_UI_DIVE_INFO_DIVE_BUTTON_TEXT, NKCUtilString.GET_STRING_DIVE_GO);
		}
	}

	public override void CloseInternal()
	{
		NKCSoundManager.StopAllSound(SOUND_TRACK.VOICE);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		if (m_operationSkip != null && m_operationSkip.gameObject.activeSelf)
		{
			m_operationSkip.Close();
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetReservedDiveReverseAni(bSet: true);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
	}

	private bool CheckTutorial()
	{
		return NKCTutorialManager.TutorialRequired(TutorialPoint.DiveReady) != TutorialStep.None;
	}
}
