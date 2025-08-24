using System;
using System.Collections.Generic;
using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public class NKMDiveFloor : ISerializable
{
	protected int stageID;

	protected NKMDiveTemplet templet;

	protected List<NKMDiveSlotSet> slotSets = new List<NKMDiveSlotSet>();

	protected long expireDate;

	public const int MAX_DISCOVERED_SLOT_SET_COUNT = 2;

	public NKMDiveTemplet Templet => templet;

	public List<NKMDiveSlotSet> SlotSets => slotSets;

	public long ExpireDate
	{
		get
		{
			return expireDate;
		}
		set
		{
			expireDate = value;
		}
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageID);
		stream.PutOrGet(ref slotSets);
		stream.PutOrGet(ref expireDate);
		if (stream is PacketReader)
		{
			OnPacketRead();
		}
	}

	public void OnPacketRead()
	{
		templet = NKMDiveTemplet.Find(stageID);
	}

	public NKMDiveSlot GetSlot(int curSlotSetIndex, int cutSlotIndex)
	{
		if (curSlotSetIndex >= SlotSets.Count)
		{
			return null;
		}
		NKMDiveSlotSet nKMDiveSlotSet = SlotSets[curSlotSetIndex];
		if (cutSlotIndex >= nKMDiveSlotSet.Slots.Count)
		{
			return null;
		}
		return nKMDiveSlotSet.Slots[cutSlotIndex];
	}

	public bool isExpire(DateTime now)
	{
		return now.Ticks >= expireDate;
	}

	public int GetDiveStormCostCount()
	{
		return templet.RandomSetCount * NKMDiveTemplet.DiveStormCostMultiply;
	}

	public int GetDiveStormRewardCount()
	{
		return templet.RandomSetCount * NKMDiveTemplet.DiveStormRewardMultiply;
	}

	public void Rebuild(int distance, int nextSlotSetIndex, int nextSlotIndex)
	{
		NKMDiveSlotSet nKMDiveSlotSet = SlotSets[nextSlotSetIndex];
		NKMDiveSlot item = nKMDiveSlotSet.Slots[nextSlotIndex];
		if (distance > 0)
		{
			SlotSets.RemoveAt(0);
		}
		nKMDiveSlotSet.Slots.Clear();
		nKMDiveSlotSet.Slots.Add(item);
	}
}
