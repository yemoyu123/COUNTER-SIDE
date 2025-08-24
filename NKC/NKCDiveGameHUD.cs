using System;
using System.Collections.Generic;
using Cs.Logging;
using DG.Tweening;
using NKC.UI;
using NKC.UI.Guide;
using NKC.UI.Warfare;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCDiveGameHUD : MonoBehaviour
{
	[Header("상단바")]
	public NKCUIComButton m_NKM_UI_DIVE_PAUSE_BUTTON;

	public Text m_NKM_UI_DIVE_TITLE_TEXT;

	public Text m_NKM_UI_DIVE_SUBTITLE_TEXT_CONTENT;

	public Text m_NKM_UI_DIVE_LEFT_COUNT;

	public NKCUIComToggle m_NKM_UI_DIVE_AUTO;

	public Text m_NKM_UI_DIVE_PAUSE_GAME_TIME_Text;

	public NKCUIComStateButton m_NKM_UI_DIVE_HELP_BUTTON;

	private float m_fPrevUpdateTime;

	[Header("좌측 부대정보 리스트")]
	public NKCUIComToggle m_NKM_UI_DIVE_SQAUD_INFO;

	public GameObject m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SCROLL;

	public GameObject m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_CONTENT;

	public GameObject m_BG_NORMAL;

	public GameObject m_BG_HURDLE;

	private List<NKCDiveGameSquadSlot> m_lstNKCDiveGameSquadSlot = new List<NKCDiveGameSquadSlot>();

	[Header("가운데 부대정보")]
	public GameObject m_NKM_UI_DIVE_PROCESS_SQUAD_VIEW;

	public GameObject m_NKM_UI_DIVE_SQUAD_UNIT_GRID;

	public NKCUIShipInfoSummary m_UIShipInfo;

	public Image m_ANIM_SHIP_IMG;

	public Text m_NKM_UI_DIVE_SQUAD_TITLE_TEXT1;

	public Text m_NKM_UI_DIVE_SQUAD_TITLE_TEXT2;

	private List<NKCDeckViewUnitSlot> m_lstDeckViewUnitSlot = new List<NKCDeckViewUnitSlot>();

	private NKMDeckIndex m_LastSelectedDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, 0);

	[Header("우측 섹터 정보")]
	public GameObject m_NKM_UI_DIVE_PROCESS_INFO;

	public CanvasGroup m_NKM_UI_DIVE_PROCESS_INFO_CG;

	public RawImage m_NKM_UI_DIVE_PROCESS_SECTOR_THUMBNAIL;

	public Image m_NKM_UI_DIVE_PROCESS_SECTOR_ICON;

	public Text m_NKM_UI_DIVE_PROCESS_SECTOR_TITLE;

	public Text m_NKM_UI_DIVE_PROCESS_SECTOR_SUBTITLE;

	public GameObject m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_TEXT_DECO;

	public Text m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_TEXT;

	public Text m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_DESC;

	public GameObject m_NKM_UI_DIVE_PROCESS_SECTOR_BIGSECTOR_TEXT;

	public NKCUIComStateButton m_NKM_UI_DIVE_PROCESS_RESEARCH_BUTTON;

	public NKCUIComStateButton m_NKM_UI_DIVE_PROCESS_BATTLE_BUTTON;

	public CanvasGroup m_NKM_UI_DIVE_PROCESS_BATTLE_BUTTON_CG;

	public GameObject m_DiveProcessBattleReqItem;

	public Image m_DiveProcessBattleReqItemIcon;

	public Text m_DiveProcessBattleReqItemCountText;

	[Header("아티팩트")]
	public NKCDiveGameHUDArtifact m_NKCDiveGameHUDArtifact;

	[Header("Etc")]
	public Animator m_NKM_UI_DIVE_INTRO_FX;

	public NKCUIComStateButton m_csbtnEnemyList;

	[Header("오퍼레이터")]
	public GameObject m_OPERATOR_INFO;

	public NKCUIOperatorDeckSlot m_NKM_UI_OPERATOR_DECK_SLOT;

	private NKCPopupWarfareSelectShip m_NKCPopupWarfareSelectShip;

	private int m_dungeonId;

	private bool m_isBossSector;

	private NKCPopupWarfareSelectShip NKCPopupWarfareSelectShip
	{
		get
		{
			if (m_NKCPopupWarfareSelectShip == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupWarfareSelectShip>("AB_UI_NKM_UI_POPUP_WARFARE_SELECT", "NKM_UI_POPUP_WARFARE_SELECT_SHIP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCPopupWarfareSelectShip = loadedUIData.GetInstance<NKCPopupWarfareSelectShip>();
				m_NKCPopupWarfareSelectShip.InitUI();
			}
			return m_NKCPopupWarfareSelectShip;
		}
	}

	public static NKCDiveGameHUD InitUI(NKCDiveGame cNKCDiveGame)
	{
		NKCDiveGameHUD nKCDiveGameHUD = NKCUIManager.OpenUI<NKCDiveGameHUD>("NKM_UI_DIVE_PROCESS_2D");
		nKCDiveGameHUD.m_NKM_UI_DIVE_PAUSE_BUTTON.PointerClick.RemoveAllListeners();
		nKCDiveGameHUD.m_NKM_UI_DIVE_PAUSE_BUTTON.PointerClick.AddListener(cNKCDiveGame.OnBackButton);
		nKCDiveGameHUD.m_NKM_UI_DIVE_AUTO.OnValueChanged.RemoveAllListeners();
		nKCDiveGameHUD.m_NKM_UI_DIVE_AUTO.OnValueChanged.AddListener(cNKCDiveGame.OnChangedAuto);
		nKCDiveGameHUD.m_NKM_UI_DIVE_SQAUD_INFO.OnValueChanged.RemoveAllListeners();
		nKCDiveGameHUD.m_NKM_UI_DIVE_SQAUD_INFO.OnValueChanged.AddListener(nKCDiveGameHUD.OnChangedSquadList);
		nKCDiveGameHUD.m_NKM_UI_DIVE_PROCESS_RESEARCH_BUTTON.PointerClick.RemoveAllListeners();
		nKCDiveGameHUD.m_NKM_UI_DIVE_PROCESS_RESEARCH_BUTTON.PointerClick.AddListener(cNKCDiveGame.OnClickSectorInfoSearch);
		nKCDiveGameHUD.m_NKM_UI_DIVE_PROCESS_BATTLE_BUTTON.PointerClick.RemoveAllListeners();
		nKCDiveGameHUD.m_NKM_UI_DIVE_PROCESS_BATTLE_BUTTON.PointerClick.AddListener(cNKCDiveGame.OnClickBattle);
		nKCDiveGameHUD.m_NKM_UI_DIVE_HELP_BUTTON.PointerClick.RemoveAllListeners();
		nKCDiveGameHUD.m_NKM_UI_DIVE_HELP_BUTTON.PointerClick.AddListener(nKCDiveGameHUD.OnClickHelpBtn);
		NKCUtil.SetGameobjectActive(nKCDiveGameHUD.m_NKM_UI_DIVE_SQAUD_INFO, bValue: false);
		NKCUtil.SetGameobjectActive(nKCDiveGameHUD.m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SCROLL, bValue: true);
		nKCDiveGameHUD.SetDeckViewUnitSlotCount(8);
		NKCUtil.SetGameobjectActive(nKCDiveGameHUD.gameObject, bValue: false);
		nKCDiveGameHUD.m_NKCDiveGameHUDArtifact.InitUI(cNKCDiveGame.OnFinishScrollToArtifactDummySlot);
		NKCUtil.SetButtonClickDelegate(nKCDiveGameHUD.m_csbtnEnemyList, nKCDiveGameHUD.OnClickEnemyList);
		NKCUtil.SetGameobjectActive(nKCDiveGameHUD.m_DiveProcessBattleReqItem, bValue: false);
		NKCUtil.SetGameobjectActive(nKCDiveGameHUD.m_DiveProcessBattleReqItemIcon, bValue: false);
		NKCUtil.SetGameobjectActive(nKCDiveGameHUD.m_DiveProcessBattleReqItemCountText, bValue: false);
		return nKCDiveGameHUD;
	}

	public void UpdateBattleCost(NKMDiveTemplet cNKMDiveTemplet, NKMDiveSlot cNKMDiveSlot)
	{
		NKCUtil.SetGameobjectActive(m_DiveProcessBattleReqItem, bValue: false);
		NKCUtil.SetGameobjectActive(m_DiveProcessBattleReqItemCountText, bValue: false);
		NKCUtil.SetGameobjectActive(m_DiveProcessBattleReqItemIcon, bValue: false);
		if (cNKMDiveTemplet == null)
		{
			Log.Error("[DiveGameHud] InitUI - DiveTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCDiveGameHUD.cs", 148);
		}
		else if (cNKMDiveSlot != null && cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_DUNGEON_BOSS)
		{
			NKCScenManager.CurrentUserData();
			int bossStageReqItemCount = cNKMDiveTemplet.BossStageReqItemCount;
			if (bossStageReqItemCount != 0)
			{
				Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(cNKMDiveTemplet.BossStageReqItemID);
				NKCUtil.SetImageSprite(m_DiveProcessBattleReqItemIcon, orLoadMiscItemSmallIcon);
				NKCUtil.SetLabelText(m_DiveProcessBattleReqItemCountText, bossStageReqItemCount.ToString());
				NKCUtil.SetGameobjectActive(m_DiveProcessBattleReqItem, bValue: true);
				NKCUtil.SetGameobjectActive(m_DiveProcessBattleReqItemCountText, bValue: true);
				NKCUtil.SetGameobjectActive(m_DiveProcessBattleReqItemIcon, bValue: true);
			}
		}
	}

	private void Update()
	{
		if (m_fPrevUpdateTime < Time.time + 1f)
		{
			m_fPrevUpdateTime = Time.time;
			NKMDiveGameData diveGameData = GetDiveGameData();
			if (diveGameData != null)
			{
				DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
				TimeSpan timeSpan = new DateTime(diveGameData.Floor.ExpireDate) - serverUTCTime;
				m_NKM_UI_DIVE_PAUSE_GAME_TIME_Text.text = NKCUtilString.GetTimeSpanString(timeSpan);
			}
		}
	}

	private void OnClickHelpBtn()
	{
		NKCUIPopUpGuide.Instance.Open("ARTICLE_DIVE_SEARCH");
	}

	public void PlayIntro()
	{
		if (m_NKM_UI_DIVE_INTRO_FX != null)
		{
			m_NKM_UI_DIVE_INTRO_FX.Play("NKM_UI_DIVE_INTRO_FX_BASE");
		}
	}

	private void SetDeckViewUnitSlotCount(int count)
	{
		while (m_lstDeckViewUnitSlot.Count < count)
		{
			NKCDeckViewUnitSlot newInstance = NKCDeckViewUnitSlot.GetNewInstance(m_NKM_UI_DIVE_SQUAD_UNIT_GRID.transform);
			newInstance.Init(m_lstDeckViewUnitSlot.Count, bEnableDrag: false);
			newInstance.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			m_lstDeckViewUnitSlot.Add(newInstance);
			NKCUtil.SetGameobjectActive(newInstance, bValue: true);
		}
	}

	public void OpenSquadList()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SCROLL, bValue: true);
		m_NKM_UI_DIVE_SQAUD_INFO.Select(bSelect: true, bForce: true);
	}

	private void OnChangedSquadList(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_SCROLL, bSet);
	}

	public NKMDeckIndex GetLastSelectedDeckIndex()
	{
		return m_LastSelectedDeckIndex;
	}

	public void SetSelectedSquadSlot(int deckIndexToSelect)
	{
		for (int i = 0; i < m_lstNKCDiveGameSquadSlot.Count; i++)
		{
			if (!(m_lstNKCDiveGameSquadSlot[i] == null))
			{
				if (m_lstNKCDiveGameSquadSlot[i].GetDeckIndex() == deckIndexToSelect)
				{
					m_lstNKCDiveGameSquadSlot[i].SetSelected(bSet: true);
				}
				else
				{
					m_lstNKCDiveGameSquadSlot[i].SetSelected(bSet: false);
				}
			}
		}
	}

	public void OpenSquadView(int deckIndex)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		SetSelectedSquadSlot(deckIndex);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_VIEW, bValue: true);
		NKCCamera.EnableBlur(bEnable: true);
		int num = 0;
		NKMDeckIndex nKMDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, deckIndex);
		NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(nKMDeckIndex);
		if (deckData == null)
		{
			return;
		}
		m_LastSelectedDeckIndex = nKMDeckIndex;
		for (num = 0; num < m_lstDeckViewUnitSlot.Count; num++)
		{
			if (num >= 0 && num < 8)
			{
				m_lstDeckViewUnitSlot[num].SetData(myUserData.m_ArmyData.GetUnitFromUID(deckData.m_listDeckUnitUID[num]), bEnableButton: false);
				if (num == deckData.m_LeaderIndex)
				{
					m_lstDeckViewUnitSlot[num].SetLeader(bLeader: true, bEffect: false);
				}
			}
		}
		NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
		NKMUnitTempletBase nKMUnitTempletBase = null;
		if (shipFromUID != null)
		{
			nKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
		}
		if (nKMUnitTempletBase != null)
		{
			m_NKM_UI_DIVE_SQUAD_TITLE_TEXT1.text = string.Format(NKCUtilString.GET_STRING_SQUAD_ONE_PARAM, NKCUtilString.GetDeckNumberString(nKMDeckIndex));
			int num2 = nKMDeckIndex.m_iIndex + 1;
			m_NKM_UI_DIVE_SQUAD_TITLE_TEXT2.text = string.Format(NKCUtilString.GET_STRING_SQUAD_TWO_PARAM, num2, NKCUtilString.GetRankNumber(num2).ToUpper());
			Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, nKMUnitTempletBase);
			Sprite sprite2 = null;
			if (sprite != null)
			{
				sprite2 = sprite;
			}
			if (sprite2 == null)
			{
				NKCAssetResourceData assetResourceUnitInvenIconEmpty = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
				if (assetResourceUnitInvenIconEmpty != null)
				{
					m_ANIM_SHIP_IMG.sprite = assetResourceUnitInvenIconEmpty.GetAsset<Sprite>();
				}
				else
				{
					m_ANIM_SHIP_IMG.sprite = null;
				}
			}
			else
			{
				m_ANIM_SHIP_IMG.sprite = sprite2;
			}
			m_UIShipInfo.SetShipData(shipFromUID, nKMUnitTempletBase, nKMDeckIndex);
		}
		if (!NKCOperatorUtil.IsHide())
		{
			if (deckData.m_OperatorUID != 0L)
			{
				m_NKM_UI_OPERATOR_DECK_SLOT.SetData(NKCOperatorUtil.GetOperatorData(deckData.m_OperatorUID));
			}
			else
			{
				m_NKM_UI_OPERATOR_DECK_SLOT.SetEmpty();
			}
		}
		NKCUtil.SetGameobjectActive(m_OPERATOR_INFO, !NKCOperatorUtil.IsHide());
	}

	public bool IsOpenSquadView()
	{
		return m_NKM_UI_DIVE_PROCESS_SQUAD_VIEW.activeSelf;
	}

	public void CloseSquadView()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SQUAD_VIEW, bValue: false);
		NKCCamera.EnableBlur(bEnable: false);
	}

	public void UpdateSectorInfoUI(NKMDiveTemplet cNKMDiveTemplet, NKMDiveSlot cNKMDiveSlot, bool bSameCol)
	{
		if (!m_NKM_UI_DIVE_PROCESS_INFO.activeSelf)
		{
			return;
		}
		if (NKCDiveManager.IsReimannSectorType(cNKMDiveSlot.SectorType))
		{
			m_NKM_UI_DIVE_PROCESS_SECTOR_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SECTOR_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_SECTOR_REIMANN");
			m_NKM_UI_DIVE_PROCESS_SECTOR_ICON.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SPRITE", "AB_UI_DIVE_ICON_SECTOR_MAP_REIMANN_B");
			m_NKM_UI_DIVE_PROCESS_SECTOR_TITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SECTOR_TITLE_BATTLE");
			m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_DESC.text = "";
		}
		else if (NKCDiveManager.IsBossSectorType(cNKMDiveSlot.SectorType))
		{
			m_NKM_UI_DIVE_PROCESS_SECTOR_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SECTOR_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_SECTOR_BOSS");
			m_NKM_UI_DIVE_PROCESS_SECTOR_ICON.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SPRITE", "AB_UI_DIVE_ICON_SECTOR_BOSS");
			m_NKM_UI_DIVE_PROCESS_SECTOR_TITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SECTOR_TITLE_CORE");
			m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_DESC.text = "";
		}
		else if (NKCDiveManager.IsPoincareSectorType(cNKMDiveSlot.SectorType))
		{
			m_NKM_UI_DIVE_PROCESS_SECTOR_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SECTOR_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_SECTOR_POINCARE");
			m_NKM_UI_DIVE_PROCESS_SECTOR_ICON.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SPRITE", "AB_UI_DIVE_ICON_SECTOR_MAP_POINCARE_A");
			m_NKM_UI_DIVE_PROCESS_SECTOR_TITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SECTOR_TITLE_BATTLE");
			m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_DESC.text = "";
		}
		else if (NKCDiveManager.IsGauntletSectorType(cNKMDiveSlot.SectorType))
		{
			m_NKM_UI_DIVE_PROCESS_SECTOR_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SECTOR_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_SECTOR_GAUNTLET");
			m_NKM_UI_DIVE_PROCESS_SECTOR_ICON.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SPRITE", "AB_UI_DIVE_ICON_SECTOR_MAP_GAUNTLET_C");
			m_NKM_UI_DIVE_PROCESS_SECTOR_TITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SECTOR_TITLE_BATTLE");
			m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_DESC.text = "";
		}
		else if (NKCDiveManager.IsEuclidSectorType(cNKMDiveSlot.SectorType))
		{
			m_NKM_UI_DIVE_PROCESS_SECTOR_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SECTOR_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_SECTOR_EUCLID");
			switch (cNKMDiveSlot.EventType)
			{
			case NKM_DIVE_EVENT_TYPE.NDET_ARTIFACT:
				m_NKM_UI_DIVE_PROCESS_SECTOR_ICON.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SPRITE", "AB_UI_DIVE_ICON_SECTOR_MAP_ARTIFACT");
				break;
			case NKM_DIVE_EVENT_TYPE.NDET_REPAIR:
				m_NKM_UI_DIVE_PROCESS_SECTOR_ICON.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SPRITE", "AB_UI_DIVE_ICON_SECTOR_MAP_EUCLID_REPAIR");
				break;
			default:
				m_NKM_UI_DIVE_PROCESS_SECTOR_ICON.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_DIVE_SPRITE", "AB_UI_DIVE_ICON_SECTOR_EUCLID");
				break;
			}
			m_NKM_UI_DIVE_PROCESS_SECTOR_TITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SECTOR_TITLE_SAFE");
			m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_DESC.text = "";
		}
		if (!NKCDiveManager.IsEuclidSectorType(cNKMDiveSlot.SectorType))
		{
			if (GetDiveGameData() != null && GetDiveGameData().Floor != null && GetDiveGameData().Floor.Templet != null)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_TEXT_DECO, bValue: true);
				m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_TEXT.text = string.Format(NKCUtilString.GET_STRING_DIVE_FLOOR_LEVEL_ONE_PARAM, GetDiveGameData().Floor.Templet.StageLevel);
			}
			m_NKM_UI_DIVE_PROCESS_SECTOR_SUBTITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SUBTITLE_DUNGEON");
		}
		else
		{
			if (NKCDiveManager.IsItemEventType(cNKMDiveSlot.EventType))
			{
				m_NKM_UI_DIVE_PROCESS_SECTOR_SUBTITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SUBTITLE_ITEM_SIGNAL");
			}
			else if (NKCDiveManager.IsLostContainerEventType(cNKMDiveSlot.EventType))
			{
				m_NKM_UI_DIVE_PROCESS_SECTOR_SUBTITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SUBTITLE_SUPPLY");
			}
			else if (NKCDiveManager.IsRandomEventType(cNKMDiveSlot.EventType))
			{
				m_NKM_UI_DIVE_PROCESS_SECTOR_SUBTITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SUBTITLE_NO_SIGNAL");
			}
			else if (NKCDiveManager.IsRescueSignalEventType(cNKMDiveSlot.EventType))
			{
				m_NKM_UI_DIVE_PROCESS_SECTOR_SUBTITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SUBTITLE_HELP_SIGNAL");
			}
			else if (NKCDiveManager.IsLostShipEventType(cNKMDiveSlot.EventType))
			{
				m_NKM_UI_DIVE_PROCESS_SECTOR_SUBTITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SUBTITLE_LOSTSHIP");
			}
			else if (NKCDiveManager.IsSafetyEventType(cNKMDiveSlot.EventType))
			{
				m_NKM_UI_DIVE_PROCESS_SECTOR_SUBTITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SUBTITLE_SAFE");
			}
			else if (NKCDiveManager.IsRepairKitEventType(cNKMDiveSlot.EventType))
			{
				m_NKM_UI_DIVE_PROCESS_SECTOR_SUBTITLE.text = NKCStringTable.GetString("SI_DIVE_EVENT_SUBTITLE_REPAIR_KIT");
			}
			else if (NKCDiveManager.IsArtifactEventType(cNKMDiveSlot.EventType))
			{
				m_NKM_UI_DIVE_PROCESS_SECTOR_SUBTITLE.text = NKCStringTable.GetString("SI_DP_DIVE_EVENT_TEXT_NDET_BLANK_TITLE");
			}
			m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_TEXT.text = "";
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SECTOR_INFO_TEXT_DECO, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_SECTOR_BIGSECTOR_TEXT, NKCDiveManager.IsSectorHardType(cNKMDiveSlot.SectorType));
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_RESEARCH_BUTTON, !bSameCol);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_BATTLE_BUTTON, bSameCol);
		if (bSameCol)
		{
			m_NKM_UI_DIVE_PROCESS_BATTLE_BUTTON_CG.alpha = 1f;
		}
		UpdateBattleCost(cNKMDiveTemplet, cNKMDiveSlot);
	}

	public bool OpenSectorInfo(NKMDiveTemplet cNKMDiveTemplet, NKMDiveSlot cNKMDiveSlot, bool bSameCol)
	{
		if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_NONE)
		{
			return false;
		}
		bool flag = false;
		if (!m_NKM_UI_DIVE_PROCESS_INFO.activeSelf)
		{
			flag = true;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_INFO, bValue: true);
		if (flag)
		{
			m_NKM_UI_DIVE_PROCESS_INFO_CG.alpha = 0f;
			m_NKM_UI_DIVE_PROCESS_INFO_CG.DOFade(1f, 0.6f);
		}
		else
		{
			m_NKM_UI_DIVE_PROCESS_INFO_CG.DOKill();
			m_NKM_UI_DIVE_PROCESS_INFO_CG.alpha = 1f;
		}
		UpdateSectorInfoUI(cNKMDiveTemplet, cNKMDiveSlot, bSameCol);
		m_dungeonId = 0;
		if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_DUNGEON || cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_DUNGEON_BOSS)
		{
			m_dungeonId = cNKMDiveSlot.EventValue;
		}
		m_isBossSector = NKCDiveManager.IsBossSectorType(cNKMDiveSlot.SectorType);
		NKCUtil.SetGameobjectActive(m_csbtnEnemyList, m_dungeonId > 0);
		return true;
	}

	public void SetSectorInfoBottomButtonToBattle()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_RESEARCH_BUTTON, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_BATTLE_BUTTON, bValue: true);
	}

	public void CloseSectorInfo()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_INFO, bValue: false);
		m_dungeonId = 0;
		m_isBossSector = false;
	}

	private NKMDiveGameData GetDiveGameData()
	{
		return NKCScenManager.GetScenManager().GetMyUserData().m_DiveGameData;
	}

	private void OnClickSquadSlot(NKMDiveSquad cNKMDiveSquad)
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData == null)
		{
			return;
		}
		if (diveGameData.Player.PlayerBase.State == NKMDivePlayerState.BattleReady)
		{
			if (!(cNKMDiveSquad.CurHp <= 0f))
			{
				OpenSquadView(cNKMDiveSquad.DeckIndex);
			}
		}
		else
		{
			NKCPopupWarfareSelectShip.OpenForMyShipInDive(new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, cNKMDiveSquad.DeckIndex));
		}
	}

	public void UpdateSquadListUI()
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData == null || diveGameData.Player == null || diveGameData.Player.Squads == null)
		{
			return;
		}
		int num = 0;
		if (diveGameData.Player.Squads.Count > m_lstNKCDiveGameSquadSlot.Count)
		{
			int num2 = diveGameData.Player.Squads.Count - m_lstNKCDiveGameSquadSlot.Count;
			for (num = 0; num < num2; num++)
			{
				NKCDiveGameSquadSlot newInstance = NKCDiveGameSquadSlot.GetNewInstance(m_NKM_UI_DIVE_PROCESS_SQUAD_LIST_CONTENT.transform, OnClickSquadSlot);
				m_lstNKCDiveGameSquadSlot.Add(newInstance);
			}
		}
		List<NKMDiveSquad> list = new List<NKMDiveSquad>(diveGameData.Player.Squads.Values);
		for (num = 0; num < m_lstNKCDiveGameSquadSlot.Count; num++)
		{
			if (num < diveGameData.Player.Squads.Count)
			{
				m_lstNKCDiveGameSquadSlot[num].SetUI(list[num]);
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSquadSlot[num], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstNKCDiveGameSquadSlot[num], bValue: false);
			}
		}
	}

	public void UpdateExploreCountLeftUI()
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData != null && diveGameData.Floor.Templet != null)
		{
			SetLeftExploreCount(diveGameData.Floor.Templet.RandomSetCount + 1 - diveGameData.Player.PlayerBase.Distance);
		}
	}

	private void SetSquadListBG(bool bHurdle = false)
	{
		NKCUtil.SetGameobjectActive(m_BG_NORMAL, !bHurdle);
		NKCUtil.SetGameobjectActive(m_BG_HURDLE, bHurdle);
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		CloseSectorInfo();
		CloseSquadView();
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData != null)
		{
			if (diveGameData.Floor.Templet != null)
			{
				SetTitle(diveGameData.Floor.Templet.Get_STAGE_NAME());
				SetSubTitle(diveGameData.Floor.Templet.Get_STAGE_NAME_SUB());
				SetSquadListBG(diveGameData.Floor.Templet.StageType == NKM_DIVE_STAGE_TYPE.NDST_HARD);
			}
			UpdateSquadListUI();
		}
		UpdateExploreCountLeftUI();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKMUserOption userOption = myUserData.m_UserOption;
			if (userOption != null)
			{
				m_NKM_UI_DIVE_AUTO.Select(userOption.m_bAutoDive, bForce: true);
			}
		}
		m_NKCDiveGameHUDArtifact.ResetUI(diveGameData.Floor.Templet.StageType == NKM_DIVE_STAGE_TYPE.NDST_HARD);
		m_NKCDiveGameHUDArtifact.Close(bAnimate: false);
	}

	public void SetTitle(string str)
	{
		if (m_NKM_UI_DIVE_TITLE_TEXT != null)
		{
			m_NKM_UI_DIVE_TITLE_TEXT.text = str;
		}
	}

	public void SetSubTitle(string str)
	{
		if (m_NKM_UI_DIVE_SUBTITLE_TEXT_CONTENT != null)
		{
			m_NKM_UI_DIVE_SUBTITLE_TEXT_CONTENT.text = str;
		}
	}

	public void SetLeftExploreCount(int leftCount)
	{
		if (m_NKM_UI_DIVE_LEFT_COUNT != null)
		{
			m_NKM_UI_DIVE_LEFT_COUNT.text = string.Format(NKCUtilString.GET_STRING_DIVE_LEFT_COUNT_ONE_PARAM, leftCount);
		}
	}

	private void OnClickEnemyList()
	{
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_dungeonId);
		if (dungeonTempletBase != null)
		{
			NKMDiveGameData diveGameData = GetDiveGameData();
			NKCPopupEnemyList.Instance.Open(dungeonTempletBase, diveGameData?.Floor?.Templet, m_isBossSector);
		}
	}

	public void Close()
	{
		CloseSquadView();
		m_NKM_UI_DIVE_PROCESS_BATTLE_BUTTON_CG.DOKill();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_NKCPopupWarfareSelectShip != null && m_NKCPopupWarfareSelectShip.IsOpen)
		{
			m_NKCPopupWarfareSelectShip.Close();
		}
		m_NKCPopupWarfareSelectShip = null;
	}
}
