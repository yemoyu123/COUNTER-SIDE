using System;
using ClientPacket.Common;
using Cs.Core.Util;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMIntervalTemplet : INKMTemplet, IIntervalTemplet
{
	public static NKMIntervalTemplet Invalid { get; } = new NKMIntervalTemplet();

	public static NKMIntervalTemplet Unuseable { get; } = new NKMIntervalTemplet
	{
		StartDate = DateTime.MinValue,
		EndDate = DateTime.MinValue
	};

	public int Key { get; internal set; }

	public string StrKey { get; internal set; }

	public DateTime StartDate { get; internal set; }

	public DateTime EndDate { get; internal set; }

	public int RepeatStartDate { get; internal set; }

	public int RepeatEndDate { get; internal set; }

	public bool IsRepeatDate => RepeatStartDate > 0;

	public bool IsValid => this != Invalid;

	public static implicit operator NKMIntervalData(NKMIntervalTemplet templet)
	{
		return new NKMIntervalData
		{
			key = templet.Key,
			strKey = templet.StrKey,
			startDate = templet.StartDate,
			endDate = templet.EndDate,
			repeatStartDate = templet.RepeatStartDate,
			repeatEndDate = templet.RepeatEndDate
		};
	}

	public static implicit operator NKMIntervalTemplet(NKMIntervalData data)
	{
		return new NKMIntervalTemplet
		{
			Key = data.key,
			StrKey = data.strKey,
			StartDate = data.startDate,
			EndDate = data.endDate,
			RepeatStartDate = data.repeatStartDate,
			RepeatEndDate = data.repeatEndDate
		};
	}

	public static NKMIntervalTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMIntervalTemplet.cs", 59))
		{
			return null;
		}
		string text = "";
		foreach (CountryTagType value in Enum.GetValues(typeof(CountryTagType)))
		{
			if (NKMContentsVersionManager.HasCountryTag(value))
			{
				text = "_" + value.ToString().ToUpper();
				break;
			}
		}
		return new NKMIntervalTemplet
		{
			Key = lua.GetInt32("m_DateID"),
			StrKey = lua.GetString("m_DateStrID"),
			StartDate = lua.GetDateTime("m_DateStart" + text, DateTime.MinValue),
			EndDate = lua.GetDateTime("m_DateEnd" + text, DateTime.MinValue),
			RepeatStartDate = lua.GetInt32("m_RepeatDateStart", 0),
			RepeatEndDate = lua.GetInt32("m_RepeatDateEnd", 0)
		};
	}

	public static NKMIntervalTemplet Find(int key)
	{
		return NKMTempletContainer<NKMIntervalTemplet>.Find(key);
	}

	public static NKMIntervalTemplet Find(string key)
	{
		return NKMTempletContainer<NKMIntervalTemplet>.Find(key);
	}

	public int GetDays()
	{
		if (IsRepeatDate)
		{
			return RepeatEndDate - RepeatStartDate;
		}
		return (int)(EndDate - StartDate).TotalDays;
	}

	public bool IsValidTime(DateTime serviceTime)
	{
		if (this == Invalid)
		{
			return true;
		}
		if (IsRepeatDate)
		{
			if (RepeatStartDate <= serviceTime.Day)
			{
				return serviceTime.Day < RepeatEndDate;
			}
			return false;
		}
		return serviceTime.IsBetween(StartDate, EndDate);
	}

	public DateTime CalcStartDate(DateTime current)
	{
		if (IsRepeatDate)
		{
			return new DateTime(current.Year, current.Month, RepeatStartDate, current.Hour, current.Minute, current.Second, current.Millisecond, current.Kind);
		}
		return StartDate;
	}

	public DateTime CalcEndDate(DateTime current)
	{
		if (IsRepeatDate)
		{
			return new DateTime(current.Year, current.Month, RepeatEndDate, current.Hour, current.Minute, current.Second, current.Millisecond, current.Kind);
		}
		return EndDate;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (RepeatStartDate > 0 || RepeatEndDate > 0)
		{
			if (RepeatStartDate == 0 || RepeatEndDate == 0)
			{
				NKMTempletError.Add($"[IntervalTemplet:{StrKey}] 반복기간 설정 오류. {RepeatStartDate} ~ {RepeatEndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMIntervalTemplet.cs", 163);
			}
			if (RepeatStartDate == RepeatEndDate)
			{
				NKMTempletError.Add($"[IntervalTemplet:{StrKey}] 반복기간 일자가 같을 수 없음. {RepeatStartDate} ~ {RepeatEndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMIntervalTemplet.cs", 168);
			}
			if (StartDate != DateTime.MinValue || EndDate != DateTime.MinValue)
			{
				NKMTempletError.Add($"[IntervalTemplet:{StrKey}] 반복기간 설정시엔 일반기간 지정 불가. {StartDate} ~ {EndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMIntervalTemplet.cs", 173);
			}
			if (RepeatStartDate > RepeatEndDate)
			{
				NKMTempletError.Add($"[IntervalTemplet:{StrKey}] 시작일자는 종료일자보다 작아야 함. {RepeatStartDate} ~ {RepeatEndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMIntervalTemplet.cs", 178);
			}
			if (RepeatEndDate > 28)
			{
				NKMTempletError.Add($"[IntervalTemplet:{StrKey}] 반복기간 종료일자는 28일보다 클 수 없음. {RepeatStartDate} ~ {RepeatEndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMIntervalTemplet.cs", 183);
			}
		}
		else if (StartDate == DateTime.MinValue || EndDate == DateTime.MinValue)
		{
			NKMTempletError.Add($"[IntervalTemplet:{StrKey}] 날짜 데이터가 올바르지 않음. {StartDate} ~ {EndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMIntervalTemplet.cs", 190);
		}
		else if (StartDate >= EndDate)
		{
			NKMTempletError.Add($"[IntervalTemplet:{StrKey}] 시작일자는 종료일자보다 작아야 함. {StartDate} ~ {EndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMIntervalTemplet.cs", 195);
		}
	}

	public override string ToString()
	{
		return $"{StrKey} {StartDate} ~ {EndDate} repeated:{RepeatStartDate} {RepeatEndDate}";
	}

	public string DumpBasicFormat(DateTime current)
	{
		DateTime dateTime = CalcStartDate(current);
		DateTime dateTime2 = CalcEndDate(current);
		return $"start:{dateTime} end:{dateTime2}";
	}

	public DateTime GetStartDate()
	{
		if (IsRepeatDate)
		{
			DateTime serviceTime = NKCSynchronizedTime.ServiceTime;
			return new DateTime(serviceTime.Year, serviceTime.Month, RepeatStartDate);
		}
		return StartDate;
	}

	public DateTime GetEndDate()
	{
		if (IsRepeatDate)
		{
			DateTime serviceTime = NKCSynchronizedTime.ServiceTime;
			return new DateTime(serviceTime.Year, serviceTime.Month, RepeatEndDate);
		}
		return EndDate;
	}

	public DateTime GetStartDateUtc()
	{
		if (!IsValid)
		{
			return DateTime.MinValue;
		}
		return NKCSynchronizedTime.ToUtcTime(GetStartDate());
	}

	public DateTime GetEndDateUtc()
	{
		if (!IsValid)
		{
			return DateTime.MinValue;
		}
		return NKCSynchronizedTime.ToUtcTime(GetEndDate());
	}
}
