using System;
using System.Collections.Generic;
using ClientPacket.Common;
using NKC;
using NKM.Contract2.Detail;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Contract2;

public abstract class ContractTempletBase : INKMTemplet
{
	private readonly ContractBaseData baseData;

	public int Key => baseData.Key;

	public string DebugName => $"[{Key}]{ContractStrID}";

	public int Order => baseData.m_Order;

	public bool PickUp => baseData.m_bPickUp;

	public NKMIntervalTemplet EventIntervalTemplet => baseData.EventIntervalTemplet;

	public string ContractStrID => baseData.m_ContractStrID;

	public string ContractNameId => baseData.m_ContractName;

	public string ContractDescId => baseData.m_ContractDesc;

	public CONTRACT_CONDITION OpenCondition => baseData.m_OpenCond;

	public int OpenConditionValue => baseData.m_OpenCondValue;

	public string MainBannerName => baseData.m_MainBannerFileName;

	public string ContractBannerNameID => baseData.m_ContractBannerName;

	public string ContractBannerDescID => baseData.m_ContractBannerDesc;

	public NKMIntervalTemplet BannerDescIntervalTemplet => baseData.BannerDescIntervalTemplet;

	public ContractTempletBase PreviousContract { get; private set; }

	public NKMStageTempletV2 StageCondition { get; private set; }

	public abstract bool IsClosingType { get; }

	public bool EnableByTag => NKMOpenTagManager.IsOpened(baseData.m_OpenTag);

	public bool MissionCountIgnore => baseData.m_MissionCountIgnore;

	public bool CheckReturningUser => baseData.m_CheckReturningUser;

	public ReturningUserType ReturningUserType => baseData.m_ReturningUserType;

	public int RequiredStageClearId => baseData.m_requiredStageClearId;

	public int RequiredContractClearId => baseData.m_requiredContractClearId;

	public int TriggerStageClearId => baseData.m_triggerStageClearId;

	public int TriggerShopProductId => baseData.m_triggerShopProductId;

	public DateTime m_DateStartUtc => NKMTime.LocalToUTC(baseData.EventIntervalTemplet.StartDate);

	public DateTime m_DateEndUtc => NKMTime.LocalToUTC(baseData.EventIntervalTemplet.EndDate);

	public int Category => baseData.m_ContractCategory;

	public string ImageName => baseData.m_Image;

	public int Priority => baseData.m_Priority;

	public abstract HashSet<int> GetPriceItemIDSet();

	internal ContractTempletBase(ContractBaseData baseData)
	{
		this.baseData = baseData;
	}

	public bool IsAvailableTime(DateTime currentServiceTime)
	{
		return baseData.EventIntervalTemplet.IsValidTime(currentServiceTime);
	}

	public virtual void Join()
	{
		switch (OpenCondition)
		{
		case CONTRACT_CONDITION.CONTRACT_FINISH:
			PreviousContract = NKMTempletContainer<ContractTempletBase>.Find(OpenConditionValue);
			if (PreviousContract == null)
			{
				NKMTempletError.Add($"[ContractTemplet] 선행조건 채용 아이디 오류:{OpenConditionValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletBase.cs", 77);
			}
			break;
		case CONTRACT_CONDITION.STAGE_CLEARED:
			StageCondition = NKMStageTempletV2.Find(OpenConditionValue);
			if (StageCondition == null)
			{
				NKMTempletError.Add($"[ContractTemplet] 선행조건 스테이지 아이디 오류:{OpenConditionValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletBase.cs", 84);
			}
			break;
		}
	}

	public abstract void Validate();

	public override string ToString()
	{
		return $"[{Key}] {ContractStrID}";
	}

	public DateTime GetDateEnd()
	{
		return baseData.EventIntervalTemplet.EndDate;
	}

	public DateTime GetDateStart()
	{
		return baseData.EventIntervalTemplet.StartDate;
	}

	public string GetContractName()
	{
		return NKCStringTable.GetString(ContractNameId);
	}

	public string GetContractDesc()
	{
		return NKCStringTable.GetString(ContractDescId);
	}

	public DateTime GetDateStartUtc()
	{
		return m_DateStartUtc;
	}

	public DateTime GetDateEndUtc()
	{
		return m_DateEndUtc;
	}

	public int GetOrder()
	{
		return Order;
	}

	public bool IsPickUp()
	{
		return PickUp;
	}

	public string GetMainBannerName()
	{
		return MainBannerName;
	}

	public static ContractTempletBase FindBase(int key)
	{
		return NKMTempletContainer<ContractTempletBase>.Find(key);
	}

	public static ContractTempletBase Find(string key)
	{
		return NKMTempletContainer<ContractTempletBase>.Find((ContractTempletBase x) => x.ContractStrID == key);
	}
}
