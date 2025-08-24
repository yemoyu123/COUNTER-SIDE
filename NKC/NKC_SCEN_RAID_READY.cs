using System.Collections.Generic;
using ClientPacket.Guild;
using ClientPacket.Raid;
using NKC.UI;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKC_SCEN_RAID_READY : NKC_SCEN_BASIC
{
	private long m_RaidUID;

	private NKMDeckIndex m_LastDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_RAID, 0);

	private bool m_bIsGuildRaid;

	private int m_PacketWaitingCount;

	public NKC_SCEN_RAID_READY()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_RAID_READY;
	}

	public void SetRaidUID(long raidUID)
	{
		m_RaidUID = raidUID;
	}

	public void SetGuildRaid(bool bGuildRaid)
	{
		m_bIsGuildRaid = bGuildRaid;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
	}

	public override void ScenDataReq()
	{
		if (m_bIsGuildRaid && NKCGuildCoopManager.m_GuildDungeonState == GuildDungeonState.Invalid && NKCGuildManager.GetMyGuildSimpleData() != null)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_INFO_REQ(NKCGuildManager.GetMyGuildSimpleData().guildUid);
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_MEMBER_INFO_REQ(NKCGuildManager.GetMyGuildSimpleData().guildUid);
			m_PacketWaitingCount = 2;
		}
		base.ScenDataReq();
	}

	public override void ScenDataReqWaitUpdate()
	{
		base.ScenDataReqWaitUpdate();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCCamera.EnableBloom(bEnable: false);
		NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
		{
			MenuName = NKCUtilString.GET_STRING_RAID
		};
		if (m_RaidUID == 0L)
		{
			options.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.DeckSetupOnly;
			options.dOnBackButton = delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
			};
		}
		else if (m_bIsGuildRaid)
		{
			options.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.GuildCoopBoss;
			options.dOnBackButton = delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_COOP);
			};
		}
		else
		{
			options.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.PrepareRaid;
			options.dOnBackButton = delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID);
			};
		}
		options.dOnSideMenuButtonConfirm = null;
		if (NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(m_LastDeckIndex) == null)
		{
			m_LastDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_RAID, 0);
		}
		options.DeckIndex = m_LastDeckIndex;
		options.SelectLeaderUnitOnOpen = true;
		options.bEnableDefaultBackground = true;
		options.bUpsideMenuHomeButton = false;
		options.raidUID = m_RaidUID;
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (nKMRaidDetailData != null)
		{
			NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
			if (nKMRaidTemplet.StageReqItemID == 1)
			{
				options.upsideMenuShowResourceList = new List<int> { 1, 101 };
			}
			else
			{
				options.upsideMenuShowResourceList = new List<int> { nKMRaidTemplet.StageReqItemID, 101 };
			}
		}
		else
		{
			options.upsideMenuShowResourceList = new List<int> { 3, 101 };
		}
		options.StageBattleStrID = string.Empty;
		options.bSlot24Extend = true;
		options.bNoUseLeaderBtn = true;
		NKCUIDeckViewer.Instance.Open(options);
		CheckTutorial();
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		NKCUIDeckViewer.CheckInstanceAndClose();
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void OnRecv(NKMPacket_GUILD_DUNGEON_INFO_ACK sPacket)
	{
		m_PacketWaitingCount--;
		if (m_PacketWaitingCount == 0)
		{
			ScenDataReqWaitUpdate();
		}
	}

	public void OnRecv(NKMPacket_GUILD_DUNGEON_MEMBER_INFO_ACK sPacket)
	{
		m_PacketWaitingCount--;
		if (m_PacketWaitingCount == 0)
		{
			ScenDataReqWaitUpdate();
		}
	}

	public void SetLastDeckIndex(NKMDeckIndex deckIndex)
	{
		m_LastDeckIndex = deckIndex;
	}

	public void DoAfterLogout()
	{
		m_LastDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_RAID, 0);
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.RaidStart);
	}
}
