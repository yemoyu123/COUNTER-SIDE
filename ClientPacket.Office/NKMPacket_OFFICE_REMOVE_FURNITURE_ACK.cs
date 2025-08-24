using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_REMOVE_FURNITURE_ACK)]
public sealed class NKMPacket_OFFICE_REMOVE_FURNITURE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long furnitureUid;

	public NKMOfficeRoom room = new NKMOfficeRoom();

	public NKMInteriorData changedInterior = new NKMInteriorData();

	public List<NKMUnitData> updatedUnits = new List<NKMUnitData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref furnitureUid);
		stream.PutOrGet(ref room);
		stream.PutOrGet(ref changedInterior);
		stream.PutOrGet(ref updatedUnits);
	}
}
