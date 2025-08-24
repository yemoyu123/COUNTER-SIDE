using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_AUTO_ACK)]
public sealed class NKMPacket_DIVE_AUTO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isAuto;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isAuto);
	}
}
