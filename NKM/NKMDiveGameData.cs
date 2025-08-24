using Cs.Protocol;

namespace NKM;

public class NKMDiveGameData : ISerializable
{
	private long diveUid;

	private NKMDiveFloor floor;

	private NKMDivePlayer player;

	public long DiveUid
	{
		get
		{
			return diveUid;
		}
		set
		{
			diveUid = value;
		}
	}

	public NKMDiveFloor Floor
	{
		get
		{
			return floor;
		}
		set
		{
			floor = value;
		}
	}

	public NKMDivePlayer Player
	{
		get
		{
			return player;
		}
		set
		{
			player = value;
		}
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref diveUid);
		stream.PutOrGet(ref floor);
		stream.PutOrGet(ref player);
	}

	public void UpdateData(bool isWin, NKMDiveSyncData cNKMDiveSyncData)
	{
		if (cNKMDiveSyncData == null)
		{
			return;
		}
		for (int i = 0; i < cNKMDiveSyncData.UpdatedSlots.Count; i++)
		{
			NKMDiveSlotWithIndexes nKMDiveSlotWithIndexes = cNKMDiveSyncData.UpdatedSlots[i];
			if (nKMDiveSlotWithIndexes != null)
			{
				Floor.GetSlot(nKMDiveSlotWithIndexes.SlotSetIndex, nKMDiveSlotWithIndexes.SlotIndex)?.DeepCopyFrom(nKMDiveSlotWithIndexes.Slot);
			}
		}
		if (cNKMDiveSyncData.UpdatedPlayer != null && isWin)
		{
			Floor.Rebuild(Player.PlayerBase.Distance, Player.PlayerBase.SlotSetIndex, Player.PlayerBase.SlotIndex);
		}
		if (cNKMDiveSyncData.UpdatedPlayer != null)
		{
			Player.PlayerBase.DeepCopyFromSource(cNKMDiveSyncData.UpdatedPlayer);
		}
		if (cNKMDiveSyncData.UpdatedSquads.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < cNKMDiveSyncData.UpdatedSquads.Count; j++)
		{
			NKMDiveSquad nKMDiveSquad = cNKMDiveSyncData.UpdatedSquads[j];
			if (nKMDiveSquad != null)
			{
				NKMDiveSquad value = null;
				Player.Squads.TryGetValue(nKMDiveSquad.DeckIndex, out value);
				value?.DeepCopyFromSource(nKMDiveSquad);
			}
		}
	}
}
