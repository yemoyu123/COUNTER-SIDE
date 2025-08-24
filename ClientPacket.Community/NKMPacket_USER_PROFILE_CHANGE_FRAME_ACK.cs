using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_USER_PROFILE_CHANGE_FRAME_ACK)]
public sealed class NKMPacket_USER_PROFILE_CHANGE_FRAME_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int selfiFrameId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref selfiFrameId);
	}
}
