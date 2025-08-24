using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_BACKGROUND_CHANGE_ACK)]
public sealed class NKMPacket_BACKGROUND_CHANGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMBackgroundInfo backgroundInfo = new NKMBackgroundInfo();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref backgroundInfo);
	}
}
