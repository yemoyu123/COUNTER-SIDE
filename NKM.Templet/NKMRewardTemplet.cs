using Cs.Logging;
using Cs.Math;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMRewardTemplet
{
	private int m_EquipExp_Min;

	private int m_EquipExp_Max;

	private string m_DateStrID = string.Empty;

	public int m_RewardGroupID;

	public string m_RewardGroupStrID = "";

	public NKM_REWARD_TYPE m_eRewardType = NKM_REWARD_TYPE.RT_MISC;

	public int m_RewardID;

	public string m_RewardStrID = "";

	public int m_Ratio;

	public int m_Quantity_Min;

	public int m_Quantity_Max;

	public NKMIntervalTemplet intervalTemplet = NKMIntervalTemplet.Invalid;

	public int GetRandomEquipExp => RandomGenerator.Range(m_EquipExp_Min, m_EquipExp_Max);

	public static bool IsValidReward(NKM_REWARD_TYPE rewardType, int rewardID)
	{
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
			return NKMUnitManager.GetUnitTempletBase(rewardID) != null;
		case NKM_REWARD_TYPE.RT_MOLD:
			return NKMItemManager.GetItemMoldTempletByID(rewardID) != null;
		case NKM_REWARD_TYPE.RT_MISC:
		case NKM_REWARD_TYPE.RT_MISSION_POINT:
			return NKMItemManager.GetItemMiscTempletByID(rewardID) != null;
		case NKM_REWARD_TYPE.RT_EQUIP:
			return NKMItemManager.GetEquipTemplet(rewardID) != null;
		case NKM_REWARD_TYPE.RT_SKIN:
			return NKMSkinManager.GetSkinTemplet(rewardID) != null;
		case NKM_REWARD_TYPE.RT_BUFF:
			return NKMCompanyBuffManager.GetCompanyBuffTemplet(rewardID) != null;
		case NKM_REWARD_TYPE.RT_EMOTICON:
			return NKMEmoticonTemplet.Find(rewardID) != null;
		case NKM_REWARD_TYPE.RT_BINGO_TILE:
			return NKMEventManager.GetBingoTemplet(rewardID) != null;
		default:
			return true;
		}
	}

	public static bool IsOpenedReward(NKM_REWARD_TYPE rewardType, int rewardID, bool useRandomContract)
	{
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_MISC:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(rewardID);
			if (itemMiscTempletByID == null)
			{
				return false;
			}
			if (!itemMiscTempletByID.EnableByTag)
			{
				return false;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(rewardID);
			if (unitTempletBase2 == null)
			{
				return false;
			}
			if (!unitTempletBase2.CollectionEnableByTag)
			{
				return false;
			}
			if (useRandomContract)
			{
				if (!unitTempletBase2.PickupEnableByTag && !unitTempletBase2.ContractEnableByTag)
				{
					return false;
				}
			}
			else if (!unitTempletBase2.ContractEnableByTag)
			{
				return false;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(rewardID);
			if (unitTempletBase == null)
			{
				return false;
			}
			if (!unitTempletBase.CollectionEnableByTag)
			{
				return false;
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(rewardID);
			if (skinTemplet == null)
			{
				return false;
			}
			if (!skinTemplet.EnableByTag)
			{
				return false;
			}
			break;
		}
		}
		return true;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		int result = (int)(1u & (cNKMLua.GetData("m_RewardGroupID", ref m_RewardGroupID) ? 1u : 0u) & (cNKMLua.GetData("m_RewardGroupStrID", ref m_RewardGroupStrID) ? 1u : 0u) & (cNKMLua.GetData("m_RewardType", ref m_eRewardType) ? 1u : 0u) & (cNKMLua.GetData("m_RewardID", ref m_RewardID) ? 1u : 0u) & (cNKMLua.GetData("m_RewardStrID", ref m_RewardStrID) ? 1u : 0u) & (cNKMLua.GetData("m_Ratio", ref m_Ratio) ? 1u : 0u) & (cNKMLua.GetData("m_Quantity_Min", ref m_Quantity_Min) ? 1u : 0u) & (cNKMLua.GetData("m_Quantity_Max", ref m_Quantity_Max) ? 1u : 0u) & (cNKMLua.GetData("m_EquipExp_Min", ref m_EquipExp_Min) ? 1u : 0u)) & (cNKMLua.GetData("m_EquipExp_Max", ref m_EquipExp_Max) ? 1 : 0);
		cNKMLua.GetData("m_DateStrID", ref m_DateStrID);
		return (byte)result != 0;
	}

	public void Join()
	{
		if (!string.IsNullOrEmpty(m_DateStrID))
		{
			intervalTemplet = NKMIntervalTemplet.Find(m_DateStrID);
			if (intervalTemplet == null)
			{
				NKMTempletError.Add($"[NKMRewardTemplet] 리워드의 인터벌을 찾을 수 없습니다. m_RewardGroupID:{m_RewardGroupID} m_RewardID:{m_RewardID} m_DateStrId:{m_DateStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRewardTemplet.cs", 316);
			}
		}
	}

	public void Validate()
	{
		if (m_Ratio <= 0)
		{
			Log.ErrorAndExit($"[RewardTemplet] 보상 비율 값은 1보다 작을 수 없음 RewardGroupID:{m_RewardGroupID} m_eRewardType:{m_eRewardType} m_RewardID:{m_RewardID} ratio:{m_Ratio}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRewardTemplet.cs", 325);
		}
		if (!IsValidReward(m_eRewardType, m_RewardID))
		{
			Log.ErrorAndExit($"[RewardTemplet] 보상 정보가 존재하지 않음 m_RewardGroupID:{m_RewardGroupID} m_eRewardType:{m_eRewardType} m_RewardID:{m_RewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRewardTemplet.cs", 330);
		}
		if (m_Quantity_Min > m_Quantity_Max)
		{
			Log.ErrorAndExit($"[RewardTemplet] 최소 수량이 최대 수량 보다 큽니다. m_ReawrdGroupID:{m_RewardGroupID}, m_eRewardType:{m_eRewardType}, m_RewardID:{m_RewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRewardTemplet.cs", 335);
		}
		if (m_EquipExp_Max != 0 && m_EquipExp_Min != 0 && (m_EquipExp_Max < m_EquipExp_Min || m_EquipExp_Max < 0 || m_EquipExp_Min < 0))
		{
			Log.Warn($"[RewardTemplet] 장비 획득 랜덤 경험치 이상. m_RewardGroupID:{m_RewardGroupID} EquipExpMin:{m_EquipExp_Min} EquipExpMax{m_EquipExp_Max}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRewardTemplet.cs", 342);
		}
	}

	public NKMRewardInfo ToRewardInfo()
	{
		return new NKMRewardInfo
		{
			rewardType = m_eRewardType,
			ID = m_RewardID,
			Count = DecideCount(),
			paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
		};
	}

	public int DecideCount()
	{
		return RandomGenerator.Range(m_Quantity_Min, m_Quantity_Max + 1);
	}
}
