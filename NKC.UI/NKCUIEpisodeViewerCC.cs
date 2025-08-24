using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEpisodeViewerCC : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_COUNTER_CASE";

	private const string UI_ASSET_NAME = "NKM_EPISODE_CC_Panel";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public Slider m_CollectionRateSlider;

	public Text m_lbCollectionRate;

	public LoopScrollRect m_LoopScrollRect;

	public RectTransform m_rectContentRect;

	public GridLayoutGroup m_GridLayoutGroup;

	public Vector2 m_vUnitSlotSize;

	public Vector2 m_vUnitSlotSpacing;

	public NKCUIComSafeArea m_safeArea;

	private static int m_EpisodeID = 50;

	private static int m_ActID = 0;

	private List<NKCUIEpisodeActSlotCC> m_listItemSlot = new List<NKCUIEpisodeActSlotCC>();

	private Stack<NKCUIEpisodeActSlotCC> m_stkItemSlot = new Stack<NKCUIEpisodeActSlotCC>();

	private List<int> m_lstActIDs = new List<int>();

	private NKMEpisodeTempletV2 cNKMEpisodeTemplet;

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

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_CC;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override List<int> UpsideMenuShowResourceList => new List<int> { 3 };

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIEpisodeViewerCC>("AB_UI_NKM_UI_COUNTER_CASE", "NKM_EPISODE_CC_Panel", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIEpisodeViewerCC GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIEpisodeViewerCC>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public void InitUI()
	{
		m_LoopScrollRect.dOnGetObject += OnGetObject;
		m_LoopScrollRect.dOnReturnObject += OnReturnObject;
		m_LoopScrollRect.dOnProvideData += OnProvideData;
		m_LoopScrollRect.dOnRepopulate += CalculateContentRectSize;
		CalculateContentRectSize();
		m_LoopScrollRect.PrepareCells();
		NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		if ((bool)base.gameObject)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private RectTransform OnGetObject(int index)
	{
		if (m_stkItemSlot.Count == 0)
		{
			NKCUIEpisodeActSlotCC newInstance = NKCUIEpisodeActSlotCC.GetNewInstance(m_rectContentRect.transform, OnSelectedActSlot);
			if (newInstance == null)
			{
				return null;
			}
			newInstance.gameObject.GetComponent<RectTransform>().localScale = Vector2.one;
			m_listItemSlot.Add(newInstance);
			return newInstance.GetComponent<RectTransform>();
		}
		NKCUIEpisodeActSlotCC nKCUIEpisodeActSlotCC = m_stkItemSlot.Pop();
		m_listItemSlot.Add(nKCUIEpisodeActSlotCC);
		return nKCUIEpisodeActSlotCC.GetComponent<RectTransform>();
	}

	private void OnReturnObject(Transform go)
	{
		NKCUIEpisodeActSlotCC component = go.GetComponent<NKCUIEpisodeActSlotCC>();
		m_listItemSlot.Remove(component);
		m_stkItemSlot.Push(component);
		go.SetParent(base.transform);
		NKCUtil.SetGameobjectActive(component, bValue: false);
	}

	private void OnProvideData(Transform transform, int idx)
	{
		transform.GetComponent<NKCUIEpisodeActSlotCC>().SetData(cNKMEpisodeTemplet, m_lstActIDs[idx]);
		NKCUtil.SetGameobjectActive(transform, bValue: true);
	}

	public void SetActID(int id)
	{
		m_ActID = id;
	}

	private void OnSelectedActSlot(int actID)
	{
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().OpenCounterCaseNormalAct(actID);
	}

	public void UpdateUI()
	{
		cNKMEpisodeTemplet = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
		if (cNKMEpisodeTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_CollectionRateSlider.gameObject, bValue: false);
			m_lbCollectionRate.text = "";
			m_LoopScrollRect.RefreshCells();
		}
	}

	public void Open()
	{
		NKCUIFadeInOut.FadeIn(0.1f);
		cNKMEpisodeTemplet = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
		if (cNKMEpisodeTemplet == null)
		{
			return;
		}
		m_lstActIDs.Clear();
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in cNKMEpisodeTemplet.m_DicStage)
		{
			bool flag = true;
			for (int i = 0; i < item.Value.Count; i++)
			{
				NKMStageTempletV2 nKMStageTempletV = item.Value[i];
				if (!nKMStageTempletV.EnableByTag)
				{
					flag = false;
					break;
				}
				if (nKMStageTempletV.m_StageIndex == 1 && nKMStageTempletV.m_StageBasicUnlockType == STAGE_BASIC_UNLOCK_TYPE.SBUT_OPEN && !NKCContentManager.IsStageUnlocked(ContentsType.EPISODE, nKMStageTempletV.Key))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(NKMEpisodeMgr.GetUnitID(cNKMEpisodeTemplet, item.Key));
				if (unitTempletBase != null && unitTempletBase.CollectionEnableByTag)
				{
					m_lstActIDs.Add(item.Key);
				}
			}
		}
		UIOpened();
		m_LoopScrollRect.TotalCount = m_lstActIDs.Count;
		m_LoopScrollRect.RefreshCells();
		UpdateUI();
		m_ActID = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetCounterCaseNormalActID();
		if (m_ActID > 0)
		{
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().OpenCounterCaseNormalAct(m_ActID);
			m_ActID = 0;
		}
		else
		{
			CheckTutorial();
		}
	}

	private void CalculateContentRectSize()
	{
		int minColumn = 5;
		Vector2 vUnitSlotSize = m_vUnitSlotSize;
		Vector2 vUnitSlotSpacing = m_vUnitSlotSpacing;
		if (m_safeArea != null)
		{
			m_safeArea.SetSafeAreaBase();
		}
		NKCUtil.CalculateContentRectSize(m_LoopScrollRect, m_GridLayoutGroup, minColumn, vUnitSlotSize, vUnitSlotSpacing);
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedEpisodeTemplet(null);
		Close();
	}

	public override void UnHide()
	{
		base.UnHide();
		m_LoopScrollRect.RefreshCells();
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.CounterCase);
	}

	public RectTransform GetSlotByUnitID(int unitID)
	{
		int num = -1;
		for (int i = 0; i < m_lstActIDs.Count; i++)
		{
			if (NKMEpisodeMgr.GetUnitID(cNKMEpisodeTemplet, m_lstActIDs[i]) == unitID)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			m_LoopScrollRect.SetIndexPosition(num);
			NKCUIEpisodeActSlotCC nKCUIEpisodeActSlotCC = m_listItemSlot.Find((NKCUIEpisodeActSlotCC v) => v.UnitID == unitID);
			if (nKCUIEpisodeActSlotCC != null)
			{
				return nKCUIEpisodeActSlotCC.GetComponent<RectTransform>();
			}
		}
		return null;
	}
}
