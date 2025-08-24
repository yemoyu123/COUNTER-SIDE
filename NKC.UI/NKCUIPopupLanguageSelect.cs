using System.Collections.Generic;
using System.Linq;
using NKC.Publisher;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupLanguageSelect : NKCUIBase
{
	public delegate void OnClose(NKM_NATIONAL_CODE selectedLanguage);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_LANGUAGE";

	private static NKCUIPopupLanguageSelect m_Instance;

	private Dictionary<NKM_NATIONAL_CODE, NKCUIComToggle> m_dicLanguageButton = new Dictionary<NKM_NATIONAL_CODE, NKCUIComToggle>();

	public NKCUIComButton m_closeButton;

	public Image m_imageTitle;

	public NKCUIComToggle m_tglButtonOrg;

	public NKCUIComToggleGroup m_tgrpLanguageButton;

	public NKCUIComStateButton m_csbtnOK;

	public Image m_imageOkBtn;

	private OnClose dOnClose;

	private NKM_NATIONAL_CODE m_eSelectedLanguage;

	public static NKCUIPopupLanguageSelect Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupLanguageSelect>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_LANGUAGE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupLanguageSelect>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public static bool HasInstance => m_Instance != null;

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

	public override string MenuName => "Language Select";

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

	public void Init()
	{
		if (m_csbtnOK != null)
		{
			m_csbtnOK.PointerClick.RemoveAllListeners();
			m_csbtnOK.PointerClick.AddListener(base.Close);
		}
		if (m_closeButton != null)
		{
			m_closeButton.PointerClick.RemoveAllListeners();
			m_closeButton.PointerClick.AddListener(CloseWithoutCallback);
		}
	}

	private void MakeButtons(HashSet<NKM_NATIONAL_CODE> setLanguages)
	{
		foreach (NKM_NATIONAL_CODE setLanguage in setLanguages)
		{
			if (!m_dicLanguageButton.ContainsKey(setLanguage))
			{
				NKCUIComToggle nKCUIComToggle = Object.Instantiate(m_tglButtonOrg, m_tgrpLanguageButton.transform);
				nKCUIComToggle.SetToggleGroup(m_tgrpLanguageButton);
				nKCUIComToggle.m_DataInt = (int)setLanguage;
				nKCUIComToggle.OnValueChangedWithData = OnTglLanguage;
				if (nKCUIComToggle.m_ButtonBG_Normal != null)
				{
					NKCUtil.SetImageSprite(nKCUIComToggle.m_ButtonBG_Normal.GetComponent<Image>(), Resources.Load<Sprite>("LANGUAGE/Toggle_Off" + NKCStringTable.GetNationalPostfix(setLanguage)));
				}
				if (nKCUIComToggle.m_ButtonBG_Selected != null)
				{
					NKCUtil.SetImageSprite(nKCUIComToggle.m_ButtonBG_Selected.GetComponent<Image>(), Resources.Load<Sprite>("LANGUAGE/Toggle_On" + NKCStringTable.GetNationalPostfix(setLanguage)));
				}
				m_dicLanguageButton.Add(setLanguage, nKCUIComToggle);
			}
		}
		foreach (KeyValuePair<NKM_NATIONAL_CODE, NKCUIComToggle> item in m_dicLanguageButton)
		{
			NKCUtil.SetGameobjectActive(item.Value, setLanguages.Contains(item.Key));
		}
	}

	public void Open(HashSet<NKM_NATIONAL_CODE> setLanguages, OnClose onClose)
	{
		dOnClose = onClose;
		m_eSelectedLanguage = NKCStringTable.GetNationalCode();
		if (!setLanguages.Contains(m_eSelectedLanguage))
		{
			if (setLanguages.Count > 0)
			{
				m_eSelectedLanguage = setLanguages.First();
			}
			else
			{
				m_eSelectedLanguage = NKCPublisherModule.Localization.GetDefaultLanguage();
			}
		}
		MakeButtons(setLanguages);
		if (m_dicLanguageButton.TryGetValue(m_eSelectedLanguage, out var value))
		{
			value.Select(bSelect: true);
		}
		SetUIByLanguage(m_eSelectedLanguage);
		UIOpened();
	}

	private void OnTglLanguage(bool value, int data)
	{
		if (value)
		{
			m_eSelectedLanguage = (NKM_NATIONAL_CODE)data;
			SetUIByLanguage(m_eSelectedLanguage);
		}
	}

	private void SetUIByLanguage(NKM_NATIONAL_CODE code)
	{
		NKCUtil.SetImageSprite(m_imageTitle, Resources.Load<Sprite>("LANGUAGE/Title" + NKCStringTable.GetNationalPostfix(code)));
		if (m_csbtnOK != null)
		{
			NKCUtil.SetImageSprite(m_imageOkBtn, Resources.Load<Sprite>("LANGUAGE/OK" + NKCStringTable.GetNationalPostfix(code)));
		}
	}

	public void CloseWithoutCallback()
	{
		dOnClose = null;
		Close();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		Debug.Log("Language Select : " + m_eSelectedLanguage);
		dOnClose?.Invoke(m_eSelectedLanguage);
	}

	private string GetTitleText(NKM_NATIONAL_CODE code)
	{
		switch (code)
		{
		default:
			return "언어 선택";
		case NKM_NATIONAL_CODE.NNC_JAPAN:
			return "言語";
		case NKM_NATIONAL_CODE.NNC_ENG:
			return "Select Language";
		case NKM_NATIONAL_CODE.NNC_CENSORED_CHINESE:
		case NKM_NATIONAL_CODE.NNC_SIMPLIFIED_CHINESE:
			return "选择语言";
		case NKM_NATIONAL_CODE.NNC_TRADITIONAL_CHINESE:
			return "選擇語言";
		case NKM_NATIONAL_CODE.NNC_THAILAND:
			return "เล\u0e37อกภาษา";
		case NKM_NATIONAL_CODE.NNC_VIETNAM:
			return "Chọn Ngôn ngữ";
		}
	}

	private string GetOKText(NKM_NATIONAL_CODE code)
	{
		switch (code)
		{
		default:
			return "확인";
		case NKM_NATIONAL_CODE.NNC_JAPAN:
			return "確認";
		case NKM_NATIONAL_CODE.NNC_ENG:
			return "OK";
		case NKM_NATIONAL_CODE.NNC_CENSORED_CHINESE:
		case NKM_NATIONAL_CODE.NNC_SIMPLIFIED_CHINESE:
			return "确认";
		case NKM_NATIONAL_CODE.NNC_TRADITIONAL_CHINESE:
			return "確認";
		case NKM_NATIONAL_CODE.NNC_THAILAND:
			return "ตกลง";
		case NKM_NATIONAL_CODE.NNC_VIETNAM:
			return "OK";
		}
	}
}
