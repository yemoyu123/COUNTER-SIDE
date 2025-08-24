using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_DRAFT_PVP_PICK_LEADER_ACK)]
public sealed class NKMPacket_DRAFT_PVP_PICK_LEADER_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
	}
}
