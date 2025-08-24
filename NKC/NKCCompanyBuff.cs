using System;
using System.Collections.Generic;
using NKC.UI;
using NKM;
using NKM.Templet;

namespace NKC;

public static class NKCCompanyBuff
{
	public static void UpsertCompanyBuffData(List<NKMCompanyBuffData> companyBuffs, NKMCompanyBuffData data)
	{
		NKMCompanyBuffData nKMCompanyBuffData = companyBuffs.Find((NKMCompanyBuffData ele) => ele.Id == data.Id);
		if (nKMCompanyBuffData == null)
		{
			companyBuffs.Add(data);
			NKCScenManager.CurrentUserData().dOnCompanyBuffUpdate?.Invoke(NKCScenManager.CurrentUserData());
		}
		else
		{
			NKMCompanyBuffTemplet companyBuffTemplet = NKMCompanyBuffManager.GetCompanyBuffTemplet(data.Id);
			nKMCompanyBuffData.UpdateExpireTicksAsMinutes(companyBuffTemplet.m_CompanyBuffTime);
			data.SetExpireTicks(nKMCompanyBuffData.ExpireTicks);
		}
		NKCPopupMessageCompanyBuff.Instance.Open(data, bAddBuff: true);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME().RefreshBuff();
		}
	}

	public static void SetDiscountOfEterniumInEnteringWarfare(List<NKMCompanyBuffData> companyBuffs, ref int eternium)
	{
		int discountOfEterniumInEnteringWarfare = GetDiscountOfEterniumInEnteringWarfare(companyBuffs, eternium);
		discountOfEterniumInEnteringWarfare += GetDiscountOfEterniumInEnteringWarfareDungeon(companyBuffs, eternium);
		eternium = Math.Max(0, eternium - discountOfEterniumInEnteringWarfare);
	}

	public static void SetDiscountOfEterniumInEnteringDungeon(List<NKMCompanyBuffData> companyBuffs, ref int eternium)
	{
		int discountOfEterniumInEnteringWarfareDungeon = GetDiscountOfEterniumInEnteringWarfareDungeon(companyBuffs, eternium);
		eternium = Math.Max(0, eternium - discountOfEterniumInEnteringWarfareDungeon);
	}

	public static void SetDiscountOfCreditInNegotiation(List<NKMCompanyBuffData> companyBuffs, ref long credit)
	{
		long discountOfCreditInNegotiation = GetDiscountOfCreditInNegotiation(companyBuffs, credit);
		credit = Math.Max(0L, credit - discountOfCreditInNegotiation);
	}

	public static void SetDiscountOfCreditInCraft(List<NKMCompanyBuffData> companyBuffs, ref int credit)
	{
		int discountOfCreditInCraft = GetDiscountOfCreditInCraft(companyBuffs, credit);
		credit = Math.Max(0, credit - discountOfCreditInCraft);
	}

	public static void SetDiscountOfCreditInEnchantTuning(List<NKMCompanyBuffData> companyBuffs, ref int credit)
	{
		int discountOfCreditInEnchantTuning = GetDiscountOfCreditInEnchantTuning(companyBuffs, credit);
		credit = Math.Max(0, credit - discountOfCreditInEnchantTuning);
	}

	public static void SetDiscountOfCreditInPotentialSocket(List<NKMCompanyBuffData> companyBuffs, ref long credit)
	{
		long discountOfCreditInPotentialSocket = GetDiscountOfCreditInPotentialSocket(companyBuffs, credit);
		credit = Math.Max(0L, credit - discountOfCreditInPotentialSocket);
	}

	public static void IncreaseMissioRateInWorldMap(List<NKMCompanyBuffData> companyBuffs, ref int successRate)
	{
		int totalRatio = GetTotalRatio(companyBuffs, NKMConst.Buff.BuffType.WORLDMAP_MISSION_COMPLETE_RATIO_BONUS);
		if (totalRatio > 0)
		{
			int num = successRate * totalRatio / 100;
			successRate = Math.Min(100, successRate + num);
		}
	}

	public static void IncreaseChargePointOfPvpWithBonusRatio(List<NKMCompanyBuffData> companyBuffs, ref int rewardChargePoint, out int bonusRatio)
	{
		if (rewardChargePoint == 0)
		{
			bonusRatio = 0;
			return;
		}
		bonusRatio = GetTotalRatio(companyBuffs, NKMConst.Buff.BuffType.PVP_POINT_CHARGE);
		if (bonusRatio > 0)
		{
			rewardChargePoint += rewardChargePoint * bonusRatio / 100;
		}
	}

	public static void RemoveExpiredBuffs(List<NKMCompanyBuffData> companyBuffs)
	{
		List<NKMCompanyBuffData> list = new List<NKMCompanyBuffData>();
		for (int i = 0; i < companyBuffs.Count; i++)
		{
			NKMCompanyBuffTemplet nKMCompanyBuffTemplet = NKMCompanyBuffTemplet.Find(companyBuffs[i].Id);
			if (nKMCompanyBuffTemplet == null)
			{
				list.Add(companyBuffs[i]);
				continue;
			}
			NKMCompanyBuffSource companyBuffSource = nKMCompanyBuffTemplet.m_CompanyBuffSource;
			if (companyBuffSource != NKMCompanyBuffSource.ON_TIME_EVENT && companyBuffSource == NKMCompanyBuffSource.LEVEL)
			{
				if (nKMCompanyBuffTemplet.m_AccountLevelMax < NKCScenManager.CurrentUserData().UserLevel || nKMCompanyBuffTemplet.m_AccountLevelMin > NKCScenManager.CurrentUserData().UserLevel)
				{
					list.Add(companyBuffs[i]);
				}
			}
			else if (NKCSynchronizedTime.IsFinished(companyBuffs[i].ExpireDate))
			{
				list.Add(companyBuffs[i]);
			}
		}
		if (list.Count > 0)
		{
			for (int j = 0; j < list.Count; j++)
			{
				NKCPopupMessageCompanyBuff.Instance.Open(list[j], bAddBuff: false);
			}
			companyBuffs.RemoveAll((NKMCompanyBuffData ele) => NKMCompanyBuffTemplet.Find(ele.Id) != null && NKMCompanyBuffTemplet.Find(ele.Id).m_CompanyBuffSource != NKMCompanyBuffSource.LEVEL && NKCSynchronizedTime.IsFinished(ele.ExpireDate));
			companyBuffs.RemoveAll((NKMCompanyBuffData ele) => NKMCompanyBuffTemplet.Find(ele.Id) != null && NKMCompanyBuffTemplet.Find(ele.Id).m_CompanyBuffSource == NKMCompanyBuffSource.LEVEL && ele.ExpireDate.Equals(DateTime.MaxValue) && NKMCompanyBuffTemplet.Find(ele.Id).m_AccountLevelMax < NKCScenManager.CurrentUserData().UserLevel);
			NKCPacketSender.Send_NKMPacket_REFRESH_COMPANY_BUFF_REQ();
			NKCScenManager.CurrentUserData().dOnCompanyBuffUpdate?.Invoke(NKCScenManager.CurrentUserData());
		}
	}

	public static bool NeedShowEventMark(List<NKMCompanyBuffData> companyBuffs, NKMConst.Buff.BuffType buffType)
	{
		foreach (NKMCompanyBuffData companyBuff in companyBuffs)
		{
			NKMCompanyBuffTemplet companyBuffTemplet = NKMCompanyBuffManager.GetCompanyBuffTemplet(companyBuff.Id);
			foreach (NKMCompanyBuffInfo companyBuffInfo in companyBuffTemplet.m_CompanyBuffInfoList)
			{
				if (companyBuffInfo.m_CompanyBuffType == buffType)
				{
					return companyBuffTemplet.m_ShowEventMark;
				}
			}
		}
		return false;
	}

	public static int GetTotalRatio(List<NKMCompanyBuffData> companyBuffs, NKMConst.Buff.BuffType buffType)
	{
		int num = 0;
		foreach (NKMCompanyBuffData companyBuff in companyBuffs)
		{
			foreach (NKMCompanyBuffInfo companyBuffInfo in NKMCompanyBuffManager.GetCompanyBuffTemplet(companyBuff.Id).m_CompanyBuffInfoList)
			{
				if (companyBuffInfo.m_CompanyBuffType == buffType)
				{
					num += companyBuffInfo.m_CompanyBuffRatio;
				}
			}
		}
		return num;
	}

	private static int GetDiscountOfEterniumInEnteringWarfare(List<NKMCompanyBuffData> companyBuffs, int eternium)
	{
		int totalRatio = GetTotalRatio(companyBuffs, NKMConst.Buff.BuffType.WARFARE_ETERNIUM_DISCOUNT);
		if (totalRatio == 0)
		{
			return 0;
		}
		return totalRatio * eternium / 100;
	}

	private static int GetDiscountOfEterniumInEnteringWarfareDungeon(List<NKMCompanyBuffData> companyBuffs, int eternium)
	{
		int totalRatio = GetTotalRatio(companyBuffs, NKMConst.Buff.BuffType.WARFARE_DUNGEON_ETERNIUM_DISCOUNT);
		if (totalRatio == 0)
		{
			return 0;
		}
		return totalRatio * eternium / 100;
	}

	private static long GetDiscountOfCreditInNegotiation(List<NKMCompanyBuffData> companyBuffs, long credit)
	{
		int totalRatio = GetTotalRatio(companyBuffs, NKMConst.Buff.BuffType.BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT);
		if (totalRatio == 0)
		{
			return 0L;
		}
		return totalRatio * credit / 100;
	}

	private static int GetDiscountOfCreditInCraft(List<NKMCompanyBuffData> companyBuffs, int credit)
	{
		int totalRatio = GetTotalRatio(companyBuffs, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT);
		if (totalRatio == 0)
		{
			return 0;
		}
		return totalRatio * credit / 100;
	}

	private static int GetDiscountOfCreditInEnchantTuning(List<NKMCompanyBuffData> companyBuffs, int credit)
	{
		int totalRatio = GetTotalRatio(companyBuffs, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT);
		if (totalRatio == 0)
		{
			return 0;
		}
		return totalRatio * credit / 100;
	}

	public static void SetDiscountOfOperatorSkillEnhanceCost(List<NKMCompanyBuffData> companyBuffs, ref int info, ref int bounsRatio)
	{
		int discountOfInfomationInOperatorSkillEnhance = GetDiscountOfInfomationInOperatorSkillEnhance(companyBuffs, info, ref bounsRatio);
		info = Math.Max(0, info - discountOfInfomationInOperatorSkillEnhance);
	}

	private static long GetDiscountOfCreditInPotentialSocket(List<NKMCompanyBuffData> companyBuffs, long credit)
	{
		int totalRatio = GetTotalRatio(companyBuffs, NKMConst.Buff.BuffType.BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT);
		if (totalRatio == 0)
		{
			return 0L;
		}
		return totalRatio * credit / 100;
	}

	private static int GetDiscountOfInfomationInOperatorSkillEnhance(List<NKMCompanyBuffData> companyBuffs, int info, ref int bounsRatio)
	{
		bounsRatio = GetTotalRatio(companyBuffs, NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_COST_DISCOUNT);
		if (bounsRatio == 0)
		{
			return 0;
		}
		return bounsRatio * info / 100;
	}

	public static void IncreaseChargeOperatorSkillEnhanceRatio(List<NKMCompanyBuffData> companyBuffs, NKM_UNIT_GRADE unit_grade, ref float changeRatio, ref float bonusRatio)
	{
		bonusRatio = 0f;
		if (changeRatio == 0f)
		{
			return;
		}
		NKMConst.Buff.BuffType buffType = NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N;
		buffType = unit_grade switch
		{
			NKM_UNIT_GRADE.NUG_SSR => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SSR, 
			NKM_UNIT_GRADE.NUG_SR => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SR, 
			NKM_UNIT_GRADE.NUG_R => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_R, 
			_ => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N, 
		};
		foreach (NKMCompanyBuffData companyBuff in companyBuffs)
		{
			foreach (NKMCompanyBuffInfo companyBuffInfo in NKMCompanyBuffManager.GetCompanyBuffTemplet(companyBuff.Id).m_CompanyBuffInfoList)
			{
				if (companyBuffInfo.m_CompanyBuffType == buffType)
				{
					float num = (float)companyBuffInfo.m_CompanyBuffRatio * 0.01f;
					bonusRatio = changeRatio * num;
					changeRatio += bonusRatio;
					break;
				}
			}
		}
	}

	public static void IncreaseChargeOperatorSkillEnhanceRatio(List<NKMCompanyBuffData> companyBuffs, NKM_ITEM_GRADE item_grade, ref float changeRatio, ref float bonusRatio)
	{
		bonusRatio = 0f;
		if (changeRatio == 0f)
		{
			return;
		}
		NKMConst.Buff.BuffType buffType = NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N;
		buffType = item_grade switch
		{
			NKM_ITEM_GRADE.NIG_SSR => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SSR, 
			NKM_ITEM_GRADE.NIG_SR => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SR, 
			NKM_ITEM_GRADE.NIG_R => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_R, 
			_ => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N, 
		};
		foreach (NKMCompanyBuffData companyBuff in companyBuffs)
		{
			foreach (NKMCompanyBuffInfo companyBuffInfo in NKMCompanyBuffManager.GetCompanyBuffTemplet(companyBuff.Id).m_CompanyBuffInfoList)
			{
				if (companyBuffInfo.m_CompanyBuffType == buffType)
				{
					float num = (float)companyBuffInfo.m_CompanyBuffRatio * 0.01f;
					bonusRatio = changeRatio * num;
					changeRatio += bonusRatio;
					break;
				}
			}
		}
	}
}
