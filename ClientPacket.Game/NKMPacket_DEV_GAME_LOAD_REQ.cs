using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_DEV_GAME_LOAD_REQ)]
public sealed class NKMPacket_DEV_GAME_LOAD_REQ : ISerializable
{
	public NKMGameData gamedata;

	public NKMGameRuntimeData gameRuntimeData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref gamedata);
		stream.PutOrGet(ref gameRuntimeData);
	}
}
