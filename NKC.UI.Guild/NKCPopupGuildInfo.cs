using System.Collections.Generic;
using ClientPacket.Guild;
using NKM;
using NKM.Guild;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildInfo : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_POPUP_INFO";

	private static NKCPopupGuildInfo m_Instance;

	public NKCUIGuildBadge m_BadgeUI;

	public Text m_lbLevel;

	public Text m_lbName;

	public Text m_lbDesc;

	public NKCUIComStateButton m_btnClose;

	public Text m_lbMemberCount;

	public LoopScrollRect m_loopMember;

	public Transform m_trContentParent;

	public NKCUIComStateButton m_btnJoin;

	public Text m_lbJoinType;

	private Stack<NKCUIGuildMemberSlot> m_stkMember = new Stack<NKCUIGuildMemberSlot>();

	private List<NKCUIGuildMemberSlot> m_lstVisibleSlot = new List<NKCUIGuildMemberSlot>();

	private NKMGuildData m_GuildData;

	public static NKCPopupGuildInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildInfo>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_POPUP_INFO", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildInfo>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

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
			NKCUtil.SetGameobjectActive(tr, bValue: false);
		}
		else
		{
			component.SetData(m_GuildData.members[idx], m_GuildData.guildUid == NKCGuildManager.MyData.guildUid);
		}
	}

	public void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnJoin.PointerClick.RemoveAllListeners();
		m_btnJoin.PointerClick.AddListener(OnClickJoin);
		NKCUtil.SetHotkey(m_btnJoin, HotkeyEventType.Confirm);
		m_loopMember.dOnGetObject += GetObject;
		m_loopMember.dOnReturnObject += ReturnObject;
		m_loopMember.dOnProvideData += ProvideData;
		m_loopMember.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loopMember);
		m_BadgeUI.InitUI();
	}

	public void Open(NKMGuildData guildData)
	{
		m_GuildData = guildData;
		SetBasicData(guildData);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loopMember.TotalCount = m_GuildData.members.Count;
		m_loopMember.SetIndexPosition(0);
		UIOpened();
	}

	private void SetBasicData(NKMGuildData guildData)
	{
		m_BadgeUI.SetData(guildData.badgeId);
		NKCUtil.SetLabelText(m_lbName, guildData.name);
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, guildData.guildLevel));
		NKCUtil.SetLabelText(m_lbDesc, guildData.greeting);
		switch (guildData.guildJoinType)
		{
		case GuildJoinType.DirectJoin:
			NKCUtil.SetLabelText(m_lbJoinType, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_RIGHTOFF_DESC);
			break;
		case GuildJoinType.NeedApproval:
			NKCUtil.SetLabelText(m_lbJoinType, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_CONFIRM_DESC);
			break;
		case GuildJoinType.Closed:
			NKCUtil.SetLabelText(m_lbJoinType, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_BLIND_DESC);
			break;
		}
		NKCUtil.SetGameobjectActive(m_btnJoin, !NKCGuildManager.HasGuild() && guildData.guildJoinType != GuildJoinType.Closed && !NKCGuildManager.AlreadyRequested(m_GuildData.guildUid) && !NKCGuildManager.AlreadyInvited(m_GuildData.guildUid));
		NKCUtil.SetLabelText(m_lbMemberCount, $"({guildData.members.Count}/{NKMTempletContainer<GuildExpTemplet>.Find(guildData.guildLevel).MaxMemberCount})");
	}

	private void OnSelectedSlot(long userUid)
	{
		NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(userUid, NKM_DECK_TYPE.NDT_NORMAL);
	}

	private void OnClickJoin()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_JOIN_CONFIRM_JOIN_POPUP_TITLE_DESC, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_JOIN_RIGHTOFF_JOIN_POPUP_BODY_DESC, m_GuildData.name), OnConfirmJoin);
	}

	private void OnConfirmJoin()
	{
		NKCGuildManager.Send_GUILD_JOIN_REQ(m_GuildData.guildUid, m_GuildData.name, m_GuildData.guildJoinType);
		Close();
	}
}
