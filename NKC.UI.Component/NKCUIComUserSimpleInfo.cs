using ClientPacket.Common;
using NKC.UI.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIComUserSimpleInfo : MonoBehaviour
{
	[Header("기본정보")]
	public Text m_lbLevel;

	public Text m_lbName;

	public Text m_lbUID;

	[Header("프로필 슬롯")]
	public NKCUISlotProfile m_Slot;

	[Header("컨소시움")]
	public NKCUIGuildBadge m_GuildBadge;

	public GameObject m_objGuildName;

	public Text m_lbGuildName;

	private bool m_bInit;

	private void Init()
	{
		m_bInit = true;
		if (m_Slot != null)
		{
			m_Slot.Init();
		}
	}

	public void SetData(NKMUserProfileData userProfileData)
	{
		if (userProfileData != null)
		{
			SetData(userProfileData.commonProfile, userProfileData.guildData);
		}
	}

	public void SetData(NKMCommonProfile profile, NKMGuildSimpleData guildData)
	{
		if (!m_bInit)
		{
			Init();
		}
		if (profile != null)
		{
			NKCUtil.SetLabelText(m_lbName, profile.nickname);
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, profile.level));
			NKCUtil.SetLabelText(m_lbUID, NKCUtilString.GetFriendCode(profile.friendCode));
			if (m_Slot != null)
			{
				NKCUtil.SetGameobjectActive(m_Slot, bValue: true);
				m_Slot.SetProfiledata(profile, null);
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbName, "");
			NKCUtil.SetLabelText(m_lbLevel, "");
			NKCUtil.SetLabelText(m_lbUID, "");
			NKCUtil.SetGameobjectActive(m_Slot, bValue: false);
		}
		if (guildData != null && guildData.guildUid > 0)
		{
			NKCUtil.SetGameobjectActive(m_GuildBadge, bValue: true);
			NKCUtil.SetGameobjectActive(m_objGuildName, bValue: true);
			if (m_GuildBadge != null)
			{
				m_GuildBadge.SetData(guildData.badgeId);
			}
			NKCUtil.SetLabelText(m_lbGuildName, guildData.guildName);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_GuildBadge, bValue: false);
			NKCUtil.SetGameobjectActive(m_objGuildName, bValue: false);
		}
	}
}
