using System.Collections;
using Cs.Engine.Util;
using NKC;
using NKC.Publisher;
using NKC.UI;
using NKM;
using UnityEngine.UI;

public class NKCUIAccountWithdrawCheckPopup : NKCUIBase
{
	public Text m_SignOutText;

	public Text m_Desc;

	public InputField m_InputField;

	public NKCUIComStateButton m_OK_BUTTON;

	public NKCUIComStateButton m_CANCLE_BUTTON;

	private static NKCUIAccountWithdrawCheckPopup m_Instance;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName { get; }

	public static NKCUIAccountWithdrawCheckPopup Instance
	{
		get
		{
			if (m_Instance == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCUIAccountWithdrawCheckPopup>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_SIGN_OUT", NKCUIManager.eUIBaseRect.UIOverlay, CleanupInstance);
				if (loadedUIData != null)
				{
					m_Instance = loadedUIData.GetInstance<NKCUIAccountWithdrawCheckPopup>();
				}
			}
			return m_Instance;
		}
	}

	public override void CloseInternal()
	{
		DeActive();
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void OnClickCheckButton(bool bInitAccount = false)
	{
		if (NKCDefineManager.DEFINE_NXTOY() || NKCDefineManager.DEFINE_SB_GB())
		{
			if (string.IsNullOrEmpty(m_InputField.text) || !string.Equals(m_SignOutText.text.ToUpper(), m_InputField.text.ToUpper()))
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_OPTION_SIGN_OUT_MESSAGE_MISS_MATCHED);
				return;
			}
		}
		else if (string.IsNullOrEmpty(m_InputField.text) || !string.Equals(m_SignOutText.text.ToUpper(), m_InputField.text.ToUpper()))
		{
			return;
		}
		if (bInitAccount)
		{
			NKCPacketSender.Send_NKMPacket_ACCOUNT_UNLINK_REQ();
		}
		else if (NKCDefineManager.DEFINE_SB_GB())
		{
			StartCoroutine(WithdrawProcess());
		}
		else
		{
			ResetConnection();
		}
	}

	private void OnCompleteWithdraw(NKC_PUBLISHER_RESULT_CODE result, string additionalError = null)
	{
		ResetConnection();
	}

	private void ResetConnection()
	{
		NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
		NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
	}

	public void OpenUI(bool bInitAccount = false)
	{
		if (m_InputField != null)
		{
			m_InputField.text = "";
		}
		if (NKCDefineManager.DEFINE_SELECT_SERVER())
		{
			if (bInitAccount)
			{
				NKCUtil.SetLabelText(m_SignOutText, NKCStringTable.GetString("SI_PF_POPUP_SERVER_INITIALIZATION_TEXT"));
				NKCUtil.SetLabelText(m_Desc, NKCStringTable.GetString("SI_PF_POPUP_SERVER_INITIALIZATION_DESC"));
			}
			else
			{
				NKCUtil.SetLabelText(m_SignOutText, NKCStringTable.GetString("SI_PF_POPUP_DELETE_ACCOUNT_TEXT"));
				NKCUtil.SetLabelText(m_Desc, NKCStringTable.GetString("SI_PF_POPUP_DELETE_ACCOUNT_DESC"));
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_SignOutText, NKCStringTable.GetString("SI_PF_POPUP_SIGN_OUT_TEXT"));
			NKCUtil.SetLabelText(m_Desc, NKCStringTable.GetString("SI_PF_POPUP_SIGN_OUT_INFO_TEXT"));
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_OK_BUTTON.PointerClick.RemoveAllListeners();
		m_OK_BUTTON.PointerClick.AddListener(delegate
		{
			OnClickCheckButton(bInitAccount);
		});
		m_CANCLE_BUTTON.PointerClick.RemoveAllListeners();
		m_CANCLE_BUTTON.PointerClick.AddListener(CloseUI);
		UIOpened();
	}

	public void CloseUI()
	{
		DeActive();
		Close();
	}

	private void DeActive()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private IEnumerator WithdrawProcess()
	{
		NKMPopUpBox.OpenWaitBox(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		bool bSuccess = true;
		foreach (NKCConnectionInfo.LoginServerInfo loginServerInfo in NKCConnectionInfo.LoginServerInfos)
		{
			using WithdrawPacketController controller = new WithdrawPacketController();
			yield return controller.WithdrawPacketProcess(loginServerInfo.m_serviceIP, loginServerInfo.m_servicePort);
			if (controller.Ack == null)
			{
				bSuccess = false;
				break;
			}
		}
		if (bSuccess)
		{
			if (NKCDefineManager.DEFINE_SB_GB())
			{
				if (NKCPublisherModule.Auth.IsGuest())
				{
					NKCPublisherModule.Auth.Withdraw(delegate
					{
						NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
						NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
					});
				}
				else
				{
					NKCPublisherModule.Auth.TemporaryWithdrawal(delegate
					{
						NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
						NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
						NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
					});
				}
			}
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_FAIL_QUIT, null);
		}
		NKMPopUpBox.CloseWaitBox();
		CloseUI();
	}
}
