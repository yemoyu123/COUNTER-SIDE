using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_BACKGROUND_CHANGE_REQ)]
public sealed class NKMPacket_BACKGROUND_CHANGE_REQ : ISerializable
{
	public NKMBackgroundInfo backgroundInfo = new NKMBackgroundInfo();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref backgroundInfo);
	}
}
