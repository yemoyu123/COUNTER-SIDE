using System.Collections.Generic;
using TMPro;

namespace NKC.UI;

public class NKCPopupCommonChoice : NKCUIBase
{
	public struct ChoiceOption
	{
		public string text;

		public int data;

		public ChoiceOption(string text, int data)
		{
			this.text = text;
			this.data = data;
		}
	}

	public delegate void OnConfirm(int data);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_COMMON_SELECT";

	private static NKCPopupCommonChoice m_Instance;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	private OnConfirm dOnConfirm;

	private int m_currentOption = -1;

	public TextMeshProUGUI m_lbTitle;

	public List<NKCUIComToggle> m_lstTglChoice;

	public NKCUIComStateButton m_csbtnComfirm;

	public NKCUIComStateButton m_csbtnCancel;

	public static NKCPopupCommonChoice Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupCommonChoice>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_COMMON_SELECT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
				m_Instance = m_loadedUIData.GetInstance<NKCPopupCommonChoice>();
				m_Instance.Init();
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

	public override string MenuName => "CommonChoice";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_loadedUIData != null)
		{
			m_loadedUIData.CloseInstance();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		foreach (NKCUIComToggle item in m_lstTglChoice)
		{
			if (item != null)
			{
				item.OnValueChangedWithData = OnOptionSelect;
			}
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnComfirm, OnBtnConfirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnCancel, base.Close);
		NKCUtil.SetHotkey(m_csbtnComfirm, HotkeyEventType.Confirm);
	}

	public void Open(List<ChoiceOption> lstChoices, string title, OnConfirm onConfirm)
	{
		NKCUtil.SetLabelText(m_lbTitle, title);
		for (int i = 0; i < m_lstTglChoice.Count; i++)
		{
			if (i < lstChoices.Count && m_lstTglChoice[i] != null)
			{
				NKCUtil.SetGameobjectActive(m_lstTglChoice[i], bValue: true);
				m_lstTglChoice[i].m_DataInt = lstChoices[i].data;
				m_lstTglChoice[i].SetTitleText(lstChoices[i].text);
				if (i == 0)
				{
					m_lstTglChoice[i].Select(bSelect: true, bForce: true);
					m_currentOption = lstChoices[i].data;
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstTglChoice[i], bValue: false);
			}
		}
		dOnConfirm = onConfirm;
		UIOpened();
	}

	private void OnOptionSelect(bool bSelect, int data)
	{
		if (bSelect)
		{
			m_currentOption = data;
		}
	}

	private void OnBtnConfirm()
	{
		Close();
		dOnConfirm?.Invoke(m_currentOption);
	}
}
