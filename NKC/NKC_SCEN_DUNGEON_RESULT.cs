using NKC.UI.Result;
using NKM;

namespace NKC;

public class NKC_SCEN_DUNGEON_RESULT : NKC_SCEN_BASIC
{
	private NKCUIResult.BattleResultData m_battleResultData;

	private long m_leaderUnitUID;

	private long m_leaderShipUID;

	private int m_dummyLeaderID;

	private int m_dummyLeaderSkinID;

	public NKC_SCEN_DUNGEON_RESULT()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_DUNGEON_RESULT;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		_ = NKCUIWarfareResult.Instance;
	}

	public void SetDummyLeader(int unitID, int skinID)
	{
		m_dummyLeaderID = unitID;
		m_dummyLeaderSkinID = skinID;
	}

	public void SetData(NKCUIResult.BattleResultData battleResultData, long leaderUnitUID, long leaderShipUID)
	{
		m_battleResultData = battleResultData;
		m_leaderUnitUID = leaderUnitUID;
		m_leaderShipUID = leaderShipUID;
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCUIWarfareResult.Instance.OpenForDungeon(m_battleResultData, m_leaderUnitUID, m_leaderShipUID, m_dummyLeaderID, m_dummyLeaderSkinID, OnCloseResultUI);
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		NKCUIWarfareResult.CheckInstanceAndClose();
		m_battleResultData = null;
		m_leaderUnitUID = 0L;
		m_leaderShipUID = 0L;
		m_dummyLeaderID = 0;
		m_dummyLeaderSkinID = 0;
	}

	private void OnCloseResultUI(NKM_SCEN_ID scenID)
	{
		NKCScenManager.GetScenManager().ScenChangeFade(scenID);
	}
}
