using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_SET_DUNGEON_SUPPORT_UNIT_ACK)]
public sealed class NKMPacket_SET_DUNGEON_SUPPORT_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMDungeonSupportData selectUnitData = new NKMDungeonSupportData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref selectUnitData);
	}
}
