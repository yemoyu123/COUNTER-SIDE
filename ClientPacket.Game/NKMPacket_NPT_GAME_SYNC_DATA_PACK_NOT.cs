using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT)]
public sealed class NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT : ISerializable
{
	public float gameTime;

	public float absoluteGameTime;

	public NKMGameSyncDataPack gameSyncDataPack;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref gameTime);
		stream.PutOrGet(ref absoluteGameTime);
		stream.PutOrGet(ref gameSyncDataPack);
	}
}
