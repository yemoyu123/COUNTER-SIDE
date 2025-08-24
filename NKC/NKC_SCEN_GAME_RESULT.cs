using NKC.Publisher;
using NKC.UI;
using NKC.UI.Gauntlet;
using NKC.UI.Result;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_GAME_RESULT : NKC_SCEN_BASIC
{
	public delegate void DoAtScenStart();

	private DoAtScenStart m_DoAtScenStart;

	private NKCAssetResourceData m_UILoadResourceData;

	private int m_StageID;

	private const float FIVE_SECONDS = 5f;

	private float m_deltaTime;

	public void SetStageID(int stageID)
	{
		m_StageID = stageID;
	}

	public int GetStageID()
	{
		return m_StageID;
	}

	public NKC_SCEN_GAME_RESULT()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GAME_RESULT;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (NKCUIManager.NKCUIGauntletResult == null)
		{
			m_UILoadResourceData = NKCUIGauntletResult.OpenInstanceAsync();
		}
		else
		{
			m_UILoadResourceData = null;
		}
	}

	public override void ScenLoadUpdate()
	{
		if (!NKCAssetResourceManager.IsLoadEnd())
		{
			return;
		}
		if (NKCUIManager.NKCUIGauntletResult == null && m_UILoadResourceData != null)
		{
			NKCUIGauntletResult retVal = null;
			if (!NKCUIGauntletResult.CheckInstanceLoaded(m_UILoadResourceData, out retVal))
			{
				return;
			}
			NKCUIManager.NKCUIGauntletResult = retVal;
			m_UILoadResourceData = null;
		}
		ScenLoadLastStart();
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		if (NKCUIManager.NKCUIGauntletResult != null)
		{
			NKCUIManager.NKCUIGauntletResult.InitUI();
		}
		if (NKCPublisherModule.Auth.LogoutReservedAfterGame)
		{
			NKCPublisherModule.Auth.LogoutReserved();
		}
	}

	public override void ScenDataReq()
	{
		base.ScenDataReq();
	}

	public override void ScenDataReqWaitUpdate()
	{
		if (!NKCGuildCoopManager.m_bWaitForArenaResult)
		{
			base.ScenDataReqWaitUpdate();
			return;
		}
		m_deltaTime += Time.deltaTime;
		if (m_deltaTime > 5f)
		{
			m_deltaTime = 0f;
			Set_NKC_SCEN_STATE(NKC_SCEN_STATE.NSS_FAIL);
		}
		else if (NKCGuildCoopManager.m_bArenaResultRecved)
		{
			NKCGuildCoopManager.SetWaitForArenaResult(bValue: false);
			m_deltaTime = 0f;
			base.ScenDataReqWaitUpdate();
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCUIResult.CheckInstanceAndClose();
		NKCUIManager.NKCUIGauntletResult.Close();
		DoWhenScenStart();
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		NKCUIResult.CheckInstanceAndClose();
		NKCUIManager.NKCUIGauntletResult?.Close();
	}

	private void Update()
	{
	}

	public void SetDoAtScenStart(DoAtScenStart doAtScenStart)
	{
		m_DoAtScenStart = doAtScenStart;
	}

	private void DoWhenScenStart()
	{
		if (m_DoAtScenStart != null)
		{
			m_DoAtScenStart();
		}
	}
}
