using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_SESSION_REWARD_ACK)]
public sealed class NKMPacket_GUILD_DUNGEON_SESSION_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int stageIndex;

	public long remainHp;

	public int clearPoint;

	public List<NKMItemMiscData> rewardList = new List<NKMItemMiscData>();

	public List<NKMItemMiscData> artifactReward = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref stageIndex);
		stream.PutOrGet(ref remainHp);
		stream.PutOrGet(ref clearPoint);
		stream.PutOrGet(ref rewardList);
		stream.PutOrGet(ref artifactReward);
	}
}
