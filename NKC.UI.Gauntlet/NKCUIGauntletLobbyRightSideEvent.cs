using System;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyRightSideEvent : NKCUIGauntletLobbyRightSideBase
{
	[Header("\ufffd\u033a\ufffdƮ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCUIComStateButton m_csbtnEventMatchReady;

	public NKCUIComStateButton m_csbtnReward;

	public NKCUIGauntletEventReward m_gauntletEventReward;

	public GameObject m_objRedDotReward;

	public Text m_lbSeasonTitle1;

	public Text m_lbSeasonTitle2;

	public Text m_lbSeasonRule;

	[Header("\ufffd\ufffd\ufffd\ufffdƮ")]
	public GameObject m_objPVPDoublePoint;

	public Text m_lbPVPDoublePoint;

	public GameObject m_objPVPPoint;

	public Text m_lbRemainPVPPoint;

	public GameObject m_objRemainPVPPointPlusTime;

	public Text m_lbPlusPVPPoint;

	public Text m_lbRemainPVPPointPlusTime;

	private bool m_rewardOpenStandby;

	public override void InitUI()
	{
		base.InitUI();
		NKCUtil.SetButtonClickDelegate(m_csbtnEventMatchReady, OnClickEventMatchReady);
		NKCUtil.SetButtonClickDelegate(m_csbtnReward, OnClickReward);
		if (m_csbtnReward != null)
		{
			m_csbtnReward.m_bGetCallbackWhileLocked = true;
		}
		m_gauntletEventReward?.Init();
	}

	public void OnCloseInstance()
	{
		m_gauntletEventReward?.OnCloseInstance();
	}

	public void UpdateEventPVPUI()
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		bool flag = true;
		string eventMatchIntervalString = NKCEventPvpMgr.GetEventMatchIntervalString();
		if (eventPvpSeasonTemplet != null)
		{
			flag = eventPvpSeasonTemplet.EventPvpRewardTemplets.Count <= 0;
		}
		NKCUtil.SetGameobjectActive(m_csbtnReward, !flag);
		m_gauntletEventReward?.CloseImmediately();
		m_rewardOpenStandby = false;
		NKCUtil.SetLabelText(m_lbSeasonTitle1, NKCStringTable.GetString(eventPvpSeasonTemplet.SeasonName));
		NKCUtil.SetLabelText(m_lbSeasonTitle2, eventMatchIntervalString);
		NKCUtil.SetLabelText(m_lbSeasonRule, NKCStringTable.GetString(eventPvpSeasonTemplet.SeasonRule));
	}

	public bool CanClose()
	{
		if (m_gauntletEventReward != null && !m_gauntletEventReward.IsClosed())
		{
			m_gauntletEventReward.Close();
			return false;
		}
		return true;
	}

	public void UpdateRewardRedDot()
	{
		NKCUtil.SetGameobjectActive(m_objRedDotReward, NKCEventPvpMgr.CanGetReward());
	}

	public void OnRecvEventPvpSeasonInfo()
	{
		NKCUtil.SetGameobjectActive(m_objRedDotReward, NKCEventPvpMgr.CanGetReward());
		if (m_rewardOpenStandby)
		{
			m_rewardOpenStandby = false;
			m_gauntletEventReward?.Open();
		}
	}

	public void RefreshReward()
	{
		NKCUtil.SetGameobjectActive(m_objRedDotReward, NKCEventPvpMgr.CanGetReward());
		if (m_gauntletEventReward.IsOpened())
		{
			m_gauntletEventReward.Refresh();
		}
	}

	public void UpdateRankPVPPointUI()
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet == null || !eventPvpSeasonTemplet.ForceGetWinPoint)
		{
			NKCUtil.SetGameobjectActive(m_objPVPPoint, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objPVPPoint, bValue: true);
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		long countMiscItem = inventoryData.GetCountMiscItem(301);
		int cHARGE_POINT_MAX_COUNT = NKMPvpCommonConst.Instance.CHARGE_POINT_MAX_COUNT;
		long countMiscItem2 = inventoryData.GetCountMiscItem(6);
		int rewardChargePoint = NKMPvpCommonConst.Instance.CHARGE_POINT_ONE_STEP;
		int bonusRatio = 0;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKCCompanyBuff.IncreaseChargePointOfPvpWithBonusRatio(nKMUserData.m_companyBuffDataList, ref rewardChargePoint, out bonusRatio);
		}
		if (countMiscItem > 0)
		{
			NKCUtil.SetGameobjectActive(m_objPVPDoublePoint, bValue: true);
			NKCUtil.SetLabelText(m_lbPVPDoublePoint, countMiscItem.ToString());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objPVPDoublePoint, bValue: false);
		}
		NKCUtil.SetLabelText(m_lbRemainPVPPoint, $"{countMiscItem2}<color=#5d77a3>/{cHARGE_POINT_MAX_COUNT}</color>");
		if (NKCScenManager.GetScenManager().GetMyUserData().m_PvpData == null)
		{
			return;
		}
		if (countMiscItem2 < cHARGE_POINT_MAX_COUNT)
		{
			NKCUtil.SetGameobjectActive(m_objRemainPVPPointPlusTime, bValue: true);
			if (bonusRatio > 0)
			{
				NKCUtil.SetLabelText(m_lbPlusPVPPoint, $"<color=#00baff>+{rewardChargePoint}</color>");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbPlusPVPPoint, $"+{rewardChargePoint}");
			}
			DateTime dateTime = new DateTime(NKCPVPManager.GetLastUpdateChargePointTicks());
			DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
			TimeSpan timeSpan = new DateTime(dateTime.Ticks + NKMPvpCommonConst.Instance.CHARGE_POINT_REFRESH_INTERVAL_TICKS) - serverUTCTime;
			if (timeSpan.TotalHours >= 1.0)
			{
				NKCUtil.SetLabelText(m_lbRemainPVPPointPlusTime, $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbRemainPVPPointPlusTime, $"{timeSpan.Minutes}:{timeSpan.Seconds:00}");
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().ProcessPVPPointCharge();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRemainPVPPointPlusTime, bValue: false);
		}
	}

	private bool HasDeterminedDeck(NKMDungeonEventDeckTemplet eventDeckTemplet)
	{
		if (eventDeckTemplet == null)
		{
			return false;
		}
		if (NKCEventPvpMgr.IsDeteminedSlotType(eventDeckTemplet.ShipSlot.m_eType))
		{
			return true;
		}
		if (NKCEventPvpMgr.IsDeteminedSlotType(eventDeckTemplet.OperatorSlot.m_eType))
		{
			return true;
		}
		int count = eventDeckTemplet.m_lstUnitSlot.Count;
		for (int i = 0; i < count; i++)
		{
			if (NKCEventPvpMgr.IsDeteminedSlotType(eventDeckTemplet.m_lstUnitSlot[i].m_eType))
			{
				return true;
			}
		}
		return false;
	}

	private void OnClickReward()
	{
		if (m_csbtnReward.m_bLock)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENTMATCH_REWARD_NONE);
			return;
		}
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (m_gauntletEventReward.IsOpened())
		{
			m_gauntletEventReward.Close();
		}
		else if (NKCEventPvpMgr.EventPvpRewardInfo == null)
		{
			m_rewardOpenStandby = true;
			NKCPacketSender.Send_NKMPacket_EVENT_PVP_SEASON_INFO_REQ(eventPvpSeasonTemplet.SeasonId);
		}
		else
		{
			m_gauntletEventReward?.Open();
		}
	}

	private void OnClickEventMatchReady()
	{
		if (!NKCEventPvpMgr.IsEventPvpTime())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENTMATCH_CANNOT_ENTER);
			return;
		}
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet != null && eventPvpSeasonTemplet.DraftBanPick && !NKCBanManager.IsTryDraftBan())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_GLOBAL_BAN_NOT_SELECT_UNIT);
		}
		else if (m_csbtnEventMatchReady.gameObject.activeSelf)
		{
			if (eventPvpSeasonTemplet != null && eventPvpSeasonTemplet.DraftBanPick)
			{
				NKCEventPvpMgr.EventDeckData = new NKMEventDeckData();
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().SetReservedGameType(NKM_GAME_TYPE.NGT_PVP_EVENT);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH);
			}
			else
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_EVENT_READY);
			}
		}
	}

	protected override PvpState GetPvpData()
	{
		return NKCScenManager.CurrentUserData()?.m_eventPvpData;
	}

	protected override NKMPvpRankSeasonTemplet GetSeasonTemplet()
	{
		return null;
	}

	protected override string GetLeagueNameByScore(int seasonID, PvpState pvpData)
	{
		return "";
	}

	protected override string GetLeagueNameByTier(int seasonID, PvpState pvpData)
	{
		return "";
	}
}
