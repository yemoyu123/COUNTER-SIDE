using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISSION_GIVE_ITEM_REQ)]
public sealed class NKMPacket_MISSION_GIVE_ITEM_REQ : ISerializable
{
	public int missionId;

	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref missionId);
		stream.PutOrGet(ref count);
	}
}
