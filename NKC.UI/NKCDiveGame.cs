using System.Collections;
using ClientPacket.Mode;
using Cs.Logging;
using DG.Tweening;
using NKC.PacketHandler;
using NKC.UI.NPC;
using NKC.UI.Option;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace NKC.UI;

public class NKCDiveGame : NKCUIBase
{
	public struct NKCDGArrival
	{
		public bool m_bOpenEvent;

		public NKCDiveGameSector m_NKCDiveGameSector;

		public NKMRewardData m_RewardData;

		public bool m_bUpdateSquadListUI;

		public bool m_bSectorAddEvent;

		public bool m_bChangedFloor;

		public bool m_bUpdatedPlayerData;

		public void Reset()
		{
			m_bOpenEvent = false;
			m_NKCDiveGameSector = null;
			m_RewardData = null;
			m_bUpdateSquadListUI = false;
			m_bSectorAddEvent = false;
			m_bChangedFloor = false;
			m_bUpdatedPlayerData = false;
		}
	}

	private NKCDiveGameHUD m_NKCDiveGameHUD;

	private NKCDGArrival m_NKCDGArrival;

	public NKCDiveGameSectorLines m_SectorLinesFromSelected;

	public NKCDiveGameSectorLines m_SectorLinesFromMyPos;

	public NKCDiveGameSectorLines m_SectorLinesFromSelectedMyPos;

	private NKCDiveGameSectorSetMgr m_NKCDiveGameSectorSetMgr;

	public NKCDiveGameSector m_StartSector;

	public GameObject m_NKM_UI_DIVE_PROCESS_SECTOR_GRID;

	private NKCDiveGameSector m_Selected_NKCDiveGameSector;

	public GameObject m_NKM_UI_DIVE_PROCESS_UNIT_LAYER;

	private NKCDiveGameUnit m_NKCDiveGameUnit;

	private Rect m_rtCamBound;

	public GameObject m_NKM_UI_DIVE_PROCESS_3D_CONTENT;

	private Transform m_CIRCLESET;

	private CanvasGroup m_cgCIRCLESET;

	private Animator m_NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI;

	private Transform m_tr_NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI;

	private static bool m_bIntro = false;

	private Coroutine m_coIntro;

	private static bool m_bSectorAddEventWhenStart = false;

	private static bool m_bSectorAddEvent = false;

	private static float m_fElapsedTimeForSectorAddEvent = 0f;

	private float m_fTimeForSectorAddEvent = 0.35f;

	private bool m_bRealSetSectorSets;

	private float m_fElapsedTimeForRealSetSecterSets;

	private float m_fTimeForRealSetSecterSets = 0.35f;

	public float m_KeyScrollSensitivity;

	private float m_ScrollReduceTime;

	private KeyCode m_prevKeyCode;

	private float DIVE_UNIT_MOVE_TIME_ = 1.3f;

	private float MOVE_REQ_COROUTINE_WAIT_TIME_ = 2.5f;

	private bool m_bPause;

	private float t_IdleDeltaTime;

	private float m_IdleVoiceInterval = 10f;

	private int m_LastSelectedArtifactSlotIndex = -1;

	private Coroutine m_coDoAfterSectorEventAniEnd;

	private NKCPopupDiveEvent m_NKCPopupDiveEvent;

	private NKCPopupDiveArtifactGet m_NKCPopupDiveArtifactGet;

	private Coroutine m_coMoveReq;

	private static bool m_bReservedUnitDieShow = false;

	private static int m_PrevDeckIndexDied = -1;

	private static Vector3 m_lastGameUnitPos = new Vector3(0f, 0f, 0f);

	public GameObject m_NUM_WARFARE_FX_UNIT_EXPLOSION;

	public GameObject m_NUM_WARFARE_FX_UNIT_ESCAPE;

	private static NKC_DIVE_GAME_UNIT_DIE_TYPE m_NKC_DIVE_GAME_UNIT_DIE_TYPE = NKC_DIVE_GAME_UNIT_DIE_TYPE.NDGUDT_EXPLOSION;

	private bool m_bWaitingBossBeforeCutscenInAuto;

	public float MoviePlaySpeed = 1f;

	private bool m_bWaitingMovie;

	private int m_introSoundUID;

	public override string MenuName => NKCUtilString.GET_STRING_DIVE;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public float DIVE_UNIT_MOVE_TIME
	{
		get
		{
			if (CheckAuto())
			{
				return DIVE_UNIT_MOVE_TIME_ / NKCClientConst.DiveAutoSpeed;
			}
			return DIVE_UNIT_MOVE_TIME_;
		}
		set
		{
			DIVE_UNIT_MOVE_TIME_ = value;
		}
	}

	public float MOVE_REQ_COROUTINE_WAIT_TIME
	{
		get
		{
			if (CheckAuto())
			{
				return MOVE_REQ_COROUTINE_WAIT_TIME_ / NKCClientConst.DiveAutoSpeed;
			}
			return MOVE_REQ_COROUTINE_WAIT_TIME_;
		}
		set
		{
			MOVE_REQ_COROUTINE_WAIT_TIME_ = value;
		}
	}

	public static int WarfareRecoverItemCost { get; private set; }

