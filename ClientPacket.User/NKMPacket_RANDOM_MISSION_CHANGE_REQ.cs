using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_RANDOM_MISSION_CHANGE_REQ)]
public sealed class NKMPacket_RANDOM_MISSION_CHANGE_REQ : ISerializable
{
	public int tabId;

	public int missionId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tabId);
		stream.PutOrGet(ref missionId);
	}
}
