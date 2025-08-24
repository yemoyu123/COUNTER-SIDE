using System;
using System.Collections;
using System.Collections.Generic;
using NKC.Patcher;

namespace NKC.UI;

public class NKCPopupDownloadTypeSelection : NKCUIBase
{
	public NKCPopupDownloadTypeSlot[] m_downloadTypeSlot;

	private NKCPatchDownloader.DownType m_currentDownloadType;

	private Action<NKCPatchDownloader.DownType> _onClickOk;

	public NKCUIComButton OkButton;

	private bool _waitForClick;

	private List<NKCPatchDownloader.DownType> m_activatedDownloadTypeSelection = new List<NKCPatchDownloader.DownType>();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "NKCPopupDownloadTypeSelection";

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Awake()
	{
		Init();
	}

	public void Init()
	{
		if (m_downloadTypeSlot != null)
		{
			for (int i = 0; i < m_downloadTypeSlot.Length; i++)
			{
				NKCUtil.SetGameobjectActive(m_downloadTypeSlot[i], bValue: false);
			}
			m_activatedDownloadTypeSelection.Clear();
			m_activatedDownloadTypeSelection.Add(NKCPatchDownloader.DownType.FullDownload);
			if (!NKCPatchUtility.GetTutorialClearedStatus())
			{
				m_activatedDownloadTypeSelection.Add(NKCPatchDownloader.DownType.TutorialWithBackground);
			}
			m_currentDownloadType = NKCPatchDownloader.DownType.FullDownload;
			RefreshUI();
			OkButton.PointerClick.RemoveAllListeners();
			OkButton.PointerClick.AddListener(OnClickOk);
		}
	}

	public void RefreshUI()
	{
		if (m_downloadTypeSlot == null)
		{
			return;
		}
		foreach (NKCPatchDownloader.DownType item in m_activatedDownloadTypeSelection)
		{
			int num = (int)item;
			m_downloadTypeSlot[num].SetSelect(active: false);
			m_downloadTypeSlot[num].SetData(item, ChangeToggleActive);
			NKCUtil.SetGameobjectActive(m_downloadTypeSlot[num], bValue: true);
			if (m_currentDownloadType == item)
			{
				m_downloadTypeSlot[num].SetSelect(active: true);
			}
		}
	}

	public void Open(Action<NKCPatchDownloader.DownType> onClickOK, List<NKCPopupDownloadTypeData> downloadTypeDataList)
	{
	}

	public void Open(Action<NKCPatchDownloader.DownType> okPopup, float totalDownloadSize, float essentialDownloadSize, float nonEssentialDownloadSize, float tutorialDownloadSize)
	{
		_waitForClick = true;
		NKCUIManager.OpenUI(base.gameObject);
		base.gameObject.SetActive(value: true);
		Init();
		for (int i = 0; i < 2; i++)
		{
			string downloadSizeText = (NKCPatchDownloader.DownType)i switch
			{
				NKCPatchDownloader.DownType.FullDownload => $"{totalDownloadSize:F}mb", 
				NKCPatchDownloader.DownType.TutorialWithBackground => $"{tutorialDownloadSize:F}mb + ({essentialDownloadSize:F}mb)", 
				_ => "", 
			};
			m_downloadTypeSlot[i].SetDownloadSizeText(downloadSizeText);
		}
		if (tutorialDownloadSize <= 0f)
		{
			m_activatedDownloadTypeSelection.Remove(NKCPatchDownloader.DownType.TutorialWithBackground);
		}
		_onClickOk = okPopup;
	}

	private void ChangeToggleActive(NKCPatchDownloader.DownType downloadType)
	{
		m_currentDownloadType = downloadType;
		RefreshUI();
	}

	public void OnClickOk()
	{
		_waitForClick = false;
		_onClickOk?.Invoke(m_currentDownloadType);
	}

	public IEnumerator WaitForClick()
	{
		while (_waitForClick)
		{
			yield return null;
		}
		CloseInternal();
	}
}
