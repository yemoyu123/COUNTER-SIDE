using System;
using System.Collections.Generic;
using DG.Tweening;
using NKC.UI.Collection;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuCollection : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnMenu;

	public Text m_NKM_UI_LOBBY_RIGHT_MENU_2_ALBUM_COUNT;

	public Image m_imgProgress;

	public GameObject m_NKM_UI_LOBBY_RIGHT_MENU_2_ALBUM_Reddot;

	private float m_fPercent;

	private const float m_fAnimTime = 0.6f;

	private const Ease m_eAnimEase = Ease.InCubic;

	private string animBuffer;

	private string PercentString => Mathf.FloorToInt(m_fPercent * 100f) + "%";

	public float Fillrate
	{
		get
		{
			if (!(m_imgProgress != null))
			{
				return 0f;
			}
			return m_imgProgress.fillAmount;
		}
		set
		{
			if (m_imgProgress != null)
			{
				m_imgProgress.fillAmount = value;
			}
		}
	}

	public void Init()
	{
		if (m_csbtnMenu != null)
		{
			m_csbtnMenu.PointerClick.RemoveAllListeners();
			m_csbtnMenu.PointerClick.AddListener(OnButton);
		}
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		NKCCollectionManager.Init();
		SetNotify(value: false);
		CheckCollectionTotalRate(userData);
	}

	private void CheckCollectionTotalRate(NKMUserData userData)
	{
		int hasTeamUpCount = 0;
		int totalTeamUpCount = 0;
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (armyData != null)
		{
			bool flag = false;
			new List<NKCUICollectionTeamUp.TeamUpSlotData>();
			NKCUICollectionTeamUp.UpdateTeamUpList(ref hasTeamUpCount, ref totalTeamUpCount, armyData, getTeamUpList: false, out var bNotify);
			if (bNotify && NKCUnitMissionManager.GetOpenTagCollectionTeamUp())
			{
				flag = true;
			}
			if (NKCUnitMissionManager.HasRewardEnableMission())
			{
				flag = true;
			}
			NKCUICollectionUnitList.UpdateCollectionUnitList(ref hasTeamUpCount, ref totalTeamUpCount, NKM_UNIT_TYPE.NUT_NORMAL, getUnitDataList: false);
			NKCUICollectionUnitList.UpdateCollectionUnitList(ref hasTeamUpCount, ref totalTeamUpCount, NKM_UNIT_TYPE.NUT_SHIP, getUnitDataList: false);
			if (!NKCOperatorUtil.IsHide() && NKCOperatorUtil.IsActive())
			{
				NKCUICollectionOperatorList.UpdateCollectionUnitList(ref hasTeamUpCount, ref totalTeamUpCount, createOperatorDataList: false);
			}
			AddCollectionStoryCount(userData, ref hasTeamUpCount, ref totalTeamUpCount);
			m_fPercent = Mathf.Floor(hasTeamUpCount) / Mathf.Floor(totalTeamUpCount);
			m_fPercent = Mathf.Clamp(m_fPercent, 0f, 1f);
			NKCUtil.SetLabelText(m_NKM_UI_LOBBY_RIGHT_MENU_2_ALBUM_COUNT, PercentString);
			NKCUtil.SetImageFillAmount(m_imgProgress, m_fPercent);
			NKCUtil.SetGameobjectActive(m_NKM_UI_LOBBY_RIGHT_MENU_2_ALBUM_Reddot, flag);
			base.SetNotify(flag);
		}
	}

	private void AddCollectionUnitCount(ref int collected, ref int total, NKM_UNIT_TYPE type)
	{
		List<int> unitList = NKCCollectionManager.GetUnitList(type);
		for (int i = 0; i < unitList.Count; i++)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitList[i]);
			if (unitTempletBase == null)
			{
				continue;
			}
			NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(unitTempletBase.m_UnitID);
			if ((unitTemplet == null || !unitTemplet.m_bExclude) && unitTempletBase.CollectionEnableByTag)
			{
				if (NKCUICollectionUnitList.IsHasUnit(type, unitList[i]))
				{
					collected++;
				}
				total++;
			}
		}
	}

	private void AddCollectionStoryCount(NKMUserData userData, ref int collected, ref int total)
	{
		if (userData == null)
		{
			return;
		}
		Dictionary<int, storyUnlockData> storyData = NKCCollectionManager.GetStoryData();
		Dictionary<int, List<int>> epiSodeStageIdData = NKCCollectionManager.GetEpiSodeStageIdData();
		if (storyData == null)
		{
			return;
		}
		Dictionary<NKCCollectionManager.COLLECTION_STORY_CATEGORY, List<NKCUICollectionStory.EpData>> dictionary = new Dictionary<NKCCollectionManager.COLLECTION_STORY_CATEGORY, List<NKCUICollectionStory.EpData>>();
		foreach (NKMEpisodeTempletV2 value in NKMEpisodeTempletV2.Values)
		{
			if (!NKCUICollectionStory.IsVaildCollectionStory(value) || !epiSodeStageIdData.ContainsKey(value.m_EpisodeID))
			{
				continue;
			}
			if (!storyData.ContainsKey(epiSodeStageIdData[value.m_EpisodeID][0]))
			{
				Debug.LogError($"CutScene Templet does'nt contain stage ID: {epiSodeStageIdData[value.m_EpisodeID][0]}");
				continue;
			}
			EPISODE_CATEGORY episodeCategory = storyData[epiSodeStageIdData[value.m_EpisodeID][0]].m_EpisodeCategory;
			int sortIndex = value?.m_SortIndex ?? 0;
			NKCUICollectionStory.EpData epData = new NKCUICollectionStory.EpData(episodeCategory, value.m_EpisodeID, value.m_EpisodeTitle, value.m_EpisodeName, sortIndex);
			int count = epiSodeStageIdData[value.m_EpisodeID].Count;
			for (int i = 0; i < count; i++)
			{
				int key = epiSodeStageIdData[value.m_EpisodeID][i];
				NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(key);
				if (nKMStageTempletV != null)
				{
					epData.m_lstEpisodeStages.Add(new NKCUICollectionStory.EpSlotData(storyData[nKMStageTempletV.Key].m_UnlockReqList, storyData[key].m_ActID, nKMStageTempletV.m_StageIndex, "", ""));
				}
			}
			NKCCollectionManager.COLLECTION_STORY_CATEGORY collectionStoryCategory = NKCCollectionManager.GetCollectionStoryCategory(episodeCategory);
			if (dictionary.ContainsKey(collectionStoryCategory))
			{
				dictionary[collectionStoryCategory].Add(epData);
				continue;
			}
			List<NKCUICollectionStory.EpData> list = new List<NKCUICollectionStory.EpData>();
			list.Add(epData);
			dictionary.Add(collectionStoryCategory, list);
		}
		NKCUICollectionStory.EpData epData2 = new NKCUICollectionStory.EpData(new NKMDiveTemplet());
		int num = 0;
		foreach (NKMDiveTemplet value2 in NKMTempletContainer<NKMDiveTemplet>.Values)
		{
			if (NKCUICollectionStory.IsValidCollectionStory(value2) && storyData.ContainsKey(value2.StageID))
			{
				storyUnlockData storyUnlockData = storyData[value2.StageID];
				if (!string.IsNullOrEmpty(value2.CutsceneDiveEnter))
				{
					List<UnlockInfo> unlockReqList = storyUnlockData.m_UnlockReqList;
					epData2.m_lstEpisodeStages.Add(new NKCUICollectionStory.EpSlotData(unlockReqList, num, 1, value2.CutsceneDiveEnter, ""));
				}
				if (!string.IsNullOrEmpty(value2.CutsceneDiveStart))
				{
					List<UnlockInfo> unlockReqList2 = storyUnlockData.m_UnlockReqList;
					epData2.m_lstEpisodeStages.Add(new NKCUICollectionStory.EpSlotData(unlockReqList2, num, 2, value2.CutsceneDiveStart, ""));
				}
				if (!string.IsNullOrEmpty(value2.CutsceneDiveBossBefore))
				{
					List<UnlockInfo> unlockReqList3 = storyUnlockData.m_UnlockReqList;
					epData2.m_lstEpisodeStages.Add(new NKCUICollectionStory.EpSlotData(unlockReqList3, num, 3, value2.CutsceneDiveBossBefore, ""));
				}
				if (!string.IsNullOrEmpty(value2.CutsceneDiveBossAfter))
				{
					List<UnlockInfo> unlockReqList4 = storyUnlockData.m_UnlockReqList;
					epData2.m_lstEpisodeStages.Add(new NKCUICollectionStory.EpSlotData(unlockReqList4, num, 4, value2.CutsceneDiveBossAfter, ""));
				}
				num++;
			}
		}
		if (epData2.m_lstEpisodeStages.Count > 0)
		{
			List<NKCUICollectionStory.EpData> list2 = new List<NKCUICollectionStory.EpData>();
			list2.Add(epData2);
			if (!dictionary.ContainsKey(NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP))
			{
				dictionary.Add(NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP, list2);
			}
			else
			{
				dictionary[NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP].Add(epData2);
			}
		}
		foreach (NKCCollectionManager.COLLECTION_STORY_CATEGORY value3 in Enum.GetValues(typeof(NKCCollectionManager.COLLECTION_STORY_CATEGORY)))
		{
			Dictionary<int, NKCUICollectionStory.StorySlotData> dicStory = new Dictionary<int, NKCUICollectionStory.StorySlotData>();
			NKCUICollectionStory.UpdateEpisodeCategory(ref collected, ref total, value3, dictionary, getStoryData: false, ref dicStory);
		}
	}

	public override void PlayAnimation(bool bActive)
	{
		m_NKM_UI_LOBBY_RIGHT_MENU_2_ALBUM_COUNT.DOKill();
		m_imgProgress.DOKill();
		if (bActive)
		{
			animBuffer = Mathf.FloorToInt(m_fPercent * 100f).ToString();
			DOTween.To(() => animBuffer, delegate(string x)
			{
				animBuffer = x;
				m_NKM_UI_LOBBY_RIGHT_MENU_2_ALBUM_COUNT.text = x + "%";
			}, Mathf.FloorToInt(m_fPercent * 100f).ToString(), 0.6f).SetOptions(richTextEnabled: false, ScrambleMode.Numerals).SetTarget(m_NKM_UI_LOBBY_RIGHT_MENU_2_ALBUM_COUNT)
				.SetEase(Ease.InCubic);
			Fillrate = 0f;
			m_imgProgress.DOFillAmount(m_fPercent, 0.6f).SetEase(Ease.InCubic);
		}
		else
		{
			NKCUtil.SetLabelText(m_NKM_UI_LOBBY_RIGHT_MENU_2_ALBUM_COUNT, PercentString);
			Fillrate = m_fPercent;
		}
	}

	private void OnButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_COLLECTION);
	}
}
