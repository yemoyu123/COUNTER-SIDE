using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_MINI_GAME_INFO_REQ)]
public sealed class NKMPacket_MINI_GAME_INFO_REQ : ISerializable
{
	public NKM_MINI_GAME_TYPE miniGameType;

	public int templetId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref miniGameType);
		stream.PutOrGet(ref templetId);
	}
}
