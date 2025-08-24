using TMPro;

namespace NKC.UI;

public class NKCUIPopupDungeonGuide : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "UI_EVENT_MD_POPUP_SINGLE_DEF_2";

	private const string UI_ASSET_NAME = "UI_EVENT_MD_POPUP_SINGLE_DEF_2_RULE";

	private static NKCUIPopupDungeonGuide m_Instance;

	public NKCUIComStateButton m_btnClose;

	public TMP_Text m_lbTitle;

	public TMP_Text m_lbDesc;

	public static NKCUIPopupDungeonGuide Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupDungeonGuide>("UI_EVENT_MD_POPUP_SINGLE_DEF_2", "UI_EVENT_MD_POPUP_SINGLE_DEF_2_RULE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupDungeonGuide>();
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

	private void Init()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
	}

	public void Open(string dungeonName, string bannerDesc)
	{
		NKCUtil.SetLabelText(m_lbTitle, dungeonName);
		NKCUtil.SetLabelText(m_lbDesc, bannerDesc);
		UIOpened();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
