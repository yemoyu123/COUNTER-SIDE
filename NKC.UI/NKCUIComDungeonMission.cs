using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComDungeonMission : MonoBehaviour
{
	public GameObject m_objMissionRoot;

	public GameObject m_objMission1;

	public GameObject m_objMission2;

	public GameObject m_objMission3;

	public GameObject m_MissionIcon1_BG;

	public GameObject m_MissionIcon2_BG;

	public GameObject m_MissionIcon3_BG;

	public GameObject m_MissionIcon1;

	public GameObject m_MissionIcon2;

	public GameObject m_MissionIcon3;

	public Text m_MissionText1;

	public Text m_MissionText2;

	public Text m_MissionText3;

	private Color successTextColor = new Color(1f, 1f, 1f, 1f);

	private Color failTextColor = new Color(0.4392157f, 41f / 85f, 44f / 85f, 1f);

	public void SetData(NKMWarfareTemplet cNKMWarfareTemplet, bool bTextOnly = false)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null || cNKMWarfareTemplet == null)
		{
			return;
		}
		WARFARE_GAME_MISSION_TYPE wFMissionType_ = cNKMWarfareTemplet.m_WFMissionType_1;
		WARFARE_GAME_MISSION_TYPE wFMissionType_2 = cNKMWarfareTemplet.m_WFMissionType_2;
		int wFMissionValue_ = cNKMWarfareTemplet.m_WFMissionValue_1;
		int wFMissionValue_2 = cNKMWarfareTemplet.m_WFMissionValue_2;
		NKCUtil.SetGameobjectActive(m_objMission1, bValue: true);
		NKCUtil.SetGameobjectActive(m_objMission2, wFMissionType_ != WARFARE_GAME_MISSION_TYPE.WFMT_NONE);
		NKCUtil.SetGameobjectActive(m_objMission3, wFMissionType_ != WARFARE_GAME_MISSION_TYPE.WFMT_NONE);
		NKCUtil.SetLabelText(m_MissionText1, NKCUtilString.GetWFMissionText(WARFARE_GAME_MISSION_TYPE.WFMT_CLEAR, 0));
		NKCUtil.SetLabelText(m_MissionText2, NKCUtilString.GetWFMissionText(wFMissionType_, wFMissionValue_));
		NKCUtil.SetLabelText(m_MissionText3, NKCUtilString.GetWFMissionText(wFMissionType_2, wFMissionValue_2));
		NKMWarfareClearData warfareClearData = myUserData.GetWarfareClearData(cNKMWarfareTemplet.m_WarfareID);
		if (warfareClearData != null)
		{
			if (m_MissionText1 != null)
			{
				m_MissionText1.color = successTextColor;
			}
			if (m_MissionText2 != null)
			{
				if (warfareClearData.m_mission_result_1)
				{
					m_MissionText2.color = successTextColor;
				}
				else
				{
					m_MissionText2.color = failTextColor;
				}
			}
			if (m_MissionText3 != null)
			{
				if (warfareClearData.m_mission_result_2)
				{
					m_MissionText3.color = successTextColor;
				}
				else
				{
					m_MissionText3.color = failTextColor;
				}
			}
			NKCUtil.SetGameobjectActive(m_MissionIcon1_BG, bValue: false);
			NKCUtil.SetGameobjectActive(m_MissionIcon2_BG, !warfareClearData.m_mission_result_1);
			NKCUtil.SetGameobjectActive(m_MissionIcon3_BG, !warfareClearData.m_mission_result_2);
			if (!bTextOnly)
			{
				if (m_MissionIcon1 != null && !m_MissionIcon1.activeSelf)
				{
					m_MissionIcon1.SetActive(value: true);
				}
				if (m_MissionIcon2 != null && m_MissionIcon2.activeSelf == !warfareClearData.m_mission_result_1)
				{
					m_MissionIcon2.SetActive(warfareClearData.m_mission_result_1);
				}
				if (m_MissionIcon3 != null && m_MissionIcon3.activeSelf == !warfareClearData.m_mission_result_2)
				{
					m_MissionIcon3.SetActive(warfareClearData.m_mission_result_2);
				}
			}
			else
			{
				m_MissionIcon1.SetActive(value: false);
				m_MissionIcon2.SetActive(value: false);
				m_MissionIcon3.SetActive(value: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_MissionIcon1_BG, bValue: true);
			NKCUtil.SetGameobjectActive(m_MissionIcon2_BG, bValue: true);
			NKCUtil.SetGameobjectActive(m_MissionIcon3_BG, bValue: true);
			if (m_MissionIcon1 != null && m_MissionIcon1.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_MissionIcon1, bValue: false);
			}
			if (m_MissionIcon2 != null && m_MissionIcon2.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_MissionIcon2, bValue: false);
			}
			if (m_MissionIcon3 != null && m_MissionIcon3.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_MissionIcon3, bValue: false);
			}
			if (m_MissionText1 != null)
			{
				NKCUtil.SetLabelTextColor(m_MissionText1, failTextColor);
			}
			if (m_MissionText2 != null)
			{
				NKCUtil.SetLabelTextColor(m_MissionText2, failTextColor);
			}
			if (m_MissionText3 != null)
			{
				NKCUtil.SetLabelTextColor(m_MissionText3, failTextColor);
			}
		}
	}

	public void SetData(NKMDungeonTempletBase cNKMDungeonTempletBase, bool bTextOnly = false)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null || cNKMDungeonTempletBase == null)
		{
			return;
		}
		NKMDungeonClearData dungeonClearData = myUserData.GetDungeonClearData(cNKMDungeonTempletBase.m_DungeonID);
		DUNGEON_GAME_MISSION_TYPE dGMissionType_ = cNKMDungeonTempletBase.m_DGMissionType_1;
		DUNGEON_GAME_MISSION_TYPE dGMissionType_2 = cNKMDungeonTempletBase.m_DGMissionType_2;
		int dGMissionValue_ = cNKMDungeonTempletBase.m_DGMissionValue_1;
		int dGMissionValue_2 = cNKMDungeonTempletBase.m_DGMissionValue_2;
		NKCUtil.SetGameobjectActive(m_objMission1, bValue: true);
		NKCUtil.SetGameobjectActive(m_objMission2, dGMissionType_ != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE);
		NKCUtil.SetGameobjectActive(m_objMission3, dGMissionType_ != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE);
		NKCUtil.SetLabelText(m_MissionText1, NKCUtilString.GetDGMissionText(DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR, 0));
		NKCUtil.SetLabelText(m_MissionText2, NKCUtilString.GetDGMissionText(dGMissionType_, dGMissionValue_));
		NKCUtil.SetLabelText(m_MissionText3, NKCUtilString.GetDGMissionText(dGMissionType_2, dGMissionValue_2));
		if (dungeonClearData != null)
		{
			NKCUtil.SetGameobjectActive(m_MissionIcon1_BG, bValue: false);
			NKCUtil.SetGameobjectActive(m_MissionIcon2_BG, !dungeonClearData.missionResult1);
			NKCUtil.SetGameobjectActive(m_MissionIcon3_BG, !dungeonClearData.missionResult2);
			if (!bTextOnly)
			{
				if (m_MissionIcon1 != null && !m_MissionIcon1.activeSelf)
				{
					m_MissionIcon1.SetActive(value: true);
				}
				if (m_MissionIcon2 != null && m_MissionIcon2.activeSelf == !dungeonClearData.missionResult1)
				{
					m_MissionIcon2.SetActive(dungeonClearData.missionResult1);
				}
				if (m_MissionIcon3 != null && m_MissionIcon3.activeSelf == !dungeonClearData.missionResult2)
				{
					m_MissionIcon3.SetActive(dungeonClearData.missionResult2);
				}
			}
			else
			{
				m_MissionIcon1.SetActive(value: false);
				m_MissionIcon2.SetActive(value: false);
				m_MissionIcon3.SetActive(value: false);
			}
			if (m_MissionText1 != null)
			{
				NKCUtil.SetLabelTextColor(m_MissionText1, successTextColor);
			}
			if (m_MissionText2 != null)
			{
				if (dungeonClearData.missionResult1)
				{
					NKCUtil.SetLabelTextColor(m_MissionText2, successTextColor);
				}
				else
				{
					NKCUtil.SetLabelTextColor(m_MissionText2, failTextColor);
				}
			}
			if (m_MissionText3 != null)
			{
				if (dungeonClearData.missionResult2)
				{
					NKCUtil.SetLabelTextColor(m_MissionText3, successTextColor);
				}
				else
				{
					NKCUtil.SetLabelTextColor(m_MissionText3, failTextColor);
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_MissionIcon1_BG, bValue: true);
			NKCUtil.SetGameobjectActive(m_MissionIcon2_BG, bValue: true);
			NKCUtil.SetGameobjectActive(m_MissionIcon3_BG, bValue: true);
			if (m_MissionIcon1 != null && m_MissionIcon1.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_MissionIcon1, bValue: false);
			}
			if (m_MissionIcon2 != null && m_MissionIcon2.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_MissionIcon2, bValue: false);
			}
			if (m_MissionIcon3 != null && m_MissionIcon3.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_MissionIcon3, bValue: false);
			}
			if (m_MissionText1 != null)
			{
				NKCUtil.SetLabelTextColor(m_MissionText1, failTextColor);
			}
			if (m_MissionText2 != null)
			{
				NKCUtil.SetLabelTextColor(m_MissionText2, failTextColor);
			}
			if (m_MissionText3 != null)
			{
				NKCUtil.SetLabelTextColor(m_MissionText3, failTextColor);
			}
		}
	}

	public void SetData(NKMPhaseTemplet phaseTemplet, bool bTextOnly = false)
	{
		if (NKCScenManager.GetScenManager().GetMyUserData() == null || phaseTemplet == null)
		{
			return;
		}
		DUNGEON_GAME_MISSION_TYPE dGMissionType_ = phaseTemplet.m_DGMissionType_1;
		DUNGEON_GAME_MISSION_TYPE dGMissionType_2 = phaseTemplet.m_DGMissionType_2;
		int dGMissionValue_ = phaseTemplet.m_DGMissionValue_1;
		int dGMissionValue_2 = phaseTemplet.m_DGMissionValue_2;
		NKCUtil.SetGameobjectActive(m_objMission1, bValue: true);
		NKCUtil.SetGameobjectActive(m_objMission2, dGMissionType_ != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE);
		NKCUtil.SetGameobjectActive(m_objMission3, dGMissionType_ != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE);
		NKCUtil.SetLabelText(m_MissionText1, NKCUtilString.GetDGMissionText(DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR, 0));
		NKCUtil.SetLabelText(m_MissionText2, NKCUtilString.GetDGMissionText(dGMissionType_, dGMissionValue_));
		NKCUtil.SetLabelText(m_MissionText3, NKCUtilString.GetDGMissionText(dGMissionType_2, dGMissionValue_2));
		NKMPhaseClearData phaseClearData = NKCPhaseManager.GetPhaseClearData(phaseTemplet);
		if (phaseClearData != null)
		{
			if (m_MissionText1 != null)
			{
				m_MissionText1.color = successTextColor;
			}
			if (m_MissionText2 != null)
			{
				if (phaseClearData.missionResult1)
				{
					m_MissionText2.color = successTextColor;
				}
				else
				{
					m_MissionText2.color = failTextColor;
				}
			}
			if (m_MissionText3 != null)
			{
				if (phaseClearData.missionResult2)
				{
					m_MissionText3.color = successTextColor;
				}
				else
				{
					m_MissionText3.color = failTextColor;
				}
			}
			NKCUtil.SetGameobjectActive(m_MissionIcon1_BG, bValue: false);
			NKCUtil.SetGameobjectActive(m_MissionIcon2_BG, !phaseClearData.missionResult1);
			NKCUtil.SetGameobjectActive(m_MissionIcon3_BG, !phaseClearData.missionResult2);
			if (!bTextOnly)
			{
				if (m_MissionIcon1 != null && !m_MissionIcon1.activeSelf)
				{
					m_MissionIcon1.SetActive(value: true);
				}
				if (m_MissionIcon2 != null && m_MissionIcon2.activeSelf == !phaseClearData.missionResult1)
				{
					m_MissionIcon2.SetActive(phaseClearData.missionResult1);
				}
				if (m_MissionIcon3 != null && m_MissionIcon3.activeSelf == !phaseClearData.missionResult2)
				{
					m_MissionIcon3.SetActive(phaseClearData.missionResult2);
				}
			}
			else
			{
				m_MissionIcon1.SetActive(value: false);
				m_MissionIcon2.SetActive(value: false);
				m_MissionIcon3.SetActive(value: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_MissionIcon1_BG, bValue: true);
			NKCUtil.SetGameobjectActive(m_MissionIcon2_BG, bValue: true);
			NKCUtil.SetGameobjectActive(m_MissionIcon3_BG, bValue: true);
			if (m_MissionIcon1 != null && m_MissionIcon1.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_MissionIcon1, bValue: false);
			}
			if (m_MissionIcon2 != null && m_MissionIcon2.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_MissionIcon2, bValue: false);
			}
			if (m_MissionIcon3 != null && m_MissionIcon3.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_MissionIcon3, bValue: false);
			}
			if (m_MissionText1 != null)
			{
				NKCUtil.SetLabelTextColor(m_MissionText1, failTextColor);
			}
			if (m_MissionText2 != null)
			{
				NKCUtil.SetLabelTextColor(m_MissionText2, failTextColor);
			}
			if (m_MissionText3 != null)
			{
				NKCUtil.SetLabelTextColor(m_MissionText3, failTextColor);
			}
		}
	}
}
