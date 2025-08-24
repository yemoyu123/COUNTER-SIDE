using ClientPacket.Raid;
using NKM.Templet;

namespace NKC;

public static class NKCRaidSeasonManager
{
	public static NKMRaidSeason RaidSeason = new NKMRaidSeason();

	public static NKMRaidSeasonTemplet GetNowSeasonTemplet()
	{
		NKMRaidSeasonTemplet nKMRaidSeasonTemplet = NKMRaidSeasonTemplet.Find(RaidSeason.seasonId);
		if (nKMRaidSeasonTemplet == null)
		{
			return null;
		}
		if (nKMRaidSeasonTemplet.IntervalTemplet.GetStartDateUtc() > NKCSynchronizedTime.GetServerUTCTime() || nKMRaidSeasonTemplet.IntervalTemplet.GetEndDateUtc() <= NKCSynchronizedTime.GetServerUTCTime())
		{
			return null;
		}
		return nKMRaidSeasonTemplet;
	}
}
