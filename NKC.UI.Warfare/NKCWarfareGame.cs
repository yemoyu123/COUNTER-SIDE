using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Community;
using ClientPacket.Warfare;
using Cs.Logging;
using DG.Tweening;
using NKC.PacketHandler;
using NKC.Publisher;
using NKC.UI.Option;
using NKC.UI.Result;
using NKM;
using NKM.Templet;
using NKM.Warfare;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Warfare;

public class NKCWarfareGame : NKCUIBase
{
	public class DataBeforeBattle
	{
		public int PrevContainerCount;

		public List<WarfareTileData> PrevTiles = new List<WarfareTileData>();

		public DataBeforeBattle(WarfareGameData gameData, WarfareSyncData syncData)
		{
			if (syncData.tiles != null)
			{
				for (int i = 0; i < syncData.tiles.Count; i++)
				{
					WarfareTileData tileData = gameData.GetTileData(syncData.tiles[i].index);
					if (tileData != null)
					{
						WarfareTileData item = new WarfareTileData
						{
							index = tileData.index,
							tileType = tileData.tileType,
							battleConditionId = tileData.battleConditionId
						};
						PrevTiles.Add(item);
					}
				}
			}
			if (syncData.gameState != null)
			{
				PrevContainerCount = gameData.containerCount;
			}
		}
	}

	public class RetryData
	{
		public class UnitData
		{
			public int DeckIndex;

			public int TileIndex;
		}

		public string WarfareStrID;

		public int FlagShipDeckIndex;

		public List<UnitData> UnitList;

		public RetryData(string warfareStrID, WarfareTeamData teamA)
		{
			WarfareStrID = warfareStrID;
			UnitList = new List<UnitData>();
			foreach (WarfareUnitData value in teamA.warfareUnitDataByUIDMap.Values)
			{
				if (value.friendCode == 0L)
				{
					UnitData item = new UnitData
					{
						DeckIndex = value.deckIndex.m_iIndex,
						TileIndex = value.tileIndex
					};
					UnitList.Add(item);
					if (teamA.flagShipWarfareUnitUID == value.warfareGameUnitUID)
					{
						FlagShipDeckIndex = value.deckIndex.m_iIndex;
					}
				}
			}
		}
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_warfare";

	private const string UI_ASSET_NAME = "NUM_WARFARE_UI";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	private List<int> RESOURCE_LIST = new List<int>();

	public NKCWarfareGameHUD m_NKCWarfareGameHUD;

	private GameObject m_NUM_WARFARE;

	public GameObject m_NUM_WARFARE_TILE_Panel;

	public GameObject m_NUM_WARFARE_UNIT_LIST;

	public GameObject m_NUM_WARFARE_UNIT_INFO_LIST;

	public GameObject m_NUM_WARFARE_DIVE_POINT;

	public GameObject m_NUM_WARFARE_LABEL;

	public GameObject m_NUM_WARFARE_BATTILE_CONDITION_LIST;

	public GameObject m_NUM_WARFARE_BG_A;

	public Image m_NUM_WARFARE_BG_A_img;

	public GameObject m_NUM_WARFARE_BG_B;

	public Image m_NUM_WARFARE_BG_B_img;

	public GameObject m_NUM_WARFARE_BG_IMG_B;

	public GameObject m_NUM_WARFARE_BG_B_MOVIE;

	public GameObject m_NUM_WARFARE_BG_B_MOVIE_IMG;

	public GameObject m_NUM_WARFARE_FX_SHIP_DIVE;

	public GameObject m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL;

	public GameObject m_NUM_WARFARE_FX_UNIT_EXPLOSION;

	public GameObject m_NUM_WARFARE_FX_UNIT_EXPLOSION_BIG;

	public GameObject m_NUM_WARFARE_FX_UNIT_ESCAPE;

	public GridLayoutGroup m_NUM_WARFARE_TILE_Panel_GLG;

	public RectTransform m_NUM_WARFARE_BG_WARBOX_A;

	public RectTransform m_NUM_WARFARE_BG_WARBOX_B;

	public CanvasGroup m_CanvasGroup;

	public float m_fStartAlhpa = 0.1f;

	public float m_fFadeTime = 1f;

	[Header("카메라 초기값")]
	public Vector3 m_CameraIntroPos;

	public Vector3 m_CameraIntroRot;

	private NKCWarfareGameTileMgr m_tileMgr;

	private NKCWarfareGameUnitMgr m_unitMgr;

	private NKCWarfareGameLabelMgr m_labelMgr;

	private NKCWarfareGameBattleCondition m_battleCondition;

	private NKCWarfareGameItemMgr m_containerMgr;

	private List<Vector3> m_listUnitPos = new List<Vector3>();

	private List<GameObject> m_listDivePoint = new List<GameObject>();

	private List<GameObject> m_listAssultPoint = new List<GameObject>();

	private List<GameObject> m_listRecoveryPoint = new List<GameObject>();

	private List<NKCWarfareGameAssist> m_listAssist = new List<NKCWarfareGameAssist>();

	private string m_WarfareStrID = "";

	private const int DEFAULT_TILE_COUNT = 70;

	private int m_LastClickedSpawnPoint = -1;

	private NKM_WARFARE_SPAWN_POINT_TYPE m_Last_WARFARE_SPAWN_POINT_TYPE = NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE;

	private int m_UserUnitMaxCount = -1;

	private int m_LastClickedUnitTileIndex = -1;

	private const int DEFAULT_CAMERA_Z_DIST = -798;

	private NKMWarfareClearData m_NKMWarfareClearData;

	private NKMDeckIndex m_TeamAFlagDeckIndex;

	private float m_fElapsedTimeToBattle;

	private bool m_bReservedBattle;

	private const float ZOOM_IN_TIME_FOR_BATTLE = 0.6f;

	public const float MOVE_TIME = 0.9f;

	private Coroutine m_CamTrackMovingUnitCoroutine;

	private bool m_bRunningCamTrackMovingUnit;

	private const float X_OFFSET_FOR_BATTLE = 100f;

	private bool m_bReservedShowBattleResult;

	private Sequence m_UnitDieSequence;

	private NKCWarfareGameUnit m_WarfareGameUnitToDie;

	private int m_RefPause;

	private bool m_bWaitEnemyTurn;

	private float m_fElapsedTimeForEnemyTurn;

	private const float ENEMY_TURN_WAIT_TIME = 1f;

	private Tweener m_twUnitInfoDie;

	private NKMWarfareMapTemplet m_NKMWarfareMapTemplet;

	private Rect m_rtCamBound;

	private const float CAM_Y_OFFSET_FOR_UNIT = -250f;

	private bool m_bReservedCallOnUnitShakeEnd;

	private const float UNIT_SHAKE_TIME = 2f;

	private const float UNIT_FADE_OUT_TIME = 0.6f;

	private float m_fElapsedTimeForShakeEnd;

	private bool m_bReservedCallNextOrderREQ;

	private int m_LastEnterUnitUID;

	private bool m_bOpenDeckView;

	private bool m_bWaitingNextOreder;

	public bool WaitAutoPacket;

	private DataBeforeBattle m_DataBeforeBattle;

	private RetryData m_RetryData;

	private List<NKCAssetInstanceData> m_listAssetInstance = new List<NKCAssetInstanceData>();

	private readonly Vector3 BATCH_EFFECT_POS = new Vector3(-500f, 1000f, -1000f);

	private const float BATCH_EFFECT_TIME = 1.5f;

	private NKCPopupWarfareSelectShip m_NKCPopupWarfareSelectShip;

	private bool m_bFirstOpenDeck = true;

	private NKCUIDeckViewer.DeckViewerMode m_lastDeckView;

	private bool m_bSelectRecovery;

	private bool m_bPlayingIntro;

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

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_WARFARE;

	public override List<int> UpsideMenuShowResourceList => RESOURCE_LIST;

