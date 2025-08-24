using Cs.Protocol;

namespace NKM;

public sealed class NKMDiveSlotWithIndexes : ISerializable
{
	private NKMDiveSlot slot = new NKMDiveSlot();

	private int slotSetIndex;

	private int slotIndex;

	public NKMDiveSlot Slot => slot;

	public int SlotSetIndex => slotSetIndex;

	public int SlotIndex => slotIndex;

	public NKMDiveSlotWithIndexes()
	{
	}

	public NKMDiveSlotWithIndexes(NKMDiveSlot slot, int slotSetIndex, int slotIndex)
	{
		this.slot = slot.DeepCopy();
		this.slotSetIndex = slotSetIndex;
		this.slotIndex = slotIndex;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref slot);
		stream.PutOrGet(ref slotSetIndex);
		stream.PutOrGet(ref slotIndex);
	}

	public void UpdateSlot(NKMDiveSlot slot)
	{
		this.slot = slot.DeepCopy();
	}
}
