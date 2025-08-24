using Cs.Protocol;

namespace NKM;

public class NKMTeamCollectionData : ISerializable
{
	private int m_TeamID;

	private bool m_bReward;

	public int TeamID => m_TeamID;

	public bool Reward => m_bReward;

	public NKMTeamCollectionData()
	{
	}

	public NKMTeamCollectionData(int teamID, bool reward)
	{
		m_TeamID = teamID;
		m_bReward = reward;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_TeamID);
		stream.PutOrGet(ref m_bReward);
	}

	public void GiveReward()
	{
		m_bReward = true;
	}

	public bool IsRewardComplete()
	{
		return m_bReward;
	}
}
