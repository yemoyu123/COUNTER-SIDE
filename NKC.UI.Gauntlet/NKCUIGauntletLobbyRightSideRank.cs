using System;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyRightSideRank : NKCUIGauntletLobbyRightSideBase
{
	[Header("랭크전 전용")]
	public NKCUIComStateButton m_csbtnRankMatchReady;

	public NKCUIComStateButton m_csbtnRankMatchReadyDisable;

	public NKCUIComStateButton m_csbtnRankPVPPoint;

	public NKCUIComStateButton m_csbtnBanListOld;

	public NKCUIComStateButton m_csbtnBanList;

	public NKCUIComStateButton m_csbtnRankCastingBan;

	public Image m_imgRankCastingBan;

	public NKCUIComStateButton m_csbtnUnitUsageInfo;

	[Header("포인트")]
	public GameObject m_objPVPDoublePoint;

	public Text m_lbPVPDoublePoint;

	public GameObject m_objPVPPoint;

	public Text m_lbRemainPVPPoint;

	public GameObject m_objRemainPVPPointPlusTime;

	public Text m_lbPlusPVPPoint;

	public Text m_lbRemainPVPPointPlusTime;

	public GameObject m_objCastingBanRedDot;

	[Header("Bot Matching")]
	public NKCUIComToggle m_cstglBotMatching;

	public static string RankBotMatchLocalSaveKey = "RANK_BOT_MATCHING";

	private bool m_bUseBotMatching;

	public override void InitUI()
	{
		base.InitUI();
		if (m_csbtnRankMatchReady != null)
		{
			m_csbtnRankMatchReady.PointerClick.RemoveAllListeners();
			m_csbtnRankMatchReady.PointerClick.AddListener(OnClickRankMatchReady);
		}
		if (m_csbtnRankMatchReadyDisable != null)
		{
			m_csbtnRankMatchReadyDisable.PointerClick.RemoveAllListeners();
			m_csbtnRankMatchReadyDisable.PointerClick.AddListener(OnClickRankMatchReady);
		}
		if (m_csbtnRankPVPPoint != null)
		{
			m_csbtnRankPVPPoint.PointerClick.RemoveAllListeners();
			m_csbtnRankPVPPoint.PointerClick.AddListener(delegate
			{
				NKCUIPopUpGuide.Instance.Open("ARTICLE_PVP_RANK", 1);
			});
		}
		bool flag = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_CASTING_BAN);
		NKCUtil.SetGameobjectActive(m_csbtnBanListOld, !flag);
		NKCUtil.SetGameobjectActive(m_csbtnRankCastingBan, flag);
		NKCUtil.SetGameobjectActive(m_csbtnBanList, flag);
		NKCUtil.SetGameobjectActive(m_csbtnUnitUsageInfo, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_PICK_RATE));
		NKCUtil.SetBindFunction(m_csbtnBanListOld, OnClickBanList);
		NKCUtil.SetBindFunction(m_csbtnBanList, OnClickBanList);
		NKCUtil.SetBindFunction(m_csbtnRankCastingBan, OnClickRankCastingBan);
		NKCUtil.SetBindFunction(m_csbtnUnitUsageInfo, OnClickUnitUsageInfo);
		NKCUtil.SetToggleValueChangedDelegate(m_cstglBotMatching, OnToggleBotMatch);
	}

	private void OnClickRankMatchReady()
	{
		if (m_csbtnRankMatchReady.gameObject.activeSelf)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH_READY().SetReservedGameType(NKM_GAME_TYPE.NGT_PVP_RANK);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY);
			return;
		}
		int seasonID = NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime());
		int weekIDForRank = NKCPVPManager.GetWeekIDForRank(NKCSynchronizedTime.GetServerUTCTime(), seasonID);
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCPVPManager.CanPlayPVPRankGame(NKCScenManager.CurrentUserData(), seasonID, weekIDForRank, NKCSynchronizedTime.GetServerUTCTime());
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
		}
	}

	private void OnClickRankCastingBan()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_CASTING_BAN) && CheckCanPlayPVPRankGame())
		{
			NKCPopupGauntletBan.Instance.Open(bIsCastingBan: true);
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_GAUNTLET_SELECT_IMPOSSIBLE);
		}
	}

	public void UpdateRankPVPPointUI()
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

	public void UpdateReadyRankButtonUI()
	{
		bool flag = CheckCanPlayPVPRankGame();
		NKCUtil.SetGameobjectActive(m_csbtnRankMatchReady, flag);
		NKCUtil.SetGameobjectActive(m_csbtnRankMatchReadyDisable, !flag);
		if (m_csbtnRankCastingBan.gameObject.activeSelf)
		{
			NKCUtil.SetImageSprite(m_imgRankCastingBan, NKCUtil.GetButtonSprite(flag ? NKCUtil.ButtonColor.BC_BLUE : NKCUtil.ButtonColor.BC_GRAY));
		}
	}

	public void UpdateCastingBanVoteState()
	{
		NKCUtil.SetGameobjectActive(m_objCastingBanRedDot, !NKCBanManager.IsCastingBanVoted());
	}

	private void OnClickBanList()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_CASTING_BAN))
		{
			NKCPopupGauntletBanListV2.Instance.Open();
		}
		else
		{
			NKCPopupGauntletBanList.Instance.Open();
		}
	}

	public static bool CheckCanPlayPVPRankGame()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		int seasonID = NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime());
		int weekIDForRank = NKCPVPManager.GetWeekIDForRank(NKCSynchronizedTime.GetServerUTCTime(), seasonID);
		if (NKCPVPManager.CanPlayPVPRankGame(myUserData, seasonID, weekIDForRank, NKCSynchronizedTime.GetServerUTCTime()) == NKM_ERROR_CODE.NEC_OK)
		{
			return true;
		}
		return false;
	}

	private void OnClickUnitUsageInfo()
	{
		if (NKCRankPVPMgr.PickRateData == null)
		{
			NKCPacketSender.NKMPacket_PVP_PICK_RATE_REQ(NKM_GAME_TYPE.NGT_PVP_RANK);
		}
		else
		{
			NKCPopupGauntletUnitUsage.Instance.Open();
		}
	}

	private void OnToggleBotMatch(bool bActive)
	{
		m_bUseBotMatching = bActive;
		if (bActive)
		{
			PlayerPrefs.SetInt(RankBotMatchLocalSaveKey, 1);
		}
		else
		{
			PlayerPrefs.DeleteKey(RankBotMatchLocalSaveKey);
		}
	}

	public void UpdateRankBotMatchUI()
	{
		m_bUseBotMatching = PlayerPrefs.GetInt(RankBotMatchLocalSaveKey, 0) == 1;
		m_cstglBotMatching.Select(m_bUseBotMatching, bForce: true);
	}

	protected override PvpState GetPvpData()
	{
		return NKCScenManager.GetScenManager().GetMyUserData()?.m_PvpData;
	}

	protected override NKMPvpRankSeasonTemplet GetSeasonTemplet()
	{
		return NKCPVPManager.GetPvpRankSeasonTemplet(NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime()));
	}

	protected override string GetLeagueNameByScore(int seasonID, PvpState pvpData)
	{
		NKMPvpRankTemplet pvpRankTempletByScore = NKCPVPManager.GetPvpRankTempletByScore(seasonID, NKCUtil.GetScoreBySeason(seasonID, pvpData.SeasonID, pvpData.Score, NKM_GAME_TYPE.NGT_PVP_RANK));
		if (pvpRankTempletByScore != null)
		{
			return pvpRankTempletByScore.GetLeagueName();
		}
		return "";
	}

	protected override string GetLeagueNameByTier(int seasonID, PvpState pvpData)
	{
		NKMPvpRankTemplet pvpRankTempletByTier = NKCPVPManager.GetPvpRankTempletByTier(seasonID, pvpData.LeagueTierID);
		if (pvpRankTempletByTier != null)
		{
			return pvpRankTempletByTier.GetLeagueName();
		}
		return "";
	}
}
