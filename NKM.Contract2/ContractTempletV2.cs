using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Contract;
using Cs.Core.Util;
using Cs.Math;
using Cs.Math.Lottery;
using Cs.Shared.Time;
using NKM.Contract2.Detail;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Contract2;

[SkipDerivedClassJoin]
public sealed class ContractTempletV2 : ContractTempletBase
{
	private const int RequirementItemCount = 3;

	private RandomUnitTempletV2 addUnit;

	public readonly MiscItemUnit[] m_SingleTryRequireItems = new MiscItemUnit[3];

	public readonly List<RewardUnit> m_ResultRewards = new List<RewardUnit>(3);

	public bool m_ContractGetUnitClose;

	public int m_FreeTryCnt;

	public int m_FreeTryEventCnt;

	public DateTime m_EventDateStart;

	public DateTime m_EventDateEnd;

	public int m_ContractBounsItemReqireCount;

	public int m_ContractBonusCountGroupID;

	public string m_RandomGradeId;

	public string m_UnitPoolId;

	public NKM_UNIT_TYPE m_NKM_UNIT_TYPE;

	public bool m_ClassifiedBtnBool;

	public NKM_SHORTCUT_TYPE m_ShortCutType;

	public string m_ShortCut = "";

	public int m_TotalLimit;

	public int m_DailyLimit;

	public bool m_expireDbData;

	public bool m_resetFreeCount;

	public int m_freeCountDays;

	public int CollectionMergeID;

	public int m_pickUnitLevel = 1;

	public int m_pickUnitLimits;

	public bool m_isMaxSkillLevelUnits;

	public int m_triggeredContractTimeLimit;

	public int m_extensionCondition;

	public static IEnumerable<ContractTempletV2> Values => NKMTempletContainer<ContractTempletV2>.Values;

	public RandomGradeTempletV2 RandomGradeTemplet { get; private set; }

	public IRandomUnitPool UnitPoolTemplet { get; private set; }

	public BonusGroupTemplet BonusGroupTemplet { get; private set; }

	public override bool IsClosingType
	{
		get
		{
			if (!m_ContractGetUnitClose)
			{
				return m_TotalLimit > 0;
			}
			return true;
		}
	}

	public DateTime ExpireDbDate
	{
		get
		{
			if (!m_expireDbData)
			{
				return ServiceTime.Forever;
			}
			return base.EventIntervalTemplet.EndDate;
		}
	}

	public bool IsFreeOnlyContract => m_SingleTryRequireItems.All((MiscItemUnit e) => e == null);

	public int EventCollectionMergeID => CollectionMergeID;

	public bool IsTriggeredContract => m_triggeredContractTimeLimit > 0;

	public int ExtensionCondition => m_extensionCondition;

	private ContractTempletV2(ContractBaseData baseData)
		: base(baseData)
	{
	}

