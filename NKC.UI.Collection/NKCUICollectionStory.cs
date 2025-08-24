using System;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionStory : MonoBehaviour
{
	public class EpData
	{
		public NKCCollectionManager.COLLECTION_STORY_CATEGORY m_Category;

		public int m_EpisodeID;

		public int m_SortIndex;

		public string m_EpisodeTitle;

		public string m_EpisodeName;

		public List<EpSlotData> m_lstEpisodeStages = new List<EpSlotData>();

		public EpData(EPISODE_CATEGORY category, int epID, string epTitle, string epName, int sortIndex)
		{
			m_Category = NKCCollectionManager.GetCollectionStoryCategory(category);
			m_EpisodeID = epID;
			m_EpisodeTitle = epTitle;
			m_EpisodeName = epName;
			m_SortIndex = sortIndex;
		}

		public EpData(NKMDiveTemplet templet)
		{
			m_Category = NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP;
			m_EpisodeID = -1;
			m_EpisodeTitle = "SI_DP_DIVE";
			m_EpisodeName = "SI_DP_DIVE";
			m_SortIndex = 0;
		}

		public EpData(string EpisodeTitleKey, string EpisodeNameKey)
		{
			m_Category = NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC;
			m_EpisodeID = -2;
			m_EpisodeTitle = EpisodeTitleKey;
			m_EpisodeName = EpisodeNameKey;
			m_SortIndex = 0;
		}

		public EpData(string EpisodeTitleKey, string EpisodeNameKey, int EpisodeID, int sortIndex = 0)
		{
			m_Category = NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC;
			m_EpisodeID = EpisodeID;
			m_EpisodeTitle = EpisodeTitleKey;
			m_EpisodeName = EpisodeNameKey;
			m_SortIndex = sortIndex;
		}
	}

	public struct EpSlotData
	{
		public List<UnlockInfo> m_UnlockReqList { get; private set; }

		public int m_MissionIndex { get; private set; }

		public int m_ActID { get; private set; }

		public string m_CutSceneStrIDBefore { get; private set; }

		public string m_CutSceneStrIDAfter { get; private set; }

		public bool m_bShowUnlockedOnly { get; private set; }

		public EpSlotData(List<UnlockInfo> unlockReqList, int actID, int missionID, string idBefore, string idAfter, bool bShowUnlockedOnly = false)
		{
			m_UnlockReqList = unlockReqList;
			m_MissionIndex = missionID;
			m_ActID = actID;
			m_CutSceneStrIDBefore = idBefore;
			m_CutSceneStrIDAfter = idAfter;
			m_bShowUnlockedOnly = bShowUnlockedOnly;
		}
	}

	public class StoryData
	{
		public int m_ActID;

		public int m_MissionIdx;

		public bool m_bClear;

		public UnlockInfo m_UnlockInfo;

		public string m_strBeforeCutScene;

		public string m_strAfterCutScene;

		public bool m_bShowUnlockedOnly;

		public StoryData(int actID, int MissionIdx, UnlockInfo unlockInfo, bool bClear, string strBeforeCutScene = "", string strAfterCutScene = "", bool bShowUnlockedOnly = false)
		{
			m_ActID = actID;
			m_MissionIdx = MissionIdx;
			m_UnlockInfo = unlockInfo;
			m_bClear = bClear;
			m_strBeforeCutScene = strBeforeCutScene;
			m_strAfterCutScene = strAfterCutScene;
			m_bShowUnlockedOnly = bShowUnlockedOnly;
		}
	}

	public class StorySlotData
	{
		public string m_EpisodeTitle;

		public string m_EpisodeName;

		public int m_iEpisodeClearCnt;

		public NKCCollectionManager.COLLECTION_STORY_CATEGORY m_eCategory;

		public List<StoryData> m_lstStoryData = new List<StoryData>();

		public StorySlotData(string Title, string Name, NKCCollectionManager.COLLECTION_STORY_CATEGORY type)
		{
			m_EpisodeTitle = Title;
			m_EpisodeName = Name;
			m_eCategory = type;
		}
	}

	public delegate void OnPlayCutScene(string name, int stageID);

	[Header("탭 버튼")]
	public NKCUIComStateButton m_NKM_UI_COLLECTION_STORY_TOP_MENU_MAINSTREAM;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_STORY_TOP_MENU_SIDESTORY;

	[Header("왼쪽(메인스트림,외전) 메뉴")]
	public RectTransform m_rtNKM_UI_COLLECTION_STORY_LEFTMENU_contents;

	public NKCUIComToggleGroup m_SideMenuToggleGroup;

	[Header("슬롯")]
	public RectTransform m_rtNKM_UI_COLLECTION_STORY_LIST_Contents;

	public NKCSideMenuSlot m_pfbSideSlot;

	public NKCSideMenuSlotChild m_pfbSideSlotChild;

	public NKCUISkinSlot m_pbfNKM_UI_COLLECTION_UNIT_INFO_UNIT_SKIN_STORY_SLOT;

	public LoopScrollRect m_LoopScrollRect;

	public RectTransform m_rtSkinSlotPool;

	[Header("달성률")]
	public NKCUICollectionRate m_CollectionRate;

	[Space]
	public GameObject m_NKM_UI_COLLECTION_SKIN_STORY_LIST;

	public RectTransform m_rtNKM_UI_COLLECTION_SKIN_STORY_LIST_Contents;

	public ScrollRect m_srStory;

	public NKCUICollectionStorySlot m_pfStorySlot;

	public RectTransform m_rtStorySlotPool;

	private Stack<RectTransform> m_stkStorySlotPool = new Stack<RectTransform>();

	public NKCUICollectionStorySubTitle m_pfSubTitle;

	public RectTransform m_rtSubTitlePool;

	private Stack<RectTransform> m_stkStorySubTitlePool = new Stack<RectTransform>();

	public NKCUICollectionStoryContent m_pfStoryContent;

	public RectTransform m_rtStoryContentPool;

	private Stack<RectTransform> m_stkStoryContentPool = new Stack<RectTransform>();

	private NKCUICollection.OnSyncCollectingData dOnSyncCollectingData;

	private NKCUICollection.OnStoryCutscen dOnStoryCutscen;

	private Dictionary<NKCCollectionManager.COLLECTION_STORY_CATEGORY, List<EpData>> m_dicCollectionEpisodeData = new Dictionary<NKCCollectionManager.COLLECTION_STORY_CATEGORY, List<EpData>>();

	private Dictionary<EPISODE_CATEGORY, EpData> m_dicEpData = new Dictionary<EPISODE_CATEGORY, EpData>();

	private Dictionary<int, StorySlotData> m_dicStory = new Dictionary<int, StorySlotData>();

	public List<StorySlotData> m_lstStorySlotData = new List<StorySlotData>();

	private int m_iReservePageIdx;

	private int m_iSelectEpsodeIdx;

	private List<NKCSideMenuSlot> m_lstSideMenuSlot = new List<NKCSideMenuSlot>();

	private NKCUICollectionStoryContent StroyContent;

	private List<NKCUICollectionStoryContent> m_lstStorySlot = new List<NKCUICollectionStoryContent>();

	private int m_iCollected;

	private int m_iTotalCollected;

	private List<NKCUISkinSlot> m_lstSkinSlot = new List<NKCUISkinSlot>();

	private List<storyUnlockData> m_lstSkinStoryList = new List<storyUnlockData>();

	private Stack<NKCUISkinSlot> m_stkSkinSlotPool = new Stack<NKCUISkinSlot>();

	private NKCUICollectionStoryContent m_StoryContent
	{
		get
		{
			if (StroyContent == null)
			{
				NKCUICollectionStoryContent nKCUICollectionStoryContent = UnityEngine.Object.Instantiate(m_pfStoryContent);
				nKCUICollectionStoryContent.Init();
				nKCUICollectionStoryContent.transform.localPosition = Vector3.zero;
				nKCUICollectionStoryContent.transform.localScale = Vector3.one;
				nKCUICollectionStoryContent.GetComponent<RectTransform>().SetParent(m_rtNKM_UI_COLLECTION_STORY_LIST_Contents, worldPositionStays: false);
				StroyContent = nKCUICollectionStoryContent;
			}
			return StroyContent;
		}
	}

	public void Init(NKCUICollection.OnSyncCollectingData callBack, NKCUICollection.OnStoryCutscen cutscenCallBack)
	{
		if (callBack != null)
		{
			dOnSyncCollectingData = callBack;
		}
		if (cutscenCallBack != null)
		{
			dOnStoryCutscen = cutscenCallBack;
		}
		if (null != m_LoopScrollRect)
		{
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnSlot;
			m_LoopScrollRect.dOnProvideData += ProvideSlotData;
			m_LoopScrollRect.PrepareCells();
		}
		NKCUtil.SetScrollHotKey(m_srStory);
		InitEpisodeData();
	}

	public static bool IsVaildCollectionStory(NKMEpisodeTempletV2 epTemplet)
	{
		if (epTemplet == null)
		{
			return false;
		}
		if (epTemplet.m_bNoCollectionCutscene)
		{
			return false;
		}
		if (!epTemplet.CollectionEnableByTag)
		{
			return false;
		}
		if (epTemplet.m_EPCategory == EPISODE_CATEGORY.EC_MAINSTREAM || epTemplet.m_EPCategory == EPISODE_CATEGORY.EC_SIDESTORY || epTemplet.m_EPCategory == EPISODE_CATEGORY.EC_EVENT || epTemplet.m_EPCategory == EPISODE_CATEGORY.EC_SEASONAL)
		{
			return true;
		}
		return false;
	}

	public static bool IsValidCollectionStory(NKMDiveTemplet diveTemplet)
	{
		if (diveTemplet == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(diveTemplet.CutsceneDiveStart) && string.IsNullOrEmpty(diveTemplet.CutsceneDiveEnter) && string.IsNullOrEmpty(diveTemplet.CutsceneDiveBossBefore) && string.IsNullOrEmpty(diveTemplet.CutsceneDiveBossAfter))
		{
			return false;
		}
		return true;
	}

	private void InitEpisodeData()
	{
		Dictionary<int, storyUnlockData> storyData = NKCCollectionManager.GetStoryData();
		Dictionary<int, List<int>> epiSodeStageIdData = NKCCollectionManager.GetEpiSodeStageIdData();
		foreach (NKMEpisodeTempletV2 epTemplet in NKMEpisodeTempletV2.Values)
		{
			if (!IsVaildCollectionStory(epTemplet) || !epiSodeStageIdData.ContainsKey(epTemplet.m_EpisodeID))
			{
				continue;
			}
			if (!storyData.ContainsKey(epiSodeStageIdData[epTemplet.m_EpisodeID][0]))
			{
				Debug.LogError($"CutScene Templet does'nt contain stage ID: {epiSodeStageIdData[epTemplet.m_EpisodeID][0]}");
				continue;
			}
			EPISODE_CATEGORY episodeCategory = storyData[epiSodeStageIdData[epTemplet.m_EpisodeID][0]].m_EpisodeCategory;
			if (m_dicCollectionEpisodeData.ContainsKey(NKCCollectionManager.GetCollectionStoryCategory(episodeCategory)) && m_dicCollectionEpisodeData[NKCCollectionManager.GetCollectionStoryCategory(episodeCategory)].Find((EpData x) => x.m_EpisodeID == epTemplet.m_EpisodeID) != null)
			{
				continue;
			}
			int sortIndex = ((epTemplet != null) ? epTemplet.m_SortIndex : 0);
			EpData epData = new EpData(episodeCategory, epTemplet.m_EpisodeID, epTemplet.m_EpisodeTitle, epTemplet.m_EpisodeName, sortIndex);
			int count = epiSodeStageIdData[epTemplet.m_EpisodeID].Count;
			for (int num = 0; num < count; num++)
			{
				int key = epiSodeStageIdData[epTemplet.m_EpisodeID][num];
				NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(key);
				if (nKMStageTempletV == null)
				{
					continue;
				}
				string idBefore = "";
				string idAfter = "";
				if (nKMStageTempletV.m_STAGE_TYPE == STAGE_TYPE.ST_PHASE)
				{
					NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(nKMStageTempletV.m_StageBattleStrID);
					if (nKMPhaseTemplet != null)
					{
						idBefore = nKMPhaseTemplet.m_CutScenStrIDBefore;
						idAfter = nKMPhaseTemplet.m_CutScenStrIDAfter;
					}
				}
				else if (nKMStageTempletV.DungeonTempletBase != null)
				{
					idBefore = nKMStageTempletV.DungeonTempletBase.m_CutScenStrIDBefore;
					idAfter = nKMStageTempletV.DungeonTempletBase.m_CutScenStrIDAfter;
				}
				else if (nKMStageTempletV.WarfareTemplet != null)
				{
					idBefore = nKMStageTempletV.WarfareTemplet.m_CutScenStrIDBefore;
					idAfter = nKMStageTempletV.WarfareTemplet.m_CutScenStrIDAfter;
				}
				epData.m_lstEpisodeStages.Add(new EpSlotData(storyData[nKMStageTempletV.Key].m_UnlockReqList, storyData[key].m_ActID, nKMStageTempletV.m_StageUINum, idBefore, idAfter));
			}
			if (m_dicCollectionEpisodeData.ContainsKey(NKCCollectionManager.GetCollectionStoryCategory(episodeCategory)))
			{
				m_dicCollectionEpisodeData[NKCCollectionManager.GetCollectionStoryCategory(episodeCategory)].Add(epData);
				continue;
			}
			List<EpData> list = new List<EpData>();
			list.Add(epData);
			m_dicCollectionEpisodeData.Add(NKCCollectionManager.GetCollectionStoryCategory(episodeCategory), list);
		}
		EpData epData2 = new EpData(new NKMDiveTemplet());
		int num2 = 0;
		foreach (NKMDiveTemplet value2 in NKMTempletContainer<NKMDiveTemplet>.Values)
		{
			if (IsValidCollectionStory(value2) && storyData.ContainsKey(value2.StageID))
			{
				storyUnlockData storyUnlockData = storyData[value2.StageID];
				if (!string.IsNullOrEmpty(value2.CutsceneDiveEnter))
				{
					List<UnlockInfo> unlockReqList = storyUnlockData.m_UnlockReqList;
					epData2.m_lstEpisodeStages.Add(new EpSlotData(unlockReqList, num2, 1, value2.CutsceneDiveEnter, ""));
				}
				if (!string.IsNullOrEmpty(value2.CutsceneDiveStart))
				{
					List<UnlockInfo> unlockReqList2 = storyUnlockData.m_UnlockReqList;
					epData2.m_lstEpisodeStages.Add(new EpSlotData(unlockReqList2, num2, 2, value2.CutsceneDiveStart, ""));
				}
				if (!string.IsNullOrEmpty(value2.CutsceneDiveBossBefore))
				{
					List<UnlockInfo> unlockReqList3 = storyUnlockData.m_UnlockReqList;
					epData2.m_lstEpisodeStages.Add(new EpSlotData(unlockReqList3, num2, 3, value2.CutsceneDiveBossBefore, ""));
				}
				if (!string.IsNullOrEmpty(value2.CutsceneDiveBossAfter))
				{
					List<UnlockInfo> unlockReqList4 = storyUnlockData.m_UnlockReqList;
					epData2.m_lstEpisodeStages.Add(new EpSlotData(unlockReqList4, num2, 4, value2.CutsceneDiveBossAfter, ""));
				}
				num2++;
			}
		}
		if (epData2.m_lstEpisodeStages.Count > 0)
		{
			List<EpData> list2 = new List<EpData>();
			list2.Add(epData2);
			if (!m_dicCollectionEpisodeData.ContainsKey(NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP))
			{
				m_dicCollectionEpisodeData.Add(NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP, list2);
			}
			else
			{
				m_dicCollectionEpisodeData[NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP].Add(epData2);
			}
		}
		if (storyData.ContainsKey(4001))
		{
			int num3 = 0;
			if (storyData[4001].m_UnlockReqList.Count > 0)
			{
				UnlockInfo item = new UnlockInfo(storyData[4001].m_UnlockReqList[0].eReqType, storyData[4001].m_UnlockReqList[0].reqValue, storyData[4001].m_UnlockReqList[0].reqValueStr);
				List<EpSlotData> list3 = new List<EpSlotData>();
				string episodeNameKey = "";
				foreach (NKMTrimTemplet value3 in NKMTrimTemplet.Values)
				{
					foreach (List<NKMTrimDungeonTemplet> value4 in value3.TrimDungeonTemplets.Values)
					{
						foreach (NKMTrimDungeonTemplet item3 in value4)
						{
							if (item3.ShowCutScene && item3.DungeonTempletBase != null)
							{
								episodeNameKey = value3.TirmGroupName;
								string cutScenStrIDBefore = item3.DungeonTempletBase.m_CutScenStrIDBefore;
								string cutScenStrIDAfter = item3.DungeonTempletBase.m_CutScenStrIDAfter;
								list3.Add(new EpSlotData(new List<UnlockInfo> { item }, 0, ++num3, cutScenStrIDBefore, cutScenStrIDAfter));
							}
						}
					}
				}
				if (list3.Count > 0)
				{
					EpData epData3 = new EpData("SI_EPISODE_EPISODE_TRIM", episodeNameKey);
					epData3.m_lstEpisodeStages = list3;
					if (m_dicCollectionEpisodeData.ContainsKey(NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC))
					{
						m_dicCollectionEpisodeData[NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC].Add(epData3);
					}
					else
					{
						m_dicCollectionEpisodeData.Add(NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC, new List<EpData> { epData3 });
					}
				}
			}
		}
		if (storyData.ContainsKey(-3))
		{
			return;
		}
		string text = string.Empty;
		string text2 = string.Empty;
		int num4 = -3;
		if (!NKCCollectionManager.GetCollectionCutsceneData().TryGetValue(num4, out var value))
		{
			return;
		}
		List<EpSlotData> list4 = new List<EpSlotData>();
		for (int num5 = 0; num5 < value.Count; num5++)
		{
			if (!NKMOpenTagManager.IsOpened(value[num5].OpenTag))
			{
				continue;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = value[num5].m_SubTabName;
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = value[num5].m_SubIndexName;
			}
			if (value[num5].UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_HISTORY_BIRTHDAY)
			{
				NKCLoginCutSceneTemplet loginCutSceneTemplet = NKCLoginCutSceneManager.GetLoginCutSceneTemplet(NKCCollectionManager.GetEventUnlockCond(value[num5].UnlockInfo.eReqType), value[num5].UnlockInfo.reqValue.ToString());
				if (loginCutSceneTemplet != null)
				{
					EpSlotData item2 = new EpSlotData(new List<UnlockInfo> { value[num5].UnlockInfo }, value[num5].m_SubIndex, num5, loginCutSceneTemplet.m_CutSceneStrID, "", !value[num5].m_bExclude);
					list4.Add(item2);
				}
			}
		}
		if (list4.Count > 0)
		{
			EpData epData4 = new EpData(text, text2, num4, num4);
			epData4.m_lstEpisodeStages = list4;
			if (m_dicCollectionEpisodeData.ContainsKey(NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC))
			{
				m_dicCollectionEpisodeData[NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC].Add(epData4);
				return;
			}
			m_dicCollectionEpisodeData.Add(NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC, new List<EpData> { epData4 });
		}
	}

	public void Open()
	{
		if (m_dicStory.Count <= 0)
		{
			m_iCollected = 0;
			m_iTotalCollected = 0;
			foreach (NKCCollectionManager.COLLECTION_STORY_CATEGORY value in Enum.GetValues(typeof(NKCCollectionManager.COLLECTION_STORY_CATEGORY)))
			{
				int iReservePageIdx = UpdateEpisodeCategory(ref m_iCollected, ref m_iTotalCollected, value, m_dicCollectionEpisodeData, getStoryData: true, ref m_dicStory);
				if (m_iReservePageIdx == 0)
				{
					m_iReservePageIdx = iReservePageIdx;
				}
			}
			CreatSideMenu();
			OnClicked(m_iReservePageIdx.ToString());
			foreach (NKCSideMenuSlot item in m_lstSideMenuSlot)
			{
				if (item.HasChild(m_iReservePageIdx.ToString()))
				{
					item.ForceSelect(select: true);
					item.OnValueChange(bVal: true);
				}
			}
		}
		SyncCollectionData();
	}

	public void Clear()
	{
		m_iCollected = 0;
		m_iTotalCollected = 0;
		m_lstStorySlot.Clear();
		m_iSelectEpsodeIdx = 0;
		m_dicStory.Clear();
	}

	private void CreatSideMenu()
	{
		m_lstSideMenuSlot.Clear();
		CreateSideMenu(NKCCollectionManager.COLLECTION_STORY_CATEGORY.MAINSTREAM);
		CreateSideMenu(NKCCollectionManager.COLLECTION_STORY_CATEGORY.SIDESTORY);
		CreateSideMenu(NKCCollectionManager.COLLECTION_STORY_CATEGORY.SEASONAL);
		CreateSideMenu(NKCCollectionManager.COLLECTION_STORY_CATEGORY.EVENT);
		CreateSideMenu(NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP);
		CreateSideMenu(NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC);
		CreateExceptionSideMenu();
	}

	private void CreateSideMenu(NKCCollectionManager.COLLECTION_STORY_CATEGORY category)
	{
		if (!m_dicCollectionEpisodeData.ContainsKey(category))
		{
			return;
		}
		NKCSideMenuSlot nKCSideMenuSlot = CreatSideMenuSlot(category);
		if (nKCSideMenuSlot == null)
		{
			return;
		}
		List<EpData> list = m_dicCollectionEpisodeData[category];
		if (category == NKCCollectionManager.COLLECTION_STORY_CATEGORY.SIDESTORY && list.Count > 0 && list[0].m_SortIndex > 0)
		{
			list.Sort(delegate(EpData e1, EpData e2)
			{
				if (e1.m_SortIndex > e2.m_SortIndex)
				{
					return 1;
				}
				return (e1.m_SortIndex < e2.m_SortIndex) ? (-1) : 0;
			});
		}
		int num = 0;
		int num2 = 0;
		bool clear = true;
		for (int num3 = 0; num3 < list.Count; num3++)
		{
			if (!m_dicStory.ContainsKey(list[num3].m_EpisodeID))
			{
				Debug.Log($"fail - can not found episode id : {list[num3].m_EpisodeID}");
				continue;
			}
			if (m_dicStory[list[num3].m_EpisodeID].m_iEpisodeClearCnt < m_dicStory[list[num3].m_EpisodeID].m_lstStoryData.Count)
			{
				clear = false;
			}
			NKCSideMenuSlotChild nKCSideMenuSlotChild = CreateSideMenuSlotchild(m_dicStory[list[num3].m_EpisodeID].m_EpisodeName, list[num3].m_EpisodeID.ToString());
			if (nKCSideMenuSlotChild == null)
			{
				Debug.LogWarning("fail - create side menu child : " + m_dicStory[list[num3].m_EpisodeID].m_EpisodeTitle);
				continue;
			}
			if (m_dicStory[list[num3].m_EpisodeID].m_iEpisodeClearCnt <= 0)
			{
				nKCSideMenuSlotChild.m_Toggle.Lock();
			}
			else
			{
				nKCSideMenuSlotChild.m_Toggle.UnLock();
			}
			nKCSideMenuSlotChild.SetProgressState(m_dicStory[list[num3].m_EpisodeID].m_lstStoryData.Count, m_dicStory[list[num3].m_EpisodeID].m_iEpisodeClearCnt);
			if (category == NKCCollectionManager.COLLECTION_STORY_CATEGORY.EVENT)
			{
				num += m_dicStory[list[num3].m_EpisodeID].m_iEpisodeClearCnt;
			}
			else
			{
				num2 += m_dicStory[list[num3].m_EpisodeID].m_iEpisodeClearCnt;
			}
			nKCSideMenuSlot.AddSubSlot(nKCSideMenuSlotChild);
		}
		nKCSideMenuSlot.SetClear(clear);
		switch (category)
		{
		case NKCCollectionManager.COLLECTION_STORY_CATEGORY.SIDESTORY:
		case NKCCollectionManager.COLLECTION_STORY_CATEGORY.SEASONAL:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.SIDESTORY))
			{
				nKCSideMenuSlot.Lock();
			}
			break;
		case NKCCollectionManager.COLLECTION_STORY_CATEGORY.EVENT:
			NKMEpisodeMgr.GetListNKMEpisodeTempletByCategory(GetEpisodeCategory(category), bOnlyOpen: true);
			if (num > 0)
			{
				nKCSideMenuSlot.UnLock();
			}
			else
			{
				nKCSideMenuSlot.Lock();
			}
			break;
		case NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.DIVE))
			{
				nKCSideMenuSlot.Lock();
			}
			break;
		case NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.DIMENSION_TRIM) && num2 <= 0)
			{
				nKCSideMenuSlot.Lock();
			}
			break;
		}
		m_lstSideMenuSlot.Add(nKCSideMenuSlot);
	}

	private EPISODE_CATEGORY GetEpisodeCategory(NKCCollectionManager.COLLECTION_STORY_CATEGORY category)
	{
		return category switch
		{
			NKCCollectionManager.COLLECTION_STORY_CATEGORY.MAINSTREAM => EPISODE_CATEGORY.EC_MAINSTREAM, 
			NKCCollectionManager.COLLECTION_STORY_CATEGORY.SIDESTORY => EPISODE_CATEGORY.EC_SIDESTORY, 
			_ => EPISODE_CATEGORY.EC_EVENT, 
		};
	}

	private NKCSideMenuSlot CreatSideMenuSlot(NKCCollectionManager.COLLECTION_STORY_CATEGORY category)
	{
		return CreatSideMenuSlot(NKCUtilString.GetCollectionStoryCategory(category));
	}

	private NKCSideMenuSlot CreatSideMenuSlot(string title)
	{
		NKCSideMenuSlot nKCSideMenuSlot = UnityEngine.Object.Instantiate(m_pfbSideSlot);
		NKCUtil.SetGameobjectActive(nKCSideMenuSlot, bValue: true);
		nKCSideMenuSlot.Init(title, m_SideMenuToggleGroup, m_rtNKM_UI_COLLECTION_STORY_LEFTMENU_contents);
		nKCSideMenuSlot.transform.localScale = Vector3.one;
		return nKCSideMenuSlot;
	}

	private NKCSideMenuSlotChild CreateSideMenuSlotchild(string title, string key)
	{
		NKCSideMenuSlotChild nKCSideMenuSlotChild = UnityEngine.Object.Instantiate(m_pfbSideSlotChild);
		NKCUtil.SetGameobjectActive(nKCSideMenuSlotChild, bValue: true);
		nKCSideMenuSlotChild.Init(title, key, m_rtNKM_UI_COLLECTION_STORY_LEFTMENU_contents, OnClicked);
		nKCSideMenuSlotChild.transform.localScale = Vector3.one;
		return nKCSideMenuSlotChild;
	}

	private void OnSkinClick(int skinID)
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SKIN_STORY_REPLAY_CONFIRM, delegate
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
			if (skinTemplet != null && !string.IsNullOrEmpty(skinTemplet.m_CutscenePurchase))
			{
				PlayCutScene(skinTemplet.m_CutscenePurchase, 0);
			}
		});
	}

	public void OnClicked(string key)
	{
		int num = int.Parse(key);
		if (!m_dicStory.ContainsKey(num))
		{
			Debug.Log("식별할 수 없는 에피소드 id : " + key);
		}
		else
		{
			if (m_iSelectEpsodeIdx == num)
			{
				return;
			}
			m_iSelectEpsodeIdx = num;
			foreach (NKCSideMenuSlot item in m_lstSideMenuSlot)
			{
				item.NotifySelectID(m_iSelectEpsodeIdx.ToString());
			}
			UpdateEpisodeSlot();
		}
	}

	public static int UpdateEpisodeCategory(ref int collectedCount, ref int totalCollectedCount, NKCCollectionManager.COLLECTION_STORY_CATEGORY type, Dictionary<NKCCollectionManager.COLLECTION_STORY_CATEGORY, List<EpData>> dicCollectionEpisodeData, bool getStoryData, ref Dictionary<int, StorySlotData> dicStory)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return 0;
		}
		if (dicCollectionEpisodeData == null)
		{
			return 0;
		}
		if (!dicCollectionEpisodeData.ContainsKey(type))
		{
			return 0;
		}
		int num = 0;
		List<EpData> list = dicCollectionEpisodeData[type];
		bool flag = false;
		if (nKMUserData.IsSuperUser())
		{
			flag = true;
		}
		if (list != null)
		{
			foreach (EpData item2 in list)
			{
				StorySlotData storySlotData = null;
				if (getStoryData)
				{
					storySlotData = new StorySlotData(NKCStringTable.GetString(item2.m_EpisodeTitle), NKCStringTable.GetString(item2.m_EpisodeName), type);
				}
				List<EpSlotData> lstEpisodeStages = item2.m_lstEpisodeStages;
				for (int i = 0; i < lstEpisodeStages.Count; i++)
				{
					bool flag2 = false;
					int count = lstEpisodeStages[i].m_UnlockReqList.Count;
					for (int j = 0; j < count; j++)
					{
						flag2 |= NKMContentUnlockManager.IsContentUnlocked(nKMUserData, lstEpisodeStages[i].m_UnlockReqList[j]);
					}
					if (flag)
					{
						flag2 = true;
					}
					bool flag3 = false;
					bool flag4 = false;
					for (int k = 0; k < count; k++)
					{
						if (lstEpisodeStages[i].m_UnlockReqList[k].eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_STAGE)
						{
							NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(lstEpisodeStages[i].m_UnlockReqList[k].reqValue);
							if (nKMStageTempletV != null)
							{
								flag3 |= nKMStageTempletV.GetStageBeforeCutscen() != null;
								flag4 |= nKMStageTempletV.GetStageAfterCutscen() != null;
							}
						}
						else if (lstEpisodeStages[i].m_UnlockReqList[k].eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON)
						{
							NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(lstEpisodeStages[i].m_UnlockReqList[k].reqValue);
							if (dungeonTempletBase != null)
							{
								flag3 |= !string.IsNullOrEmpty(dungeonTempletBase.m_CutScenStrIDBefore);
								flag4 |= !string.IsNullOrEmpty(dungeonTempletBase.m_CutScenStrIDAfter);
							}
						}
						else if (lstEpisodeStages[i].m_UnlockReqList[k].eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE)
						{
							NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(lstEpisodeStages[i].m_UnlockReqList[k].reqValue);
							if (nKMPhaseTemplet != null)
							{
								flag3 |= !string.IsNullOrEmpty(nKMPhaseTemplet.m_CutScenStrIDBefore);
								flag4 |= !string.IsNullOrEmpty(nKMPhaseTemplet.m_CutScenStrIDAfter);
							}
						}
						else if (lstEpisodeStages[i].m_UnlockReqList[k].eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE)
						{
							NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(lstEpisodeStages[i].m_UnlockReqList[k].reqValue);
							if (nKMWarfareTemplet != null)
							{
								flag3 |= !string.IsNullOrEmpty(nKMWarfareTemplet.m_CutScenStrIDBefore);
								flag4 |= !string.IsNullOrEmpty(nKMWarfareTemplet.m_CutScenStrIDAfter);
							}
						}
						else if (lstEpisodeStages[i].m_UnlockReqList[k].eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_DIVE_HISTORY_CLEARED)
						{
							NKMDiveTemplet nKMDiveTemplet = NKMDiveTemplet.Find(lstEpisodeStages[i].m_UnlockReqList[k].reqValue);
							flag3 = flag3 || nKMDiveTemplet != null;
						}
						else if (lstEpisodeStages[i].m_UnlockReqList[k].eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_TRIM)
						{
							flag3 |= !string.IsNullOrEmpty(lstEpisodeStages[i].m_CutSceneStrIDBefore);
							flag4 |= !string.IsNullOrEmpty(lstEpisodeStages[i].m_CutSceneStrIDAfter);
						}
						else if (lstEpisodeStages[i].m_UnlockReqList[k].eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_HISTORY_BIRTHDAY)
						{
							if (NKCScenManager.CurrentUserData().m_BirthDayData != null)
							{
								flag3 |= !string.IsNullOrEmpty(lstEpisodeStages[i].m_CutSceneStrIDBefore);
							}
						}
						else
						{
							Debug.LogError($"NKCUICollectionStory::UpdateEpisodeCategory - Can not define reqType - {lstEpisodeStages[i].m_UnlockReqList[k].eReqType}");
						}
					}
					StoryData item = new StoryData(lstEpisodeStages[i].m_ActID, lstEpisodeStages[i].m_MissionIndex, lstEpisodeStages[i].m_UnlockReqList[0], flag2, lstEpisodeStages[i].m_CutSceneStrIDBefore, lstEpisodeStages[i].m_CutSceneStrIDAfter, lstEpisodeStages[i].m_bShowUnlockedOnly);
					if (flag2)
					{
						if (num == 0)
						{
							num = item2.m_EpisodeID;
						}
						if (flag3)
						{
							collectedCount++;
						}
						if (flag4)
						{
							collectedCount++;
						}
						if (getStoryData)
						{
							storySlotData.m_iEpisodeClearCnt++;
						}
					}
					storySlotData.m_lstStoryData.Add(item);
					if (flag3)
					{
						totalCollectedCount++;
					}
					if (flag4)
					{
						totalCollectedCount++;
					}
				}
				if (getStoryData)
				{
					dicStory.Add(item2.m_EpisodeID, storySlotData);
				}
			}
		}
		else
		{
			Debug.LogError("Can not Found Story Data - " + type);
		}
		return num;
	}

	private void UpdateEpisodeSlot()
	{
		if (!m_dicStory.ContainsKey(m_iSelectEpsodeIdx) || m_dicStory[m_iSelectEpsodeIdx].m_iEpisodeClearCnt <= 0)
		{
			Debug.LogWarning("[ERROR] Collection story data is wrong : " + m_iSelectEpsodeIdx);
			return;
		}
		OnActiveSkinStory(bActive: false);
		ClearExceptionSlot();
		ClearEpisodeSlot();
		List<RectTransform> uISlot = GetUISlot(m_dicStory[m_iSelectEpsodeIdx].m_lstStoryData.Count);
		HashSet<int> hashSet = new HashSet<int>();
		foreach (StoryData lstStoryDatum in m_dicStory[m_iSelectEpsodeIdx].m_lstStoryData)
		{
			hashSet.Add(lstStoryDatum.m_ActID);
		}
		List<RectTransform> titleUISlot = GetTitleUISlot(hashSet.Count);
		m_StoryContent.SetData(m_dicStory[m_iSelectEpsodeIdx], uISlot, titleUISlot);
	}

	private void ClearEpisodeSlot()
	{
		List<RectTransform> rentalList = m_StoryContent.GetRentalList();
		for (int i = 0; i < rentalList.Count; i++)
		{
			rentalList[i].SetParent(m_rtStorySlotPool);
			NKCUtil.SetGameobjectActive(rentalList[i].gameObject, bValue: false);
			m_stkStorySlotPool.Push(rentalList[i]);
		}
		List<RectTransform> subRentalList = m_StoryContent.GetSubRentalList();
		for (int j = 0; j < subRentalList.Count; j++)
		{
			subRentalList[j].SetParent(m_rtSubTitlePool);
			NKCUtil.SetGameobjectActive(subRentalList[j].gameObject, bValue: false);
			m_stkStorySubTitlePool.Push(subRentalList[j]);
		}
		m_StoryContent.ClearRentalList();
	}

	public void PlayCutScene(string name, int stageID)
	{
		NKCUICutScenPlayer.Instance.LoadAndPlay(name, stageID, EndCutScene);
		dOnStoryCutscen(bPlay: true);
	}

	public void EndCutScene()
	{
		NKCSoundManager.StopAllSound();
		NKCSoundManager.PlayScenMusic(NKCScenManager.GetScenManager().GetNowScenID());
		dOnStoryCutscen(bPlay: false);
	}

	private List<RectTransform> GetUISlot(int iCnt)
	{
		List<RectTransform> list = new List<RectTransform>();
		for (int i = 0; i < iCnt; i++)
		{
			if (m_stkStorySlotPool.Count > 0)
			{
				RectTransform item = m_stkStorySlotPool.Pop();
				list.Add(item);
				continue;
			}
			NKCUICollectionStorySlot nKCUICollectionStorySlot = UnityEngine.Object.Instantiate(m_pfStorySlot);
			nKCUICollectionStorySlot.Init(PlayCutScene);
			nKCUICollectionStorySlot.transform.localPosition = Vector3.zero;
			nKCUICollectionStorySlot.transform.localScale = Vector3.one;
			RectTransform component = nKCUICollectionStorySlot.GetComponent<RectTransform>();
			list.Add(component);
		}
		return list;
	}

	private List<RectTransform> GetTitleUISlot(int iCnt)
	{
		List<RectTransform> list = new List<RectTransform>();
		for (int i = 0; i < iCnt; i++)
		{
			if (m_stkStorySubTitlePool.Count > 0)
			{
				RectTransform item = m_stkStorySubTitlePool.Pop();
				list.Add(item);
				continue;
			}
			NKCUICollectionStorySubTitle nKCUICollectionStorySubTitle = UnityEngine.Object.Instantiate(m_pfSubTitle);
			nKCUICollectionStorySubTitle.Init();
			nKCUICollectionStorySubTitle.transform.localPosition = Vector3.zero;
			nKCUICollectionStorySubTitle.transform.localScale = Vector3.one;
			RectTransform component = nKCUICollectionStorySubTitle.GetComponent<RectTransform>();
			list.Add(component);
		}
		return list;
	}

	public void PlayCutscen(bool bPlay)
	{
		if (dOnStoryCutscen != null)
		{
			dOnStoryCutscen(bPlay);
		}
	}

	private void SyncCollectionData()
	{
		if (dOnSyncCollectingData != null)
		{
			dOnSyncCollectingData(NKCUICollectionGeneral.CollectionType.CT_STORY, m_iCollected, m_iTotalCollected);
		}
		m_CollectionRate?.SetData(NKCUICollectionGeneral.CollectionType.CT_STORY, m_iCollected, m_iTotalCollected);
	}

	private void CreateExceptionSideMenu()
	{
		if (NKCCollectionManager.GetSkinStoryData().Count > 0)
		{
			NKCSideMenuSlot nKCSideMenuSlot = CreatSideMenuSlot(NKCUtilString.GET_STRING_COLLECTION_SKIN_SLOT_NAME);
			if (!(nKCSideMenuSlot == null))
			{
				nKCSideMenuSlot.SetCallBackFunction(UpdateSkinCutscenSlot);
				m_lstSideMenuSlot.Add(nKCSideMenuSlot);
			}
		}
	}

	private void OnActiveSkinStory(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_rtNKM_UI_COLLECTION_STORY_LIST_Contents.gameObject, !bActive);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_SKIN_STORY_LIST, bActive);
	}

	private void UpdateSkinCutscenSlot()
	{
		m_iSelectEpsodeIdx = 0;
		foreach (NKCSideMenuSlot item in m_lstSideMenuSlot)
		{
			item.NotifySelectID(m_iSelectEpsodeIdx.ToString());
		}
		OnActiveSkinStory(bActive: true);
		m_lstSkinStoryList = NKCCollectionManager.GetSkinStoryData();
		if (m_lstSkinStoryList != null && m_lstSkinStoryList.Count > 0)
		{
			m_LoopScrollRect.TotalCount = m_lstSkinStoryList.Count;
			m_LoopScrollRect.RefreshCells();
		}
	}

	private void ClearExceptionSlot()
	{
		foreach (NKCUISkinSlot item in m_lstSkinSlot)
		{
			item.GetComponent<RectTransform>().SetParent(m_rtSkinSlotPool);
		}
	}

	private RectTransform GetSlot(int index)
	{
		if (m_stkSkinSlotPool.Count > 0)
		{
			return m_stkSkinSlotPool.Pop().GetComponent<RectTransform>();
		}
		NKCUISkinSlot nKCUISkinSlot = UnityEngine.Object.Instantiate(m_pbfNKM_UI_COLLECTION_UNIT_INFO_UNIT_SKIN_STORY_SLOT);
		NKCUtil.SetGameobjectActive(nKCUISkinSlot, bValue: true);
		nKCUISkinSlot.transform.localScale = Vector3.one;
		nKCUISkinSlot.transform.SetParent(m_rtNKM_UI_COLLECTION_SKIN_STORY_LIST_Contents);
		return nKCUISkinSlot.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		go.SetParent(m_rtSkinSlotPool);
		NKCUISkinSlot component = go.GetComponent<NKCUISkinSlot>();
		if (component != null)
		{
			m_stkSkinSlotPool.Push(component);
		}
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		if (m_lstSkinStoryList == null || m_lstSkinStoryList.Count < idx)
		{
			return;
		}
		storyUnlockData storyUnlockData = m_lstSkinStoryList[idx];
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(storyUnlockData.m_UnlockReqList[0].reqValue);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (skinTemplet == null)
		{
			return;
		}
		bool flag = false;
		if (nKMUserData != null)
		{
			flag = nKMUserData.m_InventoryData.HasItemSkin(storyUnlockData.m_UnlockReqList[0].reqValue);
		}
		NKCUISkinSlot component = tr.GetComponent<NKCUISkinSlot>();
		if (component != null)
		{
			if (flag)
			{
				component.Init(OnSkinClick);
			}
			component.SetData(skinTemplet, flag, bEquipped: false, !flag);
			m_lstSkinSlot.Add(component);
		}
	}
}
