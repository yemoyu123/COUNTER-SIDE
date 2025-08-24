using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Pvp;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyAsyncV2 : MonoBehaviour
{
	public enum PVP_ASYNC_TYPE
	{
		PAT_BATTLE,
		PAT_NPC,
		PAT_REVENGE,
		PAT_RANK,
		MAX
	}

	public Animator m_amtorRankCenter;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdv1(off)")]
	public GameObject m_objNKM_UI_GAUNTLET_RANK_UPSIDEMENU;

	public GameObject m_objNKM_UI_GAUNTLET_RANK_ASYNC_SCROLL;

	public GameObject m_objNKM_UI_GAUNTLET_RANK_ALL_SCROLL;

	public GameObject m_objNKM_UI_GAUNTLET_RANK_FRIEND_SCROLL;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdv2")]
	public GameObject m_objNKM_UI_GAUNTLET_RANK_UPSIDEMENU_NEW;

	[Space]
	public NKCUIComToggle m_ctglAsyncBattle;

	public NKCUIComToggle m_ctglAsyncNPC;

	public NKCUIComToggle m_ctglAsyncRevenge;

	public NKCUIComToggle m_ctglAsyncRank;

	[Space]
	public LoopVerticalScrollRect m_lvsrAsyncBattle;

	public LoopVerticalScrollRect m_lvsrAsyncNPC;

	public LoopVerticalScrollRect m_lvsrAsyncRevenge;

	public LoopVerticalScrollRect m_lvsrAsyncRank;

	[Space]
	public LoopHorizontalScrollRect m_lhsrNPCSub;

	[Header("\ufffd\ufffdũ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffdƮ")]
	public GameObject m_objAsyncBattle;

	public GameObject m_objAsyncNPC;

	public GameObject m_objAsyncRevenge;

	public GameObject m_objAsyncRank;

	[Header("\ufffd\ufffdŷ \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd")]
	public NKCUIComToggle m_ctglAll;

	public NKCUIComToggle m_ctglMyLeague;

	public NKCUIComToggle m_ctglFriend;

	[Header("\ufffd߾\ufffd \ufffdϴ\ufffd")]
	public Text m_lbRemainRankTime;

	public NKCUIComStateButton m_csbtnAsyncRefresh;

	public GameObject m_objAsyncRefreshOn;

	public GameObject m_objAsyncRefreshOff;

	public Text m_txtAsyncRefreshTimer;

	[Header("etc")]
	public GameObject m_objAsyncLock;

	public Text m_txtAsyncLock;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public NKCUIGauntletLobbyRightSideAsync m_RightSide;

	private NKCPopupGauntletLeagueGuide m_NKCPopupGauntletLeagueGuide;

	private bool m_bFirstOpen = true;

	private bool m_bPrepareLoopScrollCells;

	private List<AsyncPvpTarget> m_listAsyncTarget = new List<AsyncPvpTarget>();

	private List<RevengePvpTarget> m_listRevengeTarget = new List<RevengePvpTarget>();

	private float m_fPrevUpdateTime;

	private float m_fAsyncRefreshTimer;

	private float m_fAsyncNpcBotTimer;

	private bool m_bPlayIntro = true;

	private const int ASYNC_REFRESH_TIMER = 60;

	private bool[] m_arRankREQ = new bool[4];

	private bool[] m_arAllRankREQ = new bool[4];

	private bool[] m_arOpened = new bool[4];

	private PVP_ASYNC_TYPE m_CurAsyncType;

	private RANK_TYPE m_CurRankType;

	private Dictionary<RANK_TYPE, List<NKMUserSimpleProfileData>> m_dicUserSimpleData = new Dictionary<RANK_TYPE, List<NKMUserSimpleProfileData>>();

	private int m_iCurNPCBotTier;

	private List<NpcPvpTarget> m_lstCurNpcBot = new List<NpcPvpTarget>();

	private int m_iNPCBotMaxOpendTier;

	private bool m_bSendBattleListReq;

	private bool m_bSendRankListReq;

	private bool m_bSendNPCListReq;

	private bool m_bNotSetDefenceDeck;

	private List<NKCUIGauntletLobbyAsyncSubTab> m_lstSubTalSlots = new List<NKCUIGauntletLobbyAsyncSubTab>();

	private int m_iNewOpendNpcTier;

	public void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnAsyncRefresh, OnClickAsyncRefresh);
		if (null != m_lvsrAsyncBattle)
		{
			m_lvsrAsyncBattle.dOnGetObject += GetSlotAsyncBattle;
			m_lvsrAsyncBattle.dOnReturnObject += ReturnSlotAsyncBattle;
			m_lvsrAsyncBattle.dOnProvideData += ProvideDataAsyncBattle;
			m_lvsrAsyncBattle.ContentConstraintCount = 1;
			NKCUtil.SetScrollHotKey(m_lvsrAsyncBattle);
		}
		if (null != m_lvsrAsyncNPC)
		{
			m_lvsrAsyncNPC.dOnGetObject += GetSlotAsyncNPC;
			m_lvsrAsyncNPC.dOnReturnObject += ReturnSlotAsyncNPC;
			m_lvsrAsyncNPC.dOnProvideData += ProvideDataAsyncNPC;
			m_lvsrAsyncNPC.ContentConstraintCount = 1;
			NKCUtil.SetScrollHotKey(m_lvsrAsyncNPC);
		}
		if (null != m_lvsrAsyncRevenge)
		{
			m_lvsrAsyncRevenge.dOnGetObject += GetSlotAsyncRevenge;
			m_lvsrAsyncRevenge.dOnReturnObject += ReturnSlotAsyncRevenge;
			m_lvsrAsyncRevenge.dOnProvideData += ProvideDataAsyncRevenge;
			m_lvsrAsyncRevenge.ContentConstraintCount = 1;
			NKCUtil.SetScrollHotKey(m_lvsrAsyncRevenge);
		}
		if (null != m_lvsrAsyncRank)
		{
			m_lvsrAsyncRank.dOnGetObject += GetSlotAsyncRank;
			m_lvsrAsyncRank.dOnReturnObject += ReturnSlotAsyncRank;
			m_lvsrAsyncRank.dOnProvideData += ProvideDataAsyncRank;
			m_lvsrAsyncRank.ContentConstraintCount = 1;
			NKCUtil.SetScrollHotKey(m_lvsrAsyncRank);
		}
		if (null != m_lhsrNPCSub)
		{
			m_lhsrNPCSub.dOnGetObject += GetSlotAsyncNPCSub;
			m_lhsrNPCSub.dOnReturnObject += ReturnSlotAsyncNPCSub;
			m_lhsrNPCSub.dOnProvideData += ProvideDataAsyncNPCSub;
			m_lhsrNPCSub.ContentConstraintCount = 1;
		}
		m_RightSide.InitUI();
		m_RightSide.SetCallback(CloseDefenseDeck);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglAsyncBattle, delegate(bool _b)
		{
			OnToggleAsyncTab(_b, PVP_ASYNC_TYPE.PAT_BATTLE);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglAsyncNPC, delegate(bool _b)
		{
			OnToggleAsyncTab(_b, PVP_ASYNC_TYPE.PAT_NPC);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglAsyncRevenge, delegate(bool _b)
		{
			OnToggleAsyncTab(_b, PVP_ASYNC_TYPE.PAT_REVENGE);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglAsyncRank, delegate(bool _b)
		{
			OnToggleAsyncTab(_b, PVP_ASYNC_TYPE.PAT_RANK);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglAll, delegate(bool _b)
		{
			OnToggleRankTab(_b, RANK_TYPE.ALL);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglMyLeague, delegate(bool _b)
		{
			OnToggleRankTab(_b, RANK_TYPE.MY_LEAGUE);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglFriend, delegate(bool _b)
		{
			OnToggleRankTab(_b, RANK_TYPE.FRIEND);
		});
		NKCUtil.SetGameobjectActive(m_ctglAsyncRevenge.gameObject, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_REVENGE_MODE));
		NKCUtil.SetGameobjectActive(m_ctglAsyncNPC.gameObject, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_REVENGE_MODE));
		m_CurAsyncType = PVP_ASYNC_TYPE.PAT_BATTLE;
		m_CurRankType = RANK_TYPE.ALL;
	}

	private void Update()
	{
		if (m_fPrevUpdateTime + 1f < Time.time)
		{
			m_fPrevUpdateTime = Time.time;
			UpdateRemainTimeUI();
			UpdateAsyncRefreshTime();
			m_RightSide.UpdateRankPVPPointUI();
		}
	}

	private void UpdateRemainTimeUI()
	{
		if (m_CurAsyncType == PVP_ASYNC_TYPE.PAT_REVENGE)
		{
			return;
		}
		NKMPvpRankSeasonTemplet pvpAsyncSeasonTemplet = NKCPVPManager.GetPvpAsyncSeasonTemplet(GetCurrentSeasonID());
		if (pvpAsyncSeasonTemplet == null)
		{
			return;
		}
		if (!NKCPVPManager.IsRewardWeek(pvpAsyncSeasonTemplet, NKCPVPManager.WeekCalcStartDateUtc))
		{
			if (!pvpAsyncSeasonTemplet.CheckMySeason(NKCSynchronizedTime.GetServerUTCTime()))
			{
				m_lbRemainRankTime.text = NKCUtilString.GET_STRING_GAUNTLET_THIS_SEASON_LEAGUE_BEING_EVALUATED;
			}
			else
			{
				m_lbRemainRankTime.text = string.Format(NKCUtilString.GET_STRING_GAUNTLET_THIS_SEASON_LEAGUE_REMAIN_TIME_ONE_PARAM, NKCUtilString.GetRemainTimeStringEx(pvpAsyncSeasonTemplet.EndDate));
			}
		}
		else
		{
			m_lbRemainRankTime.text = string.Format(NKCUtilString.GET_STRING_GAUNTLET_THIS_WEEK_LEAGUE_ONE_PARAM, NKCUtilString.GetRemainTimeStringForGauntletWeekly());
		}
	}

	private void UpdateAsyncRefreshTime()
	{
		if (m_CurAsyncType != PVP_ASYNC_TYPE.PAT_BATTLE || m_fAsyncRefreshTimer <= 0f)
		{
			return;
		}
		if (m_fAsyncRefreshTimer + 60f < Time.time)
		{
			RefreshAsyncButton();
			return;
		}
		float num = 60f - (Time.time - m_fAsyncRefreshTimer);
		NKCUtil.SetLabelText(m_txtAsyncRefreshTimer, $"00:{num:00}");
		if (num <= 0f)
		{
			RefreshAsyncButton();
		}
	}

	private bool TrySendRankUserListREQ(RANK_TYPE rt, bool all)
	{
		if (all)
		{
			if (m_arAllRankREQ[(int)rt])
			{
				return false;
			}
			m_arAllRankREQ[(int)rt] = true;
		}
		else
		{
			if (m_arRankREQ[(int)rt])
			{
				return false;
			}
			m_arRankREQ[(int)rt] = true;
		}
		NKCPacketSender.Send_NKMPacket_ASYNC_PVP_RANK_LIST_REQ(rt, all);
		NKCUtil.SetGameobjectActive(m_lvsrAsyncRank.gameObject, bValue: false);
		return true;
	}

	private void OnEventPanelBeginDragAll()
	{
		TrySendRankUserListREQ(m_CurRankType, all: true);
	}

	private void RefreshScrollRect(LoopVerticalScrollRect scrollRect, PVP_ASYNC_TYPE type, int count)
	{
		scrollRect.TotalCount = count;
		if (!m_arOpened[(int)type])
		{
			m_arOpened[(int)type] = true;
			scrollRect.velocity = Vector2.zero;
			scrollRect.SetIndexPosition(0);
		}
		else
		{
			scrollRect.RefreshCells();
		}
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_TARGET_LIST_ACK sPacket)
	{
		if (NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().AsyncTargetList.Count > 0)
		{
			m_fAsyncRefreshTimer = Time.time;
			NKC_SCEN_GAUNTLET_LOBBY.AsyncRefreshCooltime = m_fAsyncRefreshTimer;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetAsyncTargetList(sPacket.targetList);
		SetTartgetList(NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().AsyncTargetList);
		RefreshAsyncButton();
		UpdateAsyncRefreshTime();
		if (m_CurAsyncType == PVP_ASYNC_TYPE.PAT_BATTLE)
		{
			UpdateScroll();
		}
	}

	public void OnRecv(NKMPacket_NPC_PVP_TARGET_LIST_ACK sPacket)
	{
		m_fAsyncNpcBotTimer = Time.time;
		m_lstCurNpcBot = sPacket.targetList;
		NKCUtil.SetGameobjectActive(m_lvsrAsyncNPC.gameObject, bValue: true);
		UpdateScroll();
		OnNewOpenTierSlotEffect();
	}

	public void OnRecv(NKMPacket_REVENGE_PVP_TARGET_LIST_ACK sPacket)
	{
		SetRevengeList(sPacket.targetList);
		NKCUtil.SetGameobjectActive(m_lvsrAsyncRevenge.gameObject, bValue: true);
		if (m_CurAsyncType == PVP_ASYNC_TYPE.PAT_REVENGE)
		{
			UpdateScroll();
		}
	}

	public void SetTartgetList(List<AsyncPvpTarget> targetList)
	{
		m_listAsyncTarget.Clear();
		m_listAsyncTarget.AddRange(targetList);
	}

	public void SetRevengeList(List<RevengePvpTarget> targetList)
	{
		m_listRevengeTarget.Clear();
		m_listRevengeTarget.AddRange(targetList.FindAll((RevengePvpTarget v) => InvalidTarget(v)));
	}

	private bool InvalidTarget(RevengePvpTarget target)
	{
		if (target == null)
		{
			return false;
		}
		if (target.asyncDeck == null)
		{
			return false;
		}
		if (target.asyncDeck.ship == null)
		{
			return false;
		}
		if (target.asyncDeck.units == null)
		{
			return false;
		}
		if (target.asyncDeck.units.Count != 8)
		{
			return false;
		}
		return true;
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_RANK_LIST_ACK spacket)
	{
		NKCUtil.SetGameobjectActive(m_lvsrAsyncRank.gameObject, bValue: true);
		AddUserSimpleList(spacket.rankType, spacket.userProfileDataList);
		if (spacket.rankType == RANK_TYPE.FRIEND)
		{
			GetUserSimpleData(spacket.rankType).Sort((NKMUserSimpleProfileData a, NKMUserSimpleProfileData b) => b.pvpScore.CompareTo(a.pvpScore));
		}
		if (spacket.userProfileDataList.Count < NKMPvpCommonConst.Instance.RANK_SIMPLE_COUNT)
		{
			m_arAllRankREQ[(int)spacket.rankType] = true;
		}
		if (m_CurRankType == spacket.rankType)
		{
			UpdateScroll();
		}
	}

	public void OnRecv(NKMPacket_UPDATE_DEFENCE_DECK_ACK packet)
	{
		if (!RefreshAsyncLock() && m_bNotSetDefenceDeck)
		{
			m_bNotSetDefenceDeck = false;
			OnToggleAsyncTab(_bSet: true, m_CurAsyncType, bForce: true);
		}
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_ACK packet)
	{
		m_iCurNPCBotTier = 0;
		RefreshNpcSubTabUI();
		UpdateTabUI();
		m_RightSide.UpdateNowSeasonPVPInfoUI(NKM_GAME_TYPE.NGT_ASYNC_PVP);
	}

	public void OnRecv(NKMPacket_PVP_CHARGE_POINT_REFRESH_ACK cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK)
	{
		m_RightSide.UpdateRankPVPPointUI();
	}

	public void SetUI(PVP_ASYNC_TYPE reservedTab = PVP_ASYNC_TYPE.MAX)
	{
		m_bPlayIntro = true;
		m_fAsyncRefreshTimer = NKC_SCEN_GAUNTLET_LOBBY.AsyncRefreshCooltime;
		if ((uint)(reservedTab - 1) <= 1u)
		{
			m_CurAsyncType = reservedTab;
		}
		RefreshAsyncLock();
		if (!m_bPrepareLoopScrollCells)
		{
			NKCUtil.SetGameobjectActive(m_objAsyncBattle, bValue: true);
			NKCUtil.SetGameobjectActive(m_objAsyncNPC, bValue: true);
			NKCUtil.SetGameobjectActive(m_objAsyncRevenge, bValue: true);
			NKCUtil.SetGameobjectActive(m_objAsyncRank, bValue: true);
			m_lvsrAsyncBattle?.PrepareCells();
			m_lvsrAsyncNPC?.PrepareCells();
			m_lhsrNPCSub?.PrepareCells();
			m_lvsrAsyncRevenge?.PrepareCells();
			m_lvsrAsyncRank?.PrepareCells();
			m_bPrepareLoopScrollCells = true;
		}
		m_RightSide.UpdateNowSeasonPVPInfoUI(NKM_GAME_TYPE.NGT_ASYNC_PVP);
		m_RightSide.UpdateBattleCondition();
		if (m_bFirstOpen)
		{
			for (int i = 0; i < 3; i++)
			{
				m_arRankREQ[i] = false;
				m_arAllRankREQ[i] = false;
				m_arOpened[i] = false;
			}
			RefreshNpcSubTabUI();
			OnToggleAsyncTab(_bSet: true, m_CurAsyncType, bForce: true);
			OnSelectTierSlot(m_iCurNPCBotTier);
			m_bFirstOpen = false;
		}
		UpdateRemainTimeUI();
		m_RightSide.UpdateRankPVPPointUI();
		UpdateTabUI();
		RefreshAsyncButton();
	}

	public void ClearCacheData()
	{
		if (m_NKCPopupGauntletLeagueGuide != null)
		{
			if (m_NKCPopupGauntletLeagueGuide.IsOpen)
			{
				m_NKCPopupGauntletLeagueGuide.Close();
			}
			m_NKCPopupGauntletLeagueGuide = null;
		}
		m_lvsrAsyncBattle.ClearCells();
		m_lvsrAsyncNPC.ClearCells();
		m_lvsrAsyncRevenge.ClearCells();
		m_lvsrAsyncRank.ClearCells();
		m_lhsrNPCSub.ClearCells();
	}

	public void Close()
	{
		if (m_NKCPopupGauntletLeagueGuide != null && m_NKCPopupGauntletLeagueGuide.IsOpen)
		{
			m_NKCPopupGauntletLeagueGuide.Close();
		}
		m_iNewOpendNpcTier = 0;
		NKCPopupGauntletBanList.CheckInstanceAndClose();
	}

	private void RefreshNpcSubTabUI()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_NPC_MODE))
		{
			NKCUtil.SetGameobjectActive(m_objAsyncNPC, bValue: true);
			m_iNPCBotMaxOpendTier = NKCScenManager.GetScenManager().GetMyUserData().m_NpcData.MaxOpenedTier;
			m_iCurNPCBotTier = ((m_iCurNPCBotTier != 0) ? m_iCurNPCBotTier : m_iNPCBotMaxOpendTier);
			m_lhsrNPCSub.TotalCount = Math.Max(1, NKCScenManager.GetScenManager().GetMyUserData().m_NpcData.MaxTierCount);
			m_lhsrNPCSub.SetIndexPosition(m_iCurNPCBotTier - 1);
		}
	}

	private void OnClickFriendProfile(long friendCode)
	{
		if (friendCode > 0)
		{
			NKCPacketSender.Send_NKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ(friendCode);
		}
	}

	private void OnClickAsyncBattle(long friendCode, NKM_GAME_TYPE gameType)
	{
		if (IsEnableAsync())
		{
			AsyncPvpTarget asyncPvpTarget = m_listAsyncTarget.Find((AsyncPvpTarget v) => v.userFriendCode == friendCode);
			if (asyncPvpTarget != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_ASYNC_READY().SetReserveData(asyncPvpTarget, gameType);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_ASYNC_READY);
			}
		}
	}

	private void OnClickAsyncRevenge(long friendCode, NKM_GAME_TYPE gameType)
	{
		if (IsEnableAsync())
		{
			RevengePvpTarget revengePvpTarget = m_listRevengeTarget.Find((RevengePvpTarget v) => v.userFriendCode == friendCode);
			if (revengePvpTarget != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_ASYNC_READY().SetReserveData(ConventToAsyncPvpTarget(revengePvpTarget), gameType);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_ASYNC_READY);
			}
		}
	}

	private bool IsEnableAsync()
	{
		if (CheckDefenseDeck() != NKM_ERROR_CODE.NEC_OK)
		{
			return false;
		}
		if (CheckCanPlayPVPGame() != NKM_ERROR_CODE.NEC_OK)
		{
			return false;
		}
		return true;
	}

	public static AsyncPvpTarget ConventToAsyncPvpTarget(RevengePvpTarget _revengeTarget)
	{
		AsyncPvpTarget asyncPvpTarget = new AsyncPvpTarget();
		if (_revengeTarget != null)
		{
			asyncPvpTarget.userLevel = _revengeTarget.userLevel;
			asyncPvpTarget.userNickName = _revengeTarget.userNickName;
			asyncPvpTarget.userFriendCode = _revengeTarget.userFriendCode;
			asyncPvpTarget.rank = _revengeTarget.rank;
			asyncPvpTarget.score = _revengeTarget.score;
			asyncPvpTarget.tier = _revengeTarget.tier;
			asyncPvpTarget.mainUnitId = _revengeTarget.mainUnitId;
			asyncPvpTarget.mainUnitSkinId = _revengeTarget.mainUnitSkinId;
			asyncPvpTarget.selfieFrameId = _revengeTarget.selfieFrameId;
			asyncPvpTarget.asyncDeck = _revengeTarget.asyncDeck;
			asyncPvpTarget.guildData = _revengeTarget.guildData;
		}
		return asyncPvpTarget;
	}

	private void CloseDefenseDeck()
	{
		NKCUIDeckViewer.Instance.Close();
		RefreshAsyncLock();
	}

	private NKM_ERROR_CODE CheckDefenseDeck()
	{
		NKMDeckIndex nKMDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP_DEFENCE, 0);
		return NKMMain.IsValidDeck(NKCScenManager.CurrentUserData().m_ArmyData, nKMDeckIndex.m_eDeckType, nKMDeckIndex.m_iIndex, NKM_GAME_TYPE.NGT_ASYNC_PVP);
	}

	public bool RefreshAsyncLock()
	{
		bool flag = false;
		NKM_ERROR_CODE nKM_ERROR_CODE = CheckCanPlayPVPGame();
		if (nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_FAIL_END_SEASON || nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_FAIL_END_WEEK)
		{
			flag = true;
			NKCUtil.SetLabelText(m_txtAsyncLock, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_LOCK_CLOSING);
		}
		else if (CheckDefenseDeck() != NKM_ERROR_CODE.NEC_OK)
		{
			flag = true;
			NKCUtil.SetLabelText(m_txtAsyncLock, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_LOCK_DEFENSE_DECK);
		}
		NKCUtil.SetGameobjectActive(m_objAsyncLock, flag && m_CurAsyncType != PVP_ASYNC_TYPE.PAT_RANK);
		return flag;
	}

	private void OnClickAsyncRefresh()
	{
		if (CheckAsyncRfreshTime() && IsEnableAsync())
		{
			NKCPacketSender.Send_NKMPacket_ASYNC_PVP_TARGET_LIST_REQ();
		}
	}

	private bool CheckAsyncRfreshTime()
	{
		if (m_fAsyncRefreshTimer <= 0f)
		{
			return true;
		}
		if (m_fAsyncRefreshTimer + 60f < Time.time)
		{
			return true;
		}
		return false;
	}

	private bool CheckNpcRfreshTime()
	{
		if (m_fAsyncNpcBotTimer <= 0f)
		{
			return true;
		}
		if (m_fAsyncNpcBotTimer + 60f < Time.time)
		{
			return true;
		}
		return false;
	}

	private void RefreshAsyncButton()
	{
		bool flag = CheckAsyncRfreshTime();
		NKCUtil.SetGameobjectActive(m_objAsyncRefreshOn, flag);
		NKCUtil.SetGameobjectActive(m_objAsyncRefreshOff, !flag);
		if (flag)
		{
			NKCUtil.SetLabelText(m_txtAsyncRefreshTimer, "");
		}
	}

	private NKMUserSimpleProfileData GetUserSimpleData(RANK_TYPE type, int index)
	{
		List<NKMUserSimpleProfileData> userSimpleData = GetUserSimpleData(type);
		if (userSimpleData.Count > index)
		{
			return userSimpleData[index];
		}
		return null;
	}

	private List<NKMUserSimpleProfileData> GetUserSimpleData(RANK_TYPE _type)
	{
		if (m_dicUserSimpleData.TryGetValue(_type, out var value))
		{
			return value;
		}
		return new List<NKMUserSimpleProfileData>();
	}

	private void AddUserSimpleList(RANK_TYPE type, List<NKMUserSimpleProfileData> list)
	{
		if (!m_dicUserSimpleData.ContainsKey(type))
		{
			m_dicUserSimpleData.Add(type, new List<NKMUserSimpleProfileData>());
		}
		m_dicUserSimpleData[type].Clear();
		m_dicUserSimpleData[type] = list;
	}

	private int GetCurrentSeasonID()
	{
		return NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime());
	}

	private NKM_ERROR_CODE CheckCanPlayPVPGame()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		int seasonID = NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime());
		int weekIDForAsync = NKCPVPManager.GetWeekIDForAsync(NKCSynchronizedTime.GetServerUTCTime(), seasonID);
		return NKCPVPManager.CanPlayPVPAsyncGame(myUserData, seasonID, weekIDForAsync, NKCSynchronizedTime.GetServerUTCTime());
	}

	private void OnToggleRankTab(bool _bSet, RANK_TYPE _newType)
	{
		if (_bSet && m_CurRankType != _newType)
		{
			UpdateRankData(_newType);
		}
	}

	private void UpdateRankData(RANK_TYPE _newType)
	{
		m_CurRankType = _newType;
		if (!TrySendRankUserListREQ(_newType, all: false))
		{
			UpdateScroll();
		}
	}

	public void OnToggleAsyncTab(bool _bSet, PVP_ASYNC_TYPE _newTab, bool bForce = false)
	{
		if (!_bSet || (m_CurAsyncType == _newTab && !bForce))
		{
			return;
		}
		if ((_newTab == PVP_ASYNC_TYPE.PAT_NPC && NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_NPC_MODE)) || (_newTab == PVP_ASYNC_TYPE.PAT_REVENGE && NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_REVENGE_MODE)))
		{
			switch (_newTab)
			{
			case PVP_ASYNC_TYPE.PAT_RANK:
				m_ctglAsyncRank.Select(bSelect: true, bForce: true);
				break;
			case PVP_ASYNC_TYPE.PAT_NPC:
				m_ctglAsyncNPC.Select(bSelect: true, bForce: true);
				break;
			case PVP_ASYNC_TYPE.PAT_BATTLE:
				m_ctglAsyncBattle.Select(bSelect: true, bForce: true);
				break;
			case PVP_ASYNC_TYPE.PAT_REVENGE:
				m_ctglAsyncRevenge.Select(bSelect: true, bForce: true);
				break;
			}
		}
		m_CurAsyncType = _newTab;
		UpdateTabUI();
		TrySendAllRankUserListREQ();
	}

	private void TrySendAllRankUserListREQ()
	{
		if ((m_CurAsyncType == PVP_ASYNC_TYPE.PAT_BATTLE && m_bSendBattleListReq) || (m_CurAsyncType == PVP_ASYNC_TYPE.PAT_RANK && m_bSendRankListReq) || (m_CurAsyncType == PVP_ASYNC_TYPE.PAT_NPC && m_bSendNPCListReq))
		{
			UpdateScroll();
			return;
		}
		if (UpdateAsyncLockUI())
		{
			m_bNotSetDefenceDeck = CheckDefenseDeck() != NKM_ERROR_CODE.NEC_OK;
			return;
		}
		if (m_CurAsyncType == PVP_ASYNC_TYPE.PAT_BATTLE)
		{
			m_bSendBattleListReq = true;
		}
		if (m_CurAsyncType == PVP_ASYNC_TYPE.PAT_RANK)
		{
			m_bSendRankListReq = true;
		}
		if (m_CurAsyncType == PVP_ASYNC_TYPE.PAT_NPC)
		{
			m_bSendNPCListReq = true;
		}
		SendPVPListREQ();
	}

	private void SendPVPListREQ()
	{
		switch (m_CurAsyncType)
		{
		case PVP_ASYNC_TYPE.PAT_BATTLE:
		{
			List<AsyncPvpTarget> asyncTargetList = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().AsyncTargetList;
			if (asyncTargetList.Count > 0)
			{
				SetTartgetList(asyncTargetList);
				RefreshAsyncButton();
				UpdateAsyncRefreshTime();
				UpdateScroll();
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_ASYNC_PVP_TARGET_LIST_REQ();
			}
			break;
		}
		case PVP_ASYNC_TYPE.PAT_RANK:
			switch (m_CurRankType)
			{
			case RANK_TYPE.ALL:
				m_ctglAll.Select(bSelect: true, bForce: true);
				break;
			case RANK_TYPE.MY_LEAGUE:
				m_ctglMyLeague.Select(bSelect: true, bForce: true);
				break;
			case RANK_TYPE.FRIEND:
				m_ctglFriend.Select(bSelect: true, bForce: true);
				break;
			}
			UpdateRankData(m_CurRankType);
			break;
		case PVP_ASYNC_TYPE.PAT_REVENGE:
			NKCUtil.SetGameobjectActive(m_lvsrAsyncRevenge.gameObject, bValue: false);
			NKCPacketSender.Send_NKMPacket_REVENGE_PVP_TARGET_LIST_REQ();
			break;
		case PVP_ASYNC_TYPE.PAT_NPC:
			if (CheckNpcRfreshTime())
			{
				NKCPacketSender.Send_NKMPacket_NPC_PVP_TARGET_LIST_REQ(m_iCurNPCBotTier);
				NKCUtil.SetGameobjectActive(m_lvsrAsyncNPC.gameObject, bValue: false);
			}
			else
			{
				UpdateScroll();
			}
			OnSelectTierSlot(m_iCurNPCBotTier);
			break;
		}
	}

	private bool UpdateAsyncLockUI()
	{
		if (m_CurAsyncType == PVP_ASYNC_TYPE.PAT_RANK)
		{
			NKCUtil.SetGameobjectActive(m_objAsyncLock, bValue: false);
			return false;
		}
		return RefreshAsyncLock();
	}

	private void UpdateTabUI()
	{
		bool flag = false;
		if (m_CurAsyncType == PVP_ASYNC_TYPE.PAT_RANK)
		{
			NKCUtil.SetGameobjectActive(m_objAsyncLock, bValue: false);
		}
		else
		{
			flag = RefreshAsyncLock();
		}
		NKCUtil.SetGameobjectActive(m_objAsyncBattle, m_CurAsyncType == PVP_ASYNC_TYPE.PAT_BATTLE && !flag);
		NKCUtil.SetGameobjectActive(m_objAsyncNPC, m_CurAsyncType == PVP_ASYNC_TYPE.PAT_NPC && !flag);
		NKCUtil.SetGameobjectActive(m_objAsyncRevenge, m_CurAsyncType == PVP_ASYNC_TYPE.PAT_REVENGE && !flag);
		NKCUtil.SetGameobjectActive(m_objAsyncRank, m_CurAsyncType == PVP_ASYNC_TYPE.PAT_RANK);
		NKCUtil.SetGameobjectActive(m_csbtnAsyncRefresh, m_CurAsyncType == PVP_ASYNC_TYPE.PAT_BATTLE);
		NKCUtil.SetGameobjectActive(m_lbRemainRankTime.gameObject, m_CurAsyncType != PVP_ASYNC_TYPE.PAT_REVENGE);
		if (!m_bPlayIntro)
		{
			m_amtorRankCenter.Play("NKM_UI_GAUNTLET_LOBBY_CONTENT_INTRO_CENTER_FADEIN");
		}
		else
		{
			m_bPlayIntro = false;
		}
	}

	private void UpdateScroll()
	{
		if (base.gameObject.activeInHierarchy)
		{
			switch (m_CurAsyncType)
			{
			case PVP_ASYNC_TYPE.PAT_BATTLE:
				RefreshScrollRect(m_lvsrAsyncBattle, m_CurAsyncType, m_listAsyncTarget.Count);
				break;
			case PVP_ASYNC_TYPE.PAT_RANK:
				RefreshScrollRect(m_lvsrAsyncRank, m_CurAsyncType, GetUserSimpleData(m_CurRankType).Count);
				break;
			case PVP_ASYNC_TYPE.PAT_REVENGE:
				RefreshScrollRect(m_lvsrAsyncRevenge, m_CurAsyncType, m_listRevengeTarget.Count);
				break;
			case PVP_ASYNC_TYPE.PAT_NPC:
				RefreshScrollRect(m_lvsrAsyncNPC, m_CurAsyncType, m_lstCurNpcBot.Count);
				break;
			}
		}
	}

	public RectTransform GetSlotAsyncBattle(int index)
	{
		return NKCUIGauntletAsyncSlot.GetNewInstance(m_lvsrAsyncRevenge.content, OnClickAsyncBattle, OnClickFriendProfile).GetComponent<RectTransform>();
	}

	public void ReturnSlotAsyncBattle(Transform tr)
	{
		tr.SetParent(base.transform);
		NKCUIGauntletAsyncSlot component = tr.GetComponent<NKCUIGauntletAsyncSlot>();
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideDataAsyncBattle(Transform tr, int index)
	{
		NKCUIGauntletAsyncSlot component = tr.GetComponent<NKCUIGauntletAsyncSlot>();
		if (component != null)
		{
			if (m_listAsyncTarget.Count <= index)
			{
				Debug.LogError($"Async PVP data \ufffd\u033b\ufffd\ufffd\ufffd. target : {m_listAsyncTarget.Count} <= {index}");
			}
			component.SetUI(m_listAsyncTarget[index], NKM_GAME_TYPE.NGT_PVP_STRATEGY);
		}
	}

	public RectTransform GetSlotAsyncNPC(int index)
	{
		return NKCUIGauntletAsyncSlot.GetNewInstance(m_lvsrAsyncNPC.content, OnClickAsyncNPC).GetComponent<RectTransform>();
	}

	public void ReturnSlotAsyncNPC(Transform tr)
	{
		tr.SetParent(base.transform);
		NKCUIGauntletAsyncSlot component = tr.GetComponent<NKCUIGauntletAsyncSlot>();
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideDataAsyncNPC(Transform tr, int index)
	{
		NKCUIGauntletAsyncSlot component = tr.GetComponent<NKCUIGauntletAsyncSlot>();
		if (m_lstCurNpcBot.Count > index)
		{
			component.SetUI(m_lstCurNpcBot[index]);
		}
		else
		{
			Debug.LogError($"ProvideDataAsyncNPC target : {m_lstCurNpcBot.Count} <= {index}");
		}
	}

	public RectTransform GetSlotAsyncRevenge(int index)
	{
		return NKCUIGauntletAsyncSlotNew.GetNewInstance(m_lvsrAsyncRevenge.content, OnClickAsyncRevenge, OnClickFriendProfile).GetComponent<RectTransform>();
	}

	public void ReturnSlotAsyncRevenge(Transform tr)
	{
		tr.SetParent(base.transform);
		NKCUIGauntletAsyncSlotNew component = tr.GetComponent<NKCUIGauntletAsyncSlotNew>();
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideDataAsyncRevenge(Transform tr, int index)
	{
		NKCUIGauntletAsyncSlotNew component = tr.GetComponent<NKCUIGauntletAsyncSlotNew>();
		if (component != null)
		{
			if (m_listRevengeTarget.Count <= index)
			{
				Debug.LogError($"Revenge data \ufffd\u033b\ufffd\ufffd\ufffd. target : {m_listRevengeTarget.Count} <= {index}");
			}
			component.SetUI(m_listRevengeTarget[index], NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE);
		}
	}

	public RectTransform GetSlotAsyncRank(int index)
	{
		return NKCUIGauntletLRSlot.GetNewInstance(m_lvsrAsyncRank.content, OnEventPanelBeginDragAll).GetComponent<RectTransform>();
	}

	public void ReturnSlotAsyncRank(Transform tr)
	{
		tr.SetParent(base.transform);
		NKCUIGauntletLRSlot component = tr.GetComponent<NKCUIGauntletLRSlot>();
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideDataAsyncRank(Transform tr, int index)
	{
		NKCUIGauntletLRSlot component = tr.GetComponent<NKCUIGauntletLRSlot>();
		if (component != null)
		{
			component.SetUI(GetUserSimpleData(m_CurRankType, index), index + 1, NKM_GAME_TYPE.NGT_PVP_STRATEGY);
		}
	}

	public RectTransform GetSlotAsyncNPCSub(int idx)
	{
		NKCUIGauntletLobbyAsyncSubTab newInstance = NKCUIGauntletLobbyAsyncSubTab.GetNewInstance(m_lhsrNPCSub.transform);
		if (newInstance != null)
		{
			RectTransform component = newInstance.GetComponent<RectTransform>();
			if (null != component)
			{
				return component;
			}
		}
		return null;
	}

	public void ReturnSlotAsyncNPCSub(Transform tr)
	{
		if (!(tr == null))
		{
			NKCUIGauntletLobbyAsyncSubTab component = tr.GetComponent<NKCUIGauntletLobbyAsyncSubTab>();
			if (null != component)
			{
				m_lstSubTalSlots.Remove(component);
				component.DestoryInstance();
			}
			else
			{
				UnityEngine.Object.Destroy(tr.gameObject);
			}
			NKCUtil.SetGameobjectActive(tr, bValue: false);
		}
	}

	public void ProvideDataAsyncNPCSub(Transform tr, int index)
	{
		NKCUIGauntletLobbyAsyncSubTab component = tr.GetComponent<NKCUIGauntletLobbyAsyncSubTab>();
		component?.SetData(index + 1, OnClickAsyncNPCSubTab, index + 1 == m_iCurNPCBotTier);
		if (null != component)
		{
			m_lstSubTalSlots.Add(component);
		}
	}

	public void OnClickAsyncNPCSubTab(int _iSelectedTier)
	{
		if (m_iCurNPCBotTier != _iSelectedTier)
		{
			m_iCurNPCBotTier = _iSelectedTier;
			OnSelectTierSlot(m_iCurNPCBotTier);
			NKCPacketSender.Send_NKMPacket_NPC_PVP_TARGET_LIST_REQ(m_iCurNPCBotTier);
		}
	}

	private void OnClickAsyncNPC(long _targetNpcFriendCode)
	{
		foreach (NpcPvpTarget item in m_lstCurNpcBot)
		{
			if (item.userFriendCode == _targetNpcFriendCode)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_ASYNC_READY().SetReserveData(item);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_ASYNC_READY);
				break;
			}
		}
	}

	private void OnSelectTierSlot(int iKey)
	{
		if (m_CurAsyncType != PVP_ASYNC_TYPE.PAT_NPC)
		{
			return;
		}
		foreach (NKCUIGauntletLobbyAsyncSubTab lstSubTalSlot in m_lstSubTalSlots)
		{
			lstSubTalSlot.OnSelect(lstSubTalSlot.Tier == iKey);
		}
	}

	public static AsyncPvpTarget ConventToAsyncPvpTarget(NpcPvpTarget _npcTarget)
	{
		AsyncPvpTarget asyncPvpTarget = new AsyncPvpTarget();
		if (_npcTarget != null)
		{
			asyncPvpTarget.userLevel = _npcTarget.userLevel;
			asyncPvpTarget.userNickName = _npcTarget.userNickName;
			asyncPvpTarget.userFriendCode = _npcTarget.userFriendCode;
			asyncPvpTarget.rank = 0;
			asyncPvpTarget.score = _npcTarget.score;
			asyncPvpTarget.tier = _npcTarget.tier;
			asyncPvpTarget.mainUnitId = 0;
			asyncPvpTarget.mainUnitSkinId = 0;
			asyncPvpTarget.selfieFrameId = 0;
			asyncPvpTarget.asyncDeck = _npcTarget.asyncDeck;
			asyncPvpTarget.guildData = null;
		}
		return asyncPvpTarget;
	}

	public void SetReserveOpenNpcBotTier(int newOpenTier)
	{
		m_iNewOpendNpcTier = newOpenTier;
	}

	private void OnNewOpenTierSlotEffect()
	{
		if (m_iNewOpendNpcTier == 0)
		{
			return;
		}
		foreach (NKCUIGauntletLobbyAsyncSubTab lstSubTalSlot in m_lstSubTalSlots)
		{
			if (lstSubTalSlot.Tier == m_iNewOpendNpcTier)
			{
				lstSubTalSlot.OnActiveEffect();
				break;
			}
		}
		m_iNewOpendNpcTier = 0;
	}
}
