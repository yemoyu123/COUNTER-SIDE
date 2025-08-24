using System;
using Cs.Logging;
using NKC;
using UnityEngine;

namespace NKA.Service.Android;

internal class PushServiceImpl : MonoBehaviour, IService
{
	private static PushServiceImpl _instance;

	internal static PushServiceImpl Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject obj = new GameObject
				{
					name = "PushServiceObject"
				};
				_instance = obj.AddComponent<PushServiceImpl>();
				_instance.BindService();
				UnityEngine.Object.DontDestroyOnLoad(obj);
			}
			return _instance;
		}
	}

	public void BindService()
	{
		if (!IsValid())
		{
			return;
		}
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager");
		Log.Debug("[PushServiceImpl] BindPushService", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/PushServiceImpl.cs", 43);
		androidJavaClass.CallStatic("BindPushService");
	}

	public void UnbindService()
	{
		if (!IsValid())
		{
			return;
		}
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager");
		Log.Debug("[PushServiceImpl] UnbindPushService", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/PushServiceImpl.cs", 57);
		androidJavaClass.CallStatic("UnbindPushService");
	}

	public void OnPause(bool pauseState)
	{
		IsValid();
	}

	public bool IsValid()
	{
		return NKCDefineManager.USE_ANDROIDSERVICE();
	}

	public void ReservePush(DateTime newUtcTime, int eventId, string title, string message)
	{
		if (!IsValid())
		{
			return;
		}
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager");
		Log.Debug("[PushServiceImpl] ReservePush", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/PushServiceImpl.cs", 84);
		Log.Debug($"[PushServiceImpl] EventId : {eventId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/PushServiceImpl.cs", 86);
		Log.Debug($"[PushServiceImpl] ReserveTime : {newUtcTime}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/PushServiceImpl.cs", 87);
		androidJavaClass.CallStatic("ReservePush", eventId, title, message, newUtcTime.Year, newUtcTime.Month, newUtcTime.Day, newUtcTime.Hour, newUtcTime.Minute, newUtcTime.Second);
	}

	public void CancelPush(NKC_GAME_OPTION_ALARM_GROUP evtType)
	{
		if (!IsValid())
		{
			return;
		}
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.studiobside.nkaservice.ServiceManager");
		Log.Debug("[PushServiceImpl] CancelPush", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/PushServiceImpl.cs", 102);
		Log.Debug($"[PushServiceImpl] PushType : {evtType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKA/Service/Android/PushServiceImpl.cs", 104);
		androidJavaClass.CallStatic("CancelPush", (int)evtType);
	}
}
