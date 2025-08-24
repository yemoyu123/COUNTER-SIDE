using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_CUTSCEN_SIM : NKC_SCEN_BASIC
{
	private GameObject m_NUF_CUTSCEN_SIM;

	private NKC_SCEN_CUTSCEN_SIM_UI_DATA m_NKC_SCEN_CUTSCEN_SIM_UI_DATA = new NKC_SCEN_CUTSCEN_SIM_UI_DATA();

	private NKCUICutsceneSimViewer m_NKCUICutsceneSimViewer;

	public NKC_SCEN_CUTSCEN_SIM()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_CUTSCENE_SIM;
		m_NUF_CUTSCEN_SIM = GameObject.Find("NUF_CUTSCEN_SIM");
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		m_NUF_CUTSCEN_SIM.SetActive(value: true);
		if (!m_bLoadedUI && m_NKC_SCEN_CUTSCEN_SIM_UI_DATA.m_NUF_CUTSCEN_SIM_PREFAB == null)
		{
			m_NKC_SCEN_CUTSCEN_SIM_UI_DATA.m_NUF_CUTSCEN_SIM_PREFAB = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_CUTSCEN", "NUF_CUTSCEN_SIM_PREFAB", bAsync: true);
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (!m_bLoadedUI)
		{
			m_NKC_SCEN_CUTSCEN_SIM_UI_DATA.m_NUF_CUTSCEN_SIM_PREFAB.m_Instant.transform.SetParent(m_NUF_CUTSCEN_SIM.transform, worldPositionStays: false);
			NKCUICutScenPlayer.InitiateInstance();
			m_NKCUICutsceneSimViewer = NKCUICutsceneSimViewer.InitUI();
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_NKCUICutsceneSimViewer.Open();
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_NKCUICutsceneSimViewer.Close();
		m_NUF_CUTSCEN_SIM.SetActive(value: false);
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_NKCUICutsceneSimViewer = null;
		m_NKC_SCEN_CUTSCEN_SIM_UI_DATA.Init();
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}
}
