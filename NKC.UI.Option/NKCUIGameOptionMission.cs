using System.Text;
using ClientPacket.Warfare;
using Cs.Core.Util;
using NKC.Trim;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCUIGameOptionMission : NKCUIGameOptionContentBase
{
	private enum MissionContentGroup
	{
		None = -1,
		Warfare,
		Mission,
		Dive,
		Max
	}

	private struct MissionData
	{
		public int m_StageID;

		public string m_MissionName;

		public string m_MissionCondition1Text;

		public string m_MissionCondition2Text;

		public string m_MissionCondition3Text;
	}

	public GameObject NKM_UI_GAME_OPTION_MISSION_TEXT;

	public Text m_NKM_UI_GAME_OPTION_EPISODE_TEXT_TITLE;

	public Text m_NKM_UI_GAME_OPTION_EPISODE_TEXT_TITLE2;

	public Text m_NKM_UI_GAME_OPTION_EPISODE_TEXT_NAME;

	public Text m_NKM_UI_GAME_OPTION_EPISODE_TEXT_DESC;

	public Text m_NKM_UI_GAME_OPTION_MISSION_TEXT_TITLE;

	public Text m_NKM_UI_GAME_OPTION_MISSION_TEXT_CONDITION1;

	public Text m_NKM_UI_GAME_OPTION_MISSION_TEXT_CONDITION2;

	public Text m_NKM_UI_GAME_OPTION_MISSION_TEXT_CONDITION3;

	public NKCUIComStateButton m_csbtnClassInfoHelp;

	private NKCUIGameOptionMissionContentBase[] m_MissionConditions = new NKCUIGameOptionMissionContentBase[3];

	public NKCUIGameOptionMissionContentBase NKM_UI_GAME_OPTION_MISSION_WARFARE;

	public NKCUIGameOptionMissionContentBase NKM_UI_GAME_OPTION_MISSION_MISSION;

	public NKCUIGameOptionMissionContentBase NKM_UI_GAME_OPTION_MISSION_DIVE;

	private string MISSION_MEDAL_CONDITION_STRING => NKCUtilString.GET_STRING_OPTION_MEDAL_COND;

	private string MISSION_RANK_CONDITION_STRING => NKCUtilString.GET_STRING_OPTION_RANK_COND;

	private string MISSION_GIVE_UP_TITLE_STRING => NKCUtilString.GET_STRING_WARNING;

	private string MISSION_GIVE_UP_CONTENT_STRING => NKCUtilString.GET_STRING_OPTION_MISSION_GIVE_UP_WARNING;

	private string MISSION_GIVE_UP_MULTIPLY_CONTENT_STRING => NKCUtilString.GET_STRING_OPTION_MISSION_GIVE_UP_WARNING_MULTIPLY;

	public override void Init()
	{
		m_MissionConditions[0] = NKM_UI_GAME_OPTION_MISSION_WARFARE;
		m_MissionConditions[0].m_GiveUpButton.PointerClick.AddListener(OnClickWarfareGiveUpButton);
		m_MissionConditions[0].m_LeaveButton.PointerClick.AddListener(OnClickWarfareLeaveButton);
		m_MissionConditions[1] = NKM_UI_GAME_OPTION_MISSION_MISSION;
		m_MissionConditions[1].m_GiveUpButton.PointerClick.AddListener(OnClickDungeonGiveUpButton);
		m_MissionConditions[2] = NKM_UI_GAME_OPTION_MISSION_DIVE;
		m_MissionConditions[2].m_GiveUpButton.PointerClick.AddListener(OnClickDiveGiveUpButton);
		m_MissionConditions[2].m_LeaveButton.PointerClick.AddListener(OnClickDiveLeaveButton);
		m_csbtnClassInfoHelp.PointerClick.AddListener(OnClassInfoHelp);
	}

	public override void SetContent()
	{
		NKC_GAME_OPTION_MENU_TYPE menuType = NKCUIGameOption.Instance.GetMenuType();
		MissionContentGroup missionContentGroupByMenuType = GetMissionContentGroupByMenuType(menuType);
		ChangeMissionContent(missionContentGroupByMenuType);
	}

	private MissionContentGroup GetMissionContentGroupByMenuType(NKC_GAME_OPTION_MENU_TYPE menuType)
	{
		MissionContentGroup result = MissionContentGroup.None;
		switch (menuType)
		{
		case NKC_GAME_OPTION_MENU_TYPE.WARFARE:
			result = MissionContentGroup.Warfare;
			break;
		case NKC_GAME_OPTION_MENU_TYPE.DUNGEON:
			result = MissionContentGroup.Mission;
			break;
		case NKC_GAME_OPTION_MENU_TYPE.DIVE:
			result = MissionContentGroup.Dive;
			break;
		}
		return result;
	}

	private MissionData GetMissionDataByMissionContentGroup(MissionContentGroup contentGroup)
	{
		MissionData result = new MissionData
		{
			m_StageID = 0
		};
		switch (contentGroup)
		{
		case MissionContentGroup.Warfare:
		{
			if (NKCScenManager.GetScenManager().GetMyUserData() == null)
			{
				break;
			}
			WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData != null)
			{
				NKMWarfareTemplet nKMWarfareTemplet2 = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
				if (nKMWarfareTemplet2 != null)
				{
					result.m_StageID = ((nKMWarfareTemplet2.StageTemplet != null) ? nKMWarfareTemplet2.StageTemplet.Key : 0);
					result.m_MissionName = nKMWarfareTemplet2.GetWarfareName();
					result.m_MissionCondition3Text = NKCUtilString.GetWFMissionTextWithProgress(warfareGameData, WARFARE_GAME_MISSION_TYPE.WFMT_CLEAR, 1);
					result.m_MissionCondition2Text = NKCUtilString.GetWFMissionTextWithProgress(warfareGameData, nKMWarfareTemplet2.m_WFMissionType_1, nKMWarfareTemplet2.m_WFMissionValue_1);
					result.m_MissionCondition1Text = NKCUtilString.GetWFMissionTextWithProgress(warfareGameData, nKMWarfareTemplet2.m_WFMissionType_2, nKMWarfareTemplet2.m_WFMissionValue_2);
				}
			}
			break;
		}
		case MissionContentGroup.Mission:
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (gameClient == null)
			{
				break;
			}
			NKMGameData gameData = gameClient.GetGameData();
			if (gameData == null)
			{
				break;
			}
			if (NKCPhaseManager.IsCurrentPhaseDungeon(gameData.m_DungeonID))
			{
				NKMPhaseTemplet phaseTemplet = NKCPhaseManager.GetPhaseTemplet();
				if (phaseTemplet != null)
				{
					result.m_StageID = ((phaseTemplet.StageTemplet != null) ? phaseTemplet.StageTemplet.Key : 0);
					result.m_MissionName = phaseTemplet.GetName();
					result.m_MissionCondition1Text = NKCUtilString.GetDGMissionTextWithProgress(gameClient, DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR, 1);
					result.m_MissionCondition2Text = NKCUtilString.GetDGMissionTextWithProgress(gameClient, phaseTemplet.m_DGMissionType_1, phaseTemplet.m_DGMissionValue_1);
					result.m_MissionCondition3Text = NKCUtilString.GetDGMissionTextWithProgress(gameClient, phaseTemplet.m_DGMissionType_2, phaseTemplet.m_DGMissionValue_2);
				}
				break;
			}
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(gameData.m_DungeonID);
			if (dungeonTempletBase == null)
			{
				break;
			}
			bool flag = false;
			if (gameData.m_WarfareID > 0)
			{
				NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(gameData.m_WarfareID);
				if (nKMWarfareTemplet != null)
				{
					result.m_StageID = ((nKMWarfareTemplet.StageTemplet != null) ? nKMWarfareTemplet.StageTemplet.Key : 0);
					result.m_MissionName = nKMWarfareTemplet.GetWarfareName();
					flag = true;
				}
			}
			if (!flag)
			{
				result.m_StageID = ((dungeonTempletBase.StageTemplet != null) ? dungeonTempletBase.StageTemplet.Key : 0);
				result.m_MissionName = dungeonTempletBase.GetDungeonName();
			}
			if (gameData.GetGameType() == NKM_GAME_TYPE.NGT_PVE_DEFENCE)
			{
				if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVE_DEFENCE_MEDAL_REWARD))
				{
					NKMDefenceTemplet currentDefenceDungeonTemplet = NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Now);
					if (currentDefenceDungeonTemplet != null && currentDefenceDungeonTemplet.m_ClearScore > 0)
					{
						result.m_MissionCondition1Text = NKCUtilString.GetDGMissionTextWithProgress(gameClient, DUNGEON_GAME_MISSION_TYPE.DGMT_TEAM_A_KILL_COUNT, currentDefenceDungeonTemplet.m_ClearScore);
					}
					else
					{
						result.m_MissionCondition1Text = NKCUtilString.GetDGMissionTextWithProgress(gameClient, DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR, 1);
					}
				}
			}
			else
			{
				result.m_MissionCondition1Text = NKCUtilString.GetDGMissionTextWithProgress(gameClient, DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR, 1);
			}
			result.m_MissionCondition2Text = NKCUtilString.GetDGMissionTextWithProgress(gameClient, dungeonTempletBase.m_DGMissionType_1, dungeonTempletBase.m_DGMissionValue_1);
			result.m_MissionCondition3Text = NKCUtilString.GetDGMissionTextWithProgress(gameClient, dungeonTempletBase.m_DGMissionType_2, dungeonTempletBase.m_DGMissionValue_2);
			break;
		}
		}
		return result;
	}

	private void ChangeMissionContent(MissionContentGroup contentGroup)
	{
		MissionData missionData = default(MissionData);
		NKMStageTempletV2 nKMStageTempletV = null;
		NKMEpisodeTempletV2 nKMEpisodeTempletV = null;
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		if (contentGroup != MissionContentGroup.Dive)
		{
			bool flag = false;
			missionData = GetMissionDataByMissionContentGroup(contentGroup);
			if (contentGroup >= MissionContentGroup.Warfare && contentGroup <= MissionContentGroup.Mission)
			{
				flag = true;
			}
			if (!flag)
			{
				SetDeactiveMissionContentWithoutButton(contentGroup);
				return;
			}
		}
		SetActiveMissionContent(active: true, contentGroup);
		if (contentGroup == MissionContentGroup.Dive)
		{
			NKMDiveGameData diveGameData = NKCScenManager.GetScenManager().GetMyUserData().m_DiveGameData;
			if (diveGameData != null && diveGameData.Floor.Templet != null)
			{
				m_MissionConditions[2].m_LV_COUNT.text = diveGameData.Floor.Templet.StageLevel.ToString();
				m_MissionConditions[2].m_RANDOM_SET_COUNT.text = diveGameData.Floor.Templet.RandomSetCount.ToString();
				m_MissionConditions[2].m_NAME_TEXT.text = diveGameData.Floor.Templet.Get_STAGE_NAME();
				m_MissionConditions[2].m_NAME_SUB_TEXT.text = diveGameData.Floor.Templet.Get_STAGE_NAME_SUB();
			}
		}
		else
		{
			text3 = missionData.m_MissionName;
			nKMStageTempletV = NKMStageTempletV2.Find(missionData.m_StageID);
			if (nKMStageTempletV != null)
			{
				nKMEpisodeTempletV = nKMStageTempletV.EpisodeTemplet;
				if (nKMEpisodeTempletV != null)
				{
					text = NKCUtilString.GetEpisodeTitle(nKMEpisodeTempletV, nKMStageTempletV);
					text2 = NKCUtilString.GetEpisodeNumber(nKMEpisodeTempletV, nKMStageTempletV);
					text4 = nKMStageTempletV.GetStageDesc();
				}
			}
			if (NKCTrimManager.TrimModeState != null)
			{
				NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(NKCTrimManager.TrimModeState.trimId);
				string value = ((nKMTrimTemplet != null) ? NKCStringTable.GetString(nKMTrimTemplet.TirmGroupName) : " - ");
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(value);
				stringBuilder.Append(" ");
				stringBuilder.Append(NKCStringTable.GetString("SI_PF_TRIM_MAIN_LEVEL_TEXT"));
				stringBuilder.Append(NKCTrimManager.TrimModeState.trimLevel);
				text3 = stringBuilder.ToString();
				text4 = ((nKMTrimTemplet != null) ? NKCStringTable.GetString(nKMTrimTemplet.TirmGroupDesc) : "");
			}
			m_NKM_UI_GAME_OPTION_EPISODE_TEXT_TITLE.text = text;
			m_NKM_UI_GAME_OPTION_EPISODE_TEXT_TITLE2.text = text2;
			m_NKM_UI_GAME_OPTION_EPISODE_TEXT_NAME.text = text3;
			m_NKM_UI_GAME_OPTION_EPISODE_TEXT_DESC.text = text4;
			string text5 = "";
			switch (contentGroup)
			{
			case MissionContentGroup.Warfare:
				text5 = MISSION_MEDAL_CONDITION_STRING;
				break;
			case MissionContentGroup.Mission:
				text5 = MISSION_RANK_CONDITION_STRING;
				break;
			}
			m_NKM_UI_GAME_OPTION_MISSION_TEXT_TITLE.text = text5;
			m_NKM_UI_GAME_OPTION_MISSION_TEXT_CONDITION1.text = missionData.m_MissionCondition1Text;
			m_NKM_UI_GAME_OPTION_MISSION_TEXT_CONDITION2.text = missionData.m_MissionCondition2Text;
			m_NKM_UI_GAME_OPTION_MISSION_TEXT_CONDITION3.text = missionData.m_MissionCondition3Text;
			NKCUtil.SetGameobjectActive(m_MissionConditions[(int)contentGroup].m_ConditionImage_1, !string.IsNullOrEmpty(missionData.m_MissionCondition1Text));
			NKCUtil.SetGameobjectActive(m_MissionConditions[(int)contentGroup].m_ConditionImage_2, !string.IsNullOrEmpty(missionData.m_MissionCondition2Text));
			NKCUtil.SetGameobjectActive(m_MissionConditions[(int)contentGroup].m_ConditionImage_3, !string.IsNullOrEmpty(missionData.m_MissionCondition3Text));
		}
		if (contentGroup != MissionContentGroup.Mission)
		{
			return;
		}
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient != null)
		{
			NKMGameData gameData = gameClient.GetGameData();
			if (gameData != null)
			{
				NKCUtil.SetGameobjectActive(m_MissionConditions[1].m_GiveUpButton, NKCTutorialManager.CanGiveupDungeon(gameData.m_DungeonID));
			}
		}
	}

	private void SetActiveMissionContent(bool active, MissionContentGroup contentGroup = MissionContentGroup.None)
	{
		if (contentGroup == MissionContentGroup.Dive)
		{
			NKCUtil.SetGameobjectActive(NKM_UI_GAME_OPTION_MISSION_TEXT, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(NKM_UI_GAME_OPTION_MISSION_TEXT, active);
		}
		bool flag = active;
		for (int i = 0; i < 3; i++)
		{
			flag = ((i != (int)contentGroup) ? (!active) : active);
			NKCUtil.SetGameobjectActive(m_MissionConditions[i], flag);
			NKCUtil.SetGameobjectActive(m_MissionConditions[i].m_Condition, flag);
			NKCUtil.SetGameobjectActive(m_MissionConditions[i].m_Button, flag);
		}
	}

	private void SetDeactiveMissionContentWithoutButton(MissionContentGroup contentGroup)
	{
		NKCUtil.SetGameobjectActive(NKM_UI_GAME_OPTION_MISSION_TEXT, bValue: false);
		for (int i = 0; i < 3; i++)
		{
			NKCUtil.SetGameobjectActive(m_MissionConditions[i], bValue: true);
			NKCUtil.SetGameobjectActive(m_MissionConditions[i].m_Condition, bValue: false);
			if (i == (int)contentGroup)
			{
				NKCUtil.SetGameobjectActive(m_MissionConditions[i].m_Button, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_MissionConditions[i].m_Button, bValue: false);
			}
		}
	}

	private void OnClickDungeonGiveUpButton()
	{
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient == null)
		{
			return;
		}
		string content;
		switch (gameClient.GetGameData().GetGameType())
		{
		case NKM_GAME_TYPE.NGT_PRACTICE:
			gameClient.GetGameHud().PracticeGoBack();
			return;
		case NKM_GAME_TYPE.NGT_FIERCE:
			content = NKCUtilString.GET_FIERCE_BATTLE_GIVE_UP_DESC;
			break;
		case NKM_GAME_TYPE.NGT_TRIM:
		{
			NKMTrimIntervalTemplet nKMTrimIntervalTemplet = NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime);
			bool flag = false;
			if (nKMTrimIntervalTemplet != null)
			{
				flag = NKCSynchronizedTime.IsStarted(nKMTrimIntervalTemplet.IntervalTemplet.StartDate) && !NKCSynchronizedTime.IsFinished(nKMTrimIntervalTemplet.IntervalTemplet.EndDate);
			}
			content = ((!flag || nKMTrimIntervalTemplet == null || nKMTrimIntervalTemplet.IsResetUnLimit) ? NKCUtilString.GET_STRING_OPTION_MISSION_GIVE_UP_WARNING : ((NKCScenManager.CurrentUserData().TrimData.TrimIntervalData.trimRetryCount <= 0) ? NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_RESULT_RESET_NO_COUNT_EXIT") : NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_RESULT_RESET_COUNT_EXIT", NKCScenManager.CurrentUserData().TrimData.TrimIntervalData.trimRetryCount)));
			break;
		}
		case NKM_GAME_TYPE.NGT_PVE_DEFENCE:
			content = NKCStringTable.GetString("SI_PF_DEFENCE_DUNGEON_EXIT_POPUP_TEXT");
			break;
		default:
			if (gameClient.GetGameData().GetGameType() != NKM_GAME_TYPE.NGT_WARFARE && NKCScenManager.GetScenManager().GetNKCRepeatOperaion().CheckRepeatOperationRealStop())
			{
				return;
			}
			content = ((gameClient.MultiplyReward <= 1) ? MISSION_GIVE_UP_CONTENT_STRING : MISSION_GIVE_UP_MULTIPLY_CONTENT_STRING);
			break;
		}
		NKCPopupOKCancel.OpenOKCancelBox(MISSION_GIVE_UP_TITLE_STRING, content, OnClickDungeonGiveUpOKButton);
	}

	private static void OnClickDungeonGiveUpOKButton()
	{
		NKCPacketSender.Send_NKMPacket_GAME_GIVEUP_REQ();
	}

	private void OnClickWarfareGiveUpButton()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME()?.TryGiveUp();
	}

	private void OnClickWarfareLeaveButton()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME()?.TryTempLeave();
	}

	private void OnClickDiveGiveUpButton()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE()?.TryGiveUp();
	}

	private void OnClickDiveLeaveButton()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE()?.TryTempLeave();
	}

	private void OnClassInfoHelp()
	{
		NKCUIPopupTutorialImagePanel.Instance.Open("GUIDE_BATTLE_UNIT_2", null);
	}
}
