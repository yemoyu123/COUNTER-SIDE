using System.Linq;
using Cs.Logging;
using Cs.Math;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMRandomBoxItemTemplet : INKMTemplet
{
	private int EquipExp_Min;

	private int EquipExp_Max;

	public int m_RewardGroupID;

	public NKM_REWARD_TYPE m_reward_type;

	public int m_RewardID;

	public int m_Ratio;

	public int FreeQuantity_Min;

	public int FreeQuantity_Max;

	public int PaidQuantity_Min;

	public int PaidQuantity_Max;

	public int m_OrderList;

	public string m_OpenTag;

	public int TotalQuantity_Min => FreeQuantity_Min + PaidQuantity_Min;

	public int TotalQuantity_Max => FreeQuantity_Max + PaidQuantity_Max;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public int Key => m_RewardGroupID;

	public int GetRandomEquipExp => RandomGenerator.Range(EquipExp_Min, EquipExp_Max);

	public static NKMRandomBoxItemTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomBoxItemTemplet.cs", 33))
		{
			return null;
		}
		NKMRandomBoxItemTemplet nKMRandomBoxItemTemplet = new NKMRandomBoxItemTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_RewardGroupID", ref nKMRandomBoxItemTemplet.m_RewardGroupID) ? 1u : 0u) & (cNKMLua.GetData("m_RewardType", ref nKMRandomBoxItemTemplet.m_reward_type) ? 1u : 0u) & (cNKMLua.GetData("m_RewardID", ref nKMRandomBoxItemTemplet.m_RewardID) ? 1u : 0u)) & (cNKMLua.GetData("m_Ratio", ref nKMRandomBoxItemTemplet.m_Ratio) ? 1 : 0);
		cNKMLua.GetData("m_FreeQuantity_Min", ref nKMRandomBoxItemTemplet.FreeQuantity_Min);
		cNKMLua.GetData("m_FreeQuantity_Max", ref nKMRandomBoxItemTemplet.FreeQuantity_Max);
		cNKMLua.GetData("m_PaidQuantity_Min", ref nKMRandomBoxItemTemplet.PaidQuantity_Min);
		cNKMLua.GetData("m_PaidQuantity_Max", ref nKMRandomBoxItemTemplet.PaidQuantity_Max);
		cNKMLua.GetData("m_OrderList", ref nKMRandomBoxItemTemplet.m_OrderList);
		cNKMLua.GetData("m_OpenTag", ref nKMRandomBoxItemTemplet.m_OpenTag);
		cNKMLua.GetData("m_EquipExp_Min", ref nKMRandomBoxItemTemplet.EquipExp_Min);
		cNKMLua.GetData("m_EquipExp_Max", ref nKMRandomBoxItemTemplet.EquipExp_Max);
		if (num == 0)
		{
			return null;
		}
		return nKMRandomBoxItemTemplet;
	}

	public override string ToString()
	{
		return $"rewardType:{m_reward_type} rewardId:{m_RewardID} freeRewardCount:{FreeQuantity_Min}~{FreeQuantity_Max} paidRewardCount:{PaidQuantity_Min}~{PaidQuantity_Max}";
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (!NKMRewardTemplet.IsValidReward(m_reward_type, m_RewardID))
		{
			NKMTempletError.Add($"[RandomBoxItem] 보상 정보가 존재하지 않음 m_RewardGroupID:{m_RewardGroupID} m_reward_type:{m_reward_type} m_RewardID:{m_RewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomBoxItemTemplet.cs", 75);
		}
		if (m_Ratio <= 0)
		{
			NKMTempletError.Add($"[RandomBoxItem] m_Ratio 는 0보다 커야 함. groupId:{m_RewardGroupID} rewardId:{m_RewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomBoxItemTemplet.cs", 80);
		}
		if (m_reward_type == NKM_REWARD_TYPE.RT_MISC)
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(m_RewardID);
			if (nKMItemMiscTemplet != null && nKMItemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE)
			{
				NKMTempletError.Add($"[RandomBoxItem] 아이템 그룹에 커스텀패키지 유형은 담을 수 없음. groupId:{m_RewardGroupID} rewardType:{m_reward_type} rewardId:{m_RewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomBoxItemTemplet.cs", 88);
			}
			if (FreeQuantity_Min == 0 && FreeQuantity_Max == 0 && PaidQuantity_Min == 0 && PaidQuantity_Max == 0)
			{
				NKMTempletError.Add($"[RandomBoxItem] 보상 범위 설정 오류. groupId:{m_RewardGroupID} FreeQuantity_Min:{FreeQuantity_Min} FreeQuantity_Max:{FreeQuantity_Max} PaidQuantity_Min:{PaidQuantity_Min} PaidQuantity_Max:{PaidQuantity_Max}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomBoxItemTemplet.cs", 93);
			}
		}
		if (EquipExp_Max != 0 && EquipExp_Min != 0 && (EquipExp_Max < EquipExp_Min || EquipExp_Max < 0 || EquipExp_Min < 0))
		{
			NKMTempletError.Add($"[RandomBoxItem] 장비 획득 랜덤 경험치 이상. groupId:{m_RewardGroupID} EquipExpMin:{EquipExp_Min} EquipExpMax{EquipExp_Max}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomBoxItemTemplet.cs", 108);
		}
		if (m_reward_type != NKM_REWARD_TYPE.RT_EQUIP)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(m_RewardID);
		if (equipTemplet == null)
		{
			NKMTempletError.Add($"[RandomBoxItem] 장비 정보가 존재하지 않음. groupId:{m_RewardGroupID} m_RewardID:{m_RewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomBoxItemTemplet.cs", 116);
		}
		foreach (NKMItemMiscTemplet item in NKMItemMiscTemplet.Values.Where((NKMItemMiscTemplet e) => e.IsChoiceItem() && e.m_RewardGroupID == m_RewardGroupID).ToList())
		{
			if (equipTemplet.potentialOptionGroupId2 > 0 && item.m_ItemMiscSubType == NKM_ITEM_MISC_SUBTYPE.IMST_EQUIP_CHOICE_OPTION_CUSTOM && item.ChangePotenOption)
			{
				Log.Warn($"[MiscItem] 2번 잠재 옵션을 가지는 장비는 선택권에서 잠재옵션을 선택할 수 없음. key:{Key} misc Item Id:{item.Key} type:{item.m_ItemMiscType} m_RewardGroupID:{m_RewardGroupID} randomBoxItemTemplet.rewardId:{m_RewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomBoxItemTemplet.cs", 129);
			}
		}
	}
}
