using Cs.Logging;
using NKC.Publisher;
using UnityEngine.Events;

namespace NKC.UI;

public class NKCPopupServiceTransferTerms : NKCUIBase
{
	private const string DEBUG_HEADER = "[ServiceTransfer][Terms]";

	private const string ASSET_BUNDLE_NAME = "AB_UI_SERVICE_TRANSFER";

	private const string UI_ASSET_NAME = "AB_UI_SERVICE_TRANSFER_TERMS_JPN";

	private static NKCPopupServiceTransferTerms m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnCancel;

	public NKCUIComStateButton m_btnDetailLeft;

	public NKCUIComStateButton m_btnDetailRight;

	public static NKCPopupServiceTransferTerms Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupServiceTransferTerms>("AB_UI_SERVICE_TRANSFER", "AB_UI_SERVICE_TRANSFER_TERMS_JPN", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupServiceTransferTerms>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

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

	private void SetUI(string leftTermsDetailUrl, string rightTermsDetailUrl)
	{
		m_NKCUIOpenAnimator.PlayOpenAni();
		NKCUtil.SetButtonClickDelegate(m_btnOk, OnClickTermsOk);
		NKCUtil.SetButtonClickDelegate(m_btnCancel, NKCServiceTransferMgr.CancelServiceTransferRegistProcess);
		NKCUtil.SetButtonClickDelegate(m_btnDetailLeft, (UnityAction)delegate
		{
			NKCPublisherModule.Notice.OpenURL(leftTermsDetailUrl, null);
		});
		NKCUtil.SetButtonClickDelegate(m_btnDetailRight, (UnityAction)delegate
		{
			NKCPublisherModule.Notice.OpenURL(rightTermsDetailUrl, null);
		});
	}

	public void Open(string leftTermsDetailUrl, string rightTermsDetailUrl)
	{
		Log.Debug("[ServiceTransfer][Terms] Open", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCPopupServiceTransferTerms.cs", 70);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetUI(leftTermsDetailUrl, rightTermsDetailUrl);
		UIOpened();
	}

	private void OnClickTermsOk()
	{
		NKCServiceTransferMgr.OpenServiceTransferSteps();
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
		Log.Debug("[ServiceTransfer][Terms] CloseInternal", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/ServiceTransfer/NKCPopupServiceTransferTerms.cs", 92);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
