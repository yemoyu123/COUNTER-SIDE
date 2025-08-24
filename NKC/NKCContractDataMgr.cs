using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Contract;
using Cs.Core.Util;
using Cs.Shared.Time;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKCContractDataMgr
{
	private List<NKMContractState> m_lstContractState = new List<NKMContractState>();

	private bool m_bIsPossibleFreeContract;

	private DateTime m_FreeContractResetTime;

	private bool m_bCanFreeChance;

	private List<NKMContractBonusState> m_lstBonusState = new List<NKMContractBonusState>();

	private NKMSelectableContractState m_SelectableContractState;

	private List<NKMInstantContract> m_lstInstantContract;

	public bool PossibleFreeContract => m_bIsPossibleFreeContract;

	public static NKM_ERROR_CODE CanTryContract(NKMUserData userData, ContractTempletV2 templet, ContractCostType costType, int tryCount)
	{
		if (userData == null)
		{
			return NKM_ERROR_CODE.NEC_DB_FAIL_USER_DATA;
		}
		if (templet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_CONTRACT_TEMPLET_NULL;
		}
		if (costType == ContractCostType.FreeChance)
		{
			return NKM_ERROR_CODE.NEC_OK;
		}
		MiscItemUnit[] singleTryRequireItems = templet.m_SingleTryRequireItems;
		bool flag = false;
		MiscItemUnit[] array = singleTryRequireItems;
		foreach (MiscItemUnit miscItemUnit in array)
		{
			if (userData.m_InventoryData.GetCountMiscItem(miscItemUnit.ItemId) >= miscItemUnit.Count * tryCount)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_RESOURCE;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	private bool IsContractFinished(ContractTempletBase templet)
	{
		if (templet is ContractTempletV2 && templet is ContractTempletV2 contractTempletV)
		{
			NKMContractState contractState = GetContractState(contractTempletV.Key);
			if (contractState != null)
			{
				if (contractTempletV.m_TotalLimit > 0 && contractState.totalUseCount >= contractTempletV.m_TotalLimit)
				{
					return true;
				}
				if (contractTempletV.m_ContractGetUnitClose && GetContractBonusResetCnt(contractTempletV.Key) > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CheckOpenCond(int iContractID)
	{
		ContractTempletBase contractTempletBase = ContractTempletBase.FindBase(iContractID);
		if (contractTempletBase == null)
		{
			return false;
		}
		return CheckOpenCond(contractTempletBase);
	}

	public bool CheckOpenCond(ContractTempletBase templet)
	{
		if (templet == null)
		{
			return false;
		}
		if (!templet.EnableByTag)
		{
			return false;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		if (templet.CheckReturningUser && !myUserData.IsReturnUser(templet.ReturningUserType))
		{
			return false;
		}
		if (NKCSynchronizedTime.IsFinished(templet.GetDateEndUtc()) || !NKCSynchronizedTime.IsEventTime(templet.GetDateStartUtc(), templet.GetDateEndUtc()))
		{
			return false;
		}
		if (templet is SelectableContractTemplet)
		{
			NKMSelectableContractState selectableContractState = GetSelectableContractState();
			if (selectableContractState != null && !selectableContractState.isActive)
			{
				return false;
			}
		}
		if (templet is ContractTempletV2 && templet is ContractTempletV2 contractTempletV)
		{
			NKMContractState contractState = GetContractState(contractTempletV.Key);
			if (contractState != null)
			{
				if (IsContractFinished(contractTempletV))
				{
					return false;
				}
				if (contractTempletV.m_DailyLimit > 0 && contractState.dailyUseCount >= contractTempletV.m_DailyLimit)
				{
					if ((contractTempletV.GetDateEndUtc() - contractState.nextResetDate).TotalSeconds <= 0.0)
					{
						return false;
					}
				}
				else if (contractTempletV.IsFreeOnlyContract && !contractTempletV.m_resetFreeCount && contractTempletV.m_FreeTryCnt * contractTempletV.m_freeCountDays <= contractState.totalUseCount)
				{
					return false;
				}
			}
			if (contractTempletV.IsTriggeredContract && NKCSynchronizedTime.IsFinished(NKCSynchronizedTime.ToUtcTime(GetInstantContractEndDateTime(templet.Key))))
			{
				return false;
			}
			if (contractTempletV.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR && (!NKMContentsVersionManager.HasTag("OPERATOR") || !NKCContentManager.IsContentsUnlocked(ContentsType.OPERATOR)))
			{
				return false;
			}
		}
		switch (templet.OpenCondition)
		{
		case CONTRACT_CONDITION.CONTRACT_FINISH:
		{
			ContractTempletV2 contractTempletV2 = ContractTempletV2.Find(templet.OpenConditionValue);
			if (contractTempletV2 != null)
			{
				if (IsContractFinished(contractTempletV2))
				{
					return true;
				}
			}
			else if (SelectableContractTemplet.Find(templet.Key) != null)
			{
				NKMSelectableContractState selectableContractState2 = GetSelectableContractState();
				if (selectableContractState2 == null || selectableContractState2.contractId != templet.Key)
				{
					return true;
				}
				return selectableContractState2.isActive;
			}
			Debug.Log($"name : {templet.GetContractName()}에 CONTRACT_FINISH - OpenConditionValue({templet.OpenConditionValue})를 확인할 수 없습니다.");
			return false;
		}
		case CONTRACT_CONDITION.WARFARE_CLEARED:
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(templet.OpenConditionValue);
			if (nKMWarfareTemplet != null)
			{
				return myUserData.CheckWarfareClear(nKMWarfareTemplet.m_WarfareStrID);
			}
			Debug.Log($"name : {templet.GetContractName()}에 WARFARE_CLEARED - OpenConditionValue({templet.OpenConditionValue})를 확인할 수 없습니다.");
			return false;
		}
		case CONTRACT_CONDITION.STAGE_CLEARED:
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(templet.OpenConditionValue);
			if (nKMStageTempletV != null)
			{
				return myUserData.CheckStageCleared(nKMStageTempletV.Key);
			}
			Debug.Log($"name : {templet.GetContractName()}에 STAGE_CLEARED - OpenConditionValue({templet.OpenConditionValue})를 확인할 수 없습니다.");
			return false;
		}
		default:
			return true;
		}
	}

	public void ResetContractState()
	{
		m_lstContractState = new List<NKMContractState>();
	}

	public void SetContractState(List<NKMContractState> contractState)
	{
		m_lstContractState.Clear();
		if (contractState.Count > 0)
		{
			m_lstContractState = contractState;
			UpdateContractFreeTryState();
		}
	}

	public void UpdateContractState(NKMContractState contractState)
	{
		if (contractState == null)
		{
			return;
		}
		bool flag = false;
		foreach (NKMContractState item in m_lstContractState)
		{
			if (item.contractId == contractState.contractId)
			{
				item.nextResetDate = contractState.nextResetDate;
				item.remainFreeChance = contractState.remainFreeChance;
				item.isActive = contractState.isActive;
				item.dailyUseCount = contractState.dailyUseCount;
				item.totalUseCount = contractState.totalUseCount;
				item.bonusCandidate = contractState.bonusCandidate;
				flag = true;
				Debug.Log($"id : {contractState.contractId} / remainFreeChance : {item.remainFreeChance} / totalUseCount : {item.totalUseCount}");
				break;
			}
		}
		if (!flag)
		{
			m_lstContractState.Add(contractState);
		}
		UpdateContractFreeTryState();
	}

	private void UpdateContractFreeTryState()
	{
		m_bCanFreeChance = false;
		m_FreeContractResetTime = DateTime.MinValue;
		_ = ServiceTime.Recent;
		foreach (ContractTempletV2 value in ContractTempletV2.Values)
		{
			if (CheckOpenCond(value) && NKCSynchronizedTime.IsEventTime(value.GetDateStartUtc(), value.GetDateEndUtc()) && value.m_FreeTryCnt > 0)
			{
				NKMContractState contractState = GetContractState(value.Key);
				if (contractState == null || contractState.remainFreeChance > 0 || (!NKCSynchronizedTime.IsFinished(value.GetDateStartUtc().AddDays(value.m_freeCountDays)) && NKCSynchronizedTime.IsFinished(contractState.nextResetDate)))
				{
					m_bIsPossibleFreeContract = true;
					m_bCanFreeChance = true;
				}
			}
		}
		m_FreeContractResetTime = GetFixedResetTime();
	}

	private DateTime GetFixedResetTime()
	{
		return NKMTime.GetNextResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Day);
	}

	public bool IsActiveContrctConfirmation(int contractId)
	{
		NKMContractState contractState = GetContractState(contractId);
		if (contractState != null)
		{
			return contractState.isActive;
		}
		ContractTempletV2 contractTempletV = ContractTempletV2.Find(contractId);
		if (contractTempletV != null)
		{
			return contractTempletV.m_ContractBonusCountGroupID != 0;
		}
		return false;
	}

	public int GetRemainFreeChangeCnt(int contractId)
	{
		NKMContractState contractState = GetContractState(contractId);
		if (contractState != null)
		{
			return contractState.remainFreeChance;
		}
		ContractTempletV2 contractTempletV = ContractTempletV2.Find(contractId);
		if (contractTempletV != null)
		{
			if (contractTempletV.m_resetFreeCount)
			{
				return contractTempletV.m_FreeTryCnt;
			}
			DateTime current = contractTempletV.EventIntervalTemplet.CalcStartDate(ServiceTime.Recent);
			int num = 1;
			for (DateTime dateTime = DailyReset.CalcNextReset(current); dateTime <= ServiceTime.Recent; dateTime += TimeSpan.FromDays(1.0))
			{
				num++;
			}
			int num2 = Math.Min(num, contractTempletV.m_freeCountDays);
			return contractTempletV.m_FreeTryCnt * num2;
		}
		return 0;
	}

	public bool IsHasContractStateData(int contractId)
	{
		return GetContractState(contractId) != null;
	}

	public bool IsActiveNextFreeChance(int contractId)
	{
		NKMContractState contractState = GetContractState(contractId);
		if (contractState != null && NKCSynchronizedTime.GetTimeLeft(contractState.nextResetDate).Ticks <= 0)
		{
			return true;
		}
		return false;
	}

	public DateTime GetNextResetTime(int contractID)
	{
		return GetContractState(contractID)?.nextResetDate ?? DateTime.MinValue;
	}

	private NKMContractState GetContractState(int contractId)
	{
		if (m_lstContractState == null)
		{
			return null;
		}
		return m_lstContractState.Find((NKMContractState x) => x.contractId == contractId);
	}

	public List<int> GetCurSelectableUnitList(int contractID)
	{
		NKMContractState contractState = GetContractState(contractID);
		if (contractState != null)
		{
			return contractState.bonusCandidate;
		}
		return new List<int>();
	}

	public bool IsPossibleFreeChance()
	{
		if (m_bCanFreeChance || IsActiveFreeContract())
		{
			return true;
		}
		return false;
	}

	private bool IsActiveFreeContract()
	{
		bool result = false;
		_ = ServiceTime.Recent;
		foreach (ContractTempletV2 value in ContractTempletV2.Values)
		{
			if (CheckOpenCond(value) && NKCSynchronizedTime.IsEventTime(value.GetDateStartUtc(), value.GetDateEndUtc()) && value.m_FreeTryCnt > 0)
			{
				NKMContractState contractState = GetContractState(value.Key);
				if (contractState == null || contractState.remainFreeChance > 0 || (!NKCSynchronizedTime.IsFinished(value.GetDateStartUtc().AddDays(value.m_freeCountDays)) && NKCSynchronizedTime.IsFinished(contractState.nextResetDate)))
				{
					result = true;
				}
			}
		}
		return result;
	}

	public bool hasContractLimit(int contractID)
	{
		ContractTempletV2 contractTempletV = ContractTempletV2.Find(contractID);
		if (contractTempletV != null)
		{
			if (contractTempletV.m_TotalLimit <= 0)
			{
				return contractTempletV.m_DailyLimit > 0;
			}
			return true;
		}
		return false;
	}

	public int GetContractLimitCnt(int contractID)
	{
		int num = -1;
		ContractTempletV2 contractTempletV = ContractTempletV2.Find(contractID);
		if (contractTempletV != null)
		{
			NKMContractState contractState = GetContractState(contractID);
			if (contractTempletV.m_TotalLimit > 0)
			{
				num = contractTempletV.m_TotalLimit;
				if (contractState != null)
				{
					num -= contractState.totalUseCount;
				}
			}
			else if (contractTempletV.m_DailyLimit > 0)
			{
				DateTime recent = ServiceTime.Recent;
				return CalculateLimitContractCount(recent, contractTempletV);
			}
		}
		return num;
	}

	public bool IsDailyContractLimit(int contractID)
	{
		ContractTempletV2 templet = ContractTempletV2.Find(contractID);
		return IsDailyContractLimit(templet);
	}

	public bool IsDailyContractLimit(ContractTempletV2 templet)
	{
		if (templet != null)
		{
			NKMContractState contractState = GetContractState(templet.Key);
			if (contractState != null && templet.m_DailyLimit > 0 && !IsActiveNextFreeChance(templet.Key))
			{
				return templet.m_DailyLimit <= contractState.dailyUseCount;
			}
		}
		return false;
	}

	public DateTime GetNextResetTime()
	{
		if (IsNextResetTimeOver())
		{
			return GetFixedResetTime();
		}
		return m_FreeContractResetTime;
	}

	private bool IsNextResetTimeOver()
	{
		return NKCSynchronizedTime.IsFinished(m_FreeContractResetTime);
	}

	public void SetContractBonusState(List<NKMContractBonusState> contractBonus)
	{
		m_lstBonusState.Clear();
		if (contractBonus.Count > 0)
		{
			m_lstBonusState = contractBonus;
		}
	}

	public void UpdateContractBonusState(NKMContractBonusState contractBonus)
	{
		if (contractBonus == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < m_lstBonusState.Count; i++)
		{
			if (m_lstBonusState[i].bonusGroupId == contractBonus.bonusGroupId)
			{
				m_lstBonusState[i].useCount = contractBonus.useCount;
				m_lstBonusState[i].resetCount = contractBonus.resetCount;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			m_lstBonusState.Add(contractBonus);
		}
		if (!flag)
		{
			return;
		}
		foreach (CustomPickupContractTemplet value in CustomPickupContractTemplet.Values)
		{
			if (value != null && value.CustomPickUpType == CustomPickupContractTemplet.CUSTOM_PICK_UP_TYPE.AWAKEN && value.ContractBonusCountGroupID == contractBonus.bonusGroupId)
			{
				DateTime currentServiceTime = NKMTime.UTCtoLocal(NKCSynchronizedTime.GetServerUTCTime());
				if (value.IsAvailableTime(currentServiceTime))
				{
					value.UpdateTotalUseCount(contractBonus.useCount);
					break;
				}
			}
		}
	}

	public int GetContractBonusCnt(int groupId)
	{
		return m_lstBonusState.Find((NKMContractBonusState x) => x.bonusGroupId == groupId)?.useCount ?? 0;
	}

	private int GetContractBonusResetCnt(int groupId)
	{
		return m_lstBonusState.Find((NKMContractBonusState x) => x.bonusGroupId == groupId)?.resetCount ?? 0;
	}

	public void SetSelectableContractState(NKMSelectableContractState state)
	{
		m_SelectableContractState = state;
	}

	public NKMSelectableContractState GetSelectableContractState()
	{
		return m_SelectableContractState;
	}

	public int GetSelectableContractChangeCnt(int ContractID)
	{
		if (m_SelectableContractState != null && m_SelectableContractState.contractId == ContractID)
		{
			return m_SelectableContractState.unitPoolChangeCount;
		}
		return 0;
	}

	public int GetContractTryCnt(int bonusGroupID)
	{
		foreach (NKMContractBonusState item in m_lstBonusState)
		{
			if (item.bonusGroupId == bonusGroupID)
			{
				return item.useCount;
			}
		}
		return 0;
	}

	public bool IsActiveNewFreeChance()
	{
		foreach (ContractTempletV2 item in (from e in ContractTempletV2.Values
			where CheckOpenCond(e)
			orderby e.GetOrder(), e.GetDateStart()
			select e).ToList())
		{
			if (!IsHasContractStateData(item.Key) && NKCSynchronizedTime.IsEventTime(item.m_DateStartUtc, item.m_DateEndUtc) && item.m_FreeTryCnt > 0)
			{
				UpdateContractFreeTryState();
				return true;
			}
		}
		return false;
	}

	private int CalculateLimitContractCount(DateTime current, ContractTempletV2 templet)
	{
		NKMContractState nKMContractState = m_lstContractState.Find((NKMContractState e) => e.contractId == templet.Key);
		if (nKMContractState == null)
		{
			return templet.m_DailyLimit;
		}
		if (DailyReset.IsOutOfDate(current, GetNextResetTime(templet.Key)))
		{
			return templet.m_DailyLimit;
		}
		return templet.m_DailyLimit - nKMContractState.dailyUseCount;
	}

	public void ResetInstantContract()
	{
		m_lstInstantContract = new List<NKMInstantContract>();
	}

	public void UpdateInstantContract(List<NKMInstantContract> InstantContract)
	{
		m_lstInstantContract = InstantContract;
	}

	public DateTime GetInstantContractEndDateTime(int iCntractID)
	{
		if (m_lstInstantContract != null)
		{
			foreach (NKMInstantContract item in m_lstInstantContract)
			{
				if (item.contractId == iCntractID)
				{
					return item.endDate;
				}
			}
		}
		return default(DateTime);
	}

	public void UpdateCustomPickUpContract(List<NKMCustomPickupContract> customPickupContracts)
	{
		if (customPickupContracts.Count == 0)
		{
			foreach (CustomPickupContractTemplet value in CustomPickupContractTemplet.Values)
			{
				value.UpdateData(0, 0, 0);
			}
			return;
		}
		foreach (NKMCustomPickupContract customPickupContract in customPickupContracts)
		{
			UpdateCustomPickUpContract(customPickupContract);
		}
	}

	public void UpdateCustomPickUpContract(NKMCustomPickupContract customPickupContract)
	{
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(customPickupContract.customPickupId);
		if (customPickupContractTemplet == null)
		{
			Debug.LogError($"<color=red>해당 CustomPickUp Templet 가 존재하지 않습니다. id : {customPickupContract.customPickupId}</color>");
		}
		else
		{
			customPickupContractTemplet.UpdateData(customPickupContract);
		}
	}
}
