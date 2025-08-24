using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Pvp;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyAsync : MonoBehaviour
{
	public Animator m_amtorRankCenter;

	[Header("상단 탭")]
	public NKCUIComToggle m_ctglAsyncTab;

	public NKCUIComToggle m_ctglAllTab;

	public NKCUIComToggle m_ctglFriendTab;

	[Header("스크롤 관련")]
	public GameObject m_objAsyncTarget;

	public GameObject m_objRankAll;

	public GameObject m_objRankFriend;

	public LoopVerticalScrollRect m_lvsrAsyncTartget;

	public LoopVerticalScrollRect m_lvsrRankAll;

	public LoopVerticalScrollRect m_lvsrRankFriend;

	public Transform m_trAsyncTartget;

	public Transform m_trRankAll;

	public Transform m_trRankFriend;

	public GameObject m_objAsyncLock;

	public Text m_txtAsyncLock;

	public GameObject m_objEmptyList;

	public Text m_lbEmptyMessage;

	[Header("중앙 하단")]
	public Text m_lbRemainRankTime;

	public NKCUIComStateButton m_csbtnAsyncRefresh;

	public GameObject m_objAsyncRefreshOn;

	public GameObject m_objAsyncRefreshOff;

	public Text m_txtAsyncRefreshTimer;

	[Header("V2 대응 처리")]
	public GameObject m_objNKM_UI_GAUNTLET_RANK_UPSIDEMENU;

	public GameObject m_objNKM_UI_GAUNTLET_RANK_UPSIDEMENU_NEW;

	public GameObject m_NKM_UI_GAUNTLET_ASYNC_LIST_SCROLL;

	public GameObject m_NKM_UI_GAUNTLET_ASYNC_NPC_SCROLL;

	public GameObject m_NKM_UI_GAUNTLET_ASYNC_REVENGE_SCROLL;

	public GameObject m_NKM_UI_GAUNTLET_ASYNC_RANK_SCROLL;

	[Header("정보")]
	public NKCUIGauntletLobbyRightSideAsync m_RightSide;

	private NKCPopupGauntletLeagueGuide m_NKCPopupGauntletLeagueGuide;

	private RANK_TYPE m_RANK_TYPE;

	private bool m_bFirstOpen = true;

	private bool m_bPrepareLoopScrollCells;

	private bool[] m_arRankREQ = new bool[3];

	private bool[] m_arAllRankREQ = new bool[3];

	private bool[] m_arOpened = new bool[3];

	private Dictionary<RANK_TYPE, List<NKMUserSimpleProfileData>> m_dicUserSimpleData = new Dictionary<RANK_TYPE, List<NKMUserSimpleProfileData>>();

	private List<AsyncPvpTarget> m_listAsyncTarget = new List<AsyncPvpTarget>();

	private float m_fPrevUpdateTime;

	private float m_fAsyncRefreshTimer;

	private bool m_bPlayIntro = true;

	private const int ASYNC_REFRESH_TIMER = 60;

	public RANK_TYPE GetCurrRankType()
	{
		return m_RANK_TYPE;
	}

	public void Init()
	{
		m_csbtnAsyncRefresh.PointerClick.RemoveAllListeners();
		m_csbtnAsyncRefresh.PointerClick.AddListener(OnClickAsyncRefresh);
		m_ctglAsyncTab.OnValueChanged.RemoveAllListeners();
		m_ctglAsyncTab.OnValueChanged.AddListener(OnTabChangedToAsyncTarget);
		m_ctglAllTab.OnValueChanged.RemoveAllListeners();
		m_ctglAllTab.OnValueChanged.AddListener(OnTabChangedToAll);
		m_ctglFriendTab.OnValueChanged.RemoveAllListeners();
		m_ctglFriendTab.OnValueChanged.AddListener(OnTabChangedToFriend);
		m_lvsrAsyncTartget.dOnGetObject += GetSlotAsync;
		m_lvsrAsyncTartget.dOnReturnObject += ReturnAsyncSlot;
		m_lvsrAsyncTartget.dOnProvideData += ProvideDataAsync;
		m_lvsrAsyncTartget.ContentConstraintCount = 1;
		m_lvsrRankAll.dOnGetObject += GetSlotAll;
		m_lvsrRankAll.dOnReturnObject += ReturnSlot;
		m_lvsrRankAll.dOnProvideData += ProvideDataAll;
		m_lvsrRankAll.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_lvsrRankAll);
		m_lvsrRankFriend.dOnGetObject += GetSlotFriend;
		m_lvsrRankFriend.dOnReturnObject += ReturnSlot;
		m_lvsrRankFriend.dOnProvideData += ProvideDataFriend;
		m_lvsrRankFriend.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_lvsrRankFriend);
		m_RightSide.InitUI();
		m_RightSide.SetCallback(CloseDefenseDeck);
		bool flag = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_ASYNC_NEW_MODE);
		NKCUtil.SetGameobjectActive(m_objNKM_UI_GAUNTLET_RANK_UPSIDEMENU, !flag);
		NKCUtil.SetGameobjectActive(m_objNKM_UI_GAUNTLET_RANK_UPSIDEMENU_NEW, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAUNTLET_ASYNC_LIST_SCROLL, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAUNTLET_ASYNC_NPC_SCROLL, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAUNTLET_ASYNC_REVENGE_SCROLL, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_GAUNTLET_ASYNC_RANK_SCROLL, flag);
	}

	private void UpdateRemainTimeUI()
	{
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

	private void UpdateAsyncRefreshTime()
	{
		if (m_fAsyncRefreshTimer <= 0f)
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

	private void TrySendAllRankUserListREQ(RANK_TYPE rt)
	{
		if (!m_arAllRankREQ[(int)rt])
		{
			m_arAllRankREQ[(int)rt] = true;
			SendPVPListREQ(rt, all: true);
		}
	}

	private void OnEventPanelBeginDragMyLeague()
	{
		TrySendAllRankUserListREQ(RANK_TYPE.MY_LEAGUE);
	}

	private void OnEventPanelBeginDragAll()
	{
		TrySendAllRankUserListREQ(RANK_TYPE.ALL);
	}

	private void OnEventPanelBeginDragFriend()
	{
		TrySendAllRankUserListREQ(RANK_TYPE.FRIEND);
	}

	public RectTransform GetSlotAsync(int index)
	{
		return NKCUIGauntletAsyncSlot.GetNewInstance(m_trAsyncTartget, OnClickAsyncBattle).GetComponent<RectTransform>();
	}

	public void ReturnSlot(Transform tr)
	{
		tr.SetParent(base.transform);
		NKCUIGauntletLRSlot component = tr.GetComponent<NKCUIGauntletLRSlot>();
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	public void ReturnAsyncSlot(Transform tr)
	{
		tr.SetParent(base.transform);
		NKCUIGauntletAsyncSlot component = tr.GetComponent<NKCUIGauntletAsyncSlot>();
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideDataAsync(Transform tr, int index)
	{
		NKCUIGauntletAsyncSlot component = tr.GetComponent<NKCUIGauntletAsyncSlot>();
		if (component != null)
		{
			if (m_listAsyncTarget.Count <= index)
			{
				Debug.LogError($"Async PVP data 이상함. target : {m_listAsyncTarget.Count} <= {index}");
			}
			component.SetUI(m_listAsyncTarget[index]);
		}
	}

	public RectTransform GetSlotAll(int index)
	{
		return NKCUIGauntletLRSlot.GetNewInstance(m_trRankAll, OnEventPanelBeginDragAll).GetComponent<RectTransform>();
	}

	public void ProvideDataAll(Transform tr, int index)
	{
		NKCUIGauntletLRSlot component = tr.GetComponent<NKCUIGauntletLRSlot>();
		if (component != null)
		{
			component.SetUI(GetUserSimpleData(RANK_TYPE.ALL, index), index + 1, NKM_GAME_TYPE.NGT_ASYNC_PVP);
		}
	}

	public RectTransform GetSlotFriend(int index)
	{
		return NKCUIGauntletLRSlot.GetNewInstance(m_trRankFriend, OnEventPanelBeginDragFriend).GetComponent<RectTransform>();
	}

	public void ProvideDataFriend(Transform tr, int index)
	{
		NKCUIGauntletLRSlot component = tr.GetComponent<NKCUIGauntletLRSlot>();
		if (component != null)
		{
			component.SetUI(GetUserSimpleData(RANK_TYPE.FRIEND, index), index + 1, NKM_GAME_TYPE.NGT_ASYNC_PVP);
		}
	}

	private void RefreshRankTabCells()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (m_RANK_TYPE == RANK_TYPE.MY_LEAGUE)
			{
				RefreshScrollRect(m_lvsrAsyncTartget, m_RANK_TYPE, m_listAsyncTarget.Count);
			}
			else if (m_RANK_TYPE == RANK_TYPE.ALL)
			{
				RefreshScrollRect(m_lvsrRankAll, m_RANK_TYPE, GetUserSimpleList(m_RANK_TYPE).Count);
			}
			else if (m_RANK_TYPE == RANK_TYPE.FRIEND)
			{
				RefreshScrollRect(m_lvsrRankFriend, m_RANK_TYPE, GetUserSimpleList(m_RANK_TYPE).Count);
				m_lbEmptyMessage.text = NKCUtilString.GET_STRING_FRIEND_LIST_IS_EMPTY;
				NKCUtil.SetGameobjectActive(m_objEmptyList, m_lvsrRankFriend.TotalCount == 0);
			}
		}
	}

	private void RefreshScrollRect(LoopVerticalScrollRect scrollRect, RANK_TYPE type, int count)
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

	public void OnRecv(NKMPacket_ASYNC_PVP_TARGET_LIST_ACK spacket)
	{
		if (NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().AsyncTargetList.Count > 0)
		{
			m_fAsyncRefreshTimer = Time.time;
			NKC_SCEN_GAUNTLET_LOBBY.AsyncRefreshCooltime = m_fAsyncRefreshTimer;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetAsyncTargetList(spacket.targetList);
		SetTartgetList(NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().AsyncTargetList);
		RefreshAsyncButton();
		UpdateAsyncRefreshTime();
		if (m_RANK_TYPE == RANK_TYPE.MY_LEAGUE)
		{
			RefreshRankTabCells();
		}
	}

	public void SetTartgetList(List<AsyncPvpTarget> targetList)
	{
		m_listAsyncTarget.Clear();
		m_listAsyncTarget.AddRange(targetList);
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_RANK_LIST_ACK spacket)
	{
		AddUserSimpleList(spacket.rankType, spacket.userProfileDataList);
		if (spacket.rankType == RANK_TYPE.FRIEND)
		{
			GetUserSimpleList(spacket.rankType).Sort((NKMUserSimpleProfileData a, NKMUserSimpleProfileData b) => b.pvpScore.CompareTo(a.pvpScore));
		}
		if (spacket.userProfileDataList.Count < NKMPvpCommonConst.Instance.RANK_SIMPLE_COUNT)
		{
			m_arAllRankREQ[(int)spacket.rankType] = true;
		}
		if (m_RANK_TYPE == spacket.rankType)
		{
			RefreshRankTabCells();
		}
	}

	public void OnRecv(NKMPacket_UPDATE_DEFENCE_DECK_ACK packet)
	{
		RefreshAsyncLock();
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_ACK packet)
	{
		m_RightSide.UpdateNowSeasonPVPInfoUI(NKM_GAME_TYPE.NGT_ASYNC_PVP);
		if (m_RANK_TYPE != RANK_TYPE.MY_LEAGUE)
		{
			SendPVPListREQ(m_RANK_TYPE, all: false);
		}
	}

	public void OnRecv(NKMPacket_PVP_CHARGE_POINT_REFRESH_ACK cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK)
	{
		m_RightSide.UpdateRankPVPPointUI();
	}

	public void SetUI()
	{
		m_bPlayIntro = true;
		m_fAsyncRefreshTimer = NKC_SCEN_GAUNTLET_LOBBY.AsyncRefreshCooltime;
		RefreshAsyncLock();
		if (!m_bPrepareLoopScrollCells)
		{
			NKCUtil.SetGameobjectActive(m_objAsyncTarget, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRankAll, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRankFriend, bValue: true);
			m_lvsrAsyncTartget.PrepareCells();
			m_lvsrRankAll.PrepareCells();
			m_lvsrRankFriend.PrepareCells();
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
			m_bFirstOpen = false;
		}
		if (m_RANK_TYPE == RANK_TYPE.MY_LEAGUE)
		{
			m_ctglAsyncTab.Select(bSelect: false, bForce: true);
			m_ctglAsyncTab.Select(bSelect: true);
		}
		else if (m_RANK_TYPE == RANK_TYPE.ALL)
		{
			m_ctglAllTab.Select(bSelect: false, bForce: true);
			m_ctglAllTab.Select(bSelect: true);
		}
		else if (m_RANK_TYPE == RANK_TYPE.FRIEND)
		{
			m_ctglFriendTab.Select(bSelect: false, bForce: true);
			m_ctglFriendTab.Select(bSelect: true);
		}
		else
		{
			m_ctglAsyncTab.Select(bSelect: false, bForce: true);
			m_ctglAsyncTab.Select(bSelect: true);
		}
		UpdateRemainTimeUI();
		m_RightSide.UpdateRankPVPPointUI();
		SetRankTabUI();
		RefreshAsyncButton();
	}

	private void SetRankTabUI()
	{
		NKCUtil.SetGameobjectActive(m_objAsyncTarget, m_RANK_TYPE == RANK_TYPE.MY_LEAGUE);
		NKCUtil.SetGameobjectActive(m_objRankAll, m_RANK_TYPE == RANK_TYPE.ALL);
		NKCUtil.SetGameobjectActive(m_objRankFriend, m_RANK_TYPE == RANK_TYPE.FRIEND);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetLatestRANK_TYPE(m_RANK_TYPE);
		RefreshRankTabCells();
		if (!m_bPlayIntro)
		{
			m_amtorRankCenter.Play("NKM_UI_GAUNTLET_LOBBY_CONTENT_INTRO_CENTER_FADEIN");
		}
		else
		{
			m_bPlayIntro = false;
		}
	}

	private void SendPVPListREQ(RANK_TYPE type, bool all)
	{
		if (type == RANK_TYPE.MY_LEAGUE)
		{
			List<AsyncPvpTarget> asyncTargetList = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().AsyncTargetList;
			if (asyncTargetList.Count > 0)
			{
				SetTartgetList(asyncTargetList);
				RefreshAsyncButton();
				UpdateAsyncRefreshTime();
				RefreshRankTabCells();
				return;
			}
			NKM_ERROR_CODE nKM_ERROR_CODE = CheckCanPlayPVPGame();
			if (nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_FAIL_END_WEEK || nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_FAIL_END_SEASON)
			{
				SetTartgetList(asyncTargetList);
				RefreshAsyncLock();
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_ASYNC_PVP_TARGET_LIST_REQ();
			}
		}
		else
		{
			NKMPacket_ASYNC_PVP_RANK_LIST_REQ nKMPacket_ASYNC_PVP_RANK_LIST_REQ = new NKMPacket_ASYNC_PVP_RANK_LIST_REQ();
			nKMPacket_ASYNC_PVP_RANK_LIST_REQ.rankType = type;
			nKMPacket_ASYNC_PVP_RANK_LIST_REQ.isAll = all;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_ASYNC_PVP_RANK_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
	}

	private void IfCanSendRankListREQByCurrRankType()
	{
		if (!m_arRankREQ[(int)m_RANK_TYPE])
		{
			SendPVPListREQ(m_RANK_TYPE, all: false);
			m_arRankREQ[(int)m_RANK_TYPE] = true;
		}
	}

	private void OnTabChangedToAsyncTarget(bool bSet)
	{
		if (bSet)
		{
			m_RANK_TYPE = RANK_TYPE.MY_LEAGUE;
		}
		IfCanSendRankListREQByCurrRankType();
		SetRankTabUI();
	}

	private void OnTabChangedToAll(bool bSet)
	{
		if (bSet)
		{
			m_RANK_TYPE = RANK_TYPE.ALL;
		}
		IfCanSendRankListREQByCurrRankType();
		SetRankTabUI();
	}

	private void OnTabChangedToFriend(bool bSet)
	{
		if (bSet)
		{
			m_RANK_TYPE = RANK_TYPE.FRIEND;
		}
		IfCanSendRankListREQByCurrRankType();
		SetRankTabUI();
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
		m_lvsrRankAll.ClearCells();
		m_lvsrRankFriend.ClearCells();
		m_lvsrAsyncTartget.ClearCells();
	}

	public void Close()
	{
		if (m_NKCPopupGauntletLeagueGuide != null && m_NKCPopupGauntletLeagueGuide.IsOpen)
		{
			m_NKCPopupGauntletLeagueGuide.Close();
		}
		m_bFirstOpen = true;
		NKCPopupGauntletBanList.CheckInstanceAndClose();
	}

	private void OnClickAsyncBattle(long friendCode)
	{
		if (CheckDefenseDeck() == NKM_ERROR_CODE.NEC_OK && CheckCanPlayPVPGame() == NKM_ERROR_CODE.NEC_OK)
		{
			AsyncPvpTarget asyncPvpTarget = m_listAsyncTarget.Find((AsyncPvpTarget v) => v.userFriendCode == friendCode);
			if (asyncPvpTarget != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_ASYNC_READY().SetReserveData(asyncPvpTarget);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_ASYNC_READY);
			}
		}
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

	public void RefreshAsyncLock()
	{
		bool bValue = false;
		NKM_ERROR_CODE nKM_ERROR_CODE = CheckCanPlayPVPGame();
		if (nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_FAIL_END_SEASON || nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_FAIL_END_WEEK)
		{
			bValue = true;
			NKCUtil.SetLabelText(m_txtAsyncLock, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_LOCK_CLOSING);
		}
		else if (CheckDefenseDeck() != NKM_ERROR_CODE.NEC_OK)
		{
			bValue = true;
			NKCUtil.SetLabelText(m_txtAsyncLock, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_LOCK_DEFENSE_DECK);
		}
		NKCUtil.SetGameobjectActive(m_objAsyncLock, bValue);
	}

	private void OnClickAsyncRefresh()
	{
		if (CheckAsyncRfreshTime() && CheckDefenseDeck() == NKM_ERROR_CODE.NEC_OK && CheckCanPlayPVPGame() == NKM_ERROR_CODE.NEC_OK)
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

	private List<NKMUserSimpleProfileData> GetUserSimpleList(RANK_TYPE type)
	{
		if (m_dicUserSimpleData.TryGetValue(type, out var value))
		{
			return value;
		}
		return new List<NKMUserSimpleProfileData>();
	}

	private NKMUserSimpleProfileData GetUserSimpleData(RANK_TYPE type, int index)
	{
		List<NKMUserSimpleProfileData> userSimpleList = GetUserSimpleList(type);
		if (userSimpleList.Count > index)
		{
			return userSimpleList[index];
		}
		return null;
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
}
