using System.Collections.Generic;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;

namespace NKC.UI;

public class NKCUIOperationV2 : NKCUIBase
{
	public enum OPERATION_CATEGORY
	{
		Summary,
		MainStream,
		Story,
		Growth,
		Challenge
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_OPERATION";

	private const string UI_ASSET_NAME = "AB_UI_OPERATION_UI";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public NKCUIOperationSubSummary m_Summary;

	public NKCUIOperationSubMainStream m_MainStream;

	public NKCUIOperationSubStory m_Story;

	public NKCUIOperationSubGrowth m_Growth;

	public NKCUIOperationSubChallenge m_Challenge;

	public NKCUIComToggle m_tglFavorite;

	public NKCUIOperationCategorySlot m_tglSummary;

	public NKCUIOperationCategorySlot m_tglMainStream;

	public NKCUIOperationCategorySlot m_tglStory;

	public NKCUIOperationCategorySlot m_tglGrowth;

	public NKCUIOperationCategorySlot m_tglChallenge;

	public float m_FadeTime = 0.3f;

	private Dictionary<EPISODE_GROUP, NKCUIOperationCategorySlot> m_dicCategory = new Dictionary<EPISODE_GROUP, NKCUIOperationCategorySlot>();

	private EPISODE_GROUP m_CurSelectedGroup;

	private bool m_bByPassContentUnlock;

