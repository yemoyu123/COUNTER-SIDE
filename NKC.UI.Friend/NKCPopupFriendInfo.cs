using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Community;
using ClientPacket.Guild;
using NKC.UI.Collection;
using NKC.UI.Component;
using NKC.UI.Gauntlet;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Friend;

public class NKCPopupFriendInfo : NKCUIBase
{
	public enum DeckTab
	{
		Rank,
		Async
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_FRIEND";

	private const string UI_ASSET_NAME = "NKM_UI_FRIEND_INFO_POPUP";

	private static NKCPopupFriendInfo m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	[Header("소대 토글")]
	public GameObject m_NKM_UI_FRIEND_INFO_POPUP_SQUAD_TOGGLE;

	public NKCUIComToggle m_tgMain;

	public NKCUIComToggle m_tgRank;

	public NKCUIComToggle m_tgAsync;

	[Header("정보")]
	public Text m_NKM_UI_FRIEND_PROFILE_INFO_NAME;

	public Text m_NKM_UI_FRIEND_PROFILE_INFO_LEVEL;

	public Text m_UID_TEXT;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_BadgeUI;

	public Text m_lbGuildName;

	public NKCUISlotProfile m_NKCUISlot;

	public NKCUIComTitlePanel m_TitlePanel;

	public Text m_NKM_UI_FRIEND_PROFILE_COMMENT_TEXT;

	public Image m_ANIM_SHIP_IMG;

	public NKCUIOperatorDeckSlot m_OperatorSlot;

	public List<NKCDeckViewUnitSlot> m_lstNKCDeckViewUnitSlot;

	public NKCUILeagueTier m_NKCUILeagueTier;

	public Text m_NKM_UI_FRIEND_PROFILE_TIER_RANK;

	public Text m_NKM_UI_FRIEND_PROFILE_TIER_TIER;

	public Text m_NKM_UI_FRIEND_PROFILE_TIER_SCORE;

	public List<NKCUISlot> m_lstEmblem;

	[Header("버튼")]
	public NKCUIComStateButton m_cbtnChat;

	public NKCUIComStateButton m_cbtnInvite;

	public NKCUIComButton m_cbtnBlock;

	public NKCUIComButton m_cbtnBlockCancel;

	public NKCUIComStateButton m_csbtnDormitory;

	public NKCUIComStateButton m_csbtnSimulatedPvpTest;

	public NKCUIComStateButton m_csbtnDeckCopy;

	[Space]
	public GameObject m_NKM_UI_POPUP_OK_ROOT;

	public GameObject m_objFriendMenu;

	public GameObject m_objTroopsPanel;

	public GameObject m_objEmpty;

	public GameObject m_objDomitoryLocked;

	public Text m_lbEmptyMessage;

	[Header("Sprite")]
	public Sprite SpriteTierBG_Rank;

	public Sprite SpriteTierBG_Async;

	[Header("격전지원")]
	public GameObject m_NKM_UI_FRIEND_INFO_POPUP_TITLE;

	public Text m_TITLE_TEXT;

	[Header("소대 정보")]
	public GameObject m_NKM_UI_FRIEND_PROFILE_UNIT_POWER_ALL;

	public Text m_ArmyOperationPower;

	public Text m_ArmyAvgCost;

	[Header("커스텀 매치 초대")]
	public GameObject m_objPVPMenu;

	public NKCUIComStateButton m_csbtnInviteToCustomMatch;

	public NKCPopupGuildUserInfo m_GuildUserInfo;

	private NKMUserProfileData m_cNKMUserProfileData;

	private DeckTab m_deckTab;

	[Header("지원유닛")]
	public GameObject m_objAssistUnit;

	public NKCUIComStateButton m_csbtnAssistUnitSlot;

	public NKCUIUnitSelectListSlotAssist m_AssistUnitSlot;

	private NKMGuildSimpleData m_GuildSimpleData;

	private NKMUnitData m_SupportUnitData;

	private int m_iFierceBossGroupID;

	private long m_lFierceUserUID;

