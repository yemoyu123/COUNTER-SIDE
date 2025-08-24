using Cs.Logging;
using NKM;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupServiceTransferInput : NKCUIBase
{
	private const string DEBUG_HEADER = "[ServiceTransfer][Input]";

	private const string ASSET_BUNDLE_NAME = "AB_UI_SERVICE_TRANSFER";

	private const string UI_ASSET_NAME = "AB_UI_SERVICE_TRANSFER_INPUT";

	private static NKCPopupServiceTransferInput m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_csbtnOk;

	public NKCUIComStateButton m_csbtnCancel;

	public InputField m_inputField;

	public static NKCPopupServiceTransferInput Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupServiceTransferInput>("AB_UI_SERVICE_TRANSFER", "AB_UI_SERVICE_TRANSFER_INPUT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupServiceTransferInput>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "Input";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void SetUI()
	{
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_csbtnOk?.PointerClick.RemoveAllListeners();
		m_csbtnOk?.PointerClick.AddListener(OnClickOk);
		m_csbtnCancel?.PointerClick.RemoveAllListeners();
		m_csbtnCancel?.PointerClick.AddListener(NKCServiceTransferMgr.CancelServiceTransferProcess);
		m_inputField.text = "";
	}

	public void Open()
	{
		Log.Debug("[ServiceTransfer][Input] Open", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCPopupServiceTransferInput.cs", 74);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetUI();
		UIOpened();
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public override void CloseInternal()
	{
		Log.Debug("[ServiceTransfer][Input] CloseInternal", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCPopupServiceTransferInput.cs", 91);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickOk()
	{
		string text = m_inputField.text;
		if (!ValidateCodeFormat(text))
		{
			NKCPopupOKCancel.OpenOKBox(NKCStringTable.GetString("SI_PF_SERVICE_TRANSFER_NOTICE_TITLE"), NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_SERVICE_TRANSFER_WRONG_FORMAT_REGIST_CODE));
		}
		else
		{
			NKCServiceTransferMgr.Send_NKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_REQ(text);
		}
	}

	private bool ValidateCodeFormat(string code)
	{
		if (string.IsNullOrEmpty(code))
		{
			return false;
		}
		if (code.Length != 16)
		{
			return false;
		}
		for (int i = 0; i < code.Length; i++)
		{
			if (!char.IsUpper(code, i))
			{
				return false;
			}
		}
		return true;
	}
}
