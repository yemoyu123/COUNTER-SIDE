using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMCompanyBuffTemplet : INKMTemplet
{
	public int m_CompanyBuffID;

	public NKMCompanyBuffSource m_CompanyBuffSource;

	public NKMCompanyBuffCategory m_CompanyBuffCategory;

	public string m_CompanyBuffIcon;

	public string m_CompanyBuffItemIcon;

	public string m_CompanyBuffTitle;

	public string m_CompanyBuffDesc;

	public bool m_ShowEventMark = true;

	public int m_CompanyBuffTime;

	public int m_AccountLevelMin;

	public int m_AccountLevelMax;

	public List<NKMCompanyBuffInfo> m_CompanyBuffInfoList = new List<NKMCompanyBuffInfo>();

	public int Key => m_CompanyBuffID;

	public DateTime GetExpireDate(DateTime utc)
	{
		return utc.Add(TimeSpan.FromMinutes(m_CompanyBuffTime));
	}

	public static NKMCompanyBuffTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCompanyBuffTemplet.cs", 44))
		{
			return null;
		}
		NKMCompanyBuffTemplet nKMCompanyBuffTemplet = new NKMCompanyBuffTemplet();
		bool data = lua.GetData("m_CompanyBuffID", ref nKMCompanyBuffTemplet.m_CompanyBuffID);
		data &= lua.GetDataEnum<NKMCompanyBuffSource>("m_CompanyBuffSource", out nKMCompanyBuffTemplet.m_CompanyBuffSource);
		data &= lua.GetDataEnum<NKMCompanyBuffCategory>("m_CompanyBuffCategory", out nKMCompanyBuffTemplet.m_CompanyBuffCategory);
		data &= lua.GetData("m_CompanyBuffIcon", ref nKMCompanyBuffTemplet.m_CompanyBuffIcon);
		lua.GetData("m_CompanyBuffItemIcon", ref nKMCompanyBuffTemplet.m_CompanyBuffItemIcon);
		data &= lua.GetData("m_CompanyBuffTitle", ref nKMCompanyBuffTemplet.m_CompanyBuffTitle);
		data &= lua.GetData("m_CompanyBuffDesc", ref nKMCompanyBuffTemplet.m_CompanyBuffDesc);
		data &= lua.GetData("m_CompanyBuffTime", ref nKMCompanyBuffTemplet.m_CompanyBuffTime);
		lua.GetData("m_ShowEventMark", ref nKMCompanyBuffTemplet.m_ShowEventMark);
		for (int i = 1; i <= 5; i++)
		{
			data &= lua.GetDataEnum<NKMConst.Buff.BuffType>($"m_CompanyBuffType{i}", out var result);
			if (!data)
			{
				Log.ErrorAndExit($"company buff type is invalid, id: {nKMCompanyBuffTemplet.m_CompanyBuffID}, index: {i}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCompanyBuffTemplet.cs", 66);
			}
			if (result != NKMConst.Buff.BuffType.NONE)
			{
				int rValue = 0;
				data &= lua.GetData($"m_CompanyBuffRatio{i}", ref rValue);
				if (!data || rValue == 0)
				{
					Log.ErrorAndExit($"company buff ratio is invalid, id: {nKMCompanyBuffTemplet.m_CompanyBuffID}, index: {i}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCompanyBuffTemplet.cs", 78);
					return null;
				}
				NKMCompanyBuffInfo item = new NKMCompanyBuffInfo
				{
					m_CompanyBuffType = result,
					m_CompanyBuffRatio = rValue
				};
				nKMCompanyBuffTemplet.m_CompanyBuffInfoList.Add(item);
			}
		}
		if (nKMCompanyBuffTemplet.m_CompanyBuffSource == NKMCompanyBuffSource.LEVEL)
		{
			data &= lua.GetData("m_AccountLevel_Max", ref nKMCompanyBuffTemplet.m_AccountLevelMax);
			data &= lua.GetData("m_AccountLevel_Min", ref nKMCompanyBuffTemplet.m_AccountLevelMin);
		}
		if (!data)
		{
			Log.ErrorAndExit($"[CompanyBuff] loading templet failed, company buff id: {nKMCompanyBuffTemplet.m_CompanyBuffID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCompanyBuffTemplet.cs", 100);
			return null;
		}
		return nKMCompanyBuffTemplet;
	}

	public static NKMCompanyBuffTemplet Find(int key)
	{
		return NKMTempletContainer<NKMCompanyBuffTemplet>.Find(key);
	}

	public static IEnumerable<NKMCompanyBuffTemplet> GetActiveCompanyBuffTempletsByLevel(int accountLevel)
	{
		return NKMTempletContainer<NKMCompanyBuffTemplet>.Values.Where((NKMCompanyBuffTemplet templet) => templet.m_CompanyBuffSource == NKMCompanyBuffSource.LEVEL && accountLevel >= templet.m_AccountLevelMin && accountLevel <= templet.m_AccountLevelMax);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		switch (m_CompanyBuffSource)
		{
		case NKMCompanyBuffSource.GENERAL:
		case NKMCompanyBuffSource.ON_TIME_EVENT:
			if (m_CompanyBuffTime <= 0)
			{
				Log.ErrorAndExit($"[CompanyBuff : {m_CompanyBuffID}] 회사버프 로드에 실패했습니다. 회사 버프의 시간이 0 입니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCompanyBuffTemplet.cs", 135);
			}
			break;
		case NKMCompanyBuffSource.LEVEL:
			if (m_AccountLevelMin > m_AccountLevelMax)
			{
				Log.ErrorAndExit($"[CompanyBuff : {m_CompanyBuffID}] 회사버프 로드에 실패했습니다. 레벨 값 지정이 잘못되었습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCompanyBuffTemplet.cs", 143);
			}
			if (m_CompanyBuffTime > 0)
			{
				Log.ErrorAndExit($"[CompanyBuff : {m_CompanyBuffID}] 회사버프 로드에 실패했습니다. 회사 버프의 시간이 존재합니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCompanyBuffTemplet.cs", 148);
			}
			break;
		}
	}

	public string GetBuffName()
	{
		return NKCStringTable.GetString(m_CompanyBuffTitle);
	}

	public string GetBuffDescForItemPopup()
	{
		return NKCStringTable.GetString(m_CompanyBuffDesc);
	}
}
