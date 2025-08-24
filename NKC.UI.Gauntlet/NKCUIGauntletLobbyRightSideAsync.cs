using System;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyRightSideAsync : NKCUIGauntletLobbyRightSideBase
{
	[Header("포인트")]
	public GameObject m_objPVPDoublePoint;

	public Text m_lbPVPDoublePoint;

	public GameObject m_objPVPPoint;

	public Text m_lbRemainPVPPoint;

	public GameObject m_objRemainPVPPointPlusTime;

	public Text m_lbPlusPVPPoint;

	public Text m_lbRemainPVPPointPlusTime;

	[Header("방어덱")]
	public NKCUIComStateButton m_csbtnAsyncDefenseDeck;

	private NKCUIDeckViewer.DeckViewerOption.OnBackButton m_dOnBackButton;

	public override void InitUI()
	{
		base.InitUI();
		m_csbtnAsyncDefenseDeck.PointerClick.RemoveAllListeners();
		m_csbtnAsyncDefenseDeck.PointerClick.AddListener(OnClickAsyncDefenseDeck);
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
		if (GetPvpData() == null)
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

	public void SetCallback(NKCUIDeckViewer.DeckViewerOption.OnBackButton dCallback)
	{
		m_dOnBackButton = dCallback;
	}

	private void OnClickAsyncDefenseDeck()
	{
		NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
		{
			MenuName = NKCUtilString.GET_STRING_GAUNTLET,
			eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.AsyncPvpDefenseDeck,
			dOnDeckSideButtonConfirmForAsync = SelectDefenseDeck,
			DeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP_DEFENCE, 0),
			SelectLeaderUnitOnOpen = true,
			bEnableDefaultBackground = false,
			bUpsideMenuHomeButton = false,
			upsideMenuShowResourceList = new List<int> { 13, 5, 101 },
			StageBattleStrID = string.Empty,
			dOnBackButton = m_dOnBackButton,
			bUseAsyncDeckSetting = true
		};
		NKCUIDeckViewer.Instance.Open(options);
	}

	private void SelectDefenseDeck(NKMDeckIndex deckIndex, NKMDeckData originalDeck)
	{
		NKMDeckData deckData = NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(deckIndex);
		if (HasDeckChanged(deckData, originalDeck))
		{
			NKCPacketSender.Send_NKMPacket_UPDATE_DEFENCE_DECK_REQ(deckData);
		}
		m_dOnBackButton?.Invoke();
	}

	private bool HasDeckChanged(NKMDeckData newDeck, NKMDeckData originalDeck)
	{
		if (originalDeck == null)
		{
			return false;
		}
		if (newDeck == null)
		{
			return false;
		}
		if (originalDeck.m_ShipUID != newDeck.m_ShipUID)
		{
			return true;
		}
		if (originalDeck.m_LeaderIndex != newDeck.m_LeaderIndex)
		{
			return true;
		}
		if (originalDeck.m_OperatorUID != newDeck.m_OperatorUID)
		{
			return true;
		}
		for (int i = 0; i < newDeck.m_listDeckUnitUID.Count; i++)
		{
			if (i < originalDeck.m_listDeckUnitUID.Count && newDeck.m_listDeckUnitUID[i] != originalDeck.m_listDeckUnitUID[i])
			{
				return true;
			}
		}
		return false;
	}

	protected override PvpState GetPvpData()
	{
		return NKCScenManager.GetScenManager().GetMyUserData()?.m_AsyncData;
	}

	protected override NKMPvpRankSeasonTemplet GetSeasonTemplet()
	{
		return NKCPVPManager.GetPvpAsyncSeasonTemplet(NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime()));
	}

	protected override string GetLeagueNameByScore(int seasonID, PvpState pvpData)
	{
		NKMPvpRankTemplet asyncPvpRankTempletByScore = NKCPVPManager.GetAsyncPvpRankTempletByScore(seasonID, NKCUtil.GetScoreBySeason(seasonID, pvpData.SeasonID, pvpData.Score, NKM_GAME_TYPE.NGT_ASYNC_PVP));
		if (asyncPvpRankTempletByScore != null)
		{
			return asyncPvpRankTempletByScore.GetLeagueName();
		}
		return "";
	}

	protected override string GetLeagueNameByTier(int seasonID, PvpState pvpData)
	{
		NKMPvpRankTemplet asyncPvpRankTempletByTier = NKCPVPManager.GetAsyncPvpRankTempletByTier(seasonID, pvpData.LeagueTierID);
		if (asyncPvpRankTempletByTier != null)
		{
			return asyncPvpRankTempletByTier.GetLeagueName();
		}
		return "";
	}
}
