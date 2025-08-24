using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CREATE_ACK)]
public sealed class NKMPacket_GUILD_CREATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	public NKMGuildData guildData = new NKMGuildData();

	public PrivateGuildData privateGuildData = new PrivateGuildData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref costItemDataList);
		stream.PutOrGet(ref guildData);
		stream.PutOrGet(ref privateGuildData);
	}
}
