using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_GAME_NEXT_ORDER_ACK)]
public sealed class NKMPacket_WARFARE_GAME_NEXT_ORDER_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public WarfareSyncData warfareSyncData = new WarfareSyncData();

	public NKMWarfareClearData warfareClearData;

	public NKMEpisodeCompleteData episodeCompleteData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref warfareSyncData);
		stream.PutOrGet(ref warfareClearData);
		stream.PutOrGet(ref episodeCompleteData);
	}
}
