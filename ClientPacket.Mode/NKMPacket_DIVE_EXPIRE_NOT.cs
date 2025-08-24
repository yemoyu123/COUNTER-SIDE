using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_EXPIRE_NOT)]
public sealed class NKMPacket_DIVE_EXPIRE_NOT : ISerializable
{
	public int stageID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageID);
	}
}
