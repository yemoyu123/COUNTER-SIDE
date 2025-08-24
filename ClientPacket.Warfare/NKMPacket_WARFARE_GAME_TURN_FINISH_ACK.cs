using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_GAME_TURN_FINISH_ACK)]
public sealed class NKMPacket_WARFARE_GAME_TURN_FINISH_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public WarfareSyncData warfareSyncData = new WarfareSyncData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref warfareSyncData);
	}
}
