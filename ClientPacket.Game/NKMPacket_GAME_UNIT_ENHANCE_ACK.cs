using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_UNIT_ENHANCE_ACK)]
public sealed class NKMPacket_GAME_UNIT_ENHANCE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long unitUID;

	public sbyte enhanceCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref enhanceCount);
	}
}
