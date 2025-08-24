using System;
using System.Collections.Generic;
using System.Text;
using ClientPacket.Game;
using ClientPacket.User;
using Cs.Logging;
using Cs.Protocol;
using NKC.Game;
using NKC.Loading;
using NKC.PacketHandler;
using NKC.UI;
using NKC.UI.HUD;
using NKC.UI.Option;
using NKM;
using NKM.Game;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCGameClient : NKMGame
{
	private bool m_bIntrude;

	private bool m_bLoadComplete;

	private NKMGameData m_NKMGameDataDummy;

	private bool m_bIntrudeDummy;

	protected NKCEffectManager m_NKCEffectManager = new NKCEffectManager();

	private NKM_GAME_CAMERA_MODE m_NKM_GAME_CAMERA_MODE;

	private float m_fCameraNormalTackingWaitTime;

	private bool m_bCameraDrag;

	private Vector2 m_vLastDragPos = Vector2.zero;

	private float m_fCameraStopDragTime;

	private short m_CameraFocusGameUnitUID;

	private NKCGameShipSkillArea m_ShipSkillArea;

	private NKCGameUnitDragOffset m_UnitDragOffset;

	private bool m_bShipSkillDrag;

	private float m_fShipSkillDragPosX;

	private int m_SelectShipSkillID;

	private List<NKCAssetResourceData> m_listLoadAssetResourceData = new List<NKCAssetResourceData>();

	private List<NKCAssetResourceData> m_listLoadAssetResourceDataTemp = new List<NKCAssetResourceData>();

	private List<NKCASEffect> m_listEffectLoadTemp = new List<NKCASEffect>();

	private LinkedList<NKCASEffect> m_linklistHitEffect = new LinkedList<NKCASEffect>();

	private bool m_bSimpleHitEffectFrame;

	private NKCSkillCutInSide m_NKCSkillCutInSideLeft;

	private NKCSkillCutInSide m_NKCSkillCutInSideRight;

	private NKCSkillCutInSide m_NKCSkillCutInSideRedLeft;

	private NKCSkillCutInSide m_NKCSkillCutInSideRedRight;

	private NKCDamageEffectManager m_DEManager = new NKCDamageEffectManager();

	private NKCMap m_Map = new NKCMap();

	private NKCBattleCondition m_BattleCondition = new NKCBattleCondition();

	private NKMTrackingFloat m_CameraDrag = new NKMTrackingFloat();

	private float m_fRemainGameTimeBeforeSync;

	private LinkedList<NKMGameSyncData_Base> m_linklistGameSyncData = new LinkedList<NKMGameSyncData_Base>();

	private Dictionary<short, NKCUnitViewer> m_dicUnitViewer = new Dictionary<short, NKCUnitViewer>();

	private List<short> m_listUnitViewerRemove = new List<short>();

	private float m_CameraDragTime;

	private float m_CameraDragDist;

	private bool m_CameraDragPositive;

	private bool m_bTutorialGameReRespawnAllowed;

	private NKCGameHud m_NKCGameHud;

	private bool m_bDeckDrag;

	private bool m_bDeckTouchDown;

	private bool m_bShipSkillDeckTouchDown;

	private bool m_bCameraTouchDown;

	private long m_DeckSelectUnitUID;

	private NKMUnitTemplet m_DeckSelectUnitTemplet;

	private float m_bDeckSelectPosX;

	private Dictionary<long, float> m_dicRespawnCostHolder = new Dictionary<long, float>();

	private Dictionary<long, float> m_dicRespawnCostHolderTC = new Dictionary<long, float>();

	private Dictionary<long, float> m_dicRespawnCostHolderAssist = new Dictionary<long, float>();

	private int m_DeckDragIndex = -1;

	private int m_ShipSkillDeckDragIndex = -1;

	private bool m_bShowUI = true;

	private Vector3 m_Vec3Temp;

	private StringBuilder m_StringBuilder = new StringBuilder();

	private int m_StartEffectUID;

	private int m_EndEffectUID;

	private float m_fLocalGameTime;

	private float m_fLastRecvTime;

	private float m_fLatencyLevel;

	private float m_fLatestSyncPacketGameTime;

	private float m_fNoSyncDataTime;

	private float m_fGameTimeDifference = 3f;

	private const float DEFAULT_WIN_LOSE_WAIT_TIME = 3f;

	private float m_fWinLoseWaitTime = 3f;

	private const float m_DECK_DRAG_INVALID_GAP_SIZE_Z = 70f;

	public NKM_TEAM_TYPE m_MyTeam;

	private NKMMinMaxVec3 m_TempMinMaxVec3 = new NKMMinMaxVec3();

	protected bool m_bSortUnitZDirty = true;

	protected List<NKCUnitClient> m_listSortUnitZ = new List<NKCUnitClient>();

	private float m_fGCTime = 60f;

	private NKC_OPEN_POPUP_TYPE_AFTER_PAUSE m_NKC_OPEN_POPUP_TYPE_AFTER_PAUSE;

	private bool m_bRespawnUnit;

	private const float DANGER_MSG_UNIT_REPWAN_TIME = 5f;

	private bool m_bCostSpeedVoicePlayed;

	private float m_lastMyShipHpRate;

	private float m_lastEnemyShipHpRate;

	private int m_loadedUnitId = -1;

	public long DeckSelectUnitUID => m_DeckSelectUnitUID;

	public NKMUnitTemplet DeckSelectUnitTemplet => m_DeckSelectUnitTemplet;

	public float LatencyLevel
	{
		get
		{
			return m_fLatencyLevel;
		}
		set
		{
			m_fLatencyLevel = value;
		}
	}

	public int SyncPacketCount => m_linklistGameSyncData.Count;

	public float LastSyncPacketTimeDiff => m_fGameTimeDifference;

	public int MultiplyReward { get; private set; }

	public bool GetIntrude()
	{
		return m_bIntrude;
	}

	public void SetIntrude(bool value)
	{
		m_bIntrude = value;
	}

	public bool GetLoadComplete()
	{
		return m_bLoadComplete;
	}

	public NKMGameData GetGameDataDummy()
	{
		return m_NKMGameDataDummy;
	}

	public NKCEffectManager GetNKCEffectManager()
	{
		return m_NKCEffectManager;
	}

	public void SetCameraNormalTackingWaitTime(float fCameraNormalTackingWaitTime)
	{
		m_fCameraNormalTackingWaitTime = fCameraNormalTackingWaitTime;
	}

	public bool GetShipSkillDrag()
	{
		return m_bShipSkillDrag;
	}

	public float GetShipSkillDragPosX()
	{
		return m_fShipSkillDragPosX;
	}

	public int GetSelectShipSkillID()
	{
		return m_SelectShipSkillID;
	}

	public NKCGameHud GetGameHud()
	{
		return m_NKCGameHud;
	}

	public void ClearGameHud()
	{
		if (m_NKCGameHud != null)
		{
			m_NKCGameHud.Clear();
		}
	}

	public NKM_GAME_CAMERA_MODE GetCameraMode()
	{
		return m_NKM_GAME_CAMERA_MODE;
	}

	public void SetCameraMode(NKM_GAME_CAMERA_MODE eNKM_GAME_CAMERA_MODE)
	{
		if (eNKM_GAME_CAMERA_MODE != NKM_GAME_CAMERA_MODE.NGCM_FOCUS_UNIT)
		{
			m_CameraFocusGameUnitUID = 0;
		}
		m_NKM_GAME_CAMERA_MODE = eNKM_GAME_CAMERA_MODE;
		if (eNKM_GAME_CAMERA_MODE != NKM_GAME_CAMERA_MODE.NGCM_DRAG)
		{
			m_bCameraDrag = false;
		}
	}

	public void SetCameraFocusUnit(short gameUnitUID)
	{
		m_CameraFocusGameUnitUID = gameUnitUID;
	}

	public void SetCameraFocusUnitOut(short gameUnitUID)
	{
		if (gameUnitUID == m_CameraFocusGameUnitUID)
		{
			m_CameraFocusGameUnitUID = 0;
		}
	}

	public bool GetDeckDrag()
	{
		return m_bDeckDrag;
	}

	public NKCUnitClient GetDragUnit()
	{
		NKMUnitData unitDataByUnitUID = base.m_NKMGameData.GetUnitDataByUnitUID(m_DeckSelectUnitUID);
		if (unitDataByUnitUID != null && unitDataByUnitUID.m_listGameUnitUID.Count > 0)
		{
			short gameUnitUID = unitDataByUnitUID.m_listGameUnitUID[0];
			return (NKCUnitClient)GetUnit(gameUnitUID, bChain: true, bPool: true);
		}
		return null;
	}

	public float GetDeckSelectPosX()
	{
		return m_bDeckSelectPosX;
	}

	public bool GetDeckTouchDown()
	{
		return m_bDeckTouchDown;
	}

	public bool GetShipSkillDeckTouchDown()
	{
		return m_bShipSkillDeckTouchDown;
	}

	public bool GetCameraTouchDown()
	{
		return m_bCameraTouchDown;
	}

	public bool IsShowUI()
	{
		return m_bShowUI;
	}

	public void SetSortUnitZDirty(bool bSortUnitZDirty)
	{
		m_bSortUnitZDirty = bSortUnitZDirty;
	}

	public NKCGameClient()
	{
		m_NKM_GAME_CLASS_TYPE = NKM_GAME_CLASS_TYPE.NGCT_GAME_CLIENT;
		m_ObjectPool = new NKCObjectPool();
		Init();
	}

	public void InitUI(NKCGameHud gameHUD)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.LATENCY_OPTIMIZATION))
		{
			Log.Debug("[LATENCY_OPTIMIZATION] Starting with LATENCY_OPTIMIZATION ON", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 358);
		}
		if (gameHUD == null)
		{
			throw new Exception("HUD Prefab error!");
		}
		m_NKCGameHud = gameHUD;
		m_NKCGameHud.SetGameClient(this);
		if (NKCReplayMgr.IsPlayingReplay())
		{
			m_NKCGameHud.InitUI(NKCGameHud.HUDMode.Replay);
		}
		else if (IsObserver(NKCScenManager.CurrentUserData()))
		{
			m_NKCGameHud.InitUI(NKCGameHud.HUDMode.Observer);
		}
		else
		{
			m_NKCGameHud.InitUI(NKCGameHud.HUDMode.Normal);
		}
	}

	public override void Init()
	{
		ObjectParentRestore();
		base.Init();
		m_bLoadComplete = false;
		SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_NORMAL_TRACKING);
		m_fCameraNormalTackingWaitTime = 3f;
		m_fCameraStopDragTime = 0f;
		m_CameraFocusGameUnitUID = 0;
		if (m_UnitDragOffset == null)
		{
			m_UnitDragOffset = NKCGameUnitDragOffset.GetInstance();
		}
		m_UnitDragOffset.Init();
		if (m_ShipSkillArea == null)
		{
			m_ShipSkillArea = NKCGameShipSkillArea.GetInstance();
		}
		m_ShipSkillArea.Init();
		m_dicUnitViewer.Clear();
		m_NKCEffectManager.Init();
		m_DEManager.Init(this);
		m_Map.Init();
		m_CameraDrag.SetNowValue(0f);
		m_BattleCondition.Init();
		m_StartEffectUID = 0;
		m_fRemainGameTimeBeforeSync = 0f;
		m_linklistGameSyncData.Clear();
		m_CameraDragTime = 0f;
		m_CameraDragDist = 0f;
		m_CameraDragPositive = true;
		m_bDeckTouchDown = false;
		m_bShipSkillDeckTouchDown = false;
		m_bCameraTouchDown = false;
		m_bDeckDrag = false;
		m_DeckSelectUnitUID = 0L;
		m_DeckSelectUnitTemplet = null;
		m_bDeckSelectPosX = 0f;
		m_dicRespawnCostHolder.Clear();
		m_dicRespawnCostHolderAssist.Clear();
		m_dicRespawnCostHolderTC.Clear();
		m_DeckDragIndex = -1;
		m_ShipSkillDeckDragIndex = -1;
		m_fLocalGameTime = 0f;
		m_fLastRecvTime = 0f;
		m_fLatencyLevel = 1f;
		m_fNoSyncDataTime = 0f;
		m_fWinLoseWaitTime = 3f;
		if (m_NKMGameRuntimeData != null)
		{
			m_NKMGameRuntimeData.m_NKM_GAME_STATE = NKM_GAME_STATE.NGS_INVALID;
		}
		m_NKCSkillCutInSideLeft = null;
		m_NKCSkillCutInSideRight = null;
		m_NKCSkillCutInSideRedLeft = null;
		m_NKCSkillCutInSideRedRight = null;
		m_linklistHitEffect.Clear();
		m_bRespawnUnit = false;
		m_NKC_OPEN_POPUP_TYPE_AFTER_PAUSE = NKC_OPEN_POPUP_TYPE_AFTER_PAUSE.NOPTAP_GAME_OPTION_POPUP;
		m_bCostSpeedVoicePlayed = false;
		m_lastMyShipHpRate = 0f;
		m_lastEnemyShipHpRate = 0f;
	}

	public override void InitUnit()
	{
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnitPool.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKCUnitClient nKCUnitClient = (NKCUnitClient)enumerator.Current.Value;
			if (nKCUnitClient != null)
			{
				GetObjectPool().CloseObj(nKCUnitClient);
			}
		}
		m_dicNKMUnitPool.Clear();
		for (int i = 0; i < m_listNKMUnit.Count; i++)
		{
			NKCUnitClient nKCUnitClient2 = (NKCUnitClient)m_listNKMUnit[i];
			if (nKCUnitClient2 != null)
			{
				GetObjectPool().CloseObj(nKCUnitClient2);
			}
		}
		m_listNKMUnit.Clear();
		m_dicNKMUnit.Clear();
	}

	public NKMGameTeamData GetMyTeamData()
	{
		return GetGameData().GetTeamData(m_MyTeam);
	}

	public NKMGameRuntimeTeamData GetMyRunTimeTeamData()
	{
		return GetGameRuntimeData().GetMyRuntimeTeamData(m_MyTeam);
	}

	public void SetGameDataDummy(NKMGameData cNKMGameData, bool bIntrude = false)
	{
		m_NKMGameDataDummy = cNKMGameData;
		m_bIntrudeDummy = bIntrude;
		if (m_NKMGameDataDummy.m_DungeonID <= 0 || cNKMGameData.m_NKMGameTeamDataB == null || !string.IsNullOrEmpty(cNKMGameData.m_NKMGameTeamDataB.m_UserNickname))
		{
			return;
		}
		NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(m_NKMGameDataDummy.m_DungeonID);
		if (dungeonTemplet != null && dungeonTemplet.m_DungeonTempletBase.m_DungeonType != NKM_DUNGEON_TYPE.NDT_WAVE)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(dungeonTemplet.m_BossUnitStrID);
			if (unitTempletBase != null)
			{
				cNKMGameData.m_NKMGameTeamDataB.m_UserNickname = unitTempletBase.GetUnitName();
			}
		}
	}

	private void SetGameData(NKMGameData cNKMGameData, bool bIntrude = false)
	{
		if (cNKMGameData == null)
		{
			Debug.LogError("FATAL : GameData Null!!");
			return;
		}
		Debug.Log("<color=#FFFF00>NKCGameClient.SetGameData()</color>");
		Init();
		m_bIntrude = bIntrude;
		base.m_NKMGameData = cNKMGameData;
		base.m_NKMGameData.SetDungeonRespawnUnitTemplet();
		if (base.m_NKMGameData != null)
		{
			NKCMMPManager.OnEnterDungeon(base.m_NKMGameData.m_DungeonID);
		}
		SetMyTeam();
		SetDefaultTacticalCommand();
		if (base.m_NKMGameData.m_DungeonID > 0)
		{
			m_NKMDungeonTemplet = NKMDungeonManager.GetDungeonTemplet(base.m_NKMGameData.m_DungeonID);
		}
		else
		{
			m_NKMDungeonTemplet = null;
		}
		InitDungeonEventData();
		ResetWinLoseWaitTime();
		if (m_NKMDungeonTemplet != null)
		{
			GetGameDevModeData().m_bDevForcePvp = m_NKMDungeonTemplet.m_bDevForcePVP;
		}
		if (!IsGameUsingDoubleCostTime())
		{
			base.m_NKMGameData.m_fDoubleCostTime = -100f;
		}
		Log.Info("NKCGameClient.SetGameData End", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 577);
	}

	public bool IsObserver(NKMUserData userData)
	{
		if (m_NKMGameDataDummy == null)
		{
			return false;
		}
		if (m_NKMGameDataDummy.GetGameType() != NKM_GAME_TYPE.NGT_PVP_PRIVATE)
		{
			return false;
		}
		if (userData == null)
		{
			return false;
		}
		long userUID = userData.m_UserUID;
		if (userUID == m_NKMGameDataDummy.m_NKMGameTeamDataA.m_user_uid)
		{
			return false;
		}
		if (userUID == m_NKMGameDataDummy.m_NKMGameTeamDataB.m_user_uid)
		{
			return false;
		}
		return true;
	}

	public void SetMyTeam()
	{
		if (NKCReplayMgr.IsPlayingReplay() || IsObserver(NKCScenManager.CurrentUserData()))
		{
			if (NKCScenManager.CurrentUserData().m_UserUID == 0L)
			{
				m_MyTeam = NKM_TEAM_TYPE.NTT_A1;
			}
			else
			{
				NKMGameTeamData teamData = base.m_NKMGameData.GetTeamData(NKCScenManager.CurrentUserData().m_UserUID);
				if (teamData != null)
				{
					m_MyTeam = teamData.m_eNKM_TEAM_TYPE;
				}
				else
				{
					m_MyTeam = NKM_TEAM_TYPE.NTT_A1;
				}
			}
		}
		else if (NKCScenManager.CurrentUserData() != null && base.m_NKMGameData.GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE)
		{
			m_MyTeam = base.m_NKMGameData.GetTeamType(NKCScenManager.CurrentUserData().m_UserUID);
		}
		else
		{
			m_MyTeam = NKM_TEAM_TYPE.NTT_A1;
		}
		GetGameHud().CurrentViewTeamType = m_MyTeam;
	}

	private void ResetWinLoseWaitTime()
	{
		if (GetDungeonType() == NKM_DUNGEON_TYPE.NDT_WAVE)
		{
			m_fWinLoseWaitTime = 0.01f;
		}
		else
		{
			m_fWinLoseWaitTime = 3f;
		}
	}

	public void SetGameRuntimeData(NKMGameRuntimeData cNKMGameRuntimeData)
	{
		m_NKMGameRuntimeData = cNKMGameRuntimeData;
	}

	public void LoadGame()
	{
		Debug.Log("<color=#FFFF00>NKCGameClient.LoadGame()</color>");
		NKCUIManager.m_NUF_GAME_TOUCH_OBJECT.SetActive(value: true);
		SetGameData(m_NKMGameDataDummy.DeepCopy(), m_bIntrudeDummy);
		Log.Info("NKCGameClient.LoadGame SetGameData", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 666);
		LoadGameMap();
		Log.Info("NKCGameClient.LoadGame LoadGameMap", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 669);
		LoadGameUnit();
		Log.Info("NKCGameClient.LoadGame LoadGameUnit", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 672);
		LoadGameUI();
		Log.Info("NKCGameClient.LoadGame LoadGameUI", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 675);
		LoadGameEffectInst();
		Log.Info("NKCGameClient.LoadGame LoadGameEffectInst", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 678);
		LoadGameBattleCondition();
		Log.Info("NKCGameClient.LoadGame LoadGameBattleCondition", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 681);
	}

	private bool LoadGameMap()
	{
		NKMMapTemplet mapTempletByID = NKMMapManager.GetMapTempletByID(base.m_NKMGameData.m_MapID);
		if (mapTempletByID == null)
		{
			return false;
		}
		m_NKMMapTemplet = mapTempletByID;
		m_Map.Load(base.m_NKMGameData.m_MapID);
		return true;
	}

	private void LoadGameUnit()
	{
		LoadOperator(base.m_NKMGameData.m_NKMGameTeamDataA);
		LoadOperator(base.m_NKMGameData.m_NKMGameTeamDataB);
		CreatePoolUnit(bAsync: true);
		CreateDynaminRespawnPoolUnit(bAsync: true);
	}

	private void LoadOperator(NKMGameTeamData cNKMGameTeamData)
	{
		if (cNKMGameTeamData != null && cNKMGameTeamData.m_Operator != null)
		{
			string finalOperatorCutinEffectName = GetFinalOperatorCutinEffectName(cNKMGameTeamData.m_Operator.id);
			NKCASEffect item = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, finalOperatorCutinEffectName, finalOperatorCutinEffectName, bAsync: true);
			m_listEffectLoadTemp.Add(item);
		}
	}

	private string GetFinalOperatorCutinEffectName(int operatorID)
	{
		string unitStrID = NKMUnitManager.GetUnitStrID(operatorID);
		return "AB_FX_SKILL_CUTIN_" + unitStrID;
	}

	private void LoadGameEffectInst()
	{
		NKCASEffect nKCASEffect = null;
		for (int i = 0; i < 20; i++)
		{
			nKCASEffect = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, "AB_FX_DAMAGE_TEXT", "AB_FX_DAMAGE_TEXT", bAsync: true);
			m_listEffectLoadTemp.Add(nKCASEffect);
		}
		nKCASEffect = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, "AB_FX_COOLTIME", "AB_FX_COOLTIME", bAsync: true);
		m_listEffectLoadTemp.Add(nKCASEffect);
		nKCASEffect = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, "AB_FX_COST", "AB_FX_COST_NEW", bAsync: true);
		m_listEffectLoadTemp.Add(nKCASEffect);
		nKCASEffect = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, "AB_FX_EXCLAMATION_MARK", "AB_FX_EXCLAMATION_MARK", bAsync: true);
		m_listEffectLoadTemp.Add(nKCASEffect);
		nKCASEffect = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, "AB_FX_CMN_INSTANTKILL", "AB_FX_CMN_INSTANTKILL", bAsync: true);
		m_listEffectLoadTemp.Add(nKCASEffect);
		nKCASEffect = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, "AB_FX_CMN_DISPEL", "AB_FX_CMN_DISPEL", bAsync: true);
		m_listEffectLoadTemp.Add(nKCASEffect);
		foreach (NKMUnitStatusTemplet value in NKMTempletContainer<NKMUnitStatusTemplet>.Values)
		{
			if (!string.IsNullOrEmpty(value.m_StatusEffectName))
			{
				NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(value.m_StatusEffectName, value.m_StatusEffectName);
				nKCASEffect = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, nKMAssetName.m_BundleName, nKMAssetName.m_AssetName, bAsync: true);
				m_listEffectLoadTemp.Add(nKCASEffect);
			}
		}
		m_NKCSkillCutInSideLeft = new NKCSkillCutInSide();
		m_NKCSkillCutInSideLeft.Load(m_listEffectLoadTemp, "AB_FX_SKILL_CUTIN_SIDE_RENDER_BLUE_OTHER", "AB_FX_SKILL_CUTIN_SIDE_BLUE_OTHER");
		m_NKCSkillCutInSideRight = new NKCSkillCutInSide();
		m_NKCSkillCutInSideRight.Load(m_listEffectLoadTemp, "AB_FX_SKILL_CUTIN_SIDE_RENDER_BLUE", "AB_FX_SKILL_CUTIN_SIDE_BLUE");
		m_NKCSkillCutInSideRedLeft = new NKCSkillCutInSide();
		m_NKCSkillCutInSideRedLeft.Load(m_listEffectLoadTemp, "AB_FX_SKILL_CUTIN_SIDE_RENDER_RED_OTHER", "AB_FX_SKILL_CUTIN_SIDE_RED_OTHER");
		m_NKCSkillCutInSideRedRight = new NKCSkillCutInSide();
		m_NKCSkillCutInSideRedRight.Load(m_listEffectLoadTemp, "AB_FX_SKILL_CUTIN_SIDE_RENDER_RED", "AB_FX_SKILL_CUTIN_SIDE_RED");
	}

	private void LoadGameBattleCondition()
	{
		List<int> list = new List<int>();
		foreach (int key in GetGameData().m_BattleConditionIDs.Keys)
		{
			NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(key);
			if (templetByID == null || templetByID.UseContentsType != NKMBattleConditionTemplet.USE_CONTENT_TYPE.UCT_BATTLE_CONDITION)
			{
				continue;
			}
			if (templetByID.ActiveTimeLeft > 0f)
			{
				if (!string.IsNullOrEmpty(templetByID.BattleCondMapStrID))
				{
					m_listLoadAssetResourceDataTemp.Add(NKCAssetResourceManager.OpenResource<GameObject>("AB_FX_ENV", templetByID.BattleCondMapStrID, bAsync: true));
				}
			}
			else
			{
				list.Add(key);
			}
		}
		m_BattleCondition.Load(list);
		m_StartEffectUID = 0;
	}

	private void LoadGameCompleteEffectInst()
	{
		for (int i = 0; i < m_listEffectLoadTemp.Count; i++)
		{
			NKCASEffect nKCASEffect = m_listEffectLoadTemp[i];
			if (nKCASEffect != null)
			{
				NKCScenManager.GetScenManager().GetObjectPool().CloseObj(nKCASEffect);
			}
		}
		m_listEffectLoadTemp.Clear();
	}

	private bool LoadGameUI()
	{
		GetGameHud().LoadUI(base.m_NKMGameData);
		m_listLoadAssetResourceDataTemp.Add(NKCAssetResourceManager.OpenResource<GameObject>("AB_FX_UI_START_WIN_LOSE", "AB_FX_UI_START_WIN_LOSE", bAsync: true));
		m_listLoadAssetResourceDataTemp.Add(NKCAssetResourceManager.OpenResource<GameObject>("AB_FX_UI_WARNING", "AB_FX_UI_WARNING", bAsync: true));
		m_listLoadAssetResourceDataTemp.Add(NKCAssetResourceManager.OpenResource<GameObject>("AB_FX_UI_RESOURCE_DOUBLE", "AB_FX_UI_RESOURCE_DOUBLE", bAsync: true));
		NKMAssetName introName = NKCLoadingScreenManager.GetIntroName(base.m_NKMGameData);
		if (introName != null)
		{
			m_listLoadAssetResourceDataTemp.Add(NKCAssetResourceManager.OpenResource<GameObject>(introName, bAsync: true));
		}
		NKMAssetName outroName = NKCLoadingScreenManager.GetOutroName(base.m_NKMGameData);
		if (outroName != null)
		{
			m_listLoadAssetResourceDataTemp.Add(NKCAssetResourceManager.OpenResource<GameObject>(outroName, bAsync: true));
		}
		return true;
	}

	protected override NKMUnit CreateNewUnitObj()
	{
		NKMUnit nKMUnit = base.CreateNewUnitObj();
		if (nKMUnit == null && m_NKM_GAME_CLASS_TYPE == NKM_GAME_CLASS_TYPE.NGCT_GAME_CLIENT)
		{
			nKMUnit = (NKMUnit)GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCUnitClient);
		}
		return nKMUnit;
	}

	public void ClearResource()
	{
		for (int i = 0; i < m_listLoadAssetResourceData.Count; i++)
		{
			NKCAssetResourceManager.CloseResource(m_listLoadAssetResourceData[i]);
		}
		m_listLoadAssetResourceData.Clear();
	}

	public void LoadGameComplete()
	{
		ClearResource();
		for (int i = 0; i < m_listLoadAssetResourceDataTemp.Count; i++)
		{
			m_listLoadAssetResourceData.Add(m_listLoadAssetResourceDataTemp[i]);
		}
		m_listLoadAssetResourceDataTemp.Clear();
		if (NKCScenManager.GetScenManager().m_NKCMemoryCleaner != null)
		{
			NKCScenManager.GetScenManager().m_NKCMemoryCleaner.Clean(null, NKCUIManager.eUIUnloadFlag.ON_PLAY_GAME);
		}
		LoadCompleteUnit();
		LoadGameCompleteEffectInst();
		GetGameHud().LoadComplete(base.m_NKMGameData);
		m_DEManager.Init(this);
		m_Map.LoadComplete();
		m_BattleCondition.LoadComplete();
		SetCamera(bResetPosition: true);
		NKCScenManager.GetScenManager().PreloadSprite("ab_fx_tx_cmn_atlas", "CMN_GLOW_00");
		NKCScenManager.GetScenManager().PreloadTexture("ab_fx_tx_mask", "MASK_RECT_05");
		NKCScenManager.GetScenManager().PreloadTexture("ab_fx_tx_ptcs", "PTCS_TRIANGLE_00");
		NKCScenManager.GetScenManager().PreloadSprite("ab_fx_tx_sprite", "NK_CYBER_DECO_B");
	}

	private string FindMusic()
	{
		if (GetGameRuntimeData().m_lstPermanentDungeonEvent != null)
		{
			for (int num = GetGameRuntimeData().m_lstPermanentDungeonEvent.Count - 1; num >= 0; num--)
			{
				NKMGameSyncData_DungeonEvent nKMGameSyncData_DungeonEvent = GetGameRuntimeData().m_lstPermanentDungeonEvent[num];
				if (nKMGameSyncData_DungeonEvent.m_eEventActionType == NKM_EVENT_ACTION_TYPE.CHANGE_BGM || nKMGameSyncData_DungeonEvent.m_eEventActionType == NKM_EVENT_ACTION_TYPE.CHANGE_BGM_TRACK)
				{
					return nKMGameSyncData_DungeonEvent.m_strEventActionValue;
				}
			}
		}
		if (base.m_NKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PVE_SIMULATED && NKCTournamentManager.m_TournamentTemplet != null)
		{
			return NKCTournamentManager.m_TournamentTemplet.GetMusicAssetName();
		}
		if (NKMGame.IsPVP(base.m_NKMGameData.GetGameType()))
		{
			string userGuntletBGM = GetUserGuntletBGM();
			if (!string.IsNullOrEmpty(userGuntletBGM))
			{
				return userGuntletBGM;
			}
			return m_NKMMapTemplet.m_PVPMusicName;
		}
		string text = "";
		NKCCutScenTemplet nKCCutScenTemplet = null;
		if (m_NKMDungeonTemplet != null && m_NKMDungeonTemplet.m_DungeonTempletBase != null)
		{
			if (m_NKMDungeonTemplet.m_DungeonTempletBase.m_MusicName.Length >= 1)
			{
				return m_NKMDungeonTemplet.m_DungeonTempletBase.m_MusicName;
			}
			text = m_NKMDungeonTemplet.m_DungeonTempletBase.m_CutScenStrIDBefore;
			if (!string.IsNullOrEmpty(text))
			{
				nKCCutScenTemplet = NKCCutScenManager.GetCutScenTemple(text);
				if (nKCCutScenTemplet != null)
				{
					string lastMusicAssetName = nKCCutScenTemplet.GetLastMusicAssetName();
					if (!string.IsNullOrEmpty(lastMusicAssetName) && lastMusicAssetName.Length > 0)
					{
						return lastMusicAssetName;
					}
				}
			}
		}
		if (base.m_NKMGameData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_WARFARE)
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(base.m_NKMGameData.m_WarfareID);
			if (nKMWarfareTemplet != null)
			{
				if (nKMWarfareTemplet.m_WarfareBGM.Length > 0)
				{
					return nKMWarfareTemplet.m_WarfareBGM;
				}
				text = nKMWarfareTemplet.m_CutScenStrIDBefore;
				if (!string.IsNullOrEmpty(text))
				{
					nKCCutScenTemplet = NKCCutScenManager.GetCutScenTemple(text);
					if (nKCCutScenTemplet != null)
					{
						string lastMusicAssetName2 = nKCCutScenTemplet.GetLastMusicAssetName();
						if (!string.IsNullOrEmpty(lastMusicAssetName2) && lastMusicAssetName2.Length > 0)
						{
							return lastMusicAssetName2;
						}
					}
				}
			}
		}
		return m_NKMMapTemplet.m_MusicName;
	}

	private string GetUserGuntletBGM()
	{
		NKCBGMInfoTemplet nKCBGMInfoTemplet = NKCBGMInfoTemplet.Find(NKCScenManager.CurrentUserData().m_JukeboxData.GetJukeboxBgmId(NKM_BGM_TYPE.PVP_INGAME));
		if (nKCBGMInfoTemplet == null)
		{
			return "";
		}
		return nKCBGMInfoTemplet.m_BgmAssetID;
	}

	public void PlayMusic()
	{
		string text = FindMusic();
		if (text.Length >= 1 && !NKCSoundManager.IsSameMusic(text))
		{
			NKCSoundManager.FadeOutMusic();
		}
	}

	public override void StartGame(bool bIntrude)
	{
		base.StartGame(bIntrude);
		if (!CanUseAutoRespawn(NKCScenManager.GetScenManager().GetMyUserData()))
		{
			NKMGameRuntimeTeamData myRunTimeTeamData = GetMyRunTimeTeamData();
			if (myRunTimeTeamData != null)
			{
				myRunTimeTeamData.m_bAutoRespawn = false;
			}
		}
		GetGameHud().StartGame(m_NKMGameRuntimeData);
		PlayMusic();
		SetShowUI(bShowUI: true, bDev: true);
		if (m_NKMGameRuntimeData.m_bPause)
		{
			if (NKCGameEventManager.IsPauseEventPlaying())
			{
				NKCGameEventManager.ResumeEvent();
			}
			else
			{
				if (NKCGameEventManager.IsEventPlaying())
				{
					NKCGameEventManager.ResumeEvent();
				}
				if (!base.m_NKMGameData.m_bLocal && base.m_NKMGameData.IsPVE())
				{
					if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
					{
						CancelPause();
					}
					else
					{
						GetGameHud().OpenPause(CancelPause);
					}
				}
			}
		}
		if (!GetIntrude())
		{
			PlayGameIntro();
			if (GetMyTeamData() != null)
			{
				NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_FIGHT_START, GetMyTeamData().m_Operator);
			}
		}
		else
		{
			NKCLoadingScreenManager.CleanupIntroObject();
		}
		Debug.LogFormat("StartGame: {0}", base.m_NKMGameData.m_NKM_GAME_TYPE);
		if (base.m_NKMGameData.m_WarfareID > 0)
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(base.m_NKMGameData.m_WarfareID);
			if (nKMWarfareTemplet != null)
			{
				Debug.LogFormat("m_WarfareID: {0}", nKMWarfareTemplet.m_WarfareStrID);
			}
		}
		if (base.m_NKMGameData.m_DungeonID > 0)
		{
			NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(base.m_NKMGameData.m_DungeonID);
			if (dungeonTemplet != null)
			{
				Debug.LogFormat("m_DungeonID: {0}", dungeonTemplet.m_DungeonTempletBase.m_DungeonStrID);
			}
		}
		if (IsATeam(m_MyTeam))
		{
			m_Map.SetRespawnValidLandFactor(m_fRespawnValidLandTeamA, bTracking: false);
		}
		else if (IsBTeam(m_MyTeam))
		{
			m_Map.SetRespawnValidLandFactor(m_fRespawnValidLandTeamB, bTracking: false);
		}
		if (GetGameRuntimeData().m_lstPermanentDungeonEvent != null)
		{
			ReprocessPermanentDungeonEvent(GetGameRuntimeData().m_lstPermanentDungeonEvent);
		}
	}

	public override void EndGame()
	{
		NKCCamera.BattleEnd();
		base.EndGame();
		if (NKCUIManager.m_NUF_GAME_TOUCH_OBJECT != null)
		{
			NKCUIManager.m_NUF_GAME_TOUCH_OBJECT.SetActive(value: false);
		}
		if (GetGameHud() != null)
		{
			GetGameHud().EndGame();
		}
		if (m_Map != null)
		{
			m_Map.Init();
		}
		m_linklistHitEffect.Clear();
		if (m_NKMGameRuntimeData != null)
		{
			m_NKMGameRuntimeData.m_NKM_GAME_STATE = NKM_GAME_STATE.NGS_INVALID;
			m_NKMGameRuntimeData.m_bPause = false;
		}
		m_BattleCondition.Close();
		UnloadUI();
		NKCSoundManager.StopAllSound();
		if (NKCScenManager.GetScenManager().m_NKCMemoryCleaner != null)
		{
			NKCScenManager.GetScenManager().m_NKCMemoryCleaner.Clean(null, NKCUIManager.eUIUnloadFlag.ON_PLAY_GAME);
		}
		NKCGameEventManager.ClearEvent();
		m_bTutorialGameReRespawnAllowed = false;
	}

	private void SetNetworkLatencyLevel(float fLatencyLevel)
	{
		m_fLatencyLevel = fLatencyLevel;
		if (m_fLatencyLevel < 1f)
		{
			m_fLatencyLevel = 1f;
		}
		if (m_fLatencyLevel > 10f)
		{
			m_fLatencyLevel = 10f;
		}
		GetGameHud().SetNetworkLatencyLevel((int)(m_fLatencyLevel - 2f));
	}

	public void UnloadUI()
	{
		ObjectParentWait();
		ClearGameHud();
	}

	public void ObjectParentWait()
	{
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnitPool.GetEnumerator();
		while (enumerator.MoveNext())
		{
			((NKCUnitClient)enumerator.Current.Value)?.ObjectParentWait();
		}
		for (int i = 0; i < m_listNKMUnit.Count; i++)
		{
			((NKCUnitClient)m_listNKMUnit[i])?.ObjectParentWait();
		}
		GetNKCEffectManager().ObjectParentWait();
	}

	public void ObjectParentRestore()
	{
		Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnitPool.GetEnumerator();
		while (enumerator.MoveNext())
		{
			((NKCUnitClient)enumerator.Current.Value)?.ObjectParentRestore();
		}
		for (int i = 0; i < m_listNKMUnit.Count; i++)
		{
			((NKCUnitClient)m_listNKMUnit[i])?.ObjectParentRestore();
		}
		GetNKCEffectManager().ObjectParentRestore();
	}

	public void SetCamera(bool bResetPosition)
	{
		NKCCamera.InitBattle(m_NKMMapTemplet.m_fCamMinX, m_NKMMapTemplet.m_fCamMaxX, m_NKMMapTemplet.m_fCamMinY, m_NKMMapTemplet.m_fCamMaxY, m_NKMMapTemplet.m_fCamSize, m_NKMMapTemplet.m_fCamSizeMax);
		NKCCamera.EnableBloom(bEnable: false);
		if (bResetPosition)
		{
			NKCCamera.SetPos(m_NKMMapTemplet.m_fCamMinX, 0f, -1000f, bTrackingStop: true, bForce: true);
		}
		SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_NORMAL_TRACKING);
		m_fCameraNormalTackingWaitTime = 3f;
		m_fCameraStopDragTime = 0f;
		m_CameraDrag.SetNowValue(0f);
	}

	public void KeyDownF6()
	{
		List<string> list = new List<string>();
		NKMUnitManager.ReloadFromLUA();
		list.Clear();
		list.Add("LUA_DAMAGE_TEMPLET");
		list.Add("LUA_DAMAGE_TEMPLET2");
		list.Add("LUA_DAMAGE_TEMPLET3");
		list.Add("LUA_DAMAGE_TEMPLET4");
		list.Add("LUA_DAMAGE_TEMPLET5");
		list.Add("LUA_DAMAGE_TEMPLET6");
		NKMDamageManager.LoadFromLUA(new string[6] { "LUA_DAMAGE_TEMPLET_BASE", "LUA_DAMAGE_TEMPLET_BASE2", "LUA_DAMAGE_TEMPLET_BASE3", "LUA_DAMAGE_TEMPLET_BASE4", "LUA_DAMAGE_TEMPLET_BASE5", "LUA_DAMAGE_TEMPLET_BASE6" }, list);
		NKMBuffManager.LoadFromLUA();
		NKMBuffTemplet.ParseAllSkinDic();
		NKMEventConditionV2.LoadTempletMacro();
		list.Clear();
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET");
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET2");
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET3");
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET4");
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET5");
		list.Add("LUA_DAMAGE_EFFECT_TEMPLET6");
		NKMDETempletManager.LoadFromLUA(list, bReload: true);
		NKMCommonUnitEvent.LoadFromLUA("LUA_COMMON_UNIT_EVENT_HEAL", bReload: true);
		NKCLocalServerManager.GetGameServerLocal().GameEndFlush();
		NKMMapManager.LoadFromLUA("LUA_MAP_TEMPLET");
	}

	public void Update(float deltaTime)
	{
		if (m_NKMGameRuntimeData != null)
		{
			switch (m_NKMGameRuntimeData.m_NKM_GAME_SPEED_TYPE)
			{
			case NKM_GAME_SPEED_TYPE.NGST_1:
				UpdateInner(deltaTime * 1.1f);
				break;
			case NKM_GAME_SPEED_TYPE.NGST_2:
				UpdateInner(deltaTime * 1.5f);
				break;
			case NKM_GAME_SPEED_TYPE.NGST_3:
				UpdateInner(deltaTime * 1.1f);
				UpdateInner(deltaTime * 1.1f);
				break;
			case NKM_GAME_SPEED_TYPE.NGST_10:
				UpdateInner(deltaTime * 1.1f);
				UpdateInner(deltaTime * 1.1f);
				UpdateInner(deltaTime * 1.1f);
				UpdateInner(deltaTime * 1.1f);
				UpdateInner(deltaTime * 1.1f);
				UpdateInner(deltaTime * 1.1f);
				UpdateInner(deltaTime * 1.1f);
				UpdateInner(deltaTime * 1.1f);
				UpdateInner(deltaTime * 1.1f);
				UpdateInner(deltaTime * 1.1f);
				break;
			case NKM_GAME_SPEED_TYPE.NGST_05:
				UpdateInner(deltaTime * 0.6f);
				break;
			default:
				UpdateInner(deltaTime);
				break;
			}
		}
	}

	private void PlayOperatorVoiceMyTeam(VOICE_TYPE eVOICE_TYPE)
	{
		if (GetMyTeamData() != null)
		{
			NKCUIVoiceManager.PlayOperatorVoice(eVOICE_TYPE, GetMyTeamData().m_Operator);
		}
	}

	public void UpdateInner(float deltaTime)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && !gameOptionData.UseVideoTexture && m_fGCTime > 0f)
		{
			m_fGCTime -= deltaTime;
			if (m_fGCTime <= 0f)
			{
				m_fGCTime = 60f;
				GC.Collect();
			}
		}
		if (!NKCDefineManager.DEFINE_SERVICE())
		{
			if (Input.GetKeyDown(KeyCode.F2))
			{
				SetShowUI(!m_bShowUI, bDev: true);
			}
			if (GetGameData().m_bLocal)
			{
				if (Input.GetKeyDown(KeyCode.F5))
				{
					List<string> list = new List<string>();
					NKMUnitManager.ReloadFromLUA();
					list.Clear();
					list.Add("LUA_DAMAGE_TEMPLET");
					list.Add("LUA_DAMAGE_TEMPLET2");
					list.Add("LUA_DAMAGE_TEMPLET3");
					list.Add("LUA_DAMAGE_TEMPLET4");
					list.Add("LUA_DAMAGE_TEMPLET5");
					list.Add("LUA_DAMAGE_TEMPLET6");
					NKMDamageManager.LoadFromLUA(new string[6] { "LUA_DAMAGE_TEMPLET_BASE", "LUA_DAMAGE_TEMPLET_BASE2", "LUA_DAMAGE_TEMPLET_BASE3", "LUA_DAMAGE_TEMPLET_BASE4", "LUA_DAMAGE_TEMPLET_BASE5", "LUA_DAMAGE_TEMPLET_BASE6" }, list);
					NKMBuffManager.LoadFromLUA();
					NKMBuffTemplet.ParseAllSkinDic();
					NKMEventConditionV2.LoadTempletMacro();
					list.Clear();
					list.Add("LUA_DAMAGE_EFFECT_TEMPLET");
					list.Add("LUA_DAMAGE_EFFECT_TEMPLET2");
					list.Add("LUA_DAMAGE_EFFECT_TEMPLET3");
					list.Add("LUA_DAMAGE_EFFECT_TEMPLET4");
					list.Add("LUA_DAMAGE_EFFECT_TEMPLET5");
					list.Add("LUA_DAMAGE_EFFECT_TEMPLET6");
					NKMDETempletManager.LoadFromLUA(list, bReload: true);
					NKMCommonUnitEvent.LoadFromLUA("LUA_COMMON_UNIT_EVENT_HEAL", bReload: true);
					NKMMapManager.LoadFromLUA("LUA_MAP_TEMPLET");
				}
				if (Input.GetKeyDown(KeyCode.F6))
				{
					KeyDownF6();
				}
			}
		}
		if (m_NKMGameRuntimeData.m_bPause)
		{
			if ((int)m_NKMGameRuntimeData.m_NKM_GAME_STATE < 4 || NKCGameEventManager.IsPauseEventPlaying())
			{
				UpdatePause(deltaTime);
				return;
			}
			m_NKMGameRuntimeData.m_bPause = false;
		}
		m_fDeltaTime = deltaTime * m_GameSpeed.GetNowValue();
		m_GameSpeed.Update(deltaTime);
		m_NKCEffectManager.Update(m_fDeltaTime);
		switch (m_NKMGameRuntimeData.m_NKM_GAME_STATE)
		{
		case NKM_GAME_STATE.NGS_PLAY:
			m_fLocalGameTime += m_fDeltaTime;
			if (m_fDeltaTime < 1f / 60f)
			{
				m_fLastRecvTime += m_fDeltaTime;
			}
			else
			{
				m_fLastRecvTime += 1f / 60f;
			}
			if (m_fLastRecvTime > 10f)
			{
				Debug.LogError("Play State에서 10초간 패킷전달 없음, 재접속 시도");
				m_fLastRecvTime = 0f;
				NKCScenManager.GetScenManager().SetAppEnableConnectCheckTime(1f);
			}
			if (m_StartEffectUID != 0 && !m_NKCEffectManager.IsLiveEffect(m_StartEffectUID))
			{
				m_StartEffectUID = 0;
			}
			break;
		case NKM_GAME_STATE.NGS_FINISH:
			if (m_fFinishWaitTime > 0f)
			{
				m_fFinishWaitTime -= deltaTime;
				if (m_fFinishWaitTime < 0f)
				{
					GameStateChange(NKM_GAME_STATE.NGS_END, m_NKMGameRuntimeData.m_WinTeam, m_NKMGameRuntimeData.m_WaveID);
				}
			}
			if (m_fWinLoseWaitTime > 0f)
			{
				m_fWinLoseWaitTime -= deltaTime;
				if (m_fWinLoseWaitTime < 0f)
				{
					GetGameHud().SetTimeOver(bSet: false);
					if (m_NKMGameRuntimeData.m_WinTeam == NKM_TEAM_TYPE.NTT_DRAW)
					{
						m_EndEffectUID = m_NKCEffectManager.UseEffect(0, "AB_FX_UI_START_WIN_LOSE", "AB_FX_UI_START_WIN_LOSE", NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_EFFECT, 0f, 0f, 0f, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "DRAW")?.m_EffectUID ?? 0;
					}
					else if (!IsEnemy(m_NKMGameRuntimeData.m_WinTeam, m_MyTeam))
					{
						NKCASEffect nKCASEffect = m_NKCEffectManager.UseEffect(0, "AB_FX_UI_START_WIN_LOSE", "AB_FX_UI_START_WIN_LOSE", NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_EFFECT, 0f, 0f, 0f, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "WIN");
						PlayOperatorVoiceMyTeam(VOICE_TYPE.VT_SHIP_FIGHT_VICTORY);
						m_EndEffectUID = nKCASEffect?.m_EffectUID ?? 0;
					}
					else
					{
						NKCASEffect nKCASEffect2 = m_NKCEffectManager.UseEffect(0, "AB_FX_UI_START_WIN_LOSE", "AB_FX_UI_START_WIN_LOSE", NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_EFFECT, 0f, 0f, 0f, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "LOSE");
						PlayOperatorVoiceMyTeam(VOICE_TYPE.VT_SHIP_FIGHT_LOSE);
						m_EndEffectUID = nKCASEffect2?.m_EffectUID ?? 0;
					}
				}
			}
			if (m_EndEffectUID != 0 && !GetNKCEffectManager().IsLiveEffect(m_EndEffectUID))
			{
				m_EndEffectUID = 0;
				PlayGameOutro();
			}
			break;
		}
		NKM_GAME_STATE nKM_GAME_STATE = m_NKMGameRuntimeData.m_NKM_GAME_STATE;
		if (nKM_GAME_STATE - 2 <= NKM_GAME_STATE.NGS_START)
		{
			m_AbsoluteGameTime += m_fDeltaTime;
			if (m_fWorldStopTime <= 0f)
			{
				m_NKMGameRuntimeData.m_GameTime += m_fDeltaTime;
				ProcecssGameTime();
			}
			else
			{
				m_fWorldStopTime -= m_fDeltaTime;
				if (m_fWorldStopTime < 0f)
				{
					m_fWorldStopTime = 0f;
				}
			}
			ProcessStopTime();
			if (NKCReplayMgr.IsPlayingReplay())
			{
				m_fLastRecvTime = 0f;
				NKCReplayMgr.GetNKCReplaMgr()?.Update(m_NKMGameRuntimeData);
			}
			ProcessSyncData();
			ProcessTacticalCommand();
			ProcessCamera();
			ProcessMap();
			ProcessUnit();
			if (m_fWorldStopTime <= 0f)
			{
				ProcessReAttack();
			}
			m_DEManager.Update(m_fDeltaTime);
			ProcessBloomPoint();
			ProcessUI();
			ProcessDelayedBattleConditions();
		}
		NKMUnit nKMUnit = null;
		NKMUnit nKMUnit2 = null;
		short gameUnitUID = 0;
		short gameUnitUID2 = 0;
		if (IsATeam(m_MyTeam))
		{
			if (base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip != null)
			{
				gameUnitUID = base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID[0];
			}
			nKMUnit = GetUnit(gameUnitUID);
			if (base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip != null)
			{
				gameUnitUID2 = base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID[0];
			}
			nKMUnit2 = GetUnit(gameUnitUID2);
		}
		else if (IsBTeam(m_MyTeam))
		{
			if (base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip != null)
			{
				gameUnitUID2 = base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID[0];
			}
			nKMUnit2 = GetUnit(gameUnitUID2);
			if (base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip != null)
			{
				gameUnitUID = base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID[0];
			}
			nKMUnit = GetUnit(gameUnitUID);
		}
		if (nKMUnit != null)
		{
			if (GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY)
			{
				if (m_lastMyShipHpRate > 0.6f && nKMUnit.GetHPRate() <= 0.6f)
				{
					if (GetMyTeamData() != null)
					{
						NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_SHIP_ENEMY_AREA_2ND, GetMyTeamData().m_Operator);
					}
				}
				else if (m_lastMyShipHpRate > 0.3f && nKMUnit.GetHPRate() <= 0.3f && GetMyTeamData() != null)
				{
					NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_SHIP_ENEMY_AREA_3RD, GetMyTeamData().m_Operator);
				}
			}
			m_lastMyShipHpRate = nKMUnit.GetHPRate();
		}
		if (nKMUnit2 == null)
		{
			return;
		}
		if (GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY)
		{
			if (m_lastEnemyShipHpRate > 0.6f && nKMUnit2.GetHPRate() <= 0.6f)
			{
				if (GetMyTeamData() != null)
				{
					NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_SHIP_OUR_AREA_2ND, GetMyTeamData().m_Operator);
				}
			}
			else if (m_lastEnemyShipHpRate > 0.3f && nKMUnit2.GetHPRate() <= 0.3f && GetMyTeamData() != null)
			{
				NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_SHIP_OUR_AREA_3RD, GetMyTeamData().m_Operator);
			}
		}
		m_lastEnemyShipHpRate = nKMUnit2.GetHPRate();
	}

	public void UpdatePause(float deltaTime)
	{
		if (NKCGameEventManager.IsGameCameraStopRequired())
		{
			Dictionary<short, NKCUnitViewer>.Enumerator enumerator = m_dicUnitViewer.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.Update(m_fDeltaTime);
			}
			ProcessMap();
			if (NKCUIOverlayTutorialGuide.IsInstanceOpen && NKCUIOverlayTutorialGuide.Instance.IsShowingInvalidMap)
			{
				m_Map.SetRespawnValidLandAlpha(1f);
			}
		}
		else
		{
			ProcessCamera();
			ProcessMap();
		}
		GetNKCEffectManager().StopEffectAnim();
		m_fDeltaTime = deltaTime;
	}

	public void GameStateChange(NKM_GAME_STATE newState, NKM_TEAM_TYPE eWinTeam, int waveID = 0)
	{
		if ((int)newState < (int)m_NKMGameRuntimeData.m_NKM_GAME_STATE)
		{
			Debug.LogErrorFormat("GameStateChange Back : {0} to {1} (waveID {2})", m_NKMGameRuntimeData.m_NKM_GAME_STATE, newState, waveID);
		}
		Debug.LogFormat("Game State Change : {0} to {1} (waveID {2})", m_NKMGameRuntimeData.m_NKM_GAME_STATE, newState, waveID);
		m_NKMGameRuntimeData.m_WaveID = waveID;
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY && newState == NKM_GAME_STATE.NGS_PLAY)
		{
			string text = FindMusic();
			if (text.Length > 1)
			{
				NKCSoundManager.PlayMusic(text, bLoop: true);
			}
			if (GetDungeonType() == NKM_DUNGEON_TYPE.NDT_WAVE)
			{
				GetGameHud().SetCurrentWaveUI(waveID);
				m_StartEffectUID = PlayWaveAlarmUI()?.m_EffectUID ?? 0;
			}
			else if (GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE)
			{
				m_StartEffectUID = m_NKCEffectManager.UseEffect(0, "AB_FX_UI_START_WIN_LOSE", "AB_FX_UI_START_WIN_LOSE", NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_EFFECT, 0f, 0f, 0f, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "START")?.m_EffectUID ?? 0;
			}
		}
		else if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_END && newState == NKM_GAME_STATE.NGS_END)
		{
			if (NKCReplayMgr.IsRecording())
			{
				NKCScenManager.GetScenManager().GetNKCReplayMgr().StopRecording(saveData: true);
			}
			NKCScenManager.GetScenManager().Get_SCEN_GAME().EndGameWithReservedGameData();
		}
		else if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_FINISH && newState == NKM_GAME_STATE.NGS_FINISH)
		{
			if (base.m_NKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_TUTORIAL)
			{
				NKMGameRuntimeTeamData myRuntimeTeamData = m_NKMGameRuntimeData.GetMyRuntimeTeamData(m_MyTeam);
				if (myRuntimeTeamData != null && myRuntimeTeamData.m_bAutoRespawn)
				{
					Send_Packet_GAME_AUTO_RESPAWN_REQ(bAutoRespawn: false, bMsg: false);
				}
			}
		}
		else if (newState == NKM_GAME_STATE.NGS_PLAY && GetDungeonType() == NKM_DUNGEON_TYPE.NDT_WAVE)
		{
			GetGameHud().SetCurrentWaveUI(waveID);
			PlayWaveAlarmUI();
			if (waveID > 1 && GetMyTeamData() != null)
			{
				NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_WAVE_START, GetMyTeamData().m_Operator);
			}
		}
		m_NKMGameRuntimeData.m_NKM_GAME_STATE = newState;
		m_NKMGameRuntimeData.m_WinTeam = eWinTeam;
	}

	protected override void ProcessUnit()
	{
		base.ProcessUnit();
		Dictionary<short, NKCUnitViewer>.Enumerator enumerator = m_dicUnitViewer.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value.Update(m_fDeltaTime);
		}
		GetSortUnitListByZ();
	}

	protected NKCASEffect PlayWaveAlarmUI()
	{
		NKCASEffect nKCASEffect = m_NKCEffectManager.UseEffect(0, "AB_FX_UI_START_WIN_LOSE", "AB_FX_UI_START_WIN_LOSE", NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_EFFECT, 0f, 0f, 0f, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "WAVE");
		if (nKCASEffect != null && nKCASEffect.m_EffectInstant != null && nKCASEffect.m_EffectInstant.m_Instant != null)
		{
			Transform transform = nKCASEffect.m_EffectInstant.m_Instant.transform.Find("WAVE/WAVE_TEXT2");
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				if (gameObject != null)
				{
					Text component = gameObject.GetComponent<Text>();
					if (component != null)
					{
						component.text = m_NKMGameRuntimeData.m_WaveID + " / " + GetGameHud().GetWaveCount();
					}
				}
			}
		}
		return nKCASEffect;
	}

	public void ProcessSyncDataLatency()
	{
		if (m_linklistGameSyncData.Count == 0 && m_fLastRecvTime > 0.4f)
		{
			if (m_fDeltaTime < 1f / 60f)
			{
				m_fNoSyncDataTime += m_fDeltaTime;
			}
			else
			{
				m_fNoSyncDataTime += 1f / 60f;
			}
		}
		else
		{
			m_fNoSyncDataTime = 0f;
			GetGameHud().SetNetworkWeak(bOn: false);
			if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.LATENCY_OPTIMIZATION))
			{
				SetNetworkLatencyLevel(1f);
			}
		}
		if (m_NKMGameRuntimeData.m_GameTime > 5f && m_fNoSyncDataTime > 0.4f && !GetGameData().m_bLocal)
		{
			SetNetworkLatencyLevel(m_fLatencyLevel + 0.5f);
			if (m_fLatencyLevel >= 10f)
			{
				SetNetworkLatencyLevel(10f);
			}
			if (m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY)
			{
				GetGameHud().SetNetworkWeak(bOn: true);
			}
			Debug.LogWarningFormat("레이턴시 레벨 증가 m_fNoSyncDataTime: {0:F2}, m_fLatencyLevel: {1:F2}", m_fNoSyncDataTime, m_fLatencyLevel);
			m_fNoSyncDataTime = -0.4f * m_fLatencyLevel;
		}
	}

	public void ProcessSyncData()
	{
		ProcessSyncDataLatency();
		LinkedListNode<NKMGameSyncData_Base> linkedListNode = m_linklistGameSyncData.First;
		while (linkedListNode != null)
		{
			NKMGameSyncData_Base value = linkedListNode.Value;
			if (value != null && SyncGameTimer(value.m_fGameTime))
			{
				for (int i = 0; i < value.m_NKMGameSyncData_DieUnit.Count; i++)
				{
					NKMGameSyncData_DieUnit nKMGameSyncData_DieUnit = value.m_NKMGameSyncData_DieUnit[i];
					foreach (short item in nKMGameSyncData_DieUnit.m_DieGameUnitUID)
					{
						((NKCUnitClient)GetUnit(item))?.SetDie();
					}
					nKMGameSyncData_DieUnit.m_DieGameUnitUID.Clear();
				}
				for (int j = 0; j < value.m_NKMGameSyncData_Unit.Count; j++)
				{
					NKMGameSyncData_Unit nKMGameSyncData_Unit = value.m_NKMGameSyncData_Unit[j];
					if (nKMGameSyncData_Unit == null || nKMGameSyncData_Unit.m_NKMGameUnitSyncData == null)
					{
						continue;
					}
					if (IsReversePosTeam(m_MyTeam))
					{
						ReverseSyncData(nKMGameSyncData_Unit);
					}
					nKMGameSyncData_Unit.m_NKMGameUnitSyncData.Encrypt();
					NKCUnitClient nKCUnitClient = (NKCUnitClient)GetUnit(nKMGameSyncData_Unit.m_NKMGameUnitSyncData.m_GameUnitUID);
					if (!nKMGameSyncData_Unit.m_NKMGameUnitSyncData.m_bRespawnThisFrame)
					{
						if (nKCUnitClient == null && nKMGameSyncData_Unit.m_NKMGameUnitSyncData.GetHP() > 0f)
						{
							Log.Warn("Processing unit sync packet without unit spawn!!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 1884);
							nKCUnitClient = RespawnUnit(nKMGameSyncData_Unit.m_NKMGameUnitSyncData);
							if (nKCUnitClient == null)
							{
								Log.Error("Force respawn unit failed. Check unit pool.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 1889);
							}
						}
						nKCUnitClient?.OnRecv(nKMGameSyncData_Unit.m_NKMGameUnitSyncData);
					}
					else
					{
						if (nKCUnitClient != null && nKMGameSyncData_Unit.m_NKMGameUnitSyncData.m_bRespawnThisFrame)
						{
							DieBeforeRespawnSync(nKMGameSyncData_Unit.m_NKMGameUnitSyncData.m_GameUnitUID, nKCUnitClient);
						}
						RespawnUnit(nKMGameSyncData_Unit.m_NKMGameUnitSyncData);
					}
				}
				for (int k = 0; k < value.m_NKMGameSyncDataSimple_Unit.Count; k++)
				{
					NKMGameSyncDataSimple_Unit nKMGameSyncDataSimple_Unit = value.m_NKMGameSyncDataSimple_Unit[k];
					if (IsReversePosTeam(m_MyTeam))
					{
						ReverseSyncDataSimple(nKMGameSyncDataSimple_Unit);
					}
					((NKCUnitClient)GetUnit(nKMGameSyncDataSimple_Unit.m_GameUnitUID))?.OnRecv(nKMGameSyncDataSimple_Unit);
				}
				for (int l = 0; l < value.m_NKMGameSyncData_ShipSkill.Count; l++)
				{
					NKMGameSyncData_ShipSkill nKMGameSyncData_ShipSkill = value.m_NKMGameSyncData_ShipSkill[l];
					if (nKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData == null)
					{
						continue;
					}
					if (IsReversePosTeam(m_MyTeam))
					{
						ReverseSyncData(nKMGameSyncData_ShipSkill);
					}
					NKCUnitClient nKCUnitClient2 = (NKCUnitClient)GetUnit(nKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData.m_GameUnitUID);
					if (nKCUnitClient2 != null && !nKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData.m_bRespawnThisFrame)
					{
						nKCUnitClient2.OnRecv(nKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData);
					}
					else
					{
						if (nKCUnitClient2 != null && nKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData.m_bRespawnThisFrame)
						{
							DieBeforeRespawnSync(nKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData.m_GameUnitUID, nKCUnitClient2);
						}
						nKCUnitClient2 = RespawnUnit(nKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData);
					}
					NKMShipSkillTemplet shipSkillTempletByID = NKMShipSkillManager.GetShipSkillTempletByID(nKMGameSyncData_ShipSkill.m_ShipSkillID);
					if (shipSkillTempletByID != null)
					{
						nKCUnitClient2.GetUnitFrameData().m_ShipSkillTemplet = shipSkillTempletByID;
						nKCUnitClient2.GetUnitFrameData().m_fShipSkillPosX = nKMGameSyncData_ShipSkill.m_SkillPosX;
					}
					m_ShipSkillArea.SetShow(bShow: false);
					GetGameHud().ReturnDeckByShipSkillID(shipSkillTempletByID.m_ShipSkillID);
				}
				for (int m = 0; m < value.m_NKMGameSyncData_Deck.Count; m++)
				{
					NKMGameSyncData_Deck nKMGameSyncData_Deck = value.m_NKMGameSyncData_Deck[m];
					NKMGameTeamData teamData = GetGameData().GetTeamData(value.m_NKMGameSyncData_Deck[m].m_NKM_TEAM_TYPE);
					if (teamData == null || nKMGameSyncData_Deck.m_UnitDeckIndex < 0)
					{
						continue;
					}
					teamData.m_DeckData.SetListUnitDeck(nKMGameSyncData_Deck.m_UnitDeckIndex, nKMGameSyncData_Deck.m_UnitDeckUID);
					if (nKMGameSyncData_Deck.m_DeckUsedAddUnitUID != -1)
					{
						NKMUnitData unitDataByUnitUID = teamData.GetUnitDataByUnitUID(nKMGameSyncData_Deck.m_DeckUsedAddUnitUID);
						if (unitDataByUnitUID != null)
						{
							NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(unitDataByUnitUID.m_UnitID);
							if (unitTemplet != null)
							{
								if (nKMGameSyncData_Deck.m_DeckUsedAddUnitUID == teamData.m_LeaderUnitUID)
								{
									GetRespawnCost(unitTemplet.m_StatTemplet, bLeader: true, teamData.m_eNKM_TEAM_TYPE);
								}
								else
								{
									GetRespawnCost(unitTemplet.m_StatTemplet, bLeader: false, teamData.m_eNKM_TEAM_TYPE);
								}
							}
						}
						teamData.m_DeckData.AddListUnitDeckUsed(nKMGameSyncData_Deck.m_DeckUsedAddUnitUID);
						teamData.m_DeckData.DecreaseRespawnLimitCount(nKMGameSyncData_Deck.m_DeckUsedAddUnitUID);
					}
					else
					{
						teamData.m_DeckData.DecreaseRespawnLimitCount(nKMGameSyncData_Deck.m_UnitDeckUID);
					}
					if (nKMGameSyncData_Deck.m_DeckUsedRemoveIndex != -1)
					{
						teamData.m_DeckData.RemoveAtListUnitDeckUsed(nKMGameSyncData_Deck.m_DeckUsedRemoveIndex);
					}
					if (nKMGameSyncData_Deck.m_DeckTombAddUnitUID != -1)
					{
						teamData.m_DeckData.AddListUnitDeckTomb(nKMGameSyncData_Deck.m_DeckTombAddUnitUID);
						GetGameHud().UpdateRemainUnitCount(teamData.m_listUnitData.Count - teamData.m_DeckData.GetListUnitDeckTombCount());
					}
					if (nKMGameSyncData_Deck.m_AutoRespawnIndex != -1)
					{
						teamData.m_DeckData.SetAutoRespawnIndex(nKMGameSyncData_Deck.m_AutoRespawnIndex);
					}
					if (nKMGameSyncData_Deck.m_NextDeckUnitUID != -1)
					{
						teamData.m_DeckData.SetNextDeck(nKMGameSyncData_Deck.m_NextDeckUnitUID);
					}
					GetGameHud().SetSyncDeck(nKMGameSyncData_Deck);
				}
				for (int n = 0; n < value.m_NKMGameSyncData_DeckAssist.Count; n++)
				{
					NKMGameSyncData_DeckAssist nKMGameSyncData_DeckAssist = value.m_NKMGameSyncData_DeckAssist[n];
					NKMGameTeamData myTeamData = GetMyTeamData();
					if (myTeamData != null)
					{
						myTeamData.m_DeckData.SetAutoRespawnIndexAssist(nKMGameSyncData_DeckAssist.m_AutoRespawnIndexAssist);
						if (m_MyTeam == nKMGameSyncData_DeckAssist.m_NKM_TEAM_TYPE)
						{
							GetGameHud().SetAssistDeck(myTeamData);
						}
					}
				}
				for (int num = 0; num < value.m_NKMGameSyncData_GameState.Count; num++)
				{
					NKMGameSyncData_GameState nKMGameSyncData_GameState = value.m_NKMGameSyncData_GameState[num];
					if (nKMGameSyncData_GameState.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_INVALID)
					{
						GameStateChange(nKMGameSyncData_GameState.m_NKM_GAME_STATE, nKMGameSyncData_GameState.m_WinTeam, nKMGameSyncData_GameState.m_WaveID);
					}
				}
				for (int num2 = 0; num2 < value.m_NKMGameSyncData_GameEvent.Count; num2++)
				{
					NKMGameSyncData_GameEvent cNKMGameSyncData_GameEvent = value.m_NKMGameSyncData_GameEvent[num2];
					ProcessGameEventSync(cNKMGameSyncData_GameEvent);
				}
				if (GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE && m_fRemainGameTimeBeforeSync > GetGameData().m_fDoubleCostTime && value.m_fRemainGameTime <= GetGameData().m_fDoubleCostTime)
				{
					GetGameHud().EnqueueTimeWarningAlarm();
					GetGameHud().SetTimeWarningFX(bActive: true);
					if (!m_bCostSpeedVoicePlayed && GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY)
					{
						m_bCostSpeedVoicePlayed = true;
						if (GetMyTeamData() != null)
						{
							NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_COST_SPEED, GetMyTeamData().m_Operator);
						}
					}
				}
				if (HasTimeLimit() && GetDungeonType() != NKM_DUNGEON_TYPE.NDT_WAVE && m_fRemainGameTimeBeforeSync > 0f && value.m_fRemainGameTime <= 0f)
				{
					GetGameHud().SetTimeOver(bSet: true);
				}
				m_NKMGameRuntimeData.m_fRemainGameTime = value.m_fRemainGameTime;
				m_fRemainGameTimeBeforeSync = value.m_fRemainGameTime;
				switch (m_MyTeam)
				{
				case NKM_TEAM_TYPE.NTT_A1:
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_fRespawnCost = value.m_fRespawnCostA1;
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_fRespawnCostAssist = value.m_fRespawnCostAssistA1;
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_fUsedRespawnCost = value.m_fUsedRespawnCostA1;
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_fRespawnCost = value.m_fRespawnCostB1;
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_fRespawnCostAssist = value.m_fRespawnCostAssistB1;
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_NKM_GAME_AUTO_SKILL_TYPE = value.m_NKM_GAME_AUTO_SKILL_TYPE_A;
					break;
				case NKM_TEAM_TYPE.NTT_B1:
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_fRespawnCost = value.m_fRespawnCostB1;
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_fRespawnCostAssist = value.m_fRespawnCostAssistB1;
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_fUsedRespawnCost = value.m_fUsedRespawnCostB1;
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_fRespawnCost = value.m_fRespawnCostA1;
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_fRespawnCostAssist = value.m_fRespawnCostAssistA1;
					m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_NKM_GAME_AUTO_SKILL_TYPE = value.m_NKM_GAME_AUTO_SKILL_TYPE_B;
					break;
				}
				GetGameHud().SetRespawnCost();
				GetGameHud().SetRespawnCostAssist();
				m_NKMGameRuntimeData.m_fShipDamage = value.m_fShipDamage;
				m_NKMGameRuntimeData.m_NKM_GAME_SPEED_TYPE = value.m_NKM_GAME_SPEED_TYPE;
				if (NKCReplayMgr.IsPlayingReplay())
				{
					m_NKMGameRuntimeData.m_NKM_GAME_SPEED_TYPE = NKCReplayMgr.GetNKCReplaMgr().GetPlayingGameSpeedType();
				}
				for (int num3 = 0; num3 < value.m_NKMGameSyncData_DungeonEvent.Count; num3++)
				{
					ProcessDungeonEventSync(value.m_NKMGameSyncData_DungeonEvent[num3]);
				}
				if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_FIERCE && value.m_NKMGameSyncData_GamePoint != null)
				{
					GetGameHud().SetFierceBattleScore(value.m_NKMGameSyncData_GamePoint.m_fGamePoint);
					Debug.Log($"<color=green>fierce point : {value.m_NKMGameSyncData_GamePoint.m_fGamePoint}</color>");
				}
				if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE && value.m_NKMGameSyncData_GamePoint != null)
				{
					GetGameHud().SetKillCount(value.m_NKMGameSyncData_GamePoint.m_fGamePoint);
					Debug.Log($"<color=green>defence point : {value.m_NKMGameSyncData_GamePoint.m_fGamePoint}</color>");
				}
				NKCPacketObjectPool.CloseObject(value);
				LinkedListNode<NKMGameSyncData_Base> next = linkedListNode.Next;
				m_linklistGameSyncData.Remove(linkedListNode);
				linkedListNode = next;
			}
			else if (linkedListNode != null)
			{
				linkedListNode = linkedListNode.Next;
			}
		}
	}

	private void DieBeforeRespawnSync(short gameUnitUID, NKCUnitClient cNKCUnitClient)
	{
		if (!m_dicNKMUnitPool.ContainsKey(gameUnitUID))
		{
			m_dicNKMUnitPool.Add(gameUnitUID, cNKCUnitClient);
		}
		if (m_dicNKMUnit.ContainsKey(gameUnitUID))
		{
			m_dicNKMUnit.Remove(gameUnitUID);
		}
		if (m_listNKMUnit.Contains(cNKCUnitClient))
		{
			m_listNKMUnit.Remove(cNKCUnitClient);
		}
		cNKCUnitClient.SetDie();
	}

	public float GetRespawnCostClient(NKM_TEAM_TYPE teamType)
	{
		float num = 0f;
		foreach (KeyValuePair<long, float> item in m_dicRespawnCostHolder)
		{
			num += item.Value;
		}
		foreach (KeyValuePair<long, float> item2 in m_dicRespawnCostHolderTC)
		{
			num += item2.Value;
		}
		float num2 = m_NKMGameRuntimeData.GetMyRuntimeTeamData(teamType).m_fRespawnCost - num;
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		return num2;
	}

	public float GetRespawnCostAssistClient(NKM_TEAM_TYPE teamType)
	{
		float num = 0f;
		foreach (KeyValuePair<long, float> item in m_dicRespawnCostHolderAssist)
		{
			num += item.Value;
		}
		float num2 = m_NKMGameRuntimeData.GetMyRuntimeTeamData(teamType).m_fRespawnCostAssist - num;
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		return num2;
	}

	public float GetMyRespawnCostClient()
	{
		float num = 0f;
		foreach (KeyValuePair<long, float> item in m_dicRespawnCostHolder)
		{
			num += item.Value;
		}
		foreach (KeyValuePair<long, float> item2 in m_dicRespawnCostHolderTC)
		{
			num += item2.Value;
		}
		float num2 = m_NKMGameRuntimeData.GetMyRuntimeTeamData(m_MyTeam).m_fRespawnCost - num;
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		return num2;
	}

	private float GetMyRespawnCostAssistClient()
	{
		float num = 0f;
		foreach (KeyValuePair<long, float> item in m_dicRespawnCostHolderAssist)
		{
			num += item.Value;
		}
		float num2 = m_NKMGameRuntimeData.GetMyRuntimeTeamData(m_MyTeam).m_fRespawnCostAssist - num;
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		return num2;
	}

	private void ReverseSyncData(NKMGameSyncData_Unit cNKMGameSyncData_Unit)
	{
		cNKMGameSyncData_Unit.m_NKMGameUnitSyncData.m_PosX = GetMapTemplet().ReversePosX(cNKMGameSyncData_Unit.m_NKMGameUnitSyncData.m_PosX);
		cNKMGameSyncData_Unit.m_NKMGameUnitSyncData.m_bRight = !cNKMGameSyncData_Unit.m_NKMGameUnitSyncData.m_bRight;
		cNKMGameSyncData_Unit.m_NKMGameUnitSyncData.m_bDamageSpeedXNegative = !cNKMGameSyncData_Unit.m_NKMGameUnitSyncData.m_bDamageSpeedXNegative;
	}

	private void ReverseSyncDataSimple(NKMGameSyncDataSimple_Unit cNKMGameSyncDataSimple_Unit)
	{
		cNKMGameSyncDataSimple_Unit.m_bRight = !cNKMGameSyncDataSimple_Unit.m_bRight;
	}

	private void ReverseSyncData(NKMGameSyncData_ShipSkill cNKMGameSyncData_ShipSkill)
	{
		cNKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData.m_PosX = GetMapTemplet().ReversePosX(cNKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData.m_PosX);
		cNKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData.m_bRight = !cNKMGameSyncData_ShipSkill.m_NKMGameUnitSyncData.m_bRight;
		cNKMGameSyncData_ShipSkill.m_SkillPosX = GetMapTemplet().ReversePosX(cNKMGameSyncData_ShipSkill.m_SkillPosX);
	}

	public void ProcessCamera()
	{
		if (NKCGameEventManager.IsGameCameraStopRequired())
		{
			return;
		}
		if (m_NKM_GAME_CAMERA_MODE == NKM_GAME_CAMERA_MODE.NGCM_FOCUS_UNIT)
		{
			NKCUnitClient nKCUnitClient = (NKCUnitClient)GetUnit(m_CameraFocusGameUnitUID);
			if (nKCUnitClient != null && nKCUnitClient.ProcessCamera())
			{
				m_fCameraNormalTackingWaitTime = 0.1f;
			}
		}
		ProcessCameraShortcut();
		bool flag = false;
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			flag = gameOptionData.TrackCamera;
			if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_DEV || IsObserver(NKCScenManager.CurrentUserData()) || IsPause())
			{
				flag = false;
			}
		}
		if (m_NKM_GAME_CAMERA_MODE == NKM_GAME_CAMERA_MODE.NGCM_NORMAL_TRACKING && m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_FINISH && flag)
		{
			float trackingCamPosX = GetTrackingCamPosX();
			NKCCamera.TrackingPos(3f, trackingCamPosX, 0f);
			NKCCamera.TrackingZoom(0.2f, NKCCamera.GetCameraSizeOrg());
			m_fCameraNormalTackingWaitTime = 3f;
		}
		else
		{
			if (m_NKM_GAME_CAMERA_MODE == NKM_GAME_CAMERA_MODE.NGCM_STOP)
			{
				NKCCamera.SetPos(-1f, 0f);
			}
			m_fCameraNormalTackingWaitTime -= m_fDeltaTime;
			if (m_fCameraNormalTackingWaitTime <= 0f)
			{
				SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_NORMAL_TRACKING);
				NKCCamera.TrackingZoom(0.2f, NKCCamera.GetCameraSizeOrg());
				NKCCamera.TrackingPos(3f, -1f, 0f);
			}
		}
		if (m_NKM_GAME_CAMERA_MODE == NKM_GAME_CAMERA_MODE.NGCM_DRAG)
		{
			m_fCameraStopDragTime += m_fDeltaTime;
			m_CameraDrag.Update(m_fDeltaTime);
			float num = NKCCamera.GetCameraSizeNow() * NKCCamera.GetCameraAspect() * 0.5f;
			if (m_CameraDrag.GetNowValue() < m_NKMMapTemplet.m_fCamMinX + num)
			{
				m_CameraDrag.SetNowValue(m_NKMMapTemplet.m_fCamMinX + num);
			}
			if (m_CameraDrag.GetNowValue() > m_NKMMapTemplet.m_fCamMaxX - num)
			{
				m_CameraDrag.SetNowValue(m_NKMMapTemplet.m_fCamMaxX - num);
			}
			NKCCamera.SetPos(m_CameraDrag.GetNowValue(), 0f);
			NKCCamera.TrackingZoom(0.2f, NKCCamera.GetCameraSizeOrg());
			m_CameraDragTime += m_fDeltaTime;
			if (!m_CameraDrag.IsTracking() && m_fCameraStopDragTime > 0.1f)
			{
				m_CameraDrag.StopTracking();
				m_CameraDrag.SetNowValue(NKCCamera.GetPosNowX());
				m_CameraDragTime = 0f;
				m_CameraDragDist = 0f;
			}
		}
	}

	private void ProcessCameraShortcut()
	{
		if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Right))
		{
			m_CameraDrag.SetNowValue(NKCCamera.GetPosNowX() + 2500f * Time.deltaTime);
			SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_DRAG);
			m_fCameraNormalTackingWaitTime = 3f;
		}
		else if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Left))
		{
			m_CameraDrag.SetNowValue(NKCCamera.GetPosNowX() - 2500f * Time.deltaTime);
			SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_DRAG);
			m_fCameraNormalTackingWaitTime = 3f;
		}
		if (Input.mouseScrollDelta != Vector2.zero)
		{
			m_CameraDrag.SetNowValue(NKCCamera.GetPosNowX() + (Input.mouseScrollDelta.x - Input.mouseScrollDelta.y) * 2f * NKCInputManager.ScrollSensibility);
			SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_DRAG);
			m_fCameraNormalTackingWaitTime = 3f;
		}
	}

	private float GetTrackingCamPosX(bool bFront = true)
	{
		float num = GetMapTemplet().m_fCamMinX;
		for (int i = 0; i < m_listNKMUnit.Count; i++)
		{
			NKMUnit nKMUnit = m_listNKMUnit[i];
			if (nKMUnit == null || nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY || IsEnemy(m_MyTeam, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE))
			{
				continue;
			}
			if (bFront)
			{
				if (num == GetMapTemplet().m_fCamMinX || num < nKMUnit.GetUnitSyncData().m_PosX)
				{
					num = nKMUnit.GetUnitSyncData().m_PosX;
				}
			}
			else if (nKMUnit.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && (num == GetMapTemplet().m_fCamMinX || num > nKMUnit.GetUnitSyncData().m_PosX))
			{
				num = nKMUnit.GetUnitSyncData().m_PosX;
			}
		}
		return num;
	}

	public void ProcessMap()
	{
		if (ProcecssValidLand(m_MyTeam))
		{
			if (IsATeam(m_MyTeam))
			{
				int currentValidLandLevel = GetCurrentValidLandLevel(NKM_TEAM_TYPE.NTT_A1);
				float validLandFactorByBossHPLevel = GetValidLandFactorByBossHPLevel(currentValidLandLevel, IsPVP());
				m_Map.SetRespawnValidLandFactor(validLandFactorByBossHPLevel, bTracking: true);
			}
			else if (IsBTeam(m_MyTeam))
			{
				int currentValidLandLevel2 = GetCurrentValidLandLevel(NKM_TEAM_TYPE.NTT_B1);
				float validLandFactorByBossHPLevel2 = GetValidLandFactorByBossHPLevel(currentValidLandLevel2, IsPVP());
				m_Map.SetRespawnValidLandFactor(validLandFactorByBossHPLevel2, bTracking: true);
			}
		}
		if (m_bDeckDrag)
		{
			bool flag = false;
			if (m_DeckSelectUnitTemplet != null && m_DeckSelectUnitTemplet.m_UnitTempletBase.RespawnFreePos)
			{
				flag = true;
			}
			if (GetDungeonTemplet() != null && GetDungeonTemplet().m_bRespawnFreePos)
			{
				flag = true;
			}
			if (flag)
			{
				m_Map.SetRespawnValidLandAlpha(1f, 0.2f);
			}
			else
			{
				m_Map.SetRespawnValidLandAlpha(1f);
				if (IsATeam(m_MyTeam))
				{
					m_Map.SetRespawnValidLandFactor(m_fRespawnValidLandTeamA, bTracking: false);
				}
				else if (IsBTeam(m_MyTeam))
				{
					m_Map.SetRespawnValidLandFactor(m_fRespawnValidLandTeamB, bTracking: false);
				}
			}
		}
		m_Map.Update(m_fDeltaTime);
	}

	public void ProcessBloomPoint()
	{
		for (int i = 0; i < m_NKMMapTemplet.m_listBloomPoint.Count; i++)
		{
			NKMBloomPoint nKMBloomPoint = m_NKMMapTemplet.m_listBloomPoint[i];
			if (nKMBloomPoint == null)
			{
				continue;
			}
			float value = Mathf.Abs(NKCCamera.GetPosNowX() - nKMBloomPoint.m_fBloomPointX) / nKMBloomPoint.m_fBloomDistance;
			value = Mathf.Clamp(value, 0f, 1f);
			value = 1f - value;
			NKCCamera.SetBloomIntensity(NKCCamera.GetBloomIntensityOrg() + NKCCamera.GetBloomIntensityOrg() * value * nKMBloomPoint.m_fBloomAddIntensity);
			NKCCamera.SetBloomThreshHold(NKCCamera.GetBloomThreshHoldOrg() + NKCCamera.GetBloomThreshHoldOrg() * value * nKMBloomPoint.m_fBloomAddThreshHold);
			if (nKMBloomPoint.m_LensFlareName.m_AssetName.Length > 1)
			{
				NKCASLensFlare lensFlare = m_Map.GetLensFlare(i);
				if (lensFlare != null)
				{
					lensFlare.SetPos(nKMBloomPoint.m_fBloomPointX, nKMBloomPoint.m_fBloomPointY);
					lensFlare.SetLensFlareBright(lensFlare.GetLensFlareBrightOrg() * value * m_Map.GetMapBright());
				}
			}
		}
	}

	public void ProcessUI()
	{
		short gameUnitUID = -1;
		NKMUnit nKMUnit = null;
		short gameUnitUID2 = -1;
		NKMUnit nKMUnit2 = null;
		if (IsATeam(m_MyTeam))
		{
			if (base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip != null)
			{
				gameUnitUID = base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID[0];
			}
			nKMUnit = GetUnit(gameUnitUID);
			if (base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip != null)
			{
				gameUnitUID2 = base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID[0];
			}
			nKMUnit2 = GetUnit(gameUnitUID2);
		}
		else if (IsBTeam(m_MyTeam))
		{
			if (base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip != null)
			{
				gameUnitUID2 = base.m_NKMGameData.m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID[0];
			}
			nKMUnit2 = GetUnit(gameUnitUID2);
			if (base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip != null)
			{
				gameUnitUID = base.m_NKMGameData.m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID[0];
			}
			nKMUnit = GetUnit(gameUnitUID);
		}
		if (nKMUnit != null)
		{
			GetGameHud().SetMainGage(bGageA: true, nKMUnit, GetDungeonType() == NKM_DUNGEON_TYPE.NDT_WAVE || GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE);
		}
		if (GetDungeonType() == NKM_DUNGEON_TYPE.NDT_WAVE || GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE)
		{
			if (GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH && IsATeam(GetGameRuntimeData().m_WinTeam))
			{
				GetGameHud().SetMainGage(bGageA: false, 0f, 0f, 0f);
			}
			else if (GetGameHud().GetWaveCount() > 0)
			{
				float num = GetGameHud().GetWaveCount() - (m_NKMGameRuntimeData.m_WaveID - 1);
				if (num <= 0f)
				{
					num = 0f;
				}
				if (num >= (float)GetGameHud().GetWaveCount())
				{
					num = GetGameHud().GetWaveCount();
				}
				GetGameHud().SetMainGage(bGageA: false, num, GetGameHud().GetWaveCount(), 0f);
			}
		}
		else if (nKMUnit2 != null)
		{
			GetGameHud().SetMainGage(bGageA: false, nKMUnit2, GetDungeonType() == NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE);
			if (GetDungeonType() == NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE || GetDungeonType() == NKM_DUNGEON_TYPE.NDT_RAID || GetDungeonType() == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
			{
				GetGameHud().SetAttackPoint(bLeft: true, (int)nKMUnit2.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP) - (int)nKMUnit2.GetUnitSyncData().GetHP());
				GetGameHud().SetAttackPoint(bLeft: false, (int)nKMUnit2.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP));
			}
		}
		if (m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY)
		{
			if (!m_bRespawnUnit && m_fLocalGameTime > 5f)
			{
				if (!GetGameHud().IsShowDangerMsg())
				{
					GetGameHud().PlayDangerMsg(NKCUtilString.GET_STRING_DANGER_MSG_UNIT_RESPAWN);
				}
			}
			else if (m_bRespawnUnit && GetGameHud().IsShowDangerMsg())
			{
				GetGameHud().StopDangerMsg();
			}
		}
		else if (m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH && GetGameHud().IsShowDangerMsg())
		{
			GetGameHud().StopDangerMsg();
		}
		GetGameHud().UpdateHud(m_fDeltaTime);
	}

	public void FadeColor(NKCUnitClient callerUnit, float fR, float fG, float fB, float fMapColorKeepTime, float fMapColorReturnTime, bool bHideObject)
	{
		m_Map.FadeColor(fR, fG, fB, fMapColorKeepTime, fMapColorReturnTime, bHideObject);
	}

	public void SetSkillFadeUnit(NKCUnitClient callerUnit, float fTime)
	{
		List<NKMUnit> unitChain = GetUnitChain();
		for (int i = 0; i < unitChain.Count; i++)
		{
			NKCUnitClient nKCUnitClient = (NKCUnitClient)unitChain[i];
			if (nKCUnitClient != null)
			{
				if (nKCUnitClient == callerUnit)
				{
					nKCUnitClient.SetUnitSkillColorFade(0f);
				}
				else
				{
					nKCUnitClient.SetUnitSkillColorFade(fTime);
				}
			}
		}
	}

	public bool SyncGameTimer(float fGameTime)
	{
		if (m_NKMGameRuntimeData.m_GameTime == 0f && fGameTime == 0f)
		{
			return true;
		}
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.LATENCY_OPTIMIZATION))
		{
			float num = 0.4f * LatencyLevel;
			if (m_NKMGameRuntimeData.m_GameTime > fGameTime + num)
			{
				return true;
			}
		}
		else if (m_NKMGameRuntimeData.m_GameTime > fGameTime)
		{
			return true;
		}
		return false;
	}

	public void RespawnPreviewBegin(int deckIndex, Vector2 pos)
	{
		long deckUnitUID = GetGameHud().GetDeckUnitUID(deckIndex);
		NKMUnitData unitDataByUnitUID = base.m_NKMGameData.GetUnitDataByUnitUID(deckUnitUID);
		if (unitDataByUnitUID == null)
		{
			return;
		}
		for (int i = 0; i < unitDataByUnitUID.m_listGameUnitUID.Count; i++)
		{
			short gameUnitUID = unitDataByUnitUID.m_listGameUnitUID[i];
			NKCUnitClient nKCUnitClient = (NKCUnitClient)GetUnit(gameUnitUID, bChain: true, bPool: true);
			if (nKCUnitClient == null)
			{
				continue;
			}
			NKCUnitViewer unitViewer = nKCUnitClient.GetUnitViewer();
			if (unitViewer != null)
			{
				unitViewer.SetActiveSprite(bActive: true);
				unitViewer.SetActiveShadow(bActive: true);
				unitViewer.StopTimer();
				unitViewer.Play("ASTAND", bLoop: true);
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitDataByUnitUID.m_UnitID);
				int num = 0;
				num = ((deckUnitUID != GetMyTeamData().m_LeaderUnitUID) ? GetRespawnCost(unitStatTemplet, bLeader: false, GetMyTeamData().m_eNKM_TEAM_TYPE) : GetRespawnCost(unitStatTemplet, bLeader: true, GetMyTeamData().m_eNKM_TEAM_TYPE));
				bool flag = GetMyTeamData().IsAssistUnit(deckUnitUID);
				if (!flag && GetMyRespawnCostClient() < (float)num)
				{
					unitViewer.SetColor(1f, 0f, 0f, 0.5f);
				}
				else if (flag && GetMyRespawnCostAssistClient() < (float)num)
				{
					unitViewer.SetColor(1f, 0f, 0f, 0.5f);
				}
				else if (GetGameHud().GetGameClient().IsGameUnitAllInBattle(deckUnitUID) != NKM_ERROR_CODE.NEC_OK)
				{
					unitViewer.SetColor(1f, 0f, 0f, 0.5f);
				}
				else
				{
					unitViewer.SetColor(1f, 1f, 1f, 0.5f);
				}
				unitViewer.SetLayer("UNIT_C");
				NKCCamera.GetScreenPosToWorldPos(out m_Vec3Temp, pos.x, pos.y);
				float fRespawnValidLandTeamA = m_fRespawnValidLandTeamA;
				fRespawnValidLandTeamA = ((m_MyTeam != NKM_TEAM_TYPE.NTT_A1 && m_MyTeam != NKM_TEAM_TYPE.NTT_A2) ? m_fRespawnValidLandTeamB : m_fRespawnValidLandTeamA);
				bool flag2 = false;
				if (unitViewer.GetUnitTemplet().m_UnitTempletBase.RespawnFreePos)
				{
					flag2 = true;
				}
				if (GetDungeonTemplet() != null && GetDungeonTemplet().m_bRespawnFreePos)
				{
					flag2 = true;
				}
				if (flag2)
				{
					fRespawnValidLandTeamA = 0.8f;
				}
				float minOffset = (GetGameData().IsPVP() ? NKMCommonConst.PVP_SUMMON_MIN_POS : NKMCommonConst.PVE_SUMMON_MIN_POS);
				m_Vec3Temp.x = m_NKMMapTemplet.GetNearLandX(m_Vec3Temp.x, bTeamA: true, fRespawnValidLandTeamA, minOffset);
				if (m_Vec3Temp.y - Screen.safeArea.y < m_NKMMapTemplet.m_fMinZ - 70f && pos.x < (float)Screen.width * 0.5f)
				{
					unitViewer.SetActiveSprite(bActive: false);
					unitViewer.SetActiveShadow(bActive: false);
					unitViewer.StopTimer();
					m_DeckSelectUnitUID = 0L;
				}
				else
				{
					unitViewer.SetActiveSprite(bActive: true);
					unitViewer.SetActiveShadow(bActive: true);
					unitViewer.StopTimer();
					m_DeckSelectUnitUID = deckUnitUID;
					m_DeckSelectUnitTemplet = unitViewer.GetUnitTemplet();
					m_bDeckSelectPosX = m_Vec3Temp.x;
					m_Vec3Temp.y = m_NKMMapTemplet.GetNearLandZ(m_Vec3Temp.y);
				}
				unitViewer.SetPos(m_Vec3Temp.x, 0f, m_Vec3Temp.y + 100f * (float)i);
				unitViewer.SetRight(bRight: true);
				if (!m_dicUnitViewer.ContainsKey(nKCUnitClient.GetUnitDataGame().m_GameUnitUID))
				{
					m_dicUnitViewer.Add(nKCUnitClient.GetUnitDataGame().m_GameUnitUID, unitViewer);
				}
			}
			if (i == 0)
			{
				NKMUnitTemplet unitTemplet = nKCUnitClient.GetUnitTemplet();
				NKMUnitTemplet.RespawnEffectData respawnEffectData = nKCUnitClient.GetRespawnEffectData();
				if (respawnEffectData != null && respawnEffectData.HasRespawnEffectRange)
				{
					SetShowDeckDragAffect(value: true);
					m_ShipSkillArea.SetCenterUnit();
					SetDeckDragAffectScale(unitTemplet, respawnEffectData);
					SetDeckDragAffectColor(respawnEffectData);
					SetDeckDragAffectPos(m_bDeckSelectPosX, respawnEffectData, unitTemplet);
				}
				else
				{
					SetShowDeckDragAffect(value: false);
				}
				if (nKCUnitClient.HasRespawnOffset() && m_DeckSelectUnitUID != 0L)
				{
					SetShowDeckDragOffset(value: true);
					SetDeckDragOffset(m_bDeckSelectPosX, unitTemplet, respawnEffectData);
				}
				else
				{
					SetShowDeckDragOffset(value: false);
				}
			}
		}
	}

	public void RespawnPreviewDrag(int deckIndex, Vector2 pos)
	{
		long deckUnitUID = GetGameHud().GetDeckUnitUID(deckIndex);
		float fFactor = m_fRespawnValidLandTeamA;
		if (IsATeam(m_MyTeam))
		{
			fFactor = m_fRespawnValidLandTeamA;
		}
		else if (IsBTeam(m_MyTeam))
		{
			fFactor = m_fRespawnValidLandTeamB;
		}
		int num = 0;
		NKMUnitData unitDataByUnitUID = base.m_NKMGameData.GetUnitDataByUnitUID(deckUnitUID);
		if (unitDataByUnitUID == null)
		{
			return;
		}
		for (int i = 0; i < unitDataByUnitUID.m_listGameUnitUID.Count; i++)
		{
			short num2 = unitDataByUnitUID.m_listGameUnitUID[i];
			if (!m_dicUnitViewer.ContainsKey(num2))
			{
				continue;
			}
			NKCUnitViewer nKCUnitViewer = m_dicUnitViewer[num2];
			if (nKCUnitViewer != null)
			{
				NKCCamera.GetScreenPosToWorldPos(out m_Vec3Temp, pos.x, pos.y);
				bool flag = false;
				if (nKCUnitViewer.GetUnitTemplet().m_UnitTempletBase.RespawnFreePos)
				{
					flag = true;
				}
				if (GetDungeonTemplet() != null && GetDungeonTemplet().m_bRespawnFreePos)
				{
					flag = true;
				}
				if (flag)
				{
					fFactor = 0.8f;
				}
				float minOffset = (GetGameData().IsPVP() ? NKMCommonConst.PVP_SUMMON_MIN_POS : NKMCommonConst.PVE_SUMMON_MIN_POS);
				m_Vec3Temp.x = m_NKMMapTemplet.GetNearLandX(m_Vec3Temp.x, bTeamA: true, fFactor, minOffset);
				if (m_Vec3Temp.y - Screen.safeArea.y < m_NKMMapTemplet.m_fMinZ - 70f && pos.x < (float)Screen.width * 0.7f)
				{
					nKCUnitViewer.SetActiveSprite(bActive: false);
					nKCUnitViewer.SetActiveShadow(bActive: false);
					nKCUnitViewer.StopTimer();
					m_DeckSelectUnitUID = 0L;
				}
				else
				{
					nKCUnitViewer.SetActiveSprite(bActive: true);
					nKCUnitViewer.SetActiveShadow(bActive: true);
					nKCUnitViewer.StopTimer();
					m_DeckSelectUnitUID = deckUnitUID;
					m_DeckSelectUnitTemplet = nKCUnitViewer.GetUnitTemplet();
					m_bDeckSelectPosX = m_Vec3Temp.x;
					m_Vec3Temp.y = m_NKMMapTemplet.GetNearLandZ(m_Vec3Temp.y);
				}
				nKCUnitViewer.SetPos(m_Vec3Temp.x, 0f, m_Vec3Temp.y + 100f * (float)num);
				nKCUnitViewer.SetRight(bRight: true);
				NKMUnitData unitDataByUnitUID2 = base.m_NKMGameData.GetUnitDataByUnitUID(deckUnitUID);
				if (unitDataByUnitUID2 != null)
				{
					NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitDataByUnitUID2.m_UnitID);
					if (unitStatTemplet != null)
					{
						int num3 = 0;
						num3 = ((deckUnitUID != GetMyTeamData().m_LeaderUnitUID) ? GetRespawnCost(unitStatTemplet, bLeader: false, GetMyTeamData().m_eNKM_TEAM_TYPE) : GetRespawnCost(unitStatTemplet, bLeader: true, GetMyTeamData().m_eNKM_TEAM_TYPE));
						bool flag2 = GetMyTeamData().IsAssistUnit(deckUnitUID);
						if (!flag2 && GetMyRespawnCostClient() < (float)num3)
						{
							nKCUnitViewer.SetColor(1f, 0f, 0f, 0.5f);
						}
						else if (flag2 && GetMyRespawnCostAssistClient() < (float)num3)
						{
							nKCUnitViewer.SetColor(1f, 0f, 0f, 0.5f);
						}
						else if (GetGameHud().GetGameClient().IsGameUnitAllInBattle(deckUnitUID) != NKM_ERROR_CODE.NEC_OK)
						{
							nKCUnitViewer.SetColor(1f, 0f, 0f, 0.5f);
						}
						else
						{
							nKCUnitViewer.SetColor(1f, 1f, 1f, 0.5f);
						}
					}
				}
				num++;
			}
			if (i == 0)
			{
				NKCUnitClient obj = (NKCUnitClient)GetUnit(num2, bChain: true, bPool: true);
				NKMUnitTemplet unitTemplet = obj.GetUnitTemplet();
				NKMUnitTemplet.RespawnEffectData respawnEffectData = obj.GetRespawnEffectData();
				if (respawnEffectData != null && respawnEffectData.HasRespawnEffectRange && m_DeckSelectUnitUID != 0L)
				{
					SetShowDeckDragAffect(value: true);
					m_ShipSkillArea.SetCenterUnit();
					SetDeckDragAffectScale(unitTemplet, respawnEffectData);
					SetDeckDragAffectColor(respawnEffectData);
					SetDeckDragAffectPos(m_bDeckSelectPosX, respawnEffectData, unitTemplet);
				}
				else
				{
					SetShowDeckDragAffect(value: false);
				}
				if (obj.HasRespawnOffset() && m_DeckSelectUnitUID != 0L)
				{
					SetShowDeckDragOffset(value: true);
					SetDeckDragOffset(m_bDeckSelectPosX, unitTemplet, respawnEffectData);
				}
				else
				{
					SetShowDeckDragOffset(value: false);
				}
			}
		}
	}

	private void SetShowDeckDragAffect(bool value)
	{
		m_ShipSkillArea.SetShow(value);
	}

	private void SetDeckDragAffectPos(float centerPos, NKMUnitTemplet.RespawnEffectData respawnEffectData, NKMUnitTemplet unitTemplet)
	{
		float num = respawnEffectData.m_RespawnEffectRange.m_Min;
		float num2 = respawnEffectData.m_RespawnEffectRange.m_Max;
		if (respawnEffectData.m_RespawnEffectUseUnitSize)
		{
			float num3 = unitTemplet.m_UnitSizeX * 0.5f;
			if (num < 0f)
			{
				num -= num3;
			}
			if (num2 > 0f)
			{
				num2 += num3;
			}
		}
		SetDeckDragAffectPos(centerPos, num, num2, respawnEffectData.m_RespawnEffectFullMap);
	}

	private void SetDeckDragAffectPos(float centerPos, float minRange, float maxRange, bool bFullMap)
	{
		if (bFullMap)
		{
			m_ShipSkillArea.SetPos((m_NKMMapTemplet.m_fMaxX + m_NKMMapTemplet.m_fMinX) * 0.5f, m_NKMMapTemplet.m_fMinZ + Mathf.Abs(m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ) * 0.5f);
			return;
		}
		float num = (maxRange + minRange) * 0.5f;
		m_ShipSkillArea.SetPos(centerPos + num, m_NKMMapTemplet.m_fMinZ + Mathf.Abs(m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ) * 0.5f);
	}

	private void SetDeckDragAffectScale(NKMUnitTemplet unitTemplet, NKMUnitTemplet.RespawnEffectData respawnEffectData)
	{
		float num = respawnEffectData.m_RespawnEffectRange.m_Min;
		float num2 = respawnEffectData.m_RespawnEffectRange.m_Max;
		if (respawnEffectData.m_RespawnEffectUseUnitSize)
		{
			float num3 = unitTemplet.m_UnitSizeX * 0.5f;
			if (num < 0f)
			{
				num -= num3;
			}
			if (num2 > 0f)
			{
				num2 += num3;
			}
		}
		SetDeckDragAffectScale(num, num2, respawnEffectData.m_RespawnEffectFullMap);
	}

	private void SetDeckDragAffectScale(float minRange, float maxRange, bool bFullMap)
	{
		float fX = ((!bFullMap) ? Mathf.Abs(maxRange - minRange) : (m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX));
		float fY = Mathf.Max(Mathf.Abs(m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ), 100f);
		m_ShipSkillArea.SetScale(fX, fY);
	}

	private void SetDeckDragAffectColor(NKMUnitTemplet.RespawnEffectData respawnEffectData)
	{
		if (respawnEffectData == null || respawnEffectData.m_RespawnEffectColorR < 0f)
		{
			m_ShipSkillArea.SetDefaultColor();
		}
		else
		{
			m_ShipSkillArea.SetColor(respawnEffectData.m_RespawnEffectColorR, respawnEffectData.m_RespawnEffectColorG, respawnEffectData.m_RespawnEffectColorB, respawnEffectData.m_RespawnEffectColorA);
		}
	}

	private void SetDeckDragAffectColorDefault()
	{
		m_ShipSkillArea.SetDefaultColor();
	}

	private void SetShowDeckDragOffset(bool value)
	{
		NKCUtil.SetGameobjectActive(m_UnitDragOffset, value);
	}

	private void SetDeckDragOffset(float unitPos, NKMUnitTemplet unitTemplet, NKMUnitTemplet.RespawnEffectData respawnEffectData)
	{
		if (!(m_UnitDragOffset == null))
		{
			if (unitTemplet.m_fForceRespawnXpos >= 0f)
			{
				float fZ = m_NKMMapTemplet.m_fMinZ + Mathf.Abs(m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ) * 0.5f;
				float respawnPosX = GetRespawnPosX(bTeamA: true, unitTemplet.m_fForceRespawnXpos);
				m_UnitDragOffset.SetOffset(unitPos, fZ, respawnPosX);
			}
			else if (respawnEffectData != null && respawnEffectData.m_RespawnOffset != 0f)
			{
				float fZ2 = m_NKMMapTemplet.m_fMinZ + Mathf.Abs(m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ) * 0.5f;
				float targetOffsetPos = Mathf.Clamp(unitPos + respawnEffectData.m_RespawnOffset, m_NKMMapTemplet.m_fMinX, m_NKMMapTemplet.m_fMaxX);
				m_UnitDragOffset.SetOffset(unitPos, fZ2, targetOffsetPos);
			}
		}
	}

	public void RespawnPreviewShipSkillBegin(int deckIndex, Vector2 pos)
	{
		NKCUIHudShipSkillDeck shipSkillDeck = GetGameHud().GetShipSkillDeck(deckIndex);
		if (shipSkillDeck.m_NKMShipSkillTemplet != null)
		{
			float num = Mathf.Abs(shipSkillDeck.m_NKMShipSkillTemplet.m_fRange);
			float fY = Mathf.Max(Mathf.Abs(m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ), 100f);
			if (shipSkillDeck.m_NKMShipSkillTemplet.m_bFullMap)
			{
				num = m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX;
			}
			m_ShipSkillArea.SetScale(num, fY);
			m_ShipSkillArea.SetCenterShip();
			m_ShipSkillArea.SetDefaultColor();
			NKCCamera.GetScreenPosToWorldPos(out m_Vec3Temp, pos.x, pos.y);
			if (m_NKMMapTemplet.m_fMinX > m_Vec3Temp.x - num * 0.5f)
			{
				m_Vec3Temp.x = m_NKMMapTemplet.m_fMinX + num * 0.5f;
			}
			if (m_NKMMapTemplet.m_fMaxX < m_Vec3Temp.x + num * 0.5f)
			{
				m_Vec3Temp.x = m_NKMMapTemplet.m_fMaxX - num * 0.5f;
			}
			m_ShipSkillArea.SetPos(m_Vec3Temp.x, m_NKMMapTemplet.m_fMinZ + Mathf.Abs(m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ) * 0.5f);
			m_ShipSkillArea.SetShow(bShow: true);
			m_bShipSkillDrag = true;
			m_fShipSkillDragPosX = m_Vec3Temp.x;
			m_SelectShipSkillID = shipSkillDeck.m_NKMShipSkillTemplet.m_ShipSkillID;
		}
	}

	public void RespawnPreviewShipSkillDrag(int deckIndex, Vector2 pos)
	{
		NKCUIHudShipSkillDeck shipSkillDeck = GetGameHud().GetShipSkillDeck(deckIndex);
		if (shipSkillDeck.m_NKMShipSkillTemplet == null)
		{
			return;
		}
		float num = Mathf.Abs(shipSkillDeck.m_NKMShipSkillTemplet.m_fRange);
		float fY = Mathf.Max(Mathf.Abs(m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ), 100f);
		if (shipSkillDeck.m_NKMShipSkillTemplet.m_bFullMap)
		{
			num = m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX;
		}
		m_ShipSkillArea.SetScale(num, fY);
		m_ShipSkillArea.SetCenterShip();
		m_ShipSkillArea.SetDefaultColor();
		NKCCamera.GetScreenPosToWorldPos(out m_Vec3Temp, pos.x, pos.y);
		if (m_Vec3Temp.y - Screen.safeArea.y < m_NKMMapTemplet.m_fMinZ - 70f && pos.x > (float)Screen.width * 0.5f)
		{
			m_ShipSkillArea.SetShow(bShow: false);
			m_SelectShipSkillID = 0;
			return;
		}
		if (m_NKMMapTemplet.m_fMinX > m_Vec3Temp.x - num * 0.5f)
		{
			m_Vec3Temp.x = m_NKMMapTemplet.m_fMinX + num * 0.5f;
		}
		if (m_NKMMapTemplet.m_fMaxX < m_Vec3Temp.x + num * 0.5f)
		{
			m_Vec3Temp.x = m_NKMMapTemplet.m_fMaxX - num * 0.5f;
		}
		m_ShipSkillArea.SetPos(m_Vec3Temp.x, m_NKMMapTemplet.m_fMinZ + Mathf.Abs(m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ) * 0.5f);
		m_ShipSkillArea.SetShow(bShow: true);
		m_bShipSkillDrag = true;
		m_fShipSkillDragPosX = m_Vec3Temp.x;
		m_SelectShipSkillID = shipSkillDeck.m_NKMShipSkillTemplet.m_ShipSkillID;
	}

	public void UI_GAME_CAMERA_TOUCH_DOWN()
	{
		m_bCameraTouchDown = true;
	}

	public void UI_GAME_CAMERA_TOUCH_UP()
	{
		m_bCameraTouchDown = false;
	}

	public void OnUnitDeckHotkey(int unitIndex)
	{
		if (m_bCameraDrag || m_bDeckDrag || m_bShipSkillDrag)
		{
			if (m_bDeckDrag || m_bShipSkillDrag)
			{
				DeckDragCancel();
				ShipSkillDeckDragCancel();
				ClearUnitViewer();
				GetGameHud().UnSelectShipSkillDeck();
			}
			OnDragBeginDeck(m_vLastDragPos);
			OnDragDeck(m_vLastDragPos);
		}
	}

	public void OnShipSkillhotkey(int index)
	{
		if (m_bCameraDrag || m_bDeckDrag || m_bShipSkillDrag)
		{
			if (m_bDeckDrag || m_bShipSkillDrag)
			{
				DeckDragCancel();
				ShipSkillDeckDragCancel();
				ClearUnitViewer();
				GetGameHud().UnSelectUnitDeck();
			}
			OnDragBeginShipSkill(m_vLastDragPos);
			OnDragShipSkill(m_vLastDragPos);
		}
	}

	public void UI_GAME_CAMERA_DRAG_BEGIN(Vector2 pos)
	{
		m_NKCEffectManager.StopCutInEffect();
		m_vLastDragPos = pos;
		m_bCameraDrag = true;
		if (GetGameHud().GetSelectUnitDeckIndex() < 0 && GetGameHud().GetSelectShipSkillDeckIndex() < 0)
		{
			if (!NKCGameEventManager.IsGameCameraStopRequired())
			{
				SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_DRAG);
				m_fCameraNormalTackingWaitTime = 999999f;
				if (NKCCamera.GetPosNowX() < m_NKMMapTemplet.m_fCamMinX)
				{
					float num = NKCCamera.GetCameraSizeNow() * NKCCamera.GetCameraAspect() * 0.5f;
					NKCCamera.SetPos(m_NKMMapTemplet.m_fCamMinX + num, 0f);
				}
				if (NKCCamera.GetPosNowX() > m_NKMMapTemplet.m_fCamMaxX)
				{
					float num2 = NKCCamera.GetCameraSizeNow() * NKCCamera.GetCameraAspect() * 0.5f;
					NKCCamera.SetPos(m_NKMMapTemplet.m_fCamMaxX - num2, 0f);
				}
				NKCCamera.StopTrackingCamera();
				m_CameraDrag.SetNowValue(NKCCamera.GetPosNowX());
				m_CameraDragTime = 0f;
				m_CameraDragDist = 0f;
			}
		}
		else if (GetGameHud().GetSelectUnitDeckIndex() >= 0)
		{
			OnDragBeginDeck(pos);
		}
		else if (GetGameHud().GetSelectShipSkillDeckIndex() >= 0)
		{
			OnDragBeginShipSkill(pos);
		}
	}

	private void OnDragBeginDeck(Vector2 pos)
	{
		DeckDragCancel();
		ShipSkillDeckDragCancel();
		NKCCamera.StopTrackingCamera();
		SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
		m_fCameraNormalTackingWaitTime = 3f;
		ClearUnitViewer();
		long deckUnitUID = GetGameHud().GetDeckUnitUID(GetGameHud().GetSelectUnitDeckIndex());
		NKMUnitData unitDataByUnitUID = base.m_NKMGameData.GetUnitDataByUnitUID(deckUnitUID);
		if (unitDataByUnitUID != null)
		{
			GetGameHud().MoveDeck(GetGameHud().GetSelectUnitDeckIndex(), pos.x, pos.y);
			m_bDeckDrag = true;
			m_DeckSelectUnitTemplet = NKMUnitManager.GetUnitTemplet(unitDataByUnitUID.m_UnitID);
			RespawnPreviewBegin(GetGameHud().GetSelectUnitDeckIndex(), pos);
		}
	}

	private void OnDragBeginShipSkill(Vector2 pos)
	{
		DeckDragCancel();
		ShipSkillDeckDragCancel();
		NKCCamera.StopTrackingCamera();
		SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
		m_fCameraNormalTackingWaitTime = 3f;
		m_ShipSkillDeckDragIndex = GetGameHud().GetSelectShipSkillDeckIndex();
		if (GetGameHud().GetShipSkillDeck(m_ShipSkillDeckDragIndex).m_NKMShipSkillTemplet != null)
		{
			GetGameHud().MoveShipSkillDeck(m_ShipSkillDeckDragIndex, pos.x, pos.y);
			RespawnPreviewShipSkillBegin(GetGameHud().GetSelectShipSkillDeckIndex(), pos);
		}
	}

	public void UI_GAME_CAMERA_DRAG(Vector2 delta, Vector2 pos)
	{
		m_vLastDragPos = pos;
		if (m_NKM_GAME_CAMERA_MODE == NKM_GAME_CAMERA_MODE.NGCM_DRAG && GetGameHud().GetSelectUnitDeckIndex() < 0 && GetGameHud().GetSelectShipSkillDeckIndex() < 0)
		{
			if (!NKCGameEventManager.IsGameCameraStopRequired())
			{
				SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_DRAG);
				m_fCameraNormalTackingWaitTime = 99999f;
				m_fCameraStopDragTime = 0f;
				NKCCamera.SetPosRel(0f - delta.x, 0f, 0f);
				if (NKCCamera.GetPosNowX() < m_NKMMapTemplet.m_fCamMinX)
				{
					NKCCamera.SetPos(m_NKMMapTemplet.m_fCamMinX, 0f);
				}
				if (NKCCamera.GetPosNowX() > m_NKMMapTemplet.m_fCamMaxX)
				{
					NKCCamera.SetPos(m_NKMMapTemplet.m_fCamMaxX, 0f);
				}
				m_CameraDrag.StopTracking();
				m_CameraDrag.SetNowValue(NKCCamera.GetPosNowX());
				if (m_CameraDragPositive && delta.x < 0f)
				{
					m_CameraDragTime = 0f;
					m_CameraDragDist = 0f;
					m_CameraDragPositive = false;
				}
				else if (!m_CameraDragPositive && delta.x > 0f)
				{
					m_CameraDragTime = 0f;
					m_CameraDragDist = 0f;
					m_CameraDragPositive = true;
				}
				else if (Mathf.Abs(delta.x) < 1f)
				{
					m_CameraDragTime = 0f;
					m_CameraDragDist = 0f;
					m_CameraDragPositive = true;
				}
				m_CameraDragDist += delta.x;
			}
		}
		else if (GetGameHud().GetSelectUnitDeckIndex() >= 0)
		{
			OnDragDeck(pos);
		}
		else if (GetGameHud().GetSelectShipSkillDeckIndex() >= 0)
		{
			OnDragShipSkill(pos);
		}
	}

	private void OnDragDeck(Vector2 pos)
	{
		NKCCamera.StopTrackingCamera();
		SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
		m_fCameraNormalTackingWaitTime = 3f;
		GetGameHud().MoveDeck(GetGameHud().GetSelectUnitDeckIndex(), pos.x, pos.y);
		m_bDeckDrag = true;
		RespawnPreviewDrag(GetGameHud().GetSelectUnitDeckIndex(), pos);
	}

	private void OnDragShipSkill(Vector2 pos)
	{
		NKCCamera.StopTrackingCamera();
		SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
		m_fCameraNormalTackingWaitTime = 3f;
		int selectShipSkillDeckIndex = GetGameHud().GetSelectShipSkillDeckIndex();
		if (m_ShipSkillDeckDragIndex == selectShipSkillDeckIndex && GetGameHud().GetShipSkillDeck(selectShipSkillDeckIndex).m_NKMShipSkillTemplet != null)
		{
			GetGameHud().MoveShipSkillDeck(selectShipSkillDeckIndex, pos.x, pos.y);
			RespawnPreviewShipSkillDrag(GetGameHud().GetSelectShipSkillDeckIndex(), pos);
		}
	}

	public void UI_GAME_CAMERA_DRAG_END(Vector2 delta, Vector2 pos)
	{
		m_bCameraDrag = false;
		if (m_NKM_GAME_CAMERA_MODE == NKM_GAME_CAMERA_MODE.NGCM_DRAG && GetGameHud().GetSelectUnitDeckIndex() < 0 && GetGameHud().GetSelectShipSkillDeckIndex() < 0)
		{
			if (!NKCGameEventManager.IsGameCameraStopRequired())
			{
				SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_DRAG);
				m_fCameraNormalTackingWaitTime = 3f;
				if (m_CameraDragTime > 0f)
				{
					float num = m_CameraDragDist / m_CameraDragTime;
					m_CameraDrag.SetTracking(m_CameraDrag.GetNowValue() - num * 0.5f, 1f, TRACKING_DATA_TYPE.TDT_SLOWER);
				}
			}
		}
		else if (GetGameHud().GetSelectUnitDeckIndex() >= 0)
		{
			DeckDragEnd(GetGameHud().GetSelectUnitDeckIndex());
			DeckDragCancel();
			ShipSkillDeckDragCancel();
			ClearUnitViewer();
			GetGameHud().UnSelectUnitDeck();
			GetGameHud().UnSelectShipSkillDeck();
		}
		else if (GetGameHud().GetSelectShipSkillDeckIndex() >= 0)
		{
			ShipSkillDeckDragEnd(GetGameHud().GetSelectShipSkillDeckIndex());
			GetGameHud().UnSelectUnitDeck();
			GetGameHud().UnSelectShipSkillDeck();
		}
	}

	public void QuickCamMove(NKM_GAME_QUICK_MOVE eNKM_GAME_QUICK_MOVE)
	{
		if (!NKCGameEventManager.IsGameCameraStopRequired() && GetGameHud().GetSelectUnitDeckIndex() < 0 && GetGameHud().GetSelectShipSkillDeckIndex() < 0)
		{
			m_NKCEffectManager.StopCutInEffect();
			SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_DRAG);
			m_fCameraNormalTackingWaitTime = 3f;
			float num = NKCCamera.GetCameraSizeNow() * NKCCamera.GetCameraAspect() * 0.5f;
			switch (eNKM_GAME_QUICK_MOVE)
			{
			case NKM_GAME_QUICK_MOVE.NGCQM_LEFT_END:
				NKCCamera.SetPos(m_NKMMapTemplet.m_fCamMinX + num, 0f);
				break;
			case NKM_GAME_QUICK_MOVE.NGCQM_LEFT:
				NKCCamera.SetPos(GetTrackingCamPosX(bFront: false), 0f);
				break;
			case NKM_GAME_QUICK_MOVE.NGCQM_RIGHT:
				NKCCamera.SetPos(GetTrackingCamPosX(), 0f);
				break;
			case NKM_GAME_QUICK_MOVE.NGCQM_RIGHT_END:
				NKCCamera.SetPos(m_NKMMapTemplet.m_fCamMaxX - num, 0f);
				break;
			}
			NKCCamera.StopTrackingCamera();
			m_CameraDrag.SetNowValue(NKCCamera.GetPosNowX());
			m_CameraDragTime = 0f;
			m_CameraDragDist = 0f;
		}
	}

	public void UnlockTutorialReRespawn()
	{
		m_bTutorialGameReRespawnAllowed = true;
	}

	public void TutorialForceCamMove(float mapPositionRatio)
	{
		if (GetGameHud().GetSelectUnitDeckIndex() < 0 && GetGameHud().GetSelectShipSkillDeckIndex() < 0)
		{
			mapPositionRatio = Mathf.Clamp01(mapPositionRatio);
			m_NKCEffectManager.StopCutInEffect();
			SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_DRAG);
			m_fCameraNormalTackingWaitTime = 3f;
			float num = NKCCamera.GetCameraSizeNow() * NKCCamera.GetCameraAspect() * 0.5f;
			float a = m_NKMMapTemplet.m_fCamMinX + num;
			float b = m_NKMMapTemplet.m_fCamMaxX - num;
			NKCCamera.SetPos(Mathf.Lerp(a, b, mapPositionRatio), 0f);
			NKCCamera.StopTrackingCamera();
			m_CameraDrag.SetNowValue(NKCCamera.GetPosNowX());
			m_CameraDragTime = 0f;
			m_CameraDragDist = 0f;
		}
	}

	public void UI_HUD_DECK_DOWN(int index)
	{
		DeckDragCancel();
		ShipSkillDeckDragCancel();
		ClearUnitViewer();
		GetGameHud().TouchDownDeck(index);
		m_bDeckTouchDown = true;
	}

	public void UI_HUD_DECK_UP(int index)
	{
		GetGameHud().TouchUpDeck(index, bUseTouchScale: true);
		if (GetGameHud().GetSelectUnitDeckIndex() < 0)
		{
			GetGameHud().ReturnDeck(index);
			ClearUnitViewer();
		}
		m_bDeckTouchDown = false;
	}

	public void UI_HUD_DECK_DRAG_BEGIN(GameObject deckObject, Vector2 pos)
	{
		DeckDragCancel();
		ShipSkillDeckDragCancel();
		m_NKCEffectManager.StopCutInEffect();
		NKCCamera.StopTrackingCamera();
		SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
		m_fCameraNormalTackingWaitTime = 3f;
		ClearUnitViewer();
		m_DeckDragIndex = GetGameHud().GetDeckIndex(deckObject);
		if (EnableControlByGameType(NKM_ERROR_CODE.NEC_FAIL_ASYNC_PVP_MANUAL_PLAY_DISABLE))
		{
			long deckUnitUID = GetGameHud().GetDeckUnitUID(m_DeckDragIndex);
			NKMUnitData unitDataByUnitUID = base.m_NKMGameData.GetUnitDataByUnitUID(deckUnitUID);
			if (unitDataByUnitUID != null)
			{
				GetGameHud().MoveDeck(m_DeckDragIndex, pos.x, pos.y);
				m_bDeckDrag = true;
				m_DeckSelectUnitTemplet = NKMUnitManager.GetUnitTemplet(unitDataByUnitUID.m_UnitID);
			}
			RespawnPreviewBegin(m_DeckDragIndex, pos);
		}
	}

	public void UI_HUD_DECK_DRAG(GameObject deckObject, Vector2 deckPos, Vector2 charPos)
	{
		NKCCamera.StopTrackingCamera();
		SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
		m_fCameraNormalTackingWaitTime = 3f;
		int deckIndex = GetGameHud().GetDeckIndex(deckObject);
		if (m_DeckDragIndex == deckIndex && EnableControlByGameType())
		{
			GetGameHud().MoveDeck(deckIndex, deckPos.x, deckPos.y);
			m_bDeckDrag = true;
			RespawnPreviewDrag(deckIndex, charPos);
		}
	}

	public void UI_HUD_DECK_DRAG_END(GameObject deckObject, Vector2 pos)
	{
		int deckIndex = GetGameHud().GetDeckIndex(deckObject);
		if (m_DeckDragIndex == deckIndex)
		{
			DeckDragEnd(deckIndex);
			DeckDragCancel();
			ShipSkillDeckDragCancel();
			ClearUnitViewer();
			GetGameHud().UnSelectUnitDeck();
			GetGameHud().UnSelectShipSkillDeck();
		}
	}

	private void DeckDragCancel()
	{
		if (GetGameHud().GetSelectUnitDeckIndex() >= 0)
		{
			GetGameHud().ReturnDeck(GetGameHud().GetSelectUnitDeckIndex());
		}
		if (GetGameHud().GetSelectShipSkillDeckIndex() >= 0)
		{
			GetGameHud().ReturnShipSkillDeck(GetGameHud().GetSelectShipSkillDeckIndex());
		}
		if (m_DeckDragIndex >= 0)
		{
			GetGameHud().ReturnDeck(m_DeckDragIndex);
		}
		ClearUnitViewer();
		m_DeckSelectUnitUID = 0L;
		m_DeckSelectUnitTemplet = null;
		m_DeckDragIndex = -1;
		m_bDeckDrag = false;
	}

	private bool DeckDragEnd(int deckIndex)
	{
		bool result = false;
		GetGameHud().ReturnDeck(deckIndex);
		if (!EnableControlByGameType())
		{
			ClearUnitViewer();
			m_bDeckDrag = false;
			return result;
		}
		if (m_DeckSelectUnitUID > 0)
		{
			if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY)
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_RESPAWN_FAIL_STATE);
				ClearUnitViewer();
				m_bDeckDrag = false;
				return result;
			}
			if (!IsGameUnitAllDie(m_DeckSelectUnitUID) && !CanReRespawn())
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_RESPAWN_FAIL_ALREADY_SPAWN);
				ClearUnitViewer();
				m_bDeckDrag = false;
				return result;
			}
			NKMUnitData unitDataByUnitUID = GetMyTeamData().GetUnitDataByUnitUID(m_DeckSelectUnitUID);
			if (unitDataByUnitUID == null)
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_RESPAWN_FAIL_UNIT_DATA);
				ClearUnitViewer();
				m_bDeckDrag = false;
				return result;
			}
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitDataByUnitUID.m_UnitID);
			if (unitStatTemplet == null)
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_RESPAWN_FAIL_UNIT_DATA);
				ClearUnitViewer();
				m_bDeckDrag = false;
				return result;
			}
			bool flag = GetMyTeamData().IsAssistUnit(unitDataByUnitUID.m_UnitUID);
			if (flag && GetMyTeamData().m_DeckData.GetAutoRespawnIndexAssist() == -1)
			{
				ClearUnitViewer();
				m_bDeckDrag = false;
				return result;
			}
			int num = 0;
			num = ((m_DeckSelectUnitUID != GetMyTeamData().m_LeaderUnitUID || flag) ? GetRespawnCost(unitStatTemplet, bLeader: false, GetMyTeamData().m_eNKM_TEAM_TYPE) : GetRespawnCost(unitStatTemplet, bLeader: true, GetMyTeamData().m_eNKM_TEAM_TYPE));
			float num2 = 0f;
			num2 = (flag ? GetMyRespawnCostAssistClient() : GetMyRespawnCostClient());
			if (num2 < (float)num)
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_RESPAWN_FAIL_COST);
				ClearUnitViewer();
				m_bDeckDrag = false;
				PlayOperatorVoiceMyTeam(VOICE_TYPE.VT_COST_LACK);
				return result;
			}
			float fFactor = m_fRespawnValidLandTeamA;
			if (IsATeam(m_MyTeam))
			{
				fFactor = m_fRespawnValidLandTeamA;
			}
			else if (IsBTeam(m_MyTeam))
			{
				fFactor = m_fRespawnValidLandTeamB;
			}
			bool flag2 = false;
			if (m_DeckSelectUnitTemplet != null && m_DeckSelectUnitTemplet.m_UnitTempletBase.RespawnFreePos)
			{
				flag2 = true;
			}
			if (GetDungeonTemplet() != null && GetDungeonTemplet().m_bRespawnFreePos)
			{
				flag2 = true;
			}
			if (flag2)
			{
				fFactor = 0.8f;
			}
			float minOffset = (GetGameData().IsPVP() ? NKMCommonConst.PVP_SUMMON_MIN_POS : NKMCommonConst.PVE_SUMMON_MIN_POS);
			m_bDeckSelectPosX = m_NKMMapTemplet.GetNearLandX(m_bDeckSelectPosX, bTeamA: true, fFactor, minOffset);
			if (!m_NKMMapTemplet.IsValidLandX(m_bDeckSelectPosX, bTeamA: true, fFactor))
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_RESPAWN_FAIL_MAP);
				ClearUnitViewer();
				m_bDeckDrag = false;
				return result;
			}
			if (!flag)
			{
				AddCostHolder(unitDataByUnitUID.m_UnitUID, num);
				GetGameHud().SetRespawnCost();
			}
			else
			{
				AddCostHolderAssist(unitDataByUnitUID.m_UnitUID, 10f);
				GetGameHud().SetRespawnCostAssist();
			}
			NKMUnitData unitDataByUnitUID2 = base.m_NKMGameData.GetUnitDataByUnitUID(m_DeckSelectUnitUID);
			if (unitDataByUnitUID2 != null)
			{
				for (int i = 0; i < unitDataByUnitUID2.m_listGameUnitUID.Count; i++)
				{
					short key = unitDataByUnitUID2.m_listGameUnitUID[i];
					if (!m_dicUnitViewer.ContainsKey(key))
					{
						continue;
					}
					NKCUnitViewer nKCUnitViewer = m_dicUnitViewer[key];
					if (nKCUnitViewer == null)
					{
						continue;
					}
					nKCUnitViewer.SetRespawnReady(bRespawnReady: true);
					if (i == 0)
					{
						float num3 = m_fGameTimeDifference;
						if (NKMCommonConst.USE_ROLLBACK && IsGameUnitAllDie(m_DeckSelectUnitUID))
						{
							NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(unitDataByUnitUID2.m_UnitID);
							num3 -= GetRollbackTime(unitTemplet);
						}
						nKCUnitViewer.PlayTimer(num3);
					}
				}
			}
			GetGameHud().UseDeck(deckIndex, bRetreat: false);
			if (!flag)
			{
				Send_Packet_GAME_RESPAWN_REQ(m_DeckSelectUnitUID, bAssist: false, m_bDeckSelectPosX, m_NKMGameRuntimeData.m_GameTime);
			}
			else
			{
				Send_Packet_GAME_RESPAWN_REQ(-1L, bAssist: true, m_bDeckSelectPosX, m_NKMGameRuntimeData.m_GameTime);
			}
			result = true;
		}
		m_bDeckDrag = false;
		return result;
	}

	private void AddCostHolder(long unitUID, float fCost)
	{
		if (!m_dicRespawnCostHolder.ContainsKey(unitUID))
		{
			m_dicRespawnCostHolder.Add(unitUID, fCost);
			return;
		}
		float num = m_dicRespawnCostHolder[unitUID];
		m_dicRespawnCostHolder[unitUID] = num + fCost;
	}

	public void AddCostHolderTC(int TCID, float fCost)
	{
		if (!m_dicRespawnCostHolderTC.ContainsKey(TCID))
		{
			m_dicRespawnCostHolderTC.Add(TCID, fCost);
			return;
		}
		float num = m_dicRespawnCostHolderTC[TCID];
		m_dicRespawnCostHolderTC[TCID] = num + fCost;
	}

	private void RemoveCostHolder(long unitUID, float fCost)
	{
		if (m_dicRespawnCostHolder.ContainsKey(unitUID))
		{
			float num = m_dicRespawnCostHolder[unitUID] - fCost;
			if (num < 0f)
			{
				num = 0f;
			}
			m_dicRespawnCostHolder[unitUID] = num;
		}
	}

	private void RemoveCostHolderTC(int TCID)
	{
		if (m_dicRespawnCostHolderTC.ContainsKey(TCID))
		{
			m_dicRespawnCostHolderTC[TCID] = 0f;
		}
	}

	private void AddCostHolderAssist(long unitUID, float fCost)
	{
		if (!m_dicRespawnCostHolderAssist.ContainsKey(unitUID))
		{
			m_dicRespawnCostHolderAssist.Add(unitUID, fCost);
			return;
		}
		float num = m_dicRespawnCostHolderAssist[unitUID];
		m_dicRespawnCostHolderAssist[unitUID] = num + fCost;
	}

	private void RemoveCostHolderAssist(long unitUID, float fCost)
	{
		if (m_dicRespawnCostHolderAssist.ContainsKey(unitUID))
		{
			float num = m_dicRespawnCostHolderAssist[unitUID] - fCost;
			if (num < 0f)
			{
				num = 0f;
			}
			m_dicRespawnCostHolderAssist[unitUID] = num;
		}
	}

	private void ClearUnitViewer()
	{
		m_listUnitViewerRemove.Clear();
		SetShowDeckDragAffect(value: false);
		SetShowDeckDragOffset(value: false);
		Dictionary<short, NKCUnitViewer>.Enumerator enumerator = m_dicUnitViewer.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKCUnitViewer value = enumerator.Current.Value;
			if (value != null && !value.GetRespawnReady())
			{
				value.SetActiveSprite(bActive: false);
				value.SetActiveShadow(bActive: false);
				value.StopTimer();
				m_listUnitViewerRemove.Add(enumerator.Current.Key);
			}
		}
		for (int i = 0; i < m_listUnitViewerRemove.Count; i++)
		{
			m_dicUnitViewer.Remove(m_listUnitViewerRemove[i]);
		}
	}

	private void RespawnCompleteUnitViewer(long unitUID)
	{
		NKMUnitData nKMUnitData = null;
		if (unitUID == -1)
		{
			for (int i = 0; i < GetMyTeamData().m_listAssistUnitData.Count; i++)
			{
				nKMUnitData = GetMyTeamData().m_listAssistUnitData[i];
				RespawnCompleteUnitViewer(nKMUnitData);
			}
		}
		else
		{
			nKMUnitData = base.m_NKMGameData.GetUnitDataByUnitUID(unitUID);
			RespawnCompleteUnitViewer(nKMUnitData);
		}
	}

	private void RespawnCompleteUnitViewer(NKMUnitData cNKMUnitData)
	{
		if (cNKMUnitData == null)
		{
			return;
		}
		for (int i = 0; i < cNKMUnitData.m_listGameUnitUID.Count; i++)
		{
			short key = cNKMUnitData.m_listGameUnitUID[i];
			if (m_dicUnitViewer.ContainsKey(key))
			{
				NKCUnitViewer nKCUnitViewer = m_dicUnitViewer[key];
				nKCUnitViewer.SetRespawnReady(bRespawnReady: false);
				nKCUnitViewer.SetActiveSprite(bActive: false);
				nKCUnitViewer.SetActiveShadow(bActive: false);
				nKCUnitViewer.StopTimer();
				m_dicUnitViewer.Remove(key);
			}
		}
	}

	public void SetShowUI(bool bShowUI, bool bDev)
	{
		if (m_bShowUI != bShowUI)
		{
			m_bShowUI = bShowUI;
			GetGameHud().SetShowUI(m_bShowUI, bDev);
			GetGameHud().SetLeftMenu(m_NKMGameRuntimeData);
			Dictionary<short, NKMUnit>.Enumerator enumerator = m_dicNKMUnit.GetEnumerator();
			while (enumerator.MoveNext())
			{
				((NKCUnitClient)enumerator.Current.Value).SetShowUI();
			}
		}
	}

	public void UI_HUD_SHIP_SKILL_DECK_DOWN(int index, Vector2 touchPos)
	{
		if (GetGameHud().GetShipSkillDeck(index).m_NKMShipSkillTemplet != null)
		{
			GetGameHud().ShowTooltip(index, touchPos);
			GetGameHud().TouchDownShipSkillDeck(index);
			m_bShipSkillDeckTouchDown = true;
		}
	}

	public void UI_HUD_SHIP_SKILL_DECK_UP(int index)
	{
		GetGameHud().GetShipSkillDeck(index);
		GetGameHud().TouchUpShipSkillDeck(index);
		if (GetGameHud().GetSelectShipSkillDeckIndex() < 0)
		{
			GetGameHud().ReturnShipSkillDeck(index);
			ShipSkillDeckDragCancel();
		}
		m_bShipSkillDeckTouchDown = false;
	}

	public void UI_HUD_SHIP_SKILL_DECK_DRAG_BEGIN(GameObject deckObject, Vector2 pos)
	{
		DeckDragCancel();
		ShipSkillDeckDragCancel();
		m_NKCEffectManager.StopCutInEffect();
		NKCCamera.StopTrackingCamera();
		SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
		m_fCameraNormalTackingWaitTime = 3f;
		GetGameHud().CloseTooltip();
		m_ShipSkillDeckDragIndex = GetGameHud().GetShipSkillDeckIndex(deckObject);
		NKCUIHudShipSkillDeck shipSkillDeck = GetGameHud().GetShipSkillDeck(m_ShipSkillDeckDragIndex);
		if (shipSkillDeck.m_NKMShipSkillTemplet != null && shipSkillDeck.m_NKMShipSkillTemplet.m_NKM_SKILL_TYPE != NKM_SKILL_TYPE.NST_PASSIVE && EnableControlByGameType(NKM_ERROR_CODE.NEC_FAIL_ASYNC_PVP_MANUAL_PLAY_DISABLE))
		{
			GetGameHud().MoveShipSkillDeck(m_ShipSkillDeckDragIndex, pos.x, pos.y);
			RespawnPreviewShipSkillBegin(m_ShipSkillDeckDragIndex, pos);
		}
	}

	public void UI_HUD_SHIP_SKILL_DECK_DRAG(GameObject deckObject, Vector2 pos)
	{
		NKCCamera.StopTrackingCamera();
		SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
		m_fCameraNormalTackingWaitTime = 3f;
		int shipSkillDeckIndex = GetGameHud().GetShipSkillDeckIndex(deckObject);
		if (m_ShipSkillDeckDragIndex == shipSkillDeckIndex)
		{
			NKCUIHudShipSkillDeck shipSkillDeck = GetGameHud().GetShipSkillDeck(shipSkillDeckIndex);
			if (shipSkillDeck.m_NKMShipSkillTemplet != null && shipSkillDeck.m_NKMShipSkillTemplet.m_NKM_SKILL_TYPE != NKM_SKILL_TYPE.NST_PASSIVE && EnableControlByGameType())
			{
				GetGameHud().MoveShipSkillDeck(shipSkillDeckIndex, pos.x, pos.y);
				RespawnPreviewShipSkillDrag(shipSkillDeckIndex, pos);
			}
		}
	}

	public void UI_HUD_SHIP_SKILL_DECK_DRAG_END(GameObject deckObject, Vector2 pos)
	{
		int shipSkillDeckIndex = GetGameHud().GetShipSkillDeckIndex(deckObject);
		if (m_ShipSkillDeckDragIndex == shipSkillDeckIndex)
		{
			ShipSkillDeckDragEnd(shipSkillDeckIndex);
			GetGameHud().UnSelectUnitDeck();
			GetGameHud().UnSelectShipSkillDeck();
		}
	}

	private bool ShipSkillDeckDragEnd(int deckIndex)
	{
		bool result = false;
		NKCUIHudShipSkillDeck shipSkillDeck = GetGameHud().GetShipSkillDeck(deckIndex);
		if (shipSkillDeck.m_NKMShipSkillTemplet == null || shipSkillDeck.m_NKMShipSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_PASSIVE || !EnableControlByGameType())
		{
			return false;
		}
		GetGameHud().ReturnShipSkillDeck(deckIndex);
		if (m_SelectShipSkillID > 0)
		{
			if (m_NKMGameRuntimeData.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY)
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_SHIP_SKILL_FAIL_STATE);
				m_ShipSkillArea.SetShow(bShow: false);
				m_bShipSkillDrag = false;
				return false;
			}
			NKMUnit liveBossUnit = GetLiveBossUnit(m_MyTeam);
			if (liveBossUnit == null)
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_SHIP_SKILL_FAIL_DIE);
				m_ShipSkillArea.SetShow(bShow: false);
				m_bShipSkillDrag = false;
				return false;
			}
			NKMUnitState unitStateNow = liveBossUnit.GetUnitStateNow();
			if (unitStateNow == null)
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_SHIP_SKILL_FAIL_STATE);
				m_ShipSkillArea.SetShow(bShow: false);
				m_bShipSkillDrag = false;
				return false;
			}
			if (unitStateNow.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER || unitStateNow.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_SKILL)
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_SHIP_SKILL_FAIL_USE_OTHER_SKILL);
				m_ShipSkillArea.SetShow(bShow: false);
				m_bShipSkillDrag = false;
				return false;
			}
			NKMUnit nKMUnit = null;
			NKMGameTeamData myTeamData = GetMyTeamData();
			if (myTeamData != null && myTeamData.m_MainShip != null)
			{
				nKMUnit = GetUnit(myTeamData.m_MainShip.m_listGameUnitUID[0]);
			}
			if (nKMUnit == null)
			{
				m_ShipSkillArea.SetShow(bShow: false);
				m_bShipSkillDrag = false;
				return false;
			}
			if (nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY || nKMUnit.GetUnitSyncData().GetHP() <= 0f)
			{
				m_ShipSkillArea.SetShow(bShow: false);
				m_bShipSkillDrag = false;
				return false;
			}
			if (nKMUnit.GetStateCoolTime(shipSkillDeck.m_NKMShipSkillTemplet.m_UnitStateName) > 0f)
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_SHIP_SKILL_FAIL_COOLTIME);
				m_ShipSkillArea.SetShow(bShow: false);
				m_bShipSkillDrag = false;
				return false;
			}
			if (nKMUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SILENCE))
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_SHIP_SKILL_FAIL_SILENCE);
				m_ShipSkillArea.SetShow(bShow: false);
				m_bShipSkillDrag = false;
				return false;
			}
			if (nKMUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_SLEEP))
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_SHIP_SKILL_FAIL_SLEEP);
				m_ShipSkillArea.SetShow(bShow: false);
				m_bShipSkillDrag = false;
				return false;
			}
			if (nKMUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_FEAR) || nKMUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_FREEZE) || nKMUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_HOLD))
			{
				GetGameHud().SetMessage(NKCUtilString.GET_STRING_INGAME_SHIP_SKILL_FAIL_STATE);
				m_ShipSkillArea.SetShow(bShow: false);
				m_bShipSkillDrag = false;
				return false;
			}
			GetGameHud().UseShipSkillDeck(deckIndex);
			Send_Packet_SHIP_SKILL_REQ(m_SelectShipSkillID, m_fShipSkillDragPosX);
			result = true;
		}
		m_bShipSkillDrag = false;
		return result;
	}

	private void ShipSkillDeckDragCancel()
	{
		if (m_ShipSkillDeckDragIndex >= 0)
		{
			GetGameHud().ReturnShipSkillDeck(m_ShipSkillDeckDragIndex);
			m_ShipSkillArea.SetShow(bShow: false);
			m_bShipSkillDrag = false;
			m_DeckDragIndex = -1;
			m_SelectShipSkillID = 0;
		}
	}

	public SpriteRenderer GetMapInvalidLandRenderer()
	{
		return m_Map.GetInvalidLandRenderer();
	}

	public void UI_HUD_AUTO_RESPAWN_TOGGLE()
	{
		NKMGameRuntimeTeamData myRuntimeTeamData = m_NKMGameRuntimeData.GetMyRuntimeTeamData(m_MyTeam);
		if (myRuntimeTeamData != null)
		{
			Send_Packet_GAME_AUTO_RESPAWN_REQ(!myRuntimeTeamData.m_bAutoRespawn);
		}
	}

	public void UI_HUD_ACTION_CAMERA_TOGGLE()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			Send_Packet_GAME_OPTION_CHANGE_REQ(gameOptionData.ActionCamera, gameOptionData.TrackCamera, gameOptionData.ViewSkillCutIn);
		}
	}

	public void UI_HUD_TRACK_CAMERA_TOGGLE()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			Send_Packet_GAME_OPTION_CHANGE_REQ(gameOptionData.ActionCamera, !gameOptionData.TrackCamera, gameOptionData.ViewSkillCutIn);
		}
	}

	public void UI_GAME_PAUSE()
	{
		if (NKCReplayMgr.IsPlayingReplay())
		{
			bool flag = false;
			if (GetGameHud().IsOpenPause())
			{
				flag = true;
			}
			NKMPacket_GAME_PAUSE_ACK nKMPacket_GAME_PAUSE_ACK = new NKMPacket_GAME_PAUSE_ACK();
			nKMPacket_GAME_PAUSE_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
			nKMPacket_GAME_PAUSE_ACK.isPause = !m_NKMGameRuntimeData.m_bPause;
			nKMPacket_GAME_PAUSE_ACK.isPauseEvent = false;
			m_NKC_OPEN_POPUP_TYPE_AFTER_PAUSE = NKC_OPEN_POPUP_TYPE_AFTER_PAUSE.NOPTAP_GAME_OPTION_POPUP;
			OnRecv(nKMPacket_GAME_PAUSE_ACK);
			if (flag)
			{
				GetGameHud().RefreshClassGuide();
			}
		}
		else if (base.m_NKMGameData.m_bLocal || (base.m_NKMGameData.IsPVE() && !base.m_NKMGameData.IsGuildDungeon()))
		{
			if ((m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_START || m_NKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY) && !NKCGameEventManager.IsEventPlaying())
			{
				if (GetGameHud().IsOpenPause())
				{
					GetGameHud().OnClickContinueOnPause();
					return;
				}
				NKMPopUpBox.OpenWaitBox();
				bool bPause = !m_NKMGameRuntimeData.m_bPause;
				Send_Packet_GAME_PAUSE_REQ(bPause);
			}
		}
		else if (base.m_NKMGameData != null && base.m_NKMGameData.IsPVP())
		{
			if (GetGameHud().IsOpenPause())
			{
				GetGameHud().OnClickContinueOnPause();
			}
			else
			{
				GetGameHud().OpenPause(CancelPause);
			}
		}
		else
		{
			NKCUIGameOption.Instance.Open(NKC_GAME_OPTION_MENU_TYPE.NORMAL, delegate
			{
				GetGameHud().RefreshClassGuide();
			});
		}
	}

	public void UI_GAME_SKILL_NORMAL_COOL_RESET(bool bEnemy = false)
	{
		if (base.m_NKMGameData.m_bLocal)
		{
			if (!bEnemy)
			{
				Send_Packet_GAME_DEV_COOL_TIME_RESET_REQ(bSkill: true, NKM_TEAM_TYPE.NTT_A1);
			}
			else
			{
				Send_Packet_GAME_DEV_COOL_TIME_RESET_REQ(bSkill: true, NKM_TEAM_TYPE.NTT_B1);
			}
		}
	}

	public void UI_GAME_SKILL_HYPER_COOL_RESET(bool bEnemy = false)
	{
		if (base.m_NKMGameData.m_bLocal)
		{
			if (!bEnemy)
			{
				Send_Packet_GAME_DEV_COOL_TIME_RESET_REQ(bSkill: false, NKM_TEAM_TYPE.NTT_A1);
			}
			else
			{
				Send_Packet_GAME_DEV_COOL_TIME_RESET_REQ(bSkill: false, NKM_TEAM_TYPE.NTT_B1);
			}
		}
	}

	public NKCUnitClient RespawnUnit(NKMUnitSyncData cNKMUnitSyncData)
	{
		NKCUnitClient nKCUnitClient = (NKCUnitClient)GetUnit(cNKMUnitSyncData.m_GameUnitUID, bChain: true, bPool: true);
		if (nKCUnitClient != null)
		{
			if (m_dicNKMUnitPool.ContainsKey(cNKMUnitSyncData.m_GameUnitUID))
			{
				m_dicNKMUnitPool.Remove(cNKMUnitSyncData.m_GameUnitUID);
			}
			if (!m_dicNKMUnit.ContainsKey(cNKMUnitSyncData.m_GameUnitUID))
			{
				m_dicNKMUnit.Add(cNKMUnitSyncData.m_GameUnitUID, nKCUnitClient);
			}
			if (!m_listNKMUnit.Contains(nKCUnitClient))
			{
				m_listNKMUnit.Add(nKCUnitClient);
			}
			nKCUnitClient.RespawnUnit(cNKMUnitSyncData.m_PosX, cNKMUnitSyncData.m_PosZ, cNKMUnitSyncData.m_JumpYPos);
			bool flag = GetMyTeamData().IsAssistUnit(nKCUnitClient.GetUnitData().m_UnitUID);
			bool bReturnDeckActive = true;
			if (flag && GetMyTeamData().m_DeckData.GetAutoRespawnIndexAssist() < 0)
			{
				bReturnDeckActive = false;
			}
			GetGameHud().UseCompleteDeckByUnitUID(nKCUnitClient.GetUnitData().m_UnitUID, bReturnDeckActive);
			if (flag)
			{
				GetGameHud().UseCompleteDeckAssist(bReturnDeckActive);
			}
			if (m_dicUnitViewer.ContainsKey(nKCUnitClient.GetUnitDataGame().m_GameUnitUID))
			{
				m_dicUnitViewer[nKCUnitClient.GetUnitDataGame().m_GameUnitUID].SetRespawnReady(bRespawnReady: false);
				m_dicUnitViewer.Remove(nKCUnitClient.GetUnitDataGame().m_GameUnitUID);
			}
			NKCUnitClient nKCUnitClient2 = nKCUnitClient;
			nKCUnitClient2.OnRecv(cNKMUnitSyncData);
			int num = 0;
			NKMGameTeamData myTeamData = GetMyTeamData();
			num = ((myTeamData == null || nKCUnitClient.GetUnitData().m_UnitUID != myTeamData.m_LeaderUnitUID) ? GetRespawnCost(nKCUnitClient2.GetUnitTemplet().m_StatTemplet, bLeader: false, myTeamData.m_eNKM_TEAM_TYPE) : GetRespawnCost(nKCUnitClient2.GetUnitTemplet().m_StatTemplet, bLeader: true, myTeamData.m_eNKM_TEAM_TYPE));
			if (m_MyTeam == nKCUnitClient2.GetUnitDataGame().m_NKM_TEAM_TYPE_ORG)
			{
				if (!flag)
				{
					RemoveCostHolder(nKCUnitClient.GetUnitData().m_UnitUID, num);
					GetGameHud().SetRespawnCost();
				}
				else
				{
					RemoveCostHolderAssist(nKCUnitClient.GetUnitData().m_UnitUID, 10f);
					GetGameHud().SetRespawnCostAssist();
				}
				if (!m_bRespawnUnit && nKCUnitClient2.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
				{
					m_bRespawnUnit = true;
				}
			}
			if (IsEnemy(m_MyTeam, nKCUnitClient2.GetUnitDataGame().m_NKM_TEAM_TYPE_ORG))
			{
				nKCUnitClient2.MiniMapFaceWarrning();
				if (IsPVP() && nKCUnitClient2.GetUnitTemplet().m_UnitTempletBase.m_bAwaken && !nKCUnitClient2.IsSummonUnit())
				{
					GetGameHud().SetUnitIndicator(nKCUnitClient2);
				}
			}
			Debug.LogFormat("RespawnUnit: {0}", nKCUnitClient.GetUnitTemplet().m_UnitTempletBase.m_UnitStrID);
			SetSortUnitZDirty(bSortUnitZDirty: true);
			return nKCUnitClient2;
		}
		return null;
	}

	public override NKMDamageEffectManager GetDEManager()
	{
		return m_DEManager;
	}

	public override NKMDamageEffect GetDamageEffect(short DEUID)
	{
		return m_DEManager.GetDamageEffect(DEUID);
	}

	public void TrySendGamePauseEnableREQ()
	{
		if (m_NKMGameRuntimeData != null && GetGameData() != null && !m_NKMGameRuntimeData.m_bPause && GetGameData().IsPVE())
		{
			Send_Packet_GAME_PAUSE_REQ(bPause: true);
		}
	}

	public void Send_Packet_GAME_PAUSE_REQ(bool bPause, bool bPauseEvent = false, NKC_OPEN_POPUP_TYPE_AFTER_PAUSE eNKC_OPEN_POPUP_TYPE_AFTER_PAUSE = NKC_OPEN_POPUP_TYPE_AFTER_PAUSE.NOPTAP_GAME_OPTION_POPUP)
	{
		m_NKC_OPEN_POPUP_TYPE_AFTER_PAUSE = eNKC_OPEN_POPUP_TYPE_AFTER_PAUSE;
		NKMPacket_GAME_PAUSE_REQ nKMPacket_GAME_PAUSE_REQ = new NKMPacket_GAME_PAUSE_REQ();
		nKMPacket_GAME_PAUSE_REQ.isPause = bPause;
		nKMPacket_GAME_PAUSE_REQ.isPauseEvent = bPauseEvent;
		if (GetGameData().m_bLocal)
		{
			NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_PAUSE_REQ);
		}
		else
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_PAUSE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
	}

	public void Send_Packet_GAME_SPEED_2X_REQ(NKM_GAME_SPEED_TYPE eNKM_GAME_SPEED_TYPE)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BATTLE_2X))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BATTLE_2X);
			return;
		}
		NKMPacket_GAME_SPEED_2X_REQ nKMPacket_GAME_SPEED_2X_REQ = new NKMPacket_GAME_SPEED_2X_REQ();
		nKMPacket_GAME_SPEED_2X_REQ.gameSpeedType = eNKM_GAME_SPEED_TYPE;
		if (GetGameData().m_bLocal)
		{
			NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_SPEED_2X_REQ);
		}
		else
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_SPEED_2X_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
	}

	public void Send_Packet_GAME_AUTO_SKILL_CHANGE_REQ(NKM_GAME_AUTO_SKILL_TYPE eNKM_GAME_AUTO_SKILL_TYPE, bool bMsg = true)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BATTLE_AUTO_SKILL) && eNKM_GAME_AUTO_SKILL_TYPE == NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO)
		{
			if (bMsg)
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.BATTLE_AUTO_SKILL);
			}
		}
		else if (EnableControlByGameType(bMsg ? NKM_ERROR_CODE.NEC_FAIL_ASYNC_PVP_MANUAL_PLAY_DISABLE : NKM_ERROR_CODE.NEC_OK))
		{
			NKMPacket_GAME_AUTO_SKILL_CHANGE_REQ nKMPacket_GAME_AUTO_SKILL_CHANGE_REQ = new NKMPacket_GAME_AUTO_SKILL_CHANGE_REQ();
			nKMPacket_GAME_AUTO_SKILL_CHANGE_REQ.gameAutoSkillType = eNKM_GAME_AUTO_SKILL_TYPE;
			if (GetGameData().m_bLocal)
			{
				NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_AUTO_SKILL_CHANGE_REQ);
			}
			else
			{
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_AUTO_SKILL_CHANGE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
			}
		}
	}

	public void Send_Packet_GAME_USE_UNIT_SKILL_REQ(short gameUnitUID)
	{
		NKMPacket_GAME_USE_UNIT_SKILL_REQ nKMPacket_GAME_USE_UNIT_SKILL_REQ = new NKMPacket_GAME_USE_UNIT_SKILL_REQ();
		nKMPacket_GAME_USE_UNIT_SKILL_REQ.gameUnitUID = gameUnitUID;
		if (GetGameData().m_bLocal)
		{
			NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_USE_UNIT_SKILL_REQ);
		}
		else
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_USE_UNIT_SKILL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
	}

	public void Send_Packet_GAME_CHECK_DIE_UNIT_REQ()
	{
		NKMPacket_GAME_CHECK_DIE_UNIT_REQ packet = new NKMPacket_GAME_CHECK_DIE_UNIT_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
	}

	public void Send_Packet_GAME_RESPAWN_REQ(long unitUID, bool bAssist, float fX, float gameTime)
	{
		if (IsReversePosTeam(m_MyTeam))
		{
			fX = GetMapTemplet().ReversePosX(fX);
		}
		NKMPacket_GAME_RESPAWN_REQ nKMPacket_GAME_RESPAWN_REQ = new NKMPacket_GAME_RESPAWN_REQ();
		nKMPacket_GAME_RESPAWN_REQ.unitUID = unitUID;
		nKMPacket_GAME_RESPAWN_REQ.assistUnit = bAssist;
		nKMPacket_GAME_RESPAWN_REQ.respawnPosX = fX;
		nKMPacket_GAME_RESPAWN_REQ.gameTime = gameTime - 0.4f * m_fLatencyLevel;
		if (!GetGameData().m_bLocal)
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_RESPAWN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
		else
		{
			NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_RESPAWN_REQ);
		}
	}

	public void Send_Packet_SHIP_SKILL_REQ(int shipSkillID, float fX)
	{
		if (GetMyTeamData() != null && GetMyTeamData().m_MainShip != null)
		{
			if (IsReversePosTeam(m_MyTeam))
			{
				fX = GetMapTemplet().ReversePosX(fX);
			}
			NKMPacket_GAME_SHIP_SKILL_REQ nKMPacket_GAME_SHIP_SKILL_REQ = new NKMPacket_GAME_SHIP_SKILL_REQ();
			nKMPacket_GAME_SHIP_SKILL_REQ.gameUnitUID = GetMyTeamData().m_MainShip.m_listGameUnitUID[0];
			nKMPacket_GAME_SHIP_SKILL_REQ.shipSkillID = shipSkillID;
			nKMPacket_GAME_SHIP_SKILL_REQ.skillPosX = fX;
			if (!GetGameData().m_bLocal)
			{
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_SHIP_SKILL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
			}
			else
			{
				NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_SHIP_SKILL_REQ);
			}
		}
	}

	public void Send_Packet_GAME_TACTICAL_COMMAND_REQ(int TCID)
	{
		NKMPacket_GAME_TACTICAL_COMMAND_REQ nKMPacket_GAME_TACTICAL_COMMAND_REQ = new NKMPacket_GAME_TACTICAL_COMMAND_REQ();
		nKMPacket_GAME_TACTICAL_COMMAND_REQ.TCID = TCID;
		if (!GetGameData().m_bLocal)
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_TACTICAL_COMMAND_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
		else
		{
			NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_TACTICAL_COMMAND_REQ);
		}
	}

	public void Send_Packet_GAME_AUTO_RESPAWN_REQ(bool bAutoRespawn, bool bMsg = true)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BATTLE_AUTO_RESPAWN) && bAutoRespawn)
		{
			if (bMsg)
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.BATTLE_AUTO_RESPAWN);
			}
		}
		else if (!CanUseAutoRespawn(NKCScenManager.GetScenManager().GetMyUserData()) && bAutoRespawn)
		{
			if (bMsg)
			{
				NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_AUTO_CAN_NOT_USE);
			}
		}
		else if (EnableControlByGameType(bMsg ? NKM_ERROR_CODE.NEC_FAIL_ASYNC_PVP_MANUAL_PLAY_DISABLE : NKM_ERROR_CODE.NEC_OK))
		{
			NKMPacket_GAME_AUTO_RESPAWN_REQ nKMPacket_GAME_AUTO_RESPAWN_REQ = new NKMPacket_GAME_AUTO_RESPAWN_REQ();
			nKMPacket_GAME_AUTO_RESPAWN_REQ.isAutoRespawn = bAutoRespawn;
			if (!GetGameData().m_bLocal)
			{
				NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_AUTO_RESPAWN_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
			}
			else
			{
				NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_AUTO_RESPAWN_REQ);
			}
		}
	}

	public void Send_Packet_GAME_OPTION_CHANGE_REQ(ActionCameraType actionCameraType, bool bTrackCamera, bool bViewSkillCutIn)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			bool bLocal = false;
			if (GetGameData() != null)
			{
				bLocal = GetGameData().m_bLocal;
			}
			NKCPacketSender.Send_NKMPacket_GAME_OPTION_CHANGE_REQ(actionCameraType, bTrackCamera, bViewSkillCutIn, gameOptionData.PvPAutoRespawn, gameOptionData.AutSyncFriendDeck, bLocal);
		}
	}

	public void Send_Packet_GAME_DEV_COOL_TIME_RESET_REQ(bool bSkill, NKM_TEAM_TYPE eNKM_TEAM_TYPE)
	{
		if (GetGameData().m_bLocal)
		{
			NKCLocalPacketHandler.SendPacketToLocalServer(new NKMPacket_GAME_DEV_COOL_TIME_RESET_REQ
			{
				isSkill = bSkill,
				teamType = eNKM_TEAM_TYPE
			});
		}
	}

	public void Send_Packet_GAME_UNIT_RETREAT_REQ(long unitUID)
	{
		NKMPacket_GAME_UNIT_RETREAT_REQ nKMPacket_GAME_UNIT_RETREAT_REQ = new NKMPacket_GAME_UNIT_RETREAT_REQ();
		nKMPacket_GAME_UNIT_RETREAT_REQ.unitUID = unitUID;
		if (!GetGameData().m_bLocal)
		{
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_UNIT_RETREAT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
		else
		{
			NKCLocalPacketHandler.SendPacketToLocalServer(nKMPacket_GAME_UNIT_RETREAT_REQ);
		}
	}

	public void OnRecv(NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT cPacket_NPT_GAME_SYNC_DATA_PACK_NOT)
	{
		if (cPacket_NPT_GAME_SYNC_DATA_PACK_NOT != null)
		{
			OnRecvSyncDataPack(cPacket_NPT_GAME_SYNC_DATA_PACK_NOT.gameTime, cPacket_NPT_GAME_SYNC_DATA_PACK_NOT.absoluteGameTime, cPacket_NPT_GAME_SYNC_DATA_PACK_NOT.gameSyncDataPack);
		}
	}

	public void OnRecv(NKMPacket_GAME_INTRUDE_START_NOT cNKMPacket_GAME_INTRUDE_START_NOT)
	{
		foreach (short item in new List<short>(m_dicNKMUnit.Keys))
		{
			NKCUnitClient cNKCUnitClient = (NKCUnitClient)GetUnit(item);
			DieBeforeRespawnSync(item, cNKCUnitClient);
		}
		if (cNKMPacket_GAME_INTRUDE_START_NOT.gameSyncDataPack != null)
		{
			OnRecvSyncDataPack(cNKMPacket_GAME_INTRUDE_START_NOT.gameTime, cNKMPacket_GAME_INTRUDE_START_NOT.absoluteGameTime, cNKMPacket_GAME_INTRUDE_START_NOT.gameSyncDataPack);
			GetGameData().m_NKMGameTeamDataA.m_DeckData.DeepCopyFrom(cNKMPacket_GAME_INTRUDE_START_NOT.gameTeamDeckDataA);
			GetGameData().m_NKMGameTeamDataB.m_DeckData.DeepCopyFrom(cNKMPacket_GAME_INTRUDE_START_NOT.gameTeamDeckDataB);
			GetGameHud().SetDeck(GetGameData());
		}
		if (GetGameData().m_NKMGameTeamDataA.m_MainShip != null)
		{
			if (GetGameData().m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID.Count == 1)
			{
				GetUnit(GetGameData().m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID[0], bChain: true, bPool: true)?.SetStateCoolTime(cNKMPacket_GAME_INTRUDE_START_NOT.mainShipAStateCoolTimeMap);
			}
			else
			{
				Log.Error("GAME_INTRUDE_START_NOT, Team A flag ship is not one unit", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 4625);
			}
		}
		if (GetGameData().m_NKMGameTeamDataB.m_MainShip != null)
		{
			if (GetGameData().m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID.Count == 1)
			{
				GetUnit(GetGameData().m_NKMGameTeamDataB.m_MainShip.m_listGameUnitUID[0], bChain: true, bPool: true)?.SetStateCoolTime(cNKMPacket_GAME_INTRUDE_START_NOT.mainShipBStateCoolTimeMap);
			}
			else
			{
				Log.Error("GAME_INTRUDE_START_NOT, Team B flag ship is not one unit", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 4641);
			}
		}
		PlayMusic();
	}

	public void OnRecvSyncDataPack(float fGameTime, float fAbsoluteTime, NKMGameSyncDataPack cNKMGameSyncDataPack)
	{
		m_fLastRecvTime = 0f;
		if (cNKMGameSyncDataPack == null)
		{
			return;
		}
		if (fGameTime >= m_NKMGameRuntimeData.m_GameTime)
		{
			m_NKMGameRuntimeData.m_GameTime = fGameTime;
		}
		float num = 0f;
		m_fLatestSyncPacketGameTime = 0f;
		for (int i = 0; i < cNKMGameSyncDataPack.m_listGameSyncData.Count; i++)
		{
			NKMGameSyncData_Base nKMGameSyncData_Base = cNKMGameSyncDataPack.m_listGameSyncData[i];
			if (nKMGameSyncData_Base != null)
			{
				if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.LATENCY_OPTIMIZATION))
				{
					nKMGameSyncData_Base.m_fGameTime += 0.4f * m_fLatencyLevel;
					nKMGameSyncData_Base.m_fAbsoluteGameTime += 0.4f * m_fLatencyLevel;
				}
				num = Mathf.Max(num, nKMGameSyncData_Base.m_fGameTime);
				m_linklistGameSyncData.AddLast(nKMGameSyncData_Base);
			}
		}
		m_fGameTimeDifference = num - m_NKMGameRuntimeData.m_GameTime;
		cNKMGameSyncDataPack.m_listGameSyncData.Clear();
	}

	private void OpenReservedPopupAfterPause()
	{
		if (m_NKC_OPEN_POPUP_TYPE_AFTER_PAUSE == NKC_OPEN_POPUP_TYPE_AFTER_PAUSE.NOPTATP_REPEAT_OPERATION_POPUP)
		{
			NKCPopupRepeatOperation.Instance.Open(delegate
			{
				CancelPause();
			});
		}
		else if (GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_DEV)
		{
			GetGameHud().OpenPause(CancelPause);
		}
	}

	public void OnRecv(NKMPacket_GAME_PAUSE_ACK cNKMPacket_GAME_PAUSE_ACK)
	{
		NKMPopUpBox.CloseWaitBox();
		if (cNKMPacket_GAME_PAUSE_ACK.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			if (GetGameHud() != null && GetGameData() != null)
			{
				GetGameHud().TogglePause(cNKMPacket_GAME_PAUSE_ACK.isPause, GetGameData().m_bLocal);
			}
			m_NKMGameRuntimeData.m_bPause = cNKMPacket_GAME_PAUSE_ACK.isPause;
			if (!cNKMPacket_GAME_PAUSE_ACK.isPauseEvent)
			{
				if (m_NKMGameRuntimeData.m_bPause)
				{
					OpenReservedPopupAfterPause();
				}
				else
				{
					GetGameHud().ClosePause();
				}
				string text = FindMusic();
				if (text.Length > 1)
				{
					NKCSoundManager.PlayMusic(text, bLoop: true);
				}
			}
		}
		else
		{
			if (cNKMPacket_GAME_PAUSE_ACK.errorCode != NKM_ERROR_CODE.NEC_FAIL_GAME_NOT_IN_PLAY)
			{
				return;
			}
			if (NKCGameEventManager.IsPauseEventPlaying())
			{
				Debug.LogWarning("Game not in play. will try pause!");
				if (GetGameHud() != null && GetGameData() != null)
				{
					GetGameHud().TogglePause(bSet: true, GetGameData().m_bLocal);
				}
				m_NKMGameRuntimeData.m_bPause = true;
				return;
			}
			Debug.LogWarning("Game not in play. will try unpause!");
			if (GetGameHud() != null && GetGameData() != null)
			{
				GetGameHud().TogglePause(bSet: false, GetGameData().m_bLocal);
			}
			m_NKMGameRuntimeData.m_bPause = false;
			GetGameHud().ClosePause();
		}
	}

	public void OnRecv(NKMPacket_GAME_SPEED_2X_ACK cNKMPacket_GAME_SPEED_2X_ACK)
	{
		if (cNKMPacket_GAME_SPEED_2X_ACK.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			GetGameHud().ChangeGameSpeedTypeUI(cNKMPacket_GAME_SPEED_2X_ACK.gameSpeedType);
		}
	}

	public void OnRecv(NKMPacket_GAME_AUTO_SKILL_CHANGE_ACK cNKMPacket_GAME_AUTO_SKILL_CHANGE_ACK)
	{
		if (cNKMPacket_GAME_AUTO_SKILL_CHANGE_ACK.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			GetGameHud().ChangeGameAutoSkillTypeUI(cNKMPacket_GAME_AUTO_SKILL_CHANGE_ACK.gameAutoSkillType);
		}
	}

	public void OnRecv(NKMPacket_GAME_USE_UNIT_SKILL_ACK cNKMPacket_GAME_USE_UNIT_SKILL_ACK)
	{
		((NKCUnitClient)GetUnit(cNKMPacket_GAME_USE_UNIT_SKILL_ACK.gameUnitUID))?.OnRecv(cNKMPacket_GAME_USE_UNIT_SKILL_ACK);
	}

	public bool IsPause()
	{
		if (m_NKMGameRuntimeData == null)
		{
			return false;
		}
		return m_NKMGameRuntimeData.m_bPause;
	}

	private void CancelPause()
	{
		if (NKCReplayMgr.IsPlayingReplay())
		{
			UI_GAME_PAUSE();
			return;
		}
		Send_Packet_GAME_PAUSE_REQ(bPause: false);
		GetGameHud().RefreshClassGuide();
	}

	public int GetRemainSupplyOfTeamA()
	{
		return GetGameData()?.m_TeamASupply ?? 0;
	}

	public void OnRecv(NKMPacket_GAME_RESPAWN_ACK cPacket_GAME_RESPAWN_ACK)
	{
		if (cPacket_GAME_RESPAWN_ACK.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			return;
		}
		NKM_ERROR_CODE errorCode = cPacket_GAME_RESPAWN_ACK.errorCode;
		if ((uint)(errorCode - 74) <= 4u || (uint)(errorCode - 80) <= 3u)
		{
			GetGameHud().SetMessage(NKCStringTable.GetString(cPacket_GAME_RESPAWN_ACK.errorCode));
		}
		bool flag = cPacket_GAME_RESPAWN_ACK.assistUnit;
		if (flag && cPacket_GAME_RESPAWN_ACK.unitUID == -1)
		{
			NKMGameTeamData myTeamData = GetMyTeamData();
			NKMUnitData assistUnitDataByIndex = myTeamData.GetAssistUnitDataByIndex(myTeamData.m_DeckData.GetAutoRespawnIndexAssist());
			if (assistUnitDataByIndex != null)
			{
				cPacket_GAME_RESPAWN_ACK.unitUID = assistUnitDataByIndex.m_UnitUID;
			}
		}
		NKMGameTeamData myTeamData2 = GetMyTeamData();
		if (myTeamData2 != null)
		{
			NKMUnitData unitDataByUnitUID = myTeamData2.GetUnitDataByUnitUID(cPacket_GAME_RESPAWN_ACK.unitUID);
			if (unitDataByUnitUID != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitDataByUnitUID.m_UnitID);
				if (unitStatTemplet != null)
				{
					flag = GetMyTeamData().IsAssistUnit(unitDataByUnitUID.m_UnitUID);
					int num = 0;
					num = ((unitDataByUnitUID.m_UnitUID != GetMyTeamData().m_LeaderUnitUID || flag) ? GetRespawnCost(unitStatTemplet, bLeader: false, GetMyTeamData().m_eNKM_TEAM_TYPE) : GetRespawnCost(unitStatTemplet, bLeader: true, GetMyTeamData().m_eNKM_TEAM_TYPE));
					if (!flag)
					{
						RemoveCostHolder(unitDataByUnitUID.m_UnitUID, num);
						GetGameHud().SetRespawnCost();
					}
					else
					{
						RemoveCostHolderAssist(unitDataByUnitUID.m_UnitUID, 10f);
						GetGameHud().SetRespawnCostAssist();
					}
				}
			}
		}
		bool bReturnDeckActive = true;
		if (flag && GetMyTeamData().m_DeckData.GetAutoRespawnIndexAssist() < 0)
		{
			bReturnDeckActive = false;
		}
		GetGameHud().UseCompleteDeckByUnitUID(cPacket_GAME_RESPAWN_ACK.unitUID, bReturnDeckActive);
		if (flag)
		{
			GetGameHud().UseCompleteDeckAssist(bReturnDeckActive);
		}
		RespawnCompleteUnitViewer(cPacket_GAME_RESPAWN_ACK.unitUID);
	}

	public void OnRecv(NKMPacket_GAME_UNIT_RETREAT_ACK cPacket_GAME_UNIT_RETREAT_ACK)
	{
		if (cPacket_GAME_UNIT_RETREAT_ACK.errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			NKM_ERROR_CODE errorCode = cPacket_GAME_UNIT_RETREAT_ACK.errorCode;
			if ((uint)(errorCode - 74) <= 4u || (uint)(errorCode - 81) <= 2u)
			{
				GetGameHud().SetMessage(NKCStringTable.GetString(cPacket_GAME_UNIT_RETREAT_ACK.errorCode));
			}
			GetGameHud().UseCompleteDeckByUnitUID(cPacket_GAME_UNIT_RETREAT_ACK.unitUID);
		}
	}

	public void OnRecv(NKMPacket_GAME_SHIP_SKILL_ACK cPacket_GAME_SHIP_SKILL_ACK)
	{
		Log.Debug("[NKMPacket_GAME_SHIP_SKILL_ACK] ErrorCode[" + cPacket_GAME_SHIP_SKILL_ACK.errorCode.ToString() + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameClient.cs", 5014);
		if (cPacket_GAME_SHIP_SKILL_ACK.errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			NKM_ERROR_CODE errorCode = cPacket_GAME_SHIP_SKILL_ACK.errorCode;
			if ((uint)(errorCode - 84) <= 1u || (uint)(errorCode - 87) <= 2u)
			{
				GetGameHud().SetMessage(NKCStringTable.GetString(cPacket_GAME_SHIP_SKILL_ACK.errorCode));
			}
			m_ShipSkillArea.SetShow(bShow: false);
			GetGameHud().ReturnDeckByShipSkillID(cPacket_GAME_SHIP_SKILL_ACK.shipSkillID);
		}
	}

	public void OnRecv(NKMPacket_GAME_TACTICAL_COMMAND_ACK cPacket_GAME_TACTICAL_COMMAND_ACK)
	{
		if (cPacket_GAME_TACTICAL_COMMAND_ACK.errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			NKM_ERROR_CODE errorCode = cPacket_GAME_TACTICAL_COMMAND_ACK.errorCode;
			if ((uint)(errorCode - 92) <= 1u)
			{
				GetGameHud().SetMessage(NKCStringTable.GetString(cPacket_GAME_TACTICAL_COMMAND_ACK.errorCode));
			}
			if (cPacket_GAME_TACTICAL_COMMAND_ACK.cTacticalCommandData.m_TCID > 0)
			{
				GetGameHud().ReturnDeckByTacticalCommandID(cPacket_GAME_TACTICAL_COMMAND_ACK.cTacticalCommandData.m_TCID);
				RemoveCostHolderTC(cPacket_GAME_TACTICAL_COMMAND_ACK.cTacticalCommandData.m_TCID);
				GetGameHud().SetRespawnCost();
			}
		}
		if (cPacket_GAME_TACTICAL_COMMAND_ACK.cTacticalCommandData.m_TCID > 0)
		{
			GetMyTeamData().GetTacticalCommandDataByID(cPacket_GAME_TACTICAL_COMMAND_ACK.cTacticalCommandData.m_TCID)?.DeepCopyFromSource(cPacket_GAME_TACTICAL_COMMAND_ACK.cTacticalCommandData);
		}
	}

	public void OnRecv(NKMPacket_GAME_AUTO_RESPAWN_ACK cPacket_GAME_AUTO_RESPAWN_ACK)
	{
		if (cPacket_GAME_AUTO_RESPAWN_ACK.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			NKMGameRuntimeTeamData myRuntimeTeamData = m_NKMGameRuntimeData.GetMyRuntimeTeamData(m_MyTeam);
			if (myRuntimeTeamData != null)
			{
				myRuntimeTeamData.m_bAutoRespawn = cPacket_GAME_AUTO_RESPAWN_ACK.isAutoRespawn;
			}
			GetGameHud().ToggleAutoRespawn(cPacket_GAME_AUTO_RESPAWN_ACK.isAutoRespawn);
		}
	}

	public void OnRecv(NKMPacket_GAME_OPTION_CHANGE_ACK cNKMPacket_GAME_OPTION_CHANGE_ACK)
	{
		NKMPopUpBox.CloseWaitBox();
		if (cNKMPacket_GAME_OPTION_CHANGE_ACK.errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			return;
		}
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			if (gameOptionData.ActionCamera != cNKMPacket_GAME_OPTION_CHANGE_ACK.actionCameraType)
			{
				gameOptionData.SetUseActionCamera(cNKMPacket_GAME_OPTION_CHANGE_ACK.actionCameraType, bForce: true);
			}
			if (gameOptionData.TrackCamera != cNKMPacket_GAME_OPTION_CHANGE_ACK.isTrackCamera)
			{
				gameOptionData.SetUseTrackCamera(cNKMPacket_GAME_OPTION_CHANGE_ACK.isTrackCamera, bForce: true);
			}
			if (gameOptionData.ViewSkillCutIn != cNKMPacket_GAME_OPTION_CHANGE_ACK.isViewSkillCutIn)
			{
				gameOptionData.SetViewSkillCutIn(cNKMPacket_GAME_OPTION_CHANGE_ACK.isViewSkillCutIn, bForce: true);
			}
			if (gameOptionData.PvPAutoRespawn != cNKMPacket_GAME_OPTION_CHANGE_ACK.defaultPvpAutoRespawn)
			{
				gameOptionData.SetPvPAutoRespawn(cNKMPacket_GAME_OPTION_CHANGE_ACK.defaultPvpAutoRespawn, bForce: true);
			}
			if (gameOptionData.AutSyncFriendDeck != cNKMPacket_GAME_OPTION_CHANGE_ACK.autoSyncFriendDeck)
			{
				gameOptionData.SetAutoSyncFriendDeck(cNKMPacket_GAME_OPTION_CHANGE_ACK.autoSyncFriendDeck, bForce: true);
			}
			NKCUIGameOption.CheckInstanceAndClose();
		}
	}

	public void OnRecv(NKMPacket_GAME_LOAD_COMPLETE_ACK cNKMPacket_GAME_LOAD_COMPLETE_ACK)
	{
		SetGameRuntimeData(cNKMPacket_GAME_LOAD_COMPLETE_ACK.gameRuntimeData);
		GetGameHud().SetUIByRuntimeData(GetGameData(), GetGameRuntimeData());
		MultiplyReward = cNKMPacket_GAME_LOAD_COMPLETE_ACK.rewardMultiply;
		GetGameHud().SetMultiply(MultiplyReward);
	}

	public void OnRecv(NKMPacket_GAME_DEV_COOL_TIME_RESET_ACK cNKMPacket_GAME_DEV_COOL_TIME_RESET_ACK)
	{
		if (cNKMPacket_GAME_DEV_COOL_TIME_RESET_ACK.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			if (cNKMPacket_GAME_DEV_COOL_TIME_RESET_ACK.isSkill)
			{
				DEV_SkillCoolTimeReset(cNKMPacket_GAME_DEV_COOL_TIME_RESET_ACK.teamType);
			}
			else
			{
				DEV_HyperSkillCoolTimeReset(cNKMPacket_GAME_DEV_COOL_TIME_RESET_ACK.teamType);
			}
		}
	}

	public void OnRecv(NKMPacket_GAME_DEV_RESPAWN_ACK cNKMPacket_GAME_DEV_RESPAWN_ACK)
	{
		NKMPopUpBox.CloseWaitBox();
		if (cNKMPacket_GAME_DEV_RESPAWN_ACK.errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			switch (cNKMPacket_GAME_DEV_RESPAWN_ACK.errorCode)
			{
			case NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_UNIT_LIVE:
				GetGameHud().SetMessage("이미 출격중인 유닛입니다.");
				break;
			case NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_INVALID_POS:
				GetGameHud().SetMessage("출격이 불가능한 지역입니다.");
				break;
			case NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_RESPAWN_COST:
				GetGameHud().SetMessage("출격 비용이 부족합니다.");
				break;
			case NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_DECK:
				GetGameHud().SetMessage("존재하지 않는 유닛입니다.");
				break;
			case NKM_ERROR_CODE.NEC_FAIL_NPT_GAME_RESPAWN_ACK_NO_GAME_STATE:
				GetGameHud().SetMessage("지금은 출격할 수 없습니다.");
				break;
			}
		}
		else
		{
			GetGameData().m_NKMGameTeamDataA.m_listDynamicRespawnUnitData = cNKMPacket_GAME_DEV_RESPAWN_ACK.dynamicRespawnUnitDataTeamA;
			GetGameData().m_NKMGameTeamDataB.m_listDynamicRespawnUnitData = cNKMPacket_GAME_DEV_RESPAWN_ACK.dynamicRespawnUnitDataTeamB;
			CreatePoolUnit(null, cNKMPacket_GAME_DEV_RESPAWN_ACK.unitData, 0, cNKMPacket_GAME_DEV_RESPAWN_ACK.teamType, bAsync: false);
			CreateDynaminRespawnPoolUnit(bAsync: false);
		}
	}

	private void ProcessDungeonEventSync(NKMGameSyncData_DungeonEvent cNKMGameSyncData_DungeonEvent)
	{
		if (cNKMGameSyncData_DungeonEvent == null)
		{
			return;
		}
		Debug.Log($"Got GameEvent! {cNKMGameSyncData_DungeonEvent.m_eEventActionType} {cNKMGameSyncData_DungeonEvent.m_EventID} pause : {cNKMGameSyncData_DungeonEvent.m_bPause}");
		if (cNKMGameSyncData_DungeonEvent.m_bPause)
		{
			Send_Packet_GAME_PAUSE_REQ(cNKMGameSyncData_DungeonEvent.m_bPause, bPauseEvent: true);
			GetGameHud().TogglePause(cNKMGameSyncData_DungeonEvent.m_bPause, GetGameData().m_bLocal);
			m_NKMGameRuntimeData.m_bPause = cNKMGameSyncData_DungeonEvent.m_bPause;
			if (NKCUIGameOption.IsInstanceOpen)
			{
				Debug.LogWarning("Pause was open, forcing event");
				NKCUIGameOption.Instance.RemoveCloseCallBack();
				NKCUIGameOption.Instance.Close();
			}
		}
		if (NKMDungeonEventTemplet.IsPermanent(cNKMGameSyncData_DungeonEvent.m_eEventActionType))
		{
			if (m_NKMGameRuntimeData.m_lstPermanentDungeonEvent == null)
			{
				m_NKMGameRuntimeData.m_lstPermanentDungeonEvent = new List<NKMGameSyncData_DungeonEvent>();
			}
			m_NKMGameRuntimeData.m_lstPermanentDungeonEvent.Add(cNKMGameSyncData_DungeonEvent);
		}
		ApplyDungeonEvent(cNKMGameSyncData_DungeonEvent);
	}

	private void ApplyDungeonEvent(NKMGameSyncData_DungeonEvent cNKMGameSyncData_DungeonEvent)
	{
		if (cNKMGameSyncData_DungeonEvent == null)
		{
			return;
		}
		switch (cNKMGameSyncData_DungeonEvent.m_eEventActionType)
		{
		case NKM_EVENT_ACTION_TYPE.GAME_EVENT:
			NKCGameEventManager.PlayGameEvent(cNKMGameSyncData_DungeonEvent.m_EventID, cNKMGameSyncData_DungeonEvent.m_bPause, EventFinished);
			break;
		case NKM_EVENT_ACTION_TYPE.SET_UNIT_HYPER_FULL:
			if (cNKMGameSyncData_DungeonEvent.m_iEventActionValue != 0)
			{
				NKMUnit unitByUnitID = GetUnitByUnitID(cNKMGameSyncData_DungeonEvent.m_iEventActionValue, bChain: true, bPool: true);
				if (unitByUnitID != null)
				{
					List<NKMAttackStateData> listHyperSkillStateData = unitByUnitID.GetUnitTemplet().m_listHyperSkillStateData;
					for (int i = 0; i < listHyperSkillStateData.Count; i++)
					{
						unitByUnitID.SetStateCoolTime(listHyperSkillStateData[i].m_StateName, bMax: false, 0f);
					}
				}
				break;
			}
			foreach (KeyValuePair<short, NKMUnit> item in m_dicNKMUnit)
			{
				NKMUnit value7 = item.Value;
				List<NKMAttackStateData> listHyperSkillStateData2 = value7.GetUnitTemplet().m_listHyperSkillStateData;
				for (int j = 0; j < listHyperSkillStateData2.Count; j++)
				{
					value7.SetStateCoolTime(listHyperSkillStateData2[j].m_StateName, bMax: false, 0f);
				}
			}
			{
				foreach (KeyValuePair<short, NKMUnit> item2 in m_dicNKMUnitPool)
				{
					NKMUnit value8 = item2.Value;
					List<NKMAttackStateData> listHyperSkillStateData3 = value8.GetUnitTemplet().m_listHyperSkillStateData;
					for (int k = 0; k < listHyperSkillStateData3.Count; k++)
					{
						value8.SetStateCoolTime(listHyperSkillStateData3[k].m_StateName, bMax: false, 0f);
					}
				}
				break;
			}
		case NKM_EVENT_ACTION_TYPE.NEAT_RESPAWN_COST_A_TEAM:
			m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_A1).m_fRespawnCost = cNKMGameSyncData_DungeonEvent.m_iEventActionValue;
			break;
		case NKM_EVENT_ACTION_TYPE.NEAT_RESPAWN_COST_B_TEAM:
			m_NKMGameRuntimeData.GetMyRuntimeTeamData(NKM_TEAM_TYPE.NTT_B1).m_fRespawnCost = cNKMGameSyncData_DungeonEvent.m_iEventActionValue;
			break;
		case NKM_EVENT_ACTION_TYPE.ADD_TEAM_A_EXTRA_RESPAWN_COST:
		{
			float num2 = (float)cNKMGameSyncData_DungeonEvent.m_iEventActionValue * 0.01f;
			Debug.Log($"Gameevent : A팀 리스폰코스트 초당 {num2} 추가");
			base.m_NKMGameData.fExtraRespawnCostAddForA += num2;
			break;
		}
		case NKM_EVENT_ACTION_TYPE.ADD_TEAM_B_EXTRA_RESPAWN_COST:
		{
			float num = (float)cNKMGameSyncData_DungeonEvent.m_iEventActionValue * 0.01f;
			Debug.Log($"Gameevent : B팀 리스폰코스트 초당 {num} 추가");
			base.m_NKMGameData.fExtraRespawnCostAddForB += num;
			break;
		}
		case NKM_EVENT_ACTION_TYPE.UNLOCK_TUTORIAL_GAME_RE_RESPAWN:
			Debug.Log("Gameevent : 튜토리얼 재출격 활성화");
			m_bTutorialGameReRespawnAllowed = true;
			break;
		case NKM_EVENT_ACTION_TYPE.CHANGE_BGM:
			if (string.IsNullOrEmpty(cNKMGameSyncData_DungeonEvent.m_strEventActionValue))
			{
				NKCSoundManager.FadeOutMusic();
			}
			else if (cNKMGameSyncData_DungeonEvent.m_iEventActionValue != 0)
			{
				float fStartTime = (float)cNKMGameSyncData_DungeonEvent.m_iEventActionValue / 100f;
				NKCSoundManager.PlayMusic(cNKMGameSyncData_DungeonEvent.m_strEventActionValue, bLoop: true, 1f, bForce: true, fStartTime);
			}
			else if (!NKCSoundManager.IsSameMusic(cNKMGameSyncData_DungeonEvent.m_strEventActionValue))
			{
				NKCSoundManager.PlayMusic(cNKMGameSyncData_DungeonEvent.m_strEventActionValue, bLoop: true);
			}
			break;
		case NKM_EVENT_ACTION_TYPE.CHANGE_BGM_TRACK:
		{
			if (string.IsNullOrEmpty(cNKMGameSyncData_DungeonEvent.m_strEventActionValue))
			{
				NKCSoundManager.FadeOutMusic();
				break;
			}
			float musicTime = NKCSoundManager.GetMusicTime();
			if (!NKCSoundManager.IsSameMusic(cNKMGameSyncData_DungeonEvent.m_strEventActionValue))
			{
				NKCSoundManager.PlayMusic(cNKMGameSyncData_DungeonEvent.m_strEventActionValue, bLoop: true, 1f, bForce: true, musicTime);
			}
			break;
		}
		case NKM_EVENT_ACTION_TYPE.HUD_ALERT:
			GetGameHud().SetMessage(NKCStringTable.GetString(cNKMGameSyncData_DungeonEvent.m_strEventActionValue));
			break;
		case NKM_EVENT_ACTION_TYPE.POPUP_MESSAGE:
			if (NKCScenManager.GetScenManager().GetGameClient().IsShowUI())
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(cNKMGameSyncData_DungeonEvent.m_strEventActionValue), NKCPopupMessage.eMessagePosition.TopIngame);
			}
			break;
		case NKM_EVENT_ACTION_TYPE.MAP_ANIMATION:
			m_Map.PlayMapSpineAnimation(cNKMGameSyncData_DungeonEvent.m_strEventActionValue);
			break;
		case NKM_EVENT_ACTION_TYPE.DELTA_GAME_TIME:
			GetGameHud().ShowChangeRemainTimeEffect((float)cNKMGameSyncData_DungeonEvent.m_iEventActionValue / 100f);
			break;
		case NKM_EVENT_ACTION_TYPE.CHANGE_CAMERA:
		{
			Dictionary<string, float> dictionary = NKCUtil.ParseStringTable<float>(cNKMGameSyncData_DungeonEvent.m_strEventActionValue, float.TryParse);
			if (!dictionary.TryGetValue("m_fCamMinX", out var value))
			{
				value = m_NKMMapTemplet.m_fCamMinX;
			}
			if (!dictionary.TryGetValue("m_fCamMaxX", out var value2))
			{
				value2 = m_NKMMapTemplet.m_fCamMaxX;
			}
			if (!dictionary.TryGetValue("m_fCamMinY", out var value3))
			{
				value3 = m_NKMMapTemplet.m_fCamMinY;
			}
			if (!dictionary.TryGetValue("m_fCamMaxY", out var value4))
			{
				value4 = m_NKMMapTemplet.m_fCamMaxY;
			}
			if (!dictionary.TryGetValue("m_fCamSize", out var value5))
			{
				value5 = m_NKMMapTemplet.m_fCamSize;
			}
			if (!dictionary.TryGetValue("m_fCamSizeMax", out var value6))
			{
				value6 = m_NKMMapTemplet.m_fCamSizeMax;
			}
			NKCCamera.InitBattle(value, value2, value3, value4, value5, value6);
			break;
		}
		case NKM_EVENT_ACTION_TYPE.SET_ENEMY_BOSS_HP_RATE:
		case NKM_EVENT_ACTION_TYPE.KILL_ALL_TAGGED_UNIT:
		case NKM_EVENT_ACTION_TYPE.ADD_EVENTTAG:
		case NKM_EVENT_ACTION_TYPE.SET_EVENTTAG:
		case NKM_EVENT_ACTION_TYPE.FORCE_WIN:
		case NKM_EVENT_ACTION_TYPE.FORCE_LOSE:
		case NKM_EVENT_ACTION_TYPE.SET_UNIT_STATE:
		case NKM_EVENT_ACTION_TYPE.SET_ENEMY_UNIT_STATE:
		case NKM_EVENT_ACTION_TYPE.SET_GAME_TIME:
			break;
		}
	}

	private void ReprocessPermanentDungeonEvent(List<NKMGameSyncData_DungeonEvent> lstPermanentEvents)
	{
		foreach (NKMGameSyncData_DungeonEvent lstPermanentEvent in lstPermanentEvents)
		{
			if (lstPermanentEvent.m_eEventActionType == NKM_EVENT_ACTION_TYPE.MAP_ANIMATION)
			{
				m_Map.PlayMapSpineAnimationLastOnly(lstPermanentEvent.m_strEventActionValue);
			}
			else
			{
				ApplyDungeonEvent(lstPermanentEvent);
			}
		}
	}

	private void ProcessGameEventSync(NKMGameSyncData_GameEvent cNKMGameSyncData_GameEvent)
	{
		switch (cNKMGameSyncData_GameEvent.m_NKM_GAME_EVENT_TYPE)
		{
		case NKM_GAME_EVENT_TYPE.NGET_TACTICAL_COMMAND:
			if (cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE == NKM_TEAM_TYPE.NTT_INVALID || GetMyTeamData().m_eNKM_TEAM_TYPE == cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE)
			{
				ProcessGameEventTCSync(cNKMGameSyncData_GameEvent);
			}
			break;
		case NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_SUCCESS:
		case NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_FAIL:
		case NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_SKILL_SUCCESS:
		case NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_SKILL_REAL_APPLY_AFTER_SUCCESS:
		case NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_COOL_TIME_ON:
			ProcessGameEventTC_Combo(cNKMGameSyncData_GameEvent);
			break;
		case NKM_GAME_EVENT_TYPE.NGET_KILL_COUNT:
			if ((cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE == NKM_TEAM_TYPE.NTT_INVALID || GetMyTeamData().m_eNKM_TEAM_TYPE == cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE) && GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_PVE_DEFENCE)
			{
				NKCKillCountManager.CurrentStageKillCount = (long)Mathf.Round(cNKMGameSyncData_GameEvent.m_fValue);
				GetGameHud().SetKillCount(NKCKillCountManager.CurrentStageKillCount);
			}
			break;
		case NKM_GAME_EVENT_TYPE.NGET_AUTO_RESPAWN_WARNING:
			if (GetMyTeamData().m_eNKM_TEAM_TYPE == cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE)
			{
				GetGameHud().SetMessage(NKCStringTable.GetString("SI_PF_GAUNTLET_HAVE_NO_ACTION_TEXT"));
			}
			break;
		case NKM_GAME_EVENT_TYPE.NGET_AUTO_RESPAWN_SET:
			if (GetMyTeamData().m_eNKM_TEAM_TYPE == cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE)
			{
				bool flag = cNKMGameSyncData_GameEvent.m_EventID != 0;
				NKMGameRuntimeTeamData myRunTimeTeamData = GetMyRunTimeTeamData();
				if (myRunTimeTeamData != null)
				{
					myRunTimeTeamData.m_bAutoRespawn = flag;
					GetGameHud().ToggleAutoRespawn(flag);
				}
			}
			break;
		case NKM_GAME_EVENT_TYPE.NGET_COST_RETURN:
			if (GetMyTeamData().m_eNKM_TEAM_TYPE == cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE)
			{
				GetGameHud().PlayRespawnAddEvent(cNKMGameSyncData_GameEvent.m_fValue);
			}
			break;
		}
	}

	private void ProcessGameEventTCSync(NKMGameSyncData_GameEvent cNKMGameSyncData_GameEvent)
	{
		GetGameHud().ReturnDeckByTacticalCommandID(cNKMGameSyncData_GameEvent.m_EventID);
		RemoveCostHolderTC(cNKMGameSyncData_GameEvent.m_EventID);
		GetGameHud().SetRespawnCost();
	}

	private void PlayOperatorSkillCutinEffect(bool bRight, float fDurationTime, string operatorName, string skillName, string cutinBG_EffectName, string cutinEffectName, string cutInEffectAnimName)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && !gameOptionData.ViewSkillCutIn)
		{
			return;
		}
		GetNKCEffectManager().StopCutInEffect();
		float num = 1.1f;
		num = GetGameRuntimeData().m_NKM_GAME_SPEED_TYPE switch
		{
			NKM_GAME_SPEED_TYPE.NGST_2 => 1.5f, 
			NKM_GAME_SPEED_TYPE.NGST_05 => 0.6f, 
			_ => 1.1f, 
		};
		NKCASEffect nKCASEffect = GetNKCEffectManager().UseEffect(0, cutinBG_EffectName, cutinBG_EffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, 0f, 0f, 0f, bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "BASE", num, bNotStart: false, bCutIn: true, 0f, fDurationTime);
		if (nKCASEffect != null && nKCASEffect.m_Animator != null)
		{
			nKCASEffect.m_Animator.SetInteger("ColorIndex", 1);
		}
		GetNKCEffectManager().UseEffect(0, cutinEffectName, cutinEffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_CONTROL_EFFECT, 0f, 0f, 0f, bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, cutInEffectAnimName, num, bNotStart: false, bCutIn: true);
		NKCASEffect nKCASEffect2 = GetNKCEffectManager().UseEffect(0, "AB_FX_SKILL_CUTIN_COMMON_DESC", "AB_FX_SKILL_CUTIN_COMMON_DESC", NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_CONTROL_EFFECT, 0f, 0f, 0f, bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "BASE", num, bNotStart: false, bCutIn: true, 0f, fDurationTime);
		if (nKCASEffect2 != null)
		{
			nKCASEffect2.Init_AB_FX_SKILL_CUTIN_COMMON_DESC();
			NKCUtil.SetLabelText(nKCASEffect2.m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME, operatorName);
			if (bRight)
			{
				NKCUtil.SetRectTransformLocalRotate(nKCASEffect2.m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME_RectTransform, 0f, 0f, 0f);
			}
			else
			{
				NKCUtil.SetRectTransformLocalRotate(nKCASEffect2.m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME_RectTransform, 0f, 180f, 0f);
			}
			NKCUtil.SetLabelText(nKCASEffect2.m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME, skillName);
			if (bRight)
			{
				NKCUtil.SetRectTransformLocalRotate(nKCASEffect2.m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME_RectTransform, 0f, 0f, 0f);
			}
			else
			{
				NKCUtil.SetRectTransformLocalRotate(nKCASEffect2.m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME_RectTransform, 0f, 180f, 0f);
			}
		}
	}

	private void ProcessGameEventTC_Combo(NKMGameSyncData_GameEvent cNKMGameSyncData_GameEvent)
	{
		if (cNKMGameSyncData_GameEvent == null)
		{
			return;
		}
		if (cNKMGameSyncData_GameEvent.m_NKM_GAME_EVENT_TYPE == NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_SKILL_REAL_APPLY_AFTER_SUCCESS)
		{
			SetStopTime(NKMCommonConst.OPERATOR_SKILL_STOP_TIME, NKM_STOP_TIME_INDEX.NSTI_OPERATOR_SKILL);
			if (GetGameData() == null)
			{
				return;
			}
			NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(cNKMGameSyncData_GameEvent.m_EventID);
			if (tacticalCommandTempletByID != null)
			{
				bool flag = IsSameTeam(GetMyTeamData().m_eNKM_TEAM_TYPE, cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE);
				float num = 0f;
				float posZ = (m_NKMMapTemplet.m_fMinZ + m_NKMMapTemplet.m_fMaxZ) / 2f;
				num = ((!flag) ? m_NKMMapTemplet.m_fMaxX : m_NKMMapTemplet.m_fMinX);
				if (!string.IsNullOrWhiteSpace(tacticalCommandTempletByID.m_TCEffectSound))
				{
					NKCSoundManager.PlaySound(tacticalCommandTempletByID.m_TCEffectSound, 1f, 0f, 0f, bLoop: false, 1f);
				}
				GetNKCEffectManager().UseEffect(0, tacticalCommandTempletByID.m_TCEffectName, tacticalCommandTempletByID.m_TCEffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, num, -200f, posZ, flag, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "BASE", 1f, bNotStart: false, bCutIn: false, 1f);
			}
			NKMGameTeamData teamData = GetGameData().GetTeamData(cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE);
			if (teamData == null || teamData.m_Operator == null)
			{
				return;
			}
			teamData.GetTC_Combo().SetActiveTime(tacticalCommandTempletByID);
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_COMBO_COMPLETE, teamData.m_Operator);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(teamData.m_Operator.id);
			if (unitTempletBase != null)
			{
				string skillName = "";
				if (tacticalCommandTempletByID != null)
				{
					skillName = tacticalCommandTempletByID.GetTCName();
				}
				PlayOperatorSkillCutinEffect(IsSameTeam(GetMyTeamData().m_eNKM_TEAM_TYPE, cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE), 1f, unitTempletBase.GetUnitName(), skillName, "AB_FX_SKILL_CUTIN_COMMON_A", GetFinalOperatorCutinEffectName(teamData.m_Operator.id), "BASE");
			}
		}
		else
		{
			if ((cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE != NKM_TEAM_TYPE.NTT_INVALID && GetMyTeamData().m_eNKM_TEAM_TYPE != cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE && !IsObserver(NKCScenManager.CurrentUserData())) || cNKMGameSyncData_GameEvent.m_EventID <= 0)
			{
				return;
			}
			NKMTacticalCommandData tacticalCommandDataByID = GetMyTeamData().GetTacticalCommandDataByID(cNKMGameSyncData_GameEvent.m_EventID);
			if (IsObserver(NKCScenManager.CurrentUserData()))
			{
				tacticalCommandDataByID = GetGameData().GetTeamData(cNKMGameSyncData_GameEvent.m_NKM_TEAM_TYPE).GetTacticalCommandDataByID(cNKMGameSyncData_GameEvent.m_EventID);
			}
			if (tacticalCommandDataByID == null)
			{
				return;
			}
			_ = tacticalCommandDataByID.m_ComboCount;
			NKMTacticalCommandTemplet tacticalCommandTempletByID2 = NKMTacticalCommandManager.GetTacticalCommandTempletByID(cNKMGameSyncData_GameEvent.m_EventID);
			switch (cNKMGameSyncData_GameEvent.m_NKM_GAME_EVENT_TYPE)
			{
			case NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_SUCCESS:
				tacticalCommandDataByID.m_ComboCount = (byte)cNKMGameSyncData_GameEvent.m_fValue;
				tacticalCommandDataByID.m_fComboResetCoolTimeNow = tacticalCommandTempletByID2.m_fComboResetCoolTime;
				break;
			case NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_FAIL:
				tacticalCommandDataByID.m_ComboCount = (byte)cNKMGameSyncData_GameEvent.m_fValue;
				break;
			case NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_SKILL_SUCCESS:
				tacticalCommandDataByID.m_ComboCount = (byte)cNKMGameSyncData_GameEvent.m_fValue;
				tacticalCommandDataByID.m_fCoolTimeNow = tacticalCommandTempletByID2.m_fCoolTime;
				tacticalCommandDataByID.m_bCoolTimeOn = false;
				break;
			case NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_COOL_TIME_ON:
				if (cNKMGameSyncData_GameEvent.m_fValue == 1f)
				{
					tacticalCommandDataByID.m_bCoolTimeOn = true;
				}
				else
				{
					tacticalCommandDataByID.m_bCoolTimeOn = false;
				}
				break;
			case NKM_GAME_EVENT_TYPE.NGET_TC_COMBO_SKILL_REAL_APPLY_AFTER_SUCCESS:
				break;
			}
		}
	}

	public void EventFinished(bool bUnpause)
	{
		if (bUnpause)
		{
			Send_Packet_GAME_PAUSE_REQ(bPause: false);
		}
	}

	public void PlaySkillCutIn(NKCUnitClient caller, bool bHyper, bool bRight, Sprite faceSprite, string unitName, string skillName)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && !gameOptionData.ViewSkillCutIn)
		{
			return;
		}
		if (!bHyper)
		{
			if (bRight)
			{
				m_NKCSkillCutInSideRedRight.Stop();
				m_NKCSkillCutInSideRight.Play(GetNKCEffectManager(), faceSprite, unitName, skillName);
			}
			else
			{
				m_NKCSkillCutInSideRedLeft.Stop();
				m_NKCSkillCutInSideLeft.Play(GetNKCEffectManager(), faceSprite, unitName, skillName);
			}
		}
		else if (bRight)
		{
			m_NKCSkillCutInSideRight.Stop();
			m_NKCSkillCutInSideRedRight.Play(GetNKCEffectManager(), faceSprite, unitName, skillName);
		}
		else
		{
			m_NKCSkillCutInSideLeft.Stop();
			m_NKCSkillCutInSideRedLeft.Play(GetNKCEffectManager(), faceSprite, unitName, skillName);
		}
	}

	private bool HasTimeLimit()
	{
		NKM_GAME_TYPE gameType = GetGameData().GetGameType();
		if (gameType <= NKM_GAME_TYPE.NGT_PRACTICE || gameType == NKM_GAME_TYPE.NGT_TUTORIAL)
		{
			return false;
		}
		return true;
	}

	public bool EnableControlByGameType(NKM_ERROR_CODE failErrorCode = NKM_ERROR_CODE.NEC_OK)
	{
		if (IsObserver(NKCScenManager.CurrentUserData()))
		{
			return false;
		}
		if (NKCReplayMgr.IsPlayingReplay())
		{
			return false;
		}
		switch (GetGameData().GetGameType())
		{
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
			if (failErrorCode != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupMessageManager.AddPopupMessage(failErrorCode);
			}
			return false;
		case NKM_GAME_TYPE.NGT_PVP_EVENT:
			if (GetGameData().m_bForcedAuto)
			{
				NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_EVENT_PVP_MANUAL_PLAY_DISABLE);
				return false;
			}
			break;
		}
		return true;
	}

	public bool CanReRespawn()
	{
		if (GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_TUTORIAL)
		{
			return m_bTutorialGameReRespawnAllowed;
		}
		return true;
	}

	public void HitEffect(NKCUnitClient cUnit, NKMDamageInst cNKMDamageInst, string hitEffectBundleName, string hitEffect, string hitEffectAnimName, float fHitEffectRange, float fHitEffectOffsetZ, bool bHitEffectLand)
	{
		if (cUnit == null || cNKMDamageInst == null)
		{
			return;
		}
		float num = cUnit.GetUnitSyncData().m_PosZ + fHitEffectOffsetZ;
		m_TempMinMaxVec3.m_MinX = cUnit.GetUnitSyncData().m_PosX - fHitEffectRange;
		m_TempMinMaxVec3.m_MinY = num + cUnit.GetUnitSyncData().m_JumpYPos + cUnit.GetUnitTemplet().m_UnitSizeY * 0.5f - fHitEffectRange;
		m_TempMinMaxVec3.m_MinZ = num - fHitEffectRange;
		m_TempMinMaxVec3.m_MaxX = cUnit.GetUnitSyncData().m_PosX + fHitEffectRange;
		m_TempMinMaxVec3.m_MaxY = num + cUnit.GetUnitSyncData().m_JumpYPos + cUnit.GetUnitTemplet().m_UnitSizeY * 0.5f + fHitEffectRange;
		m_TempMinMaxVec3.m_MaxZ = num + fHitEffectRange;
		if (cUnit.GetUnitFrameData().m_BarrierBuffData != null && cUnit.GetUnitFrameData().m_BarrierBuffData.m_fBarrierHP > 0f && cUnit.GetUnitFrameData().m_BarrierBuffData.m_NKMBuffTemplet.m_BarrierDamageEffectName.Length > 1)
		{
			GetNKCEffectManager().UseEffect(0, cUnit.GetUnitFrameData().m_BarrierBuffData.m_NKMBuffTemplet.m_BarrierDamageEffectName, cUnit.GetUnitFrameData().m_BarrierBuffData.m_NKMBuffTemplet.m_BarrierDamageEffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_TempMinMaxVec3.GetRandomX(), m_TempMinMaxVec3.GetRandomY(), num, cUnit.GetUnitSyncData().m_bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: true, hitEffectAnimName, 1f, bNotStart: false, bCutIn: false, 0.05f * (float)cNKMDamageInst.m_listHitUnit.Count);
			if (cUnit.GetBuffEffectDic().ContainsKey(cUnit.GetUnitFrameData().m_BarrierBuffData.m_NKMBuffTemplet.m_BuffID))
			{
				cUnit.GetBuffEffectDic()[cUnit.GetUnitFrameData().m_BarrierBuffData.m_NKMBuffTemplet.m_BuffID].PlayAnim("DAMAGED");
			}
			return;
		}
		if (bHitEffectLand)
		{
			m_TempMinMaxVec3.m_MinY = num;
			m_TempMinMaxVec3.m_MaxY = num;
		}
		NKCASEffect nKCASEffect = null;
		LinkedListNode<NKCASEffect> linkedListNode = m_linklistHitEffect.First;
		while (linkedListNode != null)
		{
			nKCASEffect = linkedListNode.Value;
			if (nKCASEffect != null && !GetNKCEffectManager().IsLiveEffect(nKCASEffect.m_EffectUID))
			{
				LinkedListNode<NKCASEffect> next = linkedListNode.Next;
				m_linklistHitEffect.Remove(linkedListNode);
				linkedListNode = next;
			}
			else if (linkedListNode != null)
			{
				linkedListNode = linkedListNode.Next;
			}
		}
		bool flag = true;
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && !gameOptionData.UseHitEffect)
		{
			flag = false;
		}
		if (m_bSimpleHitEffectFrame && m_linklistHitEffect.Count > 2)
		{
			flag = false;
			m_bSimpleHitEffectFrame = false;
		}
		else
		{
			m_bSimpleHitEffectFrame = true;
		}
		if (!flag)
		{
			hitEffectBundleName = "AB_FX_HIT_SIMPLE";
			hitEffect = "AB_FX_HIT_SIMPLE";
		}
		nKCASEffect = GetNKCEffectManager().UseEffect(0, hitEffectBundleName, hitEffect, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_TempMinMaxVec3.GetRandomX(), m_TempMinMaxVec3.GetRandomY(), num, cUnit.GetUnitSyncData().m_bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: true, hitEffectAnimName, 1f, bNotStart: false, bCutIn: false, 0.05f * (float)cNKMDamageInst.m_listHitUnit.Count);
		if (flag && nKCASEffect != null)
		{
			m_linklistHitEffect.AddLast(nKCASEffect);
		}
	}

	private void GetSortUnitListByZ()
	{
		if (!m_bSortUnitZDirty)
		{
			return;
		}
		m_listSortUnitZ.Clear();
		List<NKMUnit> unitChain = GetUnitChain();
		for (int i = 0; i < unitChain.Count; i++)
		{
			NKCUnitClient nKCUnitClient = (NKCUnitClient)unitChain[i];
			if (nKCUnitClient.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_ENV)
			{
				m_listSortUnitZ.Add(nKCUnitClient);
			}
		}
		m_listSortUnitZ.Sort((NKCUnitClient b, NKCUnitClient a) => a.GetUnitSyncData().m_PosZ.CompareTo(b.GetUnitSyncData().m_PosZ));
		m_bSortUnitZDirty = false;
		SortUnitSkillTouchObject(m_listSortUnitZ);
	}

	private void SortUnitSkillTouchObject(List<NKCUnitClient> unitList)
	{
		for (int i = 0; i < unitList.Count; i++)
		{
			unitList[i]?.MoveToLastTouchObject();
		}
	}

	protected override void ActivateDelayedBattleConditions(NKMBattleConditionTemplet bcTemplet, int level)
	{
		if (bcTemplet != null)
		{
			base.ActivateDelayedBattleConditions(bcTemplet, level);
			m_BattleCondition.EnableDelayedBC(bcTemplet);
			GetGameHud().EnqueueHudBattleConditionAlert(bcTemplet);
		}
	}

	private void PlayGameIntro()
	{
		NKCLoadingScreenManager.PlayDungeonIntro(GetGameData());
	}

	private void PlayGameOutro()
	{
		NKCLoadingScreenManager.PlayDungeonOutro(GetGameData());
	}
}
