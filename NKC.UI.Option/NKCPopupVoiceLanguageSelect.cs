using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCPopupVoiceLanguageSelect : NKCUIBase
{
	[Serializable]
	public class VoiceButton
	{
		public NKC_VOICE_CODE m_eVoiceCode;

		public NKCUIComToggle m_tglButton;
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_login_select";

	private const string UI_ASSET_NAME = "AB_UI_LOGIN_SELECT_VOICE";

	private static NKCPopupVoiceLanguageSelect m_Instance;

	public List<VoiceButton> m_lstButtons;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnClose;

	public Text m_lbDesc;

	private NKC_VOICE_CODE m_eSelectedVoiceCode;

	public static NKCPopupVoiceLanguageSelect Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupVoiceLanguageSelect>("ab_ui_login_select", "AB_UI_LOGIN_SELECT_VOICE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupVoiceLanguageSelect>();
				m_Instance.Init(delegate
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_OPTION_GAME_VOICE_CHANGE_NOTICE"));
				});
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Init(Action onComplete, bool bPatcher = false)
	{
		NKCUtil.SetGameobjectActive(m_csbtnClose, !bPatcher);
		NKCUtil.SetGameobjectActive(m_lbDesc, bPatcher);
		foreach (VoiceButton lstButton in m_lstButtons)
		{
			if (lstButton != null && lstButton.m_tglButton != null)
			{
				lstButton.m_tglButton.OnValueChangedWithData = OnTglButtonSelect;
				lstButton.m_tglButton.m_DataInt = (int)lstButton.m_eVoiceCode;
			}
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, (UnityAction)delegate
		{
			OnOK(onComplete);
		});
	}

	public void Open()
	{
		HashSet<NKC_VOICE_CODE> hashSet = new HashSet<NKC_VOICE_CODE>(NKCUIVoiceManager.GetAvailableVoiceCode());
		foreach (VoiceButton lstButton in m_lstButtons)
		{
			if (lstButton != null && !(lstButton.m_tglButton == null))
			{
				NKCUtil.SetGameobjectActive(lstButton.m_tglButton, hashSet.Contains(lstButton.m_eVoiceCode));
				if (NKCUIVoiceManager.CurrentVoiceCode == lstButton.m_eVoiceCode)
				{
					lstButton.m_tglButton.Select(bSelect: true, bForce: true);
				}
			}
		}
		UIOpened();
	}

	private void OnOK(Action onComplete)
	{
		NKCUIVoiceManager.SetVoiceCode(m_eSelectedVoiceCode);
		Close();
		onComplete?.Invoke();
	}

	private void OnTglButtonSelect(bool value, int data)
	{
		if (value)
		{
			m_eSelectedVoiceCode = (NKC_VOICE_CODE)data;
		}
	}
}
