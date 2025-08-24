using ClientPacket.Warfare;
using Cs.Logging;
using NKC.UI;
using NKC.UI.Warfare;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_WARFARE_GAME : NKC_SCEN_BASIC
{
	private GameObject m_NUM_WARFARE;

	public NKCUIManager.LoadedUIData m_WarfareGameUIData;

	private NKCWarfareGame m_NKCWarfareGame;

	private string m_WarfareStrID = "";

	private bool m_bAfterBattle;

	private bool m_bRetry;

	private NKCWarfareGame.DataBeforeBattle m_DataBeforeBattle;

	private NKCWarfareGame.RetryData m_retryData;

	private int TutorialSelectDeckIndex = -1;

	public bool WaitAutoPacekt
	{
		get
		{
			return m_NKCWarfareGame?.WaitAutoPacket ?? false;
		}
		set
		{
			if (m_NKCWarfareGame != null)
			{
				m_NKCWarfareGame.WaitAutoPacket = value;
			}
		}
	}

	public Transform GetWarfareParentTransform()
	{
		return m_NUM_WARFARE?.transform;
	}

	public NKC_SCEN_WARFARE_GAME()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_WARFARE_GAME;
		m_NUM_WARFARE = GameObject.Find("NUM_WARFARE");
	}

	public NKCWarfareGame.RetryData GetRetryData()
	{
		return m_retryData;
	}

	public void OpenWaitBox()
	{
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.OpenWaitBox();
		}
	}

	public string GetWarfareStrID()
	{
		return m_WarfareStrID;
	}

	public void SetWarfareStrID(string warfareStrID)
	{
		m_WarfareStrID = warfareStrID;
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.SetWarfareStrID(m_WarfareStrID);
		}
	}

	public void SetRetry(bool bSet)
	{
		m_bRetry = bSet;
	}

	public void SetBattleInfo(WarfareGameData gameData, WarfareSyncData syncData)
	{
		if (gameData != null && syncData != null)
		{
			m_DataBeforeBattle = new NKCWarfareGame.DataBeforeBattle(gameData, syncData);
		}
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_START_ACK cNKMPacket_WARFARE_GAME_START_ACK)
	{
		if (cNKMPacket_WARFARE_GAME_START_ACK.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			m_retryData = new NKCWarfareGame.RetryData(m_WarfareStrID, cNKMPacket_WARFARE_GAME_START_ACK.warfareGameData.warfareTeamDataA);
		}
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.OnRecv(cNKMPacket_WARFARE_GAME_START_ACK);
		}
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_MOVE_ACK cNKMPacket_WARFARE_GAME_MOVE_ACK)
	{
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.OnRecv(cNKMPacket_WARFARE_GAME_MOVE_ACK);
		}
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_TURN_FINISH_ACK cNKMPacket_WARFARE_GAME_TURN_FINISH_ACK)
	{
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.OnRecv(cNKMPacket_WARFARE_GAME_TURN_FINISH_ACK);
		}
	}

	public void InitWaitNextOrder()
	{
		m_NKCWarfareGame?.InitWaitNextOrder();
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_NEXT_ORDER_ACK cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK)
	{
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.OnRecv(cNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK);
		}
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_USE_SERVICE_ACK cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK)
	{
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.OnRecv(cNKMPacket_WARFARE_GAME_USE_SERVICE_ACK);
		}
	}

	public void OnRecv(NKMPacket_WARFARE_EXPIRED_NOT cNKMPacket_WARFARE_EXPIRED_NOT)
	{
		if (NKCWarfareGame.IsInstanceOpen)
		{
			NKCWarfareGame.GetInstance().ForceBack();
		}
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		m_NUM_WARFARE.SetActive(value: true);
		if (!NKCUIManager.IsValid(m_WarfareGameUIData))
		{
			m_WarfareGameUIData = NKCWarfareGame.OpenNewInstanceAsync();
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			Debug.LogError("NKC_SCEN_WARFARE_GAME.LoadUIStart - WarfareGameData is null");
			return;
		}
		switch (warfareGameData.warfareGameState)
		{
		case NKM_WARFARE_GAME_STATE.NWGS_STOP:
		{
			NKMWarfareTemplet nKMWarfareTemplet2 = NKMWarfareTemplet.Find(m_WarfareStrID);
			if (nKMWarfareTemplet2 != null && !NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing() && (!myUserData.CheckWarfareClear(m_WarfareStrID) || NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene) && NKCCutScenManager.GetCutScenTemple(nKMWarfareTemplet2.m_CutScenStrIDBefore) != null)
			{
				NKCUICutScenPlayer.Instance.UnLoad();
				NKCUICutScenPlayer.Instance.Load(nKMWarfareTemplet2.m_CutScenStrIDBefore);
			}
			break;
		}
		case NKM_WARFARE_GAME_STATE.NWGS_RESULT:
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_WarfareStrID);
			if (warfareGameData.isWinTeamA && !NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing() && (NKCUtil.m_sHsFirstClearWarfare.Contains(warfareGameData.warfareTempletID) || NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene) && nKMWarfareTemplet != null && NKCCutScenManager.GetCutScenTemple(nKMWarfareTemplet.m_CutScenStrIDAfter) != null)
			{
				NKCUICutScenPlayer.Instance.UnLoad();
				NKCUICutScenPlayer.Instance.Load(nKMWarfareTemplet.m_CutScenStrIDAfter);
			}
			break;
		}
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (!m_bLoadedUI && m_NKCWarfareGame == null)
		{
			if (m_WarfareGameUIData == null || !m_WarfareGameUIData.CheckLoadAndGetInstance<NKCWarfareGame>(out m_NKCWarfareGame))
			{
				Debug.LogError("Error - NKC_SCEN_WARFARE_GAME.ScenLoadComplete() : UI Load Failed!");
				return;
			}
			m_NKCWarfareGame.transform.SetParent(m_NUM_WARFARE.transform, worldPositionStays: false);
			m_NKCWarfareGame.InitUI();
		}
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.SetWarfareStrID(m_WarfareStrID);
		}
	}

	public override void ScenLoadLastStart()
	{
		base.ScenLoadLastStart();
		m_NKC_SCEN_STATE = NKC_SCEN_STATE.NSS_LOADING_LAST;
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
	}

	public void SetEpisodeID(bool bReservedDetailOption = false)
	{
		if (NKMWarfareTemplet.Find(m_WarfareStrID) != null && NKMEpisodeMgr.FindStageTempletByBattleStrID(m_WarfareStrID) == null)
		{
		}
	}

	public void TryPause()
	{
		if (!(m_NKCWarfareGame == null) && m_NKCWarfareGame.IsOpen)
		{
			m_NKCWarfareGame.OnClickPause();
		}
	}

	public void TryGiveUp()
	{
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.GiveUp();
		}
	}

	public void TryTempLeave()
	{
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.TempLeave();
		}
	}

	public void ProcessReLogin()
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		if (warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			if (NKCUICutScenPlayer.IsInstanceOpen && NKCUICutScenPlayer.Instance.IsPlaying())
			{
				Log.Debug("WarfareGame - 컷씬 플레이중", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKC_SCEN_WARFARE_GAME.cs", 313);
				if (m_NKCWarfareGame != null)
				{
					m_NKCWarfareGame.ResetGameOption();
				}
			}
			else
			{
				SaveTutorialSelectDeck();
				Log.Debug("WarfareGame - ScenChangeFade(WarfareGame)", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKC_SCEN_WARFARE_GAME.cs", 322);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WARFARE_GAME);
			}
		}
		else
		{
			NKCScenManager.GetScenManager().WarfareGameData.warfareTempletID = NKCWarfareManager.GetWarfareID(m_WarfareStrID);
			if (m_NKCWarfareGame != null)
			{
				m_NKCWarfareGame.SetUserUnitDeckWarfareState();
			}
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCWarfareGame.RetryData retryData = null;
		if (m_bRetry)
		{
			retryData = m_retryData;
			m_retryData = null;
			m_bRetry = false;
		}
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.Open(m_bAfterBattle, m_DataBeforeBattle, retryData);
		}
		m_bAfterBattle = false;
		m_DataBeforeBattle = null;
		SetEpisodeID();
		RefreshTutorialSelectDeck();
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_GIVE_UP_ACK cNKMPacket_WARFARE_GAME_GIVE_UP_ACK)
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().SetEpisodeID(bReservedDetailOption: true);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
		NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_AUTO_ACK cNKMPacket_WARFARE_GAME_AUTO_ACK)
	{
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.SetActiveAutoOnOff(cNKMPacket_WARFARE_GAME_AUTO_ACK.isAuto, cNKMPacket_WARFARE_GAME_AUTO_ACK.isAutoRepair);
		}
	}

	public void OnRecv(NKMPacket_WARFARE_FRIEND_LIST_ACK sPacket)
	{
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_WARFARE_RECOVER_ACK sPacket)
	{
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.OnRecv(sPacket);
		}
	}

	public void SetReservedShowBattleResult(bool bSet)
	{
		m_bAfterBattle = bSet;
	}

	public override void ScenEnd()
	{
		if (NKCUICutScenPlayer.HasInstance)
		{
			NKCUICutScenPlayer.Instance.StopWithInvalidatingCallBack();
			NKCUICutScenPlayer.Instance.UnLoad();
		}
		if (m_NKCWarfareGame != null)
		{
			m_NKCWarfareGame.Close();
		}
		base.ScenEnd();
		if (m_NUM_WARFARE != null)
		{
			m_NUM_WARFARE.SetActive(value: false);
		}
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_NKCWarfareGame = null;
		m_WarfareGameUIData?.CloseInstance();
		m_WarfareGameUIData = null;
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void DoAfterLogout()
	{
		m_retryData = null;
	}

	public NKCWarfareGame GetWarfareGame()
	{
		return m_NKCWarfareGame;
	}

	private void SaveTutorialSelectDeck()
	{
		TutorialSelectDeckIndex = m_NKCWarfareGame?.GetTutorialSelectDeck() ?? (-1);
	}

	public void RefreshTutorialSelectDeck()
	{
		if (NKCGameEventManager.IsEventPlaying())
		{
			m_NKCWarfareGame?.RefreshTutorialSelectDeck(TutorialSelectDeckIndex);
			TutorialSelectDeckIndex = -1;
			NKCGameEventManager.ResumeEvent();
		}
	}
}
