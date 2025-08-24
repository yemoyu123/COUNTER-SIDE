using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_MISSION_COMPLETE_REQ)]
public sealed class NKMPacket_MISSION_COMPLETE_REQ : ISerializable
{
	public int tabId;

	public int groupId;

	public int missionID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tabId);
		stream.PutOrGet(ref groupId);
		stream.PutOrGet(ref missionID);
	}
}
