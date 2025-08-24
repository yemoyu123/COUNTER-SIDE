using System.Collections.Generic;
using NKC.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupFilterTheme : NKCUIBase
{
	public delegate void OnSelectTheme(int themeID);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_selection";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FILTER_FNC";

	private static NKCPopupFilterTheme m_Instance;

	public LoopScrollRect m_LoopScrollRect;

	public NKCPopupFilterThemeSlot m_pfbSlot;

	public int MinColumnCount = 4;

	public NKCUIComToggleGroup m_ToggleGroup;

	public NKCUIComStateButton m_csbtnReset;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnBackground;

	private List<NKCThemeGroupTemplet> m_lstThemeTemplet = new List<NKCThemeGroupTemplet>();

	private OnSelectTheme dOnSelectTheme;

	private int m_SelectedID;

	public static NKCPopupFilterTheme Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFilterTheme>("ab_ui_nkm_ui_popup_selection", "NKM_UI_POPUP_FILTER_FNC", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFilterTheme>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => string.Empty;

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

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Init()
	{
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetObject;
			m_LoopScrollRect.dOnReturnObject += ReturnObject;
			m_LoopScrollRect.dOnProvideData += ProvideData;
			m_LoopScrollRect.SetAutoResize(MinColumnCount);
			m_LoopScrollRect.PrepareCells();
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnReset, OnBtnReset);
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnBackground, base.Close);
	}

	public void Open(OnSelectTheme onSelectTheme, int currentSelectedThemeID)
	{
		NKCThemeGroupTemplet.Load();
		m_SelectedID = currentSelectedThemeID;
		m_lstThemeTemplet.Clear();
		foreach (NKCThemeGroupTemplet value in NKMTempletContainer<NKCThemeGroupTemplet>.Values)
		{
			if (value.EnableByTag)
			{
				m_lstThemeTemplet.Add(value);
			}
		}
		base.gameObject.SetActive(value: true);
		m_LoopScrollRect.TotalCount = m_lstThemeTemplet.Count;
		m_LoopScrollRect.SetIndexPosition(0);
		dOnSelectTheme = onSelectTheme;
		UIOpened();
	}

	private RectTransform GetObject(int idx)
	{
		NKCPopupFilterThemeSlot nKCPopupFilterThemeSlot = Object.Instantiate(m_pfbSlot);
		RectTransform component = nKCPopupFilterThemeSlot.GetComponent<RectTransform>();
		nKCPopupFilterThemeSlot.Init(m_ToggleGroup);
		return component;
	}

	private void ReturnObject(Transform tr)
	{
		tr.SetParent(base.transform);
		tr.gameObject.SetActive(value: false);
		Object.Destroy(tr.gameObject);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupFilterThemeSlot component = tr.GetComponent<NKCPopupFilterThemeSlot>();
		if (!(component == null))
		{
			component.SetData(m_lstThemeTemplet[idx], OnSelectSlot);
			component.SetSelected(m_lstThemeTemplet[idx].Key == m_SelectedID);
		}
	}

	private void OnSelectSlot(int themeID)
	{
		m_SelectedID = themeID;
		Close();
		dOnSelectTheme?.Invoke(themeID);
	}

	private void OnBtnReset()
	{
		m_SelectedID = 0;
		m_LoopScrollRect.RefreshCells();
		dOnSelectTheme?.Invoke(0);
	}
}
