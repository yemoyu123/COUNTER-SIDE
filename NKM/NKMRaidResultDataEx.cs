using System.Collections.Generic;
using ClientPacket.Raid;
using NKC;

namespace NKM;

public static class NKMRaidResultDataEx
{
	public class Comp : IComparer<NKMRaidJoinData>
	{
		public int Compare(NKMRaidJoinData x, NKMRaidJoinData y)
		{
			if (y.damage > x.damage)
			{
				return 1;
			}
			if (y.damage < x.damage)
			{
				return -1;
			}
			if (y.userUID <= x.userUID)
			{
				return 1;
			}
			return -1;
		}
	}

	public static bool IsOnGoing(this NKMRaidResultData data)
	{
		if (!NKCSynchronizedTime.IsFinished(data.expireDate) && data.curHP > 0f)
		{
			return true;
		}
		return false;
	}

	public static void SortJoinDataByDamage(this NKMRaidResultData data)
	{
		data.raidJoinDataList.Sort(new Comp());
	}
}
