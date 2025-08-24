using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_RESULT_ACCEPT_ACK)]
public sealed class NKMPacket_RAID_RESULT_ACCEPT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long raidUID;

	public NKMRewardData rewardData;

	public int rewardRaidPoint;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref raidUID);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref rewardRaidPoint);
	}
}
