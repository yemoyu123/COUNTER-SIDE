using System.Collections;
using AssetBundles;
using ClientPacket.Account;
using ClientPacket.Community;
using Cs.Core.Util;
using NKC.Publisher;
using NKC.Templet;
using NKC.UI;
using NKC.UI.Option;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_LOGIN : NKC_SCEN_BASIC
{
	private NKCUIManager.LoadedUIData m_LoginUIData;

	private NKCUILoginBaseMenu m_LoginBaseMenu;

	private NKCUILoginDevMenu m_LoginDevMenu;

	private NKMTrackingFloat m_BloomIntensity = new NKMTrackingFloat();

	private const float CONST_CAMERA_DISTANCE = -1777.7778f;

	private bool m_bFirstSuccessLoginToPublisher = true;

	private bool m_bShutdownPopup;

	private NKM_ERROR_CODE m_errorCodeForNGS;

	private bool m_bDuplicateConnect;

	private const float DEFAULT_DELAY_TITLECALL_END = 0.5f;

	private float m_fBgmDelay;

	private float m_fTimeToOpenNotice;

	public NKCUILoginBaseMenu LoginBaseMenu => m_LoginBaseMenu;

	public NKCUILoginDevMenu LoginDevMenu => m_LoginDevMenu;

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCUIManager.IsValid(m_LoginUIData))
		{
			m_LoginUIData = NKCUILoginBaseMenu.OpenNewInstanceAsync();
		}
	}

	public void SetShutdownPopup()
	{
		m_bShutdownPopup = true;
	}

	public void SetErrorCodeForNGS(NKM_ERROR_CODE eNKM_ERROR_CODE)
	{
		m_errorCodeForNGS = eNKM_ERROR_CODE;
	}

	public void SetDuplicateConnectPopup()
	{
		m_bDuplicateConnect = true;
	}

	public NKC_SCEN_LOGIN()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_LOGIN;
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (LoginBaseMenu == null)
		{
			if (m_LoginUIData != null && m_LoginUIData.CheckLoadAndGetInstance<NKCUILoginBaseMenu>(out m_LoginBaseMenu))
			{
				LoginBaseMenu.InitUI();
				m_LoginDevMenu = LoginBaseMenu.GetLoginDevMenu();
			}
			else
			{
				Debug.LogError("Error - NKC_SCEN_LOGIN.ScenLoadUIComplete() : UI Load Failed!");
			}
		}
	}

	public override void ScenStart()
	{
		NKCUIManager.SetScreenInputBlock(bSet: false);
		if (!NKCDefineManager.DEFINE_ZLONG())
		{
			PlayTitleCall();
		}
		base.ScenStart();
		NKCCamera.EnableBloom(bEnable: true);
		NKCCamera.GetCamera().orthographic = false;
		NKCCamera.GetTrackingPos().SetNowValue(0f, 0f, GetCameraDistance());
		NKCCamera.StopTrackingCamera();
		switch (NKCPublisherModule.InitState)
		{
		case NKCPublisherModule.ePublisherInitState.Initialized:
			NKCPublisherModule.DoAfterLogout();
			TryLoginToPublisher();
			break;
		case NKCPublisherModule.ePublisherInitState.Maintanance:
			NKCPublisherModule.Notice.NotifyMainenance(delegate
			{
				Application.Quit();
			});
			break;
		case NKCPublisherModule.ePublisherInitState.NotInitialized:
			TryInitPublisher();
			break;
		}
		NKCScenManager.GetScenManager().AdMobInitializeProcess();
		LoginBaseMenu.Open();
		NKCScenManager.GetScenManager().DoAfterLogout();
		ProcessPopupOnStartScen();
	}

	private void ProcessPopupOnStartScen()
	{
		if (m_bShutdownPopup)
		{
			m_bShutdownPopup = false;
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SHUTDOWN_ALARM, delegate
			{
				NKCMain.QuitGame();
			});
		}
		else if (m_errorCodeForNGS != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(m_errorCodeForNGS, delegate
			{
				NKCMain.QuitGame();
			});
			m_errorCodeForNGS = NKM_ERROR_CODE.NEC_OK;
		}
		else if (m_bDuplicateConnect)
		{
			m_bDuplicateConnect = false;
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ERROR_MULTIPLE_CONNECT, delegate
			{
				NKCMain.QuitGame();
			});
		}
	}

	private void PlayTitleCall()
	{
		m_fBgmDelay = 0f;
		SoundData playingVoiceData = NKCSoundManager.GetPlayingVoiceData(NKCUIVoiceManager.PlayRandomVoiceInBundle("ab_titlecall_default"));
		if (playingVoiceData != null)
		{
			m_fBgmDelay = playingVoiceData.m_AudioSource.clip.length + 0.5f;
		}
		Debug.Log($"Play Title Call. BgmDelay : {m_fBgmDelay}");
	}

	public override void PlayScenMusic()
	{
		NKCLoginBackgroundTemplet currentBackgroundTemplet = NKCLoginBackgroundTemplet.GetCurrentBackgroundTemplet();
		if (currentBackgroundTemplet != null)
		{
			if (AssetBundleManager.IsBundleExists("ab_music/" + currentBackgroundTemplet.m_MusicName.ToLower()))
			{
				NKCSoundManager.PlayMusic(currentBackgroundTemplet.m_MusicName, bLoop: true, 1f, bForce: true, currentBackgroundTemplet.m_MusicStartTime, m_fBgmDelay);
				return;
			}
			Debug.LogWarning("playing default login music!");
			NKCSoundManager.PlayMusic("cutscene_login", bLoop: true);
		}
		else
		{
			base.PlayScenMusic();
		}
	}

	public float GetCameraDistance()
	{
		return -1777.7778f * ((float)Screen.height / (float)Screen.width);
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		NKCSoundManager.StopAllSound();
		if (LoginBaseMenu != null)
		{
			LoginBaseMenu.Close();
		}
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_LoginUIData?.CloseInstance();
		m_LoginUIData = null;
		m_LoginBaseMenu = null;
		m_LoginDevMenu = null;
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
		m_BloomIntensity.Update(Time.deltaTime);
		if (!m_BloomIntensity.IsTracking())
		{
			m_BloomIntensity.SetTracking(NKMRandom.Range(1f, 2f), 4f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		NKCCamera.SetBloomIntensity(m_BloomIntensity.GetNowValue());
		if (LoginBaseMenu != null)
		{
			LoginBaseMenu.Update();
		}
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void OnLoginSuccess(NKMPacket_JOIN_LOBBY_ACK res)
	{
		if (LoginDevMenu != null)
		{
			LoginDevMenu.SaveIDPass();
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().OnLoginSuccess();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().OnLoginSuccess();
		NKCSynchronizedTime.ForceUpdateTime();
		NKCMailManager.Cleanup();
		NKCMailManager.RefreshMailList();
		NKCShopManager.InvalidateShopItemList();
		NKCEquipPresetDataManager.RequestPresetData(_openUI: false);
		if (NKCUIGameOption.HasInstance)
		{
			NKCUIGameOption.Instance.RemoveCloseCallBack();
		}
		OnLoginSuccessForFriend();
		OnLoginSuccessForMission();
		OnLoginSuccessForAttendance();
		OnLoginSuccessForDive();
		OnLoginSuccessForOptions();
		OnLoginSuccessForChat();
		OnLoginSuccessForOperationFavorite();
		OnLoginSuccessForDefence();
		OnLoginSuccessForTournament();
		NKCPublisherModule.Statistics.OnLoginSuccessToCS(res);
	}

	private void OnLoginSuccessForDive()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && nKMUserData.m_DiveGameData != null)
		{
			nKMUserData.m_DiveGameData.Floor.OnPacketRead();
		}
	}

	private void OnLoginSuccessForMission()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null && myUserData.m_MissionData != null)
		{
			NKMMissionManager.SetHaveClearedMission(myUserData.m_MissionData.CheckCompletableMission(myUserData));
			NKMMissionManager.SetHaveClearedMissionGuide(myUserData.m_MissionData.CheckCompletableGuideMission(myUserData));
		}
	}

	private void OnLoginSuccessForFriend()
	{
		NKCFriendManager.Initialize();
		NKCScenManager.GetScenManager().Get_SCEN_HOME()?.SetFriendNewIcon(bSet: false);
		NKCPacketSender.Send_NKMPacket_FRIEND_LIST_REQ(NKM_FRIEND_LIST_TYPE.RECEIVE_REQUEST);
		NKCPacketSender.Send_NKMPacket_FRIEND_LIST_REQ(NKM_FRIEND_LIST_TYPE.FRIEND);
		NKCPacketSender.Send_NKMPacket_FRIEND_LIST_REQ(NKM_FRIEND_LIST_TYPE.BLOCKER);
		NKMPacket_GREETING_MESSAGE_REQ packet = new NKMPacket_GREETING_MESSAGE_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void OnLoginSuccessForAttendance()
	{
		NKC_SCEN_HOME sCEN_HOME = NKCScenManager.GetScenManager().Get_SCEN_HOME();
		if (sCEN_HOME != null)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (myUserData != null && myUserData.m_MissionData != null)
			{
				sCEN_HOME.SetAttendanceRequired(NKMAttendanceManager.CheckNeedAttendance(myUserData.m_AttendanceData, NKCSynchronizedTime.GetServerUTCTime()));
			}
		}
	}

	private void OnLoginSuccessForOptions()
	{
		NKCPublisherModule.Push.UpdateAllLocalPush();
	}

	private void OnLoginSuccessForChat()
	{
		if (NKCGuildManager.MyData != null && NKCGuildManager.MyData.guildUid > 0)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_CHAT_LIST_REQ(NKCGuildManager.MyData.guildUid);
		}
	}

	private void OnLoginSuccessForOperationFavorite()
	{
		NKCPacketSender.Send_NKMPacket_FAVORITES_STAGE_REQ();
	}

	private void OnLoginSuccessForDefence()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVE_DEFENCE))
		{
			NKMDefenceTemplet currentDefenceDungeonTemplet = NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Now);
			if (currentDefenceDungeonTemplet != null)
			{
				NKCPacketSender.Send_NKMPacket_DEFENCE_INFO_REQ(currentDefenceDungeonTemplet.Key);
			}
		}
	}

	private void OnLoginSuccessForTournament()
	{
		if (NKMTournamentTemplet.Find(ServiceTime.Now) != null)
		{
			NKCPacketSender.Send_NKMPacket_TOURNAMENT_INFO_REQ();
		}
	}

	public void UpdateLoginMsgUI()
	{
		if (LoginBaseMenu != null)
		{
			LoginBaseMenu.UpdateLoginMsgUI();
		}
	}

	public void TryLogin()
	{
		if (NKCPublisherModule.Auth.LoginToPublisherCompleted)
		{
			Debug.Log($"TryLogin : {LoginBaseMenu.GetCurrentServerAddress()}:{LoginBaseMenu.GetCurrentServerPort()}");
			NKCConnectLogin connectLogin = NKCScenManager.GetScenManager().GetConnectLogin();
			connectLogin.SetRemoteAddress(LoginBaseMenu.GetCurrentServerAddress(), LoginBaseMenu.GetCurrentServerPort());
			connectLogin.ResetConnection();
			NKMPopUpBox.OpenWaitBox();
			NKCScenManager.GetScenManager().VersionCheck(delegate
			{
				NKCPublisherModule.Auth.PrepareCSLogin(OnLoginReady);
			});
		}
		else
		{
			TryLoginToPublisher();
		}
	}

	private void OnLoginReady(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError))
		{
			NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.TryLoginToGameServer);
			NKCScenManager.GetScenManager().OnLoginReady();
		}
	}

	public NKM_USER_AUTH_LEVEL GetAuthLevel()
	{
		if (LoginDevMenu != null)
		{
			return LoginDevMenu.AuthLevel;
		}
		return NKM_USER_AUTH_LEVEL.NORMAL_USER;
	}

	private void TryInitPublisher()
	{
		NKMPopUpBox.OpenWaitBox();
		NKCPublisherModule.InitInstance(OnInitComplete);
	}

	private void OnInitComplete(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		NKMPopUpBox.CloseWaitBox();
		switch (resultCode)
		{
		case NKC_PUBLISHER_RESULT_CODE.NPRC_OK:
			TryLoginToPublisher();
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_MAINTENANCE:
			NKCPublisherModule.Notice.NotifyMainenance(delegate
			{
				Application.Quit();
			});
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_STEAM_INITIALIZE_FAIL:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_STEAM_INITIALIZE), delegate
			{
				NKCMain.QuitGame();
			});
			break;
		default:
			NKCPopupOKCancel.OpenOKBox(resultCode, additionalError, TryInitPublisher, NKCStringTable.GetString("SI_DP_TOY_RE_TRY_TITLE"));
			break;
		}
	}

	private void TryLoginToPublisher()
	{
		Debug.Log("TryLoginToPublisher");
		NKMPopUpBox.OpenWaitBox();
		NKCPublisherModule.Auth.LoginToPublisher(OnLoginToPublisherComplete);
	}

	private IEnumerator DelayedOpenNotice()
	{
		NKMPopUpBox.OpenWaitBox();
		m_fTimeToOpenNotice = Time.time + 0.5f;
		while (m_fTimeToOpenNotice > Time.time)
		{
			yield return null;
		}
		NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.Login_ShowNotice);
		NKCPublisherModule.Notice.OpenNotice(delegate
		{
			NKMPopUpBox.CloseWaitBox();
			NKCPublisherModule.Auth.OpenCertification();
		});
	}

	private void OnLoginToPublisherComplete(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		Debug.Log("OnLoginToPublisherComplete");
		NKMPopUpBox.CloseWaitBox();
		switch (resultCode)
		{
		case NKC_PUBLISHER_RESULT_CODE.NPRC_OK:
			NKCPublisherModule.Permission.RequestAppTrackingPermission();
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_QUIT_USER:
			NKMPopUpBox.OpenWaitBox();
			NKCPublisherModule.Auth.TryRestoreQuitUser(OnQuitUserRestore);
			return;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_USER_RESOLVE_REQUIRED:
			NKMPopUpBox.OpenWaitBox();
			NKCPublisherModule.Auth.TryResolveUser(OnSyncResolve);
			return;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_MAINTENANCE:
			NKCPublisherModule.Notice.NotifyMainenance(delegate
			{
				Application.Quit();
			});
			return;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_TIMEOUT:
			Debug.Log("Login TimeOut. Can be normal state : skip error popup");
			return;
		default:
			NKCPopupOKCancel.OpenOKBox(resultCode, additionalError);
			break;
		}
		if (LoginBaseMenu != null)
		{
			LoginBaseMenu.AfterLoginToPublisherCompleted(resultCode, additionalError);
		}
		if (m_bFirstSuccessLoginToPublisher)
		{
			m_bFirstSuccessLoginToPublisher = false;
			if (NKCPublisherModule.Notice.CheckOpenNoticeWhenFirstLoginSuccess() && LoginBaseMenu != null)
			{
				NKCUtil.SetGameobjectActive(LoginBaseMenu, bValue: true);
				LoginBaseMenu.StartCoroutine(DelayedOpenNotice());
			}
		}
	}

	private void OnSyncResolve(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError, bCloseWaitBox: true, ReturnLogin))
		{
			TryLoginToPublisher();
		}
	}

	private void OnQuitUserRestore(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError, bCloseWaitBox: true, ReturnLogin))
		{
			TryLoginToPublisher();
		}
	}

	private void ReturnLogin()
	{
		NKMPopUpBox.CloseWaitBox();
		NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
		NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
	}
}
