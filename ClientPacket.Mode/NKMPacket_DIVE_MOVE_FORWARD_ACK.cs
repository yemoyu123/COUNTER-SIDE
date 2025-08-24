using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_MOVE_FORWARD_ACK)]
public sealed class NKMPacket_DIVE_MOVE_FORWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMDiveSyncData diveSyncData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref diveSyncData);
	}
}
