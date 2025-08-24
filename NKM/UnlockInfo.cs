using System;
using System.Collections.Generic;
using Cs.Logging;

namespace NKM;

public readonly struct UnlockInfo
{
	public readonly STAGE_UNLOCK_REQ_TYPE eReqType;

	public readonly int reqValue;

	public readonly string reqValueStr;

	public readonly DateTime reqDateTime;

	public UnlockInfo(STAGE_UNLOCK_REQ_TYPE reqType, int reqValue)
	{
		eReqType = reqType;
		this.reqValue = reqValue;
		reqValueStr = string.Empty;
		reqDateTime = DateTime.MinValue;
	}

	public UnlockInfo(STAGE_UNLOCK_REQ_TYPE reqType, int reqValue, string reqValueStr)
	{
		eReqType = reqType;
		this.reqValue = reqValue;
		this.reqValueStr = reqValueStr;
		reqDateTime = DateTime.MinValue;
	}

	public UnlockInfo(STAGE_UNLOCK_REQ_TYPE reqType, int reqValue, DateTime reqDateTime)
	{
		eReqType = reqType;
		this.reqValue = reqValue;
		reqValueStr = string.Empty;
		this.reqDateTime = reqDateTime;
	}

	public UnlockInfo(STAGE_UNLOCK_REQ_TYPE reqType, int reqValue, string reqValueStr, DateTime reqDateTime)
	{
		eReqType = reqType;
		this.reqValue = reqValue;
		this.reqValueStr = reqValueStr;
		this.reqDateTime = reqDateTime;
	}

	public static UnlockInfo LoadFromLua(NKMLua lua, bool nullable = true)
	{
		STAGE_UNLOCK_REQ_TYPE result = STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_UNLOCKED;
		if (!lua.GetData("m_UnlockReqType", ref result))
		{
			if (!nullable)
			{
				Log.ErrorAndExit("invalid m_UnlockReqType", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMContentUnlockManager.cs", 137);
			}
			return new UnlockInfo(result, 0);
		}
		int rValue = 0;
		lua.GetData("m_UnlockReqValue", ref rValue);
		string rValue2 = "";
		if (IsDateTimeData(result))
		{
			lua.GetData("m_UnlockReqValueStr", ref rValue2);
			DateTime.TryParse(rValue2, out var result2);
			return new UnlockInfo(result, rValue, result2);
		}
		lua.GetData("m_UnlockReqValueStr", ref rValue2);
		if (result == STAGE_UNLOCK_REQ_TYPE.SURT_UNLOCK_STAGE)
		{
			lua.GetData("m_ContentsValue", ref rValue);
		}
		return new UnlockInfo(result, rValue, rValue2);
	}

	public static List<UnlockInfo> LoadFromLua2(NKMLua lua)
	{
		List<UnlockInfo> list = new List<UnlockInfo>();
		int num = 1;
		while (true)
		{
			STAGE_UNLOCK_REQ_TYPE result = STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_UNLOCKED;
			if (!lua.GetData($"m_UnlockReqType{num}", ref result))
			{
				break;
			}
			int rValue = 0;
			lua.GetData($"m_UnlockReqValue{num}", ref rValue);
			string rValue2 = "";
			if (IsDateTimeData(result))
			{
				lua.GetData($"m_UnlockReqValueStr{num}", ref rValue2);
				DateTime.TryParse(rValue2, out var result2);
				list.Add(new UnlockInfo(result, rValue, result2));
			}
			else
			{
				lua.GetData("m_UnlockReqValueStr", ref rValue2);
				list.Add(new UnlockInfo(result, rValue, rValue2));
			}
			num++;
		}
		return list;
	}

	public static bool IsDateTimeData(STAGE_UNLOCK_REQ_TYPE type)
	{
		if ((uint)(type - 17) <= 2u || type == STAGE_UNLOCK_REQ_TYPE.SURT_REGISTER_DATE)
		{
			return true;
		}
		return false;
	}
}
