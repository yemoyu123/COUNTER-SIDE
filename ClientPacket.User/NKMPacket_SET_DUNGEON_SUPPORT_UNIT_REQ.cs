using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_SET_DUNGEON_SUPPORT_UNIT_REQ)]
public sealed class NKMPacket_SET_DUNGEON_SUPPORT_UNIT_REQ : ISerializable
{
	public NKMDungeonSupportData selectUnitData = new NKMDungeonSupportData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selectUnitData);
	}
}
