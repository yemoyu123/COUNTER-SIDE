using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_WEEKLY_REFRESH_NOT)]
public sealed class NKMPacket_WEEKLY_REFRESH_NOT : ISerializable
{
	public List<NKMItemMiscData> refreshItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref refreshItemDataList);
	}
}
