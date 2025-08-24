using System;
using System.Collections.Generic;
using System.Linq;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Event;

public sealed class NKMEventCollectionIndexTemplet : INKMTemplet, INKMTempletEx
{
	private int eventId;

	private string openTag;

	private List<int> eventMissionTabIds;

	private List<NKMMissionTabTemplet> missionTabs;

	private string dateStrId;

	public string EventBannerStrId;

	public string EventBannerTitleStrId;

	private string EventPrefabId;

	public string BgmAssetId;

	public int BgmVolume;

	private string EventContractPrefabId;

	public int EventContractId;

	public string EventMissionPrefabId;

	public string EventMissionSlotPrefabId;

	public string EventMergePrefabID;

	public string EventMergeResultPrefabID;

	public int CollectionMergeId;

	private string EventCollectionPrefabId;

	private string EventCollectionSlotPrefabId;

	public int EventCollectionGroupId;

	public string EventShopPrefabId;

	public string ShopShortCutType;

	public string ShopShortCut;

	public int EventMissionAllClearTabId;

	public string EventContractAnimationPrefabID;

	public string EventResultPrefabID;

	public Dictionary<string, string> m_Option = new Dictionary<string, string>();

	private string m_strOption = "";

	public int Key => eventId;

	public List<int> MissionTabIds => eventMissionTabIds;

	public List<NKMMissionTabTemplet> MissionTabTemplets => missionTabs;

	public string DateStrId => dateStrId;

	public bool IsOpen => NKMOpenTagManager.IsOpened(openTag);

	public static IEnumerable<NKMEventCollectionIndexTemplet> Values => NKMTempletContainer<NKMEventCollectionIndexTemplet>.Values;

	public NKMIntervalTemplet Intervaltemplet { get; private set; }

	public string EventPrefabID_BundleName => EventPrefabId.Split('@')[0];

	public string EventPrefabID_AssetName => EventPrefabId.Split('@')[1];

	public string EventContractPrefabID_BundleName => EventContractPrefabId.Split('@')[0];

	public string EventContractPrefabID_AssetName => EventContractPrefabId.Split('@')[1];

	public string EventCollectionPrefabID_BundleName => EventCollectionPrefabId.Split('@')[0];

	public string EventCollectionPrefabID_AssetName => EventCollectionPrefabId.Split('@')[1];

	public string EventCollectionSlotPrefabID_BundleName => EventCollectionSlotPrefabId.Split('@')[0];

	public string EventCollectionSlotPrefabID_AssetName => EventCollectionSlotPrefabId.Split('@')[1];

	public string EventContractAniPrefabID_AssetName => EventContractAnimationPrefabID.Split('@')[0];

	public string EventContractAniPrefabID_BundleName => EventContractAnimationPrefabID.Split('@')[1];

	public static NKMEventCollectionIndexTemplet GetByTime(DateTime current)
	{
		return Values.FirstOrDefault((NKMEventCollectionIndexTemplet e) => e.IsValidTime(current));
	}

	public bool IsValidTime(DateTime date)
	{
		if (Intervaltemplet != null)
		{
			return Intervaltemplet.IsValidTime(date);
		}
		return false;
	}

	public static NKMEventCollectionIndexTemplet Find(int key)
	{
		return NKMTempletContainer<NKMEventCollectionIndexTemplet>.Find(key);
	}

	public static NKMEventCollectionIndexTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionIndexTemplet.cs", 59))
		{
			return null;
		}
		NKMEventCollectionIndexTemplet nKMEventCollectionIndexTemplet = new NKMEventCollectionIndexTemplet();
		bool flag = true;
		flag &= lua.GetData("EventID", ref nKMEventCollectionIndexTemplet.eventId);
		flag &= lua.GetData("OpenTag", ref nKMEventCollectionIndexTemplet.openTag);
		flag &= lua.GetData("DateStrID", ref nKMEventCollectionIndexTemplet.dateStrId);
		flag &= lua.GetData("EventBannerStrID", ref nKMEventCollectionIndexTemplet.EventBannerStrId);
		flag &= lua.GetData("EventBannerTitleStrID", ref nKMEventCollectionIndexTemplet.EventBannerTitleStrId);
		flag &= lua.GetData("EventPrefabID", ref nKMEventCollectionIndexTemplet.EventPrefabId);
		flag &= lua.GetData("BgmAssetID", ref nKMEventCollectionIndexTemplet.BgmAssetId);
		flag &= lua.GetData("BgmVolume", ref nKMEventCollectionIndexTemplet.BgmVolume);
		lua.GetData("EventContractID", ref nKMEventCollectionIndexTemplet.EventContractId);
		lua.GetData("EventContractPrefabID", ref nKMEventCollectionIndexTemplet.EventContractPrefabId);
		lua.GetData("EventMissionSlotPrefabID", ref nKMEventCollectionIndexTemplet.EventMissionSlotPrefabId);
		lua.GetData("EventMergePrefabID", ref nKMEventCollectionIndexTemplet.EventMergePrefabID);
		lua.GetData("EventMergeResultPrefabID", ref nKMEventCollectionIndexTemplet.EventMergeResultPrefabID);
		lua.GetData("CollectionMergeID", ref nKMEventCollectionIndexTemplet.CollectionMergeId);
		lua.GetData("EventCollectionGroupID", ref nKMEventCollectionIndexTemplet.EventCollectionGroupId);
		lua.GetData("EventCollectionPrefabID", ref nKMEventCollectionIndexTemplet.EventCollectionPrefabId);
		lua.GetData("EventCollectionSlotPrefabID", ref nKMEventCollectionIndexTemplet.EventCollectionSlotPrefabId);
		lua.GetData("EventShopPrefabID", ref nKMEventCollectionIndexTemplet.EventShopPrefabId);
		lua.GetData("ShopShortCutType", ref nKMEventCollectionIndexTemplet.ShopShortCutType);
		lua.GetData("ShopShortCut", ref nKMEventCollectionIndexTemplet.ShopShortCut);
		lua.GetData("EventMissionAllClearTabID", ref nKMEventCollectionIndexTemplet.EventMissionAllClearTabId);
		lua.GetData("EventContractAnimationPrefabID", ref nKMEventCollectionIndexTemplet.EventContractAnimationPrefabID);
		lua.GetData("EventResultPrefabID", ref nKMEventCollectionIndexTemplet.EventResultPrefabID);
		lua.GetData("EventMissionPrefabID", ref nKMEventCollectionIndexTemplet.EventMissionPrefabId);
		nKMEventCollectionIndexTemplet.eventMissionTabIds = new List<int>();
		if (lua.OpenTable("EventMissionTabID"))
		{
			int i = 1;
			for (int rValue = 0; lua.GetData(i, ref rValue); i++)
			{
				nKMEventCollectionIndexTemplet.eventMissionTabIds.Add(rValue);
			}
			lua.CloseTable();
		}
		lua.GetData("m_Option", ref nKMEventCollectionIndexTemplet.m_strOption);
		if (!flag)
		{
			return null;
		}
		return nKMEventCollectionIndexTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinTemplet();
		}
	}

	public void JoinTemplet()
	{
		missionTabs = new List<NKMMissionTabTemplet>();
		foreach (int eventMissionTabId in eventMissionTabIds)
		{
			if (!NKMMissionManager.DicMissionTab.TryGetValue(eventMissionTabId, out var value))
			{
				NKMTempletError.Add($"[NKMEventCollectionIndexTemplet:{eventId}] Mission Tab Templet이 존재하지 않음. missionTabId:{eventMissionTabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionIndexTemplet.cs", 128);
			}
			else
			{
				MissionTabTemplets.Add(value);
			}
		}
		Intervaltemplet = NKMIntervalTemplet.Find(dateStrId);
		if (Intervaltemplet == null)
		{
			NKMTempletError.Add($"[NKMEventCollectionIndexTemplet:{eventId}] IntervalTemplet을 찾지 못함. dateStrId:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionIndexTemplet.cs", 138);
		}
	}

	public void Validate()
	{
		if (CollectionMergeId > 0 && NKMTempletContainer<NKMEventCollectionMergeTemplet>.Find(CollectionMergeId) == null)
		{
			NKMTempletError.Add($"[NKMEventCollectionIndexTemplet:{Key}] CollectionMergeId를 키로 가진 NKMEventCollectionMergeTemplet가 없음 CollectionMergeId:{CollectionMergeId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionIndexTemplet.cs", 148);
		}
		NKMEventCollectionTemplet nKMEventCollectionTemplet = null;
		if (EventCollectionGroupId > 0)
		{
			nKMEventCollectionTemplet = NKMTempletContainer<NKMEventCollectionTemplet>.Find(EventCollectionGroupId);
			if (nKMEventCollectionTemplet == null)
			{
				NKMTempletError.Add($"[NKMEventCollectionIndexTemplet:{Key}] EventCollectionGroupId를 키로 가진 NKMEventCollectionTemplet가 없음 EventCollectionGroupId:{EventCollectionGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionIndexTemplet.cs", 161);
			}
		}
		if (CollectionMergeId <= 0)
		{
			return;
		}
		NKMEventCollectionMergeTemplet nKMEventCollectionMergeTemplet = NKMTempletContainer<NKMEventCollectionMergeTemplet>.Find(CollectionMergeId);
		if (nKMEventCollectionMergeTemplet == null)
		{
			NKMTempletError.Add($"[NKMEventCollectionIndexTemplet:{Key}] CollectionMergeId를 키로 가진 NKMEventCollectionMergeTemplet이 없음. CollectionMergeId:{CollectionMergeId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionIndexTemplet.cs", 171);
		}
		else
		{
			if (nKMEventCollectionTemplet == null)
			{
				return;
			}
			foreach (NKMEventCollectionMergeRecipeTemplet recipeTemplet in nKMEventCollectionMergeTemplet.RecipeTemplets)
			{
				if (!nKMEventCollectionTemplet.Details.Any((NKMEventCollectionDetailTemplet e) => e.CollectionGradeGroupId == recipeTemplet.MergeInputGradeGroupId) || !nKMEventCollectionTemplet.Details.Any((NKMEventCollectionDetailTemplet e) => e.CollectionGradeGroupId == recipeTemplet.MergeOutputGradeGroupId))
				{
					NKMTempletError.Add($"[NKMEventCollectionIndexTemplet:{Key}] collectionTemplet에 합성 재료/결과에 사용되는 gradeGroupId가 없음. collectionGroupId:{EventCollectionGroupId} mergeId:{CollectionMergeId} mergeInputGroupId:{recipeTemplet.MergeInputGradeGroupId} mergeOutputGroupId:{recipeTemplet.MergeOutputGradeGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Event/NKMEventCollectionIndexTemplet.cs", 183);
				}
			}
		}
	}

	public static NKMEventCollectionIndexTemplet GetEventCollectionIndexTemplet(DateTime serviceTime)
	{
		foreach (NKMEventCollectionIndexTemplet value in Values)
		{
			if (value != null && value.IsOpen)
			{
				NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(value.DateStrId);
				if (nKMIntervalTemplet != null && nKMIntervalTemplet.IsValidTime(serviceTime))
				{
					return value;
				}
			}
		}
		return null;
	}

	public void PostJoin()
	{
		JoinTemplet();
		m_Option = NKCUtil.ParseStringTable(m_strOption);
	}
}
