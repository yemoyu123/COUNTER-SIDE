using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_SHADOW_PALACE_GIVEUP_ACK)]
public sealed class NKMPacket_SHADOW_PALACE_GIVEUP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int palaceId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref palaceId);
	}
}
