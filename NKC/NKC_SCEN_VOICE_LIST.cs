using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_VOICE_LIST : NKC_SCEN_BASIC
{
	private GameObject m_NUF_VOICE_LIST;

	private NKC_SCEN_VOICE_LIST_UI_DATA m_NKC_SCEN_VOICE_LIST_UI_DATA = new NKC_SCEN_VOICE_LIST_UI_DATA();

	private NKCUIVoiceListDev m_NKCUIVoiceListDev;

	public NKC_SCEN_VOICE_LIST()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_VOICE_LIST;
		m_NUF_VOICE_LIST = GameObject.Find("NUF_VOICE_LIST");
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		m_NUF_VOICE_LIST.SetActive(value: true);
		if (!m_bLoadedUI && m_NKC_SCEN_VOICE_LIST_UI_DATA.m_NUF_VOICE_LIST_PREFAB == null)
		{
			m_NKC_SCEN_VOICE_LIST_UI_DATA.m_NUF_VOICE_LIST_PREFAB = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_VOICE_LIST", "NUF_VOICE_LIST_PREFAB", bAsync: true);
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (!m_bLoadedUI)
		{
			m_NKC_SCEN_VOICE_LIST_UI_DATA.m_NUF_VOICE_LIST_PREFAB.m_Instant.transform.SetParent(m_NUF_VOICE_LIST.transform, worldPositionStays: false);
			m_NKCUIVoiceListDev = NKCUIVoiceListDev.Init(m_NKC_SCEN_VOICE_LIST_UI_DATA.m_NUF_VOICE_LIST_PREFAB.m_Instant);
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_NKCUIVoiceListDev.Open();
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_NKCUIVoiceListDev.Close();
		m_NUF_VOICE_LIST.SetActive(value: false);
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_NKCUIVoiceListDev = null;
		m_NKC_SCEN_VOICE_LIST_UI_DATA.Init();
	}
}
