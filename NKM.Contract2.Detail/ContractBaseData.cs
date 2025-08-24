using ClientPacket.Common;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Contract2.Detail;

internal sealed class ContractBaseData : INKMTemplet, INKMTempletEx
{
	public string m_OpenTag;

	public bool m_bPickUp;

	public int m_Order;

	public string m_ContractStrID;

	public string m_ContractName;

	public string m_ContractDesc;

	public string m_MainBannerFileName;

	public int m_ContractCategory;

	public string m_Image;

	public int m_Priority;

	public string m_ContractBannerName;

	public string m_ContractBannerDesc;

	public CONTRACT_CONDITION m_OpenCond;

	public int m_OpenCondValue;

	public bool m_MissionCountIgnore;

	public bool m_CheckReturningUser;

	public ReturningUserType m_ReturningUserType = ReturningUserType.Return15;

	public int m_requiredStageClearId;

	public int m_requiredContractClearId;

	public int m_triggerStageClearId;

	public int m_triggerShopProductId;

	private string intervalId;

	private string bannerDescIntervalId;

	public int Key { get; }

	public NKMIntervalTemplet EventIntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public NKMIntervalTemplet BannerDescIntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public ContractBaseData(int contractId)
	{
		Key = contractId;
	}

	public static ContractBaseData LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 53))
		{
			return null;
		}
		int rValue;
		bool data = lua.GetData("m_ContractID", out rValue, -1);
		ContractBaseData contractBaseData = new ContractBaseData(rValue);
		lua.GetData("m_OpenTag", ref contractBaseData.m_OpenTag);
		if (!lua.GetData("m_bPickUp", ref contractBaseData.m_bPickUp))
		{
			NKMTempletError.Add($"[LUA_CONTRACT_TAB_TABLE] m_bPickUp is empty. contractId:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 62);
			return null;
		}
		if (!data)
		{
			NKMTempletError.Add("[LUA_CONTRACT_TAB_TABLE] m_ContractID is empty. openTag:" + contractBaseData.m_OpenTag, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 68);
			return null;
		}
		if (!lua.GetData("m_Order", ref contractBaseData.m_Order))
		{
			NKMTempletError.Add($"[LUA_CONTRACT_TAB_TABLE] m_Order is empty. contractId:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 74);
			return null;
		}
		if (!lua.GetData("m_ContractStrID", ref contractBaseData.m_ContractStrID))
		{
			NKMTempletError.Add($"[LUA_CONTRACT_TAB_TABLE] m_ContractStrID is empty. contractId:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 80);
			return null;
		}
		if (!lua.GetData("m_ContractName", ref contractBaseData.m_ContractName))
		{
			NKMTempletError.Add($"[LUA_CONTRACT_TAB_TABLE] m_ContractName is empty. contractId:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 86);
			return null;
		}
		lua.GetData("m_ContractDesc", ref contractBaseData.m_ContractDesc);
		if (!lua.GetData("m_MainBannerFileName", ref contractBaseData.m_MainBannerFileName))
		{
			NKMTempletError.Add($"[LUA_CONTRACT_TAB_TABLE] m_ContractDesc is empty. contractId:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 93);
			return null;
		}
		if (!lua.GetData("m_DateStrID", ref contractBaseData.intervalId))
		{
			NKMTempletError.Add($"[LUA_CONTRACT_TAB_TABLE] m_DateStrID is empty. contractId:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 98);
			return null;
		}
		if (!lua.GetData("m_ContractCategory", ref contractBaseData.m_ContractCategory))
		{
			NKMTempletError.Add($"[LUA_CONTRACT_TAB_TABLE] m_ContractCategory is empty. contractId:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 103);
			return null;
		}
		lua.GetData("m_Image", ref contractBaseData.m_Image);
		lua.GetData("m_Priority", ref contractBaseData.m_Priority);
		lua.GetData("m_ContractBannerName", ref contractBaseData.m_ContractBannerName);
		lua.GetData("m_ContractBannerDesc", ref contractBaseData.m_ContractBannerDesc);
		lua.GetData("m_BannerDescInterval", ref contractBaseData.bannerDescIntervalId);
		lua.GetData("m_OpenCond", ref contractBaseData.m_OpenCond);
		lua.GetData("m_OpenCondValue", ref contractBaseData.m_OpenCondValue);
		lua.GetData("m_MissionCountIgnore", ref contractBaseData.m_MissionCountIgnore);
		lua.GetData("m_CheckReturningUser", ref contractBaseData.m_CheckReturningUser);
		lua.GetData("m_ReturningUserType", ref contractBaseData.m_ReturningUserType);
		lua.GetData("m_requiredContractClearId", ref contractBaseData.m_requiredContractClearId);
		lua.GetData("m_requiredStageClearId", ref contractBaseData.m_requiredStageClearId);
		if (lua.GetData("m_triggerStageClearId", ref contractBaseData.m_triggerStageClearId))
		{
			ContractTempletLoader.AddTriggerStageClearId(contractBaseData.m_triggerStageClearId, rValue);
		}
		if (lua.GetData("m_triggerShopProductId", ref contractBaseData.m_triggerShopProductId))
		{
			ContractTempletLoader.AddTriggerShopProductId(contractBaseData.m_triggerShopProductId, rValue);
		}
		return contractBaseData;
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
			NKMTempletError.Add($"[ContractBase:{Key}] 잘못된 interval id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 145);
		}
		else
		{
			EventIntervalTemplet = NKMIntervalTemplet.Find(intervalId);
			if (EventIntervalTemplet == null)
			{
				NKMTempletError.Add($"[ContractBase:{Key}] 잘못된 interval id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 152);
				EventIntervalTemplet = NKMIntervalTemplet.Unuseable;
				return;
			}
			if (EventIntervalTemplet.IsRepeatDate)
			{
				NKMTempletError.Add($"[ContractBase:{Key}] 반복 기간설정 사용 불가. id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/Detail/ContractBaseData.cs", 159);
			}
		}
		if (!string.IsNullOrEmpty(bannerDescIntervalId))
		{
			BannerDescIntervalTemplet = NKMIntervalTemplet.Find(bannerDescIntervalId);
			if (BannerDescIntervalTemplet == null)
			{
				BannerDescIntervalTemplet = NKMIntervalTemplet.Unuseable;
			}
		}
	}

	public void Validate()
	{
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
