using System;
using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using UnityEngine;

namespace NKC;

public sealed class NKCLocalLoginData : ISerializable
{
	public const string PREF_LAST_LOGIN_DATA = "PREF_LAST_LOGIN_DATA";

	public DateTime LastLoginTime;

	public HashSet<int> m_hsPlayedCutin = new HashSet<int>();

	public bool IsFirstLoginToday { get; private set; }

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref LastLoginTime);
		stream.PutOrGet(ref m_hsPlayedCutin);
	}

	public static NKCLocalLoginData LoadLastLoginData()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		string text = PlayerPrefs.GetString("PREF_LAST_LOGIN_DATA" + nKMUserData.m_UserUID, null);
		NKCLocalLoginData nKCLocalLoginData = new NKCLocalLoginData();
		if (string.IsNullOrEmpty(text) || !nKCLocalLoginData.FromBase64(text))
		{
			nKCLocalLoginData.LastLoginTime = DateTime.MinValue;
			nKCLocalLoginData.m_hsPlayedCutin = new HashSet<int>();
		}
		nKCLocalLoginData.IsFirstLoginToday = nKCLocalLoginData.LastLoginTime.Date < DateTime.UtcNow.Date;
		if (nKCLocalLoginData.IsFirstLoginToday)
		{
			nKCLocalLoginData.m_hsPlayedCutin.Clear();
		}
		Debug.Log($"Last Login : {nKCLocalLoginData.LastLoginTime}, TodayFirstLogin {nKCLocalLoginData.IsFirstLoginToday}");
		return nKCLocalLoginData;
	}

	public void SaveLastLoginData()
	{
		LastLoginTime = DateTime.UtcNow;
		string value = this.ToBase64();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		PlayerPrefs.SetString("PREF_LAST_LOGIN_DATA" + nKMUserData.m_UserUID, value);
		PlayerPrefs.Save();
	}
}
