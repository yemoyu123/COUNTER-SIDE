using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_USER_PROFILE_CHANGE_FRAME_REQ)]
public sealed class NKMPacket_USER_PROFILE_CHANGE_FRAME_REQ : ISerializable
{
	public int selfiFrameId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selfiFrameId);
	}
}
