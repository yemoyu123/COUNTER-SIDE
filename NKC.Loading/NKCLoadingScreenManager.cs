using System;
using System.Collections.Generic;
using System.Linq;
using NKC.UI;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.Loading;

public static class NKCLoadingScreenManager
{
	public enum eGameContentsType
	{
		NONE,
		WARFARE,
		DUNGEON,
		DIVE,
		FIERCE,
		GAUNTLET,
		RAID,
		SHADOW_PALACE,
		EPISODE,
		DEFAULT,
		TRIM,
		DEFENCE,
		COUNT
	}

	public class NKCLoadingScreenTemplet : INKMTemplet
	{
		public class LoadingScreenData : INKMTemplet
		{
			public eGameContentsType m_eContentType;

			public int m_ContentValue;

			public UnlockInfo m_UnlockInfo;

			public int m_ImgID;

			public string m_DescStrID;

			public bool m_bUnlockCache;

			private string m_OpenTag;

			public int Key => BuildKey(m_eContentType, m_ContentValue);

			public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

			public static LoadingScreenData LoadFromLua(NKMLua cNKMLua)
			{
				if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCLoadingScreenManager.cs", 72))
				{
					return null;
				}
				LoadingScreenData loadingScreenData = new LoadingScreenData();
				int num = 1 & (cNKMLua.GetData("m_eContentType", ref loadingScreenData.m_eContentType) ? 1 : 0);
				cNKMLua.GetData("m_ContentValue", ref loadingScreenData.m_ContentValue);
				int num2 = num & (cNKMLua.GetData("m_ImgID", ref loadingScreenData.m_ImgID) ? 1 : 0);
				cNKMLua.GetData("m_DescStrID", ref loadingScreenData.m_DescStrID);
				cNKMLua.GetData("m_OpenTag", ref loadingScreenData.m_OpenTag);
				loadingScreenData.m_UnlockInfo = UnlockInfo.LoadFromLua(cNKMLua);
				if (num2 == 0)
				{
					return null;
				}
				return loadingScreenData;
			}

			public void Join()
			{
			}

			public void Validate()
			{
			}
		}

		private List<LoadingScreenData> m_lstLoadingScreenData = new List<LoadingScreenData>();

		public int Key { get; }

		public NKCLoadingScreenTemplet(int id, IEnumerable<LoadingScreenData> lstData)
		{
			Key = id;
			m_lstLoadingScreenData = new List<LoadingScreenData>(lstData);
		}

