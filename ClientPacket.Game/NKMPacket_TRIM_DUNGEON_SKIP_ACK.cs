using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TRIM_DUNGEON_SKIP_ACK)]
public sealed class NKMPacket_TRIM_DUNGEON_SKIP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMTrimClearData trimClearData = new NKMTrimClearData();

	public List<NKMRewardData> rewardDatas = new List<NKMRewardData>();

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	public List<UnitLoyaltyUpdateData> updatedUnits = new List<UnitLoyaltyUpdateData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref trimClearData);
		stream.PutOrGet(ref rewardDatas);
		stream.PutOrGet(ref costItems);
		stream.PutOrGet(ref updatedUnits);
	}
}
