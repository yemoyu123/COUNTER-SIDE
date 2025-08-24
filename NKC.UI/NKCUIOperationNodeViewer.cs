using System.Collections.Generic;
using ClientPacket.Warfare;
using Cs.Logging;
using DG.Tweening;
using NKC.Templet;
using NKC.UI.Component;
using NKC.UI.Shop;
using NKM;
using NKM.Event;
using NKM.Shop;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationNodeViewer : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_OPERATION";

	private const string UI_ASSET_NAME = "AB_UI_OPERATION_MAIN";

	private static NKCUIOperationNodeViewer m_Instance;

	private Dictionary<int, INKCUIStageViewer> m_dicStageViewer = new Dictionary<int, INKCUIStageViewer>();

	private INKCUIStageViewer m_CurStageViewer;

	[Header("\ufffd\ufffd\ufffd")]
	public TMP_FontAsset m_fontRajhani;

	public int m_fontRajhaniSize;

	public TMP_FontAsset m_fontNormal;

	public int m_fontNormalSize;

	public TMP_Text m_lbEpTitle;

	public TMP_Text m_lbEpSubTitle;

	public NKCUIComStateButton m_btnMedal;

	public Text m_lbMedalCount;

	public GameObject m_objMedalReddot;

	public NKCUIComStateButton m_btnEventMission;

	public GameObject m_objEventMissionReddot;

	[Header("\ufffd߾\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public Image m_imgBG;

	public ScrollRect m_srContent;

	public Transform m_trNodeStageParent;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public Animator m_AniRightSide;

	public NKCUIStageInfo m_StageInfo;

	[Header("\ufffdϴ\ufffd \ufffd\u07b4\ufffd")]
	public GameObject m_objBottom;

	public NKCUIComStateButton m_btnDifficulty;

	public GameObject m_objDifficultyNormal;

	public GameObject m_objDifficultyHard;

	public LoopScrollRect m_srAct;

	public NKCUIComToggleGroup m_tgAct;

	public NKCUIStageActSlot m_pfbActSlot;

	public NKCUIComStateButton m_btnShop;

	public Image m_imgShopIcon;

	public TMP_Text m_lbShopCount;

	public Image m_imgShopBG;

	[Header("\ufffd\ufffd\ufffd\u0335\ufffd \ufffdð\ufffd")]
	public float m_FadeTime = 0.3f;

	private Stack<NKCUIStageActSlot> m_stkActSlot = new Stack<NKCUIStageActSlot>();

	private List<NKCUIStageActSlot> m_lstActSlot = new List<NKCUIStageActSlot>();

	private List<int> m_lstStageViewerID = new List<int>();

	private Dictionary<int, GameObject> m_dicUnlockEffectGo = new Dictionary<int, GameObject>();

	private NKMEpisodeTempletV2 m_EpisodeTemplet;

	private NKMStageTempletV2 m_ReservedStageTemplet;

	private int m_SelectedActSlotID = 1;

	private int m_StageViewerNodeID;

	private bool m_bUseEpSlot;

	private bool m_bFirstSetting;

	private NKCPopupAchieveRateReward m_NKCPopupAchieveRateReward;

	private Vector2 m_vDownPos = Vector2.zero;

	private float m_fDragCheckDistance = 100f;

	private NKCUIShopSingleTab m_OperationShop;

	public static NKCUIOperationNodeViewer Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOperationNodeViewer>("AB_UI_OPERATION", "AB_UI_OPERATION_MAIN", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIOperationNodeViewer>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "";

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (m_EpisodeTemplet != null && m_EpisodeTemplet.ResourceIdList != null && m_EpisodeTemplet.ResourceIdList.Count > 0)
			{
				return m_EpisodeTemplet.ResourceIdList;
			}
			return base.UpsideMenuShowResourceList;
		}
	}

	private NKCPopupAchieveRateReward NKCPopupAchieveRateReward
	{
		get
		{
			if (m_NKCPopupAchieveRateReward == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupAchieveRateReward>("AB_UI_NKM_UI_OPERATION", "NKM_UI_OPERATION_POPUP_MEDAL", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCPopupAchieveRateReward = loadedUIData.GetInstance<NKCPopupAchieveRateReward>();
				m_NKCPopupAchieveRateReward?.InitUI();
			}
			return m_NKCPopupAchieveRateReward;
		}
	}

	private NKCUIShopSingleTab OperationShop
	{
		get
		{
			if (m_OperationShop == null)
			{
				m_OperationShop = NKCUIShopSingleTab.GetInstance("AB_UI_NKM_UI_SHOP", "NKM_UI_OPERATION_SHOP");
			}
			return m_OperationShop;
		}
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static bool isOpen()
	{
		if (m_Instance != null)
		{
			return m_Instance.IsOpen;
		}
		return false;
	}

	public void InitUI()
	{
		if (m_srAct != null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			m_srAct.dOnGetObject += GetObject;
			m_srAct.dOnReturnObject += ReturnObject;
			m_srAct.dOnProvideData += ProvideData;
			Canvas.ForceUpdateCanvases();
			m_srAct.PrepareCells();
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
		if (m_btnMedal != null)
		{
			m_btnMedal.PointerClick.RemoveAllListeners();
			m_btnMedal.PointerClick.AddListener(OnClickMedal);
		}
		if (m_btnEventMission != null)
		{
			m_btnEventMission.PointerClick.RemoveAllListeners();
			m_btnEventMission.PointerClick.AddListener(OnClickEventMission);
		}
		if (m_btnDifficulty != null)
		{
			m_btnDifficulty.PointerClick.RemoveAllListeners();
			m_btnDifficulty.PointerClick.AddListener(OnClickChangeDifficulty);
			m_btnDifficulty.m_bGetCallbackWhileLocked = true;
			m_btnDifficulty.m_HotkeyEventType = HotkeyEventType.NextTab;
		}
		if (m_btnShop != null)
		{
			m_btnShop.PointerClick.RemoveAllListeners();
			m_btnShop.PointerClick.AddListener(OnClickShop);
		}
		if (m_StageInfo != null)
		{
			m_StageInfo.InitUI(OnClickStart);
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback = new EventTrigger.TriggerEvent();
		entry.callback.AddListener(OnPointClick);
		EventTrigger eventTrigger = base.gameObject.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = base.gameObject.AddComponent<EventTrigger>();
		}
		eventTrigger.triggers.Add(entry);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_CurStageViewer = null;
	}

	public override void OnBackButton()
	{
		if (m_StageInfo != null && m_StageInfo.gameObject.activeSelf)
		{
			OnPointClick(null);
			return;
		}
		NKCUIFadeInOut.FadeOut(m_FadeTime, delegate
		{
			base.OnBackButton();
		});
	}

	public override void UnHide()
	{
		base.UnHide();
		SetEventMissionReddot();
		SetShopButton();
	}

	public RectTransform GetObject(int idx)
	{
		NKCUIStageActSlot nKCUIStageActSlot = null;
		nKCUIStageActSlot = ((m_stkActSlot.Count <= 0) ? Object.Instantiate(m_pfbActSlot, m_srAct.content) : m_stkActSlot.Pop());
		return nKCUIStageActSlot.GetComponent<RectTransform>();
	}

	public void ReturnObject(Transform tr)
	{
		NKCUIStageActSlot component = tr.GetComponent<NKCUIStageActSlot>();
		if (!(component == null))
		{
			m_lstActSlot.Remove(component);
			component.ResetData();
			m_stkActSlot.Push(component);
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
	}

	public void ProvideData(Transform tr, int idx)
	{
		NKCUIStageActSlot slot = tr.GetComponent<NKCUIStageActSlot>();
		if (slot == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(slot, bValue: true);
		if (!m_bUseEpSlot)
		{
			slot.SetData(m_EpisodeTemplet.m_EpisodeID, m_EpisodeTemplet.m_Difficulty, m_lstStageViewerID[idx], GetActName(m_EpisodeTemplet, idx), m_tgAct, OnSelectedActSlot);
		}
		else
		{
			NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_lstStageViewerID[idx], EPISODE_DIFFICULTY.NORMAL);
			if (nKMEpisodeTempletV != null)
			{
				slot.SetData(nKMEpisodeTempletV.m_EpisodeID, nKMEpisodeTempletV.m_Difficulty, m_lstStageViewerID[idx], nKMEpisodeTempletV.GetEpisodeName(), m_tgAct, OnSelectedActSlot);
			}
		}
		slot.SetSelected(slot.GetStageViewerID() == m_StageViewerNodeID);
		if (m_lstActSlot.Find((NKCUIStageActSlot x) => x.GetStageViewerID() == slot.GetStageViewerID()) == null)
		{
			m_lstActSlot.Add(slot);
		}
	}

	public void Open(NKMEpisodeTempletV2 epTemplet, bool bByPassContentUnlockPopup = false)
	{
		if (epTemplet == null)
		{
			OnBackButton();
			return;
		}
		if (!string.IsNullOrEmpty(epTemplet.m_BG_Music))
		{
			NKCBGMInfoTemplet nKCBGMInfoTemplet = NKCBGMInfoTemplet.Find(epTemplet.m_BG_Music);
			if (nKCBGMInfoTemplet != null && !NKCSoundManager.IsSameMusic(nKCBGMInfoTemplet.m_BgmAssetID))
			{
				NKCSoundManager.PlayMusic(nKCBGMInfoTemplet.m_BgmAssetID);
			}
		}
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().PlayByFavorite = false;
		m_ReservedStageTemplet = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedStageTemplet();
		if (m_ReservedStageTemplet != null)
		{
			epTemplet = m_ReservedStageTemplet.EpisodeTemplet;
		}
		m_bUseEpSlot = epTemplet.UseEpSlot();
		m_lstActSlot.Clear();
		if (m_StageInfo.IsOpened())
		{
			m_StageInfo.Close();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetData(epTemplet.m_EpisodeID, epTemplet.m_Difficulty);
		UIOpened();
		NKCUIFadeInOut.FadeIn(0.3f);
		if (m_ReservedStageTemplet != null)
		{
			SetScrollToIndex(m_ReservedStageTemplet.m_StageIndex - 1);
			m_StageInfo.Open(m_ReservedStageTemplet);
			m_ReservedStageTemplet = null;
		}
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedStage(null);
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedEpisodeTemplet(null);
		if (!bByPassContentUnlockPopup)
		{
			NKCContentManager.ShowContentUnlockPopup(delegate
			{
				TutorialCheck();
			});
		}
	}

	public void SetData(int episodeID, EPISODE_DIFFICULTY difficulty)
	{
		m_EpisodeTemplet = NKMEpisodeTempletV2.Find(episodeID, difficulty);
		if (m_EpisodeTemplet == null)
		{
			Log.Error($"EpisodeTemplet is null - {episodeID} : {difficulty}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationNodeViewer.cs", 332);
			Close();
			return;
		}
		m_lstStageViewerID.Clear();
		if (!m_bUseEpSlot)
		{
			foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in m_EpisodeTemplet.m_DicStage)
			{
				if (item.Value.Count > 0)
				{
					m_lstStageViewerID.Add(item.Value[0].ActId);
				}
			}
			m_lstStageViewerID.Sort();
		}
		else
		{
			NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(m_EpisodeTemplet.m_GroupID);
			for (int i = 0; i < nKMEpisodeGroupTemplet.lstEpisodeTemplet.Count; i++)
			{
				m_lstStageViewerID.Add(nKMEpisodeGroupTemplet.lstEpisodeTemplet[i].m_EpisodeID);
			}
		}
		if (m_lstStageViewerID.Count <= 0)
		{
			Log.Error($"Ȱ\ufffd\ufffdȭ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd - {m_EpisodeTemplet.m_EpisodeID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationNodeViewer.cs", 360);
			OnBackButton();
			return;
		}
		if (m_ReservedStageTemplet != null)
		{
			if (!m_bUseEpSlot)
			{
				m_StageViewerNodeID = m_ReservedStageTemplet.ActId;
			}
			else
			{
				m_StageViewerNodeID = m_ReservedStageTemplet.EpisodeId;
			}
		}
		else
		{
			m_StageViewerNodeID = GetLastNodeID();
		}
		m_srAct.TotalCount = m_lstStageViewerID.Count;
		if (m_srAct.TotalCount > 1)
		{
			NKCUtil.SetGameobjectActive(m_objBottom, bValue: true);
			NKCUtil.SetGameobjectActive(m_srAct, bValue: true);
			m_srAct.RefreshCells();
			int index = m_lstStageViewerID.FindIndex((int x) => x == m_StageViewerNodeID);
			m_srAct.ScrollToCell(index, 0.1f);
		}
		m_bFirstSetting = true;
		NKCUtil.SetGameobjectActive(m_objDifficultyNormal, m_EpisodeTemplet.m_Difficulty == EPISODE_DIFFICULTY.NORMAL);
		NKCUtil.SetGameobjectActive(m_objDifficultyHard, m_EpisodeTemplet.m_Difficulty == EPISODE_DIFFICULTY.HARD);
		OnSelectedActSlot(m_StageViewerNodeID);
	}

	private int GetLastNodeID()
	{
		if (m_bUseEpSlot)
		{
			if (m_ReservedStageTemplet != null)
			{
				return m_ReservedStageTemplet.EpisodeId;
			}
			return m_EpisodeTemplet.m_EpisodeID;
		}
		if (m_ReservedStageTemplet != null)
		{
			return m_ReservedStageTemplet.ActId;
		}
		for (int num = m_lstStageViewerID.Count - 1; num >= 0; num--)
		{
			if (NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in m_EpisodeTemplet.GetFirstStage(m_lstStageViewerID[num]).m_UnlockInfo))
			{
				return m_lstStageViewerID[num];
			}
		}
		return m_lstStageViewerID[0];
	}

	private void OnSelectedActSlot(int stageViewerNodeID)
	{
		m_StageViewerNodeID = stageViewerNodeID;
		if (m_bUseEpSlot)
		{
			m_EpisodeTemplet = NKMEpisodeTempletV2.Find(m_StageViewerNodeID, EPISODE_DIFFICULTY.NORMAL);
			int firstStageID = NKCContentManager.GetFirstStageID(m_EpisodeTemplet, 1, m_EpisodeTemplet.m_Difficulty);
			if (m_dicUnlockEffectGo.ContainsKey(firstStageID))
			{
				Object.Destroy(m_dicUnlockEffectGo[firstStageID]);
				m_dicUnlockEffectGo.Remove(firstStageID);
			}
			NKCContentManager.RemoveUnlockedContent(ContentsType.ACT, firstStageID);
		}
		else
		{
			int firstStageID2 = NKCContentManager.GetFirstStageID(m_EpisodeTemplet, stageViewerNodeID, m_EpisodeTemplet.m_Difficulty);
			if (m_dicUnlockEffectGo.ContainsKey(firstStageID2))
			{
				Object.Destroy(m_dicUnlockEffectGo[firstStageID2]);
				m_dicUnlockEffectGo.Remove(firstStageID2);
			}
			NKCContentManager.RemoveUnlockedContent(ContentsType.ACT, firstStageID2);
		}
		foreach (KeyValuePair<int, INKCUIStageViewer> item in m_dicStageViewer)
		{
			if (item.Value is NKCUIStageViewer)
			{
				NKCUtil.SetGameobjectActive(item.Value as NKCUIStageViewer, bValue: false);
			}
			else if (item.Value is NKCUIStageViewerV2)
			{
				NKCUtil.SetGameobjectActive(item.Value as NKCUIStageViewerV2, bValue: false);
			}
		}
		if (m_StageInfo.IsOpened())
		{
			m_StageInfo.Close();
		}
		int num = 0;
		string stage_Viewer_Prefab = m_EpisodeTemplet.m_Stage_Viewer_Prefab;
		int num2 = 0;
		int num3 = 0;
		if (!m_bUseEpSlot)
		{
			num = m_EpisodeTemplet.m_EpisodeID;
			num2 = m_EpisodeTemplet.m_DicStage.Count;
			num3 = stageViewerNodeID;
			m_SelectedActSlotID = stageViewerNodeID;
		}
		else
		{
			NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(m_EpisodeTemplet.m_GroupID);
			num = nKMEpisodeGroupTemplet.EpisodeGroupID;
			num2 = nKMEpisodeGroupTemplet.lstEpisodeTemplet.Count;
			num3 = m_EpisodeTemplet.m_EpisodeID;
			m_SelectedActSlotID = 1;
		}
		if (!m_dicStageViewer.ContainsKey(num))
		{
			GameObject orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<GameObject>(stage_Viewer_Prefab, stage_Viewer_Prefab, tryParseAssetName: true);
			if (orLoadAssetResource == null)
			{
				Log.Error($"\ufffd\u033a\ufffdƮ \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffdƮ \ufffdε忡 \ufffd\ufffd\ufffd\ufffd : {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationNodeViewer.cs", 493);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
				return;
			}
			INKCUIStageViewer component = Object.Instantiate(orLoadAssetResource).GetComponent<INKCUIStageViewer>();
			if (component != null)
			{
				component.ResetPosition(m_trNodeStageParent);
				m_dicStageViewer.Add(num, component);
			}
		}
		if (m_dicStageViewer[num].GetActCount(m_EpisodeTemplet.m_Difficulty) != num2)
		{
			Log.Error($"ACt \ufffd\ufffd\ufffdڰ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffdհ\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd - EpisodeID : {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationNodeViewer.cs", 510);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		foreach (KeyValuePair<int, INKCUIStageViewer> item2 in m_dicStageViewer)
		{
			if (item2.Key != num)
			{
				continue;
			}
			item2.Value.SetActive(bValue: true);
			m_CurStageViewer = item2.Value;
			Vector2 zero = Vector2.zero;
			zero = ((!m_bUseEpSlot) ? item2.Value.SetData(m_bUseEpSlot, num, num3, m_EpisodeTemplet.m_Difficulty, OnSelecteNode, m_EpisodeTemplet.m_ScrollType) : item2.Value.SetData(m_bUseEpSlot, num, stageViewerNodeID, m_EpisodeTemplet.m_Difficulty, OnSelecteNode, m_EpisodeTemplet.m_ScrollType));
			if (!m_EpisodeTemplet.m_bHideActTab && (m_EpisodeTemplet.m_EPCategory != EPISODE_CATEGORY.EC_CHALLENGE || m_EpisodeTemplet.m_DicStage.Count > 1))
			{
				NKCUtil.SetGameobjectActive(m_srAct, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_srAct, bValue: false);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(m_srContent.content);
			if (item2.Value.UseNormalizedPos())
			{
				m_srContent.enabled = true;
				if (m_srContent.GetComponent<NKCUIComScrollRectHotkey>() != null)
				{
					m_srContent.GetComponent<NKCUIComScrollRectHotkey>().enabled = true;
				}
				if (m_srContent.horizontal)
				{
					m_srContent.horizontalNormalizedPosition = zero.x;
				}
				if (m_srContent.vertical)
				{
					m_srContent.verticalNormalizedPosition = zero.y;
				}
			}
			else
			{
				m_srContent.enabled = false;
				if (m_srContent.GetComponent<NKCUIComScrollRectHotkey>() != null)
				{
					m_srContent.GetComponent<NKCUIComScrollRectHotkey>().enabled = false;
				}
				m_srContent.content.transform.localPosition = -zero;
			}
			NKCUtil.SetGameobjectActive(m_btnDifficulty, NKMEpisodeMgr.HasHardDifficulty(m_EpisodeTemplet.m_EpisodeID));
			if (m_btnDifficulty.gameObject.activeSelf)
			{
				if (NKMEpisodeMgr.IsPossibleEpisode(NKCScenManager.CurrentUserData(), m_EpisodeTemplet.m_EpisodeID, (m_EpisodeTemplet.m_Difficulty == EPISODE_DIFFICULTY.NORMAL) ? EPISODE_DIFFICULTY.HARD : EPISODE_DIFFICULTY.NORMAL))
				{
					m_btnDifficulty.UnLock();
				}
				else
				{
					m_btnDifficulty.Lock();
				}
			}
			break;
		}
		SetActInfo();
		int indexPosition = 0;
		for (int i = 0; i < m_lstActSlot.Count; i++)
		{
			indexPosition = i;
		}
		if (m_bFirstSetting)
		{
			m_srAct.SetIndexPosition(indexPosition);
		}
		m_bFirstSetting = false;
	}

	private void SetActInfo()
	{
		NKCScenManager.CurrentUserData();
		NKMStageTempletV2 firstStage = m_EpisodeTemplet.GetFirstStage(m_SelectedActSlotID);
		if (!m_bUseEpSlot && string.IsNullOrEmpty(firstStage.m_ACT_BG_Image))
		{
			Log.Error($"\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd - EpisodeID : {m_EpisodeTemplet.m_EpisodeID}, ActID : {m_SelectedActSlotID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationNodeViewer.cs", 605);
			for (int num = m_SelectedActSlotID - 1; num >= 0; num--)
			{
				firstStage = m_EpisodeTemplet.GetFirstStage(num);
				if (!string.IsNullOrEmpty(firstStage.m_ACT_BG_Image))
				{
					break;
				}
			}
		}
		if (firstStage != null && !string.IsNullOrEmpty(firstStage.m_ACT_BG_Image))
		{
			NKCUtil.SetImageSprite(m_imgBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Bg", firstStage.m_ACT_BG_Image));
		}
		NKCUtil.SetLabelText(m_lbEpTitle, m_EpisodeTemplet.GetEpisodeTitle());
		NKCUtil.SetLabelText(m_lbEpSubTitle, m_EpisodeTemplet.GetEpisodeName());
		if (m_lbEpTitle != null)
		{
			if (m_EpisodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_MAINSTREAM)
			{
				if (m_fontRajhani != null)
				{
					m_lbEpTitle.font = m_fontRajhani;
					m_lbEpTitle.fontSize = m_fontRajhaniSize;
				}
			}
			else if (m_fontNormal != null)
			{
				m_lbEpTitle.font = m_fontNormal;
				m_lbEpTitle.fontSize = m_fontNormalSize;
			}
		}
		NKCUtil.SetGameobjectActive(m_btnMedal, m_EpisodeTemplet.HasCompletionReward);
		if (m_EpisodeTemplet.HasCompletionReward)
		{
			NKCUtil.SetLabelText(m_lbMedalCount, $"{NKMEpisodeMgr.GetEPProgressClearCount(NKCScenManager.CurrentUserData(), m_EpisodeTemplet)}/{NKMEpisodeMgr.GetTotalMedalCount(m_EpisodeTemplet)}");
			bool bValue = NKMEpisodeMgr.CanGetEpisodeCompleteReward(NKCScenManager.CurrentUserData(), m_EpisodeTemplet.m_EpisodeID) == NKM_ERROR_CODE.NEC_OK;
			NKCUtil.SetGameobjectActive(m_objMedalReddot, bValue);
		}
		bool flag = m_EpisodeTemplet.m_ButtonShortCutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE;
		if (flag)
		{
			NKMEventTabTemplet nKMEventTabTemplet = null;
			if (int.TryParse(m_EpisodeTemplet.m_ButtonShortCutParam, out var result))
			{
				nKMEventTabTemplet = NKMEventTabTemplet.Find(result);
			}
			flag &= nKMEventTabTemplet?.IsAvailable ?? false;
		}
		NKCUtil.SetGameobjectActive(m_btnEventMission, flag);
		if (m_btnEventMission != null && m_btnEventMission.gameObject.activeSelf)
		{
			SetEventMissionReddot();
		}
		SetShopButton();
		if (m_btnShop != null && m_btnDifficulty != null && m_srAct != null)
		{
			NKCUtil.SetGameobjectActive(m_objBottom, m_btnShop.gameObject.activeSelf || m_btnDifficulty.gameObject.activeSelf || m_srAct.gameObject.activeSelf);
		}
		UpdateUpsideMenu();
	}

	private void SetShopButton()
	{
		if (!m_bUseEpSlot)
		{
			NKMStageTempletV2 firstStage = m_EpisodeTemplet.GetFirstStage(m_StageViewerNodeID);
			NKCUtil.SetGameobjectActive(m_btnShop, firstStage.m_ShopShortcut != "TAB_NONE");
			if (firstStage.m_ShopShortcut != "TAB_NONE")
			{
				NKCUtil.SetLabelText(m_lbShopCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(firstStage.m_ShopShortcutResourceID).ToString("#,##0"));
				NKCUtil.SetImageSprite(m_imgShopIcon, NKCResourceUtility.GetOrLoadMiscItemIcon(firstStage.m_ShopShortcutResourceID));
				if (!string.IsNullOrEmpty(firstStage.m_ShopShortcutBgName))
				{
					NKCUtil.SetImageSprite(m_imgShopBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("", firstStage.m_ShopShortcutBgName));
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_btnShop, bValue: false);
		}
	}

	private void SetEventMissionReddot()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		int.TryParse(m_EpisodeTemplet.m_ButtonShortCutParam, out var result);
		NKCEventMissionTemplet nKCEventMissionTemplet = NKCEventMissionTemplet.Find(result);
		if (nKCEventMissionTemplet != null)
		{
			bool flag = false;
			for (int i = 0; i < nKCEventMissionTemplet.m_lstMissionTab.Count; i++)
			{
				int num = nKCEventMissionTemplet.m_lstMissionTab[i];
				if (num > 0)
				{
					flag = nKMUserData.m_MissionData.CheckCompletableMission(nKMUserData, num);
					if (flag)
					{
						break;
					}
				}
			}
			NKCUtil.SetGameobjectActive(m_objEventMissionReddot, flag);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objEventMissionReddot, bValue: false);
		}
	}

	public void RefreshFavoriteInfo()
	{
		if (m_StageInfo.gameObject.activeSelf)
		{
			m_StageInfo.RefreshFavoriteState();
		}
	}

	public void Refresh()
	{
		RefreshFavoriteInfo();
		SetActInfo();
		m_srAct.RefreshCells();
		m_CurStageViewer.RefreshData();
		if (m_NKCPopupAchieveRateReward != null)
		{
			NKCPopupAchieveRateReward.ResetUI();
		}
	}

	private void OnClickMedal()
	{
		if (NKCPopupAchieveRateReward != null)
		{
			NKCPopupAchieveRateReward.Open(m_EpisodeTemplet);
		}
	}

	private void OnClickEventMission()
	{
		NKCContentManager.MoveToShortCut(m_EpisodeTemplet.m_ButtonShortCutType, m_EpisodeTemplet.m_ButtonShortCutParam);
	}

	private void OnClickChangeDifficulty()
	{
		EPISODE_DIFFICULTY difficulty = ((m_EpisodeTemplet.m_Difficulty == EPISODE_DIFFICULTY.NORMAL) ? EPISODE_DIFFICULTY.HARD : EPISODE_DIFFICULTY.NORMAL);
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_EpisodeTemplet.m_EpisodeID, difficulty);
		if (m_btnDifficulty.m_bLock)
		{
			NKMStageTempletV2 firstStage = nKMEpisodeTempletV.GetFirstStage(1);
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GetUnlockConditionRequireDesc(firstStage), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (nKMEpisodeTempletV != null)
		{
			SetData(nKMEpisodeTempletV.m_EpisodeID, nKMEpisodeTempletV.m_Difficulty);
		}
	}

	private void OnClickShop()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.LOBBY_SUBMENU))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.LOBBY_SUBMENU);
			return;
		}
		int curActID = m_CurStageViewer.GetCurActID();
		NKMStageTempletV2 firstStage = m_EpisodeTemplet.GetFirstStage(curActID);
		if (firstStage == null || firstStage.m_ShopShortcut == "TAB_NONE" || string.IsNullOrEmpty(firstStage.m_ShopShortcut))
		{
			return;
		}
		string[] array = firstStage.m_ShopShortcut.Split(',', ' ', '@');
		if (array.Length != 0)
		{
			int result = 0;
			int result2 = 0;
			string selectedTab = array[0];
			if (array.Length > 1)
			{
				int.TryParse(array[1], out result);
			}
			if (array.Length > 2)
			{
				int.TryParse(array[2], out result2);
			}
			NKMAssetName cNKMAssetName = new NKMAssetName("AB_UI_OPERATION_Bg", firstStage.m_ACT_BG_Image);
			OperationShop.Open(firstStage.EpisodeTemplet.GetEpisodeTitle(), firstStage.EpisodeTemplet.GetEpisodeName(), firstStage.m_ShopShortcutResourceID, cNKMAssetName, NKCShopManager.ShopTabCategory.NONE, selectedTab, result);
		}
	}

	private void OnSelecteNode(int dunIndex, string dunStrID, bool isPlaying)
	{
		NKMStageTempletV2 nKMStageTempletV = null;
		int num = 0;
		if (!m_EpisodeTemplet.UseEpSlot())
		{
			if (!m_EpisodeTemplet.m_DicStage.ContainsKey(m_StageViewerNodeID) || m_EpisodeTemplet.m_DicStage[m_StageViewerNodeID].Count <= dunIndex - 1)
			{
				return;
			}
			nKMStageTempletV = m_EpisodeTemplet.m_DicStage[m_StageViewerNodeID][dunIndex - 1];
			num = nKMStageTempletV.EpisodeId;
		}
		else
		{
			if (m_EpisodeTemplet.m_DicStage[1].Count <= dunIndex - 1)
			{
				return;
			}
			nKMStageTempletV = m_EpisodeTemplet.m_DicStage[1][dunIndex - 1];
			num = m_EpisodeTemplet.m_GroupID;
		}
		if (nKMStageTempletV != null)
		{
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKMStageTempletV.m_UnlockInfo))
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GetUnlockConditionRequireDesc(nKMStageTempletV), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				return;
			}
			m_dicStageViewer[num].SetSelectNode(nKMStageTempletV);
			m_StageInfo.Open(nKMStageTempletV);
			SetScrollToIndex(dunIndex - 1);
		}
	}

	private void SetScrollToIndex(int targetIndex)
	{
		Vector2 targetPos = m_CurStageViewer.GetTargetPos(targetIndex, m_EpisodeTemplet.m_ScrollType, m_CurStageViewer.UseNormalizedPos());
		if (m_CurStageViewer.UseNormalizedPos())
		{
			m_srContent.enabled = true;
			if (m_srContent.GetComponent<NKCUIComScrollRectHotkey>() != null)
			{
				m_srContent.GetComponent<NKCUIComScrollRectHotkey>().enabled = true;
			}
			m_srContent.DOKill();
			if (m_EpisodeTemplet.m_ScrollType == EPISODE_SCROLL_TYPE.HORIZONTAL)
			{
				m_srContent.DOHorizontalNormalizedPos(targetPos.x, 0.2f).SetEase(Ease.OutQuint);
			}
			else if (m_EpisodeTemplet.m_ScrollType == EPISODE_SCROLL_TYPE.VERTICAL)
			{
				m_srContent.DOVerticalNormalizedPos(targetPos.x, 0.2f).SetEase(Ease.OutQuint);
			}
		}
		else
		{
			m_srContent.enabled = false;
			if (m_srContent.GetComponent<NKCUIComScrollRectHotkey>() != null)
			{
				m_srContent.GetComponent<NKCUIComScrollRectHotkey>().enabled = false;
			}
			m_srContent.content.transform.DOKill();
			m_srContent.content.transform.DOLocalMove(targetPos, 0.2f);
		}
	}

	private void OnClickStart(NKMStageTempletV2 stageTemplet, bool bSkip, int skipCount)
	{
		if (stageTemplet == null)
		{
			return;
		}
		NKMEpisodeTempletV2 episodeTemplet = stageTemplet.EpisodeTemplet;
		if (episodeTemplet == null)
		{
			return;
		}
		if (!episodeTemplet.IsOpen)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EXCEPTION_EVENT_EXPIRED_POPUP, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCUtil.CheckCommonStartCond(myUserData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCUtil.OnExpandInventoryPopup(nKM_ERROR_CODE);
			return;
		}
		if (bSkip)
		{
			if (stageTemplet.DungeonTempletBase == null)
			{
				return;
			}
			if (!myUserData.CheckStageCleared(stageTemplet))
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_STAGE"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				return;
			}
			List<long> unitList = new List<long>();
			if (stageTemplet.IsUsingEventDeck())
			{
				NKMEventDeckData nKMEventDeckData = NKMDungeonManager.LoadDungeonDeck(stageTemplet);
				if (nKMEventDeckData == null)
				{
					NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_DP_DIVE_NO_SELECT_DECK"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
					return;
				}
				unitList.AddRange(nKMEventDeckData.m_dicUnit.Values);
			}
			else
			{
				NKM_ERROR_CODE nKM_ERROR_CODE2 = NKMMain.IsValidDeck(selectDeckIndex: new NKMDeckIndex(NKM_DECK_TYPE.NDT_DAILY, 0), cNKMArmyData: myUserData.m_ArmyData);
				if (nKM_ERROR_CODE2 != NKM_ERROR_CODE.NEC_OK)
				{
					NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString(nKM_ERROR_CODE2), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
					return;
				}
				myUserData.m_ArmyData.GetDeckList(NKM_DECK_TYPE.NDT_DAILY, 0, ref unitList);
			}
			NKCPacketSender.Send_NKMPacket_DUNGEON_SKIP_REQ(stageTemplet.DungeonTempletBase.m_DungeonID, unitList, skipCount);
			return;
		}
		if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY)
		{
			bool flag = true;
			if (stageTemplet.m_STAGE_TYPE == STAGE_TYPE.ST_WARFARE)
			{
				WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
				if (warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP && warfareGameData.warfareTempletID == stageTemplet.WarfareTemplet.Key)
				{
					flag = false;
				}
			}
			if (flag && stageTemplet.m_StageReqItemCount - myUserData.m_InventoryData.GetCountMiscItem(stageTemplet.m_StageReqItemID) > 0)
			{
				int dailyMissionTicketShopID = NKCShopManager.GetDailyMissionTicketShopID(m_EpisodeTemplet.m_EpisodeID);
				if (NKCShopManager.GetBuyCountLeft(dailyMissionTicketShopID) > 0)
				{
					NKCShopManager.OnBtnProductBuy(ShopItemTemplet.Find(dailyMissionTicketShopID).Key, bSupply: false);
				}
				else
				{
					NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_ENTER_LIMIT_OVER);
				}
				return;
			}
		}
		if (stageTemplet.EnterLimit > 0)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData.IsHaveStatePlayData(stageTemplet.Key) && nKMUserData.GetStatePlayCnt(stageTemplet.Key) >= stageTemplet.EnterLimit)
			{
				if (nKMUserData.GetStageRestoreCnt(stageTemplet.Key) >= stageTemplet.RestoreLimit)
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ENTER_LIMIT_OVER);
					return;
				}
				NKCPopupResourceWithdraw.Instance.OpenForRestoreEnterLimit(stageTemplet, delegate
				{
					NKCPacketSender.Send_NKMPacket_RESET_STAGE_PLAY_COUNT_REQ(stageTemplet.Key);
				}, nKMUserData.GetStageRestoreCnt(stageTemplet.Key));
				return;
			}
		}
		if (!NKMEpisodeMgr.HasEnoughResource(stageTemplet))
		{
			return;
		}
		m_StageInfo.Close();
		switch (stageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
		{
			NKMDungeonTempletBase cNKMDungeonTempletBase = stageTemplet.DungeonTempletBase;
			if (cNKMDungeonTempletBase == null)
			{
				break;
			}
			NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(stageTemplet);
			NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedCutscenStage(stageTemplet);
			if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_MAINSTREAM)
			{
				NKCUIOperationIntro.Instance.Open(stageTemplet, delegate
				{
					NKCScenManager.GetScenManager().ScenChangeFade(Get_Next_NKM_SCEN_ID_By_DT(cNKMDungeonTempletBase.m_DungeonType));
				});
			}
			else
			{
				NKCScenManager.GetScenManager().ScenChangeFade(Get_Next_NKM_SCEN_ID_By_DT(cNKMDungeonTempletBase.m_DungeonType));
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
			NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(stageTemplet, DeckContents.PHASE);
			if (stageTemplet.PhaseTemplet == null)
			{
				break;
			}
			if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_MAINSTREAM)
			{
				NKCUIOperationIntro.Instance.Open(stageTemplet, delegate
				{
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
				});
			}
			else
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
			}
			break;
		}
	}

	public void OnPointDown(BaseEventData eventData)
	{
		m_vDownPos = ((PointerEventData)eventData).position;
	}

	private void OnPointUp(BaseEventData eventData)
	{
		if (Vector2.Distance(m_vDownPos, ((PointerEventData)eventData).position) < m_fDragCheckDistance)
		{
			if (m_CurStageViewer != null)
			{
				m_CurStageViewer.SetSelectNode(null);
			}
			if (m_StageInfo.gameObject.activeSelf)
			{
				m_StageInfo.Close();
			}
			if (!m_srContent.enabled)
			{
				m_srContent.enabled = true;
			}
			if (m_srContent.GetComponent<NKCUIComScrollRectHotkey>() != null)
			{
				m_srContent.GetComponent<NKCUIComScrollRectHotkey>().enabled = true;
			}
		}
	}

	private void OnPointClick(BaseEventData eventData)
	{
		if (m_CurStageViewer != null)
		{
			m_CurStageViewer.SetSelectNode(null);
		}
		if (m_StageInfo.gameObject.activeSelf)
		{
			m_StageInfo.Close();
		}
		if (!m_srContent.enabled)
		{
			m_srContent.enabled = true;
		}
		if (m_srContent.GetComponent<NKCUIComScrollRectHotkey>() != null)
		{
			m_srContent.GetComponent<NKCUIComScrollRectHotkey>().enabled = true;
		}
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		return hotkey switch
		{
			HotkeyEventType.Down => MoveTab(1), 
			HotkeyEventType.Up => MoveTab(-1), 
			_ => false, 
		};
	}

	private bool MoveTab(int moveCount)
	{
		int num = m_lstStageViewerID.FindIndex((int x) => x == m_StageViewerNodeID);
		if (num >= 0)
		{
			int index = (num + moveCount + m_lstStageViewerID.Count) % m_lstStageViewerID.Count;
			for (int num2 = 0; num2 < m_lstActSlot.Count; num2++)
			{
				if (!(m_lstActSlot[num2] != null))
				{
					continue;
				}
				bool flag = m_lstActSlot[num2].GetStageViewerID() == m_lstStageViewerID[index];
				if (flag && m_lstActSlot[num2].IsLocked())
				{
					if (moveCount > 0)
					{
						MoveTab(moveCount + 1);
					}
					else
					{
						MoveTab(moveCount - 1);
					}
					return false;
				}
				m_lstActSlot[num2].SetSelected(flag);
			}
			OnSelectedActSlot(m_lstStageViewerID[index]);
			return true;
		}
		return false;
	}

	private NKM_SCEN_ID Get_Next_NKM_SCEN_ID_By_DT(NKM_DUNGEON_TYPE eNKM_DUNGEON_TYPE)
	{
		if (eNKM_DUNGEON_TYPE == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
		{
			return NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON;
		}
		return NKM_SCEN_ID.NSI_DUNGEON_ATK_READY;
	}

	private string GetActName(NKMEpisodeTempletV2 epTemplet, int idx)
	{
		if (!epTemplet.UseEpSlot())
		{
			if (m_lstStageViewerID.Count > idx && epTemplet.m_DicStage.ContainsKey(m_lstStageViewerID[idx]) && epTemplet.m_DicStage[m_lstStageViewerID[idx]].Count > 0)
			{
				return string.Format("{0} {1}", NKCStringTable.GetString("SI_PF_ACT"), m_lstStageViewerID[idx]);
			}
			return "";
		}
		return epTemplet.GetEpisodeName();
	}

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Episode);
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		base.OnInventoryChange(itemData);
		if (m_StageInfo.IsOpened())
		{
			m_StageInfo.RefreshUI();
		}
		SetShopButton();
	}
}
