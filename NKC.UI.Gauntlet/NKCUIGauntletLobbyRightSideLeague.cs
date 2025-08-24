using System;
using NKC.PacketHandler;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyRightSideLeague : NKCUIGauntletLobbyRightSideBase
{
	[Header("리그전 전용")]
	public NKCUIComStateButton m_csbtnRankMatchReady;

	public NKCUIComStateButton m_csbtnRankMatchReadyDisable;

	public NKCUIComStateButton m_csbtnRankPVPPoint;

	public GameObject m_objPVPDoublePoint;

	public Text m_lbPVPDoublePoint;

	public GameObject m_objPVPPoint;

	public Text m_lbRemainPVPPoint;

	public GameObject m_objRemainPVPPointPlusTime;

	public Text m_lbPlusPVPPoint;

	public Text m_lbRemainPVPPointPlusTime;

	public Text m_lbLoseCount;

	public Text m_lbWinRate;

	public override void InitUI()
	{
		base.InitUI();
		m_csbtnRankMatchReady.PointerClick.RemoveAllListeners();
		m_csbtnRankMatchReady.PointerClick.AddListener(OnClickLeagueMatchFind);
		m_csbtnRankMatchReadyDisable.PointerClick.RemoveAllListeners();
		m_csbtnRankMatchReadyDisable.PointerClick.AddListener(OnClickLeagueMatchFind);
		m_csbtnRankPVPPoint.PointerClick.RemoveAllListeners();
		m_csbtnRankPVPPoint.PointerClick.AddListener(delegate
		{
			NKCUIPopUpGuide.Instance.Open("ARTICLE_PVP_RANK", 1);
		});
	}

	public void UpdatePVPPointUI()
	{
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
		if (NKCScenManager.GetScenManager().GetMyUserData().m_LeagueData == null)
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

	public override void UpdateNowSeasonPVPInfoUI(NKM_GAME_TYPE gameType)
	{
		base.UpdateNowSeasonPVPInfoUI(gameType);
		if (NKCScenManager.GetScenManager().GetMyUserData() == null)
		{
			return;
		}
		PvpState leagueData = NKCScenManager.GetScenManager().GetMyUserData().m_LeagueData;
		int num = NKCUtil.FindPVPSeasonIDForLeague(NKCSynchronizedTime.GetServerUTCTime());
		if (leagueData == null)
		{
			return;
		}
		if (leagueData.SeasonID != num)
		{
			NKCUtil.SetLabelText(m_lbLoseCount, "-");
			NKCUtil.SetLabelText(m_lbWinRate, "-");
			return;
		}
		NKCUtil.SetLabelText(m_lbLoseCount, leagueData.LoseCount.ToString());
		if (leagueData.WinCount + leagueData.LoseCount <= 0)
		{
			NKCUtil.SetLabelText(m_lbWinRate, "0%");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbWinRate, $"{leagueData.WinCount * 100 / (leagueData.WinCount + leagueData.LoseCount)}%");
		}
	}

	private bool CheckCanPlayPVPGame()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		int seasonID = NKCUtil.FindPVPSeasonIDForLeague(NKCSynchronizedTime.GetServerUTCTime());
		if (NKCPVPManager.CanPlayPVPLeagueGame(myUserData, seasonID, NKCSynchronizedTime.GetServerUTCTime()) == NKM_ERROR_CODE.NEC_OK)
		{
			return true;
		}
		return false;
	}

	public void UpdateReadyButtonUI()
	{
		bool flag = CheckCanPlayPVPGame();
		NKCUtil.SetGameobjectActive(m_csbtnRankMatchReady, flag);
		NKCUtil.SetGameobjectActive(m_csbtnRankMatchReadyDisable, !flag);
	}

	private void OnClickLeagueMatchFind()
	{
		if (m_csbtnRankMatchReady.gameObject.activeSelf)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().SetReservedGameType(NKM_GAME_TYPE.NGT_PVP_LEAGUE);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH);
			return;
		}
		int seasonID = NKCUtil.FindPVPSeasonIDForLeague(NKCSynchronizedTime.GetServerUTCTime());
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCPVPManager.CanPlayPVPLeagueGame(NKCScenManager.CurrentUserData(), seasonID, NKCSynchronizedTime.GetServerUTCTime());
		switch (nKM_ERROR_CODE)
		{
		case NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_NOT_ENOUGH_UNIT_COUNT:
		case NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_NOT_ENOUGH_SHIP_COUNT:
			NKCPopupGauntletLeagueEnterCondition.Instance.Open();
			break;
		case NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_INVALID_TIME:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCStringTable.GetString("SI_DP_GAUNTLET_LEAGUE_START_REQ_TIME_POPUP_DESC"), NKCPVPManager.GetLeagueOpenDaysString(), NKCPVPManager.GetLeagueOpenTimeString()));
			break;
		default:
			NKCPacketHandlers.Check_NKM_ERROR_CODE(nKM_ERROR_CODE);
			break;
		}
	}

	protected override PvpState GetPvpData()
	{
		return NKCScenManager.CurrentUserData().m_LeagueData;
	}

	protected override NKMPvpRankSeasonTemplet GetSeasonTemplet()
	{
		return null;
	}

	protected override string GetLeagueNameByScore(int seasonID, PvpState pvpData)
	{
		NKMLeaguePvpRankGroupTemplet nKMLeaguePvpRankGroupTemplet = NKMLeaguePvpRankGroupTemplet.Find(seasonID);
		if (nKMLeaguePvpRankGroupTemplet != null)
		{
			return NKCStringTable.GetString(nKMLeaguePvpRankGroupTemplet.GetByScore(pvpData.Score).LeagueName);
		}
		return "";
	}

	protected override string GetLeagueNameByTier(int seasonID, PvpState pvpData)
	{
		NKMLeaguePvpRankGroupTemplet nKMLeaguePvpRankGroupTemplet = NKMLeaguePvpRankGroupTemplet.Find(seasonID);
		if (nKMLeaguePvpRankGroupTemplet != null)
		{
			return NKCStringTable.GetString(nKMLeaguePvpRankGroupTemplet.GetByTier(pvpData.LeagueTierID).LeagueName);
		}
		return "";
	}
}
