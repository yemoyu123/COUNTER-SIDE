using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Guild;
using ClientPacket.Warfare;
using Cs.Core.Util;
using Cs.Logging;
using NKC.PacketHandler;
using NKM;
using NKM.Guild;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildCoop : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_COOP_FRONT";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public NKCPopupGuildCoopArtifactStorage m_ArtifactPopup;

	[Header("좌측 UI")]
	public Animator m_Animator;

	public Text m_lbTaskNum;

	public Text m_lbTitle;

	public Text m_lbSeasonName;

	public Text m_lbRemainTime;

	public Text m_lbRemainArenaCount;

	public Text m_lbRemainBossCount;

	public NKCUIComToggle m_tglBossAttackStop;

	public NKCUIComToggle m_tglBossAttackStart;

	public NKCUIComStateButton m_btnStatus;

	public NKCUIComStateButton m_btnSeasonReward;

	public GameObject m_objSeasonRewardRedDot;

	public NKCUIComStateButton m_btnViewArtifact;

	public GameObject m_objDisabled;

	[Header("길드 정보")]
	public NKCUIGuildBadge m_GuildBadge;

	public Text m_lbGuildName;

	public Text m_lbGuildLevel;

	[Header("공지")]
	public GameObject m_objNotice;

	public TMP_InputField m_IFNotice;

	public NKCUIComStateButton m_btnEditNotice;

	[Header("중앙 하단 보스정보")]
	public Text m_lbBossName;

	[Header("채팅창 호출")]
	public NKCUIComStateButton m_btnChat;

	public GameObject m_objNewCount;

	public Text m_lbNewCount;

	private NKCUIGuildCoopBack m_NKCUIGuildCoopBack;

	private GuildSeasonTemplet m_SeasonTemplet;

	private GuildSeasonTemplet.SessionData m_CurSessionData;

	private float m_fDeltaTime;

	private bool bSessionChanged;

	public static bool IsInstanceOpen
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsUIOpen;
			}
			return false;
		}
	}

	public static bool IsInstanceLoaded
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsLoadComplete;
			}
			return false;
		}
	}

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public override string MenuName => NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_BATTLE");

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID => "ARTICLE_GUILD_DUNGEON";

	public override List<int> UpsideMenuShowResourceList => new List<int>();

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIGuildCoop>("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_CONSORTIUM_COOP_FRONT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIGuildCoop GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIGuildCoop>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public void InitUI()
	{
		NKCUtil.SetGameobjectActive(m_btnChat, bValue: false);
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.HIDE_GUILD_CHAT))
		{
			m_btnChat.PointerClick.RemoveAllListeners();
			m_btnChat.PointerClick.AddListener(OnClickChat);
			NKCUtil.SetGameobjectActive(m_btnChat, bValue: true);
		}
		m_tglBossAttackStop.OnValueChanged.RemoveAllListeners();
		m_tglBossAttackStop.OnValueChanged.AddListener(OnAttackStop);
		m_tglBossAttackStart.OnValueChanged.RemoveAllListeners();
		m_tglBossAttackStart.OnValueChanged.AddListener(OnAttackStart);
		m_btnStatus.PointerClick.RemoveAllListeners();
		m_btnStatus.PointerClick.AddListener(OnClickStatus);
		m_btnSeasonReward.PointerClick.RemoveAllListeners();
		m_btnSeasonReward.PointerClick.AddListener(OnClickSeasonReward);
		m_btnViewArtifact.PointerClick.RemoveAllListeners();
		m_btnViewArtifact.PointerClick.AddListener(OnClickArtifact);
		m_IFNotice.onValidateInput = NKCFilterManager.FilterEmojiInput;
		m_IFNotice.onEndEdit.RemoveAllListeners();
		m_IFNotice.onEndEdit.AddListener(OnNoticeEndEdit);
		m_IFNotice.onValueChanged.RemoveAllListeners();
		m_IFNotice.onValueChanged.AddListener(OnNoticeChanged);
		m_IFNotice.characterLimit = 36;
		m_btnEditNotice.PointerClick.RemoveAllListeners();
		m_btnEditNotice.PointerClick.AddListener(OnClickEditGuildNotice);
		m_btnEditNotice.m_bGetCallbackWhileLocked = true;
		if (m_ArtifactPopup != null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			m_ArtifactPopup.InitUI();
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
		m_SeasonTemplet = GuildDungeonTempletManager.GetCurrentSeasonTemplet(ServiceTime.Recent);
		if (m_SeasonTemplet == null)
		{
			Log.Error($"시즌 진행중이 아님 - current : {ServiceTime.Recent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/GuildCoop/NKCUIGuildCoop.cs", 157);
			return;
		}
		m_CurSessionData = m_SeasonTemplet.GetCurrentSession(ServiceTime.Recent);
		m_CurSessionData.templet.GetDungeonList();
		GameObject orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<GameObject>(m_SeasonTemplet.GetSeasonBgPrefabName(), m_SeasonTemplet.GetSeasonBgPrefabName());
		if (orLoadAssetResource != null)
		{
			GameObject gameObject = Object.Instantiate(orLoadAssetResource);
			m_NKCUIGuildCoopBack = gameObject.GetComponent<NKCUIGuildCoopBack>();
			if (m_NKCUIGuildCoopBack != null)
			{
				m_NKCUIGuildCoopBack.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIMidCanvas), worldPositionStays: false);
				m_NKCUIGuildCoopBack.transform.position = Vector3.zero;
				m_NKCUIGuildCoopBack.Init(m_SeasonTemplet.Key, OnClickArena, OnClickBoss);
			}
		}
		m_Animator.Play("NKM_UI_CONSORTIUM_COOP_FRONT_IMTRO");
		NKCUtil.SetGameobjectActive(m_objNotice, bValue: true);
		m_GuildBadge.InitUI();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
		Object.Destroy(m_NKCUIGuildCoopBack.gameObject);
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
	}

	public void Open()
	{
		m_GuildBadge.SetData(NKCGuildManager.MyGuildData.badgeId);
		NKCUtil.SetLabelText(m_lbGuildLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, NKCGuildManager.MyGuildData.guildLevel));
		NKCUtil.SetLabelText(m_lbGuildName, NKCGuildManager.MyGuildData.name);
		if (NKCGuildCoopManager.m_cGuildRaidTemplet == null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageId());
		if (dungeonTempletBase != null)
		{
			NKCUtil.SetLabelText(m_lbBossName, dungeonTempletBase.GetDungeonName());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbBossName, "");
		}
		NKCUtil.SetLabelText(m_lbTitle, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_SESSION_PROGRESS_OF_ROUND_INFO, m_CurSessionData.templet.GetSessionIndex()));
		NKCUtil.SetLabelText(m_lbTaskNum, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_SESSION_PROGRESS_OF_ROUND_TASK_INFO, m_CurSessionData.templet.GetSessionIndex()));
		NKCUtil.SetLabelText(m_lbSeasonName, NKCStringTable.GetString(m_SeasonTemplet.GetSeasonNameID()));
		bSessionChanged = false;
		m_fDeltaTime = 0f;
		UpdateRemainTime();
		NKMGuildMemberData nKMGuildMemberData = NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
		if (nKMGuildMemberData != null && nKMGuildMemberData.grade <= GuildMemberGrade.Staff)
		{
			NKCUtil.SetGameobjectActive(m_tglBossAttackStop, bValue: true);
			NKCUtil.SetGameobjectActive(m_tglBossAttackStart, bValue: true);
			m_tglBossAttackStop.Select(NKCGuildCoopManager.BossOrderIndex == NKCGuildCoopManager.BOSS_ORDER_TYPE.STOP, bForce: true);
			m_tglBossAttackStart.Select(NKCGuildCoopManager.BossOrderIndex == NKCGuildCoopManager.BOSS_ORDER_TYPE.START, bForce: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_tglBossAttackStop, bValue: false);
			NKCUtil.SetGameobjectActive(m_tglBossAttackStart, bValue: false);
		}
		SetNewChatCount(NKCChatManager.GetUncheckedMessageCount(NKCGuildManager.MyData.guildUid));
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_NKCUIGuildCoopBack != null)
		{
			m_NKCUIGuildCoopBack.SetData();
			m_NKCUIGuildCoopBack.SetEnableDrag(bSet: true);
		}
		NKCUtil.SetGameobjectActive(m_btnStatus, NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_DUNGEON_LEADERBOARD));
		NKCUtil.SetGameobjectActive(m_btnSeasonReward, NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_DUNGEON_REWARD_SEASON));
		GuildDungeonMemberInfo guildDungeonMemberInfo = NKCGuildCoopManager.GetGuildDungeonMemberInfo().Find((GuildDungeonMemberInfo x) => x.profile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
		NKCUtil.SetGameobjectActive(m_objDisabled, guildDungeonMemberInfo == null);
		if (guildDungeonMemberInfo == null)
		{
			NKCUtil.SetLabelText(m_lbRemainArenaCount, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_COOP_FRONT_COUNT_ARENA, 0));
			NKCUtil.SetLabelText(m_lbRemainBossCount, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_COOP_FRONT_COUNT_RAID, 0));
			m_tglBossAttackStop.Lock();
			m_tglBossAttackStart.Lock();
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRemainArenaCount, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_COOP_FRONT_COUNT_ARENA, NKCGuildCoopManager.m_ArenaPlayableCount));
			NKCUtil.SetLabelText(m_lbRemainBossCount, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_COOP_FRONT_COUNT_RAID, NKCGuildCoopManager.m_BossData.playCount));
		}
		SetGuildCoopNotice();
		if (!IsInstanceOpen)
		{
			UIOpened();
		}
		NKCUtil.SetGameobjectActive(m_objSeasonRewardRedDot, NKCGuildCoopManager.CheckSeasonRewardEnable());
		if ((NKCGuildCoopManager.m_GuildDungeonState == GuildDungeonState.SessionOut || NKCGuildCoopManager.m_GuildDungeonState == GuildDungeonState.SeasonOut) && !NKCUIGuildCoopEnd.IsInstanceOpen)
		{
			NKCUIGuildCoopEnd.Instance.Open();
		}
		if (NKCGuildCoopManager.m_GuildDungeonState != GuildDungeonState.Adjust && NKCGuildCoopManager.m_bCanReward)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_SESSION_REWARD_REQ();
		}
	}

	public void SetGuildCoopNotice()
	{
		bool flag = false;
		if (NKCGuildManager.GetMyGuildGrade() != GuildMemberGrade.Member)
		{
			flag = true;
		}
		m_IFNotice.text = NKCFilterManager.CheckBadChat(NKCGuildManager.MyGuildData.dungeonNotice);
		m_IFNotice.interactable = flag;
		NKCUtil.SetGameobjectActive(m_btnEditNotice, flag);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.HIDE_GUILD_NOTICE))
		{
			m_IFNotice.interactable = false;
			NKCUtil.SetGameobjectActive(m_btnEditNotice, bValue: false);
		}
	}

	private void UpdateRemainTime()
	{
		NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GetRemainTimeStringEx(NKCGuildCoopManager.m_NextSessionStartDateUTC));
	}

	private void Update()
	{
		m_fDeltaTime += Time.deltaTime;
		if (!(m_fDeltaTime > 1f))
		{
			return;
		}
		m_fDeltaTime -= 1f;
		if (NKCGuildManager.LastDungeonNoticeChangedTimeUTC.Add(GuildTemplet.NoticeCooltime) > NKCSynchronizedTime.GetServerUTCTime())
		{
			m_IFNotice.enabled = false;
			m_btnEditNotice.Lock();
		}
		else
		{
			m_IFNotice.enabled = true;
			m_btnEditNotice.UnLock();
		}
		if (NKCGuildCoopManager.m_GuildDungeonState == GuildDungeonState.SeasonOut && NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_NextSessionStartDateUTC) && !NKCGuildCoopManager.HasNextSessionData(NKCGuildCoopManager.m_NextSessionStartDateUTC))
		{
			NKCUtil.SetLabelText(m_lbRemainTime, "");
		}
		else if (NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_NextSessionStartDateUTC))
		{
			if (!bSessionChanged)
			{
				bSessionChanged = true;
				NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_INFO_REQ(NKCGuildManager.MyData.guildUid);
			}
		}
		else
		{
			UpdateRemainTime();
		}
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

	public void OnCloseInfoPopup()
	{
		StopAllCoroutines();
		StartCoroutine(ProcessStartCamera(m_NKCUIGuildCoopBack.GetTargetPosition(0), bZoomin: false));
	}

	public void OnClickArena(GuildDungeonInfoTemplet templet)
	{
		if (templet != null)
		{
			if (NKCPopupGuildCoopBossInfo.IsInstanceOpen)
			{
				NKCPopupGuildCoopBossInfo.Instance.Close();
			}
			StopAllCoroutines();
			StartCoroutine(ProcessStartCamera(m_NKCUIGuildCoopBack.GetTargetPosition(templet.GetArenaIndex()), bZoomin: true));
			NKCPopupGuildCoopArenaInfo.Instance.Open(templet, OnStartArena);
		}
	}

	private IEnumerator ProcessStartCamera(Vector3 pinPos, bool bZoomin)
	{
		yield return null;
		if (bZoomin)
		{
			m_Animator.Play("NKM_UI_CONSORTIUM_COOP_FRONT_OUTRO");
			NKCCamera.TrackingPos(0.4f, pinPos.x + m_NKCUIGuildCoopBack.m_fCameraXPosAddValue, pinPos.y, m_NKCUIGuildCoopBack.m_fCameraZPosZoomIn);
			m_NKCUIGuildCoopBack.SetEnableDrag(bSet: false);
			NKCUtil.SetGameobjectActive(m_objNotice, bValue: false);
		}
		else
		{
			m_Animator.Play("NKM_UI_CONSORTIUM_COOP_FRONT_INTRO");
			NKCCamera.TrackingPos(0.4f, pinPos.x, pinPos.y, m_NKCUIGuildCoopBack.m_fCameraZPosZoomOut);
			m_NKCUIGuildCoopBack.SetEnableDrag(bSet: true);
			NKCUtil.SetGameobjectActive(m_objNotice, bValue: true);
		}
		yield return new WaitForSecondsWithCancel(1.6f, CanSkipSDCamera, null);
	}

	private bool CanSkipSDCamera()
	{
		return Input.anyKeyDown;
	}

	private void OnStartArena(NKMDungeonTempletBase templet, int arenaIdx)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(NKCGuildCoopManager.CanStartArena(arenaIdx)))
		{
			return;
		}
		if (NKCScenManager.GetScenManager().WarfareGameData != null && NKCScenManager.GetScenManager().WarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP && NKCScenManager.GetScenManager().WarfareGameData.warfareTempletID > 0)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EPISODE_GIVE_UP_WARFARE, OnClickOkGiveUpINGWarfare);
			return;
		}
		if (NKCPopupGuildCoopArenaInfo.IsInstanceOpen)
		{
			NKCPopupGuildCoopArenaInfo.Instance.Close();
		}
		NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(templet, DeckContents.GUILD_COOP);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
	}

	private void OnClickOkGiveUpINGWarfare()
	{
		NKMPacket_WARFARE_GAME_GIVE_UP_REQ packet = new NKMPacket_WARFARE_GAME_GIVE_UP_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void OnClickBoss(int stageID)
	{
		GuildSeasonTemplet currentSeasonTemplet = GuildDungeonTempletManager.GetCurrentSeasonTemplet(ServiceTime.Recent);
		if (currentSeasonTemplet == null)
		{
			return;
		}
		if (NKCPopupGuildCoopArenaInfo.IsInstanceOpen)
		{
			NKCPopupGuildCoopArenaInfo.Instance.Close();
		}
		StopAllCoroutines();
		StartCoroutine(ProcessStartCamera(m_NKCUIGuildCoopBack.GetTargetPosition(0, bIsArena: false), bZoomin: true));
		int seasonRaidGroup = currentSeasonTemplet.GetSeasonRaidGroup();
		int lastBossStageIndex = -1;
		List<GuildRaidTemplet> raidTempletList = GuildDungeonTempletManager.GetRaidTempletList(currentSeasonTemplet.GetSeasonRaidGroup());
		if (raidTempletList != null)
		{
			lastBossStageIndex = raidTempletList.Max((GuildRaidTemplet e) => e.GetStageIndex());
		}
		NKCPopupGuildCoopBossInfo.Instance.Open(GuildDungeonTempletManager.GetGuildRaidTemplet(seasonRaidGroup, NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageId()), OnStartBoss, lastBossStageIndex);
	}

	private void OnStartBoss()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID_READY().SetRaidUID(NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageId());
		NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID_READY().SetGuildRaid(bGuildRaid: true);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID_READY);
	}

	public void OnClickChat()
	{
		NKCUtil.SetGameobjectActive(m_objNewCount, bValue: false);
		NKCPopupGuildChat.Instance.Open(NKCGuildManager.MyData.guildUid);
	}

	private void OnAttackStop(bool bValue)
	{
		if (bValue)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ(NKCGuildManager.MyData.guildUid, 1);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ(NKCGuildManager.MyData.guildUid, 0);
		}
	}

	private void OnAttackStart(bool bValue)
	{
		if (bValue)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ(NKCGuildManager.MyData.guildUid, 2);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ(NKCGuildManager.MyData.guildUid, 0);
		}
	}

	public void OnClickStatus()
	{
		NKCPopupGuildCoopStatus.Instance.Open();
	}

	public void OnClickSeasonReward()
	{
		NKCPopupGuildCoopSeasonReward.Instance.Open(RefreshSeasonReward);
	}

	public void OnClickArtifact()
	{
		if (!m_ArtifactPopup.gameObject.activeSelf)
		{
			m_ArtifactPopup.Open();
		}
		else
		{
			m_ArtifactPopup.Close();
		}
	}

	private void OnNoticeChanged(string notice)
	{
		if (!string.IsNullOrEmpty(notice))
		{
			notice = NKCFilterManager.RemoveNewLine(notice);
			m_IFNotice.text = NKCFilterManager.CheckBadChat(notice);
		}
	}

	private void OnNoticeEndEdit(string notice)
	{
		if (!string.IsNullOrEmpty(notice))
		{
			m_IFNotice.text = NKCFilterManager.CheckBadChat(notice);
			if (NKCInputManager.IsChatSubmitEnter())
			{
				OnClickEditGuildNotice();
				EventSystem.current.SetSelectedGameObject(null);
			}
		}
	}

	public void OnClickEditGuildNotice()
	{
		if (m_btnEditNotice.m_bLock)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_NOTICE_CHANGE_COOLTIME_TOAST_TEXT"));
		}
		else if (!string.IsNullOrWhiteSpace(m_IFNotice.text) && !string.Equals(m_IFNotice.text, NKCGuildManager.MyGuildData.dungeonNotice))
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_REQ(NKCGuildManager.MyData.guildUid, m_IFNotice.text);
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_NOTICE_NOT_CHANGE_TOAST_TEXT"));
		}
	}

	public void RefreshSeasonReward()
	{
		NKCUtil.SetGameobjectActive(m_objSeasonRewardRedDot, NKCGuildCoopManager.CheckSeasonRewardEnable());
	}

	public void RefreshArenaSlot(int arenaIdx)
	{
		m_NKCUIGuildCoopBack?.RefreshArenaSlot(arenaIdx);
	}

	public void RefreshBossSlot()
	{
		m_NKCUIGuildCoopBack?.RefreshBossSlot();
	}

	public void RefreshBossOrder()
	{
		m_tglBossAttackStop.Select(NKCGuildCoopManager.BossOrderIndex == NKCGuildCoopManager.BOSS_ORDER_TYPE.STOP, bForce: true);
		m_tglBossAttackStart.Select(NKCGuildCoopManager.BossOrderIndex == NKCGuildCoopManager.BOSS_ORDER_TYPE.START, bForce: true);
	}
}
