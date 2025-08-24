using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_PRESET_APPLY_ACK)]
public sealed class NKMPacket_OFFICE_PRESET_APPLY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int presetId;

	public NKMOfficeRoom room = new NKMOfficeRoom();

	public List<NKMUnitData> updatedUnits = new List<NKMUnitData>();

	public List<NKMInteriorData> changedInteriors = new List<NKMInteriorData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref presetId);
		stream.PutOrGet(ref room);
		stream.PutOrGet(ref updatedUnits);
		stream.PutOrGet(ref changedInteriors);
	}
}
