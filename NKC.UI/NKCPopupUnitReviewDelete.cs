using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupUnitReviewDelete : NKCUIBase
{
	public delegate void OnButton();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_review";

	private const string UI_ASSET_NAME = "NKM_UI_UNIT_REVIEW_POPUP_POST_DELETE";

	private static NKCPopupUnitReviewDelete m_Instance;

	private NKCUIOpenAnimator m_openAni;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnDelete;

	public NKCUIComStateButton m_btnCancel;

	public Text m_lbTitle;

	public Text m_lbDesc;

	private OnButton dOnDeleteButton;

	private bool m_bInitComplete;

	public static NKCPopupUnitReviewDelete Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupUnitReviewDelete>("ab_ui_nkm_ui_unit_review", "NKM_UI_UNIT_REVIEW_POPUP_POST_DELETE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupUnitReviewDelete>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_POPUP_UNIT_REVIEW_DELETE;

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
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		m_openAni = new NKCUIOpenAnimator(base.gameObject);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnDelete.PointerClick.RemoveAllListeners();
		m_btnDelete.PointerClick.AddListener(OnClickDelete);
		NKCUtil.SetHotkey(m_btnDelete, HotkeyEventType.Confirm);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
		m_bInitComplete = true;
	}

	public void OpenUI(string title, string desc, OnButton onDeleteButton)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		m_openAni.PlayOpenAni();
		if (onDeleteButton != null)
		{
			dOnDeleteButton = onDeleteButton;
		}
		m_lbTitle.text = title;
		m_lbDesc.text = desc;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UIOpened();
	}

	private void OnClickDelete()
	{
		dOnDeleteButton?.Invoke();
		Close();
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_openAni.Update();
		}
	}
}
