using Cs.Protocol;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_GAME_USE_SERVICE_REQ)]
public sealed class NKMPacket_WARFARE_GAME_USE_SERVICE_REQ : ISerializable
{
	public int warfareGameUnitUID;

	public NKM_WARFARE_SERVICE_TYPE serviceType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref warfareGameUnitUID);
		stream.PutOrGetEnum(ref serviceType);
	}
}
