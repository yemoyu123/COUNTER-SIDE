using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;

namespace NKC;

public class NKCEventCollectionTemplet : INKMTemplet
{
	public int m_Idx;

	public string m_OpenTag;

	public string DateStrID;

	public string EventBannerStrID;

	public string EventBannerTitleStrID;

	public string EventPrefabID;

	public string BgmAssetID;

	public int BgmVolume;

	public string EventContractPrefabID;

	public int EventContractID;

	public string EventMissionPrefabID;

	public HashSet<int> EventMissionTabID;

	public string EventMergePrefabID;

	public int CollectionMergeID;

	public string EventCollectionPrefabID;

	public int EventCollectionGroupID;

	public string EventShopPrefabID;

	public string ShopShortCutType;

	public string ShopShortCut;

	public int Key => m_Idx;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static NKCEventCollectionTemplet Find(int index)
	{
		return NKMTempletContainer<NKCEventCollectionTemplet>.Find(index);
	}

	public static NKCEventCollectionTemplet LoadFromLUA(NKMLua lua)
	{
		NKCEventCollectionTemplet nKCEventCollectionTemplet = new NKCEventCollectionTemplet();
		bool flag = true;
		flag &= lua.GetData("EventID", ref nKCEventCollectionTemplet.m_Idx);
		flag &= lua.GetData("OpenTag", ref nKCEventCollectionTemplet.m_OpenTag);
		flag &= lua.GetData("DateStrID", ref nKCEventCollectionTemplet.DateStrID);
		flag &= lua.GetData("EventBannerStrID", ref nKCEventCollectionTemplet.EventBannerStrID);
		flag &= lua.GetData("EventPrefabID", ref nKCEventCollectionTemplet.EventPrefabID);
		flag &= lua.GetData("BgmAssetID", ref nKCEventCollectionTemplet.BgmAssetID);
		flag &= lua.GetData("BgmVolume", ref nKCEventCollectionTemplet.BgmVolume);
		flag &= lua.GetData("EventContractID", ref nKCEventCollectionTemplet.EventContractID);
		flag &= lua.GetData("EventContractPrefabID", ref nKCEventCollectionTemplet.EventContractPrefabID);
		flag &= lua.GetData("EventMissionPrefabID", ref nKCEventCollectionTemplet.EventMissionPrefabID);
		if (lua.OpenTable("EventMissionTabID"))
		{
			nKCEventCollectionTemplet.EventMissionTabID = new HashSet<int>();
			int i = 1;
			for (int rValue = 0; lua.GetData(i, ref rValue); i++)
			{
				nKCEventCollectionTemplet.EventMissionTabID.Add(rValue);
			}
			lua.CloseTable();
		}
		flag &= lua.GetData("EventMergePrefabID", ref nKCEventCollectionTemplet.EventMergePrefabID);
		flag &= lua.GetData("CollectionMergeID", ref nKCEventCollectionTemplet.CollectionMergeID);
		flag &= lua.GetData("EventCollectionPrefabID", ref nKCEventCollectionTemplet.EventCollectionPrefabID);
		flag &= lua.GetData("EventCollectionGroupID", ref nKCEventCollectionTemplet.EventCollectionGroupID);
		flag &= lua.GetData("EventShopPrefabID", ref nKCEventCollectionTemplet.EventShopPrefabID);
		flag &= lua.GetData("ShopShortCutType", ref nKCEventCollectionTemplet.ShopShortCutType);
		if (!(flag & lua.GetData("ShopShortCut", ref nKCEventCollectionTemplet.ShopShortCut)))
		{
			return null;
		}
		return nKCEventCollectionTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
