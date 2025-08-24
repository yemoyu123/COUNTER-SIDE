using System;
using NKC.Patcher;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupDownloadTypeSlot : MonoBehaviour
{
	public NKCUIComStateButton m_slotButton;

	public Text NKM_UI_POPUP_DOWN_TITLE_TEXT;

	public Text NKM_UI_POPUP_DOWNLOAD_SIZE_TEXT;

	public Text NKM_UI_POPUP_DOWN_INFO_TEXT;

	private Action<NKCPatchDownloader.DownType> m_onClick;

	public NKCPatchDownloader.DownType m_downloadType { get; private set; }

	private void Awake()
	{
		if (m_slotButton != null)
		{
			m_slotButton.PointerClick.RemoveAllListeners();
			m_slotButton.PointerClick.AddListener(OnClick);
		}
	}

	public void SetData(NKCPatchDownloader.DownType downloadType, Action<NKCPatchDownloader.DownType> onClickButton)
	{
		m_downloadType = downloadType;
		m_onClick = onClickButton;
		if (NKM_UI_POPUP_DOWN_TITLE_TEXT != null)
		{
			NKM_UI_POPUP_DOWN_TITLE_TEXT.text = NKCStringTable.GetString("SI_DP_PATCHER_DOWNLOADTYPE_TITLE_" + downloadType);
		}
		if (NKM_UI_POPUP_DOWN_INFO_TEXT != null)
		{
			NKM_UI_POPUP_DOWN_INFO_TEXT.text = NKCStringTable.GetString("SI_DP_PATCHER_DOWNLOADTYPE_DESC_" + downloadType);
		}
	}

	public void SetDownloadSizeText(string downloadSizeString)
	{
		if (NKM_UI_POPUP_DOWNLOAD_SIZE_TEXT != null)
		{
			NKM_UI_POPUP_DOWNLOAD_SIZE_TEXT.text = NKCStringTable.GetString("SI_DP_PATCHER_DOWNLOAD_SIZE") + " : " + downloadSizeString;
		}
	}

	private void OnClick()
	{
		m_onClick?.Invoke(m_downloadType);
	}

	public void SetSelect(bool active)
	{
		m_slotButton.Select(active);
	}
}
