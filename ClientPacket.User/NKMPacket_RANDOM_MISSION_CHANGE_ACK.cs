using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_RANDOM_MISSION_CHANGE_ACK)]
public sealed class NKMPacket_RANDOM_MISSION_CHANGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int beforeGroupId;

	public NKMMissionData afterMissionData;

	public int remainRefreshCount;

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref beforeGroupId);
		stream.PutOrGet(ref afterMissionData);
		stream.PutOrGet(ref remainRefreshCount);
		stream.PutOrGet(ref costItemData);
	}
}
