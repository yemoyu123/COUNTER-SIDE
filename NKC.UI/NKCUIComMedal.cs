using ClientPacket.Common;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComMedal : MonoBehaviour
{
	[Header("메달 이미지")]
	public Sprite m_sprMissionOn;

	public Sprite m_sprMissionOff;

	[Header("미션 있을 때 켜지는 최상위 오브젝트")]
	public GameObject m_objMissionRoot;

	[Header("미션이 있을 경우 켜지는 오브젝트들")]
	public GameObject m_objMission1;

	public GameObject m_objMission2;

	public GameObject m_objMission3;

	[Header("미션 메달 아이콘")]
	public Image m_MissionIcon1;

	public Image m_MissionIcon2;

	public Image m_MissionIcon3;

	[Header("미션 내용")]
	public TMP_Text m_MissionText1;

	public TMP_Text m_MissionText2;

	public TMP_Text m_MissionText3;

	[Header("미션 완료/미완료 텍스트 컬러")]
	public Color successTextColor = new Color(1f, 1f, 1f, 1f);

	public Color failTextColor = new Color(0.4392157f, 41f / 85f, 44f / 85f, 1f);

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
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (dungeonClearData != null)
		{
			flag = true;
			flag2 |= dungeonClearData.missionResult1;
			flag3 |= dungeonClearData.missionResult2;
		}
		if (m_MissionIcon1 != null)
		{
			if (flag)
			{
				NKCUtil.SetImageSprite(m_MissionIcon1, m_sprMissionOn);
			}
			else
			{
				NKCUtil.SetImageSprite(m_MissionIcon1, m_sprMissionOff);
			}
		}
		if (m_MissionIcon2 != null)
		{
			if (flag2)
			{
				NKCUtil.SetImageSprite(m_MissionIcon2, m_sprMissionOn);
			}
			else
			{
				NKCUtil.SetImageSprite(m_MissionIcon2, m_sprMissionOff);
			}
		}
		if (m_MissionIcon3 != null)
		{
			if (flag3)
			{
				NKCUtil.SetImageSprite(m_MissionIcon3, m_sprMissionOn);
			}
			else
			{
				NKCUtil.SetImageSprite(m_MissionIcon3, m_sprMissionOff);
			}
		}
		if (m_MissionText1 != null)
		{
			if (flag)
			{
				NKCUtil.SetLabelTextColor(m_MissionText1, successTextColor);
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_MissionText1, failTextColor);
			}
		}
		if (m_MissionText2 != null)
		{
			if (flag2)
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
			if (flag3)
			{
				NKCUtil.SetLabelTextColor(m_MissionText3, successTextColor);
			}
			else
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
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (phaseClearData != null)
		{
			flag = true;
			flag2 |= phaseClearData.missionResult1;
			flag3 |= phaseClearData.missionResult2;
		}
		if (m_MissionIcon1 != null)
		{
			if (flag)
			{
				NKCUtil.SetImageSprite(m_MissionIcon1, m_sprMissionOn);
			}
			else
			{
				NKCUtil.SetImageSprite(m_MissionIcon1, m_sprMissionOff);
			}
		}
		if (m_MissionIcon2 != null)
		{
			if (flag2)
			{
				NKCUtil.SetImageSprite(m_MissionIcon2, m_sprMissionOn);
			}
			else
			{
				NKCUtil.SetImageSprite(m_MissionIcon2, m_sprMissionOff);
			}
		}
		if (m_MissionIcon3 != null)
		{
			if (flag3)
			{
				NKCUtil.SetImageSprite(m_MissionIcon3, m_sprMissionOn);
			}
			else
			{
				NKCUtil.SetImageSprite(m_MissionIcon3, m_sprMissionOff);
			}
		}
		if (m_MissionText1 != null)
		{
			if (flag)
			{
				NKCUtil.SetLabelTextColor(m_MissionText1, successTextColor);
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_MissionText1, failTextColor);
			}
		}
		if (m_MissionText2 != null)
		{
			if (flag2)
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
			if (flag3)
			{
				NKCUtil.SetLabelTextColor(m_MissionText3, successTextColor);
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_MissionText3, failTextColor);
			}
		}
	}

	public void SetData(NKMDefenceTemplet defenceTemplet, bool bTextOnly = false)
	{
		if (NKCScenManager.GetScenManager().GetMyUserData() == null || defenceTemplet == null)
		{
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(defenceTemplet.m_DungeonID);
		if (dungeonTempletBase == null)
		{
			return;
		}
		DUNGEON_GAME_MISSION_TYPE dGMissionType_ = dungeonTempletBase.m_DGMissionType_1;
		DUNGEON_GAME_MISSION_TYPE dGMissionType_2 = dungeonTempletBase.m_DGMissionType_2;
		int dGMissionValue_ = dungeonTempletBase.m_DGMissionValue_1;
		int dGMissionValue_2 = dungeonTempletBase.m_DGMissionValue_2;
		NKCUtil.SetGameobjectActive(m_objMission1, bValue: true);
		NKCUtil.SetGameobjectActive(m_objMission2, dGMissionType_ != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE);
		NKCUtil.SetGameobjectActive(m_objMission3, dGMissionType_ != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE);
		NKCUtil.SetLabelText(m_MissionText1, NKCUtilString.GetDGMissionText(DUNGEON_GAME_MISSION_TYPE.DGMT_TEAM_A_KILL_COUNT, defenceTemplet.m_ClearScore));
		NKCUtil.SetLabelText(m_MissionText2, NKCUtilString.GetDGMissionText(dGMissionType_, dGMissionValue_));
		NKCUtil.SetLabelText(m_MissionText3, NKCUtilString.GetDGMissionText(dGMissionType_2, dGMissionValue_2));
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (NKCDefenceDungeonManager.m_DefenceTempletId == defenceTemplet.Key)
		{
			flag = NKCDefenceDungeonManager.m_BestClearScore >= defenceTemplet.m_ClearScore;
			flag2 |= NKCDefenceDungeonManager.m_bMissionResult1;
			flag3 |= NKCDefenceDungeonManager.m_bMissionResult2;
		}
		if (m_MissionIcon1 != null)
		{
			if (flag)
			{
				NKCUtil.SetImageSprite(m_MissionIcon1, m_sprMissionOn);
			}
			else
			{
				NKCUtil.SetImageSprite(m_MissionIcon1, m_sprMissionOff);
			}
		}
		if (m_MissionIcon2 != null)
		{
			if (flag2)
			{
				NKCUtil.SetImageSprite(m_MissionIcon2, m_sprMissionOn);
			}
			else
			{
				NKCUtil.SetImageSprite(m_MissionIcon2, m_sprMissionOff);
			}
		}
		if (m_MissionIcon3 != null)
		{
			if (flag3)
			{
				NKCUtil.SetImageSprite(m_MissionIcon3, m_sprMissionOn);
			}
			else
			{
				NKCUtil.SetImageSprite(m_MissionIcon3, m_sprMissionOff);
			}
		}
		if (m_MissionText1 != null)
		{
			if (flag)
			{
				NKCUtil.SetLabelTextColor(m_MissionText1, successTextColor);
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_MissionText1, failTextColor);
			}
		}
		if (m_MissionText2 != null)
		{
			if (flag2)
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
			if (flag3)
			{
				NKCUtil.SetLabelTextColor(m_MissionText3, successTextColor);
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_MissionText3, failTextColor);
			}
		}
	}
}
