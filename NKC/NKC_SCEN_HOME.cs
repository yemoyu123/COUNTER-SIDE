using System;
using System.Collections;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Community;
using ClientPacket.Game;
using ClientPacket.Mode;
using ClientPacket.Pvp;
using ClientPacket.User;
using ClientPacket.Warfare;
using NKA.Service;
using NKC.Publisher;
using NKC.Templet;
using NKC.UI;
using NKC.UI.Event;
using NKC.UI.Fierce;
using NKC.UI.Friend;
using NKC.UI.Lobby;
using NKC.UI.Module;
using NKM;
using NKM.Event;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_HOME : NKC_SCEN_BASIC
{
	public enum RESERVE_OPEN_TYPE
	{
		ROT_NONE,
		ROT_MISSION,
		ROT_RANKING_BOARD,
		ROT_GUIDE_MISSION,
		ROT_EVENT_COLLECTION,
		ROT_EVENT_BANNER,
		ROT_EVENT_RACE,
		ROT_PROFILE,
		ROT_TOURNAMENT
	}

	private NKMTrackingFloat m_BloomIntensity = new NKMTrackingFloat();

	private float m_fElapsedTimeToRefreshDailyContents;

	private NKCUILobbyV2 m_UILobby;

	private NKCUIManager.LoadedUIData m_UILobbyData;

	private NKCPopupTopPlayer m_PopupTopPlayer;

	private bool m_bHaveNewFriendRequest;

	private bool m_bAttendanceRequired;

	private bool m_bFirstLobby = true;

	private float m_pauseTime;

	private int m_ReservedRankingBoardID;

	private int m_eReservedGuideMissionTabID;

	private Coroutine ProcessCoroutine;

	private RESERVE_OPEN_TYPE m_eReservedOpendUIType;

	private int m_iReservedOpenUIID;

	private static bool m_bNeedNewsPopup = true;

	private bool m_bNeedRefreshMail;

	private bool m_bReserverAttendance;

	private List<NKMAttendance> m_lstAttendance = new List<NKMAttendance>();

	private long m_lAttendanceUpdateTime;

	private bool m_bWaitGauntletLeagueTopAck;

	private bool m_bWait;

	private bool m_bRunningLobbyProcess;

	private NKCLocalLoginData m_NKCLocalLoginData;

	public NKC_SCEN_HOME()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_HOME;
	}

	public void SetFriendNewIcon(bool bSet)
	{
		m_bHaveNewFriendRequest = bSet;
	}

	public bool GetHasNewFriendRequest()
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.FRIENDS, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		return m_bHaveNewFriendRequest;
	}

	public void SetMentoringRewardAlarm()
	{
	}

	public void SetAttendanceRequired(bool bSet)
	{
		m_bAttendanceRequired = bSet;
		if (m_UILobby != null && m_UILobby.IsOpen)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			m_UILobby.UpdateButton(NKCUILobbyV2.eUIMenu.Attendance, myUserData);
		}
	}

	public bool GetAttendanceRequired()
	{
		return m_bAttendanceRequired;
	}

	public void SetReservedOpenUI(RESERVE_OPEN_TYPE _reserveType, int _reservedID)
	{
		m_eReservedOpendUIType = _reserveType;
		m_iReservedOpenUIID = _reservedID;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCUIManager.IsValid(m_UILobbyData))
		{
			m_UILobbyData = NKCUILobbyV2.OpenNewInstanceAsync();
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_UILobby == null)
		{
			if (m_UILobbyData == null || !m_UILobbyData.CheckLoadAndGetInstance<NKCUILobbyV2>(out m_UILobby))
			{
				Debug.LogError("Error - NKC_SCEN_HOME.ScenLoadComplete() : UI Load Failed!");
				return;
			}
			m_UILobby.Init();
		}
		SetAttendanceRequired(m_bAttendanceRequired);
	}

	public override void ScenStart()
	{
		base.ScenStart();
		ServiceManager.DownloadService.UnbindService();
		NKCCamera.EnableBloom(bEnable: true);
		NKCCamera.GetCamera().orthographic = false;
		Open();
		NKCCamera.GetTrackingPos().SetNowValue(0f, 0f, -1000f);
		NKCCamera.GetCamera().transform.position = new Vector3(0f, 0f, -1000f);
		NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
		NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetStopReason("");
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().PlayByFavorite = false;
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedEpisodeCategory(EPISODE_CATEGORY.EC_COUNT);
		OnHomeEnter();
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_UILobby.IsOpen)
		{
			m_UILobby.Close();
		}
		Close();
		if (ProcessCoroutine != null)
		{
			NKCScenManager.GetScenManager().StopCoroutine(ProcessCoroutine);
		}
		ProcessCoroutine = null;
		m_bRunningLobbyProcess = false;
		m_bWait = false;
		if (m_UILobby != null)
		{
			m_UILobby.Close();
		}
		m_UILobby = null;
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_UILobbyData?.CloseInstance();
		m_UILobbyData = null;
	}

	public void UpdateRightSide3DButton(NKCUILobbyV2.eUIMenu _eUIMenu)
	{
		if (m_UILobby != null && m_UILobby.IsOpen)
		{
			m_UILobby.UpdateButton(_eUIMenu, NKCScenManager.CurrentUserData());
		}
	}

	public void Open()
	{
		m_fElapsedTimeToRefreshDailyContents = 0f;
		m_UILobby.Open(NKCScenManager.GetScenManager().GetMyUserData());
		switch (m_eReservedOpendUIType)
		{
		case RESERVE_OPEN_TYPE.ROT_MISSION:
			NKCUIMissionAchievement.Instance.Open(m_iReservedOpenUIID);
			break;
		case RESERVE_OPEN_TYPE.ROT_RANKING_BOARD:
		{
			NKMLeaderBoardTemplet reservedTemplet = NKMTempletContainer<NKMLeaderBoardTemplet>.Find(m_ReservedRankingBoardID);
			NKCUILeaderBoard.Instance.Open(reservedTemplet);
			break;
		}
		case RESERVE_OPEN_TYPE.ROT_GUIDE_MISSION:
			NKCUIMissionGuide.Instance.Open(m_eReservedGuideMissionTabID);
			break;
		case RESERVE_OPEN_TYPE.ROT_EVENT_COLLECTION:
			NKCUIModuleHome.OpenEventModule(NKMTempletContainer<NKMEventCollectionIndexTemplet>.Find(m_iReservedOpenUIID));
			break;
		case RESERVE_OPEN_TYPE.ROT_EVENT_BANNER:
		{
			NKMEventTabTemplet reservedTabTemplet = NKMEventTabTemplet.Find(m_iReservedOpenUIID);
			NKCUIEvent.Instance.Open(reservedTabTemplet);
			break;
		}
		case RESERVE_OPEN_TYPE.ROT_EVENT_RACE:
		{
			NKMEventRaceTemplet nKMEventRaceTemplet = NKMEventRaceTemplet.Find(m_iReservedOpenUIID);
			if (nKMEventRaceTemplet == null)
			{
				Debug.Log($"<color=red>찾을 수 없는레이스 이벤트 ID : {m_iReservedOpenUIID}</color>");
				return;
			}
			NKCPopupEventRaceV2.Instance.Open(m_iReservedOpenUIID);
			break;
		}
		case RESERVE_OPEN_TYPE.ROT_PROFILE:
			NKCUIUserInfoV2.Instance.Open(NKCScenManager.GetScenManager().GetMyUserData());
			break;
		case RESERVE_OPEN_TYPE.ROT_TOURNAMENT:
			NKCUIModuleHome.OpenEventModule(NKMTempletContainer<NKMEventCollectionIndexTemplet>.Find(m_iReservedOpenUIID)).GetComponent<NKCUIModuleSubUITournament>()?.OpenPrevTournamentUI();
			break;
		}
		m_eReservedOpendUIType = RESERVE_OPEN_TYPE.ROT_NONE;
		m_iReservedOpenUIID = 0;
		if (NKCDefineManager.DEFINE_ZLONG() && NKCDefineManager.DEFINE_ANDROID() && NKCDefineManager.DEFINE_USE_CHEAT() && !NKCDefineManager.DEFINE_UNITY_EDITOR())
		{
			Debug.Log("SystemInfo.deviceModel : " + SystemInfo.deviceModel + ", Screen.safeArea.x : " + Screen.safeArea.x + ", Screen.safeArea.y : " + Screen.safeArea.y + ", Screen.safeArea.width : " + Screen.safeArea.width + ", Screen.safeArea.height : " + Screen.safeArea.height + ", Screen.currentResolution.width : " + Screen.currentResolution.width + ", Screen.currentResolution.height : " + Screen.currentResolution.height);
		}
	}

	public void ForceOpenLobbyChange()
	{
		NKCUIManager.CloseAllPopup();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKCUIChangeLobby.Instance.Open(myUserData);
	}

	public void Close()
	{
		NKCUIMissionAchievement.CheckInstanceAndClose();
		NKCUIMissionGuide.CheckInstanceAndClose();
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		NKCUtil.ClearGauntletCacheData(NKCScenManager.GetScenManager());
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
		if (m_UILobby != null && m_UILobby.UseCameraTracking() && !NKCCamera.IsTrackingCameraPos() && !NKCUIChangeLobby.IsInstanceOpen)
		{
			NKCCamera.TrackingPos(10f, NKMRandom.Range(-50f, 50f), NKMRandom.Range(-50f, 50f), NKMRandom.Range(-1000f, -900f));
		}
		m_BloomIntensity.Update(Time.deltaTime);
		if (!m_BloomIntensity.IsTracking())
		{
			m_BloomIntensity.SetTracking(NKMRandom.Range(1f, 2f), 4f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		NKCCamera.SetBloomIntensity(m_BloomIntensity.GetNowValue());
		if (!m_bRunningLobbyProcess && !NKCGameEventManager.IsEventPlaying() && !NKCTutorialManager.IsCloseDailyContents() && !NKCUIManager.IsAnyPopupOpened() && !NKMPopUpBox.IsOpenedWaitBox() && m_fElapsedTimeToRefreshDailyContents < Time.time)
		{
			m_fElapsedTimeToRefreshDailyContents = Time.time + 60f;
			CheckDailyContentsReset();
		}
	}

	private void CheckDailyContentsReset()
	{
		if (m_bNeedNewsPopup && NKCNewsManager.CheckNeedNewsPopup(NKCSynchronizedTime.GetServerUTCTime()))
		{
			NKCUINews.Instance.SetDataAndOpen(m_bNeedNewsPopup);
			m_bNeedNewsPopup = false;
		}
		if (m_bNeedRefreshMail)
		{
			NKCMailManager.RefreshMailList();
			m_bNeedRefreshMail = false;
		}
		NKCContentManager.ShowContentUnlockPopup(null);
	}

	private bool CheckAttandence()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		if (NKCContentManager.CheckContentStatus(ContentsType.LOBBY_SUBMENU, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		if (m_bReserverAttendance)
		{
			if (NKCUIAttendance.IsInstanceOpen)
			{
				NKCUIAttendance.Instance.Close();
			}
			SetAttendanceRequired(NKMAttendanceManager.CheckNeedAttendance(myUserData.m_AttendanceData, NKCSynchronizedTime.GetServerUTCTime()));
			OpenAttendanceUI(GetAttendanceKeyList());
		}
		return false;
	}

	public List<int> GetAttendanceKeyList()
	{
		List<int> list = new List<int>();
		if (m_bReserverAttendance && m_lstAttendance != null)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null)
			{
				myUserData.m_AttendanceData.LastUpdateDate = new DateTime(m_lAttendanceUpdateTime);
				int i;
				for (i = 0; i < m_lstAttendance.Count; i++)
				{
					if (myUserData.m_AttendanceData.AttList.Find((NKMAttendance x) => x.IDX == m_lstAttendance[i].IDX) == null)
					{
						myUserData.m_AttendanceData.AttList.Add(m_lstAttendance[i]);
					}
					else
					{
						myUserData.m_AttendanceData.AttList.Find((NKMAttendance x) => x.IDX == m_lstAttendance[i].IDX).Count = m_lstAttendance[i].Count;
						myUserData.m_AttendanceData.AttList.Find((NKMAttendance x) => x.IDX == m_lstAttendance[i].IDX).EventEndDate = m_lstAttendance[i].EventEndDate;
					}
					list.Add(m_lstAttendance[i].IDX);
				}
			}
			m_bReserverAttendance = false;
			m_lAttendanceUpdateTime = 0L;
			m_lstAttendance.Clear();
		}
		return list;
	}

	public void ReserveAttendanceData(List<NKMAttendance> lstAttendance, long updateTime)
	{
		if (lstAttendance.Count > 0)
		{
			m_bReserverAttendance = true;
		}
		else
		{
			m_bReserverAttendance = false;
		}
		m_lstAttendance = lstAttendance;
		m_lAttendanceUpdateTime = updateTime;
	}

	public void OpenAttendanceUI(List<int> lstAttendanceKey)
	{
		if (m_UILobby != null && m_UILobby.IsOpen)
		{
			m_UILobby.SetUIVisible(value: true);
		}
		NKCUIAttendance.Instance.Open(lstAttendanceKey);
	}

	private bool CheckEventCollection()
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.LOBBY_EVENT, out var _) == NKCContentManager.eContentStatus.Open)
		{
			foreach (NKCLobbyEventIndexTemplet item in NKCLobbyEventIndexTemplet.GetCurrentLobbyEvents().FindAll((NKCLobbyEventIndexTemplet x) => x.bLoginEntry))
			{
				if (!item.CheckLobbyEventSeen())
				{
					item.MarkLobbyEventAsSeen();
					NKCContentManager.MoveToShortCut(item.ShortCutType, item.ShortCutParam);
					return true;
				}
			}
		}
		return false;
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	private void CheckFierceNotice()
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.WORLDMAP, out var bAdmin) != NKCContentManager.eContentStatus.Open || NKCContentManager.CheckContentStatus(ContentsType.FIERCE, out bAdmin) != NKCContentManager.eContentStatus.Open)
		{
			return;
		}
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr == null || nKCFierceBattleSupportDataMgr.GetStatus() != NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE)
		{
			return;
		}
		string key = "FIERCE_KEY_" + NKCScenManager.CurrentUserData().m_UserUID;
		if (!PlayerPrefs.HasKey(key))
		{
			PlayerPrefs.SetString(key, nKCFierceBattleSupportDataMgr.GetFierceBattleID().ToString());
		}
		else
		{
			string text = PlayerPrefs.GetString(key, "");
			string[] array = text.Split(':');
			for (int i = 0; i < array.Length; i++)
			{
				if (string.Equals(array[i], nKCFierceBattleSupportDataMgr.GetFierceBattleID().ToString()))
				{
					return;
				}
			}
			PlayerPrefs.SetString(key, text + ":" + nKCFierceBattleSupportDataMgr.GetFierceBattleID());
		}
		NKCUIPopupFierceBattleNotice.Instance.Open();
	}

	private void CheckGauntletLeagueTopPlayers()
	{
		if (PlayerPrefs.HasKey(NKCPVPManager.GetLeagueTop3Key()))
		{
			long ticks = long.Parse(PlayerPrefs.GetString(NKCPVPManager.GetLeagueTop3Key()));
			if (new DateTime(ticks) < NKMTime.GetResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKMTime.TimePeriod.Week))
			{
				PlayerPrefs.SetString(NKCPVPManager.GetLeagueTop3Key(), NKCSynchronizedTime.GetServerUTCTime().Ticks.ToString());
				NKCPacketSender.Send_NKMPacket_LEAGUE_PVP_WEEKLY_RANKER_REQ();
				m_bWaitGauntletLeagueTopAck = true;
			}
		}
		else
		{
			PlayerPrefs.SetString(NKCPVPManager.GetLeagueTop3Key(), NKCSynchronizedTime.GetServerUTCTime().Ticks.ToString());
			NKCPacketSender.Send_NKMPacket_LEAGUE_PVP_WEEKLY_RANKER_REQ();
			m_bWaitGauntletLeagueTopAck = true;
		}
	}

	public void OnRecv(NKMPacket_GAME_LOAD_ACK cNKMPacket_GAME_LOAD_ACK, int multiply = 1)
	{
		NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(cNKMPacket_GAME_LOAD_ACK.costItemDataList);
		NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(cNKMPacket_GAME_LOAD_ACK.gameData);
		NKCUtil.PlayStartCutscenAndStartGame(cNKMPacket_GAME_LOAD_ACK.gameData);
	}

	public void OnRecv(NKMPacket_PVP_CHARGE_POINT_REFRESH_ACK cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK)
	{
		if (m_UILobby != null)
		{
			m_UILobby.UpdateButton(NKCUILobbyV2.eUIMenu.PVP, NKCScenManager.CurrentUserData());
		}
	}

	public void OnRecv(NKMPacket_WARFARE_EXPIRED_NOT cNKMPacket_WARFARE_EXPIRED_NOT)
	{
		if (m_UILobby != null)
		{
			m_UILobby.UpdateButton(NKCUILobbyV2.eUIMenu.Operation, NKCScenManager.CurrentUserData());
		}
	}

	public void OnRecv(NKMPacket_DIVE_EXPIRE_NOT cNKMPacket_DIVE_EXPIRE_NOT)
	{
		if (m_UILobby != null)
		{
			m_UILobby.UpdateButton(NKCUILobbyV2.eUIMenu.Worldmap, NKCScenManager.CurrentUserData());
		}
	}

	public void OnRecv(NKMPacket_MENTORING_DATA_ACK sPacket)
	{
		NKMMentoringTemplet currentTempet = NKCMentoringUtil.GetCurrentTempet();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (!m_bHaveNewFriendRequest && currentTempet != null && nKMUserData != null)
		{
			MentoringIdentity mentoringIdentity = NKCMentoringUtil.GetMentoringIdentity(nKMUserData);
			if (mentoringIdentity != MentoringIdentity.Mentor && mentoringIdentity == MentoringIdentity.Mentee && (NKCMentoringUtil.IsCanReceiveMenteeMissionReward(nKMUserData) || NKCMentoringUtil.IsDontHaveMentor(nKMUserData)))
			{
				nKMUserData.SetMentoringNotify(bSet: true);
				SetMentoringRewardAlarm();
			}
		}
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_WEEKLY_RANKER_ACK sPacket)
	{
		m_bWaitGauntletLeagueTopAck = false;
	}

	public void OnRecvAutoSupplyMsg()
	{
	}

	public void RefreshBuff()
	{
		if (m_UILobby != null)
		{
			m_UILobby.RefreshUserBuff();
		}
	}

	public void TryPause()
	{
		m_pauseTime = Time.time;
	}

	public void OnReturnApp()
	{
		if (!(Time.time < m_pauseTime + 60f))
		{
			m_pauseTime = 0f;
			PlayVoice(VOICE_TYPE.VT_LOBBY_RETURN);
		}
	}

	public void PlayConnectVoice()
	{
		PlayVoice(VOICE_TYPE.VT_LOBBY_CONNECT);
	}

	private void PlayVoice(VOICE_TYPE type)
	{
		NKMBackgroundUnitInfo backgroundUnitInfo = NKCScenManager.GetScenManager().GetMyUserData().GetBackgroundUnitInfo(0);
		if (backgroundUnitInfo == null)
		{
			return;
		}
		long unitUid = backgroundUnitInfo.unitUid;
		switch (backgroundUnitInfo.unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
		{
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(unitUid);
			if (unitFromUID != null)
			{
				NKCUIVoiceManager.PlayVoice(type, unitFromUID, bIgnoreShowNormalAfterLifeTimeOption: false, bShowCaption: true);
			}
			break;
		}
		case NKM_UNIT_TYPE.NUT_OPERATOR:
		{
			NKMOperator operatorFromUId = NKCScenManager.CurrentUserData().m_ArmyData.GetOperatorFromUId(unitUid);
			if (operatorFromUId != null)
			{
				NKCUIVoiceManager.PlayVoice(type, operatorFromUId, bShowCaption: true);
			}
			break;
		}
		}
	}

	public void RefreshNickname()
	{
		m_UILobby?.RefreshNickname();
	}

	public void DoAfterLogout()
	{
		m_bFirstLobby = true;
	}

	public void UnhideLobbyUI()
	{
		if (m_UILobby != null)
		{
			m_UILobby.TryUIUnhide();
		}
	}

	public void RefreshRechargeEternium()
	{
		m_UILobby?.RefreshRechargeEternium();
	}

	public void OnHomeEnter()
	{
		if (ProcessCoroutine != null)
		{
			NKCScenManager.GetScenManager().StopCoroutine(ProcessCoroutine);
			m_bRunningLobbyProcess = false;
		}
		m_NKCLocalLoginData = NKCLocalLoginData.LoadLastLoginData();
		ProcessCoroutine = NKCScenManager.GetScenManager().StartCoroutine(Process());
	}

	public bool IsRunningLobbyProcess()
	{
		return m_bRunningLobbyProcess;
	}

	private IEnumerator Process()
	{
		if (m_bRunningLobbyProcess)
		{
			yield break;
		}
		Debug.Log("Home Process : Begin");
		m_bRunningLobbyProcess = true;
		while (!NKCUIManager.IsTopmostUI(m_UILobby))
		{
			yield return null;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		Debug.Log("Home Process : Check login cutscene");
		if (NKCLoginCutSceneManager.IsPlaying() || NKCLoginCutSceneManager.CheckLoginCutScene(null))
		{
			Debug.Log("Home Process : Play login cutscene");
			m_bRunningLobbyProcess = false;
			ProcessCoroutine = null;
			yield break;
		}
		Debug.Log("Home Process : Check Tutorial");
		TutorialCheck();
		if (NKCGameEventManager.IsEventPlaying() || NKCTutorialManager.IsCloseDailyContents())
		{
			Debug.Log("Home Process : Waiting Tutorial. Process break.");
			m_bRunningLobbyProcess = false;
			ProcessCoroutine = null;
			yield break;
		}
		m_UILobby.SetEventPanelAutoScroll(value: false);
		if (NKCScenManager.GetScenManager().WarfareGameData != null && NKCScenManager.GetScenManager().WarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			NKMStageTempletV2 stageTemplet = NKMEpisodeMgr.FindStageTempletByBattleStrID("NKM_WARFARE_EP1_4_1");
			if (nKMUserData != null && !NKMTutorialManager.IsTutorialCompleted(TutorialStep.SecondDeckSetup, nKMUserData) && NKMEpisodeMgr.CheckClear(NKCScenManager.CurrentUserData(), stageTemplet))
			{
				m_bWait = true;
				Debug.Log("Home Process : 140 tutorial force fix");
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_SECOND_DECK_TUTORIAL_ERROR_NOTICE"), delegate
				{
					NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(NKMMissionManager.GetMissionTemplet(140));
					WaitFinished();
				});
				while (m_bWait)
				{
					yield return null;
				}
			}
		}
		if (m_bFirstLobby)
		{
			NKMSkinTemplet loginSkinCutin = GetLoginSkinCutin();
			if (loginSkinCutin != null && !string.IsNullOrEmpty(loginSkinCutin.m_LoginCutin))
			{
				if (m_NKCLocalLoginData != null)
				{
					m_NKCLocalLoginData.m_hsPlayedCutin.Add(loginSkinCutin.Key);
				}
				m_bWait = true;
				Debug.Log("Home Process : skin Cutin " + loginSkinCutin.m_LoginCutin + " found. play");
				NKCUIEventSequence.PlaySkinCutin(loginSkinCutin, WaitFinished);
				while (m_bWait)
				{
					yield return null;
				}
			}
		}
		if (NKCPopupFirstRunOptionSetup.IsOptionSetupRequired())
		{
			Debug.Log("Home Process : First play option setup");
			m_bWait = true;
			NKCPopupFirstRunOptionSetup.Instance.Open(WaitFinished);
			while (m_bWait)
			{
				yield return null;
			}
		}
		if (m_bFirstLobby)
		{
			Debug.Log("Home Process : firstlobby actions");
			NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
			if (gameOptionData != null)
			{
				Debug.Log("Home Process : push alarm setup");
				NKCPublisherModule.Push.SetAlarm(NKC_GAME_OPTION_ALARM_GROUP.ALLOW_ALL_ALARM, gameOptionData.GetAllowAlarm(NKC_GAME_OPTION_ALARM_GROUP.ALLOW_ALL_ALARM));
			}
			while (NKCPublisherModule.Busy)
			{
				yield return null;
			}
			Debug.Log("Home Process : purchase restore");
			NKCPublisherModule.InAppPurchase.BillingRestore(NKCShopManager.OnBillingRestore);
			yield return null;
			while (NKCPublisherModule.Busy)
			{
				yield return null;
			}
			Debug.Log("Home Process : check attandence");
			CheckAttandence();
			while (NKCUIAttendance.IsInstanceOpen)
			{
				yield return null;
			}
			if (m_NKCLocalLoginData != null)
			{
				m_NKCLocalLoginData.SaveLastLoginData();
			}
			m_bFirstLobby = false;
			Debug.Log("Home Process : first lobby flag off");
			m_bWait = true;
			Debug.Log("Home Process : OpenPromotionalBanner");
			NKCPublisherModule.Notice.OpenPromotionalBanner(NKCPublisherModule.NKCPMNotice.eOptionalBannerPlaces.EnterLobby, WaitFinished);
			while (m_bWait)
			{
				yield return null;
			}
			Debug.Log("Home Process : check OpenNotice");
			if (NKCPublisherModule.Notice.CheckOpenNoticeWhenFirstLobbyVisit())
			{
				Debug.Log("Home Process : OpenNotice");
				m_bWait = true;
				NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.Lobby_ShowNotice);
				NKCPublisherModule.Notice.OpenNotice(WaitFinished);
				while (m_bWait)
				{
					yield return null;
				}
			}
			Debug.Log("Home Process : check lobby event");
			bool bOpenLobbyEvent = CheckEventCollection();
			if (NKCUIModuleHome.IsAnyInstanceOpen())
			{
				yield return null;
			}
			if (!bOpenLobbyEvent)
			{
				Debug.Log("Home Process : check event");
				if (NKCContentManager.CheckContentStatus(ContentsType.LOBBY_EVENT, out var _) == NKCContentManager.eContentStatus.Open)
				{
					NKMEventTabTemplet requiredEventTemplet = NKCUIEvent.GetRequiredEventTemplet();
					if (requiredEventTemplet != null)
					{
						Debug.Log("Home Process : Event open");
						NKCUIEvent.Instance.Open(requiredEventTemplet);
					}
					while (NKCUIEvent.IsInstanceOpen)
					{
						yield return null;
					}
				}
			}
			if (!bOpenLobbyEvent)
			{
				Debug.Log("Home Process : fierce notice");
				CheckFierceNotice();
				while (NKCUIPopupFierceBattleNotice.IsInstanceOpen)
				{
					yield return null;
				}
			}
			PlayConnectVoice();
		}
		else
		{
			Debug.Log("Home Process : unlockedcontent popup");
			NKCContentManager.SetUnlockedContent();
			NKCContentManager.ShowContentUnlockPopup(null);
		}
		m_UILobby.SetEventPanelAutoScroll(value: true);
		Debug.Log("Home Process : finished");
		ProcessCoroutine = null;
		m_bRunningLobbyProcess = false;
	}

	public void ResetFirstLobby()
	{
		m_bFirstLobby = false;
	}

	private void WaitFinished()
	{
		m_bWait = false;
	}

	private void WaitFinished(NKC_PUBLISHER_RESULT_CODE code, string additionalError)
	{
		if (code == NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_NOT_EXIST_RESTORE_ITEM)
		{
			m_bWait = false;
			NKMPopUpBox.CloseWaitBox();
		}
		else if (NKCPublisherModule.CheckError(code, additionalError, bCloseWaitBox: true, delegate
		{
			m_bWait = false;
		}, popupMessage: true))
		{
			m_bWait = false;
		}
	}

	public NKMSkinTemplet GetLoginSkinCutin()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData.LoginCutin == NKCGameOptionDataSt.GraphicOptionLoginCutin.Off)
		{
			return null;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return null;
		}
		NKMBackgroundInfo backGroundInfo = nKMUserData.backGroundInfo;
		if (backGroundInfo == null)
		{
			return null;
		}
		List<NKMSkinTemplet> list = new List<NKMSkinTemplet>();
		foreach (NKMBackgroundUnitInfo unitInfo in backGroundInfo.unitInfoList)
		{
			if (unitInfo.unitUid == 0L)
			{
				continue;
			}
			NKMUnitData unitOrTrophyFromUID = nKMUserData.m_ArmyData.GetUnitOrTrophyFromUID(unitInfo.unitUid);
			if (unitOrTrophyFromUID != null && unitOrTrophyFromUID.m_SkinID != 0)
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(unitOrTrophyFromUID.m_SkinID);
				if (skinTemplet != null && !string.IsNullOrEmpty(skinTemplet.m_LoginCutin))
				{
					list.Add(skinTemplet);
				}
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		switch (gameOptionData.LoginCutin)
		{
		case NKCGameOptionDataSt.GraphicOptionLoginCutin.Always:
			return list[0];
		case NKCGameOptionDataSt.GraphicOptionLoginCutin.Random:
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}
		case NKCGameOptionDataSt.GraphicOptionLoginCutin.OncePerDay:
			foreach (NKMSkinTemplet item in list)
			{
				if (m_NKCLocalLoginData == null || !m_NKCLocalLoginData.m_hsPlayedCutin.Contains(item.Key))
				{
					return item;
				}
			}
			break;
		}
		return null;
	}

	public void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Lobby);
	}
}
