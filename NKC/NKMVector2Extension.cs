using NKM;
using UnityEngine;

namespace NKC;

public static class NKMVector2Extension
{
	public static Vector2 GetNowUnityValue(this NKMTrackingVector2 cNKMVector2)
	{
		NKMVector2 nowValue = cNKMVector2.GetNowValue();
		return new Vector2(nowValue.x, nowValue.y);
	}

	public static Vector2 GetUnityDelta(this NKMTrackingVector2 cNKMVector2)
	{
		NKMVector2 delta = cNKMVector2.GetDelta();
		return new Vector2(delta.x, delta.y);
	}

	public static Vector2 GetBeforeUnityValue(this NKMTrackingVector2 cNKMVector2)
	{
		NKMVector2 beforeValue = cNKMVector2.GetBeforeValue();
		return new Vector2(beforeValue.x, beforeValue.y);
	}

	public static Vector2 GetTargetUnityValue(this NKMTrackingVector2 cNKMVector2)
	{
		NKMVector2 targetValue = cNKMVector2.GetTargetValue();
		return new Vector2(targetValue.x, targetValue.y);
	}

	public static void SetNowValue(this NKMTrackingVector2 cNKMVector2, Vector2 NowValue)
	{
		NKMVector2 nowValue = new NKMVector2(NowValue.x, NowValue.y);
		cNKMVector2.SetNowValue(nowValue);
	}

	public static void SetTracking(this NKMTrackingVector2 cNKMVector2, Vector2 targetVal, float fTime, TRACKING_DATA_TYPE eTrackingType)
	{
		NKMVector2 targetVal2 = new NKMVector2(targetVal.x, targetVal.y);
		cNKMVector2.SetTracking(targetVal2, fTime, eTrackingType);
	}
}
