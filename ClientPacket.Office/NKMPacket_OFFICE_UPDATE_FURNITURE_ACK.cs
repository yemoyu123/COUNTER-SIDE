using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_UPDATE_FURNITURE_ACK)]
public sealed class NKMPacket_OFFICE_UPDATE_FURNITURE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMOfficeRoom room = new NKMOfficeRoom();

	public NKMOfficeFurniture furniture = new NKMOfficeFurniture();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref room);
		stream.PutOrGet(ref furniture);
	}
}
