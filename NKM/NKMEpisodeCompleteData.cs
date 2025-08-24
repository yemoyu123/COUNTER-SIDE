using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public class NKMEpisodeCompleteData : ISerializable
{
	public int m_EpisodeID;

	public EPISODE_DIFFICULTY m_EpisodeDifficulty;

	public int m_EpisodeCompleteCount;

	public bool[] m_bRewards = new bool[3];

	public void DeepCopyFromSource(NKMEpisodeCompleteData source)
	{
		m_EpisodeID = source.m_EpisodeID;
		m_EpisodeDifficulty = source.m_EpisodeDifficulty;
		m_EpisodeCompleteCount = source.m_EpisodeCompleteCount;
		for (int i = 0; i < 3; i++)
		{
			m_bRewards[i] = source.m_bRewards[i];
		}
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_EpisodeID);
		stream.PutOrGetEnum(ref m_EpisodeDifficulty);
		stream.PutOrGet(ref m_EpisodeCompleteCount);
		stream.PutOrGet(ref m_bRewards);
	}
}
