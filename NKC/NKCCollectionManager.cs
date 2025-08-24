using System;
using System.Collections.Generic;
using Cs.Logging;
using NKC.Templet;
using NKC.Templet.Base;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKCCollectionManager
{
	public enum COLLECTION_STORY_CATEGORY
	{
		MAINSTREAM,
		SIDESTORY,
		EVENT,
		WORLDMAP,
		ETC,
		SEASONAL,
		Count
	}

	public enum COLLECTION_ETC_TAB_ID
	{
		TAB_DIVE = -1,
		TAB_TRIM = -2,
		TAB_BIRTHDAY = -3
	}

	private static Dictionary<int, NKCCollectionIllustTemplet> m_dicNKCCollectionIllustTemplet = new Dictionary<int, NKCCollectionIllustTemplet>(100);

	private static Dictionary<int, NKCCollectionUnitTemplet> m_dicNKCCollectionUnitTemplet = new Dictionary<int, NKCCollectionUnitTemplet>(300);

	private static Dictionary<int, NKCCollectionTagTemplet> m_dicNKCCollectionTagTemplet = new Dictionary<int, NKCCollectionTagTemplet>(100);

	private static Dictionary<int, storyUnlockData> m_dicNKCCollectionStoryTemplet = new Dictionary<int, storyUnlockData>(300);

	private static Dictionary<int, List<NKCCollectionEtcCutsceneTemplet>> m_dicNKCCollectionEtcCutsceneTemplet = new Dictionary<int, List<NKCCollectionEtcCutsceneTemplet>>();

	private static List<storyUnlockData> m_lstSkinTemplet = new List<storyUnlockData>();

	private static Dictionary<int, NKCollectionStoryTemplet> m_dicCollectionStoryData = new Dictionary<int, NKCollectionStoryTemplet>(300);

	private static Dictionary<int, List<int>> m_dicEpisodeStageIdData = new Dictionary<int, List<int>>(300);

	private static Dictionary<int, NKCCollectionMenuTemplet> m_dicCollectionMenuTemplet = new Dictionary<int, NKCCollectionMenuTemplet>();

	private static Dictionary<NKM_ITEM_MISC_TYPE, Dictionary<int, NKMCollectionV2MiscTemplet>> m_dicMiscCollection = new Dictionary<NKM_ITEM_MISC_TYPE, Dictionary<int, NKMCollectionV2MiscTemplet>>();

	private static Dictionary<int, NKCCollectionEmployeeTemplet> m_dicCollectionEmployeeTemplet = new Dictionary<int, NKCCollectionEmployeeTemplet>();

	private static Dictionary<string, List<NKCCollectionCRFTemplet>> m_dicCollectionCRFTemplet = new Dictionary<string, List<NKCCollectionCRFTemplet>>();

	private static Dictionary<string, NKCCollectionProfileToolTipTemplet> m_dicCollectionProfileToolTipTemplet = new Dictionary<string, NKCCollectionProfileToolTipTemplet>();

	public static Dictionary<int, List<NKCUnitTagData>> m_dic_UnitTagData = new Dictionary<int, List<NKCUnitTagData>>(100);

	private static List<int> m_lstCollectionUnit = new List<int>();

	private static List<int> m_lstCollectionShip = new List<int>();

	private static List<int> m_lstCollectionOperator = new List<int>();

	private static bool m_bPreLoading = false;

	public static bool IsCollectionV2Active => NKMOpenTagManager.IsOpened("COLLECTION_V2");

	public static bool IsProfileCollectionV2Opened
	{
		get
		{
			if (NKMOpenTagManager.IsOpened("COLLECTION_V2"))
			{
				return NKMOpenTagManager.IsOpened("COLLECTION_V2_PROFILE");
			}
			return false;
		}
	}

	public static bool IsMiscCollectionOpened
	{
		get
		{
			if (NKMOpenTagManager.IsOpened("COLLECTION_V2"))
			{
				return NKMOpenTagManager.IsOpened("COLLECTION_V2_MISC");
			}
			return false;
		}
	}

	public static Dictionary<int, NKCCollectionIllustTemplet> GetIllustData()
	{
		return m_dicNKCCollectionIllustTemplet;
	}

	public static Dictionary<int, NKCCollectionUnitTemplet> GetUnitData()
	{
		return m_dicNKCCollectionUnitTemplet;
	}

	public static Dictionary<int, NKCCollectionTagTemplet> GetTagData()
	{
		return m_dicNKCCollectionTagTemplet;
	}

	public static Dictionary<int, storyUnlockData> GetStoryData()
	{
		return m_dicNKCCollectionStoryTemplet;
	}

	public static Dictionary<int, List<int>> GetEpiSodeStageIdData()
	{
		return m_dicEpisodeStageIdData;
	}

	public static Dictionary<int, NKCCollectionMenuTemplet> GetCollectionMenuData()
	{
		return m_dicCollectionMenuTemplet;
	}

	public static Dictionary<int, List<NKCCollectionEtcCutsceneTemplet>> GetCollectionCutsceneData()
	{
		return m_dicNKCCollectionEtcCutsceneTemplet;
	}

	public static Dictionary<int, NKMCollectionV2MiscTemplet> GetMiscCollectionData(NKM_ITEM_MISC_TYPE miscType)
	{
		if (m_dicMiscCollection.TryGetValue(miscType, out var value))
		{
			return value;
		}
		return null;
	}

	public static Dictionary<int, NKCCollectionEmployeeTemplet> GetCollectionEmployeeData()
	{
		return m_dicCollectionEmployeeTemplet;
	}

	public static Dictionary<string, List<NKCCollectionCRFTemplet>> GetCollectionCRFData()
	{
		return m_dicCollectionCRFTemplet;
	}

	public static COLLECTION_STORY_CATEGORY GetCollectionStoryCategory(EPISODE_CATEGORY category)
	{
		return category switch
		{
			EPISODE_CATEGORY.EC_MAINSTREAM => COLLECTION_STORY_CATEGORY.MAINSTREAM, 
			EPISODE_CATEGORY.EC_SIDESTORY => COLLECTION_STORY_CATEGORY.SIDESTORY, 
			EPISODE_CATEGORY.EC_SEASONAL => COLLECTION_STORY_CATEGORY.SEASONAL, 
			_ => COLLECTION_STORY_CATEGORY.EVENT, 
		};
	}

	public static string GetTagTitle(short tagType)
	{
		NKCCollectionTagTemplet nKCCollectionTagTemplet = Find(tagType, ref m_dicNKCCollectionTagTemplet);
		if (nKCCollectionTagTemplet != null)
		{
			return nKCCollectionTagTemplet.GetTagName();
		}
		return "";
	}

	public static void SetVote(int unitID, short tagType, int tagCount, bool voted)
	{
		if (!m_dic_UnitTagData.ContainsKey(unitID))
		{
			Debug.Log($"NKCCollectionManager.SetVote() - invaild unit id {unitID}");
			return;
		}
		NKCUnitTagData nKCUnitTagData = m_dic_UnitTagData[unitID].Find((NKCUnitTagData x) => x.TagType == tagType);
		nKCUnitTagData.VoteCount = tagCount;
		nKCUnitTagData.Voted = voted;
	}

	public static bool IsVoted(int unitID, short tagType)
	{
		if (!m_dic_UnitTagData.ContainsKey(unitID))
		{
			Debug.Log($"NKCCollectionManager.IsVoted() - invaild unit id {unitID}");
			return false;
		}
		return m_dic_UnitTagData[unitID].Find((NKCUnitTagData x) => x.TagType == tagType).Voted;
	}

	public static List<NKCUnitTagData> GetUnitTagData(int unitID)
	{
		if (m_dic_UnitTagData.ContainsKey(unitID))
		{
			return m_dic_UnitTagData[unitID];
		}
		return null;
	}

	public static int GetUnitVoteCount(int unitID, short type)
	{
		if (m_dic_UnitTagData.ContainsKey(unitID))
		{
			return m_dic_UnitTagData[unitID].Find((NKCUnitTagData x) => x.TagType == type).VoteCount;
		}
		return 0;
	}

	public static void SetUnitTagData(int unitID, List<NKCUnitTagData> lst)
	{
		if (lst.Count > 1)
		{
			lst.Sort((NKCUnitTagData a, NKCUnitTagData b) => a.VoteCount.CompareTo(b.VoteCount));
		}
		List<NKCUnitTagData> list = new List<NKCUnitTagData>();
		foreach (KeyValuePair<int, NKCCollectionTagTemplet> tag in m_dicNKCCollectionTagTemplet)
		{
			NKCUnitTagData nKCUnitTagData = lst.Find((NKCUnitTagData x) => x.TagType == tag.Value.m_TagOrder);
			if (nKCUnitTagData != null)
			{
				list.Add(nKCUnitTagData);
			}
			else
			{
				list.Add(new NKCUnitTagData(tag.Value.m_TagOrder, vote: false, 0, top: false));
			}
		}
		if (m_dic_UnitTagData.ContainsKey(unitID))
		{
			m_dic_UnitTagData[unitID] = list;
		}
		else
		{
			m_dic_UnitTagData.Add(unitID, list);
		}
	}

	public static List<int> GetUnitList(NKM_UNIT_TYPE type)
	{
		return type switch
		{
			NKM_UNIT_TYPE.NUT_OPERATOR => m_lstCollectionOperator, 
			NKM_UNIT_TYPE.NUT_SHIP => m_lstCollectionShip, 
			NKM_UNIT_TYPE.NUT_NORMAL => m_lstCollectionUnit, 
			_ => new List<int>(), 
		};
	}

	public static T Find<T>(int key, ref Dictionary<int, T> data)
	{
		data.TryGetValue(key, out var value);
		return value;
	}

	public static void SetReloadCollectionData()
	{
		m_bPreLoading = false;
	}

	public static void Init()
	{
		if (!m_bPreLoading)
		{
			m_dicNKCCollectionUnitTemplet.Clear();
			m_dicNKCCollectionTagTemplet.Clear();
			m_dicNKCCollectionIllustTemplet.Clear();
			m_dicNKCCollectionStoryTemplet.Clear();
			m_dicNKCCollectionEtcCutsceneTemplet.Clear();
			m_dicEpisodeStageIdData.Clear();
			m_lstSkinTemplet.Clear();
			m_dicCollectionMenuTemplet.Clear();
			m_dicMiscCollection.Clear();
			m_dicCollectionEmployeeTemplet.Clear();
			m_dicCollectionCRFTemplet.Clear();
			DivideCollectionUnitData();
			Load("AB_SCRIPT", "LUA_COLLECTION_TAG_TEMPLET", "COLLECTION_TAG_TEMPLET", NKCCollectionTagTemplet.LoadFromLUA, ref m_dicNKCCollectionTagTemplet);
			Load("AB_SCRIPT", "LUA_COLLECTION_ILLUST_TEMPLET", "COLLECTION_ILLUST_TEMPLET", TempNKCCollectionIllustTemplet.LoadFromLUA, ref m_dicNKCCollectionIllustTemplet);
			Load("AB_SCRIPT", "LUA_COLLECTION_CUTSCENE_TEMPLET", "COLLECTION_CUTSCENE_TEMPLET", NKMCollectionStoryTemplet.LoadFromLUA, ref m_dicNKCCollectionStoryTemplet, ref m_lstSkinTemplet, bTempletNullAllowed: true);
			Load("AB_SCRIPT", "LUA_COLLECTION_V2_INDEX", "COLLECTION_V2_INDEX", NKCCollectionMenuTemplet.LoadFromLUA, ref m_dicCollectionMenuTemplet);
			Load("AB_SCRIPT", "LUA_COLLECTION_V2_EMPLOYEE", "COLLECTION_V2_EMPLOYEE", NKCCollectionEmployeeTemplet.LoadFromLUA, ref m_dicCollectionEmployeeTemplet);
			LoadCollectionEtcTemplet(ref m_dicNKCCollectionEtcCutsceneTemplet);
			LoadCollectionCRFData(ref m_dicCollectionCRFTemplet);
			if (!NKMTempletContainer<NKMCollectionV2MiscTemplet>.HasValue())
			{
				NKMTempletContainer<NKMCollectionV2MiscTemplet>.Load("AB_SCRIPT", "LUA_COLLECTION_V2_MISC", "COLLECTION_V2_MISC", NKMCollectionV2MiscTemplet.LoadFromLUA);
				NKMTempletContainer<NKMCollectionV2MiscTemplet>.Join();
			}
			MakeCollectionMiscData(ref m_dicMiscCollection);
			LoadCollectionProfileDescData(ref m_dicCollectionProfileToolTipTemplet);
			m_bPreLoading = true;
		}
	}

	private static void DivideCollectionUnitData()
	{
		Load("AB_SCRIPT", "LUA_COLLECTION_UNIT_TEMPLET", "COLLECTION_UNIT_TEMPLET", NKCCollectionUnitTemplet.LoadFromLUA, ref m_dicNKCCollectionUnitTemplet, bTempletNullAllowed: true);
		m_lstCollectionUnit.Clear();
		m_lstCollectionShip.Clear();
		m_lstCollectionOperator.Clear();
		foreach (KeyValuePair<int, NKCCollectionUnitTemplet> item in m_dicNKCCollectionUnitTemplet)
		{
			NKCCollectionUnitTemplet value = item.Value;
			switch (value.m_NKM_UNIT_TYPE)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				m_lstCollectionUnit.Add(value.m_UnitID);
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
				m_lstCollectionShip.Add(value.m_UnitID);
				break;
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				m_lstCollectionOperator.Add(value.m_UnitID);
				break;
			}
		}
	}

	public static NKCCollectionIllustTemplet GetIllustTemplet(int key)
	{
		return Find(key, ref m_dicNKCCollectionIllustTemplet);
	}

	public static NKCCollectionUnitTemplet GetUnitTemplet(int key)
	{
		return Find(key, ref m_dicNKCCollectionUnitTemplet);
	}

	public static string GetEmployeeNumber(int key)
	{
		string result = "";
		NKCCollectionUnitTemplet nKCCollectionUnitTemplet = Find(key, ref m_dicNKCCollectionUnitTemplet);
		if (nKCCollectionUnitTemplet != null)
		{
			result = ((nKCCollectionUnitTemplet.Idx >= 100) ? nKCCollectionUnitTemplet.Idx.ToString() : ((nKCCollectionUnitTemplet.Idx < 10) ? ("00" + nKCCollectionUnitTemplet.Idx) : ("0" + nKCCollectionUnitTemplet.Idx)));
		}
		return result;
	}

	public static NKCCollectionTagTemplet GetTagTemplet(int idx)
	{
		return Find(idx, ref m_dicNKCCollectionTagTemplet);
	}

	public static List<storyUnlockData> GetSkinStoryData()
	{
		return m_lstSkinTemplet;
	}

	public static string GetVoiceActorName(int unitId)
	{
		return NKCVoiceActorNameTemplet.FindActorName(GetUnitTemplet(unitId));
	}

	public static NKCCollectionEmployeeTemplet GetEmployeeTemplet(int unitId)
	{
		return Find(unitId, ref m_dicCollectionEmployeeTemplet);
	}

	public static NKCCollectionCRFTemplet GetCRFTemplet(string CRFType, int amount)
	{
		if (!m_dicCollectionCRFTemplet.ContainsKey(CRFType))
		{
			return null;
		}
		return m_dicCollectionCRFTemplet[CRFType].Find((NKCCollectionCRFTemplet e) => e.ProfileAmountMin <= amount && e.ProfileAmountMax >= amount);
	}

	public static NKCCollectionProfileToolTipTemplet GetProfileToolTipTemplet(string profileType)
	{
		if (m_dicCollectionProfileToolTipTemplet.TryGetValue(profileType, out var value))
		{
			return value;
		}
		return null;
	}

	public static List<int> GetMiscIdList(NKM_ITEM_MISC_TYPE type)
	{
		List<int> list = new List<int>();
		Dictionary<int, NKMCollectionV2MiscTemplet> miscCollectionData = GetMiscCollectionData(type);
		if (miscCollectionData == null)
		{
			return list;
		}
		foreach (NKMCollectionV2MiscTemplet value in miscCollectionData.Values)
		{
			if (value.EnableByTag && (!value.Exclude || NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(value.CollectionItemId) > 0))
			{
				list.Add(value.CollectionItemId);
			}
		}
		return list;
	}

	public static List<int> GetMiscHaveList(List<int> miscIdList)
	{
		List<int> list = new List<int>();
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		if (miscIdList != null)
		{
			foreach (int miscId in miscIdList)
			{
				if (inventoryData.GetCountMiscItem(miscId) > 0)
				{
					list.Add(miscId);
					continue;
				}
				NKMCollectionV2MiscTemplet nKMCollectionV2MiscTemplet = NKMCollectionV2MiscTemplet.Find(miscId);
				if (nKMCollectionV2MiscTemplet != null && nKMCollectionV2MiscTemplet.DefaultCollection)
				{
					list.Add(miscId);
				}
			}
		}
		return list;
	}

	public static List<int> GetMiscRewardEnabledIdList(List<int> lstMiscIdHave)
	{
		List<int> list = new List<int>();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && lstMiscIdHave != null)
		{
			foreach (int item in lstMiscIdHave)
			{
				NKMCollectionV2MiscTemplet nKMCollectionV2MiscTemplet = NKMCollectionV2MiscTemplet.Find(item);
				if (nKMCollectionV2MiscTemplet == null || !nKMCollectionV2MiscTemplet.DefaultCollection)
				{
					NKMMiscCollectionData miscCollectionData = nKMUserData.m_InventoryData.GetMiscCollectionData(item);
					if (miscCollectionData == null || !miscCollectionData.IsRewardComplete())
					{
						list.Add(item);
					}
				}
			}
		}
		return list;
	}

	public static bool IsMiscRewardEnable()
	{
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		foreach (NKMCollectionV2MiscTemplet value in NKMTempletContainer<NKMCollectionV2MiscTemplet>.Values)
		{
			if (!value.DefaultCollection)
			{
				NKMMiscCollectionData miscCollectionData = inventoryData.GetMiscCollectionData(value.CollectionItemId);
				if ((miscCollectionData == null || !miscCollectionData.IsRewardComplete()) && inventoryData.GetCountMiscItem(value.CollectionItemId) > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsMiscRewardEnable(NKM_ITEM_MISC_TYPE type)
	{
		Dictionary<int, NKMCollectionV2MiscTemplet> miscCollectionData = GetMiscCollectionData(type);
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		foreach (NKMCollectionV2MiscTemplet value in miscCollectionData.Values)
		{
			if (!value.DefaultCollection)
			{
				NKMMiscCollectionData miscCollectionData2 = inventoryData.GetMiscCollectionData(value.CollectionItemId);
				if ((miscCollectionData2 == null || !miscCollectionData2.IsRewardComplete()) && inventoryData.GetCountMiscItem(value.CollectionItemId) > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static void Load<T>(string assetName, string fileName, string tableName, Func<NKMLua, T> factory, ref Dictionary<int, T> data, bool bTempletNullAllowed = false) where T : INKCTemplet
	{
		string name = typeof(T).Name;
		NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath(assetName, fileName))
		{
			Log.ErrorAndExit("[" + name + "] lua file loading fail. assetName:" + assetName + " fileName:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 744);
		}
		if (!nKMLua.OpenTable(tableName))
		{
			Log.ErrorAndExit("[" + name + "] lua table open fail. fileName:" + fileName + " tableName:" + tableName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 749);
		}
		int num = 1;
		while (nKMLua.OpenTable(num))
		{
			T val = factory(nKMLua);
			if (!bTempletNullAllowed)
			{
				if (val == null)
				{
					Log.ErrorAndExit($"[{name}] data load fail. fileName:{fileName} tableName:{tableName} index:{num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 761);
					break;
				}
			}
			else if (val == null)
			{
				num++;
				nKMLua.CloseTable();
				continue;
			}
			if (data.ContainsKey(val.Key))
			{
				Log.ErrorAndExit($"[{name}] Table contains duplicate key. tableName:{tableName} key:{val.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 780);
				break;
			}
			data.Add(val.Key, val);
			num++;
			nKMLua.CloseTable();
		}
		nKMLua.CloseTable();
		nKMLua.LuaClose();
	}

	private static void Load<T>(string assetName, string fileName, string tableName, Func<NKMLua, T> factory, ref Dictionary<int, NKCCollectionIllustTemplet> data) where T : INKCTemplet
	{
		string name = typeof(T).Name;
		NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath(assetName, fileName))
		{
			Log.ErrorAndExit("[" + name + "] lua file loading fail. assetName:" + assetName + " fileName:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 800);
		}
		if (!nKMLua.OpenTable(tableName))
		{
			Log.ErrorAndExit("[" + name + "] lua table open fail. fileName:" + fileName + " tableName:" + tableName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 805);
		}
		int num = 1;
		while (nKMLua.OpenTable(num))
		{
			T val = factory(nKMLua);
			if (val == null)
			{
				Log.ErrorAndExit($"[{name}] data load fail. fileName:{fileName} tableName:{tableName} index:{num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 814);
				break;
			}
			if (data.ContainsKey(val.Key))
			{
				NKCCollectionIllustTemplet nKCCollectionIllustTemplet = data[val.Key];
				if (val is TempNKCCollectionIllustTemplet tempNKCCollectionIllustTemplet)
				{
					Dictionary<int, NKCCollectionIllustData> dicIllustData = nKCCollectionIllustTemplet.m_dicIllustData;
					if (dicIllustData.ContainsKey(tempNKCCollectionIllustTemplet.m_BGGroupID))
					{
						NKCCollectionIllustData nKCCollectionIllustData = dicIllustData[tempNKCCollectionIllustTemplet.m_BGGroupID];
						if (nKCCollectionIllustData != null)
						{
							NKCIllustFileData item = new NKCIllustFileData(tempNKCCollectionIllustTemplet.m_BGThumbnailFileName, tempNKCCollectionIllustTemplet.m_BGFileName, tempNKCCollectionIllustTemplet.m_GameObjectBGAniName);
							nKCCollectionIllustData.m_FileData.Add(item);
						}
					}
					else
					{
						NKCCollectionIllustData value = new NKCCollectionIllustData(tempNKCCollectionIllustTemplet.GetBGGroupTitle(), tempNKCCollectionIllustTemplet.GetBGGroupText(), tempNKCCollectionIllustTemplet.m_BGThumbnailFileName, tempNKCCollectionIllustTemplet.m_BGFileName, tempNKCCollectionIllustTemplet.m_GameObjectBGAniName, tempNKCCollectionIllustTemplet.m_UnlockReqType, tempNKCCollectionIllustTemplet.m_UnlockReqValue);
						dicIllustData.Add(tempNKCCollectionIllustTemplet.m_BGGroupID, value);
					}
				}
			}
			else
			{
				if (!(val is TempNKCCollectionIllustTemplet tempNKCCollectionIllustTemplet2))
				{
					Log.ErrorAndExit($"[{name}] Load Faile - Table Data. tableName:{tableName} key:{val.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 847);
					break;
				}
				NKCCollectionIllustData illustData = new NKCCollectionIllustData(tempNKCCollectionIllustTemplet2.GetBGGroupTitle(), tempNKCCollectionIllustTemplet2.GetBGGroupText(), tempNKCCollectionIllustTemplet2.m_BGThumbnailFileName, tempNKCCollectionIllustTemplet2.m_BGFileName, tempNKCCollectionIllustTemplet2.m_GameObjectBGAniName, tempNKCCollectionIllustTemplet2.m_UnlockReqType, tempNKCCollectionIllustTemplet2.m_UnlockReqValue);
				NKCCollectionIllustTemplet value2 = new NKCCollectionIllustTemplet(tempNKCCollectionIllustTemplet2.m_CategoryID, tempNKCCollectionIllustTemplet2.m_CategoryTitle, tempNKCCollectionIllustTemplet2.m_CategorySubTitle, tempNKCCollectionIllustTemplet2.m_BGGroupID, illustData);
				data.Add(tempNKCCollectionIllustTemplet2.m_CategoryID, value2);
			}
			num++;
			nKMLua.CloseTable();
		}
		nKMLua.CloseTable();
		nKMLua.LuaClose();
	}

	private static void Load<T>(string assetName, string fileName, string tableName, Func<NKMLua, T> factory, ref Dictionary<int, storyUnlockData> data, ref List<storyUnlockData> lstShopCutScene, bool bTempletNullAllowed = false) where T : INKMTemplet
	{
		string name = typeof(T).Name;
		NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath(assetName, fileName))
		{
			Log.ErrorAndExit("[" + name + "] lua file loading fail. assetName:" + assetName + " fileName:" + fileName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 868);
		}
		if (!nKMLua.OpenTable(tableName))
		{
			Log.ErrorAndExit("[" + name + "] lua table open fail. fileName:" + fileName + " tableName:" + tableName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 873);
		}
		int num = 1;
		while (nKMLua.OpenTable(num))
		{
			T val = factory(nKMLua);
			if (!bTempletNullAllowed)
			{
				if (val == null)
				{
					Log.ErrorAndExit($"[{name}] data load fail. fileName:{fileName} tableName:{tableName} index:{num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 885);
					break;
				}
			}
			else if (val == null)
			{
				num++;
				nKMLua.CloseTable();
				continue;
			}
			if (!(val is NKMCollectionStoryTemplet nKMCollectionStoryTemplet))
			{
				Log.ErrorAndExit($"[{name}] Load Faile - Table Data. tableName:{tableName} key:{val.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 902);
			}
			else if (nKMCollectionStoryTemplet.m_StageID == 0)
			{
				lstShopCutScene.Add(new storyUnlockData(nKMCollectionStoryTemplet.m_UnlockReqList, nKMCollectionStoryTemplet.m_EPCategory, nKMCollectionStoryTemplet.m_EpisodeID, nKMCollectionStoryTemplet.m_ActID, nKMCollectionStoryTemplet.m_StageID));
			}
			else
			{
				data.Add(nKMCollectionStoryTemplet.m_StageID, new storyUnlockData(nKMCollectionStoryTemplet.m_UnlockReqList, nKMCollectionStoryTemplet.m_EPCategory, nKMCollectionStoryTemplet.m_EpisodeID, nKMCollectionStoryTemplet.m_ActID, nKMCollectionStoryTemplet.m_StageID));
				if (!m_dicEpisodeStageIdData.ContainsKey(nKMCollectionStoryTemplet.m_EpisodeID))
				{
					m_dicEpisodeStageIdData.Add(nKMCollectionStoryTemplet.m_EpisodeID, new List<int>());
				}
				m_dicEpisodeStageIdData[nKMCollectionStoryTemplet.m_EpisodeID].Add(nKMCollectionStoryTemplet.m_StageID);
			}
			num++;
			nKMLua.CloseTable();
		}
		nKMLua.CloseTable();
		nKMLua.LuaClose();
	}

	private static void LoadCollectionCRFData(ref Dictionary<string, List<NKCCollectionCRFTemplet>> dicCollectionCRFData)
	{
		Dictionary<int, NKCCollectionCRFTemplet> data = new Dictionary<int, NKCCollectionCRFTemplet>();
		Load("AB_SCRIPT", "LUA_COLLECTION_V2_CRF", "COLLECTION_V2_CRF", NKCCollectionCRFTemplet.LoadFromLUA, ref data);
		foreach (NKCCollectionCRFTemplet value in data.Values)
		{
			if (!dicCollectionCRFData.ContainsKey(value.CRFType))
			{
				dicCollectionCRFData.Add(value.CRFType, new List<NKCCollectionCRFTemplet>());
			}
			dicCollectionCRFData[value.CRFType].Add(value);
		}
	}

	private static void LoadCollectionEtcTemplet(ref Dictionary<int, List<NKCCollectionEtcCutsceneTemplet>> dicCollectionEtcData)
	{
		Dictionary<int, NKCCollectionEtcCutsceneTemplet> data = new Dictionary<int, NKCCollectionEtcCutsceneTemplet>();
		Load("AB_SCRIPT", "LUA_COLLECTION_CUTSCENE_TEMPLET2", "COLLECTION_CUTSCENE_TEMPLET2", NKCCollectionEtcCutsceneTemplet.LoadFromLUA, ref data);
		foreach (NKCCollectionEtcCutsceneTemplet value in data.Values)
		{
			if (!dicCollectionEtcData.ContainsKey((int)value.m_SubTabId))
			{
				dicCollectionEtcData.Add((int)value.m_SubTabId, new List<NKCCollectionEtcCutsceneTemplet>());
			}
			dicCollectionEtcData[(int)value.m_SubTabId].Add(value);
		}
		foreach (List<NKCCollectionEtcCutsceneTemplet> value2 in dicCollectionEtcData.Values)
		{
			value2.Sort(SortByValue);
		}
	}

	private static int SortByValue(NKCCollectionEtcCutsceneTemplet lItem, NKCCollectionEtcCutsceneTemplet rItem)
	{
		return lItem.UnlockInfo.reqValue.CompareTo(rItem.UnlockInfo.reqValue);
	}

	private static void MakeCollectionMiscData(ref Dictionary<NKM_ITEM_MISC_TYPE, Dictionary<int, NKMCollectionV2MiscTemplet>> dicCollectionMisc)
	{
		foreach (NKMCollectionV2MiscTemplet value2 in NKMTempletContainer<NKMCollectionV2MiscTemplet>.Values)
		{
			if (value2 != null)
			{
				if (!dicCollectionMisc.TryGetValue(value2.CollectionMiscTemplet.m_ItemMiscType, out var value))
				{
					value = new Dictionary<int, NKMCollectionV2MiscTemplet>();
					dicCollectionMisc.Add(value2.CollectionMiscTemplet.m_ItemMiscType, value);
				}
				if (value.ContainsKey(value2.CollectionItemId))
				{
					Log.Error($"[LUA_COLLECTION_V2_MISC] CollectionItemId {value2.CollectionItemId} is duplicated", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 979);
				}
				else
				{
					value.Add(value2.CollectionItemId, value2);
				}
			}
		}
	}

	private static void LoadCollectionProfileDescData(ref Dictionary<string, NKCCollectionProfileToolTipTemplet> dicCollectionProfileToopTipTemplet)
	{
		Dictionary<int, NKCCollectionProfileToolTipTemplet> data = new Dictionary<int, NKCCollectionProfileToolTipTemplet>();
		Load("AB_SCRIPT", "LUA_COLLECTION_V2_PROFILE_TYPE", "COLLECTION_V2_PROFILE_TYPE", NKCCollectionProfileToolTipTemplet.LoadFromLUA, ref data);
		foreach (NKCCollectionProfileToolTipTemplet value in data.Values)
		{
			if (!dicCollectionProfileToopTipTemplet.ContainsKey(value.ProfileType))
			{
				dicCollectionProfileToopTipTemplet.Add(value.ProfileType, value);
			}
			else
			{
				Log.Error("LUA_COLLECTION_V2_PROFILE_TYPE has duplicated ProfileType", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 995);
			}
		}
	}

	public static EventUnlockCond GetEventUnlockCond(STAGE_UNLOCK_REQ_TYPE stageUnlockReqType)
	{
		if (stageUnlockReqType == STAGE_UNLOCK_REQ_TYPE.SURT_HISTORY_BIRTHDAY)
		{
			return EventUnlockCond.Birthday;
		}
		return EventUnlockCond.None;
	}
}
