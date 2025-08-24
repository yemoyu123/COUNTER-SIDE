using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Contract;
using Cs.Core.Util;
using Cs.Math;
using Cs.Math.Lottery;
using NKC;
using NKM.Contract2.Detail;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Contract2;

public class CustomPickupContractTemplet : INKMTemplet, INKMTempletEx
{
	public enum CUSTOM_PICK_UP_TYPE
	{
		BASIC = 10,
		AWAKEN = 20,
		OPERATOR = 30
	}

	private const int RequirementItemCount = 3;

	private int customPickupId;

	private string openTag;

	private string dateStrId;

	private int unitPoolId;

	private int maxSelectTargetCount;

	private int contractBounsItemReqireCount;

	private string randomGradeId;

	private bool checkReturningUser;

	private ReturningUserType returningUserType;

	private int returningUserStatePeriod;

	private int contractBonusCountGroupId;

	private CUSTOM_PICK_UP_TYPE customPickupType;

	private int triggerShopProductId;

	private int triggeredContractTimeLimit;

	public int m_Order;

	public string m_ContractStrID;

	public string m_ContractName;

	public string m_MainBannerFileName;

	public int m_ContractCategory;

	public string m_ContractBannerDesc;

	public string m_Image;

	public int m_Priority;

	public readonly MiscItemUnit[] SingleTryRequireItems = new MiscItemUnit[3];

	public readonly List<RewardUnit> ResultRewards = new List<RewardUnit>(3);

	private int m_TotalUseCount;

	private int m_PickupTargetUnitId;

	private int m_CurSelectCount;

	public int Key => customPickupId;

	public RandomGradeTempletV2 RandomGradeTemplet { get; private set; }

	public RandomUnitPoolTempletV2 UnitPoolTemplet { get; private set; }

	public NKMIntervalTemplet IntervalTemplet { get; private set; } = NKMIntervalTemplet.Invalid;

	public static IEnumerable<CustomPickupContractTemplet> Values => NKMTempletContainer<CustomPickupContractTemplet>.Values;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public DateTime EndDate => IntervalTemplet.EndDate;

	public bool MissionCountIgnore => false;

	public bool CheckReturningUser => checkReturningUser;

	public ReturningUserType ReturningUserType => returningUserType;

	public int ReturningUserStatePeriod => returningUserStatePeriod;

	public int ContractBounsItemReqireCount => contractBounsItemReqireCount;

	public int ContractBonusCountGroupID => contractBonusCountGroupId;

	public CUSTOM_PICK_UP_TYPE CustomPickUpType => customPickupType;

	public BonusGroupTemplet BonusGroupTemplet { get; private set; }

	public int TriggeredContractTimeLimit => triggeredContractTimeLimit;

	public int PickUpTargetUnitID => m_PickupTargetUnitId;

	public int CurrentSelectCount => m_CurSelectCount;

	public int MaxSelectCount => maxSelectTargetCount;

	public int CurrentBonusCount => m_TotalUseCount;

	public int MaxBonusReqoreCount => contractBounsItemReqireCount;

	public DateTime EndDateUtc => ServiceTime.ToUtcTime(IntervalTemplet.EndDate);

	public int LeftCountUntilConfirm => contractBounsItemReqireCount - m_TotalUseCount;

	public int TriggetShopProductID => triggerShopProductId;

	public int Category => m_ContractCategory;

	public string ImageName => m_Image;

	public int Priority => m_Priority;

	public bool IsAvailableTime(DateTime currentServiceTime)
	{
		return IntervalTemplet.IsValidTime(currentServiceTime);
	}

	public bool CanSelectTarget(int currentCount)
	{
		return currentCount + 1 <= maxSelectTargetCount;
	}

	public bool isBonusReqCount(int currentCount)
	{
		return currentCount + 1 >= contractBounsItemReqireCount;
	}

