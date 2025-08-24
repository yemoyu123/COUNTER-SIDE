using Cs.Logging;

namespace NKC.UI;

public class NKCPopupServiceTransferSteps : NKCUIBase
{
	private const string DEBUG_HEADER = "[ServiceTransfer][Terms]";

	private const string ASSET_BUNDLE_NAME = "AB_UI_SERVICE_TRANSFER";

	private const string UI_ASSET_NAME = "AB_UI_SERVICE_TRANSFER_TERMS";

	private static NKCPopupServiceTransferSteps m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnCancel;

	public static NKCPopupServiceTransferSteps Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupServiceTransferSteps>("AB_UI_SERVICE_TRANSFER", "AB_UI_SERVICE_TRANSFER_TERMS", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupServiceTransferSteps>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override string MenuName => "ServiceTransfer";

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
		NKCUtil.SetButtonClickDelegate(m_btnOk, OnClickStepsOk);
		NKCUtil.SetButtonClickDelegate(m_btnCancel, NKCServiceTransferMgr.CancelServiceTransferRegistProcess);
	}

	public void Open()
	{
		Log.Debug("[ServiceTransfer][Terms] Open", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCPopupServiceTransferSteps.cs", 63);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetUI();
		UIOpened();
	}

	private void OnClickStepsOk()
	{
		NKCServiceTransferMgr.Send_NKMPacket_SERVICE_TRANSFER_REGIST_CODE_REQ();
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
		Log.Debug("[ServiceTransfer][Terms] CloseInternal", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCPopupServiceTransferSteps.cs", 85);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
