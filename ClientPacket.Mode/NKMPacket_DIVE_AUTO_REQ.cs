using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_AUTO_REQ)]
public sealed class NKMPacket_DIVE_AUTO_REQ : ISerializable
{
	public bool isAuto;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isAuto);
	}
}
