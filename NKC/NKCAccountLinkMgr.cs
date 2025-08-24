using System;
using ClientPacket.Account;
using ClientPacket.Common;
using Cs.Logging;
using NKC.PacketHandler;
using NKC.Publisher;
using NKC.UI;
using NKC.UI.Option;
using NKM;

namespace NKC;

public static class NKCAccountLinkMgr
{
	private static bool _testMode;

	public static NKMAccountLinkUserProfile m_requestUserProfile;

	public static float m_fCodeInputRemainingTime;

	public static void StartLinkProcess()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCStringTable.GetString("SI_PF_STEAMLINK_NOTICE_TITLE"), NKCStringTable.GetString("SI_PF_STEAMLINK_NOTICE_DESC") + "\n" + NKCStringTable.GetString("SI_PF_STEAMLINK_NOTICE_WARNING"), OpenAccountCodeInput, CancelLinkProcess);
	}

	public static void CheckForCancelProcess()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCStringTable.GetString("SI_PF_STEAMLINK_CODE_ENTER_TITLE"), NKCStringTable.GetString("SI_PF_STEAMLINK_CANCEL_PROCESS"), Send_NKMPacket_ACCOUNT_LINK_CANCEL_REQ);
	}

	public static void CancelLinkProcess()
	{
		NKMPopUpBox.CloseWaitBox();
		NKCPopupOKCancel.ClosePopupBox();
		NKCPopupAccountCodeInput.Instance.Close();
		NKCPopupAccountCodeOutput.Instance.Close();
		NKCPopupAccountSelect.Instance.Close();
		NKCPopupAccountSelectConfirm.Instance.Close();
		m_requestUserProfile = null;
		m_fCodeInputRemainingTime = 0f;
		if (NKCUIGameOption.IsInstanceOpen)
		{
			NKCUIGameOption.Instance.UpdateOptionContent(NKCUIGameOption.GameOptionGroup.Account);
		}
	}

	public static void OpenAccountCodeInput()
	{
		NKCPopupOKCancel.ClosePopupBox();
		NKCPopupAccountCodeInput.Instance.Open(isStartingProcess: true);
	}

	public static void OpenPrivateLinkCodeInput()
	{
		if (m_requestUserProfile != null)
		{
			NKCPopupAccountCodeInput.Instance.Open(isStartingProcess: false);
		}
	}

	public static void Send_NKMPacket_BSIDE_ACCOUNT_LINK_REQ(string friendCodeString)
	{
		long num = Convert.ToInt64(friendCodeString);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null && myUserData.m_FriendCode == num)
		{
			Log.Debug("[AccountLink] Send_NKMPacket_BSIDE_ACCOUNT_LINK_REQ - my friendCode", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 76);
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_STEAM_LINK_INVALID_PUBLISHER_CODE);
			return;
		}
		Log.Debug("[AccountLink] Send_NKMPacket_BSIDE_ACCOUNT_LINK_REQ[" + friendCodeString + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 81);
		NKMPacket_BSIDE_ACCOUNT_LINK_REQ nKMPacket_BSIDE_ACCOUNT_LINK_REQ = new NKMPacket_BSIDE_ACCOUNT_LINK_REQ();
		nKMPacket_BSIDE_ACCOUNT_LINK_REQ.friendCode = num;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_BSIDE_ACCOUNT_LINK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		if (_testMode)
		{
			OnRecv(new NKMPacket_BSIDE_ACCOUNT_LINK_ACK
			{
				errorCode = NKM_ERROR_CODE.NEC_OK
			});
			NKMPacket_BSIDE_ACCOUNT_LINK_NOT nKMPacket_BSIDE_ACCOUNT_LINK_NOT = new NKMPacket_BSIDE_ACCOUNT_LINK_NOT();
			if (friendCodeString.Equals("111"))
			{
				nKMPacket_BSIDE_ACCOUNT_LINK_NOT.linkCode = "123456789";
			}
			else
			{
				nKMPacket_BSIDE_ACCOUNT_LINK_NOT.linkCode = "";
			}
			OnRecv(nKMPacket_BSIDE_ACCOUNT_LINK_NOT);
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_ACK sPacket)
	{
		Log.Debug("[AccountLink] OnRecv NKMPacket_BSIDE_ACCOUNT_LINK_ACK", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 108);
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPopupAccountCodeInput.Instance.Close();
			NKMPopUpBox.OpenWaitBox();
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_NOT sPacket)
	{
		Log.Debug("[AccountLink] OnRecv NKMPacket_BSIDE_ACCOUNT_LINK_NOT", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 120);
		NKMPopUpBox.CloseWaitBox();
		float remainingTime = sPacket.remainingTime;
		if (string.IsNullOrEmpty(sPacket.linkCode))
		{
			m_requestUserProfile = sPacket.requestUserProfile;
			m_fCodeInputRemainingTime = sPacket.remainingTime;
			if (NKCUIGameOption.IsInstanceOpen)
			{
				NKCUIGameOption.Instance.UpdateOptionContent(NKCUIGameOption.GameOptionGroup.Account);
			}
		}
		else
		{
			NKCPopupAccountCodeOutput.Instance.Open(sPacket.linkCode, remainingTime);
		}
	}

	public static void Send_NKMPacket_BSIDE_ACCOUNT_LINK_CODE_REQ(string privateLinkCode)
	{
		Log.Debug("[AccountLink] Send_NKMPacket_BSIDE_ACCOUNT_LINK_CODE_REQ[" + privateLinkCode + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 144);
		NKMPacket_BSIDE_ACCOUNT_LINK_CODE_REQ nKMPacket_BSIDE_ACCOUNT_LINK_CODE_REQ = new NKMPacket_BSIDE_ACCOUNT_LINK_CODE_REQ();
		nKMPacket_BSIDE_ACCOUNT_LINK_CODE_REQ.linkCode = privateLinkCode;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_BSIDE_ACCOUNT_LINK_CODE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		if (_testMode)
		{
			NKMPacket_BSIDE_ACCOUNT_LINK_CODE_ACK nKMPacket_BSIDE_ACCOUNT_LINK_CODE_ACK = new NKMPacket_BSIDE_ACCOUNT_LINK_CODE_ACK();
			if (!privateLinkCode.Equals("123456789"))
			{
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_ACK.errorCode = NKM_ERROR_CODE.NEC_FAIL_STEAM_LINK_INVALID_PRIVATE_CODE;
			}
			else
			{
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_ACK.errorCode = NKM_ERROR_CODE.NEC_OK;
			}
			OnRecv(nKMPacket_BSIDE_ACCOUNT_LINK_CODE_ACK);
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_CODE_ACK sPacket)
	{
		Log.Debug("[AccountLink] OnRecv NKMPacket_BSIDE_ACCOUNT_LINK_CODE_ACK", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 167);
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPopupAccountCodeInput.Instance.Close();
			NKMPopUpBox.OpenWaitBox();
			if (_testMode)
			{
				NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
				NKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT = new NKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT();
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.requestUserProfile = new NKMAccountLinkUserProfile();
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.requestUserProfile.commonProfile = new NKMCommonProfile();
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.requestUserProfile.commonProfile.nickname = "모바일순돌";
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.requestUserProfile.commonProfile.level = 50;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.requestUserProfile.commonProfile.userUid = myUserData.m_UserUID;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.requestUserProfile.creditCount = 100L;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.requestUserProfile.eterniumCount = 9999L;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.requestUserProfile.cashCount = 400L;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.requestUserProfile.medalCount = 30L;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.requestUserProfile.publisherType = NKM_PUBLISHER_TYPE.NPT_STUDIO_BSIDE;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.targetUserProfile = new NKMAccountLinkUserProfile();
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.targetUserProfile.commonProfile = new NKMCommonProfile();
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.targetUserProfile.commonProfile.nickname = "스팀순돌";
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.targetUserProfile.commonProfile.level = 3;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.targetUserProfile.commonProfile.userUid = myUserData.m_UserUID;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.targetUserProfile.creditCount = 5L;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.targetUserProfile.eterniumCount = 10L;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.targetUserProfile.cashCount = 55L;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.targetUserProfile.medalCount = 3L;
				nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT.targetUserProfile.publisherType = NKM_PUBLISHER_TYPE.NPT_STEAM;
				OnRecv(nKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT);
			}
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT sPacket)
	{
		Log.Debug("[AccountLink] OnRecv NKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 214);
		NKMPopUpBox.CloseWaitBox();
		NKCPopupAccountCodeOutput.Instance.Close();
		NKCPopupAccountCodeInput.Instance.Close();
		NKCPopupAccountSelect.Instance.Open(sPacket.requestUserProfile, sPacket.targetUserProfile);
	}

	public static void Send_NKMPacket_ACCOUNT_LINK_SELECT_USERDATA_REQ(long userUID)
	{
		Log.Debug($"[AccountLink] Send_NKMPacket_ACCOUNT_LINK_SELECT_USERDATA_REQ[{userUID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 226);
		NKMPacket_BSIDE_ACCOUNT_LINK_SELECT_USERDATA_REQ nKMPacket_BSIDE_ACCOUNT_LINK_SELECT_USERDATA_REQ = new NKMPacket_BSIDE_ACCOUNT_LINK_SELECT_USERDATA_REQ();
		nKMPacket_BSIDE_ACCOUNT_LINK_SELECT_USERDATA_REQ.selectedUserUid = userUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_BSIDE_ACCOUNT_LINK_SELECT_USERDATA_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		if (_testMode)
		{
			OnRecv(new NKMPacket_BSIDE_ACCOUNT_LINK_SELECT_USERDATA_ACK
			{
				errorCode = NKM_ERROR_CODE.NEC_OK
			});
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_SELECT_USERDATA_ACK sPacket)
	{
		Log.Debug("[AccountLink] OnRecv NKMPacket_ACCOUNT_LINK_SELECT_USERDATA_ACK", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 241);
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPopupAccountSelect.Instance.Close();
			NKCPopupAccountSelectConfirm.Instance.Close();
			if (_testMode)
			{
				NKCPopupOKCancel.OpenOKBox("테스트 성공", "로그아웃을 기다리세요", CancelLinkProcess);
			}
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_SUCCESS_NOT sPacket)
	{
		Log.Debug("[AccountLink] OnRecv NKMPacket_BSIDE_ACCOUNT_LINK_SUCCESS_NOT", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 260);
		NKMPopUpBox.CloseWaitBox();
		NKCPopupAccountSelectConfirm.Instance.Close();
		NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
		NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
		NKCPopupOKCancel.OpenOKBox(NKCStringTable.GetString("SI_PF_STEAMLINK_NOTICE_TITLE"), NKCStringTable.GetString("SI_PF_STEAMLINK_NOTICE_SUCCESS"), delegate
		{
			NKCMain.QuitGame();
		});
	}

	public static void Send_NKMPacket_ACCOUNT_LINK_CANCEL_REQ()
	{
		Log.Debug("[AccountLink] Send_NKMPacket_ACCOUNT_LINK_CANCEL_REQ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 272);
		NKMPacket_BSIDE_ACCOUNT_LINK_CANCEL_REQ packet = new NKMPacket_BSIDE_ACCOUNT_LINK_CANCEL_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		if (_testMode)
		{
			OnRecv(new NKMPacket_BSIDE_ACCOUNT_LINK_CANCEL_NOT());
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_CANCEL_ACK sPacket)
	{
		Log.Debug("[AccountLink] NKMPacket_BSIDE_ACCOUNT_LINK_CANCEL_ACK", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 284);
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_STEAM_LINK_NOT_EXIST_LINK_INFO)
			{
				CancelLinkProcess();
			}
		}
		else
		{
			CancelLinkProcess();
		}
	}

	public static void OnRecv(NKMPacket_BSIDE_ACCOUNT_LINK_CANCEL_NOT sPacket)
	{
		CancelLinkProcess();
	}

	public static void OnClickSuccessConfirm()
	{
	}

	private static void OnLogoutComplete(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (NKCPublisherModule.CheckError(resultCode, additionalError))
		{
			Log.Debug("[AccountLink] OnLogoutComplete", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCAccountLinkMgr.cs", 314);
			NKCPacketHandlersLobby.MoveToLogin();
		}
	}
}
