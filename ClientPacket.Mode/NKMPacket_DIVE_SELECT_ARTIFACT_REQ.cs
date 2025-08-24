using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_SELECT_ARTIFACT_REQ)]
public sealed class NKMPacket_DIVE_SELECT_ARTIFACT_REQ : ISerializable
{
	public int artifactID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref artifactID);
	}
}
