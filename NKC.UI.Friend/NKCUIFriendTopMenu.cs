using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Community;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Friend;

public class NKCUIFriendTopMenu : MonoBehaviour
{
	public enum FRIEND_SORT_TYPE
	{
		FST_LAST_LOGIN,
		FST_LEVEL
	}

	public class NKCUIFriendSlotMgr
	{
		public class CompLevelDescending : IComparer<NKCUIFriendSlot>
		{
			public int Compare(NKCUIFriendSlot x, NKCUIFriendSlot y)
			{
				if (!x.IsActive())
				{
					return 1;
				}
				if (!y.IsActive())
				{
					return -1;
				}
				if (y.GetFriendListData().commonProfile.level >= x.GetFriendListData().commonProfile.level)
				{
					return 1;
				}
				return -1;
			}
		}

		public class CompLevelAscending : IComparer<NKCUIFriendSlot>
		{
			public int Compare(NKCUIFriendSlot x, NKCUIFriendSlot y)
			{
				if (!x.IsActive())
				{
					return 1;
				}
				if (!y.IsActive())
				{
					return -1;
				}
				if (y.GetFriendListData().commonProfile.level <= x.GetFriendListData().commonProfile.level)
				{
					return 1;
				}
				return -1;
			}
		}

		public class CompLoginTimeDescending : IComparer<NKCUIFriendSlot>
		{
			public int Compare(NKCUIFriendSlot x, NKCUIFriendSlot y)
			{
				if (!x.IsActive())
				{
					return 1;
				}
				if (!y.IsActive())
				{
					return -1;
				}
				if (y.GetFriendListData().lastLoginDate <= x.GetFriendListData().lastLoginDate)
				{
					return 1;
				}
				return -1;
			}
		}

		public class CompLoginTimeAscending : IComparer<NKCUIFriendSlot>
		{
			public int Compare(NKCUIFriendSlot x, NKCUIFriendSlot y)
			{
				if (!x.IsActive())
				{
					return 1;
				}
				if (!y.IsActive())
				{
					return -1;
				}
				if (y.GetFriendListData().lastLoginDate >= x.GetFriendListData().lastLoginDate)
				{
					return 1;
				}
				return -1;
			}
		}

		private const int DEFAULT_SIZE = 20;

		private List<NKCUIFriendSlot> m_lstNKCUIFriendSlot = new List<NKCUIFriendSlot>();

		private ScrollRect m_ScrollRect;

		private Transform m_ParentTransform;

		private bool m_bAscend;

		private FRIEND_SORT_TYPE m_FRIEND_SORT_TYPE;

		public void Init(ScrollRect scrollRect, Transform parentTransform)
		{
			m_ScrollRect = scrollRect;
			m_ParentTransform = parentTransform;
			for (int i = 0; i < 20; i++)
			{
				NKCUIFriendSlot newInstance = NKCUIFriendSlot.GetNewInstance(m_ParentTransform);
				if (newInstance != null)
				{
					m_lstNKCUIFriendSlot.Add(newInstance);
					newInstance.SetActive(bSet: false);
				}
			}
		}

		public void SetAscend(bool bAscend)
		{
			if (m_bAscend != bAscend)
			{
				m_bAscend = bAscend;
				Sort();
			}
		}

		public void SetFriendSortType(FRIEND_SORT_TYPE _FRIEND_SORT_TYPE)
		{
			if (m_FRIEND_SORT_TYPE != _FRIEND_SORT_TYPE)
			{
				m_FRIEND_SORT_TYPE = _FRIEND_SORT_TYPE;
				Sort();
			}
		}

		private void Sort()
		{
			if (m_bAscend)
			{
				if (m_FRIEND_SORT_TYPE == FRIEND_SORT_TYPE.FST_LAST_LOGIN)
				{
					CompLoginTimeAscending comparer = new CompLoginTimeAscending();
					m_lstNKCUIFriendSlot.Sort(comparer);
				}
				else if (m_FRIEND_SORT_TYPE == FRIEND_SORT_TYPE.FST_LEVEL)
				{
					CompLevelAscending comparer2 = new CompLevelAscending();
					m_lstNKCUIFriendSlot.Sort(comparer2);
				}
			}
			else if (m_FRIEND_SORT_TYPE == FRIEND_SORT_TYPE.FST_LAST_LOGIN)
			{
				CompLoginTimeDescending comparer3 = new CompLoginTimeDescending();
				m_lstNKCUIFriendSlot.Sort(comparer3);
			}
			else if (m_FRIEND_SORT_TYPE == FRIEND_SORT_TYPE.FST_LEVEL)
			{
				CompLevelDescending comparer4 = new CompLevelDescending();
				m_lstNKCUIFriendSlot.Sort(comparer4);
			}
			int num = 0;
			for (num = 0; num < m_lstNKCUIFriendSlot.Count; num++)
			{
				NKCUIFriendSlot nKCUIFriendSlot = m_lstNKCUIFriendSlot[num];
				if (nKCUIFriendSlot.IsActive())
				{
					nKCUIFriendSlot.transform.SetSiblingIndex(num);
				}
			}
		}

