using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK)]
public sealed class NKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMDungeonClearData dungeonClearData = new NKMDungeonClearData();

	public NKMEpisodeCompleteData episodeCompleteData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref dungeonClearData);
		stream.PutOrGet(ref episodeCompleteData);
	}
}