		public LoadingScreenData GetRandomData()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < m_lstLoadingScreenData.Count; i++)
			{
				LoadingScreenData loadingScreenData = m_lstLoadingScreenData[i];
				if (loadingScreenData.EnableByTag)
				{
					if (loadingScreenData.m_bUnlockCache)
					{
						list.Add(i);
					}
					else if (loadingScreenData.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_UNLOCKED)
					{
						list.Add(i);
					}
					else if (NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in loadingScreenData.m_UnlockInfo))
					{
						loadingScreenData.m_bUnlockCache = true;
						list.Add(i);
					}
				}
			}
			int index = list[NKMRandom.Range(0, list.Count)];
			return m_lstLoadingScreenData[index];
		}

		public void ResetUnlockCache()
		{
			foreach (LoadingScreenData lstLoadingScreenDatum in m_lstLoadingScreenData)
			{
				lstLoadingScreenDatum.m_bUnlockCache = false;
			}
		}

		public void Join()
		{
		}

		public void Validate()
		{
			if (m_lstLoadingScreenData.Count == 0)
			{
				Debug.LogError(Key + " : Loading Screen Data Length 0");
			}
			bool flag = false;
			foreach (LoadingScreenData lstLoadingScreenDatum in m_lstLoadingScreenData)
			{
				if (lstLoadingScreenDatum.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_UNLOCKED)
				{
					flag = true;
				}
				if (NKMTempletContainer<NKCLoadingImgTemplet>.Find(lstLoadingScreenDatum.m_ImgID) == null)
				{
					Debug.LogError($"{lstLoadingScreenDatum.m_eContentType} {lstLoadingScreenDatum.m_ContentValue} : ImageID {lstLoadingScreenDatum.m_ImgID} Not exist!");
				}
			}
			if (!flag)
			{
				Debug.LogError(Key + " : Every Loading Screen Data has unlock condition, potential error");
			}
		}
	}

	public class NKCLoadingDescTemplet
	{
		public string m_StrTooltipDesc;

		public UnlockInfo m_UnlockInfo;

		public bool m_bUnlockCache;

		public static NKCLoadingDescTemplet LoadFromLUA(NKMLua cNKMLua)
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCLoadingScreenManager.cs", 186))
			{
				return null;
			}
			NKCLoadingDescTemplet nKCLoadingDescTemplet = new NKCLoadingDescTemplet();
			int num = 1 & (cNKMLua.GetData("m_StrTooltipDesc", ref nKCLoadingDescTemplet.m_StrTooltipDesc) ? 1 : 0);
			nKCLoadingDescTemplet.m_UnlockInfo = UnlockInfo.LoadFromLua(cNKMLua);
			if (num == 0)
			{
				return null;
			}
			return nKCLoadingDescTemplet;
		}
	}

	public class NKCLoadingImgTemplet : INKMTemplet
	{
		public enum eImgType
		{
			FULL,
			CARTOON
		}

		public int m_imgID;

		public eImgType m_eImgType;

		public string m_ImgAssetName;

		public int Key => m_imgID;

		public static NKCLoadingImgTemplet LoadFromLua(NKMLua cNKMLua)
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCManager/NKCLoadingScreenManager.cs", 214))
			{
				return null;
			}
			NKCLoadingImgTemplet nKCLoadingImgTemplet = new NKCLoadingImgTemplet();
			if ((1u & (cNKMLua.GetData("m_imgID", ref nKCLoadingImgTemplet.m_imgID) ? 1u : 0u) & (cNKMLua.GetData("m_eImgType", ref nKCLoadingImgTemplet.m_eImgType) ? 1u : 0u) & (cNKMLua.GetData("m_ImgAssetName", ref nKCLoadingImgTemplet.m_ImgAssetName) ? 1u : 0u)) == 0)
			{
				return null;
			}
			return nKCLoadingImgTemplet;
		}

		public void Join()
		{
		}

		public void Validate()
		{
			if (string.IsNullOrEmpty(m_ImgAssetName))
			{
				Debug.LogError(m_imgID + " : m_ImgAssetName null!");
			}
		}
	}

	private enum eIntroState
	{
		None,
		Outro,
		LoadingLoop,
		Intro_Prefired,
		Intro
	}

	private const int DEFAULT_KEY = 9;

	private static List<NKCLoadingDescTemplet> s_lstLoadingDescTemplet = new List<NKCLoadingDescTemplet>();

	private static NKCAssetInstanceData s_instanceIntro;

	private static NKCUILoadingPhaseTransition s_UIPhaseTransition;

	private static eIntroState s_eState = eIntroState.None;

	private static int BuildKey(eGameContentsType type, int value)
	{
		return (int)(value * 12 + type);
	}

	public static eGameContentsType GetGameContentsType(NKM_GAME_TYPE gameType)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_INVALID:
		case NKM_GAME_TYPE.NGT_DEV:
		case NKM_GAME_TYPE.NGT_PRACTICE:
			return eGameContentsType.DEFAULT;
		case NKM_GAME_TYPE.NGT_DUNGEON:
		case NKM_GAME_TYPE.NGT_TUTORIAL:
		case NKM_GAME_TYPE.NGT_PHASE:
			return eGameContentsType.DUNGEON;
		case NKM_GAME_TYPE.NGT_TRIM:
			return eGameContentsType.TRIM;
		case NKM_GAME_TYPE.NGT_PVE_DEFENCE:
			return eGameContentsType.DEFENCE;
		case NKM_GAME_TYPE.NGT_WARFARE:
			return eGameContentsType.WARFARE;
		case NKM_GAME_TYPE.NGT_DIVE:
			return eGameContentsType.DIVE;
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
			return eGameContentsType.GAUNTLET;
		case NKM_GAME_TYPE.NGT_RAID:
		case NKM_GAME_TYPE.NGT_RAID_SOLO:
			return eGameContentsType.RAID;
		case NKM_GAME_TYPE.NGT_SHADOW_PALACE:
			return eGameContentsType.SHADOW_PALACE;
		case NKM_GAME_TYPE.NGT_FIERCE:
			return eGameContentsType.FIERCE;
		case NKM_GAME_TYPE.NGT_CUTSCENE:
		case NKM_GAME_TYPE.NGT_WORLDMAP:
			return eGameContentsType.DEFAULT;
		default:
			return eGameContentsType.DEFAULT;
		}
	}

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKCLoadingScreenTemplet>.Load(from e in NKMTempletLoader<NKCLoadingScreenTemplet.LoadingScreenData>.LoadGroup("AB_SCRIPT", "LUA_LOADING_TEMPLET", "m_LoadingTemplet", NKCLoadingScreenTemplet.LoadingScreenData.LoadFromLua)
			select new NKCLoadingScreenTemplet(e.Key, e.Value), null);
		NKMTempletContainer<NKCLoadingImgTemplet>.Load("AB_SCRIPT", "LUA_LOADING_IMG_TEMPLET", "m_LoadingImg", NKCLoadingImgTemplet.LoadFromLua);
		LoadLoadingTipString();
		if (NKMTempletContainer<NKCLoadingScreenTemplet>.Find(9) == null)
		{
			Debug.LogError("Default Loading Screen Not Exist!!!");
		}
	}

	private static bool LoadLoadingTipString()
	{
		NKMLua nKMLua = new NKMLua();
		bool flag = nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_TOOLTIP_TEMPLET");
		if (flag)
		{
			flag = nKMLua.OpenTable("m_TooltipTemplet");
			if (flag)
			{
				int num = 1;
				while (nKMLua.OpenTable(num))
				{
					NKCLoadingDescTemplet nKCLoadingDescTemplet = NKCLoadingDescTemplet.LoadFromLUA(nKMLua);
					if (nKCLoadingDescTemplet != null)
					{
						s_lstLoadingDescTemplet.Add(nKCLoadingDescTemplet);
					}
					num++;
					nKMLua.CloseTable();
				}
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		return flag;
	}

	public static bool HasLoadingTemplet(eGameContentsType contentType, int contentValue)
	{
		return NKMTempletContainer<NKCLoadingScreenTemplet>.Find(BuildKey(contentType, contentValue)) != null;
	}

	private static NKCLoadingScreenTemplet GetLoadingTemplet(eGameContentsType contentType, int contentValue, int dungeonID = 0)
	{
		NKCLoadingScreenTemplet nKCLoadingScreenTemplet = null;
		if (contentValue != 0)
		{
			nKCLoadingScreenTemplet = NKMTempletContainer<NKCLoadingScreenTemplet>.Find(BuildKey(contentType, contentValue));
		}
		if (nKCLoadingScreenTemplet == null && dungeonID != 0)
		{
			nKCLoadingScreenTemplet = NKMTempletContainer<NKCLoadingScreenTemplet>.Find(BuildKey(eGameContentsType.DUNGEON, dungeonID));
		}
		if (nKCLoadingScreenTemplet == null)
		{
			nKCLoadingScreenTemplet = NKMTempletContainer<NKCLoadingScreenTemplet>.Find(BuildKey(contentType, 0));
		}
		return nKCLoadingScreenTemplet;
	}

	public static Tuple<NKCLoadingImgTemplet, string> GetLoadingScreen(eGameContentsType contentType, int contentValue, int dungeonID = 0)
	{
		if (NKCScenManager.CurrentUserData() == null)
		{
			return new Tuple<NKCLoadingImgTemplet, string>(null, "");
		}
		NKCLoadingScreenTemplet nKCLoadingScreenTemplet = GetLoadingTemplet(contentType, contentValue, dungeonID);
		if (nKCLoadingScreenTemplet == null)
		{
			nKCLoadingScreenTemplet = GetLoadingTempletFromEpisode(contentType, contentValue);
		}
		if (nKCLoadingScreenTemplet == null)
		{
			nKCLoadingScreenTemplet = NKMTempletContainer<NKCLoadingScreenTemplet>.Find(9);
		}
		if (nKCLoadingScreenTemplet == null)
		{
			return new Tuple<NKCLoadingImgTemplet, string>(null, "");
		}
		NKCLoadingScreenTemplet.LoadingScreenData randomData = nKCLoadingScreenTemplet.GetRandomData();
		NKCLoadingImgTemplet item = NKMTempletContainer<NKCLoadingImgTemplet>.Find(randomData.m_ImgID);
		string item2 = ((!string.IsNullOrEmpty(randomData.m_DescStrID)) ? randomData.m_DescStrID : GetRandomLoadingTip());
		return new Tuple<NKCLoadingImgTemplet, string>(item, item2);
	}

	public static void ResetUnlockCache()
	{
		foreach (NKCLoadingScreenTemplet value in NKMTempletContainer<NKCLoadingScreenTemplet>.Values)
		{
			value.ResetUnlockCache();
		}
		foreach (NKCLoadingDescTemplet item in s_lstLoadingDescTemplet)
		{
			item.m_bUnlockCache = false;
		}
	}

	private static string GetRandomLoadingTip()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < s_lstLoadingDescTemplet.Count; i++)
		{
			NKCLoadingDescTemplet nKCLoadingDescTemplet = s_lstLoadingDescTemplet[i];
			if (nKCLoadingDescTemplet.m_bUnlockCache)
			{
				list.Add(i);
			}
			else if (nKCLoadingDescTemplet.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_UNLOCKED)
			{
				list.Add(i);
			}
			else if (NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKCLoadingDescTemplet.m_UnlockInfo))
			{
				nKCLoadingDescTemplet.m_bUnlockCache = true;
				list.Add(i);
			}
		}
		int index = list[NKMRandom.Range(0, list.Count)];
		return s_lstLoadingDescTemplet[index].m_StrTooltipDesc;
	}

	private static NKCLoadingScreenTemplet GetLoadingTempletFromEpisode(eGameContentsType contentType, int contentValue)
	{
		switch (contentType)
		{
		case eGameContentsType.DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(contentValue);
			if (dungeonTempletBase != null)
			{
				NKMStageTempletV2 nKMStageTempletV2 = NKMEpisodeMgr.FindStageTempletByBattleStrID(dungeonTempletBase.m_DungeonStrID);
				if (nKMStageTempletV2 != null)
				{
					return GetLoadingTemplet(eGameContentsType.EPISODE, nKMStageTempletV2.EpisodeId);
				}
			}
			break;
		}
		case eGameContentsType.WARFARE:
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(contentValue);
			if (nKMWarfareTemplet != null)
			{
				NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(nKMWarfareTemplet.m_WarfareStrID);
				return GetLoadingTemplet(eGameContentsType.EPISODE, nKMStageTempletV.EpisodeId);
			}
			break;
		}
		}
		return null;
	}

	public static void SetLoadingProgress(float progress)
	{
		NKCUIManager.LoadingUI.SetLoadingProgress(progress);
		if (s_UIPhaseTransition != null)
		{
			s_UIPhaseTransition.SetLoadingProgress(progress);
		}
	}

	public static void Update()
	{
		if (s_UIPhaseTransition != null && s_UIPhaseTransition.IsAnimFinished())
		{
			AdvanceIntroState();
		}
	}

	private static void AdvanceIntroState()
	{
		switch (s_eState)
		{
		case eIntroState.Outro:
			if (s_UIPhaseTransition != null)
			{
				s_UIPhaseTransition.PlayIdle();
			}
			s_eState = eIntroState.LoadingLoop;
			break;
		case eIntroState.Intro:
			CleanupIntroObject();
			break;
		case eIntroState.Intro_Prefired:
			NKCUtil.SetGameobjectActive(s_instanceIntro.m_Instant, bValue: false);
			break;
		case eIntroState.LoadingLoop:
			if (NKCUICutScenPlayer.IsInstanceOpen)
			{
				if (s_UIPhaseTransition != null)
				{
					s_UIPhaseTransition.PlayIntro();
				}
				s_eState = eIntroState.Intro_Prefired;
			}
			break;
		}
	}

	public static void PlayDungeonIntro(NKMGameData gameData)
	{
		NKMAssetName introName = GetIntroName(gameData);
		if (introName == null)
		{
			CleanupIntroObject();
			return;
		}
		if (s_eState == eIntroState.Intro_Prefired)
		{
			CleanupIntroObject();
			return;
		}
		MakeIntroObject(introName);
		if (s_UIPhaseTransition != null)
		{
			s_UIPhaseTransition.PlayIntro();
		}
		s_eState = eIntroState.Intro;
	}

	public static void PlayDungeonOutro(NKMGameData gameData)
	{
		NKMAssetName outroName = GetOutroName(gameData);
		if (outroName == null)
		{
			CleanupIntroObject();
			return;
		}
		MakeIntroObject(outroName);
		if (s_UIPhaseTransition != null)
		{
			s_UIPhaseTransition.PlayOutro();
		}
		s_eState = eIntroState.Outro;
	}

	public static void CleanupIntroObject()
	{
		if (s_instanceIntro != null)
		{
			s_instanceIntro.Close();
		}
		s_instanceIntro = null;
		s_UIPhaseTransition = null;
		s_eState = eIntroState.None;
	}

	private static NKCUILoadingPhaseTransition MakeIntroObject(NKMAssetName assetName)
	{
		if (s_instanceIntro != null && s_instanceIntro.m_Instant != null && s_instanceIntro.m_BundleName == assetName.m_BundleName && s_instanceIntro.m_AssetName == assetName.m_AssetName)
		{
			return s_UIPhaseTransition;
		}
		CleanupIntroObject();
		s_instanceIntro = NKCAssetResourceManager.OpenInstance<GameObject>(assetName, bAsync: false, NKCScenManager.GetScenManager().Get_NUF_AFTER_UI_EFFECT());
		if (s_instanceIntro != null && (bool)s_instanceIntro.m_Instant)
		{
			s_UIPhaseTransition = s_instanceIntro.m_Instant.GetComponent<NKCUILoadingPhaseTransition>();
		}
		return s_UIPhaseTransition;
	}

	public static NKMAssetName GetIntroName(NKMGameData gameData)
	{
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(gameData.m_DungeonID);
		if (dungeonTempletBase != null && !string.IsNullOrEmpty(dungeonTempletBase.m_Intro))
		{
			return NKMAssetName.ParseBundleName(dungeonTempletBase.m_Intro, dungeonTempletBase.m_Intro);
		}
		if (gameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PHASE && !NKCPhaseManager.IsFirstStage(gameData.m_DungeonID))
		{
			NKMPhaseTemplet phaseTemplet = NKCPhaseManager.GetPhaseTemplet();
			if (phaseTemplet != null && !string.IsNullOrEmpty(phaseTemplet.m_Intro))
			{
				return NKMAssetName.ParseBundleName(phaseTemplet.m_Intro, phaseTemplet.m_Intro);
			}
		}
		return null;
	}

	public static NKMAssetName GetOutroName(NKMGameData gameData)
	{
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(gameData.m_DungeonID);
		if (dungeonTempletBase != null && !string.IsNullOrEmpty(dungeonTempletBase.m_Outro))
		{
			return NKMAssetName.ParseBundleName(dungeonTempletBase.m_Outro, dungeonTempletBase.m_Outro);
		}
		if (gameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PHASE && !NKCPhaseManager.IsLastStage(gameData.m_DungeonID))
		{
			NKMPhaseTemplet phaseTemplet = NKCPhaseManager.GetPhaseTemplet();
			if (phaseTemplet != null && !string.IsNullOrEmpty(phaseTemplet.m_Outro))
			{
				return NKMAssetName.ParseBundleName(phaseTemplet.m_Outro, phaseTemplet.m_Outro);
			}
		}
		return null;
	}
}
