using Cs.Protocol;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_PROFILE_REQ)]
public sealed class NKMPacket_DEFENCE_PROFILE_REQ : ISerializable
{
	public long userUid;

	public bool isForce;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref isForce);
	}
}
