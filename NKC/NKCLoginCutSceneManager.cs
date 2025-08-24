using System;
using System.Collections.Generic;
using NKC.UI;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public static class NKCLoginCutSceneManager
{
	private static Dictionary<int, NKCLoginCutSceneTemplet> m_dicTemplet = new Dictionary<int, NKCLoginCutSceneTemplet>();

	private const string LOGIN_CUT_SCENE_KEY = "LOGIN_CUT_SCENE_KEY_{0}_{1}";

	private static bool m_bCutsceneLoading = false;

	private static int m_currentTempletKey = 0;

	private static bool m_bPlaying = false;

	private static Action m_EndCallback = null;

	public static bool LoadFromLua()
	{
		m_dicTemplet = NKMTempletLoader.LoadDictionary("ab_script", "LUA_LOGIN_CUTSCENE_TEMPLET", "m_LoginCutSceneTemplet", NKCLoginCutSceneTemplet.LoadFromLUA);
		return true;
	}

	public static void Join()
	{
		foreach (NKCLoginCutSceneTemplet value in m_dicTemplet.Values)
		{
			value.Join();
		}
	}

	public static void PostJoin()
	{
		foreach (NKCLoginCutSceneTemplet value in m_dicTemplet.Values)
		{
			value.PostJoin();
		}
	}

	public static NKCLoginCutSceneTemplet GetLoginCutSceneTemplet(EventUnlockCond condType, string condValue)
	{
		foreach (KeyValuePair<int, NKCLoginCutSceneTemplet> item in m_dicTemplet)
		{
			if (item.Value.m_CondType == condType && item.Value.m_CondValue == condValue)
			{
				return item.Value;
			}
		}
		return null;
	}

	public static bool CheckLoginCutScene(Action endCallback)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (!NKMTutorialManager.IsTutorialCompleted(TutorialStep.NicknameChange, myUserData))
		{
			return false;
		}
		m_EndCallback = endCallback;
		List<NKCLoginCutSceneTemplet> list = new List<NKCLoginCutSceneTemplet>();
		foreach (NKCLoginCutSceneTemplet value in m_dicTemplet.Values)
		{
			if (CheckCond(myUserData, value))
			{
				list.Add(value);
			}
		}
		if (list.Count > 0)
		{
			list.Sort((NKCLoginCutSceneTemplet e1, NKCLoginCutSceneTemplet e2) => e1.m_OrderList.CompareTo(e2.m_OrderList));
			Debug.Log("LoginCutScene enabled : " + list[0].m_CutSceneStrID);
			NKCUICutScenPlayer.Instance.UnLoad();
			NKCUICutScenPlayer.Instance.Load(list[0].m_CutSceneStrID);
			NKMPopUpBox.OpenSmallWaitBox();
			m_bCutsceneLoading = true;
			m_currentTempletKey = list[0].Key;
			m_bPlaying = false;
			return true;
		}
		return false;
	}

	public static bool IsPlaying()
	{
		if (!m_bCutsceneLoading)
		{
			return m_bPlaying;
		}
		return true;
	}

	public static void Update()
	{
		if (m_bCutsceneLoading && NKCAssetResourceManager.IsLoadEnd())
		{
			NKCResourceUtility.SwapResource();
			NKMPopUpBox.CloseWaitBox();
			if (m_dicTemplet.TryGetValue(m_currentTempletKey, out var value))
			{
				Debug.Log($"Playing cutscene {m_currentTempletKey}..");
				SetComplete(NKCScenManager.GetScenManager().GetMyUserData(), m_currentTempletKey);
				NKCUICutScenPlayer.Instance.Play(value.m_CutSceneStrID, 0, EndCutScene);
				m_bPlaying = true;
			}
			m_bCutsceneLoading = false;
			m_currentTempletKey = 0;
		}
	}

	private static void EndCutScene()
	{
		NKCSoundManager.StopAllSound();
		NKCSoundManager.PlayScenMusic(NKCScenManager.GetScenManager().GetNowScenID());
		Debug.Log($"Cutscene {m_currentTempletKey} complete..");
		m_bCutsceneLoading = false;
		m_currentTempletKey = 0;
		if (!CheckLoginCutScene(m_EndCallback))
		{
			m_bPlaying = false;
			if (m_EndCallback != null)
			{
				m_EndCallback();
				m_EndCallback = null;
			}
		}
	}

	private static bool IsComplete(NKMUserData userData, int templetKey)
	{
		return PlayerPrefs.GetInt($"LOGIN_CUT_SCENE_KEY_{userData.m_UserUID}_{templetKey}", 0) == 1;
	}

	private static void SetComplete(NKMUserData userData, int templetKey)
	{
		PlayerPrefs.SetInt($"LOGIN_CUT_SCENE_KEY_{userData.m_UserUID}_{templetKey}", 1);
		PlayerPrefs.Save();
	}

	private static bool CheckCond(NKMUserData userData, NKCLoginCutSceneTemplet templet)
	{
		if (userData == null || templet == null)
		{
			return false;
		}
		if (IsComplete(userData, templet.Key))
		{
			return false;
		}
		if (templet.HasDateLimit && !NKCSynchronizedTime.IsEventTime(templet.StartDateUTC, templet.EndDateUTC))
		{
			return false;
		}
		if (templet.m_CondType != EventUnlockCond.None)
		{
			switch (templet.m_CondType)
			{
			case EventUnlockCond.EventClear:
			{
				string condValue = templet.m_CondValue;
				TutorialStep result;
				bool flag = Enum.TryParse<TutorialStep>(condValue, out result);
				int result2;
				bool flag2 = int.TryParse(condValue, out result2);
				if (flag)
				{
					if (!NKCTutorialManager.TutorialCompleted(result))
					{
						return false;
					}
					break;
				}
				if (flag2)
				{
					if (!NKCTutorialManager.TutorialCompleted((TutorialStep)result2))
					{
						return false;
					}
					break;
				}
				return false;
			}
			case EventUnlockCond.DungeonClear:
				if (!userData.CheckDungeonClear(templet.m_CondValue))
				{
					return false;
				}
				break;
			case EventUnlockCond.WarfareClear:
				if (!userData.CheckWarfareClear(templet.m_CondValue))
				{
					return false;
				}
				break;
			case EventUnlockCond.PhaseClear:
				if (!NKCPhaseManager.CheckPhaseClear(NKMPhaseTemplet.Find(templet.m_CondValue)))
				{
					return false;
				}
				break;
			case EventUnlockCond.StageClear:
			{
				NKMStageTempletV2 stageTemplet = NKMStageTempletV2.Find(templet.m_CondValue);
				if (!userData.CheckStageCleared(stageTemplet))
				{
					return false;
				}
				break;
			}
			case EventUnlockCond.Birthday:
			{
				if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.BIRTHDAY))
				{
					return false;
				}
				if (!IsReservedBirthdayCutscene())
				{
					return false;
				}
				if (NKCScenManager.CurrentUserData().m_BirthDayData == null || NKCScenManager.CurrentUserData().m_BirthDayData.Years == 0)
				{
					return false;
				}
				if (!int.TryParse(templet.m_CondValue, out var result3) || result3 != NKCScenManager.CurrentUserData().m_BirthDayData.Years)
				{
					return false;
				}
				break;
			}
			case EventUnlockCond.ReturnUser:
				if (!NKMOpenTagManager.IsOpened("RETURN_USER_CUTSCENE"))
				{
					return false;
				}
				if (!NKCScenManager.CurrentUserData().IsReturnUser())
				{
					return false;
				}
				break;
			}
		}
		return true;
	}

	private static string GetBirthdayKey()
	{
		return $"{NKCScenManager.CurrentUserData().m_UserUID}_BIRTHDAY_RESERVED";
	}

	public static void SetReservedBirthdayCutscene(bool bValue)
	{
		PlayerPrefs.SetInt(GetBirthdayKey(), bValue ? 1 : 0);
	}

	public static bool IsReservedBirthdayCutscene()
	{
		if (PlayerPrefs.HasKey(GetBirthdayKey()) && PlayerPrefs.GetInt(GetBirthdayKey()) == 1)
		{
			return true;
		}
		return false;
	}
}
