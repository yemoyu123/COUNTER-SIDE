namespace NKC.Advertise;

public class NKCAdNone : NKCAdBase
{
	public override void Initialize(bool bAskUserConsent)
	{
	}

	public override void WatchRewardedAd(AD_TYPE adType, OnUserEarnedReward onUserEarnedReward, OnAdFailedToShowAd onFailedToShowAd)
	{
	}
}
