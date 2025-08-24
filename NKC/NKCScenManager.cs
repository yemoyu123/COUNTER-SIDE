using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using AssetBundles;
using ClientPacket.Warfare;
using Cs.Logging;
using Cs.Protocol;
using NKC.Advertise;
using NKC.Loading;
using NKC.Localization;
using NKC.PacketHandler;
using NKC.Patcher;
using NKC.Publisher;
using NKC.Trim;
using NKC.UI;
using NKC.UI.Event;
using NKC.UI.Option;
using NKC.Util;
using NKM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NKC;

public class NKCScenManager : MonoBehaviour
{
	public delegate void DoAfterScenChange();

	private static bool m_bApplicationDelegateRegistered;

	private static NKCScenManager m_ScenManager;

	private int m_SystemMemorySize;

	public NKCSystemEvent m_NKCSystemEvent;

	public NKCMemoryCleaner m_NKCMemoryCleaner;

	private bool m_UIInit;

	private GameObject m_NKM_UI_FPS;

	private Text m_NKM_UI_FPS_Text;

	private NKM_SCEN_ID m_eNextScen;

	private Dictionary<NKM_SCEN_ID, NKC_SCEN_BASIC> m_dicSCEN = new Dictionary<NKM_SCEN_ID, NKC_SCEN_BASIC>();

	private NKC_SCEN_BASIC m_NKM_SCEN_NOW;

	private NKC_SCEN_LOGIN m_NKC_SCEN_LOGIN;

	private NKC_SCEN_HOME m_NKC_SCEN_HOME;

	private NKC_SCEN_GAME m_NKC_SCEN_GAME;

	private NKC_SCEN_TEAM m_NKC_SCEN_TEAM;

	private NKC_SCEN_BASE m_NKC_SCEN_BASE;

	private NKC_SCEN_CONTRACT m_NKC_SCEN_CONTRACT;

	private NKC_SCEN_INVENTORY m_NKC_SCEN_INVENTORY;

	private NKC_SCEN_CUTSCEN_SIM m_NKC_SCEN_CUTSCEN_SIM;

	private NKC_SCEN_OPERATION_V2 m_NKC_SCEN_OPERATION_V2;

	private NKC_SCEN_DUNGEON_ATK_READY m_NKC_SCEN_DUNGEON_ATK_READY;

	private NKC_SCEN_UNIT_LIST m_NKC_SCEN_UNIT_LIST;

	private NKC_SCEN_COLLECTION m_NKC_SCEN_COLLECTION;

	private NKC_SCEN_WARFARE_GAME m_NKC_SCEN_WARFARE_GAME;

	private NKC_SCEN_SHOP m_NKC_SCEN_SHOP;

	private NKC_SCEN_FRIEND m_NKC_SCEN_FRIEND;

	private NKC_SCEN_WORLDMAP m_NKC_SCEN_WORLDMAP;

	private NKC_SCEN_CUTSCEN_DUNGEON m_NKC_SCEN_CUTSCEN_DUNGEON;

	private NKC_SCEN_GAME_RESULT m_NKC_SCEN_GAME_RESULT;

	private NKC_SCEN_DIVE_READY m_NKC_SCEN_DIVE_READY;

	private NKC_SCEN_DIVE m_NKC_SCEN_DIVE;

	private NKC_SCEN_DIVE_RESULT m_NKC_SCEN_DIVE_RESULT;

	private NKC_SCEN_GAUNTLET_INTRO m_NKC_SCEN_GAUNTLET_INTRO;

	private NKC_SCEN_GAUNTLET_LOBBY m_NKC_SCEN_GAUNTLET_LOBBY;

	private NKC_SCEN_GAUNTLET_MATCH_READY m_NKC_SCEN_GAUNTLET_MATCH_READY;

	private NKC_SCEN_GAUNTLET_MATCH m_NKC_SCEN_GAUNTLET_MATCH;

	private NKC_SCEN_GAUNTLET_ASYNC_READY m_NKC_SCEN_GAUNTLET_ASYNC_READY;

	private NKC_SCEN_GAUNTLET_LEAGUE_ROOM m_NKC_SCEN_GAUNTLET_LEAGUE_ROOM;

	private NKC_SCEN_GAUNTLET_PRIVATE_ROOM m_NKC_SCEN_GAUNTLET_PRIVATE_ROOM;

	private NKC_SCEN_GAUNTLET_EVENT_READY m_NKC_SCEN_GAUNTLET_EVENT_READY;

	private NKC_SCEN_RAID m_NKC_SCEN_RAID;

	private NKC_SCEN_RAID_READY m_NKC_SCEN_RAID_READY;

	private NKC_SCEN_VOICE_LIST m_NKC_SCEN_VOICE_LIST;

	private NKC_SCEN_SHADOW_PALACE m_NKC_SCEN_SHADOW_PALACE;

	private NKC_SCEN_SHADOW_BATTLE m_NKC_SCEN_SHADOW_BATTLE;

	private NKC_SCEN_SHADOW_RESULT m_NKC_SCEN_SHADOW_RESULT;

	private NKC_SCEN_GUILD_INTRO m_NKC_SCEN_GUILD_INTRO;

	private NKC_SCEN_GUILD_LOBBY m_NKC_SCEN_GUILD_LOBBY;

	private NKC_SCEN_FIERCE_BATTLE_SUPPORT m_NKC_SCEN_FIERCE_BATTLE_SUPPORT;

	private NKC_SCEN_GUILD_COOP m_NKC_SCEN_GUILD_COOP;

	private NKC_SCEN_OFFICE m_NKC_SCEN_OFFICE;

	private NKC_SCEN_TRIM m_NKC_SCEN_TRIM;

	private NKC_SCEN_TRIM_RESULT m_NKC_SCEN_TRIM_RESULT;

	private NKC_SCEN_DUNGEON_RESULT m_NKC_SCEN_DUNGEON_RESULT;

	private DoAfterScenChange dDoAfterScenChange;

	private float m_enableConnectCheckTime = -1f;

	private NKCConnectLogin m_NKCConnectLogin = new NKCConnectLogin();

	private NKCConnectGame m_NKCConnectGame = new NKCConnectGame();

	private NKCGameClient m_NKCGameClient;

	private float m_FixedFrameTime = 1f / 60f;

	private float m_fFPSTime;

	private StringBuilder m_FPSText = new StringBuilder();

	private bool m_bScenChanging;

	private Queue<NKCScenChangeOrder> m_qScenChange = new Queue<NKCScenChangeOrder>();

	private NKMUserData m_MyUserData;

	private NKCReplayMgr m_NKCReplayMgr = new NKCReplayMgr();

	private NKCContractDataMgr m_NKCContractDataMgr = new NKCContractDataMgr();

	private NKCFierceBattleSupportDataMgr m_NKCFierceBattleSupportDataMgr = new NKCFierceBattleSupportDataMgr();

	private NKCRaidDataMgr m_NKCRaidDataMgr = new NKCRaidDataMgr();

	private NKCRepeatOperaion m_NKCRepeatOperaion = new NKCRepeatOperaion();

	private NKCPowerSaveMode m_NKCPowerSaveMode = new NKCPowerSaveMode();

	private NKCSurveyMgr m_NKCSurveyMgr = new NKCSurveyMgr();

	private NKCGameOptionData m_GameOptionData = new NKCGameOptionData();

	private NKCEventPassDataManager m_NKCEventPassDataManager = new NKCEventPassDataManager();

	private GameObject m_objTextureCamera;

	public const int TextureCaptureLayer = 31;

	private bool[] m_bTouch = new bool[3];

	private Vector3[] m_TouchPos3D = new Vector3[3];

	private Vector2[] m_TouchPos2D = new Vector2[3];

	private bool m_bHasPinch;

	private float m_fPinchDeltaMagnitude;

	private float m_fPinchDeltaRotation;

	private Vector2 m_vPinchCenter;

	private float m_fScreenWidthInvSquare;

	private GameObject m_NKM_NEW_INSTANT;

	protected NKCEffectManager m_NKCEffectManager = new NKCEffectManager();

	private RectTransform m_rtNUF_AFTER_UI_EFFECT;

	private Rect m_RectToDrawFPS;

	private bool m_bSkipScenChangeFadeOutEffect;

	private NKCHealthyGame m_NKCHealthyGame;

	private bool m_bSetLanguage;

	private RenderTexture m_tempTexture;

	private MeshRenderer tempMeshRenderer;

	private NKM_SCEN_ID m_CurScenID;

	private NKM_SCEN_ID m_BeforeScenID;

	private Coroutine CameraPermissionCoroutine;

	private bool m_bVersionCheckRunning;

	private float m_fAppEnableConnectCheckTime
	{
		get
		{
			return m_enableConnectCheckTime;
		}
		set
		{
			m_enableConnectCheckTime = value;
		}
	}

	public WarfareGameData WarfareGameData { get; private set; }

	public Camera TextureCamera { get; private set; }

	public static NKCScenManager GetScenManager()
	{
		return m_ScenManager;
	}

	public int GetSystemMemorySize()
	{
		return m_SystemMemorySize;
	}

	public void SetAppEnableConnectCheckTime(float fTime, bool bForce = false)
	{
		if (fTime == -1f || m_fAppEnableConnectCheckTime < 0f || m_fAppEnableConnectCheckTime > fTime || bForce)
		{
			m_fAppEnableConnectCheckTime = fTime;
		}
	}

	public NKCConnectLogin GetConnectLogin()
	{
		return m_NKCConnectLogin;
	}

	public NKCConnectGame GetConnectGame()
	{
		return m_NKCConnectGame;
	}

	public NKCGameClient GetGameClient()
	{
		return m_NKCGameClient;
	}

