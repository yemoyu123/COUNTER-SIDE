using Cs.Logging;
using NKC.Publisher;
using NKC.UI;
using UnityEngine;

namespace NKC;

public class NKCUIAgreementNotice : MonoBehaviour
{
	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public NKCUIComToggle m_checkAgreeAll;

	public NKCUIComButton m_openAgreement;

	public NKCUIComButton m_openPrivacyPolicy;

	public GameObject m_toggleOn;

	public GameObject m_toggleOff;

	public NKCUIComButton m_AgeLimitButton;

	public GameObject m_ageLimitPopup;

	public NKCUIComStateButton m_ageLimitPopupClose;

	private static bool m_toggleValue = false;

	private static bool m_currentValue = false;

	private static string LocalSaveKey = "CHECK_SERVICE_AGREEMENT";

	public static string PopupMessage => NKCStringTable.GetString("SI_PF_TOAST_MESSAGE_CHECK_SERVICE_AGREEMENT");

	public static string AgeLimitMessage => NKCStringTable.GetString("SI_PF_POPUP_MESSAGE_AGE_LIMIT_NOTICE");

	public static string ResetPrivacyPolicyMessage => NKCStringTable.GetString("SI_PF_POPUP_MESSAGE_RESET_PRIVACY_POLICY");

	public static string ResetPrivacyPolicySuccess => NKCStringTable.GetString("SI_PF_POPUP_MESSAGE_RESET_PRIVACY_POLICY_SUCCESS");

	private void Start()
	{
		Log.Debug("[Agreement] InitUI", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Components/NKCUIAgreementNotice.cs", 37);
		InitUI();
	}

	public void InitUI()
	{
		if (m_checkAgreeAll != null)
		{
			m_checkAgreeAll.OnValueChanged.RemoveAllListeners();
			m_checkAgreeAll.OnValueChanged.AddListener(OnToggleValueChanged);
		}
		if (m_openAgreement != null)
		{
			m_openAgreement.PointerClick.RemoveAllListeners();
			m_openAgreement.PointerClick.AddListener(OnClickAgreement);
		}
		if (m_openPrivacyPolicy != null)
		{
			m_openPrivacyPolicy.PointerClick.RemoveAllListeners();
			m_openPrivacyPolicy.PointerClick.AddListener(OnClickPrivacy);
		}
		if (m_AgeLimitButton != null)
		{
			m_AgeLimitButton.PointerClick.RemoveAllListeners();
			m_AgeLimitButton.PointerClick.AddListener(OnClickAgeLimit);
		}
		if (m_ageLimitPopupClose != null)
		{
			m_ageLimitPopupClose.PointerClick.RemoveAllListeners();
			m_ageLimitPopupClose.PointerClick.AddListener(OnClickAgeLimitClose);
		}
		OnToggleValueChanged(IsAgreementChecked());
	}

	public static bool IsAgreementChecked()
	{
		if (PlayerPrefs.HasKey(LocalSaveKey) && PlayerPrefs.GetInt(LocalSaveKey) == 1)
		{
			m_currentValue = true;
		}
		else
		{
			m_currentValue = false;
		}
		return m_currentValue;
	}

	public void OnToggleValueChanged(bool value)
	{
		Log.Debug($"[Agreement] Toggle[{value}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Components/NKCUIAgreementNotice.cs", 93);
		NKCUtil.SetGameobjectActive(m_toggleOn, value);
		NKCUtil.SetGameobjectActive(m_toggleOff, !value);
		SetAgreementLocalValue(value);
		m_toggleValue = value;
		m_checkAgreeAll.Select(value, bForce: true, bImmediate: true);
	}

	public static void SetAgreementLocalValue(bool value)
	{
		m_currentValue = value;
		if (value)
		{
			PlayerPrefs.SetInt(LocalSaveKey, 1);
		}
		else
		{
			PlayerPrefs.DeleteKey(LocalSaveKey);
		}
	}

	public static void OnClickAgreement()
	{
		NKCPublisherModule.Notice.OpenAgreement(OnCompleteAgreement);
	}

	public static void OnClickPrivacy()
	{
		NKCPublisherModule.Notice.OpenPrivacyPolicy(OnCompletePrivacy);
	}

	public void OnClickAgeLimit()
	{
		NKCUtil.SetGameobjectActive(m_ageLimitPopup, bValue: true);
	}

	public void OnClickAgeLimitClose()
	{
		NKCUtil.SetGameobjectActive(m_ageLimitPopup, bValue: false);
	}

	private void Update()
	{
		if (m_toggleValue != m_currentValue)
		{
			OnToggleValueChanged(m_currentValue);
		}
	}

	private static void OnCompleteAgreement(NKC_PUBLISHER_RESULT_CODE eNKC_PUBLISHER_RESULT_CODE, string additionalError)
	{
		NKCPublisherModule.CheckError(eNKC_PUBLISHER_RESULT_CODE, additionalError, bCloseWaitBox: false);
	}

	private static void OnCompletePrivacy(NKC_PUBLISHER_RESULT_CODE eNKC_PUBLISHER_RESULT_CODE, string additionalError)
	{
		NKCPublisherModule.CheckError(eNKC_PUBLISHER_RESULT_CODE, additionalError, bCloseWaitBox: false);
	}

	public static void OnResetAgreement(bool bApplicationQuit)
	{
		Log.Debug("[ResetAgreement] OnResetAgreement", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Components/NKCUIAgreementNotice.cs", 157);
		SetAgreementLocalValue(value: false);
		if (bApplicationQuit)
		{
			NKCPublisherModule.Statistics.OnResetAgreement();
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, ResetPrivacyPolicySuccess, delegate
			{
				Application.Quit();
			});
		}
	}
}
