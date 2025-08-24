using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_UNIT_SKILL_UPGRADE_REQ)]
public sealed class NKMPacket_UNIT_SKILL_UPGRADE_REQ : ISerializable
{
	public long unitUID;

	public int skillID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref skillID);
	}
}
