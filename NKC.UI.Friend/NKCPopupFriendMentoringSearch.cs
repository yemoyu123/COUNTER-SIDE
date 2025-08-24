using System.Collections.Generic;
using ClientPacket.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Friend;

public class NKCPopupFriendMentoringSearch : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_friend";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FRIEND_MENTORING_SEARCH";

	private static NKCPopupFriendMentoringSearch m_Instance;

	public EventTrigger m_NKM_UI_POPUP_FRIEND_MENTORING_SEARCH_BG;

	public InputField m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT;

	public NKCUIComButton m_NKM_UI_FRIEND_TOP_SEARCH_BUTTON;

	public NKCUIComButton m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH;

	public LoopScrollRect m_MENTORING_INVITE_LIST_ScrollRect;

	public NKCUIComStateButton m_NKM_UI_POPUP_CLOSE_BUTTON;

	private List<FriendListData> m_lstInvited;

	private List<FriendListData> m_lstMentor;

	public Transform m_parentMentoringSlot;

	private List<NKCUIMentoringSlot> m_slotList = new List<NKCUIMentoringSlot>();

	private Stack<RectTransform> m_slotPool = new Stack<RectTransform>();

	public static NKCPopupFriendMentoringSearch Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFriendMentoringSearch>("ab_ui_nkm_ui_friend", "NKM_UI_POPUP_FRIEND_MENTORING_SEARCH", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupFriendMentoringSearch>();
				m_Instance.Init();
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

	public override string MenuName => "NKCPopupFriendMentoringSearch";

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

	public void Init()
	{
		if (m_NKM_UI_POPUP_FRIEND_MENTORING_SEARCH_BG != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_NKM_UI_POPUP_FRIEND_MENTORING_SEARCH_BG.triggers.Add(entry);
		}
		if (m_NKM_UI_FRIEND_TOP_SEARCH_BUTTON != null)
		{
			m_NKM_UI_FRIEND_TOP_SEARCH_BUTTON.PointerClick.RemoveAllListeners();
			m_NKM_UI_FRIEND_TOP_SEARCH_BUTTON.PointerClick.AddListener(OnClickSearch);
		}
		if (m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH != null)
		{
			m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH.PointerClick.RemoveAllListeners();
			m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH.PointerClick.AddListener(OnClickMentorListRefresh);
		}
		if (m_NKM_UI_POPUP_CLOSE_BUTTON != null)
		{
			m_NKM_UI_POPUP_CLOSE_BUTTON.PointerClick.RemoveAllListeners();
			m_NKM_UI_POPUP_CLOSE_BUTTON.PointerClick.AddListener(CheckInstanceAndClose);
		}
		if (m_MENTORING_INVITE_LIST_ScrollRect != null)
		{
			m_MENTORING_INVITE_LIST_ScrollRect.dOnGetObject += GetMentorSlot;
			m_MENTORING_INVITE_LIST_ScrollRect.dOnReturnObject += ReturnMentorSlot;
			m_MENTORING_INVITE_LIST_ScrollRect.dOnProvideData += ProvideData;
			m_MENTORING_INVITE_LIST_ScrollRect.PrepareCells();
		}
		if (m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT != null)
		{
			m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT.onEndEdit.RemoveAllListeners();
			m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT.onEndEdit.AddListener(OnEndEditSearch);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickSearch()
	{
		if (m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT != null)
		{
			NKCPacketSender.Send_NKMPacket_MENTORING_SEARCH_LIST_REQ(MentoringIdentity.Mentee, m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT.text);
		}
	}

	private void OnClickMentorListRefresh()
	{
		NKCPacketSender.Send_kNKMPacket_MENTORING_RECEIVE_LIST_REQ(MentoringIdentity.Mentee, bForce: true);
	}

	public void Open()
	{
		UpdateData();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
	}

	public void ResetUI()
	{
		UpdateData();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	private void UpdateData()
	{
		m_lstMentor = NKCScenManager.CurrentUserData().MentoringData.lstRecommend;
		m_lstInvited = NKCScenManager.CurrentUserData().MentoringData.lstInvited;
		if (m_lstInvited != null && m_lstMentor != null)
		{
			int i = 0;
			while (i < m_lstMentor.Count)
			{
				int num;
				if (m_lstMentor[i] != null && m_lstInvited.Find((FriendListData e) => e.commonProfile.userUid == m_lstMentor[i].commonProfile.userUid) != null)
				{
					m_lstMentor.Remove(m_lstMentor[i]);
					num = i - 1;
					i = num;
				}
				num = i + 1;
				i = num;
			}
		}
		if (m_lstMentor != null)
		{
			int num2 = m_lstMentor.Count;
			if (m_lstInvited != null)
			{
				num2 += m_lstInvited.Count;
			}
			m_MENTORING_INVITE_LIST_ScrollRect.TotalCount = num2;
			m_MENTORING_INVITE_LIST_ScrollRect.SetIndexPosition(0);
		}
	}

	private void OnEndEditSearch(string input)
	{
		if (NKCInputManager.IsChatSubmitEnter())
		{
			if (!m_NKM_UI_FRIEND_TOP_SEARCH_BUTTON.m_bLock)
			{
				OnClickSearch();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	private RectTransform GetMentorSlot(int index)
	{
		if (m_slotPool.Count > 0)
		{
			RectTransform rectTransform = m_slotPool.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		NKCUIMentoringSlot newInstance = NKCUIMentoringSlot.GetNewInstance(m_parentMentoringSlot);
		if (newInstance == null)
		{
			return null;
		}
		newInstance.transform.localScale = Vector3.one;
		m_slotList.Add(newInstance);
		return newInstance.GetComponent<RectTransform>();
	}

	public void ReturnMentorSlot(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
		m_slotPool.Push(tr.GetComponent<RectTransform>());
	}

	public void ProvideData(Transform tr, int index)
	{
		FriendListData friendListData = null;
		bool invited = false;
		if (m_lstInvited != null)
		{
			if (m_lstInvited.Count > 0 && m_lstInvited.Count > index)
			{
				friendListData = m_lstInvited[index];
				invited = true;
			}
			else
			{
				friendListData = m_lstMentor[index - m_lstInvited.Count];
			}
		}
		else
		{
			friendListData = m_lstMentor[index];
		}
		NKCUIMentoringSlot component = tr.GetComponent<NKCUIMentoringSlot>();
		if (component != null)
		{
			component.SetData(friendListData.commonProfile, friendListData.lastLoginDate.Ticks, invited);
		}
	}
}