	public float GetFixedFrameTime()
	{
		return m_FixedFrameTime;
	}

	public NKCScenChangeOrder PeekNextScenChangeOrder()
	{
		if (m_qScenChange.Count <= 0)
		{
			return null;
		}
		return m_qScenChange.Peek();
	}

	public NKM_SCEN_ID GetNowScenID()
	{
		if (m_NKM_SCEN_NOW != null)
		{
			return m_NKM_SCEN_NOW.Get_NKM_SCEN_ID();
		}
		return NKM_SCEN_ID.NSI_INVALID;
	}

	public NKC_SCEN_STATE GetNowScenState()
	{
		if (m_NKM_SCEN_NOW != null)
		{
			return m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE();
		}
		return NKC_SCEN_STATE.NSS_INVALID;
	}

	public NKC_SCEN_LOGIN Get_SCEN_LOGIN()
	{
		return m_NKC_SCEN_LOGIN;
	}

	public NKC_SCEN_HOME Get_SCEN_HOME()
	{
		return m_NKC_SCEN_HOME;
	}

	public NKC_SCEN_GAME Get_SCEN_GAME()
	{
		return m_NKC_SCEN_GAME;
	}

	public NKC_SCEN_TEAM Get_SCEN_TEAM()
	{
		return m_NKC_SCEN_TEAM;
	}

	public NKC_SCEN_BASE Get_SCEN_BASE()
	{
		return m_NKC_SCEN_BASE;
	}

	public NKC_SCEN_CONTRACT GET_SCEN_CONTRACT()
	{
		return m_NKC_SCEN_CONTRACT;
	}

	public NKC_SCEN_INVENTORY Get_SCEN_INVENTORY()
	{
		return m_NKC_SCEN_INVENTORY;
	}

	public NKC_SCEN_CUTSCEN_SIM Get_SCEN_CUTSCEN_SIM()
	{
		return m_NKC_SCEN_CUTSCEN_SIM;
	}

	public NKC_SCEN_OPERATION_V2 Get_SCEN_OPERATION()
	{
		return m_NKC_SCEN_OPERATION_V2;
	}

	public NKC_SCEN_DUNGEON_ATK_READY Get_SCEN_DUNGEON_ATK_READY()
	{
		return m_NKC_SCEN_DUNGEON_ATK_READY;
	}

	public NKC_SCEN_UNIT_LIST GET_NKC_SCEN_UNIT_LIST()
	{
		return m_NKC_SCEN_UNIT_LIST;
	}

	public NKC_SCEN_COLLECTION Get_NKC_SCEN_COLLECTION()
	{
		return m_NKC_SCEN_COLLECTION;
	}

	public NKC_SCEN_WARFARE_GAME Get_NKC_SCEN_WARFARE_GAME()
	{
		return m_NKC_SCEN_WARFARE_GAME;
	}

	public NKC_SCEN_SHOP Get_NKC_SCEN_SHOP()
	{
		return m_NKC_SCEN_SHOP;
	}

	public NKC_SCEN_FRIEND Get_NKC_SCEN_FRIEND()
	{
		return m_NKC_SCEN_FRIEND;
	}

	public NKC_SCEN_WORLDMAP Get_NKC_SCEN_WORLDMAP()
	{
		return m_NKC_SCEN_WORLDMAP;
	}

	public NKC_SCEN_CUTSCEN_DUNGEON Get_NKC_SCEN_CUTSCEN_DUNGEON()
	{
		return m_NKC_SCEN_CUTSCEN_DUNGEON;
	}

	public NKC_SCEN_GAME_RESULT Get_NKC_SCEN_GAME_RESULT()
	{
		return m_NKC_SCEN_GAME_RESULT;
	}

	public NKC_SCEN_DIVE_READY Get_NKC_SCEN_DIVE_READY()
	{
		return m_NKC_SCEN_DIVE_READY;
	}

	public NKC_SCEN_DIVE Get_NKC_SCEN_DIVE()
	{
		return m_NKC_SCEN_DIVE;
	}

	public NKC_SCEN_DIVE_RESULT Get_NKC_SCEN_DIVE_RESULT()
	{
		return m_NKC_SCEN_DIVE_RESULT;
	}

	public NKC_SCEN_GAUNTLET_INTRO Get_NKC_SCEN_GAUNTLET_INTRO()
	{
		return m_NKC_SCEN_GAUNTLET_INTRO;
	}

	public NKC_SCEN_GAUNTLET_LOBBY Get_NKC_SCEN_GAUNTLET_LOBBY()
	{
		return m_NKC_SCEN_GAUNTLET_LOBBY;
	}

	public NKC_SCEN_GAUNTLET_MATCH_READY Get_NKC_SCEN_GAUNTLET_MATCH_READY()
	{
		return m_NKC_SCEN_GAUNTLET_MATCH_READY;
	}

	public NKC_SCEN_GAUNTLET_MATCH Get_NKC_SCEN_GAUNTLET_MATCH()
	{
		return m_NKC_SCEN_GAUNTLET_MATCH;
	}

	public NKC_SCEN_GAUNTLET_ASYNC_READY Get_NKC_SCEN_GAUNTLET_ASYNC_READY()
	{
		return m_NKC_SCEN_GAUNTLET_ASYNC_READY;
	}

	public NKC_SCEN_GAUNTLET_LEAGUE_ROOM Get_NKC_SCEN_GAUNTLET_LEAGUE_ROOM()
	{
		return m_NKC_SCEN_GAUNTLET_LEAGUE_ROOM;
	}

	public NKC_SCEN_GAUNTLET_PRIVATE_ROOM Get_NKC_SCEN_GAUNTLET_PRIVATE_ROOM()
	{
		return m_NKC_SCEN_GAUNTLET_PRIVATE_ROOM;
	}

	public NKC_SCEN_GAUNTLET_EVENT_READY GET_NKC_SCEN_GAUNTLET_EVENT_READY()
	{
		return m_NKC_SCEN_GAUNTLET_EVENT_READY;
	}

	public NKC_SCEN_RAID Get_NKC_SCEN_RAID()
	{
		return m_NKC_SCEN_RAID;
	}

	public NKC_SCEN_RAID_READY Get_NKC_SCEN_RAID_READY()
	{
		return m_NKC_SCEN_RAID_READY;
	}

	public NKC_SCEN_SHADOW_PALACE Get_NKC_SCEN_SHADOW_PALACE()
	{
		return m_NKC_SCEN_SHADOW_PALACE;
	}

	public NKC_SCEN_SHADOW_BATTLE Get_NKC_SCEN_SHADOW_BATTLE()
	{
		return m_NKC_SCEN_SHADOW_BATTLE;
	}

	public NKC_SCEN_SHADOW_RESULT Get_NKC_SCEN_SHADOW_RESULT()
	{
		return m_NKC_SCEN_SHADOW_RESULT;
	}

	public NKC_SCEN_GUILD_INTRO Get_NKC_SCEN_GUILD_INTRO()
	{
		return m_NKC_SCEN_GUILD_INTRO;
	}

	public NKC_SCEN_GUILD_LOBBY Get_NKC_SCEN_GUILD_LOBBY()
	{
		return m_NKC_SCEN_GUILD_LOBBY;
	}

	public NKC_SCEN_GUILD_COOP Get_NKC_SCEN_GUILD_COOP()
	{
		return m_NKC_SCEN_GUILD_COOP;
	}

	public NKC_SCEN_OFFICE Get_NKC_SCEN_OFFICE()
	{
		return m_NKC_SCEN_OFFICE;
	}

	public NKC_SCEN_TRIM Get_NKC_SCEN_TRIM()
	{
		return m_NKC_SCEN_TRIM;
	}

	public NKC_SCEN_TRIM_RESULT Get_NKC_SCEN_TRIM_RESULT()
	{
		return m_NKC_SCEN_TRIM_RESULT;
	}

	public NKC_SCEN_DUNGEON_RESULT GET_NKC_SCEN_DUNGEON_RESULT()
	{
		return m_NKC_SCEN_DUNGEON_RESULT;
	}

	public NKC_SCEN_FIERCE_BATTLE_SUPPORT Get_NKC_SCEN_FIERCE_BATTLE_SUPPORT()
	{
		return m_NKC_SCEN_FIERCE_BATTLE_SUPPORT;
	}

	public NKMUserData GetMyUserData()
	{
		return m_MyUserData;
	}

	public void SetMyUserData(NKMUserData cNKMUserData)
	{
		m_MyUserData = cNKMUserData;
		m_MyUserData.m_ArmyData.SetOwner(cNKMUserData);
		NKCUIManager.RegisterUICallback(m_MyUserData);
		NKCContentManager.RegisterCallback(m_MyUserData);
		NKCCompanyBuffManager.RegisterCallback(m_MyUserData);
		m_MyUserData.m_LastDiveHistoryData = new HashSet<int>(m_MyUserData.m_DiveHistoryData);
	}

	public static NKMUserData CurrentUserData()
	{
		if (GetScenManager() != null)
		{
			return GetScenManager().GetMyUserData();
		}
		return null;
	}

	public static NKMArmyData CurrentArmyData()
	{
		if (GetScenManager() == null)
		{
			return null;
		}
		return GetScenManager().GetMyUserData()?.m_ArmyData;
	}

	public void SetWarfareGameData(WarfareGameData warfare)
	{
		WarfareGameData = warfare;
	}

	public NKCReplayMgr GetNKCReplayMgr()
	{
		return m_NKCReplayMgr;
	}

	public NKCContractDataMgr GetNKCContractDataMgr()
	{
		return m_NKCContractDataMgr;
	}

	public NKCFierceBattleSupportDataMgr GetNKCFierceBattleSupportDataMgr()
	{
		return m_NKCFierceBattleSupportDataMgr;
	}

