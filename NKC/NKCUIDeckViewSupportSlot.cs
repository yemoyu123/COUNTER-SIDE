using System;
using ClientPacket.Community;
using ClientPacket.Guild;
using NKC.UI;
using NKC.UI.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDeckViewSupportSlot : MonoBehaviour
{
	public delegate void OnSelect(long supCode);

	public NKCUIComStateButton m_btn;

	public NKCUISlotProfile m_slot;

	public Text m_txtLevel;

	public Text m_txtName;

	public Text m_txtUID;

	public Text m_txtLastConnectTime;

	public Text m_txtPower;

	public Text m_txtCoolTime;

	public GameObject m_objCoolTime;

	public GameObject m_objSelect;

	public GameObject m_objGuest;

	public GameObject m_objGuildLabel;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_GuildBadgeUI;

	public Text m_lbGuildName;

	private DateTime m_lastUsedTime;

	private float m_time;

	private OnSelect dOnSelect;

	public long SuppoterCode { get; private set; }

	public void Init()
	{
		m_slot.Init();
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(OnSelectButton);
	}

	public void SetData(WarfareSupporterListData data, bool bGuest, OnSelect onSelect)
	{
		SuppoterCode = data.commonProfile.friendCode;
		dOnSelect = onSelect;
		m_slot.SetProfiledata(data.commonProfile, delegate
		{
			OnSelectButton();
		});
		NKCUtil.SetLabelText(m_txtLevel, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, data.commonProfile.level);
		NKCUtil.SetLabelText(m_txtName, data.commonProfile.nickname);
		NKCUtil.SetLabelText(m_txtUID, NKCUtilString.GetFriendCode(data.commonProfile.friendCode));
		NKCUtil.SetLabelText(m_txtLastConnectTime, NKCUtilString.GetLastTimeString(data.lastLoginDate));
		NKCUtil.SetLabelText(m_txtPower, data.deckData.CalculateOperationPower().ToString());
		SetGuildData(data);
		m_lastUsedTime = data.lastUsedDate;
		SetCooltime();
		NKCUtil.SetGameobjectActive(m_objGuest, bGuest);
		if (bGuest && NKCGuildManager.HasGuild() && NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == data.commonProfile.userUid) != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuest, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objGuildLabel, bValue: false);
	}

	private void SetGuildData(WarfareSupporterListData data)
	{
		if (!(m_objGuild != null))
		{
			return;
		}
		if (data.guildData != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, data.guildData.guildUid > 0);
			if (m_objGuild.activeSelf)
			{
				m_GuildBadgeUI.SetData(data.guildData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, data.guildData.guildName);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objGuild, bValue: false);
		}
	}

	public bool IsCooltime()
	{
		return m_objCoolTime.activeSelf;
	}

	public void SelectUI(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objSelect, bSet);
	}

	private void OnSelectButton()
	{
		dOnSelect?.Invoke(SuppoterCode);
	}

	private void Update()
	{
		if (m_objCoolTime.activeSelf)
		{
			m_time += Time.deltaTime;
			if (m_time > 1f)
			{
				SetCooltime();
				m_time -= 1f;
			}
		}
	}

	private void SetCooltime()
	{
		TimeSpan timeSpan = NKCSynchronizedTime.GetServerUTCTime() - m_lastUsedTime;
		TimeSpan timeSpan2 = TimeSpan.FromHours(12.0) - timeSpan;
		NKCUtil.SetLabelText(m_txtCoolTime, NKCUtilString.GetTimeSpanString(timeSpan2));
		NKCUtil.SetGameobjectActive(m_objCoolTime, timeSpan2.TotalSeconds > 0.0);
	}
}
