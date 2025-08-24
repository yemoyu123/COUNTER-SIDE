using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_UPDATE_PVP_INVITATION_OPTION_ACK)]
public sealed class NKMPacket_UPDATE_PVP_INVITATION_OPTION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public PrivatePvpInvitation value;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref value);
	}
}
