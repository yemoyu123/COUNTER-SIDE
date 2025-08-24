using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CHAT_TRANSLATE_ACK)]
public sealed class NKMPacket_GUILD_CHAT_TRANSLATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long messageUid;

	public string textTranslated;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref messageUid);
		stream.PutOrGet(ref textTranslated);
	}
}
