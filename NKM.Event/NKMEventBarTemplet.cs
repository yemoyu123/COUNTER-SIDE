using System;
using System.Collections.Generic;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Event;

public class NKMEventBarTemplet : INKMTemplet
{
	private int eventId;

	private string openTag;

	private int materialItemId01;

	private int materialItemValue01;

	private int materialItemId02;

	private int materialItemValue02;

	private ManufacturingTechnique manufacturingTechnique;

	private int rewardItemId;

	private int deliveryLimitValue;

	private int deliveryValue;

	private int deliveryRewardItemId;

	private int deliveryRewardValue;

	private static List<NKMEventBarTemplet> templets = new List<NKMEventBarTemplet>();

	public static List<int> barMissionGroupList = new List<int>();

	public NKMEventTabTemplet EventTabTemplet { get; private set; }

	public int Key => rewardItemId;

	public static IEnumerable<NKMEventBarTemplet> Values => NKMTempletContainer<NKMEventBarTemplet>.Values;

	public static IReadOnlyList<NKMEventBarTemplet> List => templets;

	public int EventID => eventId;

	public int MaterialItemId01 => materialItemId01;

	public int MaterialItemValue01 => materialItemValue01;

	public int MaterialItemId02 => materialItemId02;

	public int MaterialItemValue02 => materialItemValue02;

	public ManufacturingTechnique Technique => manufacturingTechnique;

	public int RewardItemId => rewardItemId;

	public int DeliveryLimitValue => deliveryLimitValue;

	public int DeliveryValue => deliveryValue;

	public int DeliveryRewardItemId => deliveryRewardItemId;

	public int DeliveryRewardValue => deliveryRewardValue;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public static NKMEventBarTemplet Find(int key)
	{
		return NKMTempletContainer<NKMEventBarTemplet>.Find((NKMEventBarTemplet x) => x.Key == key);
	}

	public static NKMEventBarTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBarTemplet.cs", 70))
		{
			return null;
		}
		NKMEventBarTemplet nKMEventBarTemplet = new NKMEventBarTemplet();
		int num = 1 & (lua.GetData("m_EventID", ref nKMEventBarTemplet.eventId) ? 1 : 0);
		lua.GetData("m_OpenTag", ref nKMEventBarTemplet.openTag);
		if (((uint)num & (lua.GetData("MaterialID_1", ref nKMEventBarTemplet.materialItemId01) ? 1u : 0u) & (lua.GetData("MaterialValue_1", ref nKMEventBarTemplet.materialItemValue01) ? 1u : 0u) & (lua.GetData("MaterialID_2", ref nKMEventBarTemplet.materialItemId02) ? 1u : 0u) & (lua.GetData("MaterialValue_2", ref nKMEventBarTemplet.materialItemValue02) ? 1u : 0u) & (lua.GetData("ManufacturingTechnique", ref nKMEventBarTemplet.manufacturingTechnique) ? 1u : 0u) & (lua.GetData("RewardItemID", ref nKMEventBarTemplet.rewardItemId) ? 1u : 0u) & (lua.GetData("DeliveryLimitValue", ref nKMEventBarTemplet.deliveryLimitValue) ? 1u : 0u) & (lua.GetData("DeliveryValue", ref nKMEventBarTemplet.deliveryValue) ? 1u : 0u) & (lua.GetData("DeliveryRewardItemID", ref nKMEventBarTemplet.deliveryRewardItemId) ? 1u : 0u) & (lua.GetData("DeliveryRewardValue", ref nKMEventBarTemplet.deliveryRewardValue) ? 1u : 0u)) == 0)
		{
			return null;
		}
		if (nKMEventBarTemplet.EnableByTag)
		{
			templets.Add(nKMEventBarTemplet);
			if (barMissionGroupList.Count == 0)
			{
				foreach (BarMissionTabId value in Enum.GetValues(typeof(BarMissionTabId)))
				{
					foreach (NKMMissionTemplet item in NKMMissionManager.GetMissionTempletListByTabID((int)value))
					{
						if (!barMissionGroupList.Contains(item.m_GroupId))
						{
							barMissionGroupList.Add(item.m_GroupId);
						}
					}
				}
			}
		}
		return nKMEventBarTemplet;
	}

	public void Join()
	{
		EventTabTemplet = NKMEventTabTemplet.Find(eventId);
		if (EventTabTemplet == null)
		{
			NKMTempletError.Add($"[NKMEventBarTemplet] eventId로 event tab templet을 찾을 수 없음. EventId:{eventId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBarTemplet.cs", 124);
		}
		else if (EventTabTemplet.EventIntervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMEventBarTemplet] interval templet을 찾을 수 없음. EventId:{eventId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBarTemplet.cs", 131);
		}
	}

	public void Validate()
	{
		if (!Enum.IsDefined(typeof(ManufacturingTechnique), manufacturingTechnique))
		{
			NKMTempletError.Add($"[NKMEventBarTemplet] 제조기술 설정값이 올바르지 않음. EventId:{eventId} ManufacturingTechnique: {manufacturingTechnique}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBarTemplet.cs", 140);
		}
		if (!NKMRewardTemplet.IsValidReward(NKM_REWARD_TYPE.RT_MISC, materialItemId01))
		{
			NKMTempletError.Add($"[NKMEventBarTemplet] 칵테일 제조 아이템 정보가 템플릿 정보에 없음. EventId:{eventId} MaterialItemId01: {materialItemId01} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBarTemplet.cs", 145);
		}
		if (!NKMRewardTemplet.IsValidReward(NKM_REWARD_TYPE.RT_MISC, materialItemId02))
		{
			NKMTempletError.Add($"[NKMEventBarTemplet] 칵테일 제조 아이템 정보가 템플릿 정보에 없음. EventId:{eventId} MaterialItemId02: {materialItemId02} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBarTemplet.cs", 150);
		}
		if (!NKMRewardTemplet.IsValidReward(NKM_REWARD_TYPE.RT_MISC, rewardItemId))
		{
			NKMTempletError.Add($"[NKMEventBarTemplet] 칵테일 아이템 정보가 템플릿 정보에 없음. EventId:{eventId} rewardItemId: {rewardItemId} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBarTemplet.cs", 155);
		}
		if (!NKMRewardTemplet.IsValidReward(NKM_REWARD_TYPE.RT_MISC, deliveryRewardItemId))
		{
			NKMTempletError.Add($"[NKMEventBarTemplet] 칵테일 보상 아이템 정보가 템플릿 정보에 없음. EventId:{eventId} deliveryRewardItemId: {deliveryRewardItemId} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventBarTemplet.cs", 160);
		}
	}
}