		public NKCUIFriendSlot GetSlot(int index)
		{
			if (index < 0 || index >= m_lstNKCUIFriendSlot.Count)
			{
				return null;
			}
			return m_lstNKCUIFriendSlot[index];
		}

		public int GetActiveSlotCount()
		{
			int num = 0;
			for (int i = 0; i < m_lstNKCUIFriendSlot.Count; i++)
			{
				NKCUIFriendSlot nKCUIFriendSlot = m_lstNKCUIFriendSlot[i];
				if (nKCUIFriendSlot != null && nKCUIFriendSlot.IsActive() && nKCUIFriendSlot.GetFriendListData() != null)
				{
					num++;
				}
			}
			return num;
		}

		public bool InvalidSlot(long friendCode)
		{
			for (int i = 0; i < m_lstNKCUIFriendSlot.Count; i++)
			{
				NKCUIFriendSlot nKCUIFriendSlot = m_lstNKCUIFriendSlot[i];
				if (nKCUIFriendSlot != null && nKCUIFriendSlot.IsActive() && nKCUIFriendSlot.GetFriendListData() != null && nKCUIFriendSlot.GetFriendListData().commonProfile.friendCode == friendCode)
				{
					nKCUIFriendSlot.SetActive(bSet: false);
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			for (int i = 0; i < m_lstNKCUIFriendSlot.Count; i++)
			{
				NKCUIFriendSlot nKCUIFriendSlot = m_lstNKCUIFriendSlot[i];
				if (nKCUIFriendSlot != null)
				{
					nKCUIFriendSlot.SetActive(bSet: false);
				}
			}
		}

		public void CloseSlotInstance()
		{
			foreach (NKCUIFriendSlot item in m_lstNKCUIFriendSlot)
			{
				item?.Clear();
			}
		}

		public void Add(FriendListData friend, NKCUIFriendSlot.FRIEND_SLOT_TYPE slotType)
		{
			if (GetActiveSlotCount() <= m_lstNKCUIFriendSlot.Count)
			{
				NKCUIFriendSlot newInstance = NKCUIFriendSlot.GetNewInstance(m_ParentTransform);
				if (newInstance != null)
				{
					m_lstNKCUIFriendSlot.Add(newInstance);
					newInstance.SetActive(bSet: false);
				}
			}
			if (friend != null)
			{
				m_lstNKCUIFriendSlot[GetActiveSlotCount()].SetData(slotType, friend);
			}
			for (int i = GetActiveSlotCount(); i < m_lstNKCUIFriendSlot.Count; i++)
			{
				m_lstNKCUIFriendSlot[i].SetActive(bSet: false);
			}
			Sort();
		}

		public void SetForSearch(bool bRecommend, List<FriendListData> friend_list)
		{
			if (friend_list.Count > m_lstNKCUIFriendSlot.Count)
			{
				int count = m_lstNKCUIFriendSlot.Count;
				for (int i = 0; i < friend_list.Count - count; i++)
				{
					NKCUIFriendSlot newInstance = NKCUIFriendSlot.GetNewInstance(m_ParentTransform);
					if (newInstance != null)
					{
						m_lstNKCUIFriendSlot.Add(newInstance);
						newInstance.SetActive(bSet: false);
					}
				}
			}
			int num = 0;
			for (num = 0; num < friend_list.Count; num++)
			{
				FriendListData friendListData = friend_list[num];
				if (friendListData != null)
				{
					if (bRecommend)
					{
						m_lstNKCUIFriendSlot[num].SetData(NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_SEARCH_RECOMMEND, friendListData);
					}
					else
					{
						m_lstNKCUIFriendSlot[num].SetData(NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_SEARCH, friendListData);
					}
				}
			}
			for (int j = num; j < m_lstNKCUIFriendSlot.Count; j++)
			{
				m_lstNKCUIFriendSlot[j].SetActive(bSet: false);
			}
			Sort();
		}

		public void Set(NKM_FRIEND_LIST_TYPE list_type, List<FriendListData> friend_list)
		{
			if (friend_list.Count > m_lstNKCUIFriendSlot.Count)
			{
				int count = m_lstNKCUIFriendSlot.Count;
				for (int i = 0; i < friend_list.Count - count; i++)
				{
					NKCUIFriendSlot newInstance = NKCUIFriendSlot.GetNewInstance(m_ParentTransform);
					if (newInstance != null)
					{
						m_lstNKCUIFriendSlot.Add(newInstance);
						newInstance.SetActive(bSet: false);
					}
				}
			}
			int num = 0;
			for (num = 0; num < friend_list.Count; num++)
			{
				FriendListData friendListData = friend_list[num];
				if (friendListData != null)
				{
					NKCUIFriendSlot.FRIEND_SLOT_TYPE slotType = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_LIST;
					switch (list_type)
					{
					case NKM_FRIEND_LIST_TYPE.FRIEND:
						slotType = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_LIST;
						break;
					case NKM_FRIEND_LIST_TYPE.BLOCKER:
						slotType = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_BLOCK_LIST;
						break;
					case NKM_FRIEND_LIST_TYPE.RECEIVE_REQUEST:
						slotType = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_RECEIVE_REQ;
						break;
					case NKM_FRIEND_LIST_TYPE.SEND_REQUEST:
						slotType = NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_SENT_REQ;
						break;
					}
					m_lstNKCUIFriendSlot[num].SetData(slotType, friendListData);
				}
			}
			for (int j = num; j < m_lstNKCUIFriendSlot.Count; j++)
			{
				m_lstNKCUIFriendSlot[j].SetActive(bSet: false);
			}
			Sort();
			m_ScrollRect.normalizedPosition = Vector2.zero;
		}
	}

	public enum FRIEND_TOP_MENU_TYPE
	{
		FTMT_MANAGE,
		FTMT_REGISTER,
		FTMT_MY_PROFILE
	}

	public GameObject m_NKM_UI_FRIEND_TOP_COUNT;

	public GameObject m_NKM_UI_FRIEND_TOP_SEARCH;

	public GameObject m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT;

	public GameObject m_NKM_UI_FRIEND_TOP_SUBMENU_ADD;

	public GameObject m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH;

	public Text m_NKM_UI_FRIEND_TOP_COUNT_TEXT;

	public NKCUIComToggle m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT_LIST;

	public NKCUIComToggle m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT_BLOCK;

	public NKCUIComToggle m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_SEARCH;

	public NKCUIComToggle m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_RECEIVE;

	public NKCUIComToggle m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_SEND;

	public GameObject m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_RECEIVE_NEW;

	public Text m_NKM_UI_FRIEND_TOP_SORTING_TEXT;

	public GameObject m_NKM_UI_FRIEND_TOP_SORTING_MENU;

	public GameObject m_NKM_UI_FRIEND_TOP_SORTING_MENU_01;

	public GameObject m_NKM_UI_FRIEND_TOP_SORTING_MENU_02;

	public GameObject m_NKM_UI_FRIEND_LIST;

	public Transform m_ParentOfFriendSlots;

	public ScrollRect m_ScrollRect;

	public InputField m_IFNicknameOrUID;

	public GameObject m_objEmpty;

	public Text m_lbEmptyMessage;

	public GameObject m_NKM_UI_FRIEND_TOP_SORT;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SORTING;

	public NKCUIComToggle m_cbtlNKM_UI_FRIEND_TOP_ARRAY;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH_Disable;

	private float m_fNextPossibleRefreshTime;

	private const float REFRESH_INTERVAL_TIME = 3f;

	private FRIEND_TOP_MENU_TYPE m_FRIEND_TOP_MENU_TYPE;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SORTING_MENU_01;

	public NKCUIComButton m_cbtnNKM_UI_FRIEND_TOP_SORTING_MENU_02;

	private NKCUIFriendSlotMgr m_NKCUIFriendSlotMgr = new NKCUIFriendSlotMgr();

	[Header("멘토")]
	public GameObject m_NKM_UI_FRIEND_MENTORING_LIST_INFO;

	public GameObject m_NKM_UI_FRIEND_LIST_ScrollView;

	public GameObject m_NKM_UI_FRIEND_MENTORIG_LIST_ScrollView;

	public GameObject m_MENTORING_INVITE_REWARD_TIME;

	public GameObject m_NKM_UI_FRIEND_TOP_SUBMENU_MENTORING;

	public Text m_MENTORIG_REWARD_TIME;

	public GameObject m_m_NKM_UI_FRIEND_MENTORING_INVITE_ILLUST_VIEW;

	public NKCUIComToggle m_NKM_UI_FRIEND_TOP_SUBMENU_MENTORING_LIST;

	public NKCUIComToggle m_NKM_UI_FRIEND_TOP_SUBMENU_MENTORING_ADD;

	public NKCUIComToggle m_NKM_UI_FRIEND_TOP_SUBMENU_MENTORING_INVITE;

	public NKCUIComStateButton m_sbtnNKM_UI_FRIEND_TOP_SUBMENU_MENTORING_INVITE;

	public GameObject m_SUBMENU_MENTORING_INVITE_ICON_OFF;

	public GameObject m_SUBMENU_MENTORING_INVITE_ICON_OFF_LOCK;

	public GameObject m_MENTORING_COUNT;

	public Text m_MENTORING_COUNT_TEXT;

	public void Init()
	{
		m_NKCUIFriendSlotMgr.Init(m_ScrollRect, m_ParentOfFriendSlots);
		m_NKCUIFriendSlotMgr.SetAscend(bAscend: true);
		m_NKCUIFriendSlotMgr.SetFriendSortType(FRIEND_SORT_TYPE.FST_LAST_LOGIN);
		if (m_cbtnNKM_UI_FRIEND_TOP_SORTING != null)
		{
			m_cbtnNKM_UI_FRIEND_TOP_SORTING.PointerClick.RemoveAllListeners();
			m_cbtnNKM_UI_FRIEND_TOP_SORTING.PointerClick.AddListener(OnClickOpenSortMenu);
		}
		if (m_cbtlNKM_UI_FRIEND_TOP_ARRAY != null)
		{
			m_cbtlNKM_UI_FRIEND_TOP_ARRAY.OnValueChanged.RemoveAllListeners();
			m_cbtlNKM_UI_FRIEND_TOP_ARRAY.OnValueChanged.AddListener(OnClickAscendToggle);
		}
		if (m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON != null)
		{
			m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON.PointerClick.RemoveAllListeners();
			m_cbtnNKM_UI_FRIEND_TOP_SEARCH_BUTTON.PointerClick.AddListener(OnClickSearch);
		}
		if (m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH != null)
		{
			m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH.PointerClick.RemoveAllListeners();
			m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH.PointerClick.AddListener(delegate
			{
				OnClickRefresh();
			});
		}
		if (m_cbtnNKM_UI_FRIEND_TOP_SORTING_MENU_01 != null)
		{
			m_cbtnNKM_UI_FRIEND_TOP_SORTING_MENU_01.PointerClick.RemoveAllListeners();
			m_cbtnNKM_UI_FRIEND_TOP_SORTING_MENU_01.PointerClick.AddListener(OnClickLastLoginSort);
		}
		if (m_cbtnNKM_UI_FRIEND_TOP_SORTING_MENU_02 != null)
		{
			m_cbtnNKM_UI_FRIEND_TOP_SORTING_MENU_02.PointerClick.RemoveAllListeners();
			m_cbtnNKM_UI_FRIEND_TOP_SORTING_MENU_02.PointerClick.AddListener(OnClickUserLevelSort);
		}
		if (m_IFNicknameOrUID != null)
		{
			m_IFNicknameOrUID.onEndEdit.RemoveAllListeners();
			m_IFNicknameOrUID.onEndEdit.AddListener(OnEndEditSearch);
		}
	}

	private void OnClickRefresh()
	{
		OnClickFriendSearch();
		m_fNextPossibleRefreshTime = Time.time + 3f;
	}

	private void Update()
	{
		NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH, m_fNextPossibleRefreshTime < Time.time);
		NKCUtil.SetGameobjectActive(m_cbtnNKM_UI_FRIEND_TOP_SUBMENU_REFRESH_Disable, m_fNextPossibleRefreshTime >= Time.time);
	}

