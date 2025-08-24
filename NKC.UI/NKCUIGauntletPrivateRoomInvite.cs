using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Pvp;
using NKC.UI.Gauntlet;
using NKM;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIGauntletPrivateRoomInvite : NKCUIBase
{
	private enum PRIVATE_PVP_TAB_TYPE
	{
		FRIEND,
		GUILD,
		SEARCH
	}

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	public const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_PRIVATE_ROOM_INVITE_POPUP";

	public TMP_Text m_lbObserveCount;

	public LoopVerticalScrollRect m_lvsrRankFriend;

	public Transform m_trRankFriend;

	public RectTransform m_LayoutGroup;

	[Header("친구")]
	public NKCUIComToggle m_ctglRankFriendTab;

	public GameObject m_objEmptyList;

	public Text m_lbEmptyMessage;

	[Header("검색")]
	public NKCUIComToggle m_ctglTabSearchID;

	public GameObject m_objSearch;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON_DISABLE;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH_Disable;

	public InputField m_IFNicknameOrUID;

	private static float m_minimumSearchDelay;

	private static float m_minimumRefreshDelay;

	private const float MIN_SEARCH_DELAY = 9f;

	private const float MIN_REFRESH_DELAY = 9f;

	[Header("연합원")]
	public NKCUIComToggle m_ctglTabGuild;

	private PRIVATE_PVP_TAB_TYPE m_currentTabType;

	private bool m_bFirstOpen = true;

	private bool m_bFriendTabOpened;

	private bool m_bSearchTabOpened;

	private bool m_bGuildTabOpened;

	private float m_fPrevUpdateTime;

	[Header("공통")]
	public NKCUIComStateButton m_cbtnClose;

	public EventTrigger m_evtBG;

	private List<FriendListData> m_friendSlotDataList = new List<FriendListData>();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET;

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private bool IsGuildOpened()
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

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		SetUI();
		UIOpened();
	}

	public void Init()
	{
		NKCPrivatePVPRoomMgr.ResetSearchData();
		if (m_cbtnClose != null)
		{
			m_cbtnClose.PointerClick.RemoveAllListeners();
			m_cbtnClose.PointerClick.AddListener(base.Close);
		}
		if (m_evtBG != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnClickBg);
			m_evtBG.triggers.Add(entry);
		}
		m_ctglRankFriendTab.OnValueChanged.RemoveAllListeners();
		m_ctglRankFriendTab.OnValueChanged.AddListener(OnTabChangedToFriend);
		m_lvsrRankFriend.dOnGetObject += GetSlotFriend;
		m_lvsrRankFriend.dOnReturnObject += ReturnSlot;
		m_lvsrRankFriend.dOnProvideData += ProvideSlotData;
		m_lvsrRankFriend.dOnRepopulate += ResizeScrollRect;
		m_lvsrRankFriend.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_lvsrRankFriend);
		if (m_ctglTabSearchID != null)
		{
			m_ctglTabSearchID.OnValueChanged.RemoveAllListeners();
			m_ctglTabSearchID.OnValueChanged.AddListener(OnTabChangedToSearchID);
		}
		NKCUtil.SetGameobjectActive(m_ctglTabSearchID, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_SEARCH));
		if (m_ctglTabGuild != null)
		{
			m_ctglTabGuild.OnValueChanged.RemoveAllListeners();
			m_ctglTabGuild.OnValueChanged.AddListener(OnTabChangedToGuild);
		}
		NKCUtil.SetGameobjectActive(m_ctglTabGuild, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PRIVATE_GUILD));
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
	}

	private void OnTabChangedToFriend(bool bSet)
	{
		if (bSet)
		{
			m_bSearchTabOpened = false;
			m_bGuildTabOpened = false;
			m_currentTabType = PRIVATE_PVP_TAB_TYPE.FRIEND;
			UpdateFriendList();
			RefreshTabCells();
			NKCUtil.SetGameobjectActive(m_objSearch, bValue: false);
			ResizeScrollRect();
		}
	}

	private void OnTabChangedToSearchID(bool bSet)
	{
		if (bSet)
		{
			m_bFriendTabOpened = false;
			m_bGuildTabOpened = false;
			m_currentTabType = PRIVATE_PVP_TAB_TYPE.SEARCH;
			UpdateSearchList();
			RefreshTabCells();
			NKCUtil.SetGameobjectActive(m_objSearch, bValue: true);
			NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON, m_minimumSearchDelay <= 0f);
			NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON_DISABLE, !m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON.gameObject.activeSelf);
			ResizeScrollRect();
		}
	}

	private void OnTabChangedToGuild(bool bSet)
	{
		if (bSet)
		{
			m_bFriendTabOpened = false;
			m_bSearchTabOpened = false;
			m_currentTabType = PRIVATE_PVP_TAB_TYPE.GUILD;
			UpdateGuildList();
			RefreshTabCells();
			NKCUtil.SetGameobjectActive(m_objSearch, bValue: false);
			ResizeScrollRect();
		}
	}

	private void Update()
	{
		if (m_minimumSearchDelay > 0f)
		{
			if (m_minimumSearchDelay < Time.time && m_minimumSearchDelay > 0f)
			{
				m_minimumSearchDelay = 0f;
			}
			if (m_minimumSearchDelay <= 0f)
			{
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
		}
	}

	private void OnEventPanelBeginDragFriend()
	{
	}

	private void ReturnSlot(Transform tr)
	{
		NKCUIGauntletPrivateRoomFriendSlot component = tr.GetComponent<NKCUIGauntletPrivateRoomFriendSlot>();
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

	private RectTransform GetSlotFriend(int index)
	{
		return NKCUIGauntletPrivateRoomFriendSlot.GetNewInstance(null, OnEventPanelBeginDragFriend)?.GetComponent<RectTransform>();
	}

	private void ProvideSlotData(Transform tr, int index)
	{
		NKCUIGauntletPrivateRoomFriendSlot component = tr.GetComponent<NKCUIGauntletPrivateRoomFriendSlot>();
		if (component != null)
		{
			component.SetUI(GetUserSimpleData(index), showTimeAndButtons: true);
		}
	}

	private void ProvideSlotDataGuild(Transform tr, int index)
	{
		NKCUIGauntletPrivateRoomFriendSlot component = tr.GetComponent<NKCUIGauntletPrivateRoomFriendSlot>();
		if (component != null)
		{
			component.SetUI(GetUserSimpleDataGuild(index), showTimeAndButtons: true);
		}
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
		m_lvsrRankFriend.TotalCount = m_friendSlotDataList.Count;
		m_lvsrRankFriend.velocity = Vector2.zero;
		m_lvsrRankFriend.SetIndexPosition(0);
		if (!m_bSearchTabOpened)
		{
			m_bSearchTabOpened = true;
		}
		NKCUtil.SetLabelText(m_lbEmptyMessage, NKCStringTable.GetString("SI_PF_CONSORTIUM_MEMBER_INVITE_NO_RESULT"));
		NKCUtil.SetGameobjectActive(m_objEmptyList, m_lvsrRankFriend.TotalCount == 0);
	}

	private void RefreshScrollRectGuild()
	{
		m_lvsrRankFriend.TotalCount = 0;
		if (IsGuildOpened())
		{
			m_lvsrRankFriend.TotalCount = m_friendSlotDataList.Count;
		}
		if (!m_bGuildTabOpened)
		{
			m_bGuildTabOpened = true;
			m_lvsrRankFriend.velocity = Vector2.zero;
			m_lvsrRankFriend.SetIndexPosition(0);
		}
		else
		{
			m_lvsrRankFriend.RefreshCells();
		}
		NKCUtil.SetLabelText(m_lbEmptyMessage, NKCUtilString.GET_STRING_PRIVATE_PVP_GUILD_MEMBER_EMPTY);
		NKCUtil.SetGameobjectActive(m_objEmptyList, m_lvsrRankFriend.TotalCount == 0);
	}

	public void SetUI()
	{
		if (m_bFirstOpen)
		{
			m_bFriendTabOpened = false;
			m_bSearchTabOpened = false;
			m_bGuildTabOpened = false;
			m_bFirstOpen = false;
			NKCUtil.SetGameobjectActive(m_objSearch, bValue: false);
			ResizeScrollRect();
			m_lvsrRankFriend.PrepareCells();
		}
		switch (m_currentTabType)
		{
		case PRIVATE_PVP_TAB_TYPE.FRIEND:
			m_ctglRankFriendTab.Select(bSelect: false, bForce: true);
			m_ctglRankFriendTab.Select(bSelect: true);
			break;
		case PRIVATE_PVP_TAB_TYPE.GUILD:
			m_ctglTabGuild.Select(bSelect: false, bForce: true);
			m_ctglTabGuild.Select(bSelect: true);
			break;
		case PRIVATE_PVP_TAB_TYPE.SEARCH:
			m_ctglTabSearchID.Select(bSelect: false, bForce: true);
			m_ctglTabSearchID.Select(bSelect: true);
			break;
		}
		RefreshObserveCount();
	}

	public void RefreshObserveCount()
	{
		List<NKMPvpGameLobbyUserState> playerList = NKCPrivatePVPRoomMgr.GetPlayerList();
		NKCUtil.SetLabelText(m_lbObserveCount, $"{playerList.Count + 1}/{NKMPvpCommonConst.Instance.PvpRoomMaxPlayerCount}");
	}

	private void ClearCacheData()
	{
		m_lvsrRankFriend.ClearCells();
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
			friendListData.guildData = NKCGuildManager.GetMyGuildSimpleData();
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

	private void ResizeScrollRect()
	{
		Transform transform = m_LayoutGroup?.transform.GetChild(0);
		Transform transform2 = m_LayoutGroup?.transform.GetChild(1);
		if (transform == null || transform2 == null)
		{
			return;
		}
		RectTransform component = transform.GetComponent<RectTransform>();
		RectTransform component2 = transform2.GetComponent<RectTransform>();
		if (!(component == null) && !(component2 == null))
		{
			VerticalLayoutGroup component3 = m_LayoutGroup.GetComponent<VerticalLayoutGroup>();
			if (!(component3 == null))
			{
				float num = (float)(component3.padding.top + component3.padding.bottom) + component3.spacing;
				float num2 = (m_objSearch.activeSelf ? component.rect.height : 0f);
				component2.sizeDelta = new Vector2(component2.rect.width, m_LayoutGroup.rect.height - num2 - num);
			}
		}
	}

	private void UpdateFriendList()
	{
		m_friendSlotDataList.Clear();
		m_friendSlotDataList.AddRange(NKCFriendManager.FriendListData);
		m_friendSlotDataList.Sort((FriendListData a, FriendListData b) => b.lastLoginDate.CompareTo(a.lastLoginDate));
	}

	private void UpdateSearchList()
	{
		m_friendSlotDataList.Clear();
		m_friendSlotDataList.AddRange(NKCPrivatePVPRoomMgr.SearchResult);
		m_friendSlotDataList.Sort((FriendListData a, FriendListData b) => b.lastLoginDate.CompareTo(a.lastLoginDate));
	}

	private void UpdateGuildList()
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

	private void OnClickSearch()
	{
		if (!(m_minimumSearchDelay > 0f))
		{
			UpdateSearchList();
			NKCPacketSender.Send_NKMPacket_PRIVATE_PVP_SEARCH_USER_REQ(m_IFNicknameOrUID.text);
			m_minimumSearchDelay = Time.time + 9f;
			NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON, bValue: false);
			NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON_DISABLE, bValue: true);
		}
	}

	private void OnClickRefresh()
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

	private void OnClickBg(BaseEventData cBaseEventData)
	{
		Close();
	}
}