	public NKCRaidDataMgr GetNKCRaidDataMgr()
	{
		return m_NKCRaidDataMgr;
	}

	public NKCRepeatOperaion GetNKCRepeatOperaion()
	{
		return m_NKCRepeatOperaion;
	}

	public NKCPowerSaveMode GetNKCPowerSaveMode()
	{
		return m_NKCPowerSaveMode;
	}

	public NKCSurveyMgr GetNKCSurveyMgr()
	{
		return m_NKCSurveyMgr;
	}

	public NKCGameOptionData GetGameOptionData()
	{
		return m_GameOptionData;
	}

	public NKCEventPassDataManager GetEventPassDataManager()
	{
		return m_NKCEventPassDataManager;
	}

	public bool GetHasTouch(int index)
	{
		return m_bTouch[index];
	}

	public Vector2 GetTouchPos2D(int index)
	{
		return m_TouchPos2D[index];
	}

	public bool GetHasPinch()
	{
		return m_bHasPinch;
	}

	public float GetPinchDeltaMagnitude()
	{
		return m_fPinchDeltaMagnitude;
	}

	public void ResetPinchDeltaMagnitude()
	{
		m_fPinchDeltaMagnitude = 0f;
	}

	public float GetPinchDeltaRotation()
	{
		return m_fPinchDeltaRotation;
	}

	public Vector2 GetPinchCenter()
	{
		return m_vPinchCenter;
	}

	public GameObject Get_NKM_NEW_INSTANT()
	{
		return m_NKM_NEW_INSTANT;
	}

	public NKCEffectManager GetEffectManager()
	{
		return m_NKCEffectManager;
	}

	public RectTransform Get_NUF_AFTER_UI_EFFECT()
	{
		return m_rtNUF_AFTER_UI_EFFECT;
	}

	public NKMObjectPool GetObjectPool()
	{
		return GetGameClient().GetObjectPool();
	}

	public void SetSkipScenChangeFadeOutEffect(bool bSet)
	{
		m_bSkipScenChangeFadeOutEffect = bSet;
	}

	public void SetLanguage()
	{
		if (!m_bSetLanguage)
		{
			m_bSetLanguage = true;
			NKM_NATIONAL_CODE nKM_NATIONAL_CODE = NKM_NATIONAL_CODE.NNC_KOREA;
			NKCGameOptionData gameOptionData = GetGameOptionData();
			if (gameOptionData != null)
			{
				nKM_NATIONAL_CODE = gameOptionData.NKM_NATIONAL_CODE;
			}
			NKCStringTable.LoadFromLUA(nKM_NATIONAL_CODE);
			NKC_VOICE_CODE nKC_VOICE_CODE = NKCUIVoiceManager.LoadLocalVoiceCode();
			NKCUIVoiceManager.SetVoiceCode(nKC_VOICE_CODE);
			AssetBundleManager.ActiveVariants = NKCLocalization.GetVariants(nKM_NATIONAL_CODE, nKC_VOICE_CODE);
		}
	}

