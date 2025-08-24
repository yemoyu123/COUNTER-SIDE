using System;
using System.Collections.Generic;
using ClientPacket.Guild;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildJoin : NKCUIBase
{
	public enum GuildJoinUIType
	{
		None,
		Search,
		Requested,
		Invited
	}

	private const string BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string ASSET_NAME = "NKM_UI_CONSORTIUM_JOIN";

	private static NKCUIGuildJoin m_Instance;

	[Header("탭")]
	public NKCUIComToggle m_tglSearchtab;

	public NKCUIComToggle m_tglRequestedtab;

	public NKCUIComToggle m_tglInvitedtab;

	public Text m_lbRequestedCount;

	[Header("검색")]
	public GameObject m_objSearch;

	public InputField m_IFSearch;

	public NKCUIComStateButton m_btnSearch;

	public NKCUIComStateButton m_btnRefresh;

	[Header("리스트")]
	public LoopScrollRect m_loopList;

	public Transform m_trContentParent;

	public float m_SearchInterval = 3f;

	private Stack<NKCUIGuildListSlot> m_stkSlot = new Stack<NKCUIGuildListSlot>();

	private List<NKCUIGuildListSlot> m_lstSlot = new List<NKCUIGuildListSlot>();

	private GuildJoinUIType m_CurrentUIType;

	private Dictionary<GuildJoinUIType, DateTime> m_LastRequestedTime = new Dictionary<GuildJoinUIType, DateTime>();

	private DateTime m_tLastSearchTime = DateTime.MinValue;

	private float m_fDeltaTime;

	public static NKCUIGuildJoin Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIGuildJoin>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_JOIN", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), CleanupInstance).GetInstance<NKCUIGuildJoin>();
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

	public override string MenuName => NKCUtilString.GET_STRING_CONSORTIUM_INTRO_JOIN_TEXT;

	public override eMenutype eUIType => eMenutype.FullScreen;

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
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void InitUI()
	{
		m_tglSearchtab.OnValueChanged.RemoveAllListeners();
		m_tglSearchtab.OnValueChanged.AddListener(OnSearchTab);
		m_tglRequestedtab.OnValueChanged.RemoveAllListeners();
		m_tglRequestedtab.OnValueChanged.AddListener(OnRequestedTab);
		m_tglInvitedtab.OnValueChanged.RemoveAllListeners();
		m_tglInvitedtab.OnValueChanged.AddListener(OnInvitedTab);
		m_btnRefresh.PointerClick.RemoveAllListeners();
		m_btnRefresh.PointerClick.AddListener(OnRefreshBtn);
		m_btnSearch.PointerClick.RemoveAllListeners();
		m_btnSearch.PointerClick.AddListener(OnSearchBtn);
		m_loopList.dOnGetObject += GetObject;
		m_loopList.dOnReturnObject += ReturnObject;
		m_loopList.dOnProvideData += ProvideData;
		m_loopList.PrepareCells();
		m_IFSearch.onEndEdit.RemoveAllListeners();
		m_IFSearch.onEndEdit.AddListener(OnEndEdit);
	}

	private RectTransform GetObject(int index)
	{
		NKCUIGuildListSlot nKCUIGuildListSlot = null;
		nKCUIGuildListSlot = ((m_stkSlot.Count <= 0) ? NKCUIGuildListSlot.GetNewInstance(m_trContentParent, OnSelectedSlot) : m_stkSlot.Pop());
		NKCUtil.SetGameobjectActive(nKCUIGuildListSlot, bValue: false);
		return nKCUIGuildListSlot?.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIGuildListSlot component = tr.GetComponent<NKCUIGuildListSlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		tr.SetParent(base.transform);
		m_lstSlot.Remove(component);
		m_stkSlot.Push(component);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIGuildListSlot component = tr.GetComponent<NKCUIGuildListSlot>();
		switch (m_CurrentUIType)
		{
		case GuildJoinUIType.Search:
			if (NKCGuildManager.m_lstSearchData.Count >= idx)
			{
				component.SetData(NKCGuildManager.m_lstSearchData[idx], m_CurrentUIType);
			}
			break;
		case GuildJoinUIType.Requested:
			if (NKCGuildManager.m_lstRequestedData.Count >= idx)
			{
				component.SetData(NKCGuildManager.m_lstRequestedData[idx], m_CurrentUIType);
			}
			break;
		case GuildJoinUIType.Invited:
			if (NKCGuildManager.m_lstInvitedData.Count >= idx)
			{
				component.SetData(NKCGuildManager.m_lstInvitedData[idx], m_CurrentUIType);
			}
			break;
		}
	}

	public void Open()
	{
		m_CurrentUIType = GuildJoinUIType.Search;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		ResetUI();
		NKCPacketSender.Send_NKMPacket_GUILD_SEARCH_REQ("");
		UIOpened();
	}

	private void ResetUI()
	{
		m_IFSearch.text = string.Empty;
		m_btnSearch.UnLock();
		m_btnRefresh.UnLock();
		m_LastRequestedTime = new Dictionary<GuildJoinUIType, DateTime>();
		m_loopList.TotalCount = 0;
		m_loopList.RefreshCells();
		m_loopList.SetIndexPosition(0);
		m_tglSearchtab.Select(bSelect: true, bForce: true, bImmediate: true);
	}

	private void Update()
	{
		m_fDeltaTime += Time.deltaTime;
		if (m_fDeltaTime > m_SearchInterval)
		{
			if (m_btnSearch.m_bLock)
			{
				m_btnSearch.UnLock();
			}
			if (m_btnRefresh.m_bLock)
			{
				m_btnRefresh.UnLock();
			}
		}
	}

	private void OnSearchTab(bool bValue)
	{
		if (bValue && m_CurrentUIType != GuildJoinUIType.Search)
		{
			m_CurrentUIType = GuildJoinUIType.Search;
			NKCUtil.SetGameobjectActive(m_objSearch, bValue: true);
			if (m_LastRequestedTime.ContainsKey(m_CurrentUIType) && NKCSynchronizedTime.GetServerUTCTime() - m_LastRequestedTime[m_CurrentUIType] < TimeSpan.FromSeconds(3.0))
			{
				RefreshUI();
			}
			else
			{
				OnSearchBtn();
			}
		}
	}

	private void OnRequestedTab(bool bValue)
	{
		if (bValue && m_CurrentUIType != GuildJoinUIType.Requested)
		{
			m_CurrentUIType = GuildJoinUIType.Requested;
			NKCUtil.SetGameobjectActive(m_objSearch, bValue: false);
			if (m_LastRequestedTime.ContainsKey(m_CurrentUIType) && NKCSynchronizedTime.GetServerUTCTime() - m_LastRequestedTime[m_CurrentUIType] < TimeSpan.FromSeconds(3.0))
			{
				RefreshUI();
				return;
			}
			SaveLastRequestedTime(GuildJoinUIType.Requested);
			NKCGuildManager.Send_GUILD_LIST_REQ(GuildListType.SendRequest);
		}
	}

	private void OnInvitedTab(bool bValue)
	{
		if (bValue && m_CurrentUIType != GuildJoinUIType.Invited)
		{
			m_CurrentUIType = GuildJoinUIType.Invited;
			NKCUtil.SetGameobjectActive(m_objSearch, bValue: false);
			if (m_LastRequestedTime.ContainsKey(m_CurrentUIType) && NKCSynchronizedTime.GetServerUTCTime() - m_LastRequestedTime[m_CurrentUIType] < TimeSpan.FromSeconds(3.0))
			{
				RefreshUI();
				return;
			}
			SaveLastRequestedTime(GuildJoinUIType.Invited);
			NKCGuildManager.Send_GUILD_LIST_REQ(GuildListType.ReceiveInvite);
		}
	}

	public void RefreshUI()
	{
		switch (m_CurrentUIType)
		{
		case GuildJoinUIType.Search:
			m_loopList.TotalCount = NKCGuildManager.m_lstSearchData.Count;
			break;
		case GuildJoinUIType.Requested:
			m_loopList.TotalCount = NKCGuildManager.m_lstRequestedData.Count;
			break;
		case GuildJoinUIType.Invited:
			m_loopList.TotalCount = NKCGuildManager.m_lstInvitedData.Count;
			break;
		}
		NKCUtil.SetLabelText(m_lbRequestedCount, $"{NKCUtilString.GET_STRING_CONSORTIUM_JOIN_CONFIRM_SITUATION} {NKCGuildManager.m_lstRequestedData.Count}/{NKMCommonConst.Guild.MaxJoinRequestCount}");
		m_loopList.RefreshCells();
		m_loopList.SetIndexPosition(0);
	}

	private void SaveLastRequestedTime(GuildJoinUIType joinUIType)
	{
		if (!m_LastRequestedTime.ContainsKey(joinUIType))
		{
			m_LastRequestedTime.Add(joinUIType, NKCSynchronizedTime.GetServerUTCTime());
		}
		else
		{
			m_LastRequestedTime[joinUIType] = NKCSynchronizedTime.GetServerUTCTime();
		}
	}

	private void OnSearchBtn()
	{
		m_btnSearch.Lock();
		m_btnRefresh.Lock();
		m_fDeltaTime = 0f;
		SaveLastRequestedTime(GuildJoinUIType.Search);
		NKCPacketSender.Send_NKMPacket_GUILD_SEARCH_REQ(m_IFSearch.text);
	}

	private void OnRefreshBtn()
	{
		m_IFSearch.text = string.Empty;
		OnSearchBtn();
	}

	private void OnSelectedSlot(GuildListData guildData)
	{
		NKCPacketSender.Send_NKMPacket_GUILD_DATA_REQ(guildData.guildUid);
	}

	public override void OnGuildDataChanged()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
	}

	private void OnEndEdit(string input)
	{
		if (NKCInputManager.IsChatSubmitEnter())
		{
			if (!m_btnSearch.m_bLock)
			{
				OnSearchBtn();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
