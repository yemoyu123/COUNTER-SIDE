using Cs.Protocol;

namespace NKM;

public class PvpState : ISerializable
{
	private static int m_sPrevScore;

	public int SeasonID;

	public int WeekID;

	public int WinCount;

	public int LoseCount;

	public int LeagueTierID;

	public int MaxLeagueTierID;

	public int Score;

	public int MaxScore;

	public int WinStreak;

	public int MaxWinStreak;

	public int Rank;

	public int SeasonPlayCount;

	public int SeasonWinCount;

	public static bool IsBanPossibleScore(int score)
	{
		if (score >= NKMPvpCommonConst.Instance.PvpUnitBanExceptionScore)
		{
			return true;
		}
		return false;
	}

	public bool IsBanPossibleScore()
	{
		return IsBanPossibleScore(Score);
	}

	public static void SetPrevScore(int score)
	{
		m_sPrevScore = score;
	}

	public static int GetPrevScore()
	{
		return m_sPrevScore;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref SeasonID);
		stream.PutOrGet(ref WeekID);
		stream.PutOrGet(ref WinCount);
		stream.PutOrGet(ref LoseCount);
		stream.PutOrGet(ref LeagueTierID);
		stream.PutOrGet(ref MaxLeagueTierID);
		stream.PutOrGet(ref Score);
		stream.PutOrGet(ref MaxScore);
		stream.PutOrGet(ref WinStreak);
		stream.PutOrGet(ref MaxWinStreak);
		stream.PutOrGet(ref Rank);
		stream.PutOrGet(ref SeasonPlayCount);
		stream.PutOrGet(ref SeasonWinCount);
	}
}
