using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClientPacket.Game;
using ClientPacket.Pvp;
using ClientPacket.Service;
using Cs.GameServer.Replay;
using Cs.Logging;
using Cs.Protocol;
using NKC.UI;
using NKC.UI.Option;
using NKC.UI.Result;
using NKM;

namespace NKC;

public class NKCReplayMgr
{
	private enum ReplayMgrState
	{
		RMS_None,
		RMS_Recording,
		RMS_Playing,
		RMS_PlayingResult
	}

	private ReplayMgrState m_currentRMS;

	private ReplayRecorder m_replayRecorder;

	private ReplayData m_currentReplayData;

	private NKM_GAME_SPEED_TYPE m_NKM_GAME_SPEED_TYPE;

	private bool m_bFinishState;

	private Dictionary<string, string> m_dicReplaydata = new Dictionary<string, string>();

	public ReplayData CurrentReplayData => m_currentReplayData;

	public static bool IsReplayLobbyTabOpened()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.FIREBASE_CRASH_TEST))
		{
			return true;
		}
		return false;
	}

	public static bool IsReplayRecordingOpened()
	{
		if (NKCScenManager.GetScenManager().GetNKCReplayMgr() == null)
		{
			return false;
		}
		if (NKCScenManager.GetScenManager().GetGameClient() != null && NKCScenManager.GetScenManager().GetGameClient().IsObserver(NKCScenManager.CurrentUserData()))
		{
			return false;
		}
		if (IsReplayOpened())
		{
			return true;
		}
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_REPLAY_RECORDING))
		{
			return false;
		}
		return true;
	}

	public static bool IsReplayOpened()
	{
		if (NKCScenManager.GetScenManager().GetNKCReplayMgr() == null)
		{
			return false;
		}
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_REPLAY))
		{
			return false;
		}
		return true;
	}

	public static bool IsRecording()
	{
		if (NKCScenManager.GetScenManager().GetNKCReplayMgr() == null)
		{
			return false;
		}
		if (NKCScenManager.GetScenManager().GetNKCReplayMgr().m_currentRMS == ReplayMgrState.RMS_Recording)
		{
			return true;
		}
		return false;
	}

	public static bool IsPlayingReplay()
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_REPLAY))
		{
			return false;
		}
		if (NKCScenManager.GetScenManager().GetNKCReplayMgr() == null)
		{
			return false;
		}
		if (NKCScenManager.GetScenManager().GetNKCReplayMgr().m_currentRMS == ReplayMgrState.RMS_Playing || NKCScenManager.GetScenManager().GetNKCReplayMgr().m_currentRMS == ReplayMgrState.RMS_PlayingResult)
		{
			return true;
		}
		return false;
	}

	public static NKCReplayMgr GetNKCReplaMgr()
	{
		if (NKCScenManager.GetScenManager() == null)
		{
			return null;
		}
		return NKCScenManager.GetScenManager().GetNKCReplayMgr();
	}

	public void CreateNewReplayData(NKMGameData cNKMGameData, NKMGameRuntimeData cNKMGameRuntimeData)
	{
		if (!IsReplayRecordingOpened() || m_currentRMS != ReplayMgrState.RMS_None || cNKMGameData == null || cNKMGameRuntimeData == null)
		{
			return;
		}
		if (m_replayRecorder != null)
		{
			SaveReplayData();
		}
		string text = MakeReplayDataFileName(cNKMGameData.m_GameUID);
		NKMGameData gameData = cNKMGameData.DeepCopy();
		ResetLoadingData(ref gameData);
		try
		{
			m_replayRecorder = new ReplayRecorder(text, gameData, cNKMGameRuntimeData);
			m_currentRMS = ReplayMgrState.RMS_Recording;
			Log.Info($"<color=#FFFFF0FF>[Replay] Create New ReplayData [{text}] GameType[{cNKMGameData.GetGameType()}]</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 188);
		}
		catch
		{
			m_currentRMS = ReplayMgrState.RMS_None;
			Log.Info("<color=#FFFFF0FF>[Replay] Create New ReplayData Failed </color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 193);
		}
	}

	public void FillReplayData(NKMPacket_GAME_EMOTICON_NOT cNKMPacket_GAME_EMOTICON_NOT)
	{
		if (m_replayRecorder != null && m_currentRMS == ReplayMgrState.RMS_Recording)
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (gameClient != null && gameClient.GetGameRuntimeData() != null)
			{
				new NKMPacket_GAME_EMOTICON_NOT().DeepCopyFrom(cNKMPacket_GAME_EMOTICON_NOT);
				ReplayData.EmoticonData emoticonData = new ReplayData.EmoticonData();
				emoticonData.not = new NKMPacket_GAME_EMOTICON_NOT();
				emoticonData.not.DeepCopyFrom(cNKMPacket_GAME_EMOTICON_NOT);
				emoticonData.time = gameClient.GetGameRuntimeData().m_GameTime;
				m_replayRecorder.AddEmoticonData(emoticonData);
			}
		}
	}

	public void FillReplayData(NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT cNKMPacket_GAME_SYNC_DATA_PACK_NOT)
	{
		if (m_replayRecorder != null && m_currentRMS == ReplayMgrState.RMS_Recording)
		{
			m_replayRecorder.AddSyncData(cNKMPacket_GAME_SYNC_DATA_PACK_NOT.DeepCopy());
		}
	}

	public void FillReplayData(NKMPacket_ASYNC_PVP_GAME_END_NOT cNKMPacket_ASYNC_PVP_GAME_END_NOT)
	{
		if (m_replayRecorder != null && m_currentRMS == ReplayMgrState.RMS_Recording)
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			m_replayRecorder.SetGameResult(cNKMPacket_ASYNC_PVP_GAME_END_NOT.result, gameClient.GetGameRuntimeData().m_GameTime, cNKMPacket_ASYNC_PVP_GAME_END_NOT.gameRecord.DeepCopy());
			Log.Info("<color=#FFFFF0FF>[Replay] - GAME_END_NOT</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 257);
		}
	}

	public void FillReplayData(NKMPacket_GAME_END_NOT cNKMPacket_GAME_END_NOT)
	{
		if (m_replayRecorder != null && m_currentRMS == ReplayMgrState.RMS_Recording)
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (cNKMPacket_GAME_END_NOT.pvpResultData != null)
			{
				m_replayRecorder.SetGameResult(cNKMPacket_GAME_END_NOT.pvpResultData.result, gameClient.GetGameRuntimeData().m_GameTime, cNKMPacket_GAME_END_NOT.gameRecord.DeepCopy());
			}
			else
			{
				m_replayRecorder.SetGameResult((!cNKMPacket_GAME_END_NOT.win) ? PVP_RESULT.LOSE : PVP_RESULT.WIN, gameClient.GetGameRuntimeData().m_GameTime, cNKMPacket_GAME_END_NOT.gameRecord.DeepCopy());
			}
			Log.Info("<color=#FFFFF0FF>[Replay] - GAME_END_NOT</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 283);
		}
	}

	public void StopRecording(bool saveData)
	{
		Log.Info("<color=#FFFFF0FF>[Replay] - StopRecording</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 288);
		if (saveData)
		{
			SaveReplayData();
		}
		m_replayRecorder = null;
		m_currentRMS = ReplayMgrState.RMS_None;
	}

	private void SaveReplayData()
	{
		if (m_replayRecorder == null)
		{
			m_currentRMS = ReplayMgrState.RMS_None;
		}
		else if (m_currentRMS == ReplayMgrState.RMS_Recording)
		{
			string userUIDString = "0";
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				userUIDString = nKMUserData.m_UserUID.ToString();
			}
			ReplayRecorder recorder = m_replayRecorder;
			Task.Run(() => recorder.FinishAsync(userUIDString));
			m_replayRecorder = null;
			m_currentRMS = ReplayMgrState.RMS_None;
		}
	}

	public void ReadReplayData()
	{
		if (m_dicReplaydata == null)
		{
			m_dicReplaydata = new Dictionary<string, string>();
		}
		string path = "0";
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			path = nKMUserData.m_UserUID.ToString();
		}
		string text = Path.Combine(Path.Combine(NKCLogManager.GetSavePath(), "Replay"), path);
		if (!Directory.Exists(text))
		{
			Log.Info("<color=#FFFFF0FF>[Replay] Folder doesn't exist : " + text + "</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 344);
			return;
		}
		string[] files = Directory.GetFiles(text);
		Log.Info($"<color=#FFFFF0FF>[Replay] Files in Folder : {files.Length}</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 349);
		string[] array = files;
		foreach (string text2 in array)
		{
			string text3 = Path.GetFileNameWithoutExtension(text2).ToLower();
			if (!text3.Contains("rp2_"))
			{
				Log.Info("<color=#FFFFF0FF>[Replay] Deleting File - old style : " + text2 + "</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 358);
				File.Delete(text2);
			}
			else if (!CheckForExistingPVPHistory(text3))
			{
				Log.Info("<color=#FFFFF0FF>[Replay] Deleting File - old gameUID : " + text2 + "</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 366);
				File.Delete(text2);
			}
			else if (!IsInReplayDataFileList(text3))
			{
				m_dicReplaydata.Add(text3, text2);
			}
		}
		Log.Info($"<color=#FFFFF0FF>[Replay] ReplayCount : {m_dicReplaydata.Count}</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 380);
	}

	public static string MakeReplayDataFileName(long gameUID)
	{
		return $"rp2_{gameUID}";
	}

	public bool IsInReplayDataFileList(string fileName)
	{
		if (m_dicReplaydata == null)
		{
			return false;
		}
		return m_dicReplaydata.ContainsKey(fileName.ToLower());
	}

	public bool IsInReplayDataFileList(long gameUID)
	{
		string fileName = MakeReplayDataFileName(gameUID).ToLower();
		return IsInReplayDataFileList(fileName);
	}

	public ReplayData GetReplayDataByUID(long gameUID)
	{
		if (m_dicReplaydata == null)
		{
			return null;
		}
		string text = MakeReplayDataFileName(gameUID).ToLower();
		if (!m_dicReplaydata.ContainsKey(text.ToLower()))
		{
			return null;
		}
		string path = "0";
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			path = nKMUserData.m_UserUID.ToString();
		}
		string text2 = Path.Combine(Path.Combine(NKCLogManager.GetSavePath(), "Replay"), path);
		if (!Directory.Exists(text2))
		{
			Log.Info("<color=#FFFFF0FF>[Replay] Folder doesn't exist 2: " + text2 + "</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 433);
			return null;
		}
		return ReplayLoader.Load(Path.Combine(text2, text + ".replay"));
	}

	private bool CheckForExistingPVPHistory(string fileName)
	{
		string[] array = fileName.Split('_');
		if (array == null || array.Length < 2)
		{
			return false;
		}
		if (!long.TryParse(array[1], out var result))
		{
			return false;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return true;
		}
		if (nKMUserData.m_AsyncPvpHistory.GetDataByGameUID(result) != null)
		{
			return true;
		}
		if (nKMUserData.m_SyncPvpHistory.GetDataByGameUID(result) != null)
		{
			return true;
		}
		if (nKMUserData.m_LeaguePvpHistory.GetDataByGameUID(result) != null)
		{
			return true;
		}
		if (nKMUserData.m_PrivatePvpHistory.GetDataByGameUID(result) != null)
		{
			return true;
		}
		if (nKMUserData.m_EventPvpHistory.GetDataByGameUID(result) != null)
		{
			return true;
		}
		return false;
	}

	public void OnGameScenEnd()
	{
		if (IsPlayingReplay())
		{
			StopPlaying();
		}
	}

	public void StopPlaying()
	{
		Log.Info("<color=#FFFFF0FF>[Replay] - StopPlaying</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 504);
		m_currentRMS = ReplayMgrState.RMS_None;
		m_currentReplayData = null;
		m_NKM_GAME_SPEED_TYPE = NKM_GAME_SPEED_TYPE.NGST_1;
	}

	public void LeavePlaying()
	{
		Log.Info("<color=#FFFFF0FF>[Replay] - LeavePlaying</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 512);
		NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData()
			.m_GameTime = m_currentReplayData.gameEndTime;
		NKCScenManager.GetScenManager().GetGameClient().UI_GAME_PAUSE();
		NKCUIGameOption.Instance.RemoveCloseCallBack();
		NKCUIGameOption.Instance.Close();
	}

	public void StartPlaying(ReplayData cReplayData)
	{
		if (IsReplayOpened() && m_currentRMS == ReplayMgrState.RMS_None)
		{
			m_currentReplayData = null;
			m_currentReplayData = cReplayData;
			m_currentRMS = ReplayMgrState.RMS_Playing;
			m_NKM_GAME_SPEED_TYPE = NKM_GAME_SPEED_TYPE.NGST_1;
			m_bFinishState = false;
			ResetLoadingData(ref m_currentReplayData.gameData);
			NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(m_currentReplayData.gameData);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
		}
	}

	public void StartPlaying(long gameUID)
	{
		ReplayData replayDataByUID = GetReplayDataByUID(gameUID);
		if (replayDataByUID != null)
		{
			StartPlaying(replayDataByUID);
		}
	}

	private void ResetLoadingData(ref NKMGameData gameData)
	{
		if (gameData.m_NKMGameTeamDataA.m_listDynamicRespawnUnitData.Count > 0)
		{
			foreach (NKMDynamicRespawnUnitData listDynamicRespawnUnitDatum in gameData.m_NKMGameTeamDataA.m_listDynamicRespawnUnitData)
			{
				listDynamicRespawnUnitDatum.m_bLoadedClient = false;
				listDynamicRespawnUnitDatum.m_bLoadedServer = false;
			}
		}
		if (gameData.m_NKMGameTeamDataB.m_listDynamicRespawnUnitData.Count <= 0)
		{
			return;
		}
		foreach (NKMDynamicRespawnUnitData listDynamicRespawnUnitDatum2 in gameData.m_NKMGameTeamDataB.m_listDynamicRespawnUnitData)
		{
			listDynamicRespawnUnitDatum2.m_bLoadedClient = false;
			listDynamicRespawnUnitDatum2.m_bLoadedServer = false;
		}
	}

	public void SetPlayingGameSpeedType(NKM_GAME_SPEED_TYPE gameSpeedType)
	{
		m_NKM_GAME_SPEED_TYPE = gameSpeedType;
	}

	public NKM_GAME_SPEED_TYPE GetPlayingGameSpeedType()
	{
		return m_NKM_GAME_SPEED_TYPE;
	}

	public void PrintPoolingData(NKMGameData cNKMGameData)
	{
		Log.Info("<color=#FFFFF0FF>[Replay] --------------PrintPoolingData--------------</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 585);
		foreach (NKMUnitData listUnitDatum in cNKMGameData.m_NKMGameTeamDataA.m_listUnitData)
		{
			Log.Info($"<color=#FFFFF0FF>[Replay] Create TeamA - [{listUnitDatum.m_UnitUID}]</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 588);
		}
		foreach (NKMUnitData listUnitDatum2 in cNKMGameData.m_NKMGameTeamDataB.m_listUnitData)
		{
			Log.Info($"<color=#FFFFF0FF>[Replay] Create TeamB - [{listUnitDatum2.m_UnitUID}]</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 593);
		}
		if (cNKMGameData.m_NKMGameTeamDataA.m_listDynamicRespawnUnitData.Count > 0)
		{
			foreach (NKMDynamicRespawnUnitData listDynamicRespawnUnitDatum in cNKMGameData.m_NKMGameTeamDataA.m_listDynamicRespawnUnitData)
			{
				foreach (short item in listDynamicRespawnUnitDatum.m_NKMUnitData.m_listGameUnitUID)
				{
					Log.Info($"<color=#FFFFF0FF>[Replay] Create TeamA D - [{item}]</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 602);
				}
			}
		}
		if (cNKMGameData.m_NKMGameTeamDataB.m_listDynamicRespawnUnitData.Count > 0)
		{
			foreach (NKMDynamicRespawnUnitData listDynamicRespawnUnitDatum2 in cNKMGameData.m_NKMGameTeamDataB.m_listDynamicRespawnUnitData)
			{
				foreach (short item2 in listDynamicRespawnUnitDatum2.m_NKMUnitData.m_listGameUnitUID)
				{
					Log.Info($"<color=#FFFFF0FF>[Replay] Create TeamB D - [{item2}]</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 613);
				}
			}
		}
		Log.Info("<color=#FFFFF0FF>[Replay] --------------PrintPoolingData End---------------</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 617);
	}

	private void PrintRespawnAndDieInfo(ReplayData cReplayData)
	{
		Log.Info("<color=#FFFFF0FF>[Replay] --------------PrintRespawnAndDieInfo--------------</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 622);
		foreach (NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT sync in cReplayData.syncList)
		{
			foreach (NKMGameSyncData_Base listGameSyncDatum in sync.gameSyncDataPack.m_listGameSyncData)
			{
				foreach (NKMGameSyncData_Unit item in listGameSyncDatum.m_NKMGameSyncData_Unit)
				{
					if (item.m_NKMGameUnitSyncData.m_bRespawnThisFrame)
					{
						Log.Info($"<color=#FFFFF0FF>[Replay] [{sync.gameTime}] respawn unit[{item.m_NKMGameUnitSyncData.m_GameUnitUID}]</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 631);
					}
				}
			}
		}
		Log.Info("<color=#FFFFF0FF>-------------------------------------------</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 638);
		foreach (NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT sync2 in cReplayData.syncList)
		{
			foreach (NKMGameSyncData_Base listGameSyncDatum2 in sync2.gameSyncDataPack.m_listGameSyncData)
			{
				foreach (NKMGameSyncData_DieUnit item2 in listGameSyncDatum2.m_NKMGameSyncData_DieUnit)
				{
					foreach (short item3 in item2.m_DieGameUnitUID)
					{
						Log.Info($"<color=#FFFFF0FF>[Replay] [{sync2.gameTime}] die unit [{item3.ToString()}]</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 647);
					}
				}
			}
		}
		Log.Info("<color=#FFFFF0FF>[Replay] --------------PrintRespawnAndDieInfo End--------------</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 652);
	}

	public void Update(NKMGameRuntimeData cNKMGameRuntimeData)
	{
		if (m_currentRMS != ReplayMgrState.RMS_Playing && m_currentRMS != ReplayMgrState.RMS_PlayingResult)
		{
			return;
		}
		if (cNKMGameRuntimeData.m_NKM_GAME_STATE == NKM_GAME_STATE.NGS_FINISH && m_currentRMS == ReplayMgrState.RMS_Playing)
		{
			BATTLE_RESULT_TYPE bATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;
			bATTLE_RESULT_TYPE = ((m_currentReplayData.pvpResult != PVP_RESULT.WIN) ? ((m_currentReplayData.pvpResult == PVP_RESULT.LOSE) ? BATTLE_RESULT_TYPE.BRT_LOSE : BATTLE_RESULT_TYPE.BRT_DRAW) : BATTLE_RESULT_TYPE.BRT_WIN);
			NKCUIBattleStatistics.BattleData battleData = NKCUIBattleStatistics.MakeBattleData(NKCScenManager.GetScenManager().GetGameClient(), m_currentReplayData.gameRecord, m_currentReplayData.gameData.GetGameType());
			NKCUIResult.BattleResultData resultData = NKCUIResult.MakePvPResultData(bATTLE_RESULT_TYPE, null, battleData, m_currentReplayData.gameData.GetGameType());
			NKCScenManager.GetScenManager().Get_SCEN_GAME().ReserveGameEndData(resultData);
			m_currentRMS = ReplayMgrState.RMS_PlayingResult;
			return;
		}
		while (m_currentReplayData.emoticonList.Count > 0 && !(m_currentReplayData.emoticonList[0].time > cNKMGameRuntimeData.m_GameTime))
		{
			NKCScenManager.GetScenManager().GetGameClient().GetGameHud()
				.GetNKCGameHudEmoticon()
				.OnRecv(m_currentReplayData.emoticonList[0].not);
			m_currentReplayData.emoticonList.RemoveAt(0);
		}
		while (m_currentReplayData.syncList.Count > 0)
		{
			if (m_currentReplayData.syncList[0] != null)
			{
				if (m_currentReplayData.syncList[0].gameTime > cNKMGameRuntimeData.m_GameTime)
				{
					break;
				}
				foreach (NKMGameSyncData_Base listGameSyncDatum in m_currentReplayData.syncList[0].gameSyncDataPack.m_listGameSyncData)
				{
					if (listGameSyncDatum.m_NKMGameSyncData_GameState == null)
					{
						continue;
					}
					foreach (NKMGameSyncData_GameState item2 in listGameSyncDatum.m_NKMGameSyncData_GameState)
					{
						switch (item2.m_NKM_GAME_STATE)
						{
						case NKM_GAME_STATE.NGS_START:
						case NKM_GAME_STATE.NGS_PLAY:
						case NKM_GAME_STATE.NGS_END:
							Log.Info("<color=#FFFFF0FF>[Replay] SyncGameState [" + item2.m_NKM_GAME_STATE.ToString() + "]</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 722);
							break;
						case NKM_GAME_STATE.NGS_FINISH:
							m_bFinishState = true;
							if (m_currentReplayData.pvpResult == PVP_RESULT.WIN)
							{
								cNKMGameRuntimeData.m_WinTeam = NKM_TEAM_TYPE.NTT_A1;
							}
							else if (m_currentReplayData.pvpResult == PVP_RESULT.LOSE)
							{
								cNKMGameRuntimeData.m_WinTeam = NKM_TEAM_TYPE.NTT_B1;
							}
							else
							{
								cNKMGameRuntimeData.m_WinTeam = NKM_TEAM_TYPE.NTT_DRAW;
							}
							Log.Info("<color=#FFFFF0FF>[Replay] SyncGameState [" + item2.m_NKM_GAME_STATE.ToString() + "]</color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 742);
							break;
						}
					}
				}
			}
			NKCScenManager.GetScenManager().GetGameClient().OnRecv(m_currentReplayData.syncList[0]);
			m_currentReplayData.syncList.RemoveAt(0);
			if (m_currentReplayData.syncList.Count <= 0 && !m_bFinishState)
			{
				Log.Info("<color=#FFFFF0FF>[Replay] This replay data has some logic error. Add SyncData include finish state. </color>", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCReplayMgr.cs", 757);
				NKM_TEAM_TYPE winTeam = ((m_currentReplayData.pvpResult == PVP_RESULT.WIN) ? NKM_TEAM_TYPE.NTT_A1 : ((m_currentReplayData.pvpResult != PVP_RESULT.LOSE) ? NKM_TEAM_TYPE.NTT_DRAW : NKM_TEAM_TYPE.NTT_B1));
				NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT item = new NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT
				{
					gameSyncDataPack = new NKMGameSyncDataPack
					{
						m_listGameSyncData = new List<NKMGameSyncData_Base>
						{
							new NKMGameSyncData_Base
							{
								m_NKMGameSyncData_GameState = new List<NKMGameSyncData_GameState>
								{
									new NKMGameSyncData_GameState
									{
										m_NKM_GAME_STATE = NKM_GAME_STATE.NGS_FINISH,
										m_WinTeam = winTeam
									}
								}
							}
						}
					}
				};
				m_currentReplayData.syncList.Add(item);
			}
		}
	}

	public void OnRecv(NKMPacket_INFORM_MY_LOADING_PROGRESS_REQ cNKMPacket_INFORM_MY_LOADING_PROGRESS_REQ)
	{
	}

	public void OnRecv(NKMPacket_GAME_LOAD_COMPLETE_REQ cNKMPacket_GAME_LOAD_COMPLETE_REQ)
	{
		NKMPacket_GAME_LOAD_COMPLETE_ACK nKMPacket_GAME_LOAD_COMPLETE_ACK = new NKMPacket_GAME_LOAD_COMPLETE_ACK();
		nKMPacket_GAME_LOAD_COMPLETE_ACK.gameRuntimeData = new NKMGameRuntimeData();
		nKMPacket_GAME_LOAD_COMPLETE_ACK.gameRuntimeData.m_NKM_GAME_STATE = NKM_GAME_STATE.NGS_START;
		nKMPacket_GAME_LOAD_COMPLETE_ACK.gameRuntimeData.m_NKM_GAME_SPEED_TYPE = NKM_GAME_SPEED_TYPE.NGST_1;
		NKCScenManager.GetScenManager().GetGameClient().OnRecv(nKMPacket_GAME_LOAD_COMPLETE_ACK);
		NKMPacket_GAME_START_NOT cNKMPacket_GAME_START_NOT = new NKMPacket_GAME_START_NOT();
		NKCScenManager.GetScenManager().Get_SCEN_GAME().OnRecv(cNKMPacket_GAME_START_NOT);
	}
}
