using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_ASYNC_PVP_START_GAME_REQ)]
public sealed class NKMPacket_ASYNC_PVP_START_GAME_REQ : ISerializable
{
	public long targetFriendCode;

	public byte selectDeckIndex;

	public NKM_GAME_TYPE gameType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref targetFriendCode);
		stream.PutOrGet(ref selectDeckIndex);
		stream.PutOrGetEnum(ref gameType);
	}
}
