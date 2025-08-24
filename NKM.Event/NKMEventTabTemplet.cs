using System;
using System.Collections.Generic;
using ClientPacket.Common;
using NKC;
using NKC.Publisher;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Event;

public sealed class NKMEventTabTemplet : INKMTemplet, INKMTempletEx
{
	private string intervalId;

	public int m_EventID;

	public NKM_EVENT_TYPE m_EventType;

	public bool m_Visible = true;

	private string m_OpenTag;

	public int m_OrderList;

	public string m_EventTabImage;

	public string m_EventTabDesc;

	public string m_EventHelpDesc;

	public string m_EventBannerPrefabName;

	public string m_LobbyBannerType = "BASIC";

	public string m_LobbyBannerText;

	public string m_LobbyButtonImage;

	public string m_LobbyButtonString;

	public NKM_SHORTCUT_TYPE m_ShortCutType;

	public string m_ShortCut;

	public UnlockInfo m_UnlockInfo;

	public List<int> m_lstResourceTypeID = new List<int>();

	public int Key => m_EventID;

	public bool HasDateLimit => EventIntervalTemplet.IsValid;

	public NKMIntervalTemplet EventIntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public DateTime EventDateStart => EventIntervalTemplet.StartDate;

	public DateTime EventDateEnd => EventIntervalTemplet.EndDate;

	public DateTime EventDateStartUtc => NKMTime.LocalToUTC(EventIntervalTemplet.StartDate);

	public DateTime EventDateEndUtc => NKMTime.LocalToUTC(EventIntervalTemplet.EndDate);

