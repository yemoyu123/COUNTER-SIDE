using System;

namespace NKM.Shop;

public static class ShopEnumExtension
{
	public static int ToSucscriptionDays(this PURCHASE_EVENT_REWARD_TYPE self)
	{
		return self switch
		{
			PURCHASE_EVENT_REWARD_TYPE.SUBSCRIBE_7_DAYS => 7, 
			PURCHASE_EVENT_REWARD_TYPE.SUBSCRIBE_10_DAYS => 10, 
			PURCHASE_EVENT_REWARD_TYPE.SUBSCRIBE_15_DAYS => 15, 
			PURCHASE_EVENT_REWARD_TYPE.SUBSCRIBE_18_DAYS => 18, 
			PURCHASE_EVENT_REWARD_TYPE.SUBSCRIBE_30_DAYS => 30, 
			_ => 0, 
		};
	}

	public static bool ToDayOfWeek(this SHOP_RESET_TYPE self, out DayOfWeek result)
	{
		switch (self)
		{
		case SHOP_RESET_TYPE.WEEK_SUN:
			result = DayOfWeek.Sunday;
			return true;
		case SHOP_RESET_TYPE.WEEK_MON:
			result = DayOfWeek.Monday;
			return true;
		case SHOP_RESET_TYPE.WEEK_TUE:
			result = DayOfWeek.Tuesday;
			return true;
		case SHOP_RESET_TYPE.WEEK_WED:
			result = DayOfWeek.Wednesday;
			return true;
		case SHOP_RESET_TYPE.WEEK_THU:
			result = DayOfWeek.Thursday;
			return true;
		case SHOP_RESET_TYPE.WEEK_FRI:
			result = DayOfWeek.Friday;
			return true;
		case SHOP_RESET_TYPE.WEEK_SAT:
			result = DayOfWeek.Saturday;
			return true;
		default:
			result = DayOfWeek.Sunday;
			return false;
		}
	}
}