	private NKCPopupDiveEvent NKCPopupDiveEvent
	{
		get
		{
			if (m_NKCPopupDiveEvent == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupDiveEvent>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NKM_UI_DIVE_EVENT_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), delegate
				{
					m_NKCPopupDiveEvent = null;
				});
				m_NKCPopupDiveEvent = loadedUIData.GetInstance<NKCPopupDiveEvent>();
				m_NKCPopupDiveEvent.InitUI();
			}
			return m_NKCPopupDiveEvent;
		}
	}

	public NKCPopupDiveArtifactGet NKCPopupDiveArtifactGet
	{
		get
		{
			if (m_NKCPopupDiveArtifactGet == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupDiveArtifactGet>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NKM_UI_DIVE_ARTIFACT_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), delegate
				{
					m_NKCPopupDiveArtifactGet = null;
				});
				m_NKCPopupDiveArtifactGet = loadedUIData.GetInstance<NKCPopupDiveArtifactGet>();
				m_NKCPopupDiveArtifactGet.InitUI(OnArtifactEffectExplode, OnFinishScrollToArtifactDummySlot);
			}
			return m_NKCPopupDiveArtifactGet;
		}
	}

	public bool IsOpenNKCPopupDiveArtifactGet
	{
		get
		{
			if (m_NKCPopupDiveArtifactGet != null)
			{
				return m_NKCPopupDiveArtifactGet.IsOpen;
			}
			return false;
		}
	}

	public void SetLastSelectedArtifactSlotIndex(int index)
	{
		m_LastSelectedArtifactSlotIndex = index;
	}

	public void OnArtifactEffectExplode()
	{
		m_NKCDiveGameHUD.m_NKCDiveGameHUDArtifact.InvalidDummySlot();
		m_NKCDiveGameHUD.m_NKCDiveGameHUDArtifact.m_LoopScrollRect.RefreshCells();
		m_NKCDiveGameHUD.m_NKCDiveGameHUDArtifact.UpdateTotalViewTextUI();
	}

	public static void SetReservedUnitDieShow(bool bSet, int prevDiedDeckIndex = -1, NKC_DIVE_GAME_UNIT_DIE_TYPE dieType = NKC_DIVE_GAME_UNIT_DIE_TYPE.NDGUDT_EXPLOSION)
	{
		m_NKC_DIVE_GAME_UNIT_DIE_TYPE = dieType;
		m_bReservedUnitDieShow = bSet;
		m_PrevDeckIndexDied = prevDiedDeckIndex;
	}

	public static void SetDieType(NKC_DIVE_GAME_UNIT_DIE_TYPE _type)
	{
		m_NKC_DIVE_GAME_UNIT_DIE_TYPE = _type;
	}

	public static NKCDiveGame Init()
	{
		NKCDiveGame nKCDiveGame = NKCUIManager.OpenUI<NKCDiveGame>("NKM_UI_DIVE_PROCESS_3D");
		nKCDiveGame.gameObject.SetActive(value: false);
		nKCDiveGame.m_NKCDiveGameHUD = NKCDiveGameHUD.InitUI(nKCDiveGame);
		nKCDiveGame.m_StartSector.SetUI(new NKMDiveSlot(NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_START, NKM_DIVE_EVENT_TYPE.NDET_NONE, 0, 0, 0));
		nKCDiveGame.m_NKCDiveGameSectorSetMgr = new NKCDiveGameSectorSetMgr();
		nKCDiveGame.m_NKCDiveGameSectorSetMgr.Init(nKCDiveGame.m_NKM_UI_DIVE_PROCESS_SECTOR_GRID.transform);
		nKCDiveGame.m_SectorLinesFromSelected.Init();
		nKCDiveGame.m_SectorLinesFromMyPos.Init();
		nKCDiveGame.m_SectorLinesFromSelectedMyPos.Init();
		NKCNPCTemplet nPCTemplet = NKCUINPCBase.GetNPCTemplet(NPC_TYPE.OPERATOR_CHLOE, NPC_ACTION_TYPE.IDLE);
		if (nPCTemplet != null && nPCTemplet.m_ConditionType == NPC_CONDITION.IDLE_TIME && nPCTemplet.m_ConditionValue > 0)
		{
			nKCDiveGame.m_IdleVoiceInterval = nPCTemplet.m_ConditionValue;
		}
		nKCDiveGame.m_bWaitingBossBeforeCutscenInAuto = false;
		return nKCDiveGame;
	}

	public void OnChangedAuto(bool bSet)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null && myUserData.m_UserOption != null)
		{
			NKMPacket_DIVE_AUTO_REQ nKMPacket_DIVE_AUTO_REQ = new NKMPacket_DIVE_AUTO_REQ();
			nKMPacket_DIVE_AUTO_REQ.isAuto = bSet;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DIVE_AUTO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
		}
	}

	private bool CheckAuto()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKMUserOption userOption = myUserData.m_UserOption;
			if (userOption != null)
			{
				return userOption.m_bAutoDive;
			}
		}
		return false;
	}

	private void ClearMoveReqCoroutine()
	{
		if (m_coMoveReq != null)
		{
			StopCoroutine(m_coMoveReq);
			m_coMoveReq = null;
		}
	}

	private IEnumerator MoveReqCoroutine()
	{
		yield return new WaitForSeconds(MOVE_REQ_COROUTINE_WAIT_TIME);
		while (m_bPause)
		{
			yield return null;
		}
		OnClickSectorInfoSearch();
		m_coMoveReq = null;
	}

	private void DoNextThingByAutoWhenExploring()
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData != null)
		{
			NKCDiveGameSector nextDiveGameSectorByAuto = m_NKCDiveGameSectorSetMgr.GetNextDiveGameSectorByAuto(diveGameData.Player.PlayerBase.Distance == 0);
			if (nextDiveGameSectorByAuto != null)
			{
				OnClickSector(nextDiveGameSectorByAuto, bByAuto: true);
				m_coMoveReq = StartCoroutine(MoveReqCoroutine());
			}
		}
	}

	private bool CheckGiveUpRecommendPopupOpenTiming()
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData == null)
		{
			return false;
		}
		if (diveGameData.Player.PlayerBase.State != NKMDivePlayerState.Exploring)
		{
			return false;
		}
		if (!diveGameData.Player.CheckExistPossibleSquadForBattle() && !m_NKCDiveGameSectorSetMgr.CheckExistEuclidInNextSectors())
		{
			return true;
		}
		return false;
	}

	private void DoNextThingByAuto()
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData == null || CheckUnitMoving())
		{
			return;
		}
		if (diveGameData.Player.PlayerBase.State == NKMDivePlayerState.Exploring)
		{
			if (CheckGiveUpRecommendPopupOpenTiming())
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_DIVE_GIVE_UP_RECOMMEND, OnClickOkGiveUpINGDive, DoNextThingByAutoWhenExploring);
			}
			else
			{
				DoNextThingByAutoWhenExploring();
			}
		}
		else if (diveGameData.Player.PlayerBase.State == NKMDivePlayerState.BattleReady)
		{
			if (!m_bWaitingBossBeforeCutscenInAuto && m_Selected_NKCDiveGameSector != null && m_Selected_NKCDiveGameSector.GetNKMDiveSlot().EventType == NKM_DIVE_EVENT_TYPE.NDET_DUNGEON_BOSS && !string.IsNullOrEmpty(GetDiveTemplet().CutsceneDiveBossBefore) && !NKCScenManager.CurrentUserData().CheckDiveHistory(GetDiveTemplet().StageID))
			{
				m_bWaitingBossBeforeCutscenInAuto = true;
				NKCUICutScenPlayer.Instance.LoadAndPlay(GetDiveTemplet().CutsceneDiveBossBefore, 0, DoNextThingByAuto);
				return;
			}
			if (diveGameData.Player.PlayerBase.ReservedDeckIndex >= 0)
			{
				TryEnterDiveGame((byte)diveGameData.Player.PlayerBase.ReservedDeckIndex, diveGameData);
				return;
			}
			NKMDiveSquad squadForBattleByAuto = diveGameData.Player.GetSquadForBattleByAuto();
			if (squadForBattleByAuto != null && squadForBattleByAuto.CurHp >= 0f)
			{
				if (squadForBattleByAuto.Supply <= 0)
				{
					Send_NKMPacket_DIVE_SUICIDE_REQ((byte)squadForBattleByAuto.DeckIndex);
				}
				else
				{
					TryEnterDiveGame((byte)squadForBattleByAuto.DeckIndex, diveGameData);
				}
			}
		}
		else if (diveGameData.Player.PlayerBase.State == NKMDivePlayerState.SelectArtifact)
		{
			NKCPopupDiveArtifactGet.Open(diveGameData.Player.PlayerBase.ReservedArtifacts, CheckAuto(), OnCloseArtifactGetPopup);
		}
	}

	private void OnCloseArtifactGetPopup()
	{
		if (CheckAuto())
		{
			DoNextThingByAuto();
		}
		else if (CheckGiveUpRecommendPopupOpenTiming())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_DIVE_GIVE_UP_RECOMMEND, OnClickOkGiveUpINGDive);
		}
	}

	public void OnClickPause()
	{
		if (!m_bPause && !NKMPopUpBox.IsOpenedWaitBox())
		{
			if (m_NKCDiveGameUnit != null)
			{
				m_NKCDiveGameUnit.SetPause(bSet: true);
			}
			NKCCamera.GetTrackingPos().SetPause(bSet: true);
			NKCUIGameOption.Instance.Open(NKC_GAME_OPTION_MENU_TYPE.DIVE, OnCloseGameOption);
			m_bPause = true;
		}
	}

	public void GiveUp()
	{
		if (GetDiveGameData() != null)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_DIVE_GIVE_UP, OnClickOkGiveUpINGDive);
		}
	}

	private void OnClickOkGiveUpINGDive()
	{
		NKCUIGameOption.CheckInstanceAndClose();
		NKMPacket_DIVE_GIVE_UP_REQ packet = new NKMPacket_DIVE_GIVE_UP_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void TempLeave()
	{
		NKCUIGameOption.CheckInstanceAndClose();
		NKCUtil.SetDiveTargetEventID();
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DIVE_READY);
	}

	private void OnCloseGameOption()
	{
		if (m_NKCDiveGameUnit != null)
		{
			m_NKCDiveGameUnit.SetPause(bSet: false);
		}
		NKCCamera.GetTrackingPos().SetPause(bSet: false);
		m_bPause = false;
	}

	public static void SetIntro(bool bSet)
	{
		m_bIntro = bSet;
	}

	public static void SetSectorAddEvent(bool bSet)
	{
		m_bSectorAddEvent = bSet;
		m_fElapsedTimeForSectorAddEvent = 0f;
	}

	public static void SetSectorAddEventWhenStart(bool bSet)
	{
		m_bSectorAddEventWhenStart = bSet;
		m_fElapsedTimeForSectorAddEvent = 0f;
	}

	public NKMDiveTemplet GetDiveTemplet()
	{
		NKMDiveGameData diveGameData = NKCScenManager.GetScenManager().GetMyUserData().m_DiveGameData;
		if (diveGameData == null)
		{
			return null;
		}
		if (diveGameData.Floor == null)
		{
			return null;
		}
		return diveGameData.Floor.Templet;
	}

	private NKMDiveGameData GetDiveGameData()
	{
		return NKCScenManager.GetScenManager().GetMyUserData().m_DiveGameData;
	}

	private float GetDefaultZDist()
	{
		return -800f;
	}

	private bool CheckUnitMoving()
	{
		if (m_NKCDiveGameUnit != null && m_NKCDiveGameUnit.IsMoving())
		{
			return true;
		}
		return false;
	}

	private void CloseSelectedSectorLines()
	{
		m_SectorLinesFromSelected.Close();
		m_SectorLinesFromSelectedMyPos.Close();
	}

	private void CloseAllSectorLines()
	{
		CloseSelectedSectorLines();
		m_SectorLinesFromMyPos.Close();
	}

	private void OnClickSector(NKCDiveGameSector cNKCDiveGameSector, bool bByAuto)
	{
		if (cNKCDiveGameSector == null || GetDiveGameData() == null || CheckUnitMoving() || (!bByAuto && CheckAuto()))
		{
			return;
		}
		bool flag = !cNKCDiveGameSector.GetGrey();
		if (flag)
		{
			flag = false;
			NKMDiveSlot nKMDiveSlot = cNKCDiveGameSector.GetNKMDiveSlot();
			if (nKMDiveSlot != null && GetDiveGameData().Player.PlayerBase.Distance + 1 == cNKCDiveGameSector.GetDistance())
			{
				if (IsSameCol())
				{
					if (IsSameRow(cNKCDiveGameSector.GetSlotIndex()))
					{
						flag = m_NKCDiveGameHUD.OpenSectorInfo(GetDiveTemplet(), nKMDiveSlot, IsSameCol());
					}
				}
				else
				{
					flag = m_NKCDiveGameHUD.OpenSectorInfo(GetDiveTemplet(), nKMDiveSlot, IsSameCol());
				}
			}
		}
		if (m_Selected_NKCDiveGameSector != null)
		{
			m_Selected_NKCDiveGameSector.SetSelected(bSet: false);
		}
		if (!flag)
		{
			m_NKCDiveGameHUD.CloseSectorInfo();
			InvalidSelectedSector();
			NKCCamera.GetTrackingPos().SetTracking(new NKMVector3(cNKCDiveGameSector.GetFinalPos().x, NKCCamera.GetTrackingPos().GetNowValueY(), GetDefaultZDist()), 1.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
			return;
		}
		m_Selected_NKCDiveGameSector = cNKCDiveGameSector;
		m_Selected_NKCDiveGameSector.SetSelected(bSet: true);
		if (m_Selected_NKCDiveGameSector.GetNKMDiveSlot().EventType == NKM_DIVE_EVENT_TYPE.NDET_DUNGEON_BOSS)
		{
			m_SectorLinesFromSelected.Close();
			OpenSectorLinesFromSelectedMyPos();
		}
		else
		{
			int realSetSize = m_Selected_NKCDiveGameSector.GetRealSetSize();
			int uISlotIndex = m_Selected_NKCDiveGameSector.GetUISlotIndex();
			m_SectorLinesFromSelected.OpenFromSelected(realSetSize, uISlotIndex, m_Selected_NKCDiveGameSector.GetSlotIndex(), NKCDiveManager.IsDiveJump() || m_Selected_NKCDiveGameSector.GetDistance() == GetDiveGameData().Floor.Templet.RandomSetCount);
			m_SectorLinesFromSelected.transform.localPosition = m_Selected_NKCDiveGameSector.GetFinalPos();
			OpenSectorLinesFromSelectedMyPos();
		}
		NKCCamera.GetTrackingPos().SetTracking(new NKMVector3(cNKCDiveGameSector.GetFinalPos().x, cNKCDiveGameSector.GetFinalPos().y, GetDefaultZDist() + 200f), 1.3f, TRACKING_DATA_TYPE.TDT_SLOWER);
	}

	private void OpenSectorLinesFromMyPos()
	{
		NKCDiveGameSector sectorUnderMyShip = GetSectorUnderMyShip();
		if (sectorUnderMyShip != null)
		{
			int realSetSize = sectorUnderMyShip.GetRealSetSize();
			int uISlotIndex = sectorUnderMyShip.GetUISlotIndex();
			int toRealSetSize = (NKCDiveManager.IsDiveJump() ? 1 : GetDiveTemplet().SlotCount);
			m_SectorLinesFromMyPos.OpenFromMyPos(realSetSize, toRealSetSize, uISlotIndex, sectorUnderMyShip.GetSlotIndex(), sectorUnderMyShip == m_StartSector, NKCDiveManager.IsDiveJump() || sectorUnderMyShip.GetDistance() == GetDiveGameData().Floor.Templet.RandomSetCount);
			m_SectorLinesFromMyPos.transform.localPosition = sectorUnderMyShip.GetFinalPos();
		}
	}

	private void OpenSectorLinesFromSelectedMyPos()
	{
		NKCDiveGameSector sectorUnderMyShip = GetSectorUnderMyShip();
		if (sectorUnderMyShip != null && m_Selected_NKCDiveGameSector != null)
		{
			int realSetSize = sectorUnderMyShip.GetRealSetSize();
			int realSetSize2 = m_Selected_NKCDiveGameSector.GetRealSetSize();
			int uISlotIndex = sectorUnderMyShip.GetUISlotIndex();
			int uISlotIndex2 = m_Selected_NKCDiveGameSector.GetUISlotIndex();
			m_SectorLinesFromSelectedMyPos.OpenFromSelectedMyPos(realSetSize, realSetSize2, uISlotIndex, uISlotIndex2, sectorUnderMyShip.GetSlotIndex(), m_Selected_NKCDiveGameSector.GetSlotIndex(), sectorUnderMyShip == m_StartSector, sectorUnderMyShip.GetDistance() == GetDiveGameData().Floor.Templet.RandomSetCount);
			m_SectorLinesFromSelectedMyPos.transform.localPosition = GetPlayerPosByData();
		}
	}

	private void InvalidSelectedSector()
	{
		if (m_Selected_NKCDiveGameSector != null)
		{
			m_Selected_NKCDiveGameSector.SetSelected(bSet: false);
		}
		m_Selected_NKCDiveGameSector = null;
		CloseSelectedSectorLines();
	}

	private void PlayPrevLeaderUnitDieAni()
	{
		NKCUIManager.SetScreenInputBlock(bSet: true);
		m_cgCIRCLESET.DOFade(0f, 2.6f);
		if (m_NKC_DIVE_GAME_UNIT_DIE_TYPE == NKC_DIVE_GAME_UNIT_DIE_TYPE.NDGUDT_EXPLOSION)
		{
			m_NUM_WARFARE_FX_UNIT_EXPLOSION.transform.localPosition = new Vector3(m_NKCDiveGameUnit.transform.localPosition.x, m_NKCDiveGameUnit.transform.localPosition.y - 40f, m_NKCDiveGameUnit.transform.localPosition.z);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION, bValue: false);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_EXPLOSION, bValue: true);
			m_NKCDiveGameUnit.PlayDieAniExplosion(SpawnNewLeaderUnit);
		}
		else if (m_NKC_DIVE_GAME_UNIT_DIE_TYPE == NKC_DIVE_GAME_UNIT_DIE_TYPE.NDGUDT_WARP)
		{
			m_NUM_WARFARE_FX_UNIT_ESCAPE.transform.localPosition = new Vector3(m_NKCDiveGameUnit.transform.localPosition.x, m_NKCDiveGameUnit.transform.localPosition.y - 40f, m_NKCDiveGameUnit.transform.localPosition.z);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_ESCAPE, bValue: false);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_UNIT_ESCAPE, bValue: true);
			m_NKCDiveGameUnit.PlayDieAniWarp(SpawnNewLeaderUnit);
		}
	}

	private void SpawnNewLeaderUnit()
	{
		UpdateDiveGameUnitUI();
		m_NKCDiveGameUnit.transform.localPosition = GetPlayerPosByData();
		NKCCamera.GetTrackingPos().SetTracking(new NKMVector3(m_NKCDiveGameUnit.transform.localPosition.x, m_NKCDiveGameUnit.transform.localPosition.y - 40f, GetDefaultZDist()), 1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		m_NKCDiveGameUnit.PlaySpawnAni(OnCompleteSpawnAni);
	}

	private void OnCompleteSpawnAni()
	{
		OpenSectorLinesFromMyPos();
		NKCUIManager.SetScreenInputBlock(bSet: false);
		if (CheckAuto())
		{
			DoNextThingByAuto();
		}
		else if (CheckGiveUpRecommendPopupOpenTiming())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_DIVE_GIVE_UP_RECOMMEND, OnClickOkGiveUpINGDive);
		}
	}

	private IEnumerator DoAfterSectorEventAniEnd()
	{
		while (m_NKCDiveGameSectorSetMgr.IsAnimating())
		{
			yield return null;
		}
		DoAfterSectorEvent();
	}

	private void Update()
	{
		if (!base.IsOpen)
		{
			return;
		}
		if (m_NKCDiveGameSectorSetMgr != null)
		{
			m_NKCDiveGameSectorSetMgr.Update();
		}
		if (m_NKCDiveGameUnit != null)
		{
			if (m_CIRCLESET != null)
			{
				m_CIRCLESET.localPosition = m_NKCDiveGameUnit.transform.localPosition;
			}
			if (m_tr_NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI != null)
			{
				m_tr_NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI.localPosition = m_NKCDiveGameUnit.transform.localPosition;
			}
			m_lastGameUnitPos = m_NKCDiveGameUnit.transform.localPosition;
		}
		if (m_coIntro == null && (m_bSectorAddEvent || m_bSectorAddEventWhenStart))
		{
			float num = Time.deltaTime;
			if (num > NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f)
			{
				num = NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f;
			}
			m_fElapsedTimeForSectorAddEvent += num;
			if (m_fElapsedTimeForSectorAddEvent >= m_fTimeForSectorAddEvent)
			{
				m_bSectorAddEvent = false;
				m_bSectorAddEventWhenStart = false;
				m_bRealSetSectorSets = true;
				m_fElapsedTimeForRealSetSecterSets = 0f;
				if (m_NKCDiveGameUnit != null)
				{
					m_NKCDiveGameUnit.PlaySearch();
				}
				if (m_NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI != null)
				{
					m_NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI.Play("NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI");
				}
				m_CIRCLESET.transform.DOScale(new Vector3(0f, 0f, 1f), 1.3f).SetEase(Ease.OutQuad).From();
			}
		}
		if (m_bRealSetSectorSets)
		{
			m_fElapsedTimeForRealSetSecterSets += Time.deltaTime;
			if (m_fElapsedTimeForRealSetSecterSets >= m_fTimeForRealSetSecterSets)
			{
				m_bRealSetSectorSets = false;
				m_NKCDiveGameSectorSetMgr.SetUI(GetDiveGameData(), bShowSpawnAni: true);
				m_coDoAfterSectorEventAniEnd = StartCoroutine(DoAfterSectorEventAniEnd());
				if (m_Selected_NKCDiveGameSector != null && !m_Selected_NKCDiveGameSector.CheckSelectable())
				{
					InvalidSelectedSector();
					m_NKCDiveGameHUD.CloseSectorInfo();
				}
			}
		}
		if (Input.anyKeyDown)
		{
			t_IdleDeltaTime = 0f;
		}
		if (!NKCSoundManager.IsPlayingVoice())
		{
			t_IdleDeltaTime += Time.deltaTime;
			if (t_IdleDeltaTime > m_IdleVoiceInterval)
			{
				t_IdleDeltaTime = 0f;
				NKCUINPCOperatorChloe.PlayVoice(NPC_TYPE.OPERATOR_CHLOE, NPC_ACTION_TYPE.IDLE, bStopCurrentSound: true);
			}
		}
		bool flag = false;
		if (Input.GetKey(KeyCode.W))
		{
			flag = KeyScroll(KeyCode.W);
		}
		if (Input.GetKey(KeyCode.S))
		{
			flag = KeyScroll(KeyCode.S);
		}
		if (Input.GetKey(KeyCode.A))
		{
			flag = KeyScroll(KeyCode.A);
		}
		if (Input.GetKey(KeyCode.D))
		{
			flag = KeyScroll(KeyCode.D);
		}
		if (!flag)
		{
			KeyScroll(KeyCode.None);
		}
	}

	private void UpdateDiveGameUnitUI(bool bUsePrevDiedDeck = false)
	{
		if (m_NKCDiveGameUnit != null)
		{
			int leaderUnitID = GetLeaderUnitID(bUsePrevDiedDeck);
			if (leaderUnitID > 0)
			{
				m_NKCDiveGameUnit.SetUI(leaderUnitID);
			}
			m_NKCDiveGameUnit.ResetRotation();
			m_cgCIRCLESET.alpha = 1f;
		}
	}

	private int GetLeaderUnitID(bool bUsePrevDiedDeck = false)
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData == null)
		{
			return 0;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return 0;
		}
		NKMDeckIndex deckIndex = ((!bUsePrevDiedDeck) ? new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, diveGameData.Player.PlayerBase.LeaderDeckIndex) : new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, m_PrevDeckIndexDied));
		NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(deckIndex);
		if (deckData != null)
		{
			NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
			if (shipFromUID != null)
			{
				return shipFromUID.m_UnitID;
			}
		}
		return 0;
	}

	private void SetUIAfterMovie()
	{
		m_coIntro = null;
		NKMDiveGameData diveGameData = GetDiveGameData();
		NKMDiveTemplet diveTemplet = GetDiveTemplet();
		if (diveGameData == null || diveTemplet == null)
		{
			return;
		}
		if (!string.IsNullOrWhiteSpace(diveTemplet.MusicBundleName) && !string.IsNullOrWhiteSpace(diveTemplet.MusicFileName))
		{
			NKCSoundManager.PlayMusic(diveTemplet.MusicFileName, bLoop: true);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_3D_CONTENT, bValue: true);
		m_NKCDiveGameHUD.Open();
		m_NKCDiveGameHUD.PlayIntro();
		m_NKCDiveGameHUD.SetSelectedSquadSlot(-1);
		InvalidSelectedSector();
		NKMDiveSlot nKMDiveSlot = null;
		UpdateDiveGameUnitUI(m_bReservedUnitDieShow);
		if (m_bReservedUnitDieShow)
		{
			m_NKCDiveGameUnit.transform.localPosition = m_lastGameUnitPos;
			PlayPrevLeaderUnitDieAni();
		}
		else
		{
			m_NKCDiveGameUnit.transform.localPosition = GetPlayerPosByData();
			OpenSectorLinesFromMyPos();
		}
		NKCCamera.SetPos(m_NKCDiveGameUnit.transform.localPosition.x, m_NKCDiveGameUnit.transform.localPosition.y, GetDefaultZDist() + 150f, bTrackingStop: true, bForce: true);
		NKCCamera.GetTrackingPos().SetTracking(new NKMVector3(m_NKCDiveGameUnit.transform.localPosition.x, m_NKCDiveGameUnit.transform.localPosition.y, GetDefaultZDist()), 1f, TRACKING_DATA_TYPE.TDT_SLOWER);
		NKCCamera.GetTrackingRotation().SetNowValue(0f, 0f, 0f);
		if (!m_bSectorAddEvent && !m_bSectorAddEventWhenStart)
		{
			if (CheckAuto())
			{
				if (!m_bReservedUnitDieShow)
				{
					DoNextThingByAuto();
				}
			}
			else if (diveGameData.Player.PlayerBase.State == NKMDivePlayerState.BattleReady)
			{
				nKMDiveSlot = diveGameData.Floor.GetSlot(diveGameData.Player.PlayerBase.SlotSetIndex, diveGameData.Player.PlayerBase.SlotIndex);
				if (nKMDiveSlot != null)
				{
					m_NKCDiveGameHUD.OpenSectorInfo(GetDiveTemplet(), nKMDiveSlot, bSameCol: true);
					m_NKCDiveGameHUD.OpenSquadList();
					m_NKCDiveGameHUD.OpenSquadView(diveGameData.Player.PlayerBase.LeaderDeckIndex);
					m_Selected_NKCDiveGameSector = m_NKCDiveGameSectorSetMgr.GetSector(nKMDiveSlot);
					if (m_Selected_NKCDiveGameSector != null)
					{
						m_Selected_NKCDiveGameSector.SetSelected(bSet: true);
					}
					if (diveGameData.Player.PlayerBase.ReservedDeckIndex >= 0)
					{
						m_NKCDiveGameHUD.OpenSquadView(diveGameData.Player.PlayerBase.ReservedDeckIndex);
						OnClickBattle();
					}
				}
			}
			else if (diveGameData.Player.PlayerBase.State == NKMDivePlayerState.SelectArtifact)
			{
				NKCPopupDiveArtifactGet.Open(diveGameData.Player.PlayerBase.ReservedArtifacts, CheckAuto(), OnCloseArtifactGetPopup);
			}
			else if (diveGameData.Player.PlayerBase.State == NKMDivePlayerState.Exploring && CheckGiveUpRecommendPopupOpenTiming() && !m_bReservedUnitDieShow)
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_DIVE_GIVE_UP_RECOMMEND, OnClickOkGiveUpINGDive);
			}
		}
		m_bReservedUnitDieShow = false;
		NKCCamera.GetCamera().orthographic = false;
		m_rtCamBound = new Rect
		{
			xMin = m_StartSector.GetFinalPos().x,
			xMax = GetBossPos().x,
			yMin = -200f,
			yMax = 200f
		};
		CheckTutorial();
	}

	private void AddBG(string prefabName)
	{
		NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<GameObject>("AB_UI_NKM_UI_WORLD_MAP_DIVE", prefabName);
		if (nKCAssetResourceData != null)
		{
			GameObject gameObject = Object.Instantiate(nKCAssetResourceData.GetAsset<GameObject>());
			gameObject.transform.SetParent(m_NKM_UI_DIVE_PROCESS_3D_CONTENT.transform);
			gameObject.transform.SetAsFirstSibling();
			m_NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI = gameObject.transform.Find("NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI").gameObject.GetComponent<Animator>();
			m_tr_NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI = gameObject.transform.Find("NKM_UI_DIVE_PROCESS_3D_SEARCH_ANI").gameObject.transform;
			GameObject gameObject2 = gameObject.transform.Find("CIRCLESET").gameObject;
			m_CIRCLESET = gameObject2.transform;
			m_cgCIRCLESET = gameObject2.gameObject.AddComponent<CanvasGroup>();
			NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
		}
	}

	public void Open()
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		NKMDiveTemplet diveTemplet = GetDiveTemplet();
		if (diveGameData == null || diveTemplet == null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		m_LastSelectedArtifactSlotIndex = -1;
		UIOpened();
		CloseAllSectorLines();
		if (diveTemplet.StageType == NKM_DIVE_STAGE_TYPE.NDST_HARD)
		{
			AddBG("NKM_UI_DIVE_PROCESS_3D_BG_HURDLE");
		}
		else
		{
			AddBG("NKM_UI_DIVE_PROCESS_3D_BG");
		}
		int depth = (NKCDiveManager.IsDiveJump() ? 1 : diveGameData.Floor.Templet.RandomSetCount);
		m_NKCDiveGameSectorSetMgr.Reset(depth, diveTemplet.SlotCount, OnClickSector);
		if (m_bSectorAddEvent)
		{
			m_NKCDiveGameSectorSetMgr.SetUIWhenAddSectorBeforeScan(diveGameData);
		}
		else if (m_bSectorAddEventWhenStart)
		{
			m_NKCDiveGameSectorSetMgr.SetUIWhenStartBeforeScan(diveGameData);
		}
		else
		{
			m_NKCDiveGameSectorSetMgr.SetUI(diveGameData);
		}
		if (m_NKCDiveGameUnit == null)
		{
			m_NKCDiveGameUnit = NKCDiveGameUnit.GetNewInstance(m_NKM_UI_DIVE_PROCESS_UNIT_LAYER.transform);
			NKCUtil.SetGameobjectActive(m_NKCDiveGameUnit, bValue: true);
		}
		Canvas.ForceUpdateCanvases();
		m_fElapsedTimeForSectorAddEvent = 0f;
		m_fElapsedTimeForRealSetSecterSets = 0f;
		m_bRealSetSectorSets = false;
		if (m_bIntro && m_bSectorAddEventWhenStart && !string.IsNullOrEmpty(diveTemplet.CutsceneDiveEnter) && !NKCScenManager.CurrentUserData().CheckDiveHistory(diveTemplet.StageID))
		{
			NKCUICutScenPlayer.Instance.LoadAndPlay(diveTemplet.CutsceneDiveEnter, 0, AfterEnterCutscene);
		}
		else
		{
			AfterEnterCutscene();
		}
	}

	private void AfterEnterCutscene()
	{
		if (m_bIntro)
		{
			NKCUINPCOperatorChloe.PlayVoice(NPC_TYPE.OPERATOR_CHLOE, NPC_ACTION_TYPE.DIVE_START, bStopCurrentSound: true);
			m_bIntro = false;
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_PROCESS_3D_CONTENT, bValue: false);
			m_NKCDiveGameHUD.Close();
			if (m_coIntro != null)
			{
				StopCoroutine(m_coIntro);
			}
			m_coIntro = StartCoroutine(DiveGameUIOpenProcess());
		}
		else
		{
			SetUIAfterMovie();
		}
	}

	private void VideoPlayMessageCallback(NKCUIComVideoPlayer.eVideoMessage message)
	{
		switch (message)
		{
		case NKCUIComVideoPlayer.eVideoMessage.PlayFailed:
		case NKCUIComVideoPlayer.eVideoMessage.PlayComplete:
			m_bWaitingMovie = false;
			break;
		case NKCUIComVideoPlayer.eVideoMessage.PlayBegin:
			break;
		}
	}

	private IEnumerator DiveGameUIOpenProcess()
	{
		NKCUIComVideoCamera videoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (videoPlayer != null)
		{
			videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
			videoPlayer.m_fMoviePlaySpeed = MoviePlaySpeed;
			m_bWaitingMovie = true;
			videoPlayer.Play("Worldmap_Dive_Intro.mp4", bLoop: false, bPlaySound: false, VideoPlayMessageCallback);
			m_introSoundUID = NKCSoundManager.PlaySound("FX_UI_DIVE_START_MOVIE_FRONT", 1f, 0f, 0f);
			while (m_bWaitingMovie)
			{
				yield return null;
				if (Input.anyKeyDown && PlayerPrefs.GetInt("DIVE_GAME_INTRO_SKIP", 0) == 1)
				{
					break;
				}
			}
			videoPlayer.Stop();
			NKCSoundManager.StopSound(m_introSoundUID);
			m_introSoundUID = 0;
			NKCSoundManager.PlaySound("FX_UI_DIVE_START_MOVIE_BACK", 1f, 0f, 0f);
			if (PlayerPrefs.GetInt("DIVE_GAME_INTRO_SKIP", 0) == 0)
			{
				PlayerPrefs.SetInt("DIVE_GAME_INTRO_SKIP", 1);
			}
		}
		m_bWaitingMovie = false;
		NKMDiveTemplet diveTemplet = GetDiveTemplet();
		if (m_bSectorAddEventWhenStart && !string.IsNullOrEmpty(diveTemplet.CutsceneDiveStart) && !NKCScenManager.CurrentUserData().CheckDiveHistory(diveTemplet.StageID))
		{
			NKCUICutScenPlayer.Instance.LoadAndPlay(diveTemplet.CutsceneDiveStart, 0, SetUIAfterMovie);
		}
		else
		{
			SetUIAfterMovie();
		}
	}

	public NKCDiveGameSector GetSectorUnderMyShip()
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData == null)
		{
			return null;
		}
		if (diveGameData.Player.PlayerBase.Distance == 0 && diveGameData.Player.PlayerBase.SlotSetIndex == -1)
		{
			return m_StartSector;
		}
		int num = 0;
		num = diveGameData.Player.PlayerBase.SlotSetIndex;
		return m_NKCDiveGameSectorSetMgr.GetActiveDiveGameSector(num, diveGameData.Player.PlayerBase.SlotIndex);
	}

	public Vector3 GetBossPos()
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		NKCDiveGameSector bossSector = m_NKCDiveGameSectorSetMgr.GetBossSector();
		if (bossSector != null)
		{
			return bossSector.GetFinalPos();
		}
		return result;
	}

	public Vector3 GetPlayerPosByData(bool bMoveACK = false)
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData == null)
		{
			return result;
		}
		if (diveGameData.Player.PlayerBase.Distance == 0 && diveGameData.Player.PlayerBase.SlotSetIndex == -1)
		{
			return m_StartSector.transform.localPosition;
		}
		int num = 0;
		num = diveGameData.Player.PlayerBase.SlotSetIndex;
		if (bMoveACK && diveGameData.Player.PlayerBase.Distance != 1 && diveGameData.Player.PlayerBase.Distance != 0 && (diveGameData.Player.PlayerBase.Distance != diveGameData.Floor.Templet.RandomSetCount || diveGameData.Player.PlayerBase.SlotSetIndex != 1))
		{
			num++;
			if (num > 1)
			{
				num = 1;
			}
		}
		NKCDiveGameSector activeDiveGameSector = m_NKCDiveGameSectorSetMgr.GetActiveDiveGameSector(num, diveGameData.Player.PlayerBase.SlotIndex);
		if (activeDiveGameSector != null)
		{
			return activeDiveGameSector.GetFinalPos();
		}
		return result;
	}

	private bool KeyScroll(KeyCode keyCode)
	{
		if (CheckUnitMoving())
		{
			return false;
		}
		if (m_NKCDiveGameHUD.IsOpenSquadView())
		{
			return false;
		}
		if (CheckAuto())
		{
			return false;
		}
		float num = NKCCamera.GetPosNowX();
		float num2 = NKCCamera.GetPosNowY();
		bool flag = true;
		switch (keyCode)
		{
		case KeyCode.W:
			m_ScrollReduceTime = 1f;
			num2 += Time.deltaTime * m_KeyScrollSensitivity * 0.5f;
			m_prevKeyCode = KeyCode.W;
			break;
		case KeyCode.S:
			m_ScrollReduceTime = 1f;
			num2 -= Time.deltaTime * m_KeyScrollSensitivity * 0.5f;
			m_prevKeyCode = KeyCode.S;
			break;
		case KeyCode.A:
			m_ScrollReduceTime = 1f;
			num -= Time.deltaTime * m_KeyScrollSensitivity;
			m_prevKeyCode = KeyCode.A;
			break;
		case KeyCode.D:
			m_ScrollReduceTime = 1f;
			num += Time.deltaTime * m_KeyScrollSensitivity;
			m_prevKeyCode = KeyCode.D;
			break;
		default:
			m_ScrollReduceTime -= Time.deltaTime;
			flag = false;
			break;
		}
		if (m_ScrollReduceTime <= 0f)
		{
			return false;
		}
		if (!flag)
		{
			float num3 = NKMTrackingFloat.TrackRatio(TRACKING_DATA_TYPE.TDT_FASTER, m_ScrollReduceTime, 1f);
			switch (m_prevKeyCode)
			{
			case KeyCode.W:
				num2 += Time.deltaTime * num3 * m_KeyScrollSensitivity * 0.5f;
				break;
			case KeyCode.S:
				num2 -= Time.deltaTime * num3 * m_KeyScrollSensitivity * 0.5f;
				break;
			case KeyCode.A:
				num -= Time.deltaTime * num3 * m_KeyScrollSensitivity;
				break;
			case KeyCode.D:
				num += Time.deltaTime * num3 * m_KeyScrollSensitivity;
				break;
			}
		}
		else
		{
			m_NKCDiveGameHUD.CloseSectorInfo();
			InvalidSelectedSector();
		}
		num = Mathf.Clamp(num, m_rtCamBound.xMin, m_rtCamBound.xMax);
		num2 = Mathf.Clamp(num2, m_rtCamBound.yMin, m_rtCamBound.yMax);
		NKCCamera.SetPos(num, num2, GetDefaultZDist());
		return flag;
	}

	public override void Hide()
	{
		base.Hide();
		m_NKCDiveGameHUD.Close();
	}

	public override void UnHide()
	{
		base.UnHide();
		m_NKCDiveGameHUD.Open();
	}

	public void OnClickBG(BaseEventData cBaseEventData)
	{
		if (!CheckUnitMoving() && !CheckAuto())
		{
			m_NKCDiveGameHUD.CloseSectorInfo();
			NKCCamera.TrackingPos(1.3f, -1f, -1f, GetDefaultZDist());
			InvalidSelectedSector();
		}
	}

	public void OnDragByInstance(BaseEventData cBaseEventData)
	{
		if (!CheckUnitMoving() && !m_NKCDiveGameHUD.IsOpenSquadView())
		{
			PointerEventData pointerEventData = cBaseEventData as PointerEventData;
			float value = NKCCamera.GetPosNowX() - pointerEventData.delta.x * 10f;
			float value2 = NKCCamera.GetPosNowY() - pointerEventData.delta.y * 10f;
			value = Mathf.Clamp(value, m_rtCamBound.xMin, m_rtCamBound.xMax);
			value2 = Mathf.Clamp(value2, m_rtCamBound.yMin, m_rtCamBound.yMax);
			NKCCamera.TrackingPos(1f, value, value2, GetDefaultZDist());
			if (!CheckAuto())
			{
				m_NKCDiveGameHUD.CloseSectorInfo();
				InvalidSelectedSector();
			}
		}
	}

	public void OnClickSectorInfoSearch()
	{
		if (!(m_Selected_NKCDiveGameSector == null) && !CheckUnitMoving())
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMDiveGameManager.CanMoveForward(m_Selected_NKCDiveGameSector.GetSlotIndex(), myUserData);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
				return;
			}
			NKMPacket_DIVE_MOVE_FORWARD_REQ nKMPacket_DIVE_MOVE_FORWARD_REQ = new NKMPacket_DIVE_MOVE_FORWARD_REQ();
			nKMPacket_DIVE_MOVE_FORWARD_REQ.slotIndex = m_Selected_NKCDiveGameSector.GetSlotIndex();
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_DIVE_MOVE_FORWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_SMALL);
			NKCUINPCOperatorChloe.PlayVoice(NPC_TYPE.OPERATOR_CHLOE, NKCUINPCOperatorChloe.GetNPCActionType(m_Selected_NKCDiveGameSector.GetNKMDiveSlot()), bStopCurrentSound: true);
			ClearMoveReqCoroutine();
		}
	}

	public void OnClickBattle()
	{
		if (m_Selected_NKCDiveGameSector == null)
		{
			return;
		}
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData == null || diveGameData.Player.PlayerBase.State != NKMDivePlayerState.BattleReady)
		{
			return;
		}
		NKMDiveSquad squad = diveGameData.Player.GetSquad(m_NKCDiveGameHUD.GetLastSelectedDeckIndex().m_iIndex);
		if (squad == null)
		{
			return;
		}
		if (squad.Supply <= 0)
		{
			if (!CheckAuto())
			{
				string gET_STRING_WARNING = NKCUtilString.GET_STRING_WARNING;
				string gET_STRING_DIVE_WARNING_SUPPLY = NKCUtilString.GET_STRING_DIVE_WARNING_SUPPLY;
				NKCPopupOKCancel.OpenOKCancelBox(gET_STRING_WARNING, gET_STRING_DIVE_WARNING_SUPPLY, delegate
				{
					Send_NKMPacket_DIVE_SUICIDE_REQ(m_NKCDiveGameHUD.GetLastSelectedDeckIndex().m_iIndex);
				});
			}
			else
			{
				Send_NKMPacket_DIVE_SUICIDE_REQ(m_NKCDiveGameHUD.GetLastSelectedDeckIndex().m_iIndex);
			}
		}
		else if (m_Selected_NKCDiveGameSector.GetNKMDiveSlot().EventType == NKM_DIVE_EVENT_TYPE.NDET_DUNGEON_BOSS && !string.IsNullOrEmpty(GetDiveTemplet().CutsceneDiveBossBefore) && !NKCScenManager.CurrentUserData().CheckDiveHistory(GetDiveTemplet().StageID))
		{
			NKCUICutScenPlayer.Instance.LoadAndPlay(GetDiveTemplet().CutsceneDiveBossBefore, 0, SendGameLoadREQ);
		}
		else
		{
			SendGameLoadREQ();
		}
	}

	private void Send_NKMPacket_DIVE_SUICIDE_REQ(byte deckIndex)
	{
		if (GetDiveGameData() != null)
		{
			NKCPacketSender.Send_NKMPacket_DIVE_SUICIDE_REQ(deckIndex);
		}
	}

	private void SendGameLoadREQ()
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData != null)
		{
			TryEnterDiveGame(m_NKCDiveGameHUD.GetLastSelectedDeckIndex().m_iIndex, diveGameData);
		}
	}

	private void TryEnterDiveGame(byte DeckIndex, NKMDiveGameData cNKMDiveGameData)
	{
		if (cNKMDiveGameData == null)
		{
			Log.Error("[Dive][TryEnterDiveGame] cNKMDiveGameData is Null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCDiveGame.cs", 1520);
			return;
		}
		if (cNKMDiveGameData.Player == null)
		{
			Log.Error("[Dive][TryEnterDiveGame] cNKMDiveGameData.Player is Null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCDiveGame.cs", 1526);
			return;
		}
		if (cNKMDiveGameData.Player.PlayerBase == null)
		{
			Log.Error("[Dive][TryEnterDiveGame] cNKMDiveGameData.Player.PlayerBase is Null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCDiveGame.cs", 1532);
			return;
		}
		int slotSetIndex = cNKMDiveGameData.Player.PlayerBase.SlotSetIndex;
		int slotIndex = cNKMDiveGameData.Player.PlayerBase.SlotIndex;
		NKMDiveSlot slot = cNKMDiveGameData.Floor.GetSlot(slotSetIndex, slotIndex);
		if (slot == null)
		{
			Log.Error($"[Dive][TryEnterDiveGame] SelectedSlot is Null SlotSetIndex[{slotSetIndex}] SlotIndex[{slotIndex}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCDiveGame.cs", 1542);
			return;
		}
		if (slot.EventType == NKM_DIVE_EVENT_TYPE.NDET_DUNGEON_BOSS)
		{
			int bossStageReqItemID = cNKMDiveGameData.Floor.Templet.BossStageReqItemID;
			int bossStageReqItemCount = cNKMDiveGameData.Floor.Templet.BossStageReqItemCount;
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(bossStageReqItemID) < bossStageReqItemCount)
			{
				NKCShopManager.OpenItemLackPopup(bossStageReqItemID, bossStageReqItemCount);
				return;
			}
		}
		string dungeonStrID = NKMDungeonManager.GetDungeonStrID(cNKMDiveGameData.Player.PlayerBase.ReservedDungeonID);
		int stageID = cNKMDiveGameData.Floor.Templet.StageID;
		NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(DeckIndex, 0, stageID, dungeonStrID, 0, bLocal: false, 1, 0, 0L);
	}

	public override void CloseInternal()
	{
		NKCUIManager.SetScreenInputBlock(bSet: false);
		if (m_coIntro != null)
		{
			StopCoroutine(m_coIntro);
			m_coIntro = null;
		}
		ClearMoveReqCoroutine();
		if (m_introSoundUID != 0)
		{
			NKCSoundManager.StopSound(m_introSoundUID);
			m_introSoundUID = 0;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_NKCDiveGameHUD.Close();
		if (m_NKCDiveGameUnit != null)
		{
			m_NKCDiveGameUnit.Clear();
		}
		if (m_cgCIRCLESET != null)
		{
			m_cgCIRCLESET.DOKill();
		}
		if (m_CIRCLESET != null)
		{
			m_CIRCLESET.transform.DOKill();
		}
		NKCPopupDiveEvent.Close();
		NKCPopupDiveArtifactGet.Close();
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.Stop();
		}
		m_NKCDiveGameSectorSetMgr.StopAni();
		if (m_coDoAfterSectorEventAniEnd != null)
		{
			StopCoroutine(m_coDoAfterSectorEventAniEnd);
			m_coDoAfterSectorEventAniEnd = null;
		}
	}

	public override void OnBackButton()
	{
		if (!NKCUIManager.CheckScreenInputBlock())
		{
			if (m_bWaitingMovie)
			{
				m_bWaitingMovie = false;
			}
			else
			{
				OnClickPause();
			}
		}
	}

	private bool IsSameCol()
	{
		if (GetDiveGameData().Player.PlayerBase.Distance == 0)
		{
			return GetDiveGameData().Player.PlayerBase.SlotSetIndex == 0;
		}
		return GetDiveGameData().Player.PlayerBase.SlotSetIndex == 1;
	}

	private bool IsSameRow(int row)
	{
		return GetDiveGameData().Player.PlayerBase.SlotIndex == row;
	}

	private void UpdateBattleReadyHUD()
	{
		if (m_NKCDGArrival.m_NKCDiveGameSector == null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
			return;
		}
		m_NKCDiveGameHUD.UpdateSectorInfoUI(GetDiveTemplet(), m_NKCDGArrival.m_NKCDiveGameSector.GetNKMDiveSlot(), IsSameCol());
		m_NKCDiveGameHUD.OpenSquadList();
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData == null)
		{
			return;
		}
		if (CheckAuto())
		{
			NKMDiveSquad squadForBattleByAuto = diveGameData.Player.GetSquadForBattleByAuto();
			if (squadForBattleByAuto != null)
			{
				m_NKCDiveGameHUD.OpenSquadView(squadForBattleByAuto.DeckIndex);
			}
		}
		else
		{
			m_NKCDiveGameHUD.OpenSquadView(diveGameData.Player.PlayerBase.LeaderDeckIndex);
		}
	}

	private void UpdateHUDByMoveACK()
	{
		NKMDiveGameData diveGameData = GetDiveGameData();
		if (diveGameData != null)
		{
			if (diveGameData.Player.PlayerBase.State == NKMDivePlayerState.BattleReady)
			{
				UpdateBattleReadyHUD();
			}
			else
			{
				m_NKCDiveGameHUD.CloseSectorInfo();
			}
		}
	}

	private void DoAfterSectorEvent()
	{
		if (CheckAuto())
		{
			DoNextThingByAuto();
		}
		else if (GetDiveGameData().Player.PlayerBase.State == NKMDivePlayerState.SelectArtifact)
		{
			NKCPopupDiveArtifactGet.Open(GetDiveGameData().Player.PlayerBase.ReservedArtifacts, CheckAuto(), OnCloseArtifactGetPopup);
		}
		else if (GetDiveGameData().Player.PlayerBase.State == NKMDivePlayerState.Exploring && CheckGiveUpRecommendPopupOpenTiming())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_DIVE_GIVE_UP_RECOMMEND, OnClickOkGiveUpINGDive);
		}
	}

	private void OnUnitMoveComplete()
	{
		if (m_NKCDGArrival.m_bUpdatedPlayerData)
		{
			UpdateDiveGameUnitUI();
			m_NKCDiveGameHUD.UpdateExploreCountLeftUI();
		}
		if (m_NKCDGArrival.m_bOpenEvent)
		{
			if (m_NKCDGArrival.m_bSectorAddEvent)
			{
				if (m_NKCDGArrival.m_bChangedFloor && GetDiveGameData() != null)
				{
					m_NKCDiveGameSectorSetMgr.SetUIWhenAddSectorBeforeScan(GetDiveGameData());
				}
				NKCPopupDiveEvent.Open(CheckAuto(), m_NKCDGArrival.m_NKCDiveGameSector.GetNKMDiveSlot(), m_NKCDGArrival.m_RewardData, delegate
				{
					SetSectorAddEvent(bSet: true);
				});
			}
			else
			{
				if (m_NKCDGArrival.m_bChangedFloor && GetDiveGameData() != null)
				{
					m_NKCDiveGameSectorSetMgr.SetUI(GetDiveGameData());
				}
				NKCPopupDiveEvent.Open(CheckAuto(), m_NKCDGArrival.m_NKCDiveGameSector.GetNKMDiveSlot(), m_NKCDGArrival.m_RewardData, delegate
				{
					DoAfterSectorEvent();
				});
			}
		}
		else if (m_NKCDGArrival.m_bSectorAddEvent)
		{
			SetSectorAddEvent(bSet: true);
			m_NKCDiveGameSectorSetMgr.SetUIWhenAddSectorBeforeScan(GetDiveGameData());
		}
		else
		{
			if (m_NKCDGArrival.m_bChangedFloor && GetDiveGameData() != null)
			{
				m_NKCDiveGameSectorSetMgr.SetUI(GetDiveGameData());
			}
			DoAfterSectorEvent();
		}
		if (m_NKCDGArrival.m_bUpdateSquadListUI)
		{
			m_NKCDiveGameHUD.UpdateSquadListUI();
		}
		UpdateHUDByMoveACK();
		OpenSectorLinesFromMyPos();
	}

	public void OnRecv(NKMPacket_DIVE_SUICIDE_ACK cNKMPacket_DIVE_SUICIDE_ACK)
	{
		if (m_Selected_NKCDiveGameSector != null)
		{
			m_Selected_NKCDiveGameSector.SetSelected(bSet: false);
			m_Selected_NKCDiveGameSector = null;
		}
		m_NKCDiveGameHUD.SetSelectedSquadSlot(-1);
		m_NKCDiveGameHUD.CloseSectorInfo();
		m_NKCDiveGameHUD.CloseSquadView();
		m_NKCDiveGameHUD.UpdateSquadListUI();
		m_SectorLinesFromMyPos.Close();
		m_NKCDiveGameSectorSetMgr.SetUI(GetDiveGameData());
		PlayPrevLeaderUnitDieAni();
		m_bReservedUnitDieShow = false;
	}

	public void OnRecv(NKMPacket_DIVE_AUTO_ACK cNKMPacket_DIVE_AUTO_ACK)
	{
		if (cNKMPacket_DIVE_AUTO_ACK.isAuto)
		{
			DoNextThingByAuto();
		}
		else if (m_coMoveReq != null)
		{
			StopCoroutine(m_coMoveReq);
			m_coMoveReq = null;
		}
	}

	public void OnFinishScrollToArtifactDummySlot()
	{
		NKCUIDiveGameArtifactSlot component = m_NKCDiveGameHUD.m_NKCDiveGameHUDArtifact.m_LoopScrollRect.GetLastActivatedItem().GetComponent<NKCUIDiveGameArtifactSlot>();
		if (!(component == null))
		{
			NKCPopupDiveArtifactGet.SetEffectDestPos(component.m_NKCUISlot.transform.position);
		}
	}

	public void OnRecv(NKMPacket_DIVE_SELECT_ARTIFACT_ACK cNKMPacket_DIVE_SELECT_ARTIFACT_ACK)
	{
		if (m_LastSelectedArtifactSlotIndex >= 0)
		{
			NKCPopupDiveArtifactGet.PlayOutroAni(m_LastSelectedArtifactSlotIndex);
			m_NKCDiveGameHUD.m_NKCDiveGameHUDArtifact.SetDummySlot();
		}
		else
		{
			NKCPopupDiveArtifactGet.Close();
			m_NKCDiveGameHUD.m_NKCDiveGameHUDArtifact.RefreshInvenry();
		}
	}

	public void OnRecv(NKMPacket_DIVE_MOVE_FORWARD_ACK cNKMPacket_DIVE_MOVE_FORWARD_ACK)
	{
		if (cNKMPacket_DIVE_MOVE_FORWARD_ACK.diveSyncData != null && GetDiveGameData() != null)
		{
			CloseAllSectorLines();
			m_NKCDGArrival.Reset();
			NKMDiveSyncData diveSyncData = cNKMPacket_DIVE_MOVE_FORWARD_ACK.diveSyncData;
			m_NKCDGArrival.m_bChangedFloor = true;
			if (diveSyncData.AddedSlotSets.Count > 0)
			{
				m_NKCDGArrival.m_bSectorAddEvent = true;
			}
			if (diveSyncData.UpdatedPlayer != null)
			{
				m_NKCDGArrival.m_bUpdatedPlayerData = true;
			}
			if (diveSyncData.UpdatedSquads.Count > 0)
			{
				m_NKCDGArrival.m_bUpdateSquadListUI = true;
			}
			m_NKCDiveGameUnit.Move(GetPlayerPosByData(bMoveACK: true), DIVE_UNIT_MOVE_TIME, OnUnitMoveComplete);
			NKCCamera.TrackingPos(1.3f, -1f, -1f, GetDefaultZDist());
			m_NKCDGArrival.m_NKCDiveGameSector = m_Selected_NKCDiveGameSector;
			m_NKCDGArrival.m_RewardData = cNKMPacket_DIVE_MOVE_FORWARD_ACK.diveSyncData.RewardData;
			if (m_Selected_NKCDiveGameSector != null && m_Selected_NKCDiveGameSector.GetNKMDiveSlot() != null && NKCDiveManager.IsEuclidSectorType(m_Selected_NKCDiveGameSector.GetNKMDiveSlot().SectorType) && m_Selected_NKCDiveGameSector.GetNKMDiveSlot().EventType != NKM_DIVE_EVENT_TYPE.NDET_BLANK)
			{
				m_NKCDGArrival.m_bOpenEvent = true;
			}
		}
	}

	public void OnRecv(NKMPacket_DIVE_EXPIRE_NOT cNKMPacket_DIVE_EXPIRE_NOT)
	{
		NKMDiveTemplet nKMDiveTemplet = NKMDiveTemplet.Find(cNKMPacket_DIVE_EXPIRE_NOT.stageID);
		if (nKMDiveTemplet != null)
		{
			if (nKMDiveTemplet.IsEventDive)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetReservedDiveReverseAni(bSet: true);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
			}
			else
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE_READY().SetTargetEventID(0, 0);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DIVE_READY);
			}
		}
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.DiveStart);
	}
}
