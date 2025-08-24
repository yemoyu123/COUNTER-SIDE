using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_BUY_WELFARE_POINT_ACK)]
public sealed class NKMPacket_GUILD_BUY_WELFARE_POINT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref costItemDataList);
		stream.PutOrGet(ref rewardData);
	}
}
