using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupWarning : NKCUIBase
{
	public delegate void OnClose(bool bCanceled);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_world_map_renewal";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_WARNING";

	private static NKCPopupWarning m_Instance;

	private OnClose m_dOnClose;

	public Text m_lbMessage;

	public Animator m_amtorWarning;

	public NKCUIComStateButton m_csbtnBG;

	private const float TIME_CAN_SKIP = 1.1f;

	private float m_fElapsedTime;

	public static NKCPopupWarning Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupWarning>("ab_ui_nkm_ui_world_map_renewal", "NKM_UI_POPUP_WARNING", NKCUIManager.eUIBaseRect.UIOverlay, CleanupInstance).GetInstance<NKCPopupWarning>();
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

	public override eMenutype eUIType => eMenutype.Overlay;

	public override string MenuName => "Warning";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void Open(string message, OnClose _dOnClose = null)
	{
		m_dOnClose = _dOnClose;
		NKCUtil.SetGameobjectActive(m_amtorWarning.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_lbMessage.text = message;
		m_fElapsedTime = 0f;
		UIOpened();
	}

	private void Update()
	{
		m_fElapsedTime += Time.deltaTime;
		if (Input.anyKey && m_fElapsedTime > 1.1f)
		{
			Close();
			m_dOnClose?.Invoke(bCanceled: true);
		}
		else if (m_amtorWarning.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
		{
			Close();
			m_dOnClose?.Invoke(bCanceled: false);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
