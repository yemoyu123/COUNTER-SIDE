using ClientPacket.Warfare;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKCGameHUDRepeatOperation : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnOn;

	public NKCUIComStateButton m_csbtnOff;

	public void InitUI()
	{
		m_csbtnOn.PointerClick.RemoveAllListeners();
		m_csbtnOn.PointerClick.AddListener(OnClick);
		m_csbtnOff.PointerClick.RemoveAllListeners();
		m_csbtnOff.PointerClick.AddListener(OnClick);
	}

	public void SetDisable()
	{
		DisableThisUI();
	}

	private void DisableThisUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void SetUI(NKMGameData cNKMGameData)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_REPEAT))
		{
			DisableThisUI();
			return;
		}
		switch (cNKMGameData.GetGameType())
		{
		case NKM_GAME_TYPE.NGT_WARFARE:
		{
			if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.WARFARE_REPEAT))
			{
				DisableThisUI();
				break;
			}
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(cNKMGameData.m_WarfareID);
			if (nKMWarfareTemplet == null)
			{
				DisableThisUI();
				break;
			}
			NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(nKMWarfareTemplet.m_WarfareStrID);
			if (nKMStageTempletV == null)
			{
				DisableThisUI();
				break;
			}
			if (nKMStageTempletV.m_bNoAutoRepeat)
			{
				DisableThisUI();
				break;
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			ResetBtnOnOffUI();
			break;
		}
		case NKM_GAME_TYPE.NGT_PHASE:
			if (!NKCRepeatOperaion.CheckVisible(NKCPhaseManager.GetStageTemplet()))
			{
				DisableThisUI();
				break;
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			ResetBtnOnOffUI();
			break;
		case NKM_GAME_TYPE.NGT_DUNGEON:
			if (!NKCRepeatOperaion.CheckVisible(NKMDungeonManager.GetDungeonTempletBase(cNKMGameData.m_DungeonID)?.StageTemplet))
			{
				DisableThisUI();
				break;
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			ResetBtnOnOffUI();
			break;
		default:
			DisableThisUI();
			break;
		}
	}

	public void ResetBtnOnOffUI()
	{
		SetBtnOnOff(NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing());
	}

	private void SetBtnOnOff(bool bOn)
	{
		NKCUtil.SetGameobjectActive(m_csbtnOn, bOn);
		NKCUtil.SetGameobjectActive(m_csbtnOff, !bOn);
	}

	private void OnClick()
	{
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient == null || gameClient.GetGameData() == null)
		{
			return;
		}
		NKMGameData gameData = gameClient.GetGameData();
		switch (gameData.GetGameType())
		{
		case NKM_GAME_TYPE.NGT_WARFARE:
		{
			if (!NKCRepeatOperaion.CheckPossibleForWarfare(NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareStrID()))
			{
				return;
			}
			if (NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetRetryData() == null)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_WARFARE_CANNOT_FIND_RETRY_DATA);
				return;
			}
			WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData != null && warfareGameData.rewardMultiply > 1)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_DOUBLE_OPERATION_CANNOT_REPEAT"));
				return;
			}
			break;
		}
		case NKM_GAME_TYPE.NGT_PHASE:
		{
			if (NKCPhaseManager.GetStageTemplet() == null)
			{
				return;
			}
			NKC_SCEN_DUNGEON_ATK_READY sCEN_DUNGEON_ATK_READY2 = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY();
			if (sCEN_DUNGEON_ATK_READY2 != null && sCEN_DUNGEON_ATK_READY2.GetLastMultiplyRewardCount() > 1)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_DOUBLE_OPERATION_CANNOT_REPEAT"));
				return;
			}
			if (!NKCRepeatOperaion.CheckPossible(NKCPhaseManager.PhaseModeState.stageId))
			{
				return;
			}
			break;
		}
		default:
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(gameData.m_DungeonID);
			if (dungeonTempletBase == null)
			{
				return;
			}
			NKC_SCEN_DUNGEON_ATK_READY sCEN_DUNGEON_ATK_READY = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY();
			if (sCEN_DUNGEON_ATK_READY != null && sCEN_DUNGEON_ATK_READY.GetLastMultiplyRewardCount() > 1)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_DOUBLE_OPERATION_CANNOT_REPEAT"));
				return;
			}
			if (!NKCRepeatOperaion.CheckPossible((dungeonTempletBase.StageTemplet != null) ? dungeonTempletBase.StageTemplet.Key : 0))
			{
				return;
			}
			break;
		}
		}
		gameClient.Send_Packet_GAME_PAUSE_REQ(bPause: true, bPauseEvent: false, NKC_OPEN_POPUP_TYPE_AFTER_PAUSE.NOPTATP_REPEAT_OPERATION_POPUP);
	}
}
