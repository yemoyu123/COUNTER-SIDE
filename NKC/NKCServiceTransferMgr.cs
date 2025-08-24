using ClientPacket.Account;
using Cs.Logging;
using NKC.PacketHandler;
using NKC.Publisher;
using NKC.UI;
using NKM;

namespace NKC;

public static class NKCServiceTransferMgr
{
	private const string TERMS_URL_JPN = "https://m.nexon.com/terms/12";

	private const string TERMS_URL_ZLONG = "https://www.zlongame.com/sea/agreement.html";

	private const string TERMS_URL_SB = "https://www.counterside.com/terms/item/ct/en/chk/terms";

	private static bool NeedToShowTerms()
	{
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.JPN))
		{
			return true;
		}
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.SEA))
		{
			return true;
		}
		return false;
	}

	public static void StartServiceTransferRegistProcess()
	{
		if (NKCPublisherModule.Auth.IsGuest())
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_SERVICE_TRANSFER_GUEST_ACCOUNT);
			return;
		}
		NKCPopupOKCancel.ClosePopupBox();
		if (NeedToShowTerms())
		{
			OpenServiceTransferTerms();
		}
		else
		{
			OpenServiceTransferSteps();
		}
	}

	public static void CancelServiceTransferRegistProcess()
	{
		NKMPopUpBox.CloseWaitBox();
		NKCPopupOKCancel.ClosePopupBox();
		CloseAllServiceTransferRegist();
	}

	public static void Send_NKMPacket_SERVICE_TRANSFER_REGIST_CODE_REQ()
	{
		Log.Debug("[ServiceTransferRegist] Send_NKMPacket_SERVICE_TRANSFER_REGIST_CODE_REQ]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCServiceTransferMgr.cs", 63);
		NKMPacket_SERVICE_TRANSFER_REGIST_CODE_REQ packet = new NKMPacket_SERVICE_TRANSFER_REGIST_CODE_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_SERVICE_TRANSFER_REGIST_CODE_ACK sPacket)
	{
		Log.Debug("[ServiceTransferRegist] OnRecv NKMPacket_SERVICE_TRANSFER_REGIST_CODE_ACK", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCServiceTransferMgr.cs", 71);
		NKMPopUpBox.CloseWaitBox();
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			OpenServiceTransferOutput(sPacket.code, sPacket.canReceiveReward);
		}
	}

	public static void Send_NKMPacket_SERVICE_TRANSFER_CODE_COPY_REWARD_REQ()
	{
		Log.Debug("[ServiceTransferRegist]  Send_NKMPacket_SERVICE_TRANSFER_CODE_COPY_REWARD_REQ]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCServiceTransferMgr.cs", 82);
		NKMPacket_SERVICE_TRANSFER_CODE_COPY_REWARD_REQ packet = new NKMPacket_SERVICE_TRANSFER_CODE_COPY_REWARD_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_SERVICE_TRANSFER_CODE_COPY_REWARD_ACK sPacket)
	{
		Log.Debug("[ServiceTransferRegist] OnRecv NKMPacket_SERVICE_TRANSFER_CODE_COPY_REWARD_ACK", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCServiceTransferMgr.cs", 90);
		NKMPopUpBox.CloseWaitBox();
		NKCPopupServiceTransferOutput.Instance.SetReward(bEnable: false);
		NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode);
	}

	public static void StartServiceTransferProcess()
	{
		if (NKCPublisherModule.Auth.IsGuest())
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_SERVICE_TRANSFER_GUEST_ACCOUNT);
		}
		else
		{
			Send_NKMPacket_SERVICE_TRANSFER_USER_VALIDATION_REQ();
		}
	}

	public static void CancelServiceTransferProcess()
	{
		NKMPopUpBox.CloseWaitBox();
		NKCPopupOKCancel.ClosePopupBox();
		CloseAllServiceTransfer();
	}

	public static void Send_NKMPacket_SERVICE_TRANSFER_USER_VALIDATION_REQ()
	{
		Log.Debug("[ServiceTransfer] Send_NKMPacket_SERVICE_TRANSFER_USER_VALIDATION_REQ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCServiceTransferMgr.cs", 125);
		NKMPacket_SERVICE_TRANSFER_USER_VALIDATION_REQ packet = new NKMPacket_SERVICE_TRANSFER_USER_VALIDATION_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_SERVICE_TRANSFER_USER_VALIDATION_ACK sPacket)
	{
		Log.Debug("[ServiceTransfer] OnRecv NKMPacket_SERVICE_TRANSFER_USER_VALIDATION_ACK", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCServiceTransferMgr.cs", 133);
		NKMPopUpBox.CloseWaitBox();
		if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_SERVICE_TRANSFER_USER_ALREADY_TRANSFERRED)
		{
			NKCPopupOKCancel.OpenOKBox(NKCStringTable.GetString("SI_PF_SERVICE_TRANSFER_COMPLETE_NOTICE_TITLE"), NKCStringTable.GetString("SI_PF_SERVICE_TRANSFER_SUCCESS"));
		}
		else if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			OpenServiceTransferInput();
		}
	}

	public static void Send_NKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_REQ(string code)
	{
		Log.Debug("[ServiceTransfer] Send_NKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_REQ[" + code + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCServiceTransferMgr.cs", 151);
		NKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_REQ nKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_REQ = new NKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_REQ();
		nKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_REQ.code = code;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_ACK sPacket)
	{
		Log.Debug("[ServiceTransfer] OnRecv NKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_ACK", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCServiceTransferMgr.cs", 160);
		NKMPopUpBox.CloseWaitBox();
		switch (sPacket.errorCode)
		{
		case NKM_ERROR_CODE.NEC_FAIL_SERVICE_TRANSFER_NOT_REGIST_CODE:
			NKCPopupOKCancel.OpenOKBox(NKCStringTable.GetString("SI_PF_SERVICE_TRANSFER_NOTICE_TITLE"), NKCStringTable.GetString(sPacket.errorCode.ToString(), sPacket.failCount));
			return;
		case NKM_ERROR_CODE.NEC_FAIL_SERVICE_TRANSFER_REGIST_CODE_BLOCKED:
		case NKM_ERROR_CODE.NEC_FAIL_SERVICE_TRANSFER_USED_REGIST_CODE:
			NKCPopupOKCancel.OpenOKBox(NKCStringTable.GetString("SI_PF_SERVICE_TRANSFER_NOTICE_TITLE"), NKCStringTable.GetString(sPacket.errorCode.ToString()));
			return;
		}
		if (NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			NKCPopupServiceTransferInput.CheckInstanceAndClose();
			OpenServiceTransferUserData(sPacket.userProfile);
		}
	}

	public static void Send_NKMPacket_SERVICE_TRANSFER_CONFIRM_REQ()
	{
		Log.Debug("[ServiceTransfer]Send_NKMPacket_SERVICE_TRANSFER_CONFIRM_REQ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCServiceTransferMgr.cs", 192);
		NKMPacket_SERVICE_TRANSFER_CONFIRM_REQ packet = new NKMPacket_SERVICE_TRANSFER_CONFIRM_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public static void OnRecv(NKMPacket_SERVICE_TRANSFER_CONFIRM_ACK sPacket)
	{
		Log.Debug("[ServiceTransfer] OnRecv NKMPacket_SERVICE_TRANSFER_CONFIRM_ACK", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCServiceTransferMgr.cs", 200);
		CloseAllServiceTransfer();
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			if (sPacket.errorCode == NKM_ERROR_CODE.NEC_FAIL_SERVICE_TRANSFER_USED_REGIST_CODE)
			{
				OpenServiceTransferInput();
			}
			return;
		}
		NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
		NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
		NKCPopupOKCancel.OpenOKBox(NKCStringTable.GetString("SI_PF_SERVICE_TRANSFER_COMPLETE_NOTICE_TITLE"), NKCStringTable.GetString("SI_PF_SERVICE_TRANSFER_REGIST_SUCCESS"), delegate
		{
			NKCMain.QuitGame();
		});
	}

	private static void CloseAllServiceTransferRegist()
	{
		NKCPopupServiceTransferTerms.CheckInstanceAndClose();
		NKCPopupServiceTransferSteps.CheckInstanceAndClose();
		NKCPopupServiceTransferOutput.CheckInstanceAndClose();
	}

	private static void CloseAllServiceTransfer()
	{
		NKCPopupServiceTransferInput.CheckInstanceAndClose();
		NKCPopupServiceTransferUserData.CheckInstanceAndClose();
	}

	private static void OpenServiceTransferTerms()
	{
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.JPN))
		{
			NKCPopupServiceTransferTerms.Instance.Open("https://m.nexon.com/terms/12", "https://www.counterside.com/terms/item/ct/en/chk/terms");
		}
		else if (NKMContentsVersionManager.HasCountryTag(CountryTagType.SEA))
		{
			NKCPopupServiceTransferTerms.Instance.Open("https://www.zlongame.com/sea/agreement.html", "https://www.counterside.com/terms/item/ct/en/chk/terms");
		}
		else
		{
			CloseAllServiceTransferRegist();
		}
	}

	public static void OpenServiceTransferSteps()
	{
		NKCPopupServiceTransferSteps.Instance.Open();
	}

	private static void OpenServiceTransferOutput(string code, bool bCanGetReward)
	{
		NKCPopupServiceTransferOutput.Instance.Open(code, bCanGetReward);
	}

	public static void OpenServiceTransferInput()
	{
		NKCPopupServiceTransferInput.Instance.Open();
	}

	public static void OpenServiceTransferUserData(NKMAccountLinkUserProfile userProfile)
	{
		NKCPopupServiceTransferUserData.Instance.Open(userProfile);
	}
}
