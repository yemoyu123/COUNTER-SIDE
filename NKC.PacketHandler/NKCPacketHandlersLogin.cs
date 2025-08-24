using System.Collections.Generic;
using ClientPacket.Account;
using Cs.Engine.Util;
using Cs.Logging;
using NKC.Publisher;
using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC.PacketHandler;

public static class NKCPacketHandlersLogin
{
	private static NKM_ERROR_CODE s_LastReconnectFailErrorCode;

	public static void OnRecv(NKMPacket_LOGIN_ACK lPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		OnLogin(lPacket.errorCode, lPacket.accessToken, lPacket.gameServerIP, lPacket.gameServerPort, lPacket.contentsVersion, lPacket.contentsTag, lPacket.openTag);
	}

	public static void OnRecv(NKMPacket_ZLONG_LOGIN_ACK lPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		OnLogin(lPacket.errorCode, lPacket.accessToken, lPacket.gameServerIP, lPacket.gameServerPort, lPacket.contentsVersion, lPacket.contentsTag, lPacket.openTag, lPacket.status);
	}

	public static void OnRecv(NKMPacket_GAMEBASE_LOGIN_ACK lPacket)
	{
		NKMPopUpBox.CloseWaitBox();
		if (lPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_UNDER_MAINTENANCE)
		{
			NKCScenManager.GetScenManager().Get_SCEN_LOGIN()?.LoginBaseMenu.SetLoginTouchDelay();
		}
		OnLogin(lPacket.errorCode, lPacket.accessToken, lPacket.gameServerIP, lPacket.gameServerPort, lPacket.contentsVersion, lPacket.contentsTag, lPacket.openTag, lPacket.resultCode);
	}

	public static void OnRecv(NKMPacket_RECONNECT_ACK ack)
	{
		NKMPopUpBox.CloseWaitBox();
		if (ack.errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			NKCScenManager.GetScenManager().GetConnectGame().SetReconnectKey("");
		}
		if (ack.errorCode == NKM_ERROR_CODE.NEC_FAIL_RECONNECT_PRESENCE_NOT_FOUND && NKCPublisherModule.Auth.IsTryAuthWhenSessionExpired())
		{
			NKCPublisherModule.Auth.Logout(OnLogoutComplete);
		}
		else
		{
			OnReconnect(ack.errorCode, ack.accessToken, ack.gameServerIp, ack.gameServerPort, ack.contentsVersion, ack.contentsTag, ack.openTag);
		}
	}

