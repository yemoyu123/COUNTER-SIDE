using System;
using Cs.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupAccountCodeOutput : NKCUIBase
{
	private const string DEBUG_HEADER = "[SteamLink][CodeOutput]";

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_ACCOUNT_LINK";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_ACCOUNT_CODE_OUTPUT";

	private static NKCPopupAccountCodeOutput m_Instance;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public Text m_privateLinkCodeText;

	public Text m_timerText;

	public NKCUIComStateButton m_ok;

	public NKCUIComStateButton m_cancel;

	private float m_prevTimeTextUpdate;

	private float m_remainingTime;

	public static NKCPopupAccountCodeOutput Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupAccountCodeOutput>("AB_UI_NKM_UI_ACCOUNT_LINK", "NKM_UI_POPUP_ACCOUNT_CODE_OUTPUT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupAccountCodeOutput>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "AccountLink";

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

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		OnClickClose();
	}

	public void Open(string privateLinkCode, float remainingTime)
	{
		Log.Debug("[SteamLink][CodeOutput] Open", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountCodeOutput.cs", 69);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_cancel?.PointerClick.RemoveAllListeners();
		m_cancel?.PointerClick.AddListener(OnClickClose);
		NKCUtil.SetGameobjectActive(m_ok, bValue: false);
		if (m_privateLinkCodeText != null)
		{
			m_privateLinkCodeText.text = privateLinkCode;
		}
		m_remainingTime = remainingTime;
		m_prevTimeTextUpdate = m_remainingTime + 1f;
		UpdateTimerText();
		UIOpened();
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
			m_remainingTime -= Time.deltaTime;
			if (m_remainingTime < 0f)
			{
				m_remainingTime = 0f;
				m_ok.Lock();
			}
			if ((int)m_remainingTime != (int)m_prevTimeTextUpdate)
			{
				m_prevTimeTextUpdate = m_remainingTime;
				UpdateTimerText();
			}
		}
	}

	public void OnClickClose()
	{
		Log.Debug("[SteamLink][CodeOutput] OnClickClose", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountCodeOutput.cs", 113);
		NKCAccountLinkMgr.CheckForCancelProcess();
	}

	public override void CloseInternal()
	{
		Log.Debug("[SteamLink][CodeOutput] CloseInternal", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Steam/NKCPopupAccountCodeOutput.cs", 119);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void UpdateTimerText()
	{
		string timeSpanStringMS = NKCUtilString.GetTimeSpanStringMS(TimeSpan.FromSeconds(m_remainingTime));
		if (m_timerText != null)
		{
			m_timerText.text = timeSpanStringMS;
		}
	}
}
