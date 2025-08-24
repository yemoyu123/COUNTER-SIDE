using System.Collections.Generic;
using NKC.UI.Component;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupImage : NKCUIBase
{
	public NKCUIComStateButton m_btnClose;

	public NKCComTMPUIText m_lbTitle;

	public NKCComTMPUIText m_lbDesc;

	public List<GameObject> m_lstExtraObject;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public static NKCPopupImage OpenInstance(string bundleName, string assetName)
	{
		NKCPopupImage instance = NKCUIManager.OpenNewInstance<NKCPopupImage>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIFrontPopup, null).GetInstance<NKCPopupImage>();
		if ((object)instance != null)
		{
			instance.Init();
			return instance;
		}
		return instance;
	}

	private void Init()
	{
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(base.Close);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(string title = "", string desc = "", bool bSetExtraObject = false)
	{
		if (m_lbTitle != null && !string.IsNullOrEmpty(title))
		{
			NKCUtil.SetLabelText(m_lbTitle, title);
		}
		if (m_lbDesc != null && !string.IsNullOrEmpty(desc))
		{
			NKCUtil.SetLabelText(m_lbDesc, desc);
		}
		if (m_lstExtraObject != null)
		{
			for (int i = 0; i < m_lstExtraObject.Count; i++)
			{
				NKCUtil.SetGameobjectActive(m_lstExtraObject[i], bSetExtraObject);
			}
		}
		UIOpened();
	}
}
