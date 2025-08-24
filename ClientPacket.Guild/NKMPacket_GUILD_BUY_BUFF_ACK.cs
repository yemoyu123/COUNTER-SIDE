using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_BUY_BUFF_ACK)]
public sealed class NKMPacket_GUILD_BUY_BUFF_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	public int welfareId;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	public NKMRewardData rewardData;

	public long unionPoint;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref welfareId);
		stream.PutOrGet(ref costItemDataList);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref unionPoint);
	}
}
