using Cs.Protocol;

namespace NKM;

public class NKMCounterCaseData : ISerializable
{
	public int m_DungeonID;

	public bool m_Unlocked;

	public NKMCounterCaseData()
	{
	}

	public NKMCounterCaseData(int dungeonID, bool unlocked)
	{
		m_DungeonID = dungeonID;
		m_Unlocked = unlocked;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_DungeonID);
		stream.PutOrGet(ref m_Unlocked);
	}
}
