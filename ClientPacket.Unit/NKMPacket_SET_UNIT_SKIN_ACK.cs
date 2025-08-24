using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SET_UNIT_SKIN_ACK)]
public sealed class NKMPacket_SET_UNIT_SKIN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long unitUID;

	public int skinID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref skinID);
	}
}