	public static bool IsInstanceOpen
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsUIOpen;
			}
			return false;
		}
	}

	public static bool IsInstanceLoaded
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsLoadComplete;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_OPERATION_VIEWER;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.LeftsideWithHamburger;

	public override string GuideTempletID => GetGuideTempletID();

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIOperationV2>("AB_UI_OPERATION", "AB_UI_OPERATION_UI", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIOperationV2 GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIOperationV2>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public override void UnHide()
	{
		base.UnHide();
		foreach (KeyValuePair<EPISODE_GROUP, NKCUIOperationCategorySlot> item in m_dicCategory)
		{
			item.Value.UpdateTglState();
			item.Value.SetSelected(item.Value.GetEpisodeGroup() == m_CurSelectedGroup);
		}
		OnSelectCategory(m_CurSelectedGroup);
		TutorialCheck();
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
	}

	public override void CloseInternal()
	{
		if (NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet() == null)
		{
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedEpisodeCategory(EPISODE_CATEGORY.EC_COUNT);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void InitUI()
	{
		m_Summary.InitUI();
		m_MainStream.InitUI();
		m_Story.InitUI();
		m_Growth.InitUI();
		m_dicCategory = new Dictionary<EPISODE_GROUP, NKCUIOperationCategorySlot>();
		m_dicCategory.Add(EPISODE_GROUP.EG_SUMMARY, m_tglSummary);
		m_dicCategory.Add(EPISODE_GROUP.EG_MAINSTREAM, m_tglMainStream);
		m_dicCategory.Add(EPISODE_GROUP.EG_SUBSTREAM, m_tglStory);
		m_dicCategory.Add(EPISODE_GROUP.EG_GROWTH, m_tglGrowth);
		m_dicCategory.Add(EPISODE_GROUP.EG_CHALLENGE, m_tglChallenge);
		m_tglFavorite.OnValueChanged.RemoveAllListeners();
		m_tglFavorite.OnValueChanged.AddListener(OnClickFavorite);
		m_tglSummary.InitUI(EPISODE_GROUP.EG_SUMMARY, OnClickCategory);
		m_tglMainStream.InitUI(EPISODE_GROUP.EG_MAINSTREAM, OnClickCategory);
		m_tglStory.InitUI(EPISODE_GROUP.EG_SUBSTREAM, OnClickCategory);
		m_tglGrowth.InitUI(EPISODE_GROUP.EG_GROWTH, OnClickCategory);
		m_tglChallenge.InitUI(EPISODE_GROUP.EG_CHALLENGE, OnClickCategory);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(bool bByPassContentUnlock = false)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		foreach (KeyValuePair<EPISODE_GROUP, NKCUIOperationCategorySlot> item in m_dicCategory)
		{
			item.Value.UpdateTglState();
		}
		m_tglFavorite.Select(bSelect: false, bForce: true);
		m_bByPassContentUnlock = bByPassContentUnlock;
		UIOpened();
		EPISODE_CATEGORY reservedEpisodeCategory = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeCategory();
		NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(reservedEpisodeCategory);
		Log.Debug($"[LastPlayInfo][NKCUIOperationV2] ReservedCategory[{reservedEpisodeCategory}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationV2.cs", 158);
		if (nKMEpisodeGroupTemplet != null)
		{
			OnClickCategory(nKMEpisodeGroupTemplet.GroupCategory, bShowFade: false);
		}
		else if (NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_SUMMARY))
		{
			OnClickCategory(EPISODE_GROUP.EG_SUMMARY, bShowFade: false);
		}
		else
		{
			OnClickCategory(EPISODE_GROUP.EG_MAINSTREAM, bShowFade: false);
		}
		if (NKCScenManager.GetScenManager().Get_SCEN_OPERATION().PlayByFavorite)
		{
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().PlayByFavorite = false;
			NKCPopupFavorite.Instance.Open(OnCloseFavorite);
		}
		TutorialCheck();
	}

	public void PreLoad()
	{
	}

	public void OnSelectCategory(EPISODE_GROUP category)
	{
		m_CurSelectedGroup = category;
		NKCUtil.SetGameobjectActive(m_Summary, category == EPISODE_GROUP.EG_SUMMARY);
		NKCUtil.SetGameobjectActive(m_MainStream, category == EPISODE_GROUP.EG_MAINSTREAM);
		NKCUtil.SetGameobjectActive(m_Story, category == EPISODE_GROUP.EG_SUBSTREAM);
		NKCUtil.SetGameobjectActive(m_Growth, category == EPISODE_GROUP.EG_GROWTH);
		NKCUtil.SetGameobjectActive(m_Challenge, category == EPISODE_GROUP.EG_CHALLENGE);
		if (NKCPopupOperationSubStoryList.isOpen())
		{
			NKCPopupOperationSubStoryList.Instance.Close();
		}
		if (category != EPISODE_GROUP.EG_SUBSTREAM)
		{
			NKCSoundManager.PlayScenMusic(NKM_SCEN_ID.NSI_HOME);
		}
		switch (category)
		{
		case EPISODE_GROUP.EG_SUMMARY:
			Log.Debug($"[LastPlayInfo][OnSelectCategory] m_Summary[{m_Summary == null}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationV2.cs", 205);
			m_Summary.Open();
			break;
		case EPISODE_GROUP.EG_MAINSTREAM:
			m_MainStream.Open(m_bByPassContentUnlock);
			break;
		case EPISODE_GROUP.EG_SUBSTREAM:
			m_Story.Open();
			break;
		case EPISODE_GROUP.EG_GROWTH:
			m_Growth.Open();
			break;
		case EPISODE_GROUP.EG_CHALLENGE:
			m_Challenge.Open();
			break;
		}
		UpdateUpsideMenu();
		NKCUIFadeInOut.FadeIn(m_FadeTime);
	}

	private string GetGuideTempletID()
	{
		return m_CurSelectedGroup switch
		{
			EPISODE_GROUP.EG_SUMMARY => "ARTICLE_OPERATION_INFO", 
			EPISODE_GROUP.EG_MAINSTREAM => "ARTICLE_OPERATION_MAINSTREAM", 
			EPISODE_GROUP.EG_SUBSTREAM => "ARTICLE_OPERATION_SIDE_STORY", 
			EPISODE_GROUP.EG_GROWTH => m_Growth.GetGuideTempletID(), 
			EPISODE_GROUP.EG_CHALLENGE => "ARTICLE_OPERATION_DAILY_MISSION", 
			_ => "", 
		};
	}

	public void OnClickCategory(EPISODE_GROUP category, bool bShowFade = true)
	{
		foreach (KeyValuePair<EPISODE_GROUP, NKCUIOperationCategorySlot> item in m_dicCategory)
		{
			item.Value.UpdateTglState();
			if (item.Key == m_CurSelectedGroup || item.Key == category)
			{
				item.Value.ChangeSelected(item.Key == category);
			}
		}
		Log.Debug($"[LastPlayInfo][OnClickCategory] Category[{category}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationV2.cs", 265);
		if (bShowFade)
		{
			NKCUIFadeInOut.FadeOut(m_FadeTime, delegate
			{
				OnSelectCategory(category);
			});
		}
		else
		{
			OnSelectCategory(category);
		}
	}

	public void OnClickMainStream(bool bSet = true)
	{
		if (bSet)
		{
			OnSelectCategory(EPISODE_GROUP.EG_MAINSTREAM);
		}
	}

	public void OnClickStory(bool bSet = true)
	{
		if (bSet)
		{
			OnSelectCategory(EPISODE_GROUP.EG_SUBSTREAM);
		}
	}

	public void OnClickGrowth(bool bSet = true)
	{
		if (bSet)
		{
			OnSelectCategory(EPISODE_GROUP.EG_GROWTH);
		}
	}

	public void OnClickChallenge(bool bSet = true)
	{
		if (bSet)
		{
			OnSelectCategory(EPISODE_GROUP.EG_CHALLENGE);
		}
	}

	public void OnClickFavorite(bool bValue)
	{
		if (bValue)
		{
			NKCPopupFavorite.Instance.Open(OnCloseFavorite);
		}
		else
		{
			NKCPopupFavorite.CheckInstanceAndClose();
		}
	}

	private void OnCloseFavorite()
	{
		m_tglFavorite.Select(bSelect: false, bForce: true);
	}

	private void TutorialCheck()
	{
		if (base.gameObject.activeSelf)
		{
			NKCTutorialManager.TutorialRequired(TutorialPoint.Operation);
		}
	}

	public void SetTutorialMainstreamGuide(NKCGameEventManager.NKCGameEventTemplet eventTemplet, UnityAction Complete)
	{
		OnSelectCategory(EPISODE_GROUP.EG_MAINSTREAM);
		m_MainStream.SetTutorialMainstreamGuide(eventTemplet, Complete);
	}

	public RectTransform GetDailyRect()
	{
		return null;
	}

	public RectTransform GetStageSlotRect(int stageIndex)
	{
		return null;
	}

	public RectTransform GetActSlotRect(int actIndex)
	{
		return null;
	}
}