	public static CustomPickupContractTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 79))
		{
			return null;
		}
		CustomPickupContractTemplet customPickupContractTemplet = new CustomPickupContractTemplet();
		lua.GetData("customPickupId", ref customPickupContractTemplet.customPickupId);
		lua.GetData("m_OpenTag", ref customPickupContractTemplet.openTag);
		lua.GetData("m_DateStrID", ref customPickupContractTemplet.dateStrId);
		lua.GetData("m_UnitPoolID", ref customPickupContractTemplet.unitPoolId);
		lua.GetData("maxSelectTargetCount", ref customPickupContractTemplet.maxSelectTargetCount);
		lua.GetData("m_ContractBounsItemReqireCount", ref customPickupContractTemplet.contractBounsItemReqireCount);
		lua.GetData("m_RandomGradeID", ref customPickupContractTemplet.randomGradeId);
		lua.GetData("m_CheckReturningUser", ref customPickupContractTemplet.checkReturningUser);
		lua.GetData("m_ReturningUserType", ref customPickupContractTemplet.returningUserType);
		lua.GetData("m_ReturningUserStatePeriod", ref customPickupContractTemplet.returningUserStatePeriod);
		lua.GetData("ContractBonusCountGroupID", ref customPickupContractTemplet.contractBonusCountGroupId);
		lua.GetData("m_ContractType", ref customPickupContractTemplet.customPickupType);
		lua.GetData("m_Order", ref customPickupContractTemplet.m_Order);
		lua.GetData("m_ContractCategory", ref customPickupContractTemplet.m_ContractCategory);
		lua.GetData("m_Priority", ref customPickupContractTemplet.m_Priority);
		lua.GetData("m_ContractStrID", ref customPickupContractTemplet.m_ContractStrID);
		lua.GetData("m_ContractName", ref customPickupContractTemplet.m_ContractName);
		lua.GetData("m_MainBannerFileName", ref customPickupContractTemplet.m_MainBannerFileName);
		lua.GetData("m_ContractBannerDesc", ref customPickupContractTemplet.m_ContractBannerDesc);
		lua.GetData("m_Image", ref customPickupContractTemplet.m_Image);
		for (int i = 0; i < 3; i++)
		{
			int num = i + 1;
			if ((1u & (lua.GetData($"m_SingleTryRequireItemID_{num}", out var rValue, -1) ? 1u : 0u) & (lua.GetData($"m_SingleTryRequireItemValue_{num}", out var rValue2, -1) ? 1u : 0u)) != 0)
			{
				customPickupContractTemplet.SingleTryRequireItems[i] = new MiscItemUnit(rValue, rValue2);
			}
		}
		NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
		int rValue3 = 0;
		int rValue4 = 0;
		if (lua.GetData("m_ContractResultRewardType_1", ref result))
		{
			if (!lua.GetData("m_ContractResultRewardID_1", ref rValue3) || !lua.GetData("m_ContractResultRewardValue_1", ref rValue4))
			{
				NKMTempletError.Add($"[{customPickupContractTemplet.Key}] 보상 아이템 Type_1 로딩 오류.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 135);
				return null;
			}
			customPickupContractTemplet.ResultRewards.Add(new RewardUnit(result, rValue3, rValue4));
		}
		if (lua.GetData("m_ContractResultRewardType_2", ref result))
		{
			if (!lua.GetData("m_ContractResultRewardID_2", ref rValue3) || !lua.GetData("m_ContractResultRewardValue_2", ref rValue4))
			{
				NKMTempletError.Add($"[{customPickupContractTemplet.Key}] 보상 아이템 로딩 오류.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 147);
				return null;
			}
			customPickupContractTemplet.ResultRewards.Add(new RewardUnit(result, rValue3, rValue4));
		}
		if (lua.GetData("m_triggerShopProductId", ref customPickupContractTemplet.triggerShopProductId))
		{
			ContractTempletLoader.AddTriggerShopProductId(customPickupContractTemplet.triggerShopProductId, customPickupContractTemplet.customPickupId);
			lua.GetData("m_triggeredContractTimeLimit", ref customPickupContractTemplet.triggeredContractTimeLimit);
		}
		return customPickupContractTemplet;
	}

	public static CustomPickupContractTemplet Find(int key)
	{
		return NKMTempletContainer<CustomPickupContractTemplet>.Find(key);
	}

	public void Join()
	{
		if (!EnableByTag)
		{
			return;
		}
		if (string.IsNullOrEmpty(dateStrId))
		{
			NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 잘못된 dateStrId:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 177);
		}
		else
		{
			IntervalTemplet = NKMIntervalTemplet.Find(dateStrId);
		}
		UnitPoolTemplet = RandomUnitPoolTempletV2.Find(unitPoolId);
		if (UnitPoolTemplet == null)
		{
			NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 유닛 풀 정보를 찾지 못함. unitPoolId:{unitPoolId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 187);
		}
		RandomGradeTemplet = RandomGradeTempletV2.Find(randomGradeId);
		if (RandomGradeTemplet == null)
		{
			NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 잘못된 randomGradeId. randomGradeId:{randomGradeId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 194);
		}
		if (ContractBonusCountGroupID > 0)
		{
			if (ContractBounsItemReqireCount <= 0)
			{
				NKMTempletError.Add($"[{Key}] 천장 횟수 오류:{ContractBounsItemReqireCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 201);
			}
			if (!NKMTempletContainer<BonusGroupTemplet>.TryGetValue(ContractBonusCountGroupID, out var result))
			{
				result = new BonusGroupTemplet(ContractBonusCountGroupID, ContractBounsItemReqireCount);
				NKMTempletContainer<BonusGroupTemplet>.Add(result, null);
			}
			result.Add(this);
			BonusGroupTemplet = result;
		}
	}

	public void Validate()
	{
		if (!EnableByTag)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			SingleTryRequireItems[i]?.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 225);
		}
		foreach (RewardUnit resultReward in ResultRewards)
		{
			resultReward.Validate("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 231);
		}
		if (IntervalTemplet == null)
		{
			NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] IntervalTemplet을 찾지 못함. dateStrId:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 237);
		}
		else if (IntervalTemplet.IsRepeatDate)
		{
			NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] IntervalTemplet에 반복 기간 설정은 불가. dateStrId:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 241);
		}
		if (UnitPoolTemplet == null)
		{
			NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 유닛 풀 정보가 없음. unitPoolId:{unitPoolId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 246);
			return;
		}
		foreach (NKM_UNIT_PICK_GRADE value in EnumUtil<NKM_UNIT_PICK_GRADE>.GetValues())
		{
			if (value == NKM_UNIT_PICK_GRADE.NUPG_SSR_PICK)
			{
				continue;
			}
			RatioLottery<RandomUnitTempletV2> lottery = UnitPoolTemplet.GetLottery(value);
			if (!RandomGradeTemplet.Lottery.TryGetRatePercent(value, out var ratePercent) && lottery.Count == 0)
			{
				continue;
			}
			if (lottery.Count == 0)
			{
				NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 등급 확률이 있으나 유닛 풀이 비어있음. pickGrade:{value} gradeRate:{ratePercent:0.00}% gradePoolId:{randomGradeId} unitPoolId:{unitPoolId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 268);
			}
			if (ratePercent.IsNearlyZero())
			{
				NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 유닛이 있는데 등급 확률이 설정되지 않음. pickGrade:{value} unitCount:{lottery.Count} gradePoolId:{randomGradeId} unitPoolId:{unitPoolId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 273);
			}
			foreach (RandomUnitTempletV2 caseValue in lottery.CaseValues)
			{
				caseValue.CalcFinalRate(ratePercent, lottery.TotalRatio);
			}
		}
		if (contractBounsItemReqireCount <= 0)
		{
			NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 천장 횟수 오류:{contractBounsItemReqireCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 284);
		}
		if (NKMTempletContainer<ContractBaseData>.Find(Key) != null)
		{
			NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 커스텀 픽업 템플릿 id와 같은 contract_tab_templet id가 존재.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 289);
		}
		if (!UnitPoolTemplet.ValidateCustomPickupUnit())
		{
			NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 채용 유닛 목록 중 커스텀 픽업으로 선택 가능한 유닛이 없음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 294);
		}
		if (checkReturningUser)
		{
			_ = returningUserStatePeriod;
		}
		if (triggerShopProductId > 0)
		{
			if (NKMTempletContainer<ShopItemTemplet>.Find(triggerShopProductId) == null)
			{
				NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 상점으로 개방되는 커스텀 픽업의 상품 정보를 찾을 수 없는 경우 triggerShopProductId:{triggerShopProductId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 319);
			}
			if (triggeredContractTimeLimit <= 0)
			{
				NKMTempletError.Add($"[CustomPickupContractTemplet:{Key}] 상점으로 개방되는 커스텀 픽업에 종료 days 값이 비정상인 경우. triggerShopProductId:{triggerShopProductId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/CustomPickupContractTemplet.cs", 324);
			}
		}
	}

	public MiscItemUnit GetRequireItem(ContractCostType costType)
	{
		int num = costType switch
		{
			ContractCostType.FreeChance => throw new Exception($"[Contract] invalid costType:{costType}"), 
			ContractCostType.Ticket => 0, 
			_ => 1, 
		};
		return SingleTryRequireItems[num];
	}

	public RandomUnitPoolTempletV2 GetUnitPoolTemplet(List<int> lstExculudeUnits = null)
	{
		RandomUnitPoolTempletV2 randomUnitPoolTempletV = (UnitPoolTemplet = RandomUnitPoolTempletV2.Find(unitPoolId));
		RandomUnitPoolTempletV2 randomUnitPoolTempletV3 = randomUnitPoolTempletV;
		if (randomUnitPoolTempletV3 != null)
		{
			randomUnitPoolTempletV3.RecalculateUnitTemplets(lstExculudeUnits);
			return randomUnitPoolTempletV3;
		}
		return null;
	}

	public bool IsSelectedPickUpUnit()
	{
		return m_PickupTargetUnitId != 0;
	}

	public bool CanSelectUnit()
	{
		return CurrentSelectCount < MaxSelectCount;
	}

	public static CustomPickupContractTemplet Find(string key)
	{
		return NKMTempletContainer<CustomPickupContractTemplet>.Find((CustomPickupContractTemplet e) => string.Equals(e.m_ContractStrID, key));
	}

	public string GetContractName()
	{
		return NKCStringTable.GetString(m_ContractName);
	}

	public int GetOrder()
	{
		return m_Order;
	}

	public string GetMainBannerName()
	{
		return m_MainBannerFileName;
	}

	public void PostJoin()
	{
		Join();
	}

	public void UpdateData(NKMCustomPickupContract customPickUpData)
	{
		if (customPickUpData != null)
		{
			UpdateData(customPickUpData.totalUseCount, customPickUpData.customPickupTargetUnitId, customPickUpData.currentSelectCount);
		}
	}

	public void UpdateData(int totalUseCnt, int pickUpTargetUnitID, int curSelectCnt)
	{
		UpdateTotalUseCount(totalUseCnt);
		m_PickupTargetUnitId = pickUpTargetUnitID;
		m_CurSelectCount = curSelectCnt;
	}

	public void UpdateTotalUseCount(int totalUseCnt)
	{
		m_TotalUseCount = totalUseCnt;
	}

	public static CustomPickupContractTemplet GetDummyCalculateTemplet(int baseTempletID, int iPickUpTargetUnitID)
	{
		CustomPickupContractTemplet customPickupContractTemplet = Find(baseTempletID);
		if (customPickupContractTemplet != null)
		{
			RatioLottery<RandomUnitTempletV2>[] customPickupLotteries = customPickupContractTemplet.UnitPoolTemplet.GetCustomPickupLotteries(customPickupContractTemplet, iPickUpTargetUnitID);
			{
				foreach (NKM_UNIT_PICK_GRADE value in EnumUtil<NKM_UNIT_PICK_GRADE>.GetValues())
				{
					if (customPickupLotteries.Count() <= (int)value)
					{
						continue;
					}
					RatioLottery<RandomUnitTempletV2> ratioLottery = customPickupLotteries[(int)value];
					if (!customPickupContractTemplet.RandomGradeTemplet.Lottery.TryGetRatePercent(value, out var ratePercent) && ratioLottery.Count == 0)
					{
						continue;
					}
					if (iPickUpTargetUnitID == 0 && value == NKM_UNIT_PICK_GRADE.NUPG_SSR)
					{
						customPickupContractTemplet.RandomGradeTemplet.Lottery.TryGetRatePercent(NKM_UNIT_PICK_GRADE.NUPG_SSR_PICK, out var ratePercent2);
						ratePercent += ratePercent2;
					}
					foreach (RandomUnitTempletV2 caseValue in ratioLottery.CaseValues)
					{
						caseValue.CalcFinalRate(ratePercent, ratioLottery.TotalRatio);
					}
				}
				return customPickupContractTemplet;
			}
		}
		return null;
	}
}
