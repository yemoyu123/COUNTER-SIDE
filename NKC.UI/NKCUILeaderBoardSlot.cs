using ClientPacket.Common;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using NKM.Guild;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILeaderBoardSlot : MonoBehaviour
{
	public delegate void OnDragBegin();

	[Header("공용")]
	public Image m_imgMyRankIcon;

	public Text m_lbRank;

	public Image m_imgPoint;

	public Text m_lbPoint;

	public NKCUIComStateButton m_btn;

	public GameObject m_objMyRank;

	public Text m_lbLevel;

	public Text m_lbName;

	[Header("유닛슬롯 전용")]
	public Text m_lbFriendCode;

	public NKCUISlotProfile m_mainUnitSlot;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_BadgeUI;

	public Text m_lbGuildName;

	public NKCUIComTitlePanel m_TitlePanel;

	[Space]
	public GameObject m_objUnknownUser;

	[Header("길드슬롯 전용")]
	public NKCUIGuildBadge m_guildBadge;

	public GameObject m_objMasterName;

	public Text m_lbMasterName;

	public GameObject m_objMemberCount;

	public Text m_lbMemberCount;

	[Header("국가 태그")]
	public GameObject m_objCountryTag;

	public GameObject m_objKorea;

	public GameObject m_objGlobal;

	private LeaderBoardSlotData m_slotData;

	private OnDragBegin m_dOnDragBegin;

	public void InitUI()
	{
		if (m_btn != null)
		{
			m_btn.PointerClick.RemoveAllListeners();
			m_btn.PointerClick.AddListener(OnClick);
			m_btn.PointerDown.RemoveAllListeners();
			m_btn.PointerDown.AddListener(delegate
			{
				OnDragBeginImpl();
			});
		}
		if (m_mainUnitSlot != null)
		{
			m_mainUnitSlot.Init();
		}
	}

	public void SetData(LeaderBoardSlotData data, int boardCriteria, OnDragBegin onDragBegin, bool bUsePercentRank = false, bool bShowMyRankIcon = true)
	{
		m_slotData = data;
		m_dOnDragBegin = onDragBegin;
		NKCUtil.SetGameobjectActive(m_mainUnitSlot, !data.bIsGuild);
		NKCUtil.SetGameobjectActive(m_objGuild, !data.bIsGuild);
		if (m_objGuild != null && m_objGuild.activeSelf)
		{
			SetUserSlotGuildData();
		}
		NKCUtil.SetGameobjectActive(m_guildBadge, data.bIsGuild);
		NKCUtil.SetGameobjectActive(m_objMasterName, data.bIsGuild);
		NKCUtil.SetGameobjectActive(m_objMemberCount, data.bIsGuild);
		if (data.rank > 0)
		{
			if (!bUsePercentRank)
			{
				NKCUtil.SetLabelText(m_lbRank, data.rank.ToString());
			}
			else
			{
				NKCUtil.SetLabelText(m_lbRank, $"{data.rank}%");
			}
			if (data.rank <= 3 && !bUsePercentRank)
			{
				NKCUtil.SetGameobjectActive(m_imgMyRankIcon, bValue: true);
				NKCUtil.SetImageSprite(m_imgMyRankIcon, NKCUtil.GetRankIcon(data.rank));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_imgMyRankIcon, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRank, "-");
			NKCUtil.SetGameobjectActive(m_imgMyRankIcon, bValue: false);
		}
		if (data.level > 0)
		{
			NKCUtil.SetGameobjectActive(m_lbLevel, bValue: true);
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, data.level));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbLevel, bValue: false);
		}
		NKCUtil.SetLabelText(m_lbPoint, data.score);
		NKCUtil.SetImageSprite(m_imgPoint, NKCUtil.GetLeaderBoardPointIcon(data.boardType, boardCriteria), bDisableIfSpriteNull: true);
		NKCUtil.SetGameobjectActive(m_objUnknownUser, string.Equals(data.nickname, string.Empty) && data.friendCode == 0);
		if (!data.bIsGuild)
		{
			if (data.Profile == null || data.Profile.userUid == 0L)
			{
				SetEmptySlot();
			}
			else
			{
				NKCUtil.SetLabelText(m_lbName, data.nickname);
				NKCUtil.SetLabelText(m_lbFriendCode, NKCUtilString.GetFriendCode(data.Profile.friendCode));
				NKCUtil.SetGameobjectActive(m_objMyRank, bShowMyRankIcon && data.Profile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
				m_mainUnitSlot?.SetProfiledata(data.Profile, null);
				m_TitlePanel?.SetData(data.Profile);
			}
		}
		else
		{
			m_guildBadge?.SetData(data.GuildData.badgeId);
			m_TitlePanel?.SetData(0);
			if (data.GuildData.guildUid == 0L)
			{
				SetEmptySlot();
			}
			else
			{
				NKCUtil.SetLabelText(m_lbName, data.GuildData.guildName);
				NKCUtil.SetLabelText(m_lbFriendCode, "");
				NKCUtil.SetGameobjectActive(m_objMyRank, bShowMyRankIcon && NKCGuildManager.HasGuild() && data.GuildData.guildUid == NKCGuildManager.MyGuildData.guildUid);
				NKCUtil.SetLabelText(m_lbMasterName, data.Profile.nickname);
				NKCUtil.SetLabelText(m_lbMemberCount, $"{data.memberCount}/{NKMTempletContainer<GuildExpTemplet>.Find(data.level).MaxMemberCount}");
			}
		}
		NKCUtil.SetGameobjectActive(m_objCountryTag, m_slotData.CountryCode != NKMTournamentCountryCode.None);
		NKCUtil.SetGameobjectActive(m_objKorea, m_slotData.CountryCode == NKMTournamentCountryCode.KR);
		NKCUtil.SetGameobjectActive(m_objGlobal, m_slotData.CountryCode == NKMTournamentCountryCode.GL);
	}

	public void SetEmptySlot()
	{
		NKCUtil.SetLabelText(m_lbRank, "-");
		NKCUtil.SetLabelText(m_lbPoint, "-");
		NKCUtil.SetGameobjectActive(m_lbLevel, bValue: false);
		NKCUtil.SetLabelText(m_lbMasterName, "-");
		NKCUtil.SetLabelText(m_lbMemberCount, "-");
		NKCUtil.SetLabelText(m_lbName, "-");
		NKCUtil.SetLabelText(m_lbFriendCode, "");
		NKCUtil.SetGameobjectActive(m_objMyRank, bValue: false);
		NKCUtil.SetGameobjectActive(m_objGuild, bValue: false);
		NKCUtil.SetGameobjectActive(m_mainUnitSlot, bValue: true);
		m_mainUnitSlot.SetProfiledata(0, 0, 0, null);
		NKCUtil.SetGameobjectActive(m_guildBadge, bValue: false);
		m_TitlePanel?.SetData(0);
		NKCUtil.SetGameobjectActive(m_objCountryTag, bValue: false);
	}

	private void SetUserSlotGuildData()
	{
		NKMGuildSimpleData guildData = m_slotData.GuildData;
		if (m_objGuild != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, guildData != null && guildData.guildUid > 0);
			if (m_objGuild.activeSelf)
			{
				m_BadgeUI.SetData(guildData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, guildData.guildName);
			}
		}
	}

	public void OnClick()
	{
		if (m_slotData == null)
		{
			return;
		}
		LeaderBoardType boardType = m_slotData.boardType;
		if (boardType != LeaderBoardType.BT_ACHIEVE && (uint)(boardType - 4) > 7u)
		{
			return;
		}
		if (!m_slotData.bIsGuild)
		{
			if (m_slotData.userUid != 0L)
			{
				switch (m_slotData.boardType)
				{
				case LeaderBoardType.BT_FIERCE:
					NKCPacketSender.Send_NKMPacket_FIERCE_PROFILE_REQ(m_slotData.userUid, bForce: true);
					break;
				case LeaderBoardType.BT_DEFENCE:
					NKCPacketSender.Send_NKMPacket_DEFENCE_PROFILE_REQ(m_slotData.userUid, isForce: true);
					break;
				default:
					NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(m_slotData.userUid, NKM_DECK_TYPE.NDT_NORMAL);
					break;
				case LeaderBoardType.BT_TOURNAMENT:
					break;
				}
			}
		}
		else if (m_slotData.GuildData.guildUid != 0L)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DATA_REQ(m_slotData.GuildData.guildUid);
		}
	}

	private void OnDragBeginImpl()
	{
		if (m_slotData != null && !m_slotData.bIsGuild)
		{
			m_dOnDragBegin?.Invoke();
		}
	}
}
