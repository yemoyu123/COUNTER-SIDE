using System;
using System.Collections.Generic;
using ClientPacket.Guild;
using Cs.Core.Util;
using NKM;
using NKM.Guild;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildLobby : NKCUIBase
{
	public enum GUILD_LOBBY_UI_TYPE
	{
		None,
		Info,
		Member,
		Mission,
		Point,
		Ranking,
		Manage,
		Invite
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_LOBBY";

	[Header("좌측 정보")]
	public NKCUIGuildBadge m_BadgeUI;

	public Text m_lbName;

	public Text m_lbLevel;

	public Text m_lbExp;

	public NKCUIComStateButton m_btnLvInfo;

	public Image m_imgExpGauge;

	public Text m_lbLeaderName;

	public Text m_lbMemberCount;

	public Text m_lbAttendanceCount;

	public NKCUIComStateButton m_btnHelp;

	public NKCUIComStateButton m_btnDonation;

	public GameObject m_objAttendanceRedDot;

	[Header("해체 유예기간")]
	public GameObject m_objBreakupDelay;

	public Text m_lbBreakupDelayTime;

	public NKCUIComStateButton m_btnBreakup;

	public Text m_lbBreakupBtnText;

	public NKCUIComStateButton m_btnBreakupClose;

	[Header("상단 탭")]
	public List<NKCUIGuildLobbyTab> m_lstTab = new List<NKCUIGuildLobbyTab>();

	[Header("우측 영역")]
	public NKCUIGuildLobbyInfo m_GuildLobbyInfo;

	public NKCUIGuildLobbyMember m_GuildLobbyMember;

	public NKCUIGuildLobbyMission m_GuildLobbyMission;

	public NKCUIGuildLobbyWelfare m_GuildLobbyWelfare;

	public NKCUIGuildLobbyRank m_GuildLobbyRank;

	public NKCUIGuildLobbyManage m_GuildLobbyManage;

	public GameObject m_objNone;

	[Header("채팅창 호출")]
	public NKCUIComStateButton m_btnChat;

	public GameObject m_objNewCount;

	public Text m_lbNewCount;

	private NKMGuildData m_GuildData;

	private GUILD_LOBBY_UI_TYPE m_CurrentUIType;

	private DateTime m_tExpireTime;

	private float m_fDeltaTime;

	public override List<int> UpsideMenuShowResourceList => new List<int> { 21, 1, 2, 101 };

	public override string MenuName => NKCUtilString.GET_STRING_CONSORTIUM_INTRO;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID => "ARTICLE_GUILD_INFORMATION";

	public static NKCAssetResourceData OpenInstanceAsync()
	{
		return NKCUIBase.OpenInstanceAsync<NKCUIGuildLobby>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_LOBBY");
	}

	public static bool CheckInstanceLoaded(NKCAssetResourceData loadResourceData, out NKCUIGuildLobby retVal)
	{
		return NKCUIBase.CheckInstanceLoaded<NKCUIGuildLobby>(loadResourceData, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), out retVal);
	}

	public void CloseInstance()
	{
		int num = NKCAssetResourceManager.CloseResource("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_LOBBY");
		Debug.Log($"NKCUIConsortiumLobby close resource retval is {num}");
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public override void UnHide()
	{
		base.UnHide();
		for (int i = 0; i < m_lstTab.Count; i++)
		{
			m_lstTab[i].CheckRedDot();
			m_lstTab[i].UpdateState();
		}
		if (m_GuildLobbyInfo.gameObject.activeSelf)
		{
			m_GuildLobbyInfo.ResetPosition();
		}
		else if (m_GuildLobbyMission.gameObject.activeSelf)
		{
			m_GuildLobbyMission.SetData();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_CurrentUIType = GUILD_LOBBY_UI_TYPE.None;
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
	}

	public void InitUI()
	{
		for (int i = 0; i < m_lstTab.Count; i++)
		{
			m_lstTab[i].InitUI(OnClickTab);
			m_lstTab[i].CheckRedDot();
			m_lstTab[i].UpdateState();
		}
		m_GuildLobbyInfo.InitUI();
		m_GuildLobbyMember.InitUI();
		m_GuildLobbyManage.InitUI();
		m_GuildLobbyWelfare.InitUI();
		m_GuildLobbyMission.InitUI();
		m_GuildLobbyRank.InitUI();
		m_btnLvInfo.PointerClick.RemoveAllListeners();
		m_btnLvInfo.PointerClick.AddListener(OnClickLvInfo);
		m_btnHelp.PointerClick.RemoveAllListeners();
		m_btnHelp.PointerClick.AddListener(OnClickHelp);
		m_btnDonation?.PointerClick.RemoveAllListeners();
		m_btnDonation?.PointerClick.AddListener(OnClickDonation);
		m_btnBreakup.PointerClick.RemoveAllListeners();
		m_btnBreakup.PointerClick.AddListener(OnClickBreakup);
		m_btnBreakupClose.PointerClick.RemoveAllListeners();
		m_btnBreakupClose.PointerClick.AddListener(OnClickBreakupClose);
		NKCUtil.SetGameobjectActive(m_btnChat, bValue: false);
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.HIDE_GUILD_CHAT))
		{
			NKCUtil.SetGameobjectActive(m_btnChat, bValue: true);
			m_btnChat.PointerClick.RemoveAllListeners();
			m_btnChat.PointerClick.AddListener(OnClickChat);
		}
		m_CurrentUIType = GUILD_LOBBY_UI_TYPE.None;
	}

	public void Open(GUILD_LOBBY_UI_TYPE reservedTab = GUILD_LOBBY_UI_TYPE.Info)
	{
		m_GuildData = NKCGuildManager.MyGuildData;
		if (m_GuildData == null)
		{
			Debug.LogError("길드정보 없음");
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		m_fDeltaTime = 0f;
		SetGuildBasicData();
		SetNewChatCount(NKCChatManager.GetUncheckedMessageCount(NKCGuildManager.MyData.guildUid));
		m_lstTab[0].m_tgl.Select(bSelect: true, bForce: true, bImmediate: true);
		m_CurrentUIType = GUILD_LOBBY_UI_TYPE.None;
		if (reservedTab == GUILD_LOBBY_UI_TYPE.Member)
		{
			base.gameObject.SetActive(value: true);
		}
		OnClickTab(reservedTab);
		UIOpened();
		NKMGuildMemberData nKMGuildMemberData = m_GuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
		if (!nKMGuildMemberData.HasAttendanceData(ServiceTime.Recent))
		{
			Debug.Log($"[GuildAttendence] CurrentTime : {ServiceTime.Recent}, LastAttendanceTime : {nKMGuildMemberData.lastAttendanceDate}");
			NKCPacketSender.Send_NKMPacket_GUILD_ATTENDANCE_REQ(m_GuildData.guildUid);
		}
	}

	private void SetGuildBasicData()
	{
		m_BadgeUI.SetData(m_GuildData.badgeId);
		NKCUtil.SetLabelText(m_lbName, m_GuildData.name);
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, m_GuildData.guildLevel));
		long guildLevelExp = m_GuildData.guildLevelExp;
		long guildExpRequired = GuildExpTemplet.Find(m_GuildData.guildLevel).GuildExpRequired;
		float num = (float)guildLevelExp / (float)guildExpRequired * 100f;
		NKCUtil.SetLabelText(m_lbExp, string.Format("({0}%)", num.ToString("F1")));
		m_imgExpGauge.fillAmount = num / 100f;
		NKMGuildMemberData nKMGuildMemberData = m_GuildData.members.Find((NKMGuildMemberData x) => x.grade == GuildMemberGrade.Master);
		NKCUtil.SetLabelText(m_lbLeaderName, nKMGuildMemberData?.commonProfile.nickname);
		NKCUtil.SetLabelText(m_lbMemberCount, $"{m_GuildData.members.Count}/{NKMTempletContainer<GuildExpTemplet>.Find(m_GuildData.guildLevel).MaxMemberCount}");
		NKMGuildAttendanceData todayAttendance = m_GuildData.GetTodayAttendance(ServiceTime.Recent);
		NKCUtil.SetLabelText(m_lbAttendanceCount, $"{todayAttendance?.count ?? 0}/{m_GuildData.members.Count}");
		NKCUtil.SetGameobjectActive(m_objBreakupDelay, m_GuildData.guildState == GuildState.Closing);
		if (m_objBreakupDelay.activeSelf)
		{
			if (m_GuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID).grade == GuildMemberGrade.Master)
			{
				NKCUtil.SetLabelText(m_lbBreakupBtnText, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_TITLE_DESC);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbBreakupBtnText, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_INFORMATION_BTN_TEXT);
			}
			m_tExpireTime = m_GuildData.closingTime;
			SetRemainTime();
		}
		m_btnDonation.UnLock();
		for (int num2 = 0; num2 < m_lstTab.Count; num2++)
		{
			m_lstTab[num2].CheckRedDot();
		}
	}

	private void SetRemainTime()
	{
		if (NKCGuildManager.MyGuildData != null && NKCGuildManager.MyGuildData.guildState == GuildState.Closing)
		{
			TimeSpan timeSpan = m_tExpireTime - ServiceTime.Recent;
			if (timeSpan.TotalSeconds > 1.0)
			{
				NKCUtil.SetLabelText(m_lbBreakupDelayTime, string.Format(NKCUtilString.GET_STRING_SHOP_CHAIN_NEXT_RESET_ONE_PARAM_CLOSE, NKCUtilString.GetRemainTimeString(timeSpan, 2)));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbBreakupDelayTime, NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_END_SOON"));
			}
		}
	}

	private void OnClickTab(GUILD_LOBBY_UI_TYPE uiType, bool bForce = false)
	{
		if (m_CurrentUIType == uiType && !bForce)
		{
			return;
		}
		m_CurrentUIType = uiType;
		NKCUtil.SetGameobjectActive(m_GuildLobbyInfo, uiType == GUILD_LOBBY_UI_TYPE.Info);
		NKCUtil.SetGameobjectActive(m_GuildLobbyMember, uiType == GUILD_LOBBY_UI_TYPE.Member);
		NKCUtil.SetGameobjectActive(m_GuildLobbyWelfare, uiType == GUILD_LOBBY_UI_TYPE.Point);
		NKCUtil.SetGameobjectActive(m_GuildLobbyMission, uiType == GUILD_LOBBY_UI_TYPE.Mission);
		NKCUtil.SetGameobjectActive(m_GuildLobbyRank, uiType == GUILD_LOBBY_UI_TYPE.Ranking);
		NKCUtil.SetGameobjectActive(m_GuildLobbyManage, uiType == GUILD_LOBBY_UI_TYPE.Manage);
		for (int i = 0; i < m_lstTab.Count; i++)
		{
			if (m_lstTab[i].GetTabType() == uiType)
			{
				m_lstTab[i].m_tgl.Select(bSelect: true, bForce: true, bImmediate: true);
			}
		}
		switch (uiType)
		{
		case GUILD_LOBBY_UI_TYPE.Info:
		{
			NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
			NKMGuildMemberData nKMGuildMemberData = m_GuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
			if (nKMGuildMemberData != null)
			{
				m_GuildLobbyInfo.SetData(nKMGuildMemberData.grade, OnClickTab);
			}
			break;
		}
		case GUILD_LOBBY_UI_TYPE.Member:
			NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
			m_GuildLobbyMember.SetData(m_GuildData);
			break;
		case GUILD_LOBBY_UI_TYPE.Point:
			if (NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_POINT))
			{
				NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
				m_GuildLobbyWelfare.SetData();
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_GuildLobbyWelfare, bValue: false);
				NKCUtil.SetGameobjectActive(m_objNone, bValue: true);
			}
			break;
		case GUILD_LOBBY_UI_TYPE.Mission:
			if (NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_MISSION))
			{
				NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
				m_GuildLobbyMission.SetData();
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_GuildLobbyMission, bValue: false);
				NKCUtil.SetGameobjectActive(m_objNone, bValue: true);
			}
			break;
		case GUILD_LOBBY_UI_TYPE.Ranking:
			if (NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_RANKING))
			{
				NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
				m_GuildLobbyRank.SetData();
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_GuildLobbyRank, bValue: false);
				NKCUtil.SetGameobjectActive(m_objNone, bValue: true);
			}
			break;
		case GUILD_LOBBY_UI_TYPE.Manage:
			NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
			m_GuildLobbyManage.SetData(OnClickTab);
			break;
		default:
			NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
			break;
		}
	}

	private void OnClickLvInfo()
	{
		NKCPopupGuildLvInfo.Instance.Open();
	}

	private void OnClickHelp()
	{
		int lastAttendanceCount = m_GuildData.GetYesterdayAttendance(ServiceTime.Recent)?.count ?? 0;
		NKCPopupGuildAttendance.Instance.Open(lastAttendanceCount);
	}

	private void OnClickDonation()
	{
		NKCPopupGuildDonation.Instance.Open();
	}

	private void OnClickBreakup()
	{
		if (NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID).grade == GuildMemberGrade.Master)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_BODY_DESC, delegate
			{
				NKCPacketSender.Send_NKMPacket_GUILD_CLOSE_CANCEL_REQ(NKCGuildManager.MyData.guildUid);
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_HANDOVER_POPUP_BODY_DESC, delegate
			{
				NKCPacketSender.Send_NKMPacket_GUILD_MASTER_MIGRATION_REQ(NKCGuildManager.MyData.guildUid);
			});
		}
	}

	private void OnClickBreakupClose()
	{
		NKCUtil.SetGameobjectActive(m_objBreakupDelay, bValue: false);
	}

	private void OnClickChat()
	{
		NKCUtil.SetGameobjectActive(m_objNewCount, bValue: false);
		NKCPopupGuildChat.Instance.Open(m_GuildData.guildUid);
	}

	public void SetNewChatCount(int count)
	{
		if (count > 0)
		{
			NKCUtil.SetGameobjectActive(m_objNewCount, bValue: true);
			NKCUtil.SetLabelText(m_lbNewCount, count.ToString());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objNewCount, bValue: false);
		}
	}

	public override void OnCompanyBuffUpdate(NKMUserData userData)
	{
		base.OnCompanyBuffUpdate(userData);
		if (m_CurrentUIType == GUILD_LOBBY_UI_TYPE.Point)
		{
			GUILD_LOBBY_UI_TYPE currentUIType = m_CurrentUIType;
			m_CurrentUIType = GUILD_LOBBY_UI_TYPE.None;
			OnClickTab(currentUIType, bForce: true);
		}
	}

	public override void OnGuildDataChanged()
	{
		m_GuildData = NKCGuildManager.MyGuildData;
		if (m_GuildData != null)
		{
			SetGuildBasicData();
			GUILD_LOBBY_UI_TYPE currentUIType = m_CurrentUIType;
			m_CurrentUIType = GUILD_LOBBY_UI_TYPE.None;
			OnClickTab(currentUIType, bForce: true);
		}
	}

	private void Update()
	{
		if (m_lbBreakupDelayTime.gameObject.activeInHierarchy)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRemainTime();
			}
		}
	}

	public void OpenShop()
	{
		m_GuildLobbyInfo?.OpenShop();
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		switch (hotkey)
		{
		case HotkeyEventType.NextTab:
			return ToNextTab();
		case HotkeyEventType.PrevTab:
			return ToPrevTab();
		case HotkeyEventType.ShowHotkey:
			if (m_lstTab.Count > 0)
			{
				NKCUIComHotkeyDisplay.OpenInstance(m_lstTab[0]?.m_tgl?.m_ToggleGroup?.transform, HotkeyEventType.NextTab);
			}
			return false;
		default:
			return false;
		}
	}

	private bool ToNextTab()
	{
		switch (m_CurrentUIType)
		{
		case GUILD_LOBBY_UI_TYPE.Info:
			OnClickTab(GUILD_LOBBY_UI_TYPE.Member);
			return true;
		case GUILD_LOBBY_UI_TYPE.Member:
			OnClickTab(GUILD_LOBBY_UI_TYPE.Mission);
			return true;
		case GUILD_LOBBY_UI_TYPE.Mission:
			OnClickTab(GUILD_LOBBY_UI_TYPE.Point);
			return true;
		case GUILD_LOBBY_UI_TYPE.Point:
			OnClickTab(GUILD_LOBBY_UI_TYPE.Ranking);
			return true;
		case GUILD_LOBBY_UI_TYPE.Ranking:
			OnClickTab(GUILD_LOBBY_UI_TYPE.Info);
			return true;
		default:
			return false;
		}
	}

	private bool ToPrevTab()
	{
		switch (m_CurrentUIType)
		{
		case GUILD_LOBBY_UI_TYPE.Info:
			OnClickTab(GUILD_LOBBY_UI_TYPE.Ranking);
			return true;
		case GUILD_LOBBY_UI_TYPE.Member:
			OnClickTab(GUILD_LOBBY_UI_TYPE.Info);
			return true;
		case GUILD_LOBBY_UI_TYPE.Mission:
			OnClickTab(GUILD_LOBBY_UI_TYPE.Member);
			return true;
		case GUILD_LOBBY_UI_TYPE.Point:
			OnClickTab(GUILD_LOBBY_UI_TYPE.Mission);
			return true;
		case GUILD_LOBBY_UI_TYPE.Ranking:
			OnClickTab(GUILD_LOBBY_UI_TYPE.Point);
			return true;
		default:
			return false;
		}
	}
}
