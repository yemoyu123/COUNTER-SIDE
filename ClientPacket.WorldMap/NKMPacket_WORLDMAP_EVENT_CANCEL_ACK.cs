using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_EVENT_CANCEL_ACK)]
public sealed class NKMPacket_WORLDMAP_EVENT_CANCEL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int cityID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref cityID);
	}
}
