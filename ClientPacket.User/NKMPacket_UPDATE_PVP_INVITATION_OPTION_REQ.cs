using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_UPDATE_PVP_INVITATION_OPTION_REQ)]
public sealed class NKMPacket_UPDATE_PVP_INVITATION_OPTION_REQ : ISerializable
{
	public PrivatePvpInvitation value;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref value);
	}
}
