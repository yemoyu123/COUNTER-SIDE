using System.Collections.Generic;

namespace NKC.UI;

public class NKCPopupSelectEnchantLevel : NKCUIBase
{
	public delegate void OnClose(int targetEnchantLevel);

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_REMOVE_EQUIP";

	private static NKCPopupSelectEnchantLevel m_Instance;

	public List<NKCUIComToggle> m_lstToggle = new List<NKCUIComToggle>();

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnCancel;

	public NKCUIComStateButton m_btnOK;

	private OnClose m_dOnClose;

	private Dictionary<int, NKCUIComToggle> m_dicToggle = new Dictionary<int, NKCUIComToggle>();

	private int m_LastEnchantLevel = 1;

	private int m_CurEnchantLevel = 1;

	public static NKCPopupSelectEnchantLevel Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupSelectEnchantLevel>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_REMOVE_EQUIP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupSelectEnchantLevel>();
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

	public static bool IsInstanceLoaded => m_Instance != null;

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

	private void InitUI()
	{
		m_dicToggle.Clear();
		for (int i = 0; i < m_lstToggle.Count; i++)
		{
			m_dicToggle.Add(i + 1, m_lstToggle[i]);
		}
		foreach (KeyValuePair<int, NKCUIComToggle> kvPair in m_dicToggle)
		{
			kvPair.Value.OnValueChanged.RemoveAllListeners();
			kvPair.Value.OnValueChanged.AddListener(delegate
			{
				OnTgl(kvPair.Key);
			});
		}
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(OnClickCancel);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(OnClickCancel);
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(OnClickOK);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(OnClose dOnClose, int targetEnchantLevel = 1)
	{
		m_dOnClose = dOnClose;
		m_LastEnchantLevel = targetEnchantLevel;
		m_CurEnchantLevel = targetEnchantLevel;
		SetToggleState();
		UIOpened();
	}

	private void SetToggleState()
	{
		foreach (KeyValuePair<int, NKCUIComToggle> item in m_dicToggle)
		{
			if (item.Key <= m_CurEnchantLevel)
			{
				item.Value.Select(bSelect: true, bForce: true);
			}
			else
			{
				item.Value.Select(bSelect: false, bForce: true);
			}
		}
	}

	private void OnTgl(int enchantLevel)
	{
		m_CurEnchantLevel = enchantLevel;
		SetToggleState();
	}

	private void OnClickOK()
	{
		Close();
		m_dOnClose(m_CurEnchantLevel);
	}

	private void OnClickCancel()
	{
		Close();
		m_dOnClose(m_LastEnchantLevel);
	}
}
