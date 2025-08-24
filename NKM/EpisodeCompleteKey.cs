namespace NKM;

public readonly struct EpisodeCompleteKey
{
	public readonly long m_EpisodeKey;

	public int EpisodeID => (int)(m_EpisodeKey >> 32);

	public int EpisodeDifficulty => (int)(m_EpisodeKey & 0xFFFFFFFFu);

	public EpisodeCompleteKey(int episodeID, int episodeDifficulty)
	{
		long num = episodeID;
		num <<= 32;
		num |= (uint)episodeDifficulty;
		m_EpisodeKey = num;
	}
}
