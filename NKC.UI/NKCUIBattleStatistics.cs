using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Game;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIBattleStatistics : NKCUIBase
{
	public class UnitBattleData
	{
		public int UnitID;

		public NKMUnitData unitData;

		public NKMGameRecordUnitData recordData;

		public bool bSummon;

		public bool bLeader;

		public bool bAssist;
	}

	public class BattleData
	{
		public UnitBattleData mainShipA;

		public UnitBattleData mainShipB;

		public List<UnitBattleData> teamA = new List<UnitBattleData>();

		public List<UnitBattleData> teamB = new List<UnitBattleData>();

		public NKM_GAME_TYPE gameType;

		public float maxValue;

		public float maxDps;

		public float playTime;

		public bool m_bBanGame;

		public bool m_bUpUnitGame;

		public bool m_bHideUpBanUITeamA;
	}

	public delegate void OnClose();

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_RESULT";

	public const string UI_ASSET_NAME = "NKM_UI_RESULT_BATTLE_STATISTICS";

	private static NKCUIBattleStatistics m_Instance;

	public NKCUIComStateButton m_btnClose;

	public Transform m_contents;

	public NKCUIComToggle m_tgDps;

	public Text m_subTitleText;

	public Text m_txtPlayTime;

	private List<NKCUIBattleStatisticsSlot> m_slotList = new List<NKCUIBattleStatisticsSlot>();

	private BattleData m_battleData;

	private int TEMP_SLOT_COUNT = 10;

	private OnClose dOnClose;

	public static NKCUIBattleStatistics Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIBattleStatistics>("AB_UI_NKM_UI_RESULT", "NKM_UI_RESULT_BATTLE_STATISTICS", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCUIBattleStatistics>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

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

	public override string MenuName => "전투 통계";

	public override eMenutype eUIType => eMenutype.Popup;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null)
		{
			if (m_Instance.IsOpen)
			{
				m_Instance.Close();
			}
			m_Instance.Clear();
		}
	}

	public void Init()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_tgDps.OnValueChanged.RemoveAllListeners();
		m_tgDps.OnValueChanged.AddListener(ChangeToggleDPS);
		NKCUIBattleStatisticsSlot newInstance = NKCUIBattleStatisticsSlot.GetNewInstance(m_contents, bLeader: true);
		m_slotList.Add(newInstance);
		for (int i = 0; i < TEMP_SLOT_COUNT; i++)
		{
			newInstance = NKCUIBattleStatisticsSlot.GetNewInstance(m_contents, bLeader: false);
			m_slotList.Add(newInstance);
		}
	}

	public void Open(BattleData battleData, OnClose onClose)
	{
		if (battleData != null)
		{
			m_battleData = battleData;
			m_battleData.teamA.RemoveAll((UnitBattleData x) => WillHideUnitFromUI(x));
			m_battleData.teamB.RemoveAll((UnitBattleData x) => WillHideUnitFromUI(x));
			m_tgDps.Select(bSelect: false, bForce: true);
			Sort(bDps: false);
			SetData(m_battleData);
			TimeSpan timeSpan = TimeSpan.FromSeconds(battleData.playTime);
			m_txtPlayTime.text = NKCUtilString.GetTimeSpanStringMS(timeSpan);
			if (onClose != null)
			{
				dOnClose = onClose;
			}
			UIOpened();
		}
	}

	private bool WillHideUnitFromUI(UnitBattleData battleData)
	{
		if (battleData == null)
		{
			return true;
		}
		return NKMUnitManager.GetUnitTempletBase(battleData.UnitID)?.m_bHideBattleResult ?? true;
	}

	private void ChangeToggleDPS(bool bDps)
	{
		Sort(bDps);
		SetData(m_battleData, bDps);
	}

	private void SetData(BattleData battleData, bool isDps = false)
	{
		int num = Mathf.Max(battleData.teamA.Count + 1, battleData.teamB.Count + 1);
		Debug.LogWarning($"NeedSlot : {num}");
		for (int i = m_slotList.Count; i < num; i++)
		{
			NKCUIBattleStatisticsSlot newInstance = NKCUIBattleStatisticsSlot.GetNewInstance(m_contents, bLeader: false);
			m_slotList.Add(newInstance);
		}
		float maxValue = (isDps ? battleData.maxDps : battleData.maxValue);
		bool showBossMark = NKMGame.IsPVE(battleData.gameType);
		NKCUtil.SetGameobjectActive(m_slotList[0], bValue: true);
		m_slotList[0].SetDataA(battleData.mainShipA, maxValue, isDps);
		m_slotList[0].SetDataB(battleData.mainShipB, maxValue, isDps, showBossMark);
		if (battleData.gameType == NKM_GAME_TYPE.NGT_PVE_SIMULATED)
		{
			m_slotList[0].SetLeaderBossMarkActive(bSet: false);
		}
		int num2 = 0;
		for (int j = 1; j < m_slotList.Count; j++)
		{
			if (j >= num)
			{
				NKCUtil.SetGameobjectActive(m_slotList[j], bValue: false);
				continue;
			}
			NKCUtil.SetGameobjectActive(m_slotList[j], bValue: true);
			if (num2 < battleData.teamA.Count)
			{
				m_slotList[j].SetEnableShowBan(m_battleData.m_bBanGame && !m_battleData.m_bHideUpBanUITeamA);
				m_slotList[j].SetEnableShowUpUnit(m_battleData.m_bUpUnitGame && !m_battleData.m_bHideUpBanUITeamA);
				m_slotList[j].SetDataA(battleData.teamA[num2], maxValue, isDps);
			}
			else
			{
				m_slotList[j].SetDataA(null, 0f);
			}
			if (num2 < battleData.teamB.Count)
			{
				m_slotList[j].SetEnableShowBan(m_battleData.m_bBanGame);
				m_slotList[j].SetEnableShowUpUnit(m_battleData.m_bUpUnitGame);
				m_slotList[j].SetDataB(battleData.teamB[num2], maxValue, isDps);
			}
			else
			{
				m_slotList[j].SetDataB(null, 0f);
			}
			num2++;
		}
	}

	public override void CloseInternal()
	{
		dOnClose?.Invoke();
		base.gameObject.SetActive(value: false);
		m_battleData = null;
	}

	private void Clear()
	{
		for (int i = 0; i < m_slotList.Count; i++)
		{
			m_slotList[i].CloseInstance();
		}
		m_slotList.Clear();
	}

	public static BattleData MakeBattleData(NKCGameClient game, NKMPacket_GAME_END_NOT sPacket)
	{
		return MakeBattleData(game, sPacket.gameRecord, sPacket.totalPlayTime);
	}

	public static BattleData MakeBattleData(NKCGameClient game, NKMGameRecord gameRecord, NKM_GAME_TYPE gameType = NKM_GAME_TYPE.NGT_INVALID)
	{
		return MakeBattleData(game, gameRecord, game.GetGameRuntimeData().GetGamePlayTime(), gameType);
	}

	public static BattleData MakeBattleData(NKCGameClient game, NKMGameRecord gameRecord, float totalGameTime, NKM_GAME_TYPE gameType = NKM_GAME_TYPE.NGT_INVALID)
	{
		if (game == null || gameRecord == null)
		{
			return null;
		}
		NKMGameData gameData = game.GetGameData();
		BattleData battleData = new BattleData();
		battleData.gameType = gameData.GetGameType();
		battleData.m_bBanGame = gameData.IsBanGame();
		battleData.m_bUpUnitGame = gameData.IsUpUnitGame();
		battleData.m_bHideUpBanUITeamA = gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY;
		battleData.playTime = totalGameTime;
		NKM_TEAM_TYPE nKM_TEAM_TYPE = NKM_TEAM_TYPE.NTT_INVALID;
		nKM_TEAM_TYPE = ((!NKCReplayMgr.IsPlayingReplay() && !game.IsObserver(NKCScenManager.CurrentUserData())) ? game.GetGameData().GetTeamType(NKCScenManager.CurrentUserData().m_UserUID) : ((NKCScenManager.CurrentUserData().m_UserUID == 0L) ? NKM_TEAM_TYPE.NTT_A1 : (game.GetGameData().GetTeamData(NKCScenManager.CurrentUserData().m_UserUID)?.m_eNKM_TEAM_TYPE ?? NKM_TEAM_TYPE.NTT_A1)));
		NKMGameTeamData myTeamData = null;
		NKMGameTeamData enemyTeamData = null;
		if (game.IsATeam(nKM_TEAM_TYPE))
		{
			myTeamData = gameData.m_NKMGameTeamDataA;
			enemyTeamData = gameData.m_NKMGameTeamDataB;
		}
		else
		{
			myTeamData = gameData.m_NKMGameTeamDataB;
			enemyTeamData = gameData.m_NKMGameTeamDataA;
		}
		foreach (KeyValuePair<short, NKMGameRecordUnitData> unitRecord in gameRecord.UnitRecordList)
		{
			short key = unitRecord.Key;
			NKMGameRecordUnitData value = unitRecord.Value;
			UnitBattleData unitBattleData = null;
			bool flag = game.IsSameTeam(nKM_TEAM_TYPE, value.teamType);
			NKMGameTeamData nKMGameTeamData = (flag ? myTeamData : enemyTeamData);
			int checkUnitID = value.unitId;
			bool bSummon = value.isSummonee;
			bool bAssist = value.isAssistUnit;
			bool bLeader = value.isLeader;
			string changeName = value.changeUnitName;
			unitBattleData = ((!flag) ? battleData.teamB.Find((UnitBattleData v) => v.UnitID == checkUnitID && v.bAssist == bAssist && v.bSummon == bSummon && v.bLeader == bLeader && string.Equals(v.recordData.changeUnitName, changeName)) : battleData.teamA.Find((UnitBattleData v) => v.UnitID == checkUnitID && v.bAssist == bAssist && v.bSummon == bSummon && v.bLeader == bLeader && string.Equals(v.recordData.changeUnitName, changeName)));
			if (unitBattleData == null)
			{
				unitBattleData = new UnitBattleData();
				unitBattleData.UnitID = checkUnitID;
				unitBattleData.bSummon = bSummon;
				unitBattleData.bAssist = bAssist;
				unitBattleData.bLeader = bLeader;
				unitBattleData.recordData = new NKMGameRecordUnitData();
				unitBattleData.recordData.unitId = value.unitId;
				unitBattleData.recordData.unitLevel = value.unitLevel;
				unitBattleData.recordData.isSummonee = value.isSummonee;
				unitBattleData.recordData.isAssistUnit = value.isAssistUnit;
				unitBattleData.recordData.isLeader = value.isLeader;
				unitBattleData.recordData.teamType = value.teamType;
				unitBattleData.recordData.changeUnitName = value.changeUnitName;
				if (flag)
				{
					battleData.teamA.Add(unitBattleData);
				}
				else
				{
					battleData.teamB.Add(unitBattleData);
				}
			}
			if (unitBattleData.unitData == null)
			{
				NKMUnitData nKMUnitData = null;
				NKMUnit unit = game.GetUnit(key, bChain: true, bPool: true);
				if (unit != null)
				{
					nKMUnitData = unit.GetUnitData();
				}
				else
				{
					List<NKMUnit> list = new List<NKMUnit>();
					game.GetUnitByUnitID(list, checkUnitID, bChain: true, bPool: true);
					foreach (NKMUnit item in list)
					{
						if (item.GetTeam() == value.teamType && nKMGameTeamData.IsAssistUnit(item.GetUnitData().m_UnitUID) == value.isAssistUnit && item.GetUnitDataGame().m_MasterGameUnitUID != 0 == value.isSummonee)
						{
							nKMUnitData = item.GetUnitData();
							break;
						}
					}
				}
				if (nKMUnitData == null)
				{
					nKMUnitData = NKCPhaseManager.GetTempUnitData(value);
				}
				if (nKMUnitData == null)
				{
					nKMUnitData = NKMDungeonManager.MakeUnitDataFromID(checkUnitID, -1L, value.unitLevel, -1, 0);
				}
				if (checkUnitID == nKMUnitData.m_UnitID)
				{
					unitBattleData.unitData = nKMUnitData;
				}
			}
			unitBattleData.recordData.recordGiveDamage += value.recordGiveDamage;
			unitBattleData.recordData.recordTakeDamage += value.recordTakeDamage;
			unitBattleData.recordData.recordHeal += value.recordHeal;
			unitBattleData.recordData.recordSummonCount += value.recordSummonCount;
			unitBattleData.recordData.recordDieCount += value.recordDieCount;
			unitBattleData.recordData.recordKillCount += value.recordKillCount;
			unitBattleData.recordData.playtime += value.playtime;
			battleData.maxValue = Mathf.Max(battleData.maxValue, value.recordGiveDamage, value.recordTakeDamage, value.recordHeal);
			if (unitBattleData.recordData.playtime > 0)
			{
				battleData.maxDps = Mathf.Max(battleData.maxDps, unitBattleData.recordData.recordGiveDamage / (float)unitBattleData.recordData.playtime, unitBattleData.recordData.recordTakeDamage / (float)unitBattleData.recordData.playtime);
			}
		}
		foreach (UnitBattleData item2 in battleData.teamA)
		{
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(item2.unitData.m_UnitID);
			if (unitStatTemplet != null && unitStatTemplet.m_RespawnCount > 1)
			{
				item2.recordData.playtime /= unitStatTemplet.m_RespawnCount;
				item2.recordData.recordSummonCount /= unitStatTemplet.m_RespawnCount;
			}
			if (item2.recordData.playtime > 0)
			{
				float num = item2.recordData.playtime;
				battleData.maxDps = Mathf.Max(battleData.maxDps, item2.recordData.recordGiveDamage / num, item2.recordData.recordTakeDamage / num, item2.recordData.recordHeal / num);
			}
		}
		foreach (UnitBattleData item3 in battleData.teamB)
		{
			if (!item3.bSummon)
			{
				NKMUnitStatTemplet unitStatTemplet2 = NKMUnitManager.GetUnitStatTemplet(item3.unitData.m_UnitID);
				if (unitStatTemplet2 != null && unitStatTemplet2.m_RespawnCount > 1)
				{
					item3.recordData.playtime /= unitStatTemplet2.m_RespawnCount;
					item3.recordData.recordSummonCount /= unitStatTemplet2.m_RespawnCount;
				}
			}
			if (item3.recordData.playtime > 0)
			{
				float num2 = item3.recordData.playtime;
				battleData.maxDps = Mathf.Max(battleData.maxDps, item3.recordData.recordGiveDamage / num2, item3.recordData.recordTakeDamage / num2, item3.recordData.recordHeal / num2);
			}
		}
		UnitBattleData unitBattleData2 = battleData.teamA.Find((UnitBattleData v) => v.unitData.m_UnitUID == myTeamData.m_MainShip.m_UnitUID);
		if (unitBattleData2 == null)
		{
			Debug.LogError("A팀 main Ship 찾을 수 없음");
		}
		battleData.teamA.Remove(unitBattleData2);
		battleData.mainShipA = unitBattleData2;
		UnitBattleData unitBattleData3 = null;
		if (gameData.m_NKMGameTeamDataB.m_MainShip != null)
		{
			unitBattleData3 = battleData.teamB.Find((UnitBattleData v) => v.unitData.m_UnitUID == enemyTeamData.m_MainShip.m_UnitUID);
			if (unitBattleData3 == null)
			{
				Debug.LogError("B팀 main Ship 찾을 수 없음");
			}
			battleData.teamB.Remove(unitBattleData3);
		}
		else
		{
			unitBattleData3 = battleData.teamB.Find((UnitBattleData v) => v.bLeader);
			battleData.teamB.Remove(battleData.mainShipB);
		}
		battleData.mainShipB = unitBattleData3;
		return battleData;
	}

	private void Sort(bool bDps)
	{
		if (bDps)
		{
			m_battleData.teamA.Sort(SortDps);
			m_battleData.teamB.Sort(SortDps);
		}
		else
		{
			m_battleData.teamA.Sort(SortDamage);
			m_battleData.teamB.Sort(SortDamage);
		}
	}

	private int SortDamage(UnitBattleData a, UnitBattleData b)
	{
		if (a == null && b == null)
		{
			Debug.LogError("recordData null!");
			return 0;
		}
		if (a == null)
		{
			Debug.LogError("recordData null!");
			return 1;
		}
		if (b == null)
		{
			Debug.LogError("recordData null!");
			return -1;
		}
		if (a.recordData == null && b.recordData == null)
		{
			Debug.LogError($"recordData for {a.UnitID}, {b.UnitID} null?");
			return 0;
		}
		if (a.recordData == null)
		{
			Debug.LogError($"recordData for {a.UnitID} null?");
			return 1;
		}
		if (b.recordData == null)
		{
			Debug.LogError($"recordData for {b.UnitID} null?");
			return -1;
		}
		if (a.recordData.recordGiveDamage != b.recordData.recordGiveDamage)
		{
			return b.recordData.recordGiveDamage.CompareTo(a.recordData.recordGiveDamage);
		}
		return SortCommon(a, b);
	}

	private int SortDps(UnitBattleData a, UnitBattleData b)
	{
		if (a == null && b == null)
		{
			Debug.LogError("recordData null!");
			return 0;
		}
		if (a == null)
		{
			Debug.LogError("recordData null!");
			return 1;
		}
		if (b == null)
		{
			Debug.LogError("recordData null!");
			return -1;
		}
		NKMGameRecordUnitData recordData = a.recordData;
		NKMGameRecordUnitData recordData2 = b.recordData;
		if (recordData == null && recordData2 == null)
		{
			Debug.LogError($"recordData for {a.UnitID}, {b.UnitID} null?");
			return 0;
		}
		if (recordData == null)
		{
			Debug.LogError($"recordData for {a.UnitID} null?");
			return 1;
		}
		if (recordData2 == null)
		{
			Debug.LogError($"recordData for {b.UnitID} null?");
			return -1;
		}
		if (recordData.playtime <= 0)
		{
			return 1;
		}
		if (recordData2.playtime <= 0)
		{
			return -1;
		}
		float num = recordData.recordGiveDamage / (float)recordData.playtime;
		float num2 = recordData2.recordGiveDamage / (float)recordData2.playtime;
		if (num != num2)
		{
			return num2.CompareTo(num);
		}
		return SortCommon(a, b);
	}

	private int SortCommon(UnitBattleData a, UnitBattleData b)
	{
		if (a.recordData.isLeader != b.recordData.isLeader)
		{
			return a.recordData.isLeader.CompareTo(b.recordData.isLeader);
		}
		if (a.recordData.isAssistUnit != b.recordData.isAssistUnit)
		{
			return a.recordData.isAssistUnit.CompareTo(b.recordData.isAssistUnit);
		}
		if (a.recordData.isSummonee != b.recordData.isSummonee)
		{
			return a.recordData.isSummonee.CompareTo(b.recordData.isSummonee);
		}
		return a.UnitID.CompareTo(b.UnitID);
	}
}