	public void SetAddReceiveNew(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_RECEIVE_NEW, bSet);
	}

	public void OnClickSearch()
	{
		m_NKCUIFriendSlotMgr.Clear();
		if (string.IsNullOrWhiteSpace(m_IFNicknameOrUID.text))
		{
			NKMPacket_FRIEND_RECOMMEND_REQ packet = new NKMPacket_FRIEND_RECOMMEND_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
		else
		{
			NKMPacket_FRIEND_SEARCH_REQ nKMPacket_FRIEND_SEARCH_REQ = new NKMPacket_FRIEND_SEARCH_REQ();
			nKMPacket_FRIEND_SEARCH_REQ.searchKeyword = m_IFNicknameOrUID.text;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_SEARCH_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
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

	public void OnClickAscendToggle(bool bCheck)
	{
		m_NKCUIFriendSlotMgr.SetAscend(bCheck);
	}

	public void OnRecv(NKMPacket_FRIEND_BLOCK_ACK cNKMPacket_FRIEND_BLOCK_ACK)
	{
		if (m_NKCUIFriendSlotMgr.InvalidSlot(cNKMPacket_FRIEND_BLOCK_ACK.friendCode))
		{
			SetCountUI();
		}
	}

	public void OnRecv(NKMPacket_FRIEND_ACCEPT_NOT cNKMPacket_FRIEND_ACCEPT_NOT)
	{
		if (cNKMPacket_FRIEND_ACCEPT_NOT.isAllow && IsFriendListShowing())
		{
			m_NKCUIFriendSlotMgr.Add(cNKMPacket_FRIEND_ACCEPT_NOT.friendProfileData, NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_LIST);
			SetCountUI();
		}
		if (IsSentREQListShowing())
		{
			m_NKCUIFriendSlotMgr.InvalidSlot(cNKMPacket_FRIEND_ACCEPT_NOT.friendProfileData.commonProfile.friendCode);
			SetCountUI();
		}
	}

	public void OnRecv(NKMPacket_FRIEND_DELETE_NOT cNKMPacket_FRIEND_DEL_NOT)
	{
		if (IsFriendListShowing() && m_NKCUIFriendSlotMgr.InvalidSlot(cNKMPacket_FRIEND_DEL_NOT.friendCode))
		{
			SetCountUI();
		}
	}

	public void OnRecv(NKMPacket_FRIEND_REQUEST_NOT cNKMPacket_FRIEND_ADD_NOT)
	{
		if (IsReceivedREQListShowing())
		{
			m_NKCUIFriendSlotMgr.Add(cNKMPacket_FRIEND_ADD_NOT.friendProfileData, NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_RECEIVE_REQ);
			SetCountUI();
		}
	}

	public void OnRecv(NKMPacket_FRIEND_CANCEL_REQUEST_NOT cNKMPacket_FRIEND_ADD_CANCEL_NOT)
	{
		if (IsReceivedREQListShowing() && m_NKCUIFriendSlotMgr.InvalidSlot(cNKMPacket_FRIEND_ADD_CANCEL_NOT.friendCode))
		{
			SetCountUI();
		}
	}

	public void OnRecv(NKMPacket_FRIEND_DELETE_ACK cNKMPacket_FRIEND_DEL_ACK)
	{
		if (IsFriendListShowing() && m_NKCUIFriendSlotMgr.InvalidSlot(cNKMPacket_FRIEND_DEL_ACK.friendCode))
		{
			SetCountUI();
		}
	}

	public void OnRecv(NKMPacket_FRIEND_SEARCH_ACK cNKMPacket_FRIEND_SEARCH_ACK)
	{
		if (IsSearchShowing())
		{
			m_NKCUIFriendSlotMgr.SetForSearch(bRecommend: false, cNKMPacket_FRIEND_SEARCH_ACK.list);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_RECOMMEND_ACK cNKMPacket_FRIEND_RECOMMEND_ACK)
	{
		if (IsSearchShowing())
		{
			m_NKCUIFriendSlotMgr.SetForSearch(bRecommend: true, cNKMPacket_FRIEND_RECOMMEND_ACK.list);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_REQUEST_ACK cNKMPacket_FRIEND_ADD_ACK)
	{
		if (IsSearchShowing())
		{
			m_NKCUIFriendSlotMgr.InvalidSlot(cNKMPacket_FRIEND_ADD_ACK.friendCode);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_ACCEPT_ACK cNKMPacket_FRIEND_ACCEPT_ACK)
	{
		if (IsReceivedREQListShowing() && m_NKCUIFriendSlotMgr.InvalidSlot(cNKMPacket_FRIEND_ACCEPT_ACK.friendCode))
		{
			SetCountUI();
		}
	}

	public void OnRecv(NKMPacket_FRIEND_CANCEL_REQUEST_ACK cNKMPacket_FRIEND_ADD_CANCEL_ACK)
	{
		if (IsSentREQListShowing() && m_NKCUIFriendSlotMgr.InvalidSlot(cNKMPacket_FRIEND_ADD_CANCEL_ACK.friendCode))
		{
			SetCountUI();
		}
	}

	public NKCUIFriendSlot.FRIEND_SLOT_TYPE GetCurrentSlotType()
	{
		if (IsFriendListShowing())
		{
			return NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_LIST;
		}
		if (IsBlockListShowing())
		{
			return NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_BLOCK_LIST;
		}
		if (IsReceivedREQListShowing())
		{
			return NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_RECEIVE_REQ;
		}
		if (IsSentREQListShowing())
		{
			return NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_SENT_REQ;
		}
		if (IsSearchShowing())
		{
			return NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_SEARCH;
		}
		return NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_LIST;
	}

	public void OnRecv(NKMPacket_FRIEND_LIST_ACK cNKMPacket_FRIEND_LIST_ACK)
	{
		if (cNKMPacket_FRIEND_LIST_ACK.friendListType == NKM_FRIEND_LIST_TYPE.FRIEND)
		{
			if (IsFriendListShowing())
			{
				m_NKCUIFriendSlotMgr.Set(cNKMPacket_FRIEND_LIST_ACK.friendListType, cNKMPacket_FRIEND_LIST_ACK.list);
				m_lbEmptyMessage.text = NKCUtilString.GET_STRING_FRIEND_LIST_IS_EMPTY;
				SetCountUI();
			}
		}
		else if (cNKMPacket_FRIEND_LIST_ACK.friendListType == NKM_FRIEND_LIST_TYPE.BLOCKER)
		{
			if (IsBlockListShowing())
			{
				m_NKCUIFriendSlotMgr.Set(cNKMPacket_FRIEND_LIST_ACK.friendListType, cNKMPacket_FRIEND_LIST_ACK.list);
				m_lbEmptyMessage.text = NKCUtilString.GET_STRING_FRIEND_LIST_BLOCK_IS_EMPTY;
				SetCountUI();
			}
		}
		else if (cNKMPacket_FRIEND_LIST_ACK.friendListType == NKM_FRIEND_LIST_TYPE.RECEIVE_REQUEST)
		{
			if (IsReceivedREQListShowing())
			{
				m_NKCUIFriendSlotMgr.Set(cNKMPacket_FRIEND_LIST_ACK.friendListType, cNKMPacket_FRIEND_LIST_ACK.list);
				m_lbEmptyMessage.text = NKCUtilString.GET_STRING_FRIEND_LIST_RECV_IS_EMPTY;
				SetCountUI();
			}
		}
		else if (cNKMPacket_FRIEND_LIST_ACK.friendListType == NKM_FRIEND_LIST_TYPE.SEND_REQUEST && IsSentREQListShowing())
		{
			m_NKCUIFriendSlotMgr.Set(cNKMPacket_FRIEND_LIST_ACK.friendListType, cNKMPacket_FRIEND_LIST_ACK.list);
			m_lbEmptyMessage.text = NKCUtilString.GET_STRING_FRIEND_LIST_REQ_IS_EMPTY;
			SetCountUI();
		}
		NKCUtil.SetGameobjectActive(m_objEmpty, cNKMPacket_FRIEND_LIST_ACK.list.Count == 0);
	}

	public void SetCountUI()
	{
		NKCUtil.SetGameobjectActive(m_objEmpty, m_NKCUIFriendSlotMgr.GetActiveSlotCount() == 0);
		if (IsFriendListShowing())
		{
			m_NKM_UI_FRIEND_TOP_COUNT_TEXT.text = string.Format(NKCUtilString.GET_STRING_FRIEND_COUNT_TWO_PARAM, m_NKCUIFriendSlotMgr.GetActiveSlotCount(), 60);
		}
		else if (IsBlockListShowing())
		{
			m_NKM_UI_FRIEND_TOP_COUNT_TEXT.text = string.Format(NKCUtilString.GET_STRING_FRIEND_BLOCK_COUNT_TWO_PARAM, m_NKCUIFriendSlotMgr.GetActiveSlotCount(), 30);
		}
		else if (IsReceivedREQListShowing())
		{
			m_NKM_UI_FRIEND_TOP_COUNT_TEXT.text = string.Format(NKCUtilString.GET_STRING_FRIEND_RECV_COUNT_TWO_PARAM, m_NKCUIFriendSlotMgr.GetActiveSlotCount(), 50);
		}
		else if (IsSentREQListShowing())
		{
			m_NKM_UI_FRIEND_TOP_COUNT_TEXT.text = string.Format(NKCUtilString.GET_STRING_FRIEND_REQ_COUNT_TWO_PARAM, m_NKCUIFriendSlotMgr.GetActiveSlotCount(), 50);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
		}
	}

	public void OnClickOpenSortMenu()
	{
		if (m_NKM_UI_FRIEND_TOP_SORTING_MENU.activeSelf)
		{
			CloseSortMenu();
			return;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SORTING_MENU_01, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SORTING_MENU_02, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SORTING_MENU, bValue: true);
	}

	public void CloseSortMenu(bool bAnimate = true)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SORTING_MENU_01, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SORTING_MENU_02, bValue: false);
		OnFinishSortMenuCloseAni();
	}

	private void OnFinishSortMenuCloseAni()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SORTING_MENU, bValue: false);
	}

	public void OnClickLastLoginSort()
	{
		CloseSortMenu();
		m_NKM_UI_FRIEND_TOP_SORTING_TEXT.text = NKCUtilString.GET_STRING_FRIEND_LAST_CONNECT;
		m_NKCUIFriendSlotMgr.SetFriendSortType(FRIEND_SORT_TYPE.FST_LAST_LOGIN);
	}

	public void OnClickUserLevelSort()
	{
		CloseSortMenu();
		m_NKM_UI_FRIEND_TOP_SORTING_TEXT.text = NKCUtilString.GET_STRING_FRIEND_LEVEL;
		m_NKCUIFriendSlotMgr.SetFriendSortType(FRIEND_SORT_TYPE.FST_LEVEL);
	}

	private bool IsSearchShowing()
	{
		if (m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_SEARCH.m_bChecked && m_NKM_UI_FRIEND_TOP_SUBMENU_ADD.activeSelf)
		{
			return true;
		}
		return false;
	}

	private bool IsFriendListShowing()
	{
		if (m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT_LIST.m_bChecked && m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT.activeSelf)
		{
			return true;
		}
		return false;
	}

	private bool IsBlockListShowing()
	{
		if (m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT_BLOCK.m_bChecked && m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT.activeSelf)
		{
			return true;
		}
		return false;
	}

	private bool IsReceivedREQListShowing()
	{
		if (m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_RECEIVE.m_bChecked && m_NKM_UI_FRIEND_TOP_SUBMENU_ADD.activeSelf)
		{
			return true;
		}
		return false;
	}

	private bool IsSentREQListShowing()
	{
		if (m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_SEND.m_bChecked && m_NKM_UI_FRIEND_TOP_SUBMENU_ADD.activeSelf)
		{
			return true;
		}
		return false;
	}

	public void Open(FRIEND_TOP_MENU_TYPE menuType)
	{
		m_FRIEND_TOP_MENU_TYPE = menuType;
		CloseSortMenu();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST, bValue: true);
		if (m_FRIEND_TOP_MENU_TYPE == FRIEND_TOP_MENU_TYPE.FTMT_MANAGE)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_COUNT, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SEARCH, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_ADD, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH, bValue: false);
			NKCUtil.SetGameobjectActive(m_MENTORING_INVITE_REWARD_TIME, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_MENTORING, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_LIST_INFO, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_ScrollView, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORIG_LIST_ScrollView, bValue: false);
			NKCUtil.SetGameobjectActive(m_MENTORING_COUNT, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SORT, bValue: true);
			NKCUtil.SetGameobjectActive(m_m_NKM_UI_FRIEND_MENTORING_INVITE_ILLUST_VIEW, bValue: false);
			if (m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT_LIST.m_bChecked)
			{
				OnClickFriendList();
			}
			else if (m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT_BLOCK.m_bChecked)
			{
				OnClickBlockList();
			}
		}
		else
		{
			if (m_FRIEND_TOP_MENU_TYPE != FRIEND_TOP_MENU_TYPE.FTMT_REGISTER)
			{
				return;
			}
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_MANAGEMENT, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_ADD, bValue: true);
			NKCUtil.SetGameobjectActive(m_MENTORING_INVITE_REWARD_TIME, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_MENTORING, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_LIST_INFO, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_ScrollView, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORIG_LIST_ScrollView, bValue: false);
			NKCUtil.SetGameobjectActive(m_MENTORING_COUNT, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SORT, bValue: true);
			NKCUtil.SetGameobjectActive(m_m_NKM_UI_FRIEND_MENTORING_INVITE_ILLUST_VIEW, bValue: false);
			if (m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_RECEIVE_NEW.activeSelf)
			{
				m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_RECEIVE.Select(bSelect: false);
				m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_RECEIVE.Select(bSelect: true);
				return;
			}
			m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_SEARCH.OnValueChanged.AddListener(OnClickFriendSearch);
			m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_SEND.OnValueChanged.AddListener(OnClickSentReq);
			m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_RECEIVE.OnValueChanged.AddListener(OnClickReceivedReq);
			if (m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_SEARCH.m_bChecked)
			{
				OnClickFriendSearch();
			}
			else if (m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_RECEIVE.m_bChecked)
			{
				OnClickReceivedReq();
			}
			else if (m_NKM_UI_FRIEND_TOP_SUBMENU_ADD_SEND.m_bChecked)
			{
				OnClickSentReq();
			}
		}
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST, bValue: false);
	}

	public void CloseInstance()
	{
		m_NKCUIFriendSlotMgr?.CloseSlotInstance();
	}

	public void OnClickFriendList(bool bCheck = true)
	{
		if (bCheck)
		{
			m_NKM_UI_FRIEND_TOP_COUNT_TEXT.text = "";
			m_NKCUIFriendSlotMgr.Clear();
			NKMPacket_FRIEND_LIST_REQ nKMPacket_FRIEND_LIST_REQ = new NKMPacket_FRIEND_LIST_REQ();
			nKMPacket_FRIEND_LIST_REQ.friendListType = NKM_FRIEND_LIST_TYPE.FRIEND;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}

	public void OnClickBlockList(bool bCheck = true)
	{
		if (bCheck)
		{
			m_NKM_UI_FRIEND_TOP_COUNT_TEXT.text = "";
			m_NKCUIFriendSlotMgr.Clear();
			NKMPacket_FRIEND_LIST_REQ nKMPacket_FRIEND_LIST_REQ = new NKMPacket_FRIEND_LIST_REQ();
			nKMPacket_FRIEND_LIST_REQ.friendListType = NKM_FRIEND_LIST_TYPE.BLOCKER;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}

	public void OnClickFriendSearch(bool bCheck = true)
	{
		if (bCheck)
		{
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_COUNT, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SEARCH, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH, bValue: true);
			m_NKCUIFriendSlotMgr.Clear();
			NKMPacket_FRIEND_RECOMMEND_REQ packet = new NKMPacket_FRIEND_RECOMMEND_REQ();
			NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}

	public void OnClickReceivedReq(bool bCheck = true)
	{
		if (bCheck)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_COUNT, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SEARCH, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH, bValue: false);
			m_NKM_UI_FRIEND_TOP_COUNT_TEXT.text = "";
			m_NKCUIFriendSlotMgr.Clear();
			NKMPacket_FRIEND_LIST_REQ nKMPacket_FRIEND_LIST_REQ = new NKMPacket_FRIEND_LIST_REQ();
			nKMPacket_FRIEND_LIST_REQ.friendListType = NKM_FRIEND_LIST_TYPE.RECEIVE_REQUEST;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}

	public void OnClickSentReq(bool bCheck = true)
	{
		if (bCheck)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_COUNT, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SEARCH, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH, bValue: false);
			m_NKM_UI_FRIEND_TOP_COUNT_TEXT.text = "";
			m_NKCUIFriendSlotMgr.Clear();
			NKMPacket_FRIEND_LIST_REQ nKMPacket_FRIEND_LIST_REQ = new NKMPacket_FRIEND_LIST_REQ();
			nKMPacket_FRIEND_LIST_REQ.friendListType = NKM_FRIEND_LIST_TYPE.SEND_REQUEST;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_LIST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}
}
