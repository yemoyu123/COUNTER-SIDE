using System;
using NKM.Templet;

namespace NKC.UI.Event;

public class NKCPopupEventRaceUtil
{
	public static bool IsMaintenanceTime()
	{
		DateTime serviceTime = NKCSynchronizedTime.ServiceTime;
		if ((serviceTime.Hour == 3 && serviceTime.Minute >= 50) || (serviceTime.Hour == 4 && serviceTime.Minute <= 10))
		{
			return true;
		}
		return false;
	}

	public static bool IsLastDay()
	{
		DateTime serviceTime = NKCSynchronizedTime.ServiceTime;
		NKMEventRaceTemplet nKMEventRaceTemplet = NKMEventRaceTemplet.Find(NKCScenManager.CurrentUserData().GetRaceData().CurEventID);
		if (nKMEventRaceTemplet != null && serviceTime.Month == nKMEventRaceTemplet.EndDate.Month && ((serviceTime.Day == nKMEventRaceTemplet.EndDate.Day - 1 && serviceTime.Hour >= 4) || serviceTime.Day == nKMEventRaceTemplet.EndDate.Day))
		{
			return true;
		}
		return false;
	}
}
