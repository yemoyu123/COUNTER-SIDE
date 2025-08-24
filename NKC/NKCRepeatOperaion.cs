using System;
using NKC.UI;
using NKC.UI.Warfare;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKCRepeatOperaion
{
	private bool m_bIsOnGoing;

	private long m_MaxRepeatCount;

	private long m_CurrRepeatCount;

	private long m_CostIncreaseCount;

	private long m_PrevCostIncreaseCount;

	private NKMRewardData m_NKMRewardDataPrev = new NKMRewardData();

	private NKMRewardData m_NKMRewardData = new NKMRewardData();

	private bool m_bAlarmRepeatOperationQuitByDefeat;

	private bool m_bAlarmRepeatOperationSuccess;

	private bool m_bNeedToSavePrevData;

	private DateTime m_StartDateTime;

	private TimeSpan m_tsPrevProgressDuration = new TimeSpan(0L);

	private int m_PrevCostItemID;

	private int m_PrevCostItemCount;

	private long m_PrevRepeatCount;

	private string m_PrevEPTitle = "";

	private string m_PrevEPName = "";

	private string m_StopReason = "";

	public void SetIsOnGoing(bool bSet)
	{
		m_bIsOnGoing = bSet;
	}

	public bool GetIsOnGoing()
	{
		return m_bIsOnGoing;
	}

	public long GetMaxRepeatCount()
	{
		return m_MaxRepeatCount;
	}

	public void SetMaxRepeatCount(long count)
	{
		m_MaxRepeatCount = count;
	}

	public long GetCurrRepeatCount()
	{
		return m_CurrRepeatCount;
	}

	public void SetCurrRepeatCount(long count)
	{
		m_CurrRepeatCount = count;
	}

	public long GetCostIncreaseCount()
	{
		return m_CostIncreaseCount;
	}

	public void SetCostIncreaseCount(long count)
	{
		m_CostIncreaseCount = count;
	}

	public long GetPrevCostIncreaseCount()
	{
		return m_PrevCostIncreaseCount;
	}

	public void ResetReward()
	{
		m_NKMRewardData = new NKMRewardData();
	}

	public void AddReward(NKMRewardData newNKMRewardData)
	{
		m_NKMRewardData.AddRewardDataForRepeatOperation(newNKMRewardData);
	}

	public NKMRewardData GetReward()
	{
		return m_NKMRewardData;
	}

	public NKMRewardData GetPrevReward()
	{
		return m_NKMRewardDataPrev;
	}

	public void SetAlarmRepeatOperationQuitByDefeat(bool bSet)
	{
		m_bAlarmRepeatOperationQuitByDefeat = bSet;
	}

	public bool GetAlarmRepeatOperationQuitByDefeat()
	{
		return m_bAlarmRepeatOperationQuitByDefeat;
	}

	public void SetAlarmRepeatOperationSuccess(bool bSet)
	{
		m_bAlarmRepeatOperationSuccess = bSet;
	}

	public bool GetAlarmRepeatOperationSuccess()
	{
		return m_bAlarmRepeatOperationSuccess;
	}

	public void SetNeedToSaveRewardData(bool bSet)
	{
		m_bNeedToSavePrevData = bSet;
	}

	public void SetStartTime(DateTime StartDateTime)
	{
		m_StartDateTime = StartDateTime;
	}

	public DateTime GetStartTime()
	{
		return m_StartDateTime;
	}

	public TimeSpan GetPrevProgressDuration()
	{
		return m_tsPrevProgressDuration;
	}

	public int GetPrevCostItemID()
	{
		return m_PrevCostItemID;
	}

	public int GetPrevCostItemCount()
	{
		return m_PrevCostItemCount;
	}

	public long GetPrevRepeatCount()
	{
		return m_PrevRepeatCount;
	}

	public string GetPrevEPTitle()
	{
		return m_PrevEPTitle;
	}

	public string GetPrevEPName()
	{
		return m_PrevEPName;
	}

	public void SetStopReason(string stopReason)
	{
		m_StopReason = stopReason;
		SetAlarmRepeatOperationQuitByDefeat(bSet: false);
		SetAlarmRepeatOperationSuccess(bSet: false);
	}

	public string GetStopReason()
	{
		return m_StopReason;
	}

	public void Init()
	{
		if (m_bNeedToSavePrevData)
		{
			m_bNeedToSavePrevData = false;
			m_NKMRewardDataPrev = m_NKMRewardData;
			m_NKMRewardData = new NKMRewardData();
			m_tsPrevProgressDuration = NKCSynchronizedTime.GetServerUTCTime() - GetStartTime();
			m_StartDateTime = default(DateTime);
			m_PrevRepeatCount = m_CurrRepeatCount;
			GetCostInfo(out m_PrevCostItemID, out m_PrevCostItemCount);
			m_PrevEPTitle = NKCPopupRepeatOperation.m_LastEPTitle;
			m_PrevEPName = NKCPopupRepeatOperation.m_LastEPName;
			m_PrevCostIncreaseCount = m_CostIncreaseCount;
		}
		SetIsOnGoing(bSet: false);
		SetMaxRepeatCount(0L);
		SetCurrRepeatCount(0L);
		SetCostIncreaseCount(0L);
	}

	public void UpdateRepeatOperationGameHudUI()
	{
		switch (NKCScenManager.GetScenManager().GetNowScenID())
		{
		case NKM_SCEN_ID.NSI_WARFARE_GAME:
		{
			NKCWarfareGame warfareGame = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame();
			if (warfareGame != null && warfareGame.m_NKCWarfareGameHUD != null)
			{
				warfareGame.m_NKCWarfareGameHUD.SetActiveRepeatOperationOnOff(GetIsOnGoing());
			}
			break;
		}
		case NKM_SCEN_ID.NSI_GAME:
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (gameClient != null && gameClient.GetGameHud() != null && gameClient.GetGameHud().GetNKCGameHUDRepeatOperation() != null)
			{
				gameClient.GetGameHud().GetNKCGameHUDRepeatOperation().ResetBtnOnOffUI();
			}
			break;
		}
		}
	}

	public void OnClickCancel()
	{
		NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
		UpdateRepeatOperationGameHudUI();
	}

	public bool CheckRepeatOperationRealStop()
	{
		if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_POPUP_REPEAT_OPERATION_SCENE_EXIT"), delegate
			{
				NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
				NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetStopReason(NKCUtilString.GET_STRING_REPEAT_OPERATION_IS_TERMINATED);
				NKCPopupRepeatOperation.Instance.OpenForResult();
				OnClickCancel();
			});
			return true;
		}
		return false;
	}

	public static void GetCostInfo(out int costItemID, out int costItemCount)
	{
		costItemID = 0;
		costItemCount = 0;
		if (!GetRepeatOperationType(out var eNKC_REPEAT_OPERATION_TYPE, out var stageTemplet))
		{
			return;
		}
		if (eNKC_REPEAT_OPERATION_TYPE == NKC_REPEAT_OPERATION_TYPE.NROT_WARFARE)
		{
			NKCWarfareManager.GetCurrWarfareAttackCost(out costItemID, out costItemCount);
		}
		else if (stageTemplet != null && stageTemplet.m_StageReqItemID > 0)
		{
			costItemID = stageTemplet.m_StageReqItemID;
			costItemCount = stageTemplet.m_StageReqItemCount;
			if (stageTemplet.m_StageReqItemID == 2)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref costItemCount);
			}
		}
	}

	public static string GetEpisodeBattleName()
	{
		if (!GetRepeatOperationType(out var _, out var stageTemplet))
		{
			return "";
		}
		return NKMEpisodeMgr.GetEpisodeBattleName(stageTemplet);
	}

	public static bool GetRepeatOperationType(out NKC_REPEAT_OPERATION_TYPE eNKC_REPEAT_OPERATION_TYPE, out NKMStageTempletV2 stageTemplet)
	{
		eNKC_REPEAT_OPERATION_TYPE = NKC_REPEAT_OPERATION_TYPE.NROT_NONE;
		stageTemplet = null;
		switch (NKCScenManager.GetScenManager().GetNowScenID())
		{
		case NKM_SCEN_ID.NSI_WARFARE_GAME:
			eNKC_REPEAT_OPERATION_TYPE = NKC_REPEAT_OPERATION_TYPE.NROT_WARFARE;
			break;
		case NKM_SCEN_ID.NSI_GAME:
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (gameClient == null || gameClient.GetGameData() == null)
			{
				return false;
			}
			switch (gameClient.GetGameData().GetGameType())
			{
			case NKM_GAME_TYPE.NGT_PHASE:
				stageTemplet = NKCPhaseManager.GetStageTemplet();
				break;
			case NKM_GAME_TYPE.NGT_WARFARE:
				eNKC_REPEAT_OPERATION_TYPE = NKC_REPEAT_OPERATION_TYPE.NROT_WARFARE;
				break;
			default:
				eNKC_REPEAT_OPERATION_TYPE = NKC_REPEAT_OPERATION_TYPE.NROT_DUNGEON;
				stageTemplet = NKMDungeonManager.GetDungeonTempletBase(gameClient.GetGameData().m_DungeonID)?.StageTemplet;
				break;
			}
			break;
		}
		case NKM_SCEN_ID.NSI_DUNGEON_ATK_READY:
		{
			eNKC_REPEAT_OPERATION_TYPE = NKC_REPEAT_OPERATION_TYPE.NROT_DUNGEON;
			NKC_SCEN_DUNGEON_ATK_READY sCEN_DUNGEON_ATK_READY = NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY();
			stageTemplet = sCEN_DUNGEON_ATK_READY.GetStageTemplet();
			break;
		}
		case NKM_SCEN_ID.NSI_GAME_RESULT:
		{
			NKC_SCEN_GAME_RESULT nKC_SCEN_GAME_RESULT = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT();
			stageTemplet = NKMStageTempletV2.Find(nKC_SCEN_GAME_RESULT.GetStageID());
			break;
		}
		default:
			return false;
		}
		if (eNKC_REPEAT_OPERATION_TYPE == NKC_REPEAT_OPERATION_TYPE.NROT_WARFARE)
		{
			stageTemplet = NKMWarfareTemplet.Find(NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareStrID())?.StageTemplet;
		}
		return true;
	}

	public static bool CheckPossibleForWarfare(string warfareStrID, bool bShowMessage = true)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_REPEAT))
		{
			if (bShowMessage)
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.OPERATION_REPEAT);
			}
			return false;
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.WARFARE_AUTO_MOVE))
		{
			if (bShowMessage)
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.WARFARE_AUTO_MOVE);
			}
			return false;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (!myUserData.IsSuperUser() && !myUserData.CheckWarfareClear(warfareStrID))
		{
			if (bShowMessage)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_CONTENTS_UNLOCK_CLEAR_STAGE);
			}
			return false;
		}
		return true;
	}

	public static bool CheckPossible(int stageID, bool bShowMessage = true)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_REPEAT))
		{
			if (bShowMessage)
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.OPERATION_REPEAT);
			}
			return false;
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BATTLE_AUTO_RESPAWN))
		{
			if (bShowMessage)
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.BATTLE_AUTO_RESPAWN);
			}
			return false;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(stageID);
		if (nKMStageTempletV == null)
		{
			return false;
		}
		if (nKMStageTempletV.EnterLimit > 0)
		{
			int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(nKMStageTempletV.Key);
			if (nKMStageTempletV.EnterLimit - statePlayCnt <= 0)
			{
				return false;
			}
		}
		bool flag = false;
		if (nKMStageTempletV.IsUsingEventDeck())
		{
			if (NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastEventDeck() == null)
			{
				flag = true;
			}
		}
		else if (NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().GetLastDeckIndex()
			.m_eDeckType == NKM_DECK_TYPE.NDT_NONE)
		{
			flag = true;
		}
		if (flag)
		{
			if (bShowMessage)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_TOAST_REPEAT_UNABLE_BY_RECONNECT_DUNGEON"));
			}
			return false;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (!myUserData.IsSuperUser() && !myUserData.CheckStageCleared(stageID))
		{
			if (bShowMessage)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_CONTENTS_UNLOCK_CLEAR_STAGE);
			}
			return false;
		}
		return true;
	}

	public static bool CheckVisible(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return false;
		}
		if (stageTemplet.m_STAGE_TYPE == STAGE_TYPE.ST_DUNGEON && stageTemplet.DungeonTempletBase != null && NKMDungeonManager.IsTutorialDungeon(stageTemplet.DungeonTempletBase.m_DungeonID))
		{
			return false;
		}
		if (stageTemplet.m_bNoAutoRepeat)
		{
			return false;
		}
		return true;
	}

	public static bool CheckVisible(int stageID)
	{
		return CheckVisible(NKMStageTempletV2.Find(stageID));
	}
}
