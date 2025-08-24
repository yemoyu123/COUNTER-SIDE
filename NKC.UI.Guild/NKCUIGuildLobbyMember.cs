using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Guild;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildLobbyMember : MonoBehaviour
{
	public enum GUILD_MEMBER_SLOT_TYPE
	{
		Normal,
		JoinWaiting
	}

	public NKCUIComToggle m_tglMember;

	public NKCUIComToggle m_tglJoinWaiting;

	public LoopScrollRect m_loopScroll;

	public Transform m_trContentParent;

	public NKCUIComStateButton m_btnInvite;

	public NKCUIComStateButton m_btnEditMyComment;

	[Header("내림차순/오름차순 토글")]
	public NKCUIComToggle m_ctgDescending;

	[Header("정렬 옵션")]
	public NKCUIComToggle m_tgSortTypeMenu;

	public GameObject m_objSortSelect;

	public NKCPopupSort m_NKCPopupSort;

	public Text m_txtSortType;

	public Text m_txtSelectedSortType;

	[Header("리스트 비었을 때 표시")]
	public GameObject m_objEmpty;

	private Stack<NKCUIGuildMemberSlot> m_stkMember = new Stack<NKCUIGuildMemberSlot>();

	private List<NKCUIGuildMemberSlot> m_lstVisibleSlot = new List<NKCUIGuildMemberSlot>();

	private List<NKMGuildMemberData> m_lstMember = new List<NKMGuildMemberData>();

	private List<FriendListData> m_lstJoinWaiting = new List<FriendListData>();

	private NKMGuildData m_GuildData;

	private GUILD_MEMBER_SLOT_TYPE m_MemberSlotType;

	private NKCUnitSortSystem.eSortCategory DEFATUL_SORT_CATEGORY = NKCUnitSortSystem.eSortCategory.GuildGrade;

	private NKCUnitSortSystem.eSortOption m_currentSortOption = NKCUnitSortSystem.eSortOption.Guild_Grade_High;

	private HashSet<NKCUnitSortSystem.eSortCategory> m_hsSort = new HashSet<NKCUnitSortSystem.eSortCategory>
	{
		NKCUnitSortSystem.eSortCategory.GuildGrade,
		NKCUnitSortSystem.eSortCategory.GuildWeeklyPoint,
		NKCUnitSortSystem.eSortCategory.GuildTotalPoint,
		NKCUnitSortSystem.eSortCategory.LoginTime
	};

	public void InitUI()
	{
		m_tglMember.OnValueChanged.RemoveAllListeners();
		m_tglMember.OnValueChanged.AddListener(OnClickMember);
		m_tglJoinWaiting.OnValueChanged.RemoveAllListeners();
		m_tglJoinWaiting.OnValueChanged.AddListener(OnClickJoinWaiting);
		m_loopScroll.dOnGetObject += GetObject;
		m_loopScroll.dOnReturnObject += ReturnObject;
		m_loopScroll.dOnProvideData += ProvideData;
		m_loopScroll.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loopScroll);
		m_btnInvite.PointerClick.RemoveAllListeners();
		m_btnInvite.PointerClick.AddListener(OnClickInvite);
		m_btnEditMyComment.PointerClick.RemoveAllListeners();
		m_btnEditMyComment.PointerClick.AddListener(OnClickEditMyComment);
		if (m_ctgDescending != null)
		{
			m_ctgDescending.OnValueChanged.RemoveAllListeners();
			m_ctgDescending.OnValueChanged.AddListener(OnCheckAscend);
		}
		m_tgSortTypeMenu.OnValueChanged.RemoveAllListeners();
		m_tgSortTypeMenu.OnValueChanged.AddListener(OnSortMenuOpen);
		m_MemberSlotType = GUILD_MEMBER_SLOT_TYPE.Normal;
		m_tglMember.Select(bSelect: true, bForce: true, bImmediate: true);
	}

	private RectTransform GetObject(int index)
	{
		NKCUIGuildMemberSlot nKCUIGuildMemberSlot = null;
		nKCUIGuildMemberSlot = ((m_stkMember.Count <= 0) ? NKCUIGuildMemberSlot.GetNewInstance(m_trContentParent, OnSelectedSlot) : m_stkMember.Pop());
		m_lstVisibleSlot.Add(nKCUIGuildMemberSlot);
		NKCUtil.SetGameobjectActive(nKCUIGuildMemberSlot, bValue: false);
		return nKCUIGuildMemberSlot?.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIGuildMemberSlot component = tr.GetComponent<NKCUIGuildMemberSlot>();
		m_lstVisibleSlot.Remove(component);
		m_stkMember.Push(component);
		NKCUtil.SetGameobjectActive(component, bValue: false);
		tr.SetParent(base.transform);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIGuildMemberSlot component = tr.GetComponent<NKCUIGuildMemberSlot>();
		if (component == null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			return;
		}
		switch (m_MemberSlotType)
		{
		case GUILD_MEMBER_SLOT_TYPE.Normal:
			if (m_GuildData.members.Count < idx)
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
			else
			{
				component.SetData(m_lstMember[idx], bIsMyGuild: true);
			}
			break;
		case GUILD_MEMBER_SLOT_TYPE.JoinWaiting:
			if (m_GuildData.joinWaitingList.Count < idx)
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
			}
			else
			{
				component.SetData(m_lstJoinWaiting[idx], NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Member);
			}
			break;
		}
	}

	public void SetData(NKMGuildData guildData)
	{
		m_GuildData = guildData;
		RefreshUI();
	}

	public void RefreshUI(bool bResetScroll = false)
	{
		NKMGuildMemberData nKMGuildMemberData = m_GuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
		if (nKMGuildMemberData != null)
		{
			NKCUtil.SetGameobjectActive(m_tglJoinWaiting, nKMGuildMemberData.grade != GuildMemberGrade.Member);
			NKCUtil.SetGameobjectActive(m_btnInvite, nKMGuildMemberData.grade != GuildMemberGrade.Member);
		}
		switch (m_MemberSlotType)
		{
		case GUILD_MEMBER_SLOT_TYPE.Normal:
			m_lstMember = m_GuildData.members;
			m_lstMember.Sort(Compare);
			m_loopScroll.TotalCount = m_GuildData.members.Count;
			NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
			break;
		case GUILD_MEMBER_SLOT_TYPE.JoinWaiting:
			m_lstJoinWaiting = m_GuildData.joinWaitingList;
			m_loopScroll.TotalCount = m_GuildData.joinWaitingList.Count;
			NKCUtil.SetGameobjectActive(m_objEmpty, m_loopScroll.TotalCount == 0);
			break;
		}
		m_loopScroll.RefreshCells();
		NKCUtil.SetLabelText(m_txtSortType, NKCUnitSortSystem.GetSortName(m_currentSortOption));
		NKCUtil.SetLabelText(m_txtSelectedSortType, NKCUnitSortSystem.GetSortName(m_currentSortOption));
		NKCUtil.SetGameobjectActive(m_objSortSelect, NKCUnitSortSystem.GetSortCategoryFromOption(m_currentSortOption) != DEFATUL_SORT_CATEGORY);
		if (bResetScroll)
		{
			m_loopScroll.SetIndexPosition(0);
		}
	}

	private void OnSelectedSlot(long userUid)
	{
		NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(userUid, NKM_DECK_TYPE.NDT_NORMAL);
	}

	private void OnClickInvite()
	{
		NKCPopupGuildInvite.Instance.Open();
	}

	private void OnClickMember(bool bValue)
	{
		if (bValue && m_MemberSlotType != GUILD_MEMBER_SLOT_TYPE.Normal)
		{
			m_MemberSlotType = GUILD_MEMBER_SLOT_TYPE.Normal;
			RefreshUI(bResetScroll: true);
		}
	}

	private void OnClickJoinWaiting(bool bValue)
	{
		if (bValue && m_MemberSlotType != GUILD_MEMBER_SLOT_TYPE.JoinWaiting)
		{
			m_MemberSlotType = GUILD_MEMBER_SLOT_TYPE.JoinWaiting;
			RefreshUI(bResetScroll: true);
		}
	}

	private void OnClickEditMyComment()
	{
		string text = m_GuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID).greeting;
		if (string.IsNullOrEmpty(text))
		{
			text = NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_INTRODUCE_WRITE_POPUP_GUIDE_DESC;
		}
		NKCPopupInputText.Instance.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_INTRODUCE_WRITE_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_INTRODUCE_WRITE_POPUP_BODY_DESC, "", text, OnEditMyComment, null, bDev: false, 13);
	}

	private void OnEditMyComment(string greeting)
	{
		NKCPacketSender.Send_NKMPacket_GUILD_UPDATE_MEMBER_GREETING_REQ(NKCGuildManager.MyData.guildUid, greeting);
	}

	private void OnCheckAscend(bool bValue)
	{
		if (m_lstMember.Count > 1)
		{
			m_currentSortOption = NKCUnitSortSystem.GetInvertedAscendOption(m_currentSortOption);
			RefreshUI();
		}
	}

	private void OnSortMenuOpen(bool bValue)
	{
		m_NKCPopupSort.OpenGuildMemberSortMenu(m_hsSort, m_currentSortOption, OnSort, bValue);
		SetOpenSortingMenu(bValue);
	}

	private void OnSort(List<NKCUnitSortSystem.eSortOption> sortOptionList)
	{
		m_currentSortOption = sortOptionList[0];
		SetOpenSortingMenu(bOpen: false);
		RefreshUI();
	}

	public void SetOpenSortingMenu(bool bOpen, bool bAnimate = true)
	{
		NKCUtil.SetLabelText(m_txtSelectedSortType, NKCUnitSortSystem.GetSortName(m_currentSortOption));
		m_tgSortTypeMenu.Select(bOpen, bForce: true);
		m_NKCPopupSort.StartRectMove(bOpen, bAnimate);
	}

	private int Compare(NKMGuildMemberData member_a, NKMGuildMemberData member_b)
	{
		switch (m_currentSortOption)
		{
		case NKCUnitSortSystem.eSortOption.Guild_Grade_High:
			if (member_a.grade != member_b.grade)
			{
				return member_a.grade.CompareTo(member_b.grade);
			}
			if (member_a.totalContributionPoint != member_b.totalContributionPoint)
			{
				return member_b.totalContributionPoint.CompareTo(member_a.totalContributionPoint);
			}
			break;
		case NKCUnitSortSystem.eSortOption.Guild_Grade_Low:
			if (member_a.grade != member_b.grade)
			{
				return member_b.grade.CompareTo(member_a.grade);
			}
			if (member_a.totalContributionPoint != member_b.totalContributionPoint)
			{
				return member_a.totalContributionPoint.CompareTo(member_b.totalContributionPoint);
			}
			break;
		case NKCUnitSortSystem.eSortOption.Guild_WeeklyPoint_High:
			if (member_a.weeklyContributionPoint != member_b.weeklyContributionPoint)
			{
				return member_b.weeklyContributionPoint.CompareTo(member_a.weeklyContributionPoint);
			}
			break;
		case NKCUnitSortSystem.eSortOption.Guild_WeeklyPoint_Low:
			if (member_a.weeklyContributionPoint != member_b.weeklyContributionPoint)
			{
				return member_a.weeklyContributionPoint.CompareTo(member_b.weeklyContributionPoint);
			}
			break;
		case NKCUnitSortSystem.eSortOption.Guild_TotalPoint_High:
			if (member_a.totalContributionPoint != member_b.totalContributionPoint)
			{
				return member_b.totalContributionPoint.CompareTo(member_a.totalContributionPoint);
			}
			break;
		case NKCUnitSortSystem.eSortOption.Guild_TotalPoint_Low:
			if (member_a.totalContributionPoint != member_b.totalContributionPoint)
			{
				return member_a.totalContributionPoint.CompareTo(member_b.totalContributionPoint);
			}
			break;
		case NKCUnitSortSystem.eSortOption.LoginTime_Latly:
			return member_b.lastOnlineTime.CompareTo(member_a.lastOnlineTime);
		case NKCUnitSortSystem.eSortOption.LoginTime_Old:
			return member_a.lastOnlineTime.CompareTo(member_b.lastOnlineTime);
		}
		return member_a.grade.CompareTo(member_b.grade);
	}
}
