using UnityEngine;

namespace NKC.Publisher;

public class NKCPMSelf : NKCPublisherModule
{
	private static bool s_bBusy;

	protected override ePublisherType _PublisherType => ePublisherType.None;

	protected override bool _Busy => s_bBusy;

	protected override void _Init(OnComplete onComplete)
	{
		Debug.Log("<color=red>NKCPMUnity::_Init</color>");
		onComplete?.Invoke(NKC_PUBLISHER_RESULT_CODE.NPRC_OK);
	}

	protected override NKCPMAuthentication MakeAuthInstance()
	{
		return new NKCPMNone.AuthNone();
	}

	protected override NKCPMInAppPurchase MakeInappInstance()
	{
		return new NKCPMNone.InAppNone();
	}

	protected override NKCPMNotice MakeNoticeInstance()
	{
		return new NKCPMNone.NoticeNone();
	}

	protected override NKCPMStatistics MakeStatisticsInstance()
	{
		return new NKCPMNone.StatisticsNone();
	}

	protected override NKCPMPush MakePushInstance()
	{
		return new NKCPMPushSelf();
	}

	protected override NKCPMPermission MakePermissionInstance()
	{
		return new NKCPMNone.PermissionNone();
	}

	protected override NKCPMServerInfo MakeServerInfoInstance()
	{
		return new NKCPMNone.ServerInfoDefault();
	}

	protected override NKCPMMarketing MakeMarketingInstance()
	{
		return new NKCPMNone.MarketingNone();
	}

	protected override NKCPMLocalization MakeLocalizationInstance()
	{
		return new NKCPMNone.LocalizationNone();
	}

	protected override void OnTimeOut()
	{
		s_bBusy = false;
	}
}
