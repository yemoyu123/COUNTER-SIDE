using ClientPacket.Event;
using NKM.EventPass;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKCEventPassDataManager
{
	private int m_iEventPassId;

	private int m_iTotalExp;

	private int m_iNormalRewardLevel;

	private int m_iCoreRewardLevel;

	private bool m_bCorePassPurChased;

	private bool m_bEventPassDataReceived;

	public int EventPassId
	{
		get
		{
			return m_iEventPassId;
		}
		set
		{
			m_iEventPassId = value;
		}
	}

	public int TotalExp
	{
		get
		{
			return m_iTotalExp;
		}
		set
		{
			m_iTotalExp = value;
		}
	}

	public int NormalRewardLevel
	{
		get
		{
			return m_iNormalRewardLevel;
		}
		set
		{
			m_iNormalRewardLevel = value;
		}
	}

	public int CoreRewardLevel
	{
		get
		{
			return m_iCoreRewardLevel;
		}
		set
		{
			m_iCoreRewardLevel = value;
		}
	}

	public bool CorePassPurchased
	{
		get
		{
			return m_bCorePassPurChased;
		}
		set
		{
			m_bCorePassPurChased = value;
		}
	}

	public bool EventPassDataReceived
	{
		get
		{
			return m_bEventPassDataReceived;
		}
		set
		{
			m_bEventPassDataReceived = value;
		}
	}

	public NKCEventPassDataManager()
	{
		m_iEventPassId = 0;
		m_bEventPassDataReceived = false;
	}

	public void SetEventPassData(NKMPacket_EVENT_PASS_ACK eventPassAck)
	{
		m_iTotalExp = eventPassAck.totalExp;
		m_iNormalRewardLevel = eventPassAck.rewardNormalLevel;
		m_iCoreRewardLevel = eventPassAck.rewardCoreLevel;
		m_bCorePassPurChased = eventPassAck.isCorePassPurchased;
		m_bEventPassDataReceived = true;
	}

	public int GetPassLevel()
	{
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(m_iEventPassId);
		if (!m_bEventPassDataReceived || nKMEventPassTemplet == null)
		{
			return 0;
		}
		return Mathf.Min(nKMEventPassTemplet.PassMaxExp, m_iTotalExp / nKMEventPassTemplet.PassLevelUpExp + 1);
	}
}
