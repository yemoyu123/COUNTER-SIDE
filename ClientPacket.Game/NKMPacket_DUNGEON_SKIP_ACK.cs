using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_DUNGEON_SKIP_ACK)]
public sealed class NKMPacket_DUNGEON_SKIP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMStagePlayData stagePlayData = new NKMStagePlayData();

	public List<NKMDungeonRewardSet> rewardDatas = new List<NKMDungeonRewardSet>();

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	public List<UnitLoyaltyUpdateData> updatedUnits = new List<UnitLoyaltyUpdateData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref stagePlayData);
		stream.PutOrGet(ref rewardDatas);
		stream.PutOrGet(ref costItems);
		stream.PutOrGet(ref updatedUnits);
	}
}
