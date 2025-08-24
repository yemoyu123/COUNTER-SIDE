using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.LeaderBoard;
using Cs.Core.Util;
using Cs.Logging;
using NKC.UI.Fierce;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILeaderBoard : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_LEADER_BOARD";

	private const string UI_ASSET_NAME = "NKM_UI_LEADER_BOARD";

	private static NKCUILeaderBoard m_Instance;

	[Header("좌측 탭 - 풀스크린UI전용")]
	public NKCUILeaderBoardTab m_pfbTab;

	public NKCUIComToggleGroup m_tabTglGroup;

	public ScrollRect m_ScrollTab;

	public Transform m_trTabParent;

	[Header("서브탭")]
	public NKCUILeaderBoardSubTab m_pfbSubTab;

	public GameObject m_objSubTabParent;

	public NKCUIComToggleGroup m_subTabTglGroup;

	public LoopScrollRect m_LoopScrollSubTab;

	public Transform m_trSubTabSlotParent;

	[Header("컨텐츠")]
	public Text m_lbRemainTime;

	public NKCUILeaderBoardSlotAll m_pfbSlot;

	public RectTransform m_rtContent;

	public Text m_lbTitle;

	public Text m_lbDesc;

	public Image m_imgBanner;

	public NKCUIComStateButton m_btnSeasonSelect;

	public LoopVerticalScrollFlexibleRect m_LoopScrollContent;

	public Transform m_trContentSlotParent;

	public GameObject m_objNone;

	[Header("보상확인")]
	public NKCUIComStateButton m_btnRewardInfo;

	[Header("내 랭킹")]
	public NKCUILeaderBoardSlot m_slotMyRank;

	private Dictionary<LeaderBoardType, List<NKMLeaderBoardTemplet>> m_dicTabTemplet = new Dictionary<LeaderBoardType, List<NKMLeaderBoardTemplet>>();

	private Dictionary<LeaderBoardType, NKCUILeaderBoardTab> m_dicBoardTab = new Dictionary<LeaderBoardType, NKCUILeaderBoardTab>();

	private List<NKCUILeaderBoardSubTab> m_lstVisibleSubTab = new List<NKCUILeaderBoardSubTab>();

	private Stack<NKCUILeaderBoardSubTab> m_stkSubTabPool = new Stack<NKCUILeaderBoardSubTab>();

	private List<NKCUILeaderBoardSlotAll> m_lstVisibleSlot = new List<NKCUILeaderBoardSlotAll>();

	private Stack<NKCUILeaderBoardSlotAll> m_stkSlotPool = new Stack<NKCUILeaderBoardSlotAll>();

	private LeaderBoardType m_CurrentBoardType = LeaderBoardType.BT_NONE;

	private NKMLeaderBoardTemplet m_cNKMLeaderBoardTemplet;

	private DateTime m_tNextResetTime = DateTime.MinValue;

	private float m_fDeltaTime;

	public static NKCUILeaderBoard Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUILeaderBoard>("AB_UI_NKM_UI_LEADER_BOARD", "NKM_UI_LEADER_BOARD", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUILeaderBoard>();
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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCStringTable.GetString("SI_DP_LEADERBOARD");

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
		m_dicTabTemplet.Clear();
		m_CurrentBoardType = LeaderBoardType.BT_NONE;
		m_cNKMLeaderBoardTemplet = null;
	}

	public override void Hide()
	{
		m_CurrentBoardType = LeaderBoardType.BT_NONE;
		base.Hide();
	}

	public override void UnHide()
	{
		base.UnHide();
		Open(m_cNKMLeaderBoardTemplet, bFirstOpen: false);
	}

	protected void Init()
	{
		if (m_LoopScrollSubTab != null)
		{
			m_LoopScrollSubTab.dOnGetObject += GetSubTabObject;
			m_LoopScrollSubTab.dOnReturnObject += ReturnSubTabObject;
			m_LoopScrollSubTab.dOnProvideData += ProvideSubTabData;
			m_LoopScrollSubTab.PrepareCells();
		}
		if (m_LoopScrollContent != null)
		{
			m_LoopScrollContent.dOnGetObject += GetContentObject;
			m_LoopScrollContent.dOnReturnObject += ReturnContentObject;
			m_LoopScrollContent.dOnProvideData += ProvideContentData;
			m_LoopScrollContent.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScrollContent);
		}
		if (m_btnRewardInfo != null)
		{
			m_btnRewardInfo.PointerClick.RemoveAllListeners();
			m_btnRewardInfo.PointerClick.AddListener(OnClickRewardInfo);
		}
		if (m_btnSeasonSelect != null)
		{
			m_btnSeasonSelect.PointerClick.RemoveAllListeners();
			m_btnSeasonSelect.PointerClick.AddListener(OnClickSeasonSelect);
		}
		if (m_slotMyRank != null)
		{
			m_slotMyRank.InitUI();
		}
	}

	public RectTransform GetSubTabObject(int index)
	{
		if (m_stkSubTabPool.Count > 0)
		{
			NKCUILeaderBoardSubTab nKCUILeaderBoardSubTab = m_stkSubTabPool.Pop();
			nKCUILeaderBoardSubTab.transform.SetParent(m_trSubTabSlotParent);
			m_lstVisibleSubTab.Add(nKCUILeaderBoardSubTab);
			return nKCUILeaderBoardSubTab.GetComponent<RectTransform>();
		}
		NKCUILeaderBoardSubTab nKCUILeaderBoardSubTab2 = UnityEngine.Object.Instantiate(m_pfbSubTab);
		nKCUILeaderBoardSubTab2.transform.SetParent(m_trSubTabSlotParent);
		return nKCUILeaderBoardSubTab2.GetComponent<RectTransform>();
	}

	public void ReturnSubTabObject(Transform tr)
	{
		NKCUILeaderBoardSubTab component = tr.GetComponent<NKCUILeaderBoardSubTab>();
		if (component != null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			tr.SetParent(base.gameObject.transform);
			m_lstVisibleSubTab.Remove(component);
			m_stkSubTabPool.Push(component);
		}
	}

	public void ProvideSubTabData(Transform tr, int idx)
	{
		if (m_dicTabTemplet[m_CurrentBoardType].Count > idx + 1)
		{
			NKCUILeaderBoardSubTab component = tr.GetComponent<NKCUILeaderBoardSubTab>();
			NKCUtil.SetGameobjectActive(component, bValue: true);
			m_lstVisibleSubTab.Add(component);
			component.SetData(m_subTabTglGroup, m_dicTabTemplet[m_CurrentBoardType][idx + 1].GetTabName(), m_dicTabTemplet[m_CurrentBoardType][idx + 1].m_BoardID, OnSelectTab);
		}
		else
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
		}
	}

	public RectTransform GetContentObject(int index)
	{
		if (m_stkSlotPool.Count > 0)
		{
			NKCUILeaderBoardSlotAll nKCUILeaderBoardSlotAll = m_stkSlotPool.Pop();
			nKCUILeaderBoardSlotAll.transform.SetParent(m_trContentSlotParent);
			m_lstVisibleSlot.Add(nKCUILeaderBoardSlotAll);
			NKCUtil.SetGameobjectActive(nKCUILeaderBoardSlotAll, bValue: false);
			return nKCUILeaderBoardSlotAll.GetComponent<RectTransform>();
		}
		NKCUILeaderBoardSlotAll nKCUILeaderBoardSlotAll2 = UnityEngine.Object.Instantiate(m_pfbSlot);
		nKCUILeaderBoardSlotAll2.InitUI();
		nKCUILeaderBoardSlotAll2.transform.SetParent(m_trContentSlotParent);
		m_lstVisibleSlot.Add(nKCUILeaderBoardSlotAll2);
		NKCUtil.SetGameobjectActive(nKCUILeaderBoardSlotAll2, bValue: false);
		return nKCUILeaderBoardSlotAll2.GetComponent<RectTransform>();
	}

	public void ReturnContentObject(Transform tr)
	{
		NKCUILeaderBoardSlotAll component = tr.GetComponent<NKCUILeaderBoardSlotAll>();
		if (component != null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			m_lstVisibleSlot.Remove(component);
			m_stkSlotPool.Push(component);
			tr.SetParent(base.transform, worldPositionStays: false);
		}
	}

	public void ProvideContentData(Transform tr, int idx)
	{
		NKCUILeaderBoardSlotAll component = tr.GetComponent<NKCUILeaderBoardSlotAll>();
		if (component == null)
		{
			return;
		}
		switch (m_cNKMLeaderBoardTemplet.m_LayoutType)
		{
		case LayoutType.TOP_3_AND_RANKING:
		{
			List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
			list.AddRange(NKCLeaderBoardManager.GetLeaderBoardData(m_cNKMLeaderBoardTemplet.m_BoardID));
			if (idx == 0)
			{
				for (int i = list.Count; i < 3; i++)
				{
					LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(m_cNKMLeaderBoardTemplet.m_BoardTab, CheckUseGuildSlot(m_cNKMLeaderBoardTemplet.m_BoardTab), i + 1);
					list.Add(item);
				}
				component.transform.SetParent(m_trContentSlotParent);
				NKCUtil.SetGameobjectActive(component, bValue: true);
				component.GetComponent<NKCUILeaderBoardSlotAll>().SetData(list[idx], list[idx + 1], list[idx + 2], m_cNKMLeaderBoardTemplet.m_BoardCriteria, bTop3Only: false, OnEventPanelBeginDragAll);
			}
			else if (list.Count >= idx + 2)
			{
				component.transform.SetParent(m_trContentSlotParent);
				NKCUtil.SetGameobjectActive(component, bValue: true);
				component.GetComponent<NKCUILeaderBoardSlotAll>().SetData(NKCLeaderBoardManager.GetLeaderBoardData(m_cNKMLeaderBoardTemplet.m_BoardID)[idx + 2], m_cNKMLeaderBoardTemplet.m_BoardCriteria, OnEventPanelBeginDragAll);
			}
			break;
		}
		default:
			NKCUtil.SetGameobjectActive(component, bValue: false);
			break;
		case LayoutType.NORMAL:
		case LayoutType.TOP_3_ONLY:
			break;
		}
	}

	public void Open(NKMLeaderBoardTemplet reservedTemplet = null, bool bFirstOpen = true)
	{
		if (NKCScenManager.CurrentUserData().UserProfileData == null)
		{
			NKCPacketSender.Send_NKMPacket_MY_USER_PROFILE_INFO_REQ();
		}
		if (m_pfbTab != null)
		{
			SetLeaderBoardTabTemplets();
		}
		SetLeaderBoardSubTabTemplets();
		LeaderBoardType boardType = LeaderBoardType.BT_ACHIEVE;
		if (reservedTemplet != null)
		{
			boardType = reservedTemplet.m_BoardTab;
			m_cNKMLeaderBoardTemplet = null;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_LoopScrollContent != null)
		{
			m_LoopScrollContent.TotalCount = 0;
			m_LoopScrollContent.SetIndexPosition(0);
		}
		if (bFirstOpen)
		{
			UIOpened();
		}
		OnClickMainTab(boardType);
	}

	private void SetLeaderBoardTabTemplets()
	{
		foreach (KeyValuePair<LeaderBoardType, NKCUILeaderBoardTab> item in m_dicBoardTab)
		{
			UnityEngine.Object.Destroy(item.Value.gameObject);
		}
		m_dicBoardTab.Clear();
		foreach (NKMLeaderBoardTemplet value in NKMTempletContainer<NKMLeaderBoardTemplet>.Values)
		{
			if (m_dicBoardTab.ContainsKey(value.m_BoardTab) || value.m_BoardTabSubIndex != 0 || (value.m_BoardTab == LeaderBoardType.BT_GUILD && !NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_RANKING)) || (value.m_BoardTab == LeaderBoardType.BT_DEFENCE && NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Now) == null))
			{
				continue;
			}
			if (value.m_BoardTab == LeaderBoardType.BT_LEAGUE || value.m_BoardTab == LeaderBoardType.BT_UNLIMITED)
			{
				NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(ServiceTime.Now);
				if (nKMLeaguePvpRankSeasonTemplet == null || (value.m_BoardTab == LeaderBoardType.BT_LEAGUE && nKMLeaguePvpRankSeasonTemplet.GameType != NKM_GAME_TYPE.NGT_PVP_LEAGUE) || (value.m_BoardTab == LeaderBoardType.BT_UNLIMITED && nKMLeaguePvpRankSeasonTemplet.GameType != NKM_GAME_TYPE.NGT_PVP_UNLIMITED))
				{
					continue;
				}
			}
			NKCUILeaderBoardTab nKCUILeaderBoardTab = UnityEngine.Object.Instantiate(m_pfbTab, m_trTabParent);
			nKCUILeaderBoardTab.SetData(value.m_BoardTab, m_tabTglGroup, OnClickMainTab);
			m_dicBoardTab.Add(value.m_BoardTab, nKCUILeaderBoardTab);
		}
	}

	private void SetLeaderBoardSubTabTemplets()
	{
		m_dicTabTemplet.Clear();
		foreach (NKMLeaderBoardTemplet templet in NKMTempletContainer<NKMLeaderBoardTemplet>.Values)
		{
			if ((templet.m_BoardTab == LeaderBoardType.BT_GUILD && !NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_RANKING)) || (templet.m_BoardTab == LeaderBoardType.BT_DEFENCE && NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Now) == null) || (templet.m_BoardTab == LeaderBoardType.BT_LEAGUE && !NKCPVPManager.IsPvpLeagueUnlocked()))
			{
				continue;
			}
			if (m_dicTabTemplet.ContainsKey(templet.m_BoardTab))
			{
				if (m_dicTabTemplet[templet.m_BoardTab].Find((NKMLeaderBoardTemplet x) => x.m_BoardTabSubIndex == templet.m_BoardTabSubIndex) == null)
				{
					m_dicTabTemplet[templet.m_BoardTab].Add(templet);
				}
			}
			else
			{
				List<NKMLeaderBoardTemplet> list = new List<NKMLeaderBoardTemplet>();
				list.Add(templet);
				m_dicTabTemplet.Add(templet.m_BoardTab, list);
			}
		}
		foreach (List<NKMLeaderBoardTemplet> value in m_dicTabTemplet.Values)
		{
			value.Sort(CompLeaderBoard);
		}
	}

	private void SetSubTab(NKMLeaderBoardTemplet targetTemplet)
	{
		bool flag = false;
		if (m_dicTabTemplet.ContainsKey(targetTemplet.m_BoardTab))
		{
			flag = m_dicTabTemplet[targetTemplet.m_BoardTab].Count > 2;
		}
		NKCUtil.SetGameobjectActive(m_objSubTabParent, flag);
		if (flag)
		{
			m_LoopScrollSubTab.TotalCount = m_dicTabTemplet[targetTemplet.m_BoardTab].Count - 1;
			m_LoopScrollSubTab.RefreshCells();
			m_rtContent.offsetMin = new Vector2(274f, 0f);
		}
		else
		{
			m_rtContent.offsetMin = new Vector2(0f, 0f);
		}
	}

	private int CompLeaderBoard(NKMLeaderBoardTemplet lItem, NKMLeaderBoardTemplet rItem)
	{
		if (lItem.m_OrderList == rItem.m_OrderList)
		{
			if (lItem.m_BoardTabSubIndex == rItem.m_BoardTabSubIndex)
			{
				return lItem.m_BoardID.CompareTo(rItem.m_BoardID);
			}
			return lItem.m_BoardTabSubIndex.CompareTo(rItem.m_BoardTabSubIndex);
		}
		return rItem.m_OrderList.CompareTo(lItem.m_OrderList);
	}

	public void OnClickMainTab(LeaderBoardType boardType)
	{
		if (m_CurrentBoardType == boardType)
		{
			return;
		}
		m_CurrentBoardType = boardType;
		if (!m_dicTabTemplet.ContainsKey(m_CurrentBoardType) || m_dicTabTemplet[m_CurrentBoardType].Count == 0)
		{
			Debug.LogError($"{m_CurrentBoardType} 데이터 없음");
			return;
		}
		if (m_dicBoardTab.ContainsKey(boardType))
		{
			m_dicBoardTab[boardType].GetComponent<NKCUIComToggle>().Select(bSelect: true, bForce: true);
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = null;
		nKMLeaderBoardTemplet = ((m_dicTabTemplet[m_CurrentBoardType].Count <= 1) ? m_dicTabTemplet[m_CurrentBoardType][0] : m_dicTabTemplet[m_CurrentBoardType][1]);
		SetSubTab(nKMLeaderBoardTemplet);
		OnSelectTab(nKMLeaderBoardTemplet.m_BoardID);
	}

	private void OnSelectTab(int boardTabID)
	{
		if (m_cNKMLeaderBoardTemplet != null && m_cNKMLeaderBoardTemplet.m_BoardID == boardTabID)
		{
			return;
		}
		m_cNKMLeaderBoardTemplet = NKMTempletContainer<NKMLeaderBoardTemplet>.Find(boardTabID);
		if (m_cNKMLeaderBoardTemplet == null)
		{
			Log.Error($"NKMLeaderBoardTemplet is null - TabID : {boardTabID} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/LeaderBoard/NKCUILeaderBoard.cs", 516);
			return;
		}
		m_CurrentBoardType = m_cNKMLeaderBoardTemplet.m_BoardTab;
		foreach (NKCUILeaderBoardTab value in m_dicBoardTab.Values)
		{
			if (value != null)
			{
				value.SetTitleColor(value.m_tgl.m_bChecked);
			}
		}
		for (int i = 0; i < m_lstVisibleSubTab.Count; i++)
		{
			if (m_lstVisibleSubTab[i].gameObject.activeSelf && m_lstVisibleSubTab[i].m_tabID == boardTabID)
			{
				m_lstVisibleSubTab[i].m_tgl.Select(bSelect: true);
				break;
			}
		}
		if (m_imgBanner != null && (m_imgBanner.sprite == null || !string.Equals(m_imgBanner.sprite.name, m_cNKMLeaderBoardTemplet.m_BoardBackgroundImg)))
		{
			NKCUtil.SetImageSprite(m_imgBanner, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_BG", m_cNKMLeaderBoardTemplet.m_BoardBackgroundImg));
		}
		if (!NKCLeaderBoardManager.HasLeaderBoardData(m_cNKMLeaderBoardTemplet))
		{
			NKCLeaderBoardManager.SendReq(m_cNKMLeaderBoardTemplet, bAllReq: false);
		}
		else
		{
			RefreshUI(bResetScroll: true);
		}
	}

	public void RefreshUI(bool bResetScroll = false)
	{
		foreach (KeyValuePair<LeaderBoardType, NKCUILeaderBoardTab> item in m_dicBoardTab)
		{
			item.Value.CheckRedDot();
		}
		List<LeaderBoardSlotData> leaderBoardData = NKCLeaderBoardManager.GetLeaderBoardData(m_cNKMLeaderBoardTemplet.m_BoardID);
		switch (m_cNKMLeaderBoardTemplet.m_LayoutType)
		{
		case LayoutType.NORMAL:
			m_LoopScrollContent.TotalCount = leaderBoardData.Count;
			break;
		case LayoutType.TOP_3_AND_RANKING:
			if (leaderBoardData.Count == 0)
			{
				m_LoopScrollContent.TotalCount = 0;
			}
			else
			{
				m_LoopScrollContent.TotalCount = ((leaderBoardData.Count <= 2) ? 1 : (leaderBoardData.Count - 2));
			}
			break;
		case LayoutType.TOP_3_ONLY:
			if (leaderBoardData.Count == 0)
			{
				m_LoopScrollContent.TotalCount = 0;
			}
			else
			{
				m_LoopScrollContent.TotalCount = Mathf.CeilToInt((float)leaderBoardData.Count / 3f);
			}
			break;
		default:
			m_LoopScrollContent.TotalCount = 0;
			break;
		}
		NKCUtil.SetGameobjectActive(m_lbRemainTime, IsShowRemainTime());
		if (m_lbRemainTime.gameObject.activeSelf)
		{
			m_tNextResetTime = NKCLeaderBoardManager.GetNextResetTime(m_cNKMLeaderBoardTemplet);
			SetRemainTime(m_tNextResetTime);
		}
		if (bResetScroll)
		{
			m_LoopScrollContent.SetIndexPosition(0);
		}
		else
		{
			m_LoopScrollContent.RefreshCells();
		}
		NKCUtil.SetLabelText(m_lbTitle, m_cNKMLeaderBoardTemplet.GetTitle());
		NKCUtil.SetLabelText(m_lbDesc, m_cNKMLeaderBoardTemplet.GetDesc());
		SetMyRank();
		if (NKCLeaderBoardManager.GetLeaderBoardData(m_cNKMLeaderBoardTemplet.m_BoardID).Count > 0)
		{
			NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objNone, bValue: true);
		}
	}

	private bool IsShowRemainTime()
	{
		return m_cNKMLeaderBoardTemplet.m_BoardTab switch
		{
			_ => false, 
		};
	}

	private void SetRemainTime(DateTime nextResetTime)
	{
		if (nextResetTime > DateTime.MinValue)
		{
			NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: true);
			NKCUtil.SetLabelText(m_lbRemainTime, "");
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: false);
		}
	}

	private void SetMyRank()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			NKCUtil.SetGameobjectActive(m_slotMyRank, bValue: false);
			return;
		}
		int myRank = 0;
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		NKMCommonProfile nKMCommonProfile;
		if (userProfileData != null)
		{
			nKMCommonProfile = userProfileData.commonProfile;
		}
		else
		{
			nKMCommonProfile = new NKMCommonProfile();
			nKMCommonProfile.friendCode = nKMUserData.m_FriendCode;
			nKMCommonProfile.level = nKMUserData.UserLevel;
			nKMCommonProfile.nickname = nKMUserData.m_UserNickName;
			nKMCommonProfile.userUid = nKMUserData.m_UserUID;
		}
		switch (m_cNKMLeaderBoardTemplet.m_BoardTab)
		{
		case LeaderBoardType.BT_ACHIEVE:
		{
			NKCUtil.SetGameobjectActive(m_slotMyRank, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnSeasonSelect, bValue: false);
			LeaderBoardSlotData myRankSlotData2 = NKCLeaderBoardManager.GetMyRankSlotData(m_cNKMLeaderBoardTemplet.m_BoardID);
			myRankSlotData2.Profile = nKMCommonProfile;
			if (NKCGuildManager.HasGuild())
			{
				myRankSlotData2.GuildData = new NKMGuildSimpleData();
				myRankSlotData2.GuildData.badgeId = NKCGuildManager.MyGuildData.badgeId;
				myRankSlotData2.GuildData.guildName = NKCGuildManager.MyGuildData.name;
				myRankSlotData2.GuildData.guildUid = NKCGuildManager.MyGuildData.guildUid;
			}
			m_slotMyRank.SetData(myRankSlotData2, m_cNKMLeaderBoardTemplet.m_BoardCriteria, null, bUsePercentRank: false, bShowMyRankIcon: false);
			NKCUtil.SetGameobjectActive(m_btnRewardInfo, bValue: false);
			break;
		}
		case LeaderBoardType.BT_SHADOW:
		{
			NKCUtil.SetGameobjectActive(m_slotMyRank, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnSeasonSelect, bValue: false);
			LeaderBoardSlotData myRankSlotData5 = NKCLeaderBoardManager.GetMyRankSlotData(m_cNKMLeaderBoardTemplet.m_BoardID);
			myRankSlotData5.Profile = nKMCommonProfile;
			if (NKCGuildManager.HasGuild())
			{
				myRankSlotData5.GuildData = new NKMGuildSimpleData();
				myRankSlotData5.GuildData.badgeId = NKCGuildManager.MyGuildData.badgeId;
				myRankSlotData5.GuildData.guildName = NKCGuildManager.MyGuildData.name;
				myRankSlotData5.GuildData.guildUid = NKCGuildManager.MyGuildData.guildUid;
			}
			m_slotMyRank.SetData(myRankSlotData5, m_cNKMLeaderBoardTemplet.m_BoardCriteria, null, bUsePercentRank: false, bShowMyRankIcon: false);
			NKCUtil.SetGameobjectActive(m_btnRewardInfo, bValue: false);
			break;
		}
		case LeaderBoardType.BT_FIERCE:
		{
			NKCUtil.SetGameobjectActive(m_slotMyRank, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnSeasonSelect, bValue: false);
			LeaderBoardSlotData myRankSlotData3 = NKCLeaderBoardManager.GetMyRankSlotData(m_cNKMLeaderBoardTemplet.m_BoardID);
			myRankSlotData3.Profile = nKMCommonProfile;
			if (NKCGuildManager.HasGuild())
			{
				myRankSlotData3.GuildData = new NKMGuildSimpleData();
				myRankSlotData3.GuildData.badgeId = NKCGuildManager.MyGuildData.badgeId;
				myRankSlotData3.GuildData.guildName = NKCGuildManager.MyGuildData.name;
				myRankSlotData3.GuildData.guildUid = NKCGuildManager.MyGuildData.guildUid;
			}
			myRank = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().GetRankingTotalNumber();
			if (myRank != 0 && myRank <= 100)
			{
				m_slotMyRank.SetData(myRankSlotData3, m_cNKMLeaderBoardTemplet.m_BoardCriteria, null, bUsePercentRank: false, bShowMyRankIcon: false);
			}
			else
			{
				m_slotMyRank.SetData(myRankSlotData3, m_cNKMLeaderBoardTemplet.m_BoardCriteria, null, bUsePercentRank: true, bShowMyRankIcon: false);
			}
			NKCUtil.SetGameobjectActive(m_btnRewardInfo, bValue: true);
			break;
		}
		case LeaderBoardType.BT_GUILD:
		{
			NKCUtil.SetGameobjectActive(m_slotMyRank, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnSeasonSelect, m_cNKMLeaderBoardTemplet.m_BoardCriteria != 1);
			NKCUtil.SetGameobjectActive(m_btnRewardInfo, bValue: false);
			NKMGuildRankData rankData = NKCLeaderBoardManager.MakeMyGuildRankData(m_cNKMLeaderBoardTemplet.m_BoardID, out myRank);
			m_slotMyRank.SetData(LeaderBoardSlotData.MakeSlotData(rankData, myRank), m_cNKMLeaderBoardTemplet.m_BoardCriteria, null, bUsePercentRank: false, bShowMyRankIcon: false);
			break;
		}
		case LeaderBoardType.BT_TIMEATTACK:
		{
			NKCUtil.SetGameobjectActive(m_btnSeasonSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_slotMyRank, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnRewardInfo, bValue: false);
			LeaderBoardSlotData myRankSlotData6 = NKCLeaderBoardManager.GetMyRankSlotData(m_cNKMLeaderBoardTemplet.m_BoardID);
			myRankSlotData6.Profile = nKMCommonProfile;
			if (NKCGuildManager.HasGuild())
			{
				myRankSlotData6.GuildData = new NKMGuildSimpleData();
				myRankSlotData6.GuildData.badgeId = NKCGuildManager.MyGuildData.badgeId;
				myRankSlotData6.GuildData.guildName = NKCGuildManager.MyGuildData.name;
				myRankSlotData6.GuildData.guildUid = NKCGuildManager.MyGuildData.guildUid;
			}
			m_slotMyRank.SetData(myRankSlotData6, m_cNKMLeaderBoardTemplet.m_BoardCriteria, null);
			break;
		}
		case LeaderBoardType.BT_DEFENCE:
		{
			NKCUtil.SetGameobjectActive(m_btnSeasonSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_slotMyRank, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnRewardInfo, bValue: true);
			LeaderBoardSlotData myRankSlotData4 = NKCLeaderBoardManager.GetMyRankSlotData(m_cNKMLeaderBoardTemplet.m_BoardID);
			myRankSlotData4.Profile = nKMCommonProfile;
			if (NKCGuildManager.HasGuild())
			{
				myRankSlotData4.GuildData = new NKMGuildSimpleData();
				myRankSlotData4.GuildData.badgeId = NKCGuildManager.MyGuildData.badgeId;
				myRankSlotData4.GuildData.guildName = NKCGuildManager.MyGuildData.name;
				myRankSlotData4.GuildData.guildUid = NKCGuildManager.MyGuildData.guildUid;
			}
			myRank = NKCDefenceDungeonManager.m_MyRankNum;
			if (myRank != 0 && myRank <= 100)
			{
				m_slotMyRank.SetData(myRankSlotData4, m_cNKMLeaderBoardTemplet.m_BoardCriteria, null);
			}
			else
			{
				m_slotMyRank.SetData(myRankSlotData4, m_cNKMLeaderBoardTemplet.m_BoardCriteria, null, bUsePercentRank: true);
			}
			break;
		}
		case LeaderBoardType.BT_LEAGUE:
		case LeaderBoardType.BT_UNLIMITED:
		{
			NKCUtil.SetGameobjectActive(m_btnSeasonSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_slotMyRank, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnRewardInfo, bValue: true);
			LeaderBoardSlotData myRankSlotData = NKCLeaderBoardManager.GetMyRankSlotData(m_cNKMLeaderBoardTemplet.m_BoardID);
			myRankSlotData.Profile = nKMCommonProfile;
			if (NKCGuildManager.HasGuild())
			{
				myRankSlotData.GuildData = new NKMGuildSimpleData();
				myRankSlotData.GuildData.badgeId = NKCGuildManager.MyGuildData.badgeId;
				myRankSlotData.GuildData.guildName = NKCGuildManager.MyGuildData.name;
				myRankSlotData.GuildData.guildUid = NKCGuildManager.MyGuildData.guildUid;
			}
			myRank = myRankSlotData.rank;
			m_slotMyRank.SetData(myRankSlotData, m_cNKMLeaderBoardTemplet.m_BoardCriteria, null);
			break;
		}
		default:
			NKCUtil.SetGameobjectActive(m_btnSeasonSelect, bValue: false);
			NKCUtil.SetGameobjectActive(m_slotMyRank, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnRewardInfo, bValue: false);
			break;
		}
	}

	public void Update()
	{
		if (!m_lbRemainTime.gameObject.activeSelf || !(m_tNextResetTime > DateTime.MinValue))
		{
			return;
		}
		m_fDeltaTime += Time.deltaTime;
		if (m_fDeltaTime > 1f)
		{
			m_fDeltaTime = 0f;
			if (!NKCSynchronizedTime.IsFinished(m_tNextResetTime))
			{
				m_tNextResetTime = DateTime.MinValue;
				NKCLeaderBoardManager.SendReq(m_cNKMLeaderBoardTemplet, bAllReq: true);
			}
			else
			{
				SetRemainTime(m_tNextResetTime);
			}
		}
	}

	public void OnClickRewardInfo()
	{
		switch (m_cNKMLeaderBoardTemplet.m_BoardTab)
		{
		case LeaderBoardType.BT_FIERCE:
		case LeaderBoardType.BT_DEFENCE:
			NKCUIPopupFierceBattleRewardInfo.Instance.Open(m_cNKMLeaderBoardTemplet.m_BoardTab);
			break;
		case LeaderBoardType.BT_LEAGUE:
		case LeaderBoardType.BT_UNLIMITED:
			NKCUIPopupFierceBattleRewardInfo.Instance.Open(m_cNKMLeaderBoardTemplet.m_BoardTab, NKCUIPopupFierceBattleRewardInfo.REWARD_SLOT_TYPE.GauntletLeague);
			break;
		case LeaderBoardType.BT_GUILD:
		case LeaderBoardType.BT_TIMEATTACK:
		case LeaderBoardType.BT_TOURNAMENT:
			break;
		}
	}

	public void OnClickSeasonSelect()
	{
		NKCPopupGuildRankSeasonSelect.Instance.Open(OnSelectSeason, m_cNKMLeaderBoardTemplet.m_BoardCriteria);
	}

	private void OnSelectSeason(int seasonId)
	{
		m_cNKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_GUILD, seasonId);
		if (!NKCLeaderBoardManager.HasLeaderBoardData(m_cNKMLeaderBoardTemplet))
		{
			NKCLeaderBoardManager.SendReq(m_cNKMLeaderBoardTemplet, bAllReq: false);
		}
		else
		{
			RefreshUI();
		}
	}

	private bool CheckUseGuildSlot(LeaderBoardType type)
	{
		if (type == LeaderBoardType.BT_GUILD)
		{
			return true;
		}
		return false;
	}

	private void OnEventPanelBeginDragAll()
	{
		if (!NKCLeaderBoardManager.GetReceivedAllData(m_cNKMLeaderBoardTemplet.m_BoardID) || NKCLeaderBoardManager.NeedRefreshData(m_cNKMLeaderBoardTemplet))
		{
			NKCLeaderBoardManager.SendReq(m_cNKMLeaderBoardTemplet, bAllReq: true);
		}
	}
}
