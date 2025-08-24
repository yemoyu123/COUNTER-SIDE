using System;
using Cs.Protocol;

namespace NKM;

public class NKMCraftSlotData : ISerializable
{
	public byte Index;

	public int MoldID;

	public int Count;

	public long CompleteDate;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref Index);
		stream.PutOrGet(ref MoldID);
		stream.PutOrGet(ref Count);
		stream.PutOrGet(ref CompleteDate);
	}

	public NKM_CRAFT_SLOT_STATE GetState(DateTime curTimeUTC)
	{
		NKM_CRAFT_SLOT_STATE result = NKM_CRAFT_SLOT_STATE.NECSS_EMPTY;
		if (MoldID > 0)
		{
			result = ((curTimeUTC.Ticks < CompleteDate) ? NKM_CRAFT_SLOT_STATE.NECSS_CREATING_NOW : NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED);
		}
		return result;
	}
}
