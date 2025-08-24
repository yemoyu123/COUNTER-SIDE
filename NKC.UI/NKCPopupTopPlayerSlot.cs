using ClientPacket.Common;
using NKC.UI.Guild;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupTopPlayerSlot : MonoBehaviour
{
	public NKCUIComStateButton m_btn;

	public NKCUISlotProfile m_slotProfile;

	public Text m_lbLevel;

	public Text m_lbName;

	public Text m_lbFriendCode;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_GuildBadge;

	public Text m_lbGuildName;

	public Text m_lbScore;

	public Text m_lbRank;

	[Header("레이드 전용")]
	public Text m_lbRaidTryCount;

	private long m_UserUid;

	public void SetData(NKMCommonProfile commonProfileData, NKMGuildSimpleData guildData, string score, int raidTryCount, int raidTryMaxCount, int rank)
	{
		if (m_btn != null)
		{
			m_btn.PointerClick.RemoveAllListeners();
			m_btn.PointerClick.AddListener(delegate
			{
				OnClickProfile(null, 0);
			});
		}
		if (m_slotProfile != null)
		{
			NKCUtil.SetGameobjectActive(m_slotProfile, bValue: true);
			m_slotProfile?.SetProfiledata(commonProfileData, null);
		}
		m_UserUid = commonProfileData.userUid;
		NKCUtil.SetGameobjectActive(m_lbLevel, bValue: true);
		NKCUtil.SetGameobjectActive(m_lbLevel, commonProfileData.level > 0);
		if (commonProfileData.level > 0)
		{
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, commonProfileData.level));
		}
		NKCUtil.SetLabelText(m_lbName, commonProfileData.nickname);
		NKCUtil.SetLabelText(m_lbFriendCode, $"#{commonProfileData.friendCode}");
		if (rank > 0)
		{
			NKCUtil.SetLabelText(m_lbRank, rank.ToString());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRank, "");
		}
		if (guildData != null && guildData.guildUid > 0)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, bValue: true);
			m_GuildBadge?.SetData(guildData.badgeId);
			NKCUtil.SetLabelText(m_lbGuildName, guildData.guildName);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objGuild, bValue: false);
		}
		if (raidTryCount > 0)
		{
			NKCUtil.SetGameobjectActive(m_lbRaidTryCount, bValue: true);
			NKCUtil.SetLabelText(m_lbRaidTryCount, $"{raidTryCount}/{raidTryMaxCount}");
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbRaidTryCount, bValue: false);
		}
		NKCUtil.SetLabelText(m_lbScore, score);
	}

	public void SetEmpty()
	{
		if (m_btn != null)
		{
			m_btn.PointerClick.RemoveAllListeners();
		}
		if (m_slotProfile != null)
		{
			NKCUtil.SetGameobjectActive(m_slotProfile, bValue: true);
			m_slotProfile.SetProfiledata(0, 0, 0, null);
		}
		m_UserUid = 0L;
		NKCUtil.SetGameobjectActive(m_lbLevel, bValue: false);
		NKCUtil.SetLabelText(m_lbLevel, "-");
		NKCUtil.SetLabelText(m_lbName, "-");
		NKCUtil.SetLabelText(m_lbFriendCode, "");
		NKCUtil.SetGameobjectActive(m_lbRaidTryCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objGuild, bValue: false);
		NKCUtil.SetLabelText(m_lbScore, "-");
	}

	public void OnClickProfile(NKCUISlotProfile slot, int frameID)
	{
		NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(m_UserUid, NKM_DECK_TYPE.NDT_NORMAL);
	}
}