	private static void OnLogoutByGamebase(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (!string.IsNullOrEmpty(additionalError))
		{
			Debug.Log("### additionalError : " + additionalError);
		}
		if (resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TOY_LOGOUT_SUCCESS, ResetConnection);
		}
		else
		{
			Debug.LogWarning("OnLogoutByGamebase => Logout fail");
		}
		static void ResetConnection()
		{
			NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
			NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
		}
	}

	private static void OnLogoutComplete(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (Process_TryAuthFailedWhenSessionExpired(resultCode, additionalError))
		{
			NKCPublisherModule.Auth.LoginToPublisher(OnSyncAccountComplete);
		}
	}

	private static void OnSyncAccountComplete(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (Process_TryAuthFailedWhenSessionExpired(resultCode, additionalError))
		{
			NKCPublisherModule.Auth.PrepareCSLogin(OnLoginReady);
		}
	}

	private static void OnLoginReady(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (Process_TryAuthFailedWhenSessionExpired(resultCode, additionalError))
		{
			NKCScenManager.GetScenManager().OnLoginReady();
		}
	}

	private static bool Process_TryAuthFailedWhenSessionExpired(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
		{
			return true;
		}
		Debug.LogWarningFormat("ProcessReconnectFail. result:{0}", resultCode);
		NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
		NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_LOGIN)
		{
			NKCPopupOKCancel.OpenOKBox(resultCode, additionalError);
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_DECONNECT_AND_GO_TITLE, delegate
			{
				NKCPublisherModule.Auth.ResetConnection();
				NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
				NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
			});
		}
		return false;
	}

	private static void OnReconnectFailComplete(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_OK)
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_LOGIN)
			{
				NKCPopupOKCancel.OpenOKBox(s_LastReconnectFailErrorCode);
				return;
			}
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_DECONNECT_AND_GO_TITLE, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(resultCode, additionalError);
		}
	}

	private static void OnReconnect(NKM_ERROR_CODE errorCode, string accessToken, string gameServerIP, int gameServerPort, string contentsVersion, IReadOnlyList<string> contentsTagList, IReadOnlyList<string> openTagList)
	{
		if (errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			s_LastReconnectFailErrorCode = errorCode;
			Debug.LogWarningFormat("OnReconnect failed. result:{0}", errorCode);
			NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
			NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
			NKCPublisherModule.Auth.OnReconnectFail(OnReconnectFailComplete);
		}
		else
		{
			OnLoginSuccess(accessToken, gameServerIP, gameServerPort, contentsVersion, contentsTagList, openTagList);
		}
	}

	private static void OnLoginSuccess(string accessToken, string gameServerIP, int gameServerPort, string contentsVersion, IReadOnlyList<string> contentsTagList, IReadOnlyList<string> openTagList)
	{
		bool flag = NKCContentsVersionManager.CheckSameTagList(contentsTagList);
		Log.Info($"OnLoginSucces LocalCV[{NKMContentsVersionManager.CurrentVersion.Literal}] -> LoginCV[{contentsVersion}], CheckSameTag[{flag}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketHandlersLogin.cs", 230);
		if (NKCDefineManager.DEFINE_CHECKVERSION() && NKCMain.m_ranAsSafeMode)
		{
			flag = false;
			Log.Info($"OnLoginSucces RanAsSafeMode VersionAckReceived[{ContentsVersionChecker.VersionAckReceived}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPacketHandlersLogin.cs", 236);
		}
		if (NKMContentsVersionManager.CurrentVersion.Literal != contentsVersion || !flag)
		{
			PlayerPrefs.SetString("LOCAL_SAVE_CONTENTS_VERSION_KEY", contentsVersion);
			NKCContentsVersionManager.SetTagList(contentsTagList);
			NKCContentsVersionManager.SaveTagToLocal();
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_CONTENTS_VERSION_CHANGE, NKCScenManager.GetScenManager().MoveToPatchScene);
			return;
		}
		NKMOpenTagManager.SetTagList(openTagList);
		foreach (NKMPotentialOptionGroupTemplet value in NKMPotentialOptionGroupTemplet.Values)
		{
			value.Validate();
		}
		NKCScenManager.GetScenManager().SetAppEnableConnectCheckTime(-1f, bForce: true);
		NKCScenManager.GetScenManager().GetConnectLogin().LoginComplete();
		NKCMMPManager.OnCustomEvent("06_login_complete");
		Debug.LogFormat("target server " + gameServerIP);
		NKCConnectGame connectGame = NKCScenManager.GetScenManager().GetConnectGame();
		connectGame.SetRemoteAddress(gameServerIP, gameServerPort);
		connectGame.SetAccessToken(accessToken);
		NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
		connectGame.ResetConnection();
		connectGame.ConnectToLobbyServer();
	}

	private static void OnLogin(NKM_ERROR_CODE errorCode, string accessToken, string gameServerIP, int gameServerPort, string contentsVersion, IReadOnlyList<string> contentsTagList, IReadOnlyList<string> openTagList, int status = int.MinValue)
	{
		if (errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			Debug.LogWarningFormat("Login failed. result:{0}", errorCode);
			NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
			NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
			NKCPublisherModule.Auth.ResetConnection();
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_LOGIN)
			{
				OpenLoginErrorPopup(errorCode, status);
				NKCScenManager.GetScenManager().Get_SCEN_LOGIN()?.UpdateLoginMsgUI();
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_DECONNECT_AND_GO_TITLE, delegate
				{
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
				});
			}
		}
		else
		{
			OnLoginSuccess(accessToken, gameServerIP, gameServerPort, contentsVersion, contentsTagList, openTagList);
		}
	}

	public static void OpenLoginErrorPopup(NKM_ERROR_CODE errorCode, int extraStatus)
	{
		if (NKCPublisherModule.PublisherType == NKCPublisherModule.ePublisherType.Zlong)
		{
			if (errorCode == NKM_ERROR_CODE.NEC_FAIL_ZLONG_LOGIN_INVALID_STATUS)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCStringTable.GetString(errorCode.ToString() + "_" + extraStatus));
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(errorCode);
			}
		}
		else if (NKCPublisherModule.PublisherType == NKCPublisherModule.ePublisherType.SB_Gamebase || NKCPublisherModule.PublisherType == NKCPublisherModule.ePublisherType.STEAM)
		{
			NKCPopupOKCancel.OpenOKBox(errorCode, delegate
			{
				NKCPublisherModule.Notice.OpenNotice(null);
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(errorCode);
		}
	}
}
