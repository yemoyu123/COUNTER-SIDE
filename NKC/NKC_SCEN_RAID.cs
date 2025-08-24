using ClientPacket.Game;
using ClientPacket.Raid;
using NKC.UI;
using NKM;

namespace NKC;

public class NKC_SCEN_RAID : NKC_SCEN_BASIC
{
	private NKCAssetResourceData m_UILoadResourceData;

	private NKCUIRaid m_NKCUIRaid;

	private long m_RaidUID;

	private bool m_FromEventListPopup;

	public bool FromEventListPopup => m_FromEventListPopup;

	public NKC_SCEN_RAID()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_RAID;
	}

	public void ResetUI()
	{
		if (m_NKCUIRaid != null)
		{
			m_NKCUIRaid.SetUI();
		}
	}

	public void SetRaidUID(long raidUID)
	{
		m_RaidUID = raidUID;
	}

	public void SetFromEventListPopup()
	{
		m_FromEventListPopup = true;
	}

	public void ClearCacheData()
	{
		if (m_NKCUIRaid != null)
		{
			m_NKCUIRaid.CloseInstance();
			m_NKCUIRaid = null;
		}
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (m_NKCUIRaid == null)
		{
			m_UILoadResourceData = NKCUIRaid.OpenInstanceAsync();
		}
		else
		{
			m_UILoadResourceData = null;
		}
	}

	public override void ScenLoadUpdate()
	{
		if (!NKCAssetResourceManager.IsLoadEnd())
		{
			return;
		}
		if (m_NKCUIRaid == null && m_UILoadResourceData != null)
		{
			if (!NKCUIRaid.CheckInstanceLoaded(m_UILoadResourceData, out m_NKCUIRaid))
			{
				return;
			}
			m_UILoadResourceData = null;
		}
		ScenLoadLastStart();
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		if (m_NKCUIRaid != null)
		{
			m_NKCUIRaid.InitUI();
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCCamera.EnableBloom(bEnable: false);
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (nKMRaidDetailData != null && NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate))
		{
			if (m_NKCUIRaid != null)
			{
				m_NKCUIRaid.Open(m_RaidUID, m_FromEventListPopup);
			}
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_RAID_DETAIL_INFO_REQ(m_RaidUID);
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_NKCUIRaid != null)
		{
			m_NKCUIRaid.Close();
		}
		m_FromEventListPopup = false;
		ClearCacheData();
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void OnRecv(NKMPacket_RAID_DETAIL_INFO_ACK cNKMPacket_RAID_DETAIL_INFO_ACK)
	{
		m_RaidUID = cNKMPacket_RAID_DETAIL_INFO_ACK.raidDetailData.raidUID;
		if (m_NKCUIRaid != null)
		{
			m_NKCUIRaid.Open(m_RaidUID, m_FromEventListPopup);
		}
	}

	public void OnRecv(NKMPacket_RAID_SWEEP_ACK cNKMPacket_RAID_SWEEP_ACK)
	{
		m_RaidUID = cNKMPacket_RAID_SWEEP_ACK.raidUid;
		if (m_NKCUIRaid != null)
		{
			m_NKCUIRaid.Open(m_RaidUID, m_FromEventListPopup);
		}
	}
}
