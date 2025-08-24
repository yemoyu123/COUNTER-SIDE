using System;
using NKC;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMTitleTemplet : INKMTemplet
{
	private int titleId;

	private string openTag;

	private string intervalStrId;

	private int expireDays;

	private NKMItemMiscTemplet miscItemTemplet;

	private int orderIndex;

	private string titleName;

	private string titleDesc;

	private string titleColor;

	private string outlineColor;

	private string titleBG;

	private string effectPrefab;

	private bool isIMGTitle;

	private int titleCategoryID;

	private bool isExclude;

	public int Key => titleId;

	public int TitleId => titleId;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public NKMIntervalTemplet IntervalTemplet { get; private set; }

	public NKMItemMiscTemplet MiscItemTemplet => miscItemTemplet;

	public int OrderIndex => orderIndex;

	public string TitleBG => titleBG;

	public string TitleColor => titleColor;

	public string OutlineColor => outlineColor;

	public string EffectPrefab => effectPrefab;

	public bool IsIMGTitle => isIMGTitle;

	public static bool TitleOpenTagEnabled => NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.USER_TITLE);

	public int TitleCategoryID => titleCategoryID;

	public bool bExclude => isExclude;

	public static NKMTitleTemplet Find(int key)
	{
		return NKMTempletContainer<NKMTitleTemplet>.Find(key);
	}

	public static NKMTitleTemplet LoadFromLua(NKMLua lua)
	{
		NKMTitleTemplet nKMTitleTemplet = new NKMTitleTemplet();
		if (!lua.GetData("UserTitleID", ref nKMTitleTemplet.titleId))
		{
			NKMTempletError.Add("[NKMTitleTemplet][LoadFromLua] titleId 입력은 필수입니다", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Title/NKMTitleTemplet.cs", 42);
			return null;
		}
		lua.GetData("OpenTag", ref nKMTitleTemplet.openTag);
		lua.GetData("IntervalStrId", ref nKMTitleTemplet.intervalStrId);
		lua.GetData("ExpiryDate", ref nKMTitleTemplet.expireDays);
		return nKMTitleTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
		miscItemTemplet = NKMItemMiscTemplet.Find(titleId);
		if (miscItemTemplet == null)
		{
			NKMTempletError.Add($"[NKMTitleTemplet][Join] MiscItemTemplet 을 찾을 수 없습니다 titleId[{titleId}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Title/NKMTitleTemplet.cs", 63);
		}
	}

	public void Validate()
	{
		_ = EnableByTag;
	}

	private void JoinIntervalTemplet()
	{
		if (!string.IsNullOrEmpty(intervalStrId))
		{
			IntervalTemplet = NKMIntervalTemplet.Find(intervalStrId);
			if (IntervalTemplet == null)
			{
				IntervalTemplet = NKMIntervalTemplet.Invalid;
				NKMTempletError.Add($"[NKMTitleTemplet][Join] 인터벌 아이디를 찾을 수 없습니다 titleId[{titleId}] intervalStrId[{intervalStrId}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Title/NKMTitleTemplet.cs", 87);
			}
		}
	}

	public bool CheckExpired(DateTime registerDate, DateTime currentDate)
	{
		if (IntervalTemplet != null && !IntervalTemplet.IsValidTime(currentDate))
		{
			return true;
		}
		if (expireDays > 0 && registerDate.AddDays(expireDays) <= currentDate)
		{
			return true;
		}
		return false;
	}

	public DateTime GetShortestExpireDate(DateTime registerDate)
	{
		DateTime result = registerDate.AddDays(expireDays);
		if (result.CompareTo(IntervalTemplet.EndDate) <= 0)
		{
			return result;
		}
		return IntervalTemplet.EndDate;
	}

	public static NKMTitleTemplet LoadFromLuaEx(NKMLua lua)
	{
		NKMTitleTemplet nKMTitleTemplet = LoadFromLua(lua);
		if (nKMTitleTemplet == null)
		{
			return null;
		}
		int num = 1 & (lua.GetData("OrderList", ref nKMTitleTemplet.orderIndex) ? 1 : 0);
		lua.GetData("TitleNameString", ref nKMTitleTemplet.titleName);
		lua.GetData("TitleDescString", ref nKMTitleTemplet.titleDesc);
		lua.GetData("TitleColor", ref nKMTitleTemplet.titleColor);
		lua.GetData("TitleOutlineColor", ref nKMTitleTemplet.outlineColor);
		lua.GetData("IMGTitle", ref nKMTitleTemplet.isIMGTitle);
		int num2 = num & (lua.GetData("TitleBG", ref nKMTitleTemplet.titleBG) ? 1 : 0);
		lua.GetData("EffectPrefab", ref nKMTitleTemplet.effectPrefab);
		lua.GetData("TitleCategoryID", ref nKMTitleTemplet.titleCategoryID);
		lua.GetData("bExclude", ref nKMTitleTemplet.isExclude);
		if (num2 == 0)
		{
			return null;
		}
		return nKMTitleTemplet;
	}

	public string GetTitleName()
	{
		return NKCStringTable.GetString(titleName);
	}

	public string GetTitleDesc()
	{
		return NKCStringTable.GetString(titleDesc);
	}

	public static bool IsOwnedTitle(int titleId)
	{
		if (Find(titleId) == null)
		{
			return false;
		}
		NKMInventoryData nKMInventoryData = NKCScenManager.CurrentUserData()?.m_InventoryData;
		if (nKMInventoryData != null)
		{
			return nKMInventoryData.GetCountMiscItem(titleId) > 0;
		}
		return false;
	}

	public bool IsIntervalLimited()
	{
		if (expireDays > 0 || !string.IsNullOrEmpty(intervalStrId))
		{
			return true;
		}
		return false;
	}

	public string GetRemainTime(NKMUserData userData)
	{
		NKMItemMiscData nKMItemMiscData = userData?.m_InventoryData.GetItemMisc(titleId);
		if (nKMItemMiscData == null)
		{
			return "";
		}
		if (!IsIntervalLimited())
		{
			return "";
		}
		DateTime endUTCTime = nKMItemMiscData.RegDate.AddDays(expireDays);
		if (!string.IsNullOrEmpty(intervalStrId))
		{
			NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(intervalStrId);
			if (nKMIntervalTemplet != null && (expireDays <= 0 || endUTCTime.CompareTo(nKMIntervalTemplet.EndDate) > 0))
			{
				endUTCTime = nKMIntervalTemplet.EndDate;
			}
		}
		return NKCUtilString.GetTimeString(endUTCTime);
	}
}
