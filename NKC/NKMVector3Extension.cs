using NKM;
using UnityEngine;

namespace NKC;

public static class NKMVector3Extension
{
	public static Vector3 GetNowUnityValue(this NKMTrackingVector3 cNKMVector3)
	{
		NKMVector3 nowValue = cNKMVector3.GetNowValue();
		return new Vector3(nowValue.x, nowValue.y, nowValue.z);
	}

	public static Vector3 GetUnityDelta(this NKMTrackingVector3 cNKMVector3)
	{
		NKMVector3 delta = cNKMVector3.GetDelta();
		return new Vector3(delta.x, delta.y, delta.z);
	}

	public static Vector3 GetBeforeUnityValue(this NKMTrackingVector3 cNKMVector3)
	{
		NKMVector3 beforeValue = cNKMVector3.GetBeforeValue();
		return new Vector3(beforeValue.x, beforeValue.y, beforeValue.z);
	}

	public static Vector3 GetTargetUnityValue(this NKMTrackingVector3 cNKMVector3)
	{
		NKMVector3 targetValue = cNKMVector3.GetTargetValue();
		return new Vector3(targetValue.x, targetValue.y, targetValue.z);
	}

	public static void SetNowValue(this NKMTrackingVector3 cNKMVector3, Vector3 NowValue)
	{
		cNKMVector3.SetNowValue(NowValue.x, NowValue.y, NowValue.z);
	}

	public static void SetTracking(this NKMTrackingVector3 cNKMVector3, Vector3 targetVal, float fTime, TRACKING_DATA_TYPE eTrackingType)
	{
		NKMVector3 targetVal2 = new NKMVector3(targetVal.x, targetVal.y, targetVal.z);
		cNKMVector3.SetTracking(targetVal2, fTime, eTrackingType);
	}
}
