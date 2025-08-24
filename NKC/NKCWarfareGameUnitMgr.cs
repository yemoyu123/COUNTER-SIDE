using System.Collections.Generic;
using System.Linq;
using ClientPacket.Warfare;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKCWarfareGameUnitMgr
{
	private Transform m_NUM_WARFARE_UNIT_LIST_transform;

	private Transform m_NUM_WARFARE_UNIT_INFO_LIST_transform;

	private Dictionary<int, NKCWarfareGameUnit> m_dicNKCWarfareGameUnit = new Dictionary<int, NKCWarfareGameUnit>();

	private Dictionary<int, NKCWarfareGameUnitInfo> m_dicNKCWarfareGameUnitInfo = new Dictionary<int, NKCWarfareGameUnitInfo>();

	public NKCWarfareGameUnitMgr(Transform _NUM_WARFARE_UNIT_LIST_transform, Transform _NUM_WARFARE_UNIT_INFO_LIST_transform)
	{
		m_NUM_WARFARE_UNIT_LIST_transform = _NUM_WARFARE_UNIT_LIST_transform;
		m_NUM_WARFARE_UNIT_INFO_LIST_transform = _NUM_WARFARE_UNIT_INFO_LIST_transform;
	}

	public void Init()
	{
	}

	public NKCWarfareGameUnit GetNKCWarfareGameUnit(int gameUID)
	{
		if (m_dicNKCWarfareGameUnit.ContainsKey(gameUID))
		{
			return m_dicNKCWarfareGameUnit[gameUID];
		}
		return null;
	}

	public NKCWarfareGameUnitInfo GetNKCWarfareGameUnitInfo(int gameUID)
	{
		if (m_dicNKCWarfareGameUnitInfo.ContainsKey(gameUID))
		{
			return m_dicNKCWarfareGameUnitInfo[gameUID];
		}
		return null;
	}

	public void ShowUserUnitTileFX(WarfareUnitData cNKMWarfareUnitData, WarfareUnitSyncData cNKMWarfareUnitSyncData)
	{
		NKCWarfareGameUnit nKCWarfareGameUnit = GetNKCWarfareGameUnit(cNKMWarfareUnitData.warfareGameUnitUID);
		if (nKCWarfareGameUnit != null)
		{
			nKCWarfareGameUnit.ShowUserUnitTileFX(cNKMWarfareUnitSyncData);
		}
	}

	public int GetRemainTurnOnUserUnitCount()
	{
		int num = 0;
		int num2 = 0;
		for (num2 = 0; num2 < m_dicNKCWarfareGameUnitInfo.Count; num2++)
		{
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_dicNKCWarfareGameUnitInfo.Values.ToList()[num2];
			if (nKCWarfareGameUnitInfo != null && nKCWarfareGameUnitInfo.GetNKMWarfareUnitData() != null && nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User && nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().hp > 0f && !nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().isTurnEnd)
			{
				num++;
			}
		}
		return num;
	}

	public void UpdateGameUnitUI()
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnitInfo.Count; num++)
		{
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_dicNKCWarfareGameUnitInfo.Values.ToList()[num];
			if (nKCWarfareGameUnitInfo != null && nKCWarfareGameUnitInfo.GetNKMWarfareUnitData() != null)
			{
				UpdateGameUnitUI(nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().warfareGameUnitUID);
			}
		}
	}

	public void UpdateGameUnitUI(int guuid)
	{
		UpdateGameUnitInfoUI(guuid);
		UpdateGameUnitTurnUI(guuid);
	}

	public void UpdateGameUnitInfoUI(int gameUnitUid)
	{
		NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = GetNKCWarfareGameUnitInfo(gameUnitUid);
		if (nKCWarfareGameUnitInfo != null)
		{
			nKCWarfareGameUnitInfo.SetUnitInfoUI();
		}
	}

	public void UpdateGameUnitTurnUI(int gameUnitUid)
	{
		NKCWarfareGameUnit nKCWarfareGameUnit = GetNKCWarfareGameUnit(gameUnitUid);
		if (nKCWarfareGameUnit != null)
		{
			nKCWarfareGameUnit.UpdateTurnUI();
		}
	}

	public bool CheckExistMovingUserUnit()
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnit.Count; num++)
		{
			NKCWarfareGameUnit nKCWarfareGameUnit = m_dicNKCWarfareGameUnit.Values.ToList()[num];
			if (nKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User && nKCWarfareGameUnit.IsMoving())
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckExistFlagUserUnit()
	{
		for (int i = 0; i < m_dicNKCWarfareGameUnitInfo.Count; i++)
		{
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_dicNKCWarfareGameUnitInfo.Values.ToList()[i];
			if (nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User && nKCWarfareGameUnitInfo.GetFlag())
			{
				return true;
			}
		}
		return false;
	}

	public void OnClickGameStart(NKMPacket_WARFARE_GAME_START_REQ startReq, NKMWarfareMapTemplet cNKMWarfareMapTemplet)
	{
		if (startReq == null || cNKMWarfareMapTemplet == null)
		{
			return;
		}
		List<int> list = m_dicNKCWarfareGameUnitInfo.Keys.ToList();
		list.Sort();
		for (int i = 0; i < list.Count; i++)
		{
			if (m_dicNKCWarfareGameUnitInfo.TryGetValue(list[i], out var value) && value.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User)
			{
				if (!value.IsSupporter)
				{
					NKMPacket_WARFARE_GAME_START_REQ.UnitPosition item = new NKMPacket_WARFARE_GAME_START_REQ.UnitPosition
					{
						isFlagShip = value.GetFlag(),
						deckIndex = value.GetNKMWarfareUnitData().deckIndex.m_iIndex,
						tileIndex = (short)value.TileIndex
					};
					startReq.unitPositionList.Add(item);
				}
				else
				{
					startReq.friendCode = value.FriendCode;
					startReq.friendTileIndex = (short)value.TileIndex;
				}
			}
		}
	}

	public int GetCurrentUserUnit(bool excludeSupporter = true)
	{
		int num = 0;
		for (int i = 0; i < m_dicNKCWarfareGameUnit.Count; i++)
		{
			NKCWarfareGameUnit nKCWarfareGameUnit = m_dicNKCWarfareGameUnit.Values.ToList()[i];
			if (nKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User && !(nKCWarfareGameUnit.IsSupporter && excludeSupporter))
			{
				num++;
			}
		}
		return num;
	}

	public List<int> GetCurrentUserUnitTileIndex()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < m_dicNKCWarfareGameUnit.Count; i++)
		{
			NKCWarfareGameUnit nKCWarfareGameUnit = m_dicNKCWarfareGameUnit.Values.ToList()[i];
			if (nKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User && !nKCWarfareGameUnit.IsSupporter)
			{
				list.Add(nKCWarfareGameUnit.TileIndex);
			}
		}
		return list;
	}

	public bool ContainSupporterUnit()
	{
		for (int i = 0; i < m_dicNKCWarfareGameUnit.Count; i++)
		{
			NKCWarfareGameUnit nKCWarfareGameUnit = m_dicNKCWarfareGameUnit.Values.ToList()[i];
			if (nKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User && nKCWarfareGameUnit.IsSupporter)
			{
				return true;
			}
		}
		return false;
	}

	public void PauseUnits(bool bSet)
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnit.Count; num++)
		{
			NKCWarfareGameUnit nKCWarfareGameUnit = m_dicNKCWarfareGameUnit.Values.ToList()[num];
			if (nKCWarfareGameUnit != null)
			{
				nKCWarfareGameUnit.SetPause(bSet);
			}
		}
	}

	public void ClearUnit(int gameUnitUID)
	{
		NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = GetNKCWarfareGameUnitInfo(gameUnitUID);
		if (nKCWarfareGameUnitInfo != null)
		{
			ResetDeckState(nKCWarfareGameUnitInfo.GetNKMWarfareUnitData());
			nKCWarfareGameUnitInfo.SetUnitTransform(null);
			nKCWarfareGameUnitInfo.gameObject.transform.SetParent(null);
			nKCWarfareGameUnitInfo.Close();
		}
		m_dicNKCWarfareGameUnitInfo.Remove(gameUnitUID);
		NKCWarfareGameUnit nKCWarfareGameUnit = GetNKCWarfareGameUnit(gameUnitUID);
		if (nKCWarfareGameUnit != null)
		{
			nKCWarfareGameUnit.gameObject.transform.SetParent(null);
			nKCWarfareGameUnit.Close();
		}
		m_dicNKCWarfareGameUnit.Remove(gameUnitUID);
	}

	private void ResetDeckState(WarfareUnitData cNKMWarfareUnitData)
	{
		if (NKCScenManager.GetScenManager().WarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP && cNKMWarfareUnitData.unitType == WarfareUnitData.Type.User && cNKMWarfareUnitData.friendCode == 0L)
		{
			NKMDeckData deckData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(cNKMWarfareUnitData.deckIndex);
			if (deckData != null)
			{
				deckData.SetState(NKM_DECK_STATE.DECK_STATE_NORMAL);
				NKCScenManager.CurrentUserData().m_ArmyData.DeckUpdated(cNKMWarfareUnitData.deckIndex, deckData);
			}
		}
	}

	public void ResetAllDeckState()
	{
		for (int i = 0; i < m_dicNKCWarfareGameUnitInfo.Count; i++)
		{
			WarfareUnitData nKMWarfareUnitData = m_dicNKCWarfareGameUnitInfo.Values.ToList()[i].GetNKMWarfareUnitData();
			if (nKMWarfareUnitData != null)
			{
				ResetDeckState(nKMWarfareUnitData);
			}
		}
	}

	public void SetUserUnitDeckWarfareState()
	{
		for (int i = 0; i < m_dicNKCWarfareGameUnitInfo.Count; i++)
		{
			WarfareUnitData nKMWarfareUnitData = m_dicNKCWarfareGameUnitInfo.Values.ToList()[i].GetNKMWarfareUnitData();
			if (nKMWarfareUnitData != null && nKMWarfareUnitData.unitType == WarfareUnitData.Type.User && nKMWarfareUnitData.friendCode == 0L)
			{
				NKMDeckData deckData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(nKMWarfareUnitData.deckIndex);
				if (deckData != null)
				{
					deckData.SetState(NKM_DECK_STATE.DECK_STATE_WARFARE);
					NKCScenManager.CurrentUserData().m_ArmyData.DeckUpdated(nKMWarfareUnitData.deckIndex, deckData);
				}
			}
		}
	}

	public void ClearUnits()
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnitInfo.Count; num++)
		{
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_dicNKCWarfareGameUnitInfo.Values.ToList()[num];
			ResetDeckState(nKCWarfareGameUnitInfo.GetNKMWarfareUnitData());
			nKCWarfareGameUnitInfo.SetUnitTransform(null);
			nKCWarfareGameUnitInfo.gameObject.transform.SetParent(null);
			nKCWarfareGameUnitInfo.Close();
		}
		m_dicNKCWarfareGameUnitInfo.Clear();
		for (num = 0; num < m_dicNKCWarfareGameUnit.Count; num++)
		{
			NKCWarfareGameUnit nKCWarfareGameUnit = m_dicNKCWarfareGameUnit.Values.ToList()[num];
			nKCWarfareGameUnit.gameObject.transform.SetParent(null);
			nKCWarfareGameUnit.Close();
		}
		m_dicNKCWarfareGameUnit.Clear();
	}

	public void RefreshDicUnit()
	{
		Dictionary<int, NKCWarfareGameUnit> dictionary = new Dictionary<int, NKCWarfareGameUnit>(m_dicNKCWarfareGameUnit);
		Dictionary<int, NKCWarfareGameUnitInfo> dictionary2 = new Dictionary<int, NKCWarfareGameUnitInfo>(m_dicNKCWarfareGameUnitInfo);
		m_dicNKCWarfareGameUnit.Clear();
		m_dicNKCWarfareGameUnitInfo.Clear();
		Dictionary<int, NKCWarfareGameUnit>.Enumerator enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKCWarfareGameUnit value = enumerator.Current.Value;
			if (value.GetNKMWarfareUnitData() != null)
			{
				m_dicNKCWarfareGameUnit.Add(value.GetNKMWarfareUnitData().warfareGameUnitUID, value);
			}
		}
		dictionary.Clear();
		Dictionary<int, NKCWarfareGameUnitInfo>.Enumerator enumerator2 = dictionary2.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			NKCWarfareGameUnitInfo value2 = enumerator2.Current.Value;
			if (value2.GetNKMWarfareUnitData() != null)
			{
				m_dicNKCWarfareGameUnitInfo.Add(value2.GetNKMWarfareUnitData().warfareGameUnitUID, value2);
			}
		}
		dictionary2.Clear();
	}

	public NKCWarfareGameUnit GetGameUnitByTileIndex(int tileIndex)
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnit.Count; num++)
		{
			NKCWarfareGameUnit nKCWarfareGameUnit = m_dicNKCWarfareGameUnit.Values.ToList()[num];
			if (nKCWarfareGameUnit != null && nKCWarfareGameUnit.TileIndex == tileIndex)
			{
				return nKCWarfareGameUnit;
			}
		}
		return null;
	}

	public NKCWarfareGameUnit GetWFGameUnitByDeckIndex(NKMDeckIndex sNKMDeckIndex)
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnit.Count; num++)
		{
			NKCWarfareGameUnit nKCWarfareGameUnit = m_dicNKCWarfareGameUnit.Values.ToList()[num];
			if (nKCWarfareGameUnit != null && nKCWarfareGameUnit.GetNKMWarfareUnitData() != null && nKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User && nKCWarfareGameUnit.GetNKMWarfareUnitData().deckIndex.Compare(sNKMDeckIndex))
			{
				return nKCWarfareGameUnit;
			}
		}
		return null;
	}

	public NKCWarfareGameUnitInfo GetWFGameUnitInfoByTileIndex(int tileIndex)
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnitInfo.Count; num++)
		{
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_dicNKCWarfareGameUnitInfo.Values.ToList()[num];
			if (nKCWarfareGameUnitInfo != null && nKCWarfareGameUnitInfo.TileIndex == tileIndex)
			{
				return nKCWarfareGameUnitInfo;
			}
		}
		return null;
	}

	public NKCWarfareGameUnitInfo GetWFGameUnitInfoByWFUnitData(WarfareUnitData cNKMWarfareUnitData)
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnitInfo.Count; num++)
		{
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_dicNKCWarfareGameUnitInfo.Values.ToList()[num];
			if (nKCWarfareGameUnitInfo != null && nKCWarfareGameUnitInfo.GetNKMWarfareUnitData() == cNKMWarfareUnitData)
			{
				return nKCWarfareGameUnitInfo;
			}
		}
		return null;
	}

	public void SetUserFlagShip(int gameUnitUID, bool bPlayAni = false)
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnitInfo.Count; num++)
		{
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_dicNKCWarfareGameUnitInfo.Values.ToList()[num];
			if (nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User)
			{
				nKCWarfareGameUnitInfo.SetFlag(nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().warfareGameUnitUID == gameUnitUID);
				nKCWarfareGameUnitInfo.SetUnitInfoUI();
				if (bPlayAni && nKCWarfareGameUnitInfo.GetFlag())
				{
					nKCWarfareGameUnitInfo.PlayFlagAni();
				}
			}
		}
	}

	public void ResetUserFlagShip(bool bPlayAni = false)
	{
		bool flag = true;
		for (int i = 0; i < m_dicNKCWarfareGameUnitInfo.Count; i++)
		{
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_dicNKCWarfareGameUnitInfo.Values.ToList()[i];
			if (nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().unitType != WarfareUnitData.Type.User || nKCWarfareGameUnitInfo.IsSupporter)
			{
				continue;
			}
			nKCWarfareGameUnitInfo.SetFlag(flag);
			nKCWarfareGameUnitInfo.SetUnitInfoUI();
			if (flag)
			{
				if (bPlayAni)
				{
					nKCWarfareGameUnitInfo.PlayFlagAni();
				}
				flag = false;
			}
		}
	}

	public void SetFlagDungeon(int gameUnitUID)
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnitInfo.Count; num++)
		{
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_dicNKCWarfareGameUnitInfo.Values.ToList()[num];
			if (nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.Dungeon)
			{
				nKCWarfareGameUnitInfo.SetFlag(nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().warfareGameUnitUID == gameUnitUID);
				nKCWarfareGameUnitInfo.SetUnitInfoUI();
			}
		}
	}

	public NKCWarfareGameUnit CreateNewEnemyUnit(string dungeonStrID, bool bFlag, bool bTarget, short tileIndex, NKM_WARFARE_ENEMY_ACTION_TYPE actionType, NKCWarfareGameUnit.onClickUnit onClickUnit, WarfareUnitData cNewNKMWarfareUnitData = null)
	{
		int num = 0;
		if (cNewNKMWarfareUnitData != null)
		{
			num = cNewNKMWarfareUnitData.warfareGameUnitUID;
			if (m_dicNKCWarfareGameUnit.ContainsKey(num))
			{
				return null;
			}
		}
		else
		{
			for (num = m_dicNKCWarfareGameUnit.Count; m_dicNKCWarfareGameUnit.ContainsKey(num); num++)
			{
			}
		}
		NKCWarfareGameUnit newInstance = NKCWarfareGameUnit.GetNewInstance(m_NUM_WARFARE_UNIT_LIST_transform, onClickUnit);
		if (newInstance == null)
		{
			return null;
		}
		NKCWarfareGameUnitInfo newInstance2 = NKCWarfareGameUnitInfo.GetNewInstance(m_NUM_WARFARE_UNIT_INFO_LIST_transform, newInstance.gameObject.transform);
		WarfareUnitData warfareUnitData = null;
		if (cNewNKMWarfareUnitData != null)
		{
			warfareUnitData = cNewNKMWarfareUnitData;
		}
		else
		{
			warfareUnitData = new WarfareUnitData();
			warfareUnitData.warfareGameUnitUID = num;
			warfareUnitData.unitType = WarfareUnitData.Type.Dungeon;
			warfareUnitData.dungeonID = NKMDungeonManager.GetDungeonID(dungeonStrID);
			warfareUnitData.warfareEnemyActionType = actionType;
			warfareUnitData.tileIndex = tileIndex;
		}
		newInstance.SetNKMWarfareUnitData(warfareUnitData);
		newInstance.OneTimeSetUnitUI();
		if (newInstance2 != null)
		{
			newInstance2.SetNKMWarfareUnitData(warfareUnitData);
			newInstance2.SetFlag(bFlag);
			newInstance2.SetTartget(bTarget);
			newInstance2.SetUnitInfoUI();
		}
		m_dicNKCWarfareGameUnit.Add(num, newInstance);
		m_dicNKCWarfareGameUnitInfo.Add(num, newInstance2);
		return newInstance;
	}

	public NKCWarfareGameUnit CreateNewUserUnit(NKMDeckIndex selectedDeckIndex, short tileIndex, NKCWarfareGameUnit.onClickUnit onClickUnit, WarfareUnitData cNewNKMWarfareUnitData = null, long friendConde = 0L)
	{
		int num = 0;
		if (cNewNKMWarfareUnitData != null)
		{
			num = cNewNKMWarfareUnitData.warfareGameUnitUID;
			if (m_dicNKCWarfareGameUnit.ContainsKey(num))
			{
				return null;
			}
		}
		else
		{
			for (num = m_dicNKCWarfareGameUnit.Count; m_dicNKCWarfareGameUnit.ContainsKey(num); num++)
			{
			}
		}
		NKCWarfareGameUnit newInstance = NKCWarfareGameUnit.GetNewInstance(m_NUM_WARFARE_UNIT_LIST_transform, onClickUnit);
		if (newInstance == null)
		{
			return null;
		}
		NKCWarfareGameUnitInfo newInstance2 = NKCWarfareGameUnitInfo.GetNewInstance(m_NUM_WARFARE_UNIT_INFO_LIST_transform, newInstance.gameObject.transform);
		WarfareUnitData warfareUnitData = null;
		if (cNewNKMWarfareUnitData != null)
		{
			warfareUnitData = cNewNKMWarfareUnitData;
		}
		else
		{
			warfareUnitData = new WarfareUnitData();
			warfareUnitData.warfareGameUnitUID = num;
			warfareUnitData.unitType = WarfareUnitData.Type.User;
			warfareUnitData.deckIndex = selectedDeckIndex;
			warfareUnitData.supply = 2;
			warfareUnitData.tileIndex = tileIndex;
			warfareUnitData.friendCode = friendConde;
		}
		newInstance.SetNKMWarfareUnitData(warfareUnitData);
		newInstance.OneTimeSetUnitUI();
		if (newInstance2 != null)
		{
			newInstance2.SetNKMWarfareUnitData(warfareUnitData);
			newInstance2.SetUnitInfoUI();
		}
		m_dicNKCWarfareGameUnit.Add(num, newInstance);
		m_dicNKCWarfareGameUnitInfo.Add(num, newInstance2);
		return newInstance;
	}

	public bool CheckDuplicateDeckIndex(NKMDeckIndex deckIndex)
	{
		int num = 0;
		for (num = 0; num < m_dicNKCWarfareGameUnitInfo.Count; num++)
		{
			NKCWarfareGameUnitInfo nKCWarfareGameUnitInfo = m_dicNKCWarfareGameUnitInfo.Values.ToList()[num];
			if (nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.User && nKCWarfareGameUnitInfo.GetNKMWarfareUnitData().deckIndex.Compare(deckIndex))
			{
				return true;
			}
		}
		return false;
	}

	public void ResetIcon(int unitUID = 0)
	{
		for (int i = 0; i < m_dicNKCWarfareGameUnit.Count; i++)
		{
			NKCWarfareGameUnit nKCWarfareGameUnit = m_dicNKCWarfareGameUnit.Values.ToList()[i];
			if (nKCWarfareGameUnit.GetNKMWarfareUnitData().unitType == WarfareUnitData.Type.Dungeon)
			{
				if (nKCWarfareGameUnit.GetNKMWarfareUnitData().warfareGameUnitUID != unitUID)
				{
					nKCWarfareGameUnit.SetAttackIcon(bActive: false);
				}
			}
			else
			{
				nKCWarfareGameUnit.SetChangeIcon(bActive: false);
			}
		}
		for (int j = 0; j < m_dicNKCWarfareGameUnitInfo.Count; j++)
		{
			m_dicNKCWarfareGameUnitInfo.Values.ToList()[j].SetBattleAssistIcon(bActive: false);
		}
	}

	public void Hide()
	{
		List<NKCWarfareGameUnit> list = m_dicNKCWarfareGameUnit.Values.ToList();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].HideFX();
		}
	}

	public void OnStartGameVoice()
	{
		for (int i = 0; i < m_dicNKCWarfareGameUnitInfo.Count; i++)
		{
			WarfareUnitData nKMWarfareUnitData = m_dicNKCWarfareGameUnitInfo.Values.ToList()[i].GetNKMWarfareUnitData();
			if (nKMWarfareUnitData != null && nKMWarfareUnitData.unitType == WarfareUnitData.Type.User && nKMWarfareUnitData.friendCode == 0L)
			{
				NKCOperatorUtil.PlayVoice(nKMWarfareUnitData.deckIndex, VOICE_TYPE.VT_FIELD_READY, bStopCurrentVoice: false);
			}
		}
	}
}
