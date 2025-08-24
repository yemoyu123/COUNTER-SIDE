using System.Collections.Generic;
using ClientPacket.Guild;
using NKC.UI.Component;
using NKM;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopStatusSlot : MonoBehaviour
{
	private const string ICON_ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP_Sprite";

	public NKCUIComStateButton m_btnSlot;

	public Text m_lbRank;

	public NKCUISlotProfile m_slotLeader;

	public GameObject m_objGuildMaster;

	public Text m_lbLevel;

	public Text m_lbName;

	public List<GuildCoopHistory> m_lstHistory = new List<GuildCoopHistory>();

	public Text m_lbPoint;

	public NKCUIComTitlePanel m_titlePanel;

	private long m_UserUid;

	public void InitUI()
	{
		m_btnSlot.PointerClick.RemoveAllListeners();
		m_btnSlot.PointerClick.AddListener(OnClickSlot);
		m_slotLeader.Init();
	}

	public void SetData(GuildDungeonMemberInfo memberInfo, int rank)
	{
		m_UserUid = memberInfo.profile.userUid;
		NKCUtil.SetLabelText(m_lbRank, rank.ToString());
		m_slotLeader.SetProfiledata(memberInfo.profile, null);
		m_titlePanel?.SetData(memberInfo.profile);
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, memberInfo.profile.level));
		NKCUtil.SetLabelText(m_lbName, memberInfo.profile.nickname);
		NKCUtil.SetLabelText(m_lbPoint, memberInfo.bossPoint.ToString("N0"));
		NKCUtil.SetGameobjectActive(m_objGuildMaster, NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == m_UserUid).grade == GuildMemberGrade.Master);
		for (int num = 0; num < m_lstHistory.Count; num++)
		{
			if (memberInfo.arenaList.Count <= num)
			{
				NKCUtil.SetGameobjectActive(m_lstHistory[num].m_objParent, num < NKMCommonConst.GuildDungeonConstTemplet.ArenaPlayCountBasic);
				NKCUtil.SetImageSprite(m_lstHistory[num].m_ImgIcon, GetMissionStatusIcon(-1));
				NKCUtil.SetLabelText(m_lstHistory[num].m_lbArenaName, string.Empty);
			}
			else if (NKCGuildCoopManager.m_dicGuildDungeonInfoTemplet.ContainsKey(memberInfo.arenaList[num].arenaId))
			{
				GuildDungeonInfoTemplet guildDungeonInfoTemplet = NKCGuildCoopManager.m_dicGuildDungeonInfoTemplet[memberInfo.arenaList[num].arenaId];
				if (guildDungeonInfoTemplet != null)
				{
					NKMDungeonManager.GetDungeonTempletBase(guildDungeonInfoTemplet.GetSeasonDungeonId());
					NKCUtil.SetImageSprite(m_lstHistory[num].m_ImgIcon, GetMissionStatusIcon(memberInfo.arenaList[num].grade));
					NKCUtil.SetLabelText(m_lstHistory[num].m_lbArenaName, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_DUNGEON_UI_ARENA_INFO, memberInfo.arenaList[num].arenaId));
					NKCUtil.SetGameobjectActive(m_lstHistory[num].m_objParent, memberInfo.arenaList.Count > num);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstHistory[num].m_objParent, bValue: false);
			}
		}
	}

	private Sprite GetMissionStatusIcon(int grade)
	{
		return grade switch
		{
			3 => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_COOP_Sprite", "NKM_UI_CONSORTIUM_COOP_MEDAL_ICON_01"), 
			2 => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_COOP_Sprite", "NKM_UI_CONSORTIUM_COOP_MEDAL_ICON_02"), 
			1 => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_COOP_Sprite", "NKM_UI_CONSORTIUM_COOP_MEDAL_ICON_03"), 
			0 => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_COOP_Sprite", "NKM_UI_CONSORTIUM_COOP_MEDAL_ICON_FAIL"), 
			-1 => NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_COOP_Sprite", "NKM_UI_CONSORTIUM_COOP_MEDAL_ICON_NON"), 
			_ => null, 
		};
	}

	public void OnClickSlot()
	{
		NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(m_UserUid, NKM_DECK_TYPE.NDT_NORMAL);
	}
}
