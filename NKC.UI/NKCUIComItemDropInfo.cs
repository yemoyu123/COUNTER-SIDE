using System.Collections.Generic;
using System.Linq;
using NKC.Templet;
using NKC.UI.Trim;
using NKM;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComItemDropInfo : MonoBehaviour
{
	public LoopScrollRect m_LoopScrollRect;

	private string m_ItemID;

	private List<ItemDropInfo> m_ItemDropInfoList = new List<ItemDropInfo>();

	public void Init()
	{
		base.gameObject.SetActive(value: true);
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnMissionSlot;
			m_LoopScrollRect.dOnProvideData += ProvideMissionData;
			m_LoopScrollRect.ContentConstraintCount = 1;
			m_LoopScrollRect.PrepareCells();
		}
		base.gameObject.SetActive(value: false);
	}

	public bool SetData(string itemID, bool initScrollPosition = true)
	{
		if (string.IsNullOrEmpty(itemID) || NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return false;
		}
		NKCItemDropInfoTemplet nKCItemDropInfoTemplet = NKCItemDropInfoTemplet.Find(itemID);
		if (nKCItemDropInfoTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return false;
		}
		m_ItemDropInfoList.Clear();
		SetDropInfo(nKCItemDropInfoTemplet);
		List<ItemDropInfo> list = new List<ItemDropInfo>();
		for (int num = m_ItemDropInfoList.Count - 1; num >= 0; num--)
		{
			if (m_ItemDropInfoList[num] != null && m_ItemDropInfoList[num].ContentType == DropContent.RandomMoldBox)
			{
				list.Add(m_ItemDropInfoList[num]);
				m_ItemDropInfoList.RemoveAt(num);
			}
		}
		list.ForEach(delegate(ItemDropInfo e)
		{
			m_ItemDropInfoList.Insert(0, e);
		});
		list.Clear();
		bool flag = m_ItemDropInfoList.Count > 0;
		base.gameObject.SetActive(flag);
		if (!flag)
		{
			return false;
		}
		m_ItemID = itemID;
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.TotalCount = m_ItemDropInfoList.Count;
			if (initScrollPosition)
			{
				m_LoopScrollRect.SetIndexPosition(0);
			}
			else
			{
				m_LoopScrollRect.RefreshCells();
			}
			if (!m_LoopScrollRect.isActiveAndEnabled)
			{
				m_LoopScrollRect.RefreshCells(bForce: true);
			}
		}
		return true;
	}

	public bool SetData(NKCUISlot.SlotData data, bool initScrollPosition = true)
	{
		if (data == null || data.eType != NKCUISlot.eSlotMode.ItemMisc || NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
		{
			base.gameObject.SetActive(value: false);
			return false;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(data.ID);
		NKCItemDropInfoTemplet nKCItemDropInfoTemplet = null;
		if (itemMiscTempletByID != null && itemMiscTempletByID.m_ItemDropInfo)
		{
			nKCItemDropInfoTemplet = NKCItemDropInfoTemplet.Find(itemMiscTempletByID.m_ItemMiscStrID);
		}
		m_ItemDropInfoList.Clear();
		if (nKCItemDropInfoTemplet != null)
		{
			SetDropInfo(nKCItemDropInfoTemplet);
		}
		List<ItemDropInfo> list = new List<ItemDropInfo>();
		for (int num = m_ItemDropInfoList.Count - 1; num >= 0; num--)
		{
			if (m_ItemDropInfoList[num] != null && m_ItemDropInfoList[num].ContentType == DropContent.RandomMoldBox)
			{
				list.Add(m_ItemDropInfoList[num]);
				m_ItemDropInfoList.RemoveAt(num);
			}
		}
		list.ForEach(delegate(ItemDropInfo e)
		{
			m_ItemDropInfoList.Insert(0, e);
		});
		list.Clear();
		bool flag = m_ItemDropInfoList.Count > 0;
		base.gameObject.SetActive(flag);
		if (!flag)
		{
			return false;
		}
		m_ItemID = itemMiscTempletByID.m_ItemMiscStrID;
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.TotalCount = m_ItemDropInfoList.Count;
			if (initScrollPosition)
			{
				m_LoopScrollRect.SetIndexPosition(0);
			}
			else
			{
				m_LoopScrollRect.RefreshCells();
			}
			if (!m_LoopScrollRect.isActiveAndEnabled)
			{
				m_LoopScrollRect.RefreshCells(bForce: true);
			}
		}
		return true;
	}

	private void SetDropInfo(NKCItemDropInfoTemplet itemDropInfoTemplet)
	{
		HashSet<string> worldMapMissionNameSet = new HashSet<string>();
		Dictionary<EPISODE_CATEGORY, List<ItemDropInfo>> dictionary = new Dictionary<EPISODE_CATEGORY, List<ItemDropInfo>>();
		Dictionary<DropContent, List<ItemDropInfo>> dictionary2 = new Dictionary<DropContent, List<ItemDropInfo>>();
		int count = itemDropInfoTemplet.ItemDropInfoList.Count;
		for (int i = 0; i < count; i++)
		{
			ItemDropInfo itemDropInfo = itemDropInfoTemplet.ItemDropInfoList[i];
			if (!FilterOpenedDropInfo(itemDropInfo, ref worldMapMissionNameSet))
			{
				continue;
			}
			if (itemDropInfo.ContentType == DropContent.Stage)
			{
				NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(itemDropInfo.ContentID);
				if (nKMStageTempletV.EpisodeTemplet != null)
				{
					if (!dictionary.ContainsKey(nKMStageTempletV.EpisodeCategory))
					{
						dictionary.Add(nKMStageTempletV.EpisodeCategory, new List<ItemDropInfo>());
					}
					dictionary[nKMStageTempletV.EpisodeCategory].Add(itemDropInfo);
				}
			}
			else
			{
				if (!dictionary2.ContainsKey(itemDropInfo.ContentType))
				{
					dictionary2.Add(itemDropInfo.ContentType, new List<ItemDropInfo>());
				}
				dictionary2[itemDropInfo.ContentType].Add(itemDropInfo);
			}
		}
		foreach (KeyValuePair<EPISODE_CATEGORY, List<ItemDropInfo>> item in dictionary)
		{
			int stageDropInfoCountLimit = GetStageDropInfoCountLimit(item.Key);
			if (stageDropInfoCountLimit <= 0)
			{
				continue;
			}
			if (item.Value.Count < stageDropInfoCountLimit)
			{
				m_ItemDropInfoList.AddRange(item.Value);
				continue;
			}
			item.Value.Sort(delegate(ItemDropInfo e1, ItemDropInfo e2)
			{
				if (e1.ContentID < e2.ContentID)
				{
					return 1;
				}
				return (e1.ContentID > e2.ContentID) ? (-1) : 0;
			});
			item.Value[0].Summary = true;
			m_ItemDropInfoList.Add(item.Value[0]);
		}
		foreach (KeyValuePair<DropContent, List<ItemDropInfo>> item2 in dictionary2)
		{
			if (GetDropInfoCountLimit(item2.Key) <= 0)
			{
				continue;
			}
			if (item2.Value.Count < GetDropInfoCountLimit(item2.Key))
			{
				m_ItemDropInfoList.AddRange(item2.Value);
				continue;
			}
			item2.Value.Sort(delegate(ItemDropInfo e1, ItemDropInfo e2)
			{
				if (e1.ContentID < e2.ContentID)
				{
					return -1;
				}
				return (e1.ContentID > e2.ContentID) ? 1 : 0;
			});
			item2.Value[0].Summary = true;
			m_ItemDropInfoList.Add(item2.Value[0]);
		}
	}

	private bool FilterOpenedDropInfo(ItemDropInfo itemDropInfo, ref HashSet<string> worldMapMissionNameSet)
	{
		switch (itemDropInfo.ContentType)
		{
		case DropContent.Stage:
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(itemDropInfo.ContentID);
			if (nKMStageTempletV != null)
			{
				bool flag5 = NKMEpisodeMgr.CheckEpisodeMission(NKCScenManager.CurrentUserData(), nKMStageTempletV);
				bool flag6 = nKMStageTempletV.EpisodeTemplet == null || nKMStageTempletV.EpisodeTemplet.IsOpen;
				if (nKMStageTempletV.EpisodeTemplet != null)
				{
					ContentsType contentsType = NKCContentManager.GetContentsType(nKMStageTempletV.EpisodeTemplet.m_EPCategory);
					flag6 &= NKCContentManager.IsContentsUnlocked(contentsType);
				}
				bool flag7 = nKMStageTempletV.IsOpenedDayOfWeek();
				if (flag6 && flag5 && flag7)
				{
					return true;
				}
			}
			break;
		}
		case DropContent.Shop:
		case DropContent.SubStreamShop:
		{
			ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(itemDropInfo.ContentID);
			if (shopItemTemplet == null || (shopItemTemplet.TabTemplet != null && shopItemTemplet.TabTemplet.HasDateLimit) || (shopItemTemplet.TabTemplet != null && shopItemTemplet.m_ChainIndex > NKCShopManager.GetCurrentTargetChainIndex(shopItemTemplet.TabTemplet)) || NKCShopManager.CanBuyFixShop(NKCScenManager.CurrentUserData(), shopItemTemplet, out var _, out var _) != NKM_ERROR_CODE.NEC_OK || NKCShopManager.GetBuyCountLeft(shopItemTemplet.m_ProductID) == 0)
			{
				break;
			}
			bool flag9 = true;
			if (itemDropInfo.ContentType == DropContent.SubStreamShop)
			{
				flag9 = false;
				string shopShortCut = $"{shopItemTemplet.m_TabID}@{shopItemTemplet.m_TabSubIndex}";
				NKMStageTempletV2 nKMStageTempletV2 = NKMStageTempletV2.Values.First((NKMStageTempletV2 e) => e.m_ShopShortcut == shopShortCut);
				if (nKMStageTempletV2 != null && nKMStageTempletV2.EpisodeTemplet != null)
				{
					NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(nKMStageTempletV2.EpisodeTemplet.m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
					flag9 = nKMEpisodeTempletV != null && NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), nKMEpisodeTempletV.GetUnlockInfo());
				}
			}
			if (NKCShopManager.CanExhibitItem(shopItemTemplet) && flag9)
			{
				return true;
			}
			break;
		}
		case DropContent.WorldMapMission:
		{
			NKMWorldMapMissionTemplet nKMWorldMapMissionTemplet = NKMTempletContainer<NKMWorldMapMissionTemplet>.Find(itemDropInfo.ContentID);
			if (nKMWorldMapMissionTemplet != null && !string.IsNullOrEmpty(nKMWorldMapMissionTemplet.m_MissionName) && !worldMapMissionNameSet.Contains(nKMWorldMapMissionTemplet.m_MissionName) && NKCContentManager.IsContentsUnlocked(ContentsType.WORLDMAP) && nKMWorldMapMissionTemplet.m_bEnableMission && nKMWorldMapMissionTemplet.m_eMissionType != NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_INVALID)
			{
				worldMapMissionNameSet.Add(nKMWorldMapMissionTemplet.m_MissionName);
				return true;
			}
			break;
		}
		case DropContent.Raid:
		{
			NKMRaidTemplet nKMRaidTemplet = NKMTempletContainer<NKMRaidTemplet>.Find(itemDropInfo.ContentID);
			bool flag8 = NKMTutorialManager.IsTutorialCompleted(TutorialStep.RaidEvent, NKCScenManager.CurrentUserData());
			if (nKMRaidTemplet != null && flag8)
			{
				return true;
			}
			break;
		}
		case DropContent.Shadow:
		{
			NKMShadowPalaceTemplet nKMShadowPalaceTemplet = NKMTempletContainer<NKMShadowPalaceTemplet>.Find(itemDropInfo.ContentID);
			if (nKMShadowPalaceTemplet != null)
			{
				UnlockInfo unlockInfo = new UnlockInfo(nKMShadowPalaceTemplet.STAGE_UNLOCK_REQ_TYPE, nKMShadowPalaceTemplet.STAGE_UNLOCK_REQ_VALUE);
				bool flag = NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in unlockInfo);
				if (nKMShadowPalaceTemplet != null && flag)
				{
					return true;
				}
			}
			break;
		}
		case DropContent.Dive:
		{
			NKMDiveTemplet nKMDiveTemplet = NKMTempletContainer<NKMDiveTemplet>.Find(itemDropInfo.ContentID);
			if (nKMDiveTemplet != null)
			{
				bool num2 = NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), new UnlockInfo(nKMDiveTemplet.StageUnlockReqType, nKMDiveTemplet.StageUnlockReqValue));
				NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(itemDropInfo.ItemID);
				bool flag3 = nKMItemMiscTemplet != null && nKMItemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_RESOURCE;
				bool flag4 = num2 || flag3;
				if (!nKMDiveTemplet.IsEventDive && nKMDiveTemplet.EnableByTag && flag4)
				{
					return true;
				}
			}
			break;
		}
		case DropContent.Fierce:
		{
			NKMFiercePointRewardTemplet nKMFiercePointRewardTemplet = NKMTempletContainer<NKMFiercePointRewardTemplet>.Find(itemDropInfo.ContentID);
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			int num = 0;
			bool flag2 = false;
			if (nKCFierceBattleSupportDataMgr != null)
			{
				if (nKCFierceBattleSupportDataMgr.FierceTemplet != null)
				{
					num = nKCFierceBattleSupportDataMgr.FierceTemplet.PointRewardGroupID;
				}
				NKCFierceBattleSupportDataMgr.FIERCE_STATUS status = nKCFierceBattleSupportDataMgr.GetStatus();
				flag2 = status != NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_UNUSABLE && status != NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_LOCKED;
			}
			if (nKMFiercePointRewardTemplet != null && num == nKMFiercePointRewardTemplet.FiercePointRewardGroupID && flag2)
			{
				return true;
			}
			break;
		}
		case DropContent.RandomMoldBox:
			if (itemDropInfo.RewardType == NKM_REWARD_TYPE.RT_EQUIP)
			{
				if (NKMItemManager.GetEquipTemplet(itemDropInfo.ItemID) != null && NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
				{
					return true;
				}
			}
			else if (NKMItemMiscTemplet.Find(itemDropInfo.ItemID) != null && NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
			{
				return true;
			}
			break;
		case DropContent.UnitDismiss:
			if (NKMItemMiscTemplet.Find(itemDropInfo.ItemID) != null)
			{
				return true;
			}
			break;
		case DropContent.UnitExtract:
			if (NKMItemMiscTemplet.Find(itemDropInfo.ItemID) != null && NKCContentManager.IsContentsUnlocked(ContentsType.EXTRACT))
			{
				return true;
			}
			break;
		case DropContent.Trim:
			if (itemDropInfo.RewardType == NKM_REWARD_TYPE.RT_EQUIP)
			{
				if (NKMItemManager.GetEquipTemplet(itemDropInfo.ItemID) != null && NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
				{
					return true;
				}
			}
			else if (NKMItemMiscTemplet.Find(itemDropInfo.ItemID) != null && NKCContentManager.IsContentsUnlocked(ContentsType.DIMENSION_TRIM) && NKCUITrimUtility.OpenTagEnabled)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private int GetDropInfoCountLimit(DropContent dropContent)
	{
		return dropContent switch
		{
			DropContent.Shop => NKMCommonConst.DropInfoShopLimit, 
			DropContent.WorldMapMission => NKMCommonConst.DropInfoWorldMapMissionLimit, 
			DropContent.Raid => NKMCommonConst.DropInfoRaidLimit, 
			DropContent.Shadow => NKMCommonConst.DropInfoShadowPalace, 
			DropContent.Dive => NKMCommonConst.DropInfoDiveLimit, 
			DropContent.Fierce => NKMCommonConst.DropInfoFiercePointReward, 
			DropContent.RandomMoldBox => NKMCommonConst.DropInfoRandomMoldBox, 
			DropContent.UnitDismiss => NKMCommonConst.DropInfoUnitDismiss, 
			DropContent.UnitExtract => NKMCommonConst.DropInfoUnitExtract, 
			DropContent.Trim => NKMCommonConst.DropInfoTrimDungeon, 
			DropContent.SubStreamShop => NKMCommonConst.DropInfoSubStreamShop, 
			_ => 0, 
		};
	}

	private int GetStageDropInfoCountLimit(EPISODE_CATEGORY episodeCategory)
	{
		switch (episodeCategory)
		{
		case EPISODE_CATEGORY.EC_MAINSTREAM:
			return NKMCommonConst.DropInfoMainStreamLimit;
		case EPISODE_CATEGORY.EC_SUPPLY:
			return NKMCommonConst.DropInfoSupplyLimit;
		case EPISODE_CATEGORY.EC_DAILY:
			return NKMCommonConst.DropInfoDailyLimit;
		case EPISODE_CATEGORY.EC_SIDESTORY:
			return NKMCommonConst.DropInfoSideStoryLimit;
		case EPISODE_CATEGORY.EC_CHALLENGE:
			return NKMCommonConst.DropInfoChallengeLimit;
		case EPISODE_CATEGORY.EC_COUNTERCASE:
			return NKMCommonConst.DropInfoCounterCase;
		case EPISODE_CATEGORY.EC_FIELD:
			return NKMCommonConst.DropInfoFieldLimit;
		case EPISODE_CATEGORY.EC_EVENT:
		case EPISODE_CATEGORY.EC_SEASONAL:
			return NKMCommonConst.DropInfoEventLimit;
		default:
			return 0;
		}
	}

	private RectTransform GetSlot(int index)
	{
		return NKCUIComItemDropInfoSlot.GetNewInstance(null)?.GetComponent<RectTransform>();
	}

	private void ReturnMissionSlot(Transform tr)
	{
		NKCUIComItemDropInfoSlot component = tr.GetComponent<NKCUIComItemDropInfoSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	private void ProvideMissionData(Transform tr, int index)
	{
		NKCUIComItemDropInfoSlot component = tr.GetComponent<NKCUIComItemDropInfoSlot>();
		if (!(component == null) && m_ItemDropInfoList != null && m_ItemDropInfoList.Count > index)
		{
			component.SetData(m_ItemDropInfoList[index]);
		}
	}

	private void OnDestroy()
	{
		m_ItemDropInfoList?.Clear();
		m_ItemDropInfoList = null;
	}
}