	public static NKCPopupFriendInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFriendInfo>("AB_UI_NKM_UI_FRIEND", "NKM_UI_FRIEND_INFO_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFriendInfo>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_FRIEND_INFO;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static bool IsHasInstance()
	{
		return m_Instance != null;
	}

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_lstNKCDeckViewUnitSlot[i];
			if (nKCDeckViewUnitSlot != null)
			{
				nKCDeckViewUnitSlot.Init(i);
			}
		}
		m_cbtnChat.PointerClick.RemoveAllListeners();
		m_cbtnChat.PointerClick.AddListener(OnClickChat);
		m_cbtnChat.m_bGetCallbackWhileLocked = true;
		m_cbtnInvite.PointerClick.RemoveAllListeners();
		m_cbtnInvite.PointerClick.AddListener(OnClickInvite);
		m_cbtnBlock.PointerClick.RemoveAllListeners();
		m_cbtnBlock.PointerClick.AddListener(OnClickBlock);
		m_cbtnBlockCancel.PointerClick.RemoveAllListeners();
		m_cbtnBlockCancel.PointerClick.AddListener(OnClickCancelBlockFriend);
		m_csbtnDormitory.PointerClick.RemoveAllListeners();
		m_csbtnDormitory.PointerClick.AddListener(OnClickDormitory);
		NKCUtil.SetGameobjectActive(m_csbtnSimulatedPvpTest, bValue: false);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.SIMULATED_PVP))
		{
			NKCUtil.SetGameobjectActive(m_csbtnSimulatedPvpTest, bValue: true);
			m_csbtnSimulatedPvpTest.PointerClick.RemoveAllListeners();
			m_csbtnSimulatedPvpTest.PointerClick.AddListener(OnClickSimulatedPvpTest);
		}
		m_tgRank.OnValueChanged.RemoveAllListeners();
		m_tgRank.OnValueChanged.AddListener(OnToggleChangedRank);
		m_tgAsync.OnValueChanged.RemoveAllListeners();
		m_tgAsync.OnValueChanged.AddListener(OnToggleChangedAsync);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_FRIENDLY_MODE) && m_csbtnInviteToCustomMatch != null)
		{
			m_csbtnInviteToCustomMatch.PointerClick.RemoveAllListeners();
			m_csbtnInviteToCustomMatch.PointerClick.AddListener(OnClickInviteCustomMatch);
		}
		for (int j = 0; j < m_lstEmblem.Count; j++)
		{
			m_lstEmblem[j].Init();
		}
		m_GuildUserInfo?.InitUI();
		NKCUtil.SetBindFunction(m_csbtnAssistUnitSlot, OnClickAssistUnitInfo);
		NKCUtil.SetBindFunction(m_csbtnDeckCopy, OnClickDeckCopy);
		NKCUtil.SetHotkey(m_csbtnDeckCopy, HotkeyEventType.Copy);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(NKMUserProfileData cNKMUserProfileData, NKMSupportUnitData supportUnitData = null, bool bRegisterUI = true)
	{
		NKCUIFriendSlot.FRIEND_SLOT_TYPE fRIEND_SLOT_TYPE = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_NONE;
		switch (NKCScenManager.GetScenManager().GetNowScenID())
		{
		case NKM_SCEN_ID.NSI_FRIEND:
			fRIEND_SLOT_TYPE = NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().GetCurrentSlotType();
			m_deckTab = DeckTab.Rank;
			break;
		case NKM_SCEN_ID.NSI_GAUNTLET_LOBBY:
			switch (NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().GetCurrentLobbyTab())
			{
			case NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC:
				fRIEND_SLOT_TYPE = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_GAUNTLET_LIST;
				m_deckTab = DeckTab.Async;
				break;
			case NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK:
			case NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE:
				fRIEND_SLOT_TYPE = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_GAUNTLET_LIST;
				m_deckTab = DeckTab.Rank;
				break;
			}
			break;
		case NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY:
			fRIEND_SLOT_TYPE = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_GAUNTLET_LIST;
			m_deckTab = DeckTab.Rank;
			break;
		case NKM_SCEN_ID.NSI_WARFARE_GAME:
			fRIEND_SLOT_TYPE = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_SEARCH;
			m_deckTab = DeckTab.Rank;
			break;
		case NKM_SCEN_ID.NSI_GUILD_LOBBY:
			fRIEND_SLOT_TYPE = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_GUILD_LIST;
			m_deckTab = DeckTab.Rank;
			break;
		case NKM_SCEN_ID.NSI_OFFICE:
			fRIEND_SLOT_TYPE = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_OFFICE;
			break;
		case NKM_SCEN_ID.NSI_HOME:
			if (NKCPopupPrivateChatLobby.IsInstanceOpen)
			{
				fRIEND_SLOT_TYPE = NKCPopupPrivateChatLobby.Instance.GetFriendSlotType();
			}
			break;
		}
		NKCUtil.SetGameobjectActive(m_GuildUserInfo, fRIEND_SLOT_TYPE == NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_GUILD_LIST);
		if (m_GuildUserInfo != null && m_GuildUserInfo.gameObject.activeSelf)
		{
			m_GuildUserInfo.SetData(cNKMUserProfileData);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_PROFILE_UNIT_POWER_ALL, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_INFO_POPUP_TITLE, bValue: false);
		m_tgRank.Select(m_deckTab == DeckTab.Rank, bForce: true);
		m_tgAsync.Select(m_deckTab == DeckTab.Async, bForce: true);
		SetData(fRIEND_SLOT_TYPE, cNKMUserProfileData);
		UpdateSupportUnitUI(supportUnitData);
		SetGuildData();
		m_NKCUIOpenAnimator.PlayOpenAni();
		if (bRegisterUI)
		{
			UIOpened();
		}
	}

	public void Open(NKMFierceProfileData fierceData)
	{
		m_tgRank.Select(bSelect: false);
		m_tgAsync.Select(bSelect: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetData(fierceData);
		SetGuildData();
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	private void UpdateSupportUnitUI(NKMSupportUnitData supportUnitData)
	{
		NKCUtil.SetGameobjectActive(m_objAssistUnit, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.USE_SUPPORT_UNIT));
		if (supportUnitData.asyncUnitEquip != null && supportUnitData.asyncUnitEquip.asyncUnit != null)
		{
			NKMAsyncUnitData asyncUnit = supportUnitData.asyncUnitEquip.asyncUnit;
			NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(asyncUnit.unitId, asyncUnit.unitLevel, (short)asyncUnit.limitBreakLevel, asyncUnit.tacticLevel, asyncUnit.reactorLevel);
			if (nKMUnitData != null)
			{
				nKMUnitData.m_SkinID = asyncUnit.skinId;
				m_SupportUnitData = nKMUnitData;
				m_AssistUnitSlot.SetData(nKMUnitData, supportUnitData.userUid);
				m_AssistUnitSlot.SetEquipData(NKCUIPopupAssistSelect.GetEquipSetData(supportUnitData.asyncUnitEquip));
				NKMAsyncUnitData asyncUnit2 = supportUnitData.asyncUnitEquip.asyncUnit;
				NKMUnitData nKMUnitData2 = NKCUtil.MakeDummyUnit(asyncUnit2.unitId, asyncUnit2.unitLevel, (short)asyncUnit2.limitBreakLevel, asyncUnit2.tacticLevel, asyncUnit2.reactorLevel);
				nKMUnitData2.m_UnitUID = supportUnitData.userUid;
				nKMUnitData2.m_UserUID = supportUnitData.userUid;
				m_AssistUnitSlot.SetCalculateOperatorPower(NKCUIPopupAssistSelect.GetCalculateOperatorPower(nKMUnitData2, supportUnitData.asyncUnitEquip));
				return;
			}
		}
		m_SupportUnitData = null;
		m_AssistUnitSlot.SetData(null, 0L);
	}

	private void OnClickChat()
	{
		if (NKMOpenTagManager.IsOpened("CHAT_PRIVATE"))
		{
			if (m_cbtnChat.m_bLock)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_CHAT_BLOCKED, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				return;
			}
			bool bAdmin;
			switch (NKCContentManager.CheckContentStatus(ContentsType.FRIENDS, out bAdmin))
			{
			case NKCContentManager.eContentStatus.Open:
				if (NKCScenManager.GetScenManager().GetGameOptionData().UseChatContent)
				{
					Close();
					NKCPacketSender.Send_NKMPacket_PRIVATE_CHAT_LIST_REQ(m_cNKMUserProfileData.commonProfile.userUid);
				}
				else
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_OPTION_GAME_CHAT_NOTICE);
				}
				break;
			case NKCContentManager.eContentStatus.Lock:
				NKCContentManager.ShowLockedMessagePopup(ContentsType.FRIENDS);
				break;
			}
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_COMING_SOON_SYSTEM);
		}
	}

	private void OnClickInvite()
	{
		if (NKCGuildManager.MyGuildData.inviteList.Find((FriendListData x) => x.commonProfile.userUid == m_cNKMUserProfileData.commonProfile.userUid) == null)
		{
			if (NKCGuildManager.MyGuildData.inviteList.Count == NKMCommonConst.Guild.MaxInviteCount)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GUILD_MAX_INVITE_COUNT);
				return;
			}
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_POPUP_INVITE_TITLE, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_INVITE_SEND_POPUP_BODY_DESC, m_cNKMUserProfileData.commonProfile.nickname), delegate
			{
				NKCPacketSender.Send_NKMPacket_GUILD_INVITE_REQ(NKCGuildManager.MyData.guildUid, m_cNKMUserProfileData.commonProfile.userUid);
			}, null, NKCUtilString.GET_STRING_CONSORTIUM_INVITE);
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GUILD_ALREADY_INVITED);
		}
	}

	private void OnClickBlock()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, string.Format(NKCUtilString.GET_STRING_FRIEND_BLOCK_REQ_ONE_PARAM, m_cNKMUserProfileData.commonProfile.nickname), OnClickBlock_);
	}

	private void OnClickBlock_()
	{
		NKMPacket_FRIEND_BLOCK_REQ nKMPacket_FRIEND_BLOCK_REQ = new NKMPacket_FRIEND_BLOCK_REQ();
		nKMPacket_FRIEND_BLOCK_REQ.friendCode = m_cNKMUserProfileData.commonProfile.friendCode;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_BLOCK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void OnClickCancelBlockFriend()
	{
		NKMPacket_FRIEND_BLOCK_REQ nKMPacket_FRIEND_BLOCK_REQ = new NKMPacket_FRIEND_BLOCK_REQ();
		nKMPacket_FRIEND_BLOCK_REQ.friendCode = m_cNKMUserProfileData.commonProfile.friendCode;
		nKMPacket_FRIEND_BLOCK_REQ.isCancel = true;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_BLOCK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void SetData(NKCUIFriendSlot.FRIEND_SLOT_TYPE _FRIEND_SLOT_TYPE, NKMUserProfileData cNKMUserProfileData)
	{
		if (cNKMUserProfileData == null)
		{
			return;
		}
		m_cNKMUserProfileData = cNKMUserProfileData;
		m_GuildSimpleData = cNKMUserProfileData.guildData;
		m_NKM_UI_FRIEND_PROFILE_INFO_NAME.text = cNKMUserProfileData.commonProfile.nickname;
		m_NKM_UI_FRIEND_PROFILE_INFO_LEVEL.text = string.Format(NKCUtilString.GET_STRING_FRIEND_INFO_LEVEL_ONE_PARAM, cNKMUserProfileData.commonProfile.level);
		m_UID_TEXT.text = NKCUtilString.GetFriendCode(cNKMUserProfileData.commonProfile.friendCode);
		m_NKCUISlot.SetProfiledata(cNKMUserProfileData, null, NKCTacticUpdateUtil.IsMaxTacticLevel(cNKMUserProfileData.commonProfile.mainUnitTacticLevel));
		m_TitlePanel?.SetData(cNKMUserProfileData);
		m_NKM_UI_FRIEND_PROFILE_COMMENT_TEXT.text = NKCFilterManager.CheckBadChat(cNKMUserProfileData.friendIntro);
		SetDeck(cNKMUserProfileData, m_deckTab);
		SetPvP(cNKMUserProfileData, m_deckTab);
		bool bValue = NKCFriendManager.IsFriend(cNKMUserProfileData.commonProfile.friendCode) || NKCGuildManager.IsGuildMemberByUID(cNKMUserProfileData.commonProfile.userUid);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_INFO_POPUP_SQUAD_TOGGLE, bValue: true);
		switch (_FRIEND_SLOT_TYPE)
		{
		case NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_GAUNTLET_LIST:
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_ROOT, bValue: false);
			break;
		case NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_GAUNTLET_CUSTOM_LIST:
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_ROOT, bValue: true);
			NKCUtil.SetGameobjectActive(m_objFriendMenu, bValue: false);
			NKCUtil.SetGameobjectActive(m_cbtnChat, bValue);
			NKCUtil.SetGameobjectActive(m_cbtnBlock, bValue: false);
			NKCUtil.SetGameobjectActive(m_objPVPMenu, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnDormitory, bValue: false);
			break;
		case NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_NONE:
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_ROOT, bValue: false);
			break;
		case NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_GUILD_LIST:
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_ROOT, bValue: false);
			break;
		default:
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_ROOT, bValue: true);
			NKCUtil.SetGameobjectActive(m_objPVPMenu, bValue: false);
			NKCUtil.SetGameobjectActive(m_objFriendMenu, bValue: true);
			NKCUtil.SetGameobjectActive(m_cbtnChat, bValue);
			NKCUtil.SetGameobjectActive(m_cbtnBlock, _FRIEND_SLOT_TYPE == NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_LIST || _FRIEND_SLOT_TYPE == NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_SEARCH || _FRIEND_SLOT_TYPE == NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_SEARCH_RECOMMEND || _FRIEND_SLOT_TYPE == NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_RECEIVE_REQ || _FRIEND_SLOT_TYPE == NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_SENT_REQ);
			NKCUtil.SetGameobjectActive(m_cbtnBlockCancel, _FRIEND_SLOT_TYPE == NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_BLOCK_LIST);
			NKCUtil.SetGameobjectActive(m_csbtnDormitory, _FRIEND_SLOT_TYPE == NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_LIST || _FRIEND_SLOT_TYPE == NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_OFFICE);
			NKCUtil.SetGameobjectActive(m_objDomitoryLocked, !m_cNKMUserProfileData.hasOffice);
			break;
		}
		if (NKCFriendManager.IsBlockedUser(m_cNKMUserProfileData.commonProfile.friendCode))
		{
			m_cbtnChat.Lock();
		}
		else
		{
			m_cbtnChat.UnLock();
		}
		if (cNKMUserProfileData.guildData != null && cNKMUserProfileData.guildData.guildUid > 0)
		{
			NKCUtil.SetGameobjectActive(m_cbtnInvite, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_cbtnInvite, cNKMUserProfileData.commonProfile.userUid != NKCScenManager.CurrentUserData().m_UserUID && NKCGuildManager.HasGuild() && NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID).grade != GuildMemberGrade.Member);
		}
		for (int num = 0; num < m_lstEmblem.Count; num++)
		{
			NKCUISlot nKCUISlot = m_lstEmblem[num];
			if (num < cNKMUserProfileData.emblems.Count && cNKMUserProfileData.emblems[num] != null && cNKMUserProfileData.emblems[num].id > 0 && NKMItemManager.GetItemMiscTempletByID(cNKMUserProfileData.emblems[num].id) != null)
			{
				if (num <= 3)
				{
					nKCUISlot.SetMiscItemData(cNKMUserProfileData.emblems[num].id, cNKMUserProfileData.emblems[num].count, bShowName: false, bShowCount: true, bEnableLayoutElement: true, null);
					nKCUISlot.SetOnClickAction(NKCUISlot.SlotClickType.ItemBox);
				}
				else
				{
					nKCUISlot.SetEmpty();
				}
			}
			else
			{
				nKCUISlot.SetEmpty();
			}
		}
	}

	private void SetGuildData()
	{
		if (!(m_objGuild == null))
		{
			NKCUtil.SetGameobjectActive(m_objGuild, m_GuildSimpleData != null && m_GuildSimpleData.guildUid > 0);
			if (m_objGuild.activeSelf && m_GuildSimpleData != null)
			{
				m_BadgeUI.SetData(m_GuildSimpleData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, m_GuildSimpleData.guildName);
			}
		}
	}

	public bool IsSameProfile(int bossGroupID, long userUID)
	{
		if (bossGroupID == m_iFierceBossGroupID)
		{
			return userUID == m_lFierceUserUID;
		}
		return false;
	}

	public void SetData(NKMFierceProfileData fierceProfile)
	{
		if (fierceProfile == null)
		{
			return;
		}
		m_iFierceBossGroupID = fierceProfile.fierceBossGroupId;
		SetDeck(fierceProfile.profileDeck, bSetFirstDeckLeader: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_ROOT, bValue: true);
		NKCUtil.SetGameobjectActive(m_objPVPMenu, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFriendMenu, bValue: true);
		NKCUtil.SetGameobjectActive(m_cbtnChat, bValue: false);
		NKCUtil.SetGameobjectActive(m_cbtnBlock, bValue: true);
		NKCUtil.SetGameobjectActive(m_cbtnBlockCancel, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_INFO_POPUP_SQUAD_TOGGLE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_INFO_POPUP_TITLE, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_PROFILE_UNIT_POWER_ALL, bValue: true);
		int num = 0;
		int num2 = 0;
		NKMDummyUnitData[] list = fierceProfile.profileDeck.List;
		foreach (NKMDummyUnitData nKMDummyUnitData in list)
		{
			if (nKMDummyUnitData != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(nKMDummyUnitData.UnitId);
				if (unitStatTemplet != null)
				{
					num += unitStatTemplet.GetRespawnCost(num2 == 0, null, null);
					num2++;
				}
			}
		}
		NKCUtil.SetLabelText(m_ArmyOperationPower, fierceProfile.operationPower.ToString("N0"));
		NKCUtil.SetLabelText(m_ArmyAvgCost, $"{num / num2:0.00}");
	}

	private void SetDeck(NKMUserProfileData userData, DeckTab deckTab)
	{
		switch (deckTab)
		{
		case DeckTab.Rank:
			SetDeck(userData.leagueDeck);
			break;
		case DeckTab.Async:
			SetDeck(userData.defenceDeck);
			break;
		default:
			SetDeck(userData.leagueDeck);
			break;
		}
	}

	private void SetDeck(NKMDummyDeckData deckData, bool bSetFirstDeckLeader = false)
	{
		if (deckData != null)
		{
			NKCUtil.SetGameobjectActive(m_objTroopsPanel, bValue: true);
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(deckData.Ship.UnitId);
			if (unitTempletBase != null)
			{
				m_ANIM_SHIP_IMG.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
			}
			else
			{
				m_ANIM_SHIP_IMG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_TEXTURE", "NKM_DECK_VIEW_SHIP_UNKNOWN");
			}
			for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
			{
				if (i < 8)
				{
					NKMDummyUnitData nKMDummyUnitData = deckData.List[i];
					NKMUnitData cNKMUnitData = null;
					if (nKMDummyUnitData != null && nKMDummyUnitData.UnitId > 0)
					{
						cNKMUnitData = nKMDummyUnitData.ToUnitData(-1L);
					}
					m_lstNKCDeckViewUnitSlot[i].SetData(cNKMUnitData, bEnableButton: false);
					if (bSetFirstDeckLeader && i == 0)
					{
						m_lstNKCDeckViewUnitSlot[i].SetLeader(bLeader: true, bEffect: false);
					}
				}
			}
			if (NKCOperatorUtil.IsHide())
			{
				NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: false);
			}
			else if (deckData.operatorUnit != null)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(deckData.operatorUnit.UnitId);
				if (unitTempletBase2 != null)
				{
					m_OperatorSlot.SetData(unitTempletBase2, deckData.operatorUnit.UnitLevel);
				}
				else
				{
					m_OperatorSlot.SetEmpty();
				}
			}
			else
			{
				m_OperatorSlot.SetEmpty();
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objTroopsPanel, bValue: false);
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_FIREND_NO_EXIST_PVP_LOG;
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: true);
			NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: false);
		}
	}

	private void SetDeck(NKMAsyncDeckData cNKMAsyncDeckData)
	{
		if (cNKMAsyncDeckData != null)
		{
			NKCUtil.SetGameobjectActive(m_objTroopsPanel, bValue: true);
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMAsyncDeckData.ship.unitId);
			if (unitTempletBase != null)
			{
				m_ANIM_SHIP_IMG.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
			}
			else
			{
				m_ANIM_SHIP_IMG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_TEXTURE", "NKM_DECK_VIEW_SHIP_UNKNOWN");
			}
			for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
			{
				if (i < 8)
				{
					NKMAsyncUnitData nKMAsyncUnitData = cNKMAsyncDeckData.units[i];
					NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_lstNKCDeckViewUnitSlot[i];
					if (nKMAsyncUnitData.unitId > 0)
					{
						NKMUnitData cNKMUnitData = NKMDungeonManager.MakeUnitData(nKMAsyncUnitData, -1L);
						nKCDeckViewUnitSlot.SetData(cNKMUnitData, bEnableButton: false);
						nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect: false);
					}
					else
					{
						nKCDeckViewUnitSlot.SetPrivate();
					}
				}
			}
			if (NKCOperatorUtil.IsHide())
			{
				NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: false);
			}
			else if (cNKMAsyncDeckData.operatorUnit != null)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(cNKMAsyncDeckData.operatorUnit.id);
				if (unitTempletBase2 != null)
				{
					m_OperatorSlot.SetData(unitTempletBase2, cNKMAsyncDeckData.operatorUnit.level);
				}
				else
				{
					m_OperatorSlot.SetEmpty();
				}
			}
			else
			{
				m_OperatorSlot.SetEmpty();
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objTroopsPanel, bValue: false);
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_FIREND_NO_EXIST_ASYNC_LOG;
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: true);
			NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: false);
		}
	}

	private void SetPvP(NKMUserProfileData userData, DeckTab deckTab)
	{
		switch (deckTab)
		{
		case DeckTab.Rank:
			SetPvP(NKM_GAME_TYPE.NGT_PVP_RANK, userData.rankPvpData.seasonId, userData.rankPvpData.leagueTierId, userData.rankPvpData.score);
			return;
		case DeckTab.Async:
			SetPvP(NKM_GAME_TYPE.NGT_ASYNC_PVP, userData.asyncPvpData.seasonId, userData.asyncPvpData.leagueTierId, userData.asyncPvpData.score);
			return;
		}
		if (userData.rankPvpData.score < userData.asyncPvpData.score)
		{
			SetPvP(userData, DeckTab.Async);
		}
		else
		{
			SetPvP(userData, DeckTab.Rank);
		}
	}

	private void SetPvP(NKM_GAME_TYPE gameType, int userSeasonID, int userTierID, int userScore)
	{
		int num = NKCPVPManager.FindPvPSeasonID(gameType, NKCSynchronizedTime.GetServerUTCTime());
		if (num != userSeasonID)
		{
			userScore = NKCPVPManager.GetResetScore(num, userScore, gameType);
			NKMPvpRankTemplet rankTempletByScore = NKCPVPManager.GetRankTempletByScore(gameType, num, userScore);
			if (rankTempletByScore != null)
			{
				m_NKCUILeagueTier.SetUI(rankTempletByScore);
				m_NKM_UI_FRIEND_PROFILE_TIER_TIER.text = rankTempletByScore.GetLeagueName();
			}
			else
			{
				m_NKM_UI_FRIEND_PROFILE_TIER_TIER.text = "";
			}
		}
		else
		{
			NKMPvpRankTemplet rankTempletByTier = NKCPVPManager.GetRankTempletByTier(gameType, num, userTierID);
			if (rankTempletByTier != null)
			{
				m_NKCUILeagueTier.SetUI(rankTempletByTier);
				m_NKM_UI_FRIEND_PROFILE_TIER_TIER.text = rankTempletByTier.GetLeagueName();
			}
			else
			{
				m_NKM_UI_FRIEND_PROFILE_TIER_TIER.text = "";
			}
		}
		m_NKM_UI_FRIEND_PROFILE_TIER_RANK.text = "";
		m_NKM_UI_FRIEND_PROFILE_TIER_SCORE.text = userScore.ToString();
	}

	private void OnToggleChangedRank(bool set)
	{
		if (set)
		{
			m_deckTab = DeckTab.Rank;
			SetDeck(m_cNKMUserProfileData, m_deckTab);
			SetPvP(m_cNKMUserProfileData, m_deckTab);
		}
	}

	private void OnToggleChangedAsync(bool set)
	{
		if (set)
		{
			m_deckTab = DeckTab.Async;
			SetDeck(m_cNKMUserProfileData, m_deckTab);
			SetPvP(m_cNKMUserProfileData, m_deckTab);
		}
	}

	private void OnClickInviteCustomMatch()
	{
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public override void OnGuildDataChanged()
	{
		m_GuildUserInfo?.SetData(m_cNKMUserProfileData);
	}

	public void OnClickDormitory()
	{
		if (m_cNKMUserProfileData != null)
		{
			if (m_cNKMUserProfileData.hasOffice)
			{
				NKCPacketSender.Send_NKMPacket_OFFICE_STATE_REQ(m_cNKMUserProfileData.commonProfile.userUid);
			}
			else
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_OFFICE_FRIEND_CANNOT_VISIT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
		}
	}

	public void OnClickSimulatedPvpTest()
	{
		NKCPacketSender.Send_NKMPacket_START_SIMULATED_PVP_TEST_REQ(NKCScenManager.GetScenManager().GetMyUserData().UserProfileData.commonProfile.userUid, m_cNKMUserProfileData.commonProfile.userUid);
	}

	private void OnClickAssistUnitInfo()
	{
		if (m_SupportUnitData != null)
		{
			NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(new List<NKMUnitData> { m_SupportUnitData });
			NKCUICollectionUnitInfo.CheckInstanceAndOpen(m_SupportUnitData, openOption);
		}
	}

	private void OnClickDeckCopy()
	{
		if (m_cNKMUserProfileData == null)
		{
			return;
		}
		int shipID = 0;
		int operID = 0;
		List<int> list = new List<int>();
		int leaderIndex = 0;
		switch (m_deckTab)
		{
		case DeckTab.Async:
		{
			if (m_cNKMUserProfileData.defenceDeck == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_COPY_SQUAD_EMPTY"));
				return;
			}
			if (m_cNKMUserProfileData.defenceDeck.ship != null)
			{
				shipID = m_cNKMUserProfileData.defenceDeck.ship.unitId;
			}
			if (m_cNKMUserProfileData.defenceDeck.operatorUnit != null)
			{
				operID = m_cNKMUserProfileData.defenceDeck.operatorUnit.id;
			}
			for (int j = 0; j < 8; j++)
			{
				NKMAsyncUnitData nKMAsyncUnitData = m_cNKMUserProfileData.defenceDeck.units[j];
				if (nKMAsyncUnitData == null)
				{
					list.Add(0);
				}
				else
				{
					list.Add(nKMAsyncUnitData.unitId);
				}
			}
			leaderIndex = m_cNKMUserProfileData.defenceDeck.leaderIndex;
			break;
		}
		case DeckTab.Rank:
		{
			if (m_cNKMUserProfileData.leagueDeck == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_COPY_SQUAD_EMPTY"));
				return;
			}
			if (m_cNKMUserProfileData.leagueDeck.Ship != null)
			{
				shipID = m_cNKMUserProfileData.leagueDeck.Ship.UnitId;
			}
			if (m_cNKMUserProfileData.leagueDeck.operatorUnit != null)
			{
				operID = m_cNKMUserProfileData.leagueDeck.operatorUnit.UnitId;
			}
			for (int i = 0; i < 8; i++)
			{
				NKMDummyUnitData nKMDummyUnitData = m_cNKMUserProfileData.leagueDeck.List[i];
				if (nKMDummyUnitData == null)
				{
					list.Add(0);
				}
				else
				{
					list.Add(nKMDummyUnitData.UnitId);
				}
			}
			leaderIndex = m_cNKMUserProfileData.leagueDeck.LeaderIndex;
			break;
		}
		}
		NKCPopupDeckCopy.MakeDeckCopyCode(shipID, operID, list, leaderIndex);
	}
}
