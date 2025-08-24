using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Community;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildInvite : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_POPUP_INVITE";

	private static NKCPopupGuildInvite m_Instance;

	public NKCUIComStateButton m_btnClose;

	public Text m_lbInviteWaitingCount;

	public InputField m_IFSearch;

	public NKCUIComStateButton m_btnSearch;

	public LoopScrollRect m_loop;

	public Transform m_trContentParent;

	public GameObject m_objNone;

	private Stack<NKCUIGuildMemberSlot> m_stk = new Stack<NKCUIGuildMemberSlot>();

	private List<NKCUIGuildMemberSlot> m_lstVisible = new List<NKCUIGuildMemberSlot>();

	private List<FriendListData> m_lstUserData = new List<FriendListData>();

	public static NKCPopupGuildInvite Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildInvite>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_POPUP_INVITE", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildInvite>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
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

	public override string MenuName => "";

	public override eMenutype eUIType => eMenutype.Popup;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		m_IFSearch.text = "";
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnSearch.PointerClick.RemoveAllListeners();
		m_btnSearch.PointerClick.AddListener(OnClickSearch);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		m_IFSearch.onEndEdit.RemoveAllListeners();
		m_IFSearch.onEndEdit.AddListener(OnEndEdit);
	}

	private RectTransform GetObject(int index)
	{
		NKCUIGuildMemberSlot nKCUIGuildMemberSlot = null;
		nKCUIGuildMemberSlot = ((m_stk.Count <= 0) ? NKCUIGuildMemberSlot.GetNewInstance(m_trContentParent, OnSelectedSlot) : m_stk.Pop());
		m_lstVisible.Add(nKCUIGuildMemberSlot);
		return nKCUIGuildMemberSlot?.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIGuildMemberSlot component = tr.GetComponent<NKCUIGuildMemberSlot>();
		m_lstVisible.Remove(component);
		m_stk.Push(component);
		NKCUtil.SetGameobjectActive(component, bValue: false);
		tr.SetParent(base.transform);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIGuildMemberSlot component = tr.GetComponent<NKCUIGuildMemberSlot>();
		if (m_lstUserData.Count < idx)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
		else
		{
			component.SetData(m_lstUserData[idx], NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Invite);
		}
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_IFSearch.text = "";
		OnClickSearch();
		RefreshUI();
		UIOpened();
	}

	private void SetInvitedCount()
	{
		NKCUtil.SetLabelText(m_lbInviteWaitingCount, $"{NKCGuildManager.MyGuildData.inviteList.Count}/{NKMCommonConst.Guild.MaxInviteCount}");
		if (NKCGuildManager.MyGuildData.inviteList.Count == NKMCommonConst.Guild.MaxInviteCount)
		{
			NKCUtil.SetLabelTextColor(m_lbInviteWaitingCount, Color.red);
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbInviteWaitingCount, Color.white);
		}
	}

	private void RefreshUI()
	{
		SetInvitedCount();
		m_loop.TotalCount = m_lstUserData.Count;
		m_loop.RefreshCells();
		NKCUtil.SetGameobjectActive(m_objNone, m_loop.TotalCount == 0);
	}

	private void OnClickSearch()
	{
		if (!string.IsNullOrWhiteSpace(m_IFSearch.text))
		{
			NKMPacket_FRIEND_SEARCH_REQ nKMPacket_FRIEND_SEARCH_REQ = new NKMPacket_FRIEND_SEARCH_REQ();
			nKMPacket_FRIEND_SEARCH_REQ.searchKeyword = m_IFSearch.text;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_SEARCH_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_GUILD_RECOMMEND_INVITE_LIST_REQ(NKCGuildManager.MyData.guildUid);
		}
	}

	public void OnRecv(List<FriendListData> list)
	{
		m_lstUserData = list;
		RefreshUI();
		if (m_loop.TotalCount > 0)
		{
			m_loop.SetIndexPosition(0);
		}
	}

	private void OnSelectedSlot(long userUid)
	{
		NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(userUid, NKM_DECK_TYPE.NDT_NORMAL);
	}

	public override void OnGuildDataChanged()
	{
		RefreshUI();
	}

	private void OnEndEdit(string input)
	{
		if (NKCInputManager.IsChatSubmitEnter())
		{
			if (!m_btnSearch.m_bLock)
			{
				OnClickSearch();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
