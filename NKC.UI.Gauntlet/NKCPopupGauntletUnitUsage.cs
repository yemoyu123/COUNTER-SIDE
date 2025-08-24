using System;
using System.Collections.Generic;
using ClientPacket.Pvp;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletUnitUsage : NKCUIBase
{
	public const string UI_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_gauntlet";

	public const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_POPUP_RANK_MOSTWINS";

	private static NKCPopupGauntletUnitUsage m_Instance;

	public EventTrigger m_etBG;

	public NKCUIComStateButton m_cstbnClose;

	[Header("\ufffd\ufffd\ufffd")]
	public NKCUIComToggle m_RankSlotChallenger;

	public NKCUIComToggle m_RankSlotGrandMaster;

	public NKCUIComToggle m_RankSlotMaster;

	public NKCUIComToggle m_RankSlotDiamond;

	public NKCUIComToggle m_RankSlotPlatinum;

	public NKCUIComToggle m_RankSlotGold;

	public NKCUIComToggle m_RankSlotSilver;

	public NKCUIComToggle m_RankSlotBronze;

	public GameObject m_objMyRankChallenger;

	public GameObject m_objMyRankGrandMaster;

	public GameObject m_objMyRankMaster;

	public GameObject m_objMyRankDiamond;

	public GameObject m_objMyRankPlatinum;

	public GameObject m_objMyRankGold;

	public GameObject m_objMyRankSilver;

	public GameObject m_objMyRankBronze;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCUIUnitSelectListSlot[] m_UnitSlot;

	private Dictionary<PvpPickType, NKCUIComToggle> m_dicRankSlot = new Dictionary<PvpPickType, NKCUIComToggle>();

	private Dictionary<PvpPickType, GameObject> m_dicMyRank = new Dictionary<PvpPickType, GameObject>();

	private DateTime nextResetTime;

	public static NKCPopupGauntletUnitUsage Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGauntletUnitUsage>("ab_ui_nkm_ui_gauntlet", "NKM_UI_GAUNTLET_POPUP_RANK_MOSTWINS", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanUpInstance).GetInstance<NKCPopupGauntletUnitUsage>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override string MenuName => "Gauntlet Unit Usage";

	public override eMenutype eUIType => eMenutype.Popup;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanUpInstance()
	{
		if (m_Instance != null)
		{
			m_Instance.m_dicRankSlot.Clear();
			m_Instance.m_dicMyRank.Clear();
		}
		m_Instance = null;
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_cstbnClose, base.Close);
		NKCUtil.SetEventTriggerDelegate(m_etBG, base.Close);
		m_dicRankSlot.Add(PvpPickType.Challenger, m_RankSlotChallenger);
		m_dicRankSlot.Add(PvpPickType.GrandMaster, m_RankSlotGrandMaster);
		m_dicRankSlot.Add(PvpPickType.Master, m_RankSlotMaster);
		m_dicRankSlot.Add(PvpPickType.Diamond, m_RankSlotDiamond);
		m_dicRankSlot.Add(PvpPickType.Platinum, m_RankSlotPlatinum);
		m_dicRankSlot.Add(PvpPickType.Gold, m_RankSlotGold);
		m_dicRankSlot.Add(PvpPickType.Silver, m_RankSlotSilver);
		m_dicRankSlot.Add(PvpPickType.Bornze, m_RankSlotBronze);
		m_dicMyRank.Add(PvpPickType.Challenger, m_objMyRankChallenger);
		m_dicMyRank.Add(PvpPickType.GrandMaster, m_objMyRankGrandMaster);
		m_dicMyRank.Add(PvpPickType.Master, m_objMyRankMaster);
		m_dicMyRank.Add(PvpPickType.Diamond, m_objMyRankDiamond);
		m_dicMyRank.Add(PvpPickType.Platinum, m_objMyRankPlatinum);
		m_dicMyRank.Add(PvpPickType.Gold, m_objMyRankGold);
		m_dicMyRank.Add(PvpPickType.Silver, m_objMyRankSilver);
		m_dicMyRank.Add(PvpPickType.Bornze, m_objMyRankBronze);
		foreach (KeyValuePair<PvpPickType, NKCUIComToggle> slot in m_dicRankSlot)
		{
			NKCUtil.SetToggleValueChangedDelegate(slot.Value, delegate(bool value)
			{
				OnClickedRankSlot(value, slot.Key);
			});
		}
		NKCUIUnitSelectListSlot[] unitSlot = m_UnitSlot;
		for (int num = 0; num < unitSlot.Length; num++)
		{
			unitSlot[num].Init();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void CloseInternal()
	{
		if (NKCSynchronizedTime.GetServerUTCTime() > nextResetTime)
		{
			NKCRankPVPMgr.SetPickRateData(null);
		}
		base.gameObject.SetActive(value: false);
	}

	public void Open()
	{
		nextResetTime = NKMTime.GetNextResetTime(NKCSynchronizedTime.GetServerUTCTime(), NKM_MISSION_RESET_INTERVAL.DAILY);
		UIOpened();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		PvpPickType pvpPickType = PvpPickType.Bornze;
		if (myUserData != null)
		{
			int num = NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime());
			int leagueScore = myUserData.m_PvpData.Score;
			if (myUserData.m_PvpData.SeasonID != num)
			{
				leagueScore = NKCPVPManager.GetResetScore(myUserData.m_PvpData.SeasonID, myUserData.m_PvpData.Score, NKM_GAME_TYPE.NGT_PVP_RANK);
			}
			NKMPvpRankTemplet rankTempletByScore = NKCPVPManager.GetRankTempletByScore(NKM_GAME_TYPE.NGT_PVP_RANK, num, leagueScore);
			pvpPickType = GetPvpPickType(rankTempletByScore);
		}
		if (m_dicRankSlot.ContainsKey(pvpPickType))
		{
			m_dicRankSlot[pvpPickType].Select(bSelect: false, bForce: true);
			m_dicRankSlot[pvpPickType].Select(bSelect: true);
		}
		else
		{
			m_RankSlotBronze.Select(bSelect: false, bForce: true);
			m_RankSlotBronze.Select(bSelect: true);
		}
		foreach (KeyValuePair<PvpPickType, GameObject> item in m_dicMyRank)
		{
			NKCUtil.SetGameobjectActive(item.Value, item.Key == pvpPickType);
		}
	}

	private PvpPickType GetPvpPickType(NKMPvpRankTemplet pvpRankTemplet)
	{
		if (pvpRankTemplet == null)
		{
			return PvpPickType.Bornze;
		}
		return pvpRankTemplet.LeagueTierIcon switch
		{
			LEAGUE_TIER_ICON.LTI_BRONZE => PvpPickType.Bornze, 
			LEAGUE_TIER_ICON.LTI_SILVER => PvpPickType.Silver, 
			LEAGUE_TIER_ICON.LTI_GOLD => PvpPickType.Gold, 
			LEAGUE_TIER_ICON.LTI_PLATINUM => PvpPickType.Platinum, 
			LEAGUE_TIER_ICON.LTI_DIAMOND => PvpPickType.Diamond, 
			LEAGUE_TIER_ICON.LTI_MASTER => PvpPickType.Master, 
			LEAGUE_TIER_ICON.LTI_GRANDMASTER => PvpPickType.GrandMaster, 
			LEAGUE_TIER_ICON.LTI_CHALLENGER => PvpPickType.Challenger, 
			_ => PvpPickType.Bornze, 
		};
	}

	private void OnClickedRankSlot(bool value, PvpPickType pickType)
	{
		PvpPickRateData pickRateData = NKCRankPVPMgr.GetPickRateData(pickType);
		if (pickRateData == null)
		{
			NKCUIUnitSelectListSlot[] unitSlot = m_UnitSlot;
			for (int i = 0; i < unitSlot.Length; i++)
			{
				unitSlot[i].SetEmpty(bEnableLayoutElement: true, null);
			}
			return;
		}
		int num = m_UnitSlot.Length;
		for (int j = 0; j < num; j++)
		{
			if (pickRateData.winUnits == null || j >= pickRateData.winUnits.Count)
			{
				m_UnitSlot[j].SetEmpty(bEnableLayoutElement: true, null);
				continue;
			}
			NKMUnitTempletBase templetBase = NKMUnitTempletBase.Find(pickRateData.winUnits[j]);
			m_UnitSlot[j].SetDataForBan(templetBase, bEnableLayoutElement: true, null, bUp: false, bSetOriginalCost: true);
		}
	}
}
