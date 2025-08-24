using System.Collections.Generic;
using NKM.Contract2;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildWelfareTemplet : INKMTemplet
{
	private static Dictionary<int, List<int>> companyBuffGroups = new Dictionary<int, List<int>>();

	public int ID;

	public WELFARE_BUFF_TYPE WelfareCategory;

	public int Order;

	public int CompanyBuffID;

	public int CompanyBuffGroupID;

	public string WelfareTextTitle;

	public string WelfareTextDesc;

	public int WelfareLvDisplay;

	public int WelfareRequireItemID;

	public int WelfareRequireItemValue;

	public UnlockInfo m_UnlockInfo;

	public int Key => ID;

	public NKMCompanyBuffTemplet BuffTemplet { get; private set; }

	public MiscItemUnit ReqMiscItem { get; private set; }

	public static GuildWelfareTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildWelfareTemplet.cs", 37))
		{
			return null;
		}
		GuildWelfareTemplet guildWelfareTemplet = new GuildWelfareTemplet();
		int num = (int)(1u & (cNKMLua.GetData("ID", ref guildWelfareTemplet.ID) ? 1u : 0u)) & (cNKMLua.GetData("m_WelfareCategory", ref guildWelfareTemplet.WelfareCategory) ? 1 : 0);
		cNKMLua.GetData("m_OrderList", ref guildWelfareTemplet.Order);
		int num2 = (int)((uint)num & (cNKMLua.GetData("m_CompanyBuffID", ref guildWelfareTemplet.CompanyBuffID) ? 1u : 0u)) & (cNKMLua.GetData("m_CompanyBuffGroupID", ref guildWelfareTemplet.CompanyBuffGroupID) ? 1 : 0);
		cNKMLua.GetData("m_WelfareTextTitle", ref guildWelfareTemplet.WelfareTextTitle);
		cNKMLua.GetData("m_WelfareTextDesc", ref guildWelfareTemplet.WelfareTextDesc);
		cNKMLua.GetData("m_WelfareLvDisplay", ref guildWelfareTemplet.WelfareLvDisplay);
		int num3 = (int)((uint)num2 & (cNKMLua.GetData("m_WelfareRequireItemID", ref guildWelfareTemplet.WelfareRequireItemID) ? 1u : 0u)) & (cNKMLua.GetData("m_WelfareRequireItemValue", ref guildWelfareTemplet.WelfareRequireItemValue) ? 1 : 0);
		guildWelfareTemplet.m_UnlockInfo = UnlockInfo.LoadFromLua(cNKMLua);
		guildWelfareTemplet.ReqMiscItem = new MiscItemUnit(guildWelfareTemplet.WelfareRequireItemID, guildWelfareTemplet.WelfareRequireItemValue);
		if (num3 == 0)
		{
			return null;
		}
		if (!companyBuffGroups.ContainsKey(guildWelfareTemplet.CompanyBuffGroupID))
		{
			companyBuffGroups.Add(guildWelfareTemplet.CompanyBuffGroupID, new List<int>());
		}
		companyBuffGroups[guildWelfareTemplet.CompanyBuffGroupID].Add(guildWelfareTemplet.CompanyBuffID);
		return guildWelfareTemplet;
	}

	public static GuildWelfareTemplet Find(int key)
	{
		return NKMTempletContainer<GuildWelfareTemplet>.Find(key);
	}

	public static List<int> GetCompanyBuffGroups(int buffGroupId)
	{
		if (!companyBuffGroups.TryGetValue(buffGroupId, out var value))
		{
			return null;
		}
		return value;
	}

	public void Join()
	{
		if (NKMItemManager.GetItemMiscTempletByID(WelfareRequireItemID) == null)
		{
			NKMTempletError.Add($"[GuildWelfare] 잘못된 요구아이템 아이디. requireItemId:{WelfareRequireItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildWelfareTemplet.cs", 91);
		}
		BuffTemplet = NKMCompanyBuffTemplet.Find(CompanyBuffID);
		if (BuffTemplet == null)
		{
			NKMTempletError.Add($"[GuildWelfare] 잘못된 버프 아이디. CompanyBuffID:{CompanyBuffID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildWelfareTemplet.cs", 97);
		}
		ReqMiscItem.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildWelfareTemplet.cs", 104);
	}

	public void Validate()
	{
		if (ID <= 0)
		{
			NKMTempletError.Add($"[GuildWelfare] invalid WelfareID :{ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildWelfareTemplet.cs", 111);
		}
		if (CompanyBuffID <= 0)
		{
			NKMTempletError.Add($"[GuildWelfare] invalid CompanyBuffID:{CompanyBuffID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildWelfareTemplet.cs", 116);
		}
		if (WelfareCategory == WELFARE_BUFF_TYPE.GUILD && WelfareRequireItemID != 24)
		{
			NKMTempletError.Add($"[GuildWelfare] 연합버프는 연합포인트로만 구매 가능. ID:{ID} reqItemId:{WelfareRequireItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildWelfareTemplet.cs", 121);
		}
		if (WelfareRequireItemValue <= 0)
		{
			NKMTempletError.Add($"[GuildWelfare] 잘못된 요구아이템 수량. WelfareRequireItemValue:{WelfareRequireItemValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildWelfareTemplet.cs", 126);
		}
	}
}
