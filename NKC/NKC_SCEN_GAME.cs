using System.Collections.Generic;
using ClientPacket.Game;
using ClientPacket.User;
using Cs.Core.Util;
using Cs.Logging;
using NKC.Loading;
using NKC.PacketHandler;
using NKC.Patcher;
using NKC.Publisher;
using NKC.Trim;
using NKC.UI;
using NKC.UI.Gauntlet;
using NKC.UI.HUD;
using NKC.UI.Option;
using NKC.UI.Result;
using NKM;
using NKM.Event;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_GAME : NKC_SCEN_BASIC
{
	private GameObject m_NUM_GAME;

	private GameObject m_NUF_GAME_Panel;

	private GameObject m_NUM_GAME_PREFAB;

	private GameObject m_NKM_LENS_FLARE_LIST;

	private NKC_SCEN_GAME_UI_DATA m_NKC_SCEN_GAME_UI_DATA = new NKC_SCEN_GAME_UI_DATA();

	private bool m_bWaitingEnemyGameLoading;

	private NKCUIResult.BattleResultData GameResultUIData;

	public float m_UpdateTime;

	private NKCUIGame m_NKCUIGame;

	private string m_DungeonStrIDForLocal = "NKM_DUNGEON_TEST";

	private NKCUIGameToastMSG m_NKCUIGameToastMSG = new NKCUIGameToastMSG();

	private float m_fNextCancelPauseCheckTime;

	private const float CANCEL_PAUSE_CHECK_REFRESH_TIME_ON_POWER_SAVE_MODE = 5f;

	private NKM_SCEN_ID m_ePracticeReturnScenID;

	private NKM_SHORTCUT_TYPE m_ePractiveReturnShortcut;

	private string m_strPractiveReturnShortcutParam;

	public GameObject Get_NKM_LENS_FLARE_LIST()
	{
		return m_NKM_LENS_FLARE_LIST;
	}

	public NKC_SCEN_GAME_UI_DATA Get_NKC_SCEN_GAME_UI_DATA()
	{
		return m_NKC_SCEN_GAME_UI_DATA;
	}

	public NKCUIResult.BattleResultData Get_BattleResultData()
	{
		return GameResultUIData;
	}

	public NKC_SCEN_GAME()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GAME;
		m_NUM_GAME = NKCUIManager.OpenUI("NUM_GAME");
		m_NUF_GAME_Panel = NKCUIManager.OpenUI("NUF_GAME_Panel");
		m_NUM_GAME_PREFAB = GameObject.Find("NUM_GAME_PREFAB");
		m_NKCUIGame = m_NUF_GAME_Panel.GetComponent<NKCUIGame>();
		m_NKM_LENS_FLARE_LIST = GameObject.Find("NKM_LENS_FLARE_LIST");
		m_NUM_GAME.SetActive(value: false);
		m_NUF_GAME_Panel.SetActive(value: false);
	}

	public void SetDungeonStrIDForLocal(string strID)
	{
		m_DungeonStrIDForLocal = strID;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		NKCUtil.ClearGauntletCacheData(NKCScenManager.GetScenManager());
		if (!m_bLoadedUI && m_NKC_SCEN_GAME_UI_DATA.m_NUF_GAME_PREFAB == null)
		{
			m_NKC_SCEN_GAME_UI_DATA.m_NUF_GAME_PREFAB = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_HUD_RENEWAL", "NUF_GAME_PREFAB", bAsync: true);
		}
		if (!NKCScenManager.GetScenManager().GetGameClient().GetGameDataDummy()
			.m_bLocal)
		{
			NKCPacketSender.Send_NKMPacket_INFORM_MY_LOADING_PROGRESS_REQ((byte)(m_fLoadingProgress * 100f));
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (!m_bLoadedUI)
		{
			m_NKC_SCEN_GAME_UI_DATA.m_NUFGameObjects = m_NKC_SCEN_GAME_UI_DATA.m_NUF_GAME_PREFAB.m_Instant.GetComponent<NKCGameHudObjects>();
			m_NKC_SCEN_GAME_UI_DATA.m_NUF_GAME_PREFAB.m_Instant.transform.SetParent(m_NUF_GAME_Panel.transform, worldPositionStays: false);
			m_NKC_SCEN_GAME_UI_DATA.m_GAME_BATTLE_MAP = m_NUM_GAME_PREFAB.transform.Find("GAME_BATTLE_MAP").gameObject;
			m_NKC_SCEN_GAME_UI_DATA.m_GAME_BATTLE_UNIT = m_NUM_GAME_PREFAB.transform.Find("GAME_BATTLE_UNIT").gameObject;
			m_NKC_SCEN_GAME_UI_DATA.m_GAME_BATTLE_UNIT_SHADOW = m_NUM_GAME_PREFAB.transform.Find("GAME_BATTLE_UNIT_SHADOW").gameObject;
			m_NKC_SCEN_GAME_UI_DATA.m_GAME_BATTLE_UNIT_MOTION_BLUR = m_NUM_GAME_PREFAB.transform.Find("GAME_BATTLE_UNIT_MOTION_BLUR").gameObject;
			m_NKC_SCEN_GAME_UI_DATA.m_GAME_BATTLE_UNIT_VIEWER = m_NUM_GAME_PREFAB.transform.Find("GAME_BATTLE_UNIT_VIEWER").gameObject;
			m_NKC_SCEN_GAME_UI_DATA.m_NUF_GAME_HUD_MINI_MAP = m_NKC_SCEN_GAME_UI_DATA.m_NUF_GAME_PREFAB.m_Instant.transform.Find("AB_UI_GAME_HUD/HUD_Top/HUD_TOP_MINI_MAP").gameObject;
			m_NKC_SCEN_GAME_UI_DATA.m_NUM_GAME_BATTLE_EFFECT = m_NUM_GAME_PREFAB.transform.Find("NUM_GAME_BATTLE_EFFECT").gameObject;
			NKCScenManager.GetScenManager().GetGameClient().InitUI(m_NKC_SCEN_GAME_UI_DATA.m_NUFGameObjects.m_GameHud);
		}
		m_NKCUIGameToastMSG.Invalid();
		NKCCamera.GetCamera().orthographic = true;
		NKCScenManager.GetScenManager().GetGameClient().LoadGame();
		if (!NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.m_bLocal)
		{
			NKCPacketSender.Send_NKMPacket_INFORM_MY_LOADING_PROGRESS_REQ((byte)(m_fLoadingProgress * 100f));
		}
	}

	public void OnBackButton()
	{
		if (Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_START && NKCScenManager.GetScenManager().GetGameClient() != null && NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData() != null)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_PAUSE();
		}
	}

	public override void ScenLoadLastStart()
	{
		Log.Info("NKC_SCEN_GAME:ScenLoadLastStart", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKC_SCEN_GAME.cs", 210);
		base.ScenLoadLastStart();
		if (!NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.m_bLocal)
		{
			NKCPacketSender.Send_NKMPacket_INFORM_MY_LOADING_PROGRESS_REQ((byte)(m_fLoadingProgress * 100f));
		}
		Log.Info("NKC_SCEN_GAME:ScenLoadLastStart End", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKC_SCEN_GAME.cs", 221);
	}

	public override void ScenLoadCompleteWait()
	{
		Log.Info("NKC_SCEN_GAME:ScenLoadCompleteWait Start", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKC_SCEN_GAME.cs", 226);
		base.ScenLoadCompleteWait();
		Log.Info("NKC_SCEN_GAME:ScenLoadCompleteWait End", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKC_SCEN_GAME.cs", 229);
	}

	public override void ScenLoadComplete()
	{
		Log.Info("NKC_SCEN_GAME:ScenLoadComplete Start", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKC_SCEN_GAME.cs", 234);
		base.ScenLoadComplete();
		NKC2DMotionAfterImage.Init();
		NKCScenManager.GetScenManager().GetGameClient().LoadGameComplete();
		Shader.WarmupAllShaders();
		if (!NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.m_bLocal)
		{
			NKCPacketSender.Send_NKMPacket_INFORM_MY_LOADING_PROGRESS_REQ((byte)(m_fLoadingProgress * 100f));
			Log.Info("NKC_SCEN_GAME:ScenLoadLastStart Send_NKMPacket_INFORM_MY_LOADING_PROGRESS_REQ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKC_SCEN_GAME.cs", 258);
		}
	}

	private void SetEpisodeIDOrWarfareID(bool bReservedDetail = false)
	{
		NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
		if (gameData == null)
		{
			return;
		}
		switch (gameData.GetGameType())
		{
		case NKM_GAME_TYPE.NGT_WARFARE:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().SetWarfareStrID(NKCWarfareManager.GetWarfareStrID(gameData.m_WarfareID));
			return;
		case NKM_GAME_TYPE.NGT_PHASE:
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(NKCPhaseManager.GetLastStageID());
			if (nKMStageTempletV != null)
			{
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedEpisodeTemplet(nKMStageTempletV.EpisodeTemplet);
			}
			return;
		}
		}
		string dungeonStrID = NKMDungeonManager.GetDungeonStrID(gameData.m_DungeonID);
		if (NKMDungeonManager.GetDungeonTempletBase(dungeonStrID) == null)
		{
			return;
		}
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION();
		NKMStageTempletV2 nKMStageTempletV2 = NKMEpisodeMgr.FindStageTempletByBattleStrID(dungeonStrID);
		if (nKMStageTempletV2 != null)
		{
			if (!NKCScenManager.GetScenManager().Get_SCEN_OPERATION().PlayByFavorite)
			{
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedEpisodeTemplet(nKMStageTempletV2.EpisodeTemplet);
			}
			else
			{
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedEpisodeCategory(nKMStageTempletV2.EpisodeCategory);
			}
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCScenChangeOrder nKCScenChangeOrder = NKCScenManager.GetScenManager().PeekNextScenChangeOrder();
		if (nKCScenChangeOrder != null && nKCScenChangeOrder.m_NextScen == NKM_SCEN_ID.NSI_GAME && nKCScenChangeOrder.m_bForce)
		{
			Log.Info("Game Scene Reloading. dropping current game scene!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKC_SCEN_GAME.cs", 348);
		}
		else
		{
			OnGameScenStart();
		}
	}

	public void OnGameScenStart()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			float num = (float)gameOptionData.EffectOpacity / 100f;
			Shader.SetGlobalFloat("_FxGlobalTransparency", 1f - num * num);
			float num2 = (float)gameOptionData.EffectEnemyOpacity / 100f;
			Shader.SetGlobalFloat("_FxGlobalTransparencyEnemy", 1f - num2 * num2);
		}
		if (m_NKCUIGame != null && !m_NKCUIGame.IsOpen)
		{
			m_NKCUIGame.Open();
		}
		m_NKCUIGameToastMSG.Reset(NKCScenManager.GetScenManager().GetGameClient().GetGameData(), NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData());
		if (NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.IsPVP())
		{
			NKCScenManager.GetScenManager().SetActiveLoadingUI(NKCLoadingScreenManager.eGameContentsType.GAUNTLET);
			NKCUIManager.LoadingUI.SetWaitOpponent();
			m_bWaitingEnemyGameLoading = true;
			NKCPacketSender.Send_Packet_GAME_LOAD_COMPLETE_REQ(NKCScenManager.GetScenManager().GetGameClient().GetIntrude());
		}
		else
		{
			DungeonScenStart();
		}
		SetEpisodeIDOrWarfareID();
		if (GameResultUIData != null)
		{
			EndGameWithReservedGameData();
		}
	}

	public void TrySendGamePauseEnableREQ()
	{
		NKCScenManager.GetScenManager().GetGameClient().TrySendGamePauseEnableREQ();
	}

	private void DungeonScenStart()
	{
		m_bWaitingEnemyGameLoading = false;
		SetShow(bShow: true);
		if (NKCScenManager.CurrentUserData() != null)
		{
			_ = NKCScenManager.CurrentUserData().m_UserUID;
		}
		NKMPopUpBox.OpenWaitBox();
		m_UpdateTime = 0f;
		NKCPacketSender.Send_Packet_GAME_LOAD_COMPLETE_REQ(NKCScenManager.GetScenManager().GetGameClient().GetIntrude());
	}

	public override void ScenEnd()
	{
		Shader.SetGlobalFloat("_FxGlobalTransparency", 0f);
		Shader.SetGlobalFloat("_FxGlobalTransparencyEnemy", 0f);
		ScenClear();
		m_NKCUIGame.Close();
		NKCUIOverlayCharMessage.CheckInstanceAndClose();
		NKCUIOverlayTutorialGuide.CheckInstanceAndClose();
		m_NKC_SCEN_GAME_UI_DATA.Init();
		NKC2DMotionAfterImage.CleanUp();
		NKCReplayMgr.GetNKCReplaMgr()?.OnGameScenEnd();
		base.ScenEnd();
	}

	public void ScenClear()
	{
		if (m_NUM_GAME.activeSelf)
		{
			m_NUM_GAME.SetActive(value: false);
		}
		if (m_NUF_GAME_Panel.activeSelf)
		{
			m_NUF_GAME_Panel.SetActive(value: false);
		}
		NKCScenManager.GetScenManager().GetGameClient().EndGame();
		NKCSoundManager.Unload();
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		NKCScenManager.GetScenManager().GetGameClient().UnloadUI();
		if (m_NKC_SCEN_GAME_UI_DATA.m_NUF_GAME_PREFAB != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKC_SCEN_GAME_UI_DATA.m_NUF_GAME_PREFAB);
		}
		m_NKC_SCEN_GAME_UI_DATA.Init();
	}

	public override void ScenUpdate()
	{
		if (!m_bWaitingEnemyGameLoading)
		{
			base.ScenUpdate();
			NKCScenManager.GetScenManager().GetGameClient().Update(Time.deltaTime);
			UpdateAtGamePlaying();
		}
	}

	private void UpdateAtGamePlaying()
	{
		if (Get_NKC_SCEN_STATE() != NKC_SCEN_STATE.NSS_START || NKCScenManager.GetScenManager().GetGameClient() == null || NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData() == null || NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData()
			.m_NKM_GAME_STATE != NKM_GAME_STATE.NGS_PLAY)
		{
			return;
		}
		m_NKCUIGameToastMSG.Update();
		if (m_fNextCancelPauseCheckTime < Time.time)
		{
			m_fNextCancelPauseCheckTime = Time.time + 5f;
			if (NKCScenManager.GetScenManager().GetGameClient().GetGameData() != null && NKCScenManager.GetScenManager().GetGameClient().GetGameData()
				.IsPVE() && NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable() && NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData()
				.m_bPause && !NKMPopUpBox.IsOpenedWaitBox())
			{
				NKCScenManager.GetScenManager().GetGameClient().Send_Packet_GAME_PAUSE_REQ(bPause: false);
			}
		}
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void OnRecv(NKMPacket_GAME_LOAD_COMPLETE_ACK cNKMPacket_GAME_LOAD_COMPLETE_ACK)
	{
		NKCScenManager.GetScenManager().GetGameClient().OnRecv(cNKMPacket_GAME_LOAD_COMPLETE_ACK);
	}

	public void OnRecv(NKMPacket_GAME_START_NOT cNKMPacket_GAME_START_NOT)
	{
		if (NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.IsPVP() && m_bWaitingEnemyGameLoading)
		{
			m_NUM_GAME.SetActive(value: true);
			m_NUF_GAME_Panel.SetActive(value: true);
			m_UpdateTime = 0f;
			m_bWaitingEnemyGameLoading = false;
			NKCScenManager.GetScenManager().CloseLoadingUI();
		}
		NKCScenManager.GetScenManager().GetGameClient().StartGame(bIntrude: false);
		NKMPopUpBox.CloseWaitBox();
		SetAutoIfAutoGame();
	}

	private void SetAutoIfAutoGame()
	{
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient == null || gameClient.GetGameData() == null)
		{
			return;
		}
		NKMGameRuntimeData gameRuntimeData = gameClient.GetGameRuntimeData();
		if (gameRuntimeData != null)
		{
			NKMGameRuntimeTeamData myRuntimeTeamData = gameRuntimeData.GetMyRuntimeTeamData(NKCScenManager.GetScenManager().GetGameClient().m_MyTeam);
			if (myRuntimeTeamData != null && !myRuntimeTeamData.m_bAutoRespawn && CheckAutoGame())
			{
				gameClient.Send_Packet_GAME_AUTO_RESPAWN_REQ(bAutoRespawn: true);
			}
		}
	}

	public void OnRecv(NKMPacket_GAME_INTRUDE_START_NOT cNKMPacket_GAME_INTRUDE_START_NOT)
	{
		if (NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.IsPVP() && m_bWaitingEnemyGameLoading)
		{
			if (!m_NUM_GAME.activeSelf)
			{
				m_NUM_GAME.SetActive(value: true);
			}
			if (!m_NUF_GAME_Panel.activeSelf)
			{
				m_NUF_GAME_Panel.SetActive(value: true);
			}
			m_UpdateTime = 0f;
			m_bWaitingEnemyGameLoading = false;
			NKCScenManager.GetScenManager().CloseLoadingUI();
		}
		m_NKCUIGameToastMSG.SetCost((int)cNKMPacket_GAME_INTRUDE_START_NOT.usedRespawnCost);
		NKCScenManager.GetScenManager().GetGameClient().OnRecv(cNKMPacket_GAME_INTRUDE_START_NOT);
		NKCScenManager.GetScenManager().GetGameClient().StartGame(bIntrude: false);
		NKMPopUpBox.CloseWaitBox();
	}

	private void ExitToRaid()
	{
		if (NKCScenManager.GetScenManager().GetGameClient() == null)
		{
			ExitGame(NKM_SCEN_ID.NSI_WORLDMAP);
			return;
		}
		NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
		if (gameData == null || NKCScenManager.GetScenManager().GetNKCRaidDataMgr().CheckCompletableRaid(gameData.m_RaidUID))
		{
			ExitGame(NKM_SCEN_ID.NSI_WORLDMAP);
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().SetRaidUID(gameData.m_RaidUID);
		ExitGame(NKM_SCEN_ID.NSI_RAID);
	}

	private void TutorialExit(int dungeonID)
	{
		if (NKCPatchUtility.BackgroundPatchEnabled())
		{
			if (dungeonID == 1007 && NKCPatchUtility.SaveTutorialClearedStatus())
			{
				Debug.Log("Tutorial final stage cleared!");
			}
			else
			{
				ExitGame(NKM_SCEN_ID.NSI_OPERATION);
			}
		}
		else if (NKCPatchDownloader.Instance != null && NKCPatchDownloader.Instance.ProloguePlay)
		{
			NKCScenManager.GetScenManager().GetGameClient().EndGame();
			NKCScenManager.GetScenManager().ShowBundleUpdate(bCallFromTutorial: true);
		}
		else
		{
			ExitGame(NKM_SCEN_ID.NSI_OPERATION);
		}
	}

	private void ExitToDungeonResult(NKMStageTempletV2 stageTemplet, NKCUIResult.BattleResultData resultData)
	{
		long num = 0L;
		long leaderShipUID = 0L;
		int num2 = 0;
		int skinID = 0;
		bool flag = false;
		if (stageTemplet != null)
		{
			NKMDungeonEventDeckTemplet eventDeckTemplet = stageTemplet.GetEventDeckTemplet();
			if (eventDeckTemplet != null)
			{
				NKMEventDeckData lastEventDeck = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastEventDeck();
				if (lastEventDeck != null)
				{
					leaderShipUID = lastEventDeck.m_ShipUID;
					num = lastEventDeck.GetUnitUID(lastEventDeck.m_LeaderIndex);
					if (num == 0L)
					{
						NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = eventDeckTemplet.GetUnitSlot(lastEventDeck.m_LeaderIndex);
						switch (unitSlot.m_eType)
						{
						case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
						case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
							num2 = unitSlot.m_ID;
							skinID = unitSlot.m_SkinID;
							break;
						case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
							num2 = 999;
							skinID = 0;
							break;
						}
					}
					flag = true;
				}
			}
			else
			{
				NKMDeckIndex lastDeckIndex = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastDeckIndex();
				NKMDeckData deckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(lastDeckIndex);
				if (deckData != null)
				{
					leaderShipUID = deckData.m_ShipUID;
					num = deckData.GetLeaderUnitUID();
					flag = true;
				}
			}
		}
		if (!flag)
		{
			NKMGameTeamData teamData = NKCScenManager.GetScenManager().GetGameClient().GetGameData()
				.GetTeamData(NKCScenManager.CurrentUserData().m_UserUID);
			if (teamData != null)
			{
				NKMUnitData leaderUnitData = teamData.GetLeaderUnitData();
				if (leaderUnitData != null)
				{
					num2 = leaderUnitData.m_UnitID;
					skinID = leaderUnitData.m_SkinID;
					flag = true;
				}
			}
		}
		if (flag)
		{
			NKCScenManager.GetScenManager().GET_NKC_SCEN_DUNGEON_RESULT().SetData(resultData, num, leaderShipUID);
			if (num2 != 0)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(num2);
				if (unitTempletBase == null || !unitTempletBase.m_bContractable)
				{
					num2 = 999;
					skinID = 0;
				}
				NKCScenManager.GetScenManager().GET_NKC_SCEN_DUNGEON_RESULT().SetDummyLeader(num2, skinID);
			}
			ExitGame(NKM_SCEN_ID.NSI_DUNGEON_RESULT);
		}
		else
		{
			ExitGame(NKM_SCEN_ID.NSI_OPERATION);
		}
	}

	private void ExitToDungeonResult(NKMDungeonTempletBase dungeonTempletBase, NKCUIResult.BattleResultData resultData)
	{
		long leaderUnitUID = 0L;
		long leaderShipUID = 0L;
		int num = 0;
		int skinID = 0;
		bool flag = false;
		NKMGameTeamData teamData = NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.GetTeamData(NKCScenManager.CurrentUserData().m_UserUID);
		if (teamData != null)
		{
			NKMUnitData leaderUnitData = teamData.GetLeaderUnitData();
			if (leaderUnitData != null)
			{
				num = leaderUnitData.m_UnitID;
				skinID = leaderUnitData.m_SkinID;
				flag = true;
			}
		}
		if (flag)
		{
			NKCScenManager.GetScenManager().GET_NKC_SCEN_DUNGEON_RESULT().SetData(resultData, leaderUnitUID, leaderShipUID);
			if (num != 0)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(num);
				if (unitTempletBase == null || !unitTempletBase.m_bContractable)
				{
					num = 999;
					skinID = 0;
				}
				NKCScenManager.GetScenManager().GET_NKC_SCEN_DUNGEON_RESULT().SetDummyLeader(num, skinID);
			}
			ExitGame(NKM_SCEN_ID.NSI_DUNGEON_RESULT);
		}
		else
		{
			ExitGame(NKM_SCEN_ID.NSI_OPERATION);
		}
	}

	private void ExitToDiveInstance()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			if (NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE_RESULT().GetExistNewData())
			{
				ExitGame(NKM_SCEN_ID.NSI_DIVE_RESULT);
				return;
			}
			if (myUserData.m_DiveGameData != null)
			{
				ExitGame(NKM_SCEN_ID.NSI_DIVE);
				return;
			}
		}
		ExitGame(NKM_SCEN_ID.NSI_WORLDMAP);
	}

	private void ExitToShadow(bool bGiveUp, bool bEnd)
	{
		if (bGiveUp && bEnd)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_PALACE);
			return;
		}
		if (!bGiveUp && bEnd)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_RESULT);
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_SHADOW_BATTLE().SetShadowPalaceID(NKCScenManager.CurrentUserData().m_ShadowPalace.currentPalaceId);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_BATTLE);
	}

	private void ExitToGuildCoop()
	{
		ExitGame(NKM_SCEN_ID.NSI_GUILD_COOP);
	}

	private void ExitGame(NKM_SCEN_ID scenID)
	{
		NKCScenManager.GetScenManager().GetGameClient().EndGame();
		if (NKCScenManager.GetScenManager().GetGameClient().GetGameData() != null && NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.IsPVP())
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
			return;
		}
		if (scenID == NKM_SCEN_ID.NSI_OPERATION)
		{
			SetEpisodeIDOrWarfareID(bReservedDetail: true);
		}
		NKCScenManager.GetScenManager().ScenChangeFade(scenID);
	}

	public void DoAfterGiveUp()
	{
		NKCUIGameOption.CheckInstanceAndClose();
		NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
		switch (gameData.m_NKM_GAME_TYPE)
		{
		case NKM_GAME_TYPE.NGT_WARFARE:
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().SetReservedShowBattleResult(bSet: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WARFARE_GAME);
			break;
		case NKM_GAME_TYPE.NGT_TUTORIAL:
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			break;
		case NKM_GAME_TYPE.NGT_DUNGEON:
		case NKM_GAME_TYPE.NGT_PHASE:
		{
			NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
			if (nKCRepeatOperaion != null && nKCRepeatOperaion.GetIsOnGoing())
			{
				nKCRepeatOperaion.Init();
			}
			if (NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet() == null)
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
				break;
			}
			SetEpisodeIDOrWarfareID(bReservedDetail: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
			break;
		}
		case NKM_GAME_TYPE.NGT_TRIM:
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_TRIM);
			break;
		case NKM_GAME_TYPE.NGT_DIVE:
			ExitToDiveInstance();
			break;
		case NKM_GAME_TYPE.NGT_RAID:
		case NKM_GAME_TYPE.NGT_RAID_SOLO:
			ExitToRaid();
			break;
		case NKM_GAME_TYPE.NGT_SHADOW_PALACE:
			ExitToShadow(bGiveUp: true, NKCScenManager.CurrentUserData().m_ShadowPalace.life == 0);
			break;
		case NKM_GAME_TYPE.NGT_FIERCE:
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT);
			break;
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_ARENA:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE:
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_COOP);
			break;
		default:
			if (gameData.IsPVP())
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
				break;
			}
			SetEpisodeIDOrWarfareID(bReservedDetail: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
			break;
		}
	}

	public void DoAfterRestart(NKM_GAME_TYPE gameType, int stageID, int dungeonID, NKMDeckIndex deckIndex)
	{
		NKCUIGameOption.CheckInstanceAndClose();
		NKMStageTempletV2.Find(stageID);
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_FIERCE:
		{
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			if (nKCFierceBattleSupportDataMgr == null)
			{
				Exit();
				break;
			}
			int curBossID = nKCFierceBattleSupportDataMgr.CurBossID;
			if (curBossID == 0)
			{
				Exit();
				break;
			}
			string fierceEventDeckKey = NKMDungeonManager.GetFierceEventDeckKey(nKCFierceBattleSupportDataMgr.GetBossGroupTemplet());
			NKMEventDeckData nKMEventDeckData = NKMDungeonManager.LoadDungeonDeck(dungeonTempletBase.EventDeckTemplet, fierceEventDeckKey);
			if (nKMEventDeckData == null)
			{
				Exit();
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(nKMEventDeckData, stageID, 0, dungeonID, 0, bLocal: false, 1, curBossID, 0L);
			}
			break;
		}
		case NKM_GAME_TYPE.NGT_PVE_DEFENCE:
		{
			NKMDefenceTemplet currentDefenceDungeonTemplet = NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Now);
			if (currentDefenceDungeonTemplet == null)
			{
				Exit();
				break;
			}
			string curEventDeckKey = NKMDungeonManager.GetCurEventDeckKey(NKMDungeonManager.GetDungeonTempletBase(currentDefenceDungeonTemplet.m_DungeonID), null);
			NKMEventDeckData eventDeckData = NKMDungeonManager.LoadDungeonDeck(dungeonTempletBase.EventDeckTemplet, curEventDeckKey);
			NKCPacketSender.Send_NKMPacket_DEFENCE_GAME_START_REQ(currentDefenceDungeonTemplet.Key, eventDeckData);
			break;
		}
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE:
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_COOP);
			break;
		default:
			Exit();
			break;
		}
		void Exit()
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_ERROR_GAME_RESTART_FAIL"), DoAfterGiveUp);
		}
	}

	public void OnLocalGameEndRecv(NKMPacket_GAME_END_NOT cPacket_GAME_END_NOT)
	{
		NKCUIGameOption.CheckInstanceAndClose();
		if (NKCScenManager.GetScenManager().GetGameClient().GetGameData()
			.m_bLocal)
		{
			NKCScenManager.GetScenManager().GetGameClient().EndGame();
			NKCPacketSender.Send_NKMPacket_DEV_GAME_LOAD_REQ(m_DungeonStrIDForLocal);
		}
		else
		{
			Debug.LogError("Unexpected non-local NKMPacket_GAME_END_NOT packet");
		}
	}

	public void ReserveGameEndData(NKCUIResult.BattleResultData resultData)
	{
		GameResultUIData = resultData;
	}

	public void EndGameWithReservedGameData()
	{
		if (GameResultUIData != null)
		{
			OnEndGame(GameResultUIData);
			GameResultUIData = null;
			return;
		}
		NKMGameData nKMGameData = NKCScenManager.GetScenManager()?.GetGameClient()?.GetGameData();
		if (nKMGameData != null && nKMGameData.m_bLocal && NKCReplayMgr.IsPlayingReplay())
		{
			NKMOpenTagManager.ClearOpenTag();
			NKCPacketHandlersLobby.MoveToLogin();
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR_SERVER_GAME_DATA_AND_GO_LOBBY, delegate
			{
				ReturnToLastScene();
			});
		}
	}

	private void ReturnToLastScene()
	{
		if (NKCPrivatePVPRoomMgr.LobbyData != null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM);
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		}
	}

	public void DisconnectAll()
	{
		NKCScenManager.GetScenManager().GetConnectGame().Reconnect();
	}

	private bool CheckAutoGame()
	{
		NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
		if (gameData == null)
		{
			return false;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		if (myUserData.m_UserOption == null)
		{
			return false;
		}
		switch (gameData.GetGameType())
		{
		case NKM_GAME_TYPE.NGT_WARFARE:
			if (myUserData.m_UserOption.m_bAutoWarfare)
			{
				return true;
			}
			break;
		case NKM_GAME_TYPE.NGT_DIVE:
			if (myUserData.m_UserOption.m_bAutoDive)
			{
				return true;
			}
			break;
		}
		if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
		{
			return true;
		}
		return false;
	}

	public void OnEndGame(NKCUIResult.BattleResultData resultData)
	{
		NKCUIGameOption.CheckInstanceAndClose();
		m_NUF_GAME_Panel.SetActive(value: false);
		NKMGameData cNKMGameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().SetStageID(resultData.m_stageID);
		if (NKMGame.IsPVP(cNKMGameData.GetGameType()))
		{
			NKCUIGauntletResult.SetResultData(resultData);
			NKC_GAUNTLET_LOBBY_TAB reservedLobbyTab;
			switch (cNKMGameData.GetGameType())
			{
			case NKM_GAME_TYPE.NGT_PVP_RANK:
				reservedLobbyTab = NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK;
				break;
			case NKM_GAME_TYPE.NGT_ASYNC_PVP:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
				reservedLobbyTab = NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC;
				break;
			case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
				reservedLobbyTab = NKC_GAUNTLET_LOBBY_TAB.NGLT_LEAGUE;
				break;
			case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
				reservedLobbyTab = NKC_GAUNTLET_LOBBY_TAB.NGLT_PRIVATE;
				break;
			case NKM_GAME_TYPE.NGT_PVP_EVENT:
				reservedLobbyTab = NKC_GAUNTLET_LOBBY_TAB.NGLT_EVENT;
				break;
			default:
				reservedLobbyTab = NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK;
				break;
			}
			switch (cNKMGameData.GetGameType())
			{
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedAsyncTab(NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE.PAT_NPC);
				break;
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedAsyncTab(NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE.PAT_REVENGE);
				break;
			default:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedAsyncTab();
				break;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(reservedLobbyTab);
			if (NKCReplayMgr.IsPlayingReplay())
			{
				resultData.m_OrgDoubleToken = 0L;
				resultData.m_RewardData = new NKMRewardData();
			}
			if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PVP_PRIVATE || cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PVP_EVENT)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().SetDoAtScenStart(delegate
				{
					NKCUIManager.NKCUIGauntletResult.Open(delegate
					{
						NKCUIResult.Instance.OpenPrivatePvpResult(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData, resultData, delegate
						{
							NKM_SCEN_ID nextSceneId = NKM_SCEN_ID.NSI_GAUNTLET_LOBBY;
							if (NKCPrivatePVPRoomMgr.LobbyData != null)
							{
								nextSceneId = NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM;
							}
							NKCContentManager.ShowContentUnlockPopup(delegate
							{
								NKCScenManager.GetScenManager().ScenChangeFade(nextSceneId);
							}, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_RANK_SCORE, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_RANK_SCORE_RECORD, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_ASYNC_SCORE, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_ASYNC_SCORE_RECORD);
						}, resultData.m_battleData);
					});
				});
			}
			else if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PVE_SIMULATED)
			{
				NKMEventCollectionIndexTemplet nKMEventCollectionIndexTemplet = NKMEventCollectionIndexTemplet.Find(NKCTournamentManager.m_eventCollectionIndexId);
				if (nKMEventCollectionIndexTemplet == null)
				{
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
					return;
				}
				NKCScenManager.GetScenManager().Get_SCEN_HOME().SetReservedOpenUI(NKC_SCEN_HOME.RESERVE_OPEN_TYPE.ROT_TOURNAMENT, nKMEventCollectionIndexTemplet.Key);
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().SetDoAtScenStart(delegate
				{
					NKCUIResult.Instance.OpenComplexResult(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData, resultData.m_RewardData, delegate
					{
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
					}, resultData.m_OrgDoubleToken, resultData.m_battleData, bIgnoreAutoClose: true, bAllowRewardDataNull: true);
				});
			}
			else if (cNKMGameData.IsPVP())
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().SetDoAtScenStart(delegate
				{
					NKCUIManager.NKCUIGauntletResult.Open(delegate
					{
						NKCUIResult.Instance.OpenComplexResult(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData, resultData.m_RewardData, delegate
						{
							if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PVP_LEAGUE || cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
							{
								NKCContentManager.ShowContentUnlockPopup(delegate
								{
									NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
								}, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_RANK_SCORE, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_RANK_SCORE_RECORD, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_ASYNC_SCORE, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_ASYNC_SCORE_RECORD);
							}
							else
							{
								NKCContentManager.ShowContentUnlockPopup(delegate
								{
									NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
								}, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_RANK_SCORE, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_RANK_SCORE_RECORD, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_ASYNC_SCORE, STAGE_UNLOCK_REQ_TYPE.SURT_PVP_ASYNC_SCORE_RECORD);
							}
						}, resultData.m_OrgDoubleToken, resultData.m_battleData, bIgnoreAutoClose: true, bAllowRewardDataNull: true);
					});
				});
			}
			else
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().SetDoAtScenStart(delegate
				{
					NKCUIResult.Instance.OpenComplexResult(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData, resultData.m_RewardData, delegate
					{
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
					}, resultData.m_OrgDoubleToken, resultData.m_battleData, bIgnoreAutoClose: true, bAllowRewardDataNull: true);
				});
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME_RESULT);
		}
		else if (NKMGame.IsPVE(cNKMGameData.GetGameType()))
		{
			bool flag = true;
			int dungeonID = cNKMGameData.m_DungeonID;
			NKMDungeonTempletBase cNKMDungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
			bool flag2 = true;
			NKCUIResult.OnClose ClosingDelegate;
			switch (cNKMGameData.GetGameType())
			{
			case NKM_GAME_TYPE.NGT_WARFARE:
				ClosingDelegate = delegate
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().SetReservedShowBattleResult(bSet: true);
					ExitGame(NKM_SCEN_ID.NSI_WARFARE_GAME);
				};
				break;
			case NKM_GAME_TYPE.NGT_PHASE:
				if (resultData.m_BATTLE_RESULT_TYPE == BATTLE_RESULT_TYPE.BRT_WIN)
				{
					if (NKCPhaseManager.ShouldPlayNextPhase())
					{
						flag2 = false;
						flag = false;
						ClosingDelegate = delegate
						{
							if (NKMDungeonManager.GetDungeonTempletBase(NKCPhaseManager.PhaseModeState.dungeonId) != null)
							{
								NKMStageTempletV2 stageTemplet4 = NKMStageTempletV2.Find(NKCPhaseManager.PhaseModeState.stageId);
								NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(stageTemplet4, DeckContents.PHASE);
								NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
							}
						};
						break;
					}
					int currentStageID = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().GetStageID();
					if (NKCRepeatOperaion.CheckVisible(currentStageID) && NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
					{
						NKMStageTempletV2 stageTemplet = NKMStageTempletV2.Find(currentStageID);
						ClosingDelegate = delegate
						{
							if (stageTemplet != null)
							{
								if (stageTemplet.IsUsingEventDeck())
								{
									NKCPacketSender.Send_NKMPacket_PHASE_START_REQ(currentStageID, NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastEventDeck(), NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastSupportUserUID());
								}
								else
								{
									NKCPacketSender.Send_NKMPacket_PHASE_START_REQ(currentStageID, NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastDeckIndex(), NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastSupportUserUID());
								}
							}
							else
							{
								ExitGame(NKM_SCEN_ID.NSI_OPERATION);
							}
						};
						break;
					}
					flag = false;
					bool num = CheckCutscen(cNKMGameData.m_NKM_GAME_TYPE, resultData.m_stageID, cNKMGameData.m_DungeonID);
					NKMStageTempletV2 stageTemplet2 = NKMStageTempletV2.Find(resultData.m_stageID);
					if (num)
					{
						ClosingDelegate = delegate
						{
							if (stageTemplet2 != null && stageTemplet2.PhaseTemplet != null)
							{
								NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(stageTemplet2.PhaseTemplet.m_CutScenStrIDAfter);
								if (cutScenTemple != null)
								{
									NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(cutScenTemple.m_CutScenStrID, delegate
									{
										ExitToDungeonResult(stageTemplet2, resultData);
									}, cNKMDungeonTempletBase.m_DungeonStrID);
									NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
								}
								else
								{
									ExitToDungeonResult(stageTemplet2, resultData);
								}
							}
						};
					}
					else
					{
						ClosingDelegate = delegate
						{
							ExitToDungeonResult(stageTemplet2, resultData);
						};
					}
				}
				else
				{
					flag = false;
					NKMStageTempletV2 stageTemplet3 = NKMStageTempletV2.Find(resultData.m_stageID);
					ClosingDelegate = delegate
					{
						ExitToDungeonResult(stageTemplet3, resultData);
					};
				}
				break;
			case NKM_GAME_TYPE.NGT_TRIM:
				ClosingDelegate = delegate
				{
					if (!NKCTrimManager.ProcessTrim())
					{
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_TRIM);
					}
				};
				break;
			default:
				ClosingDelegate = delegate
				{
					TutorialExit(dungeonID);
				};
				break;
			case NKM_GAME_TYPE.NGT_DUNGEON:
				flag = false;
				ClosingDelegate = delegate
				{
					int stageID = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().GetStageID();
					NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(stageID);
					if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
					{
						NKMDungeonTempletBase dungeonTempletBase = nKMStageTempletV.DungeonTempletBase;
						if (NKCRepeatOperaion.CheckVisible(stageID) && dungeonTempletBase != null)
						{
							if (dungeonTempletBase.IsUsingEventDeck())
							{
								NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastEventDeck(), stageID, 0, dungeonTempletBase.m_DungeonID, 0, bLocal: false, 1, 0, 0L);
							}
							else
							{
								NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastDeckIndex()
									.m_iIndex, stageID, 0, dungeonTempletBase.m_DungeonID, 0, bLocal: false, 1, 0, 0L);
								}
								return;
							}
						}
						ExitToDungeonResult(nKMStageTempletV, resultData);
					};
					break;
				case NKM_GAME_TYPE.NGT_DIVE:
					ClosingDelegate = ExitToDiveInstance;
					flag = resultData.GetAllListRewardSlotData().Count > 0;
					break;
				case NKM_GAME_TYPE.NGT_RAID:
				case NKM_GAME_TYPE.NGT_RAID_SOLO:
					ClosingDelegate = ExitToRaid;
					break;
				case NKM_GAME_TYPE.NGT_SHADOW_PALACE:
					if (resultData.m_ShadowCurrLife == 0 || resultData.m_bShadowAllClear)
					{
						ClosingDelegate = delegate
						{
							ExitToShadow(bGiveUp: false, bEnd: true);
						};
					}
					else
					{
						ClosingDelegate = delegate
						{
							ExitToShadow(bGiveUp: false, bEnd: false);
						};
					}
					break;
				case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_ARENA:
				case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS:
				case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE:
					ClosingDelegate = delegate
					{
						ExitToGuildCoop();
					};
					break;
				case NKM_GAME_TYPE.NGT_FIERCE:
					ClosingDelegate = delegate
					{
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT);
					};
					break;
				case NKM_GAME_TYPE.NGT_PVE_DEFENCE:
					flag = false;
					ClosingDelegate = delegate
					{
						NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(resultData.m_stageID);
						ExitToDungeonResult(dungeonTempletBase, resultData);
					};
					GameResultUIData = null;
					break;
				}
				if (flag2)
				{
					NKCLoadingScreenManager.CleanupIntroObject();
				}
				if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_TUTORIAL && cNKMDungeonTempletBase != null && NKCUtil.m_sHsFirstClearDungeon.Contains(cNKMDungeonTempletBase.m_DungeonID) && NKCTutorialManager.IsPrologueDungeon(cNKMDungeonTempletBase.m_DungeonID))
				{
					if (resultData.m_BATTLE_RESULT_TYPE == BATTLE_RESULT_TYPE.BRT_WIN)
					{
						NKCUtil.m_sHsFirstClearDungeon.Remove(cNKMDungeonTempletBase.m_DungeonID);
						PlayTutorialNextDungeon(cNKMDungeonTempletBase.m_DungeonID);
					}
					else
					{
						PlayNextDungeon(cNKMDungeonTempletBase.m_DungeonID);
					}
					return;
				}
				bool bResultAutoSkip = CheckAutoGame();
				if (cNKMGameData.GetGameType() == NKM_GAME_TYPE.NGT_TRIM && NKCScenManager.CurrentUserData().m_UserOption != null && NKCScenManager.CurrentUserData().m_UserOption.m_bAutoDive)
				{
					bResultAutoSkip = true;
				}
				if (resultData.m_BATTLE_RESULT_TYPE == BATTLE_RESULT_TYPE.BRT_WIN && cNKMDungeonTempletBase != null)
				{
					NKCCutScenTemplet cNKCCutScenTemplet = NKCCutScenManager.GetCutScenTemple(cNKMDungeonTempletBase.m_CutScenStrIDAfter);
					if (cNKCCutScenTemplet != null && CheckCutscen(cNKMGameData.m_NKM_GAME_TYPE, resultData.m_stageID, cNKMGameData.m_DungeonID))
					{
						if (flag)
						{
							NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().SetDoAtScenStart(delegate
							{
								NKCUIResult.Instance.OpenBattleResult(resultData, delegate
								{
									NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(cNKCCutScenTemplet.m_CutScenStrID, delegate
									{
										ClosingDelegate();
									}, cNKMDungeonTempletBase.m_DungeonStrID);
									NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
								}, bResultAutoSkip);
							});
							NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME_RESULT);
						}
						else
						{
							NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(cNKCCutScenTemplet.m_CutScenStrID, delegate
							{
								ClosingDelegate();
							}, cNKMDungeonTempletBase.m_DungeonStrID);
							NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
						}
						return;
					}
				}
				if (flag)
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().SetDoAtScenStart(delegate
					{
						NKCUIResult.Instance.OpenBattleResult(resultData, ClosingDelegate, bResultAutoSkip);
						if (resultData.m_stageID == 11214)
						{
							NKCPublisherModule.Notice.OpenPromotionalBanner(NKCPublisherModule.NKCPMNotice.eOptionalBannerPlaces.ep1act4clear, null);
						}
					});
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME_RESULT);
				}
				else
				{
					ClosingDelegate();
				}
			}
			else
			{
				Debug.LogError("Reward UI Type Not Defined!");
				NKCUIResult.Instance.OpenBattleResult(resultData, delegate
				{
					ExitGame(NKM_SCEN_ID.NSI_HOME);
				});
			}
		}

		private bool CheckCutscen(NKM_GAME_TYPE gameType, int stageID, int dungeonID)
		{
			if (CheckAutoGame())
			{
				return false;
			}
			bool flag = true;
			if (NKCScenManager.CurrentUserData() != null)
			{
				flag = NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene;
			}
			switch (gameType)
			{
			case NKM_GAME_TYPE.NGT_DIVE:
				return true;
			case NKM_GAME_TYPE.NGT_PHASE:
				if (flag || NKCPhaseManager.WasPhaseStageFirstClear(stageID))
				{
					return true;
				}
				break;
			case NKM_GAME_TYPE.NGT_TRIM:
				return NKCTrimManager.WillPlayTrimDungeonCutscene(NKCTrimManager.TrimModeState.trimId, dungeonID, NKCTrimManager.TrimModeState.trimLevel);
			default:
			{
				NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
				if (dungeonTempletBase == null)
				{
					return false;
				}
				if (NKCUtil.m_sHsFirstClearDungeon.Contains(dungeonTempletBase.m_DungeonID) || flag)
				{
					NKCUtil.m_sHsFirstClearDungeon.Remove(dungeonTempletBase.m_DungeonID);
					return true;
				}
				break;
			}
			}
			return false;
		}

		private bool GetIsWinGame()
		{
			if (NKCScenManager.GetScenManager().GetGameClient().GetMyTeamData() != null && !NKCScenManager.GetScenManager().GetGameClient().IsEnemy(NKCScenManager.GetScenManager().GetGameClient().GetMyTeamData()
				.m_eNKM_TEAM_TYPE, NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData()
					.m_WinTeam))
					{
						return true;
					}
					return false;
				}

				private void ExitGameToHome()
				{
					ExitGame(NKM_SCEN_ID.NSI_HOME);
				}

				private void SetShow(bool bShow)
				{
					if (m_NUM_GAME.activeSelf == !bShow)
					{
						m_NUM_GAME.SetActive(bShow);
					}
					if (m_NUF_GAME_Panel.activeSelf == !bShow)
					{
						m_NUF_GAME_Panel.SetActive(bShow);
					}
					if (m_NKM_LENS_FLARE_LIST.activeSelf == !bShow)
					{
						m_NKM_LENS_FLARE_LIST.SetActive(bShow);
					}
				}

				private void PlayNextDungeon(int dungeonID)
				{
					NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(NKMDungeonManager.GetDungeonTempletBase(dungeonID));
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
				}

				private void MoveToWarfare(string warfareStrID)
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().SetWarfareStrID(warfareStrID);
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WARFARE_GAME);
				}

				public void OpenPracticeGameComfirmPopup(NKMUnitData cNKMUnitData, NKM_SHORTCUT_TYPE returnUIShortcut = NKM_SHORTCUT_TYPE.SHORTCUT_NONE, string returnUIShortcutParam = "")
				{
					List<NKCPopupCommonChoice.ChoiceOption> list = new List<NKCPopupCommonChoice.ChoiceOption>();
					list.Add(new NKCPopupCommonChoice.ChoiceOption(NKCStringTable.GetString("SI_DP_COLLECTION_TRAINING_MODE_PRACTICE"), NKMDungeonManager.GetDungeonID("NKM_BATTLE_PRACTICE")));
					list.Add(new NKCPopupCommonChoice.ChoiceOption(NKCStringTable.GetString("SI_DP_COLLECTION_TRAINING_MODE_NORMAL"), NKMDungeonManager.GetDungeonID("NKM_BATTLE_PRACTICE_2")));
					NKCPopupCommonChoice.Instance.Open(list, NKCStringTable.GetString("SI_DP_COLLECTION_TRAINING_MODE_TITLE"), delegate(int dungeonID)
					{
						PlayPracticeGame(cNKMUnitData, dungeonID, returnUIShortcut, returnUIShortcutParam);
					});
				}

				public void PlayPracticeGame(NKMUnitData cNKMUnitData, int dungeonID, NKM_SHORTCUT_TYPE returnUIShortcut = NKM_SHORTCUT_TYPE.SHORTCUT_NONE, string returnUIShortcutParam = "")
				{
					m_ePracticeReturnScenID = NKCScenManager.GetScenManager().GetNowScenID();
					m_ePractiveReturnShortcut = returnUIShortcut;
					m_strPractiveReturnShortcutParam = returnUIShortcutParam;
					NKCPacketSender.Send_NKMPacket_PRACTICE_GAME_LOAD_REQ(cNKMUnitData, dungeonID);
				}

				public void EndPracticeGame()
				{
					NKCLocalServerManager.MakeNewLocalGame();
					if (m_ePractiveReturnShortcut == NKM_SHORTCUT_TYPE.SHORTCUT_NONE)
					{
						NKCScenManager.GetScenManager().ScenChangeFade(m_ePracticeReturnScenID);
					}
					else
					{
						NKCContentManager.MoveToShortCut(m_ePractiveReturnShortcut, m_strPractiveReturnShortcutParam, bForce: true);
					}
					m_ePractiveReturnShortcut = NKM_SHORTCUT_TYPE.SHORTCUT_NONE;
					m_strPractiveReturnShortcutParam = "";
				}

				private void PlayTutorialNextDungeon(int clearedDungeonID)
				{
					NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(clearedDungeonID);
					NKCCutScenTemplet cNKCCutScenTemplet = NKCCutScenManager.GetCutScenTemple(dungeonTempletBase.m_CutScenStrIDAfter);
					if (cNKCCutScenTemplet != null)
					{
						switch (dungeonTempletBase.m_DungeonID)
						{
						case 1004:
							PlayDungeonAfterCutscene(1005);
							break;
						case 1005:
							PlayDungeonAfterCutscene(1006);
							break;
						default:
							PlayDungeonAfterCutscene(1007);
							break;
						}
					}
					else
					{
						switch (dungeonTempletBase.m_DungeonID)
						{
						case 1004:
							PlayNextDungeon(1005);
							break;
						case 1005:
							PlayNextDungeon(1006);
							break;
						default:
							PlayNextDungeon(1007);
							break;
						}
					}
					void PlayDungeonAfterCutscene(int nextDungeonID)
					{
						NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(cNKCCutScenTemplet.m_CutScenStrID, delegate
						{
							PlayNextDungeon(nextDungeonID);
						});
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
					}
				}

				public void OnRecv(NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT cPacket_NPT_GAME_SYNC_DATA_PACK_NOT)
				{
					if (NKCReplayMgr.IsRecording())
					{
						NKCScenManager.GetScenManager().GetNKCReplayMgr().FillReplayData(cPacket_NPT_GAME_SYNC_DATA_PACK_NOT);
					}
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cPacket_NPT_GAME_SYNC_DATA_PACK_NOT);
				}

				public void OnRecv(NKMPacket_GAME_PAUSE_ACK cNKMPacket_GAME_PAUSE_ACK)
				{
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cNKMPacket_GAME_PAUSE_ACK);
				}

				public void OnRecv(NKMPacket_GAME_SPEED_2X_ACK cNKMPacket_GAME_SPEED_2X_ACK)
				{
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cNKMPacket_GAME_SPEED_2X_ACK);
				}

				public void OnRecv(NKMPacket_GAME_AUTO_SKILL_CHANGE_ACK cNKMPacket_GAME_AUTO_SKILL_CHANGE_ACK)
				{
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cNKMPacket_GAME_AUTO_SKILL_CHANGE_ACK);
				}

				public void OnRecv(NKMPacket_GAME_USE_UNIT_SKILL_ACK cNKMPacket_GAME_USE_UNIT_SKILL_ACK)
				{
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cNKMPacket_GAME_USE_UNIT_SKILL_ACK);
				}

				public void OnRecv(NKMPacket_GAME_DEV_COOL_TIME_RESET_ACK cNKMPacket_GAME_DEV_COOL_TIME_RESET_ACK)
				{
				}

				public void OnRecv(NKMPacket_GAME_DEV_RESPAWN_ACK cNKMPacket_GAME_DEV_RESPAWN_ACK)
				{
				}

				public void OnRecv(NKMPacket_GAME_SHIP_SKILL_ACK cNKMPacket_GAME_SHIP_SKILL_ACK)
				{
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cNKMPacket_GAME_SHIP_SKILL_ACK);
				}

				public void OnRecv(NKMPacket_GAME_TACTICAL_COMMAND_ACK cNKMPacket_GAME_TACTICAL_COMMAND_ACK)
				{
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cNKMPacket_GAME_TACTICAL_COMMAND_ACK);
				}

				public void OnRecv(NKMPacket_GAME_RESPAWN_ACK cPacket_GAME_RESPAWN_ACK)
				{
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cPacket_GAME_RESPAWN_ACK);
				}

				public void OnRecv(NKMPacket_GAME_UNIT_RETREAT_ACK cPacket_GAME_UNIT_RETREAT_ACK)
				{
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cPacket_GAME_UNIT_RETREAT_ACK);
				}

				public void OnRecv(NKMPacket_GAME_OPTION_CHANGE_ACK cNKMPacket_GAME_OPTION_CHANGE_ACK)
				{
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cNKMPacket_GAME_OPTION_CHANGE_ACK);
				}

				public void OnRecv(NKMPacket_GAME_AUTO_RESPAWN_ACK cPacket_GAME_AUTO_RESPAWN_ACK)
				{
					NKCScenManager.GetScenManager().GetGameClient().OnRecv(cPacket_GAME_AUTO_RESPAWN_ACK);
				}
			}
