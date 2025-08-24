using System;
using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public static class NKCAnimationEventManager
{
	private static Dictionary<string, List<NKCAnimationEventTemplet>> m_dicEventSet = new Dictionary<string, List<NKCAnimationEventTemplet>>();

	public static bool DataExist => m_dicEventSet.Count > 0;

	public static bool LoadFromLua()
	{
		m_dicEventSet.Clear();
		IEnumerable<NKCAnimationEventTemplet> enumerable = NKMTempletLoader.LoadCommonPath("AB_SCRIPT", "LUA_ANIMATION_EVENT_TEMPLET", "ANIMATION_EVENT_TEMPLET", NKCAnimationEventTemplet.LoadFromLua);
		if (m_dicEventSet == null)
		{
			Log.Error("[NKCAnimationEventTemplet] m_dicEventSet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCAnimationEventManager.cs", 43);
			return false;
		}
		foreach (NKCAnimationEventTemplet item in enumerable)
		{
			if (!m_dicEventSet.ContainsKey(item.Key))
			{
				m_dicEventSet.Add(item.Key, new List<NKCAnimationEventTemplet>());
			}
			m_dicEventSet[item.Key].Add(item);
		}
		return true;
	}

	public static List<NKCAnimationEventTemplet> Find(string strID)
	{
		if (!m_dicEventSet.TryGetValue(strID, out var value))
		{
			return null;
		}
		return value;
	}

	public static bool IsEmotionOnly(List<NKCAnimationEventTemplet> lstAnim)
	{
		return lstAnim.TrueForAll((NKCAnimationEventTemplet x) => x.m_AniEventType == AnimationEventType.PLAY_EMOTION);
	}

	public static bool CanPlayAnimEvent(INKCAnimationActor actor, string animEventID)
	{
		List<NKCAnimationEventTemplet> lstAnim = Find(animEventID);
		return CanPlayAnimEvent(actor, lstAnim);
	}

	public static bool CanPlayAnimEvent(INKCAnimationActor actor, List<NKCAnimationEventTemplet> lstAnim)
	{
		if (actor == null || lstAnim == null)
		{
			return false;
		}
		foreach (NKCAnimationEventTemplet item in lstAnim)
		{
			switch (item.m_AniEventType)
			{
			case AnimationEventType.ANIMATION_NAME_SPINE:
				if (!actor.CanPlaySpineAnimation(item.m_StrValue))
				{
					return false;
				}
				break;
			case AnimationEventType.ANIMATION_SPINE:
				if (!actor.CanPlaySpineAnimation(GetAnimationType(item.m_StrValue)))
				{
					return false;
				}
				break;
			case AnimationEventType.ANIMATION_UNITY:
				if (actor.Animator == null)
				{
					return false;
				}
				if (!actor.Animator.HasState(0, Animator.StringToHash(item.m_StrValue)))
				{
					return false;
				}
				break;
			}
		}
		return true;
	}

	public static NKCASUIUnitIllust.eAnimation GetAnimationType(string aniName)
	{
		return (NKCASUIUnitIllust.eAnimation)Enum.Parse(typeof(NKCASUIUnitIllust.eAnimation), aniName);
	}
}
