using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyCustom : MonoBehaviour
{
	private enum PRIVATE_PVP_TAB_TYPE
	{
		FRIEND,
		GUILD,
		SEARCH,
		ROOM
	}

	public Animator m_amtorRankCenter;

	[Header("친구")]
	public NKCUIComToggle m_ctglRankFriendTab;

	public GameObject m_objRankFriend;

	public LoopVerticalScrollRect m_lvsrRankFriend;

	public Transform m_trRankFriend;

	public GameObject m_objEmptyList;

	public Text m_lbEmptyMessage;

	[Header("검색")]
	public NKCUIComToggle m_ctglTabSearchID;

	public GameObject m_objSearchID;

	public LoopVerticalScrollRect m_scrollSearchID;

	public Transform m_scrollContentSearchID;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON_DISABLE;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH_Disable;

	public InputField m_IFNicknameOrUID;

	private float m_minimumSearchDelay;

	private float m_minimumRefreshDelay;

	private const float MIN_SEARCH_DELAY = 9f;

	private const float MIN_REFRESH_DELAY = 9f;

	[Header("연합원")]
	public NKCUIComToggle m_ctglTabGuild;

	public GameObject m_objGuild;

	public LoopVerticalScrollRect m_scrollGuild;

	public Transform m_scrollContentGuild;

	public GameObject m_objEmptyListGuild;

	public Text m_lbEmptyMessageGuild;

	[Header("방 생성")]
	public NKCUIComToggle m_ctglTabRoom;

	public GameObject m_objRoom;

	public NKCUIComButton m_cbtnNKM_UI_ROOM_CREATE_BUTTON;

	public NKCUIComStateButton m_csbtnRoomCreate;

	public GameObject m_objRoomOptionDesc;

	[Header("정보")]
	public NKCUIComStateButton m_csbtnBattleHistory;

	public NKCUIComStateButton m_csbtnRankMatchReady;

	public NKCUIComStateButton m_csbtnRankMatchReadyDisable;

	public NKCUIComStateButton m_csbtnEmoticonSetting;

	public NKCUIComStateButton m_csbtnBanList;

	public GameObject m_objMatchReady;

	public GameObject m_objMatchReadyDisable;

	[Header("우측정보")]
	public GameObject m_objDescGuild;

	public GameObject m_objDescSearch;

	public NKCUIGauntletPrivateRoomCustomOption m_CustomOption;

	[Header("버튼")]
	public NKCUIComStateButton m_csbtnGlobalBan;

	public NKCUIComStateButton m_csbtnObserveCode;

	private PRIVATE_PVP_TAB_TYPE m_currentTabType;

	private RANK_TYPE m_RANK_TYPE = RANK_TYPE.FRIEND;

	private bool m_bFirstOpen = true;

	private bool m_bPrepareLoopScrollCells;

	private bool m_bFriendTabOpened;

	private bool m_bSearchTabOpened;

	private bool m_bGuildTabOpened;

	private float m_fPrevUpdateTime;

	private bool m_bPlayIntro = true;

	private List<FriendListData> m_friendSlotDataList = new List<FriendListData>();

	public bool IsGuildOpened()
	{
		if (NKCGuildManager.MyGuildData == null)
		{
			Debug.Log("[PrivatePVP] NKCGuildManager.MyGuildData is null");
			return false;
		}
		if (NKCGuildManager.MyGuildMemberDataList == null)
		{
			Debug.Log("[PrivatePVP] NKCGuildManager.MyGuildMemberDataList is null");
			return false;
		}
		return NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_GUILD);
	}

	public void Init()
	{
		NKCPrivatePVPRoomMgr.ResetData();
		m_csbtnBattleHistory.PointerClick.RemoveAllListeners();
		m_csbtnBattleHistory.PointerClick.AddListener(OnClickBattleRecord);
		m_csbtnRankMatchReady.PointerClick.RemoveAllListeners();
		m_csbtnRankMatchReadyDisable.PointerClick.RemoveAllListeners();
		NKCUtil.SetBindFunction(m_csbtnEmoticonSetting, OnClickEmoticonSetting);
		NKCUtil.SetBindFunction(m_csbtnBanList, OnClickBanList);
		NKCUtil.SetGameobjectActive(m_objDescGuild, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_GUILD));
		NKCUtil.SetGameobjectActive(m_objDescSearch, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_SEARCH));
		m_ctglRankFriendTab.OnValueChanged.RemoveAllListeners();
		m_ctglRankFriendTab.OnValueChanged.AddListener(OnTabChangedToFriend);
		m_lvsrRankFriend.dOnGetObject += GetSlotFriend;
		m_lvsrRankFriend.dOnReturnObject += ReturnSlot;
		m_lvsrRankFriend.dOnProvideData += ProvideSlotData;
		m_lvsrRankFriend.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_lvsrRankFriend);
		if (m_ctglTabSearchID != null)
		{
			m_ctglTabSearchID.OnValueChanged.RemoveAllListeners();
			m_ctglTabSearchID.OnValueChanged.AddListener(OnTabChangedToSearchID);
			m_scrollSearchID.dOnGetObject += GetSlotSearch;
			m_scrollSearchID.dOnReturnObject += ReturnSlot;
			m_scrollSearchID.dOnProvideData += ProvideSlotData;
			m_scrollSearchID.ContentConstraintCount = 1;
			NKCUtil.SetScrollHotKey(m_scrollSearchID);
		}
		NKCUtil.SetGameobjectActive(m_ctglTabSearchID, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_SEARCH));
		if (m_ctglTabGuild != null)
		{
			m_ctglTabGuild.OnValueChanged.RemoveAllListeners();
			m_ctglTabGuild.OnValueChanged.AddListener(OnTabChangedToGuild);
			m_scrollGuild.dOnGetObject += GetSlotGuild;
			m_scrollGuild.dOnReturnObject += ReturnSlot;
			m_scrollGuild.dOnProvideData += ProvideSlotDataGuild;
			m_scrollGuild.ContentConstraintCount = 1;
			NKCUtil.SetScrollHotKey(m_scrollGuild);
		}
		NKCUtil.SetGameobjectActive(m_ctglTabGuild, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_GUILD));
		if (m_ctglTabRoom != null)
		{
			m_ctglTabRoom.OnValueChanged.RemoveAllListeners();
			m_ctglTabRoom.OnValueChanged.AddListener(OnTabChangedToRoom);
		}
		bool flag = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM);
		if (m_cbtnNKM_UI_ROOM_CREATE_BUTTON != null)
		{
			m_cbtnNKM_UI_ROOM_CREATE_BUTTON.PointerClick.RemoveAllListeners();
			m_cbtnNKM_UI_ROOM_CREATE_BUTTON.PointerClick.AddListener(OnClickCreateRoom);
		}
		if (m_IFNicknameOrUID != null)
		{
			m_IFNicknameOrUID.onEndEdit.RemoveAllListeners();
			m_IFNicknameOrUID.onEndEdit.AddListener(OnEndEditSearch);
		}
		if (m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON != null)
		{
			m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON.PointerClick.RemoveAllListeners();
			m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON.PointerClick.AddListener(OnClickSearch);
		}
		if (m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH != null)
		{
			NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH, bValue: false);
			m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH.PointerClick.RemoveAllListeners();
			m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH.PointerClick.AddListener(OnClickRefresh);
		}
		m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON_DISABLE?.Lock();
		m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH_Disable?.Lock();
		m_CustomOption?.Init();
		NKCUtil.SetBindFunction(m_csbtnGlobalBan, OnClickGlobalBan);
		NKCUtil.SetGameobjectActive(m_csbtnObserveCode, flag);
		NKCUtil.SetGameobjectActive(m_csbtnRoomCreate, flag);
		NKCUtil.SetGameobjectActive(m_objRoomOptionDesc, flag);
		if (flag)
		{
			NKCUtil.SetBindFunction(m_csbtnObserveCode, OnClickObserveCode);
			NKCUtil.SetButtonClickDelegate(m_csbtnRoomCreate, OnClickCreateRoom);
		}
	}

	private void RefreshTabUI()
	{
		if (base.gameObject.activeInHierarchy)
		{
			Debug.Log("[PrivatePVP] RefreshTabUI [" + m_currentTabType.ToString() + "]");
			NKCUtil.SetGameobjectActive(m_objRankFriend, m_currentTabType == PRIVATE_PVP_TAB_TYPE.FRIEND);
			NKCUtil.SetGameobjectActive(m_objSearchID, m_currentTabType == PRIVATE_PVP_TAB_TYPE.SEARCH);
			NKCUtil.SetGameobjectActive(m_objGuild, m_currentTabType == PRIVATE_PVP_TAB_TYPE.GUILD);
			NKCUtil.SetGameobjectActive(m_objRoom, m_currentTabType == PRIVATE_PVP_TAB_TYPE.ROOM);
			RefreshTabCells();
			if (!m_bPlayIntro)
			{
				m_amtorRankCenter.Play("NKM_UI_GAUNTLET_LOBBY_CONTENT_INTRO_CENTER_FADEIN");
			}
			else
			{
				m_bPlayIntro = false;
			}
		}
	}

	private void OnTabChangedToFriend(bool bSet)
	{
		if (bSet)
		{
			m_currentTabType = PRIVATE_PVP_TAB_TYPE.FRIEND;
			UpdateFriendList();
		}
		RefreshTabUI();
	}

	private void OnTabChangedToSearchID(bool bSet)
	{
		if (bSet)
		{
			m_currentTabType = PRIVATE_PVP_TAB_TYPE.SEARCH;
			UpdateSearchList();
		}
		RefreshTabUI();
		if (NKCPrivatePVPRoomMgr.SearchResult.Count == 0)
		{
			OnClickSearch();
		}
	}

	private void OnTabChangedToGuild(bool bSet)
	{
		Debug.Log($"[PrivatePVP] OnTabChangedToGuild [{bSet}]");
		if (bSet)
		{
			m_currentTabType = PRIVATE_PVP_TAB_TYPE.GUILD;
			UpdateGuildList();
		}
		RefreshTabUI();
	}

	private void OnTabChangedToRoom(bool bSet)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM))
		{
			Debug.Log($"[PrivatePVP] OnTabChangedToRoom [{bSet}]");
			if (bSet)
			{
				m_currentTabType = PRIVATE_PVP_TAB_TYPE.ROOM;
			}
			RefreshTabUI();
		}
	}

	private void OnClickBanList()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_CASTING_BAN))
		{
			NKCPopupGauntletBanListV2.Instance.Open();
		}
		else
		{
			NKCPopupGauntletBanList.Instance.Open();
		}
	}

	private void OnClickEmoticonSetting()
	{
		if (NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			NKCPopupEmoticonSetting.Instance.Open();
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetWaitForEmoticon(bValue: true);
			NKCPacketSender.Send_NKMPacket_EMOTICON_DATA_REQ();
		}
	}

	private void UpdateReadyButtonUI()
	{
	}

	private void Update()
	{
		if (m_minimumSearchDelay > 0f)
		{
			m_minimumSearchDelay -= Time.deltaTime;
			if (m_minimumSearchDelay <= 0f)
			{
				m_minimumSearchDelay = 0f;
				NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON, bValue: true);
				NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON_DISABLE, bValue: false);
			}
		}
		if (m_minimumRefreshDelay > 0f)
		{
			m_minimumRefreshDelay -= Time.deltaTime;
			if (m_minimumRefreshDelay <= 0f)
			{
				m_minimumRefreshDelay = 0f;
				NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH, bValue: true);
				NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH_Disable, bValue: false);
			}
		}
		if (m_fPrevUpdateTime + 1f < Time.time)
		{
			m_fPrevUpdateTime = Time.time;
			UpdateReadyButtonUI();
		}
	}

	private void OnEventPanelBeginDragFriend()
	{
	}

	public void ReturnSlot(Transform tr)
	{
		NKCUIGauntletFriendSlot component = tr.GetComponent<NKCUIGauntletFriendSlot>();
		tr.SetParent(base.transform);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	public RectTransform GetSlotFriend(int index)
	{
		return NKCUIGauntletFriendSlot.GetNewInstance(m_trRankFriend, OnEventPanelBeginDragFriend)?.GetComponent<RectTransform>();
	}

	public void ProvideSlotData(Transform tr, int index)
	{
		NKCUIGauntletFriendSlot component = tr.GetComponent<NKCUIGauntletFriendSlot>();
		if (component != null)
		{
			component.SetUI(GetUserSimpleData(index), showTimeAndButtons: true);
		}
	}

	public void ProvideSlotDataGuild(Transform tr, int index)
	{
		NKCUIGauntletFriendSlot component = tr.GetComponent<NKCUIGauntletFriendSlot>();
		if (component != null)
		{
			component.SetUI(GetUserSimpleDataGuild(index), showTimeAndButtons: true);
		}
	}

	public RectTransform GetSlotGuild(int index)
	{
		return NKCUIGauntletFriendSlot.GetNewInstance(m_scrollContentGuild, OnEventPanelBeginDragFriend)?.GetComponent<RectTransform>();
	}

	public RectTransform GetSlotSearch(int index)
	{
		return NKCUIGauntletFriendSlot.GetNewInstance(m_scrollContentSearchID, OnEventPanelBeginDragFriend)?.GetComponent<RectTransform>();
	}

	private void RefreshTabCells()
	{
		if (base.gameObject.activeInHierarchy)
		{
			switch (m_currentTabType)
			{
			case PRIVATE_PVP_TAB_TYPE.FRIEND:
				RefreshScrollRectFriend();
				break;
			case PRIVATE_PVP_TAB_TYPE.SEARCH:
				RefreshScrollRectSearch();
				break;
			case PRIVATE_PVP_TAB_TYPE.GUILD:
				RefreshScrollRectGuild();
				break;
			case PRIVATE_PVP_TAB_TYPE.ROOM:
				RefreshScrollRectRoom();
				break;
			}
		}
	}

	private void RefreshScrollRectFriend()
	{
		m_lvsrRankFriend.TotalCount = m_friendSlotDataList.Count;
		if (!m_bFriendTabOpened)
		{
			m_bFriendTabOpened = true;
			m_lvsrRankFriend.velocity = Vector2.zero;
			m_lvsrRankFriend.SetIndexPosition(0);
		}
		else
		{
			m_lvsrRankFriend.RefreshCells();
		}
		NKCUtil.SetLabelText(m_lbEmptyMessage, NKCUtilString.GET_STRING_FRIEND_LIST_IS_EMPTY);
		NKCUtil.SetGameobjectActive(m_objEmptyList, m_lvsrRankFriend.TotalCount == 0);
	}

	private void RefreshScrollRectSearch()
	{
		m_scrollSearchID.TotalCount = m_friendSlotDataList.Count;
		m_scrollSearchID.SetIndexPosition(0);
		if (!m_bSearchTabOpened)
		{
			m_bSearchTabOpened = true;
			m_scrollSearchID.velocity = Vector2.zero;
		}
		else
		{
			m_scrollSearchID.RefreshCells();
		}
	}

	private void RefreshScrollRectGuild()
	{
		m_scrollGuild.TotalCount = 0;
		if (IsGuildOpened())
		{
			m_scrollGuild.TotalCount = NKCGuildManager.MyGuildMemberDataList.Count;
		}
		if (!m_bGuildTabOpened)
		{
			m_bGuildTabOpened = true;
			m_scrollGuild.velocity = Vector2.zero;
			m_scrollGuild.SetIndexPosition(0);
		}
		else
		{
			m_scrollGuild.RefreshCells();
		}
		NKCUtil.SetLabelText(m_lbEmptyMessageGuild, NKCUtilString.GET_STRING_PRIVATE_PVP_GUILD_MEMBER_EMPTY);
		NKCUtil.SetGameobjectActive(m_objEmptyListGuild, m_scrollGuild.TotalCount == 0);
	}

	private void RefreshScrollRectRoom()
	{
	}

	public void SetUI()
	{
		m_bPlayIntro = true;
		if (!m_bPrepareLoopScrollCells)
		{
			NKCUtil.SetGameobjectActive(m_objRankFriend, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSearchID, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_SEARCH));
			NKCUtil.SetGameobjectActive(m_objGuild, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_GUILD));
			m_lvsrRankFriend?.PrepareCells();
			m_scrollSearchID?.PrepareCells();
			m_scrollGuild?.PrepareCells();
			m_bPrepareLoopScrollCells = true;
		}
		if (m_bFirstOpen)
		{
			m_bFriendTabOpened = false;
			m_bSearchTabOpened = false;
			m_bGuildTabOpened = false;
			m_bFirstOpen = false;
		}
		switch (m_currentTabType)
		{
		case PRIVATE_PVP_TAB_TYPE.FRIEND:
			m_ctglRankFriendTab.Select(bSelect: false, bForce: true);
			m_ctglRankFriendTab.Select(bSelect: true);
			NKCUtil.SetGameobjectActive(m_objRankFriend, bValue: true);
			UpdateFriendList();
			break;
		case PRIVATE_PVP_TAB_TYPE.GUILD:
			m_ctglTabGuild.Select(bSelect: false, bForce: true);
			m_ctglTabGuild.Select(bSelect: true);
			NKCUtil.SetGameobjectActive(m_objGuild, bValue: true);
			UpdateGuildList();
			break;
		case PRIVATE_PVP_TAB_TYPE.SEARCH:
			m_ctglTabSearchID.Select(bSelect: false, bForce: true);
			m_ctglTabSearchID.Select(bSelect: true);
			NKCUtil.SetGameobjectActive(m_objSearchID, bValue: true);
			UpdateSearchList();
			break;
		case PRIVATE_PVP_TAB_TYPE.ROOM:
			m_ctglTabRoom.Select(bSelect: false, bForce: true);
			m_ctglTabRoom.Select(bSelect: true);
			NKCUtil.SetGameobjectActive(m_objRoom, bValue: true);
			UpdateRoomList();
			break;
		}
		UpdateReadyButtonUI();
		RefreshTabCells();
		if (!m_bPlayIntro)
		{
			m_amtorRankCenter.Play("NKM_UI_GAUNTLET_LOBBY_CONTENT_INTRO_CENTER_FADEIN");
		}
		else
		{
			m_bPlayIntro = false;
		}
		m_CustomOption.SetOption(NKCPrivatePVPRoomMgr.PrivateGameConfig);
	}

	private void OnClickBattleRecord()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY()?.OpenBattleRecord(NKM_GAME_TYPE.NGT_PVP_PRIVATE);
	}

	public void ClearCacheData()
	{
		m_lvsrRankFriend.ClearCells();
		m_scrollSearchID.ClearCells();
		m_scrollGuild?.ClearCells();
	}

	public void Close()
	{
		m_bFirstOpen = true;
		NKCPopupGauntletBanList.CheckInstanceAndClose();
	}

	private FriendListData GetUserSimpleData(int index)
	{
		if (m_friendSlotDataList.Count > index)
		{
			return m_friendSlotDataList[index];
		}
		return null;
	}

	private FriendListData GetUserSimpleDataGuild(int index)
	{
		if (!IsGuildOpened())
		{
			return null;
		}
		Debug.Log($"[PrivatePVP] GetUserSimpleDataGuild index[{index}] / total[{NKCGuildManager.MyGuildMemberDataList.Count}]");
		if (NKCGuildManager.MyGuildMemberDataList.Count > index)
		{
			FriendListData friendListData = new FriendListData();
			friendListData.commonProfile = NKCGuildManager.MyGuildMemberDataList[index].commonProfile;
			friendListData.lastLoginDate = NKCGuildManager.MyGuildMemberDataList[index].lastOnlineTime;
			if (friendListData.commonProfile == null)
			{
				Debug.Log("[PrivatePVP] commonProfile is null");
				return null;
			}
			_ = friendListData.lastLoginDate;
			return friendListData;
		}
		return null;
	}

	public void UpdateFriendList()
	{
		m_friendSlotDataList.Clear();
		m_friendSlotDataList.AddRange(NKCFriendManager.FriendListData);
		m_friendSlotDataList.Sort((FriendListData a, FriendListData b) => b.lastLoginDate.CompareTo(a.lastLoginDate));
	}

	public void UpdateSearchList()
	{
		m_friendSlotDataList.Clear();
		m_friendSlotDataList.AddRange(NKCPrivatePVPRoomMgr.SearchResult);
		m_friendSlotDataList.Sort((FriendListData a, FriendListData b) => b.lastLoginDate.CompareTo(a.lastLoginDate));
	}

	public void UpdateGuildList()
	{
		Debug.Log("[PrivatePVP] UpdateGuildList");
		m_friendSlotDataList.Clear();
		if (!IsGuildOpened())
		{
			return;
		}
		for (int i = 0; i < NKCGuildManager.MyGuildMemberDataList.Count; i++)
		{
			FriendListData userSimpleDataGuild = GetUserSimpleDataGuild(i);
			if (userSimpleDataGuild != null)
			{
				m_friendSlotDataList.Add(userSimpleDataGuild);
			}
		}
		m_friendSlotDataList.Sort((FriendListData a, FriendListData b) => b.lastLoginDate.CompareTo(a.lastLoginDate));
	}

	public void UpdateRoomList()
	{
	}

	public void OnClickSearch()
	{
		if (!(m_minimumSearchDelay > 0f))
		{
			UpdateSearchList();
			NKCPacketSender.Send_NKMPacket_PRIVATE_PVP_SEARCH_USER_REQ(m_IFNicknameOrUID.text);
			m_minimumSearchDelay = 9f;
			NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON, bValue: false);
			NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON_DISABLE, bValue: true);
		}
	}

	public void OnClickRefresh()
	{
		if (!(m_minimumRefreshDelay > 0f))
		{
			NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH, bValue: false);
			NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH_Disable, bValue: true);
			NKCPacketSender.Send_NKMPacket_PRIVATE_PVP_SEARCH_USER_REQ("");
			m_minimumRefreshDelay = 9f;
		}
	}

	private void OnEndEditSearch(string input)
	{
		if (NKCInputManager.IsChatSubmitEnter())
		{
			if (!m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON.m_bLock)
			{
				OnClickSearch();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	private void OnClickCreateRoom()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_ROOM))
		{
			NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_LOBBY_CREATE_REQ();
		}
	}

	private void OnClickGlobalBan()
	{
		NKCPopupGauntletBan.Instance.Open();
	}

	private void OnClickObserveCode()
	{
		NKCPopupGauntletObserveCode.Instance.Open();
	}
}
