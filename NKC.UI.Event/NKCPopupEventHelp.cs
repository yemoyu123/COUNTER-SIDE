using NKM.Event;
using NKM.Templet.Base;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventHelp : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_EVENT_MD";

	public const string UI_ASSET_NAME = "UI_POPUP_EVENT_MD_HELP";

	private static NKCPopupEventHelp m_Instance;

	public Text m_desc;

	public NKCUIComStateButton m_close;

	public NKCUIComButton m_back;

	public static NKCPopupEventHelp Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventHelp>("AB_UI_EVENT_MD", "UI_POPUP_EVENT_MD_HELP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventHelp>();
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

	public override string MenuName => "Help";

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

	private void Init()
	{
		if (m_close != null)
		{
			m_close.PointerClick.RemoveAllListeners();
			m_close.PointerClick.AddListener(base.Close);
		}
		if (m_back != null)
		{
			m_back.PointerClick.RemoveAllListeners();
			m_back.PointerClick.AddListener(base.Close);
		}
	}

	public void Open(int eventID)
	{
		NKMEventTabTemplet nKMEventTabTemplet = NKMTempletContainer<NKMEventTabTemplet>.Find(eventID);
		if (nKMEventTabTemplet != null)
		{
			NKCUtil.SetLabelText(m_desc, NKCStringTable.GetString(nKMEventTabTemplet.m_EventHelpDesc));
			UIOpened();
		}
	}

	public void Open(string HelpDesc)
	{
		NKCUtil.SetLabelText(m_desc, HelpDesc);
		UIOpened();
	}
}
