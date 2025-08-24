using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildKick : NKCUIBase
{
	public delegate void OnClose(int reason);

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_POPUP_KICK";

	private static NKCPopupGuildKick m_Instance;

	public Text m_lbTitle;

	public Text m_lbDesc;

	public List<NKCUIComToggle> m_lstTglBanReason;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnOK;

	public GameObject m_objOKDisabled;

	public NKCUIComStateButton m_btnCancel;

	private OnClose m_dOnClose;

	private int m_selectedReason;

	public static NKCPopupGuildKick Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildKick>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_POPUP_KICK", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildKick>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
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

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		for (int i = 0; i < m_lstTglBanReason.Count; i++)
		{
			m_lstTglBanReason[i].OnValueChanged.RemoveAllListeners();
			m_lstTglBanReason[i].OnValueChanged.AddListener(OnSelectReason);
		}
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(OnClickOK);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
	}

	public void Open(string userName, OnClose onClose)
	{
		m_dOnClose = onClose;
		for (int i = 0; i < m_lstTglBanReason.Count; i++)
		{
			m_lstTglBanReason[i].Select(bSelect: false, bForce: true, bImmediate: true);
		}
		m_selectedReason = 0;
		NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_FORCE_EXIT_CONFIRM_POPUP_TITLE_DESC);
		NKCUtil.SetLabelText(m_lbDesc, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_MEMBER_FORCE_EXIT_CONFIRM_POPUP_BODY_DESC, userName));
		NKCUtil.SetGameobjectActive(m_objOKDisabled, bValue: true);
		UIOpened();
	}

	private void OnSelectReason(bool bSelect)
	{
		if (bSelect)
		{
			m_selectedReason = 0;
			for (int i = 0; i < m_lstTglBanReason.Count; i++)
			{
				if (m_lstTglBanReason[i].m_bChecked)
				{
					m_selectedReason = i + 1;
					break;
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objOKDisabled, m_selectedReason == 0);
	}

	private void OnClickOK()
	{
		if (m_selectedReason != 0)
		{
			m_dOnClose?.Invoke(m_selectedReason);
		}
	}

	private void OnClickOKDisabled()
	{
		NKCPopupMessageManager.AddPopupMessage("");
	}
}
