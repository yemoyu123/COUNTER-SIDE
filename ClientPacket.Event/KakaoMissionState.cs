namespace ClientPacket.Event;

public enum KakaoMissionState
{
	Initialized,
	Registered,
	Sent,
	Confirmed,
	Failed,
	Flopped,
	NotEnoughBudget,
	OutOfDate
}
