using System.Collections.Generic;
using System.Text;
using Cs.Logging;
using NKC.UI.Component;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCGameHud : MonoBehaviour
{
	public enum HUDMode
	{
		Normal,
		Replay,
		Observer
	}

	public delegate void OnUseDeck(int deckIndex);

	public delegate void OnUseSkill(int skillIndex);

	private NKCGameClient m_GameClient;

	private NKM_TEAM_TYPE m_CurrentViewTeamType;

	[Header("서브모드 관련")]
	public NKCGameHudObserver m_HudObserver;

	public GameObject m_objInactiveUser;

	public Text m_lbInactiveUser;

	[Header("덱")]
	public NKCGameHudDeckSlot[] m_NKCUIHudDeck;

	public GameObject m_objRemainUnitCount;

	public Text m_lbRemainUnitCount;

	public const int NEXT_DECK_INDEX = 4;

	public const int ASSIST_DECK_INDEX = 5;

	[Header("함선 스킬")]
	public NKCUIHudShipSkillDeck[] m_NKCUIHudShipSkillDeck;

	private const int TACTICAL_COMMAND_DECK_COUNT = 1;

	private NKCUIHudTacticalCommandDeck[] m_NKCUIHudTacticalCommandDeck = new NKCUIHudTacticalCommandDeck[1];

	[Header("오브젝트")]
	public GameObject m_objTop;

	public GameObject m_objRootShipSkill;

	public GameObject m_objHUDBg;

	public GameObject m_objHUDBgFx;

	[Header("상부 게이지들")]
	public NKCUIHudRespawnGage m_NKCUIHudRespawnGage;

	public NKCUIMainHPGage m_NKCUIMainHPGageAlly;

	public NKCUIMainHPGage m_NKCUIMainHPGageEnemy;

	public NKCUIMainHPGage m_NKCUIMainHPGageAllyLong;

	public NKCUIMainHPGage m_NKCUIMainHPGageEnemyLong;

	public NKCUIGameUnitSkillCooltime m_NKCUIMainSkillCoolLeft;

	public NKCUIGameUnitSkillCooltime m_NKCUIMainSkillCoolRight;

	[Header("리더/보스 얼굴(좌)")]
	public NKCUIComButton m_btnLeftUser;

	public Image m_imgLeftUser;

	public Image m_imgLeftUserRole;

	public Image m_imgLeftUserAttackType;

	public Text m_lbLeftUserName;

	public GameObject m_objLeftUserRage;

	[Header("리더/보스 얼굴(우)")]
	public NKCUIComButton m_btnRightUser;

	public Image m_imgRightUser;

	public Image m_imgRightUserRole;

	public Image m_imgRightUserAttackType;

	public Text m_lbRightUserName;

	public GameObject m_objRightUserRage;

	[Header("웨이브 넘버")]
	public GameObject m_objWave;

	public Text m_lbWave;

	private int m_WaveCount;

	[Header("남은 시간")]
	public Text m_lbTimeLeft;

	[Header("미니맵")]
	public RectTransform m_rtMiniMap;

	public RectTransform m_rtMinimapCamPanel;

	private float m_NUF_GAME_HUD_MINI_MAP_RectTransform_width;

	[Header("일시정지")]
	public NKCUIComButton m_btnPause;

	public Image m_imgPause;

	[Header("UI 숨기기")]
	public GameObject m_objUnhide;

	public NKCUIComButton m_btnUnhide;

	public GameObject m_objHide;

	public NKCUIComButton m_btnHide;

	[Header("연습전포기")]
	public NKCUIComButton m_btnGiveup;

	[Header("리그전 버프 버튼")]
	public NKCUIComStateButton m_csbtnDeadlineBuff;

	public Text m_lbDeadLineBuffLevel;

	private string m_strDeadLineBuffString;

	private int m_DeadlineBuffLevel;

	[Header("공격 포인트(레이드 등)")]
	public GameObject m_objAttackPoint;

	public Text m_lbAttackPointNow;

	public Text m_lbAttackPointMax;

	private float m_fAttackPointLeftPivotValue;

	private float m_fAttackPointLeftCurrValue;

	private int m_AttackPointLeftTargetValue;

	private float m_fElapsedTimeForAP;

	[Header("반복작전")]
	public GameObject m_objMultiplyReward;

	public Text m_lbRewardMultiply;

	public NKCGameHUDRepeatOperation m_NKCGameHUDRepeatOperation;

	[Header("배속 버튼")]
	public NKCUIComStateButton m_csbtnGameSpeed;

	public Image m_imgGameSpeed1;

	public Image m_imgGameSpeed2;

	public Image m_imgGameSpeed3;

	public Image m_imgGameSpeed05;

	protected NKM_GAME_SPEED_TYPE m_NKM_GAME_SPEED_TYPE;

	[Header("자동 출격")]
	public GameObject m_objAutoRespawn;

	public NKCUIComButton m_btnAutoRespawn;

	public Animator m_animatorAutoRespawn;

	public Image m_imgAutoRespawnOff;

	[Header("오토 스킬")]
	public NKCUIComButton m_btnAutoHyper;

	public GameObject m_objAutoHyperOn;

	public GameObject m_objAutoHyperOff;

	private NKM_GAME_AUTO_SKILL_TYPE m_eAutoSkillType;

	[Header("네트워크 현황")]
	public GameObject m_objNetworkWeak;

	public NKCUIComStateButton m_csbtnNetworkLevel;

	public Text m_lbNetworkLevel;

	[Header("알람 메시지")]
	public GameObject m_objHUDMessage;

	public Animator m_animatorHUDMessage;

	public Text m_lbHUDMessage;

	public GameObject m_objTimeOver;

	public NKCGameHudAlertCommon m_AlertEnv;

	public NKCGameHudAlertCommon m_AlertPhase;

	[Header("시간 이펙트")]
	public Animator m_animTimeChange;

	public NKCComTMPUIText m_lbTimeChangePlus;

	public NKCComTMPUIText m_lbTimeChangeMinus;

	[Header("소규모 서브 메뉴들")]
	public NKCGameHudPractice m_GameHudPractice;

	public NKCGameHudKillCount m_killCount;

	public GameObject m_objClassGuide;

	public Text m_lbUnitMaxCountSameTime;

	[Header("기타 서브메뉴")]
	public GameObject m_objOperatorPanelRoot;

	[Header("이모티콘")]
	public NKCGameHudEmoticon m_NKCGameHudEmoticon;

	[Header("핫키/인디케이터 위치잡이")]
	public Transform m_trHotkeyPosLeft;

	public Transform m_trHotkeyPosRight;

	public Transform m_trIndicatorRootLeft;

	public Transform m_trIndicatorRootRight;

	private int m_SelectUnitDeckIndex = -1;

	private int m_SelectShipSkillDeckIndex = -1;

	private int m_AutoRespawnIndex;

	private StringBuilder m_StringBuilder = new StringBuilder();

	private int m_RemainGameTimeInt;

	private bool m_bCountDownVoicePlayed;

	private bool m_bShipSkillReadyVoicePlayed;

	private bool m_bShipSkillReady;

	private float m_fShipSkillFullTime;

	private bool m_bCostFullVoicePlayed;

	private bool m_bCostFull;

	private float m_fMaxCostTime;

	private const string ICON_ENABLE_COLOR = "#FFFFFF";

	private const string ICON_DISABLE_COLOR = "#7B7B7B";

	public OnUseDeck dOnUseDeck;

	public OnUseSkill dOnUseSkill;

	private NKCGameHUDArtifact m_NKCGameHUDArtifact;

	private NKCAssetInstanceData m_NKCAssetInstanceDataArtifact;

	private NKCGameHudPause m_NKCGameHudPause;

	private NKCAssetInstanceData m_NKCAssetInstanceDataPause;

	private NKCUIDangerMessage m_NKCUIDangerMessage;

	private NKCAssetInstanceData m_NKCAssetInstanceDataDangerMessage;

	private NKCUIGameHUDMultiplyReward m_NKCUIGameHUDMultiplyReward;

	private NKCAssetInstanceData m_NKCAssetInstanceDataMultiplyReward;

	protected NKCGameHudCombo m_NKCGameHudCombo;

	private NKCAssetInstanceData m_NKCAssetInstanceDataCombo;

	private bool bReservedMultiply;

	private NKM_GAME_TYPE currentGameType;

	private NKCGameHUDFierceScore m_NKCGameHUDTrim;

	private NKCAssetInstanceData m_NKCAssetInstanceDataTrimScore;

	private HUDMode m_eMode;

	private NKCGameHudObjects m_HudObjects;

	private NKCGameHUDFierceScore m_NKCGameHUDFierce;

	private NKCAssetInstanceData m_NKCAssetInstanceDataFierceScore;

	private IGameHudAlert m_currentHudAlert;

	private Queue<IGameHudAlert> m_qHudAlert = new Queue<IGameHudAlert>();

	private List<NKCGameHudSummonIndicator> m_lstSummonIndicator = new List<NKCGameHudSummonIndicator>();

	public virtual NKM_TEAM_TYPE CurrentViewTeamType
	{
		get
		{
			HUDMode eMode = m_eMode;
			if ((uint)(eMode - 1) <= 1u)
			{
				return m_CurrentViewTeamType;
			}
			return m_GameClient.m_MyTeam;
		}
		set
		{
			m_CurrentViewTeamType = value;
		}
	}

	public int DeadlineBuffLevel => m_DeadlineBuffLevel;

	public NKM_GAME_TYPE CurrentGameType
	{
		get
		{
			if (m_GameClient == null || m_GameClient.GetGameData() == null)
			{
				return NKM_GAME_TYPE.NGT_INVALID;
			}
			if (currentGameType == NKM_GAME_TYPE.NGT_INVALID)
			{
				currentGameType = m_GameClient.GetGameData().GetGameType();
			}
			return currentGameType;
		}
	}

	public NKCGameClient GetGameClient()
	{
		return m_GameClient;
	}

	public int GetWaveCount()
	{
		return m_WaveCount;
	}

	public NKCGameHUDRepeatOperation GetNKCGameHUDRepeatOperation()
	{
		return m_NKCGameHUDRepeatOperation;
	}

	public NKCGameHudEmoticon GetNKCGameHudEmoticon()
	{
		return m_NKCGameHudEmoticon;
	}

	public int GetSelectUnitDeckIndex()
	{
		return m_SelectUnitDeckIndex;
	}

	public void SetSelectUnitDeckIndex(int index)
	{
		m_SelectUnitDeckIndex = index;
	}

	public int GetSelectShipSkillDeckIndex()
	{
		return m_SelectShipSkillDeckIndex;
	}

	public void SetSelectShipSkillDeckIndex(int index)
	{
		m_SelectShipSkillDeckIndex = index;
	}

	public NKCGameHudPause GetNKCGameHudPause()
	{
		return m_NKCGameHudPause;
	}

	public virtual void InitUI(HUDMode mode)
	{
		Log.Info("[GAME_HUD] InitUI", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Hud/NKCGameHud.cs", 336);
		m_eMode = mode;
		m_HudObjects = NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.GetHudObjects();
		RectTransform component = GetComponent<RectTransform>();
		component.anchoredPosition3D = Vector3.zero;
		component.anchoredPosition = Vector2.zero;
		NKCUtil.SetGameobjectActive(m_lbUnitMaxCountSameTime, bValue: false);
		InitUI_NetworkStrength(base.gameObject);
		InitUI_GameSpeed();
		InitUI_HideUI(base.gameObject);
		InitUI_Practice();
		InitUI_TeamDeck();
		InitUI_AutoRespawn();
		InitUI_AutoSkill();
		InitUI_MessageAlertTimeover();
		InitUI_Pause();
		InitUI_HudTop();
		InitUI_Repeat();
		InitUI_Multiply();
		InitUI_Operator();
		InitUI_Observer();
		InitUI_GuildDungeon();
		NKCUtil.SetGameobjectActive(m_killCount, bValue: false);
		m_bCountDownVoicePlayed = false;
		m_bShipSkillReadyVoicePlayed = false;
		m_bShipSkillReady = false;
		m_fShipSkillFullTime = 0f;
		m_bCostFullVoicePlayed = false;
		m_bCostFull = false;
		m_fMaxCostTime = 0f;
		InitUI_Finalize();
	}

	protected virtual void InitUI_Finalize()
	{
	}

	protected void InitUI_GuildDungeon()
	{
		NKCUtil.SetGameobjectActive(m_btnGiveup, bValue: false);
	}

	protected void SetGuildDungeonData(NKMGameData cNKMGameData)
	{
		if (NKMOpenTagManager.IsOpened("GUILD_DUNGEON_GIVEUP") && GetGameClient().GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE)
		{
			NKCUtil.SetButtonClickDelegate(m_btnGiveup, OnClickGiveup);
			NKCUtil.SetGameobjectActive(m_btnGiveup, bValue: true);
		}
	}

	protected void OnClickGiveup()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_INFORMATION, NKCStringTable.GetString("SI_DP_GUILD_DUNGEON_PRACTICE_GIVEUP_INFO"), delegate
		{
			NKCPacketSender.Send_NKMPacket_GAME_GIVEUP_REQ();
		});
	}

	protected virtual void InitUI_Observer()
	{
		HUDMode eMode = m_eMode;
		if ((uint)(eMode - 1) <= 1u)
		{
			NKCUtil.SetGameobjectActive(m_HudObserver, bValue: true);
			m_HudObserver.InitUI(this);
			NKCUtil.SetGameobjectActive(m_objInactiveUser, m_eMode != HUDMode.Observer);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_HudObserver, bValue: false);
			NKCUtil.SetGameobjectActive(m_objInactiveUser, bValue: false);
		}
	}

	public void ChangeTeamDeck()
	{
		switch (CurrentViewTeamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			CurrentViewTeamType = NKM_TEAM_TYPE.NTT_B1;
			break;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			CurrentViewTeamType = NKM_TEAM_TYPE.NTT_A1;
			break;
		}
		NKMGameTeamData currentViewTeamData = GetCurrentViewTeamData();
		if (currentViewTeamData != null)
		{
			float respawnCostClient = GetGameClient().GetRespawnCostClient(CurrentViewTeamType);
			m_NKCUIHudRespawnGage.SetRespawnCostNowValue(respawnCostClient);
			SetRespawnCost();
			SetRespawnCostAssist();
			SetDeck(currentViewTeamData);
			SetAssistDeck(currentViewTeamData);
			SetShipSkillDeck(currentViewTeamData.m_MainShip);
			if (currentViewTeamData.GetTC_Combo() == null)
			{
				NKCUtil.SetGameobjectActive(m_NKCGameHudCombo, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(m_NKCGameHudCombo, bValue: true);
			m_NKCGameHudCombo.SetUI(GetGameClient().GetGameData(), CurrentViewTeamType);
		}
	}

	protected virtual void InitUI_QuickCam()
	{
	}

	protected virtual void InitUI_NetworkStrength(GameObject cNUF_GAME_PREFAB)
	{
		NKCUtil.SetGameobjectActive(m_objNetworkWeak, bValue: false);
		if (m_csbtnNetworkLevel != null)
		{
			m_csbtnNetworkLevel.PointerDown.RemoveAllListeners();
			m_csbtnNetworkLevel.PointerDown.AddListener(delegate(PointerEventData eventData)
			{
				OnClickedNetworkLevel(eventData);
			});
		}
		if (m_eMode == HUDMode.Replay)
		{
			NKCUtil.SetGameobjectActive(m_csbtnNetworkLevel, bValue: false);
		}
	}

	protected virtual void InitUI_GameSpeed()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnGameSpeed, SendGameSpeed2X);
	}

	protected virtual void InitUI_HideUI(GameObject cNUF_GAME_PREFAB)
	{
		NKCUtil.SetButtonClickDelegate(m_btnUnhide, HUD_UNHIDE);
		NKCUtil.SetGameobjectActive(m_objUnhide, bValue: false);
		NKCUtil.SetButtonClickDelegate(m_btnHide, OnBtnHideHud);
	}

	protected virtual void InitUI_TeamDeck()
	{
		m_NKCUIMainHPGageAlly.InitUI();
		m_NKCUIMainHPGageEnemy.InitUI();
		m_NKCUIMainHPGageAllyLong.InitUI();
		m_NKCUIMainHPGageEnemyLong.InitUI();
		NKCUtil.SetGameobjectActive(m_csbtnDeadlineBuff, bValue: false);
		NKCUtil.SetButtonPointerDownDelegate(m_csbtnDeadlineBuff, OnClickedDeadlineBuff);
		for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
		{
			m_NKCUIHudDeck[i].InitUI(this, i);
		}
		m_NKCUIHudDeck[5].SetActive(bActive: false);
		NKCUtil.SetLabelText(m_lbRemainUnitCount, "");
		for (int j = 0; j < m_NKCUIHudShipSkillDeck.Length; j++)
		{
			NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[j];
			if (!(nKCUIHudShipSkillDeck == null))
			{
				nKCUIHudShipSkillDeck.InitUI(j);
			}
		}
		Transform transform = base.transform.Find("NUF_GAME_HUD_TACTICAL_COMMAND");
		if (transform != null)
		{
			for (int k = 0; k < m_NKCUIHudTacticalCommandDeck.Length; k++)
			{
				NKCUIHudTacticalCommandDeck nKCUIHudTacticalCommandDeck = new NKCUIHudTacticalCommandDeck();
				m_NKCUIHudTacticalCommandDeck[k] = nKCUIHudTacticalCommandDeck;
				nKCUIHudTacticalCommandDeck.InitUI(this, transform.gameObject, k);
			}
		}
	}

	protected virtual void InitUI_Practice()
	{
		if (m_GameHudPractice != null)
		{
			m_GameHudPractice.Init(this);
		}
	}

	protected virtual void InitUI_AutoSkill()
	{
		NKCUtil.SetButtonClickDelegate(m_btnAutoHyper, SendGameAutoSkillChange);
	}

	protected virtual void InitUI_AutoRespawn()
	{
		NKCUtil.SetButtonClickDelegate(m_btnAutoRespawn, NKCScenManager.GetScenManager().m_NKCSystemEvent.UI_HUD_AUTO_RESPAWN);
		ToggleAutoRespawn(bOn: false);
		m_imgAutoRespawnOff.color = NKCUtil.GetColor("#FFFFFF");
	}

	protected virtual void InitUI_MessageAlertTimeover()
	{
		NKCUtil.SetGameobjectActive(m_objHUDMessage, bValue: false);
		NKCUtil.SetGameobjectActive(m_AlertEnv, bValue: false);
		NKCUtil.SetGameobjectActive(m_AlertPhase, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTimeOver, bValue: false);
	}

	protected virtual void InitUI_Pause()
	{
		NKCUtil.SetButtonClickDelegate(m_btnPause, NKCScenManager.GetScenManager().m_NKCSystemEvent.UI_GAME_PAUSE);
	}

	protected virtual void InitUI_HudTop()
	{
		NKCUtil.SetGameobjectActive(m_objHUDBgFx, bValue: false);
		NKCUtil.SetButtonClickDelegate(m_btnLeftUser, NKCScenManager.GetScenManager().m_NKCSystemEvent.UI_GAME_NO_HP_DMG_MODE_TEAM_A);
		NKCUtil.SetButtonClickDelegate(m_btnRightUser, NKCScenManager.GetScenManager().m_NKCSystemEvent.UI_GAME_NO_HP_DMG_MODE_TEAM_B);
	}

	protected virtual void InitUI_Repeat()
	{
		m_NKCGameHUDRepeatOperation.InitUI();
		NKCUtil.SetGameobjectActive(m_NKCGameHUDRepeatOperation, bValue: false);
	}

	protected virtual void InitUI_Multiply()
	{
		NKCUtil.SetGameobjectActive(m_objMultiplyReward, bValue: false);
	}

	protected virtual void InitUI_Operator()
	{
		NKCUtil.SetGameobjectActive(m_objOperatorPanelRoot, bValue: false);
	}

	public void SetGameClient(NKCGameClient cGameClient)
	{
		m_GameClient = cGameClient;
		if (m_NKCUIHudRespawnGage != null)
		{
			m_NKCUIHudRespawnGage.SetData();
		}
		if (m_NKCUIMainHPGageAlly != null)
		{
			m_NKCUIMainHPGageAlly.InitData();
		}
		if (m_NKCUIMainHPGageEnemy != null)
		{
			m_NKCUIMainHPGageEnemy.InitData();
		}
		if (m_NKCUIMainHPGageAllyLong != null)
		{
			m_NKCUIMainHPGageAllyLong.InitData();
		}
		if (m_NKCUIMainHPGageEnemyLong != null)
		{
			m_NKCUIMainHPGageEnemyLong.InitData();
		}
		NKCUtil.SetGameobjectActive(m_objLeftUserRage, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRightUserRage, bValue: false);
		if (m_NKCUIMainSkillCoolLeft != null)
		{
			m_NKCUIMainSkillCoolLeft.SetSkillCoolVisible(value: false);
			m_NKCUIMainSkillCoolLeft.SetHyperCoolVisible(value: false);
		}
		if (m_NKCUIMainSkillCoolRight != null)
		{
			m_NKCUIMainSkillCoolRight.SetSkillCoolVisible(value: false);
			m_NKCUIMainSkillCoolRight.SetHyperCoolVisible(value: false);
		}
		for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
		{
			NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
			if (nKCGameHudDeckSlot != null)
			{
				nKCGameHudDeckSlot.Init();
			}
		}
		for (int j = 0; j < m_NKCUIHudShipSkillDeck.Length; j++)
		{
			NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[j];
			if (nKCUIHudShipSkillDeck != null)
			{
				nKCUIHudShipSkillDeck.Init();
			}
		}
		for (int k = 0; k < m_NKCUIHudTacticalCommandDeck.Length; k++)
		{
			m_NKCUIHudTacticalCommandDeck[k]?.Init();
		}
		m_SelectUnitDeckIndex = -1;
		m_SelectShipSkillDeckIndex = -1;
		m_RemainGameTimeInt = 0;
	}

	public void LoadUI(NKMGameData cNKMGameData)
	{
		if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			if (m_GameHudPractice != null)
			{
				m_GameHudPractice.SetEnable(value: true);
			}
			m_objTop.SetActive(value: false);
		}
		else
		{
			if (m_GameHudPractice != null)
			{
				m_GameHudPractice.SetEnable(value: false);
			}
			m_objTop.SetActive(value: true);
		}
		LoadDeck(cNKMGameData);
		LoadUserIcon(cNKMGameData);
		LoadArtifact(cNKMGameData);
		LoadRepeatOperation(cNKMGameData);
		LoadEmoticon(cNKMGameData);
		LoadPause(cNKMGameData);
		LoadDangerMessage(cNKMGameData);
		LoadMultiplyReward(cNKMGameData);
		LoadFierceBattleScore(cNKMGameData);
		LoadTrimBattleScore(cNKMGameData);
		LoadCombo();
		if (cNKMGameData.IsPVP())
		{
			MakeSummonIndicator();
			MakeSummonIndicator();
		}
		PrepareAlerts(cNKMGameData);
	}

	private void LoadRepeatOperation(NKMGameData cNKMGameData)
	{
		m_NKCGameHUDRepeatOperation.SetUI(cNKMGameData);
	}

	private void LoadArtifact(NKMGameData cNKMGameData)
	{
		if (cNKMGameData == null)
		{
			return;
		}
		if (NKCGameHUDArtifact.GetActive(cNKMGameData))
		{
			if (m_NKCGameHUDArtifact == null)
			{
				if (m_NKCAssetInstanceDataArtifact != null)
				{
					NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataArtifact);
				}
				m_NKCAssetInstanceDataArtifact = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_HUD_RENEWAL", "AB_UI_GAME_HUD_ARTIFACT");
				NKCGameHUDArtifact nKCGameHUDArtifact = (m_NKCGameHUDArtifact = m_NKCAssetInstanceDataArtifact.m_Instant.GetComponentInChildren<NKCGameHUDArtifact>());
				m_NKCAssetInstanceDataArtifact.m_Instant.transform.SetParent(base.transform, worldPositionStays: false);
				m_NKCAssetInstanceDataArtifact.m_Instant.transform.localPosition = new Vector3(m_NKCAssetInstanceDataArtifact.m_Instant.transform.localPosition.x, m_NKCAssetInstanceDataArtifact.m_Instant.transform.localPosition.y, 0f);
				nKCGameHUDArtifact.gameObject.SetActive(value: false);
			}
		}
		else
		{
			m_NKCGameHUDArtifact = null;
		}
	}

	private void LoadFierceBattleScore(NKMGameData cNKMGameData)
	{
		if (cNKMGameData == null)
		{
			return;
		}
		if (cNKMGameData.GetGameType() != NKM_GAME_TYPE.NGT_FIERCE)
		{
			m_NKCGameHUDFierce = null;
		}
		else if (m_NKCGameHUDFierce == null)
		{
			if (m_NKCAssetInstanceDataFierceScore != null)
			{
				NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataFierceScore);
			}
			m_NKCAssetInstanceDataFierceScore = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_HUD_RENEWAL", "AB_UI_GAME_HUD_FIERCE_BATTLE");
			m_NKCGameHUDFierce = m_NKCAssetInstanceDataFierceScore.m_Instant.GetComponentInChildren<NKCGameHUDFierceScore>();
			m_NKCAssetInstanceDataFierceScore.m_Instant.transform.SetParent(base.transform, worldPositionStays: false);
			m_NKCAssetInstanceDataFierceScore.m_Instant.transform.SetAsFirstSibling();
			m_NKCAssetInstanceDataFierceScore.m_Instant.transform.localPosition = new Vector3(m_NKCAssetInstanceDataFierceScore.m_Instant.transform.localPosition.x, m_NKCAssetInstanceDataFierceScore.m_Instant.transform.localPosition.y, 0f);
			m_NKCGameHUDFierce.gameObject.SetActive(value: false);
		}
	}

	public void SetFierceBattleScore(int fierceScore)
	{
		if (m_NKCGameHUDFierce != null)
		{
			m_NKCGameHUDFierce.SetData(fierceScore, NKCGameHUDFierceScore.SCORE_TYPE.FIERCE);
		}
	}

	private void LoadTrimBattleScore(NKMGameData cNKMGameData)
	{
		if (cNKMGameData == null)
		{
			return;
		}
		if (cNKMGameData.GetGameType() != NKM_GAME_TYPE.NGT_TRIM)
		{
			m_NKCGameHUDTrim = null;
		}
		else if (m_NKCGameHUDTrim == null)
		{
			if (m_NKCAssetInstanceDataTrimScore != null)
			{
				NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataTrimScore);
			}
			m_NKCAssetInstanceDataTrimScore = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_HUD_RENEWAL", "AB_UI_GAME_HUD_FIERCE_BATTLE");
			m_NKCGameHUDTrim = m_NKCAssetInstanceDataTrimScore.m_Instant.GetComponentInChildren<NKCGameHUDFierceScore>();
			m_NKCAssetInstanceDataTrimScore.m_Instant.transform.SetParent(base.transform, worldPositionStays: false);
			m_NKCAssetInstanceDataTrimScore.m_Instant.transform.SetAsFirstSibling();
			m_NKCAssetInstanceDataTrimScore.m_Instant.transform.localPosition = new Vector3(m_NKCAssetInstanceDataTrimScore.m_Instant.transform.localPosition.x, m_NKCAssetInstanceDataTrimScore.m_Instant.transform.localPosition.y, 0f);
			m_NKCGameHUDTrim.gameObject.SetActive(value: false);
		}
	}

	public void SetTrimBattleScore(int trimScore)
	{
		if (m_NKCGameHUDTrim != null)
		{
			m_NKCGameHUDTrim.SetData(trimScore, NKCGameHUDFierceScore.SCORE_TYPE.TRIM);
		}
	}

	private void LoadEmoticon(NKMGameData cNKMGameData)
	{
		if (cNKMGameData == null)
		{
			return;
		}
		if (cNKMGameData.IsPVP())
		{
			if (m_NKCGameHudEmoticon != null)
			{
				m_NKCGameHudEmoticon.SetEnableUI(value: true);
			}
		}
		else if (m_NKCGameHudEmoticon != null)
		{
			m_NKCGameHudEmoticon.SetEnableUI(value: false);
		}
	}

	public bool IsOpenPause()
	{
		if (m_NKCGameHudPause != null)
		{
			return m_NKCGameHudPause.IsOpen();
		}
		return false;
	}

	public void OpenPause(NKCGameHudPause.dOnClickContinue _dOnClickContinue = null)
	{
		if (m_NKCGameHudPause != null)
		{
			m_NKCGameHudPause.Open(_dOnClickContinue);
		}
	}

	public void OnClickContinueOnPause()
	{
		if (m_NKCGameHudPause != null && m_NKCGameHudPause.IsOpen())
		{
			m_NKCGameHudPause.OnClickContinue();
		}
	}

	public void ClosePause()
	{
		if (m_NKCGameHudPause != null)
		{
			m_NKCGameHudPause.Close();
		}
	}

	private void LoadPause(NKMGameData cNKMGameData)
	{
		if (cNKMGameData != null && m_NKCGameHudPause == null)
		{
			if (m_NKCAssetInstanceDataPause != null)
			{
				NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataPause);
			}
			m_NKCAssetInstanceDataPause = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_HUD_RENEWAL", "AB_UI_GAME_HUD_PAUSE");
			NKCGameHudPause nKCGameHudPause = (m_NKCGameHudPause = m_NKCAssetInstanceDataPause.m_Instant.GetComponentInChildren<NKCGameHudPause>());
			m_NKCAssetInstanceDataPause.m_Instant.transform.SetParent(m_HudObjects.m_NUF_GAME_HUD_UI_PAUSE.transform, worldPositionStays: false);
			m_NKCAssetInstanceDataPause.m_Instant.transform.localPosition = new Vector3(m_NKCAssetInstanceDataPause.m_Instant.transform.localPosition.x, m_NKCAssetInstanceDataPause.m_Instant.transform.localPosition.y, 0f);
			NKCUtil.SetGameobjectActive(nKCGameHudPause.gameObject, bValue: false);
		}
	}

	public void Clear()
	{
		if (m_NKCAssetInstanceDataArtifact != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataArtifact);
		}
		m_NKCAssetInstanceDataArtifact = null;
		m_NKCGameHUDArtifact = null;
		if (m_NKCGameHudEmoticon != null)
		{
			m_NKCGameHudEmoticon.Clear();
		}
		if (m_NKCAssetInstanceDataPause != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataPause);
		}
		m_NKCAssetInstanceDataPause = null;
		m_NKCGameHudPause = null;
		if (m_NKCUIDangerMessage != null)
		{
			m_NKCUIDangerMessage.Clear();
		}
		if (m_NKCAssetInstanceDataDangerMessage != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataDangerMessage);
		}
		m_NKCAssetInstanceDataDangerMessage = null;
		m_NKCUIDangerMessage = null;
		if (m_NKCAssetInstanceDataMultiplyReward != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataMultiplyReward);
		}
		m_NKCAssetInstanceDataMultiplyReward = null;
		m_NKCUIGameHUDMultiplyReward = null;
		if (m_NKCAssetInstanceDataFierceScore != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataFierceScore);
		}
		m_NKCAssetInstanceDataFierceScore = null;
		m_NKCGameHUDFierce = null;
		if (m_NKCAssetInstanceDataTrimScore != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataTrimScore);
		}
		m_NKCAssetInstanceDataTrimScore = null;
		m_NKCGameHUDTrim = null;
		if (m_NKCAssetInstanceDataCombo != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataCombo);
		}
		m_NKCAssetInstanceDataCombo = null;
		m_NKCGameHudCombo = null;
		CleanupAllHudAlert();
	}

	public void SetUserIconColor(bool bTeamA, Color _color)
	{
		if (bTeamA)
		{
			m_imgLeftUser.color = _color;
		}
		else
		{
			m_imgRightUser.color = _color;
		}
	}

	public void SetAutoEnable()
	{
		NKCUtil.SetGameobjectActive(m_objAutoRespawn, IsAutoRespawnVisible());
		NKCUtil.SetImageColor(m_imgAutoRespawnOff, NKCUtil.GetColor(IsAutoRespawnUsable() ? "#FFFFFF" : "#7B7B7B"));
	}

	public void SetMultiply(int multiply)
	{
		if (multiply > 1)
		{
			m_NKCGameHUDRepeatOperation?.SetDisable();
			if (m_NKCUIGameHUDMultiplyReward != null)
			{
				m_NKCUIGameHUDMultiplyReward.SetMultiply(multiply);
			}
		}
		NKCUtil.SetGameobjectActive(m_objMultiplyReward, multiply > 1);
		NKCUtil.SetLabelText(m_lbRewardMultiply, NKCUtilString.GET_MULTIPLY_REWARD_ONE_PARAM, multiply);
		bReservedMultiply = multiply > 1;
	}

	public NKCUIComButton GetAutoButton()
	{
		return m_btnAutoRespawn;
	}

	public void TogglePause(bool bSet, bool bLocalServer)
	{
		if (m_imgPause != null)
		{
			if (bSet)
			{
				m_imgPause.color = NKCUtil.GetColor("#FFD428");
			}
			else
			{
				m_imgPause.color = Color.white;
			}
		}
		NKCUtil.SetGameobjectActive(NKCUIManager.m_NUF_GAME_TOUCH_OBJECT, !bSet);
	}

	public void SetAttackPointUI(NKMGameData cNKMGameData)
	{
		m_fAttackPointLeftCurrValue = 0f;
		m_AttackPointLeftTargetValue = 0;
		m_fElapsedTimeForAP = 0f;
		if (cNKMGameData == null)
		{
			return;
		}
		bool bValue = false;
		if (cNKMGameData.IsPVE())
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(cNKMGameData.m_DungeonID);
			if (dungeonTempletBase != null && (dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE || dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_RAID || dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID))
			{
				bValue = true;
				m_lbAttackPointNow.text = "";
				m_lbAttackPointMax.text = "";
			}
		}
		NKCUtil.SetGameobjectActive(m_objAttackPoint, bValue);
	}

	public void SetCurrentWaveUI(int waveID)
	{
		if (m_lbWave != null)
		{
			m_lbWave.text = waveID + "/" + m_WaveCount;
		}
	}

	public void SetWaveUI(NKMGameData cNKMGameData)
	{
		if (cNKMGameData == null)
		{
			return;
		}
		bool bValue = false;
		if (cNKMGameData.IsPVE())
		{
			NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(cNKMGameData.m_DungeonID);
			if (dungeonTemplet != null && dungeonTemplet.m_DungeonTempletBase != null && dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE)
			{
				bValue = true;
				if (dungeonTemplet.m_listDungeonWave != null)
				{
					m_WaveCount = dungeonTemplet.m_listDungeonWave.Count;
					m_lbWave.text = 0 + "/" + m_WaveCount;
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objWave, bValue);
	}

	public void SetGageUI(NKMGameData cNKMGameData)
	{
		bool flag = false;
		if (cNKMGameData.IsPVE())
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(cNKMGameData.m_DungeonID);
			if (dungeonTempletBase != null)
			{
				if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE)
				{
					m_NKCUIMainHPGageEnemyLong.SetMainGageVisible(bSet: false);
					m_NKCUIMainHPGageAllyLong.SetMainGageVisible(bSet: true);
					m_NKCUIMainHPGageEnemy.SetMainGageVisible(bSet: false);
					m_NKCUIMainHPGageAlly.SetMainGageVisible(bSet: false);
					flag = true;
				}
				else if (dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE)
				{
					m_NKCUIMainHPGageEnemyLong.SetMainGageVisible(bSet: true);
					m_NKCUIMainHPGageAllyLong.SetMainGageVisible(bSet: false);
					m_NKCUIMainHPGageEnemy.SetMainGageVisible(bSet: false);
					m_NKCUIMainHPGageAlly.SetMainGageVisible(bSet: false);
					flag = true;
				}
				else if (dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE)
				{
					m_NKCUIMainHPGageEnemyLong.SetMainGageVisible(bSet: false);
					m_NKCUIMainHPGageAllyLong.SetMainGageVisible(bSet: true);
					m_NKCUIMainHPGageEnemy.SetMainGageVisible(bSet: false);
					m_NKCUIMainHPGageAlly.SetMainGageVisible(bSet: false);
					flag = true;
				}
			}
		}
		if (!flag)
		{
			m_NKCUIMainHPGageEnemy.SetMainGageVisible(bSet: true);
			m_NKCUIMainHPGageAlly.SetMainGageVisible(bSet: true);
			m_NKCUIMainHPGageEnemyLong.SetMainGageVisible(bSet: false);
			m_NKCUIMainHPGageAllyLong.SetMainGageVisible(bSet: false);
		}
	}

	public void SetAttackPoint(bool bLeft, int value)
	{
		if (bLeft)
		{
			if (value == 0)
			{
				m_fAttackPointLeftPivotValue = value;
				m_fAttackPointLeftCurrValue = value;
				m_lbAttackPointNow.text = value.ToString();
			}
			else if (value != m_AttackPointLeftTargetValue)
			{
				m_fElapsedTimeForAP = 0f;
				m_fAttackPointLeftPivotValue = m_fAttackPointLeftCurrValue;
			}
			m_AttackPointLeftTargetValue = value;
		}
		else
		{
			m_lbAttackPointMax.text = "/" + value.ToString("N0");
		}
	}

	public void SetShowUI(bool bShowUI, bool bDev)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bShowUI);
	}

	private void LoadUserIcon(NKMGameData cNKMGameData)
	{
		NKMUnitData unitDataByUnitUID = cNKMGameData.m_NKMGameTeamDataA.GetUnitDataByUnitUID(cNKMGameData.m_NKMGameTeamDataA.m_LeaderUnitUID);
		NKMUnitData unitDataByUnitUID2 = cNKMGameData.m_NKMGameTeamDataB.GetUnitDataByUnitUID(cNKMGameData.m_NKMGameTeamDataB.m_LeaderUnitUID);
		if (unitDataByUnitUID != null)
		{
			LoadUnitFaceDeckAsset(unitDataByUnitUID);
		}
		if (unitDataByUnitUID2 != null)
		{
			LoadUnitFaceDeckAsset(unitDataByUnitUID2);
		}
		if (IsFristUnitMain(cNKMGameData.GetGameType()))
		{
			unitDataByUnitUID = cNKMGameData.m_NKMGameTeamDataA.GetFirstUnitData();
			unitDataByUnitUID2 = cNKMGameData.m_NKMGameTeamDataB.GetFirstUnitData();
			if (unitDataByUnitUID != null)
			{
				LoadUnitFaceDeckAsset(unitDataByUnitUID);
			}
			if (unitDataByUnitUID2 != null)
			{
				LoadUnitFaceDeckAsset(unitDataByUnitUID2);
			}
		}
		NKCResourceUtility.PreloadUnitInvenIconEmpty();
	}

	private void LoadUnitFaceDeckAsset(NKMUnitData unitData)
	{
		NKCResourceUtility.PreloadUnitResource(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitData);
	}

	protected virtual void LoadDeck(NKMGameData cNKMGameData)
	{
		HUDMode eMode = m_eMode;
		if ((uint)(eMode - 1) > 1u)
		{
			NKMGameTeamData teamData = cNKMGameData.GetTeamData(CurrentViewTeamType);
			if (teamData != null)
			{
				LoadDeck(teamData);
			}
			return;
		}
		NKMGameTeamData teamData2 = cNKMGameData.GetTeamData(NKM_TEAM_TYPE.NTT_A1);
		NKMGameTeamData teamData3 = cNKMGameData.GetTeamData(NKM_TEAM_TYPE.NTT_B1);
		if (teamData2 != null)
		{
			LoadDeck(teamData2);
		}
		if (teamData3 != null)
		{
			LoadDeck(teamData3);
		}
	}

	protected void LoadDeck(NKMGameTeamData cNKMGameTeamData)
	{
		NKMUnitTemplet nKMUnitTemplet = null;
		if (cNKMGameTeamData.m_MainShip != null)
		{
			nKMUnitTemplet = NKMUnitManager.GetUnitTemplet(cNKMGameTeamData.m_MainShip.m_UnitID);
			LoadTacticalCommandDeck(cNKMGameTeamData);
		}
		if (nKMUnitTemplet != null)
		{
			m_objRootShipSkill.SetActive(value: true);
			LoadShipSkillDeck(nKMUnitTemplet);
		}
		else
		{
			m_objRootShipSkill.SetActive(value: false);
		}
		for (int i = 0; i < cNKMGameTeamData.m_listUnitData.Count; i++)
		{
			NKMUnitData nKMUnitData = cNKMGameTeamData.m_listUnitData[i];
			if (nKMUnitData != null)
			{
				LoadUnitDeck(nKMUnitData, bAsync: true);
			}
		}
		for (int j = 0; j < cNKMGameTeamData.m_listAssistUnitData.Count; j++)
		{
			NKMUnitData nKMUnitData2 = cNKMGameTeamData.m_listAssistUnitData[j];
			if (nKMUnitData2 != null)
			{
				LoadUnitDeck(nKMUnitData2, bAsync: true);
			}
		}
	}

	public void LoadUnitDeck(NKMUnitData cNKMUnitData, bool bAsync)
	{
		NKCResourceUtility.PreloadUnitResource(NKCResourceUtility.eUnitResourceType.INVEN_ICON, cNKMUnitData);
	}

	public void LoadUnitDeck(NKMUnitTemplet cNKMUnitTemplet, bool bAsync)
	{
		NKCResourceUtility.PreloadUnitResource(NKCResourceUtility.eUnitResourceType.INVEN_ICON, cNKMUnitTemplet.m_UnitTempletBase);
	}

	private void LoadShipSkillDeck(NKMUnitTemplet cNKMUnitTemplet)
	{
		if (cNKMUnitTemplet != null)
		{
			for (int i = 0; i < cNKMUnitTemplet.m_UnitTempletBase.m_lstSkillStrID.Count; i++)
			{
				NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(cNKMUnitTemplet.m_UnitTempletBase, i);
				if (shipSkillTempletByIndex != null)
				{
					NKCResourceUtility.LoadAssetResourceTemp<Sprite>("AB_UI_SHIP_SKILL_ICON", shipSkillTempletByIndex.m_ShipSkillIcon);
				}
			}
		}
		NKCResourceUtility.LoadAssetResourceTemp<Sprite>("AB_UI_SHIP_SKILL_ICON", "SS_NO_SKILL_ICON");
	}

	private void LoadTacticalCommandDeck(NKMGameTeamData cNKMGameTeamData)
	{
		if (cNKMGameTeamData != null)
		{
			for (int i = 0; i < cNKMGameTeamData.m_listTacticalCommandData.Count; i++)
			{
				NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(cNKMGameTeamData.m_listTacticalCommandData[i].m_TCID);
				if (tacticalCommandTempletByID != null)
				{
					NKCResourceUtility.LoadAssetResourceTemp<Sprite>("AB_UI_TACTICAL_COMMAND_ICON", tacticalCommandTempletByID.m_TCIconName);
				}
			}
		}
		NKCResourceUtility.LoadAssetResourceTemp<Sprite>("AB_UI_TACTICAL_COMMAND_ICON", "ICON_TC_NO_SKILL_ICON");
	}

	private void LoadDangerMessage(NKMGameData cNKMGameData)
	{
		if (cNKMGameData == null || NKCContentManager.IsContentsUnlocked(ContentsType.BATTLE_2X) || NKCTutorialManager.IsTutorialDungeon(cNKMGameData.m_DungeonID) || cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			return;
		}
		if (m_GameClient.EnableControlByGameType())
		{
			if (m_NKCUIDangerMessage == null)
			{
				if (m_NKCAssetInstanceDataDangerMessage != null)
				{
					NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataDangerMessage);
				}
				m_NKCAssetInstanceDataDangerMessage = NKCAssetResourceManager.OpenInstance<GameObject>("AB_FX_UI_DANGER", "AB_FX_UI_DANGER_MESSAGE");
				NKCUIDangerMessage targetMono = (m_NKCUIDangerMessage = m_NKCAssetInstanceDataDangerMessage.m_Instant.GetComponent<NKCUIDangerMessage>());
				m_NKCAssetInstanceDataDangerMessage.m_Instant.transform.SetParent(base.transform, worldPositionStays: false);
				m_NKCAssetInstanceDataDangerMessage.m_Instant.transform.localPosition = new Vector3(m_NKCAssetInstanceDataDangerMessage.m_Instant.transform.localPosition.x, m_NKCAssetInstanceDataDangerMessage.m_Instant.transform.localPosition.y, 0f);
				NKCUtil.SetGameobjectActive(targetMono, bValue: false);
			}
		}
		else
		{
			m_NKCUIDangerMessage = null;
		}
	}

	private void LoadMultiplyReward(NKMGameData cNKMGameData)
	{
		if (cNKMGameData != null && NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_MULTIPLY) && cNKMGameData.GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE && m_NKCUIGameHUDMultiplyReward == null)
		{
			if (m_NKCAssetInstanceDataMultiplyReward != null)
			{
				NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataMultiplyReward);
			}
			m_NKCAssetInstanceDataMultiplyReward = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_HUD_RENEWAL", "AB_UI_GAME_HUD_MULTIPLY_REWARD");
			NKCUIGameHUDMultiplyReward targetMono = (m_NKCUIGameHUDMultiplyReward = m_NKCAssetInstanceDataMultiplyReward.m_Instant.GetComponent<NKCUIGameHUDMultiplyReward>());
			m_NKCAssetInstanceDataMultiplyReward.m_Instant.transform.SetParent(base.transform, worldPositionStays: false);
			m_NKCAssetInstanceDataMultiplyReward.m_Instant.transform.localPosition = new Vector3(m_NKCAssetInstanceDataMultiplyReward.m_Instant.transform.localPosition.x, m_NKCAssetInstanceDataMultiplyReward.m_Instant.transform.localPosition.y, 0f);
			NKCUtil.SetGameobjectActive(targetMono, bValue: false);
		}
	}

	private void LoadCombo()
	{
		if (m_GameClient == null)
		{
			return;
		}
		NKMGameTeamData myTeamData = m_GameClient.GetMyTeamData();
		if (myTeamData != null && myTeamData.GetTC_Combo() != null && m_NKCAssetInstanceDataCombo == null && m_objOperatorPanelRoot != null)
		{
			if (m_NKCAssetInstanceDataCombo != null)
			{
				NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataCombo);
			}
			m_NKCAssetInstanceDataCombo = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_HUD_RENEWAL", "AB_UI_GAME_HUD_OPERATOR");
			NKCGameHudCombo targetMono = (m_NKCGameHudCombo = m_NKCAssetInstanceDataCombo.m_Instant.GetComponent<NKCGameHudCombo>());
			m_NKCAssetInstanceDataCombo.m_Instant.transform.SetParent(m_objOperatorPanelRoot.transform, worldPositionStays: false);
			m_NKCAssetInstanceDataCombo.m_Instant.transform.localPosition = new Vector3(m_NKCAssetInstanceDataCombo.m_Instant.transform.localPosition.x, m_NKCAssetInstanceDataCombo.m_Instant.transform.localPosition.y, 0f);
			NKCUtil.SetGameobjectActive(m_objOperatorPanelRoot, bValue: true);
			NKCUtil.SetGameobjectActive(targetMono, bValue: false);
		}
	}

	public void LoadComplete(NKMGameData cNKMGameData)
	{
		SetDeck(cNKMGameData);
		SetUserInfoUI(cNKMGameData);
		NKMGameTeamData teamData = cNKMGameData.GetTeamData(CurrentViewTeamType);
		if (teamData != null)
		{
			SetShipSkillDeck(teamData.m_MainShip);
		}
		NKCUtil.SetGameobjectActive(m_objHUDMessage, bValue: false);
		NKCUtil.SetGameobjectActive(m_AlertEnv, bValue: false);
		NKCUtil.SetGameobjectActive(m_AlertPhase, bValue: false);
		m_objHUDBg.SetActive(value: false);
		m_objHUDBg.SetActive(value: true);
		m_NUF_GAME_HUD_MINI_MAP_RectTransform_width = m_rtMiniMap.rect.width;
		SetUserIconColor(bTeamA: true, new Color(1f, 1f, 1f));
		SetUserIconColor(bTeamA: false, new Color(1f, 1f, 1f));
		UnSelectUnitDeckAll();
		UnSelectShipSkillDeckAll();
		SetAttackPointUI(cNKMGameData);
		SetWaveUI(cNKMGameData);
		SetGageUI(cNKMGameData);
		SetMinimapUI(cNKMGameData);
		SetMainGageSkillCool(cNKMGameData);
		SetEnableRemainUnitCountUI(cNKMGameData);
		SetGuildDungeonData(cNKMGameData);
		int remainSupplyOfTeamA = m_GameClient.GetRemainSupplyOfTeamA();
		m_NKCUIHudRespawnGage.SetSupply(remainSupplyOfTeamA);
		SetNetworkLatencyLevel(1);
		if (NKCTutorialManager.IsTutorialDungeon(cNKMGameData.m_DungeonID))
		{
			HideHud(bEventControl: true);
			NKCUtil.SetGameobjectActive(m_objHide, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objHide, bValue: true);
		}
		if (m_GameHudPractice != null)
		{
			m_GameHudPractice.LoadComplete(cNKMGameData);
		}
		if (m_NKCGameHUDArtifact != null)
		{
			m_NKCGameHUDArtifact.SetUI(cNKMGameData);
		}
		if (m_NKCGameHudEmoticon != null)
		{
			m_NKCGameHudEmoticon.SetUI(cNKMGameData);
		}
		if (m_NKCGameHudCombo != null)
		{
			m_NKCGameHudCombo.SetUI(cNKMGameData, m_GameClient.m_MyTeam);
		}
	}

	public void SetNetworkLatencyLevel(int level)
	{
		if (!(m_lbNetworkLevel == null))
		{
			float num = 1f - 0.1f * ((float)level - 1f);
			Color color = m_lbNetworkLevel.color;
			color.g = num;
			color.b = num;
			m_lbNetworkLevel.color = color;
			m_lbNetworkLevel.text = $"{level}";
		}
	}

	public void SetUIByRuntimeData(NKMGameData cNKMGameData, NKMGameRuntimeData cNKMGameRuntimeData)
	{
		if (cNKMGameData != null && cNKMGameRuntimeData != null && cNKMGameData.IsPVE())
		{
			NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(cNKMGameData.m_DungeonID);
			if (dungeonTemplet != null && dungeonTemplet.m_DungeonTempletBase != null && dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE)
			{
				SetCurrentWaveUI(cNKMGameRuntimeData.m_WaveID);
			}
		}
	}

	public void UpdateRemainUnitCount(int count)
	{
		NKCUtil.SetLabelText(m_lbRemainUnitCount, NKCUtilString.GET_STRING_REMAIN_UNIT_COUNT_ONE_PARAM, count);
	}

	private void SetEnableRemainUnitCountUI(NKMGameData cNKMGameData)
	{
		bool bValue = false;
		if (cNKMGameData.IsPVE())
		{
			NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(cNKMGameData.m_DungeonID);
			if (dungeonTemplet != null && dungeonTemplet.m_DungeonTempletBase != null && (dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_RAID || dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID))
			{
				bValue = true;
				NKMGameTeamData teamData = cNKMGameData.GetTeamData(NKM_TEAM_TYPE.NTT_A1);
				if (teamData != null)
				{
					UpdateRemainUnitCount(teamData.m_listUnitData.Count - teamData.m_DeckData.GetListUnitDeckTombCount());
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objRemainUnitCount, bValue);
	}

	private void SetMainGageSkillCool(NKMGameData cNKMGameData)
	{
		NKMUnitData unitDataByUnitUID = cNKMGameData.m_NKMGameTeamDataB.GetUnitDataByUnitUID(cNKMGameData.m_NKMGameTeamDataB.m_LeaderUnitUID);
		NKMUnitTemplet nKMUnitTemplet = null;
		if (unitDataByUnitUID != null)
		{
			nKMUnitTemplet = NKMUnitManager.GetUnitTemplet(unitDataByUnitUID.m_UnitID);
		}
		if (nKMUnitTemplet != null && !NKMGame.IsPVP(cNKMGameData.m_NKM_GAME_TYPE))
		{
			m_NKCUIMainSkillCoolRight.SetUnit(nKMUnitTemplet, unitDataByUnitUID);
		}
	}

	private void SetMinimapUI(NKMGameData cNKMGameData)
	{
		if (cNKMGameData == null)
		{
			return;
		}
		bool bValue = true;
		if (cNKMGameData.IsPVE())
		{
			NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(cNKMGameData.m_DungeonID);
			if (dungeonTemplet != null && dungeonTemplet.m_DungeonTempletBase != null)
			{
				if (dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE || dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE)
				{
					if (dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE)
					{
						bValue = false;
					}
				}
				else if (dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_RAID || dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
				{
					bValue = false;
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_rtMiniMap, bValue);
	}

	private void SetUserInfoUI(NKMGameData cNKMGameData)
	{
		m_lbLeftUserName.text = "";
		m_lbRightUserName.text = "";
		NKMUnitData nKMUnitData = null;
		NKMUnitData nKMUnitData2 = null;
		if (cNKMGameData.IsPVE())
		{
			nKMUnitData = cNKMGameData.m_NKMGameTeamDataA.GetLeaderUnitData();
			nKMUnitData2 = cNKMGameData.m_NKMGameTeamDataB.GetLeaderUnitData();
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(cNKMGameData.m_DungeonID);
			if (dungeonTempletBase != null && (dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_RAID || dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID) && nKMUnitData == null)
			{
				nKMUnitData = cNKMGameData.m_NKMGameTeamDataA.GetFirstUnitData();
			}
			int num = 0;
			int num2 = 0;
			if (nKMUnitData != null)
			{
				num = nKMUnitData.m_UnitLevel;
			}
			if (nKMUnitData2 != null)
			{
				num2 = nKMUnitData2.m_UnitLevel;
			}
			if (dungeonTempletBase != null && dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE)
			{
				num2 = ((cNKMGameData.GetGameType() != NKM_GAME_TYPE.NGT_DIVE) ? dungeonTempletBase.m_DungeonLevel : (cNKMGameData.m_NKMGameTeamDataB.GetFirstUnitData()?.m_UnitLevel ?? dungeonTempletBase.m_DungeonLevel));
			}
			if (nKMUnitData != null)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(nKMUnitData.m_UnitID);
				if (unitTempletBase != null)
				{
					m_StringBuilder.Remove(0, m_StringBuilder.Length);
					m_StringBuilder.AppendFormat(NKCUtilString.GET_STRING_INGAME_USER_A_NAME_TWO_PARAM, num, unitTempletBase.GetUnitName());
					m_lbLeftUserName.text = m_StringBuilder.ToString();
				}
			}
			if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE)
			{
				NKCUtil.SetGameobjectActive(m_btnRightUser, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_btnRightUser, bValue: true);
				if (nKMUnitData2 != null)
				{
					NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(nKMUnitData2.m_UnitID);
					if (unitTempletBase2 != null)
					{
						NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(cNKMGameData.m_DungeonID);
						string arg = ((!string.IsNullOrEmpty(dungeonTemplet.m_BossUnitChangeName)) ? NKCStringTable.GetString(dungeonTemplet.m_BossUnitChangeName) : unitTempletBase2.GetUnitName());
						m_StringBuilder.Remove(0, m_StringBuilder.Length);
						m_StringBuilder.AppendFormat(NKCUtilString.GET_STRING_INGAME_USER_B_NAME_TWO_PARAM, num2, arg);
						m_lbRightUserName.text = m_StringBuilder.ToString();
					}
				}
				else
				{
					m_StringBuilder.Remove(0, m_StringBuilder.Length);
					m_StringBuilder.AppendFormat(NKCUtilString.GET_STRING_INGAME_USER_B_LEVEL_ONE_PARAM, num2);
					m_lbRightUserName.text = m_StringBuilder.ToString();
				}
			}
		}
		else
		{
			switch (m_GameClient.m_MyTeam)
			{
			case NKM_TEAM_TYPE.NTT_A1:
				nKMUnitData = cNKMGameData.m_NKMGameTeamDataA.GetLeaderUnitData();
				nKMUnitData2 = ((!IsFristUnitMain(m_GameClient.GetGameData().GetGameType())) ? cNKMGameData.m_NKMGameTeamDataB.GetLeaderUnitData() : cNKMGameData.m_NKMGameTeamDataB.GetFirstUnitData());
				if (nKMUnitData != null && NKMUnitManager.GetUnitTempletBase(nKMUnitData.m_UnitID) != null)
				{
					m_StringBuilder.Remove(0, m_StringBuilder.Length);
					m_StringBuilder.AppendFormat(NKCUtilString.GET_STRING_INGAME_TEAM_A_NAME_TWO_PARAM, cNKMGameData.m_NKMGameTeamDataA.m_UserLevel, NKCUtilString.GetUserNickname(cNKMGameData.m_NKMGameTeamDataA.m_UserNickname, bOpponent: false), cNKMGameData.m_NKMGameTeamDataA.m_Score);
					m_lbLeftUserName.text = m_StringBuilder.ToString();
				}
				if (nKMUnitData2 != null && NKMUnitManager.GetUnitTempletBase(nKMUnitData2.m_UnitID) != null)
				{
					m_StringBuilder.Remove(0, m_StringBuilder.Length);
					m_StringBuilder.AppendFormat(NKCUtilString.GET_STRING_INGAME_TEAM_B_NAME_TWO_PARAM, cNKMGameData.m_NKMGameTeamDataB.m_UserLevel, NKCUtilString.GetUserNickname(cNKMGameData.m_NKMGameTeamDataB.m_UserNickname, bOpponent: true), cNKMGameData.m_NKMGameTeamDataB.m_Score);
					m_lbRightUserName.text = m_StringBuilder.ToString();
				}
				break;
			case NKM_TEAM_TYPE.NTT_B1:
				nKMUnitData = cNKMGameData.m_NKMGameTeamDataB.GetLeaderUnitData();
				nKMUnitData2 = ((!IsFristUnitMain(m_GameClient.GetGameData().GetGameType())) ? cNKMGameData.m_NKMGameTeamDataA.GetLeaderUnitData() : cNKMGameData.m_NKMGameTeamDataA.GetFirstUnitData());
				if (nKMUnitData != null && NKMUnitManager.GetUnitTempletBase(nKMUnitData.m_UnitID) != null)
				{
					m_StringBuilder.Remove(0, m_StringBuilder.Length);
					m_StringBuilder.AppendFormat(NKCUtilString.GET_STRING_INGAME_TEAM_A_NAME_TWO_PARAM, cNKMGameData.m_NKMGameTeamDataA.m_UserLevel, NKCUtilString.GetUserNickname(cNKMGameData.m_NKMGameTeamDataA.m_UserNickname, bOpponent: true), cNKMGameData.m_NKMGameTeamDataA.m_Score);
					m_lbRightUserName.text = m_StringBuilder.ToString();
				}
				if (nKMUnitData2 != null && NKMUnitManager.GetUnitTempletBase(nKMUnitData2.m_UnitID) != null)
				{
					m_StringBuilder.Remove(0, m_StringBuilder.Length);
					m_StringBuilder.AppendFormat(NKCUtilString.GET_STRING_INGAME_TEAM_B_NAME_TWO_PARAM, cNKMGameData.m_NKMGameTeamDataB.m_UserLevel, NKCUtilString.GetUserNickname(cNKMGameData.m_NKMGameTeamDataB.m_UserNickname, bOpponent: false), cNKMGameData.m_NKMGameTeamDataB.m_Score);
					m_lbLeftUserName.text = m_StringBuilder.ToString();
				}
				break;
			}
		}
		Image imgLeftUser = m_imgLeftUser;
		NKCAssetResourceData unitFaceDeckAssetResourceData = GetUnitFaceDeckAssetResourceData(nKMUnitData);
		if (unitFaceDeckAssetResourceData != null)
		{
			imgLeftUser.sprite = unitFaceDeckAssetResourceData.GetAsset<Sprite>();
		}
		else
		{
			imgLeftUser.sprite = null;
		}
		imgLeftUser = m_imgRightUser;
		imgLeftUser.enabled = true;
		if (cNKMGameData.IsPVE())
		{
			NKMDungeonTempletBase dungeonTempletBase2 = NKMDungeonManager.GetDungeonTempletBase(cNKMGameData.m_DungeonID);
			if ((dungeonTempletBase2 != null && dungeonTempletBase2.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE) || cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE)
			{
				imgLeftUser.enabled = false;
			}
			else
			{
				unitFaceDeckAssetResourceData = GetUnitFaceDeckAssetResourceData(nKMUnitData2);
				if (unitFaceDeckAssetResourceData != null)
				{
					imgLeftUser.sprite = unitFaceDeckAssetResourceData.GetAsset<Sprite>();
				}
				else
				{
					imgLeftUser.sprite = null;
				}
			}
		}
		else
		{
			unitFaceDeckAssetResourceData = GetUnitFaceDeckAssetResourceData(nKMUnitData2);
			if (unitFaceDeckAssetResourceData != null)
			{
				imgLeftUser.sprite = unitFaceDeckAssetResourceData.GetAsset<Sprite>();
			}
			else
			{
				imgLeftUser.sprite = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty()?.GetAsset<Sprite>();
			}
		}
		if (nKMUnitData != null)
		{
			NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(nKMUnitData.m_UnitID);
			if (unitTempletBase3 != null)
			{
				if (m_imgLeftUserRole != null)
				{
					m_imgLeftUserRole.enabled = false;
				}
				if (m_imgLeftUserAttackType != null)
				{
					m_imgLeftUserAttackType.enabled = true;
					m_imgLeftUserAttackType.sprite = NKCResourceUtility.GetOrLoadUnitRoleAttackTypeIcon(unitTempletBase3, bSmall: true);
				}
			}
			else
			{
				if (m_imgLeftUserRole != null)
				{
					m_imgLeftUserRole.enabled = false;
				}
				m_imgLeftUserAttackType.enabled = true;
			}
		}
		else
		{
			if (m_imgLeftUserRole != null)
			{
				m_imgLeftUserRole.enabled = false;
			}
			m_imgLeftUserAttackType.enabled = true;
		}
		if (nKMUnitData2 != null)
		{
			NKMUnitTempletBase unitTempletBase4 = NKMUnitManager.GetUnitTempletBase(nKMUnitData2.m_UnitID);
			if (unitTempletBase4 != null && !unitTempletBase4.m_bHideBattleResult)
			{
				if (m_imgRightUserRole != null)
				{
					m_imgRightUserRole.enabled = false;
				}
				m_imgRightUserAttackType.enabled = true;
				m_imgRightUserAttackType.sprite = NKCResourceUtility.GetOrLoadUnitRoleAttackTypeIcon(unitTempletBase4, bSmall: true);
			}
			else
			{
				if (m_imgRightUserRole != null)
				{
					m_imgRightUserRole.enabled = false;
				}
				m_imgRightUserAttackType.enabled = false;
			}
		}
		else
		{
			if (m_imgRightUserRole != null)
			{
				m_imgRightUserRole.enabled = false;
			}
			m_imgRightUserAttackType.enabled = false;
		}
	}

	private NKCAssetResourceData GetUnitFaceDeckAssetResourceData(NKMUnitData unitData)
	{
		NKCAssetResourceData unitResource = NKCResourceUtility.GetUnitResource(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitData);
		if (unitResource == null)
		{
			return NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
		}
		return unitResource;
	}

	private NKCAssetResourceData GetUnitFaceDeckAssetResourceData(int unitID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase == null)
		{
			return NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
		}
		return NKCResourceUtility.GetUnitResource(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase);
	}

	public void StartGame(NKMGameRuntimeData cNKMGameRuntimeData)
	{
		SetAutoEnable();
		ChangeGameSpeedTypeUI(m_GameClient.GetGameRuntimeData().m_NKM_GAME_SPEED_TYPE);
		NKMGameRuntimeTeamData myRunTimeTeamData = m_GameClient.GetMyRunTimeTeamData();
		if (myRunTimeTeamData != null)
		{
			ChangeGameAutoSkillTypeUI(myRunTimeTeamData.m_NKM_GAME_AUTO_SKILL_TYPE);
		}
		RefreshClassGuide();
		SetTimeWarningFX(bActive: false);
		int multiply = ((m_GameClient.GetGameData() != null && m_GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE) ? 1 : m_GameClient.MultiplyReward);
		SetMultiply(multiply);
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BATTLE_AUTO_SKILL) && m_GameClient.GetMyRunTimeTeamData().m_NKM_GAME_AUTO_SKILL_TYPE != NKM_GAME_AUTO_SKILL_TYPE.NGST_OFF_HYPER)
		{
			m_GameClient.Send_Packet_GAME_AUTO_SKILL_CHANGE_REQ(NKM_GAME_AUTO_SKILL_TYPE.NGST_OFF_HYPER, bMsg: false);
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BATTLE_2X) && m_GameClient.GetGameRuntimeData().m_NKM_GAME_SPEED_TYPE != NKM_GAME_SPEED_TYPE.NGST_1)
		{
			m_GameClient.Send_Packet_GAME_SPEED_2X_REQ(NKM_GAME_SPEED_TYPE.NGST_1);
		}
		NKCUtil.SetGameobjectActive(m_btnPause, m_GameClient.GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE);
		SetLeftMenu(cNKMGameRuntimeData);
		TogglePause(cNKMGameRuntimeData.m_bPause, m_GameClient.GetGameData().m_bLocal);
		if (m_NKCGameHUDArtifact != null)
		{
			m_NKCGameHUDArtifact.PlayEffectNoticeAni();
		}
		if (!(m_killCount != null))
		{
			return;
		}
		if (NKCKillCountManager.IsKillCountDungeon(m_GameClient.GetGameData()))
		{
			NKCUtil.SetGameobjectActive(m_killCount, bValue: true);
			if (m_GameClient.GetIntrude())
			{
				m_killCount.SetKillCount("-");
			}
			else if (NKCPhaseManager.IsPhaseOnGoing() && !NKCPhaseManager.IsFirstStage(m_GameClient.GetGameData().m_DungeonID))
			{
				if (m_GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE)
				{
					m_killCount.SetKillCountForDefence(NKCKillCountManager.CurrentStageKillCount);
				}
				else
				{
					m_killCount.SetKillCount(NKCKillCountManager.CurrentStageKillCount);
				}
			}
			else if (m_GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE)
			{
				m_killCount.SetKillCountForDefence(0L);
			}
			else
			{
				m_killCount.SetKillCount(0L);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_killCount, bValue: false);
			m_killCount.SetKillCount("-");
		}
	}

	public int GetRespawnCountMax()
	{
		if (CurrentGameType == NKM_GAME_TYPE.NGT_PVP_EVENT)
		{
			NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
			if (eventPvpSeasonTemplet != null)
			{
				return eventPvpSeasonTemplet.RespawnCountMaxSameTime;
			}
		}
		else
		{
			NKMDungeonTemplet dungeonTemplet = m_GameClient.GetDungeonTemplet();
			if (dungeonTemplet != null)
			{
				return dungeonTemplet.m_DungeonTempletBase.m_RespawnCountMaxSameTime;
			}
		}
		return 0;
	}

	public void SetUnitCountMaxSameTime()
	{
		int respawnCountMax = GetRespawnCountMax();
		if (respawnCountMax > 0)
		{
			NKCUtil.SetGameobjectActive(m_lbUnitMaxCountSameTime, bValue: true);
			m_StringBuilder.Remove(0, m_StringBuilder.Length);
			m_StringBuilder.AppendFormat(NKCUtilString.GET_STRING_INGAME_UNIT_COUNT_MAX_SAME_TIME, m_GameClient.GetLiveUnitCount(m_GameClient.m_MyTeam), respawnCountMax);
			m_lbUnitMaxCountSameTime.text = m_StringBuilder.ToString();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbUnitMaxCountSameTime, bValue: false);
		}
	}

	public void EndGame()
	{
	}

	public void SetLeftMenu(NKMGameRuntimeData cNKMGameRuntimeData)
	{
		NKMGameRuntimeTeamData myRuntimeTeamData = cNKMGameRuntimeData.GetMyRuntimeTeamData(NKCScenManager.GetScenManager().GetGameClient().m_MyTeam);
		if (myRuntimeTeamData != null)
		{
			ToggleAutoRespawn(myRuntimeTeamData.m_bAutoRespawn);
		}
	}

	public void SetDeck(NKMGameData cNKMGameData)
	{
		NKMGameTeamData teamData = cNKMGameData.GetTeamData(CurrentViewTeamType);
		if (teamData != null)
		{
			SetDeck(teamData);
			SetAssistDeck(teamData);
		}
	}

	public void SetDeck(NKMGameTeamData cNKMGameTeamData)
	{
		for (int i = 0; i < cNKMGameTeamData.m_DeckData.GetListUnitDeckCount(); i++)
		{
			SetDeck(i, cNKMGameTeamData);
		}
		SetNextDeck(cNKMGameTeamData);
		SetAutoRespawnIndex(cNKMGameTeamData.m_DeckData.GetAutoRespawnIndex());
	}

	public void SetAssistDeck(NKMGameTeamData cNKMGameTeamData)
	{
		NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[5];
		if (cNKMGameTeamData.m_listAssistUnitData.Count <= 0 || cNKMGameTeamData.m_DeckData.GetAutoRespawnIndexAssist() < 0)
		{
			nKCGameHudDeckSlot.SetActive(bActive: false);
			return;
		}
		nKCGameHudDeckSlot.SetActive(bActive: true);
		NKMUnitData unitData = cNKMGameTeamData.m_listAssistUnitData[cNKMGameTeamData.m_DeckData.GetAutoRespawnIndexAssist()];
		nKCGameHudDeckSlot.SetDeckSprite(unitData, bLeader: false, bAssist: true, bAutoRespawn: false, 0.1f * (float)(cNKMGameTeamData.m_DeckData.GetListUnitDeckCount() - 5));
		ReturnDeck(5);
		nKCGameHudDeckSlot.SetEnemy(bSet: false);
	}

	public void SetDeck(int index, NKMGameTeamData cNKMGameTeamData)
	{
		long listUnitDeck = cNKMGameTeamData.m_DeckData.GetListUnitDeck(index);
		NKCGameHudDeckSlot obj = m_NKCUIHudDeck[index];
		obj.SetActive(bActive: true);
		bool bAutoRespawn = false;
		if (IsAutoRespawnUsable() && m_AutoRespawnIndex == index && cNKMGameTeamData.GetUnitDataByUnitUID(listUnitDeck) != null)
		{
			bAutoRespawn = true;
		}
		obj.SetDeckSprite(cNKMGameTeamData.GetUnitDataByUnitUID(listUnitDeck), cNKMGameTeamData.m_LeaderUnitUID == listUnitDeck, bAssist: false, bAutoRespawn, 0.1f * (float)(cNKMGameTeamData.m_DeckData.GetListUnitDeckCount() - index));
		ReturnDeck(index);
		obj.SetEnemy(bSet: false);
	}

	private void SetNextDeck(NKMGameTeamData cNKMGameTeamData)
	{
		m_NKCUIHudDeck[4].SetDeckSprite(cNKMGameTeamData.GetUnitDataByUnitUID(cNKMGameTeamData.m_DeckData.GetNextDeck()), cNKMGameTeamData.m_LeaderUnitUID == cNKMGameTeamData.m_DeckData.GetNextDeck(), bAssist: false, bAutoRespawn: false, 0f);
	}

	public void SetShipSkillDeck(NKMUnitData unitData)
	{
		for (int i = 0; i < m_NKCUIHudShipSkillDeck.Length; i++)
		{
			m_NKCUIHudShipSkillDeck[i].SetDeckSprite(unitData, null);
		}
		NKMUnitTempletBase nKMUnitTempletBase = null;
		if (unitData != null)
		{
			nKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		}
		if (nKMUnitTempletBase != null)
		{
			int num = 0;
			for (int j = 0; j < nKMUnitTempletBase.GetSkillCount(); j++)
			{
				NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(nKMUnitTempletBase, j);
				if (num < m_NKCUIHudShipSkillDeck.Length && (shipSkillTempletByIndex == null || shipSkillTempletByIndex.m_NKM_SKILL_TYPE != NKM_SKILL_TYPE.NST_PASSIVE))
				{
					m_NKCUIHudShipSkillDeck[num].SetDeckSprite(unitData, shipSkillTempletByIndex);
					num++;
				}
			}
		}
		for (int k = 0; k < m_NKCUIHudShipSkillDeck.Length; k++)
		{
			if (nKMUnitTempletBase != null && nKMUnitTempletBase.GetSkillCount() == 0)
			{
				m_NKCUIHudShipSkillDeck[k].SetActive(bActive: false);
			}
			else
			{
				ReturnShipSkillDeck(k);
			}
		}
	}

	public void SetTacticalCommandDeck(NKMGameTeamData cNKMGameTeamData)
	{
		for (int i = 0; i < m_NKCUIHudTacticalCommandDeck.Length; i++)
		{
			m_NKCUIHudTacticalCommandDeck[i].SetDeckSprite(null);
		}
		for (int j = 0; j < cNKMGameTeamData.m_listTacticalCommandData.Count && j < m_NKCUIHudTacticalCommandDeck.Length; j++)
		{
			NKMTacticalCommandData nKMTacticalCommandData = cNKMGameTeamData.m_listTacticalCommandData[j];
			if (nKMTacticalCommandData != null)
			{
				NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(nKMTacticalCommandData.m_TCID);
				if (tacticalCommandTempletByID == null || tacticalCommandTempletByID.m_NKM_TACTICAL_COMMAND_TYPE == NKM_TACTICAL_COMMAND_TYPE.NTCT_ACTIVE)
				{
					m_NKCUIHudTacticalCommandDeck[j].SetDeckSprite(tacticalCommandTempletByID);
				}
			}
		}
	}

	private void UpdateGameTimeUI()
	{
		int num = (int)m_GameClient.GetGameRuntimeData().m_fRemainGameTime;
		if (m_RemainGameTimeInt == num)
		{
			return;
		}
		m_RemainGameTimeInt = num;
		bool flag = false;
		m_StringBuilder.Remove(0, m_StringBuilder.Length);
		if (m_GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE)
		{
			float num2 = m_GameClient.GetDungeonTemplet().m_DungeonTempletBase.m_fGameTime - m_GameClient.GetGameRuntimeData().m_fRemainGameTime;
			if (num2 >= 0f)
			{
				m_StringBuilder.AppendFormat("{0}:{1}", ((int)(num2 / 60f)).ToString(), ((int)(num2 % 60f)).ToString("D2"));
			}
		}
		else
		{
			if (m_GameClient.GetDungeonType() == NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE)
			{
				if (m_GameClient.GetGameRuntimeData().m_fRemainGameTime > 10f)
				{
					flag = false;
					m_StringBuilder.AppendFormat("{0}:{1}", ((int)(m_GameClient.GetGameRuntimeData().m_fRemainGameTime / 60f)).ToString(), ((int)(m_GameClient.GetGameRuntimeData().m_fRemainGameTime % 60f)).ToString("D2"));
				}
				else if (m_GameClient.GetGameRuntimeData().m_fRemainGameTime >= 0f)
				{
					flag = true;
					m_StringBuilder.AppendFormat("{0}:{1}", ((int)(m_GameClient.GetGameRuntimeData().m_fRemainGameTime / 60f)).ToString(), ((int)(m_GameClient.GetGameRuntimeData().m_fRemainGameTime % 60f)).ToString("D2"));
				}
				else
				{
					flag = true;
					m_StringBuilder.AppendFormat("-{0}:{1}", ((int)((0f - m_GameClient.GetGameRuntimeData().m_fRemainGameTime) / 60f)).ToString(), ((int)((0f - m_GameClient.GetGameRuntimeData().m_fRemainGameTime) % 60f)).ToString("D2"));
				}
			}
			else if (m_GameClient.GetGameRuntimeData().m_fRemainGameTime > m_GameClient.GetGameData().m_fDoubleCostTime)
			{
				flag = false;
				m_StringBuilder.AppendFormat("{0}:{1}", ((int)(m_GameClient.GetGameRuntimeData().m_fRemainGameTime / 60f)).ToString(), ((int)(m_GameClient.GetGameRuntimeData().m_fRemainGameTime % 60f)).ToString("D2"));
			}
			else if (m_GameClient.GetGameRuntimeData().m_fRemainGameTime >= 0f)
			{
				flag = true;
				m_StringBuilder.AppendFormat("{0}:{1}", ((int)(m_GameClient.GetGameRuntimeData().m_fRemainGameTime / 60f)).ToString(), ((int)(m_GameClient.GetGameRuntimeData().m_fRemainGameTime % 60f)).ToString("D2"));
			}
			else
			{
				flag = true;
				m_StringBuilder.AppendFormat("-{0}:{1}", ((int)((0f - m_GameClient.GetGameRuntimeData().m_fRemainGameTime) / 60f)).ToString(), ((int)((0f - m_GameClient.GetGameRuntimeData().m_fRemainGameTime) % 60f)).ToString("D2"));
			}
			if (m_GameClient.GetGameRuntimeData().m_fRemainGameTime <= 10f && !m_bCountDownVoicePlayed)
			{
				m_bCountDownVoicePlayed = true;
				if (m_GameClient.GetMyTeamData() != null && m_GameClient.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY)
				{
					NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_COUNT_DOWN, m_GameClient.GetMyTeamData().m_Operator);
				}
			}
		}
		m_lbTimeLeft.text = m_StringBuilder.ToString();
		if (!flag)
		{
			m_lbTimeLeft.color = Color.white;
		}
		else
		{
			m_lbTimeLeft.color = Color.red;
		}
		m_NKCGameHudPause.IsPossibleSurrenderTime((int)m_GameClient.GetGameRuntimeData().GetGamePlayTime());
	}

	private void UpdateComboUI()
	{
		if (!(m_NKCGameHudCombo == null))
		{
			m_NKCGameHudCombo.UpdatePerFrame(GetCurrentViewTeamData().GetTC_Combo());
		}
	}

	private void UpdateAttackPointUI(float fDeltaTime)
	{
		if (m_GameClient.GetDungeonType() == NKM_DUNGEON_TYPE.NDT_DAMAGE_ACCRUE || m_GameClient.GetDungeonType() == NKM_DUNGEON_TYPE.NDT_RAID || m_GameClient.GetDungeonType() == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
		{
			if (m_fElapsedTimeForAP >= 1f)
			{
				m_fElapsedTimeForAP = 0f;
				m_fAttackPointLeftCurrValue = m_AttackPointLeftTargetValue;
				m_lbAttackPointNow.text = ((int)m_fAttackPointLeftCurrValue).ToString("N0");
			}
			if ((int)m_fAttackPointLeftCurrValue != m_AttackPointLeftTargetValue)
			{
				m_fAttackPointLeftCurrValue = Mathf.Lerp(m_fAttackPointLeftPivotValue, m_AttackPointLeftTargetValue, m_fElapsedTimeForAP);
				m_fElapsedTimeForAP += fDeltaTime * 2f;
				m_lbAttackPointNow.text = ((int)m_fAttackPointLeftCurrValue).ToString("N0");
			}
		}
	}

	private void UpdateMainGage(float fDeltaTime)
	{
		if (m_NKCUIMainHPGageAlly.IsVisibleMainGage())
		{
			m_NKCUIMainHPGageAlly.UpdateGage(fDeltaTime);
		}
		if (m_NKCUIMainHPGageEnemy.IsVisibleMainGage())
		{
			m_NKCUIMainHPGageEnemy.UpdateGage(fDeltaTime);
		}
		if (m_NKCUIMainHPGageAllyLong.IsVisibleMainGage())
		{
			m_NKCUIMainHPGageAllyLong.UpdateGage(fDeltaTime);
		}
		if (m_NKCUIMainHPGageEnemyLong.IsVisibleMainGage())
		{
			m_NKCUIMainHPGageEnemyLong.UpdateGage(fDeltaTime);
		}
	}

	protected NKMGameTeamData GetCurrentViewTeamData()
	{
		return m_GameClient.GetGameData().GetTeamData(CurrentViewTeamType);
	}

	private void UpdateDeck(float fDeltaTime)
	{
		NKMGameTeamData currentViewTeamData = GetCurrentViewTeamData();
		if (currentViewTeamData != null)
		{
			NKMTacticalCombo cNKMTacticalComboGoal = null;
			NKMTacticalCommandData tC_Combo = currentViewTeamData.GetTC_Combo();
			if (tC_Combo != null && tC_Combo.m_bCoolTimeOn)
			{
				cNKMTacticalComboGoal = tC_Combo.GetNKMTacticalComboGoal();
			}
			for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
			{
				NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
				if (!(nKCGameHudDeckSlot != null))
				{
					continue;
				}
				nKCGameHudDeckSlot.UpdateDeck(fDeltaTime);
				if (nKCGameHudDeckSlot.m_UnitData == null)
				{
					continue;
				}
				NKCUnitClient nKCUnitClient = null;
				if (nKCGameHudDeckSlot.m_UnitData.m_listGameUnitUID.Count > 0)
				{
					short gameUnitUID = nKCGameHudDeckSlot.m_UnitData.m_listGameUnitUID[0];
					nKCUnitClient = (NKCUnitClient)m_GameClient.GetUnit(gameUnitUID, bChain: true, bPool: true);
				}
				if (nKCUnitClient != null)
				{
					float fSkillCoolNow = 0f;
					float fSkillCoolMax = 0f;
					float fHyperSkillCoolNow = 0f;
					float fHyperSkillMax = 0f;
					NKMAttackStateData fastestCoolTimeSkillData = nKCUnitClient.GetFastestCoolTimeSkillData();
					if (fastestCoolTimeSkillData != null)
					{
						fSkillCoolNow = nKCUnitClient.GetStateCoolTime(fastestCoolTimeSkillData.m_StateName);
						fSkillCoolMax = nKCUnitClient.GetStateMaxCoolTime(fastestCoolTimeSkillData.m_StateName);
					}
					fastestCoolTimeSkillData = nKCUnitClient.GetFastestCoolTimeHyperSkillData();
					if (fastestCoolTimeSkillData != null)
					{
						fHyperSkillCoolNow = nKCUnitClient.GetStateCoolTime(fastestCoolTimeSkillData.m_StateName);
						fHyperSkillMax = nKCUnitClient.GetStateMaxCoolTime(fastestCoolTimeSkillData.m_StateName);
					}
					float fRespawnCostNow = m_NKCUIHudRespawnGage.GetRespawnCostGage();
					if (i == 5)
					{
						fRespawnCostNow = m_NKCUIHudRespawnGage.GetRespawnCostGageAssist();
					}
					nKCGameHudDeckSlot.SetDeckData(fRespawnCostNow, m_GameClient.IsGameUnitAllDie(nKCGameHudDeckSlot.m_UnitData.m_UnitUID), currentViewTeamData.m_LeaderUnitUID == nKCGameHudDeckSlot.m_UnitData.m_UnitUID, fSkillCoolNow, fSkillCoolMax, fHyperSkillCoolNow, fHyperSkillMax, cNKMTacticalComboGoal, currentViewTeamData.m_DeckData.GetRespawnLimitCount(nKCUnitClient.GetUnitData().m_UnitUID));
				}
			}
		}
		if (m_NKCUIHudRespawnGage.GetRespawnCostGage() >= 10f)
		{
			m_bCostFull = true;
		}
		else
		{
			m_bCostFull = false;
			m_fMaxCostTime = 0f;
		}
		if (m_GameClient.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY && m_bCostFull)
		{
			m_fMaxCostTime += Time.deltaTime;
			if (!m_bCostFullVoicePlayed)
			{
				if (m_fMaxCostTime >= 1f)
				{
					m_bCostFullVoicePlayed = true;
					NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_COST_FULL, currentViewTeamData.m_Operator);
					m_fMaxCostTime -= 1f;
				}
			}
			else if (m_fMaxCostTime >= 13f)
			{
				NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_COST_FULL, currentViewTeamData.m_Operator);
				m_fMaxCostTime -= 13f;
			}
		}
		bool flag = false;
		for (int j = 0; j < m_NKCUIHudShipSkillDeck.Length; j++)
		{
			NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[j];
			if (!(nKCUIHudShipSkillDeck != null) || nKCUIHudShipSkillDeck.m_NKMShipSkillTemplet == null)
			{
				continue;
			}
			NKMUnit unit = m_GameClient.GetUnit(nKCUIHudShipSkillDeck.m_GameUnitUID);
			if (unit == null)
			{
				continue;
			}
			if (nKCUIHudShipSkillDeck.m_NKMShipSkillTemplet.m_UnitStateName.Length > 1)
			{
				float stateCoolTime = unit.GetStateCoolTime(nKCUIHudShipSkillDeck.m_NKMShipSkillTemplet.m_UnitStateName);
				nKCUIHudShipSkillDeck.UpdateShipSkillDeck(fDeltaTime);
				nKCUIHudShipSkillDeck.SetDeckData(stateCoolTime);
				if (stateCoolTime == 0f)
				{
					flag = true;
				}
			}
			else
			{
				nKCUIHudShipSkillDeck.UpdateShipSkillDeck(fDeltaTime);
				nKCUIHudShipSkillDeck.SetDeckData(0f);
			}
		}
		if (m_GameClient.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY)
		{
			if (flag)
			{
				if (!m_bShipSkillReady)
				{
					m_bShipSkillReady = true;
					m_fShipSkillFullTime = 0f;
				}
			}
			else
			{
				m_bShipSkillReady = false;
			}
			if (m_bShipSkillReady)
			{
				if (!m_bShipSkillReadyVoicePlayed)
				{
					m_bShipSkillReadyVoicePlayed = true;
					NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_SHIP_SKILL, currentViewTeamData.m_Operator);
				}
				else
				{
					m_fShipSkillFullTime += Time.deltaTime;
					if (m_fShipSkillFullTime >= 17f)
					{
						NKCUIVoiceManager.PlayOperatorVoice(VOICE_TYPE.VT_SHIP_SKILL, currentViewTeamData.m_Operator);
						m_fShipSkillFullTime -= 17f;
					}
				}
			}
			else
			{
				m_bShipSkillReadyVoicePlayed = false;
			}
		}
		for (int k = 0; k < m_NKCUIHudTacticalCommandDeck.Length; k++)
		{
			NKCUIHudTacticalCommandDeck nKCUIHudTacticalCommandDeck = m_NKCUIHudTacticalCommandDeck[k];
			if (nKCUIHudTacticalCommandDeck == null || nKCUIHudTacticalCommandDeck.m_NKMTacticalCommandTemplet == null)
			{
				continue;
			}
			for (int l = 0; l < m_GameClient.GetMyTeamData().m_listTacticalCommandData.Count; l++)
			{
				if (nKCUIHudTacticalCommandDeck.m_NKMTacticalCommandTemplet.m_TCID == m_GameClient.GetMyTeamData().m_listTacticalCommandData[l].m_TCID)
				{
					nKCUIHudTacticalCommandDeck.SetDeckData(m_NKCUIHudRespawnGage.GetRespawnCostGage(), m_GameClient.GetMyTeamData().m_listTacticalCommandData[l]);
					break;
				}
			}
		}
	}

	public void UpdateHud(float fDeltaTime)
	{
		UpdateGameTimeUI();
		UpdateAttackPointUI(fDeltaTime);
		UpdateComboUI();
		m_NKCUIHudRespawnGage.UpdateGage(fDeltaTime);
		UpdateMainGage(fDeltaTime);
		ProcessHotkey();
		UpdateDeck(fDeltaTime);
		NKCUnitClient nKCUnitClient = (NKCUnitClient)m_GameClient.GetLiveBossUnit(m_GameClient.m_MyTeam);
		if (nKCUnitClient != null)
		{
			if (m_NKCUIMainHPGageAlly.IsVisibleMainGage())
			{
				m_NKCUIMainHPGageAlly.GetMainGageBuff().SetUnit(nKCUnitClient);
			}
			if (m_NKCUIMainHPGageAllyLong.IsVisibleMainGage())
			{
				m_NKCUIMainHPGageAllyLong.GetMainGageBuff().SetUnit(nKCUnitClient);
			}
		}
		nKCUnitClient = (NKCUnitClient)m_GameClient.GetLiveBossUnit(m_GameClient.GetGameData().GetEnemyTeamType(m_GameClient.m_MyTeam));
		if (nKCUnitClient != null)
		{
			if (m_NKCUIMainHPGageEnemy.IsVisibleMainGage())
			{
				m_NKCUIMainHPGageEnemy.GetMainGageBuff().SetUnit(nKCUnitClient);
			}
			if (m_NKCUIMainHPGageEnemyLong.IsVisibleMainGage())
			{
				m_NKCUIMainHPGageEnemyLong.GetMainGageBuff().SetUnit(nKCUnitClient);
			}
			m_NKCUIMainSkillCoolRight.SetCooltime(nKCUnitClient.GetSkillCoolRate(), nKCUnitClient.GetHyperSkillCoolRate());
		}
		NKMMapTemplet mapTemplet = m_GameClient.GetMapTemplet();
		Vector2 sizeDelta = m_rtMinimapCamPanel.sizeDelta;
		if (mapTemplet != null)
		{
			float num = NKCCamera.GetCameraSizeNow() * 2f * NKCCamera.GetCameraAspect();
			float newX = m_NUF_GAME_HUD_MINI_MAP_RectTransform_width * (num / (mapTemplet.m_fMaxX - mapTemplet.m_fMinX));
			sizeDelta.Set(newX, sizeDelta.y);
		}
		m_rtMinimapCamPanel.sizeDelta = sizeDelta;
		sizeDelta = m_rtMinimapCamPanel.anchoredPosition;
		if (mapTemplet != null)
		{
			float newX2 = (NKCCamera.GetPosNowX(bUseEdge: true) - mapTemplet.m_fMinX) / (mapTemplet.m_fMaxX - mapTemplet.m_fMinX) * m_NUF_GAME_HUD_MINI_MAP_RectTransform_width;
			sizeDelta.Set(newX2, sizeDelta.y);
		}
		m_rtMinimapCamPanel.anchoredPosition = sizeDelta;
		SetUnitCountMaxSameTime();
		UpdateHudAlert();
	}

	public float GetMiniMapRectWidth()
	{
		return m_NUF_GAME_HUD_MINI_MAP_RectTransform_width;
	}

	public void SetRespawnCost()
	{
		float respawnCostClient = m_GameClient.GetRespawnCostClient(CurrentViewTeamType);
		m_NKCUIHudRespawnGage.SetRespawnCost(respawnCostClient);
	}

	public void SetRespawnCostAssist()
	{
		float respawnCostAssistClient = m_GameClient.GetRespawnCostAssistClient(CurrentViewTeamType);
		m_NKCUIHudRespawnGage.SetRespawnCostAssist(respawnCostAssistClient);
	}

	public void PlayRespawnAddEvent(float value)
	{
		m_NKCUIHudRespawnGage.PlayRespawnAddEvent(value);
	}

	public int GetDeckIndex(GameObject go)
	{
		for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
		{
			NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
			if (nKCGameHudDeckSlot != null && nKCGameHudDeckSlot.gameObject == go)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetShipSkillDeckIndex(GameObject go)
	{
		for (int i = 0; i < m_NKCUIHudShipSkillDeck.Length; i++)
		{
			NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[i];
			if (nKCUIHudShipSkillDeck != null && nKCUIHudShipSkillDeck.gameObject == go)
			{
				return i;
			}
		}
		return -1;
	}

	public long GetDeckUnitUID(int index)
	{
		if (index < 0 || index > m_NKCUIHudDeck.Length)
		{
			return 0L;
		}
		NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[index];
		if (nKCGameHudDeckSlot != null && nKCGameHudDeckSlot.m_UnitData != null)
		{
			return nKCGameHudDeckSlot.m_UnitData.m_UnitUID;
		}
		return 0L;
	}

	public void MoveDeck(int index, float fPosX, float fPosY)
	{
		if (index >= 0 && index < m_NKCUIHudDeck.Length)
		{
			NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[index];
			if (nKCGameHudDeckSlot != null)
			{
				nKCGameHudDeckSlot.MoveDeck(fPosX, fPosY);
			}
		}
	}

	public void MoveShipSkillDeck(int index, float fPosX, float fPosY)
	{
		if (index >= 0 && index < m_NKCUIHudShipSkillDeck.Length)
		{
			NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[index];
			if (nKCUIHudShipSkillDeck != null)
			{
				nKCUIHudShipSkillDeck.MoveShipSkillDeck(fPosX, fPosY);
			}
		}
	}

	public void ReturnDeck(int index)
	{
		if (index >= 0 && index < m_NKCUIHudDeck.Length)
		{
			NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[index];
			if (nKCGameHudDeckSlot != null)
			{
				nKCGameHudDeckSlot.ReturnDeck();
			}
		}
	}

	public void ReturnShipSkillDeck(int index)
	{
		if (index >= 0 && index < m_NKCUIHudShipSkillDeck.Length)
		{
			NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[index];
			if (nKCUIHudShipSkillDeck != null)
			{
				nKCUIHudShipSkillDeck.ReturnShipSkillDeck();
			}
		}
	}

	public void ReturnTacticalCommandDeck(int index)
	{
		if (index >= 0 && index < m_NKCUIHudTacticalCommandDeck.Length)
		{
			m_NKCUIHudTacticalCommandDeck[index]?.ReturnTacticalCommandDeck();
		}
	}

	public void UseCompleteDeckByUnitUID(long unitUID, bool bReturnDeckActive = true)
	{
		for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
		{
			NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
			if (nKCGameHudDeckSlot != null && nKCGameHudDeckSlot.m_UnitData != null && nKCGameHudDeckSlot.m_UnitData.m_UnitUID == unitUID)
			{
				nKCGameHudDeckSlot.UseCompleteDeck(bReturnDeckActive);
			}
		}
	}

	public void UseCompleteDeckAssist(bool bReturnDeckActive = true)
	{
		NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[5];
		if (nKCGameHudDeckSlot != null && nKCGameHudDeckSlot.m_UnitData != null)
		{
			nKCGameHudDeckSlot.UseCompleteDeck(bReturnDeckActive);
		}
	}

	public void UseCompleteDeck()
	{
		for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
		{
			NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
			if (nKCGameHudDeckSlot != null && nKCGameHudDeckSlot.m_UnitData != null)
			{
				nKCGameHudDeckSlot.UseCompleteDeck();
			}
		}
	}

	public void ReturnDeckByShipSkillID(int shipSkill)
	{
		for (int i = 0; i < m_NKCUIHudShipSkillDeck.Length; i++)
		{
			NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[i];
			if (nKCUIHudShipSkillDeck != null && nKCUIHudShipSkillDeck.m_NKMShipSkillTemplet != null && nKCUIHudShipSkillDeck.m_NKMShipSkillTemplet.m_ShipSkillID == shipSkill)
			{
				ReturnShipSkillDeck(i);
			}
		}
	}

	public void ReturnDeckByTacticalCommandID(int TCID)
	{
		for (int i = 0; i < m_NKCUIHudTacticalCommandDeck.Length; i++)
		{
			NKCUIHudTacticalCommandDeck nKCUIHudTacticalCommandDeck = m_NKCUIHudTacticalCommandDeck[i];
			if (nKCUIHudTacticalCommandDeck != null && nKCUIHudTacticalCommandDeck.m_NKMTacticalCommandTemplet != null && nKCUIHudTacticalCommandDeck.m_NKMTacticalCommandTemplet.m_TCID == TCID)
			{
				ReturnTacticalCommandDeck(i);
			}
		}
	}

	public void SetDeck(NKMGameTeamData cNKMGameTeamData, int unitDeckIndex, long unitDeckUID)
	{
		if (m_SelectUnitDeckIndex == unitDeckIndex)
		{
			UnSelectUnitDeck();
		}
		NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[unitDeckIndex];
		NKCUnitClient nKCUnitClient = null;
		if (nKCGameHudDeckSlot.m_UnitData != null)
		{
			if (nKCGameHudDeckSlot.m_UnitData.m_UnitUID != unitDeckUID)
			{
				nKCGameHudDeckSlot.RespawnReady = false;
				nKCGameHudDeckSlot.RetreatReady = false;
			}
			nKCGameHudDeckSlot.SetActive(bActive: true);
			bool bAutoRespawn = false;
			if (IsAutoRespawnUsable() && m_AutoRespawnIndex == unitDeckIndex && cNKMGameTeamData.GetUnitDataByUnitUID(unitDeckUID) != null)
			{
				bAutoRespawn = true;
			}
			nKCGameHudDeckSlot.SetDeckSprite(cNKMGameTeamData.GetUnitDataByUnitUID(unitDeckUID), cNKMGameTeamData.m_LeaderUnitUID == unitDeckUID, bAssist: false, bAutoRespawn);
		}
		if (nKCGameHudDeckSlot.m_UnitData != null && nKCGameHudDeckSlot.m_UnitData.m_listGameUnitUID.Count > 0)
		{
			short gameUnitUID = nKCGameHudDeckSlot.m_UnitData.m_listGameUnitUID[0];
			nKCUnitClient = (NKCUnitClient)m_GameClient.GetUnit(gameUnitUID, bChain: true, bPool: true);
		}
		if (nKCUnitClient != null)
		{
			float fSkillCoolNow = 0f;
			float fSkillCoolMax = 0f;
			float fHyperSkillCoolNow = 0f;
			float fHyperSkillMax = 0f;
			NKMAttackStateData fastestCoolTimeSkillData = nKCUnitClient.GetFastestCoolTimeSkillData();
			if (fastestCoolTimeSkillData != null)
			{
				fSkillCoolNow = nKCUnitClient.GetStateCoolTime(fastestCoolTimeSkillData.m_StateName);
				fSkillCoolMax = nKCUnitClient.GetStateMaxCoolTime(fastestCoolTimeSkillData.m_StateName);
			}
			fastestCoolTimeSkillData = nKCUnitClient.GetFastestCoolTimeHyperSkillData();
			if (fastestCoolTimeSkillData != null)
			{
				fHyperSkillCoolNow = nKCUnitClient.GetStateCoolTime(fastestCoolTimeSkillData.m_StateName);
				fHyperSkillMax = nKCUnitClient.GetStateMaxCoolTime(fastestCoolTimeSkillData.m_StateName);
			}
			NKMTacticalCombo cNKMTacticalComboGoal = null;
			NKMTacticalCommandData tC_Combo = cNKMGameTeamData.GetTC_Combo();
			if (tC_Combo != null && tC_Combo.m_bCoolTimeOn)
			{
				cNKMTacticalComboGoal = tC_Combo.GetNKMTacticalComboGoal();
			}
			nKCGameHudDeckSlot.SetDeckData(m_NKCUIHudRespawnGage.GetRespawnCostGage(), m_GameClient.IsGameUnitAllDie(unitDeckUID), cNKMGameTeamData.m_LeaderUnitUID == unitDeckUID, fSkillCoolNow, fSkillCoolMax, fHyperSkillCoolNow, fHyperSkillMax, cNKMTacticalComboGoal, cNKMGameTeamData.m_DeckData.GetRespawnLimitCount(nKCUnitClient.GetUnitData().m_UnitUID));
		}
		SetNextDeck(cNKMGameTeamData);
	}

	public void UseDeck(int unitDeckIndex, bool bRetreat)
	{
		UnSelectShipSkillDeck();
		if (m_SelectUnitDeckIndex != unitDeckIndex)
		{
			UnSelectUnitDeck();
		}
		m_NKCUIHudDeck[unitDeckIndex].UseDeck(bRetreat);
		dOnUseDeck?.Invoke(unitDeckIndex);
	}

	public void UseShipSkillDeck(int shipSkillDeckIndex)
	{
		UnSelectUnitDeck();
		if (m_SelectShipSkillDeckIndex != shipSkillDeckIndex)
		{
			UnSelectShipSkillDeck();
		}
		m_NKCUIHudShipSkillDeck[shipSkillDeckIndex].SetActive(bActive: false);
		dOnUseSkill?.Invoke(shipSkillDeckIndex);
	}

	public NKCUIHudShipSkillDeck GetShipSkillDeck(int index)
	{
		return m_NKCUIHudShipSkillDeck[index];
	}

	public void SetSyncDeck(NKMGameSyncData_Deck deckSyncDeckData)
	{
		if (CurrentViewTeamType == deckSyncDeckData.m_NKM_TEAM_TYPE)
		{
			NKMGameTeamData currentViewTeamData = GetCurrentViewTeamData();
			SetAutoRespawnIndex(currentViewTeamData.m_DeckData.GetAutoRespawnIndex());
			SetDeck(currentViewTeamData, deckSyncDeckData.m_UnitDeckIndex, deckSyncDeckData.m_UnitDeckUID);
		}
	}

	public void SetAutoRespawnIndex(int index)
	{
		m_AutoRespawnIndex = index;
		for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
		{
			NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
			if (nKCGameHudDeckSlot != null)
			{
				if (IsAutoRespawnUsable() && m_AutoRespawnIndex == i && nKCGameHudDeckSlot.m_UnitData != null)
				{
					nKCGameHudDeckSlot.SetAuto(bSet: true);
				}
				else
				{
					nKCGameHudDeckSlot.SetAuto(bSet: false);
				}
			}
		}
	}

	public void SetMainGage(bool bGageA, NKMUnit unit, bool bLong = false)
	{
		if (bLong)
		{
			if (bGageA)
			{
				m_NKCUIMainHPGageAllyLong.SetMainGage(unit);
			}
			else
			{
				m_NKCUIMainHPGageEnemyLong.SetMainGage(unit);
			}
		}
		else if (bGageA)
		{
			m_NKCUIMainHPGageAlly.SetMainGage(unit);
		}
		else
		{
			m_NKCUIMainHPGageEnemy.SetMainGage(unit);
		}
	}

	public void SetMainGage(bool bGageA, float fHP, float maxHP, float barrierHP, bool bLong = false)
	{
		if (bLong)
		{
			if (bGageA)
			{
				m_NKCUIMainHPGageAllyLong.SetMainGage(fHP, maxHP, barrierHP);
			}
			else
			{
				m_NKCUIMainHPGageEnemyLong.SetMainGage(fHP, maxHP, barrierHP);
			}
		}
		else if (bGageA)
		{
			m_NKCUIMainHPGageAlly.SetMainGage(fHP, maxHP, barrierHP);
		}
		else
		{
			m_NKCUIMainHPGageEnemy.SetMainGage(fHP, maxHP, barrierHP);
		}
	}

	public void SetRageMode(bool bRageMode, bool isMyTeam)
	{
		if (isMyTeam)
		{
			if (m_NKCUIMainHPGageAlly != null)
			{
				m_NKCUIMainHPGageAlly.SetRageMode(bRageMode);
			}
			NKCUtil.SetGameobjectActive(m_objLeftUserRage, bRageMode);
		}
		else
		{
			if (m_NKCUIMainHPGageEnemy != null)
			{
				m_NKCUIMainHPGageEnemy.SetRageMode(bRageMode);
			}
			NKCUtil.SetGameobjectActive(m_objRightUserRage, bRageMode);
		}
	}

	public void SetDeadlineMode(int buffLevel, string updateBuffDesc)
	{
		m_DeadlineBuffLevel = buffLevel;
		NKCUtil.SetGameobjectActive(m_csbtnDeadlineBuff, bValue: true);
		NKCUtil.SetLabelText(m_lbDeadLineBuffLevel, buffLevel.ToString());
		m_strDeadLineBuffString = updateBuffDesc;
	}

	public void SetMessage(string str, float time = -1f)
	{
		NKCUtil.SetGameobjectActive(m_objHUDMessage, bValue: true);
		m_lbHUDMessage.text = str;
		m_animatorHUDMessage.Play("BASE", -1, 0f);
		if (time > 0f)
		{
			AnimationClip animationClip = m_animatorHUDMessage.runtimeAnimatorController.animationClips[0];
			if (animationClip != null)
			{
				m_animatorHUDMessage.speed = animationClip.length / time;
			}
			else
			{
				m_animatorHUDMessage.speed = 1f;
			}
		}
		else
		{
			m_animatorHUDMessage.speed = 1f;
		}
	}

	public void SetTimeOver(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objTimeOver, bSet);
	}

	public void ToggleAutoRespawn(bool bOn)
	{
		if (!(m_animatorAutoRespawn == null))
		{
			if (bOn)
			{
				m_animatorAutoRespawn.Play("ON_START", -1, 0f);
			}
			else
			{
				m_animatorAutoRespawn.Play("OFF_START", -1, 0f);
			}
		}
	}

	public void TouchDownDeck(int index)
	{
		NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[index];
		if (nKCGameHudDeckSlot != null)
		{
			nKCGameHudDeckSlot.TouchDown();
		}
		UnSelectShipSkillDeck();
		if (m_SelectUnitDeckIndex != index)
		{
			UnSelectUnitDeck();
		}
	}

	public void TouchUpDeck(int index, bool bUseTouchScale)
	{
		for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
		{
			NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
			if (!(nKCGameHudDeckSlot != null) || nKCGameHudDeckSlot.m_UnitData == null)
			{
				continue;
			}
			if (i == index)
			{
				if (m_SelectUnitDeckIndex == index)
				{
					UnSelectUnitDeck();
				}
				else if (m_GameClient.EnableControlByGameType())
				{
					m_SelectUnitDeckIndex = index;
					nKCGameHudDeckSlot.TouchSelectUnitDeck(bUseTouchScale);
				}
			}
			else
			{
				if (i == 5 && m_GameClient.GetMyTeamData().m_DeckData.GetAutoRespawnIndexAssist() == -1)
				{
					nKCGameHudDeckSlot.ReturnDeck(bReturnDeckActive: false);
				}
				else
				{
					nKCGameHudDeckSlot.ReturnDeck();
				}
				nKCGameHudDeckSlot.TouchUnSelectUnitDeck();
			}
		}
	}

	public void TouchDownShipSkillDeck(int index)
	{
		NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[index];
		if (nKCUIHudShipSkillDeck != null)
		{
			nKCUIHudShipSkillDeck.TouchDown();
		}
		UnSelectUnitDeck();
		if (m_SelectShipSkillDeckIndex != index)
		{
			UnSelectShipSkillDeck();
		}
	}

	public void TouchUpShipSkillDeck(int index)
	{
		for (int i = 0; i < m_NKCUIHudShipSkillDeck.Length; i++)
		{
			NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[i];
			if (!(nKCUIHudShipSkillDeck != null))
			{
				continue;
			}
			if (i == index)
			{
				nKCUIHudShipSkillDeck.TouchUp();
				if (m_SelectShipSkillDeckIndex == index)
				{
					UnSelectShipSkillDeck();
				}
				else if (nKCUIHudShipSkillDeck.m_NKMShipSkillTemplet != null && nKCUIHudShipSkillDeck.m_NKMShipSkillTemplet.m_NKM_SKILL_TYPE != NKM_SKILL_TYPE.NST_PASSIVE && m_GameClient.EnableControlByGameType())
				{
					m_SelectShipSkillDeckIndex = index;
					nKCUIHudShipSkillDeck.TouchSelectShipSkillDeck();
				}
			}
			else
			{
				nKCUIHudShipSkillDeck.TouchUnSelectShipSkillDeck();
			}
		}
	}

	public void UnSelectUnitDeck()
	{
		if (m_SelectUnitDeckIndex >= 0 && m_SelectUnitDeckIndex < m_NKCUIHudDeck.Length)
		{
			NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[m_SelectUnitDeckIndex];
			if (nKCGameHudDeckSlot != null)
			{
				nKCGameHudDeckSlot.TouchUnSelectUnitDeck();
			}
			m_SelectUnitDeckIndex = -1;
		}
	}

	public void UnSelectShipSkillDeck()
	{
		if (m_SelectShipSkillDeckIndex >= 0 && m_SelectShipSkillDeckIndex < m_NKCUIHudDeck.Length)
		{
			NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[m_SelectShipSkillDeckIndex];
			if (nKCUIHudShipSkillDeck != null)
			{
				nKCUIHudShipSkillDeck.TouchUnSelectShipSkillDeck();
			}
			m_SelectShipSkillDeckIndex = -1;
		}
	}

	public void UnSelectUnitDeckAll()
	{
		for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
		{
			NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
			if (nKCGameHudDeckSlot != null)
			{
				nKCGameHudDeckSlot.TouchUnSelectUnitDeck();
			}
		}
		m_SelectUnitDeckIndex = -1;
	}

	public void UnSelectShipSkillDeckAll()
	{
		for (int i = 0; i < m_NKCUIHudShipSkillDeck.Length; i++)
		{
			NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[i];
			if (nKCUIHudShipSkillDeck != null)
			{
				nKCUIHudShipSkillDeck.TouchUnSelectShipSkillDeck();
			}
		}
		m_SelectShipSkillDeckIndex = -1;
	}

	public void ShowTooltip(int index, Vector2 touchPos)
	{
		NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[index];
		if (nKCUIHudShipSkillDeck != null)
		{
			NKCUITooltip.Instance.Open(nKCUIHudShipSkillDeck.m_NKMShipSkillTemplet, touchPos);
		}
	}

	public void CloseTooltip()
	{
		if (NKCUITooltip.IsInstanceOpen)
		{
			NKCUITooltip.Instance.Close();
		}
	}

	public void PracticeGoBack()
	{
		if (m_GameHudPractice != null)
		{
			m_GameHudPractice.PracticeGoBack();
		}
	}

	public virtual void SendGameSpeed2X()
	{
		if (m_eMode == HUDMode.Replay || !NKMGame.IsPVPSync(m_GameClient.GetGameData().GetGameType()))
		{
			NKM_GAME_SPEED_TYPE nKM_GAME_SPEED_TYPE = m_NKM_GAME_SPEED_TYPE;
			nKM_GAME_SPEED_TYPE = m_NKM_GAME_SPEED_TYPE switch
			{
				NKM_GAME_SPEED_TYPE.NGST_1 => NKM_GAME_SPEED_TYPE.NGST_2, 
				NKM_GAME_SPEED_TYPE.NGST_2 => NKM_GAME_SPEED_TYPE.NGST_1, 
				NKM_GAME_SPEED_TYPE.NGST_3 => NKM_GAME_SPEED_TYPE.NGST_05, 
				NKM_GAME_SPEED_TYPE.NGST_05 => NKM_GAME_SPEED_TYPE.NGST_1, 
				_ => NKM_GAME_SPEED_TYPE.NGST_1, 
			};
			HUDMode eMode = m_eMode;
			if (eMode == HUDMode.Normal || eMode != HUDMode.Replay)
			{
				m_GameClient.Send_Packet_GAME_SPEED_2X_REQ(nKM_GAME_SPEED_TYPE);
				return;
			}
			NKCReplayMgr.GetNKCReplaMgr().SetPlayingGameSpeedType(nKM_GAME_SPEED_TYPE);
			ChangeGameSpeedTypeUI(nKM_GAME_SPEED_TYPE);
		}
	}

	public void SendGameAutoSkillChange()
	{
		switch (m_eAutoSkillType)
		{
		case NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO:
			m_GameClient.Send_Packet_GAME_AUTO_SKILL_CHANGE_REQ(NKM_GAME_AUTO_SKILL_TYPE.NGST_OFF_HYPER);
			break;
		case NKM_GAME_AUTO_SKILL_TYPE.NGST_OFF_HYPER:
			m_GameClient.Send_Packet_GAME_AUTO_SKILL_CHANGE_REQ(NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO);
			break;
		default:
			m_GameClient.Send_Packet_GAME_AUTO_SKILL_CHANGE_REQ(NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO);
			break;
		}
	}

	protected virtual bool IsChangeGameSpeedVisible()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BATTLE_2X))
		{
			return false;
		}
		switch (m_eMode)
		{
		case HUDMode.Normal:
			if (m_GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
			{
				return false;
			}
			if (m_GameClient.GetGameData().IsPVP() && NKMGame.IsPVPSync(m_GameClient.GetGameData().GetGameType()))
			{
				return false;
			}
			break;
		case HUDMode.Observer:
			return false;
		}
		return true;
	}

	public void ChangeGameSpeedTypeUI(NKM_GAME_SPEED_TYPE eNKM_GAME_SPEED_TYPE)
	{
		m_NKM_GAME_SPEED_TYPE = eNKM_GAME_SPEED_TYPE;
		bool flag = IsChangeGameSpeedVisible();
		NKCUtil.SetGameobjectActive(m_csbtnGameSpeed, flag);
		if (flag)
		{
			m_imgGameSpeed1.color = NKCUtil.GetColor("#FFFFFF");
			switch (m_NKM_GAME_SPEED_TYPE)
			{
			case NKM_GAME_SPEED_TYPE.NGST_1:
				m_imgGameSpeed1.gameObject.SetActive(value: true);
				m_imgGameSpeed2.gameObject.SetActive(value: false);
				m_imgGameSpeed3.gameObject.SetActive(value: false);
				m_imgGameSpeed05.gameObject.SetActive(value: false);
				break;
			case NKM_GAME_SPEED_TYPE.NGST_2:
				m_imgGameSpeed1.gameObject.SetActive(value: false);
				m_imgGameSpeed2.gameObject.SetActive(value: true);
				m_imgGameSpeed3.gameObject.SetActive(value: false);
				m_imgGameSpeed05.gameObject.SetActive(value: false);
				break;
			case NKM_GAME_SPEED_TYPE.NGST_3:
				m_imgGameSpeed1.gameObject.SetActive(value: false);
				m_imgGameSpeed2.gameObject.SetActive(value: false);
				m_imgGameSpeed3.gameObject.SetActive(value: true);
				m_imgGameSpeed05.gameObject.SetActive(value: false);
				break;
			case NKM_GAME_SPEED_TYPE.NGST_05:
				m_imgGameSpeed1.gameObject.SetActive(value: false);
				m_imgGameSpeed2.gameObject.SetActive(value: false);
				m_imgGameSpeed3.gameObject.SetActive(value: false);
				m_imgGameSpeed05.gameObject.SetActive(value: true);
				break;
			case NKM_GAME_SPEED_TYPE.NGST_10:
				m_imgGameSpeed1.gameObject.SetActive(value: false);
				m_imgGameSpeed2.gameObject.SetActive(value: false);
				m_imgGameSpeed3.gameObject.SetActive(value: true);
				m_imgGameSpeed05.gameObject.SetActive(value: false);
				break;
			default:
				m_imgGameSpeed1.gameObject.SetActive(value: true);
				m_imgGameSpeed2.gameObject.SetActive(value: false);
				m_imgGameSpeed3.gameObject.SetActive(value: false);
				m_imgGameSpeed05.gameObject.SetActive(value: false);
				break;
			}
		}
	}

	protected virtual bool IsAutoSkillVisible()
	{
		HUDMode eMode = m_eMode;
		if (eMode == HUDMode.Normal || (uint)(eMode - 1) > 1u)
		{
			bool result = true;
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.BATTLE_AUTO_SKILL))
			{
				result = false;
			}
			if (m_GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
			{
				result = false;
			}
			return result;
		}
		return false;
	}

	public void ChangeGameAutoSkillTypeUI(NKM_GAME_AUTO_SKILL_TYPE eNKM_GAME_AUTO_SKILL_TYPE)
	{
		m_eAutoSkillType = eNKM_GAME_AUTO_SKILL_TYPE;
		bool flag = IsAutoSkillVisible();
		NKCUtil.SetGameobjectActive(m_btnAutoHyper, flag);
		if (flag)
		{
			NKCUtil.SetGameobjectActive(m_objAutoHyperOn, m_eAutoSkillType == NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO);
			NKCUtil.SetGameobjectActive(m_objAutoHyperOff, m_eAutoSkillType == NKM_GAME_AUTO_SKILL_TYPE.NGST_OFF_HYPER);
		}
	}

	public void DevHideHud(bool bHide)
	{
		if (bHide)
		{
			m_GameClient.SetShowUI(bShowUI: false, bDev: true);
			NKCUtil.SetGameobjectActive(m_objHide, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnhide, bValue: false);
		}
		else
		{
			HUD_UNHIDE();
		}
	}

	private void OnBtnHideHud()
	{
		HUD_HIDE();
	}

	public void HUD_HIDE(bool bHideCompletly = false)
	{
		m_GameClient.SetShowUI(bShowUI: false, bDev: true);
		NKCUtil.SetGameobjectActive(m_objUnhide, !bHideCompletly);
		NKCUtil.SetGameobjectActive(m_objHide, bValue: false);
		if (NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_NUF_BEFORE_HUD_CONTROL_EFFECT() != null && NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_NUF_AFTER_HUD_EFFECT() != null)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_NUF_BEFORE_HUD_CONTROL_EFFECT()
				.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_NUF_AFTER_HUD_EFFECT()
				.transform, worldPositionStays: false);
			}
			if (bHideCompletly)
			{
				NKCUtil.SetGameobjectActive(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_AFTER_HUD_EFFECT(), bValue: false);
				NKCUtil.SetGameobjectActive(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_BEFORE_HUD_EFFECT(), bValue: false);
				NKCUtil.SetGameobjectActive(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_BEFORE_HUD_CONTROL_EFFECT_ANCHOR(), bValue: false);
				NKCUtil.SetGameobjectActive(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_BEFORE_HUD_CONTROL_EFFECT(), bValue: false);
			}
		}

		public void HUD_UNHIDE()
		{
			m_GameClient.SetShowUI(bShowUI: true, bDev: true);
			NKCUtil.SetGameobjectActive(m_objUnhide, bValue: false);
			NKCUtil.SetGameobjectActive(m_objHide, bValue: true);
			if (NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_NUF_BEFORE_HUD_CONTROL_EFFECT() != null && NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_NUF_BEFORE_HUD_CONTROL_EFFECT_ANCHOR() != null)
			{
				NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_BEFORE_HUD_CONTROL_EFFECT()
					.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_BEFORE_HUD_CONTROL_EFFECT_ANCHOR()
					.transform, worldPositionStays: false);
				}
				NKCUtil.SetGameobjectActive(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_AFTER_HUD_EFFECT(), bValue: true);
				NKCUtil.SetGameobjectActive(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_BEFORE_HUD_EFFECT(), bValue: true);
				NKCUtil.SetGameobjectActive(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_BEFORE_HUD_CONTROL_EFFECT_ANCHOR(), bValue: true);
				NKCUtil.SetGameobjectActive(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_BEFORE_HUD_CONTROL_EFFECT(), bValue: true);
			}

			public virtual void SetNetworkWeak(bool bOn)
			{
				NKCUtil.SetGameobjectActive(m_objNetworkWeak, bOn);
			}

			private bool IsAutoRespawnUsable()
			{
				if (!IsAutoRespawnVisible())
				{
					return false;
				}
				if (!m_GameClient.CanUseAutoRespawn(NKCScenManager.CurrentUserData()))
				{
					return false;
				}
				return true;
			}

			protected virtual bool IsAutoRespawnVisible()
			{
				HUDMode eMode = m_eMode;
				if (eMode == HUDMode.Normal || (uint)(eMode - 1) > 1u)
				{
					if (!NKCContentManager.IsContentsUnlocked(ContentsType.BATTLE_AUTO_RESPAWN))
					{
						return false;
					}
					if (m_GameClient.GetDungeonTemplet() != null && !m_GameClient.GetDungeonTemplet().m_bCanUseAuto)
					{
						return false;
					}
					if (m_GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_TUTORIAL || m_GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
					{
						return false;
					}
					return true;
				}
				return false;
			}

			public void RefreshClassGuide()
			{
				NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
				NKCUtil.SetGameobjectActive(m_objClassGuide, gameOptionData?.UseClassGuide ?? true);
			}

			public void OnClickedNetworkLevel(PointerEventData eventData)
			{
				NKCUITooltip.Instance.Open(NKCUISlot.eSlotMode.Etc, NKCUtilString.GET_STRING_TOOLTIP_ETC_NETWORK_TITLE, NKCUtilString.GET_STRING_TOOLTIP_ETC_NETWORK_DESC, eventData.position);
			}

			public void EnqueueTimeWarningAlarm()
			{
				NKCGameHudAlertEffect alert = new NKCGameHudAlertEffect("AB_FX_UI_RESOURCE_DOUBLE", "AB_FX_UI_RESOURCE_DOUBLE", NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_EFFECT, "AB_FX_UI_RESOURCE_DOUBLE");
				EnqueueHudAlert(alert);
			}

			public void SetTimeWarningFX(bool bActive)
			{
				NKCUtil.SetGameobjectActive(m_objHUDBgFx, bActive);
			}

			public int GetRespawnCost(NKMUnitStatTemplet cNKMUnitStatTemplet, bool bLeader)
			{
				return m_GameClient.GetRespawnCost(cNKMUnitStatTemplet, bLeader, CurrentViewTeamType);
			}

			public bool IsBanUnit(int unitID)
			{
				if (IsNotAllowUpBan())
				{
					return false;
				}
				return m_GameClient.IsBanUnit(unitID);
			}

			public bool IsUpUnit(int unitID)
			{
				if (IsNotAllowUpBan())
				{
					return false;
				}
				return m_GameClient.IsUpUnit(unitID);
			}

			private bool IsFristUnitMain(NKM_GAME_TYPE game_type)
			{
				if (game_type == NKM_GAME_TYPE.NGT_ASYNC_PVP || game_type - 20 <= NKM_GAME_TYPE.NGT_PRACTICE)
				{
					return true;
				}
				return false;
			}

			private bool IsNotAllowUpBan()
			{
				if (m_GameClient != null)
				{
					NKM_GAME_TYPE nKM_GAME_TYPE = m_GameClient.GetGameData().m_NKM_GAME_TYPE;
					if (nKM_GAME_TYPE - 20 <= NKM_GAME_TYPE.NGT_PRACTICE)
					{
						return true;
					}
					return false;
				}
				return false;
			}

			public void PlayDangerMsg(string msg)
			{
				if (m_NKCUIDangerMessage != null)
				{
					m_NKCUIDangerMessage.Play(msg);
				}
			}

			public void StopDangerMsg()
			{
				if (m_NKCUIDangerMessage != null)
				{
					m_NKCUIDangerMessage.Stop();
				}
			}

			public bool IsShowDangerMsg()
			{
				if (m_NKCUIDangerMessage == null)
				{
					return false;
				}
				return m_NKCUIDangerMessage.gameObject.activeInHierarchy;
			}

			private void UpdateMultiplyAlert()
			{
				if (m_GameClient.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY && bReservedMultiply)
				{
					bReservedMultiply = false;
					if (m_GameClient.MultiplyReward > 1 && m_NKCUIGameHUDMultiplyReward != null)
					{
						NKCUtil.SetGameobjectActive(m_NKCUIGameHUDMultiplyReward, bValue: true);
					}
				}
			}

			public void OnClickedDeadlineBuff(PointerEventData eventData)
			{
				NKCUITooltip.Instance.Open(NKCUISlot.eSlotMode.Etc, NKCUtilString.GET_STRING_TOOLTIP_DEADLINE_BUFF_TITLE, m_strDeadLineBuffString, eventData.position);
			}

			public void PrepareAlerts(NKMGameData gameData)
			{
				if (gameData.GetGameType() == NKM_GAME_TYPE.NGT_PHASE)
				{
					EnqueuePhaseAlarm(NKCPhaseManager.PhaseModeState.phaseIndex + 1);
				}
				foreach (int key in gameData.m_BattleConditionIDs.Keys)
				{
					NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(key);
					if (templetByID != null && templetByID.UseContentsType == NKMBattleConditionTemplet.USE_CONTENT_TYPE.UCT_BATTLE_CONDITION && !templetByID.m_bHide && templetByID.ActiveTimeLeft <= 0f)
					{
						NKCGameHudAlarmCommonControl alert = new NKCGameHudAlarmCommonControl(m_AlertEnv, templetByID.BattleCondName_Translated, templetByID.BattleCondDesc_Translated, templetByID.BattleCondIngameIcon);
						EnqueueHudAlert(alert);
					}
				}
			}

			private void EnqueueHudAlert(IGameHudAlert alert)
			{
				m_qHudAlert.Enqueue(alert);
			}

			private void UpdateHudAlert()
			{
				if (m_GameClient.GetGameRuntimeData().m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_PLAY)
				{
					if (m_currentHudAlert != null && m_currentHudAlert.IsFinished())
					{
						m_currentHudAlert.OnCleanup();
						m_currentHudAlert = null;
					}
					if (m_currentHudAlert == null && m_qHudAlert.Count > 0)
					{
						m_currentHudAlert = m_qHudAlert.Dequeue();
						m_currentHudAlert.OnStart();
					}
					if (m_currentHudAlert != null)
					{
						m_currentHudAlert.OnUpdate();
					}
					else
					{
						UpdateMultiplyAlert();
					}
				}
			}

			private void CleanupAllHudAlert()
			{
				if (m_currentHudAlert != null)
				{
					m_currentHudAlert.OnCleanup();
					m_currentHudAlert = null;
				}
				m_qHudAlert.Clear();
			}

			private bool IsAlertProcessFinished()
			{
				if (m_currentHudAlert == null)
				{
					return m_qHudAlert.Count == 0;
				}
				return false;
			}

			public void EnqueueHudBattleConditionAlert(NKMBattleConditionTemplet bcTemplet)
			{
				if (bcTemplet != null && bcTemplet.UseContentsType == NKMBattleConditionTemplet.USE_CONTENT_TYPE.UCT_BATTLE_CONDITION && !bcTemplet.m_bHide)
				{
					NKCGameHudAlarmCommonControl alert = new NKCGameHudAlarmCommonControl(m_AlertEnv, bcTemplet.BattleCondName_Translated, bcTemplet.BattleCondDesc_Translated, bcTemplet.BattleCondIngameIcon);
					EnqueueHudAlert(alert);
				}
			}

			public void EnqueuePhaseAlarm(int phaseNumber)
			{
				string title = NKCStringTable.GetString("SI_DP_GAME_PHASE_COUNT", phaseNumber);
				NKCGameHudAlarmCommonControl alert = new NKCGameHudAlarmCommonControl(m_AlertPhase, title, "");
				EnqueueHudAlert(alert);
			}

			private void ProcessHotkey()
			{
				if (IsOpenPause() || NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable())
				{
					return;
				}
				if (NKCInputManager.CheckHotKeyEvent(InGamehotkeyEventType.Unit0))
				{
					OnDeckHotkey(0);
					NKCInputManager.ConsumeHotKeyEvent(InGamehotkeyEventType.Unit0);
				}
				else if (NKCInputManager.CheckHotKeyEvent(InGamehotkeyEventType.Unit1))
				{
					OnDeckHotkey(1);
					NKCInputManager.ConsumeHotKeyEvent(InGamehotkeyEventType.Unit1);
				}
				else if (NKCInputManager.CheckHotKeyEvent(InGamehotkeyEventType.Unit2))
				{
					OnDeckHotkey(2);
					NKCInputManager.ConsumeHotKeyEvent(InGamehotkeyEventType.Unit2);
				}
				else if (NKCInputManager.CheckHotKeyEvent(InGamehotkeyEventType.Unit3))
				{
					OnDeckHotkey(3);
					NKCInputManager.ConsumeHotKeyEvent(InGamehotkeyEventType.Unit3);
				}
				else if (NKCInputManager.CheckHotKeyEvent(InGamehotkeyEventType.UnitAssist))
				{
					if (m_GameClient.GetMyTeamData().m_DeckData.GetAutoRespawnIndexAssist() == -1)
					{
						return;
					}
					OnDeckHotkey(5);
					NKCInputManager.ConsumeHotKeyEvent(InGamehotkeyEventType.UnitAssist);
				}
				else if (NKCInputManager.CheckHotKeyEvent(InGamehotkeyEventType.ShipSkill0))
				{
					OnShipSkillDeck(0);
					NKCInputManager.ConsumeHotKeyEvent(InGamehotkeyEventType.ShipSkill0);
				}
				else if (NKCInputManager.CheckHotKeyEvent(InGamehotkeyEventType.ShipSkill1))
				{
					OnShipSkillDeck(1);
					NKCInputManager.ConsumeHotKeyEvent(InGamehotkeyEventType.ShipSkill1);
				}
				if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey))
				{
					NKCUIComHotkeyDisplay.OpenInstance(m_trHotkeyPosLeft, HotkeyEventType.Left);
					NKCUIComHotkeyDisplay.OpenInstance(m_trHotkeyPosRight, HotkeyEventType.Right);
					NKCUIComHotkeyDisplay.OpenInstance(m_NKCUIHudDeck[0].m_rtSubRoot, InGamehotkeyEventType.Unit0);
					NKCUIComHotkeyDisplay.OpenInstance(m_NKCUIHudDeck[1].m_rtSubRoot, InGamehotkeyEventType.Unit1);
					NKCUIComHotkeyDisplay.OpenInstance(m_NKCUIHudDeck[2].m_rtSubRoot, InGamehotkeyEventType.Unit2);
					NKCUIComHotkeyDisplay.OpenInstance(m_NKCUIHudDeck[3].m_rtSubRoot, InGamehotkeyEventType.Unit3);
					NKCUIComHotkeyDisplay.OpenInstance(m_NKCUIHudDeck[5].m_rtSubRoot, InGamehotkeyEventType.UnitAssist);
					if (GetShipSkillDeck(0) != null)
					{
						NKCUIComHotkeyDisplay.OpenInstance(GetShipSkillDeck(0).transform, InGamehotkeyEventType.ShipSkill0);
					}
					if (GetShipSkillDeck(1) != null)
					{
						NKCUIComHotkeyDisplay.OpenInstance(GetShipSkillDeck(1).transform, InGamehotkeyEventType.ShipSkill1);
					}
				}
			}

			private void OnDeckHotkey(int index)
			{
				if (m_SelectUnitDeckIndex != index)
				{
					TouchUpDeck(index, bUseTouchScale: false);
					GetGameClient().OnUnitDeckHotkey(index);
				}
			}

			private void OnShipSkillDeck(int index)
			{
				if (m_SelectShipSkillDeckIndex != index)
				{
					TouchUpShipSkillDeck(index);
					GetGameClient().OnShipSkillhotkey(index);
				}
			}

			public void HideHud(bool bEventControl = false)
			{
				for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
				{
					m_NKCUIHudDeck[i].SetActive(bActive: false, bEventControl);
				}
				for (int j = 0; j < m_NKCUIHudShipSkillDeck.Length; j++)
				{
					m_NKCUIHudShipSkillDeck[j].SetActive(bActive: false, bEventControl);
				}
			}

			public void ShowHud(bool bEventControl = false)
			{
				for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
				{
					if (i != 5)
					{
						m_NKCUIHudDeck[i].SetActive(bActive: true, bEventControl);
					}
				}
				for (int j = 0; j < m_NKCUIHudShipSkillDeck.Length; j++)
				{
					m_NKCUIHudShipSkillDeck[j].SetActive(bActive: true, bEventControl);
				}
			}

			public void ShowHudDeck(int targetUnitID)
			{
				for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
				{
					NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
					if (nKCGameHudDeckSlot.m_UnitData != null && nKCGameHudDeckSlot.m_UnitData.m_UnitID == targetUnitID)
					{
						NKCUtil.SetGameobjectActive(nKCGameHudDeckSlot, bValue: true);
						break;
					}
				}
			}

			public NKCGameHudDeckSlot GetHudDeckByUnitID(int targetUnitID)
			{
				for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
				{
					NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
					if (nKCGameHudDeckSlot.m_UnitData != null && nKCGameHudDeckSlot.m_UnitData.m_UnitID == targetUnitID)
					{
						return nKCGameHudDeckSlot;
					}
				}
				return null;
			}

			public NKCGameHudDeckSlot GetHudDeckByUnitUID(long targetUnitUID)
			{
				for (int i = 0; i < m_NKCUIHudDeck.Length; i++)
				{
					NKCGameHudDeckSlot nKCGameHudDeckSlot = m_NKCUIHudDeck[i];
					if (nKCGameHudDeckSlot.m_UnitData != null && nKCGameHudDeckSlot.m_UnitData.m_UnitUID == targetUnitUID)
					{
						return nKCGameHudDeckSlot;
					}
				}
				return null;
			}

			public void ShowHudSkill(int skillID)
			{
				for (int i = 0; i < m_NKCUIHudShipSkillDeck.Length; i++)
				{
					m_NKCUIHudShipSkillDeck[i].SetActive(bActive: false);
				}
			}

			public NKCUIHudShipSkillDeck GetHudSkillBySkillID(int skillID)
			{
				for (int i = 0; i < m_NKCUIHudShipSkillDeck.Length; i++)
				{
					NKCUIHudShipSkillDeck nKCUIHudShipSkillDeck = m_NKCUIHudShipSkillDeck[i];
					if (nKCUIHudShipSkillDeck.m_NKMShipSkillTemplet != null && nKCUIHudShipSkillDeck.m_NKMShipSkillTemplet.m_ShipSkillID == skillID)
					{
						return nKCUIHudShipSkillDeck;
					}
				}
				return null;
			}

			public void SetKillCount(long killCount)
			{
				if (m_killCount != null)
				{
					if (m_GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE)
					{
						m_killCount.SetKillCountForDefence(killCount);
					}
					else
					{
						m_killCount.SetKillCount(killCount);
					}
				}
			}

			private NKCGameHudSummonIndicator GetSummonIndicator()
			{
				foreach (NKCGameHudSummonIndicator item in m_lstSummonIndicator)
				{
					if (item != null && item.Idle)
					{
						return item;
					}
				}
				return MakeSummonIndicator();
			}

			private NKCGameHudSummonIndicator MakeSummonIndicator()
			{
				GameObject orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<GameObject>("AB_UI_NKM_UI_HUD_RENEWAL", "AB_UI_GAME_HUD_WARNING");
				if (orLoadAssetResource == null)
				{
					return null;
				}
				GameObject gameObject = Object.Instantiate(orLoadAssetResource, m_trIndicatorRootLeft);
				if (gameObject == null)
				{
					return null;
				}
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				if (gameObject.TryGetComponent<NKCGameHudSummonIndicator>(out var component))
				{
					m_lstSummonIndicator.Add(component);
					NKCUtil.SetGameobjectActive(component, bValue: false);
					return component;
				}
				return null;
			}

			public void SetUnitIndicator(NKCUnitClient cUnit)
			{
				NKCGameHudSummonIndicator summonIndicator = GetSummonIndicator();
				if (!(summonIndicator == null) && !summonIndicator.SetData(cUnit, m_GameClient, m_trIndicatorRootLeft, m_trIndicatorRootRight))
				{
					NKCUtil.SetGameobjectActive(summonIndicator, bValue: false);
				}
			}

			public void ShowChangeRemainTimeEffect(float delta)
			{
				if (!(m_animTimeChange == null))
				{
					NKCUtil.SetGameobjectActive(m_animTimeChange.gameObject, bValue: true);
					if (delta > 0f)
					{
						NKCUtil.SetLabelText(m_lbTimeChangePlus, delta.ToString("0.##"));
						m_animTimeChange.SetTrigger("Plus");
					}
					else if (delta < 0f)
					{
						NKCUtil.SetLabelText(m_lbTimeChangeMinus, delta.ToString("0.##"));
						m_animTimeChange.SetTrigger("Minus");
					}
				}
			}
		}
