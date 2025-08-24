using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_ADD_FURNITURE_ACK)]
public sealed class NKMPacket_OFFICE_ADD_FURNITURE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMOfficeRoom room = new NKMOfficeRoom();

	public NKMOfficeFurniture furniture = new NKMOfficeFurniture();

	public NKMInteriorData changedInterior = new NKMInteriorData();

	public List<NKMUnitData> updatedUnits = new List<NKMUnitData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref room);
		stream.PutOrGet(ref furniture);
		stream.PutOrGet(ref changedInterior);
		stream.PutOrGet(ref updatedUnits);
	}
}
