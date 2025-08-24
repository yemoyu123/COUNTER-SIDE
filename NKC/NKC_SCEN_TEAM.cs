using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_TEAM : NKC_SCEN_BASIC
{
	private NKMTrackingFloat m_BloomIntensity = new NKMTrackingFloat();

	private NKCUIDeckViewer m_NKCUIDeckView;

	public NKC_SCEN_TEAM()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_TEAM;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		m_NKCUIDeckView = NKCUIDeckViewer.Instance;
		m_NKCUIDeckView.Load(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData);
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		m_NKCUIDeckView.LoadComplete();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		Open(bDeckInit: true);
		NKCCamera.EnableBloom(bEnable: false);
		NKCCamera.GetTrackingPos().SetNowValue(0f, 0f, -1000f);
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		Close();
	}

	public void Open(bool bDeckInit)
	{
		NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
		{
			MenuName = NKCUtilString.GET_STRING_ORGANIZATION,
			eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.DeckSetupOnly,
			dOnSideMenuButtonConfirm = null,
			DeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_DAILY, 0),
			dOnBackButton = ChangeSceneToHome,
			SelectLeaderUnitOnOpen = false,
			bEnableDefaultBackground = true,
			bUpsideMenuHomeButton = false,
			StageBattleStrID = string.Empty
		};
		m_NKCUIDeckView.Open(options, bDeckInit);
		CheckTutorial();
	}

	private void ChangeSceneToHome()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
	}

	public void Close()
	{
		m_NKCUIDeckView.Close();
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
		if (!NKCCamera.IsTrackingCameraPos())
		{
			NKCCamera.TrackingPos(10f, NKMRandom.Range(-50f, 50f), NKMRandom.Range(-50f, 50f), NKMRandom.Range(-1000f, -900f));
		}
		m_BloomIntensity.Update(Time.deltaTime);
		if (!m_BloomIntensity.IsTracking())
		{
			m_BloomIntensity.SetTracking(NKMRandom.Range(1f, 2f), 4f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		NKCCamera.SetBloomIntensity(m_BloomIntensity.GetNowValue());
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void Close_UnitSelectList()
	{
		NKCUIUnitSelectList.CheckInstanceAndClose();
	}

	public void Close_UnitInfo()
	{
		NKCUIUnitInfo.CheckInstanceAndClose();
	}

	public void UI_TEAM_SHIP_CLICK()
	{
		m_NKCUIDeckView.DeckViewShipClick();
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Team);
	}
}
