using System.Collections.Generic;
using ClientPacket.Mode;
using NKC.UI.Result;
using NKM;

namespace NKC;

public class NKC_SCEN_SHADOW_RESULT : NKC_SCEN_BASIC
{
	private NKCUIWarfareResult m_resultUI;

	private NKMShadowGameResult m_shadowResult;

	private List<int> m_lstBestTime = new List<int>();

	private int m_unitID;

	private int m_shipID;

	public NKC_SCEN_SHADOW_RESULT()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_SHADOW_RESULT;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		m_resultUI = NKCUIWarfareResult.Instance;
	}

	public void SetData(NKMShadowGameResult result, List<int> lstBestTime)
	{
		m_shadowResult = result;
		m_lstBestTime = lstBestTime;
		m_unitID = 999;
		m_shipID = 26001;
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_resultUI?.OpenForShadow(m_shadowResult, m_lstBestTime, m_unitID, m_shipID, OnCloseResultUI);
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_resultUI?.Close();
	}

	private void OnCloseResultUI(NKM_SCEN_ID scenID)
	{
		NKCScenManager.GetScenManager().ScenChangeFade(scenID);
	}
}
