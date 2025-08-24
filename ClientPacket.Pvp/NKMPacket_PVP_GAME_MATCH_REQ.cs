using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_GAME_MATCH_REQ)]
public sealed class NKMPacket_PVP_GAME_MATCH_REQ : ISerializable
{
	public byte selectDeckIndex;

	public NKM_GAME_TYPE gameType;

	public bool usingBot;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selectDeckIndex);
		stream.PutOrGetEnum(ref gameType);
		stream.PutOrGet(ref usingBot);
	}
}
