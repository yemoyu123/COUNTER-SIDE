using Cs.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupServiceTransferOutput : NKCUIBase
{
	private const string DEBUG_HEADER = "[ServiceTransfer][Output]";

	private const string ASSET_BUNDLE_NAME = "AB_UI_SERVICE_TRANSFER";

	private const string UI_ASSET_NAME = "AB_UI_SERVICE_TRANSFER_CODE_COPY";

	private static NKCPopupServiceTransferOutput m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnCopy;

	public Text m_lbCode;

	public GameObject m_objReward;

	private bool m_bCanGetReward;

	public static NKCPopupServiceTransferOutput Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupServiceTransferOutput>("AB_UI_SERVICE_TRANSFER", "AB_UI_SERVICE_TRANSFER_CODE_COPY", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupServiceTransferOutput>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "CodeOutput";

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
		NKCUtil.SetGameobjectActive(m_objReward, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void SetUI(string code, bool bCanGetReward)
	{
		SetReward(bCanGetReward);
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_btnOk?.PointerClick.RemoveAllListeners();
		m_btnOk?.PointerClick.AddListener(NKCServiceTransferMgr.CancelServiceTransferRegistProcess);
		m_btnCopy?.PointerClick.RemoveAllListeners();
		m_btnCopy?.PointerClick.AddListener(delegate
		{
			OnClickCopy(code);
		});
		NKCUtil.SetLabelText(m_lbCode, code);
	}

	public void Open(string code, bool bCanGetReward)
	{
		Log.Debug("[ServiceTransfer][Output] Open", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCPopupServiceTransferOutput.cs", 79);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetUI(code, bCanGetReward);
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
		Log.Debug("[ServiceTransfer][Output] CloseInternal", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCPopupServiceTransferOutput.cs", 96);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickCopy(string code)
	{
		TextEditor textEditor = new TextEditor();
		textEditor.text = code;
		textEditor.OnFocus();
		textEditor.Copy();
		if (m_bCanGetReward)
		{
			NKCServiceTransferMgr.Send_NKMPacket_SERVICE_TRANSFER_CODE_COPY_REWARD_REQ();
		}
	}

	public void SetReward(bool bEnable)
	{
		m_bCanGetReward = bEnable;
		NKCUtil.SetGameobjectActive(m_objReward, bEnable);
	}
}
