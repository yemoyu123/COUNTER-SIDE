using ClientPacket.Common;
using ClientPacket.Mode;
using NKC.UI.Result;
using NKM;

namespace NKC;

public class NKC_SCEN_TRIM_RESULT : NKC_SCEN_BASIC
{
	private NKCUIWarfareResult m_resultUI;

	private TrimModeState m_trimModeState;

	private NKMTrimClearData m_trimClearData;

	private long m_unitUId;

	private int m_bestScore;

	private bool m_bFirstClear;

	public NKC_SCEN_TRIM_RESULT()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_TRIM_RESULT;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		m_resultUI = NKCUIWarfareResult.Instance;
	}

	public void SetUnitUId(long unitUId)
	{
		m_unitUId = unitUId;
	}

	public void SetData(NKMTrimClearData trimClearData, TrimModeState trimModeState, int bestScore, bool firstClear)
	{
		m_trimModeState = trimModeState;
		m_trimClearData = trimClearData;
		m_bestScore = bestScore;
		m_bFirstClear = firstClear;
	}

	public override void ScenStart()
	{
		base.ScenStart();
		int dummyUnitId = ((m_unitUId == 0L) ? 999 : 0);
		NKCUIWarfareResult.Instance.OpenForTrim(m_trimClearData, m_trimModeState, m_unitUId, dummyUnitId, m_bFirstClear, m_bestScore, OnCloseResultUI);
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_resultUI?.Close();
		m_resultUI = null;
		m_trimClearData = null;
		m_trimModeState = null;
		m_bFirstClear = false;
		m_unitUId = 0L;
	}

	private void OnCloseResultUI(NKM_SCEN_ID scenID)
	{
		NKCScenManager.GetScenManager().ScenChangeFade(scenID);
	}
}
