using NKC.UI;
using NKM;

namespace NKC;

public class NKCUIGameToastMSG
{
	private NKMGameData m_NKMGameData;

	private NKMGameRuntimeData m_NKMGameRuntimeData;

	private NKMDungeonTempletBase m_NKMDungeonTempletBase;

	private int m_GameTime;

	private int m_Cost;

	private float m_fShipHP;

	private float m_fShipMaxHP;

	public void Reset(NKMGameData cNKMGameData, NKMGameRuntimeData cNKMGameRuntimeData)
	{
		m_NKMGameData = null;
		m_NKMGameRuntimeData = null;
		m_NKMDungeonTempletBase = null;
		m_GameTime = (int)cNKMGameRuntimeData.GetGamePlayTime();
		m_Cost = 0;
		m_fShipHP = 0f;
		m_fShipMaxHP = 0f;
		if (cNKMGameData == null || !cNKMGameData.IsPVE())
		{
			return;
		}
		m_NKMDungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(cNKMGameData.m_DungeonID);
		if (m_NKMDungeonTempletBase == null)
		{
			return;
		}
		m_NKMGameData = cNKMGameData;
		m_NKMGameRuntimeData = cNKMGameRuntimeData;
		m_Cost = (int)cNKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fUsedRespawnCost;
		m_fShipHP = 0f;
		m_fShipMaxHP = 0f;
		if (cNKMGameData.m_NKMGameTeamDataA.m_MainShip == null)
		{
			return;
		}
		for (int i = 0; i < cNKMGameData.m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID.Count; i++)
		{
			short gameUnitUID = cNKMGameData.m_NKMGameTeamDataA.m_MainShip.m_listGameUnitUID[i];
			NKMUnit unit = NKCScenManager.GetScenManager().GetGameClient().GetUnit(gameUnitUID);
			if (unit != null)
			{
				m_fShipHP += unit.GetUnitSyncData().GetHP();
				m_fShipMaxHP += unit.GetUnitFrameData().m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP);
			}
		}
	}

	public void SetCost(int cost)
	{
		m_Cost = cost;
	}

	public void Invalid()
	{
		m_NKMGameData = null;
		m_NKMGameRuntimeData = null;
		m_NKMDungeonTempletBase = null;
	}

	public void Update()
	{
		if (NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_START && m_NKMGameData != null && m_NKMGameRuntimeData != null && m_NKMDungeonTempletBase != null && m_NKMGameData.GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE)
		{
			ProcessMission(m_NKMDungeonTempletBase.m_DGMissionType_1, m_NKMDungeonTempletBase.m_DGMissionValue_1);
			ProcessMission(m_NKMDungeonTempletBase.m_DGMissionType_2, m_NKMDungeonTempletBase.m_DGMissionValue_2);
		}
	}

	private void ProcessMission(DUNGEON_GAME_MISSION_TYPE missionType, int value)
	{
		switch (missionType)
		{
		case DUNGEON_GAME_MISSION_TYPE.DGMT_COST:
		{
			int num2 = (int)m_NKMGameRuntimeData.m_NKMGameRuntimeTeamDataA.m_fUsedRespawnCost;
			if (num2 <= m_Cost)
			{
				break;
			}
			if (m_Cost < value && num2 >= value)
			{
				if (NKCScenManager.GetScenManager().GetGameClient().IsShowUI())
				{
					NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_DUNGEON_MISSION_COST_FAIL_ONE_PARAM, value), NKCPopupMessage.eMessagePosition.TopIngame);
				}
			}
			else if (m_Cost < (int)((float)value * 0.75f) && num2 >= (int)((float)value * 0.75f))
			{
				NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_DUNGEON_MISSION_COST_WARNING_THREE_PARAM, value, num2, value), NKCPopupMessage.eMessagePosition.TopIngame);
			}
			else if (m_Cost < value / 2 && num2 >= value / 2 && NKCScenManager.GetScenManager().GetGameClient().IsShowUI())
			{
				NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_DUNGEON_MISSION_COST_THREE_PARAM, value, num2, value), NKCPopupMessage.eMessagePosition.TopIngame);
			}
			m_Cost = num2;
			break;
		}
		case DUNGEON_GAME_MISSION_TYPE.DGMT_TIME:
		{
			int num = (int)m_NKMGameRuntimeData.GetGamePlayTime();
			if (num <= m_GameTime)
			{
				break;
			}
			if (m_GameTime < value && num >= value)
			{
				if (NKCScenManager.GetScenManager().GetGameClient().IsShowUI())
				{
					NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_DUNGEON_MISSION_TIME_FAIL_ONE_PARAM, value), NKCPopupMessage.eMessagePosition.TopIngame);
				}
			}
			else if (m_GameTime < (int)((float)value * 0.75f) && num >= (int)((float)value * 0.75f))
			{
				if (NKCScenManager.GetScenManager().GetGameClient().IsShowUI())
				{
					NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_DUNGEON_MISSION_TIME_WARNING_THREE_PARAM, value, num, value), NKCPopupMessage.eMessagePosition.TopIngame);
				}
			}
			else if (m_GameTime < value / 2 && num >= value / 2 && NKCScenManager.GetScenManager().GetGameClient().IsShowUI())
			{
				NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_DUNGEON_MISSION_TIME_THREE_PARAM, value, num, value), NKCPopupMessage.eMessagePosition.TopIngame);
			}
			m_GameTime = num;
			break;
		}
		default:
			_ = 5;
			break;
		}
	}
}
