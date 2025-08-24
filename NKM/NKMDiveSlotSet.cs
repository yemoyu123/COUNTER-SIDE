using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public sealed class NKMDiveSlotSet : ISerializable
{
	private List<NKMDiveSlot> slots = new List<NKMDiveSlot>();

	public List<NKMDiveSlot> Slots => slots;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref slots);
	}
}
