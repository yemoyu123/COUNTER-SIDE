using System;
using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMDivePlayer : ISerializable
{
	private class CompDiveSquad : IComparer<NKMDiveSquad>
	{
		public int Compare(NKMDiveSquad x, NKMDiveSquad y)
		{
			if (x.CurHp <= 0f)
			{
				return 1;
			}
			if (y.CurHp <= 0f)
			{
				return -1;
			}
			if (x.MaxHp <= 0f)
			{
				return 1;
			}
			if (y.MaxHp <= 0f)
			{
				return -1;
			}
			if (y.DeckIndex <= x.DeckIndex)
			{
				return 1;
			}
			return -1;
		}
	}

	private const int MaxArtifactCount = 50;

	private NKMDivePlayerBase playerBase = new NKMDivePlayerBase();

	private Dictionary<int, NKMDiveSquad> squads = new Dictionary<int, NKMDiveSquad>();

	public NKMDivePlayerBase PlayerBase => playerBase;

	public Dictionary<int, NKMDiveSquad> Squads => squads;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref playerBase);
		stream.PutOrGet(ref squads);
	}

	public NKMDiveSquad GetSquad(int deckIndex)
	{
		NKMDiveSquad value = null;
		if (!squads.TryGetValue(deckIndex, out value))
		{
			return null;
		}
		return value;
	}

	public void GetMovableSlotRange(NKMDiveFloor floor, int nextSlotSetIndex, out int minSlotIndex, out int maxSlotIndex)
	{
		int num = floor.SlotSets[nextSlotSetIndex].Slots.Count - 1;
		int num2 = floor.Templet.RandomSetCount + 1;
		if (playerBase.Distance == 0)
		{
			minSlotIndex = 0;
			maxSlotIndex = num;
			return;
		}
		if (playerBase.Distance == num2 - 1)
		{
			minSlotIndex = 0;
			maxSlotIndex = 0;
			return;
		}
		minSlotIndex = playerBase.SlotIndex - 1;
		maxSlotIndex = playerBase.SlotIndex + 1;
		minSlotIndex = Math.Max(0, minSlotIndex);
		maxSlotIndex = Math.Min(maxSlotIndex, num);
	}

	public bool CanMove(NKMDiveFloor floor, int nextSlotSetIndex, int slotIndex)
	{
		int num = floor.Templet.RandomSetCount + 1;
		if (playerBase.Distance == num)
		{
			return false;
		}
		GetMovableSlotRange(floor, nextSlotSetIndex, out var minSlotIndex, out var maxSlotIndex);
		if (minSlotIndex > slotIndex || slotIndex > maxSlotIndex)
		{
			return false;
		}
		return true;
	}

	public int GetNextSlotSetIndex()
	{
		if (playerBase.Distance != 0)
		{
			return playerBase.SlotSetIndex + 1;
		}
		return 0;
	}

	public bool IsInBattle()
	{
		if (playerBase.State != NKMDivePlayerState.BattleLoad)
		{
			return playerBase.State == NKMDivePlayerState.Battle;
		}
		return true;
	}

	public bool CanGetMoreArtifact()
	{
		return playerBase.Artifacts.Count < 50;
	}

	public NKMDiveSquad GetSquadForBattleByAuto()
	{
		List<NKMDiveSquad> list = new List<NKMDiveSquad>(squads.Values);
		list.Sort(new CompDiveSquad());
		if (list.Count > 0)
		{
			return list[0];
		}
		return null;
	}

	public bool CheckExistPossibleSquadForBattle()
	{
		foreach (KeyValuePair<int, NKMDiveSquad> squad in squads)
		{
			NKMDiveSquad value = squad.Value;
			if (value != null && value.CurHp > 0f && value.Supply > 0)
			{
				return true;
			}
		}
		return false;
	}
}
