using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupFirstRunOptionSetup : NKCUIBase
{
	private enum eUIState
	{
		GraphicOption,
		GameOption,
		EffectTransparency
	}

	public delegate void OnClose();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_game_option";

	private const string UI_ASSET_NAME = "NKM_UI_GAME_OPTION_SETTING";

	private static NKCPopupFirstRunOptionSetup m_Instance;

	[Header("상단")]
	public Text m_lbTitle;

	public Text m_lbSubTitle;

	[Header("그래픽 옵션 설정")]
	public GameObject m_objGraphicRoot;

	public NKCUIComToggle m_tglGraphicLow;

	public NKCUIComToggle m_tglGraphicMid;

	public NKCUIComToggle m_tglGraphicHigh;

	[Header("게임 옵션 설정")]
	public GameObject m_objGameRoot;

	public GameObject m_objGameSimple;

	public GameObject m_objGameFullInfo;

	public NKCUIComToggle m_tglGameSimple;

	public NKCUIComToggle m_tglGameFullInfo;

	[Header("이펙트 투명도 설정")]
	public GameObject m_objEffectRoot;

	public Image m_imgEffectTransparency;

	public Slider m_sldrEffectTransparency;

	public NKCUIComStateButton m_csbtnEffectMinus;

	public NKCUIComStateButton m_csbtnEffectPlus;

	public NKCUIComButton m_btnOK;

	private eUIState m_eCurrentState;

	private static bool m_sOptionSetupRequired = true;

	private const string FIRST_OPTION_SETUP_KEY = "FIRST_OPTION_SETUP_KEY";

	private OnClose m_dOnClose;

	public static NKCPopupFirstRunOptionSetup Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFirstRunOptionSetup>("ab_ui_nkm_ui_game_option", "NKM_UI_GAME_OPTION_SETTING", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFirstRunOptionSetup>();
				m_Instance.InitUI();
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

	public override string MenuName => NKCStringTable.GetString("SI_DP_POPUP_FIRST_OPTION");

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

	public static bool IsOptionSetupRequired()
	{
		if (NKCUtil.IsUsingSuperUserFunction() && Input.GetKey(KeyCode.Space))
		{
			return true;
		}
		if (!m_sOptionSetupRequired)
		{
			return false;
		}
		if (PlayerPrefs.HasKey("FIRST_OPTION_SETUP_KEY"))
		{
			m_sOptionSetupRequired = false;
			return false;
		}
		return true;
	}

	public static void ResetSetupReq()
	{
		m_sOptionSetupRequired = true;
		PlayerPrefs.DeleteKey("FIRST_OPTION_SETUP_KEY");
	}

	private void InitUI()
	{
		NKCUtil.SetToggleValueChangedDelegate(m_tglGameSimple, OnGameSimple);
		NKCUtil.SetToggleValueChangedDelegate(m_tglGameFullInfo, OnGameFullInfo);
		NKCUtil.SetButtonClickDelegate(m_btnOK, OnBtnOK);
		NKCUtil.SetHotkey(m_btnOK, HotkeyEventType.Confirm);
		NKCUtil.SetSliderValueChangedDelegate(m_sldrEffectTransparency, OnEffectTransparencyChanged);
		NKCUtil.SetButtonClickDelegate(m_csbtnEffectPlus, OnEffectPlus);
		NKCUtil.SetButtonClickDelegate(m_csbtnEffectMinus, OnEffectMinus);
	}

	public override void OnBackButton()
	{
		OnBtnOK();
	}

	public override void CloseInternal()
	{
		SetOptionFromSelection();
		base.gameObject.SetActive(value: false);
		m_dOnClose?.Invoke();
	}

	public void Open(OnClose onClose = null)
	{
		SetState(eUIState.GraphicOption);
		if (m_tglGameSimple != null)
		{
			m_tglGameSimple.Select(bSelect: true);
		}
		if (m_tglGraphicMid != null)
		{
			m_tglGraphicMid.Select(bSelect: true);
		}
		if (m_sldrEffectTransparency != null)
		{
			m_sldrEffectTransparency.value = m_sldrEffectTransparency.maxValue;
		}
		m_dOnClose = onClose;
		UIOpened();
	}

	private void SetState(eUIState state)
	{
		m_eCurrentState = state;
		NKCUtil.SetGameobjectActive(m_objGraphicRoot, state == eUIState.GraphicOption);
		NKCUtil.SetGameobjectActive(m_objGameRoot, state == eUIState.GameOption);
		NKCUtil.SetGameobjectActive(m_objEffectRoot, state == eUIState.EffectTransparency);
		switch (state)
		{
		case eUIState.GraphicOption:
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_DP_POPUP_FIRST_OPTION_GRAPHIC_TITLE"));
			NKCUtil.SetLabelText(m_lbSubTitle, NKCStringTable.GetString("SI_DP_POPUP_FIRST_OPTION_GRAPHIC_SUBTITLE"));
			break;
		case eUIState.GameOption:
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_DP_POPUP_FIRST_OPTION_GAME_TITLE"));
			NKCUtil.SetLabelText(m_lbSubTitle, NKCStringTable.GetString("SI_DP_POPUP_FIRST_OPTION_GAME_SUBTITLE"));
			break;
		case eUIState.EffectTransparency:
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_DP_POPUP_FIRST_OPTION_EFFECT_TITLE"));
			NKCUtil.SetLabelText(m_lbSubTitle, NKCStringTable.GetString("SI_DP_POPUP_FIRST_OPTION_EFFECT_SUBTITLE"));
			break;
		}
	}

	private void OnBtnOK()
	{
		switch (m_eCurrentState)
		{
		case eUIState.GraphicOption:
			SetState(eUIState.GameOption);
			break;
		case eUIState.GameOption:
			SetState(eUIState.EffectTransparency);
			break;
		case eUIState.EffectTransparency:
			Close();
			break;
		}
	}

	private void SetOptionFromSelection()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (m_tglGraphicHigh.m_bSelect)
		{
			Debug.Log("Graphic option : High");
			gameOptionData.SetGameOptionDataByGrahpicQuality(NKC_GAME_OPTION_GRAPHIC_QUALITY.VERY_HIGH);
		}
		else if (m_tglGraphicMid.m_bSelect)
		{
			Debug.Log("Graphic option : Mid");
			gameOptionData.SetGameOptionDataByGrahpicQuality(NKC_GAME_OPTION_GRAPHIC_QUALITY.NORMAL);
		}
		else
		{
			Debug.Log("Graphic option : Low");
			gameOptionData.SetGameOptionDataByGrahpicQuality(NKC_GAME_OPTION_GRAPHIC_QUALITY.VERY_LOW);
		}
		if (m_tglGameSimple != null && m_tglGameSimple.m_bSelect)
		{
			Debug.Log("Game Option : Simple");
			gameOptionData.UseDamageAndBuffNumberFx = NKCGameOptionDataSt.GameOptionDamageNumber.Limited;
			gameOptionData.UseClassGuide = true;
		}
		else
		{
			Debug.Log("Game Option : Full");
			gameOptionData.UseDamageAndBuffNumberFx = NKCGameOptionDataSt.GameOptionDamageNumber.On;
			gameOptionData.UseClassGuide = true;
		}
		if (m_sldrEffectTransparency != null)
		{
			int num = (int)m_sldrEffectTransparency.value;
			Debug.Log("Effect Transparency : " + num);
			gameOptionData.EffectOpacity = num;
			gameOptionData.EffectEnemyOpacity = num;
		}
		gameOptionData.Save();
		gameOptionData.ApplyToGame();
		PlayerPrefs.SetInt("FIRST_OPTION_SETUP_KEY", 0);
		PlayerPrefs.Save();
	}

	private void OnChangeGameOption(bool bSimple)
	{
		NKCUtil.SetGameobjectActive(m_objGameSimple, bSimple);
		NKCUtil.SetGameobjectActive(m_objGameFullInfo, !bSimple);
	}

	private void OnGameSimple(bool bValue)
	{
		if (bValue)
		{
			OnChangeGameOption(bSimple: true);
		}
	}

	private void OnGameFullInfo(bool bValue)
	{
		if (bValue)
		{
			OnChangeGameOption(bSimple: false);
		}
	}

	private void OnEffectPlus()
	{
		if (m_sldrEffectTransparency != null)
		{
			SetEffectTransparencyValue(m_sldrEffectTransparency.value + 1f);
		}
	}

	private void OnEffectMinus()
	{
		if (m_sldrEffectTransparency != null)
		{
			SetEffectTransparencyValue(m_sldrEffectTransparency.value - 1f);
		}
	}

	private void OnEffectTransparencyChanged(float value)
	{
		SetEffectTransparencyValue(value);
	}

	private void SetEffectTransparencyValue(float value)
	{
		if (m_sldrEffectTransparency != null)
		{
			m_sldrEffectTransparency.SetValueWithoutNotify(value);
		}
		if (m_imgEffectTransparency != null)
		{
			float num = value / 100f;
			m_imgEffectTransparency.color = new Color(1f, 1f, 1f, num * num);
		}
	}
}
