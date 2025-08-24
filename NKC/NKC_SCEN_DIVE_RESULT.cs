using System.Collections.Generic;
using NKC.UI;
using NKC.UI.Result;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKC_SCEN_DIVE_RESULT : NKC_SCEN_BASIC
{
	private NKCUIWarfareResult m_NKCUIWarfareResult;

	private bool m_bNewData;

	private bool m_bDiveClear;

	private NKMRewardData m_NKMRewardData;

	private NKMRewardData m_NKMRewardDataArtifact;

	private NKMRewardData m_NKMRewardDataStorm;

	private NKMDeckIndex m_currNKMDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_DIVE, 0);

	private bool m_bEventDive;

	private List<int> m_lstArtifact = new List<int>();

	private NKMDiveTemplet m_DiveTemplet;

	public NKC_SCEN_DIVE_RESULT()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_DIVE_RESULT;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		m_NKCUIWarfareResult = NKCUIWarfareResult.Instance;
	}

	public bool GetExistNewData()
	{
		return m_bNewData;
	}

	public void SetData(bool bDiveClear, bool bEventDive, NKMRewardData cNKMRewardData, NKMRewardData cNKMRewardDataArtifact, NKMItemMiscData cStormMiscReward, List<int> lstArtifact, NKMDeckIndex sNKMDeckIndex, NKMDiveTemplet diveTemplet)
	{
		m_bNewData = true;
		m_bDiveClear = bDiveClear;
		m_NKMRewardData = cNKMRewardData;
		m_NKMRewardDataArtifact = cNKMRewardDataArtifact;
		if (cStormMiscReward != null)
		{
			m_NKMRewardDataStorm = new NKMRewardData();
			m_NKMRewardDataStorm.SetMiscItemData(new List<NKMItemMiscData> { cStormMiscReward });
		}
		else
		{
			m_NKMRewardDataStorm = null;
		}
		m_lstArtifact.Clear();
		if (lstArtifact != null)
		{
			m_lstArtifact.AddRange(lstArtifact);
		}
		m_currNKMDeckIndex = sNKMDeckIndex;
		m_bEventDive = bEventDive;
		m_DiveTemplet = diveTemplet;
	}

	private void OnCallBackForResult(NKM_SCEN_ID scenID)
	{
		if (m_bEventDive)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetReservedDiveReverseAni(bSet: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(scenID);
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		if (!m_bNewData)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DIVE);
		}
		else if (m_bDiveClear && m_DiveTemplet != null && !string.IsNullOrEmpty(m_DiveTemplet.CutsceneDiveBossAfter) && !NKCScenManager.CurrentUserData().m_LastDiveHistoryData.Contains(m_DiveTemplet.StageID))
		{
			NKCUICutScenPlayer.Instance.LoadAndPlay(m_DiveTemplet.CutsceneDiveBossAfter, 0, ShowResult);
		}
		else
		{
			ShowResult();
		}
	}

	private void ShowResult()
	{
		m_NKCUIWarfareResult.OpenForDive(m_bDiveClear, m_NKMRewardData, m_NKMRewardDataArtifact, m_NKMRewardDataStorm, m_lstArtifact, m_currNKMDeckIndex, OnCallBackForResult);
		m_bNewData = false;
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_NKCUIWarfareResult.Close();
		m_NKCUIWarfareResult = null;
	}
}
