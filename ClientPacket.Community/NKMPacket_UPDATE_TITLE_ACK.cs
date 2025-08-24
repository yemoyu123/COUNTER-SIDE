using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UPDATE_TITLE_ACK)]
public sealed class NKMPacket_UPDATE_TITLE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int titleId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref titleId);
	}
}