	private void Awake()
	{
		if (NKCPublisherModule.ApplyCultureInfo())
		{
			CultureInfo currentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = currentCulture;
		}
		Debug.unityLogger.logEnabled = NKCDefineManager.DEFINE_UNITY_DEBUG_LOG();
		if (m_ScenManager == null)
		{
			AssetBundleManager.ActiveVariants = new string[1] { "asset" };
			NKCAssetResourceManager.Init();
			m_GameOptionData.LoadLocal();
			SetLanguage();
			m_ScenManager = this;
			if (!m_bApplicationDelegateRegistered)
			{
				Application.lowMemory += OnLowMemory;
				Application.logMessageReceived += OnLogReceived;
				m_bApplicationDelegateRegistered = true;
			}
			m_SystemMemorySize = SystemInfo.systemMemorySize;
			Log.Info($"SystemInfo.systemMemorySize: {m_SystemMemorySize}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCScenManager.cs", 370);
			Screen.sleepTimeout = -1;
			Application.targetFrameRate = 30;
			Shader.SetGlobalFloat("_FxGlobalTransparency", 0f);
			Shader.SetGlobalFloat("_FxGlobalTransparencyEnemy", 0f);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (!m_UIInit)
		{
			NKMProfiler.SetProvider(new ProfilerProvider());
			PacketController.Instance.Initialize();
			m_NKM_NEW_INSTANT = GameObject.Find("NKM_NEW_INSTANT");
			m_NKM_NEW_INSTANT.SetActive(value: false);
			m_rtNUF_AFTER_UI_EFFECT = GameObject.Find("NUF_AFTER_UI_EFFECT")?.GetComponent<RectTransform>();
			NKCPacketObjectPool.Init();
			NKCMessage.Init();
			NKCMain.NKCInit();
			m_NKCEffectManager.Init();
			m_NKCEffectManager.LoadFromLUA("LUA_EFFECT_POOL");
			NKCSoundManager.Init();
			NKCSoundManager.LoadFromLUA("LUA_SCEN_MUSIC");
			NKCSoundManager.SetAllVolume(m_GameOptionData.GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.ALL));
			NKCSoundManager.SetMusicVolume(m_GameOptionData.GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.BGM));
			NKCSoundManager.SetSoundVolume(m_GameOptionData.GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.SE));
			NKCSoundManager.SetVoiceVolume(m_GameOptionData.GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.VOICE));
			NKCUIVoiceManager.Init();
			NKCVoiceTimingManager.LoadFromLua("LUA_UNIT_VOICE_TEMPLET");
			NKCCamera.Init();
			InitTextureCamera();
			NKCCamera.SetBloomEnableUI(m_GameOptionData.UseCommonEffect);
			if (QualitySettings.GetQualityLevel() != (int)m_GameOptionData.QualityLevel)
			{
				QualitySettings.SetQualityLevel((int)m_GameOptionData.QualityLevel, applyExpensiveChanges: true);
			}
			m_NKCGameClient = new NKCGameClient();
			NKMAttendanceManager.Init(NKCSynchronizedTime.GetServerUTCTime());
			Debug.Log("UIManager Init");
			NKCUIManager.Init();
			Debug.Log("Loading UI Open");
			m_UIInit = true;
			Debug.Log("Scen Build");
			m_NKC_SCEN_LOGIN = new NKC_SCEN_LOGIN();
			m_dicSCEN.Add(m_NKC_SCEN_LOGIN.Get_NKM_SCEN_ID(), m_NKC_SCEN_LOGIN);
			m_NKC_SCEN_HOME = new NKC_SCEN_HOME();
			m_dicSCEN.Add(m_NKC_SCEN_HOME.Get_NKM_SCEN_ID(), m_NKC_SCEN_HOME);
			m_NKC_SCEN_GAME = new NKC_SCEN_GAME();
			m_dicSCEN.Add(m_NKC_SCEN_GAME.Get_NKM_SCEN_ID(), m_NKC_SCEN_GAME);
			m_NKC_SCEN_TEAM = new NKC_SCEN_TEAM();
			m_dicSCEN.Add(m_NKC_SCEN_TEAM.Get_NKM_SCEN_ID(), m_NKC_SCEN_TEAM);
			m_NKC_SCEN_BASE = new NKC_SCEN_BASE();
			m_dicSCEN.Add(m_NKC_SCEN_BASE.Get_NKM_SCEN_ID(), m_NKC_SCEN_BASE);
			m_NKC_SCEN_CONTRACT = new NKC_SCEN_CONTRACT();
			m_dicSCEN.Add(m_NKC_SCEN_CONTRACT.Get_NKM_SCEN_ID(), m_NKC_SCEN_CONTRACT);
			m_NKC_SCEN_INVENTORY = new NKC_SCEN_INVENTORY();
			m_dicSCEN.Add(m_NKC_SCEN_INVENTORY.Get_NKM_SCEN_ID(), m_NKC_SCEN_INVENTORY);
			m_NKC_SCEN_CUTSCEN_SIM = new NKC_SCEN_CUTSCEN_SIM();
			m_dicSCEN.Add(m_NKC_SCEN_CUTSCEN_SIM.Get_NKM_SCEN_ID(), m_NKC_SCEN_CUTSCEN_SIM);
			m_NKC_SCEN_OPERATION_V2 = new NKC_SCEN_OPERATION_V2();
			m_dicSCEN.Add(m_NKC_SCEN_OPERATION_V2.Get_NKM_SCEN_ID(), m_NKC_SCEN_OPERATION_V2);
			m_NKC_SCEN_DUNGEON_ATK_READY = new NKC_SCEN_DUNGEON_ATK_READY();
			m_dicSCEN.Add(m_NKC_SCEN_DUNGEON_ATK_READY.Get_NKM_SCEN_ID(), m_NKC_SCEN_DUNGEON_ATK_READY);
			m_NKC_SCEN_UNIT_LIST = new NKC_SCEN_UNIT_LIST();
			m_dicSCEN.Add(m_NKC_SCEN_UNIT_LIST.Get_NKM_SCEN_ID(), m_NKC_SCEN_UNIT_LIST);
			m_NKC_SCEN_COLLECTION = new NKC_SCEN_COLLECTION();
			m_dicSCEN.Add(m_NKC_SCEN_COLLECTION.Get_NKM_SCEN_ID(), m_NKC_SCEN_COLLECTION);
			m_NKC_SCEN_WARFARE_GAME = new NKC_SCEN_WARFARE_GAME();
			m_dicSCEN.Add(m_NKC_SCEN_WARFARE_GAME.Get_NKM_SCEN_ID(), m_NKC_SCEN_WARFARE_GAME);
			m_NKC_SCEN_SHOP = new NKC_SCEN_SHOP();
			m_dicSCEN.Add(m_NKC_SCEN_SHOP.Get_NKM_SCEN_ID(), m_NKC_SCEN_SHOP);
			m_NKC_SCEN_FRIEND = new NKC_SCEN_FRIEND();
			m_dicSCEN.Add(m_NKC_SCEN_FRIEND.Get_NKM_SCEN_ID(), m_NKC_SCEN_FRIEND);
			m_NKC_SCEN_WORLDMAP = new NKC_SCEN_WORLDMAP();
			m_dicSCEN.Add(m_NKC_SCEN_WORLDMAP.Get_NKM_SCEN_ID(), m_NKC_SCEN_WORLDMAP);
			m_NKC_SCEN_CUTSCEN_DUNGEON = new NKC_SCEN_CUTSCEN_DUNGEON();
			m_dicSCEN.Add(m_NKC_SCEN_CUTSCEN_DUNGEON.Get_NKM_SCEN_ID(), m_NKC_SCEN_CUTSCEN_DUNGEON);
			m_NKC_SCEN_GAME_RESULT = new NKC_SCEN_GAME_RESULT();
			m_dicSCEN.Add(m_NKC_SCEN_GAME_RESULT.Get_NKM_SCEN_ID(), m_NKC_SCEN_GAME_RESULT);
			m_NKC_SCEN_DIVE_READY = new NKC_SCEN_DIVE_READY();
			m_dicSCEN.Add(m_NKC_SCEN_DIVE_READY.Get_NKM_SCEN_ID(), m_NKC_SCEN_DIVE_READY);
			m_NKC_SCEN_DIVE = new NKC_SCEN_DIVE();
			m_dicSCEN.Add(m_NKC_SCEN_DIVE.Get_NKM_SCEN_ID(), m_NKC_SCEN_DIVE);
			m_NKC_SCEN_DIVE_RESULT = new NKC_SCEN_DIVE_RESULT();
			m_dicSCEN.Add(m_NKC_SCEN_DIVE_RESULT.Get_NKM_SCEN_ID(), m_NKC_SCEN_DIVE_RESULT);
			m_NKC_SCEN_GAUNTLET_INTRO = new NKC_SCEN_GAUNTLET_INTRO();
			m_dicSCEN.Add(m_NKC_SCEN_GAUNTLET_INTRO.Get_NKM_SCEN_ID(), m_NKC_SCEN_GAUNTLET_INTRO);
			m_NKC_SCEN_GAUNTLET_LOBBY = new NKC_SCEN_GAUNTLET_LOBBY();
			m_dicSCEN.Add(m_NKC_SCEN_GAUNTLET_LOBBY.Get_NKM_SCEN_ID(), m_NKC_SCEN_GAUNTLET_LOBBY);
			m_NKC_SCEN_GAUNTLET_MATCH_READY = new NKC_SCEN_GAUNTLET_MATCH_READY();
			m_dicSCEN.Add(m_NKC_SCEN_GAUNTLET_MATCH_READY.Get_NKM_SCEN_ID(), m_NKC_SCEN_GAUNTLET_MATCH_READY);
			m_NKC_SCEN_GAUNTLET_MATCH = new NKC_SCEN_GAUNTLET_MATCH();
			m_dicSCEN.Add(m_NKC_SCEN_GAUNTLET_MATCH.Get_NKM_SCEN_ID(), m_NKC_SCEN_GAUNTLET_MATCH);
			m_NKC_SCEN_GAUNTLET_ASYNC_READY = new NKC_SCEN_GAUNTLET_ASYNC_READY();
			m_dicSCEN.Add(m_NKC_SCEN_GAUNTLET_ASYNC_READY.Get_NKM_SCEN_ID(), m_NKC_SCEN_GAUNTLET_ASYNC_READY);
			m_NKC_SCEN_GAUNTLET_LEAGUE_ROOM = new NKC_SCEN_GAUNTLET_LEAGUE_ROOM();
			m_dicSCEN.Add(m_NKC_SCEN_GAUNTLET_LEAGUE_ROOM.Get_NKM_SCEN_ID(), m_NKC_SCEN_GAUNTLET_LEAGUE_ROOM);
			m_NKC_SCEN_GAUNTLET_PRIVATE_ROOM = new NKC_SCEN_GAUNTLET_PRIVATE_ROOM();
			m_dicSCEN.Add(m_NKC_SCEN_GAUNTLET_PRIVATE_ROOM.Get_NKM_SCEN_ID(), m_NKC_SCEN_GAUNTLET_PRIVATE_ROOM);
			m_NKC_SCEN_GAUNTLET_EVENT_READY = new NKC_SCEN_GAUNTLET_EVENT_READY();
			m_dicSCEN.Add(m_NKC_SCEN_GAUNTLET_EVENT_READY.Get_NKM_SCEN_ID(), m_NKC_SCEN_GAUNTLET_EVENT_READY);
			m_NKC_SCEN_RAID = new NKC_SCEN_RAID();
			m_dicSCEN.Add(m_NKC_SCEN_RAID.Get_NKM_SCEN_ID(), m_NKC_SCEN_RAID);
			m_NKC_SCEN_RAID_READY = new NKC_SCEN_RAID_READY();
			m_dicSCEN.Add(m_NKC_SCEN_RAID_READY.Get_NKM_SCEN_ID(), m_NKC_SCEN_RAID_READY);
			m_NKC_SCEN_VOICE_LIST = new NKC_SCEN_VOICE_LIST();
			m_dicSCEN.Add(m_NKC_SCEN_VOICE_LIST.Get_NKM_SCEN_ID(), m_NKC_SCEN_VOICE_LIST);
			m_NKC_SCEN_SHADOW_PALACE = new NKC_SCEN_SHADOW_PALACE();
			m_dicSCEN.Add(m_NKC_SCEN_SHADOW_PALACE.Get_NKM_SCEN_ID(), m_NKC_SCEN_SHADOW_PALACE);
			m_NKC_SCEN_SHADOW_BATTLE = new NKC_SCEN_SHADOW_BATTLE();
			m_dicSCEN.Add(m_NKC_SCEN_SHADOW_BATTLE.Get_NKM_SCEN_ID(), m_NKC_SCEN_SHADOW_BATTLE);
			m_NKC_SCEN_SHADOW_RESULT = new NKC_SCEN_SHADOW_RESULT();
			m_dicSCEN.Add(m_NKC_SCEN_SHADOW_RESULT.Get_NKM_SCEN_ID(), m_NKC_SCEN_SHADOW_RESULT);
			m_NKC_SCEN_GUILD_INTRO = new NKC_SCEN_GUILD_INTRO();
			m_dicSCEN.Add(m_NKC_SCEN_GUILD_INTRO.Get_NKM_SCEN_ID(), m_NKC_SCEN_GUILD_INTRO);
			m_NKC_SCEN_GUILD_LOBBY = new NKC_SCEN_GUILD_LOBBY();
			m_dicSCEN.Add(m_NKC_SCEN_GUILD_LOBBY.Get_NKM_SCEN_ID(), m_NKC_SCEN_GUILD_LOBBY);
			m_NKC_SCEN_FIERCE_BATTLE_SUPPORT = new NKC_SCEN_FIERCE_BATTLE_SUPPORT();
			m_dicSCEN.Add(m_NKC_SCEN_FIERCE_BATTLE_SUPPORT.Get_NKM_SCEN_ID(), m_NKC_SCEN_FIERCE_BATTLE_SUPPORT);
			m_NKC_SCEN_GUILD_COOP = new NKC_SCEN_GUILD_COOP();
			m_dicSCEN.Add(m_NKC_SCEN_GUILD_COOP.Get_NKM_SCEN_ID(), m_NKC_SCEN_GUILD_COOP);
			m_NKC_SCEN_OFFICE = new NKC_SCEN_OFFICE();
			m_dicSCEN.Add(m_NKC_SCEN_OFFICE.Get_NKM_SCEN_ID(), m_NKC_SCEN_OFFICE);
			m_NKC_SCEN_TRIM = new NKC_SCEN_TRIM();
			m_dicSCEN.Add(m_NKC_SCEN_TRIM.Get_NKM_SCEN_ID(), m_NKC_SCEN_TRIM);
			m_NKC_SCEN_TRIM_RESULT = new NKC_SCEN_TRIM_RESULT();
			m_dicSCEN.Add(m_NKC_SCEN_TRIM_RESULT.Get_NKM_SCEN_ID(), m_NKC_SCEN_TRIM_RESULT);
			m_NKC_SCEN_DUNGEON_RESULT = new NKC_SCEN_DUNGEON_RESULT();
			m_dicSCEN.Add(m_NKC_SCEN_DUNGEON_RESULT.Get_NKM_SCEN_ID(), m_NKC_SCEN_DUNGEON_RESULT);
			Debug.Log("Scen Build Complete");
			m_NKM_UI_FPS = GameObject.Find("NKM_UI_FPS");
			if (m_NKM_UI_FPS != null)
			{
				UnityEngine.Object.Destroy(m_NKM_UI_FPS);
				m_NKM_UI_FPS = null;
			}
			NKCLogManager.Init();
			Debug.Log("[Game] Logmanager Init");
			Debug.Log("[Game] Aplication Version [" + Application.version + "]");
			Debug.Log("[Game] Aplication ProductName [" + Application.productName + "]");
			Debug.Log($"[Game] Aplication Platform [{Application.platform}]");
			Debug.Log("[Game] Aplication DataPath [" + Application.dataPath + "]");
			Debug.Log("[Game] Aplication persistentDataPath [" + Application.persistentDataPath + "]");
			NKCMMPManager.Init();
			NKMPopUpBox.Init();
			Debug.Log("Obj Pool Init");
			GetObjectPool().Init();
			Debug.Log("Power Save Mode Init");
			InitPowerSaveMode();
			Debug.Log("Move to Login Scene");
			GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
			Debug.Log("GameOptionData load");
			NKCGameOptionData gameOptionData = GetGameOptionData();
			if (gameOptionData != null)
			{
				Application.targetFrameRate = gameOptionData.GetFrameLimit();
			}
			if (NKCPublisherModule.PublisherType == NKCPublisherModule.ePublisherType.NexonPC)
			{
				m_NKCHealthyGame = new NKCHealthyGame();
				m_NKCHealthyGame.Start();
			}
			Debug.Log("Init Done!");
		}
	}

	public void AdMobInitializeProcess()
	{
		NKMPopUpBox.OpenWaitBox();
		InitADBase(bAskUserConsent: true);
	}

	private void InitADBase(bool bAskUserConsent)
	{
		Debug.Log("Init NKCAdBase");
		NKCAdBase.InitInstance(bAskUserConsent);
		NKMPopUpBox.CloseWaitBox();
	}

	private void InitPowerSaveMode()
	{
		GetNKCPowerSaveMode().SetTurnOnEvent(delegate
		{
			NKCUIManager.GetNKCUIPowerSaveMode().Open();
			NKCUIManager.GetNKCUIPowerSaveMode().SetBlackScreen(bSet: false);
			NKCCamera.GetCamera().enabled = true;
			NKCCamera.GetSubUICamera().enabled = true;
			NKCCamera.SetBlackCameraEnable(value: false);
			NKCCamera.GetSubUICameraVideoPlayer().SetCamera(null);
		});
		GetNKCPowerSaveMode().SetTurnOffEvent(delegate
		{
			NKCUIManager.GetNKCUIPowerSaveMode().Close();
			NKCCamera.GetSubUICameraVideoPlayer().SetCamera(NKCCamera.GetSubUICamera());
		});
		GetNKCPowerSaveMode().SetMaxModeEvent(delegate
		{
			NKCUIManager.GetNKCUIPowerSaveMode().SetBlackScreen(bSet: true);
		});
		GetNKCPowerSaveMode().SetMaxModeNextFrameEvent(delegate
		{
			NKCCamera.GetCamera().enabled = false;
			NKCCamera.GetSubUICamera().enabled = false;
			NKCCamera.SetBlackCameraEnable(value: true);
		});
	}

	private void Update()
	{
		try
		{
			m_fFPSTime += (Time.deltaTime - m_fFPSTime) * 0.1f;
			NKCLogManager.Update();
			if (!m_UIInit)
			{
				return;
			}
			NKCSynchronizedTime.Update(Time.unscaledDeltaTime);
			NKCCamera.Update(Time.deltaTime);
			NKCAssetResourceManager.Update();
			NKCMailManager.Update(Time.deltaTime);
			NKCCompanyBuffManager.Update(Time.deltaTime);
			if (m_NKCHealthyGame != null)
			{
				m_NKCHealthyGame.Update();
			}
			if (m_NKM_SCEN_NOW != null)
			{
				try
				{
					switch (m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE())
					{
					case NKC_SCEN_STATE.NSS_DATA_REQ_WAIT:
						ScenIsDataReqWait();
						break;
					case NKC_SCEN_STATE.NSS_LOADING_UI:
						ScenIsLoadingUI();
						break;
					case NKC_SCEN_STATE.NSS_LOADING_UI_COMPLETE_WAIT:
						ScenChangeUICompleteWait();
						break;
					case NKC_SCEN_STATE.NSS_LOADING:
						ScenIsLoading();
						break;
					case NKC_SCEN_STATE.NSS_LOADING_LAST:
						ScenIsLoadingLast();
						break;
					case NKC_SCEN_STATE.NSS_LOADING_COMPLETE_WAIT:
						ScenChangeCompleteWait();
						break;
					case NKC_SCEN_STATE.NSS_LOADING_COMPLETE:
						ScenChangeComplete();
						m_bScenChanging = false;
						break;
					case NKC_SCEN_STATE.NSS_START:
						m_NKM_SCEN_NOW.ScenUpdate();
						break;
					case NKC_SCEN_STATE.NSS_FAIL:
						ScenLoadFailed();
						break;
					case NKC_SCEN_STATE.NSS_END:
						break;
					}
				}
				catch (Exception ex) when (m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE() != NKC_SCEN_STATE.NSS_START)
				{
					Debug.LogError("LoadGame Failed : Exception " + ex.Message + "\n" + ex.StackTrace);
					m_NKM_SCEN_NOW.Set_NKC_SCEN_STATE(NKC_SCEN_STATE.NSS_INVALID);
					NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_GAME_LOAD_FAILED, delegate
					{
						Application.Quit();
					});
					NKCPopupOKCancel.SetOnTop();
				}
			}
			ScenChangeFadeUpdate();
			GetObjectPool().Update();
			NKCUIManager.Update(Time.deltaTime);
			NKCPopupMessageManager.Update(Time.deltaTime);
			NKCSoundManager.Update(Time.deltaTime);
			m_NKCEffectManager.Update(Time.deltaTime);
			NKCMessage.Update();
			NKCLocalServerManager.Update(Time.deltaTime);
			m_NKCConnectLogin.Update();
			m_NKCConnectGame.Update();
			AppEnableConnectCheck();
			NKCGameEventManager.Update(Time.deltaTime);
			NKCLoginCutSceneManager.Update();
			NKCLoadingScreenManager.Update();
			GetTouch();
			if (GetNowScenID() != NKM_SCEN_ID.NSI_GAME)
			{
				for (int num = 0; num < m_bTouch.Length; num++)
				{
					if (m_bTouch[num] && m_rtNUF_AFTER_UI_EFFECT != null && m_rtNUF_AFTER_UI_EFFECT.gameObject.activeInHierarchy)
					{
						RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rtNUF_AFTER_UI_EFFECT, m_TouchPos2D[num], NKCCamera.GetSubUICamera(), out var worldPoint);
						worldPoint.x /= m_rtNUF_AFTER_UI_EFFECT.lossyScale.x;
						worldPoint.y /= m_rtNUF_AFTER_UI_EFFECT.lossyScale.y;
						m_NKCEffectManager.UseEffect(0, "AB_fx_ui_touch", "AB_fx_ui_touch", NKM_EFFECT_PARENT_TYPE.NEPT_NUF_AFTER_UI_EFFECT, worldPoint.x, worldPoint.y, m_TouchPos3D[num].z + 10f, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "BASE");
					}
				}
			}
			NKCAlarmManager.Update(Time.deltaTime);
			UpdatePowerSaveMode();
		}
		catch (Exception ex2)
		{
			Debug.LogErrorFormat("NKCScenManager Update {0}", ex2.Message);
			Debug.LogErrorFormat("NKCScenManager Update {0}", ex2.StackTrace);
			int num2 = 1;
			Exception innerException = ex2.InnerException;
			while (innerException != null)
			{
				Debug.LogErrorFormat("NKCScenManager Update InnerException depth[{0}] {1}", num2, innerException);
				Debug.LogErrorFormat("NKCScenManager Update InnerException depth[{0}] {1}", num2, innerException.StackTrace);
				innerException = innerException.InnerException;
				num2++;
			}
			ScenLoadFailed();
		}
	}

	private void InitTextureCamera()
	{
		if (TextureCamera == null)
		{
			m_objTextureCamera = new GameObject("TextureImageCamera");
			if (GetScenManager() != null)
			{
				m_objTextureCamera.transform.SetParent(base.transform, worldPositionStays: false);
			}
			m_objTextureCamera.SetActive(value: false);
			TextureCamera = m_objTextureCamera.AddComponent<Camera>();
			TextureCamera.enabled = false;
			TextureCamera.orthographic = true;
			TextureCamera.cullingMask = int.MinValue;
			TextureCamera.clearFlags = CameraClearFlags.Color;
			TextureCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		}
	}

	public void TextureCapture(Renderer targetRenderer, Bounds bound, ref RenderTexture Texture)
	{
		if (bound.size.x != 0f && bound.size.y != 0f)
		{
			Vector3 vector = bound.center - bound.extents;
			Vector3 vector2 = bound.center + bound.extents;
			NKCCamera.FitCameraToWorldRect(WorldCornerPosArray: new Vector3[4]
			{
				vector,
				new Vector3(vector.x, vector2.y, vector.z),
				vector2,
				new Vector3(vector2.x, vector.y, vector.z)
			}, camera: TextureCamera);
			int layer = targetRenderer.gameObject.layer;
			targetRenderer.gameObject.layer = 31;
			TextureCamera.targetTexture = Texture;
			TextureCamera.Render();
			targetRenderer.gameObject.layer = layer;
			TextureCamera.targetTexture = null;
		}
	}

	public void ForceRender(Renderer targetRenderer)
	{
		if (!(targetRenderer == null))
		{
			Bounds bounds = targetRenderer.bounds;
			if (m_tempTexture == null)
			{
				m_tempTexture = new RenderTexture(128, 128, 0, RenderTextureFormat.ARGB32);
				m_tempTexture.wrapMode = TextureWrapMode.Clamp;
				m_tempTexture.antiAliasing = 1;
				m_tempTexture.filterMode = FilterMode.Bilinear;
				m_tempTexture.anisoLevel = 0;
				m_tempTexture.Create();
			}
			TextureCapture(targetRenderer, bounds, ref m_tempTexture);
		}
	}

	public void ForceRender(Texture targetTexture)
	{
		if (!(targetTexture == null))
		{
			if (tempMeshRenderer == null)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
				gameObject.name = "LoadingTempMeshRenderer";
				gameObject.transform.localScale = new Vector3(256f, 256f, 1f);
				tempMeshRenderer = gameObject.GetComponent<MeshRenderer>();
				NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<Material>("AB_MATERIAL", "MAT_NKC_AFTERIMAGE");
				tempMeshRenderer.material = nKCAssetResourceData.GetAsset<Material>();
			}
			tempMeshRenderer.gameObject.SetActive(value: true);
			tempMeshRenderer.material.mainTexture = targetTexture;
			ForceRender(tempMeshRenderer);
			tempMeshRenderer.gameObject.SetActive(value: false);
			tempMeshRenderer.material.mainTexture = null;
		}
	}

	public void PreloadSprite(string bundleName, string assetName)
	{
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(bundleName, assetName);
		if (!(orLoadAssetResource == null))
		{
			ForceRender(orLoadAssetResource.texture);
		}
	}

	public void PreloadTexture(string bundleName, string assetName)
	{
		Texture orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Texture>(bundleName, assetName);
		if (!(orLoadAssetResource == null))
		{
			ForceRender(orLoadAssetResource);
		}
	}

	private void UpdatePowerSaveMode()
	{
		if (Input.anyKey)
		{
			m_NKCPowerSaveMode.SetLastKeyInputTime(Time.time);
		}
		m_NKCPowerSaveMode.Update(Time.time);
	}

	private void FPSView()
	{
		if (m_NKM_UI_FPS.activeSelf)
		{
			m_NKM_UI_FPS.SetActive(value: false);
		}
	}

	public void SetActionAfterScenChange(DoAfterScenChange action)
	{
		dDoAfterScenChange = action;
	}

	public void SetActiveLoadingUI(NKCLoadingScreenManager.eGameContentsType contentType, int contentValue = 0)
	{
		NKCUIManager.LoadingUI.ShowMainLoadingUI(contentType, contentValue);
	}

	public void CloseLoadingUI()
	{
		NKCUIManager.LoadingUI.CloseLoadingUI();
	}

	private IEnumerator CoroutineScenChangeFade()
	{
		while (!NKCUIFadeInOut.IsFinshed())
		{
			yield return null;
		}
		ScenChangeImpl(m_eNextScen);
		m_eNextScen = NKM_SCEN_ID.NSI_INVALID;
	}

	public void ScenReload()
	{
		ScenChangeImpl(m_NKM_SCEN_NOW.Get_NKM_SCEN_ID());
	}

	private void ScenChangeImpl(NKM_SCEN_ID eScenID)
	{
		if (!m_dicSCEN.ContainsKey(eScenID))
		{
			Debug.LogErrorFormat("m_dicSCEN has NO eScenID: {0}", eScenID.ToString());
			return;
		}
		Debug.Log("ScenChangeImpl Begin");
		if (m_NKM_SCEN_NOW != null)
		{
			m_NKM_SCEN_NOW.ScenEnd();
			m_BeforeScenID = m_NKM_SCEN_NOW.Get_NKM_SCEN_ID();
		}
		Debug.Log("ScenChangeImpl ScenEnd Complete");
		m_NKM_SCEN_NOW = m_dicSCEN[eScenID];
		if (eScenID == NKM_SCEN_ID.NSI_GAME)
		{
			NKCUIManager.OnScenEnd(NKCUIManager.eUIUnloadFlag.ON_PLAY_GAME);
		}
		else
		{
			NKCUIManager.OnScenEnd(NKCUIManager.eUIUnloadFlag.DEFAULT);
			if (tempMeshRenderer != null)
			{
				UnityEngine.Object.Destroy(tempMeshRenderer.gameObject);
				tempMeshRenderer = null;
			}
		}
		Debug.Log("ScenChangeImpl : Unload All UI Complete");
		m_NKM_SCEN_NOW.ScenChangeStart();
	}

	public bool CheckSceneChangeEnabled(NKM_SCEN_ID eScenID)
	{
		if ((eScenID == NKM_SCEN_ID.NSI_HOME || eScenID == NKM_SCEN_ID.NSI_OPERATION) && NKCPatchUtility.GetDownloadType() == NKCPatchDownloader.DownType.TutorialWithBackground)
		{
			NKCPatchUtility.RemoveDownloadType();
			ShowNeedUpdateAfterTutorial();
			return false;
		}
		return true;
	}

	public void ScenChangeFade(NKM_SCEN_ID eScenID, bool bForce = true)
	{
		if (!CheckSceneChangeEnabled(eScenID))
		{
			return;
		}
		if (GetNowScenID() == eScenID && !bForce)
		{
			dDoAfterScenChange?.Invoke();
			dDoAfterScenChange = null;
			return;
		}
		NKCScenChangeOrder nKCScenChangeOrder = new NKCScenChangeOrder();
		nKCScenChangeOrder.m_NextScen = eScenID;
		nKCScenChangeOrder.m_bForce = bForce;
		m_qScenChange.Enqueue(nKCScenChangeOrder);
		if (!m_bSkipScenChangeFadeOutEffect)
		{
			NKCUIFadeInOut.FadeOut(0.1f, null, bWhite: false, 7f);
		}
		m_bSkipScenChangeFadeOutEffect = false;
		NKCUIVoiceManager.StopVoice();
		NKCUIManager.NKCUIOverlayCaption.CloseAllCaption();
	}

	private void ScenChangeFadeUpdate()
	{
		if (m_bScenChanging || m_qScenChange.Count <= 0 || (m_NKM_SCEN_NOW != null && m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE() != NKC_SCEN_STATE.NSS_START))
		{
			return;
		}
		Debug.Log("ScenChangeFadeUpdate Begin");
		NKCScenChangeOrder nKCScenChangeOrder = m_qScenChange.Dequeue();
		if (GetNowScenID() == nKCScenChangeOrder.m_NextScen && !nKCScenChangeOrder.m_bForce)
		{
			return;
		}
		Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.High;
		if (nKCScenChangeOrder.m_NextScen == NKM_SCEN_ID.NSI_GAME)
		{
			Get_SCEN_GAME().ScenClear();
		}
		if (nKCScenChangeOrder.m_NextScen == NKM_SCEN_ID.NSI_GAME || GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKMGameData nKMGameData = GetGameClient().GetGameDataDummy();
			if (nKMGameData == null)
			{
				nKMGameData = GetGameClient().GetGameData();
			}
			if (nKMGameData != null)
			{
				switch (nKMGameData.GetGameType())
				{
				case NKM_GAME_TYPE.NGT_WARFARE:
					NKCUIManager.LoadingUI.ShowMainLoadingUI(NKM_GAME_TYPE.NGT_WARFARE, nKMGameData.m_WarfareID);
					break;
				case NKM_GAME_TYPE.NGT_DIVE:
				{
					NKMDiveGameData nKMDiveGameData = CurrentUserData()?.m_DiveGameData;
					if (nKMDiveGameData != null && nKMDiveGameData.Floor != null && nKMDiveGameData.Floor.Templet != null)
					{
						NKCUIManager.LoadingUI.ShowMainLoadingUI(NKM_GAME_TYPE.NGT_DIVE, nKMDiveGameData.Floor.Templet.StageID);
					}
					else
					{
						NKCUIManager.LoadingUI.ShowMainLoadingUI(NKM_GAME_TYPE.NGT_DIVE, nKMGameData.m_DungeonID);
					}
					break;
				}
				default:
					NKCUIManager.LoadingUI.ShowMainLoadingUI(nKMGameData.GetGameType(), nKMGameData.m_DungeonID);
					break;
				}
			}
			else
			{
				NKCUIManager.LoadingUI.ShowMainLoadingUI();
			}
			ScenChangeImpl(nKCScenChangeOrder.m_NextScen);
			NKCUIFadeInOut.Finish();
		}
		else if (GetNowScenID() == NKM_SCEN_ID.NSI_LOGIN && nKCScenChangeOrder.m_NextScen == NKM_SCEN_ID.NSI_HOME)
		{
			NKCUIManager.LoadingUI.ShowMainLoadingUI();
			ScenChangeImpl(nKCScenChangeOrder.m_NextScen);
			NKCUIFadeInOut.Finish();
		}
		else
		{
			m_eNextScen = nKCScenChangeOrder.m_NextScen;
			StartCoroutine(CoroutineScenChangeFade());
			NKCUIManager.LoadingUI.CloseLoadingUI();
		}
		m_bScenChanging = true;
	}

	public void ScenIsDataReqWait()
	{
		if (m_NKM_SCEN_NOW != null && m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_DATA_REQ_WAIT)
		{
			m_NKM_SCEN_NOW.ScenDataReqWaitUpdate();
		}
	}

	public void ScenIsLoadingUI()
	{
		if (m_NKM_SCEN_NOW != null && m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_LOADING_UI)
		{
			m_NKM_SCEN_NOW.ScenLoadUIUpdate();
		}
	}

	public void ScenChangeUICompleteWait()
	{
		if (m_NKM_SCEN_NOW != null)
		{
			m_NKM_SCEN_NOW.ScenLoadUICompleteWait();
		}
	}

	public void ScenIsLoading()
	{
		if (m_NKM_SCEN_NOW != null && m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_LOADING)
		{
			m_NKM_SCEN_NOW.ScenLoadUpdate();
		}
	}

	public void ScenIsLoadingLast()
	{
		if (m_NKM_SCEN_NOW != null && m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_LOADING_LAST)
		{
			m_NKM_SCEN_NOW.ScenLoadLastUpdate();
		}
	}

	public void ScenChangeCompleteWait()
	{
		if (m_NKM_SCEN_NOW != null)
		{
			m_NKM_SCEN_NOW.ScenLoadCompleteWait();
		}
	}

	public void ScenLoadFailed()
	{
		if (m_NKM_SCEN_NOW == null || m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_START)
		{
			return;
		}
		m_bScenChanging = false;
		m_BeforeScenID = m_NKM_SCEN_NOW.Get_NKM_SCEN_ID();
		m_NKM_SCEN_NOW.Set_NKC_SCEN_STATE(NKC_SCEN_STATE.NSS_START);
		if (m_BeforeScenID == NKM_SCEN_ID.NSI_SHOP)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_SHOP_WAS_NOT_ABLE_TO_GET_PRODUCT_LIST_FROM_SERVER, delegate
			{
				ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCStringTable.GetString("SI_ERROR_DEFAULT_MESSAGE"), delegate
			{
				ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
		}
	}

	public void ScenChangeComplete()
	{
		if (m_NKCMemoryCleaner != null)
		{
			m_NKCMemoryCleaner.DoUnloadUnusedAssetsAndGC();
		}
		NKCUIManager.LoadingUI.CloseLoadingUI();
		m_NKM_SCEN_NOW.ScenStart();
		if (m_CurScenID != m_NKM_SCEN_NOW.Get_NKM_SCEN_ID() && m_BeforeScenID != NKM_SCEN_ID.NSI_INVALID && IsReconnectScen())
		{
			m_CurScenID = m_NKM_SCEN_NOW.Get_NKM_SCEN_ID();
			if (GetConnectGame().IsConnected)
			{
				NKCPacketSender.Send_NKMPacket_UI_SCEN_CHANGED_REQ(m_CurScenID);
			}
			Log.Info("<color=#FFFF00FF>ScenChangeInfo: BeforeScenID : " + m_BeforeScenID.ToString() + " > AfterScenID : " + m_CurScenID.ToString() + "</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCScenManager.cs", 1378);
		}
		NKCUIOverlayPatchProgress.OpenWhenDownloading();
		dDoAfterScenChange?.Invoke();
		dDoAfterScenChange = null;
	}

	private void GetTouch()
	{
		for (int i = 0; i < m_bTouch.Length; i++)
		{
			m_bTouch[i] = false;
			if (Input.touchCount > i && Input.GetTouch(i).phase == TouchPhase.Began)
			{
				m_TouchPos2D[i] = Input.GetTouch(i).position;
				NKCCamera.GetScreenPosToWorldPos(out m_TouchPos3D[i], m_TouchPos2D[i].x, m_TouchPos2D[i].y);
				m_bTouch[i] = true;
			}
			else if (Input.GetMouseButtonDown(i))
			{
				m_TouchPos2D[i] = Input.mousePosition;
				NKCCamera.GetScreenPosToWorldPos(out m_TouchPos3D[i], m_TouchPos2D[i].x, m_TouchPos2D[i].y);
				m_bTouch[i] = true;
			}
		}
		if (Input.touchCount == 2)
		{
			m_bHasPinch = true;
			Touch touch = Input.GetTouch(0);
			Touch touch2 = Input.GetTouch(1);
			Vector2 vector = touch.position - touch.deltaPosition;
			Vector2 vector2 = touch2.position - touch2.deltaPosition;
			Vector2 vector3 = vector - vector2;
			Vector2 to = touch.position - touch2.position;
			float sqrMagnitude = vector3.sqrMagnitude;
			float sqrMagnitude2 = to.sqrMagnitude;
			float num = 1f / (float)(Screen.width * Screen.width);
			m_fPinchDeltaMagnitude = (sqrMagnitude2 - sqrMagnitude) * num;
			m_fPinchDeltaRotation = Vector2.SignedAngle(vector3, to);
			m_vPinchCenter = (touch.position + touch2.position) * 0.5f;
		}
		else
		{
			m_fPinchDeltaMagnitude = 0f;
			m_fPinchDeltaRotation = 0f;
			m_bHasPinch = false;
		}
	}

	public bool MsgProc(NKCMessageData cNKCMessageData)
	{
		switch (cNKCMessageData.m_NKC_EVENT_MESSAGE)
		{
		case NKC_EVENT_MESSAGE.NEM_UI_GAME_QUIT:
			if (Application.platform == RuntimePlatform.Android)
			{
				Application.Quit();
			}
			break;
		case NKC_EVENT_MESSAGE.NEM_NKCPACKET_SEND_TO_SERVER:
			if (NKCLocalServerManager.ScenMsgProc(cNKCMessageData))
			{
				return true;
			}
			break;
		case NKC_EVENT_MESSAGE.NEM_NKCPACKET_SEND_TO_CLIENT:
			if (NKCLocalPacketHandler.ScenMsgProc(cNKCMessageData))
			{
				return true;
			}
			break;
		}
		if (m_NKM_SCEN_NOW != null && m_NKM_SCEN_NOW.ScenMsgProc(cNKCMessageData))
		{
			return true;
		}
		return false;
	}

	private void OnHideUnity(bool isGameShown)
	{
	}

	private void OnLowMemory()
	{
		Debug.LogError("OnLowMemory!!");
		if (m_NKM_SCEN_NOW != null && GetNowScenID() != NKM_SCEN_ID.NSI_GAME && m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_START)
		{
			GetGameClient().Init();
			if (m_NKCMemoryCleaner != null)
			{
				m_NKCMemoryCleaner.Clean(null, NKCUIManager.eUIUnloadFlag.ONLY_MEMORY_SHORTAGE);
			}
		}
	}

	private static void OnLogReceived(string msg, string stackTrace, LogType type)
	{
		if (type == LogType.Exception && NKCUIManager.CheckUIOpenError())
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_UI_LOADING_ERROR);
		}
	}

	private void AppEnableConnectCheck()
	{
		if ((!NKCDefineManager.DEFINE_NX_PC() || GetNowScenID() != NKM_SCEN_ID.NSI_LOGIN) && m_fAppEnableConnectCheckTime != -1f && m_fAppEnableConnectCheckTime > 0f)
		{
			float deltaTime = Time.deltaTime;
			m_fAppEnableConnectCheckTime -= deltaTime;
			if (m_fAppEnableConnectCheckTime <= 0f)
			{
				Log.Info("[AppEnableConnectCheck] VersionCheck", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCScenManager.cs", 1607);
				m_fAppEnableConnectCheckTime = -1f;
				VersionCheck(m_NKCConnectGame.Reconnect);
			}
		}
	}

	public void SetApplicaitonPause(bool bPause)
	{
		OnApplicationPause(bPause);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		Debug.Log("OnApplicationPause " + pauseStatus);
		if (!pauseStatus)
		{
			if (IsReconnectScen() && (GetNowScenID() != NKM_SCEN_ID.NSI_GAME || (GetGameClient().GetGameData() != null && GetGameClient().GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_DEV)))
			{
				NKCPacketSender.Send_NKMPacket_NKMPacket_CONNECT_CHECK_REQ();
				m_fAppEnableConnectCheckTime = 2f;
			}
			if (GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
			{
				GetGameClient().GetGameHud().UseCompleteDeck();
				if (GetGameClient().GetGameData() != null && GetGameClient().GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE)
				{
					GetGameClient().Send_Packet_GAME_CHECK_DIE_UNIT_REQ();
				}
			}
			NKCUIFadeInOut.Close(bDoCallBack: true);
		}
		else if (GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			if (GetGameClient().GetGameData() != null && GetGameClient().GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE)
			{
				Get_SCEN_GAME().TrySendGamePauseEnableREQ();
			}
		}
		else if (GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
		{
			Get_NKC_SCEN_WARFARE_GAME().TryPause();
		}
		else if (GetNowScenID() == NKM_SCEN_ID.NSI_HOME)
		{
			Get_SCEN_HOME().TryPause();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (!NKCUIJukeBox.IsHasInstance || !NKCUIJukeBox.IsInstanceOpen)
		{
			NKCSoundManager.SetMute(GetGameOptionData().SoundMute);
		}
	}

	private void CheckForCameraPermission()
	{
		if (CameraPermissionCoroutine != null)
		{
			StopCoroutine(CameraPermissionCoroutine);
			CameraPermissionCoroutine = null;
		}
		CameraPermissionCoroutine = StartCoroutine(CameraPermission());
	}

	private IEnumerator CameraPermission()
	{
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		if (NKCPublisherModule.Permission != null)
		{
			NKCPublisherModule.Permission.CheckCameraPermission();
		}
	}

	private void OnApplicationQuit()
	{
		Debug.Log("[OnApplicationQuit] NKCScenManager - Quit Start");
		NKCMain.InvalidateSafeMode();
		Debug.Log("[OnApplicationQuit] InvalidSafeMode");
		GetConnectLogin().ResetConnection();
		Debug.Log("[OnApplicationQuit] ResetConnection");
		GetConnectGame().ResetConnection();
		Debug.Log("[OnApplicationQuit] ResetConnection");
		if (NKCPatchDownloader.Instance != null)
		{
			NKCPatchDownloader.Instance.Unload();
			Debug.Log("[OnApplicationQuit] NKCPatchDownloader Unload");
		}
		Debug.Log("[OnApplicationQuit] NKCScenManager - Quit Complete");
	}

	public void DoAfterLogout()
	{
		if (m_NKCRepeatOperaion != null)
		{
			m_NKCRepeatOperaion.Init();
			m_NKCRepeatOperaion.SetAlarmRepeatOperationQuitByDefeat(bSet: false);
			m_NKCRepeatOperaion.SetAlarmRepeatOperationSuccess(bSet: false);
		}
		NKCUIUserInfo.SetComment("");
		GetNKCSurveyMgr().Clear();
		Get_SCEN_HOME().DoAfterLogout();
		Get_NKC_SCEN_WORLDMAP().DoAfterLogout();
		Get_NKC_SCEN_DIVE().DoAfterLogout();
		Get_NKC_SCEN_WARFARE_GAME().DoAfterLogout();
		Get_NKC_SCEN_GAUNTLET_LOBBY().DoAfterLogout();
		Get_SCEN_DUNGEON_ATK_READY().DoAfterLogout();
		Get_NKC_SCEN_DIVE_READY().DoAfterLogout();
		Get_NKC_SCEN_RAID_READY().DoAfterLogout();
		Get_SCEN_OPERATION().DoAfterLogOut();
		GET_SCEN_CONTRACT().DoAfterLogout();
		NKCChatManager.Initialize();
		NKCGuildManager.Initialize();
		NKCGuildCoopManager.Initialize();
		NKCDefenceDungeonManager.Init();
		NKCLeaderBoardManager.Initialize();
		NKCTournamentManager.Initialize();
		NKCPVPManager.Initialize();
		NKMEpisodeMgr.DoAfterLogOut();
		NKCFriendManager.Initialize();
		NKCPhaseManager.Reset();
		NKCTrimManager.Reset();
		NKMItemManager.InitResetCount();
		GetScenManager().SetAppEnableConnectCheckTime(-1f, bForce: true);
		NKCUnitReviewManager.m_bReceivedUnitReviewBanList = false;
		GetConnectGame().SetReconnectKey("");
		NKCPMNexonNGS.SetNpaCode("");
		NKCUIGameOption.InvalidShowHiddenOption();
		NKCUIEventSubUIWechatFollow.DoAfterLogout();
		NKCShopManager.DoAfterLogout();
		NKCEmoticonManager.DoAfterLogout();
		NKCMatchTenManager.DoAfterLogout();
	}

	public bool IsReconnectScen()
	{
		NKM_SCEN_ID nowScenID = GetNowScenID();
		if ((uint)nowScenID <= 1u || nowScenID == NKM_SCEN_ID.NSI_CUTSCENE_SIM || nowScenID == NKM_SCEN_ID.NSI_VOICE_LIST)
		{
			return false;
		}
		return true;
	}

	public void VersionCheck(UnityAction onSuccess)
	{
		if (m_bVersionCheckRunning)
		{
			return;
		}
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_PATCHER_WARNING, NKCUtilString.GET_STRING_DECONNECT_INTERNET, delegate
			{
				GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
			});
		}
		if (NKCPatchDownloader.Instance.ProloguePlay)
		{
			onSuccess?.Invoke();
		}
		else
		{
			StartCoroutine(_VersionCheckAndReconnect(onSuccess));
		}
	}

	private IEnumerator _VersionCheckAndReconnect(UnityAction onSuccess)
	{
		NKMPopUpBox.OpenWaitBox();
		m_bVersionCheckRunning = true;
		Debug.Log("Checking Assetbundle Version");
		NKCPatchDownloader.Instance.CheckVersion(new List<string>(AssetBundleManager.ActiveVariants));
		while (NKCPatchDownloader.Instance.BuildCheckStatus == NKCPatchDownloader.BuildStatus.Unchecked)
		{
			yield return null;
		}
		switch (NKCPatchDownloader.Instance.BuildCheckStatus)
		{
		case NKCPatchDownloader.BuildStatus.RequireAppUpdate:
			m_bVersionCheckRunning = false;
			NKMPopUpBox.CloseWaitBox();
			ShowAppUpdate();
			yield break;
		case NKCPatchDownloader.BuildStatus.UpdateAvailable:
			NKMPopUpBox.CloseWaitBox();
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_PATCHER_CAN_UPDATE, delegate
			{
				m_bVersionCheckRunning = false;
				MoveToMarket();
			}, delegate
			{
				m_bVersionCheckRunning = false;
				onSuccess();
			});
			yield break;
		}
		while (NKCPatchDownloader.Instance.VersionCheckStatus == NKCPatchDownloader.VersionStatus.Unchecked)
		{
			yield return null;
		}
		if (NKCPatchDownloader.Instance.VersionCheckStatus != NKCPatchDownloader.VersionStatus.UpToDate)
		{
			m_bVersionCheckRunning = false;
			NKMPopUpBox.CloseWaitBox();
			ShowBundleUpdate(bCallFromTutorial: false);
			yield break;
		}
		NKMPopUpBox.CloseWaitBox();
		yield return NKCPatcherManager.GetPatcherManager().UpdateServerMaintenanceData();
		if (NKCConnectionInfo.IsServerUnderMaintenance())
		{
			bool bWait = true;
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_PATCHER_NOTICE, NKCStringTable.GetString("SI_SYSTEM_NOTICE_MAINTENANCE_DESC"), delegate
			{
				NKCPopupNoticeWeb.Instance.Open(NKCPublisherModule.Notice.NoticeUrl(bPatcher: true), delegate
				{
					bWait = false;
				}, bPatcher: true);
			});
			while (bWait)
			{
				yield return null;
			}
			m_bVersionCheckRunning = false;
		}
		else
		{
			onSuccess?.Invoke();
			m_bVersionCheckRunning = false;
		}
	}

	public void MoveToPatchScene()
	{
		NKCMain.InvalidateSafeMode();
		m_UIInit = false;
		if (m_NKM_SCEN_NOW != null)
		{
			m_NKM_SCEN_NOW.ScenEnd();
		}
		NKCSoundManager.Unload();
		NKCUIOverlayPatchProgress.CheckInstanceAndClose();
		NKCTempletUtility.CleanupAllTemplets();
		NKCAssetResourceManager.UnloadAllResources();
		AssetBundleManager.UnloadAllAndCleanup();
		if (m_NKCMemoryCleaner != null)
		{
			if (m_NKCMemoryCleaner.m_bWaitingMemCleaning)
			{
				m_NKCMemoryCleaner.WaitForClean(delegate
				{
					m_NKCMemoryCleaner.Clean(delegate
					{
						SceneManager.LoadSceneAsync("NKM_SCEN_PATCHER", LoadSceneMode.Single);
					}, NKCUIManager.eUIUnloadFlag.NEVER_UNLOAD, bDontCloseOpenedUI: false);
				});
			}
			else
			{
				m_NKCMemoryCleaner.Clean(delegate
				{
					SceneManager.LoadSceneAsync("NKM_SCEN_PATCHER", LoadSceneMode.Single);
				}, NKCUIManager.eUIUnloadFlag.NEVER_UNLOAD, bDontCloseOpenedUI: false);
			}
		}
		else
		{
			SceneManager.LoadSceneAsync("NKM_SCEN_PATCHER", LoadSceneMode.Single);
		}
	}

	public void ShowBundleUpdate(bool bCallFromTutorial)
	{
		if (NKCPublisherModule.IsSteamPC())
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REFRESH_DATA, delegate
			{
				Application.Quit();
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REFRESH_DATA, MoveToPatchScene);
		}
	}

	public void ShowNeedUpdateAfterTutorial()
	{
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_PATCHER_NEED_UPDATE_AFTER_TUTORIAL"), MoveToPatchScene);
	}

	private void MoveToMarket()
	{
		if (NKCPatchDownloader.Instance != null)
		{
			NKCPatchDownloader.Instance.MoveToMarket();
		}
		else
		{
			Application.Quit();
		}
	}

	private void ShowAppUpdate()
	{
		m_bVersionCheckRunning = false;
		StopAllCoroutines();
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_PATCHER_NEED_UPDATE, MoveToMarket);
	}

	public void OnLoginReady()
	{
		if (GetConnectLogin() != null)
		{
			GetConnectLogin().AuthToLoginServer();
			NKMPopUpBox.OpenWaitBox();
		}
	}

	public bool ProcessGlobalHotkey(HotkeyEventType eventtype)
	{
		switch (eventtype)
		{
		case HotkeyEventType.Cancel:
			if (!NKMPopUpBox.IsOpenedWaitBox() && !GetNKCPowerSaveMode().GetEnable() && m_NKM_SCEN_NOW != null && m_NKM_SCEN_NOW.Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_START)
			{
				NKCUIManager.OnBackButton();
			}
			return true;
		case HotkeyEventType.ScreenCapture:
			if (NKCScreenCaptureUtility.CaptureScreen())
			{
				NKCSoundManager.PlaySound("FX_CUTSCEN_NOISE_CAMERA", 1f, 0f, 0f);
			}
			return true;
		case HotkeyEventType.Mute:
			m_GameOptionData.SoundMute = !m_GameOptionData.SoundMute;
			m_GameOptionData.Save();
			if (NKCUIGameOption.IsInstanceOpen)
			{
				NKCUIGameOption.Instance.UpdateOptionContent(NKCUIGameOption.GameOptionGroup.Sound);
			}
			break;
		}
		return false;
	}

	public void ProcessGlobalHotkeyHold(HotkeyEventType eventType)
	{
		switch (eventType)
		{
		case HotkeyEventType.MasterVolumeUp:
			m_GameOptionData.ChangeSoundVolume(NKC_GAME_OPTION_SOUND_GROUP.ALL, 2);
			m_GameOptionData.Save();
			NKCSoundManager.SetAllVolume(m_GameOptionData.GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.ALL));
			if (NKCUIGameOption.IsInstanceOpen)
			{
				NKCUIGameOption.Instance.UpdateOptionContent(NKCUIGameOption.GameOptionGroup.Sound);
			}
			break;
		case HotkeyEventType.MasterVolumeDown:
			m_GameOptionData.ChangeSoundVolume(NKC_GAME_OPTION_SOUND_GROUP.ALL, -2);
			m_GameOptionData.Save();
			NKCSoundManager.SetAllVolume(m_GameOptionData.GetSoundVolumeAsFloat(NKC_GAME_OPTION_SOUND_GROUP.ALL));
			if (NKCUIGameOption.IsInstanceOpen)
			{
				NKCUIGameOption.Instance.UpdateOptionContent(NKCUIGameOption.GameOptionGroup.Sound);
			}
			break;
		}
	}
}
