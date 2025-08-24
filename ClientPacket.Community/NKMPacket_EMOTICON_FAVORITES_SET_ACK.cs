using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_EMOTICON_FAVORITES_SET_ACK)]
public sealed class NKMPacket_EMOTICON_FAVORITES_SET_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMEmoticonData emoticon = new NKMEmoticonData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref emoticon);
	}
}
