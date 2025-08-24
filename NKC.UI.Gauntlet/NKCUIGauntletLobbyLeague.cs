using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.LeaderBoard;
using ClientPacket.Pvp;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyLeague : MonoBehaviour
{
	public Animator m_amtorRankCenter;

	[Header("상단 탭")]
	public NKCUIComToggle m_ctglRankMyLeagueTab;

	public NKCUIComToggle m_ctglRankAllTab;

	public NKCUIComToggle m_ctglRankFriendTab;

	[Header("스크롤 관련")]
	public GameObject m_objRankMyLeague;

	public GameObject m_objRankAll;

	public GameObject m_objRankFriend;

	public LoopVerticalScrollRect m_lvsrRankMyLeague;

	public LoopVerticalScrollRect m_lvsrRankAll;

	public LoopVerticalScrollRect m_lvsrRankFriend;

	public Transform m_trRankMyLeague;

	public Transform m_trRankAll;

	public Transform m_trRankFriend;

	public GameObject m_objEmptyList;

	public Text m_lbEmptyMessage;

	[Header("남은 시간")]
	public Text m_lbRemainRankTime;

	[Header("우측 정보")]
	public NKCUIGauntletLobbyRightSideLeague m_RightSide;

	private NKCPopupGauntletLeagueGuide m_NKCPopupGauntletLeagueGuide;

	private RANK_TYPE m_RANK_TYPE;

	private bool m_bFirstOpen = true;

	private bool m_bPrepareLoopScrollCells;

	private bool[] m_arRankREQ = new bool[3];

	private bool[] m_arAllRankREQ = new bool[3];

	private bool[] m_arOpened = new bool[3];

	private Dictionary<RANK_TYPE, List<NKMUserSimpleProfileData>> m_dicUserSimpleData = new Dictionary<RANK_TYPE, List<NKMUserSimpleProfileData>>();

	private float m_fPrevUpdateTime;

	private static bool m_bAlertDemotion;

	private bool m_bPlayIntro = true;

	public static void SetAlertDemotion(bool bSet)
	{
		m_bAlertDemotion = bSet;
	}

	public RANK_TYPE GetCurrRankType()
	{
		return m_RANK_TYPE;
	}

	public void SetCurrRankType(RANK_TYPE eRANK_TYPE)
	{
		m_RANK_TYPE = eRANK_TYPE;
	}

	public void Init()
	{
		m_ctglRankMyLeagueTab.OnValueChanged.RemoveAllListeners();
		m_ctglRankMyLeagueTab.OnValueChanged.AddListener(OnRankTabChangedToMyLeague);
		m_ctglRankAllTab.OnValueChanged.RemoveAllListeners();
		m_ctglRankAllTab.OnValueChanged.AddListener(OnRankTabChangedToAll);
		m_ctglRankFriendTab.OnValueChanged.RemoveAllListeners();
		m_ctglRankFriendTab.OnValueChanged.AddListener(OnRankTabChangedToFriend);
		m_lvsrRankMyLeague.dOnGetObject += GetSlotMyLeague;
		m_lvsrRankMyLeague.dOnReturnObject += ReturnSlot;
		m_lvsrRankMyLeague.dOnProvideData += ProvideDataMyLeague;
		m_lvsrRankMyLeague.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_lvsrRankMyLeague);
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
	}

	private void UpdateRemainTimeUI()
	{
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(NKCUtil.FindPVPSeasonIDForLeague(NKCSynchronizedTime.GetServerUTCTime()));
		if (nKMLeaguePvpRankSeasonTemplet != null)
		{
			if (!nKMLeaguePvpRankSeasonTemplet.CheckMySeason(NKCSynchronizedTime.GetServerUTCTime()))
			{
				m_lbRemainRankTime.text = NKCUtilString.GET_STRING_GAUNTLET_THIS_SEASON_LEAGUE_BEING_EVALUATED;
			}
			else
			{
				m_lbRemainRankTime.text = string.Format(NKCUtilString.GET_STRING_GAUNTLET_THIS_SEASON_LEAGUE_REMAIN_TIME_ONE_PARAM, NKCUtilString.GetRemainTimeStringEx(nKMLeaguePvpRankSeasonTemplet.EndDateUTC));
			}
		}
	}

	private void Update()
	{
		if (m_fPrevUpdateTime + 1f < Time.time)
		{
			m_fPrevUpdateTime = Time.time;
			UpdateRemainTimeUI();
			m_RightSide.UpdateReadyButtonUI();
			m_RightSide.UpdatePVPPointUI();
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

	public RectTransform GetSlotMyLeague(int index)
	{
		return NKCUIGauntletLRSlot.GetNewInstance(m_trRankMyLeague, OnEventPanelBeginDragMyLeague)?.GetComponent<RectTransform>();
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

	public void ProvideDataMyLeague(Transform tr, int index)
	{
		NKCUIGauntletLRSlot component = tr.GetComponent<NKCUIGauntletLRSlot>();
		if (component != null)
		{
			component.SetUI(GetUserSimpleData(RANK_TYPE.MY_LEAGUE, index), index + 1, NKM_GAME_TYPE.NGT_PVP_LEAGUE);
		}
	}

	public RectTransform GetSlotAll(int index)
	{
		return NKCUIGauntletLRSlot.GetNewInstance(m_trRankAll, OnEventPanelBeginDragAll)?.GetComponent<RectTransform>();
	}

	public void ProvideDataAll(Transform tr, int index)
	{
		NKCUIGauntletLRSlot component = tr.GetComponent<NKCUIGauntletLRSlot>();
		if (component != null)
		{
			component.SetUI(GetUserSimpleData(RANK_TYPE.ALL, index), index + 1, NKM_GAME_TYPE.NGT_PVP_LEAGUE);
		}
	}

	public RectTransform GetSlotFriend(int index)
	{
		return NKCUIGauntletLRSlot.GetNewInstance(m_trRankFriend, OnEventPanelBeginDragFriend)?.GetComponent<RectTransform>();
	}

	public void ProvideDataFriend(Transform tr, int index)
	{
		NKCUIGauntletLRSlot component = tr.GetComponent<NKCUIGauntletLRSlot>();
		if (component != null)
		{
			component.SetUI(GetUserSimpleData(RANK_TYPE.FRIEND, index), index + 1, NKM_GAME_TYPE.NGT_PVP_LEAGUE);
		}
	}

	private void RefreshRankTabCells()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (m_RANK_TYPE == RANK_TYPE.MY_LEAGUE)
			{
				RefreshScrollRect(m_lvsrRankMyLeague, m_RANK_TYPE, GetUserSimpleList(m_RANK_TYPE).Count);
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

	public void OnRecv(NKMPacket_PVP_CHARGE_POINT_REFRESH_ACK cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK)
	{
		m_RightSide.UpdatePVPPointUI();
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_WEEKLY_REWARD_ACK sPacket)
	{
		m_RightSide.UpdateNowSeasonPVPInfoUI(NKM_GAME_TYPE.NGT_PVP_LEAGUE);
		SendPVPListREQ(m_RANK_TYPE, all: false);
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_SEASON_REWARD_ACK sPacket)
	{
		m_RightSide.UpdateNowSeasonPVPInfoUI(NKM_GAME_TYPE.NGT_PVP_LEAGUE);
		SendPVPListREQ(m_RANK_TYPE, all: false);
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_RANK_LIST_ACK cNKMPacket_LEAGUE_PVP_RANK_LIST_ACK)
	{
		AddUserSimpleList(cNKMPacket_LEAGUE_PVP_RANK_LIST_ACK.rankType, cNKMPacket_LEAGUE_PVP_RANK_LIST_ACK.list);
		if (cNKMPacket_LEAGUE_PVP_RANK_LIST_ACK.rankType == RANK_TYPE.FRIEND)
		{
			GetUserSimpleList(cNKMPacket_LEAGUE_PVP_RANK_LIST_ACK.rankType).Sort((NKMUserSimpleProfileData a, NKMUserSimpleProfileData b) => b.pvpScore.CompareTo(a.pvpScore));
		}
		if (cNKMPacket_LEAGUE_PVP_RANK_LIST_ACK.list.Count < NKMPvpCommonConst.Instance.RANK_SIMPLE_COUNT)
		{
			m_arAllRankREQ[(int)cNKMPacket_LEAGUE_PVP_RANK_LIST_ACK.rankType] = true;
		}
		if (m_RANK_TYPE == cNKMPacket_LEAGUE_PVP_RANK_LIST_ACK.rankType)
		{
			RefreshRankTabCells();
		}
	}

	public void SetUI()
	{
		m_bPlayIntro = true;
		if (m_bAlertDemotion)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_GAUNTLET_DEMOTE_WARNING);
			m_bAlertDemotion = false;
		}
		if (!m_bPrepareLoopScrollCells)
		{
			NKCUtil.SetGameobjectActive(m_objRankMyLeague, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRankAll, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRankFriend, bValue: true);
			m_lvsrRankMyLeague.PrepareCells();
			m_lvsrRankAll.PrepareCells();
			m_lvsrRankFriend.PrepareCells();
			m_bPrepareLoopScrollCells = true;
		}
		m_RightSide.UpdateNowSeasonPVPInfoUI(NKM_GAME_TYPE.NGT_PVP_LEAGUE);
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
			m_ctglRankMyLeagueTab.Select(bSelect: false, bForce: true);
			m_ctglRankMyLeagueTab.Select(bSelect: true);
		}
		else if (m_RANK_TYPE == RANK_TYPE.ALL)
		{
			m_ctglRankAllTab.Select(bSelect: false, bForce: true);
			m_ctglRankAllTab.Select(bSelect: true);
		}
		else if (m_RANK_TYPE == RANK_TYPE.FRIEND)
		{
			m_ctglRankFriendTab.Select(bSelect: false, bForce: true);
			m_ctglRankFriendTab.Select(bSelect: true);
		}
		else
		{
			m_ctglRankMyLeagueTab.Select(bSelect: false, bForce: true);
			m_ctglRankMyLeagueTab.Select(bSelect: true);
		}
		UpdateRemainTimeUI();
		m_RightSide.UpdateReadyButtonUI();
		m_RightSide.UpdatePVPPointUI();
		m_RightSide.UpdateBattleCondition();
		SetRankTabUI();
		TutorialCheck();
	}

	private void SetRankTabUI()
	{
		NKCUtil.SetGameobjectActive(m_objRankMyLeague, m_RANK_TYPE == RANK_TYPE.MY_LEAGUE);
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
		NKMPacket_LEAGUE_PVP_RANK_LIST_REQ nKMPacket_LEAGUE_PVP_RANK_LIST_REQ = new NKMPacket_LEAGUE_PVP_RANK_LIST_REQ();
		nKMPacket_LEAGUE_PVP_RANK_LIST_REQ.rankType = m_RANK_TYPE;
		nKMPacket_LEAGUE_PVP_RANK_LIST_REQ.range = ((!all) ? LeaderBoardRangeType.TOP10 : LeaderBoardRangeType.ALL);
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_LEAGUE_PVP_RANK_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
	}

	private void IfCanSendRankListREQByCurrRankType()
	{
		if (!m_arRankREQ[(int)m_RANK_TYPE])
		{
			SendPVPListREQ(m_RANK_TYPE, all: false);
			m_arRankREQ[(int)m_RANK_TYPE] = true;
		}
	}

	private void OnRankTabChangedToMyLeague(bool bSet)
	{
		if (bSet)
		{
			m_RANK_TYPE = RANK_TYPE.MY_LEAGUE;
		}
		IfCanSendRankListREQByCurrRankType();
		SetRankTabUI();
	}

	private void OnRankTabChangedToAll(bool bSet)
	{
		if (bSet)
		{
			m_RANK_TYPE = RANK_TYPE.ALL;
		}
		IfCanSendRankListREQByCurrRankType();
		SetRankTabUI();
	}

	private void OnRankTabChangedToFriend(bool bSet)
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
		m_lvsrRankMyLeague.ClearCells();
		m_lvsrRankFriend.ClearCells();
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

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.GauntletLobbyLeague);
	}
}
