using NKC.UI;
using TMPro;
using UnityEngine;

namespace NKC;

public class NKCUICutScenLogViewerSlot : MonoBehaviour
{
	public TextMeshProUGUI m_lbTmpDesc;

	public NKCUIComStateButton m_csbtnVoice;

	private string m_voiceFileName = string.Empty;

	private NKCUICutScenPlayer.OnVoice dOnVoice;

	public void SetData(NKCUICutScenPlayer.CutsceneLog log, NKCUICutScenPlayer.OnVoice onVoice)
	{
		m_lbTmpDesc?.SetText(log.text);
		m_voiceFileName = log.voice;
		dOnVoice = onVoice;
		bool flag = false;
		if (!string.IsNullOrEmpty(m_voiceFileName))
		{
			flag = NKCAssetResourceManager.IsBundleExists(NKCAssetResourceManager.GetBundleName(m_voiceFileName, bIgnoreNotFoundError: true), m_voiceFileName);
		}
		if (flag)
		{
			NKCUtil.SetGameobjectActive(m_csbtnVoice, bValue: true);
			NKCUtil.SetButtonClickDelegate(m_csbtnVoice, OnVoiceButton);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnVoice, bValue: false);
		}
	}

	private void OnVoiceButton()
	{
		dOnVoice?.Invoke(m_voiceFileName);
	}
}
