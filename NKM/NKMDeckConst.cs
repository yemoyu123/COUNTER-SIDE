using NKM.Templet.Base;

namespace NKM;

public sealed class NKMDeckConst
{
	private readonly int defaultRaidDeckCount = 3;

	private readonly int maxRaidDeckCount = 6;

	private readonly int maxNormalDeckCount = 10;

	private readonly int maxPvpDeckCount = 4;

	private readonly int maxDailyDeckCount = 20;

	private readonly int maxFriendDeckCount = 1;

	private readonly int maxPvpDefenceDeckCount = 1;

	private readonly int maxTrimingDeckCount = 3;

	private readonly int maxDiveDeckCount = 4;

	public int DefaultRaidDeckCount => defaultRaidDeckCount;

	public int MaxRaidDeckCount => maxRaidDeckCount;

	public int MaxNormalDeckCount => maxNormalDeckCount;

	public int MaxPvpDeckCount => maxPvpDeckCount;

	public int MaxDailyDeckCount => maxDailyDeckCount;

	public int MaxFriendDeckCount => maxFriendDeckCount;

	public int MaxPvpDefenceDeckCount => maxPvpDefenceDeckCount;

	public int MaxTrimingDeckCount => maxTrimingDeckCount;

	public int MaxDiveDeckCount => maxDiveDeckCount;

	public void Validate()
	{
		if (defaultRaidDeckCount > maxRaidDeckCount)
		{
			NKMTempletError.Add($"[DeckConst] 레이드 소대의 최대 개수가 기본 소대 개수 보다 적음 minCount:{defaultRaidDeckCount} maxCount:{maxRaidDeckCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDeckConst.cs", 32);
		}
	}
}
