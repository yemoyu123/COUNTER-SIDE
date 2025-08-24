using System;
using System.Linq;
using System.Text;
using ClientPacket.Guild;
using Cs.Core.Util;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildLobbyManage : MonoBehaviour
{
	public delegate void OnClose(NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE uiType, bool bForce = false);

	public Text m_lbGuildName;

	public InputField m_inputDesc;

	[Space]
	public GameObject m_objNameChangeRoot;

	public NKCUIComStateButton m_btnNameChange;

	public Text m_lbNameChangeRemain;

	[Space]
	public NKCUIComToggle m_tglJoinTypeDirect;

	public NKCUIComToggle m_tglJoinTypeApproval;

	public NKCUIComToggle m_tglJoinTypeClosed;

	public GameObject m_objJoinTypeError;

	[Space]
	public NKCUIComToggle m_tglGuildNotice;

	public NKCUIComToggle m_tglCoopNotice;

	[Space]
	public NKCUIGuildBadge m_badgeUI;

	public NKCUIComStateButton m_btnBadgeSetting;

	public NKCUIComStateButton m_btnBadgeRandom;

	[Space]
	public NKCUIComStateButton m_btnGuildClose;

	public NKCUIComStateButton m_btnGuildCloseCancel;

	public GameObject m_objGuildBreakupTime;

	public Text m_lbGuildBreakupTime;

	[Space]
	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnCancel;

	private OnClose m_dOnClose;

	private long m_BadgeId;

	private GuildJoinType m_JoinType;

	private string m_Greeting;

	private GuildChatNoticeType m_NoticeType;

	private DateTime m_tExpireTime;

	private float m_fDeltaTime;

	public void InitUI()
	{
		m_tglJoinTypeDirect.OnValueChanged.RemoveAllListeners();
		m_tglJoinTypeDirect.OnValueChanged.AddListener(OnSelectDirect);
		m_tglJoinTypeApproval.OnValueChanged.RemoveAllListeners();
		m_tglJoinTypeApproval.OnValueChanged.AddListener(OnSelectApproval);
		m_tglJoinTypeClosed.OnValueChanged.RemoveAllListeners();
		m_tglJoinTypeClosed.OnValueChanged.AddListener(OnSelectClosed);
		m_tglGuildNotice.OnValueChanged.RemoveAllListeners();
		m_tglGuildNotice.OnValueChanged.AddListener(OnSelectGuildNotice);
		m_tglCoopNotice.OnValueChanged.RemoveAllListeners();
		m_tglCoopNotice.OnValueChanged.AddListener(OnSelectCoopNotice);
		m_inputDesc.onValidateInput = NKCFilterManager.FilterEmojiInput;
		m_inputDesc.onEndEdit.RemoveAllListeners();
		m_inputDesc.onEndEdit.AddListener(OnGreetingChanged);
		m_inputDesc.characterLimit = 40;
		m_badgeUI?.InitUI();
		m_btnBadgeSetting.PointerClick.RemoveAllListeners();
		m_btnBadgeSetting.PointerClick.AddListener(OnClickBadgeSetting);
		m_btnBadgeRandom.PointerClick.RemoveAllListeners();
		m_btnBadgeRandom.PointerClick.AddListener(OnClickBadgeRandom);
		m_btnGuildClose.PointerClick.RemoveAllListeners();
		m_btnGuildClose.PointerClick.AddListener(OnClickGuildClose);
		m_btnGuildCloseCancel.PointerClick.RemoveAllListeners();
		m_btnGuildCloseCancel.PointerClick.AddListener(OnClickGuildCloseCancel);
		m_btnOk.PointerClick.RemoveAllListeners();
		m_btnOk.PointerClick.AddListener(OnClickOk);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(OnClickCancel);
		NKCUtil.SetButtonClickDelegate(m_btnNameChange, OnClickNameChange);
	}

	public void SetData(OnClose onClose)
	{
		m_dOnClose = onClose;
		m_fDeltaTime = 0f;
		m_BadgeId = NKCGuildManager.MyGuildData.badgeId;
		m_JoinType = NKCGuildManager.MyGuildData.guildJoinType;
		m_Greeting = NKCGuildManager.MyGuildData.greeting;
		m_NoticeType = NKCGuildManager.MyGuildData.chatNoticeType;
		NKCUtil.SetLabelText(m_lbGuildName, NKCGuildManager.MyGuildData.name);
		m_badgeUI.SetData(m_BadgeId);
		m_tglJoinTypeDirect.Select(m_JoinType == GuildJoinType.DirectJoin, bForce: true, bImmediate: true);
		m_tglJoinTypeApproval.Select(m_JoinType == GuildJoinType.NeedApproval, bForce: true, bImmediate: true);
		m_tglJoinTypeClosed.Select(m_JoinType == GuildJoinType.Closed, bForce: true, bImmediate: true);
		m_tglGuildNotice.Select(m_NoticeType == GuildChatNoticeType.Normal, bForce: true, bImmediate: true);
		m_tglCoopNotice.Select(m_NoticeType == GuildChatNoticeType.Dungeon, bForce: true, bImmediate: true);
		NKCUtil.SetGameobjectActive(m_btnGuildClose, NKCGuildManager.MyGuildData.guildState != GuildState.Closing);
		NKCUtil.SetGameobjectActive(m_btnGuildCloseCancel, NKCGuildManager.MyGuildData.guildState == GuildState.Closing);
		NKCUtil.SetGameobjectActive(m_objGuildBreakupTime, NKCGuildManager.MyGuildData.guildState == GuildState.Closing);
		NKCUtil.SetLabelText(m_inputDesc.textComponent, m_Greeting);
		m_inputDesc.text = m_Greeting;
		m_tExpireTime = NKCGuildManager.MyGuildData.closingTime;
		SetButton();
		if (m_objGuildBreakupTime.activeSelf)
		{
			SetRemainTime();
		}
		bool flag = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.GUILD_RENAME);
		NKCUtil.SetGameobjectActive(m_objNameChangeRoot, flag);
		if (flag)
		{
			TimeSpan timeSpan = NKCGuildManager.MyGuildData.latestRenameDate.AddDays(NKMCommonConst.Guild.ConsortiumNameChangeLimitDay) - ServiceTime.Recent;
			NKCUtil.SetGameobjectActive(m_lbNameChangeRemain, timeSpan.Ticks > 0);
			m_btnNameChange?.SetLock(timeSpan.Ticks > 0);
			if (timeSpan.Ticks > 0)
			{
				NKCUtil.SetLabelText(m_lbNameChangeRemain, string.Format(NKCStringTable.GetString("SI_PF_CONSORTIUM_NAME_CHANGE_LIMITDAY"), timeSpan.Days + 1));
			}
		}
	}

	private void SetRemainTime()
	{
		if (NKCGuildManager.MyGuildData == null)
		{
			return;
		}
		if (NKCGuildManager.MyGuildData.guildState == GuildState.Closing)
		{
			TimeSpan timeSpan = m_tExpireTime - ServiceTime.Recent;
			if (timeSpan.TotalSeconds > 1.0)
			{
				NKCUtil.SetLabelText(m_lbGuildBreakupTime, string.Format(NKCUtilString.GET_STRING_SHOP_CHAIN_NEXT_RESET_ONE_PARAM_CLOSE, NKCUtilString.GetRemainTimeString(timeSpan, 2)));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbGuildBreakupTime, NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_EX_END_SOON"));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objGuildBreakupTime, bValue: false);
		}
	}

	private void OnSelectDirect(bool bValue)
	{
		if (bValue)
		{
			m_JoinType = GuildJoinType.DirectJoin;
			SetButton();
		}
	}

	private void OnSelectApproval(bool bValue)
	{
		if (bValue)
		{
			m_JoinType = GuildJoinType.NeedApproval;
			SetButton();
		}
	}

	private void OnSelectClosed(bool bValue)
	{
		if (bValue)
		{
			m_JoinType = GuildJoinType.Closed;
			SetButton();
		}
	}

	private void SetButton()
	{
		if (m_JoinType != GuildJoinType.NeedApproval && NKCGuildManager.MyGuildData.joinWaitingList.Count > 0)
		{
			m_btnOk.Lock();
			NKCUtil.SetGameobjectActive(m_objJoinTypeError, bValue: true);
		}
		else
		{
			m_btnOk.UnLock();
			NKCUtil.SetGameobjectActive(m_objJoinTypeError, bValue: false);
		}
	}

	private void OnSelectGuildNotice(bool bValue)
	{
		if (bValue)
		{
			m_NoticeType = GuildChatNoticeType.Normal;
		}
	}

	private void OnSelectCoopNotice(bool bValue)
	{
		if (bValue)
		{
			m_NoticeType = GuildChatNoticeType.Dungeon;
		}
	}

	private void OnGreetingChanged(string str)
	{
		if (!string.Equals(str, NKCGuildManager.MyGuildData.greeting))
		{
			m_Greeting = NKCFilterManager.CheckBadChat(str);
			m_inputDesc.text = m_Greeting;
		}
	}

	private void OnClickBadgeRandom()
	{
		int frameId = UnityEngine.Random.Range(1, NKMTempletContainer<NKMGuildBadgeFrameTemplet>.Values.Count());
		int frameColorId = UnityEngine.Random.Range(1, NKMTempletContainer<NKMGuildBadgeColorTemplet>.Values.Count());
		int markId = UnityEngine.Random.Range(1, NKMTempletContainer<NKMGuildBadgeMarkTemplet>.Values.Count());
		int markColorId = UnityEngine.Random.Range(1, NKMTempletContainer<NKMGuildBadgeColorTemplet>.Values.Count());
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(frameId.ToString("D3"));
		stringBuilder.Append(frameColorId.ToString("D3"));
		stringBuilder.Append(markId.ToString("D3"));
		stringBuilder.Append(markColorId.ToString("D3"));
		m_BadgeId = long.Parse(stringBuilder.ToString());
		m_badgeUI.SetData(frameId, frameColorId, markId, markColorId);
	}

	private void OnClickBadgeSetting()
	{
		if (!NKCPopupGuildBadgeSetting.IsInstanceOpen)
		{
			NKCPopupGuildBadgeSetting.Instance.Open(OnCloseSetting, m_BadgeId);
		}
	}

	private void OnCloseSetting(long badgeId)
	{
		m_BadgeId = badgeId;
		m_badgeUI.SetData(m_BadgeId);
	}

	private void OnClickGuildClose()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_POPUP_BODY_DESC, delegate
		{
			NKCPacketSender.Send_NKMPacket_GUILD_CLOSE_REQ(NKCGuildManager.MyData.guildUid);
		});
	}

	private void OnClickGuildCloseCancel()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DISMANTLE_CANCEL_POPUP_BODY_DESC, delegate
		{
			NKCPacketSender.Send_NKMPacket_GUILD_CLOSE_CANCEL_REQ(NKCGuildManager.MyData.guildUid);
		});
	}

	private void OnClickOk()
	{
		if (m_JoinType != NKCGuildManager.MyGuildData.guildJoinType || m_BadgeId != NKCGuildManager.MyGuildData.badgeId || m_NoticeType != NKCGuildManager.MyGuildData.chatNoticeType || !string.Equals(m_Greeting, NKCGuildManager.MyGuildData.greeting))
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DATA_SAVE_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DATA_SAVE_POPUP_BODY_DESC, OnCinformOK);
		}
		else
		{
			Close();
		}
	}

	private void OnCinformOK()
	{
		NKCPacketSender.Send_NKMPacket_GUILD_UPDATE_DATA_REQ(NKCGuildManager.MyData.guildUid, m_BadgeId, m_inputDesc.text, m_JoinType, m_NoticeType);
		Close();
	}

	private void OnClickCancel()
	{
		if (m_JoinType != NKCGuildManager.MyGuildData.guildJoinType || m_BadgeId != NKCGuildManager.MyGuildData.badgeId || m_NoticeType != NKCGuildManager.MyGuildData.chatNoticeType || !string.Equals(m_Greeting, NKCGuildManager.MyGuildData.greeting))
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DATA_SAVE_CANCEL_POPUP_TITLE_DESC, NKCUtilString.GET_STRING_CONSORTIUM_OPTION_DATA_SAVE_CANCEL_POPUP_BODY_DESC, Close);
		}
		else
		{
			Close();
		}
	}

	private void OnClickNameChange()
	{
		NKCPopupGuildNameChange.Instance.Open();
	}

	private void Close()
	{
		m_dOnClose?.Invoke(NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Info);
	}

	private void Update()
	{
		if (m_objGuildBreakupTime.activeSelf)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRemainTime();
			}
		}
	}
}