	public bool IsAvailable
	{
		get
		{
			if (!EnableByTag)
			{
				return false;
			}
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in m_UnlockInfo))
			{
				return false;
			}
			if (HasDateLimit)
			{
				if (m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_NEWBIE_USER)
				{
					if (!NKCSynchronizedTime.IsEventTime(NKCScenManager.CurrentUserData().m_NKMUserDateData.m_RegisterTime, intervalId, EventDateStartUtc, EventDateEndUtc))
					{
						return false;
					}
				}
				else
				{
					if (m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_RETURN_USER)
					{
						NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
						if (!string.IsNullOrEmpty(m_UnlockInfo.reqValueStr))
						{
							if (Enum.TryParse<ReturningUserType>(m_UnlockInfo.reqValueStr, out var result))
							{
								if (nKMUserData.IsReturnUser(result))
								{
									return NKCSynchronizedTime.IsEventTime(nKMUserData.GetReturnStartDate(result), intervalId, EventDateStartUtc, EventDateEndUtc);
								}
								return false;
							}
							return false;
						}
						foreach (ReturningUserType value in Enum.GetValues(typeof(ReturningUserType)))
						{
							if (nKMUserData.IsReturnUser(value) && NKCSynchronizedTime.IsEventTime(nKMUserData.GetReturnStartDate(value), intervalId, EventDateStartUtc, EventDateEndUtc))
							{
								return true;
							}
						}
						return false;
					}
					if (!NKCSynchronizedTime.IsEventTime(intervalId, EventDateStartUtc, EventDateEndUtc))
					{
						return false;
					}
				}
			}
			if (m_EventBannerPrefabName == "EVENT_AP_PROMO_01")
			{
				if (!NKCDefineManager.DEFINE_IOS())
				{
					return false;
				}
			}
			else if (m_EventBannerPrefabName == "EVENT_S_FOLLOW_WECHAT_01")
			{
				return NKCPublisherModule.Marketing.IsEnableWechatFollowEvent();
			}
			return true;
		}
	}

	public bool HasTimeLimit
	{
		get
		{
			if (!HasDateLimit)
			{
				return NKMContentUnlockManager.IsTimeLimitCondition(m_UnlockInfo);
			}
			return true;
		}
	}

	public DateTime TimeLimit
	{
		get
		{
			if (NKMContentUnlockManager.IsTimeLimitCondition(m_UnlockInfo))
			{
				return NKMContentUnlockManager.GetConditionTimeLimit(m_UnlockInfo);
			}
			if (HasDateLimit)
			{
				return EventDateEndUtc;
			}
			return DateTime.MinValue;
		}
	}

	public static NKMEventTabTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventTabTemplet.cs", 59))
		{
			return null;
		}
		NKMEventTabTemplet nKMEventTabTemplet = new NKMEventTabTemplet();
		cNKMLua.GetData("m_Visible", ref nKMEventTabTemplet.m_Visible);
		int num = (int)(1u & (cNKMLua.GetData("m_EventID", ref nKMEventTabTemplet.m_EventID) ? 1u : 0u) & (cNKMLua.GetData("m_EventType", ref nKMEventTabTemplet.m_EventType) ? 1u : 0u) & (cNKMLua.GetData("m_OrderList", ref nKMEventTabTemplet.m_OrderList) ? 1u : 0u) & (cNKMLua.GetData("m_EventTabImage", ref nKMEventTabTemplet.m_EventTabImage) ? 1u : 0u) & (cNKMLua.GetData("m_EventTabDesc", ref nKMEventTabTemplet.m_EventTabDesc) ? 1u : 0u) & (cNKMLua.GetData("m_EventHelpDesc", ref nKMEventTabTemplet.m_EventHelpDesc) ? 1u : 0u)) & (cNKMLua.GetData("m_EventBannerPrefabName", ref nKMEventTabTemplet.m_EventBannerPrefabName) ? 1 : 0);
		cNKMLua.GetData("m_OpenTag", ref nKMEventTabTemplet.m_OpenTag);
		cNKMLua.GetData("m_DateStrID", ref nKMEventTabTemplet.intervalId);
		cNKMLua.GetData("m_LobbyBannerType", ref nKMEventTabTemplet.m_LobbyBannerType);
		cNKMLua.GetData("m_LobbyBannerText", ref nKMEventTabTemplet.m_LobbyBannerText);
		cNKMLua.GetData("m_LobbyButtonImage", ref nKMEventTabTemplet.m_LobbyButtonImage);
		cNKMLua.GetData("m_LobbyButtonString", ref nKMEventTabTemplet.m_LobbyButtonString);
		cNKMLua.GetData("m_ShortCutType", ref nKMEventTabTemplet.m_ShortCutType);
		cNKMLua.GetData("m_ShortCut", ref nKMEventTabTemplet.m_ShortCut);
		int rValue = 0;
		int rValue2 = 0;
		int rValue3 = 0;
		int rValue4 = 0;
		int rValue5 = 0;
		cNKMLua.GetData("m_ResourceTypeID_1", ref rValue);
		cNKMLua.GetData("m_ResourceTypeID_2", ref rValue2);
		cNKMLua.GetData("m_ResourceTypeID_3", ref rValue3);
		cNKMLua.GetData("m_ResourceTypeID_4", ref rValue4);
		cNKMLua.GetData("m_ResourceTypeID_5", ref rValue5);
		if (rValue > 0)
		{
			nKMEventTabTemplet.m_lstResourceTypeID.Add(rValue);
		}
		if (rValue2 > 0)
		{
			nKMEventTabTemplet.m_lstResourceTypeID.Add(rValue2);
		}
		if (rValue3 > 0)
		{
			nKMEventTabTemplet.m_lstResourceTypeID.Add(rValue3);
		}
		if (rValue4 > 0)
		{
			nKMEventTabTemplet.m_lstResourceTypeID.Add(rValue4);
		}
		if (rValue5 > 0)
		{
			nKMEventTabTemplet.m_lstResourceTypeID.Add(rValue5);
		}
		nKMEventTabTemplet.m_UnlockInfo = UnlockInfo.LoadFromLua(cNKMLua);
		if (num == 0)
		{
			return null;
		}
		return nKMEventTabTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void JoinIntervalTemplet()
	{
		if (string.IsNullOrEmpty(intervalId))
		{
			NKMTempletError.Add($"[EventTabTemplet:{Key}] intervalId는 생략할 수 없음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventTabTemplet.cs", 123);
			return;
		}
		EventIntervalTemplet = NKMIntervalTemplet.Find(intervalId);
		if (EventIntervalTemplet == null)
		{
			NKMTempletError.Add($"[EventTabTemplet:{Key}] 잘못된 interval id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventTabTemplet.cs", 130);
			EventIntervalTemplet = NKMIntervalTemplet.Unuseable;
		}
		else if (EventIntervalTemplet.IsRepeatDate)
		{
			NKMTempletError.Add($"[EventTabTemplet:{Key}] 반복 기간설정 사용 불가. id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventTabTemplet.cs", 135);
		}
	}

	public void Validate()
	{
	}

	public static NKMEventTabTemplet Find(int eventId)
	{
		return NKMTempletContainer<NKMEventTabTemplet>.Find(eventId);
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}

	public string GetTitle()
	{
		return NKCStringTable.GetString(m_EventTabDesc);
	}

	public bool ShowEventBanner()
	{
		if (!m_Visible)
		{
			return false;
		}
		if (m_EventBannerPrefabName == "EVENT_CAFE_STREGA")
		{
			return false;
		}
		return true;
	}
}
