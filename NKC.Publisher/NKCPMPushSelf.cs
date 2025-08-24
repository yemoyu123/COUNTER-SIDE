using System;
using UnityEngine;

namespace NKC.Publisher;

public class NKCPMPushSelf : NKCPublisherModule.NKCPMPush
{
	private const string CHANNEL_ID = "channel_id";

	private const string CHANNEL_NAME = "default channel";

	private const string CHANNEL_DESC = "generic notifications";

	public override void Init()
	{
		NKCDefineManager.DEFINE_USE_CHEAT();
	}

	public static void OnClickTestNotification()
	{
	}

	public void OnClickLastNotificationInfo()
	{
	}

	private void CheckNotificationStatus(int notifiyID)
	{
	}

	protected override bool ReserveLocalPush(DateTime newUtcTime, NKC_GAME_OPTION_ALARM_GROUP evtType)
	{
		CancelLocalPush(evtType);
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(newUtcTime.Ticks);
		DateTime dateTime = DateTime.Now + new TimeSpan(timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
		Debug.Log($"\ufffd\ufffd\ufffd\ufffdǪ\ufffd\ufffd \ufffd\ufffd\ufffd - Ÿ\ufffd\ufffd : {evtType}, \ufffdý\ufffd\ufffd\ufffd \ufffdð\ufffd : {DateTime.Now}, \ufffd\ufffd\ufffd \ufffdð\ufffd : {dateTime} : \ufffd\ufffd\ufffd\ufffd \ufffdð\ufffd : d({timeLeft.Days}),h({timeLeft.Hours}),m({timeLeft.Minutes}),s({timeLeft.Seconds})");
		return false;
	}

	protected override void CancelLocalPush(NKC_GAME_OPTION_ALARM_GROUP evtType)
	{
	}
}