	public override string GuideTempletID => "ARTICLE_WARFARE_CONTROL";

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode
	{
		get
		{
			NKM_WARFARE_GAME_STATE warfareGameState = NKCScenManager.GetScenManager().WarfareGameData.warfareGameState;
			if (warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP && warfareGameState - 1 <= NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAYING)
			{
				return NKCUIUpsideMenu.eMode.Disable;
			}
			return NKCUIUpsideMenu.eMode.Normal;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	private NKCPopupWarfareSelectShip NKCPopupWarfareSelectShip
	{
		get
		{
			if (m_NKCPopupWarfareSelectShip == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupWarfareSelectShip>("AB_UI_NKM_UI_POPUP_WARFARE_SELECT", "NKM_UI_POPUP_WARFARE_SELECT_SHIP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCPopupWarfareSelectShip = loadedUIData.GetInstance<NKCPopupWarfareSelectShip>();
				m_NKCPopupWarfareSelectShip?.InitUI();
			}
			return m_NKCPopupWarfareSelectShip;
		}
	}

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCWarfareGame>("ab_ui_nkm_ui_warfare", "NUM_WARFARE_UI", NKCUIManager.eUIBaseRect.UIMidCanvas, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public static NKCWarfareGame GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCWarfareGame>();
		}
		return null;
	}

	public NKCWarfareGameUnitMgr GetNKCWarfareGameUnitMgr()
	{
		return m_unitMgr;
	}

	public bool GetPause()
	{
		return m_RefPause > 0;
	}

	public void AddPauseRef()
	{
		m_RefPause++;
	}

	public void MinusPauseRef()
	{
		m_RefPause--;
		if (m_RefPause <= 0)
		{
			m_RefPause = 0;
		}
	}

	public void InitUI()
	{
		m_NUM_WARFARE = base.gameObject;
		m_NUM_WARFARE.SetActive(value: false);
		EventTrigger component = m_NUM_WARFARE.GetComponent<EventTrigger>();
		if (component != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				InvalidSelectedUnit();
			});
			component.triggers.Add(entry);
			EventTrigger.Entry entry2 = new EventTrigger.Entry();
			entry2.eventID = EventTriggerType.Drag;
			entry2.callback.AddListener(OnDragByInstance);
			component.triggers.Add(entry2);
		}
		m_NKCWarfareGameHUD.InitUI(this, OnStartUserPhase, OnStartEnemyPhase);
		m_NKCWarfareGameHUD.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), worldPositionStays: false);
		m_NKCWarfareGameHUD.transform.SetAsLastSibling();
		NKCUIManager.OpenUI(m_NKCWarfareGameHUD.gameObject);
		m_tileMgr = new NKCWarfareGameTileMgr(m_NUM_WARFARE_TILE_Panel.transform);
		m_unitMgr = new NKCWarfareGameUnitMgr(m_NUM_WARFARE_UNIT_LIST.transform, m_NUM_WARFARE_UNIT_INFO_LIST.transform);
		m_battleCondition = new NKCWarfareGameBattleCondition(m_NUM_WARFARE_BATTILE_CONDITION_LIST.transform);
		m_labelMgr = new NKCWarfareGameLabelMgr(m_NUM_WARFARE_LABEL.transform);
		m_containerMgr = new NKCWarfareGameItemMgr(m_NUM_WARFARE_LABEL.transform);
	}

	public void OpenWaitBox()
	{
		m_NKCWarfareGameHUD.SetWaitBox(bSet: true);
	}

	public void SetWarfareStrID(string strID)
	{
		m_WarfareStrID = strID;
		if (m_NKCWarfareGameHUD != null)
		{
			m_NKCWarfareGameHUD.SetWarfareStrID(m_WarfareStrID);
		}
	}

	public void SetBG(bool bReadyBG, bool bStart = false)
	{
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_BG_A, bReadyBG);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_BG_B, !bReadyBG);
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet != null)
		{
			if (bReadyBG)
			{
				if (nKMWarfareTemplet.m_WarfareBG_Stop.Length > 0)
				{
					m_NUM_WARFARE_BG_A_img.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WARFARE_TEXTURE", nKMWarfareTemplet.m_WarfareBG_Stop);
				}
			}
			else if (nKMWarfareTemplet.m_WarfareBG_Playing.Length > 0)
			{
				m_NUM_WARFARE_BG_B_img.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WARFARE_TEXTURE", nKMWarfareTemplet.m_WarfareBG_Playing);
			}
		}
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_BG_B_MOVIE, !bReadyBG && bStart);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_BG_B_MOVIE_IMG, !bReadyBG && !bStart);
	}

	private void CheckValidWarfare()
	{
		m_NKMWarfareMapTemplet = null;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			Debug.LogError("warfare open - NKMWarfareTemplet is null");
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			Debug.LogError("warfare open - NKMWarfareMapTemplet is null");
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		}
		else
		{
			m_NKMWarfareMapTemplet = mapTemplet;
		}
	}

	private void InitWhenFirstOpen()
	{
		if (m_listUnitPos.Count <= 0)
		{
			for (int i = 0; i < 70; i++)
			{
				m_listUnitPos.Add(new Vector3(0f, 0f, 0f));
			}
			m_tileMgr.Init(OnClickPossibleArrivalTile);
			m_unitMgr.Init();
			m_battleCondition.Init();
		}
	}

	public void Open(bool bAfterBattle, DataBeforeBattle dataBeforeBattle, RetryData retryData)
	{
		m_bReservedShowBattleResult = bAfterBattle;
		m_DataBeforeBattle = dataBeforeBattle;
		m_RetryData = retryData;
		CheckValidWarfare();
		InitWhenFirstOpen();
		m_CanvasGroup.alpha = 1f;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			Debug.LogError("전역 알수 없는 템플릿을 찾으려함 - " + m_WarfareStrID);
			return;
		}
		m_twUnitInfoDie = null;
		m_bWaitEnemyTurn = false;
		m_bWaitingNextOreder = false;
		WaitAutoPacket = false;
		m_bReservedBattle = false;
		m_fElapsedTimeForShakeEnd = 0f;
		m_bReservedCallOnUnitShakeEnd = false;
		m_bPlayingIntro = false;
		if (m_NKCWarfareGameHUD == null)
		{
			Debug.LogError("WarfareGameHUD is null");
			return;
		}
		m_NKCWarfareGameHUD.SetWarfareStrID(m_WarfareStrID);
		m_NKCWarfareGameHUD.DeActivateAllTriggerUI();
		m_NKCWarfareGameHUD.CloseSelectedSquadUI();
		m_NKCWarfareGameHUD.SetActiveContainer(bSet: false);
		m_NUM_WARFARE.SetActive(value: true);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_SHIP_DIVE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION_BIG, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_ESCAPE, bValue: false);
		m_UserUnitMaxCount = nKMWarfareTemplet.m_UserTeamCount;
		NKCScenManager.GetScenManager().WarfareGameData.warfareTempletID = nKMWarfareTemplet.m_WarfareID;
		m_NKCWarfareGameHUD.UpdateMedalInfo();
		m_NKCWarfareGameHUD.UpdateWinCondition();
		if (nKMWarfareTemplet.StageTemplet.EpisodeCategory == EPISODE_CATEGORY.EC_DAILY)
		{
			RESOURCE_LIST = new List<int>
			{
				nKMWarfareTemplet.StageTemplet.m_StageReqItemID,
				101
			};
		}
		else
		{
			RESOURCE_LIST = new List<int> { 1, 2, 3, 101 };
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			Debug.LogError("WarfareGameData is null");
			return;
		}
		UIOpened();
		if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			if (m_bReservedShowBattleResult)
			{
				ForceBack();
				return;
			}
			bool flag = false;
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (!NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing() && (!myUserData.CheckWarfareClear(warfareGameData.warfareTempletID) || NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene))
			{
				NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(nKMWarfareTemplet.m_CutScenStrIDBefore);
				if (cutScenTemple != null)
				{
					base.gameObject.SetActive(value: false);
					m_NKCWarfareGameHUD.Close();
					flag = true;
					NKCUICutScenPlayer.Instance.Play(cutScenTemple, nKMWarfareTemplet.StageTemplet.Key, OnStartCutScenEnd);
				}
			}
			if (!flag)
			{
				PrepareWarfareGameReady();
			}
			m_bReservedShowBattleResult = false;
		}
		else if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_RESULT)
		{
			bool flag2 = false;
			if (!NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing() && warfareGameData.isWinTeamA && (NKCUtil.m_sHsFirstClearWarfare.Contains(warfareGameData.warfareTempletID) || NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene))
			{
				NKCCutScenTemplet cutScenTemple2 = NKCCutScenManager.GetCutScenTemple(nKMWarfareTemplet.m_CutScenStrIDAfter);
				if (cutScenTemple2 != null)
				{
					NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
					m_NKCWarfareGameHUD.Close();
					NKCUICutScenPlayer.Instance.Play(cutScenTemple2, nKMWarfareTemplet.StageTemplet.Key, OnEndCutScenEnd);
					flag2 = true;
				}
			}
			if (!flag2)
			{
				OnEndCutScenEnd();
			}
		}
		else
		{
			OnEndCutScenEnd();
		}
	}

	private void OnEndCutScenEnd()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUIManager.UpdateUpsideMenu();
		m_NKCWarfareGameHUD.Open();
		PrepareWarfareGameIntrude();
		RefreshContainerWhenOpen(m_bReservedShowBattleResult);
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		NKCUtil.m_sHsFirstClearWarfare.Remove(warfareGameData.warfareTempletID);
		if (m_bReservedShowBattleResult && warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAY_TRY_READY && warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAY_TRY)
		{
			CheckTutorialAfterBattle();
			if (!ShowBattleResult())
			{
				OpenActionWhenNotStop();
			}
		}
		else
		{
			OpenActionWhenNotStop();
		}
		m_bReservedShowBattleResult = false;
	}

	private void ShowEndCutWhenOpened()
	{
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareGameData == null || nKMWarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_RESULT)
		{
			return;
		}
		bool flag = false;
		if (nKMWarfareGameData.isWinTeamA && !NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing() && (NKCUtil.m_sHsFirstClearWarfare.Contains(nKMWarfareGameData.warfareTempletID) || NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene) && nKMWarfareTemplet != null)
		{
			NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(nKMWarfareTemplet.m_CutScenStrIDAfter);
			if (cutScenTemple != null)
			{
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL, bValue: false);
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_SHIP_DIVE, bValue: false);
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION, bValue: false);
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION_BIG, bValue: false);
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_ESCAPE, bValue: false);
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
				m_NKCWarfareGameHUD.Close();
				NKCUICutScenPlayer.Instance.Play(cutScenTemple, nKMWarfareTemplet.StageTemplet.Key, OnEndCutScenEndWhenOpened);
				flag = true;
			}
		}
		if (!flag)
		{
			OnEndCutScenEndWhenOpened();
		}
	}

	private void OnEndCutScenEndWhenOpened()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCWarfareGameHUD.Open();
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		NKCUtil.m_sHsFirstClearWarfare.Remove(warfareGameData.warfareTempletID);
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet.m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_TILE_ENTER)
		{
			PlayEnterAni();
		}
		else if (nKMWarfareTemplet.m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD)
		{
			Debug.Log("warfare next order REQ - OnEndCutScenEndWhenOpened");
			SendGetNextOrderREQ();
		}
	}

	private bool ShowBattleResult()
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return false;
		}
		if (nKMWarfareTemplet.MapTemplet == null)
		{
			return false;
		}
		m_WarfareGameUnitToDie = null;
		List<WarfareUnitData> unitDataList = warfareGameData.GetUnitDataList();
		for (int i = 0; i < unitDataList.Count; i++)
		{
			WarfareUnitData warfareUnitData = unitDataList[i];
			if (!(warfareUnitData.hp <= 0f))
			{
				m_WarfareGameUnitToDie = CheckAndAddBattleEnemy(warfareUnitData.warfareGameUnitUID, warfareUnitData.tileIndex);
				if (m_WarfareGameUnitToDie != null)
				{
					break;
				}
			}
		}
		if (m_WarfareGameUnitToDie != null)
		{
			if (m_UnitDieSequence != null)
			{
				m_UnitDieSequence.Kill();
			}
			m_UnitDieSequence = null;
			WarfareUnitData nKMWarfareUnitData = m_WarfareGameUnitToDie.GetNKMWarfareUnitData();
			NKMDungeonTempletBase nKMDungeonTempletBase = null;
			WarfareUnitData.Type unitType = nKMWarfareUnitData.unitType;
			if (unitType != WarfareUnitData.Type.User && unitType == WarfareUnitData.Type.Dungeon)
			{
				nKMDungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(nKMWarfareUnitData.dungeonID);
			}
			bool flag = true;
			if (nKMDungeonTempletBase != null && nKMDungeonTempletBase.m_NKM_WARFARE_GAME_UNIT_DIE_TYPE == NKMDungeonTempletBase.NKM_WARFARE_GAME_UNIT_DIE_TYPE.NWGUDT_RUNAWAY)
			{
				flag = false;
				PlayUnitRunawayAni();
			}
			if (flag)
			{
				PlayUnitDieAni();
			}
			m_fElapsedTimeForShakeEnd = 0f;
			m_bReservedCallOnUnitShakeEnd = true;
			return true;
		}
		return false;
	}

	private void OpenActionWhenNotStop()
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_RESULT)
		{
			m_NKCWarfareGameHUD.DeActivateAllTriggerUI();
			Debug.Log("warfare next order REQ - OpenActionWhenNotStop NWGS_RESULT");
			SendGetNextOrderREQ();
		}
		else if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_PLAYING && (!warfareGameData.isTurnA || IsAutoWarfare()))
		{
			Debug.Log("warfare next order REQ - OpenActionWhenNotStop NWGS_PLAYING");
			SendGetNextOrderREQ(null);
		}
		else if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAY_TRY_READY)
		{
			Debug.Log("warfare next order REQ - OpenActionWhenNotStop NWGS_INGAME_PLAY_TRY_READY");
			SendGetNextOrderREQ(null);
		}
		else if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAY_TRY)
		{
			TryGameLoadReq();
		}
	}

	public override void Hide()
	{
		m_bHide = true;
		base.gameObject.SetActive(m_bOpenDeckView);
		m_bOpenDeckView = false;
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_SHIP_DIVE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION_BIG, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_ESCAPE, bValue: false);
		m_unitMgr.Hide();
		m_NKCWarfareGameHUD.Close();
		NKCCamera.EnableBlur(bEnable: true);
	}

	public override void UnHide()
	{
		base.UnHide();
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE, bValue: true);
		m_NKCWarfareGameHUD.Open();
		if (GetNKMWarfareGameData().warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			m_NKCWarfareGameHUD.SetBatchedShipCount(m_unitMgr.GetCurrentUserUnit());
		}
		NKCCamera.EnableBlur(bEnable: false);
	}

	private void OnStartCutScenEnd()
	{
		NKCUIManager.UpdateUpsideMenu();
		base.gameObject.SetActive(value: true);
		m_NKCWarfareGameHUD.Open();
		PrepareWarfareGameReady();
	}

	private void SetEnemyTurnUI()
	{
		Debug.Log("warfare show Enemy Turn");
		m_NKCWarfareGameHUD.TriggerEnemyTurnUI();
		m_NKCWarfareGameHUD.SetPhaseUserType(bUser: false);
	}

	private void OnCommonUserTurnFinished(bool bTileEffectShow = false)
	{
		m_NKCWarfareGameHUD.SetActiveTurnFinishBtn(bSet: false);
		if (bTileEffectShow)
		{
			m_bWaitEnemyTurn = true;
			m_fElapsedTimeForEnemyTurn = 1f;
		}
		else
		{
			SetEnemyTurnUI();
		}
		InvalidSelectedUnitPure();
	}

	private void OnStartEnemyPhase()
	{
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		if (nKMWarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
		{
			Debug.Log("warfare next order REQ fail - State : " + nKMWarfareGameData.warfareGameState);
			return;
		}
		if (nKMWarfareGameData.isTurnA)
		{
			Debug.Log("warfare next order REQ fail - Turn A is true");
			return;
		}
		Debug.Log("warfare next order REQ - OnStartEnemyPhase");
		SendGetNextOrderREQ();
	}

	private void OnStartUserPhase()
	{
		if (IsAutoWarfare())
		{
			WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
			if (nKMWarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
			{
				Debug.Log("warfare next order REQ fail - State : " + nKMWarfareGameData.warfareGameState);
				return;
			}
			if (!nKMWarfareGameData.isTurnA)
			{
				Debug.Log("warfare next order REQ fail - Turn A is false");
				return;
			}
			Debug.Log("warfare next order REQ - OnStartUserPhase");
			SendGetNextOrderREQ();
		}
	}

	private int GetSelectedSquadWFGUUID()
	{
		NKCWarfareGameUnit wFGameUnitByDeckIndex = m_unitMgr.GetWFGameUnitByDeckIndex(m_NKCWarfareGameHUD.GetNKMDeckIndexSelected());
		if (wFGameUnitByDeckIndex != null && wFGameUnitByDeckIndex.GetNKMWarfareUnitData() != null)
		{
			return wFGameUnitByDeckIndex.GetNKMWarfareUnitData().warfareGameUnitUID;
		}
		return -1;
	}

	private void SendGetNextOrderREQ(NKCWarfareGameUnit cNKCWarfareGameUnit)
	{
		if (cNKCWarfareGameUnit != null && cNKCWarfareGameUnit.GetNKMWarfareUnitData() != null)
		{
			if (cNKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User)
			{
				if (!GetNKMWarfareGameData().isTurnA)
				{
					return;
				}
			}
			else if (GetNKMWarfareGameData().isTurnA)
			{
				return;
			}
		}
		Debug.Log("warfare next order REQ - SendGetNextOrderREQ(NKCWarfareGameUnit");
		SendGetNextOrderREQ();
	}

	private void SendGetNextOrderREQ()
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCWarfareManager.CheckGetNextOrderCond(NKCScenManager.GetScenManager().GetMyUserData());
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			Debug.LogWarning("warfare next order NKM_ERROR_CODE : " + nKM_ERROR_CODE);
			return;
		}
		if (m_bWaitingNextOreder)
		{
			Debug.LogWarning("warfare next order is ignored by waiting prev order");
			return;
		}
		m_bWaitingNextOreder = true;
		SetPause(bSet: true);
		NKCPacketSender.Send_NKMPacket_WARFARE_GAME_NEXT_ORDER_REQ();
	}

	private void SendGetNextOrderREQIfAuto(NKCWarfareGameUnit cNKCWarfareGameUnit)
	{
		if (IsAutoWarfare())
		{
			Debug.Log("warfare next order REQ - SendGetNextOrderREQIfAuto");
			SendGetNextOrderREQ(cNKCWarfareGameUnit);
		}
	}

	private void OnCallBackForResult(NKM_SCEN_ID scenID)
	{
		NKCScenManager.GetScenManager().ScenChangeFade(scenID);
	}

	private WarfareGameData GetNKMWarfareGameData()
	{
		return NKCScenManager.GetScenManager().WarfareGameData;
	}

	private void ProcessWarfareGameSyncData(WarfareSyncData syncData)
	{
		if (syncData == null)
		{
			return;
		}
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		bool flag = false;
		WarfareGameSyncData gameState = syncData.gameState;
		if (gameState != null)
		{
			bool flag2 = false;
			if (!nKMWarfareGameData.isTurnA && gameState.isTurnA)
			{
				if (gameState.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_RESULT)
				{
					m_NKCWarfareGameHUD.TriggerPlayerTurnUI();
					m_NKCWarfareGameHUD.SetPhaseUserType(bUser: true);
					m_NKCWarfareGameHUD.SetActiveTurnFinishBtn(!IsAutoWarfare());
					Debug.Log("warfare my turn show");
				}
				else
				{
					flag2 = true;
				}
			}
			else if (nKMWarfareGameData.isTurnA && !gameState.isTurnA)
			{
				OnCommonUserTurnFinished(syncData.updatedUnits.Count + syncData.newUnits.Count > 0);
				flag = true;
			}
			if (nKMWarfareGameData.turnCount != gameState.turnCount)
			{
				m_NKCWarfareGameHUD.SetTurnCount(gameState.turnCount);
			}
			nKMWarfareGameData.UpdateGameState(gameState);
			if (nKMWarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAY_TRY)
			{
				TryGameLoadReq();
			}
			else if (nKMWarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_RESULT)
			{
				if (gameState.isWinTeamA)
				{
					if (!NKCScenManager.GetScenManager().GetMyUserData().CheckWarfareClear(nKMWarfareGameData.warfareTempletID) && !NKCUtil.m_sHsFirstClearWarfare.Contains(nKMWarfareGameData.warfareTempletID))
					{
						NKCUtil.m_sHsFirstClearWarfare.Add(nKMWarfareGameData.warfareTempletID);
					}
					if (flag2)
					{
						ShowEndCutWhenOpened();
					}
				}
				else if (flag2)
				{
					Debug.Log("warfare next order REQ - ProcessWarfareGameSyncData bShowResult");
					SendGetNextOrderREQ();
				}
			}
		}
		for (int i = 0; i < syncData.updatedUnits.Count; i++)
		{
			WarfareUnitSyncData warfareUnitSyncData = syncData.updatedUnits[i];
			WarfareUnitData unitData = nKMWarfareGameData.GetUnitData(warfareUnitSyncData.warfareGameUnitUID);
			if (unitData == null)
			{
				continue;
			}
			if (flag && unitData.unitType == WarfareUnitData.Type.User)
			{
				m_unitMgr.ShowUserUnitTileFX(unitData, warfareUnitSyncData);
			}
			bool num = !unitData.isTurnEnd && unitData.isTurnEnd != warfareUnitSyncData.isTurnEnd;
			bool flag3 = syncData.movedUnits.Exists((WarfareSyncData.MovedUnit v) => v.unitUID == unitData.warfareGameUnitUID);
			nKMWarfareGameData.UpdateUnitData(unitData, warfareUnitSyncData);
			if (num && !flag3)
			{
				NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(unitData.warfareGameUnitUID);
				if (nKCWarfareGameUnit != null)
				{
					if (unitData.unitType == WarfareUnitData.Type.User)
					{
						if (syncData.movedUnits.Count == 0 && syncData.retreaters.Count == 0)
						{
							nKCWarfareGameUnit.SetTurnEndTimer(SendGetNextOrderREQIfAuto);
						}
					}
					else
					{
						NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_unitMgr.GetNKCWarfareGameUnitInfo(unitData.warfareGameUnitUID);
						if (nKCWarfareGameUnitInfo != null && nKCWarfareGameUnitInfo.IsMovableActionType())
						{
							if (syncData.movedUnits.Count == 0 && syncData.retreaters.Count == 0)
							{
								nKCWarfareGameUnit.Move(m_listUnitPos[unitData.tileIndex], 0.9f, SendGetNextOrderREQ, bOnlyJump: true);
							}
							else
							{
								nKCWarfareGameUnit.Move(m_listUnitPos[unitData.tileIndex], 0.9f, null, bOnlyJump: true);
							}
						}
					}
				}
			}
			m_unitMgr.UpdateGameUnitUI(warfareUnitSyncData.warfareGameUnitUID);
		}
		for (int num2 = 0; num2 < syncData.newUnits.Count; num2++)
		{
			WarfareUnitData newUnit = syncData.newUnits[num2];
			if (newUnit.unitType == WarfareUnitData.Type.User)
			{
				Vector3 localPosition = m_listUnitPos[newUnit.tileIndex];
				WarfareTeamData warfareTeamDataA = nKMWarfareGameData.warfareTeamDataA;
				if (m_NUM_WARFARE_FX_SHIP_DIVE != null)
				{
					m_NUM_WARFARE_FX_SHIP_DIVE.gameObject.transform.localPosition = localPosition;
					m_NUM_WARFARE_FX_SHIP_DIVE.SetActive(value: false);
					m_NUM_WARFARE_FX_SHIP_DIVE.SetActive(value: true);
				}
				WarfareUnitData warfareUnitData = warfareTeamDataA.warfareUnitDataByUIDMap.Values.ToList().Find((WarfareUnitData v) => v.deckIndex.Compare(newUnit.deckIndex));
				if (warfareUnitData != null)
				{
					warfareTeamDataA.warfareUnitDataByUIDMap.Remove(warfareUnitData.warfareGameUnitUID);
				}
				warfareTeamDataA.warfareUnitDataByUIDMap.Add(newUnit.warfareGameUnitUID, newUnit);
				NKCWarfareGameUnit nKCWarfareGameUnit2 = m_unitMgr.CreateNewUserUnit(newUnit.deckIndex, newUnit.tileIndex, OnClickUnit, newUnit, 0L);
				if (nKCWarfareGameUnit2 != null)
				{
					nKCWarfareGameUnit2.gameObject.transform.localPosition = localPosition;
					nKCWarfareGameUnit2.transform.DOMove(BATCH_EFFECT_POS, 1.5f).SetEase(Ease.OutCubic).From(isRelative: true);
					if (m_containerMgr.IsItem(nKCWarfareGameUnit2.TileIndex))
					{
						PlayGetContainer(nKCWarfareGameUnit2.TileIndex, bWithEnemy: false);
					}
				}
				NKCOperatorUtil.PlayVoice(newUnit.deckIndex, VOICE_TYPE.VT_SHIP_RECALL);
			}
			else
			{
				NKCWarfareGameUnit nKCWarfareGameUnit3 = m_unitMgr.CreateNewEnemyUnit(NKMDungeonManager.GetDungeonStrID(newUnit.dungeonID), nKMWarfareGameData.warfareTeamDataB.flagShipWarfareUnitUID == newUnit.warfareGameUnitUID, newUnit.isTarget, newUnit.tileIndex, newUnit.warfareEnemyActionType, OnClickUnit, newUnit);
				WarfareTeamData warfareTeamDataB = nKMWarfareGameData.warfareTeamDataB;
				if (!warfareTeamDataB.warfareUnitDataByUIDMap.ContainsKey(newUnit.warfareGameUnitUID))
				{
					warfareTeamDataB.warfareUnitDataByUIDMap.Add(newUnit.warfareGameUnitUID, newUnit);
				}
				else
				{
					warfareTeamDataB.warfareUnitDataByUIDMap[newUnit.warfareGameUnitUID] = newUnit;
				}
				if (nKCWarfareGameUnit3 != null)
				{
					nKCWarfareGameUnit3.gameObject.transform.localPosition = m_listUnitPos[newUnit.tileIndex];
					nKCWarfareGameUnit3.PlayEnemySpawnAni();
				}
			}
		}
		bool flag4 = false;
		List<int> list = new List<int>();
		for (int num3 = 0; num3 < syncData.movedUnits.Count; num3++)
		{
			WarfareSyncData.MovedUnit movedUnit = syncData.movedUnits[num3];
			int unitUID = movedUnit.unitUID;
			short tileIndex = movedUnit.tileIndex;
			NKCWarfareGameUnit nKCWarfareGameUnit4 = m_unitMgr.GetNKCWarfareGameUnit(unitUID);
			if (nKCWarfareGameUnit4 == null)
			{
				continue;
			}
			int tileIndex2 = nKCWarfareGameUnit4.TileIndex;
			bool flag5 = false;
			NKCWarfareGameUnit gameUnitByTileIndex = m_unitMgr.GetGameUnitByTileIndex(tileIndex);
			if (gameUnitByTileIndex != null)
			{
				flag5 = nKMWarfareGameData.CheckTeamA_By_GameUnitUID(gameUnitByTileIndex.GetNKMWarfareUnitData().warfareGameUnitUID);
			}
			nKCWarfareGameUnit4.GetNKMWarfareUnitData().tileIndex = tileIndex;
			bool flag6 = nKMWarfareGameData.CheckTeamA_By_GameUnitUID(unitUID);
			if (gameUnitByTileIndex != null && flag5 != flag6)
			{
				InvalidSelectedUnitPure();
				Vector3 vector = m_listUnitPos[tileIndex];
				Vector3 localPosition2 = gameUnitByTileIndex.gameObject.transform.localPosition;
				if (nKCWarfareGameUnit4.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User)
				{
					vector.x -= 100f;
					localPosition2.x += 100f;
				}
				else
				{
					vector.x += 100f;
					localPosition2.x -= 100f;
				}
				bool flag7 = syncData.retreaters.Contains(unitUID);
				bool flag8 = syncData.retreaters.Contains(gameUnitByTileIndex.GetNKMWarfareUnitData().warfareGameUnitUID);
				if (!flag7 && !flag8)
				{
					nKCWarfareGameUnit4.Move(vector, 0.9f, SendGetNextOrderREQ);
					gameUnitByTileIndex.Move(localPosition2, 0.9f);
					gameUnitByTileIndex.SetPause(GetPause());
				}
				else if ((flag7 && !flag8) || (flag8 && !flag7))
				{
					if (flag7 && !flag8)
					{
						nKCWarfareGameUnit4.GetNKMWarfareUnitData().hp = 0f;
						RetreatUnit(nKCWarfareGameUnit4, gameUnitByTileIndex, vector, localPosition2);
						list.Add(unitUID);
					}
					else
					{
						gameUnitByTileIndex.GetNKMWarfareUnitData().hp = 0f;
						RetreatUnit(gameUnitByTileIndex, nKCWarfareGameUnit4, localPosition2, vector);
						list.Add(gameUnitByTileIndex.GetNKMWarfareUnitData().warfareGameUnitUID);
					}
				}
				else
				{
					Log.Error($"[전역 후퇴] m? {flag7}, t? {flag8}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCWarfareGame.cs", 1271);
				}
			}
			else if (nKCWarfareGameUnit4.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.Dungeon)
			{
				nKCWarfareGameUnit4.Move(m_listUnitPos[tileIndex], 0.9f, SendGetNextOrderREQ);
				m_containerMgr.SetPos(tileIndex, bWithEnemy: true);
				m_containerMgr.SetPos(tileIndex2, bWithEnemy: false);
			}
			else
			{
				if (m_LastClickedUnitTileIndex != -1 && !flag4 && m_LastClickedUnitTileIndex != tileIndex)
				{
					m_LastClickedUnitTileIndex = tileIndex;
					flag4 = true;
				}
				if (gameUnitByTileIndex != null)
				{
					nKCWarfareGameUnit4.Move(m_listUnitPos[tileIndex], 0.9f);
				}
				else
				{
					nKCWarfareGameUnit4.Move(m_listUnitPos[tileIndex], 0.9f, DoWhenUnitMoveEnd);
				}
			}
			nKCWarfareGameUnit4.SetPause(GetPause());
			CamTrackMovingUnit(nKCWarfareGameUnit4.transform.localPosition, m_listUnitPos[tileIndex]);
		}
		for (int num4 = 0; num4 < syncData.retreaters.Count; num4++)
		{
			int num5 = syncData.retreaters[num4];
			if (!list.Contains(num5))
			{
				NKCWarfareGameUnit nKCWarfareGameUnit5 = m_unitMgr.GetNKCWarfareGameUnit(num5);
				if (!(nKCWarfareGameUnit5 == null))
				{
					nKCWarfareGameUnit5.GetNKMWarfareUnitData().hp = 0f;
					PlayRetreatUnit(nKCWarfareGameUnit5);
					break;
				}
			}
		}
		for (int num6 = 0; num6 < syncData.tiles.Count; num6++)
		{
			WarfareTileData warfareTileData = syncData.tiles[num6];
			int index = warfareTileData.index;
			WarfareTileData tileData = nKMWarfareGameData.GetTileData(index);
			if (tileData != null)
			{
				if (warfareTileData.tileType != tileData.tileType)
				{
					tileData.tileType = warfareTileData.tileType;
				}
				if (warfareTileData.battleConditionId != tileData.battleConditionId)
				{
					tileData.battleConditionId = warfareTileData.battleConditionId;
				}
			}
		}
		DoAfterSync();
		CheckTutorialAfterSync();
	}

	private void DoAfterSync()
	{
		SetTileDefaultWhenPlay();
		UpdateLabel();
		m_unitMgr.UpdateGameUnitUI();
		m_NKCWarfareGameHUD.SetRemainTurnOnUnitCount(m_unitMgr.GetRemainTurnOnUserUnitCount());
		m_unitMgr.ResetIcon();
		CloseAssistFX();
		UpdateSelectedSquadUI();
		UpdateRecoveryCount();
		CancelRecovery();
	}

	private void DoWhenUnitMoveEnd(NKCWarfareGameUnit cNKCWarfareGameUnit)
	{
		Debug.Log("MoveEnd call Log");
		if (cNKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User && m_containerMgr.IsItem(cNKCWarfareGameUnit.TileIndex))
		{
			PlayGetContainer(cNKCWarfareGameUnit.TileIndex, bWithEnemy: false);
		}
		if (CheckEnterUnit(cNKCWarfareGameUnit.GetNKMWarfareUnitData().warfareGameUnitUID))
		{
			ShowEndCutWhenOpened();
		}
		else
		{
			SendGetNextOrderREQIfAuto(cNKCWarfareGameUnit);
		}
	}

	private void RetreatUnit(NKCWarfareGameUnit retreatUnit, NKCWarfareGameUnit tileUnit, Vector3 retreatPos, Vector3 tilePos)
	{
		retreatUnit.Move(retreatPos, 0.9f, PlayRetreatUnit);
		retreatUnit.SetPause(GetPause());
		tileUnit.Move(tilePos, 0.9f);
		tileUnit.SetPause(GetPause());
	}

	private void PlayRetreatUnit(NKCWarfareGameUnit unit)
	{
		m_WarfareGameUnitToDie = unit;
		PlayUnitRunawayAni();
		m_fElapsedTimeForShakeEnd = 0f;
		m_bReservedCallOnUnitShakeEnd = true;
	}

	private void PlayGetContainer(int index, bool bWithEnemy)
	{
		m_containerMgr.Set(index, WARFARE_ITEM_STATE.Recv, m_listUnitPos[index], bWithEnemy);
		Vector3 position = NKCCamera.GetCamera().WorldToScreenPoint(m_containerMgr.GetWorldPos(index));
		Vector3 itemPos = NKCCamera.GetSubUICamera().ScreenToWorldPoint(position);
		m_NKCWarfareGameHUD.PlayGetContainer(itemPos, GetNKMWarfareGameData().containerCount);
		NKCSoundManager.PlaySound("FX_UI_WARFARE_GET_CONTANIER", 1f, 0f, 0f);
	}

	public void SetActiveAutoOnOff(bool bAuto, bool bAutoSupply)
	{
		m_NKCWarfareGameHUD.SetActiveAutoOnOff(bAuto, bAutoSupply);
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		if (nKMWarfareGameData != null && nKMWarfareGameData.isTurnA)
		{
			if (nKMWarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP)
			{
				m_NKCWarfareGameHUD.SetActiveTurnFinishBtn(!bAuto);
			}
			if (nKMWarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_PLAYING && bAuto && !m_unitMgr.CheckExistMovingUserUnit() && !m_NKCWarfareGameHUD.CheckVisibleWarfareStateEffectUI())
			{
				Debug.Log("warfare next order REQ - SetActiveAutoOnOff");
				SendGetNextOrderREQ(null);
			}
			UpdateRecoveryCount();
		}
	}

	private void Update()
	{
		if (!base.IsOpen)
		{
			return;
		}
		if (m_bReservedBattle)
		{
			if (!GetPause())
			{
				m_fElapsedTimeToBattle += Time.deltaTime;
			}
			if (m_fElapsedTimeToBattle >= 0.6f)
			{
				m_bReservedBattle = false;
				WarfareUnitData unitData = GetNKMWarfareGameData().GetUnitData(GetNKMWarfareGameData().battleMonsterUid);
				if (unitData != null)
				{
					NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
					if (nKMWarfareTemplet != null)
					{
						NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(0, nKMWarfareTemplet.StageTemplet.Key, 0, NKMDungeonManager.GetDungeonStrID(unitData.dungeonID), 0, bLocal: false, 1, 0, 0L);
					}
				}
			}
		}
		if (m_bWaitEnemyTurn)
		{
			m_fElapsedTimeForEnemyTurn -= Time.deltaTime;
			if (m_fElapsedTimeForEnemyTurn <= 0f)
			{
				m_fElapsedTimeForEnemyTurn = 0f;
				m_bWaitEnemyTurn = false;
				SetEnemyTurnUI();
			}
		}
		if (m_bReservedCallOnUnitShakeEnd)
		{
			m_fElapsedTimeForShakeEnd += Time.deltaTime;
			if (m_fElapsedTimeForShakeEnd >= 2.6999998f)
			{
				OnUnitShakeEnd();
			}
		}
		if (m_bPlayingIntro && !NKCCamera.GetTrackingPos().IsTracking())
		{
			m_bPlayingIntro = false;
		}
	}

	private void SetActiveForDeckView(bool bAcitve)
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
		{
			NKCUtil.SetGameobjectActive(this, bAcitve);
		}
	}

	private void OnSetFlagShip(int gameUnitUID)
	{
		m_unitMgr.SetUserFlagShip(gameUnitUID, bPlayAni: true);
	}

	private void UpdateAttackCostUI()
	{
		int itemID = 0;
		int itemCount = 0;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet.StageTemplet != null && nKMWarfareTemplet.StageTemplet.EnterLimit > 0)
		{
			int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(nKMWarfareTemplet.StageTemplet.Key);
			int enterLimit = nKMWarfareTemplet.StageTemplet.EnterLimit;
			if (statePlayCnt >= enterLimit && nKMWarfareTemplet.StageTemplet.RestoreLimit > 0)
			{
				itemID = nKMWarfareTemplet.StageTemplet.RestoreReqItem.ItemId;
				itemCount = nKMWarfareTemplet.StageTemplet.RestoreReqItem.Count32;
			}
		}
		if (itemID == 0 || itemCount == 0)
		{
			NKCWarfareManager.GetCurrWarfareAttackCost(out itemID, out itemCount);
		}
		m_NKCWarfareGameHUD.SetAttackCost(itemID, itemCount);
	}

	private void OpenUserUnitInfoPopup(NKCWarfareGameUnit cNKCWarfareGameUnit, bool bPlaying = false)
	{
		if (cNKCWarfareGameUnit.GetNKMWarfareUnitData().unitType != WarfareUnitData.Type.User)
		{
			return;
		}
		if (!cNKCWarfareGameUnit.IsSupporter)
		{
			NKMDeckIndex deckIndex = cNKCWarfareGameUnit.GetNKMWarfareUnitData().deckIndex;
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(deckIndex);
			if (deckData == null)
			{
				return;
			}
			NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
			if (shipFromUID != null)
			{
				if (bPlaying)
				{
					NKCPopupWarfareSelectShip?.OpenForMyShipInWarfare(deckIndex, cNKCWarfareGameUnit.GetNKMWarfareUnitData().warfareGameUnitUID, shipFromUID.m_UnitID);
				}
				else
				{
					NKCPopupWarfareSelectShip?.OpenForMyShipInWarfare(deckIndex, cNKCWarfareGameUnit.GetNKMWarfareUnitData().warfareGameUnitUID, shipFromUID.m_UnitID, OnSetFlagShip, OnCancelBatch, OnDeckViewBtn);
				}
			}
		}
		else if (bPlaying)
		{
			NKCPopupWarfareSelectShip.OpenForSupporterInWarfare(GetNKMWarfareGameData().supportUnitData, cNKCWarfareGameUnit.GetNKMWarfareUnitData().warfareGameUnitUID);
		}
		else
		{
			NKCPopupWarfareSelectShip.OpenForSupporterInWarfare(GetNKMWarfareGameData().supportUnitData, cNKCWarfareGameUnit.GetNKMWarfareUnitData().warfareGameUnitUID, OnCancelBatch);
		}
	}

	private void InvalidSelectedUnitPure()
	{
		m_LastClickedUnitTileIndex = -1;
		m_NKCWarfareGameHUD.CloseSelectedSquadUI();
	}

	public void InvalidSelectedUnit()
	{
		if (GetNKMWarfareGameData().warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_PLAYING && GetNKMWarfareGameData().isTurnA && !IsAutoWarfare())
		{
			InvalidSelectedUnitPure();
			SetTileDefaultWhenPlay();
			m_unitMgr.ResetIcon();
			CloseAssistFX();
			CancelRecovery();
		}
	}

	private void SetTileDefaultWhenPlay()
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return;
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		for (int i = 0; i < warfareGameData.warfareTileDataList.Count; i++)
		{
			WarfareTileData tileData = warfareGameData.GetTileData(i);
			if (tileData == null)
			{
				continue;
			}
			NKMWarfareTileTemplet tile = mapTemplet.GetTile(i);
			NKCWarfareGameTile tile2 = m_tileMgr.GetTile(i);
			if (tile2 == null || tile == null)
			{
				continue;
			}
			if (tileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE)
			{
				tile2.SetDummyActive(bSet: false);
				m_battleCondition.RemoveBattleCondition(i);
				continue;
			}
			tile2.SetTileLayer1Type(tileData.tileType);
			tile2.SetTileLayer2Type(tile.m_TileWinType, tile.m_TileLoseType);
			WarfareUnitData unitDataByTileIndex = warfareGameData.GetUnitDataByTileIndex(i);
			if (unitDataByTileIndex == null)
			{
				tile2.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_NORMAL);
			}
			else if (warfareGameData.CheckTeamA_By_GameUnitUID(unitDataByTileIndex.warfareGameUnitUID))
			{
				if (!warfareGameData.isTurnA)
				{
					tile2.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_USER_UNIT_TURN_FINISHED);
				}
				else if (unitDataByTileIndex.isTurnEnd)
				{
					if (m_LastClickedUnitTileIndex == i)
					{
						tile2.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_USER_UNIT_TURN_FINISHED_SELECTED);
					}
					else
					{
						tile2.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_USER_UNIT_TURN_FINISHED);
					}
				}
				else
				{
					tile2.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_MOVABLE_USER_UNIT);
				}
			}
			else
			{
				tile2.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_ENEMY);
			}
			m_battleCondition.SetBattleCondition(i, tileData.battleConditionId, m_listUnitPos[i]);
		}
	}

	private void UpdateLabel(bool bPlaying = true)
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return;
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		m_labelMgr.HideAll();
		for (int i = 0; i < mapTemplet.TileCount; i++)
		{
			NKMWarfareTileTemplet tile = mapTemplet.GetTile(i);
			WarfareTileData tileData = warfareGameData.GetTileData(i);
			if (tile == null || tileData == null)
			{
				continue;
			}
			if (bPlaying)
			{
				if (tileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE)
				{
					continue;
				}
				if (tile.m_TileWinType == WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD)
				{
					m_labelMgr.SetLabel(i, WARFARE_LABEL_TYPE.HOLD, m_listUnitPos[i]);
					int count = nKMWarfareTemplet.m_WFWinValue - warfareGameData.holdCount;
					m_labelMgr.SetText(i, count);
				}
			}
			NKM_WARFARE_MAP_TILE_TYPE nKM_WARFARE_MAP_TILE_TYPE = ((!bPlaying) ? tile.m_TileType : tileData.tileType);
			if (nKM_WARFARE_MAP_TILE_TYPE != NKM_WARFARE_MAP_TILE_TYPE.NWMTT_INCR || tile.m_SummonCondition != WARFARE_SUMMON_CONDITION.PHASE)
			{
				continue;
			}
			int count2;
			if (bPlaying)
			{
				int num = warfareGameData.turnCount % tile.m_SummonConditionValue;
				count2 = tile.m_SummonConditionValue - num;
				if (num == 0)
				{
					count2 = 0;
				}
			}
			else
			{
				count2 = tile.m_SummonConditionValue - 1;
			}
			m_labelMgr.SetLabel(i, WARFARE_LABEL_TYPE.SUMMON, m_listUnitPos[i]);
			m_labelMgr.SetText(i, count2);
		}
	}

	private void SetTilePossibleArrival_(int i, int j, NKMWarfareMapTemplet cNKMWarfareMapTemplet, int selectedTileIndex, bool bRetreat)
	{
		int indexByPos = cNKMWarfareMapTemplet.GetIndexByPos(i, j);
		if (indexByPos == -1)
		{
			return;
		}
		NKMWarfareTileTemplet tile = cNKMWarfareMapTemplet.GetTile(indexByPos);
		if (tile == null)
		{
			return;
		}
		WarfareTileData tileData = GetNKMWarfareGameData().GetTileData(indexByPos);
		if (tileData == null || tileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE)
		{
			return;
		}
		m_tileMgr.SetTileLayer0Type(indexByPos, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_USER_UNIT_POSSIBLE_ARRIVAL);
		NKCWarfareGameUnit gameUnitByTileIndex = m_unitMgr.GetGameUnitByTileIndex(indexByPos);
		if (gameUnitByTileIndex == null)
		{
			return;
		}
		if (gameUnitByTileIndex.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.Dungeon)
		{
			gameUnitByTileIndex.SetAttackIcon(bActive: true, bRetreat);
			{
				foreach (short neighborTile in tile.NeighborTiles)
				{
					if (neighborTile != selectedTileIndex && neighborTile != indexByPos)
					{
						NKCWarfareGameUnitInfo wFGameUnitInfoByTileIndex = m_unitMgr.GetWFGameUnitInfoByTileIndex(neighborTile);
						if (!(wFGameUnitInfoByTileIndex == null) && wFGameUnitInfoByTileIndex.GetNKMWarfareUnitData().unitType != WarfareUnitData.Type.Dungeon)
						{
							wFGameUnitInfoByTileIndex.SetBattleAssistIcon(bActive: true);
							AddAssistFX(m_listUnitPos[neighborTile], m_listUnitPos[indexByPos]);
						}
					}
				}
				return;
			}
		}
		gameUnitByTileIndex.SetChangeIcon(bActive: true);
	}

	private void SetTilePossibleArrival(int selectedTileIndex, NKM_UNIT_STYLE_TYPE eNKM_UNIT_STYLE_TYPE, bool bRetreat)
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return;
		}
		int posXByIndex = mapTemplet.GetPosXByIndex(selectedTileIndex);
		int posYByIndex = mapTemplet.GetPosYByIndex(selectedTileIndex);
		MovableTileSet tileSet = MovableTileSet.GetTileSet(eNKM_UNIT_STYLE_TYPE);
		int num = 0;
		int num2 = 2;
		for (int i = posXByIndex - num2; i <= posXByIndex + num2; i++)
		{
			int num3 = 0;
			for (int j = posYByIndex - num2; j <= posYByIndex + num2; j++)
			{
				if (tileSet[num3, num])
				{
					SetTilePossibleArrival_(i, j, mapTemplet, selectedTileIndex, bRetreat);
				}
				num3++;
			}
			num++;
		}
	}

	private bool CheckWFGameStartCond(out NKMPacket_WARFARE_GAME_START_REQ startReq)
	{
		startReq = new NKMPacket_WARFARE_GAME_START_REQ
		{
			warfareTempletID = NKCWarfareManager.GetWarfareID(m_WarfareStrID)
		};
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return false;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return false;
		}
		m_unitMgr.OnClickGameStart(startReq, mapTemplet);
		m_unitMgr.ResetAllDeckState();
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCWarfareManager.CheckWFGameStartCond(NKCScenManager.GetScenManager().GetMyUserData(), startReq);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(nKM_ERROR_CODE.ToString()));
			return false;
		}
		startReq.rewardMultiply = m_NKCWarfareGameHUD.GetCurrMultiplyRewardCount();
		return true;
	}

	private void reqGameStart()
	{
		if (CheckWFGameStartCond(out var startReq))
		{
			if (m_unitMgr != null)
			{
				m_unitMgr.OnStartGameVoice();
			}
			NKCPacketSender.Send_NKMPacket_WARFARE_GAME_START_REQ(startReq);
		}
	}

	private void SetPlayingCommonUI(bool bStart = false)
	{
		m_NKCWarfareGameHUD.Open();
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		m_NKCWarfareGameHUD.SetActiveTitle(bReadyTitle: false);
		m_NKCWarfareGameHUD.SetActiveBatchGuideText(bSet: false);
		m_NKCWarfareGameHUD.SetActiveBatchSupportGuideText(bSet: false);
		m_NKCWarfareGameHUD.SetActiveOperationBtn(bSet: false);
		m_NKCWarfareGameHUD.SetActiveTurnFinishBtn(warfareGameData.isTurnA && !IsAutoWarfare());
		m_NKCWarfareGameHUD.SetActiveBatchCountUI(bSet: false);
		m_NKCWarfareGameHUD.SetActivePhase(bSet: true);
		m_NKCWarfareGameHUD.SetActiveDeco(bSet: false);
		m_NKCWarfareGameHUD.SetActiveAuto(IsAutoVisible(), IsAutoUsable());
		m_NKCWarfareGameHUD.SetActiveRepeatOperation(IsRepeatOperationVisible());
		m_NKCWarfareGameHUD.SetActiveAutoOnOff(IsAutoWarfare(), IsAutoWarfareSupply());
		m_NKCWarfareGameHUD.SetActivePause(bSet: true);
		m_NKCWarfareGameHUD.SetTurnCount(warfareGameData.turnCount);
		m_NKCWarfareGameHUD.SetPhaseUserType(warfareGameData.isTurnA);
		m_NKCWarfareGameHUD.HideOperationEnterLimit();
		m_NKCWarfareGameHUD.UpdateMultiplyUI();
		SetBG(bReadyBG: false, bStart);
		UpdateUpsideMenu();
	}

	private void PrepareWarfareGameIntrude()
	{
		PlayWarfareOnGoingMusic();
		SetPlayingCommonUI();
		m_NKCWarfareGameHUD.SetUpperRightMenuPosition(bPlaying: true);
		NKCCamera.GetCamera().orthographic = false;
		NKCCamera.SetPos(0f, -308f, GetFinalCameraZDist(), bTrackingStop: true, bForce: true);
		NKCCamera.GetTrackingRotation().SetNowValue(-20f, 0f, 0f);
		DoWhenMapSizeCalculated();
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null || nKMWarfareTemplet.MapTemplet == null)
		{
			return;
		}
		List<WarfareUnitData> unitDataList = warfareGameData.GetUnitDataList();
		for (int i = 0; i < unitDataList.Count; i++)
		{
			WarfareUnitData warfareUnitData = unitDataList[i];
			if (warfareUnitData.hp <= 0f)
			{
				continue;
			}
			if (warfareGameData.CheckTeamA_By_GameUnitUID(warfareUnitData.warfareGameUnitUID))
			{
				NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.CreateNewUserUnit(warfareUnitData.deckIndex, warfareUnitData.tileIndex, OnClickUnit, warfareUnitData, warfareUnitData.friendCode);
				if (nKCWarfareGameUnit != null)
				{
					nKCWarfareGameUnit.gameObject.transform.localPosition = m_listUnitPos[warfareUnitData.tileIndex];
					nKCWarfareGameUnit.SetNKMWarfareUnitData(warfareUnitData);
				}
			}
			else
			{
				NKCWarfareGameUnit nKCWarfareGameUnit2 = m_unitMgr.CreateNewEnemyUnit(NKMDungeonManager.GetDungeonStrID(warfareUnitData.dungeonID), warfareGameData.warfareTeamDataB.flagShipWarfareUnitUID == warfareUnitData.warfareGameUnitUID, warfareUnitData.isTarget, warfareUnitData.tileIndex, warfareUnitData.warfareEnemyActionType, OnClickUnit, warfareUnitData);
				if (nKCWarfareGameUnit2 != null)
				{
					nKCWarfareGameUnit2.gameObject.transform.localPosition = m_listUnitPos[warfareUnitData.tileIndex];
					nKCWarfareGameUnit2.SetNKMWarfareUnitData(warfareUnitData);
				}
			}
			if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAY_TRY || warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_INGAME_PLAY_TRY_READY)
			{
				CheckAndAddBattleEnemy(warfareUnitData.warfareGameUnitUID, warfareUnitData.tileIndex);
			}
		}
		m_unitMgr.SetUserFlagShip(warfareGameData.warfareTeamDataA.flagShipWarfareUnitUID);
		m_unitMgr.SetFlagDungeon(warfareGameData.warfareTeamDataB.flagShipWarfareUnitUID);
		m_NKCWarfareGameHUD.SetRemainTurnOnUnitCount(m_unitMgr.GetRemainTurnOnUserUnitCount());
		UpdateRecoveryCount();
		SetTileDefaultWhenPlay();
		UpdateLabel();
	}

	private void RefreshContainerWhenOpen(bool bBattle)
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		m_containerMgr.HideAll();
		int num = m_DataBeforeBattle?.PrevContainerCount ?? 0;
		if (bBattle && num < warfareGameData.containerCount)
		{
			m_NKCWarfareGameHUD.SetContainerCount(num);
		}
		else
		{
			m_NKCWarfareGameHUD.SetContainerCount(warfareGameData.containerCount);
		}
		int i;
		for (i = 0; i < warfareGameData.warfareTileDataList.Count; i++)
		{
			WarfareTileData tileData = warfareGameData.GetTileData(i);
			if (tileData == null)
			{
				continue;
			}
			bool flag = warfareGameData.GetUnitDataByTileIndex_TeamB(i) != null;
			if (!flag && bBattle)
			{
				flag = warfareGameData.GetUnitDataByTileIndex_TeamA(i) != null;
			}
			if (m_DataBeforeBattle != null)
			{
				WarfareTileData warfareTileData = m_DataBeforeBattle.PrevTiles.Find((WarfareTileData v) => v.index == i);
				if (warfareTileData != null && warfareTileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWNTT_CHEST && tileData.tileType != warfareTileData.tileType)
				{
					m_containerMgr.Set(i, WARFARE_ITEM_STATE.Item, m_listUnitPos[i], flag);
					continue;
				}
			}
			if (tileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWNTT_CHEST)
			{
				m_containerMgr.Set(i, WARFARE_ITEM_STATE.Item, m_listUnitPos[i], flag);
			}
		}
		m_DataBeforeBattle = null;
	}

	private void PrepareWarfareGameStart()
	{
		PlayWarfareOnGoingMusic();
		SetPlayingCommonUI(bStart: true);
		m_NKCWarfareGameHUD.SetUpperRightMenuPosition(bPlaying: true);
		if (m_NUM_WARFARE_FX_SHIP_DIVE != null)
		{
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_SHIP_DIVE, bValue: false);
		}
		if (m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL != null)
		{
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL, bValue: false);
		}
		SetCamDefaultWhenPlaying();
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		m_NKCWarfareGameHUD.TriggerPlayerTurnUI();
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null || nKMWarfareTemplet.MapTemplet == null)
		{
			return;
		}
		foreach (WarfareUnitData value in warfareGameData.warfareTeamDataA.warfareUnitDataByUIDMap.Values)
		{
			NKMDeckData deckData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(value.deckIndex);
			if (deckData != null)
			{
				deckData.SetState(NKM_DECK_STATE.DECK_STATE_WARFARE);
				NKCScenManager.CurrentUserData().m_ArmyData.DeckUpdated(value.deckIndex, deckData);
			}
		}
		m_containerMgr.HideAll();
		for (int i = 0; i < warfareGameData.warfareTileDataList.Count; i++)
		{
			WarfareTileData tileData = warfareGameData.GetTileData(i);
			SetActiveDivePoint(i, bSet: false);
			SetActiveAssultPoint(i, bSet: false);
			NKCWarfareGameTile tile = m_tileMgr.GetTile(i);
			if (tile == null || tileData == null)
			{
				continue;
			}
			if (tileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE)
			{
				tile.SetDummyActive(bSet: false);
				continue;
			}
			tile.SetDummyActive(bSet: true);
			tile.SetTileLayer1Type(tileData.tileType);
			bool bWithEnemy = warfareGameData.GetUnitDataByTileIndex_TeamB(i) != null;
			if (tileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWNTT_CHEST)
			{
				m_containerMgr.Set(i, WARFARE_ITEM_STATE.Item, m_listUnitPos[i], bWithEnemy);
			}
			WarfareUnitData unitDataByTileIndex = warfareGameData.GetUnitDataByTileIndex(i);
			if (unitDataByTileIndex == null)
			{
				tile.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_NORMAL);
				continue;
			}
			if (warfareGameData.CheckTeamA_By_GameUnitUID(unitDataByTileIndex.warfareGameUnitUID))
			{
				_ = warfareGameData.warfareTeamDataA;
				tile.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_MOVABLE_USER_UNIT);
			}
			else
			{
				_ = warfareGameData.warfareTeamDataB;
				tile.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_ENEMY);
			}
			NKCWarfareGameUnit gameUnitByTileIndex = m_unitMgr.GetGameUnitByTileIndex(unitDataByTileIndex.tileIndex);
			if (gameUnitByTileIndex != null)
			{
				gameUnitByTileIndex.SetNKMWarfareUnitData(unitDataByTileIndex);
			}
			NKCWarfareGameUnitInfo wFGameUnitInfoByTileIndex = m_unitMgr.GetWFGameUnitInfoByTileIndex(unitDataByTileIndex.tileIndex);
			if (wFGameUnitInfoByTileIndex != null)
			{
				wFGameUnitInfoByTileIndex.SetNKMWarfareUnitData(unitDataByTileIndex);
			}
		}
		m_unitMgr.RefreshDicUnit();
		if (m_NUM_WARFARE_BG_IMG_B != null)
		{
			Vector3 localPosition = m_NUM_WARFARE_BG_IMG_B.transform.localPosition;
			localPosition.z = 0f;
			m_NUM_WARFARE_BG_IMG_B.transform.localPosition = localPosition;
			m_NUM_WARFARE_BG_IMG_B.transform.DOMoveZ(-200f, 2f).From(isRelative: true).SetEase(Ease.OutExpo);
		}
		m_NKCWarfareGameHUD.SetRemainTurnOnUnitCount(m_unitMgr.GetRemainTurnOnUserUnitCount());
		UpdateRecoveryCount();
		m_NKCWarfareGameHUD.UpdateMedalInfo();
		m_NKCWarfareGameHUD.UpdateWinCondition();
		m_unitMgr.UpdateGameUnitUI();
		UpdateLabel(bPlaying: false);
		m_NKCWarfareGameHUD.UpdateMultiplyUI();
	}

	private void DoWhenMapSizeCalculated()
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return;
		}
		int tileCount = mapTemplet.TileCount;
		m_tileMgr.SetTileActive(tileCount);
		m_NUM_WARFARE_TILE_Panel_GLG.constraintCount = mapTemplet.m_MapSizeY;
		Canvas.ForceUpdateCanvases();
		for (int i = 0; i < tileCount; i++)
		{
			NKCWarfareGameTile tile = m_tileMgr.GetTile(i);
			if (tile != null)
			{
				m_listUnitPos[i] = tile.transform.localPosition;
			}
		}
		for (int j = 0; j < m_listDivePoint.Count; j++)
		{
			if (m_listDivePoint[j] != null)
			{
				NKCUtil.SetGameobjectActive(m_listDivePoint[j], bValue: false);
			}
		}
		for (int k = 0; k < m_listAssultPoint.Count; k++)
		{
			if (m_listAssultPoint[k] != null)
			{
				NKCUtil.SetGameobjectActive(m_listAssultPoint[k], bValue: false);
			}
		}
		if (tileCount > 0)
		{
			m_rtCamBound = new Rect
			{
				xMin = m_listUnitPos[0].x,
				xMax = m_listUnitPos[tileCount - 1].x,
				yMax = m_listUnitPos[0].y + -250f,
				yMin = m_listUnitPos[tileCount - 1].y + -250f
			};
		}
		float newSize = Math.Abs(m_rtCamBound.xMax - m_rtCamBound.xMin) + 500f;
		float newSize2 = Math.Abs(m_rtCamBound.yMax - m_rtCamBound.yMin) + 500f;
		m_NUM_WARFARE_BG_WARBOX_A.SetWidth(newSize);
		m_NUM_WARFARE_BG_WARBOX_A.SetHeight(newSize2);
		m_NUM_WARFARE_BG_WARBOX_B.SetWidth(newSize);
		m_NUM_WARFARE_BG_WARBOX_B.SetHeight(newSize2);
	}

	private void PrepareWarfareGameReady()
	{
		SetBG(bReadyBG: true);
		m_CanvasGroup.alpha = m_fStartAlhpa;
		m_CanvasGroup.DOFade(1f, m_fFadeTime).SetEase(Ease.InSine);
		m_NKCWarfareGameHUD.SetActiveTitle(bReadyTitle: true);
		m_NKCWarfareGameHUD.SetActiveBatchGuideText(bSet: true);
		m_NKCWarfareGameHUD.SetActiveBatchCountUI(bSet: true);
		m_NKCWarfareGameHUD.SetActivePhase(bSet: false);
		m_NKCWarfareGameHUD.SetActiveAuto(IsAutoVisible(), IsAutoUsable());
		m_NKCWarfareGameHUD.SetActiveRepeatOperation(IsRepeatOperationVisible());
		m_NKCWarfareGameHUD.SetActiveAutoOnOff(IsAutoWarfare() && IsAutoUsable(), IsAutoWarfareSupply());
		if (!IsAutoUsable() && NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bAutoWarfare)
		{
			Debug.Log("NKMPacket_WARFARE_GAME_AUTO_REQ - NKCWarfareGame.PrepareWarfareGameReady");
			SetPause(bSet: true);
			WaitAutoPacket = true;
			NKCPacketSender.Send_NKMPacket_WARFARE_GAME_AUTO_REQ(bAuto: false);
		}
		m_NKCWarfareGameHUD.SetUpperRightMenuPosition(bPlaying: false);
		m_NKCWarfareGameHUD.SetActiveDeco(bSet: true);
		m_NKCWarfareGameHUD.SetActivePause(bSet: false);
		m_NKCWarfareGameHUD.SetActiveTurnFinishBtn(bSet: false);
		NKCCamera.GetCamera().orthographic = false;
		SetCameraIntro();
		m_unitMgr.ClearUnits();
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return;
		}
		int num = 0;
		DoWhenMapSizeCalculated();
		PreProcessSpawnPoint();
		m_labelMgr.HideAll();
		m_containerMgr.HideAll();
		int num2 = 0;
		for (num = 0; num < mapTemplet.TileCount; num++)
		{
			NKMWarfareTileTemplet tile = mapTemplet.GetTile(num);
			NKCWarfareGameTile tile2 = m_tileMgr.GetTile(num);
			if (tile == null || tile2 == null)
			{
				continue;
			}
			if (tile.m_TileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE)
			{
				tile2.SetDummyActive(bSet: false);
				SetActiveDivePoint(num, bSet: false);
				SetActiveAssultPoint(num, bSet: false);
				continue;
			}
			tile2.SetDummyActive(bSet: true);
			tile2.SetTileLayer1Type(tile.m_TileType);
			tile2.SetTileLayer2Type(tile.m_TileWinType, tile.m_TileLoseType);
			if (tile.m_TileWinType == WARFARE_GAME_CONDITION.WFC_PHASE_TILE_HOLD)
			{
				m_labelMgr.SetLabel(num, WARFARE_LABEL_TYPE.HOLD, m_listUnitPos[num]);
				int wFWinValue = nKMWarfareTemplet.m_WFWinValue;
				m_labelMgr.SetText(num, wFWinValue);
			}
			bool flag = !string.IsNullOrEmpty(tile.m_DungeonStrID);
			if (tile.m_TileType == NKM_WARFARE_MAP_TILE_TYPE.NWNTT_CHEST)
			{
				m_containerMgr.Set(num, WARFARE_ITEM_STATE.Item, m_listUnitPos[num], flag);
			}
			else if (tile.NeedQuestionMark())
			{
				m_containerMgr.Set(num, WARFARE_ITEM_STATE.Question, m_listUnitPos[num], flag);
			}
			if (flag)
			{
				bool bFlag = false;
				if (tile.m_bFlagDungeon && nKMWarfareTemplet.m_WFWinCondition == WARFARE_GAME_CONDITION.WFC_KILL_BOSS)
				{
					bFlag = true;
				}
				NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.CreateNewEnemyUnit(tile.m_DungeonStrID, bFlag, tile.m_bTargetUnit, (short)num, tile.m_NKM_WARFARE_ENEMY_ACTION_TYPE, OnClickUnit);
				if (nKCWarfareGameUnit != null)
				{
					nKCWarfareGameUnit.gameObject.transform.localPosition = m_listUnitPos[num];
				}
				tile2.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_ENEMY);
			}
			else
			{
				bool flag2 = false;
				bool flag3 = false;
				if (SetActiveDivePoint(num, bSet: true))
				{
					flag2 = true;
				}
				if (SetActiveAssultPoint(num, bSet: true))
				{
					flag3 = true;
				}
				if (flag2 || flag3)
				{
					if (flag2)
					{
						tile2.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_DIVE_POINT);
					}
					else if (flag3)
					{
						tile2.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_ASSULT_POINT);
					}
					GameObject gameObject = GetDivePointGO(num);
					if (gameObject == null)
					{
						gameObject = GetAssultPointGO(num);
					}
					AnimateSpawnPoint(gameObject, 0.75f + (float)num2 * 0.1f);
					num2++;
				}
				else
				{
					tile2.SetTileLayer0Type(NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_NORMAL);
				}
			}
			if (tile.BattleCondition != null)
			{
				m_battleCondition.SetBattleCondition(num, tile.BattleCondition.BattleCondID, m_listUnitPos[num]);
			}
		}
		m_NKCWarfareGameHUD.Open();
		m_NKCWarfareGameHUD.SetBatchedShipCount(0);
		if (ProcessRetry() && NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
		{
			reqGameStart();
		}
		UpdateLabel(bPlaying: false);
		CheckTutorial();
	}

	public override void CloseInternal()
	{
		OnCloseGameOption();
		m_bReservedBattle = false;
		if (m_twUnitInfoDie != null && m_twUnitInfoDie.IsActive())
		{
			m_twUnitInfoDie.Kill();
		}
		m_twUnitInfoDie = null;
		if (m_UnitDieSequence != null)
		{
			m_UnitDieSequence.Kill();
		}
		m_UnitDieSequence = null;
		m_WarfareGameUnitToDie = null;
		CloseAssistFX();
		if (m_NUM_WARFARE_BG_IMG_B != null)
		{
			m_NUM_WARFARE_BG_IMG_B.transform.DOKill();
		}
		if (m_listDivePoint != null)
		{
			for (int i = 0; i < m_listDivePoint.Count; i++)
			{
				m_listDivePoint[i].transform.DOKill();
			}
		}
		if (m_listAssultPoint != null)
		{
			for (int j = 0; j < m_listAssultPoint.Count; j++)
			{
				m_listAssultPoint[j].transform.DOKill();
			}
		}
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_SHIP_DIVE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION_BIG, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_ESCAPE, bValue: false);
		if (m_NUM_WARFARE.activeSelf)
		{
			m_NUM_WARFARE.SetActive(value: false);
		}
		for (int k = 0; k < m_listAssetInstance.Count; k++)
		{
			NKCAssetResourceManager.CloseInstance(m_listAssetInstance[k]);
		}
		m_listAssetInstance.Clear();
		m_unitMgr.ClearUnits();
		m_battleCondition.Close();
		m_containerMgr.Close();
		m_tileMgr.Close();
		m_NKCWarfareGameHUD.Close();
		NKCUIWarfareResult.CheckInstanceAndClose();
		NKCCamera.StopTrackingCamera();
		NKCCamera.GetTrackingRotation().SetNowValue(0f, 0f, 0f);
		NKCCamera.EnableBlur(bEnable: false);
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
		UnityEngine.Object.Destroy(m_NKCWarfareGameHUD.gameObject);
	}

	public void TempLeave()
	{
		if (!NKCScenManager.GetScenManager().GetNKCRepeatOperaion().CheckRepeatOperationRealStop())
		{
			NKCUIGameOption.CheckInstanceAndClose();
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().SetEpisodeID(bReservedDetailOption: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
		}
	}

	public void ResetGameOption()
	{
		if (GetPause())
		{
			NKCUIGameOption.CheckInstanceAndClose();
			NKCUIGameOption.Instance.Open(NKC_GAME_OPTION_MENU_TYPE.WARFARE, OnCloseGameOption);
		}
	}

	public void ForceBack()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().SetEpisodeID(bReservedDetailOption: true);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
	}

	private bool IsAutoUsable()
	{
		return IsAutoVisible();
	}

	private bool IsAutoVisible()
	{
		return NKCContentManager.IsContentsUnlocked(ContentsType.WARFARE_AUTO_MOVE);
	}

	public bool IsAutoWarfare()
	{
		return NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bAutoWarfare;
	}

	public bool IsAutoWarfareSupply()
	{
		return NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bAutoWarfareRepair;
	}

	private bool IsRepeatOperationVisible()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_REPEAT))
		{
			return false;
		}
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.WARFARE_REPEAT))
		{
			return false;
		}
		if (NKCScenManager.GetScenManager().WarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP && NKCScenManager.GetScenManager().WarfareGameData.rewardMultiply > 1)
		{
			return false;
		}
		return true;
	}

	public void SetUserUnitDeckWarfareState()
	{
		m_unitMgr.SetUserUnitDeckWarfareState();
	}

	private void PlayUnitDieAni()
	{
		if (!(m_WarfareGameUnitToDie == null))
		{
			m_UnitDieSequence = DOTween.Sequence();
			m_WarfareGameUnitToDie.PlayDieAni();
			m_UnitDieSequence.Append(m_WarfareGameUnitToDie.transform.DOShakePosition(2f, 30f, 30));
			m_UnitDieSequence.AppendCallback(FadeOutUnitInfoToDie);
			m_UnitDieSequence.Append(m_WarfareGameUnitToDie.GetComponent<CanvasGroup>().DOFade(0f, 0.6f));
			Vector3 vector = m_listUnitPos[m_WarfareGameUnitToDie.TileIndex];
			NKCCamera.SetPos(vector.x, vector.y + -250f, GetFinalCameraZDist(), bTrackingStop: true, bForce: true);
			bool flag = false;
			NKCWarfareGameUnitInfo wFGameUnitInfoByWFUnitData = m_unitMgr.GetWFGameUnitInfoByWFUnitData(m_WarfareGameUnitToDie.GetNKMWarfareUnitData());
			if (wFGameUnitInfoByWFUnitData != null)
			{
				flag = wFGameUnitInfoByWFUnitData.GetFlag();
			}
			if (!flag)
			{
				m_NUM_WARFARE_FX_UNIT_EXPLOSION.transform.localPosition = m_WarfareGameUnitToDie.transform.localPosition;
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION, bValue: false);
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION, bValue: true);
			}
			else
			{
				m_NUM_WARFARE_FX_UNIT_EXPLOSION_BIG.transform.localPosition = m_WarfareGameUnitToDie.transform.localPosition;
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION_BIG, bValue: false);
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION_BIG, bValue: true);
			}
		}
	}

	private void PlayUnitRunawayAni()
	{
		if (m_WarfareGameUnitToDie == null)
		{
			return;
		}
		m_WarfareGameUnitToDie.PlayAni(NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_ANIMATION.NWGUA_RUNAWAY);
		bool flag = false;
		NKCWarfareGameUnitInfo wFGameUnitInfoByWFUnitData = m_unitMgr.GetWFGameUnitInfoByWFUnitData(m_WarfareGameUnitToDie.GetNKMWarfareUnitData());
		if (wFGameUnitInfoByWFUnitData != null)
		{
			flag = wFGameUnitInfoByWFUnitData.GetFlag();
			wFGameUnitInfoByWFUnitData.PlayAni(NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_ANIMATION.NWGUA_RUNAWAY);
		}
		if (flag)
		{
			m_NUM_WARFARE_FX_UNIT_ESCAPE.transform.localPosition = m_WarfareGameUnitToDie.transform.localPosition;
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_ESCAPE, bValue: false);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_ESCAPE, bValue: true);
			NKCSoundManager.PlaySound("FX_UI_WARFARE_RUNAWAY", 1f, 0f, 0f);
			if (m_WarfareGameUnitToDie.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User)
			{
				NKCOperatorUtil.PlayVoice(m_WarfareGameUnitToDie.GetNKMWarfareUnitData().deckIndex, VOICE_TYPE.VT_BACK_LACK);
			}
		}
	}

	private void FadeOutUnitInfoToDie()
	{
		if (!(m_WarfareGameUnitToDie != null))
		{
			return;
		}
		NKCWarfareGameUnitInfo wFGameUnitInfoByWFUnitData = m_unitMgr.GetWFGameUnitInfoByWFUnitData(m_WarfareGameUnitToDie.GetNKMWarfareUnitData());
		if (wFGameUnitInfoByWFUnitData != null)
		{
			if (m_twUnitInfoDie != null && m_twUnitInfoDie.IsActive())
			{
				m_twUnitInfoDie.Kill();
			}
			m_twUnitInfoDie = wFGameUnitInfoByWFUnitData.GetComponent<CanvasGroup>().DOFade(0f, 0.57000005f);
		}
	}

	private void OnUnitShakeEnd()
	{
		if (!m_bReservedCallOnUnitShakeEnd)
		{
			return;
		}
		m_bReservedCallOnUnitShakeEnd = false;
		Debug.Log("WarfareUnitShake End");
		if (m_UnitDieSequence != null)
		{
			m_UnitDieSequence.Kill();
		}
		m_UnitDieSequence = null;
		if (m_WarfareGameUnitToDie != null)
		{
			int tileIndex = m_WarfareGameUnitToDie.TileIndex;
			WarfareUnitData nKMWarfareUnitData = m_WarfareGameUnitToDie.GetNKMWarfareUnitData();
			m_unitMgr.ClearUnit(nKMWarfareUnitData.warfareGameUnitUID);
			m_WarfareGameUnitToDie = null;
			NKCWarfareGameUnit gameUnitByTileIndex = m_unitMgr.GetGameUnitByTileIndex(tileIndex);
			if (gameUnitByTileIndex != null)
			{
				Debug.Log("MoveAfterShake, tileIndex : " + tileIndex);
				if (gameUnitByTileIndex.gameObject.activeInHierarchy)
				{
					gameUnitByTileIndex.Move(m_listUnitPos[tileIndex], 0.9f, OnReturnMoveEnd);
				}
				gameUnitByTileIndex.SetPause(GetPause());
				if (NKCGameEventManager.IsWaiting())
				{
					NKCGameEventManager.WaitFinished();
				}
			}
			else
			{
				Debug.LogWarning("WarfareGameUnitToDie 타일에 혼자인데 죽음, tileIndex : " + tileIndex);
				OnReturnMoveEnd(null);
			}
		}
		else
		{
			Debug.LogError("WarfareGameUnitToDie is null");
			OnReturnMoveEnd(null);
		}
	}

	private void OnReturnMoveEnd(NKCWarfareGameUnit cNKCWarfareGameUnit)
	{
		if (cNKCWarfareGameUnit != null && cNKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User)
		{
			if (m_containerMgr.IsItem(cNKCWarfareGameUnit.TileIndex))
			{
				PlayGetContainer(cNKCWarfareGameUnit.TileIndex, bWithEnemy: true);
			}
			if (NKCWarfareManager.UseServiceType != NKM_WARFARE_SERVICE_TYPE.NWST_NONE)
			{
				UseServiceFX(cNKCWarfareGameUnit, NKCWarfareManager.UseServiceType);
			}
			if (CheckEnterUnit(cNKCWarfareGameUnit.GetNKMWarfareUnitData().warfareGameUnitUID))
			{
				PlayEnterAni();
				return;
			}
		}
		OpenActionWhenNotStop();
	}

	private void UseServiceFX(NKCWarfareGameUnit gameUnit, NKM_WARFARE_SERVICE_TYPE seriveType)
	{
		switch (seriveType)
		{
		case NKM_WARFARE_SERVICE_TYPE.NWST_REPAIR:
			gameUnit.TriggerRepairFX();
			break;
		case NKM_WARFARE_SERVICE_TYPE.NWST_RESUPPLY:
			gameUnit.TriggerSupplyFX();
			break;
		}
		NKCWarfareManager.ResetServiceType();
	}

	private bool CheckEnterUnit(int unitUID)
	{
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		WarfareUnitData unitData = nKMWarfareGameData.GetUnitData(unitUID);
		if (unitData == null)
		{
			return false;
		}
		if (unitData.unitType != WarfareUnitData.Type.User)
		{
			return false;
		}
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(unitUID);
		if (nKCWarfareGameUnit == null)
		{
			return false;
		}
		int tileIndex = nKCWarfareGameUnit.TileIndex;
		if (!m_tileMgr.IsTileLayer2Type(tileIndex, NKCWarfareGameTile.WARFARE_TILE_LAYER_2_TYPE.WTL2T_WIN_ENTER))
		{
			return false;
		}
		if (nKMWarfareGameData.GetTileData(tileIndex) == null)
		{
			return false;
		}
		m_LastEnterUnitUID = unitUID;
		if (nKMWarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_RESULT)
		{
			return true;
		}
		return false;
	}

	private void PlayEnterAni()
	{
		if (m_LastEnterUnitUID <= 0)
		{
			return;
		}
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(m_LastEnterUnitUID);
		if (nKCWarfareGameUnit == null)
		{
			Debug.LogError("NKCWarfareGameUnit이 없음 - " + m_LastEnterUnitUID);
			return;
		}
		m_bReservedCallNextOrderREQ = true;
		nKCWarfareGameUnit.PlayAni(NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_ANIMATION.NWGUA_ENTER, OnCompleteEnterAni);
		NKCWarfareGameUnitInfo wFGameUnitInfoByWFUnitData = m_unitMgr.GetWFGameUnitInfoByWFUnitData(nKCWarfareGameUnit.GetNKMWarfareUnitData());
		if (wFGameUnitInfoByWFUnitData != null)
		{
			wFGameUnitInfoByWFUnitData.PlayAni(NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_ANIMATION.NWGUA_ENTER);
		}
		m_NUM_WARFARE_FX_UNIT_ESCAPE.transform.localPosition = nKCWarfareGameUnit.transform.localPosition;
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_ESCAPE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_ESCAPE, bValue: true);
	}

	private void OnCompleteEnterAni()
	{
		if (!m_bReservedCallNextOrderREQ)
		{
			return;
		}
		m_bReservedCallNextOrderREQ = false;
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(m_LastEnterUnitUID);
		if (nKCWarfareGameUnit == null)
		{
			Debug.LogError("NKCWarfareGameUnit이 없음 - " + m_LastEnterUnitUID);
			return;
		}
		m_unitMgr.ClearUnit(m_LastEnterUnitUID);
		m_LastEnterUnitUID = 0;
		if (nKMWarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_RESULT)
		{
			Debug.Log("warfare next order REQ - OnCompleteEnterAni");
			SendGetNextOrderREQ(nKCWarfareGameUnit);
		}
	}

	private void DoMove(byte fromTileIndex, byte toTileIndex)
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCWarfareManager.CheckMoveCond(NKCScenManager.GetScenManager().GetMyUserData(), fromTileIndex, toTileIndex);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(nKM_ERROR_CODE.ToString()));
			return;
		}
		SetPause(bSet: true);
		NKCPacketSender.Send_NKMPacket_WARFARE_GAME_MOVE_REQ(fromTileIndex, toTileIndex);
	}

	private NKCWarfareGameUnit CheckAndAddBattleEnemy(int gameUnitUID, int tileIndex)
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		WarfareUnitData unitData = warfareGameData.GetUnitData(gameUnitUID);
		NKCWarfareGameUnit nKCWarfareGameUnit = null;
		if (unitData == null)
		{
			return null;
		}
		if (unitData.unitType == WarfareUnitData.Type.User && unitData.warfareGameUnitUID == warfareGameData.battleAllyUid)
		{
			WarfareUnitData unitData2 = warfareGameData.GetUnitData(warfareGameData.battleMonsterUid);
			if (unitData2 != null)
			{
				NKCWarfareGameUnit nKCWarfareGameUnit2 = m_unitMgr.CreateNewEnemyUnit(NKMDungeonManager.GetDungeonStrID(unitData2.dungeonID), warfareGameData.warfareTeamDataB.flagShipWarfareUnitUID == unitData2.warfareGameUnitUID, unitData2.isTarget, unitData2.tileIndex, unitData2.warfareEnemyActionType, OnClickUnit, unitData2);
				if (nKCWarfareGameUnit2 != null)
				{
					Vector3 localPosition = m_listUnitPos[tileIndex];
					localPosition.x += 100f;
					nKCWarfareGameUnit2.gameObject.transform.localPosition = localPosition;
					nKCWarfareGameUnit2.SetNKMWarfareUnitData(unitData2);
				}
				nKCWarfareGameUnit = nKCWarfareGameUnit2;
			}
		}
		else if (unitData.unitType == WarfareUnitData.Type.Dungeon && warfareGameData.battleMonsterUid == unitData.warfareGameUnitUID)
		{
			WarfareUnitData unitData3 = warfareGameData.GetUnitData(warfareGameData.battleAllyUid);
			if (unitData3 != null)
			{
				NKCWarfareGameUnit nKCWarfareGameUnit3 = m_unitMgr.CreateNewUserUnit(unitData3.deckIndex, unitData3.tileIndex, OnClickUnit, unitData3, unitData3.friendCode);
				if (nKCWarfareGameUnit3 != null)
				{
					Vector3 localPosition2 = m_listUnitPos[tileIndex];
					localPosition2.x -= 100f;
					nKCWarfareGameUnit3.gameObject.transform.localPosition = localPosition2;
					nKCWarfareGameUnit3.SetNKMWarfareUnitData(unitData3);
				}
				nKCWarfareGameUnit = nKCWarfareGameUnit3;
			}
		}
		if (nKCWarfareGameUnit != null)
		{
			NKCWarfareGameUnit nKCWarfareGameUnit4 = m_unitMgr.GetNKCWarfareGameUnit(gameUnitUID);
			if (nKCWarfareGameUnit4 != null)
			{
				Vector3 localPosition3 = nKCWarfareGameUnit4.gameObject.transform.localPosition;
				if (nKCWarfareGameUnit4.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User)
				{
					localPosition3.x -= 100f;
				}
				else
				{
					localPosition3.x += 100f;
				}
				nKCWarfareGameUnit4.gameObject.transform.localPosition = localPosition3;
			}
		}
		return nKCWarfareGameUnit;
	}

	public void UseRepairItem()
	{
		if (NKMItemManager.GetItemMiscTempletByID(2) == null)
		{
			return;
		}
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(GetSelectedSquadWFGUUID());
		if (!(nKCWarfareGameUnit == null))
		{
			if (nKCWarfareGameUnit.IsSupporter)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WARFARE_SUPPORTER_REPAIR);
			}
			else
			{
				NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_WARFARE_REPAIR_TITLE, NKCUtilString.GET_STRING_WARFARE_REPAIR_DESC, 2, 0, UseRepairItemConfirm);
			}
		}
	}

	private void UseRepairItemConfirm()
	{
		Send_NKMPacket_WARFARE_GAME_USE_SERVICE_REQ(GetSelectedSquadWFGUUID(), NKM_WARFARE_SERVICE_TYPE.NWST_REPAIR);
	}

	public void UseSupplyItem()
	{
		if (NKMItemManager.GetItemMiscTempletByID(2) == null)
		{
			return;
		}
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(GetSelectedSquadWFGUUID());
		if (nKCWarfareGameUnit == null)
		{
			return;
		}
		if (nKCWarfareGameUnit.IsSupporter)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WARFARE_SUPPORTER_SUPPLY);
			return;
		}
		int num = 0;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(GetNKMWarfareGameData().warfareTempletID);
		if (nKMWarfareTemplet != null)
		{
			NKMEpisodeMgr.FindStageTempletByBattleStrID(nKMWarfareTemplet.m_WarfareStrID);
		}
		int num2 = GetNKMWarfareGameData().rewardMultiply;
		if (num2 <= 0)
		{
			num2 = 1;
		}
		num *= num2;
		if (num <= 0)
		{
			Debug.LogError("전역 보급 비용이 이상합니다. warfareID : " + nKMWarfareTemplet.m_WarfareStrID);
			return;
		}
		string content = ((GetNKMWarfareGameData().rewardMultiply <= 1) ? NKCUtilString.GET_STRING_WARFARE_SUPPLY_DESC : NKCUtilString.GET_STRING_WARFARE_SUPPLY_DESC_MULTIPLY);
		NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_WARFARE_SUPPLY_TITLE, content, 2, num * m_NKCWarfareGameHUD.GetCurrMultiplyRewardCount(), UseSupplyItemConfirm);
	}

	private void UseSupplyItemConfirm()
	{
		Send_NKMPacket_WARFARE_GAME_USE_SERVICE_REQ(GetSelectedSquadWFGUUID(), NKM_WARFARE_SERVICE_TYPE.NWST_RESUPPLY);
	}

	private void Send_NKMPacket_WARFARE_GAME_USE_SERVICE_REQ(int warfareGameUnitUID, NKM_WARFARE_SERVICE_TYPE serviceType)
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCWarfareManager.CanTryServiceUse(NKCScenManager.GetScenManager().GetMyUserData(), NKCScenManager.GetScenManager().WarfareGameData, warfareGameUnitUID, serviceType);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			string errorMessage = NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE);
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, errorMessage);
		}
		else
		{
			SetPause(bSet: true);
			NKCPacketSender.Send_NKMPacket_WARFARE_GAME_USE_SERVICE_REQ(warfareGameUnitUID, serviceType);
		}
	}

	private void OnCancelBatch(int gameUnitUID)
	{
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(gameUnitUID);
		bool flag = false;
		bool flag2 = false;
		if (nKCWarfareGameUnit != null)
		{
			int tileIndex = nKCWarfareGameUnit.TileIndex;
			flag2 = nKCWarfareGameUnit.IsSupporter;
			if (m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL != null)
			{
				m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL.gameObject.transform.localPosition = m_listUnitPos[tileIndex];
				m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL.SetActive(value: false);
				m_NUM_WARFARE_FX_SHIP_DIVE_CANCEL.SetActive(value: true);
			}
			if (SetActiveDivePoint(tileIndex, bSet: true))
			{
				m_tileMgr.SetTileLayer0Type(tileIndex, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_DIVE_POINT);
			}
			if (SetActiveAssultPoint(tileIndex, bSet: true))
			{
				m_tileMgr.SetTileLayer0Type(tileIndex, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_ASSULT_POINT);
			}
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_unitMgr.GetNKCWarfareGameUnitInfo(gameUnitUID);
			if (nKCWarfareGameUnitInfo != null)
			{
				flag = nKCWarfareGameUnitInfo.GetFlag();
			}
			m_unitMgr.ClearUnit(gameUnitUID);
		}
		if (flag)
		{
			m_unitMgr.ResetUserFlagShip(bPlayAni: true);
		}
		m_NKCWarfareGameHUD.SetBatchedShipCount(m_unitMgr.GetCurrentUserUnit());
		m_NKCWarfareGameHUD.UpdateSupportShipTile(m_unitMgr.GetCurrentUserUnitTileIndex());
		UpdateAttackCostUI();
		if (m_unitMgr.GetCurrentUserUnit() + 1 == m_UserUnitMaxCount || flag2)
		{
			TurnOnAllAvaiableUserUnitSpawnPoint();
		}
		if (flag2)
		{
			m_NKCWarfareGameHUD.SetBatchedSupportShipCount(bSet: false);
		}
	}

	public void OnClickAssultPoint(int index)
	{
		if (!IsBatchMax())
		{
			OpenDeckView(NKCUIDeckViewer.DeckViewerMode.WarfareBatch_Assault);
			m_LastClickedSpawnPoint = index;
			m_Last_WARFARE_SPAWN_POINT_TYPE = NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT;
		}
	}

	public void OnClickDivePoint(int index)
	{
		if (!IsBatchMax())
		{
			OpenDeckView(NKCUIDeckViewer.DeckViewerMode.WarfareBatch);
			m_LastClickedSpawnPoint = index;
			m_Last_WARFARE_SPAWN_POINT_TYPE = NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE;
		}
	}

	public void OnClickWarfareBatch(NKMDeckIndex selectedDeckIndex, long supportUserUID = 0L)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (!NKCUtil.ProcessDeckErrorMsg(NKMMain.IsValidDeck(myUserData.m_ArmyData, selectedDeckIndex)))
		{
			return;
		}
		if (m_unitMgr.CheckDuplicateDeckIndex(selectedDeckIndex))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_START_BY_DUPLICATE_DECK_INDEX));
			return;
		}
		int num = -1;
		if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE)
		{
			num = GetDivePointTileIndex(m_LastClickedSpawnPoint);
		}
		else if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT)
		{
			num = GetAssultPointTileIndex(m_LastClickedSpawnPoint);
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return;
		}
		bool bAssultPoint = false;
		NKMWarfareTileTemplet tile = mapTemplet.GetTile(num);
		if (!NKCWarfareManager.CheckValidSpawnPoint(mapTemplet, tile, myUserData, selectedDeckIndex, out bAssultPoint))
		{
			if (bAssultPoint)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_ASSAULT_POSITION));
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_POSITION));
			}
			return;
		}
		if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE)
		{
			if (SetActiveDivePoint(num, bSet: false))
			{
				m_tileMgr.SetTileLayer0Type(num, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_NORMAL);
			}
		}
		else if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT && SetActiveAssultPoint(num, bSet: false))
		{
			m_tileMgr.SetTileLayer0Type(num, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_NORMAL);
		}
		if (m_NUM_WARFARE_FX_SHIP_DIVE != null)
		{
			m_NUM_WARFARE_FX_SHIP_DIVE.gameObject.transform.localPosition = m_listUnitPos[num];
			m_NUM_WARFARE_FX_SHIP_DIVE.SetActive(value: false);
			m_NUM_WARFARE_FX_SHIP_DIVE.SetActive(value: true);
		}
		NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(selectedDeckIndex);
		if (deckData != null)
		{
			deckData.SetState(NKM_DECK_STATE.DECK_STATE_WARFARE);
			myUserData.m_ArmyData.DeckUpdated(selectedDeckIndex, deckData);
		}
		NKCUIDeckViewer.Instance.Close();
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.CreateNewUserUnit(selectedDeckIndex, (short)num, OnClickUnit, null, 0L);
		if (!(nKCWarfareGameUnit == null))
		{
			nKCWarfareGameUnit.gameObject.transform.localPosition = m_listUnitPos[num];
			nKCWarfareGameUnit.transform.DOMove(BATCH_EFFECT_POS, 1.5f).SetEase(Ease.OutCubic).From(isRelative: true);
			if (deckData != null)
			{
				NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_BATTLE_READY, myUserData.m_ArmyData.GetUnitFromUID(deckData.GetLeaderUnitUID()));
			}
			if (!m_unitMgr.CheckExistFlagUserUnit())
			{
				m_unitMgr.SetUserFlagShip(nKCWarfareGameUnit.GetNKMWarfareUnitData().warfareGameUnitUID);
			}
			m_NKCWarfareGameHUD.SetBatchedShipCount(m_unitMgr.GetCurrentUserUnit());
			m_NKCWarfareGameHUD.UpdateSupportShipTile(m_unitMgr.GetCurrentUserUnitTileIndex());
			UpdateAttackCostUI();
			if (IsBatchMax())
			{
				TurnOffAllUserUnitSpawnPoint();
			}
		}
	}

	public NKM_ERROR_CODE CheckWarfareBatch(NKMDeckIndex selectedDeckIndex)
	{
		if (m_unitMgr.GetCurrentUserUnit() >= m_UserUnitMaxCount)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_START_BY_MAX_USER_UNIT_OVERFLOW;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	private bool IsBatchMax()
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (m_unitMgr.GetCurrentUserUnit() >= m_UserUnitMaxCount)
		{
			if (nKMWarfareTemplet.m_bFriendSummon)
			{
				return m_unitMgr.ContainSupporterUnit();
			}
			return true;
		}
		return false;
	}

	private bool ProcessRetry()
	{
		if (m_RetryData == null)
		{
			return false;
		}
		if (m_RetryData.WarfareStrID != m_WarfareStrID)
		{
			m_RetryData = null;
			return false;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMArmyData armyData = myUserData.m_ArmyData;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return false;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return false;
		}
		foreach (RetryData.UnitData unit in m_RetryData.UnitList)
		{
			NKMDeckIndex nKMDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, unit.DeckIndex);
			int tileIndex = unit.TileIndex;
			if (NKMMain.IsValidDeck(armyData, nKMDeckIndex) != NKM_ERROR_CODE.NEC_OK || m_unitMgr.CheckDuplicateDeckIndex(nKMDeckIndex))
			{
				continue;
			}
			NKMWarfareTileTemplet tile = mapTemplet.GetTile(tileIndex);
			if (!NKCWarfareManager.CheckValidSpawnPoint(mapTemplet, tile, myUserData, nKMDeckIndex, out var _) || (!SetActiveDivePoint(tileIndex, bSet: false) && !SetActiveAssultPoint(tileIndex, bSet: false)))
			{
				continue;
			}
			m_tileMgr.SetTileLayer0Type(tileIndex, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_NORMAL);
			NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(nKMDeckIndex);
			if (deckData != null)
			{
				deckData.SetState(NKM_DECK_STATE.DECK_STATE_WARFARE);
				myUserData.m_ArmyData.DeckUpdated(nKMDeckIndex, deckData);
				NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.CreateNewUserUnit(nKMDeckIndex, (short)tileIndex, OnClickUnit, null, 0L);
				if (!(nKCWarfareGameUnit == null))
				{
					nKCWarfareGameUnit.gameObject.transform.localPosition = m_listUnitPos[tileIndex];
				}
			}
		}
		NKCWarfareGameUnit wFGameUnitByDeckIndex = m_unitMgr.GetWFGameUnitByDeckIndex(new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, m_RetryData.FlagShipDeckIndex));
		if (wFGameUnitByDeckIndex != null)
		{
			m_unitMgr.SetUserFlagShip(wFGameUnitByDeckIndex.GetNKMWarfareUnitData().warfareGameUnitUID);
		}
		m_NKCWarfareGameHUD.SetBatchedShipCount(m_unitMgr.GetCurrentUserUnit());
		m_NKCWarfareGameHUD.UpdateSupportShipTile(m_unitMgr.GetCurrentUserUnitTileIndex());
		UpdateAttackCostUI();
		if (IsBatchMax())
		{
			TurnOffAllUserUnitSpawnPoint();
		}
		m_RetryData = null;
		return true;
	}

	public override void OnBackButton()
	{
		if (NKCUICutScenPlayer.IsInstanceOpen && NKCUICutScenPlayer.Instance.IsPlaying())
		{
			NKCUICutScenPlayer.Instance.StopWithCallBack();
		}
		else if (GetNKMWarfareGameData().warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			ForceBack();
		}
		else
		{
			OnClickPause();
		}
	}

	public bool OnClickGameStart(bool bRepeatOperation = false)
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return false;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(m_WarfareStrID);
		if (nKMStageTempletV == null)
		{
			return false;
		}
		int eternium = 0;
		int num;
		if (nKMStageTempletV.m_StageReqItemID > 0)
		{
			num = nKMStageTempletV.m_StageReqItemID;
			eternium = nKMStageTempletV.m_StageReqItemCount;
		}
		else
		{
			num = 2;
		}
		if (num == 2)
		{
			NKCCompanyBuff.SetDiscountOfEterniumInEnteringWarfare(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
		}
		if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(num) < eternium * m_NKCWarfareGameHUD.GetCurrMultiplyRewardCount())
		{
			NKCShopManager.OpenItemLackPopup(num, eternium);
			return false;
		}
		if (m_NKCWarfareGameHUD.GetCurrMultiplyRewardCount() > 1)
		{
			NKMRewardMultiplyTemplet.RewardMultiplyItem costItem = NKMRewardMultiplyTemplet.GetCostItem(NKMRewardMultiplyTemplet.ScopeType.General);
			int num2 = costItem.MiscItemCount * (m_NKCWarfareGameHUD.GetCurrMultiplyRewardCount() - 1);
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(costItem.MiscItemId) < num2)
			{
				NKCShopManager.OpenItemLackPopup(costItem.MiscItemId, num2);
				return false;
			}
		}
		if (!CheckWFGameStartCond(out var _))
		{
			SetUserUnitDeckWarfareState();
			return false;
		}
		SetUserUnitDeckWarfareState();
		if (nKMWarfareTemplet.m_UserTeamCount > m_unitMgr.GetCurrentUserUnit())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WARFARE_WARNING_GAME_START, delegate
			{
				reqGameStart();
				if (bRepeatOperation)
				{
					NKCPopupRepeatOperation.Instance.CloseAndStartWithCurrOption();
				}
			});
			return false;
		}
		reqGameStart();
		return true;
	}

	public void OnClickNextTurn()
	{
		if (NKCGameEventManager.IsWaiting())
		{
			return;
		}
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		if (nKMWarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_PLAYING || !nKMWarfareGameData.isTurnA)
		{
			return;
		}
		if (m_unitMgr.GetRemainTurnOnUserUnitCount() > 0)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WARFARE_WARNING_FINISH_TURN, delegate
			{
				SetPause(bSet: true);
				NKCPacketSender.Send_NKMPacket_WARFARE_GAME_TURN_FINISH_REQ();
			});
		}
		else
		{
			SetPause(bSet: true);
			NKCPacketSender.Send_NKMPacket_WARFARE_GAME_TURN_FINISH_REQ();
		}
	}

	public void GiveUp()
	{
		if (GetNKMWarfareGameData() != null && !NKCScenManager.GetScenManager().GetNKCRepeatOperaion().CheckRepeatOperationRealStop())
		{
			NKCPopupOKCancel.OpenOKCancelBox(Content: (GetNKMWarfareGameData().rewardMultiply <= 1) ? NKCUtilString.GET_STRING_WARFARE_WARNING_GIVE_UP : NKCUtilString.GET_STRING_WARFARE_WARNING_GIVE_UP_MULTIPLY, Title: NKCUtilString.GET_STRING_NOTICE, onOkButton: OnClickOkGiveUpINGWarfare);
		}
	}

	private void OnClickOkGiveUpINGWarfare()
	{
		if (!NKCGameEventManager.IsWaiting())
		{
			NKCUIGameOption.CheckInstanceAndClose();
			NKCPacketSender.Send_NKMPacket_WARFARE_GAME_GIVE_UP_REQ();
		}
	}

	public bool CheckEnablePause()
	{
		if (NKCGameEventManager.IsWaiting())
		{
			return false;
		}
		if (GetNKMWarfareGameData().warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			return false;
		}
		return true;
	}

	public void SetPause(bool bSet)
	{
		if (CheckEnablePause())
		{
			if (bSet)
			{
				AddPauseRef();
			}
			else
			{
				MinusPauseRef();
			}
			if (m_unitMgr != null)
			{
				m_unitMgr.PauseUnits(GetPause());
			}
			NKCCamera.GetTrackingPos().SetPause(GetPause());
			m_NKCWarfareGameHUD.SetPauseState(GetPause());
		}
	}

	public void OnClickPause()
	{
		if (CheckEnablePause() && !NKCUIGameOption.Instance.IsOpen)
		{
			SetPause(bSet: true);
			NKCUIGameOption.Instance.Open(NKC_GAME_OPTION_MENU_TYPE.WARFARE, OnCloseGameOption);
		}
	}

	private void OnCloseGameOption()
	{
		SetPause(bSet: false);
	}

	private void OnClickUnit(int gameUID)
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(gameUID);
		if (!(nKCWarfareGameUnit == null))
		{
			nKCWarfareGameUnit.transform.SetAsLastSibling();
			if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
			{
				OnClickUnitWhenStop(nKCWarfareGameUnit);
			}
			else if (warfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
			{
				OnClickUnitWhenPlaying(warfareGameData, nKCWarfareGameUnit);
			}
		}
	}

	public void OnClickSquadInfo()
	{
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(GetSelectedSquadWFGUUID());
		if (nKCWarfareGameUnit != null)
		{
			OpenUserUnitInfoPopup(nKCWarfareGameUnit, bPlaying: true);
		}
	}

	private void OnClickUnitWhenPlaying(WarfareGameData cNKMWarfareGameData, NKCWarfareGameUnit cNKCWarfareGameUnit)
	{
		if (!cNKMWarfareGameData.isTurnA || IsAutoWarfare())
		{
			return;
		}
		WarfareUnitData nKMWarfareUnitData = cNKCWarfareGameUnit.GetNKMWarfareUnitData();
		if (nKMWarfareUnitData == null)
		{
			return;
		}
		int tileIndex = cNKCWarfareGameUnit.TileIndex;
		int beforeLastClickedUnitTileIndex = m_LastClickedUnitTileIndex;
		if (beforeLastClickedUnitTileIndex != -1 && beforeLastClickedUnitTileIndex != tileIndex)
		{
			NKCWarfareGameUnit gameUnitByTileIndex = m_unitMgr.GetGameUnitByTileIndex(beforeLastClickedUnitTileIndex);
			if (gameUnitByTileIndex != null && !gameUnitByTileIndex.GetNKMWarfareUnitData().isTurnEnd && m_tileMgr.IsTileLayer0Type(beforeLastClickedUnitTileIndex, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_MOVABLE_USER_UNIT_SELECTED) && m_tileMgr.IsTileLayer0Type(tileIndex, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_USER_UNIT_POSSIBLE_ARRIVAL))
			{
				WarfareUnitData nKMWarfareUnitData2 = gameUnitByTileIndex.GetNKMWarfareUnitData();
				if (nKMWarfareUnitData2.supply == 0 && nKMWarfareUnitData2.unitType != nKMWarfareUnitData.unitType)
				{
					NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_WARFARE_WARNING_SUPPLY, delegate
					{
						m_LastClickedUnitTileIndex = tileIndex;
						DoMove((byte)beforeLastClickedUnitTileIndex, (byte)tileIndex);
					});
				}
				else
				{
					m_LastClickedUnitTileIndex = tileIndex;
					DoMove((byte)beforeLastClickedUnitTileIndex, (byte)tileIndex);
				}
				return;
			}
		}
		if (!cNKMWarfareGameData.CheckTeamA_By_GameUnitUID(nKMWarfareUnitData.warfareGameUnitUID))
		{
			string battleConditionStrID = "";
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
			if (nKMWarfareTemplet != null)
			{
				battleConditionStrID = nKMWarfareTemplet.MapTemplet.GetTile(cNKCWarfareGameUnit.TileIndex).m_BattleConditionStrID;
			}
			NKCPopupEnemyList.Instance.Open(nKMWarfareUnitData.dungeonID, battleConditionStrID);
			return;
		}
		m_LastClickedUnitTileIndex = tileIndex;
		if (beforeLastClickedUnitTileIndex == m_LastClickedUnitTileIndex)
		{
			InvalidSelectedUnit();
			return;
		}
		if (!nKMWarfareUnitData.isTurnEnd)
		{
			SetTileDefaultWhenPlay();
			m_unitMgr.ResetIcon();
			CloseAssistFX();
			CancelRecovery();
			m_tileMgr.SetTileLayer0Type(tileIndex, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_MOVABLE_USER_UNIT_SELECTED);
			SetTilePossibleArrival(tileIndex, NKCWarfareManager.GetShipStyleTypeByGUUID(NKCScenManager.GetScenManager().GetMyUserData(), cNKMWarfareGameData, nKMWarfareUnitData.warfareGameUnitUID), nKMWarfareUnitData.supply == 0);
			if (nKMWarfareUnitData.supply == 0)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_WARFARE_WARNING_NO_EXIST_SUPPLY);
				if (nKMWarfareUnitData.unitType == WarfareUnitData.Type.User)
				{
					NKCOperatorUtil.PlayVoice(nKMWarfareUnitData.deckIndex, VOICE_TYPE.VT_BULLET_LACK);
				}
			}
		}
		else
		{
			SetTileDefaultWhenPlay();
			m_unitMgr.ResetIcon();
			CloseAssistFX();
			CancelRecovery();
		}
		bool flag = NKCWarfareManager.CheckOnTileType(cNKMWarfareGameData, tileIndex, NKM_WARFARE_MAP_TILE_TYPE.NWMTT_REPAIR);
		bool flag2 = NKCWarfareManager.CheckOnTileType(cNKMWarfareGameData, tileIndex, NKM_WARFARE_MAP_TILE_TYPE.NWMTT_RESUPPLY);
		bool flag3 = NKCWarfareManager.CheckOnTileType(cNKMWarfareGameData, tileIndex, NKM_WARFARE_MAP_TILE_TYPE.NWNTT_SERVICE);
		m_NKCWarfareGameHUD.OpenSelectedSquadUI(nKMWarfareUnitData.deckIndex, flag || flag3, flag2 || flag3);
	}

	private void UpdateSelectedSquadUI()
	{
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(GetSelectedSquadWFGUUID());
		if (nKCWarfareGameUnit == null || nKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.Dungeon)
		{
			m_NKCWarfareGameHUD.CloseSelectedSquadUI();
			return;
		}
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		int tileIndex = nKCWarfareGameUnit.TileIndex;
		bool flag = NKCWarfareManager.CheckOnTileType(nKMWarfareGameData, tileIndex, NKM_WARFARE_MAP_TILE_TYPE.NWMTT_REPAIR);
		bool flag2 = NKCWarfareManager.CheckOnTileType(nKMWarfareGameData, tileIndex, NKM_WARFARE_MAP_TILE_TYPE.NWMTT_RESUPPLY);
		bool flag3 = NKCWarfareManager.CheckOnTileType(nKMWarfareGameData, tileIndex, NKM_WARFARE_MAP_TILE_TYPE.NWNTT_SERVICE);
		m_NKCWarfareGameHUD.UpdateSelectedSquadUI(flag || flag3, flag2 || flag3);
	}

	public void OnClickPossibleArrivalTile(int tileIndex)
	{
		DoMove((byte)m_LastClickedUnitTileIndex, (byte)tileIndex);
	}

	private void OnClickUnitWhenStop(NKCWarfareGameUnit cNKCWarfareGameUnit)
	{
		if (cNKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User)
		{
			OpenUserUnitInfoPopup(cNKCWarfareGameUnit);
		}
		else
		{
			if (cNKCWarfareGameUnit.GetNKMWarfareUnitData().unitType != WarfareUnitData.Type.Dungeon)
			{
				return;
			}
			string battleConditionStrID = "";
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
			if (nKMWarfareTemplet != null)
			{
				NKMWarfareTileTemplet tile = nKMWarfareTemplet.MapTemplet.GetTile(cNKCWarfareGameUnit.TileIndex);
				if (tile != null)
				{
					battleConditionStrID = tile.m_BattleConditionStrID;
				}
			}
			NKCPopupEnemyList.Instance.Open(cNKCWarfareGameUnit.GetNKMWarfareUnitData().dungeonID, battleConditionStrID);
		}
	}

	private void OpenDeckView(NKCUIDeckViewer.DeckViewerMode eDeckViewerMode)
	{
		m_lastDeckView = eDeckViewerMode;
		if (m_bFirstOpenDeck && NKMWarfareTemplet.Find(m_WarfareStrID).m_bFriendSummon)
		{
			NKCPacketSender.Send_NKMPacket_WARFARE_FRIEND_LIST_REQ();
		}
		else
		{
			OpenDeckView();
		}
	}

	private void OpenDeckView()
	{
		m_bFirstOpenDeck = false;
		NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
		{
			MenuName = NKCUtilString.GET_STRING_MENU_NAME_WARFARE,
			eDeckviewerMode = m_lastDeckView,
			dOnSideMenuButtonConfirm = OnClickWarfareBatch,
			dCheckSideMenuButton = CheckWarfareBatch,
			dOnChangeDeckUnit = OnChangeDeckUnit
		};
		int num = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetAvailableDeckIndex(NKM_DECK_TYPE.NDT_NORMAL);
		if (num == -1)
		{
			num = 0;
		}
		options.DeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, num);
		options.upsideMenuShowResourceList = UpsideMenuShowResourceList;
		options.SelectLeaderUnitOnOpen = false;
		options.bEnableDefaultBackground = false;
		options.bUpsideMenuHomeButton = false;
		options.bOpenAlphaAni = true;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		options.bUsableSupporter = nKMWarfareTemplet.m_bFriendSummon;
		if (nKMWarfareTemplet.m_bFriendSummon)
		{
			options.lstSupporter = NKCWarfareManager.SupporterList;
			options.dOnSelectSupporter = BatchSupporter;
			options.dIsValidSupport = CheckSupport;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(m_WarfareStrID);
		if (nKMStageTempletV != null)
		{
			if (nKMStageTempletV.m_StageReqItemID == 0)
			{
				options.CostItemID = 2;
			}
			else
			{
				options.CostItemID = nKMStageTempletV.m_StageReqItemID;
			}
			options.StageBattleStrID = nKMStageTempletV.m_StageBattleStrID;
		}
		options.dOnHide = delegate
		{
			SetActiveForDeckView(bAcitve: false);
		};
		options.dOnUnhide = delegate
		{
			SetActiveForDeckView(bAcitve: true);
		};
		m_bOpenDeckView = true;
		NKCUIDeckViewer.Instance.Open(options);
	}

	private void OnChangeDeckUnit(NKMDeckIndex selectedDeckIndex, long newlyAddedUnitUID)
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(selectedDeckIndex) != null)
		{
			NKCWarfareGameUnit wFGameUnitByDeckIndex = m_unitMgr.GetWFGameUnitByDeckIndex(selectedDeckIndex);
			if (wFGameUnitByDeckIndex != null)
			{
				OnCancelBatch(wFGameUnitByDeckIndex.GetNKMWarfareUnitData().warfareGameUnitUID);
			}
		}
	}

	private void OnDeckViewBtn(int gameUnitUID)
	{
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(gameUnitUID);
		if (nKCWarfareGameUnit != null)
		{
			if (GetDivePointIndex(nKCWarfareGameUnit.TileIndex) != -1)
			{
				m_LastClickedSpawnPoint = GetDivePointIndex(nKCWarfareGameUnit.TileIndex);
				m_Last_WARFARE_SPAWN_POINT_TYPE = NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE;
			}
			else if (GetAssultPointIndex(nKCWarfareGameUnit.TileIndex) != -1)
			{
				m_LastClickedSpawnPoint = GetAssultPointIndex(nKCWarfareGameUnit.TileIndex);
				m_Last_WARFARE_SPAWN_POINT_TYPE = NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT;
			}
			OnCancelBatch(gameUnitUID);
			if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT)
			{
				OpenDeckView(NKCUIDeckViewer.DeckViewerMode.WarfareBatch_Assault);
			}
			else
			{
				OpenDeckView(NKCUIDeckViewer.DeckViewerMode.WarfareBatch);
			}
		}
	}

	private void BatchSupporter(long friendCode)
	{
		if (!CheckSupport(friendCode))
		{
			return;
		}
		_ = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		WarfareSupporterListData supportUnitData = NKCWarfareManager.FindSupporter(friendCode);
		int num = -1;
		if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE)
		{
			num = GetDivePointTileIndex(m_LastClickedSpawnPoint);
		}
		else if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT)
		{
			num = GetAssultPointTileIndex(m_LastClickedSpawnPoint);
		}
		if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE)
		{
			if (SetActiveDivePoint(num, bSet: false))
			{
				m_tileMgr.SetTileLayer0Type(num, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_NORMAL);
			}
		}
		else if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT && SetActiveAssultPoint(num, bSet: false))
		{
			m_tileMgr.SetTileLayer0Type(num, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_NORMAL);
		}
		if (m_NUM_WARFARE_FX_SHIP_DIVE != null)
		{
			m_NUM_WARFARE_FX_SHIP_DIVE.gameObject.transform.localPosition = m_listUnitPos[num];
			m_NUM_WARFARE_FX_SHIP_DIVE.SetActive(value: false);
			m_NUM_WARFARE_FX_SHIP_DIVE.SetActive(value: true);
		}
		NKCUIDeckViewer.Instance.Close();
		GetNKMWarfareGameData().supportUnitData = supportUnitData;
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.CreateNewUserUnit(NKMDeckIndex.None, (short)num, OnClickUnit, null, friendCode);
		if (!(nKCWarfareGameUnit == null))
		{
			nKCWarfareGameUnit.gameObject.transform.localPosition = m_listUnitPos[num];
			nKCWarfareGameUnit.transform.DOMove(BATCH_EFFECT_POS, 1.5f).SetEase(Ease.OutCubic).From(isRelative: true);
			m_NKCWarfareGameHUD.SetBatchedShipCount(m_unitMgr.GetCurrentUserUnit());
			m_NKCWarfareGameHUD.SetBatchedSupportShipCount(bSet: true);
			UpdateAttackCostUI();
			if (IsBatchMax())
			{
				TurnOffAllUserUnitSpawnPoint();
			}
		}
	}

	private bool CheckSupport(long friendCode)
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return false;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return false;
		}
		if (!nKMWarfareTemplet.m_bFriendSummon)
		{
			Debug.Log("친구 소대 사용 불가능한 맵");
			return false;
		}
		int spawnPointCountByType = mapTemplet.GetSpawnPointCountByType(NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE);
		int spawnPointCountByType2 = mapTemplet.GetSpawnPointCountByType(NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT);
		if (spawnPointCountByType + spawnPointCountByType2 <= 1)
		{
			Debug.Log($"출격 포인트 {spawnPointCountByType + spawnPointCountByType2} = 서포터 자리 없음");
			return false;
		}
		if (m_unitMgr.ContainSupporterUnit())
		{
			Debug.Log("이미 서포터가 배치되어 있음");
			return false;
		}
		if (NKCWarfareManager.FindSupporter(friendCode) == null)
		{
			Debug.Log($"친구 없음 {friendCode}");
			return false;
		}
		if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT)
		{
			Debug.Log("게스트/친구 소대는 강습지점에 착륙 불가");
			return false;
		}
		int index = -1;
		if (m_Last_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE)
		{
			index = GetDivePointTileIndex(m_LastClickedSpawnPoint);
		}
		NKMWarfareTileTemplet tile = mapTemplet.GetTile(index);
		if (tile == null)
		{
			return false;
		}
		if (tile.m_NKM_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_NONE)
		{
			Debug.Log(NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_POSITION));
			return false;
		}
		if (tile.m_NKM_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT)
		{
			Debug.Log("게스트/친구 소대는 강습지점에 착륙 불가");
			return false;
		}
		return true;
	}

	private void PlayWarfareOnGoingMusic()
	{
		string text = FindMusic();
		if (text != null)
		{
			NKCSoundManager.PlayMusic(text, bLoop: true);
		}
	}

	private string FindMusic()
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet.m_WarfareBGM.Length > 1)
		{
			return nKMWarfareTemplet.m_WarfareBGM;
		}
		string text = "";
		NKCCutScenTemplet nKCCutScenTemplet = null;
		text = nKMWarfareTemplet.m_CutScenStrIDBefore;
		if (!string.IsNullOrEmpty(text))
		{
			nKCCutScenTemplet = NKCCutScenManager.GetCutScenTemple(text);
			if (nKCCutScenTemplet != null)
			{
				string lastMusicAssetName = nKCCutScenTemplet.GetLastMusicAssetName();
				if (lastMusicAssetName != null && lastMusicAssetName.Length > 1)
				{
					return lastMusicAssetName;
				}
			}
		}
		return null;
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_START_ACK cNKMPacket_WARFARE_GAME_START_ACK)
	{
		PrepareWarfareGameStart();
		CheckTutorialAfterStart();
		NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
		if (nKCRepeatOperaion != null && nKCRepeatOperaion.GetIsOnGoing())
		{
			if (!IsAutoWarfare())
			{
				m_NKCWarfareGameHUD.SendAutoReq(bAuto: true);
			}
			m_NKCWarfareGameHUD.SetActiveRepeatOperationOnOff(bOn: true);
			if (cNKMPacket_WARFARE_GAME_START_ACK.costItemDataList != null && cNKMPacket_WARFARE_GAME_START_ACK.costItemDataList.Count > 0)
			{
				nKCRepeatOperaion.SetCostIncreaseCount(nKCRepeatOperaion.GetCostIncreaseCount() + 1);
			}
		}
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_TURN_FINISH_ACK cNKMPacket_WARFARE_GAME_TURN_FINISH_ACK)
	{
		InvalidSelectedUnitPure();
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (cNKMPacket_WARFARE_GAME_TURN_FINISH_ACK.warfareSyncData == null)
		{
			warfareGameData.isTurnA = false;
			warfareGameData.SetUnitTurnEnd(bTurnEnd: false);
			OnCommonUserTurnFinished();
			SetTileDefaultWhenPlay();
			UpdateLabel();
			m_unitMgr.UpdateGameUnitUI();
		}
		else
		{
			warfareGameData.SetUnitTurnEnd(bTurnEnd: false);
			ProcessWarfareGameSyncData(cNKMPacket_WARFARE_GAME_TURN_FINISH_ACK.warfareSyncData);
		}
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_USE_SERVICE_ACK cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK)
	{
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK.warfareGameUnitUID);
		if (nKCWarfareGameUnit != null)
		{
			UseServiceFX(nKCWarfareGameUnit, cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK.warfareServiceType);
		}
		NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_unitMgr.GetNKCWarfareGameUnitInfo(cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK.warfareGameUnitUID);
		if (nKCWarfareGameUnitInfo != null)
		{
			nKCWarfareGameUnitInfo.SetUnitInfoUI();
			nKCWarfareGameUnitInfo.OnPlayServiceSound(cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK.warfareServiceType);
		}
	}

	public void InitWaitNextOrder()
	{
		m_bWaitingNextOreder = false;
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_NEXT_ORDER_ACK cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK)
	{
		Debug.Log("warfare next order ack process start");
		InvalidSelectedUnit();
		ProcessWarfareGameSyncData(cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.warfareSyncData);
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (nKMWarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			myUserData.m_ArmyData.ResetDeckStateOf(NKM_DECK_STATE.DECK_STATE_WARFARE);
			_ = nKMWarfareGameData.warfareTeamDataA.flagShipWarfareUnitUID;
			bool flag = !NKCScenManager.GetScenManager().GetMyUserData().CheckWarfareClear(GetNKMWarfareGameData().warfareTempletID);
			bool bNoAllClearBefore = flag;
			if (!flag)
			{
				NKMWarfareClearData warfareClearData = myUserData.GetWarfareClearData(GetNKMWarfareGameData().warfareTempletID);
				if (warfareClearData != null && (!warfareClearData.m_mission_result_1 || !warfareClearData.m_mission_result_2 || !warfareClearData.m_MissionRewardResult || (cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.warfareClearData != null && cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.warfareClearData.m_MissionReward != null)))
				{
					bNoAllClearBefore = true;
				}
			}
			m_NKMWarfareClearData = cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.warfareClearData;
			myUserData.SetWarfareClearDataOnlyTrue(m_NKMWarfareClearData);
			m_TeamAFlagDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, nKMWarfareGameData.flagshipDeckIndex);
			NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
			if (m_NKMWarfareClearData != null)
			{
				string warfareStrID = NKCWarfareManager.GetWarfareStrID(nKMWarfareGameData.warfareTempletID);
				if (!string.IsNullOrEmpty(warfareStrID))
				{
					string key = $"{myUserData.m_UserUID}_{warfareStrID}";
					if (PlayerPrefs.HasKey(key))
					{
						PlayerPrefs.DeleteKey(key);
					}
					myUserData.UpdateStagePlayData(m_NKMWarfareClearData.m_StagePlayData);
				}
				myUserData.GetReward(m_NKMWarfareClearData.m_RewardDataList);
				if (m_NKMWarfareClearData.m_ContainerRewards != null)
				{
					myUserData.GetReward(m_NKMWarfareClearData.m_ContainerRewards);
				}
				if (m_NKMWarfareClearData.m_OnetimeRewards != null)
				{
					myUserData.GetReward(m_NKMWarfareClearData.m_OnetimeRewards);
				}
				if (m_NKMWarfareClearData.m_MissionReward != null)
				{
					myUserData.GetReward(m_NKMWarfareClearData.m_MissionReward);
				}
				NKCContentManager.SetUnlockedContent(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE, m_NKMWarfareClearData.m_WarfareID);
				if (nKCRepeatOperaion.GetIsOnGoing())
				{
					nKCRepeatOperaion.AddReward(m_NKMWarfareClearData.m_RewardDataList);
					nKCRepeatOperaion.AddReward(m_NKMWarfareClearData.m_ContainerRewards);
					nKCRepeatOperaion.AddReward(m_NKMWarfareClearData.m_OnetimeRewards);
					nKCRepeatOperaion.AddReward(m_NKMWarfareClearData.m_MissionReward);
					nKCRepeatOperaion.SetCurrRepeatCount(nKCRepeatOperaion.GetCurrRepeatCount() + 1);
					if (nKCRepeatOperaion.GetCurrRepeatCount() >= nKCRepeatOperaion.GetMaxRepeatCount())
					{
						nKCRepeatOperaion.Init();
						nKCRepeatOperaion.SetStopReason(NKCUtilString.GET_STRING_REPEAT_OPERATION_IS_TERMINATED);
						nKCRepeatOperaion.SetAlarmRepeatOperationSuccess(bSet: true);
					}
				}
				NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.WarfareGameClear, nKMWarfareGameData.warfareTempletID, warfareStrID);
			}
			else if (nKCRepeatOperaion.GetIsOnGoing())
			{
				nKCRepeatOperaion.SetCurrRepeatCount(nKCRepeatOperaion.GetCurrRepeatCount() + 1);
				nKCRepeatOperaion.Init();
				nKCRepeatOperaion.SetAlarmRepeatOperationQuitByDefeat(bSet: true);
			}
			myUserData.UpdateEpisodeCompleteData(cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK.episodeCompleteData);
			WarfareSupporterListData guestSptData = null;
			if (nKMWarfareGameData.supportUnitData != null && NKCWarfareManager.IsGeustSupporter(nKMWarfareGameData.supportUnitData.commonProfile.friendCode))
			{
				guestSptData = nKMWarfareGameData.supportUnitData;
			}
			m_NUM_WARFARE.SetActive(value: false);
			NKCPopupRepeatOperation.CheckInstanceAndClose();
			NKCUIWarfareResult.Instance.Open(m_NKMWarfareClearData, m_TeamAFlagDeckIndex, OnCallBackForResult, flag, bNoAllClearBefore, guestSptData);
		}
		m_NKCWarfareGameHUD.UpdateMedalInfo();
		m_NKCWarfareGameHUD.UpdateWinCondition();
		m_NKCWarfareGameHUD.SetRemainTurnOnUnitCount(m_unitMgr.GetRemainTurnOnUserUnitCount());
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_MOVE_ACK cNKMPacket_WARFARE_GAME_MOVE_ACK)
	{
		ProcessWarfareGameSyncData(cNKMPacket_WARFARE_GAME_MOVE_ACK.warfareSyncData);
		UpdateSelectedSquadUI();
	}

	public void OnRecv(NKMPacket_WARFARE_FRIEND_LIST_ACK sPacket)
	{
		OpenDeckView();
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		base.OnInventoryChange(itemData);
	}

	private int GetDivePointTileIndex(int divePointIndex)
	{
		if (m_NKMWarfareMapTemplet == null)
		{
			return -1;
		}
		return m_NKMWarfareMapTemplet.GetDivePointTileIndex(divePointIndex);
	}

	private int GetDivePointIndex(int tileIndex)
	{
		if (m_NKMWarfareMapTemplet == null)
		{
			return -1;
		}
		return m_NKMWarfareMapTemplet.GetDivePointIndex(tileIndex);
	}

	private int GetAssultPointIndex(int tileIndex)
	{
		if (m_NKMWarfareMapTemplet == null)
		{
			return -1;
		}
		return m_NKMWarfareMapTemplet.GetAssultPointIndex(tileIndex);
	}

	private int GetAssultPointTileIndex(int assultPointIndex)
	{
		if (m_NKMWarfareMapTemplet == null)
		{
			return -1;
		}
		return m_NKMWarfareMapTemplet.GetAssultPointTileIndex(assultPointIndex);
	}

	private bool SetActiveDivePoint(int tileIndex, bool bSet)
	{
		if (m_listDivePoint.Count == 0)
		{
			return false;
		}
		int divePointIndex = GetDivePointIndex(tileIndex);
		if (divePointIndex != -1)
		{
			GameObject gameObject = m_listDivePoint[divePointIndex];
			if (bSet && gameObject != null)
			{
				gameObject.transform.localPosition = m_listUnitPos[tileIndex];
				NKCUIComButton cNKCUIComButton = gameObject.GetComponent<NKCUIComButton>();
				if (cNKCUIComButton != null)
				{
					cNKCUIComButton.m_DataInt = divePointIndex;
					cNKCUIComButton.PointerClick.RemoveAllListeners();
					cNKCUIComButton.PointerClick.AddListener(delegate
					{
						OnClickDivePoint(cNKCUIComButton.m_DataInt);
					});
				}
			}
			NKCUtil.SetGameobjectActive(gameObject, bSet);
			return true;
		}
		return false;
	}

	private GameObject GetDivePointGO(int tileIndex)
	{
		if (GetDivePointIndex(tileIndex) != -1)
		{
			return m_listDivePoint[GetDivePointIndex(tileIndex)];
		}
		return null;
	}

	private bool SetActiveAssultPoint(int tileIndex, bool bSet)
	{
		if (m_listAssultPoint.Count == 0)
		{
			return false;
		}
		int assultPointIndex = GetAssultPointIndex(tileIndex);
		if (assultPointIndex != -1)
		{
			GameObject gameObject = m_listAssultPoint[assultPointIndex];
			if (bSet && gameObject != null)
			{
				gameObject.transform.localPosition = m_listUnitPos[tileIndex];
				NKCUIComButton cNKCUIComButton = gameObject.GetComponent<NKCUIComButton>();
				if (cNKCUIComButton != null)
				{
					cNKCUIComButton.m_DataInt = assultPointIndex;
					cNKCUIComButton.PointerClick.RemoveAllListeners();
					cNKCUIComButton.PointerClick.AddListener(delegate
					{
						OnClickAssultPoint(cNKCUIComButton.m_DataInt);
					});
				}
			}
			NKCUtil.SetGameobjectActive(gameObject, bSet);
			return true;
		}
		return false;
	}

	private GameObject GetAssultPointGO(int tileIndex)
	{
		if (GetAssultPointIndex(tileIndex) != -1)
		{
			return m_listAssultPoint[GetAssultPointIndex(tileIndex)];
		}
		return null;
	}

	private void TurnOnAllAvaiableUserUnitSpawnPoint()
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null || GetNKMWarfareGameData() == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < mapTemplet.TileCount; i++)
		{
			NKMWarfareTileTemplet tile = mapTemplet.GetTile(i);
			if (tile == null)
			{
				continue;
			}
			if (tile.m_TileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE)
			{
				SetActiveDivePoint(i, bSet: false);
				SetActiveAssultPoint(i, bSet: false);
			}
			else
			{
				if (tile.m_DungeonStrID != null || !(m_unitMgr.GetGameUnitByTileIndex(i) == null))
				{
					continue;
				}
				bool flag = false;
				bool flag2 = false;
				if (SetActiveDivePoint(i, bSet: true))
				{
					flag = true;
				}
				if (SetActiveAssultPoint(i, bSet: true))
				{
					flag2 = true;
				}
				if (flag || flag2)
				{
					if (flag)
					{
						m_tileMgr.SetTileLayer0Type(i, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_DIVE_POINT);
					}
					else if (flag2)
					{
						m_tileMgr.SetTileLayer0Type(i, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_ASSULT_POINT);
					}
					GameObject gameObject = GetDivePointGO(i);
					if (gameObject == null)
					{
						gameObject = GetAssultPointGO(i);
					}
					AnimateSpawnPoint(gameObject, (float)num * 0.1f);
					num++;
				}
			}
		}
	}

	private void AnimateSpawnPoint(GameObject spawnPoint, float fDelay)
	{
		if (!(spawnPoint == null))
		{
			spawnPoint.transform.localScale = new Vector3(1f, 0f, 1f);
			spawnPoint.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBack, 2.5f).SetDelay(fDelay);
		}
	}

	private void TurnOffAllUserUnitSpawnPoint()
	{
		int num = 0;
		for (num = 0; num < m_listDivePoint.Count; num++)
		{
			GameObject gameObject = m_listDivePoint[num];
			if (gameObject != null)
			{
				NKCUtil.SetGameobjectActive(gameObject, bValue: false);
			}
		}
		for (num = 0; num < m_listAssultPoint.Count; num++)
		{
			NKCUtil.SetGameobjectActive(m_listAssultPoint[num], bValue: false);
		}
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null || GetNKMWarfareGameData() == null)
		{
			return;
		}
		for (num = 0; num < mapTemplet.TileCount; num++)
		{
			NKMWarfareTileTemplet tile = mapTemplet.GetTile(num);
			if (tile != null && tile.m_TileType != NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE && tile.m_DungeonStrID == null)
			{
				bool flag = false;
				bool flag2 = false;
				if (GetDivePointIndex(num) != -1)
				{
					flag = true;
				}
				if (GetAssultPointIndex(num) != -1)
				{
					flag2 = true;
				}
				if (flag || flag2)
				{
					m_tileMgr.SetTileLayer0Type(num, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_READY_NORMAL);
				}
			}
		}
	}

	private void PreProcessSpawnPoint()
	{
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return;
		}
		if (m_listDivePoint.Count < mapTemplet.GetSpawnPointCountByType(NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE))
		{
			int num = mapTemplet.GetSpawnPointCountByType(NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE) - m_listDivePoint.Count;
			for (int i = 0; i < num; i++)
			{
				NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WARFARE", "NUM_WARFARE_DIVE_POINT");
				GameObject instant = nKCAssetInstanceData.m_Instant;
				m_listAssetInstance.Add(nKCAssetInstanceData);
				instant.transform.SetParent(m_NUM_WARFARE_DIVE_POINT.transform);
				m_listDivePoint.Add(instant);
				NKCUtil.SetGameobjectActive(instant, bValue: false);
			}
		}
		if (m_listAssultPoint.Count < mapTemplet.GetSpawnPointCountByType(NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT))
		{
			int num2 = mapTemplet.GetSpawnPointCountByType(NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT) - m_listAssultPoint.Count;
			for (int j = 0; j < num2; j++)
			{
				NKCAssetInstanceData nKCAssetInstanceData2 = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WARFARE", "NUM_WARFARE_ASSULT_POINT");
				GameObject instant2 = nKCAssetInstanceData2.m_Instant;
				m_listAssetInstance.Add(nKCAssetInstanceData2);
				instant2.transform.SetParent(m_NUM_WARFARE_DIVE_POINT.transform);
				m_listAssultPoint.Add(instant2);
				NKCUtil.SetGameobjectActive(instant2, bValue: false);
			}
		}
	}

	private void UpdateRecoveryCount()
	{
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		if (nKMWarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
		{
			m_NKCWarfareGameHUD.SetActiveRecovery(active: false);
		}
		else if (nKMWarfareGameData.isTurnA && !IsAutoWarfare())
		{
			int recoverableUnitCount = NKCWarfareManager.GetRecoverableUnitCount(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData);
			m_NKCWarfareGameHUD.SetActiveRecovery(recoverableUnitCount > 0);
			m_NKCWarfareGameHUD.SetRecoveryCount(recoverableUnitCount);
		}
		else
		{
			m_NKCWarfareGameHUD.SetActiveRecovery(active: false);
		}
	}

	public void OnTouchRecoveryBtn()
	{
		if (m_bSelectRecovery)
		{
			InvalidSelectedUnit();
			return;
		}
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		if (nKMWarfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_PLAYING)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		List<WarfareUnitData> list = nKMWarfareGameData.warfareTeamDataA.warfareUnitDataByUIDMap.Values.ToList();
		if (NKCWarfareManager.GetRecoverableUnitCount(myUserData.m_ArmyData) <= 0)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_WARFARE_RECOVERY_NO_UNIT);
			return;
		}
		InvalidSelectedUnit();
		bool flag = false;
		List<int> listTile = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			WarfareUnitData warfareUnitData = list[i];
			if (!(warfareUnitData.hp <= 0f) && !(m_unitMgr.GetNKCWarfareGameUnit(warfareUnitData.warfareGameUnitUID) == null))
			{
				NKM_UNIT_STYLE_TYPE shipStyleTypeByGUUID = NKCWarfareManager.GetShipStyleTypeByGUUID(myUserData, nKMWarfareGameData, warfareUnitData.warfareGameUnitUID);
				if (SetTilePossibleRecovery(shipStyleTypeByGUUID, warfareUnitData.tileIndex, ref listTile))
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_WARFARE_RECOVERY_NO_TILE);
			return;
		}
		m_bSelectRecovery = true;
		m_NKCWarfareGameHUD.SetRecoveryFx(active: true);
		SetRecoveryPoint(listTile);
	}

	private void CancelRecovery()
	{
		m_NKCWarfareGameHUD.SetRecoveryFx(active: false);
		m_bSelectRecovery = false;
		SetRecoveryPoint(null);
	}

	private bool SetTilePossibleRecovery(NKM_UNIT_STYLE_TYPE moveType, int pivotTileIndex, ref List<int> listTile)
	{
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
		if (nKMWarfareTemplet == null)
		{
			return false;
		}
		NKMWarfareMapTemplet mapTemplet = nKMWarfareTemplet.MapTemplet;
		if (mapTemplet == null)
		{
			return false;
		}
		int posXByIndex = mapTemplet.GetPosXByIndex(pivotTileIndex);
		int posYByIndex = mapTemplet.GetPosYByIndex(pivotTileIndex);
		MovableTileSet tileSet = MovableTileSet.GetTileSet(moveType);
		bool result = false;
		int num = 0;
		int num2 = 2;
		for (int i = posXByIndex - num2; i <= posXByIndex + num2; i++)
		{
			int num3 = 0;
			for (int j = posYByIndex - num2; j <= posYByIndex + num2; j++)
			{
				if (!tileSet[num3++, num])
				{
					continue;
				}
				int indexByPos = mapTemplet.GetIndexByPos(i, j);
				if (indexByPos < 0)
				{
					continue;
				}
				WarfareTileData tileData = nKMWarfareGameData.GetTileData(indexByPos);
				if (tileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE || tileData.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_INCR)
				{
					continue;
				}
				NKMWarfareTileTemplet tile = mapTemplet.GetTile(indexByPos);
				if (tile != null && tile.m_TileWinType != WARFARE_GAME_CONDITION.WFC_TILE_ENTER && !(m_unitMgr.GetGameUnitByTileIndex(indexByPos) != null))
				{
					m_tileMgr.SetTileLayer0Type(indexByPos, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE.WTL0T_PLAY_USER_RECOVERY_POINT);
					result = true;
					if (!listTile.Contains(indexByPos))
					{
						listTile.Add(indexByPos);
					}
				}
			}
			num++;
		}
		return result;
	}

	private void SetRecoveryPoint(List<int> listTile)
	{
		int num = 0;
		if (listTile != null)
		{
			num = listTile.Count;
		}
		if (m_listRecoveryPoint.Count < num)
		{
			int num2 = num - m_listRecoveryPoint.Count;
			for (int i = 0; i < num2; i++)
			{
				NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WARFARE", "NUM_WARFARE_RECOVERY_POINT");
				GameObject instant = nKCAssetInstanceData.m_Instant;
				m_listAssetInstance.Add(nKCAssetInstanceData);
				instant.transform.SetParent(m_NUM_WARFARE_DIVE_POINT.transform);
				instant.transform.localScale = Vector3.one;
				m_listRecoveryPoint.Add(instant);
				NKCUtil.SetGameobjectActive(instant, bValue: false);
			}
		}
		for (int j = 0; j < m_listRecoveryPoint.Count; j++)
		{
			GameObject gameObject = m_listRecoveryPoint[j];
			NKCUtil.SetGameobjectActive(gameObject, j < num);
			if (j >= num)
			{
				continue;
			}
			int tileIndex = listTile[j];
			gameObject.transform.localPosition = m_listUnitPos[tileIndex];
			NKCUIComButton component = gameObject.GetComponent<NKCUIComButton>();
			if (component != null)
			{
				component.PointerClick.RemoveAllListeners();
				component.PointerClick.AddListener(delegate
				{
					OnTouchRecoveryTile(tileIndex);
				});
			}
		}
	}

	private void OnTouchRecoveryTile(int tileIndex)
	{
		List<int> recoverableDeckIndexList = NKCWarfareManager.GetRecoverableDeckIndexList(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData);
		if (recoverableDeckIndexList.Count == 0)
		{
			Debug.LogError("Warfare - ClickRecoveryTile - 복구할 유닛이 없다고 ..??");
			return;
		}
		m_LastClickedSpawnPoint = tileIndex;
		NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
		{
			MenuName = NKCUtilString.GET_STRING_MENU_NAME_WARFARE,
			eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.WarfareRecovery,
			dOnSideMenuButtonConfirm = OnRecoveryDeck,
			dCheckSideMenuButton = CheckRecoveryDeck,
			DeckListButtonStateText = NKCUtilString.GET_STRING_WARFARE_RECOVERABLE
		};
		recoverableDeckIndexList.Sort();
		options.DeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, recoverableDeckIndexList[0]);
		options.ShowDeckIndexList = recoverableDeckIndexList;
		options.upsideMenuShowResourceList = UpsideMenuShowResourceList;
		options.SelectLeaderUnitOnOpen = false;
		options.bEnableDefaultBackground = false;
		options.bUpsideMenuHomeButton = false;
		options.bOpenAlphaAni = true;
		options.bUsableSupporter = false;
		options.CostItemID = 101;
		_ = GetNKMWarfareGameData().rewardMultiply;
		_ = 0;
		options.CostItemCount = NKMCommonConst.WarfareRecoverItemCost;
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(m_WarfareStrID);
		if (nKMStageTempletV != null)
		{
			options.StageBattleStrID = nKMStageTempletV.m_StageBattleStrID;
		}
		options.dOnHide = delegate
		{
			SetActiveForDeckView(bAcitve: false);
		};
		options.dOnUnhide = delegate
		{
			SetActiveForDeckView(bAcitve: true);
		};
		m_bOpenDeckView = true;
		NKCUIDeckViewer.Instance.Open(options);
	}

	private NKM_ERROR_CODE CheckRecoveryDeck(NKMDeckIndex deckIndex)
	{
		NKMDeckData deckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(deckIndex);
		if (deckData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DECK_DATA_INVALID;
		}
		if (deckData.GetState() != NKM_DECK_STATE.DECK_STATE_WARFARE)
		{
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_FIND_UNIT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	private void OnRecoveryDeck(NKMDeckIndex deckIndex, long supportUserUID = 0L)
	{
		NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_WARFARE_RECOVERY, NKCUtilString.GET_STRING_WARFARE_RECOVERY_CONFIRM, 101, NKMCommonConst.WarfareRecoverItemCost, delegate
		{
			RecoveryDeck(deckIndex);
		});
	}

	private void RecoveryDeck(NKMDeckIndex deckIndex)
	{
		if (CheckRecoveryDeck(deckIndex) != NKM_ERROR_CODE.NEC_OK)
		{
			Debug.LogError($"긴급복구 불가능한 덱 ({deckIndex.m_eDeckType}, {deckIndex.m_iIndex})");
			return;
		}
		Debug.Log($"긴급복구 - {deckIndex.m_iIndex}, tile {m_LastClickedSpawnPoint}");
		SetPause(bSet: true);
		NKCPacketSender.Send_NKMPacket_WARFARE_RECOVER_REQ(deckIndex.m_iIndex, (short)m_LastClickedSpawnPoint);
	}

	public void OnRecv(NKMPacket_WARFARE_RECOVER_ACK res)
	{
		InvalidSelectedUnit();
		NKCUIDeckViewer.Instance.Close();
		ProcessWarfareGameSyncData(res.warfareSyncData);
	}

	private void CamTrackMovingUnit(Vector3 _orgPos, Vector3 _targetPos)
	{
		if (m_bRunningCamTrackMovingUnit)
		{
			StopCamTrackMovingUnit();
		}
		m_CamTrackMovingUnitCoroutine = StartCoroutine(_CamTrackMovingUnit(_orgPos, _targetPos));
	}

	private IEnumerator _CamTrackMovingUnit(Vector3 _orgPos, Vector3 _targetPos)
	{
		m_bRunningCamTrackMovingUnit = true;
		Vector3 vector = _targetPos;
		float fDeltaTime = 0f;
		NKCCamera.GetTrackingPos().SetTracking(new NKMVector3(vector.x, vector.y + -250f, GetFinalCameraZDist() + 50), 0.9f, TRACKING_DATA_TYPE.TDT_SLOWER);
		fDeltaTime += Time.deltaTime;
		yield return null;
		while (fDeltaTime < 0.9f)
		{
			yield return null;
			if (!GetPause())
			{
				fDeltaTime += Time.deltaTime;
			}
		}
		SetCamZoomOut(1.3499999f);
		m_bRunningCamTrackMovingUnit = false;
	}

	public void StopCamTrackMovingUnit()
	{
		if (m_CamTrackMovingUnitCoroutine != null)
		{
			StopCoroutine(m_CamTrackMovingUnitCoroutine);
		}
		m_CamTrackMovingUnitCoroutine = null;
		m_bRunningCamTrackMovingUnit = false;
	}

	private void TryGameLoadReq()
	{
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		if (nKMWarfareGameData != null)
		{
			PlayBattleVoice(nKMWarfareGameData.warfareTeamDataA);
			PlayBattleVoice(nKMWarfareGameData.warfareTeamDataB);
			NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(nKMWarfareGameData.battleMonsterUid);
			if (nKCWarfareGameUnit != null)
			{
				float num = 0f;
				num = -100f;
				StopCamTrackMovingUnit();
				NKCCamera.GetTrackingPos().SetTracking(new NKMVector3(nKCWarfareGameUnit.transform.localPosition.x + num, nKCWarfareGameUnit.transform.localPosition.y - 100f, GetFinalCameraZDist() + 500), 0.6f, TRACKING_DATA_TYPE.TDT_FASTER);
				m_fElapsedTimeToBattle = 0f;
				m_bReservedBattle = true;
				NKCCamera.SetFocusBlur(0.6f);
			}
		}
	}

	private void PlayBattleVoice(WarfareTeamData teamData)
	{
		if (teamData != null && teamData.warfareUnitDataByUIDMap != null && teamData.warfareUnitDataByUIDMap.ContainsKey(teamData.flagShipWarfareUnitUID))
		{
			WarfareUnitData warfareUnitData = teamData.warfareUnitDataByUIDMap[teamData.flagShipWarfareUnitUID];
			if (warfareUnitData.unitType == WarfareUnitData.Type.User)
			{
				NKCOperatorUtil.PlayVoice(warfareUnitData.deckIndex, VOICE_TYPE.VT_SHIP_MEET);
			}
		}
	}

	public void OnDragByInstance(BaseEventData cBaseEventData)
	{
		if (!m_bPlayingIntro)
		{
			PointerEventData pointerEventData = cBaseEventData as PointerEventData;
			float value = NKCCamera.GetPosNowX() - pointerEventData.delta.x * 10f;
			float value2 = NKCCamera.GetPosNowY() - pointerEventData.delta.y * 10f;
			value = Mathf.Clamp(value, m_rtCamBound.xMin, m_rtCamBound.xMax);
			value2 = Mathf.Clamp(value2, m_rtCamBound.yMin, m_rtCamBound.yMax);
			NKCCamera.TrackingPos(1f, value, value2);
		}
	}

	private void SetCamDefaultWhenPlaying(float fTime = 1.6f, TRACKING_DATA_TYPE trackingData = TRACKING_DATA_TYPE.TDT_SLOWER)
	{
		NKCCamera.GetTrackingPos().SetTracking(new NKMVector3(0f, -308f, GetFinalCameraZDist()), fTime, trackingData);
	}

	private void SetCamZoomOut(float fTime = 1.6f, TRACKING_DATA_TYPE trackingData = TRACKING_DATA_TYPE.TDT_SLOWER)
	{
		NKCCamera.GetTrackingPos().SetTracking(new NKMVector3(NKCCamera.GetTrackingPos().GetNowValueX(), NKCCamera.GetTrackingPos().GetNowValueY(), GetFinalCameraZDist()), fTime, trackingData);
	}

	private int GetFinalCameraZDist()
	{
		float num = (float)Screen.height / NKCUIManager.GetUIFrontCanvasScaler().referenceResolution.y;
		if (num <= 1f)
		{
			num = 1f;
		}
		num -= (num - 1f) * 0.5f;
		return (int)(-798f * num);
	}

	private void SetCameraIntro()
	{
		m_bPlayingIntro = true;
		NKCCamera.SetPos(m_CameraIntroPos.x, m_CameraIntroPos.y, m_CameraIntroPos.z, bTrackingStop: true, bForce: true);
		NKCCamera.GetTrackingPos().SetTracking(new NKMVector3(0f, -344f, GetFinalCameraZDist()), 1.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
		NKCCamera.GetTrackingRotation().SetNowValue(m_CameraIntroRot.x, m_CameraIntroRot.y, m_CameraIntroRot.z);
		NKCCamera.GetTrackingRotation().SetTracking(-20f, 0f, 0f, 1.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	private void AddAssistFX(Vector3 start, Vector3 target)
	{
		NKCWarfareGameAssist newInstance = NKCWarfareGameAssist.GetNewInstance(m_NUM_WARFARE_LABEL.transform, start, target);
		if (!(newInstance == null))
		{
			m_listAssist.Add(newInstance);
		}
	}

	private void CloseAssistFX()
	{
		for (int i = 0; i < m_listAssist.Count; i++)
		{
			m_listAssist[i].Close();
		}
		m_listAssist.Clear();
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Warfare);
	}

	private void CheckTutorialAfterBattle()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.WarfareBattle);
	}

	private void CheckTutorialAfterStart()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.WarfareStart);
	}

	private void CheckTutorialAfterSync()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.WarfareSync);
	}

	public GameObject GetTileObject(int tileIndex)
	{
		NKCWarfareGameTile tile = m_tileMgr.GetTile(tileIndex);
		if (tile == null)
		{
			return null;
		}
		return tile.GetActiveGameObject();
	}

	public GameObject GetUnitObject(int gameUnitUID)
	{
		NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(gameUnitUID);
		if (nKCWarfareGameUnit == null)
		{
			return null;
		}
		return nKCWarfareGameUnit.gameObject;
	}

	public void ProcessTutorialTileTouchEvent(NKCGameEventManager.NKCGameEventTemplet eventTemplet)
	{
		if (eventTemplet == null)
		{
			Log.Warn("[ProcessTutorialTileTouchEvent] eventTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCWarfareGame.cs", 5526);
		}
		else
		{
			if (eventTemplet.EventType != NKCGameEventManager.GameEventType.TUTORIAL_CLICK_WARFARE_TILE)
			{
				return;
			}
			switch (eventTemplet.StringValue)
			{
			case "CLICK_DIVE_POINT":
			{
				int divePointIndex = GetDivePointIndex(eventTemplet.Value);
				OnClickDivePoint(divePointIndex);
				break;
			}
			case "CLICK_SHIP":
			{
				WarfareUnitData unitDataByTileIndex = GetNKMWarfareGameData().GetUnitDataByTileIndex(eventTemplet.Value);
				if (unitDataByTileIndex != null)
				{
					OnClickUnit(unitDataByTileIndex.warfareGameUnitUID);
				}
				else
				{
					Log.Warn($"[ProcessTutorialTileTouchEvent] {eventTemplet.Value} 타일 위 유닛을 찾을 수 없음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCWarfareGame.cs", 5548);
				}
				break;
			}
			case "CLICK_MOVE_SHIP":
				OnClickPossibleArrivalTile(eventTemplet.Value);
				break;
			}
		}
	}

	public void SetAutoForTutorial(bool bAuto)
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bAutoWarfare != bAuto)
		{
			Debug.Log("NKMPacket_WARFARE_GAME_AUTO_REQ - NKCWarfareGame.SetAutoForTutorial");
			SetPause(bSet: true);
			WaitAutoPacket = true;
			NKCPacketSender.Send_NKMPacket_WARFARE_GAME_AUTO_REQ(bAuto);
		}
	}

	public int GetTutorialSelectDeck()
	{
		if (NKCGameEventManager.IsEventPlaying())
		{
			WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
			if (nKMWarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_PLAYING && nKMWarfareGameData.isTurnA)
			{
				NKCWarfareGameUnit nKCWarfareGameUnit = m_unitMgr.GetNKCWarfareGameUnit(GetSelectedSquadWFGUUID());
				if (nKCWarfareGameUnit != null && nKCWarfareGameUnit.GetNKMWarfareUnitData() != null)
				{
					return nKCWarfareGameUnit.GetNKMWarfareUnitData().deckIndex.m_iIndex;
				}
			}
		}
		return -1;
	}

	public void RefreshTutorialSelectDeck(int deckIndex)
	{
		if (deckIndex < 0)
		{
			return;
		}
		WarfareGameData nKMWarfareGameData = GetNKMWarfareGameData();
		if (nKMWarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_PLAYING && nKMWarfareGameData.isTurnA)
		{
			WarfareUnitData unitDataByNormalDeckIndex = nKMWarfareGameData.GetUnitDataByNormalDeckIndex((byte)deckIndex);
			if (unitDataByNormalDeckIndex != null)
			{
				OnClickUnit(unitDataByNormalDeckIndex.warfareGameUnitUID);
			}
		}
	}
}