	public static ContractTempletV2 LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 78))
		{
			return null;
		}
		if (!lua.GetData("m_ContractID", out var rValue, -1))
		{
			NKMTempletError.Add("[Contract] m_ContractID column loading failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 85);
			return null;
		}
		ContractBaseData contractBaseData = NKMTempletContainer<ContractBaseData>.Find(rValue);
		if (contractBaseData == null)
		{
			NKMTempletError.Add($"[Contract] ContractId not found in tabTamplet:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 92);
			return null;
		}
		ContractTempletV2 contractTempletV = new ContractTempletV2(contractBaseData);
		bool flag = true;
		for (int i = 0; i < 3; i++)
		{
			int num = i + 1;
			if ((1u & (lua.GetData($"m_SingleTryRequireItemID_{num}", out var rValue2, -1) ? 1u : 0u) & (lua.GetData($"m_SingleTryRequireItemValue_{num}", out var rValue3, -1) ? 1u : 0u)) != 0)
			{
				contractTempletV.m_SingleTryRequireItems[i] = new MiscItemUnit(rValue2, rValue3);
			}
		}
		lua.GetData("m_ContractGetUnitClose", ref contractTempletV.m_ContractGetUnitClose);
		lua.GetData("m_FreeTryCnt", ref contractTempletV.m_FreeTryCnt);
		lua.GetData("m_FreeTryEventCnt", ref contractTempletV.m_FreeTryEventCnt);
		lua.GetData("m_EventDateStart", ref contractTempletV.m_EventDateStart);
		lua.GetData("m_EventDateEnd", ref contractTempletV.m_EventDateEnd);
		lua.GetData("m_expireDbData", ref contractTempletV.m_expireDbData);
		NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
		int rValue4 = 0;
		int rValue5 = 0;
		if (lua.GetData("m_ContractResultRewardType_1", ref result))
		{
			if (!lua.GetData("m_ContractResultRewardID_1", ref rValue4) || !lua.GetData("m_ContractResultRewardValue_1", ref rValue5))
			{
				NKMTempletError.Add($"[{contractTempletV.Key}]{contractTempletV.ContractStrID} 보상 아이템 Type_1 로딩 오류.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 129);
				return null;
			}
			contractTempletV.m_ResultRewards.Add(new RewardUnit(result, rValue4, rValue5));
		}
		if (lua.GetData("m_ContractResultRewardType_2", ref result))
		{
			if (!lua.GetData("m_ContractResultRewardID_2", ref rValue4) || !lua.GetData("m_ContractResultRewardValue_2", ref rValue5))
			{
				NKMTempletError.Add($"[{contractTempletV.Key}]{contractTempletV.ContractStrID} 보상 아이템 로딩 오류.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 141);
				return null;
			}
			contractTempletV.m_ResultRewards.Add(new RewardUnit(result, rValue4, rValue5));
		}
		lua.GetData("m_ContractBounsItemReqireCount", ref contractTempletV.m_ContractBounsItemReqireCount);
		lua.GetData("m_ContractBonusCountGroupID", ref contractTempletV.m_ContractBonusCountGroupID);
		flag &= lua.GetData("m_RandomGradeID", ref contractTempletV.m_RandomGradeId);
		flag &= lua.GetData("m_UnitPoolID", ref contractTempletV.m_UnitPoolId);
		lua.GetData("m_ClassifiedBtnBool", ref contractTempletV.m_ClassifiedBtnBool);
		lua.GetData("m_ShortCutType", ref contractTempletV.m_ShortCutType);
		lua.GetData("m_ShortCut", ref contractTempletV.m_ShortCut);
		lua.GetData("m_TotalLimit", ref contractTempletV.m_TotalLimit);
		lua.GetData("m_DailyLimit", ref contractTempletV.m_DailyLimit);
		flag = lua.GetData("m_NKM_UNIT_TYPE", ref contractTempletV.m_NKM_UNIT_TYPE);
		lua.GetData("m_resetFreeCount", out contractTempletV.m_resetFreeCount, defValue: true);
		lua.GetData("m_freeCountDays", ref contractTempletV.m_freeCountDays);
		if (lua.GetData("m_addUnitStrId", out var rValue6, null) && !string.IsNullOrEmpty(rValue6))
		{
			lua.GetData("m_addUnitRatio", out var rValue7, 0);
			lua.GetData("m_addUnitPickUp", out var rbValue, defValue: false);
			lua.GetData("m_addUnitRatioUp", out var rbValue2, defValue: false);
			contractTempletV.addUnit = new RandomUnitTempletV2(rValue6, rValue7, rbValue, rbValue2, customPickupTarget: false);
		}
		lua.GetData("CollectionMergeID", ref contractTempletV.CollectionMergeID);
		lua.GetData("m_PickUnitLevel", ref contractTempletV.m_pickUnitLevel);
		lua.GetData("m_PickUnitLimits", ref contractTempletV.m_pickUnitLimits);
		lua.GetData("m_isMaxSkillLevelUnits", ref contractTempletV.m_isMaxSkillLevelUnits);
		lua.GetData("m_triggeredContractTimeLimit", ref contractTempletV.m_triggeredContractTimeLimit);
		lua.GetData("m_ExtensionCondition", ref contractTempletV.m_extensionCondition);
		if (!flag)
		{
			NKMTempletError.Add($"[{contractTempletV.Key}]{contractTempletV.ContractStrID} templet loading failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 184);
			return null;
		}
		return contractTempletV;
	}

	public override HashSet<int> GetPriceItemIDSet()
	{
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < m_SingleTryRequireItems.Length; i++)
		{
			if (m_SingleTryRequireItems[i] != null)
			{
				hashSet.Add(m_SingleTryRequireItems[i].ItemId);
			}
		}
		return hashSet;
	}

	public static ContractTempletV2 Find(int key)
	{
		return NKMTempletContainer<ContractTempletV2>.Find(key);
	}

	public bool IsActiveEvent()
	{
		if (m_EventDateStart != DateTime.MinValue)
		{
			return m_EventDateEnd != DateTime.MinValue;
		}
		return false;
	}

	public bool IsAvailableEventTime(DateTime currentServiceTime)
	{
		return currentServiceTime.IsBetween(m_EventDateStart, m_EventDateEnd);
	}

	public int CalculateTotalFreeCount(DateTime currentServiceTime)
	{
		if (m_resetFreeCount)
		{
			return m_FreeTryCnt;
		}
		if (m_FreeTryCnt == 0 || !base.EventIntervalTemplet.IsValidTime(currentServiceTime))
		{
			return 0;
		}
		DateTime current = base.EventIntervalTemplet.CalcStartDate(currentServiceTime);
		int num = 1;
		for (DateTime dateTime = DailyReset.CalcNextReset(current); dateTime <= currentServiceTime; dateTime += TimeSpan.FromDays(1.0))
		{
			num++;
		}
		int num2 = Math.Min(num, m_freeCountDays);
		return m_FreeTryCnt * num2;
	}

	public override void Join()
	{
		base.Join();
		for (int i = 0; i < 3; i++)
		{
			m_SingleTryRequireItems[i]?.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 251);
		}
		RandomGradeTemplet = RandomGradeTempletV2.Find(m_RandomGradeId);
		if (RandomGradeTemplet == null)
		{
			NKMTempletError.Add("[Contract] invalid randomGrade id:" + m_RandomGradeId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 258);
		}
		RandomUnitPoolTempletV2 randomUnitPoolTempletV = RandomUnitPoolTempletV2.Find(m_UnitPoolId);
		if (randomUnitPoolTempletV == null)
		{
			NKMTempletError.Add("[Contract] invalid UnitPool id:" + m_UnitPoolId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 264);
		}
		if (addUnit == null)
		{
			UnitPoolTemplet = randomUnitPoolTempletV;
		}
		else
		{
			addUnit.Join();
			UnitPoolTemplet = new RandomUnitPoolIntrusive(randomUnitPoolTempletV, addUnit);
		}
		if (m_ContractBonusCountGroupID > 0)
		{
			if (m_ContractBounsItemReqireCount <= 0)
			{
				NKMTempletError.Add($"[{base.Key}]{base.ContractStrID} 천장 횟수 오류:{m_ContractBounsItemReqireCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 281);
			}
			if (!NKMTempletContainer<BonusGroupTemplet>.TryGetValue(m_ContractBonusCountGroupID, out var result))
			{
				result = new BonusGroupTemplet(m_ContractBonusCountGroupID, m_ContractBounsItemReqireCount);
				NKMTempletContainer<BonusGroupTemplet>.Add(result, null);
			}
			result.Add(this);
			BonusGroupTemplet = result;
		}
	}

	public override void Validate()
	{
		if (!base.EnableByTag)
		{
			return;
		}
		if (base.TriggerShopProductId != 0)
		{
			if (ShopItemTemplet.Find(base.TriggerShopProductId) == null)
			{
				NKMTempletError.Add($"[Contract:{base.DebugName}] ShopProductId Trigger 대상 상품 정보가 없음 - m_triggerShopProductId[{base.TriggerShopProductId}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 307);
			}
			if (m_triggeredContractTimeLimit == 0)
			{
				NKMTempletError.Add($"[Contract:{base.DebugName}] ShopProductId Trigger 대상 상품에 제한시간이 없음 - m_triggerShopProductId[{base.TriggerShopProductId}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 312);
			}
		}
		if (base.TriggerStageClearId != 0 && m_triggeredContractTimeLimit == 0)
		{
			NKMTempletError.Add($"[Contract:{base.DebugName}] StageClear Trigger 대상 상품에 제한시간이 없음 - m_triggerStageClearId[{base.TriggerStageClearId}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 320);
		}
		foreach (RewardUnit resultReward in m_ResultRewards)
		{
			resultReward.Validate("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 326);
		}
		if (UnitPoolTemplet is RandomUnitPoolIntrusive)
		{
			UnitPoolTemplet.ValidateCommon();
			if (addUnit.Ratio <= 0)
			{
				NKMTempletError.Add("[Contract:" + base.DebugName + "] 추가하는 유닛의 확률 가중치값 오류. addUnitId:" + addUnit.UnitTemplet.DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 335);
			}
		}
		if (IsActiveEvent() && m_EventDateStart >= m_EventDateEnd)
		{
			NKMTempletError.Add($"[Contract] invalid date. start:{m_EventDateStart} end:{m_EventDateEnd}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 341);
		}
		foreach (NKM_UNIT_PICK_GRADE value in EnumUtil<NKM_UNIT_PICK_GRADE>.GetValues())
		{
			RatioLottery<RandomUnitTempletV2> lottery = UnitPoolTemplet.GetLottery(value);
			if (!RandomGradeTemplet.Lottery.TryGetRatePercent(value, out var ratePercent) && lottery.Count == 0)
			{
				continue;
			}
			if (lottery.Count == 0)
			{
				NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 등급 확률이 있으나 유닛 풀이 비어있음. pickGrade:{value} gradeRate:{ratePercent:0.00}% gradePoolId:{m_RandomGradeId} unitPoolId:{m_UnitPoolId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 355);
			}
			if (ratePercent.IsNearlyZero())
			{
				NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 유닛이 있는데 등급 확률이 설정되지 않음. pickGrade:{value} unitCount:{lottery.Count} gradePoolId:{m_RandomGradeId} unitPoolId:{m_UnitPoolId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 360);
			}
			foreach (RandomUnitTempletV2 caseValue in lottery.CaseValues)
			{
				caseValue.CalcFinalRate(ratePercent, lottery.TotalRatio);
			}
		}
		if (BonusGroupTemplet != null && BonusGroupTemplet.RequireTryCount != m_ContractBounsItemReqireCount)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID} 천장 횟수 불일치. {m_ContractBounsItemReqireCount} != {BonusGroupTemplet.RequireTryCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 371);
		}
		if (m_ContractGetUnitClose && BonusGroupTemplet == null)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 획득 완료형 채용상품은 천장 그룹을 반드시 가져야 함", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 376);
		}
		if (base.PreviousContract != null && !base.PreviousContract.IsClosingType)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 선행조건으로 설정한 채용이 완료형 타입이 아님. prevContract:{base.PreviousContract.ContractStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 381);
		}
		if (BonusGroupTemplet != null && !RandomGradeTemplet.Lottery.HasValue(NKM_UNIT_PICK_GRADE.NUPG_SSR_PICK))
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 천장이 있는 채용은 SSR_PICK 확률 필수. gradeTemplet:{RandomGradeTemplet.StringId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 386);
		}
		if (m_TotalLimit > 0 && m_DailyLimit > 0)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 횟수제한은 전체제한/일일제한 둘 중 하나만 설정 가능. totalLimit:{m_TotalLimit} dailyLimit:{m_DailyLimit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 391);
		}
		foreach (RandomUnitTempletV2 unitTemplet in UnitPoolTemplet.UnitTemplets)
		{
			if (unitTemplet.UnitTemplet.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_INVALID)
			{
				NKMTempletError.Add($"잘못된 유닛 풀 정보. 메인 채용:{m_NKM_UNIT_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 398);
			}
			if (unitTemplet.UnitTemplet.m_NKM_UNIT_TYPE != m_NKM_UNIT_TYPE)
			{
				NKMTempletError.Add($"잘못된 유닛 풀 정보. 메인 채용:{m_NKM_UNIT_TYPE} 구성유닛:{unitTemplet.UnitTemplet.m_NKM_UNIT_TYPE} 유닛 아이디:{unitTemplet.UnitTemplet.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 403);
			}
		}
		if (m_expireDbData && BonusGroupTemplet != null && BonusGroupTemplet.ShareCount > 1)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 천장 카운트를 공유하는 채용은 db 데이터 초기화 설정 불가. bonusGroupId:{m_ContractBonusCountGroupID} shareCount:{BonusGroupTemplet.ShareCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 409);
		}
		if (m_resetFreeCount && m_freeCountDays > 0)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 무료카운트를 리셋하는 상품은 지급기간을 지정할 수 없음. m_resetFreeCount:{m_resetFreeCount} m_freeCountDays:{m_freeCountDays}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 414);
		}
		if (!m_resetFreeCount && m_freeCountDays <= 0)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 무료카운트를 리셋하지 않는 상품은 지급기간을 지정해야 함. m_resetFreeCount:{m_resetFreeCount} m_freeCountDays:{m_freeCountDays}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 419);
		}
		if (!m_resetFreeCount && m_FreeTryCnt <= 0)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 무료카운트를 리셋하지 않는 상품은 무료카운트를 지정해야 함. m_resetFreeCount:{m_resetFreeCount} m_FreeTryCnt:{m_FreeTryCnt}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 424);
		}
		if (m_freeCountDays > 0 && m_freeCountDays > base.EventIntervalTemplet.GetDays())
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 무료카운트 지급기간이 채용 진행기간보다 길 수 없음. m_freeCountDays:{m_freeCountDays} date:{base.EventIntervalTemplet}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 429);
		}
		if (IsFreeOnlyContract && m_FreeTryCnt <= 0)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 무료 전용 채용에 무료카운트가 없음. m_FreeTryCnt:{m_FreeTryCnt} m_freeCountDays:{m_freeCountDays}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 435);
		}
		if (m_pickUnitLimits < 3 && m_isMaxSkillLevelUnits)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 이벤트 채용(100레벨)의 스킬레벨을 올리려면 초월이 3이상 이어야 함. m_PickUnitLimits:{m_pickUnitLimits} m_isMaxSkillLevelUnits:{m_isMaxSkillLevelUnits}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 441);
		}
		int pickUnitLimits = m_pickUnitLimits;
		if ((uint)(pickUnitLimits - 4) <= 4u)
		{
			int num = (m_pickUnitLimits - 3) * 2 + 100;
			if (m_pickUnitLevel > num)
			{
				NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 이벤트 채용의 레벨과 한계 초월 레벨이 올바르지 않음.  m_PickUnitLimits:{m_pickUnitLimits} m_PickUnitLevel:{m_pickUnitLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 455);
			}
		}
		else if (m_pickUnitLevel > 100)
		{
			NKMTempletError.Add($"[{base.Key}]{base.ContractStrID}: 이벤트 채용의 레벨과 한계 초월 레벨이 올바르지 않음.  m_PickUnitLimits:{m_pickUnitLimits} m_PickUnitLevel:{m_pickUnitLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/ContractTempletV2.cs", 462);
		}
	}

	public void CheckValid()
	{
	}

	public MiscItemUnit GetRequireItem(ContractCostType costType)
	{
		int num = costType switch
		{
			ContractCostType.FreeChance => throw new Exception($"[Contract] invalid costType:{costType}"), 
			ContractCostType.Ticket => 0, 
			_ => 1, 
		};
		return m_SingleTryRequireItems[num];
	}

	public void RecalculateIntrusivePool()
	{
		if (UnitPoolTemplet is RandomUnitPoolIntrusive randomUnitPoolIntrusive)
		{
			randomUnitPoolIntrusive.Recalculate();
		}
	}
}
